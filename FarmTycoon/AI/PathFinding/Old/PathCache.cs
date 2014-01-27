using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// Cache for paths
    /// </summary>
    public class PathCache
    {
        /// <summary>
        /// Info held by the cache
        /// </summary>
        private class PathCacheInfoObject
        {
            public List<Location> Path;
            public int WeightedLength;
        }
        
        /// <summary>
        /// The cache keyed first on start land, then on end land
        /// </summary>
        private Dictionary<Location, Dictionary<Location, PathCacheInfoObject>> _cache = new Dictionary<Location, Dictionary<Location, PathCacheInfoObject>>();
        
        
        /// <summary>
        /// Check the cache for the path
        /// </summary>
        public bool Check(Location startLocation, Location endLocation, out List<Location> path, out int expectedLength)
        {
            path = null;
            expectedLength = int.MaxValue;

            //if not in chace return false
            if (_cache.ContainsKey(startLocation) == false)
            {
                return false;
            }
            if (_cache[startLocation].ContainsKey(endLocation) == false)
            {
                return false;
            }


            expectedLength = _cache[startLocation][endLocation].WeightedLength;
            path = _cache[startLocation][endLocation].Path;
            return true;
        }

        /// <summary>
        /// Clear the entire cache
        /// </summary>
        public void Clear()
        {
            _cache.Clear();
        }



        /// <summary>
        /// Add a path to the cahce, and all the sub paths that end with the same location, because those are likely to be needed soon
        /// </summary>
        public void Add(List<Location> path, int weightedLength)
        {
            //ignore if an unknown path tries to get added
            if (path == null) { return; }

            //we will also add all sub paths to the cache.  this keeps track of expected lenght of subpaths
            int adjustedWeightedLength = weightedLength;

            //create every possible subpath to add to the cache
            for (int subPathStartIndex = 0; subPathStartIndex < path.Count; subPathStartIndex++)
            {
                //create a sub path from the subpath start to the end of the path
                List<Location> subPath = new List<Location>();
                for (int subPathIndex = subPathStartIndex; subPathIndex < path.Count; subPathIndex++)
                {
                    subPath.Add(path[subPathIndex]);
                }

                Location startLocation = subPath[0];
                Location endLocation = subPath[subPath.Count - 1];

                //set up cache to hold path, and time
                if (_cache.ContainsKey(startLocation) == false)
                {
                    _cache.Add(startLocation, new Dictionary<Location, PathCacheInfoObject>());
                }
                if (_cache[startLocation].ContainsKey(endLocation) == false)
                {
                    _cache[startLocation].Add(endLocation, new PathCacheInfoObject());
                }

                //add path and time to cahce
                _cache[startLocation][endLocation].Path = subPath;
                _cache[startLocation][endLocation].WeightedLength = adjustedWeightedLength;
                

                //determine the second land tile if there is one
                Location secondLocation = null;
                if (subPath.Count > 1)
                {
                    secondLocation = subPath[1];
                }
                
                //remove lenght for the tile just removed
                if (startLocation.Contains<Road>() && secondLocation != null && secondLocation.Contains<Road>())
                {
                    adjustedWeightedLength -= 16;
                }
                else
                {
                    adjustedWeightedLength -= 32;
                }
            }
        }


    }
}
