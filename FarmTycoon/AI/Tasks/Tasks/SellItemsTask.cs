using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace FarmTycoon
{
    public partial class SellItemsTask : Task
    {
        #region Member Vars

        /// <summary>
        /// The building that is the prefered source for the items that are to be sold
        /// </summary>
        private IStorageBuilding _preferedSource;

        /// <summary>
        /// Items to sell
        /// </summary>
        private ItemList _whatToSell = new ItemList();
        
        /// <summary>
        /// List of the items that still need to be sold
        /// Used so that if the task is aborted the items that were not sold can be refunded.
        /// </summary>
        private ItemList _leftToSell = new ItemList();

        /// <summary>
        /// The amount made for each item type when the task started
        /// </summary>
        private Dictionary<ItemType, int> _amountMadeForItem = new Dictionary<ItemType, int>();


        #endregion

        #region Setup

        /// <summary>
        /// Create a new SellItemsTask.  
        /// </summary>
        public SellItemsTask() : base() { }

        /// <summary>
        /// Clone the task (only the planning state of the task needs to be cloned)
        /// </summary>
        protected override Task CloneInner()
        {
            SellItemsTask clone = new SellItemsTask();
            clone._preferedSource = _preferedSource;
            clone._whatToSell = _whatToSell;
            return clone;
        }

        #endregion

        #region Properties

        /// <summary>
        /// What to sell (this list should be edited directly)
        /// </summary>
        public ItemList WhatToSell
        {
            set { _whatToSell = value; }
            get { return _whatToSell; }
        }

        /// <summary>
        /// The building that is the prefered source for the items that are to be sold, null for no prefered source
        /// </summary>
        public IStorageBuilding PreferedSource
        {
            get { return _preferedSource; }
            set { _preferedSource = value; }
        }

        #endregion

        #region Logic

        #region Planning

        /// <summary>
        /// Plan out the task
        /// </summary>
        protected override TaskPlan PlanTaskInner()
        {
            //create the TaskPlan to return
            TaskPlan plan = new TaskPlan(this);

            //get list list of items to actually sell (we sell as much as we can in the sell list based on how much we have in the player sellable item list)
            bool didReduce;
            ItemList actualSellList = _whatToSell.ReduceCounts(GameState.Current.PlayersSellableItemsList, out didReduce);
            if (didReduce)
            {
                plan.AddWarning("Not all items selected to be sold are avaiable to be sold.");
            }

            //used to plan what each worker should get before each trip to the delivery area
            TaskItemDivider itemDivider = new TaskItemDivider(actualSellList);
            //used to plan where to get the items we are selling
            TaskItemPlanner itemPlanner = new TaskItemPlanner();
            //if we are selling tows without tractors we need to plan where to get tractors to move the tows
            TaskEquipmentPlanner equipmentPlanner = new TaskEquipmentPlanner(itemPlanner);

            //find the delivery area building
            DeliveryArea deliveryArea = GameState.Current.MasterObjectList.Find<DeliveryArea>();
            if (deliveryArea == null)
            {
                //if no deliveryArea dont try planning
                plan.AddIssue("No Delivery Area", true);
                return plan;
            }

            //we will want to get all the items we sell from buildings near to the deliveryArea or near to the prefered source if one is specified
            Location getItemsNear = deliveryArea.ActionLocation;
            if (_preferedSource != null)
            {
                getItemsNear = _preferedSource.ActionLocation;
            }


            //we must get the items from a StorageBuilding or Pasture.
            GameObjectPredicate<IStorageBuilding> whereToGetItemsPredicate = delegate(IStorageBuilding building)
            {
                return (building is StorageBuilding || building is Pasture);
            };


            //the worker to have sell something next
            int workerNum = 0;

            //keep selling loads until none are left
            ItemList toSell = itemDivider.NextLoad();
            while (toSell != null)
            {
                //determine if we have a list of normal items or a list of equipment
                bool isEquipment = (FarmData.Current.GetEquipmentInfoForItemInfo(toSell.ItemTypes[0].BaseType) != null);
                if (isEquipment)
                {
                    PlanEquipmentItemsTrip(plan, itemPlanner, equipmentPlanner, workerNum, getItemsNear, deliveryArea, toSell, whereToGetItemsPredicate);
                }
                else
                {
                    PlanNormalItemsTrip(plan, itemPlanner, workerNum, getItemsNear, deliveryArea, toSell, whereToGetItemsPredicate);
                }

                //have the next worker find something to sell
                workerNum++;
                if (workerNum >= _numberOfWorkers)
                {
                    workerNum = 0;
                }

                //figure out what the next worker will be selling
                toSell = itemDivider.NextLoad();
            }

            //put any tractors back that were used to tow a tow from the delivery area 
            equipmentPlanner.PlanToPutAllEquipmentBack(plan);

            return plan;
        }





        /// <summary>
        /// Plan a trip for selling normal items
        /// </summary>
        private void PlanNormalItemsTrip(TaskPlan plan, TaskItemPlanner itemPlanner, int workerNum, Location getItemsNear, DeliveryArea deliveryArea, ItemList toSell, GameObjectPredicate<IStorageBuilding> whereToGetItemsPredicate)
        {
            //use the item planner to plan locations to get the items that we will sell
            itemPlanner.PlanToGetItems(plan, workerNum, toSell, getItemsNear, whereToGetItemsPredicate);

            //create action to disguard the items we are selling at the delivery area (we dont want to just put them back in our delivery area) and add to the plan
            DisgardItemsAction disgardItemsAction = new DisgardItemsAction(deliveryArea.ActionLocation, toSell);
            plan.AddAction(workerNum, disgardItemsAction);
        }


        /// <summary>
        /// Plan a trip for selling equipment items
        /// </summary>
        private void PlanEquipmentItemsTrip(TaskPlan plan, TaskItemPlanner itemPlanner, TaskEquipmentPlanner equipmentPlanner, int workerNum, Location getItemsNear, DeliveryArea deliveryArea, ItemList toSell, GameObjectPredicate<IStorageBuilding> whereToGetItemsPredicate)
        {
            //each worker will get a tractor, and a tow if possible.
            ItemType vehicleToSell = null;
            ItemType towToSell = null;

            //get the vheicle and tow they are selling
            foreach (ItemType itemType in toSell.ItemTypes)
            {
                EquipmentInfo equipmentInfo = FarmData.Current.GetEquipmentInfoForItemInfo(itemType.BaseType);
                if (equipmentInfo.IsVehicle)
                {
                    vehicleToSell = itemType;
                }
                else
                {
                    towToSell = itemType;
                }
            }

            //create an item list with just the two/just the vehicle, because the GetItemsAction wants a list
            ItemList vehicleToSellList = new ItemList();
            if (vehicleToSell != null)
            {
                vehicleToSellList.IncreaseItemCount(vehicleToSell, 1);
            }
            ItemList towToSellList = new ItemList();
            if (towToSell != null)
            {
                towToSellList.IncreaseItemCount(towToSell, 1);
            }

            //if worker is selling both tractor and tow he can get them both in one trip
            if (vehicleToSell != null && towToSell != null)
            {
                //find a place to get the tractor
                itemPlanner.PlanToGetItems(plan, workerNum, vehicleToSellList, getItemsNear, whereToGetItemsPredicate);

                //find a place to get the tow
                itemPlanner.PlanToGetItems(plan, workerNum, towToSellList, getItemsNear, whereToGetItemsPredicate);

                //sell the tow first (disguard it at the delivery area)
                DisgardItemsAction sellTow = new DisgardItemsAction(deliveryArea.ActionLocation, towToSellList);
                plan.AddAction(workerNum, sellTow);

                //sell the tractor next
                DisgardItemsAction sellTractor = new DisgardItemsAction(deliveryArea.ActionLocation, vehicleToSellList);
                plan.AddAction(workerNum, sellTractor);
            }
            //worker is just selling a tractor
            else if (vehicleToSell != null && towToSell == null)
            {
                //find a place to get the tractor
                itemPlanner.PlanToGetItems(plan, workerNum, vehicleToSellList, getItemsNear, whereToGetItemsPredicate);

                //sell the tractor
                DisgardItemsAction sellTractor = new DisgardItemsAction(deliveryArea.ActionLocation, vehicleToSellList);
                plan.AddAction(workerNum, sellTractor);
            }
            //worker is just selling a tow
            else if (vehicleToSell == null && towToSell != null)
            {
                //get the worker a vehicle if they dont have one
                equipmentPlanner.PlanToGetVehicleIfNeeded(plan, workerNum, getItemsNear);

                //find a place to get the tow
                itemPlanner.PlanToGetItems(plan, workerNum, towToSellList, getItemsNear, whereToGetItemsPredicate);

                //sell the tow
                DisgardItemsAction sellTow = new DisgardItemsAction(deliveryArea.ActionLocation, towToSellList);
                plan.AddAction(workerNum, sellTow);
            }
        }


        #endregion
        
        #region Store Inventory


        /// <summary>
        /// Just before the task is started,  get the money for the items
        /// </summary>
        protected override void BeforeStarted()
        {
            base.AfterStarted();

            //determine the items that will actually be sold, based on what is avaialbe to be sold
            ItemList actualSellList = _whatToSell.ReduceCounts(GameState.Current.PlayersSellableItemsList);

            //start the left to sell list with everything in the actual sell list
            _leftToSell.AddItems(actualSellList);

            //get the money for the item being sold, and remeber how much we got for each item type
            foreach (ItemType itemType in actualSellList.ItemTypes)
            {
                int amountToSell = actualSellList.GetItemCount(itemType);

                //determine the current cost of the item, and remeber the amount
                int itemCost = GameState.Current.Prices.GetPrice(itemType);
                _amountMadeForItem.Add(itemType, itemCost);

                //get money for the amount we sold
                int profitForItem = itemCost * amountToSell;
                GameState.Current.Treasury.Sell(Treasury.ITEMS_CATAGORY, itemType.BaseName, profitForItem);
            }
        }


        public override void ActionFinished(ActionBase<Worker> action)
        {
            base.ActionFinished(action);

            //if it was a disguard item action (we got rid of an item we were selling), then remove from left to sell.  and add the item to the stores inventory
            if (action is DisgardItemsAction)
            {
                ItemList itemsJustSold = (action as DisgardItemsAction).ToDisguard;
                foreach (ItemType itemTypeGotten in itemsJustSold.ItemTypes)
                {
                    //determine how much we sold
                    int amountSold = itemsJustSold.GetItemCount(itemTypeGotten);

                    //remove the items from the left to sell list
                    _leftToSell.DecreaseItemCount(itemTypeGotten, amountSold);

                    //add to store inventory
                    GameState.Current.StoreStock.IncreaseItemCount(itemTypeGotten, amountSold);

                    //we should never sell more than we needed to sell
                    Debug.Assert(_leftToSell.GetItemCount(itemTypeGotten) >= 0);
                }
            }
        }

        protected override void AfterAborted(bool wasStarted)
        {
            //for all the items we never sold we should give back the money we got for them
            foreach (ItemType itemType in _leftToSell.ItemTypes)
            {
                int amountNotSold = _leftToSell.GetItemCount(itemType);

                //refund the item (we do sell and pass -1 to negate the sell, as opposde to doing a buy wich would show up as a buy on the spending report)
                int amountMadeForItem = _amountMadeForItem[itemType];
                int totaltoGiveBack = amountMadeForItem * amountNotSold;
                GameState.Current.Treasury.Sell(Treasury.ITEMS_CATAGORY, itemType.BaseName, -1*totaltoGiveBack);
            }
        }

        #endregion

        #region Other

        public override bool DependsOnObject(IGameObject obj)
        {
            if (obj == _preferedSource) { return true; }
            return false;
        }


        public override string Description()
        {
            string description = "Sell";
            foreach (ItemType itemType in _whatToSell.ItemTypes)
            {
                int amount = _whatToSell.GetItemCount(itemType);
                description += " " + itemType.FullName + "(" + amount.ToString() + ")";
            }

            if (_preferedSource == null)
            {
                description += " get from nearest building.";
            }
            else
            {
                description += " get from " + _preferedSource.Name + ".";
            }

            return description;
        }


        #endregion

        #endregion

        #region Save Load
		public override void WriteStateV1(StateWriterV1 writer)
		{
			base.WriteStateV1(writer);
			writer.WriteObject(_preferedSource);
			writer.WriteObject(_whatToSell);
			writer.WriteObject(_leftToSell);

            writer.WriteInt(_amountMadeForItem.Count);
            foreach (ItemType key in _amountMadeForItem.Keys)
            {
                writer.WriteObject(key);
                writer.WriteInt(_amountMadeForItem[key]);
            }
		}
		
		public override void ReadStateV1(StateReaderV1 reader)
		{
			base.ReadStateV1(reader);
			_preferedSource = reader.ReadObject<IStorageBuilding>();
			_whatToSell = reader.ReadObject<ItemList>();
			_leftToSell = reader.ReadObject<ItemList>();

            int count = reader.ReadInt();
            for (int num = 0; num < count; num++)
            {
                ItemType key = reader.ReadObject<ItemType>();
                int item = reader.ReadInt();
                _amountMadeForItem.Add(key, item);
            }
		}
		
		public override void AfterReadStateV1()
		{
			base.AfterReadStateV1();
		}
		#endregion


    }
}
