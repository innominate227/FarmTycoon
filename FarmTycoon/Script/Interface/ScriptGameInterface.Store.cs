using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    public partial class ScriptGameInterface
    {

        public void SetStoreAmount(string itemName, int count)
        {
            if (count < 0) { count = 0; }
            ItemType item = GameState.Current.ItemPool.GetItemType(itemName);
            GameState.Current.StoreStock.SetItemCount(item, count);
        }
        public void SetStoreAmount(string itemName, int quality, int count)
        {
            if (count < 0) { count = 0; }
            ItemType item = GameState.Current.ItemPool.GetItemType(itemName, quality);
            GameState.Current.StoreStock.SetItemCount(item, count);
        }
        public void SetStoreAmount(string itemName, string uniqueName, int count)
        {
            if (count < 0) { count = 0; }
            if (count > 1) { count = 1; }
            ItemType item = GameState.Current.ItemPool.GetItemType(itemName, uniqueName);
            GameState.Current.StoreStock.SetItemCount(item, count);
        }


        public int GetStoreAmount(string itemName)
        {
            ItemType item = GameState.Current.ItemPool.GetItemType(itemName);
            return GameState.Current.StoreStock.GetItemCount(item);
        }
        public int GetStoreAmount(string itemName, int quality)
        {
            ItemType item = GameState.Current.ItemPool.GetItemType(itemName, quality);
            return GameState.Current.StoreStock.GetItemCount(item);
        }
        public int GetStoreAmount(string itemName, string uniqueName)
        {
            ItemType item = GameState.Current.ItemPool.GetItemType(itemName, uniqueName);
            return GameState.Current.StoreStock.GetItemCount(item);
        }
        public int GetStoreAmountAll(string itemName)
        {
            ItemTypeInfo itemTypeInfo = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);
            if (itemTypeInfo.ItemTypeRelation == ItemTypeRelation.One)
            {
                return GetStoreAmount(itemName);
            }
            else if (itemTypeInfo.ItemTypeRelation == ItemTypeRelation.Qualities)
            {
                int sum = 0;
                for (int quality = 0; quality < 10; quality++)
                {
                    sum += GetStoreAmount(itemName, quality);
                }
                return sum;
            }
            else if (itemTypeInfo.ItemTypeRelation == ItemTypeRelation.Many)
            {
                int sum = 0;
                foreach (ItemType itemType in GameState.Current.StoreStock.ItemTypes)
                {
                    if (itemType.BaseType == itemTypeInfo)
                    {
                        sum += GameState.Current.StoreStock.GetItemCount(itemType);
                    }
                }
                return sum;
            }
            else
            {
                Debug.Assert(false);
                return -1;
            }
        }


        public int AdjustStoreAmount(string itemName, int adjustment)
        {
            SetStoreAmount(itemName, GetStoreAmount(itemName) + adjustment);
            return GetStoreAmount(itemName);
        }
        public int AdjustStoreAmount(string itemName, int quality, int adjustment)
        {
            SetStoreAmount(itemName, GetStoreAmount(itemName, quality) + adjustment);
            return GetStoreAmount(itemName, quality);
        }
        public int AdjustStoreAmount(string itemName, string uniqueName, int adjustment)
        {
            SetStoreAmount(itemName, GetStoreAmount(itemName, uniqueName) + adjustment);
            return GetStoreAmount(itemName, uniqueName);
        }



        
        public void SetItemQuality(string itemName, string uniqueName, int quality)
        {
            ItemType item = GameState.Current.ItemPool.GetItemType(itemName, uniqueName);
            item.Quality = quality;
        }
        public void SetItemAge(string itemName, string uniqueName, int age)
        {
            ItemType item = GameState.Current.ItemPool.GetItemType(itemName, uniqueName);
            item.Age = age;
        }


    }
}
