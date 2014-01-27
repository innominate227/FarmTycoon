using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// A set of delays that all belong to one game object
    /// </summary>
    public class DelaySet : ISavable
    {
        #region Member Vars
        
        /// <summary>
        /// Other dealy set that can have an effected on the delays in this delay set
        /// For instance a workers delays can be effect by the delays of the Equipments they are on.
        /// When we get the value of a delay from this set we will first multiply it by the value of the same delay in all the currently effecting delay sets
        /// </summary>
        private List<DelaySet> _effectingDelaySets = new List<DelaySet>();
        
        /// <summary>
        /// Info about the delays in the set
        /// </summary>
        private DelayInfoSet _delaysInfo;

        /// <summary>
        /// Game object whos delays these are
        /// </summary>
        private IHasTraits _gameObject;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a DelaySet.
        /// Setup or ReadState must be called after the crop is created.
        /// </summary>
        public DelaySet() : base() { }
        
        /// <summary>
        /// Setup the delay set to have delays as specified by the delay info object passed
        /// </summary>
        public void Setup(IDelaysInfo delaysInfo, IHasTraits gameObject)
        {
            _delaysInfo = delaysInfo.Delays;
            _gameObject = gameObject;
        }


        #endregion

        #region Properties
        
        /// <summary>
        /// Get they delay for the event passed
        /// </summary>
        public double GetDelay(ActionOrEventType actionType)
        {
            //get delay info for that action type
            DelayInfo delayInfo = _delaysInfo.GetDelayInfo(actionType);

            //if we dont have delay info for that type then delay is 1
            if (delayInfo == null) { return 1.0; }
            

            //if we do not find a trait associated the delay will just be the min value
            double delay = delayInfo.MinimumValue;

            //find the Instantaneous Quality of the trait associated with this delay
            int traitQuality = -1;
            if (_gameObject != null) //game object could be null if we created a dealy set to estimate delays without having an actual worker with the dealy
            {
                _gameObject.Traits.GetTraitInstantaneousQuality(delayInfo.TraitId);
            }

            //if we found a quality
            if (traitQuality != -1)
            {
                //get the delay at that quality
                delay = delayInfo.GetDelay(traitQuality);
            }

            //if other effecting dealys should override this one completly instead of being multiplied with it,
            //and there are other effecting delays, then go back to 1
            if (delayInfo.OtherDelaysFullyOverrides && _effectingDelaySets.Count > 0)
            {
                delay = 1;
            }

            //apply any other effecting delays
            foreach (DelaySet effectingDelaySet in _effectingDelaySets)
            {
                delay *= effectingDelaySet.GetDelay(actionType);
            }

            //return the delay
            return delay;
        }

        #endregion

        #region Logic

        /// <summary>
        /// Add a set of delays that effect the delays in this set
        /// </summary>
        public void AddEffectingDelaySet(DelaySet delaySet)
        {
            _effectingDelaySets.Add(delaySet);
        }
        
        /// <summary>
        /// Remove a set of delays that effected the delays in this set
        /// </summary>
        public void RemoveEffectingDelaySet(DelaySet delaySet)
        {
            _effectingDelaySets.Remove(delaySet);
        }
        
        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteObjectList<DelaySet>(_effectingDelaySets);
            writer.WriteInfo(_delaysInfo.InfoSetOwner);
            writer.WriteObject(_gameObject);
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _effectingDelaySets = reader.ReadObjectList<DelaySet>();
            _delaysInfo = reader.ReadInfo<IDelaysInfo>().Delays;
            _gameObject = reader.ReadObject<IHasTraits>();
        }

        public void AfterReadStateV1()
        {
        }
        #endregion
        
    }
}
