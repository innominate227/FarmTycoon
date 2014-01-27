using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Diagnostics;

namespace FarmTycoon
{
    public partial class Land : GameObject, IHasActionLocation, IHasTraits
    {
        
        #region Events

        /// <summary>
        /// Event raised after the height of the land changes
        /// </summary>
        public event Action HeightChanged;

        #endregion
        
        #region Member Vars
                
        /// <summary>
        /// The height of each corner of the land
        /// index to the array is a CardinalDirection (array is faster than dictionary)
        /// </summary>
        private int[] _height = new int[4];
                
        #endregion
        
        #region Setup Delete

        /// <summary>
        /// Create a new land tile at the location passed
        /// </summary>
        public void SetupLocation(Location location)
        {
            location.Z = 2;
            AddLocationOn(location);
            _height[(int)CardinalDirection.North] = location.Z;
            _height[(int)CardinalDirection.East] = location.Z;
            _height[(int)CardinalDirection.South] = location.Z;
            _height[(int)CardinalDirection.West] = location.Z;
        }
        
        #endregion

        #region Properties
        
        /// <summary>
        /// Get the land adjacent to this land in the direction passed
        /// </summary>
        public Land GetAdjacent(OrdinalDirection direction)
        {
            return LocationOn.GetAdjacent(direction).Find<Land>();            
        }

        /// <summary>
        /// Get the land adjacent to this land in the direction passed
        /// </summary>
        public Land GetAdjacent(CardinalDirection direction)
        {
            return GetAdjacent(DirectionUtils.CounterClockwiseOneOrdinal(direction)).GetAdjacent(DirectionUtils.ClockwiseOneOrdinal(direction));
        }

        /// <summary>
        /// Get the height of the land for the corner passed
        /// </summary>
        public int GetHeight(CardinalDirection corner)
        {
            return _height[(int)corner];
        }

        /// <summary>
        /// The maximum height of any corner of the tile
        /// </summary>
        public int MaxHeight
        {
            get { return MaxOfAllDirections(GetHeight); }
        }

        /// <summary>
        /// The minimum height of any corner of the tile
        /// </summary>
        public int MinHeight
        {
            get { return MinOfAllDirections(GetHeight); }
        }

        /// <summary>
        /// The average height of the land.
        /// </summary>
        public int AverageHeight
        {
            get
            {         
                //sum the extra height in each direction
                int extraHeightSum = 0;
                foreach (CardinalDirection dir in DirectionUtils.AllCardinalDirections)
                {
                    extraHeightSum += GetExtraHeight(dir);
                }

                //if 3 tiles have extra height then return 1 more than min, otherwise return min
                if (extraHeightSum >= 3) { return MinHeight + 1; }
                else { return MinHeight; }
            }
        }

        /// <summary>
        /// Get how much higher the corner in the direction passed is than the lowerst corner.
        /// </summary>
        public int GetExtraHeight(CardinalDirection corner)
        {
            return (GetHeight(corner) - MinHeight);
        }
        
        /// <summary>
        /// The maximum amount of extra height for any of the corners of the land
        /// </summary>
        public int MaxExtraHeightForAnyCorner()
        {
            return MaxOfAllDirections(GetExtraHeight);
        }

        /// <summary>
        /// Location a worker should go to in order to do an action at this land
        /// </summary>
        public Location ActionLocation
        {
            get { return LocationOn; }
        }

        /// <summary>
        /// Land adjacnet to this land in the north east
        /// </summary>
        public Land NorthEast
        {
            get { return GetAdjacent(OrdinalDirection.NorthEast); }
        }

        /// <summary>
        /// Land adjacnet to this land in the north west
        /// </summary>
        public Land NorthWest
        {
            get { return GetAdjacent(OrdinalDirection.NorthWest); }
        }

        /// <summary>
        /// Land adjacnet to this land in the south east
        /// </summary>
        public Land SouthEast
        {
            get { return GetAdjacent(OrdinalDirection.SouthEast); }
        }

