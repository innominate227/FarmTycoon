using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// An Inventory is a set of items held by a building, worker or other game object.
    /// Some inventories are limited as to what items they can hold, and how much room they have to hold items.
    /// In order to ensure a worker will be able to place items into a buildings inventroy, the inventory allows workers to reserve space in the inventory.
    /// Also in order to ensure that when a worker arrives at a building with an inventory the items he wants are present, the inventory allows workers to reserve items in the inventory.
    /// </summary>
    public class Inventory : ISavable
    {
        #region Events

        /// <summary>
        /// Raised when the items that are reserved changeds
        /// </summary>
        public event Action ReservedItemsChanged;

        /// <summary>
        /// Raised when space that is reserved changes
        /// </summary>
        public event Action ReservedSpaceChanged;

        #endregion

        #region MemberVars

        /// <summary>
        /// Inventory info tells the types allowed in the invetory, and the capacity of the inventory        
        /// </summary>
        private IInventoryInfo _inventoryInfo;

        /// <summary>
        /// List of items in the inventory
        /// </summary>
        private ItemList _items = new ItemList();
        
        /// <summary>
        /// The space of the invetory that is reserved by some worker
        /// </summary>
        private int _spaceReserved;
        
        /// <summary>
        /// The space in the inventory that is filled with items
        /// </summary>
        private int _spaceFilled;
        
        /// <summary>
        /// Multiplier applied to the inventories capacity
        /// </summary>
        private double _capacityMultiplier = 1.0;

        /// <summary>
        /// constant value added to the capacity of an inveotry after the multiplier is applied
        /// </summary>
        private int _extraCapacity = 0;

        /// <summary>
        /// Items reserved by a worker. the items type and the amount reserved
        /// </summary>
        private Dictionary<ItemType, int> _reserved = new Dictionary<ItemType, int>();

        /// <summary>
        /// Capacity for reservered by a worker. the items type and the number of that items that will be added.
        /// </summary>
        private Dictionary<ItemType, int> _capcityReservedFor = new Dictionary<ItemType, int>();

        #endregion
        
        #region Setup Delete

        /// <summary>
        /// Create an inventory.  Call Setup, or ReadState before using
        /// </summary>
        public Inventory() { }
        
        /// <summary>
        /// create an invetory of max size holding all objects
        /// </summary>
        public void SetUp()
        {
            SetUp(null);
        }
       
        /// <summary>
        /// create an invetory holding the items and the capacity in the inventory info object passed
        /// </summary>
        public void SetUp(IInventoryInfo inventory)
        {
            _spaceFilled = 0;
            _inventoryInfo = inventory;
        }

        /// <summary>
        /// Remove all items from the inventory
        /// </summary>
        public void Delete()
        {
            foreach (ItemType itemType in Types)
            {
                RemoveFromInvetory(itemType, GetTypeCount(itemType));
            }
        }

        #endregion

        #region Properties
                
        /// <summary>
        /// Underlying list of items, used to show an inventory in an items panel.
        /// Do not edit this directly.
        /// </summary>
        public ItemList UnderlyingList
        {
            get { return _items; }
        }
        
        /// <summary>
        /// inventory info tells the types allowed in the invetory, and the capacity of the inventory        
        /// </summary>
        public IInventoryInfo InventoryInfo
        {
            get { return _inventoryInfo; }
        }
        
        /// <summary>
        /// List of all the item types currently in the invetory
        /// </summary>
        public List<ItemType> Types
        {
            get { return _items.ItemTypes; }
        }
                
        /// <summary>
        /// Get how many of one type there is in the inventory
        /// </summary>
        public int GetTypeCount(ItemType type)
        {
            if (_items.HasItem(type) == false) { return 0; }
            return _items.GetItemCount(type);
        }
        
        /// <summary>
        /// Get if the type passed is in the inventory
        /// </summary>
        public bool HasType(ItemType type)
        {
            return (GetTypeCount(type) > 0);
        }
                
        /// <summary>
        /// Get how many of one type there is in the inventory that is not reserved
        /// </summary>
        public int GetTypeCountThatsFree(ItemType type)
        {
            if (_reserved.ContainsKey(type) == false) { _reserved.Add(type, 0); }            
            return GetTypeCount(type) - _reserved[type];
        }
                
        /// <summary>
        /// The remaining capacity of the invetory after reserved space
        /// </summary>
        public int FreeSpaceAfterReservedCapacity
        {
            get { return FreeSpace - _spaceReserved; }
        }
        
        /// <summary>
        /// Will an amount of items of the type passed fit
        /// </summary>
        public bool WillItemsFitAfterReservedCapacity(ItemType type, int amount)
        {
            //see if there is room to add the item            
            return (AmountThatWillFitAfterReservedCapacity(type) >= amount);
        }

        /// <summary>
        /// Get the quantity of a type of item that will fit into the inventory
        /// </summary>
        public int AmountThatWillFitAfterReservedCapacity(ItemType type)
        {            
            return AmountThatWillFitAfterReservedCapacity(type.BaseType);
        }

        /// <summary>
        /// Get the quantity of a type of item that will fit into the inventory
        /// </summary>
        public int AmountThatWillFitAfterReservedCapacity(ItemTypeInfo type)
        {
            if (ItemAllowed(type) == false) { return 0; }
            return FreeSpaceAfterReservedCapacity / type.Size;
        }

        #endregion
        
        #region Logic

        #region Capacity
        
        /// <summary>
        /// Adjust the capacity of the inventory.  Its illegal to adjust the capcity to lower than the current fill level.
        /// </summary>
        public void AdjustCapacityMultiplier(double newCapacityMultipler, int newExtraCapacity)
        {
            Debug.Assert(_inventoryInfo == null || _spaceFilled <= (_inventoryInfo.Capacity * newCapacityMultipler) + newExtraCapacity);
            _capacityMultiplier = newCapacityMultipler;
            _extraCapacity = newExtraCapacity;
            if (ReservedSpaceChanged != null)
            {
                ReservedSpaceChanged();
            }
        }

        /// <summary>
        /// The capacity of the invetory that was added in order to accomidate equipment a worker has
        /// </summary>
        public int ExtraCapacity
        {
            get { return _extraCapacity; }
        }

        /// <summary>
        /// The capacity of the invetory
        /// </summary>
        public int Capacity
        {
            get
            {
                if (_inventoryInfo == null) { return int.MaxValue; }
                return (int)(_inventoryInfo.Capacity * _capacityMultiplier) + _extraCapacity;
            }
        }
        
        /// <summary>
        /// The remaining capacity of the invetory
        /// </summary>
        public int FreeSpace
        {
            get { return Capacity - _spaceFilled; }
        }
        
        /// <summary>
        /// Will an amount of items of the type passed fit
        /// </summary>
        public bool WillItemsFit(ItemType type, int amount)
        {
            //see if there is room to add the item            
            return (AmountThatWillFit(type) >= amount);
        }

        /// <summary>
        /// Get the quantity of a type of item that will fit into the inventory
        /// </summary>
        public int AmountThatWillFit(ItemType type)
        {
            if (ItemAllowed(type.BaseType) == false) { return 0; }
            return FreeSpace / type.Size;
        }

        #endregion
        
        #region Items
        
        /// <summary>
        /// Remove amount of a type of item from the inventory.
        /// </summary>
        public void RemoveFromInvetory(ItemType type, int amount)
        {
            //make sure we have that much
            int amountPresent = _items.GetItemCount(type);
            Debug.Assert(amount <= amountPresent);            

            //remove from the players item list
            if (_inventoryInfo != null && _inventoryInfo.UpdatePlayerItemsList)
            {
                GameState.Current.PlayersItemsList.DecreaseItemCount(type, amount);
            }

            //remove from the players sellable item list
            if (_inventoryInfo != null && _inventoryInfo.UpdatePlayerSellableItemsList)
            {
                GameState.Current.PlayersSellableItemsList.DecreaseItemCount(type, amount);
            }
            
            //decrement fill level
            _spaceFilled -= (type.Size * amount);

            //decrement the count for that type   
            _items.DecreaseItemCount(type, amount);
            if (_items.GetItemCount(type) == 0)
            {
                _items.RemoveItem(type);
            }                        
        }
                        
        /// <summary>
        /// Add an item to the inventory.  Returns the amount actually added.
        /// </summary>
        public void AddToInvetory(ItemType type, int amount)
        {
            //do nothing is amount to add is 0
            if (amount == 0)
            {
                return;
            }

            //if the type is not allowed then something is wrong
            Debug.Assert(ItemAllowed(type.BaseType));            
            
            //if that amount will not fit something is wrong                        
            Debug.Assert(amount <= AmountThatWillFit(type));            

            //add to the players item list
            if (_inventoryInfo.UpdatePlayerItemsList)
            {
                GameState.Current.PlayersItemsList.IncreaseItemCount(type, amount);
            }

            //remove from the players item list
            if (_inventoryInfo.UpdatePlayerSellableItemsList)
            {
                GameState.Current.PlayersSellableItemsList.IncreaseItemCount(type, amount);
            }

            //increase the amount of space now filled in the inventory
            _spaceFilled += (type.Size * amount);
            
            //increment the count for the item
            if (_items.HasItem(type) == false)
            {
                _items.AddItem(type);                
            }
            _items.IncreaseItemCount(type, amount);                                    
        }
                           
        /// <summary>
        /// Return if the item passed is allowed in this inventory
        /// </summary>
        public bool ItemAllowed(ItemTypeInfo baseType)
        {
            if (_inventoryInfo == null) { return true; }

            //if its a one type invneotory make sure what is being added is that same base type as what is already in there
            if (_inventoryInfo.OneBaseTypeInventory && _items.ItemTypes.Count > 0)
            {
                if (_items.ItemTypes[0].BaseType != baseType)
                {
                    return false;
                }
            }

            if (_inventoryInfo.AllowedTypes == null) { return true; }

            //see if that class of item is allowed in this invetory            
            return _inventoryInfo.AllowedTypes.Contains(baseType);           
        }

        #endregion
                   
        #region Reservered Capacity
                
        /// <summary>
        /// Reserve enough capacity in the inventory in order to hold the amount of the item type passed
        /// </summary>
        public void ReserveCapacityFor(ItemType type, int amount)
        {
            if (_capcityReservedFor.ContainsKey(type) == false) { _capcityReservedFor.Add(type, 0); }
            _capcityReservedFor[type] += amount;
            _spaceReserved += (type.Size * amount);
            Debug.Assert(_spaceReserved + _spaceFilled <= Capacity);
            Debug.Assert(_spaceReserved >= 0);

            if (ReservedSpaceChanged != null)
            {
                ReservedSpaceChanged();
            }
        }

        /// <summary>
        /// Free capacity that was reserved to hold the amount of the item type passed
        /// </summary>
        public void FreeReservedCapacityFor(ItemType type, int amount)
        {
            if (_capcityReservedFor.ContainsKey(type) == false) { _capcityReservedFor.Add(type, 0); }
            _capcityReservedFor[type] -= amount;
            _spaceReserved -= (type.Size * amount);
            Debug.Assert(_capcityReservedFor[type] >= 0);
            Debug.Assert(_spaceReserved + _spaceFilled <= Capacity);
            Debug.Assert(_spaceReserved >= 0);

            if (ReservedSpaceChanged != null)
            {
                ReservedSpaceChanged();
            }
        }

        #endregion

        #region Reservered Items
        
        /// <summary>
        /// Reserve the amount of the item passed
        /// </summary>
        public void ReserveItems(ItemType type, int amount)
        {
            //initilize reserved dictionaries if needed
            if (_reserved.ContainsKey(type) == false) { _reserved.Add(type, 0); }
            if (_capcityReservedFor.ContainsKey(type) == false) { _capcityReservedFor.Add(type, 0); }

            //increate the reserved amount
            _reserved[type] += amount;

            //since the amount is reserved it is no longer sellable
            if (_inventoryInfo.UpdatePlayerSellableItemsList)
            {
                GameState.Current.PlayersSellableItemsList.DecreaseItemCount(type, amount);
            }

            //make sure we are not trying to reserve more item than there are, or than there will be (because we have reserved space to put those items)
            Debug.Assert(_reserved[type] <= GetTypeCount(type) + _capcityReservedFor[type]);
            Debug.Assert(_reserved[type] >= 0);

            if (ReservedItemsChanged != null)
            {
                ReservedItemsChanged();
            }
        }

        /// <summary>
        /// Free Reservation for amount of the item passed
        /// </summary>
        public void FreeReservedItems(ItemType type, int amount)
        {
            if (_reserved.ContainsKey(type) == false) { _reserved.Add(type, 0); }
            _reserved[type] -= amount;
            
            //since the amount is no longer reserved it is now sellable again
            if (_inventoryInfo.UpdatePlayerSellableItemsList)
            {
                GameState.Current.PlayersSellableItemsList.IncreaseItemCount(type, amount);
            }

            //make sure the amount reserved is less or equal than the amount we have, and more than or equal to 0
            Debug.Assert(_reserved[type] <= GetTypeCount(type));
            Debug.Assert(_reserved[type] >= 0);

            if (ReservedItemsChanged != null)
            {
                ReservedItemsChanged();
            }
        }

        #endregion
        
        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteInfo(_inventoryInfo);
            writer.WriteObject(_items);
            writer.WriteDouble(_capacityMultiplier);
            writer.WriteInt(_extraCapacity);

            writer.WriteInt(_reserved.Count);
            foreach (ItemType key in _reserved.Keys)
            {
                writer.WriteObject(key);
                writer.WriteInt(_reserved[key]);
            }

            writer.WriteInt(_capcityReservedFor.Count);
            foreach (ItemType key in _capcityReservedFor.Keys)
            {
                writer.WriteObject(key);
                writer.WriteInt(_capcityReservedFor[key]);
            }  
        }

        public void ReadStateV1(StateReaderV1 reader)
		{
			_inventoryInfo = reader.ReadInfo<IInventoryInfo>();
			_items = reader.ReadObject<ItemList>();
			_capacityMultiplier = reader.ReadDouble();
			_extraCapacity = reader.ReadInt();

            int count = reader.ReadInt();
            for (int num = 0; num < count; num++)
            {
                ItemType key = reader.ReadObject<ItemType>();
                int item = reader.ReadInt();
                _reserved.Add(key, item);
            }

            count = reader.ReadInt();
            for (int num = 0; num < count; num++)
            {
                ItemType key = reader.ReadObject<ItemType>();
                int item = reader.ReadInt();
                _capcityReservedFor.Add(key, item);
            }
		}

        public void AfterReadStateV1()
        {
            //if the item type is invalid now remove the item from any dictionaries
            foreach (ItemType item in _reserved.Keys.ToArray())
            {
                if (item.BaseType == null) { _reserved.Remove(item); }
            }
            foreach (ItemType item in _capcityReservedFor.Keys.ToArray())
            {
                if (item.BaseType == null) { _capcityReservedFor.Remove(item); }
            }

            //redetermine the total reserved capcaity
            _spaceReserved = 0;
            foreach (ItemType item in _capcityReservedFor.Keys)
            {
                _spaceReserved += _capcityReservedFor[item] * item.Size;
            }

            //redetermine the fill level
            _spaceFilled = 0;
            foreach (ItemType item in _items.ItemTypes)
            {
                _spaceReserved += _items.GetItemCount(item) * item.Size;
            }
        }
        #endregion
   
    }
}
