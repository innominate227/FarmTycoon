using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// The is the main interface class to the AI path finder.
    /// It finds a path between any two locations in the game world using a heirical A* approch.
    /// It uses the approch described in "Near Optimal Hierarchical Path-Finding" (Adi Botea, Martin Muller, Jonathan Schaeffer)
    /// </summary>
    public class FastPathFinder
    {
        /// <summary>
        /// Cache of path being travelled, and recently walked paths
        /// </summary>
        private PathCache _cache = new PathCache();

        /// <summary>
        /// Clusters of 10x10 locations used for level 2 path finding
        /// </summary>
        private Cluster[][] _clusters;
                
        /// <summary>
        /// These cluster will be invalidated once EndBatchInvalidate
        /// </summary>
        private List<Cluster> _clustersToInvalidate = new List<Cluster>();

        /// <summary>
        /// If greater than 0 we are doing a batch (we use int so that if two people start a batch it will not finish until both end a batch)
        /// </summary>
        private int _doingBatchInvalidate = 0;


        /// <summary>
        /// Setup the FastPathFinder.
        /// This should be done after the LocationsManager has been setup/loaded
        /// </summary>
        public void Setup()
        {
            //determine how many clusters we need to make
            int clustersWide = GameState.Current.Locations.Size / Cluster.CLUSTER_SIZE;
            if (GameState.Current.Locations.Size % Cluster.CLUSTER_SIZE != 0)
            {
                clustersWide++;
            }

            //create clusters
            _clusters = new Cluster[clustersWide][];
            for (int x = 0; x < clustersWide; x++)            
            {
                _clusters[x] = new Cluster[clustersWide];
                for (int y = 0; y < clustersWide; y++)
                {
                    Cluster cluster = new Cluster();
                    _clusters[x][y] = cluster;
                }
            }

            //setup the clusters
            for (int x = 0; x < clustersWide; x++)
            {                
                for (int y = 0; y < clustersWide; y++)
                {
                    Cluster cluster = _clusters[x][y];

                    //get adjacent clusters (or null if no adjacent in that direction)
                    Cluster northEast = null;
                    if (y > 0) { northEast = _clusters[x][y - 1]; }
                    Cluster southEast = null;
                    if (x < clustersWide - 1) { southEast = _clusters[x + 1][y]; }                    
                    Cluster southWest = null;
                    if (y < clustersWide - 1) { southWest = _clusters[x][y + 1]; }
                    Cluster northWest = null;
                    if (x > 0) { northWest = _clusters[x - 1][y]; }

                    //setup the cluster
                    cluster.Setup(x, y, northEast, southEast, southWest, northWest);
                }
            }

            //create the doors for each cluster
            for (int x = 0; x < clustersWide; x++)
            {
                for (int y = 0; y < clustersWide; y++)
                {
                    //we just need to do these two direction because (SW will create the adjacent clusters NE, and SE the NW)
                    Cluster cluster = _clusters[x][y];
                    cluster.CreateDoorNodes(OrdinalDirection.SouthEast);
                    cluster.CreateDoorNodes(OrdinalDirection.SouthWest);
                }
            }

        }


        /// <summary>
        /// Find the cost to travel from start to end
        /// </summary>
        public int FindPathCost(Location start, Location end)
        {
            //if start and end are the same cost is 0
            if (start == end)
            {
                return 0;
            }

            //check if the cache knows that the next location to visit should be
            int cost = _cache.CheckCacheForCost(start, end);
            if (cost != -1)
            {
                return cost;
            }
            
            //find a new path, and add it to the cache
            bool pathFound = FindNewPath(start, end);
            if (pathFound)
            {
                //the next location will be in the cache now because we just added it
                cost = _cache.CheckCacheForCost(start, end);
                return cost;
            }
            else
            {
                //no path was found
                return int.MaxValue;
            }   
        }
        

        /// <summary>
        /// Find the next location to visit on the path from start to end
        /// If there is no path it will return null.
        /// If start==end it will also return null
        /// </summary>
        public Location FindPath(Location start, Location end, object traveller)
        {
            //we can not find a path from the same location to the same location
            Debug.Assert(start != end);

            //check if the cache knows that the next location to visit should be
            //the "true" at the end tell it to randomly throw away the path even 
            //if it exsits this allows the worker to try and find a new faster path if on exsists now.
            Location cacheLocation = _cache.CheckCacheForNextLocation(start, end, traveller, true);

            //if we fond a location in the cache then return the next location
            if (cacheLocation != null)
            {
                DebugToolWindow.CacheHits += 1;
                return cacheLocation;
            }

            //check if its a simple adajcent location (end is adjacent to start)
            foreach (OrdinalDirection dir in DirectionUtils.AllOrdinalDirections)
            {
                if (start.GetAdjacent(dir) == end)
                {
                    return end;
                }
            }

            DebugToolWindow.CacheMiss += 1;

            //find a new path, and add it to the cache
            bool pathFound = FindNewPath(start, end);
            if (pathFound)
            {                
                //the next location will be in the cache now because we just added it (disallow the random chance of throwing away the path)
                cacheLocation = _cache.CheckCacheForNextLocation(start, end, traveller, false);
                return cacheLocation;
            }
            else
            {
                //no path was found
                return null;
            }            
        }


        /// <summary>
        /// Find a path from start to end, and add the path to the cache.
        /// If no path could be found return false.
        /// </summary>
        private bool FindNewPath(Location start, Location end)
        {
            //this will be the cost of the path we find
            int cost;

            //this will be the head of the level 2 path
            Level2PathNode level2PathStart = null;

            //if both are with in 2 * CLUSTER_SIZE of each other then just find the direct path
            if (LocationUtils.DistanceApart(start, end) < 2 * Cluster.CLUSTER_SIZE)
            {
                //get the path to the end
                LocationPathNode level1PathStart = PathFinder.FindDirectPath(start, end, null, out cost);

                //no level 1 path means there is no possible path
                if (cost == int.MaxValue)
                {
                    return false;
                }

                //create a Level2PathNode that is really just the level1 path
                level2PathStart = new Level2PathNode();
                level2PathStart.Level1Path = level1PathStart;
                level2PathStart.Next = null;

                //make the path be in all the clusters the level1 path goes through
                LocationPathNode level1PathWalk = level1PathStart;
                while (level1PathWalk != null)
                {
                    Cluster clusterIn = GetClusterForLocation(level1PathWalk.Location);
                    if (level2PathStart.Clusters.Contains(clusterIn))
                    {
                        level2PathStart.Clusters.Add(clusterIn);
                    }
                    level1PathWalk = level1PathWalk.Next;
                }
            }
            else
            {
                //add Level2 node to the start cluster
                Cluster startCluster = GetClusterForLocation(start);
                Level2Node startLevel2Node = new Level2Node(startCluster, start);
                startCluster.AddTempNode(startLevel2Node);

                //add Level2 node to the end cluster   
                Cluster endCluster = GetClusterForLocation(end);
                Level2Node endLevel2Node = new Level2Node(endCluster, end);
                endCluster.AddTempNode(endLevel2Node);

                //find fastest Level2 Path
                level2PathStart = PathFinder.FindLevel2Path(startLevel2Node, endLevel2Node, out cost);

                //delete the temporary nodes we added to find the path
                startLevel2Node.Delete();
                endLevel2Node.Delete();

                //no level 2 path means there is no possible path
                if (cost == int.MaxValue)
                {
                    return false;
                }
            }

            //add the path we found to the cache
            _cache.AddToCahce(start, end, cost, level2PathStart);

            //we found a path
            return true;
        }


        /// <summary>
        /// Get the cluster that a loction is inside
        /// </summary>
        private Cluster GetClusterForLocation(Location location)
        {
            int clusterX = location.X / Cluster.CLUSTER_SIZE;
            int clusterY = location.Y / Cluster.CLUSTER_SIZE;
            return _clusters[clusterX][clusterY];
        }

        /// <summary>
        /// If a traveller is deleted this should be called so that any cached paths for the traveller can be dropped
        /// </summary>
        public void RemoveTraveller(object traveller)
        {
            _cache.RemovePathForTraveller(traveller);
        }

        public void Debug_MarkDoors()
        {
            for (int i = 0; i < _clusters.Length; i++)
            {
                foreach (Cluster cluster in _clusters[i])
                {
                    cluster.DEBUG_MarkDoors();
                }
            }
        }


        #region Location Invalidation

        /// <summary>
        /// Start a batch location invalidation
        /// locations marked invalidated will not be invalidated until ending the batch
        /// </summary>
        public void StartBatchInvalidate()
        {
            _doingBatchInvalidate++;
        }

        /// <summary>
        /// Invalidate and cached paths for the locations passed        
        /// </summary>
        public void InvalidateLocations(List<Location> locations)
        {
            //find the clusters that the locations passed are in            
            foreach (Location location in locations)
            {
                Cluster cluster = GetClusterForLocation(location);
                if (_clustersToInvalidate.Contains(cluster) == false)
                {
                    _clustersToInvalidate.Add(cluster);
                }
            }

            //if not in a batch update now
            if (_doingBatchInvalidate == 0)
            {
                RecreateClustersToInvalidate();
            }
        }

        /// <summary>
        /// End a batch location invalidation
        /// locations marked invalidated will now be invalidated
        /// </summary>
        public void EndBatchInvalidate()
        {
            _doingBatchInvalidate--;

            //if no longer within batch recreated invaludated clusters
            if (_doingBatchInvalidate == 0)
            {
                RecreateClustersToInvalidate();
            }
        }

        /// <summary>
        /// Recreate all clusters in the clusters to invalidate list
        /// </summary>
        private void RecreateClustersToInvalidate()
        {
            foreach (Cluster cluster in _clustersToInvalidate)
            {
                cluster.ReCreateCluster();
            }
            _clustersToInvalidate.Clear();
        }

        #endregion

    }
}
