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
        /// <summary>
        /// The building that is the prefered destination for the items that are to be bought, null for no prefered destination
        /// </summary>
        protected StorageBuilding m_preferedDestination;

        /// <summary>
        /// Items to buy
        /// </summary>
        protected ItemList m_whatToBuy = new ItemList();
        
        /// <summary>
        /// Create a new task to buy items, call Setup or ReadState before using
        /// </summary>
        public BuyItemsTask() : base() { }
        

        /// <summary>
        /// Items to buy
        /// </summary>
        public ItemList WhatToBuy
        {            
            get { return m_whatToBuy; }            
        }
                
        /// <summary>
        /// The building that is the prefered destination for the items that are to be bought, null for no prefered destination
        /// </summary>
        public StorageBuilding PreferedDestination
        {
            get { return m_preferedDestination; }
            set { m_preferedDestination = value; }
        }

        /// <summary>
        /// Plan out the task
        /// </summary>
        protected override TaskPlan PlanTaskInner()
        {
            //used to plan where to take the purchased items
            TaskItemPlanner itemPlanner = new TaskItemPlanner();
            //if we are buying tows without tractores we need to plan where to get tractors to move the tows
            TaskEquipmentPlanner equipmentPlanner = new TaskEquipmentPlanner(itemPlanner);

            //create the TaskPlan to return
            TaskPlan plan = new TaskPlan(this);

            //create a list of the items that still need to be bought and the equipment that still needs to be bought
            ItemList itemsLeftToBuy = new ItemList();
            ItemList equipmentLeftToBuy = new ItemList();
            foreach (GameItemType itemType in m_whatToBuy.ItemTypes)
            {
                //ignore items that we are set to buy zero of
                if (m_whatToBuy.GetItemCount(itemType) == 0) { continue; }

                if (itemType.Class == ItemClass.Equipment)
                {
                    equipmentLeftToBuy.SetItemCount(itemType, m_whatToBuy.GetItemCount(itemType));
                }
                else
                {
                    itemsLeftToBuy.SetItemCount(itemType, m_whatToBuy.GetItemCount(itemType));
                }
            }
            
            //find the store building
            Building store = m_game.Tools.GameObjectFinder.FindClosestObjectMeetingPredicate<DeliveryArea>(null, delegate(DeliveryArea building)
            {
                return true;
            });
            if (store == null)
            {
                //if no store dont try planning
                plan.AddIssue("No Store", true);
                return plan;
            }
                        
            //we will wont to put all the items we buy in buildings near to the store or near to the prefered destination if one is specified
            Location putItemsNear = store.LocationOn;
            if (m_preferedDestination != null)
            {
                putItemsNear = m_preferedDestination.LocationOn;
            }
            
            //the worker to have buy something next
            int workerNum = 0;

            //keep going until all the items that need to be bough have been planned for
            while (itemsLeftToBuy.ItemTypes.Count > 0)
            {
                //have each worker buy as much as it can hold
                ItemList willBuy = new ItemList();
                int spaceLeft = Worker.INVENTORY_SIZE;

                //foreach type that still needs to be bought
                foreach (GameItemType itemType in itemsLeftToBuy.ItemTypes)
                {
                    //get the size of the item
                    int itemSize = itemType.Size;
                        
                    //get the amount of this item that still needs to be bought
                    int amountToBuy = itemsLeftToBuy.GetItemCount(itemType);

                    //get the amount the worker can fit (or if more than the amount left get the amount left)
                    int amountCanFit = spaceLeft / itemSize;
                    if (amountCanFit > amountToBuy) { amountCanFit = amountToBuy; }

                    //have the worker can buy that much
                    if (amountCanFit > 0)
                    {
                        //add to workers will buy list
                        willBuy.AddItem(itemType);
                        willBuy.SetItemCount(itemType, amountCanFit);

                        //reduce space worker has left
                        spaceLeft -= (itemSize * amountCanFit);

                        //remove that amount from left to buy
                        itemsLeftToBuy.DecreaseItemCount(itemType, amountCanFit);
                    }

                    //remove the item from leftToBuy if all have been bought
                    if (itemsLeftToBuy.GetItemCount(itemType) == 0)
                    {
                        itemsLeftToBuy.RemoveItem(itemType);
                    }

                    //if the worker cant carry any more stop looking for items for him to carry
                    if (spaceLeft == 0)
                    {
                        break;
                    }
                }
                                        
                //create action to purchase the items needed, and add to the plan
                BuyItemsAction purchaseItemsAction = new BuyItemsAction(store, willBuy);
                plan.AddAction(workerNum, purchaseItemsAction);

                //use the item planner to plan locations to put the items
                itemPlanner.PlanToPutItems(plan, workerNum, willBuy, false, putItemsNear);
                
                //have the next worker find something to buy now
                workerNum++;
                if (workerNum >= m_numberOfWorkers)
                {
                    workerNum = 0;
                }
            } //while items left to buy




            //keep going until all the equipment that need to be bough has been planned for
            while (equipmentLeftToBuy.ItemTypes.Count > 0)
            {
                //each worker will buy a tractor, and a tow if possible.
                //they might buy just a tractor if no tows left, or just a tow if no tractors left.
                EquipmentType vehicleToBuy = null;
                EquipmentType towToBuy = null;
                
                //foreach type of equipmnet that still needs to be bought
                foreach (GameItemType itemType in equipmentLeftToBuy.ItemTypes)
                {
                    EquipmentType equipmentType = Program.DataFiles.EquipmentFile.GetEquipmentType(itemType);

                    if (equipmentType.IsVehicle && vehicleToBuy == null)
                    {
                        vehicleToBuy = equipmentType;
                        equipmentLeftToBuy.DecreaseItemCount(itemType, 1);
                    }
                    else if (equipmentType.IsVehicle == false && towToBuy == null)
                    {
                        towToBuy = equipmentType;
                        equipmentLeftToBuy.DecreaseItemCount(itemType, 1);
                    }

                    //remove the equipment from leftToBuy if all have been bought
                    if (equipmentLeftToBuy.GetItemCount(itemType) == 0)
                    {
                        equipmentLeftToBuy.RemoveItem(itemType);
                    }
                }

                //if worker is buying both tractor and tow he can get them both in one trip
                if (vehicleToBuy != null && towToBuy != null)
                {
                    //purchase the tractor first
                    BuyEquipmentAction purchaseTractor = new BuyEquipmentAction(store, vehicleToBuy);
                    plan.AddAction(workerNum, purchaseTractor);
                    
                    //purchase the tow next
                    BuyEquipmentAction purchaseTow = new BuyEquipmentAction(store, towToBuy);
                    plan.AddAction(workerNum, purchaseTow);

                    //find a place for the tow   
                    itemPlanner.PlanToPutEquipment(plan, workerNum, towToBuy, putItemsNear);

                    //find a place for the tractor
                    itemPlanner.PlanToPutEquipment(plan, workerNum, vehicleToBuy, putItemsNear);                 
                }
                //worker is just buying a tractor
                else if (vehicleToBuy != null && towToBuy == null)
                {
                    //purchase the tractor
                    BuyEquipmentAction purchaseTractor = new BuyEquipmentAction(store, vehicleToBuy);
                    plan.AddAction(workerNum, purchaseTractor);

                    //find a place for the tractor
                    itemPlanner.PlanToPutEquipment(plan, workerNum, vehicleToBuy, putItemsNear);
                }
                //worker is just buying a tow
                else if (vehicleToBuy == null && towToBuy != null)
                {
                    //get the worker a tractor is they dont have one
                    equipmentPlanner.PlanToGetEquipmentIfNeeded(plan, workerNum, "Tractor", store.LocationOn);
                    
                    //purchase the tow
                    BuyEquipmentAction purchaseTow = new BuyEquipmentAction(store, towToBuy);
                    plan.AddAction(workerNum, purchaseTow);

                    //find a place to put the tow   
                    itemPlanner.PlanToPutEquipment(plan, workerNum, towToBuy, putItemsNear);                     
                }                


                //have the next worker find something to buy now
                workerNum++;
                if (workerNum >= m_numberOfWorkers)
                {
                    workerNum = 0;
                }
            } //while equipment left to buy


            //put all equipmwnt that was gotten for the task back
            equipmentPlanner.PlanToPutAllEquipmentBack(plan);


            return plan;
        }

        /// <summary>
        /// List of the items that still need to be bought for this task.
        /// Used so that if the task is abort the items that were never bought can be put back into the stores inventory.
        /// </summary>
        private ItemList m_leftToBuy = new ItemList();


        protected override void AfterSetup()
        {
            base.AfterSetup();

            //start the left to buy list with everything
            m_leftToBuy.AddItems(m_whatToBuy);

            //remove the items we are going to buy from the stores stock
            foreach (GameItemType itemType in m_whatToBuy.ItemTypes)
            {
                int amountToBuy = m_whatToBuy.GetItemCount(itemType);
                m_game.StoreStock.DecreaseItemCount(itemType, amountToBuy);                                
            }                        
        }

        public override void ActionFinished(ActionBase action)
        {
            base.ActionFinished(action);

            //if it was a buy item action, remove the items bought from left to buy list
            if (action is BuyItemsAction)
            {
                ItemList itemsJustBought = (action as BuyItemsAction).BuyList;
                foreach (GameItemType itemTypeBought in itemsJustBought.ItemTypes)
                {
                    //determine how much we bought
                    int amountBought = itemsJustBought.GetItemCount(itemTypeBought);

                    //remove the items from the left to buy list
                    m_leftToBuy.DecreaseItemCount(itemTypeBought, amountBought);

                    //we should never buy more than we needed to buy
                    Debug.Assert(m_leftToBuy.GetItemCount(itemTypeBought) >= 0);
                }                
            }

            //if it was a buy equipmnet action, remove the equipmnet bought from left to buy list
            else if (action is BuyEquipmentAction)
            {
                GameItemType equipmentJustBought = (action as BuyEquipmentAction).ToBuy.ItemType;
                
                //remove the items from the left to buy list
                m_leftToBuy.DecreaseItemCount(equipmentJustBought, 1);

                //we should never buy more than we needed to buy
                Debug.Assert(m_leftToBuy.GetItemCount(equipmentJustBought) >= 0);                
            }
        }

        protected override void AfterAborted(bool wasStarted)
        {
            //put the items we never bought back into the stores inventory
            foreach (GameItemType itemType in m_leftToBuy.ItemTypes)
            {
                int amountNotBought = m_leftToBuy.GetItemCount(itemType);
                m_game.StoreStock.IncreaseItemCount(itemType, amountNotBought);
            }          
        }


        public override string LongDescription()
        {
            return "Buy Items";
        }

        public override string ShortDescription()
        {
            return "Buy Items";
        }

        public override Color TaskColor()
        {
            return Color.Black;
        }
        


        public override void WriteState(ObjectState state)
        {
            base.WriteState(state);
            state.SetValue("PreferedDestination", m_preferedDestination);
            state.WriteSubState("WhatToBuyList", m_whatToBuy);
            state.WriteSubState("LeftToBuyList", m_leftToBuy);
        }

        public override void ReadState(ObjectState state)
        {
            base.ReadState(state);
            m_preferedDestination = state.GetValue<StorageBuilding>("PreferedDestination");
            m_whatToBuy = state.ReadSubState<ItemList>("WhatToBuyList");
            m_leftToBuy = state.ReadSubState<ItemList>("LeftToBuyList");
        }

    }
}
