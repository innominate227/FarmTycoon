using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace FarmTycoon
{
    public class BuyItemsTask : Task
    {
        #region Member Vars

        /// <summary>
        /// The building that is the prefered destination for the items that are to be bought, null for no prefered destination
        /// </summary>
        private IStorageBuilding _preferedDestination;

        /// <summary>
        /// Items to buy
        /// </summary>
        private ItemList _whatToBuy = new ItemList();
        
        /// <summary>
        /// List of the items that still need to be gotten for this task.
        /// Used so that if the task is aborted the items that were not gotten can be refunded.
        /// </summary>
        private ItemList _leftToGet = new ItemList();

        /// <summary>
        /// The amount paid for each item type when they were bought
        /// </summary>
        private Dictionary<ItemType, int> _amountPaidForItem = new Dictionary<ItemType, int>();

        #endregion

        #region Setup

        /// <summary>
        /// Create a new task to buy items
        /// </summary>
        public BuyItemsTask() : base() { }

        /// <summary>
        /// Clone the task (only the planning state of the task needs to be cloned)
        /// </summary>
        protected override Task CloneInner()
        {
            BuyItemsTask clone = new BuyItemsTask();
            clone._preferedDestination = _preferedDestination;
            clone._whatToBuy = _whatToBuy;
            return clone;
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Items to buy (this list should be edited via this property)
        /// </summary>
        public ItemList WhatToBuy
        {            
            get { return _whatToBuy; }            
        }
                
        /// <summary>
        /// The building that is the prefered destination for the items that are to be bought, null for no prefered destination
        /// </summary>
        public IStorageBuilding PreferedDestination
        {
            get { return _preferedDestination; }
            set { _preferedDestination = value; }
        }

        #endregion

        #region Logic
        
        #region Plan Task

        /// <summary>
        /// Plan out the task
        /// </summary>
        protected override TaskPlan PlanTaskInner()
        {
            
            //create the TaskPlan to return
            TaskPlan plan = new TaskPlan(this);

            //get list list of items to actually buy (we buy as much as we can in the buy list based on the store inventory)
            bool didReduce;
            ItemList actualBuyList = _whatToBuy.ReduceCounts(GameState.Current.StoreStock, out didReduce);
            if (didReduce)
            {
                plan.AddWarning("Not all items are currently avilable");
            }

            //used to plan what each worker should get on each trip from the delivery area
            TaskItemDivider itemDivider = new TaskItemDivider(actualBuyList);
            //used to plan where to take the purchased items
            TaskItemPlanner itemPlanner = new TaskItemPlanner();
            //if we are buying tows without tractores we need to plan where to get tractors to move the tows
            TaskEquipmentPlanner equipmentPlanner = new TaskEquipmentPlanner(itemPlanner);
                        
            
            //find the delivery area building
            DeliveryArea deliveryArea = GameState.Current.MasterObjectList.Find<DeliveryArea>();
            if (deliveryArea == null)
            {
                //if no delivery area dont try planning
                plan.AddIssue("No Delivery Area.", true);
                return plan;
            }
                        
            //we will want to put all the items we buy in buildings near to the delivery area or near to the prefered destination if one is specified
            Location putItemsNear = deliveryArea.ActionLocation;
            if (_preferedDestination != null)
            {
                putItemsNear = _preferedDestination.ActionLocation;
            }

            //we must put the items into a StorageBuilding or Pasture.  we cannot put directly into a production building.
            GameObjectPredicate<IStorageBuilding> whereToPutItemsPredicate = delegate(IStorageBuilding building)
            {                
                return (building is StorageBuilding || building is Pasture);
            };
            
            //the worker to have get something from the delivery area next
            int workerNum = 0;

            //keep getting loads until none are left
            ItemList toGet = itemDivider.NextLoad();
            while (toGet != null)
            {
                //determine if we have a list of normal items or a list of equipment
                bool isEquipmentLoad = (FarmData.Current.GetEquipmentInfoForItemInfo(toGet.ItemTypes[0].BaseType) != null);
                if (isEquipmentLoad)
                {
                    PlanEquipmentItemsTrip(plan, itemPlanner, equipmentPlanner, workerNum, putItemsNear, deliveryArea, toGet, whereToPutItemsPredicate);
                }
                else
                {
                    PlanNormalItemsTrip(plan, itemPlanner, workerNum, putItemsNear, deliveryArea, toGet, whereToPutItemsPredicate);                    
                }

                //have the next worker find something to get
                workerNum++;
                if (workerNum >= _numberOfWorkers)
                {
                    workerNum = 0;
                }

                //figure out what the next worker will be getting
                toGet = itemDivider.NextLoad();
            }
            
            //put any tractors back that were used to tow a tow from the delivery area 
            equipmentPlanner.PlanToPutAllEquipmentBack(plan);
            
            return plan;
        }
        

        /// <summary>
        /// Plan a trip for getting normal items
        /// </summary>
        private void PlanNormalItemsTrip(TaskPlan plan, TaskItemPlanner itemPlanner, int workerNum, Location putItemsNear, DeliveryArea deliveryArea, ItemList toGet, GameObjectPredicate<IStorageBuilding> whereToPutItemsPredicate)
        {
            //create action to get the items needed from the delivery area, and add to the plan
            GetItemsAction purchaseItemsAction = new GetItemsAction(deliveryArea, toGet);
            plan.AddAction(workerNum, purchaseItemsAction);

            //use the item planner to plan locations to put the items
            itemPlanner.PlanToPutItems(plan, workerNum, toGet, false, putItemsNear, whereToPutItemsPredicate);            
        }
        
        /// <summary>
        /// Plan a trip for getting equipment items
        /// </summary>
        private void PlanEquipmentItemsTrip(TaskPlan plan, TaskItemPlanner itemPlanner, TaskEquipmentPlanner equipmentPlanner, int workerNum, Location putItemsNear, DeliveryArea deliveryArea, ItemList toGet, GameObjectPredicate<IStorageBuilding> whereToPutItemsPredicate)
        {
            //each worker will get a tractor, and a tow if possible.
            ItemType vehicleToGet = null;
            ItemType towToGet = null;
                
            //get the vheicle and tow they are getting
            foreach (ItemType itemType in toGet.ItemTypes)
            {
                EquipmentInfo equipmentInfo = FarmData.Current.GetEquipmentInfoForItemInfo(itemType.BaseType);
                if (equipmentInfo.IsVehicle)
                {
                    vehicleToGet = itemType;
                }
                else
                {
                    towToGet = itemType;                    
                }
            }

            //create an item list with just the two/just the vehicle, because the GetItemsAction wants a list
            ItemList vehicleToGetList = new ItemList();
            if (vehicleToGet != null)
            {
                vehicleToGetList.IncreaseItemCount(vehicleToGet, 1);
            }
            ItemList towToGetList = new ItemList();
            if (towToGet != null)
            {
                towToGetList.IncreaseItemCount(towToGet , 1);
            }

            
            //if worker is getting both tractor and tow he can get them both in one trip
            if (vehicleToGet != null && towToGet != null)
            {
                //get the tractor first                
                GetItemsAction purchaseTractor = new GetItemsAction(deliveryArea, vehicleToGetList);
                plan.AddAction(workerNum, purchaseTractor);
                    
                //get the tow next
                GetItemsAction purchaseTow = new GetItemsAction(deliveryArea, towToGetList);
                plan.AddAction(workerNum, purchaseTow);

                //find a place for the tow   
                itemPlanner.PlanToPutItems(plan, workerNum, towToGetList, false, putItemsNear, whereToPutItemsPredicate);

                //find a place for the tractor
                itemPlanner.PlanToPutItems(plan, workerNum, vehicleToGetList, false, putItemsNear, whereToPutItemsPredicate);                 
            }
            //worker is just buying a tractor
            else if (vehicleToGet != null && towToGet == null)
            {
                //purchase the tractor
                GetItemsAction purchaseTractor = new GetItemsAction(deliveryArea, vehicleToGetList);
                plan.AddAction(workerNum, purchaseTractor);

                //find a place for the tractor
                itemPlanner.PlanToPutItems(plan, workerNum, vehicleToGetList, false, putItemsNear, whereToPutItemsPredicate);
            }
            //worker is just buying a tow
            else if (vehicleToGet == null && towToGet != null)
            {
                //get the worker a vehicle if they dont have one
                equipmentPlanner.PlanToGetVehicleIfNeeded(plan, workerNum, deliveryArea.LocationOn);
                    
                //purchase the tow
                GetItemsAction purchaseTow = new GetItemsAction(deliveryArea, towToGetList);
                plan.AddAction(workerNum, purchaseTow);

                //find a place to put the tow   
                itemPlanner.PlanToPutItems(plan, workerNum, towToGetList, false, putItemsNear, whereToPutItemsPredicate);                     
            }  
        }

        #endregion

        #region Manage Store Inventory

        /// <summary>
        /// Just before the task is started, remove the items from the store inventory, pay for the items, and put them into the delivery area inventory
        /// </summary>
        protected override void BeforeStarted()
        {
            base.BeforeStarted();

            //get the list of items to actually buy (which is everything we want to buy, unless the store does not have enough in stock in which case it is everything we can buy)
            ItemList actualBuyList = _whatToBuy.ReduceCounts(GameState.Current.StoreStock);

            //find the delivery area
            DeliveryArea deliveryArea = GameState.Current.MasterObjectList.Find<DeliveryArea>();

            //start the left to get list with everything
            _leftToGet.AddItems(actualBuyList);

            //remove the items we are going to buy from the stores stock, and pay for them, then add them to the delivery area
            foreach (ItemType itemType in actualBuyList.ItemTypes)
            {
                //remove froms stores stock
                int amountToBuy = actualBuyList.GetItemCount(itemType);
                GameState.Current.StoreStock.DecreaseItemCount(itemType, amountToBuy);                                

                //determine the current cost of the item, and remeber the amount paid for it
                int itemCost = GameState.Current.Prices.GetPrice(itemType);
                _amountPaidForItem.Add(itemType, itemCost);

                //pay for the amount we bought
                int totalCost = itemCost * amountToBuy;
                GameState.Current.Treasury.Buy(Treasury.ITEMS_CATAGORY, itemType.BaseName, totalCost);                    

                //put the items bought into the delivery area                
                deliveryArea.Inventory.AddToInvetory(itemType, amountToBuy);
            }                        
        }


        public override void ActionFinished(ActionBase<Worker> action)
        {
            base.ActionFinished(action);

            //if it was a get item action (that we got from the delivery area), remove the items gotten from left to get list
            if (action is GetItemsAction && (action as GetItemsAction).StorageBuilding is DeliveryArea)
            {
                ItemList itemsJustGotten = (action as GetItemsAction).GetList;
                foreach (ItemType itemTypeGotten in itemsJustGotten.ItemTypes)
                {
                    //determine how much we got
                    int amountGotten = itemsJustGotten.GetItemCount(itemTypeGotten);

                    //remove the items from the left to get list
                    _leftToGet.DecreaseItemCount(itemTypeGotten, amountGotten);

                    //we should never get more than we needed to get
                    Debug.Assert(_leftToGet.GetItemCount(itemTypeGotten) >= 0);
                }                
            }
        }

        protected override void AfterAborted(bool wasStarted)
        {
            //find the delivery area
            DeliveryArea deliveryArea = GameState.Current.MasterObjectList.Find<DeliveryArea>();
            
            //all the items we never got should be removed from the delivery area, and put back into the stores inventory
            foreach (ItemType itemType in _leftToGet.ItemTypes)
            {
                //increase stores inventory
                int amountNotGotten = _leftToGet.GetItemCount(itemType);
                GameState.Current.StoreStock.IncreaseItemCount(itemType, amountNotGotten);
                                
                //refund the item (we do a buy with a negative number so it takes away from the buy total for the month instead of doing a sell)
                int amountPaidForItem = _amountPaidForItem[itemType];
                int totalRefund = amountPaidForItem * amountNotGotten;
                GameState.Current.Treasury.Buy(Treasury.ITEMS_CATAGORY, itemType.BaseName, -1 * totalRefund);

                //remove the items we didnt get from the delivery area
                deliveryArea.Inventory.RemoveFromInvetory(itemType, amountNotGotten);
            }          
        }

        #endregion
        
        #region Other
        
        public override bool DependsOnObject(IGameObject obj)
        {
            if (obj == _preferedDestination) { return true; }
            return false;
        }

        public override string Description()
        {
            string description = "Buy";            
            foreach (ItemType itemType in _whatToBuy.ItemTypes)
            {
                int amount = _whatToBuy.GetItemCount(itemType);
                description += " " + itemType.FullName + "(" + amount.ToString() + ")";
            }

            if (_preferedDestination == null)
            {
                description += " and place in nearest building.";
            }
            else
            {
                description += " and place in " + _preferedDestination.Name + ".";
            }
            
            return description;
        }


        #endregion

        #endregion

        #region Save Load
		public override void WriteStateV1(StateWriterV1 writer)
		{
			base.WriteStateV1(writer);
			writer.WriteObject(_preferedDestination);
			writer.WriteObject(_whatToBuy);
			writer.WriteObject(_leftToGet);

            writer.WriteInt(_amountPaidForItem.Count);
            foreach (ItemType key in _amountPaidForItem.Keys)
            {
                writer.WriteObject(key);
                writer.WriteInt(_amountPaidForItem[key]);
            }
		}
		
		public override void ReadStateV1(StateReaderV1 reader)
		{
			base.ReadStateV1(reader);
			_preferedDestination = reader.ReadObject<IStorageBuilding>();
			_whatToBuy = reader.ReadObject<ItemList>();
			_leftToGet = reader.ReadObject<ItemList>();

            int count = reader.ReadInt();
            for (int num = 0; num < count; num++)
            {
                ItemType key = reader.ReadObject<ItemType>();
                int item = reader.ReadInt();
                _amountPaidForItem.Add(key, item);
            }
		}
		
		public override void AfterReadStateV1()
		{
			base.AfterReadStateV1();
		}
		#endregion

    }
}
