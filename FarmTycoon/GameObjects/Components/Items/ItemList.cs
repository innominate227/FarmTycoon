using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// A list of items. with a count mantained for each item.    
    /// </summary>
    public class ItemList : ISavable
    {
        #region Events

        /// <summary>
        /// Raised when the list has changed in any way, inculding if just the count of an item in the list changes
        /// </summary>
        public event Action ListChanged;

        /// <summary>
        /// Raised when a new itemtype has been added to the list
        /// </summary>
        public event Action<ItemType> ItemAdded;

        /// <summary>
        /// Raised when an itemtype has been completly removed from the list
        /// </summary>
        public event Action<ItemType> ItemRemoved;
        
        /// <summary>
        /// Raise the list changed event if its being handeled
        /// </summary>
        private void RaiseListChanged()
        {
            if (ListChanged != null)
            {
                ListChanged();
            }
        }

        #endregion

        #region Member Vars

        /// <summary>
        /// The counts for item in the list
        /// </summary>
        private Dictionary<ItemType, int> _counts = new Dictionary<ItemType, int>();

        #endregion

        #region Properties

        /// <summary>
        /// Get the types of items in this list
        /// </summary>
        public List<ItemType> ItemTypes
        {
            get { return _counts.Keys.ToList<ItemType>(); }
        }

        /// <summary>
        /// Get if there is an item of they type in inventory
        /// </summary>
        public bool HasItem(ItemType itemType)
        {
            return _counts.ContainsKey(itemType);
        }

        /// <summary>
        /// Get the count for an item in the list, if the item was not in the list it will return 0
        /// </summary>
        public int GetItemCount(ItemType type)
        {
            if (_counts.ContainsKey(type))
            {
                return _counts[type];
            }
            else
            {
                return 0;
            }
        }


        #endregion

        #region Logic

        /// <summary>
        /// Add an item to this list, the count type will be set to 0 by default.
        /// Nothing will happen if item is already present.
        /// </summary>
        public void AddItem(ItemType type)
        {
            AddItem(type, false);
        }

        /// <summary>
        /// Add an item to this list, the count type will be set to none by default.
        /// Nothing will happen if item is already present.
        /// </summary>
        private void AddItem(ItemType type, bool supressListChanged)
        {
            if (_counts.ContainsKey(type) == false)
            {
                _counts.Add(type, 0);
            }
            if (supressListChanged == false)
            {
                RaiseListChanged();
            }

            //raise item added evnet
            if (ItemAdded != null)
            {
                ItemAdded(type);
            }
        }
        
        /// <summary>
        /// Add all the items in the list passed to this list
        /// </summary>
        public void AddItems(ItemList list)
        {
            //add all items in the passed list
            foreach (ItemType itemType in list.ItemTypes)
            {
                if (this.HasItem(itemType) == false)
                {
                    //if that type is not alreayd in there add it

                    //add the type to the combined list
                    this.AddItem(itemType, true);

                    //set it to have the same count
                    this.SetItemCount(itemType, list.GetItemCount(itemType));
                }
                else
                {
                    //add the counts
                    this.SetItemCount(itemType, this.GetItemCount(itemType) + list.GetItemCount(itemType));
                }
            }

            RaiseListChanged();
        }
                        
        /// <summary>
        /// Remove an items of a certain type from the list, of not in the list ignore
        /// </summary>
        public void RemoveItem(ItemType type)
        {
            if (_counts.ContainsKey(type))
            {
                _counts.Remove(type);                
            }
            else
            {
                //ignore
            }
            RaiseListChanged();

            if (ItemRemoved != null)
            {
                ItemRemoved(type);
            }
        }

        /// <summary>
        /// Clear all items from an item list
        /// </summary>
        public void Clear()
        {
            _counts.Clear();
            RaiseListChanged();
        }
        
        /// <summary>
        /// Set the count for an item in the list, if the item was not in the list it will be added.
        /// </summary>
        public void SetItemCount(ItemType type, int count)
        {
            if (_counts.ContainsKey(type) == false)
            {
                AddItem(type, true);
            }
            _counts[type] = count;            
            RaiseListChanged();            
        }

        /// <summary>
        /// Increase the count for an item int the list by an amount, if the item was not in the list it will be added
        /// </summary>
        public void IncreaseItemCount(ItemType type, int amount)
        {
            SetItemCount(type, GetItemCount(type) + amount);
        }

        /// <summary>
        /// Decrease the count for an item int the list by an amount, if the item was not in the list it will be added
        /// If the count is reduced to 0 the item will be removed from the list
        /// </summary>
        public void DecreaseItemCount(ItemType type, int amount)
        {
            SetItemCount(type, GetItemCount(type) - amount);
            if (GetItemCount(type) == 0)
            {
                RemoveItem(type);
            }
        }
        
        /// <summary>
        /// Combines this item list with another and returns the combined list.
        /// </summary>
        public ItemList CombineWith(ItemList itemList)
        {
            ItemList toRet = new ItemList();

            //add all the items in this list
            toRet.AddItems(this);

            //add all items in the passed list
            toRet.AddItems(itemList);

            //retrun the combined list
            return toRet;
        }

        /// <summary>
        /// Return a new ItemList that contains all the items that are contained in this items list, but with the 
        /// counts of each item reduced so as to be no higer than the count for the same item in the limit list.
        /// </summary>
        public ItemList ReduceCounts(ItemList limitList)
        {
            bool unused;
            return ReduceCounts(limitList, out unused);
        }

        /// <summary>
        /// Return a new ItemList that contains all the items that are contained in this items list, but with the 
        /// counts of each item reduced so as to be no higer than the count for the same item in the limit list.
        /// didReduce is set to true if any item in the list needed to be reduced.
        /// </summary>
        public ItemList ReduceCounts(ItemList limitList, out bool didReduce)
        {

            //create the list to return
            ItemList returnList = new ItemList();

            //false until we actuall reduce something
            didReduce = false;

            //for each item in this list
            foreach (ItemType item in this.ItemTypes)
            {
                //we would like to put the same amount into the return list
                int amountForReturnList = this.GetItemCount(item);

                //but if there is less in the limit list then reduce to the amount in the limit list
                int amountInLimitList = GameState.Current.StoreStock.GetItemCount(item);
                if (amountForReturnList > amountInLimitList)
                {
                    amountForReturnList = amountInLimitList;
                    didReduce = true;
                }

                //add the item to the return list
                returnList.IncreaseItemCount(item, amountForReturnList);
            }

            //return the reduced list
            return returnList;
        }
        
        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteInt(_counts.Count);
            foreach (ItemType key in _counts.Keys)
            {
                writer.WriteObject(key);
                writer.WriteInt(_counts[key]);
            }  
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            int count = reader.ReadInt();
            for (int num = 0; num < count; num++)
            {
                ItemType key = reader.ReadObject<ItemType>();
                int item = reader.ReadInt();
                _counts.Add(key, item);
            }
        }

        public void AfterReadStateV1()
        {
            //make sure all items in this list are still valid items
            foreach (ItemType itemType in _counts.Keys.ToArray())
            {
                if (itemType.BaseType == null)
                {
                    _counts.Remove(itemType);
                }
            }
        }
        #endregion

    }
}
