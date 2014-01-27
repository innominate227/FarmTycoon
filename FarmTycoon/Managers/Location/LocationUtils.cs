using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{

    /// <summary>
    /// Flags that detail exactly what building requirement you have, used when calling IsGoodLocationToBuild
    /// </summary>
    public enum BuildRequirementFlags
    {
        None = 0,
        NoWorkers = 1, //there can also not be workers in the area
        InsidePasture = 2, //location must be inside a pasture
    }


    public static class LocationUtils
    {

        /// <summary>
        /// Get a list of absolute locations given a center location and a list of realative locations
        /// </summary>
        public static List<Location> GetLocationList(Location centerLocation, List<RelativeLocation> relativeLocations)
        {
            List<Location> locations = new List<Location>();

            //for each location string
            foreach (RelativeLocation relativeLocation in relativeLocations)
            {
                //get the location
                locations.Add(relativeLocation.GetRealtiveLocation(centerLocation));                
            }

            return locations;
        }



        /// <summary>
        /// Is the list of locations passed good to buildin.
        /// Good if they are above ground, emptry, and not going to prevent walking on a field/road, in the farm owned land .
        /// Pass false to test that at least part of it is above ground
        /// </summary>
        public static bool IsGoodLocationToBuild(IList<Location> locations, BuildRequirementFlags buildRequirements)
        {
            //make sure there are no solid objects in the location
            foreach (Location location in locations)
            {
                foreach (GameObject gameObj in location.AllObjects)
                {
                    if (gameObj.PathEffect.HasFlag(ObjectsEffectOnPath.Solid))
                    {
                        return false;
                    }
                }
            }

            //make sure there is no highway or roads
            if (LocationUtils.AnyContains<Road>(locations)) { return false; }
            if (LocationUtils.AnyContains<Highway>(locations)) { return false; }

            //make sure not inside field
            if (LocationUtils.AnyContains<Field>(locations)) { return false; }

            //if workers are not ok either, make sure there are no workers
            if (buildRequirements.HasFlag(BuildRequirementFlags.NoWorkers))
            {
                if (LocationUtils.AnyContains<Worker>(locations)) { return false; }
            }

            //if must be inside pasture make sure we are
            if (buildRequirements.HasFlag(BuildRequirementFlags.InsidePasture))
            {
                if (LocationUtils.AllContains<Pasture>(locations) == false) { return false; }
            }
            else
            {
                if (LocationUtils.AnyContains<Pasture>(locations)) { return false; }
            }
            
            //it passed all tests
            return true;
        }


        /// <summary>
        /// Find all objects of a type in a list of locations
        /// </summary>
        public static List<T> FindAll<T>(IList<Location> locations) where T : GameObject
        {
            List<T> toReturn = new List<T>();
            foreach (Location location in locations)
            {
                foreach(T obj in location.FindAll<T>())
                {
                    if (toReturn.Contains(obj) == false)
                    {
                        toReturn.Add(obj);
                    }
                }
            }
            return toReturn;
        }
        

        /// <summary>
        /// Determines if any of the locations in the list contain the object passed
        /// </summary>
        public static bool AnyContains<T>(IList<Location> locations) where T:GameObject
        {
            //check each location for that T
            foreach (Location location in locations)
            {
                if (location.Contains<T>())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if all of the locations in the list contain the object passed
        /// </summary>
        public static bool AllContains<T>(IList<Location> locations) where T : GameObject
        {
            //check each location for that T
            foreach (Location location in locations)
            {
                if (location.Contains<T>() == false)
                {
                    return false;
                }
            }
            return true;
        }
        

        /// <summary>
        /// Get the max, and min X, Y, and Z for the list of locations passed
        /// </summary>        
        public static void GetExtrems(ICollection<Location> locations, out int minX, out int maxX, out int minY, out int maxY, out int minZ, out int maxZ)
        {
            maxX = int.MinValue;
            minX = int.MaxValue;
            maxY = int.MinValue;
            minY = int.MaxValue;
            maxZ = int.MinValue;
            minZ = int.MaxValue;

            foreach (Location locationOn in locations)
            {
                if (locationOn.X > maxX)
                {
                    maxX = locationOn.X;
                }
                if (locationOn.X < minX)
                {
                    minX = locationOn.X;
                }

                if (locationOn.Y > maxY)
                {
                    maxY = locationOn.Y;
                }
                if (locationOn.Y < minY)
                {
                    minY = locationOn.Y;
                }

                if (locationOn.Z > maxZ)
                {
                    maxZ = locationOn.Z;
                }
                if (locationOn.Z < minZ)
                {
                    minZ = locationOn.Z;
                }
            }
        }
        
        /// <summary>
        /// Retrun the values for x,y, and z that are at center of the locations passed
        /// </summary>
        public static void GetCenterPoint(ICollection<Location> locations, out float xCenter, out float yCenter, out float zCenter)
        {
            float xSum = 0;
            float ySum = 0;
            float zSum = 0;
            foreach (Location location in locations)
            {
                xSum += location.X;
                ySum += location.Y;
                zSum += location.Z;
            }
            xCenter = xSum / locations.Count;
            yCenter = ySum / locations.Count;
            zCenter = zSum / locations.Count;

        }

        /// <summary>
        /// How far apart are the two locations
        /// </summary>
        public static int DistanceApart(Location start, Location end)
        {
            return Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y);
        }
        
        
        /// <summary>
        /// What is the direction between the two locations
        /// </summary>
        public static OrdinalDirection DirectionBetween(Location start, Location end)
        {            
            int xDiff = start.X - end.X;
            int yDiff = start.Y - end.Y;
            if (Math.Abs(xDiff) > Math.Abs(yDiff))
            {
                if (xDiff > 0) { return OrdinalDirection.NorthWest; }
                else { return OrdinalDirection.SouthEast; }
            }
            else
            {
                if (yDiff > 0) { return OrdinalDirection.NorthEast; }
                else { return OrdinalDirection.SouthWest; }
            }
        }


        /// <summary>
        /// Does traveling from the start location in the direction passed cause you to go around the edge of the world
        /// </summary>
        public static bool WrapsAroundWorldEdge(Location start, OrdinalDirection dir)
        {
            Location end = start.GetAdjacent(dir);
            if (Math.Abs(start.X - end.X) > 1 || Math.Abs(start.Y - end.Y) > 1)
            {
                return true;
            }
            return false;
        }

          
        /// <summary>
        /// Collect several peices of land surrounding a center peice of land.
        /// </summary>
        public static List<Location> CollectLocations(Location centerLocation, CardinalDirection preferedDirection, int size)
        {
            Dictionary<Location, int> examined = new Dictionary<Location,int>();
            List<Location> toRet = new List<Location>();
            CollectLocations(centerLocation, preferedDirection, size, examined, toRet);
            return toRet;
        }

        
        /// <summary>
        /// Collect several peices of land surrounding a center peice of land.
        /// </summary>
        private static void CollectLocations(Location centerLocation, CardinalDirection preferedDirection, int size, Dictionary<Location, int> examined, List<Location> collectedLand)
        {
            //dont try and examine the same peice of land twice
            if (examined.ContainsKey(centerLocation) && examined[centerLocation] >= size)
            {
                return;
            }

            //rember that we examined this tile and remeber how far
            if (examined.ContainsKey(centerLocation))
            {
                examined[centerLocation] = size;
            }
            else
            {
                examined.Add(centerLocation, size);
            }

            //collect this tile
            if (collectedLand.Contains(centerLocation) == false)
            {
                collectedLand.Add(centerLocation);
            }

            if (size == 1)
            {
                //if size if one there is nothing else to collect
                return;
            }
            else if (size == 2)
            {
                OrdinalDirection dir1 = DirectionUtils.CounterClockwiseOneOrdinal(preferedDirection);
                OrdinalDirection dir2 = DirectionUtils.ClockwiseOneOrdinal(preferedDirection);

                if (WrapsAroundWorldEdge(centerLocation, dir1) == false)
                {
                    CollectLocations(centerLocation.GetAdjacent(dir1), preferedDirection, size - 1, examined, collectedLand);
                }
                if (WrapsAroundWorldEdge(centerLocation, dir2) == false)
                {
                    CollectLocations(centerLocation.GetAdjacent(dir2), preferedDirection, size - 1, examined, collectedLand);
                }
                if (WrapsAroundWorldEdge(centerLocation, dir1) == false && WrapsAroundWorldEdge(centerLocation.GetAdjacent(dir1), dir2) == false)
                {
                    CollectLocations(centerLocation.GetAdjacent(dir1).GetAdjacent(dir2), preferedDirection, size - 1, examined, collectedLand);
                }
            }
            else //size 3 or greater
            {
                foreach (OrdinalDirection dir in DirectionUtils.AllOrdinalDirections)
                {
                    OrdinalDirection dir2 = DirectionUtils.ClockwiseOne(dir);
                    if (WrapsAroundWorldEdge(centerLocation, dir) == false)
                    {
                        CollectLocations(centerLocation.GetAdjacent(dir), preferedDirection, size - 2, examined, collectedLand);
                    }
                    if (WrapsAroundWorldEdge(centerLocation, dir) == false && WrapsAroundWorldEdge(centerLocation.GetAdjacent(dir), dir2) == false)
                    {
                        CollectLocations(centerLocation.GetAdjacent(dir).GetAdjacent(dir2), preferedDirection, size - 2, examined, collectedLand);
                    }
                }
            }
        }
    



        ///// <summary>
        ///// Determines the center most land of a list of land peices  .
        ///// TODO: should this work on locations instead
        ///// </summary>
        //public static Land DetermineCenterLand(List<Land> landList)
        //{

        //    //calcualte the average x,y,z pos of all the land that makes up the field
        //    float xSum = 0;
        //    float ySum = 0;
        //    float zSum = 0;
        //    foreach (Land land in landList)
        //    {
        //        xSum += land.LocationOn.X;
        //        ySum += land.LocationOn.Y;
        //        zSum += land.LocationOn.Z;
        //    }
        //    float xAvg = xSum / landList.Count;
        //    float yAvg = ySum / landList.Count;
        //    float zAvg = zSum / landList.Count;


        //    //find the peice of land in the field closest to this spot
        //    Land closestLand = null;
        //    float closestLandDist = float.MaxValue;
        //    foreach (Land land in landList)
        //    {
        //        float landDist = (float)Math.Sqrt((land.LocationOn.X - xAvg) * (land.LocationOn.X - xAvg) + (land.LocationOn.Y - yAvg) * (land.LocationOn.Y - yAvg) + (land.LocationOn.Z - zAvg) * (land.LocationOn.Z - zAvg));
        //        if (landDist < closestLandDist)
        //        {
        //            closestLandDist = landDist;
        //            closestLand = land;
        //        }
        //    }

        //    //set land on to be the land closest to the center
        //    return closestLand;
        //}
    }
}