        /// <summary>
        /// Land adjacnet to this land in the south west
        /// </summary>
        public Land SouthWest
        {
            get { return GetAdjacent(OrdinalDirection.SouthWest); }
        }
        #endregion

        #region Logic
                        
        #region Adjust Corner Heights
           
        /// <summary>
        /// Raise the all corners of this tile
        /// </summary>
        public void RaiseAll()
        {
            foreach (CardinalDirection dir in DirectionUtils.AllCardinalDirections)
            {
                _height[(int)dir]++;
            }
            AfterHeightChanged();
        }

        /// <summary>
        /// Lower the all corners of the tile
        /// </summary>
        public void LowerAll()
        {
            foreach (CardinalDirection dir in DirectionUtils.AllCardinalDirections)
            {
                _height[(int)dir]--;
            }
            AfterHeightChanged();
        }

        /// <summary>
        /// Raise the height one of this lands corners by one,
        /// depending on the height of the corner compared to other corners other corners may be raised as well
        /// </summary>
        public void RaiseCorner(CardinalDirection corner)
        {
            //get the corners opposite the corner we are raising, and the two adjacnet to it
            CardinalDirection oppositeCorner = DirectionUtils.OppositeDirection(corner);
            CardinalDirection otherCorner1 = DirectionUtils.ClockwiseOne(corner);
            CardinalDirection otherCorner2 = DirectionUtils.CounterClockwiseOne(corner);

            //get the extra height for each corner
            int thisCornerExtra = GetExtraHeight(corner);
            int oppositeCornerExtra = GetExtraHeight(DirectionUtils.OppositeDirection(corner));
            int otherCorner1Extra = GetExtraHeight(DirectionUtils.ClockwiseOne(corner));
            int otherCorner2Extra = GetExtraHeight(DirectionUtils.CounterClockwiseOne(corner));

            //we will always raise this corner we were told to raise
            _height[(int)corner]++;

            //we may need to raise other corners as well to maintain valid heights
            if (thisCornerExtra == 2)
            {
                //we are already 2 higher than the lowest corner right now so raise everyone
                _height[(int)oppositeCorner]++;
                _height[(int)otherCorner1]++;
                _height[(int)otherCorner2]++;
            }
            else if (otherCorner1Extra == 2)
            {
                //a corner adjacent to us is 2 higher than the lowest (the one across from it), need to raise the one across from it
                _height[(int)otherCorner2]++;
            }
            else if (otherCorner2Extra == 2)
            {
                //a corner adjacent to us is 2 higher than the lowest (the one across from it), need to raise the one across from it
                _height[(int)otherCorner1]++;
            }
            else if (thisCornerExtra == 1 && otherCorner1Extra == 0 && otherCorner2Extra == 0)
            {
                //we are one higher than everyone else right now by 1, make two adjecent to us higher
                //also this coers where we and the opposite corner are higher then the two adjacent corners
                _height[(int)otherCorner1]++;
                _height[(int)otherCorner2]++;
            }
            else if (thisCornerExtra == 1 && otherCorner1Extra == 1 && otherCorner2Extra == 0)
            {
                //we, and one adjacent corner are one higher than everyone else right now by 1, and so it one of the corners adjacent to us, raise us and the one adjacent to use thats no already higher
                _height[(int)otherCorner2]++;
            }
            else if (thisCornerExtra == 1 && otherCorner1Extra == 0 && otherCorner2Extra == 1)
            {
                //we are one higher than everyone else right now by 1, and so it one of the corners adjacent to us, raise us and the one adjacent to use thats no already higher
                _height[(int)otherCorner1]++;
            }

            AfterHeightChanged();
        }

