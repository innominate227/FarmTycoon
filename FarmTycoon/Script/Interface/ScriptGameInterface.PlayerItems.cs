using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    public partial class ScriptGameInterface
    {
        

        public int GetPlayerTotalAmount(string itemName)
        {
            ItemType item = GameState.Current.ItemPool.GetItemType(itemName);
            return GameState.Current.PlayersItemsList.GetItemCount(item);
        }
        public int GetPlayerTotalAmount(string itemName, int quality)
        {
            ItemType item = GameState.Current.ItemPool.GetItemType(itemName, quality);
            return GameState.Current.PlayersItemsList.GetItemCount(item);
        }
        public int GetPlayerTotalAmount(string itemName, string uniqueName)
        {
            ItemType item = GameState.Current.ItemPool.GetItemType(itemName, uniqueName);
            return GameState.Current.PlayersItemsList.GetItemCount(item);
        }
        public int GetPlayerTotalAmountAll(string itemName)
        {
            ItemTypeInfo itemTypeInfo = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);
            if (itemTypeInfo.ItemTypeRelation == ItemTypeRelation.One)
            {
                return GetPlayerTotalAmount(itemName);
            }
            else if (itemTypeInfo.ItemTypeRelation == ItemTypeRelation.Qualities)
            {
                int sum = 0;
                for (int quality = 0; quality < 10; quality++)
                {
                    sum += GetPlayerTotalAmount(itemName, quality);
                }
                return sum;
            }
            else if (itemTypeInfo.ItemTypeRelation == ItemTypeRelation.Many)
            {
                int sum = 0;
                foreach (ItemType itemType in GameState.Current.PlayersItemsList.ItemTypes)
                {
                    if (itemType.BaseType == itemTypeInfo)
                    {
                        sum += GameState.Current.PlayersItemsList.GetItemCount(itemType);
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



        public int GetPlayerAmount(string itemName)
        {
            ItemType item = GameState.Current.ItemPool.GetItemType(itemName);
            return GameState.Current.PlayersSellableItemsList.GetItemCount(item);
        }
        public int GetPlayerAmount(string itemName, int quality)
        {
            ItemType item = GameState.Current.ItemPool.GetItemType(itemName, quality);
            return GameState.Current.PlayersSellableItemsList.GetItemCount(item);
        }
        public int GetPlayerAmount(string itemName, string uniqueName)
        {
            ItemType item = GameState.Current.ItemPool.GetItemType(itemName, uniqueName);
            return GameState.Current.PlayersSellableItemsList.GetItemCount(item);
        }
        public int GetPlayerAmountAll(string itemName)
        {
            ItemTypeInfo itemTypeInfo = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);
            if (itemTypeInfo.ItemTypeRelation == ItemTypeRelation.One)
            {
                return GetPlayerAmount(itemName);
            }
            else if (itemTypeInfo.ItemTypeRelation == ItemTypeRelation.Qualities)
            {
                int sum = 0;
                for (int quality = 0; quality < 10; quality++)
                {
                    sum += GetPlayerAmount(itemName, quality);
                }
                return sum;
            }
            else if (itemTypeInfo.ItemTypeRelation == ItemTypeRelation.Many)
            {
                int sum = 0;
                foreach (ItemType itemType in GameState.Current.PlayersSellableItemsList.ItemTypes)
                {
                    if (itemType.BaseType == itemTypeInfo)
                    {
                        sum += GameState.Current.PlayersSellableItemsList.GetItemCount(itemType);
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


        public int AdjustPlayerAmount(string itemName, int adjustment)
        {
            ItemType item = GameState.Current.ItemPool.GetItemType(itemName);
            AdjustPlayerAmount(item, adjustment);
            return GetPlayerAmount(itemName);
        }
        public int AdjustPlayerAmount(string itemName, int quality, int adjustment)
        {
            ItemType item = GameState.Current.ItemPool.GetItemType(itemName, quality);
            AdjustPlayerAmount(item, adjustment);
            return GetPlayerAmount(itemName, quality);
        }
        public int AdjustPlayerAmount(string itemName, string uniqueName, int adjustment)
        {
            //can only adjust by up 1 or down 1 for uniqye items
            if (adjustment > 1) { adjustment = 1; }
            if (adjustment < -1) { adjustment = -1; }
            
            //if we have 1 now and we are going to add one we cant do that            
            int amountPlayerHasNow = GetPlayerAmount(itemName, uniqueName);
            if (amountPlayerHasNow == 1 && adjustment == 1) 
            {
                return amountPlayerHasNow;
            }
            
            //adjust the item and return the new amount
            ItemType item = GameState.Current.ItemPool.GetItemType(itemName, uniqueName);
            AdjustPlayerAmount(item, adjustment);
            return GetPlayerAmount(itemName, uniqueName);
        }
        public int AdjustPlayerAmountAll(string itemName, int adjustment)
        {
            ItemTypeInfo itemTypeInfo = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);
            if (itemTypeInfo.ItemTypeRelation == ItemTypeRelation.One)
            {
                return AdjustPlayerAmount(itemName, adjustment);
            }
            else if (itemTypeInfo.ItemTypeRelation == ItemTypeRelation.Qualities)
            {
                if (adjustment < 0)
                {
                    //amount we want to have
                    int amountNow = GetPlayerAmountAll(itemName);
                    int desiredAmount = amountNow + adjustment;

                    for (int quality = 0; quality < 10; quality++)
                    {
                        //adjust the amount at this quality by the amount will still need to adjust
                        int adjustmentReamining = desiredAmount - amountNow;
                        AdjustPlayerAmount(itemName, quality, adjustment);

                        //see if we have the reight amount now, if so we are done
                        amountNow = GetPlayerAmountAll(itemName);
                        if (amountNow == desiredAmount) { break; }
                    }                    
                }
                else if (adjustment > 0)
                {
                    //need to increase the amount, just increase it by increase the amount at the mid quality level
                    AdjustPlayerAmount(itemName, 5, adjustment);
                }
                return GetPlayerAmountAll(itemName);
            }
            else if (itemTypeInfo.ItemTypeRelation == ItemTypeRelation.Many)
            {
                if (adjustment < 0)
                {
                    int leftToRemove = -1 * adjustment;
                    //look for sellable items of that type
                    foreach (ItemType itemType in GameState.Current.PlayersSellableItemsList.ItemTypes)
                    {
                        if (itemType.BaseType == itemTypeInfo)
                        {
                            //remove the one of that item, break if we are done now
                            AdjustPlayerAmount(itemType, -1);
                            leftToRemove--;
                            if (leftToRemove == 0) { break; }
                        }
                    }
                }
                else if (adjustment > 0)
                {
                    //create new items for that type and add them
                    for (int add = 0; add < adjustment; add++)
                    {
                        ItemType newItemType = GameState.Current.ItemPool.GetNewItemType(itemName);
                        AdjustPlayerAmount(newItemType, 1);
                    }
                }
                return GetPlayerAmountAll(itemName);
            }
            else
            {
                Debug.Assert(false);
                return -1;
            }
        }
        private void AdjustPlayerAmount(ItemType itemType, int adjustment)
        {
            if (adjustment < 0)
            {
                int amountLeftToRemove = adjustment;
                foreach (IStorageBuilding building in GameState.Current.MasterObjectList.FindAll<IStorageBuilding>())
                {
                    //only want to take from storeage buildings and pastures
                    if (building is StorageBuilding == false && building is Pasture == false) { continue; }

                    //get the amount available, remove all of it unless we dont need to remove that much more
                    int amountAvailable = building.Inventory.GetTypeCountThatsFree(itemType);
                    int amountToRemove = amountAvailable;
                    if (amountToRemove > amountLeftToRemove)
                    {
                        amountToRemove = amountLeftToRemove;
                    }

                    //remove that much
                    building.Inventory.RemoveFromInvetory(itemType, amountToRemove);

                    //if item had an object associated delete the object
                    if (itemType.ItemObject != null)
                    {
                        itemType.ItemObject.Delete();
                    }
                    
                    //we need that must less now, if we dont need any more we are done
                    amountLeftToRemove -= amountToRemove;
                    if (amountLeftToRemove == 0) { break; }
                }
            }
            else if (adjustment > 0)
            {
                int amountLeftToAdd = -1 * adjustment;
                foreach (IStorageBuilding building in GameState.Current.MasterObjectList.FindAll<IStorageBuilding>())
                {
                    //only want to add to storeage buildings and pastures
                    if (building is StorageBuilding == false && building is Pasture == false) { continue; }

                    //get the amount there is room for, fill all of it unless we dont need to fill that much more
                    int spaceAvailable = building.Inventory.AmountThatWillFitAfterReservedCapacity(itemType);
                    int amountToAdd = spaceAvailable;
                    if (amountToAdd > amountLeftToAdd)
                    {
                        amountToAdd = amountLeftToAdd;
                    }

                    //if the item type has an assoicated object we need to create it before adding to inventory (this will do nothing if it does not have an associated object)
                    itemType.CreateAssociatedObject(building.LocationOn);

                    //add that much
                    building.Inventory.AddToInvetory(itemType, amountToAdd);
                    
                    //we need to add that much less now, if we dont need any more we are done
                    amountLeftToAdd -= amountToAdd;
                    if (amountLeftToAdd == 0) { break; }
                }
            }
        }



        public void SetPlayerAmount(string itemName, int count)
        {
            if (count < 0) { count = 0; }
            int currentCount = GetPlayerAmount(itemName);
            int adjustment = count - currentCount;
            AdjustPlayerAmount(itemName, adjustment);
        }
        public void SetPlayerAmount(string itemName, int quality, int count)
        {
            if (count < 0) { count = 0; }
            int currentCount = GetPlayerAmount(itemName, quality);
            int adjustment = count - currentCount;
            AdjustPlayerAmount(itemName,quality, adjustment);
        }
        public void SetPlayerAmount(string itemName, string uniqueName, int count)
        {
            if (count < 0) { count = 0; }
            if (count > 1) { count = 1; }
            int currentCount = GetPlayerAmount(itemName, uniqueName);
            int adjustment = count - currentCount;
            AdjustPlayerAmount(itemName, uniqueName, adjustment);
        }
        public void SetPlayerAmountAll(string itemName, int count)
        {
            if (count < 0) { count = 0; }
            int currentCount = GetPlayerAmountAll(itemName);
            int adjustment = count - currentCount;
            AdjustPlayerAmountAll(itemName, adjustment);
        }



        public int GetPlayerRoomForItem(string itemName)
        {
            ItemTypeInfo itemTypeInfo = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);
            int totalSpaceAvailable = 0;
            foreach (IStorageBuilding building in GameState.Current.MasterObjectList.FindAll<IStorageBuilding>())
            {
                //only want to look at storeage buildings and pastures
                if (building is StorageBuilding == false && building is Pasture == false) { continue; }

                //get the amount there is room for, add to total
                totalSpaceAvailable += building.Inventory.AmountThatWillFitAfterReservedCapacity(itemTypeInfo);                
            }
            return totalSpaceAvailable;
        }



    }
}
