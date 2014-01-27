using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// Location that can be walked to from a another peice of land, and the cost to get there
    /// </summary>
    public class WalkToItem
    {
        public Location Location;
        public int Cost;
        public bool Road;
    }

    /// <summary>
    /// Cache of land that is adjacent to other land and can be walked to from there
    /// </summary>
    public class CanWalkToCache
    {        
        
        /// <summary>
        /// the cache, for each location a list of locations that is walkable to from there
        /// </summary>
        private Dictionary<Location, List<WalkToItem>> _cache = new Dictionary<Location, List<WalkToItem>>();



        private int _hitRate;
        private int _missRate;


        /// <summary>
        /// Get the list of all locations that can be walked to from the location passed.
        /// </summary>
        public List<WalkToItem> GetWalkToList(Location walkFrom)
        {
            ////Debug: never cache
            //if (_cache.ContainsKey(walkFrom))
            //{
            //    _cache.Remove(walkFrom);
            //}
            //UpdateForLocation(walkFrom);


            if (_cache.ContainsKey(walkFrom) == false)
            {
                UpdateForLocation(walkFrom);
                _missRate++;
            }
            else
            {
                _hitRate++;
            }

            return _cache[walkFrom];
        }


        /// <summary>
        /// Clear Cahce for location passed, and all adjacent locations
        /// </summary>
        public void ClearForLandAndAdjacnet(Location location)
        {
            //remove the location from the cahce
            _cache.Remove(location);

            //remove all adjacent locations
            foreach (OrdinalDirection direction in DirectionUtils.AllOrdinalDirections)
            {
                Location adjacentLocation = location.GetAdjacent(direction);
                _cache.Remove(adjacentLocation);
            }
        }

        public void Clear()
        {
            _cache.Clear();
        }


        /// <summary>
        /// Update the walkable list for the location passed
        /// </summary>
        private void UpdateForLocation(Location location)
        {
            //add to the cache
            _cache.Add(location, new List<WalkToItem>());

            //get the road and land at the location (which both could be null)
            Road roadAtLocation = location.Find<Road>();
            Land landAtLocation = location.Find<Land>();


            //remeber the locations we added to the cache so far so we dont add the same twice
            HashSet<Location> locationsAdded = new HashSet<Location>();


            if (landAtLocation != null)
            {
                foreach (OrdinalDirection direction in DirectionUtils.AllOrdinalDirections)
                {
                    //if set to true then the worker should prefer not to walk to this tile (but should not be prevented)
                    bool perferNotWalking = false;

                    //get the land adjacent in that direction
                    Land adjacentLand = landAtLocation.GetAdjacent(direction);

                    //if the land is on a location we have already added ignore
                    if (locationsAdded.Contains(adjacentLand.LocationOn))
                    {
                        continue;
                    }

                    //if there is an object at the adjacent location that cannot be walked through then dont go that direction                
                    if (WorkerEditor.CanWalkOn(adjacentLand.LocationOn) == false)
                    {
                        continue;
                    }

                    //if the land is not level with the land in that direction it is not walkable
                    if (landAtLocation.IsLevelWithDirection(direction) == false)
                    {
                        continue;
                    }

                    //if there is a fence on this land that prevents walking to that land, or a fence on the adjacent land that prevents walking
                    bool fencePreventsVisit = false;
                    foreach (Fence fence in landAtLocation.LocationOn.FindAll<Fence>())
                    {
                        if (fence.PlacementState != PlacementState.BeingPlaced && fence.SideOn == direction && fence.Gate == false) { fencePreventsVisit = true; }
                    }
                    foreach (Fence fence in adjacentLand.LocationOn.FindAll<Fence>())
                    {
                        if (fence.PlacementState != PlacementState.BeingPlaced && fence.SideOn == DirectionUtils.OppositeDirection(direction) && fence.Gate == false) { fencePreventsVisit = true; }
                    }
                    if (fencePreventsVisit)
                    {
                        continue;
                    }

                    //if adjacnet land is not part of the FieldLand, and this is part of the FieldLand, and its not the entrance
                    bool onFarmLand = landAtLocation.LocationOn.Contains<FarmLand>();
                    bool adjacentOnFarmLand = adjacentLand.LocationOn.Contains<FarmLand>();
                    if (onFarmLand != adjacentOnFarmLand)
                    {
                        FarmLand farmLand = Program.Game.MasterObjectList.Find<FarmLand>();
                        if (farmLand.Entrance != landAtLocation && farmLand.Entrance != adjacentLand)
                        {
                            continue;
                        }
                    }
                    
                    //if the land is looping around the edge of the world its not walkable
                    if (Math.Abs(adjacentLand.LocationOn.X - landAtLocation.LocationOn.X) != 1 || Math.Abs(adjacentLand.LocationOn.Y - landAtLocation.LocationOn.Y) != 1)
                    {
                        continue;
                    }

                    
                    //if there is a crop that wants space on the land then ok to walk there but prefer not doing so
                    Crop crop = adjacentLand.LocationOn.Find<Crop>();
                    if (crop != null && crop.CropInfo.NeedsSpace)
                    {
                        perferNotWalking = true;
                    }
                    
                    //create walk to item and add to cache
                    WalkToItem newWalkToItem = new WalkToItem();

                    //walk to the location that the land is on
                    newWalkToItem.Location = adjacentLand.LocationOn;
                    newWalkToItem.Cost = 50; //dist is slower because we are walking to a peice of land, and not a road
                    newWalkToItem.Road = false;
                    if (perferNotWalking)
                    {
                        newWalkToItem.Cost = 5000; //if we perfer not walk there try even more to get them to not walk there
                    }
                    _cache[location].Add(newWalkToItem);
                }
            }

            
        }
        
    }
}
