using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class TraitConditionInfo : ConditionInfo
    {
        /// <summary>
        /// Range at which the condition is met
        /// </summary>
        private Range _range;

        /// <summary>
        /// Id of the trait to check if it is in the range
        /// </summary>
        private int _traitId;

        /// <summary>
        /// Create a TraitConditionInfo
        /// </summary>
        public TraitConditionInfo(Range range, int traitId)
        {
            _range = range;
            _traitId = traitId;
        }
        
        /// <summary>
        /// Range at which the condition is met
        /// </summary>
        public Range Range
        {
            get { return _range; }
        }

        /// <summary>
        /// Id of the trait to check if it is in the range
        /// </summary>
        public int TraitId
        {
            get { return _traitId; }
        }


        /// <summary>
        /// Return if the gameobject passed meets the condition
        /// </summary>
        public override bool ConditionMet(IGameObject gameObject)
        {
            //if no traits we cant meet the condition
            if (gameObject is IHasTraits == false) { return false; }

            //get the trait, if the object does not have it cant meet the condition
            int traitValue = (gameObject as IHasTraits).Traits.GetTraitValue(_traitId);
            if (traitValue == -1) { return false; }

            //check the value of the trait
            return _range.IsInRange(traitValue);
        }
    }
}
