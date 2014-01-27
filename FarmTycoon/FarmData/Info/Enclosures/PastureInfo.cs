using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class PastureInfo : IInventoryInfo, IInfo, IStorageBuildingInfo
    {
        /// <summary>
        /// Unique name for the Pasture info object.
        /// </summary>
        public const string UNIQUE_NAME = "Pasture_Pasture";


        /// <summary>
        /// Types of items allowed in the building
        /// </summary>
        private HashSet<ItemTypeInfo> _allowedTypes = new HashSet<ItemTypeInfo>();


        public PastureInfo(FarmData farmInfo)
        {
            //this is messy, but this gets but Game.FarmData is not yet set so we need to get the farm info from elsewhere
            foreach (ItemTypeInfo itemTypeInfo in farmInfo.GetInfos<ItemTypeInfo>())
            {
                if (itemTypeInfo.HasTag("Animal"))
                {
                    _allowedTypes.Add(itemTypeInfo);
                }
            }
        }

        public HashSet<ItemTypeInfo> AllowedTypes
        {
            get { return _allowedTypes; }
        }

        public int Capacity
        {
            get { return int.MaxValue; }
        }
                
        public bool OneBaseTypeInventory
        {
            get { return true; }
        }

        public string UniqueName
        {
            get { return UNIQUE_NAME; }
        }

        public double GetDelayMultiplier
        {
            get { return 0.0; }
        }

        public double PutDelayMultiplier
        {
            get { return 0.0; }
        }

        /// <summary>
        /// If true when items are added to this inventory they should be added to the global players items list
        /// </summary>
        public bool UpdatePlayerItemsList
        {
            get { return true; }
        }

        /// <summary>
        /// If true when items are added to this inventory they should be added to the global players sellable items list.
        /// </summary>
        public bool UpdatePlayerSellableItemsList
        {
            get { return true; }
        }
    }
}
