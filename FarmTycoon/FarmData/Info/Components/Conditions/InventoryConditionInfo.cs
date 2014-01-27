using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class InventoryConditionInfo : ConditionInfo
    {
        /// <summary>
        /// Range at which the condition is met
        /// </summary>
        private Range _range;

        /// <summary>
        /// Name of the tag to look for on the items in the inventory
        /// </summary>
        private string _tag;

        /// <summary>
        /// Create a InventoryConditionInfo
        /// </summary>
        public InventoryConditionInfo(Range range, string tag)
        {
            _range = range;
            _tag = tag;
        }
        
        /// <summary>
        /// Range at which the condition is met
        /// </summary>
        public Range Range
        {
            get { return _range; }
        }

        /// <summary>
        /// Name of the tag to look for on the items in the inventory
        /// </summary>
        public string Tag
        {
            get { return _tag; }
        }


        /// <summary>
        /// Return if the gameobject passed meets the condition
        /// </summary>
        public override bool ConditionMet(IGameObject gameObject)
        {
            //if no inventory we cant meet the condition
            if (gameObject is IHasInventory == false) { return false; }
            Inventory inventory = (gameObject as IHasInventory).Inventory;

            //get the trait, if the object does not have it cant meet the condition
            int itemCountWithTag = 0;            
            foreach (ItemType itemType in inventory.Types)
            {
                if (itemType.HasTag(_tag))
                {
                    itemCountWithTag += inventory.GetTypeCount(itemType);
                }
            }
            

            //check the value is in the range
            return _range.IsInRange(itemCountWithTag);
        }
    }
}