        /// <summary>
        /// Lower the height one of this lands corners by one,
        /// depending on the height of the corner compared to other corners other corners may be lowered as well
        /// </summary>
        public void LowerCorner(CardinalDirection corner)
        {
            //get the corners opposite the corner we are lowering, and the two adjacnet to it
            CardinalDirection oppositeCorner = DirectionUtils.OppositeDirection(corner);
            CardinalDirection otherCorner1 = DirectionUtils.ClockwiseOne(corner);
            CardinalDirection otherCorner2 = DirectionUtils.CounterClockwiseOne(corner);

            //get the extra height for each corner
            int thisCornerExtra = GetExtraHeight(corner);
            int oppositeCornerExtra = GetExtraHeight(DirectionUtils.OppositeDirection(corner));
            int otherCorner1Extra = GetExtraHeight(DirectionUtils.ClockwiseOne(corner));
            int otherCorner2Extra = GetExtraHeight(DirectionUtils.CounterClockwiseOne(corner));

            //we will always lower this corner we were told to lower
            _height[(int)corner]--;
            
            if (oppositeCornerExtra == 2)
            {
                //the opposite corner is already 2 higher than the lowest corner right now (this corner) so lower everyone
                _height[(int)oppositeCorner]--;
                _height[(int)otherCorner1]--;
                _height[(int)otherCorner2]--;
            }
            else if (otherCorner1Extra == 2)
            {
                //a corner adjacent to us is 2 higher lower it too
                _height[(int)otherCorner1]--;
            }
            else if (otherCorner2Extra == 2)
            {
                //a corner adjacent to us is 2 higher lower it too
                _height[(int)otherCorner2]--;
            }
            else if (thisCornerExtra == 0 && otherCorner1Extra == 1 && otherCorner2Extra == 1)
            {
                //the two adjcent corners are one higher than us, lower them 
                _height[(int)otherCorner1]--;
                _height[(int)otherCorner2]--;
            }
            else if (thisCornerExtra == 0 && otherCorner1Extra == 1 && otherCorner2Extra == 0)
            {
                //one of the adjcent corners is higher than us, lower it 
                _height[(int)otherCorner1]--;
            }
            else if (thisCornerExtra == 0 && otherCorner1Extra == 0 && otherCorner2Extra == 1)
            {
                //one of the adjcent corners is higher than us, lower it
                _height[(int)otherCorner2]--;
            }
            AfterHeightChanged();
        }


        /// <summary>
        /// Raise the corener of land (or all corners if center is passed)
        /// </summary>
        public void RaiseCorner(LandCorner corner)
        {
            if (corner == LandCorner.North) { RaiseCorner(CardinalDirection.North); }
            else if (corner == LandCorner.South) { RaiseCorner(CardinalDirection.South); }
            else if (corner == LandCorner.East) { RaiseCorner(CardinalDirection.East); }
            else if (corner == LandCorner.West) { RaiseCorner(CardinalDirection.West); }
            else if (corner == LandCorner.Center) { RaiseAll(); }
        }

        /// <summary>
        /// Lower the corner of land (or all corners if center is passed)
        /// </summary>
        /// <param name="corner"></param>
        public void LowerCorner(LandCorner corner)
        {
            if (corner == LandCorner.North) { LowerCorner(CardinalDirection.North); }
            else if (corner == LandCorner.South) { LowerCorner(CardinalDirection.South); }
            else if (corner == LandCorner.East) { LowerCorner(CardinalDirection.East); }
            else if (corner == LandCorner.West) { LowerCorner(CardinalDirection.West); }
            else if (corner == LandCorner.Center) { LowerAll(); }
        }
        

        /// <summary>
        /// Method to be called after the height of one or more corners changes.        
        /// </summary>
        private void AfterHeightChanged()
        {
            CheckHeightRestrictions();
            UpdateLocationZ();
            AdjustSlopeTrait();
            TellNeighborsAboutHeightChange();
            UpdatePathEffect();
            RasieHeightChanged();
        }

        /// <summary>
        /// Check each height to ensure its between the min and max
        /// </summary>
        private void CheckHeightRestrictions()
        {
            //check the height in each direction to make sure its in bounds
            foreach (CardinalDirection dir in DirectionUtils.AllCardinalDirections)
            {
                if (_height[(int)dir] < FarmData.Current.LandInfo.MinHeight) { _height[(int)dir] = FarmData.Current.LandInfo.MinHeight; }
                if (_height[(int)dir] > FarmData.Current.LandInfo.MaxHeight) { _height[(int)dir] = FarmData.Current.LandInfo.MaxHeight; }
            }
        }

