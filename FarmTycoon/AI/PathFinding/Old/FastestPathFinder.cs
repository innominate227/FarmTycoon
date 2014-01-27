using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;


namespace FarmTycoon
{
    /// <summary>
    /// Finds the best path between two peices of land using an A* algorithm
    /// </summary>
    public class FastestPathFinderOld
    {

        private bool PATH_DEBUG = true;


        /// <summary>
        /// cache of full paths
        /// </summary>
        private PathCache _pathCache = new PathCache();

        /// <summary>
        /// cache of adjacnet land that can be walked to from a certain peice of land
        /// </summary>
        private CanWalkToCache _walkToCache = new CanWalkToCache();


        public FastestPathFinderOld()
        {
        }


        /// <summary>
        /// Clear the fastest path cache (because something has changed what the fastest paths may be)
        /// </summary>
        public void ClearPathCache()
        {
            _pathCache.Clear();
        }

        /// <summary>
        /// Clear the walk cache for the location passed, and adjacent location
        /// </summary>
        public void ClearWalkCacheFor(IList<Location> locationList)
        {
            foreach (Location location in locationList)
            {
                ClearWalkCacheFor(location);
            }
        }

        /// <summary>
        /// Clear the walk cache for the land, and adjacent land
        /// </summary>
        public void ClearWalkCacheFor(Location location)
        {
            _walkToCache.ClearForLandAndAdjacnet(location);
        }

        /// <summary>
        /// Clear the walk cache for all land
        /// </summary>
        public void ClearWalkCacheForAll()
        {
            _walkToCache.Clear();
        }

        /// <summary>
        /// Find the fastest path between two locations, and return expected time.
        /// </summary>
        public List<Location> FindPath(Location startLocation, Location endLocation)
        {
            int notUsed;
            return FindPath(startLocation, endLocation, out notUsed);
        }

        /// <summary>
        /// Find the fastest path between two locations, and return expected time.
        /// </summary>
        public List<Location> FindPath(Location startLocation, Location endLocation, out int weightedLength)
        {
            List<Location> path;
            if (_pathCache.Check(startLocation, endLocation, out path, out weightedLength))
            {
                //we found the path in the cahce return it
                return path;
            }

            if (PATH_DEBUG)
            {
                foreach (Land land in Program.Game.MasterObjectList.FindAll<Land>())
                {
                    land.CornerToHighlight = LandCorner.None;
                }
            }
            
            //calcualte the path
            path = FindBestPath(startLocation, endLocation);
            weightedLength = CalculateWeightedLength(path);

            //add the path to the cahce
            _pathCache.Add(path, weightedLength);

            return path;
        }


