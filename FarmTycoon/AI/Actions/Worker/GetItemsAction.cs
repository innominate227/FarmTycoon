using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public partial class GetItemsAction : OneLocationAction<Worker>
    {
        #region Member Vars

        /// <summary>
        /// List of items to move
        /// </summary>
        private ItemList _getList;

        /// <summary>
        /// Building to get the items from
        /// </summary>
        private IStorageBuilding _storageBuilding;

        /// <summary>
        /// If we plan to put back the items we got from the building during the same action sequence.
        /// Then this is the action where we are planning to put the items back, when we get the items
        /// we tell this action so that it can reserve the space needed to put them back.
        /// </summary>
        private PutItemsAction _putBackAction = null;

        #endregion

        #region Setup

        public GetItemsAction(){ }

        /// <summary>
        /// Create a new move items action pass the object to move to.
        /// Note: on creation the items in the building will be reserved
        /// </summary>
        public GetItemsAction(IStorageBuilding storageBuilding, ItemList getList)
        {
            _storageBuilding = storageBuilding;
            _getList = getList;
        }

        /// <summary>
        /// If there is an action dring the same sequence where we putback the item that we got with this action,
        /// then this will be called, so that we can tell the putBackAction to reserve space after we get the item.
        /// </summary>
        public void SetPutBackAction(PutItemsAction putBackAction)
        {
            _putBackAction = putBackAction;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Building to get the items from
        /// </summary>
        public IStorageBuilding StorageBuilding
        {
            get { return _storageBuilding; }
        }

        /// <summary>
        /// List of items to get
        /// </summary>
        public ItemList GetList
        {
            get { return _getList; }
        }

        #endregion

        #region Logic

        public override Location TheLocation()
        {
            return _storageBuilding.ActionLocation;
        }

        public override double GetActionTime(DelaySet delaySet)
        {
            double buildingDelay = _storageBuilding.StorageBuildingInfo.GetDelayMultiplier;
            double workerDelay = delaySet.GetDelay(ActionOrEventType.GetItems);
            return buildingDelay * workerDelay;
        }

        /// <summary>
        /// We arrived at the location where we will do the action
        /// </summary>
        public override void ArrivedAtAction()
        {
            //set action textures on objects
            _actor.SetTextureForActionOrEvent(ActionOrEventType.GetItems);
        }

        public override void DoAction()
        {   
            //move the correct amount of each item type in the move list
            foreach (ItemType itemType in _getList.ItemTypes)
            {                
                //determine the amount to move
                int amountToMove = _getList.GetItemCount(itemType);

                //determine how much is present in the building                                  
                int amountPresent = _storageBuilding.Inventory.GetTypeCount(itemType);
                //if there is not that much then thats a problem
                Debug.Assert(amountToMove <= amountPresent);
                               
                
                //determine how much the worker can hold
                int amountThatCanFit = _actor.Inventory.AmountThatWillFit(itemType);               
                //if more than the worker can hold thats a problem
                Debug.Assert(amountToMove <= amountThatCanFit);
                

                //if the item we got has an associated object, we may need to do something special with it
                if (itemType.ItemObject != null)
                {
                    //if we are getting an animal, have the animal follow the worker
                    if (itemType.ItemObject is Animal)
                    {
                        Animal animalGotten = (Animal)itemType.ItemObject;
                        animalGotten.StartFollowing(_actor);
                    }
                    else if (itemType.ItemObject is Equipment)
                    {
                        //we need to get on the equipment before adding it to our inventory or else our inventory may not be large enough to acomidate the equipment
                        Equipment equipmentGotten = (Equipment)itemType.ItemObject;
                        _actor.GetOnEquipment(equipmentGotten);
                    }
                }


                //move that amount                
                _actor.Inventory.AddToInvetory(itemType, amountToMove);
                _storageBuilding.Inventory.RemoveFromInvetory(itemType, amountToMove);

            }
      
            //we have gotten the items, and we are going to put them back in the same action sequence then 
            //tell the putback action that we got them
            if (_putBackAction != null)
            {
                _putBackAction.ReserveItemsNow();
            }
            
            //apply the action to traits
            _actor.ApplyActionOrEventToTraits(ActionOrEventType.GetItems);

            //clear action textures on objects
            _actor.ClearTextureForActionOrEvent();
        }

        #region Action State Changed

        protected override void AfterAssigned()
        {
            ReserveItems();
            MoveAnimalsToEntrance();
        }
        protected override void AfterFinished()
        {
            FreeReservedItems();
        }
        protected override void AfterAborted(bool wasStarted)
        {
            FreeReservedItems();
            AbortMovingAnimalsToEntrance();
        }


        /// <summary>
        ///reserve the items int eh inventory of the storage building we are getting the items from
        /// </summary>
        private void ReserveItems()
        {            
            foreach (ItemType itemType in _getList.ItemTypes)
            {
                _storageBuilding.Inventory.ReserveItems(itemType, _getList.GetItemCount(itemType));
            }            
        }

        /// <summary>
        /// free the reserved items
        /// </summary>
        private void FreeReservedItems()
        {
            foreach (ItemType itemType in _getList.ItemTypes)
            {
                _storageBuilding.Inventory.FreeReservedItems(itemType, _getList.GetItemCount(itemType));
            }
        }

        /// <summary>
        /// If we are getting from a pasture move the animals to the entrance
        /// </summary>
        private void MoveAnimalsToEntrance()
        {
            if (_storageBuilding is Pasture)
            {
                foreach (ItemType itemType in _getList.ItemTypes)
                {
                    if (itemType.ItemObject != null && itemType.ItemObject is Animal)
                    {
                        (itemType.ItemObject as Animal).GoToPastureEntrance();
                    }
                }
            }
        }

        /// <summary>
        /// If we are getting from a pasture tell the animals to abort their move to the entrance
        /// </summary>
        private void AbortMovingAnimalsToEntrance()
        {
            if (_storageBuilding is Pasture)
            {
                foreach (ItemType itemType in _getList.ItemTypes)
                {
                    if (itemType.ItemObject != null && itemType.ItemObject is Animal)
                    {
                        (itemType.ItemObject as Animal).AbortGoingToPastureEntrance();
                    }
                }
            }
        }
        
        #endregion


        public override bool IsObjectInvolved(IGameObject obj)
        {
            if (obj == _storageBuilding)
            {
                return true;
            }

            //it may be an animal / peice of equipmnet we are planning on getting
            if (obj is Animal || obj is Equipment)
            {
                foreach (ItemType item in _getList.ItemTypes)
                {
                    if (item.ItemObject == obj) { return true; }
                }
            }

            return false;
        }
        
        public override string Description()
        {
            string itemList = "";
            foreach (ItemType itemType in _getList.ItemTypes)
            {
                itemList += itemType.FullName;                
                itemList += "(" + _getList.GetItemCount(itemType).ToString() + ") ,";                
            }
            if (itemList == ""){ itemList= "Nothing";}
                        
            return "Get " + itemList.Trim(',') + " from " + _storageBuilding.Name;                        
        }

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteObject(_getList);
            writer.WriteObject(_storageBuilding);
            writer.WriteObject(_putBackAction);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _getList = reader.ReadObject<ItemList>();
            _storageBuilding = reader.ReadObject<IStorageBuilding>();
            _putBackAction = reader.ReadObject<PutItemsAction>();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion

    }
}
