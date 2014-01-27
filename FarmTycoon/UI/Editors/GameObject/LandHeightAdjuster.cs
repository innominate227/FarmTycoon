using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{

    /// <summary>
    /// Used to adjust height of one or more tiles of land, in such as way as the user would expect based on land selected, and adjustment mode (smooth or cliff)
    /// </summary>
    public class LandHeightAdjuster
    {
        /// <summary>
        /// Set of all land that has had its height changed
        /// </summary>
        private HashSet<Land> _landChanged = new HashSet<Land>();


        public LandHeightAdjuster()
        {
        }


        /// <summary>
        /// Raise the land passed by one height unit
        /// </summary>
        public void RaiseLand(Land landToRaise, bool smooth, LandCorner cornerToRaise)
        {
            RaiseLand(new List<Land>(new Land[] { landToRaise }), smooth, cornerToRaise);
        }
        
        /// <summary>
        /// Raise the land passed by one height unit
        /// </summary>
        public void RaiseLand(List<Land> landToRaise, bool smooth)
        {
            RaiseLand(landToRaise, smooth, LandCorner.Center);
        }

        /// <summary>
        /// Raise the land passed by one height unit, return the number of land tiles changed
        /// </summary>
        public int RaiseLand(List<Land> landToRaise, bool smooth, LandCorner cornerToRaise)
        {
            if (landToRaise.Count == 0)
            {
                return 0;
            }

            Program.Game.PathFinder.StartBatchInvalidate();

            if (landToRaise.Count == 1)
            {
                //only raise if allowed
                if (CanRaiseLand(landToRaise[0]))
                {
                    //if were just raising one tile raise the correct corner
                    landToRaise[0].RaiseCorner(cornerToRaise);
                    AddToLandChanged(landToRaise[0]);

                    //smooth the land raised
                    if (smooth)
                    {
                        SmoothSourndingLand(landToRaise[0]);
                    }
                }
            }
            else
            {
                //raise the heights of corners of all the lowest peices of land
                List<Land> lowestLand = GetLowestEditableLand(landToRaise);
                foreach (Land selectedLand in lowestLand)
                {
                    RaiseLowestCorners(selectedLand);
                    AddToLandChanged(selectedLand);                 
                }

                //smooth the land raised
                if (smooth)
                {
                    SmoothSourndingLand(lowestLand);                    
                }
            }

            Program.Game.PathFinder.EndBatchInvalidate();

            int numberChanged = _landChanged.Count;
            AfterAdjusment();

            return numberChanged;
        }




        /// <summary>
        /// Lower the land passed by one height unit
        /// </summary>
        public void LowerLand(Land landToLower, bool smooth, LandCorner cornerToLower)
        {
            LowerLand(new List<Land>(new Land[] { landToLower }), smooth, cornerToLower);
        }

        /// <summary>
        /// Lower the land passed by one height unit
        /// </summary>
        public void LowerLand(List<Land> landToLower, bool smooth)
        {
            LowerLand(landToLower, smooth, LandCorner.Center);
        }

        /// <summary>
        /// Lower the land passed by one height unit
        /// </summary>
        public int LowerLand(List<Land> landToLower, bool smooth, LandCorner cornerToLower)
        {
            if (landToLower.Count == 0)
            {
                return 0;
            }


            Program.Game.PathFinder.StartBatchInvalidate();

            if (landToLower.Count == 1)
            {
                //only raise if allowed
                if (CanLowerLand(landToLower[0]))
                {
                    //if were just lowering one tile lower the correct corner
                    landToLower[0].LowerCorner(cornerToLower);
                    AddToLandChanged(landToLower[0]);

                    //smooth the land lowered
                    if (smooth)
                    {
                        SmoothSourndingLand(landToLower[0]);
                    }
                }
            }
            else
            {
                //lower the heights corners of all the heights peices of land
                List<Land> highestLand = GetHighestEditableLand(landToLower);
                foreach (Land selectedLand in highestLand)
                {
                    LowerHeighestCorners(selectedLand);
                    AddToLandChanged(selectedLand);
                }

                //smooth the land lowered
                if (smooth)
                {
                    SmoothSourndingLand(highestLand);                    
                }
            }

            Program.Game.PathFinder.EndBatchInvalidate();

            int numberChanged = _landChanged.Count;
            AfterAdjusment();

            return numberChanged;
        }



        /// <summary>
        /// Called after rasing or lowering land to refresh land, and make other nessisary adjusments
        /// </summary>
        private void AfterAdjusment()
        {
            //locations that need to be refreshed, (changed locations + surronding locations)
            HashSet<Location> locationsToRefresh = new HashSet<Location>();

            foreach (Land landChanged in _landChanged)
            {
                Location landLocation = landChanged.LocationOn;

                if (locationsToRefresh.Contains(landLocation) == false)
                {
                    locationsToRefresh.Add(landLocation);
                }

                //add surounding locations
                foreach (OrdinalDirection direction in DirectionUtils.AllOrdinalDirections)
                {
                    Location locationAdjacent1 = landLocation.GetAdjacent(direction);
                    Location locationAdjacent2 = landLocation.GetAdjacent(direction).GetAdjacent(direction);
                    Location locationAdjacent3 = landLocation.GetAdjacent(direction).GetAdjacent(DirectionUtils.ClockwiseOne(direction));

                    if (locationsToRefresh.Contains(locationAdjacent1) == false)
                    {
                        locationsToRefresh.Add(locationAdjacent1);
                    }
                    if (locationsToRefresh.Contains(locationAdjacent2) == false)
                    {
                        locationsToRefresh.Add(locationAdjacent2);
                    }
                    if (locationsToRefresh.Contains(locationAdjacent3) == false)
                    {
                        locationsToRefresh.Add(locationAdjacent3);
                    }
                }
            }            
            
            //refresh the objects at the locations that needs to be refreshed
            Tile.StartChangeSet();
            foreach (Location location in locationsToRefresh)
            {
                foreach (GameObject gameObj in location.AllObjects.ToArray()) //to array because if a worker is on the land they end up geting removed and readded to the location when they chane Z position
                {
                    gameObj.UpdateTiles();
                }
            }
            Tile.EndChangeSet();

            //clear changed list for next time
            _landChanged.Clear();
        }
                
        /// <summary>
        /// Get the land in the list that is lower than all other land (and can be raised)
        /// </summary>
        private List<Land> GetLowestEditableLand(List<Land> landList)
        {
            List<Land> toReturn = new List<Land>();

            //determine the z of the lowest peice
            int lowestZ = int.MaxValue;
            foreach (Land land in landList)
            {
                if (CanRaiseLand(land))
                {
                    if (land.MinHeight < lowestZ)
                    {
                        lowestZ = land.MinHeight;
                    }
                }
            }

            //add all that are that low to the list to return
            foreach (Land land in landList)
            {
                if (CanRaiseLand(land))
                {
                    if (land.MinHeight == lowestZ)
                    {
                        toReturn.Add(land);
                    }
                }
            }

            return toReturn;
        }
                       
        /// <summary>
        /// Get the land in the list that is higher than all other land, and can be lowered
        /// </summary>
        /// <param name="landList"></param>
        /// <returns></returns>
        private List<Land> GetHighestEditableLand(List<Land> landList)
        {
            List<Land> toReturn = new List<Land>();

            //determine the z of the heighest peice
            int highestZ = int.MinValue;
            foreach (Land land in landList)
            {
                if (CanLowerLand(land))
                {
                    if (land.MaxHeight > highestZ)
                    {
                        highestZ = land.MaxHeight;
                    }
                }
            }

            //add all that are that high to the list to return
            foreach (Land land in landList)
            {
                if (CanLowerLand(land))
                {
                    if (land.MaxHeight == highestZ)
                    {
                        toReturn.Add(land);
                    }
                }
            }

            return toReturn;
        }
        
        /// <summary>
        /// Can the land passed be reaised
        /// </summary>
        private bool CanRaiseLand(Land land)
        {
            if (land.MinHeight == FarmData.Current.LandInfo.MaxHeight)
            {
                return false;
            }

            //can raise if there is stuff on it            
            if (land.LocationOn.Contains<Road>())
            {
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Can the land passed be lowered
        /// </summary>
        /// <param name="land"></param>
        /// <returns></returns>
        private bool CanLowerLand(Land land)
        {         
            if (land.MaxHeight == FarmData.Current.LandInfo.MinHeight)
            {
                return false;
            }

            //always possible to lower land until we are at the min height, bacuase other object must always be above land

            return true;
        }
        
        /// <summary>
        /// Smooth all raiseable surrounding land such that there are no clifs, countinues smoothing until everything is smooth
        /// </summary>
        private void SmoothSourndingLand(Land smoothCenter)
        {
            SmoothSourndingLand(new List<Land>(new Land[] { smoothCenter }));
        }

        /// <summary>
        /// Smooth all raiseable surrounding land such that there are no clifs, countinues smoothing until everything is smooth
        /// </summary>
        private void SmoothSourndingLand(List<Land> smoothCenter)
        {
            Queue<Land> smoothQueue = new Queue<Land>();
            foreach (Land smoothCenterLand in smoothCenter)
            {
                smoothQueue.Enqueue(smoothCenterLand);
            }

            while (smoothQueue.Count > 0)
            {
                //the land we are smoothing around 
                Land smoothAround = smoothQueue.Dequeue();

                foreach (OrdinalDirection ordinalDirection in DirectionUtils.AllOrdinalDirections)
                {
                    //get the land adjacent in that direction
                    Land adjacentLand = smoothAround.GetAdjacent(ordinalDirection);

                    //dont smooth around the edges of the world
                    if (Math.Abs(adjacentLand.LocationOn.X - smoothAround.LocationOn.X) > 1 || Math.Abs(adjacentLand.LocationOn.Y - smoothAround.LocationOn.Y) > 1)
                    {
                        continue;
                    }

                    //if we cant raise that land skip it
                    if (CanRaiseLand(adjacentLand) == false)
                    {
                        continue;
                    }

                    //track if the adjacent land height was changed
                    bool adjustmentsMade = false;

                    //get the height of the tiles two corners that touch that land
                    int myCorner1Height = smoothAround.GetHeight(DirectionUtils.CounterClockwiseOneCardinal(ordinalDirection));
                    int myCorner2Height = smoothAround.GetHeight(DirectionUtils.ClockwiseOneCardinal(ordinalDirection));

                    //get which corner of the adjacnet land coresponds                
                    CardinalDirection adjacentCorner1 = DirectionUtils.ClockwiseOneCardinal(DirectionUtils.OppositeDirection(ordinalDirection));
                    CardinalDirection adjacentCorner2 = DirectionUtils.CounterClockwiseOneCardinal(DirectionUtils.OppositeDirection(ordinalDirection));

                    //adjust corner 1 until they are the same height
                    int adjacentCorner1Height = adjacentLand.GetHeight(adjacentCorner1);
                    while (myCorner1Height != adjacentCorner1Height)
                    {
                        if (myCorner1Height > adjacentCorner1Height)
                        {
                            adjacentLand.RaiseCorner(adjacentCorner1);
                        }
                        else
                        {
                            adjacentLand.LowerCorner(adjacentCorner1);
                        }
                        adjacentCorner1Height = adjacentLand.GetHeight(adjacentCorner1);
                        adjustmentsMade = true;
                    }

                    //adjust corner 2 until they are the same height
                    int adjacentCorner2Height = adjacentLand.GetHeight(DirectionUtils.CounterClockwiseOneCardinal(DirectionUtils.OppositeDirection(ordinalDirection)));
                    while (myCorner2Height != adjacentCorner2Height)
                    {
                        if (myCorner2Height > adjacentCorner2Height)
                        {
                            adjacentLand.RaiseCorner(adjacentCorner2);
                        }
                        else
                        {
                            adjacentLand.LowerCorner(adjacentCorner2);
                        }
                        adjacentCorner2Height = adjacentLand.GetHeight(adjacentCorner2);
                        adjustmentsMade = true;
                    }

                    //if adjustments were made add to list of changed land
                    if (adjustmentsMade)
                    {
                        AddToLandChanged(adjacentLand);
                        smoothQueue.Enqueue(adjacentLand);
                    }
                }
            }

        }
        
        /// <summary>
        /// Add the land passed to the land changed list
        /// </summary>
        private void AddToLandChanged(Land land)
        {
            if (_landChanged.Contains(land) == false)
            {
                _landChanged.Add(land);
            }
        }
        
        /// <summary>
        /// Raise the corner(s) of the land that are lower than all others
        /// </summary>
        private void RaiseLowestCorners(Land land)
        {
            foreach (CardinalDirection dir in DirectionUtils.AllCardinalDirections)
            {
                if (land.GetExtraHeight(dir) == 0)
                {
                    land.RaiseCorner(dir);
                }
            }
        }

        /// <summary>
        /// Lower the height of the heighest corner(s) of the land
        /// </summary>
        private void LowerHeighestCorners(Land land)
        {            
            //lower any corner 2 higher than the others first if none are found then
            //try 1 higher, and finally 0 higher
            for (int amountHigher = 2; amountHigher >= 0; amountHigher--)
            {
                bool cornerLowered = false;
                //check each corner
                foreach (CardinalDirection dir in DirectionUtils.AllCardinalDirections)
                {
                    if (land.GetExtraHeight(dir) == amountHigher)
                    {
                        land.LowerCorner(dir);
                        cornerLowered = true;
                    }
                }

                //if we lowered a corner were done
                if (cornerLowered) { break; }
            }
        }

        /// <summary>
        /// Make the peice of land flatter
        /// </summary>
        private void Flatten(Land land)
        {
            int extraNorth = land.GetExtraHeight(CardinalDirection.North);
            int extraEast = land.GetExtraHeight(CardinalDirection.East);
            int extraSouth = land.GetExtraHeight(CardinalDirection.South);
            int extraWest = land.GetExtraHeight(CardinalDirection.West);
                
            if (extraNorth == 2 || extraEast == 2 || extraSouth == 2 || extraWest == 2)
            {
                if (extraNorth == 2)
                {
                    land.LowerCorner(CardinalDirection.North);
                }
                if (extraEast == 2)
                {
                    land.LowerCorner(CardinalDirection.East);
                }
                if (extraSouth == 2)
                {
                    land.LowerCorner(CardinalDirection.South);
                }
                if (extraWest == 2)
                {
                    land.LowerCorner(CardinalDirection.West);
                }
                if (extraNorth == 0)
                {
                    land.RaiseCorner(CardinalDirection.North);
                }
                if (extraEast == 0)
                {
                    land.RaiseCorner(CardinalDirection.East);
                }
                if (extraSouth == 0)
                {
                    land.RaiseCorner(CardinalDirection.South);
                }
                if (extraWest == 0)
                {
                    land.RaiseCorner(CardinalDirection.West);
                }
            }
            else
            {
                if (extraNorth == 1)
                {
                    land.LowerCorner(CardinalDirection.North);
                }
                if (extraEast == 1)
                {
                    land.LowerCorner(CardinalDirection.East);
                }
                if (extraSouth == 1)
                {
                    land.LowerCorner(CardinalDirection.South);
                }
                if (extraWest == 1)
                {
                    land.LowerCorner(CardinalDirection.West);
                }
            }
        }

    }
}
