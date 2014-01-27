using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace FarmTycoon
{
    public interface IInventoryInfo : IInfo
    {
        /// <summary>
        /// Types of items allowed in the inventory.  All types that share a ItemBaseType in the list passed are allowed.
        /// A value of null indicates all types are allowed
        /// </summary>
        HashSet<ItemTypeInfo> AllowedTypes
        {
            get;
        }


        /// <summary>
        /// Capacity of the inventory in the building
        /// </summary>
        int Capacity
        {
            get;
        }

        /// <summary>
        /// This inventory may allow multiple ItemTypes but once an ItemType is in it only other ItemTypes with the same ItemTypeInfo can be added.
        /// This is used for pastures where only 1 type of animal is allowed
        /// </summary>
        bool OneBaseTypeInventory
        {
            get;
        }

        /// <summary>
        /// If true when items are added to this inventory they should be added to the global players items list
        /// </summary>
        bool UpdatePlayerItemsList
        {
            get;
        }

        /// <summary>
        /// If true when items are added to this inventory they should be added to the global players sellable items list.
        /// </summary>
        bool UpdatePlayerSellableItemsList
        {
            get;
        }


    }
}
