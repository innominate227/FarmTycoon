using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{
    public partial class TraitInfo
    {
        private class TraitEffectingRangeInfo
        {
            /// <summary>
            /// Range at which the effect takes place
            /// </summary>
            public Range Range;

            /// <summary>
            /// Id of the trait to check if it is in the range,             
            /// </summary>
            public int TraitId;

            /// <summary>
            /// Effect that occurs when in range
            /// </summary>
            public int Effect;
        }


        /// <summary>
        /// Events that effect the trait, and the amount they effect it by
        /// </summary>
        private Dictionary<ActionOrEventType, int> _eventsThatEffect = new Dictionary<ActionOrEventType, int>();

        /// <summary>
        /// Events that effect the trait, and the amount they effect it by (while the worker has equipment)
        /// </summary>
        private Dictionary<ActionOrEventType, int> _eventsThatEffectWithEquipment = new Dictionary<ActionOrEventType, int>();
        
        /// <summary>
        /// Items that effect the trait, and the ammount they effect it
        /// </summary>
        private Dictionary<ItemTypeInfo, int> _itemsThatEffect = new Dictionary<ItemTypeInfo, int>();
        
        /// <summary>
        /// trait ranges that effect the trait, and the amount they effecy it.        
        /// </summary>
        private List<TraitEffectingRangeInfo> _rangesThatEffect = new List<TraitEffectingRangeInfo>();

        /// <summary>
        /// List of ids of other traits that have somne effect on this trait
        /// </summary>
        private List<int> _otherTraitsThatEffect = new List<int>();
        
        /// <summary>
        /// Cache of determined effects that other traits have at certain values.
        /// First key is other trait name, next key is other trait value, and value is the effect it has
        /// </summary>
        private Dictionary<string, Dictionary<int, int>> _valueEffectCache = new Dictionary<string, Dictionary<int, int>>();


        /// <summary>
        /// Foreach value that can effect the trait, the amount that value effects the trait.
        /// Although this wastes memory because some values will be empty the array lookups are significantly faster than dictionary lookups
        /// </summary>
        private int[] _valuesEffect;

        /// <summary>
        /// Foreach trait id that can effect the trait (other than this trait), and each value that can effect the trait, the amount that value effects the trait.
        /// Although this wastes memory because many trait ids will be empty the array lookups are significantly faster than dictionary lookups
        /// </summary>
        private int[][] _otherTraitValuesEffect;
        
        




        private void ReadEffectingEvent(XmlReader reader)
        {
            ActionOrEventType eventType = ActionOrEventType.Baby;
            int eventEffect = 0;
            string hasEquipmnetState = "ANY";
            if (reader.MoveToAttribute("Event"))
            {
                string eventName = reader.ReadContentAsString();
                eventType = (ActionOrEventType)Enum.Parse(typeof(ActionOrEventType), eventName);
            }
            if (reader.MoveToAttribute("Action"))
            {
                string eventName = reader.ReadContentAsString();
                eventType = (ActionOrEventType)Enum.Parse(typeof(ActionOrEventType), eventName);
            }
            if (reader.MoveToAttribute("Equipmnet"))
            {
                hasEquipmnetState = reader.ReadContentAsString();
            }
            if (reader.MoveToAttribute("Effect"))
            {
                eventEffect = reader.ReadContentAsInt();
            }

            //add to correct dictionary depeding on if it applies only if the worker has quipment, does not have equipmnet, or both
            if (hasEquipmnetState.ToUpper() == "NO" || hasEquipmnetState.ToUpper() == "ANY")
            {
                _eventsThatEffect.Add(eventType, eventEffect);
            }
            if (hasEquipmnetState.ToUpper() == "YES" || hasEquipmnetState.ToUpper() == "ANY")
            {
                _eventsThatEffectWithEquipment.Add(eventType, eventEffect);
            }
        }

        private void ReadEffectingItem(XmlReader reader, FarmData farmInfo)
        {
            ItemTypeInfo itemName = null;
            int itemEffect = 0;
            if (reader.MoveToAttribute("Item"))
            {
                itemName = reader.ReadContentAsItemTypeInfo(farmInfo);
            }
            if (reader.MoveToAttribute("Effect"))
            {
                itemEffect = reader.ReadContentAsInt();
            }
            _itemsThatEffect.Add(itemName, itemEffect);
        }

        private void ReadEffectingRange(XmlReader reader, FarmData farmInfo)
        {
            Range range = new Range(int.MinValue, true, int.MinValue, true);
            int effect = 0;

            //if no trait id is specifed assume it is this same trait
            int traitId = _id;

            if (reader.MoveToAttribute("Trait"))
            {
                traitId = reader.ReadContentAsTraitId(farmInfo);
            }
            if (reader.MoveToAttribute("Range"))
            {
                range = reader.ReadContentAsRange();
            }
            if (reader.MoveToAttribute("Effect"))
            {
                effect = reader.ReadContentAsInt();
            }

            TraitEffectingRangeInfo effectingRangeInfo = new TraitEffectingRangeInfo();
            effectingRangeInfo.TraitId = traitId;
            effectingRangeInfo.Range = range;
            effectingRangeInfo.Effect = effect;

            _rangesThatEffect.Add(effectingRangeInfo);

            //if the effecting trait is not this same trait then add to list of other effecting traits
            if (traitId != _id)
            {                
                _otherTraitsThatEffect.Add(effectingRangeInfo.TraitId);
            }
        }



        /// <summary>
        /// Get the amount an action or event effects the trait,
        /// which can also depend on if the worker is currently on equipmnet or not
        /// </summary>
        public int AmountActionOrEventEffectsTrait(ActionOrEventType eventType, bool onEquipment)
        {
            if (onEquipment)
            {
                if (_eventsThatEffectWithEquipment.ContainsKey(eventType) == false) { return 0; }
                return _eventsThatEffectWithEquipment[eventType];
            }
            else
            {
                if (_eventsThatEffect.ContainsKey(eventType) == false) { return 0; }
                return _eventsThatEffect[eventType];
            }
        }

        /// <summary>
        /// Get the amount an items effects the trait
        /// </summary>
        public int AmountItemEffectsTrait(ItemTypeInfo item)
        {
            if (_itemsThatEffect.ContainsKey(item) == false) { return 0; }
            return _itemsThatEffect[item];
        }

        /// <summary>
        /// For ranges that effect this trait compute the effect at ever legal value for the trait
        /// </summary>
        public void ComputeTraitValueEffects()
        {            
            //determine the effect the trait has on itself at all possible values of the trait
            _valuesEffect = new int[_maximumValue+1];
            for (int value = _minimumValue; value <= _maximumValue; value++)
            {
                _valuesEffect[value] = DetermineEffect(_id, value);
            }


            //find the other traits that effect this trait            
            List<TraitInfo> otherTraitsThatEffect = new List<TraitInfo>();
            foreach (TraitEffectingRangeInfo effectingRange in _rangesThatEffect)
            {
                //make sure its not this trait
                if (effectingRange.TraitId == _id) { continue;  }                

                //search other traits under the parent info
                foreach (TraitInfo otherTraitInfo in _parent.Traits.TraitInfoList)
                {
                    //find the one with the correct id
                    if (otherTraitInfo.Id == effectingRange.TraitId)
                    {
                        //add it to the list of other effecting traits
                        if (otherTraitsThatEffect.Contains(otherTraitInfo) == false)
                        {
                            otherTraitsThatEffect.Add(otherTraitInfo);
                        }
                        break;
                    }
                }
            }


            //find the max id of any other trait the effects this trait
            int maxOtherTraitId = -1;
            foreach (TraitInfo otherTraitThatEffects in otherTraitsThatEffect)
            {
                maxOtherTraitId = Math.Max(maxOtherTraitId, otherTraitThatEffects.Id);
            }

            //create array to hold the other traits the effect
            _otherTraitValuesEffect = new int[maxOtherTraitId+1][];

            //determine the effect any other trait has on this trait at every possible value for the other trait
            foreach (TraitInfo otherTraitThatEffects in otherTraitsThatEffect)
            {
                //create the array of possible values for that trait
                _otherTraitValuesEffect[otherTraitThatEffects.Id] = new int[otherTraitThatEffects.MaximumValue+1];

                //determine the effect the other trait has at each value
                for (int value = otherTraitThatEffects.MinimumValue; value <= otherTraitThatEffects.MaximumValue; value++)
                {
                    _otherTraitValuesEffect[otherTraitThatEffects.Id][value] = DetermineEffect(otherTraitThatEffects.Id, value);
                }
            }
        }
        
        /// <summary>
        /// Determine the effect of a trait with the id passed having the value passed on the value of this trait
        /// </summary>
        private int DetermineEffect(int traitId, int value)
        {
            //look at the trait effecting ranges and find the one that effects the trait in that range
            foreach (TraitEffectingRangeInfo effectingRange in _rangesThatEffect)
            {
                if (effectingRange.TraitId != traitId) { continue; }
                if (effectingRange.Range.IsInRange(value))
                {
                    return effectingRange.Effect;
                }
            }
            return 0;
        }


        /// <summary>
        /// Get the amount the traits current value effects the trait
        /// </summary>
        public int AmountCurrentValueEffectsTrait(int currentValue)
        {
            return _valuesEffect[currentValue];
        }


        /// <summary>
        /// Get the amount the traits current value effects the trait
        /// </summary>
        public int AmountOtherTraitsValueEffectsTrait(int otherTraitId, int otherTraitValue)
        {
            return _otherTraitValuesEffect[otherTraitId][otherTraitValue];
        }


        /// <summary>
        /// List of ids of other traits that have some effect on this traits value
        /// </summary>
        public List<int> OtherTraitsThatEffect
        {
            get { return _otherTraitsThatEffect; }
        }

        /// <summary>
        /// Returns true if the value of the trait can be effected by time passing (even when no items are sprayed)
        /// In otherwords the triat is has a range in wich will apply an effect each day
        /// </summary>
        public bool TraitValueChangesWithTime
        {
            get { return _rangesThatEffect.Count > 0; }
        }


    }
}
