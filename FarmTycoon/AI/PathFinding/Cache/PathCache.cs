using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Cache of paths currently being travelled on, or cached because they are frequently travled on
    /// </summary>
    public class PathCache
    {
        /// <summary>
        /// There is a random chance that we throw out a path whenever we get it from teh cache, incase there is now a faster path
        /// 1/THROW_OUT_CHANCE is the chance that we throw it out
        /// </summary>
        private const int THROW_OUT_CHANCE = 20;

        /// <summary>
        /// Size of the cahce.  We randomly remove a cahced item when we add one.  So the actually fill may never reach this level.
        /// </summary>
        private const int CACHE_SIZE = 200;

        /// <summary>
        /// List of paths currently being traveled by an object, keyed on the object traveling on them
        /// </summary>
        private Dictionary<object, AiPath> _beingTravelled = new Dictionary<object, AiPath>();

        /// <summary>
        /// Paths in the cache key on start location, end location tuples (this contains the same Paths object as the array below).
        /// </summary>
        private Dictionary<Tuple<Location, Location>, AiPath> _cache = new Dictionary<Tuple<Location, Location>, AiPath>();

        /// <summary>
        /// Paths in the cache by there position in the cache (this contains the same Paths object as the dictionary above)
        /// </summary>
        private AiPath[] _cacheArray = new AiPath[CACHE_SIZE];

        /// <summary>
        /// Used by random cache replacement algorithm
        /// </summary>
        private Random _random = new Random();
        
        /// <summary>
        /// Check if the know the cost of the path from start to end
        /// If so return the cost if not return -1
        /// </summary>
        public int CheckCacheForCost(Location start, Location end)
        {            
            Tuple<Location, Location> key = new Tuple<Location, Location>(start, end);

            //see if we know about the path
            if (_cache.ContainsKey(key) == false)
            {                
                return -1;
            }
            else
            {
                return _cache[key].Cost;
            }
        }
        
        /// <summary>
        /// Check if we know the next location on the path from start to end for the traveller passed.
        /// If so return the next location if not return null.
        /// If you pass allowRandomThrowaway as true there is a random chance that even if a path exsists it will be thrown out 
        /// (this allows workers to eventually find a new faster path).  
        /// You should not pass this as true you are expecting to get a path that you just added to the cache.
        /// </summary>
        public Location CheckCacheForNextLocation(Location start, Location end, object traveller, bool allowRandomThrowAway)
        {
            //see if we know about the traveller in the traveller cache
            Location travellerLocation = CheckTravellerCache(start, end, traveller);
            if (travellerLocation != null)
            {
                return travellerLocation;
            }

            //see if we have a path from start to end in the normal cache.
            //if we do a copy of it will be made for the traveller and added to the traveler cache
            bool foundInNormalCahce = CreateTravellerPathFromNormalCache(start, end, traveller, allowRandomThrowAway);
            if (foundInNormalCahce)
            {
                return CheckTravellerCache(start, end, traveller);
            }

            //no path could be found in the cache
            return null;
        }

        /// <summary>
        /// Check if the know about the path the traveller passed is traveling on.
        /// If so return the next location he should go to.
        /// </summary>
        private Location CheckTravellerCache(Location start, Location end, object traveller)
        {
            //see if we know the path this traveller is traveling on
            if (_beingTravelled.ContainsKey(traveller) == false)
            {
                //we dont
                return null;
            }

            //the next location on the path that we will return (if the traveller is still on the same path)
            Location nextLocation = null;

            //get the path the traveler was following when we last knew him
            AiPath travelersPath = _beingTravelled[traveller];

            //should we delete the path
            bool deletePath = false;

            //if we are still on the same path return the next location to go to
            if (travelersPath.End == end && travelersPath.CurrentLocation == start)
            {
                //go to the next location on the path
                nextLocation = travelersPath.GoToNext();

                Debug.Assert(nextLocation != start);

                //if path is complete we will delete it
                deletePath = travelersPath.PathComplete();
            }
            else
            {
                //our traveller is no longer following that path so delete it
                deletePath = true;
            }

            //if we need to delete the path do that now
            if (deletePath)
            {
                travelersPath.Delete();
            }

            //retunr the next location for the traveller to go to
            return nextLocation;
        }
        
        /// <summary>
        /// Check if we know about the path from start to end in our normal cache.
        /// If so create a copy of that Path for the traveler passed to travel on, and add it to the traveller cache.
        /// If not return false.
        /// If you pass allowRandomThrowaway as true there is a random chance that even if a path exsists it will be thrown out 
        /// (this allows workers to eventually find a new faster path)
        /// </summary>
        private bool CreateTravellerPathFromNormalCache(Location start, Location end, object traveller, bool allowRandomThrowAway)
        {
            Tuple<Location, Location> key = new Tuple<Location, Location>(start, end);

            //see if we know about the path
            if (_cache.ContainsKey(key) == false)
            {
                //we dont
                return false;
            }
            else
            {
                //get the path from the noraml cache
                AiPath normalCachePath = _cache[key];

                //it possible there is now a better path than the one we just got from the cache.  Randomly throw out the path we just got.
                if (allowRandomThrowAway && _random.Next(THROW_OUT_CHANCE) == 0)
                {
                    //throw the cached path away
                    RemovePath(normalCachePath);
                    return false;
                }

                //make a copy of it for the traveler
                AiPath travellerPath = new AiPath(normalCachePath.Start, normalCachePath.End, normalCachePath.Cost, this, normalCachePath.Level2Node, traveller);

                //add it to the traveler cahce
                _beingTravelled.Add(traveller, travellerPath);

                //we found a path
                return true;
            }
        }
        
        /// <summary>
        /// Add a new path to the normal path cache        
        /// </summary>
        public void AddToCahce(Location start, Location end, int cost, Level2PathNode pathHead)
        {
            //determine where to place it in the cache
            int cacheLoc = _random.Next(CACHE_SIZE);

            //delete the old path if there is one there (delete removes the path from the cache)
            if (_cacheArray[cacheLoc] != null)
            {
                _cacheArray[cacheLoc].Delete();
            }

            //make a path for the normal cache
            AiPath normalCachePath = new AiPath(start, end, cost, this, pathHead, cacheLoc);

            //add the path to the cache
            _cacheArray[cacheLoc] = normalCachePath;
            _cache.Add(new Tuple<Location, Location>(start, end), normalCachePath);
        }
        
        /// <summary>
        /// Remove a path from the cache 
        /// (this is called by the path itself when it becomes invalid, or is deleted)
        /// </summary>
        public void RemovePath(AiPath path)
        {
            if (path.PathCacheIndex != -1)
            {
                //it was in our normal cache, so remove it from there
                Tuple<Location, Location> key = new Tuple<Location, Location>(path.Start, path.End);
                _cache.Remove(key);
                _cacheArray[path.PathCacheIndex] = null;
            }
            else
            {
                //it was in our being travelled cache, remove it from there
                _beingTravelled.Remove(path.Traveller);
            }
        }
        
        /// <summary>
        /// If a path exsists in the cache for the taveller pass it will be removed
        /// </summary>
        public void RemovePathForTraveller(object traveller)
        {
            if (_beingTravelled.ContainsKey(traveller))
            {
                _beingTravelled[traveller].Delete();
            }
        }

    }
}
