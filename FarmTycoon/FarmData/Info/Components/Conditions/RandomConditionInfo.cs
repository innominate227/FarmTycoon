using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class RandomConditionInfo : ConditionInfo
    {
        /// <summary>
        /// Range at which the condition is met
        /// </summary>
        private Range _range;
        
        /// <summary>
        /// Create a RandomConditionInfo
        /// </summary>
        public RandomConditionInfo(Range range)
        {
            _range = range;
        }
        
        /// <summary>
        /// Range at which the condition is met
        /// </summary>
        public Range Range
        {
            get { return _range; }
        }
        

        /// <summary>
        /// Return if the gameobject passed meets the condition
        /// </summary>
        public override bool ConditionMet(IGameObject gameObject)
        {
            //get random value
            int random = Program.Game.Random.Next(0, 100);
            
            //check if in range
            return _range.IsInRange(random);
        }
    }
}
