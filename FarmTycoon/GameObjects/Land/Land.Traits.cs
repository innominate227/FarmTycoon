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
        #region MemberVars

        /// <summary>
        /// Set of traits for the land
        /// </summary>
        private TraitSet _traits;

        /// <summary>
        /// Set to true if the land is owned by the player
        /// </summary>
        private bool _owned = false;

        /// <summary>
        /// Set to true if the land is the entry to the players farm
        /// </summary>
        private bool _entry = false;

        #endregion

        #region Setup Delete
        
        /// <summary>
        /// Only called from the main Setup method when the Land is first created
        /// </summary>
        private void SetupTraits()
        {
            _traits = new TraitSet();
            _traits.Setup((LandInfo)FarmData.Current.GetInfo(LandInfo.UNIQUE_NAME));
        }

        private void DeleteTraits()
        {
            _traits.Delete();
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Set of traits for the land
        /// </summary>
        public TraitSet Traits
        {
            get { return _traits; }
        }


        /// <summary>
        /// Set to true if the land is owned by the player
        /// </summary>        
        public bool Owned
        {
            get { return _owned; }
            set 
            { 
                _owned = value;
                UpdatePathEffect();
                UpdateNeightborPathEffect();
            }
        }

        /// <summary>
        /// Set to true if the land is the entry to the players farm
        /// </summary>        
        public bool Entry
        {
            get { return _entry; }
            set
            {
                _entry = value; 
                UpdatePathEffect();
                UpdateNeightborPathEffect();                
            }
        }


        /// <summary>
        /// Update the path effect for all neighbors
        /// </summary>
        private void UpdateNeightborPathEffect()
        {
            foreach (OrdinalDirection dir in DirectionUtils.AllOrdinalDirections)
            {
                GetAdjacent(dir).UpdatePathEffect();
            }
        }
        

        #endregion

        #region Logic

        /// <summary>
        /// Update the slope traits for this peice of land.
        /// This needs to be called on each peice of land once after all land knows who its neighbors are
        /// </summary>
        public void AdjustSlopeTrait()
        {   
            //get the slope trait to update, make sure we have a slope trait
            bool hasSlopeTrait = _traits.HasTrait(SpecialTraits.SLOPE_TRAIT);
            if (hasSlopeTrait == false) { return; }
            
            //calculate the vlaue for the slope trait
            int slopeTraitValue = this.CalculateSteepness();
            foreach (OrdinalDirection direction in DirectionUtils.AllOrdinalDirections)
            {
                slopeTraitValue += this.GetAdjacent(direction).CalculateSteepness() * 2;
                slopeTraitValue += this.GetAdjacent(direction).GetAdjacent(DirectionUtils.ClockwiseOne(direction)).CalculateSteepness();
            }

            //set the value for the slope trait
            _traits.SetTraitValue(SpecialTraits.SLOPE_TRAIT, slopeTraitValue);            
        }

        /// <summary>
        /// Tell all neighbors to adjust their slope traits, and the path effects
        /// </summary>
        private void TellNeighborsAboutHeightChange()
        {
            foreach (OrdinalDirection direction in DirectionUtils.AllOrdinalDirections)
            {
                GetAdjacent(direction).AdjustSlopeTrait();                
                GetAdjacent(direction).GetAdjacent(DirectionUtils.ClockwiseOne(direction)).AdjustSlopeTrait();
                GetAdjacent(direction).UpdatePathEffect();
            }
        }
        
        /// <summary>
        /// Calculate and return the steepness for this peice of land
        /// </summary>
        private int CalculateSteepness()
        {
            //sum the extra height in each direction
            int extraHeightSum = 0;
            foreach (CardinalDirection dir in DirectionUtils.AllCardinalDirections)
            {
                extraHeightSum += GetExtraHeight(dir);
            }

            if (extraHeightSum == 0)
            {
                //no extra we are perfectly flat
                return 0;
            }
            else if (extraHeightSum == 1 || extraHeightSum == 3)
            {
                //either 1 corner higher, or 3 corners higher (1 corner lower) means we are mostly flat
                return 1;
            }
            else //2 or 4
            {
                //2 corners are higher so we are not very flat (or 1 corner is much higher, 2+1+1 = 4)
                return 2;
            }
        }                

        #endregion

        #region Save Load
        private void WriteStateV1Traits(StateWriterV1 writer)
        {
            writer.WriteObject(_traits);
            writer.WriteBool(_owned);
            writer.WriteBool(_entry);
        }

        private void ReadStateV1Traits(StateReaderV1 reader)
        {
            _traits = reader.ReadObject<TraitSet>();
            _owned = reader.ReadBool();
            _entry = reader.ReadBool();
        }

        #endregion
        
    }
}