        /// <summary>
        /// Find the best path from start location to end location by trying a few different path options and selecting the fastest
        /// </summary>
        private List<Location> FindBestPath(Location startLocation, Location endLocation)
        {
            //it is slow to calculate the real optimal path, so we will instead calculate two paths that are much faster to determine.
            //1.  the fastest path ignoring the fact that the worker can travel faster on roads
            //2.  the fastest to the nearest road from the source + road nearest to the destination + fastest path between those two road (traveling only on roads)


            //we can also increase preformance by finding a path to an enclosure entrance befroe finding the path to the inner-enclosure location 
            //and same for the exit.  Except if the start and end are in the same enclosure.
            Enclosure startEnclosure = startLocation.Find<Enclosure>();
            Enclosure endEnclosure = endLocation.Find<Enclosure>();
            bool sameEnclosure = (startEnclosure != null && startEnclosure == endEnclosure);

            //this will contain the path to the enclosures exit, or
            //this list will be empty if we do not need to first find the path to the exit
            //if we do find the path to the exit we change start location to be the exit location
            List<Location> toStartEnclosureExit = new List<Location>();
            if (startEnclosure != null && sameEnclosure == false)
            {
                //find the path
                toStartEnclosureExit = FindPathSegment(startLocation, startEnclosure.ExitLand.LocationOn, false);
                if (toStartEnclosureExit == null) { return null; }

                //remove from path so its not in the path twice
                toStartEnclosureExit.RemoveAt(toStartEnclosureExit.Count - 1);

                //update start location
                startLocation = startEnclosure.ExitLand.LocationOn;
            }


            //this will contain the path from the end enclosures entrance to the end location OR
            //this list will be empty if we do not need to find this path
            //if we do find this path to the end location will change to the fields entrance
            List<Location> toEndEnclosureDestination = new List<Location>();
            if (endEnclosure != null && sameEnclosure == false)
            {
                //find the path
                toEndEnclosureDestination = FindPathSegment(endEnclosure.EntryLand.LocationOn, endLocation, false);
                if (toEndEnclosureDestination == null) { return null; }

                //remove from start so its no in the path twice
                toEndEnclosureDestination.RemoveAt(0);

                //update end location
                endLocation = endEnclosure.EntryLand.LocationOn;
            }
            

            //determine the path where we ignore roads
            List<Location> ignoreRoadsPath = new List<Location>();
            List<Location> startToEndPath = FindPathSegment(startLocation, endLocation, false);
            if (startToEndPath == null) { return null; }
            ignoreRoadsPath.AddRange(toStartEnclosureExit);
            ignoreRoadsPath.AddRange(startToEndPath);
            ignoreRoadsPath.AddRange(toEndEnclosureDestination);


            
            //path from start to a road, get the road (remove the road from the path so it not in the total path twice)
            List<Location> toRoadPath = FindNearestRoad(startLocation, 1000, false);
            //no such path so take the ignore roads path
            if (toRoadPath == null) { return ignoreRoadsPath; }
            //the road we will start at to (remove if from the path)
            Location startRoad = toRoadPath[toRoadPath.Count - 1];
            toRoadPath.RemoveAt(toRoadPath.Count - 1);

            //path from end to a road
            List<Location> fromRoadPath = FindNearestRoad(endLocation, 1000, true);
            //no such path so take the ignore roads path
            if (fromRoadPath == null) { return ignoreRoadsPath; }
            //the road we will end at (remove it from the path)
            Location endRoad = fromRoadPath[0];
            fromRoadPath.RemoveAt(0);

            //get the on roads path (the path we take on the roads to get to the end)
            List<Location> onRoadsPath = FindPathSegment(startRoad, endRoad, true);
            if (onRoadsPath == null) { return ignoreRoadsPath; }

            //get the full with roads path
            List<Location> useRoadsPath = new List<Location>();
            useRoadsPath.AddRange(toStartEnclosureExit);
            useRoadsPath.AddRange(toRoadPath);
            useRoadsPath.AddRange(onRoadsPath);
            useRoadsPath.AddRange(fromRoadPath);
            useRoadsPath.AddRange(toEndEnclosureDestination);


            //get the lenght for the no roads path
            int noRoadsLenght = CalculateWeightedLength(ignoreRoadsPath);
            int roadsLenght = CalculateWeightedLength(useRoadsPath);

            //return the shorter path
            if (roadsLenght <= noRoadsLenght)
            {
                return useRoadsPath;
            }
            else
            {
                return ignoreRoadsPath;
            }
            
        }


        /// <summary>
        /// Finds the fastest path between two locations (without taking roads into consideration). 
        /// Pass roads only to find a fastest path that only travels on roads.        
        /// This is used to find a possible fastest path. 
        /// </summary>
        private List<Location> FindPathSegment(Location startLocation, Location endLocation, bool roadsOnly)
        {
            //we can find a path between undefined locations
            if (startLocation == null || endLocation == null) 
            {                
                return null; 
            }
            
            //locations that need to be visited
            PathFinderQueue openList = new PathFinderQueue();

            //locations we came from to get here, each location is mapped to the location we came from to get there
            Dictionary<Location, Location> previous = new Dictionary<Location, Location>();

            //land that no longer needs to be visited
            HashSet<Location> closedList = new HashSet<Location>();

            //add the start location to the open List
            openList.Enqueue(startLocation, 0, CalculateHeuristic(startLocation, endLocation));

            //search unil we found the end
            while (true)
            {
                //get the next location in the open list, and the distance to get to that location
                int distToSquaeOn;
                Location locationOn = openList.Dequeue(out distToSquaeOn);
                
                //if theres no more locations then there is no path
                if (locationOn == null)
                {
                    return null;
                }
                
                if (PATH_DEBUG)
                {
                    Land land = locationOn.Find<Land>();
                    if (land != null)
                    {
                        land.CornerToHighlight = LandCorner.Center;
                    }
                    System.Threading.Thread.Sleep(10);
                }
                
                //if we found the end then stop searching
                if (locationOn == endLocation)
                {
                    break;
                }

                //add the square were on to the closed list
                closedList.Add(locationOn);

                //add all the adjacent locations to the open list (if there are not alrady in the closed list)
                foreach (WalkToItem walkableLocation in _walkToCache.GetWalkToList(locationOn))
                {
                    //if we are doing roads only, ignore if the location is not a road
                    if (roadsOnly && walkableLocation.Road == false) { continue; }

                    //dont add locations that are in the closed list
                    if (closedList.Contains(walkableLocation.Location)) { continue; }

                    //if the adjacent land is inside an enclosure that we are not inside, and the final destination is not within that enclosure dont search that way
                    if (locationOn.Find<Enclosure>() != walkableLocation.Location.Find<Enclosure>() && endLocation.Find<Enclosure>() != walkableLocation.Location.Find<Enclosure>())
                    {
                        continue;
                    }


                    //item will be updated if its already in the queue (as long as it has a smaller cost)
                    bool addOrMoved = openList.Enqueue(walkableLocation.Location, distToSquaeOn + walkableLocation.Cost, CalculateHeuristic(walkableLocation.Location, endLocation));

                    //if the item was added or update then set the previous
                    if (addOrMoved)
                    {
                        if (previous.ContainsKey(walkableLocation.Location) == false)
                        {
                            previous.Add(walkableLocation.Location, locationOn);
                        }
                        else
                        {
                            previous[walkableLocation.Location] = locationOn;
                        }
                    }
                }
            }


            //create the path from the end Location to the start Location
            List<Location> path = new List<Location>();
            Location traceBack = endLocation;
            while (traceBack != startLocation)
            {
                path.Add(traceBack);
                traceBack = previous[traceBack];
            }
            path.Add(startLocation);

            //return the reverse (from start to end)
            path.Reverse();
            

            return path;
        }

