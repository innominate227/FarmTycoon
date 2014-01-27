using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// A path can be traveled on by an AI to get from a start location to a finsh location.  
    /// Paths are created and kept either because an AI is currently traveling on them, or they are being cached.
    /// A path knows when it has become invalid by attaching itself to the Level2Edges which it uses. 
    /// When path becomes invalid it removes itself form the cache.
    /// </summary>
    public class AiPath
    {
        /// <summary>
        /// The last level2 node we visited.  We are currently traveling on the level 1 path between this level 2 node, and the next level 2 node
        /// </summary>
        private Level2PathNode _level2NodeOn;

        /// <summary>
        /// The next location node we will visit.
        /// </summary>
        private LocationPathNode _nextLocationNode;

        /// <summary>
        /// The location node that we are currently at on the path
        /// </summary>
        private Location _currentLocation;

        /// <summary>
        /// The object walking this path.  Or null if the path is cached but not being walked.
        /// </summary>
        private object _traveller;

        /// <summary>
        /// Index of this path in the path cache. Or -1 if this is path being walked and not just cached.        
        /// </summary>
        private int _pathCacheIndex;

        /// <summary>
        /// Reference to the PathCache the Path is in
        /// </summary>
        private PathCache _cache;

        /// <summary>
        /// Location the path started from
        /// </summary>
        private Location _start;
        
        /// <summary>
        /// Location the path will end at
        /// </summary>
        private Location _end;

        /// <summary>
        /// The total cost of the path
        /// </summary>
        private int _cost;


        /// <summary>
        /// Create a new Path that will be kept in the path cache
        /// </summary>
        public AiPath(Location start, Location end, int cost, PathCache cache, Level2PathNode level2NodeHead, int pathCacheIndex) :
            this(start, end, cost, cache, level2NodeHead, null, pathCacheIndex)
        {
        }

        /// <summary>
        /// Create a new Path that an AI is currently traversing
        /// </summary>
        public AiPath(Location start, Location end, int cost, PathCache cache, Level2PathNode level2NodeHead, object traveller) :
            this(start, end, cost, cache, level2NodeHead, traveller, -1)
        {
        }
                
        /// <summary>
        /// Create a new Path
        /// </summary>
        private AiPath(Location start, Location end, int cost, PathCache cache, Level2PathNode level2NodeHead, object traveller, int pathCacheIndex)
        {
            _start = start;
            _end = end;
            _cost = cost;
            _cache = cache;
            _level2NodeOn = level2NodeHead;
            _traveller = traveller;
            _pathCacheIndex = pathCacheIndex;

            //the prev location will be the start before we have gone anywhere
            _currentLocation = start;

            //make dependent on the cluster we will go through
            AddClusterDependence();

            //determine the first location we need to go to
            DetermineNextLocation();
        }


        /// <summary>
        /// The level 2 node the path is currently on.  (If the path has not been walked this will be the head of the path)
        /// </summary>
        public Level2PathNode Level2Node
        {
            get { return _level2NodeOn; }
        }
                
        /// <summary>
        /// Location the path started from
        /// </summary>
        public Location Start
        {
            get { return _start; }
        }

        /// <summary>
        /// Location the path will end at
        /// </summary>
        public Location End
        {
            get { return _end; }
        }

        /// <summary>
        /// The total cost of the path
        /// </summary>
        public int Cost
        {
            get { return _cost; }
        }

        /// <summary>
        /// The object walking this path.  Or null if the path is cached but not being walked.
        /// </summary>
        public object Traveller
        {
            get { return _traveller; }
        }
        
        /// <summary>
        /// Index of this path in the path cache. Or -1 if this is path being walked and not just cached.        
        /// </summary>
        public int PathCacheIndex
        {
            get { return _pathCacheIndex; }
        }


        /// <summary>
        /// The location that we are currently at on the path
        /// </summary>
        public Location CurrentLocation
        {
            get { return _currentLocation; }
        }
        


        /// <summary>
        /// Go to the next location on the path.
        /// </summary>
        public Location GoToNext()
        {
            _currentLocation = _nextLocationNode.Location;
            DetermineNextLocation();
            return _currentLocation;
        }

        /// <summary>
        /// Determine if the path is complete
        /// </summary>
        public bool PathComplete()
        {
            return (_nextLocationNode == null);
        }

        

        /// <summary>
        /// Determine the next location path node we will visit
        /// </summary>
        private void DetermineNextLocation()
        {
            if (_nextLocationNode == null)
            {
                //we need to determine our first location
                _nextLocationNode = _level2NodeOn.Level1Path;
            }
            else
            {
                //move to the next location node
                _nextLocationNode = _nextLocationNode.Next;
            }

            //if the next location node is null (as so long as it is null)
            while (_nextLocationNode == null)
            {
                //we are no longer dependent on the clusters we just finished traveling through
                foreach (Cluster cluster in _level2NodeOn.Clusters)
                {
                    cluster.BreakPathDependent(this);
                }
                
                //go to the next level 2 node
                _level2NodeOn = _level2NodeOn.Next;

                //if there is a next level2 node to move to do that (otherwise we have reached the end of the path)
                if (_level2NodeOn == null)
                {
                    break;
                }

                //start at the head of the level 1 path for that node (if the level2 nodes have the same location this will be null, and we will go to the next level2 node)
                _nextLocationNode = _level2NodeOn.Level1Path;
            }
        }


        /// <summary>
        /// Make the path dependent on the clusters it will pass through        
        /// </summary>
        private void AddClusterDependence()
        {
            Level2PathNode level2Travel = _level2NodeOn;
            while (level2Travel != null)
            {
                //make dependent on all cluster it travels through
                foreach (Cluster cluster in level2Travel.Clusters)
                {
                    cluster.MakePathDependent(this);
                }

                //look at next level 2 path node
                level2Travel = level2Travel.Next;
            }
        }

        /// <summary>
        /// Make the path no longer dependent on the clusters it has yet to pass through           
        /// </summary>
        private void RemoveClusterDependence()
        {
            Level2PathNode level2Travel = _level2NodeOn;
            while (level2Travel != null)
            {
                //make not dependent on all cluster it travels through
                foreach (Cluster cluster in level2Travel.Clusters)
                {
                    cluster.BreakPathDependent(this);
                }

                //look at next level 2 path node
                level2Travel = level2Travel.Next;
            }
        }

        
        /// <summary>
        /// Delete the path, removes any reference from to the path from clusters.
        /// And removes the path from the cache.
        /// </summary>
        public void Delete()
        {
            RemoveClusterDependence();
            _cache.RemovePath(this);
        }

    }
}
