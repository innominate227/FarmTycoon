using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    /// <summary>
    /// Manages the quality of a game object, and the traits used to determine the quality of the game object.
    /// </summary>
    public class Quality : ISavable, IQuality
    {
        #region Member Vars

        /// <summary>
        /// Notification called everytime a day has passed
        /// </summary>
        private Notification _dayPassedNotification;
                
        /// <summary>
        /// Total number used with the _runningQualityDataPoints to determine the running quality
        /// </summary>
        private int _runningQualityTotal;

        /// <summary>
        /// Data Points number used with the _runningQualityTotal to determine the running quality
        /// </summary>
        private int _runningQualityDataPoints;

        /// <summary>
        /// The quality (as last calculated)
        /// </summary>
        private int _quality;
                    
        /// <summary>
        /// traits that effect the quality
        /// </summary>
        private TraitSet _traitSet;

        /// <summary>
        /// If not null, Update the quality of this item to match the quality calculated.
        /// </summary>
        private ItemType _itemWithQuality;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a QualityManager.
        /// Setup or ReadState must be called before using.
        /// </summary>
        public Quality() : base() { }
        
        /// <summary>
        /// Create a quality manager to manage the quailty of an object with the traits passed.
        /// </summary>
        public void Setup(TraitSet traits)
        {
            Setup(traits, null);
        }
            
        /// <summary>
        /// Create a quality manager to manage the quailty of an object with the traits passed.
        /// The quality of the item type passed will be updated to match the quality determined.
        /// </summary>        
        public void Setup(TraitSet traits, ItemType itemWithQuality)
        {            
            _traitSet = traits;                        
            _itemWithQuality = itemWithQuality;
            
            //initilize the special age trait based on the age of the item if there was an item, and there is an age trait            
            if (_itemWithQuality != null && _traitSet.HasTrait(SpecialTraits.AGE_TRAIT))
            {
                _traitSet.SetTraitValue(SpecialTraits.AGE_TRAIT, itemWithQuality.Age);                
            }

            //create day passed handler
            _dayPassedNotification = Program.GameThread.Clock.RegisterNotification(DayPassed, 1.0, true);            
        }
        
        /// <summary>
        /// Delete the quality manager
        /// </summary>
        public void Delete()
        {
            Program.GameThread.Clock.RemoveNotification(_dayPassedNotification);
            _traitSet.Delete();

            //its possible that quality will be requested even after the crop is deleted, this happens if we were about to harvest the crop right when it is deleted
            //set quality to 0, so that we get a real bad quality item from the harvest
            _quality = 0;
        }

        #endregion

        #region Properties
                
        /// <summary>
        /// traits that effect the quality
        /// </summary>        
        public TraitSet TraitSet
        {
            get { return _traitSet; }
        }
        
        /// <summary>
        /// The current quality of the object
        /// </summary>
        public int CurrentQuality
        {
            get { return _quality; }
        }

        /// <summary>
        /// The value of a trait with the traitId passed that determines the quality
        /// </summary>        
        public int[] TraitIds
        {
            get { return _traitSet.TraitIds; }
        }

        /// <summary>
        /// Get the value of the trait with the trait if passed
        /// </summary>        
        public int GetTraitValue(int traitId)
        {
            return _traitSet.GetTraitValue(traitId);
        }

        /// <summary>
        /// Set the value of the trait with the trait if passed
        /// </summary>        
        public void SetTraitValue(int traitId, int value)
        {
            _traitSet.SetTraitValue(traitId, value);
        }
        
        /// <summary>
        /// The value of a trait with the traitId passed that determines the quality
        /// </summary>        
        public TraitInfo GetTraitInfo(int traitId)
        {
            return _traitSet.GetTraitInfo(traitId);
        }

        #endregion 
        
        #region Logic

        /// <summary>
        /// Called when 24 game hours have passed, the current quality is recalculated each day
        /// </summary>
        private void DayPassed()
        {
            //recalculate the current running quality
            int currentRunningQualityTotal = 0;
            int currentRunningQualityCount = 0;
            foreach (int traitId in _traitSet.TraitIds)
            {
                //get info for the trait
                TraitInfo traitInfo = _traitSet.GetTraitInfoUnsafe(traitId);

                //skip traits without running quality
                if (traitInfo.RunningQualityInfo == null) { continue; }

                //determine the quality and weighted quality qualities
                int traitQuality = _traitSet.GetTraitRunningQuality(traitId);
                int traitWeight = traitInfo.RunningQualityInfo.Weight;
                int weightedQuality = traitQuality * traitWeight;

                //update sums used to calculated overall quality
                currentRunningQualityTotal += weightedQuality;
                currentRunningQualityCount += traitWeight;
            }

            //calculate the current running quality (if there are no running quality traits it is always 100)
            int currentRunningQuality = 100;
            if (currentRunningQualityCount > 0)
            {
                currentRunningQuality = currentRunningQualityTotal / currentRunningQualityCount;
            }

            //add to the total running quality
            _runningQualityTotal += currentRunningQuality;
            _runningQualityDataPoints += 1;

            //calculate running quality
            int runningQuality = _runningQualityTotal / _runningQualityDataPoints;

            //apply instantanious qualities
            foreach (int traitId in _traitSet.TraitIds)
            {
                //get info for the trait
                TraitInfo traitInfo = _traitSet.GetTraitInfoUnsafe(traitId);

                //skip traits without instantinous quality
                if (traitInfo.InstantaneousQualityInfo == null) { continue; }
                
                //determine the instantinous quality
                int instantaneousTraitQuality = _traitSet.GetTraitInstantaneousQualityUnsafe(traitId);
                double instantaneousTraitQualityDouble = instantaneousTraitQuality / 100.0;

                //apply the instantaneous quality to the running quality
                runningQuality = (int)Math.Round(runningQuality * instantaneousTraitQualityDouble);
            }

            //update the last calculated quality
            _quality = runningQuality; 

            //set quality of item if nessisary
            if (_itemWithQuality != null)
            {
                _itemWithQuality.SetQualityFrom100Scale(_quality);

                //if we have an age trait set the age of the item as well                 
                if (_traitSet.HasTrait(SpecialTraits.AGE_TRAIT))
                {
                    _itemWithQuality.Age = _traitSet.GetTraitValue(SpecialTraits.AGE_TRAIT);
                }
            }
        }

        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteNotification(_dayPassedNotification);
            writer.WriteInt(_runningQualityTotal);
            writer.WriteInt(_runningQualityDataPoints);
            writer.WriteInt(_quality);
            writer.WriteObject(_traitSet);
            writer.WriteObject(_itemWithQuality);
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _dayPassedNotification = reader.ReadNotification(DayPassed);
            _runningQualityTotal = reader.ReadInt();
            _runningQualityDataPoints = reader.ReadInt();
            _quality = reader.ReadInt();
            _traitSet = reader.ReadObject<TraitSet>();
            _itemWithQuality = reader.ReadObject<ItemType>();
        }

        public void AfterReadStateV1()
        {
        }
        #endregion

    }
}
