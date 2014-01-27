using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    public class TaskItemDivider
    {
        
        /// <summary>
        /// Items left to be assigned to a worker to get
        /// </summary>
        private ItemList _itemsLeftToGet = new ItemList();

        /// <summary>
        /// Equipment left to be assigned to a worker to get
        /// </summary>
        private ItemList _equipmentLeftToGet = new ItemList();

        /// <summary>
        /// Inventory size that determines how many items a worker can take on a trip.
        /// Defaults to workers base inventory size.  Should be adjusted if equipment will effect the inventory size.
        /// </summary>
        private int _inventorySize;
        

        /// <summary>
        /// Create a new ItemDivider to divide out the responsibility of getting all the items in the list passed
        /// </summary>
        public TaskItemDivider(ItemList allItems)
        {
            //default inventory size as worker base capacity
            _inventorySize = (FarmData.Current.GetInfo(WorkerInfo.UNIQUE_NAME) as WorkerInfo).Capacity;

            //create a list of the normal items that still need to be gotten and the equipment that still needs to be gotten            
            foreach (ItemType itemType in allItems.ItemTypes)
            {
                //ignore items that we are set to buy zero of
                if (allItems.GetItemCount(itemType) == 0) { continue; }

                //determine if the item is for a peice of equipment
                bool isEquipment = (FarmData.Current.GetEquipmentInfoForItemInfo(itemType.BaseType) != null);

                //add to appropriate list depending on if its equipment                
                if (isEquipment)
                {
                    _equipmentLeftToGet.SetItemCount(itemType, allItems.GetItemCount(itemType));
                }
                else
                {
                    _itemsLeftToGet.SetItemCount(itemType, allItems.GetItemCount(itemType));
                }
            }
        }


        /// <summary>
        /// Inventory size that determines how many items a worker can take on a trip.
        /// Defaults to workers base inventory size.  Should be adjusted if equipment will effect the inventory size.
        /// </summary>
        public int InventorySize
        {
            get { return _inventorySize; }
            set { _inventorySize = value; }
        }


        /// <summary>
        /// Return the next load of items that a worker should get from the list of all items.
        /// Each load will be such that a worker should be able to manage the load in one trip.
        /// When there is nothing left to be gotten null is returned.
        /// </summary>
        public ItemList NextLoad()
        {   
            if (_itemsLeftToGet.ItemTypes.Count > 0)
            {
                //if there are normal items left to get have the worker get normal items
                return NextNormalItemLoad();
            }
            else if (_equipmentLeftToGet.ItemTypes.Count > 0)
            {
                //if there is equipmemt left to get have the worker get equipment
                return NextEquipmentLoad();
            }
            else
            {
                //return null to signify were done
                return null;
            }
        }


        private ItemList NextNormalItemLoad()
        {
            //create a list for what to get this load.  The worker will get as much as its inventory size allows
            ItemList thisLoad = new ItemList();            
            int spaceLeft = _inventorySize;

            //foreach type that still needs to be gotten
            foreach (ItemType itemType in _itemsLeftToGet.ItemTypes)
            {
                //get the size of the item
                int itemSize = itemType.Size;
                        
                //get the amount of this item that still needs to be gotten
                int amountToGet = _itemsLeftToGet.GetItemCount(itemType);

                //get the amount the worker can fit (or if more than the amount left get the amount left)
                int amountCanFit = spaceLeft / itemSize;
                if (amountCanFit > amountToGet) { amountCanFit = amountToGet; }

                //have the worker get that much
                if (amountCanFit > 0)
                {
                    //add to load
                    thisLoad.AddItem(itemType);
                    thisLoad.SetItemCount(itemType, amountCanFit);

                    //reduce space worker has left
                    spaceLeft -= (itemSize * amountCanFit);

                    //remove that amount from left to get
                    _itemsLeftToGet.DecreaseItemCount(itemType, amountCanFit);
                }

                //remove the item from leftToGet if all have been gotten
                if (_itemsLeftToGet.GetItemCount(itemType) == 0)
                {
                    _itemsLeftToGet.RemoveItem(itemType);
                }

                //if the worker cant carry any more stop looking for items for him to get
                if (spaceLeft == 0)
                {
                    break;
                }
            }

            return thisLoad; 
        }



        private ItemList NextEquipmentLoad()
        {               
            //each worker will get a tractor, and a tow if possible.
            //they might get just a tractor if no tows left to get, or get just a tow if no tractors left to get.
            ItemType vehicleToGet = null;
            ItemType towToGet = null;
                
            //foreach type of equipmnet that still needs to be gotten
            foreach (ItemType itemType in _equipmentLeftToGet.ItemTypes)
            {
                EquipmentInfo equipmentInfo = FarmData.Current.GetEquipmentInfoForItemInfo(itemType.BaseType);

                if (equipmentInfo.IsVehicle && vehicleToGet == null)
                {
                    vehicleToGet = itemType;
                    _equipmentLeftToGet.DecreaseItemCount(itemType, 1);
                }
                else if (equipmentInfo.IsVehicle == false && towToGet == null)
                {
                    towToGet = itemType;
                    _equipmentLeftToGet.DecreaseItemCount(itemType, 1);
                }

                //remove the equipment from leftToGet if all have been gotten
                if (_equipmentLeftToGet.GetItemCount(itemType) == 0)
                {
                    _equipmentLeftToGet.RemoveItem(itemType);
                }
            }

            //make sure we got at least one peice of equipment
            Debug.Assert(vehicleToGet != null || towToGet != null);
            
            //create a list with the vechicle and/or tow to get
            ItemList equipmentToGet = new ItemList();
            if (vehicleToGet != null)
            {
                equipmentToGet.SetItemCount(vehicleToGet, 1);
            }
            if (towToGet != null)
            {
                equipmentToGet.SetItemCount(towToGet, 1);
            }
                        
            return equipmentToGet;
        }

    }
}
