using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    /// <summary>
    /// A set of taits that all belong to one game object
    /// </summary>
    public partial class TraitSet : ISavable, ITraitSet
    {
        #region Member Vars

        /// <summary>
        /// Info object with the list of traits in the set
        /// </summary>        
        private TraitInfoSet _traitInfoSet;
        
        /// <summary>
        /// value of each trait in the trait set indexed by the trait ID minus the min trait ID.
        /// using an array here is a lot faster than dictionary
        /// </summary>        
        private int[] _traitValues;
                
        /// <summary>
        /// The trait set this trait set is paired with.  Or null if it is not paired.
        /// </summary>
        private TraitSet _pairedSet;
        
        /// <summary>
        /// Notification for adjusting the traits
        /// </summary>
        private Notification _notification;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a TraitSet.
        /// Setup or ReadState must be called after the crop is created.
        /// </summary>
        public TraitSet() : base() { }
        
        /// <summary>
        /// Setup the trait set to have traits as specified by the traits info object passed
        /// </summary>
        public void Setup(ITraitsInfo traitsInfo)
        {
            _traitInfoSet = traitsInfo.Traits;            
                        
            //intitlize trait value array
            InitilizeTraitValuesArray();
                                    
            //register to be notified every one day to adjust the value of the traits
            _notification = Program.GameThread.Clock.RegisterNotification(DayPassed, 1.0, true);
        }

        /// <summary>
        /// Iniitlize the array to hold all trait values based on the current trait info set.
        /// And initlize each spot in the array to have the start value for the trait
        /// </summary>
        private void InitilizeTraitValuesArray()
        {
            if (_traitInfoSet.TraitInfoList.Count > 0)
            {
                _traitValues = new int[_traitInfoSet.MaxTraitId + 1 - _traitInfoSet.MinTraitId];
                for (int traitIndex = 0; traitIndex < _traitValues.Length; traitIndex++)
                {
                    _traitValues[traitIndex] = -1;
                }
            }

            //intilize the traits to have their start values
            foreach (TraitInfo traitInfo in _traitInfoSet.TraitInfoList)
            {
                _traitValues[traitInfo.Id - _traitInfoSet.MinTraitId] = traitInfo.StartValue;
            }
        }
        
        /// <summary>
        /// Delete the trait set
        /// </summary>
        public void Delete()
        {
            //unregister the notification
            Program.GameThread.Clock.RemoveNotification(_notification);

            //break pairings if there were any
            BreakPairing();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Info object that has a list of traits
        /// </summary>
        public ITraitsInfo TraitsInfo
        {
            get { return _traitInfoSet.InfoSetOwner; }
        }

        /// <summary>
        /// Return if the trait set has a trait with the trait id passed
        /// </summary>
        public bool HasTrait(int traitId)
        {
            int traitIndex = traitId - _traitInfoSet.MinTraitId;
            if (traitIndex < 0 || traitIndex >= _traitValues.Length) { return false; }
            return (_traitValues[traitIndex] != -1);
        }

        /// <summary>
        /// Get a the value of the trait in the set with the id passed, or -1 if no such trait exsits in the set
        /// </summary>
        public int GetTraitValue(int traitId)
        {
            int traitIndex = traitId - _traitInfoSet.MinTraitId;
            if (traitIndex < 0 || traitIndex >= _traitValues.Length) { return -1; }
            return _traitValues[traitIndex];
        }
        
        /// <summary>
        /// Get a the value of the trait in the set with the id passed.
        /// For efficeny no check is made as to wether the trait id passed is valid.
        /// </summary>
        public int GetTraitValueUnsafe(int traitId)
        {
            int traitIndex = traitId - _traitInfoSet.MinTraitId;
            return _traitValues[traitIndex];
        }
        
        /// <summary>
        /// Set the value of the trait in the set with the id passed.
        /// Or does nothing if no such trait exsits in the set.
        /// </summary>
        public void SetTraitValue(int traitId, int val)
        {
            //make sure the trait id is value
            int traitIndex = traitId - _traitInfoSet.MinTraitId;
            if (traitIndex < 0 || traitIndex >= _traitValues.Length) { return; }
            if (_traitValues[traitIndex] == -1) { return; }

            //set the value
            _traitValues[traitIndex] = val;

            //check that value is within range and set value of paired trait set
            UpdateAfterValue(traitId);
        }

        /// <summary>
        /// Set the value of the trait in the set with the id passed.
        /// For efficeny no check is made as to wether the trait id passed is valid.
        /// </summary>
        public void SetTraitValueUnsafe(int traitId, int val)
        {
            //set the value
            int traitIndex = traitId - _traitInfoSet.MinTraitId;            
            _traitValues[traitIndex] = val;

            //check that value is within range and set value of paired trait set
            UpdateAfterValue(traitId);
        }

        /// <summary>
        /// modify the value of the trait in the set with the id passed.  The modification passed will be applied to the value.
        /// For efficeny no check is made as to wether the trait id passed is valid.
        /// </summary>
        public void ModifyTraitValueUnsafe(int traitId, int modification)
        {
            //modifiy the value
            int traitIndex = traitId - _traitInfoSet.MinTraitId;
            _traitValues[traitIndex] += modification;
            
            //check that value is within range and set value of paired trait set
            UpdateAfterValue(traitId);
        }

        /// <summary>
        /// Get the value of the running quality for this trait or -1 if there is no such trait, or it does not have running quality
        /// </summary>
        public int GetTraitRunningQuality(int traitId)
        {
            if (HasTrait(traitId) == false) { return -1; }
            TraitInfo traitInfo = _traitInfoSet.GetTraitInfo(traitId);
            if (traitInfo.RunningQualityInfo == null) { return -1; }
            return traitInfo.RunningQualityInfo.GetQuality(GetTraitValueUnsafe(traitId));
        }

        /// <summary>
        /// Get the value of the running quality for this trait.
        /// </summary>
        public int GetTraitRunningQualityUnsafe(int traitId)
        {
            TraitInfo traitInfo = _traitInfoSet.GetTraitInfo(traitId);            
            return traitInfo.RunningQualityInfo.GetQuality(GetTraitValueUnsafe(traitId));
        }

        /// <summary>
        /// Get the value of the instantaneous quality for this trait or -1 if there is no such trait, or it does not have instantaneous quality
        /// </summary>
        public int GetTraitInstantaneousQuality(int traitId)
        {
            if (HasTrait(traitId) == false) { return -1; }
            TraitInfo traitInfo = _traitInfoSet.GetTraitInfo(traitId);
            if (traitInfo.InstantaneousQualityInfo == null) { return -1; }
            return traitInfo.InstantaneousQualityInfo.GetQuality(GetTraitValueUnsafe(traitId));
        }

        /// <summary>
        /// Get the value of the instantaneous quality for this trait.
        /// </summary>
        public int GetTraitInstantaneousQualityUnsafe(int traitId)
        {
            TraitInfo traitInfo = _traitInfoSet.GetTraitInfo(traitId);
            return traitInfo.InstantaneousQualityInfo.GetQuality(GetTraitValueUnsafe(traitId));
        }

        /// <summary>
        /// Get the Info for a trait in the set with the id passed
        /// </summary>
        public TraitInfo GetTraitInfo(int traitId)
        {
            if (HasTrait(traitId) == false) { return null; }
            return _traitInfoSet.GetTraitInfo(traitId);
        }

        /// <summary>
        /// Get the Info for a trait in the set with the id passed
        /// </summary>
        public TraitInfo GetTraitInfoUnsafe(int traitId)
        {            
            return _traitInfoSet.GetTraitInfo(traitId);
        }

        /// <summary>
        /// Return a all the ids of each trait in the trait set
        /// </summary>
        public int[] TraitIds
        {
            get
            {
                return _traitInfoSet.TraitIds;
            }
        }

        #endregion

        #region Logic

        #region Pairing

        /// <summary>
        /// Pair the traits in this trait set with the traits in the trait set passed.
        /// Where traits that have the same name are paired.
        /// The traits in this trait set will take on the value of the traits in the paired set.
        /// </summary>
        public void Pair(TraitSet traitSet)
        {
            //pair the this trait set with the set passed, and vice versa
            _pairedSet = traitSet;
            traitSet._pairedSet = this;

            //take on the value of the traits in the triat set passed
            foreach (int traitId in _traitInfoSet.TraitIds)
            {
                //get the value in the paired
                int pairVal = traitSet.GetTraitValue(traitId);

                //assiming it not -1 (wich means the other set does not have that trait) then set our value to its value
                if (pairVal != -1)
                {
                    SetTraitValue(traitId, pairVal);
                }
            }
        }


        /// <summary>
        /// Break pairing between the traits in this trait set and the trait set it was previouslt paired with
        /// </summary>
        public void BreakPairing()
        {
            if (_pairedSet != null)
            {
                _pairedSet._pairedSet = null;
                _pairedSet = this;
            }
        }

        #endregion

        #region Update Values

        /// <summary>
        /// Apply the event with the name passed to the traits in the set.  Uses the effect where the worker is not on equipment.
        /// </summary>        
        public void ApplyActionOrEventToTraits(ActionOrEventType actionOrEventType)
        {
            ApplyActionOrEventToTraits(actionOrEventType, false);
        }

        /// <summary>
        /// Apply the event with the name passed to the traits in the set.  Optionally use the different effect for when the worker is on equipment
        /// </summary>        
        public void ApplyActionOrEventToTraits(ActionOrEventType actionOrEventType, bool onEquipment)
        {
            //apply the event to all traits in the set
            foreach (int traitId in _traitInfoSet.TraitIds)
            {
                //determine the effect the action has
                int effect = _traitInfoSet.GetTraitInfo(traitId).AmountActionOrEventEffectsTrait(actionOrEventType, onEquipment);                
                if (effect == 0) { continue; }

                //modify the traits value
                ModifyTraitValueUnsafe(traitId, effect);
            }
        }

        /// <summary>
        /// apply the item passed to the traits
        /// </summary>        
        public void ApplyItemToTraits(ItemType itemToApply)
        {
            ApplyItemToTraits(itemToApply, 1);
        }

        /// <summary>
        /// apply the item passed to the traits.  apply the item amount times
        /// </summary>        
        public void ApplyItemToTraits(ItemType itemToApply, int amount)
        {
            ApplyItemToTraits(itemToApply.BaseType, amount);
        }

        /// <summary>
        /// apply the item passed to the traits.
        /// </summary>        
        public void ApplyItemToTraits(ItemTypeInfo itemToApply)
        {
            ApplyItemToTraits(itemToApply, 1);
        }

        /// <summary>
        /// apply the item passed to the traits.  apply the item amount times
        /// </summary>        
        public void ApplyItemToTraits(ItemTypeInfo itemToApply, int amount)
        {
            //apply the item to all traits in the set
            foreach (int traitId in _traitInfoSet.TraitIds)
            {                
                //determine the effect the item has
                int effect = _traitInfoSet.GetTraitInfo(traitId).AmountItemEffectsTrait(itemToApply) * amount;
                if (effect == 0) { continue; }
                                
                //modify the traits value
                ModifyTraitValueUnsafe(traitId, effect);
            }

        }
        
        /// <summary>
        /// Called once each day so current values of each trait in the set can effect the trait
        /// </summary>
        private void DayPassed()
        {            
            //check all traits in the set
            foreach (int traitId in _traitInfoSet.TraitIds)
            {
                //get info for this trait, and the current value of the trait
                int value = GetTraitValueUnsafe(traitId);
                TraitInfo traitInfo = _traitInfoSet.GetTraitInfo(traitId);

                //get the effect the trait has on itself
                int effect = traitInfo.AmountCurrentValueEffectsTrait(value);
                
                //if other traits also effect the value of this trait check what their effect is
                foreach (int otherEffectingTraitId in traitInfo.OtherTraitsThatEffect)
                {
                    int otherTraitValue = GetTraitValueUnsafe(otherEffectingTraitId);
                    int otherTraitsEffect = traitInfo.AmountOtherTraitsValueEffectsTrait(otherEffectingTraitId, otherTraitValue);
                    effect += otherTraitsEffect;
                }

                //modify the traits value
                ModifyTraitValueUnsafe(traitId, effect);
            }
        }

        /// <summary>
        /// Checks that the trait is between min and max value and if not puts it there.
        /// If the trait is paired update the value of the paired trait
        /// </summary>
        private void UpdateAfterValue(int traitId)
        {
            //index for the trait
            int traitIndex = traitId - _traitInfoSet.MinTraitId;      

            //get the value and the trait info for the trait of interest
            int value = GetTraitValueUnsafe(traitId);
            TraitInfo traitInfo = _traitInfoSet.GetTraitInfo(traitId);

            //make sure the value is within our range
            if (value > traitInfo.MaximumValue)
            {
                value = traitInfo.MaximumValue;
                _traitValues[traitIndex] = value;
            }
            else if (value < traitInfo.MinimumValue)
            {
                value = traitInfo.MinimumValue;
                _traitValues[traitIndex] = value;
            }

            //if we are paired then set that has the same trait then set the value of the trait in the paired set as well
            if (_pairedSet != null && _pairedSet.HasTrait(traitId))
            {
                //index for the trait in our paired set
                int traitIndexPaired = traitId - _pairedSet._traitInfoSet.MinTraitId;

                //info for the trait in the paired set
                TraitInfo pairedTraitInfo = _pairedSet.GetTraitInfo(traitId);
                
                //make sure the value we set for them is within their range
                if (value > pairedTraitInfo.MaximumValue)
                {
                    value = pairedTraitInfo.MaximumValue;
                }
                else if (value < pairedTraitInfo.MinimumValue)
                {
                    value = pairedTraitInfo.MinimumValue;
                }

                //set the value in our paired set
                _pairedSet._traitValues[traitIndexPaired] = value;
            }
        }
        
        #endregion

        #endregion
        
        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
		{
            //the TraitInfoSet is not an info object itself, so instead we write our reference to the owner of the set
            writer.WriteInfo(_traitInfoSet.InfoSetOwner);

            //write the value of each trait, save each value beside its traitinfo so we know what trait info they were for again when we load            
            writer.WriteInt(_traitInfoSet.TraitInfoList.Count);
            foreach (TraitInfo traitInfo in _traitInfoSet.TraitInfoList)
            {
                writer.WriteInfo(traitInfo);
                writer.WriteInt(GetTraitValueUnsafe(traitInfo.Id));
            }
            
            //the set we are paired with
            writer.WriteObject(_pairedSet);

            //state of our notification
            writer.WriteNotification(_notification);            
		}

        public void ReadStateV1(StateReaderV1 reader)
		{
            //get the traits info set
            ITraitsInfo traitsInfoObject = reader.ReadInfo<ITraitsInfo>();
            _traitInfoSet = traitsInfoObject.Traits;

            //initlize the trait values array
            InitilizeTraitValuesArray();

            //read in the value of each trait
            int count = reader.ReadInt();
            for (int num = 0; num < count; num++)
            {
                TraitInfo traitInfo = reader.ReadInfo<TraitInfo>();
                int traitValue = reader.ReadInt();

                //make sure the trait is still in the set (it could have been deleted from the farm data)
                //set the value that was saved
                if (traitInfo != null && this.HasTrait(traitInfo.Id))
                {
                    SetTraitValue(traitInfo.Id, traitValue);
                }
            }

            //read in the set we are paired with
            _pairedSet = reader.ReadObject<TraitSet>();

            //read in our notification
            _notification = reader.ReadNotification(DayPassed); 
		}

        public void AfterReadStateV1()
        {
        }
        #endregion

    }
}