        /// <summary>
        /// Update the Z of the location at this land to match the Z of the land
        /// </summary>
        private void UpdateLocationZ()
        {
            LocationOn.Z = MinHeight;
        }

        /// <summary>
        /// Raise the Height Changed event
        /// </summary>
        private void RasieHeightChanged()
        {
            if (HeightChanged != null)
            {
                HeightChanged();
            }
        }
        
        #endregion

        #region Min Max Helper Methods

        /// <summary>
        /// Return the minimum value the function returns for each of the Ordinal Directions
        /// </summary>
        private int MinOfAllDirections(Func<OrdinalDirection, int> function)
        {
            int min = int.MaxValue;
            foreach (OrdinalDirection dir in DirectionUtils.AllOrdinalDirections)
            {
                min = Math.Min(function(dir), min);
            }
            return min;
        }

        /// <summary>
        /// Return the maximum value the function returns for each of the Ordinal Directions
        /// </summary>
        private int MaxOfAllDirections(Func<OrdinalDirection, int> function)
        {
            int max = int.MinValue;
            foreach (OrdinalDirection dir in DirectionUtils.AllOrdinalDirections)
            {
                max = Math.Max(function(dir), max);
            }
            return max;
        }

        /// <summary>
        /// Return the minimum value the function returns for each of the Cardinal Directions
        /// </summary>
        private int MinOfAllDirections(Func<CardinalDirection, int> function)
        {
            int min = int.MaxValue;
            foreach (CardinalDirection dir in DirectionUtils.AllCardinalDirections)
            {
                min = Math.Min(function(dir), min);
            }
            return min;
        }

        /// <summary>
        /// Return the maximum value the function returns for each of the Cardinal Directions
        /// </summary>
        private int MaxOfAllDirections(Func<CardinalDirection, int> function)
        {
            int max = int.MinValue;
            foreach (CardinalDirection dir in DirectionUtils.AllCardinalDirections)
            {
                max = Math.Max(function(dir), max);
            }
            return max;
        }

        #endregion

        /// <summary>
        /// Update the path effect for the land
        /// </summary>
        public void UpdatePathEffect()
        {            
            
            if (LocationOn.Z == 0)
            {
                //if we are at sea level we are unwalkable                
                PathEffect = ObjectsEffectOnPath.Solid;
            }
            else
            {
                ObjectsEffectOnPath effect = ObjectsEffectOnPath.None;
                
                //see if we are blocked in each direction, we are block if we are part of the farm land and they are not, or vica versa
                foreach (OrdinalDirection dir in DirectionUtils.AllOrdinalDirections)
                {
                    //determine if we are owned, our neightbor is not, and we and they are not the entry
                    bool dirIsBlocked = false;
                    Land adjacent = GetAdjacent(dir);
                    bool adjacentOwned = adjacent.Owned;
                    bool adjacentEntry = adjacent.Entry;
                    if (_owned != adjacentOwned && _entry == false && adjacentEntry == false)
                    {
                        dirIsBlocked = true;
                    }

                    //if that direction is blocked or in that its blocked
                    if (dirIsBlocked)
                    {
                        effect |= DirectionUtils.BlocksDirection(dir);
                    }
                }

                //set the new path effect
                PathEffect = effect;
            }
        }

        #endregion

        #region Save Load
        private void WriteStateV1Location(StateWriterV1 writer)
        {
            writer.WriteInt(_height[0]);
            writer.WriteInt(_height[1]);
            writer.WriteInt(_height[2]);
            writer.WriteInt(_height[3]);
        }

        private void ReadStateV1Location(StateReaderV1 reader)
        {
            _height[0] = reader.ReadInt();
            _height[1] = reader.ReadInt();
            _height[2] = reader.ReadInt();
            _height[3] = reader.ReadInt();
        }
        #endregion
    }
}