        /// <summary>
        /// Find the nearest road by sprialing our from the location,
        /// retunrs a path to the nearest road, the road location will be the last location in the path
        /// or null if no road was within dist
        /// </summary>
        private List<Location> FindNearestRoad(Location startLocation, int maxDist, bool reversePath)
        {
            //locations that need to be visited
            PathFinderQueue openList = new PathFinderQueue();

            //locations we came from to get here, each location is mapped to the location we came from to get there
            Dictionary<Location, Location> previous = new Dictionary<Location, Location>();

            //land that no longer needs to be visited
            HashSet<Location> closedList = new HashSet<Location>();

            //add the start location to the open List
            openList.Enqueue(startLocation, 0, 0);

            //this is set to the location we find with a road once we find one
            Location endLocation = null;

            //search unil we found the end
            while (true)
            {
                //get the next location in the open list, and the distance to get to that location
                int distToSquaeOn;
                Location locationOn = openList.Dequeue(out distToSquaeOn);

                //if the distance to this square is more than our max then there is no path with in max
                if (distToSquaeOn > maxDist)
                {
                    return null;
                }
                
                //if theres no more locations then there is no path
                if (locationOn == null)
                {
                    return null;
                }

                //if we found a road then stop searching
                if (locationOn.Contains<Road>())
                {
                    endLocation = locationOn;
                    break;
                }

                //add the square were on to the closed list
                closedList.Add(locationOn);

                //add all the adjacent locations to the open list (if there are not alrady in the closed list)
                foreach (WalkToItem walkableLocation in _walkToCache.GetWalkToList(locationOn))
                {
                    //dont add locations that are in the closed list
                    if (closedList.Contains(walkableLocation.Location)) { continue; }
                    
                    //item will be updated if its already in the queue (as long as it has a smaller cost)
                    bool addOrMoved = openList.Enqueue(walkableLocation.Location, distToSquaeOn + walkableLocation.Cost, 0);

                    //if the item was added or update then set the previous
                    if (addOrMoved)
                    {
                        if (previous.ContainsKey(walkableLocation.Location) == false)
                        {
                            previous.Add(walkableLocation.Location, locationOn);
                        }
                        else
                        {
                            previous[walkableLocation.Location] = locationOn;
                        }
                    }
                }
            }


            //create the path from the end Location to the start Location
            List<Location> path = new List<Location>();
            Location traceBack = endLocation;
            while (traceBack != startLocation)
            {
                path.Add(traceBack);
                traceBack = previous[traceBack];
            }
            path.Add(startLocation);

            //return the reverse (from start to end), unless they wanted the reverse in which case leave it as it is
            if (reversePath == false)
            {
                path.Reverse();
            }

            return path;
        }


        /// <summary>
        /// Calculates the expected wieghted length for the path passed.
        /// Where walking between two roads adds 1, and walking between two other tiles adds 2.
        /// </summary>
        private int CalculateWeightedLength(List<Location> path)
        {
            if (path == null || path.Count == 0)
            {
                //if there is no path return very big time
                return int.MaxValue;
            }

                        
            int weightedLength = 0;
            //determine time to walk between each pair of land tiles
            for (int i = 1; i < path.Count; i++)
            {
                Location location1 = path[i - 1];
                Location location2 = path[i];

                if (location1.Contains<Road>() && location2.Contains<Road>())
                {
                    //if both are road use 16 for the number of movements between tiles
                    weightedLength += 16;
                }
                else
                {
                    //if one is not on a road use 32 since we doble to movement delay in that case, and this is used for esitmating movement delay
                    weightedLength += 32;
                }
            }

            //return total leght
            return weightedLength;
        }


        /// <summary>
        /// Calculate the heiristic between the passed square and the end square
        /// </summary>
        private int CalculateHeuristic(Location location, Location endLocation)
        {
            //return (Math.Abs(location.X - endLocation.X) * 500) + (Math.Abs(location.Y - endLocation.Y) * 500);
            return (Math.Abs(location.X - endLocation.X) * 10) + (Math.Abs(location.Y - endLocation.Y) * 10);
        }
        

    }
}
