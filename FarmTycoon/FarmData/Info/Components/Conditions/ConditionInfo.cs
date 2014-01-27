using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{
    public abstract class ConditionInfo
    {
        /// <summary>
        /// Return if the game object meets the condition
        /// </summary>
        public abstract bool ConditionMet(IGameObject gameObject);


        /// <summary>
        /// Read in a Condition
        /// </summary>
        public static ConditionInfo ReadCondition(XmlReader reader, FarmData farmInfo)
        {
            Range range = new Range(int.MinValue, true, int.MaxValue, true);
            if (reader.MoveToAttribute("Range"))
            {
                range = reader.ReadContentAsRange();
            }
            if (reader.MoveToAttribute("Trait"))
            {
                int traitId = reader.ReadContentAsTraitId(farmInfo);
                return new TraitConditionInfo(range, traitId);
            }
            if (reader.MoveToAttribute("ItemTag"))
            {
                string tag = reader.ReadContentAsString();
                return new InventoryConditionInfo(range, tag);
            }
            if (reader.MoveToAttribute("Random"))
            {
                return new RandomConditionInfo(range);
            }
            throw new Exception("Condition must specify Trait, ItemTag or Random.");
        }

    }
}
