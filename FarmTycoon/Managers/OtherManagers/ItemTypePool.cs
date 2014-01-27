using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Manages ItemTypes.
    /// Each ItemType has an ItemTypeInfo which contains information about the item such as size, and description.
    /// Multiple ItemTypes can share the same ItemTypeInfo.  For instance the two ItemTypes "wheat_5" and "wheat_6"
    /// both share the ItemTypeInfo for "wheat".  They differ because the two different types have different Quality values.
    /// For many ItemTypes the only atrubute unique to the ItemType, and not part of the ItemTypeInfo is the Quality.
    /// However some ItemTypes are more compilcated.  For instance each Cow has its own unique ItemType "cow_1234" and "cow_1235"
    /// for cows you will only ever have one "cow_1234" in your inventory.
    /// 
    /// The reasoning for this is memory saving.  Having an instance of a "wheat_5" ItemType for each grain of wheat you have would consume alot
    /// of memory.  Once a unit of wheat is created with a certain quality it remains that quality.  But for something like a cow its quality changes all 
    /// the time if we were to treat cows like wheat when a cows quality changed we would have to change the ItemType associated with the cow, and the inventory
    /// of the pasture the cow was in would be chaning all the time.  This means more memory, but for something like a cow you will generally have alot less of them
    /// than you have wheat.
    /// 
    /// Also from the store point of view it makes since.  If your going to buy wheat you might buy 100 units of quality 5 wheat.  But if you are going to buy a
    /// cow you are not going to buy 10 quality 5 cows.  You are going to buy a 360 day old cow, with quality 5 + a 180 day old cow with quality 5 + some other cows.
    /// In other words each cow is more unique than each grain of wheat so they demand their own unique ItemType.
    /// </summary>
    public partial class ItemTypePool : ISavable
    {
        #region Member Vars

        /// <summary>
        /// Item types that have been created
        /// </summary>
        private Dictionary<string, ItemType> _itemTypes = new Dictionary<string, ItemType>();

        #endregion

        #region Logic


        /// <summary>
        /// Get an new ItemType for an item where each animal/object has it own type
        /// </summary>
        public ItemType GetNewItemType(string itemName)
        {
            string uniqueName = Guid.NewGuid().ToString().Replace("-", "");
            return GetItemType(itemName, uniqueName);
        }

        /// <summary>
        /// Get an ItemType for an item where each animal/object has it own type
        /// </summary>
        public ItemType GetItemType(string itemName, string uniqueName)
        {
            string fullItemName = itemName + "_" + uniqueName;

            if (_itemTypes.ContainsKey(fullItemName) == false)
            {
                //get the ItemTypeInfo for the type, make sure its an type info with a one-one realtion with the item type
                ItemTypeInfo itemTypeInfo = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);
                Debug.Assert(itemTypeInfo.ItemTypeRelation == ItemTypeRelation.Many);

                //create the item type (we still add to the dictionary so that the type will be saved when we save)
                ItemType itemType = new ItemType();
                itemType.Setup(itemTypeInfo, uniqueName);
                _itemTypes.Add(fullItemName, itemType);
            }
            
            return _itemTypes[fullItemName];
        }
           
        /// <summary>
        /// Get an ItemType for an item with 10 quality level with the name and quality passed.
        /// </summary>
        public ItemType GetItemType(string itemName, int quality)
        {
            string itemSubName = quality.ToString();
            string fullItemName = itemName + "_" + itemSubName;

            if (_itemTypes.ContainsKey(fullItemName) == false)
            {
                //get the ItemTypeInfo for the type, make sure its an type info with a one-one realtion with the item type
                ItemTypeInfo itemTypeInfo = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);
                Debug.Assert(itemTypeInfo.ItemTypeRelation == ItemTypeRelation.Qualities);

                //create the item type
                ItemType itemType = new ItemType();
                itemType.Setup(itemTypeInfo, itemSubName);
                itemType.Quality = quality;
                _itemTypes.Add(fullItemName, itemType);
            }

            return _itemTypes[fullItemName];
        }

        /// <summary>
        /// Get an ItemType for an item that does not have multiple qualities.
        /// </summary>
        public ItemType GetItemType(string itemName)
        {
            if (_itemTypes.ContainsKey(itemName) == false)
            {
                //get the ItemTypeInfo for the type, make sure its an type info with a one-one realtion with the item type
                ItemTypeInfo itemTypeInfo = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);
                Debug.Assert(itemTypeInfo.ItemTypeRelation == ItemTypeRelation.One);   
                
                //create the item type
                ItemType itemType = new ItemType();
                itemType.Setup(itemTypeInfo, "");
                _itemTypes.Add(itemName, itemType);
            }

            return _itemTypes[itemName];
        }
        
        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteInt(_itemTypes.Count);
            foreach (string key in _itemTypes.Keys)
            {
                writer.WriteString(key);
                writer.WriteObject(_itemTypes[key]);
            }            
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            int count = reader.ReadInt();
            for(int num=0;num<count;num++)
            {
                string key = reader.ReadString();
                ItemType item = reader.ReadObject<ItemType>();
                _itemTypes.Add(key, item);
            }
        }

        public void AfterReadStateV1()
        {
            //delete items where the base type has been deleted
            foreach (string itemTypeKey in _itemTypes.Keys.ToArray())
            {
                if (_itemTypes[itemTypeKey].BaseType == null)
                {
                    _itemTypes.Remove(itemTypeKey);
                }
            }
        }
        #endregion
    }
}
