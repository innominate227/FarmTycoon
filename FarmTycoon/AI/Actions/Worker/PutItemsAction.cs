using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// Action to put items into a storgae building.  Action needs to be told what is going to be moved so it can reserve space in the building.    
    /// </summary>
    public partial class PutItemsAction : OneLocationAction<Worker>
    {
        #region Member Vars

        /// <summary>
        /// Should subsitutes be allowed when putting the items
        /// </summary>        
        protected bool _allowSubstitues = false;
        
        /// <summary>
        /// List of items to move
        /// </summary>        
        protected ItemList _putList;

        /// <summary>
        /// Storage building to move the items to
        /// </summary>        
        protected IStorageBuilding _storageBuilding;

        /// <summary>
        /// If this is an action where we put back items that we got ealier in the same action sequence then we
        /// do not want to reserve the space we need to put them items, until we first get the items from the building.
        /// This is the action where we get the items.
        /// </summary>        
        protected GetItemsAction _getItemsAction;

        /// <summary>
        /// Set to true once space has been reserved in the storage building.
        /// This way we know if we should free the reserved space incase the action is aborted
        /// </summary>        
        protected bool _spaceReserved = false;

        #endregion

        #region Setup

        public PutItemsAction() { }

        /// <summary>
        /// Create a new put items action
        /// </summary>
        public PutItemsAction(IStorageBuilding storageBuilding, ItemList moveList) : this(storageBuilding, moveList, false) { }
        

        /// <summary>
        /// Create a new put items action
        /// </summary>
        public PutItemsAction(IStorageBuilding storageBuilding, ItemList moveList, bool allowSubsitutes)
        {        
            _storageBuilding = storageBuilding;
            _putList = moveList;
            _allowSubstitues = allowSubsitutes;
        }


        /// <summary>
        /// Create a new put items action that will put back the items gotten via the GetItemsAction passed
        /// </summary>
        public PutItemsAction(GetItemsAction getItemsAction)
        {
            _getItemsAction = getItemsAction;
            _getItemsAction.SetPutBackAction(this);
            _storageBuilding = _getItemsAction.StorageBuilding;
            _putList = _getItemsAction.GetList;
            _allowSubstitues = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// List of items to move
        /// </summary>
        public ItemList PutList
        {
            get { return _putList; }
        }

        #endregion

        #region Logic

        public override Location TheLocation()
        {
            return _storageBuilding.ActionLocation;
        }

        public override double GetActionTime(DelaySet delaySet)
        {
            double buildingDelay = _storageBuilding.StorageBuildingInfo.PutDelayMultiplier;
            double workerDelay = delaySet.GetDelay(ActionOrEventType.PutItems);
            return buildingDelay * workerDelay;
        }

        /// <summary>
        /// We arrived at the location where we will do the action
        /// </summary>
        public override void ArrivedAtAction()
        {
            //set action textures on objects
            _actor.SetTextureForActionOrEvent(ActionOrEventType.PutItems);
        }
        
        public override void DoAction()
        {
            //if the worker is instructed to put something, but he does not have that item, then he can move an item that is similar but of a differnet quality instead
            //the toMoveList will be used to create the list of items the worker should actually move.
            //its starts with the list of items the action wants, but if it is determined a desired items is not in the workers inventory and a subsitiute is available then
            //a subsitiute item will be added to the "toMoveList".
            ItemList toMoveList = new ItemList();
            toMoveList.AddItems(_putList);
            
            while (toMoveList.ItemTypes.Count > 0)
            {
                //move the correct amount of each item type in the move list.  
                //items may be added while we are moving so do ToArray().  Added items will be handeled on the next iteration
                foreach (ItemType itemType in toMoveList.ItemTypes.ToArray())
                {
                    //determine the amount to move
                    int amountToMove = toMoveList.GetItemCount(itemType);
                    
                    //remove the item from the toMoveList, we will either move it all or add a subsitute type if we are unable to move it all                    
                    toMoveList.RemoveItem(itemType);

                    //determine the amount currently present
                    int amountPresent = _actor.Inventory.GetTypeCount(itemType);

                    //if there is not that much then move as much as possible, and then try and move a subsitute product
                    if (amountToMove > amountPresent)
                    {
                        int amountShort = amountToMove - amountPresent;

                        //just move as much of this item as the worker has
                        amountToMove = amountPresent;

                        //find a another type of item the worker has that will subsitute for the type they were short on
                        ItemType subsituteType = DetermineSubsituteType(itemType, _actor.Inventory.UnderlyingList);
                        if (_allowSubstitues && subsituteType != null)
                        {
                            //we need to move the amount we were short of the subsitute type now
                            toMoveList.AddItem(subsituteType);
                            toMoveList.SetItemCount(subsituteType, amountShort);
                        }
                        else
                        {
                            Debug.Assert(false, "Worker was supposed to put items, but he didnt have them");
                        }
                    }
                    
                    //determine how much the building can hold
                    int amountThatCanFit = _storageBuilding.Inventory.AmountThatWillFit(itemType);
                    //its should not be more than the amount to move
                    if (amountToMove > amountThatCanFit)
                    {
                        Debug.Assert(false, "Worker was told to put more items than could fit");
                    }

                    //move that amount                
                    _storageBuilding.Inventory.AddToInvetory(itemType, amountToMove);
                    _actor.Inventory.RemoveFromInvetory(itemType, amountToMove);
                    
                    //if the item has an object attached we might need to do something special with it
                    if (itemType.ItemObject != null)
                    {
                        if (itemType.ItemObject is Animal)
                        {
                            //if we are putting an animal, have the animal stop following the worker
                            Animal animalPutting = (Animal)itemType.ItemObject;
                            animalPutting.StopFollowing();

                            //if we are putting an animal in something thats not a pasture delete the animal game object (it will exsist as only an item)
                            if (_storageBuilding is Pasture == false)
                            {
                                animalPutting.Delete();
                                //the animal is not going to get deleted right away since it is part of the task, so hide it for now
                                animalPutting.Hide();
                                itemType.ItemObject = null;
                            }
                        }
                        else if (itemType.ItemObject is Equipment)
                        {
                            //we need to get off the equipment after we remove from our invnetory or else we may make our inventory too small
                            Equipment equipmentPut = (Equipment)itemType.ItemObject;
                            _actor.GetOffEquipment(equipmentPut);

                            //if we are putting equipment into the delivery area that means we are selling it, so delete it
                            if (_storageBuilding is DeliveryArea)
                            {
                                equipmentPut.Delete();
                                itemType.ItemObject = null;
                            }
                        }
                    }

                }
                
            }
            
            //apply the action to traits
            _actor.ApplyActionOrEventToTraits(ActionOrEventType.PutItems);

            //clear action textures on worker            
            _actor.ClearTextureForActionOrEvent();
        }

        /// <summary>
        /// When the PutItemsAction is going to be putting back items gotten by a GetItemsAction in the same action sequence
        /// then the GetItemsAction will call this method after it has gotten the items so that we can reserve space to put them back.
        /// </summary>
        public void ReserveItemsNow()
        {
            //make sure we are actually paired with an action, or else this should not have been called
            Debug.Assert(_getItemsAction != null);
            ReserveSpace();
        }

        #region Free and Reserve Space

        protected override void AfterAssigned()
        {
            //reserve space now, unless we are going to wait and not reserve until after we get the items
            if (_getItemsAction == null)
            {
                ReserveSpace();
            }
        }
        protected override void AfterFinished()
        {
            FreeReservedSpace();
        }
        protected override void AfterAborted(bool wasStarted)
        {
            FreeReservedSpace();
        }

        /// <summary>
        /// Reserve Space in the building to place the items
        /// </summary>
        private void ReserveSpace()
        {
            foreach (ItemType itemType in _putList.ItemTypes)
            {
                _storageBuilding.Inventory.ReserveCapacityFor(itemType, _putList.GetItemCount(itemType));
            }
            _spaceReserved = true;
        }

        /// <summary>
        /// Free Reserved Space in the building
        /// </summary>
        private void FreeReservedSpace()
        {
            //only free space if it had been reserved
            if (_spaceReserved)
            {
                foreach (ItemType itemType in _putList.ItemTypes)
                {
                    _storageBuilding.Inventory.FreeReservedCapacityFor(itemType, _putList.GetItemCount(itemType));
                }
            }
        }


        #endregion


        public override bool IsObjectInvolved(IGameObject obj)
        {            
            if (obj == _storageBuilding) { return true; }

            //it may be an animal / peice of equipmnet we are planning on putting
            if (obj is Animal || obj is Equipment)
            {
                foreach (ItemType item in _putList.ItemTypes)
                {
                    if (item.ItemObject == obj) { return true; }
                }
            }

            return false;
        }

        public override string Description()
        {
            string itemList = "";
            foreach (ItemType itemType in _putList.ItemTypes)
            {
                itemList += itemType.FullName;                
                itemList += "(" + _putList.GetItemCount(itemType).ToString() + ") ,";                
            }
            if (itemList == ""){ itemList= "Nothing";}
                        
            return "Puting " + itemList.Trim(',') + " into " + _storageBuilding.Name;                        
        }



        /// <summary>
        /// Determine a type that is a good subsitute for another type, given a list of items that are canidates as subsitutes. or null if there is no subsitute
        /// </summary>
        public ItemType DetermineSubsituteType(ItemType type, ItemList itemList)
        {
            foreach (ItemType itemType in itemList.ItemTypes)
            {
                //dont return the same type
                if (itemType == type) { continue; }

                //if they have the same baseItemType (then its a valid subsitution)
                if (itemType.BaseType == type.BaseType)
                {
                    return itemType;
                }
            }
            return null;
        }

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteBool(_allowSubstitues);
            writer.WriteObject(_putList);
            writer.WriteObject(_storageBuilding);
            writer.WriteObject(_getItemsAction);
            writer.WriteBool(_spaceReserved);
        }

        public override void ReadStateV1(StateReaderV1 reader)
		{
			base.ReadStateV1(reader);
			_allowSubstitues = reader.ReadBool();
			_putList = reader.ReadObject<ItemList>();
			_storageBuilding = reader.ReadObject<IStorageBuilding>();
			_getItemsAction = reader.ReadObject<GetItemsAction>();
			_spaceReserved = reader.ReadBool();
		}

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion

        
    }
}
