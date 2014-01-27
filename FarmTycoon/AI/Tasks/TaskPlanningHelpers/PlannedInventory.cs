using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{

    /// <summary>
    /// Used by the task item planner, to keep track of what items will be taken/put into each inventory during the task.
    /// </summary>
    public class ExpectedInventory
    {
        /// <summary>
        /// The inventory this is for
        /// </summary>
        private Inventory _inventory;

        /// <summary>
        /// List of items that are expected to be added to this inventory
        /// </summary>
        private ItemList _expectedAdditions = new ItemList();
        
        /// <summary>
        /// List of items that are expected to be removed from this inventory
        /// </summary>
        private ItemList _expectedRemovals = new ItemList();
                
        /// <summary>
        /// The size of all the items that are expected to be added to the inventory minus
        /// the size of all the items that are expected to be removed from the inventory
        /// </summary>
        private int _additionalFillLevel = 0;

        /// <summary>
        /// Create a new expected inventory
        /// </summary>
        public ExpectedInventory(Inventory inventory)
        {
            _inventory = inventory;
        }

        /// <summary>
        /// Adjust the planned inventory by planning to add an amount of item of the type specified
        /// </summary>
        public void PlanToAdd(ItemType itemType, int amount)
        {
            _expectedAdditions.IncreaseItemCount(itemType, amount);
            _additionalFillLevel += (amount * itemType.Size);
        }

        /// <summary>
        /// Adjust the planned inventory by planning to remove an amount of items of the type specified
        /// </summary>
        public void PlanToRemove(ItemType itemType, int amount)
        {
            _expectedRemovals.IncreaseItemCount(itemType, amount);
            _additionalFillLevel -= (amount * itemType.Size);
        }


        
        /// <summary>
        /// Get the quantity of a type of item that will fit into the planned inventory
        /// </summary>
        public int AmountThatWillFit(ItemType type)
        {            
            if (TypeCanBeAdded(type) == false) { return 0; }            
            return FreeSpace / type.Size;
        }
        
        /// <summary>
        /// Get the quantity of a type of item that will fit into the planned inventory after taking reserved capacity into account
        /// </summary>
        public int AmountThatWillFitAfterReservedCapacity(ItemType type)
        {
            if (TypeCanBeAdded(type) == false) { return 0; }              
            return FreeSpaceAfterReservedCapacity / type.Size;
        }


        //return if any amount of the type passed can be added to the inventory
        private bool TypeCanBeAdded(ItemType type)
        {
            if (_inventory.InventoryInfo == null) { return true; }

            //if its a one type inventory make sure what is being added is that same base type as what is already in there
            if (_inventory.InventoryInfo.OneBaseTypeInventory)
            {
                //look at item type in the inventory, and check if any have a count > 0                
                foreach (ItemType itemTypeToConsider in _inventory.Types)
                {
                    //if we expect to be some of that type, then make sure its the same type as we are going to add
                    int amountOfItem = GetTypeCount(itemTypeToConsider);
                    if (amountOfItem > 0)
                    {
                        //it a differernt base type than what is in there we are not allowed to add it
                        if (itemTypeToConsider.BaseType != type.BaseType)
                        {
                            return false;
                        }

                        //if it was not a different base type, we dont need to check every item in there because the fact that its a OneBaseTypeInventory means every item in there must be the same base type
                        break;
                    }
                }

                //if there was nothing in the invenotry yet look at items we are planning to add, and check if any have a count > 0.
                if (_inventory.Types.Count == 0)
                {
                    foreach (ItemType itemTypeToConsider in _expectedAdditions.ItemTypes)
                    {
                        //if we expect to be some of that type, then make sure its the same type as we are going to add
                        int amountOfItem = GetTypeCount(itemTypeToConsider);
                        if (amountOfItem > 0)
                        {
                            //it a differernt base type than what is in there we are not allowed to add it
                            if (itemTypeToConsider.BaseType != type.BaseType)
                            {
                                return false;
                            }

                            //if it was not a different base type, we dont need to check every item in there because the fact that its a OneBaseTypeInventory means every item in there must be the same base type
                            break;
                        }
                    }
                }
            }

            //see if that class of item is allowed in this invetory            
            return _inventory.InventoryInfo.AllowedTypes.Contains(type.BaseType);
        }

        
        /// <summary>
        /// The remaining capacity of the planned invetory
        /// </summary>
        public int FreeSpace
        {
            get { return _inventory.FreeSpace - _additionalFillLevel; }
        }

        /// <summary>
        /// The remaining capacity of the planned invetory after reserved space
        /// </summary>
        public int FreeSpaceAfterReservedCapacity
        {
            get { return _inventory.FreeSpaceAfterReservedCapacity - _additionalFillLevel; }
        }


        /// <summary>
        /// Get how many of one type there is in the planned inventory
        /// </summary>
        public int GetTypeCount(ItemType type)
        {
            int amountInInventory = _inventory.GetTypeCount(type);
            int amountPlannedToAdd = _expectedAdditions.GetItemCount(type);
            int amountPlannedToRemove = _expectedRemovals.GetItemCount(type);
            return amountInInventory + amountPlannedToAdd - amountPlannedToRemove;
        }


        /// <summary>
        /// Get how many of one type there is in the planned inventory that is not reserved
        /// </summary>
        public int GetTypeCountThatsFree(ItemType type)
        {
            int amountFreeInInventory = _inventory.GetTypeCountThatsFree(type);
            int amountPlannedToAdd = _expectedAdditions.GetItemCount(type);
            int amountPlannedToRemove = _expectedRemovals.GetItemCount(type);
            return amountFreeInInventory + amountPlannedToAdd - amountPlannedToRemove;
        }


        /// <summary>
        /// Get a list of the types that are expected to be available in the planned inventory
        /// </summary>
        public List<ItemType> TypesAvailable()
        {
            List<ItemType> typesAvailable = new List<ItemType>();

            //add types in the inventory that are still expected to be there
            foreach(ItemType itemType in _inventory.Types)
            {
                if (GetTypeCountThatsFree(itemType) > 0)
                {
                    typesAvailable.Add(itemType);
                }
            }

            //if there are expected additions add them, so long as they are still expected to be there
            foreach (ItemType itemType in _expectedAdditions.ItemTypes)
            {
                if (GetTypeCountThatsFree(itemType) > 0 && typesAvailable.Contains(itemType) == false)
                {
                    typesAvailable.Add(itemType);
                }
            }
            
            return typesAvailable;
        }

    }
}
