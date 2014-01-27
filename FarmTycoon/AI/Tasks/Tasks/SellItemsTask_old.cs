using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FarmTycoon
{
    public class SellItemsTask : Task
    {
        /// <summary>
        /// The building that is the prefered source for the items that are to be bought
        /// </summary>
        protected StorageBuilding m_preferedSource;

        /// <summary>
        /// Items to sell
        /// </summary>
        protected ItemList m_whatToSell = new ItemList();

        
        /// <summary>
        /// Create a new SellItemsTask.  
        /// </summary>
        public SellItemsTask() : base() { }
        
        /// <summary>
        /// What to sell
        /// </summary>
        public ItemList WhatToSell
        {
            set { m_whatToSell = value; }
            get { return m_whatToSell; }
        }

        /// <summary>
        /// The building that is the prefered source for the items that are to be bought, null for no prefered source
        /// </summary>
        public StorageBuilding PreferedSource
        {
            get { return m_preferedSource; }
            set { m_preferedSource = value; }
        }

        /// <summary>
        /// Plan out the task
        /// </summary>
        protected override TaskPlan PlanTaskInner()
        {
            //used to plan where to get the items we are selling
            TaskItemPlanner itemPlanner = new TaskItemPlanner();
            //if we are selling tows without tractores we need to plan where to get tractors to move the tows
            TaskEquipmentPlanner equipmentPlanner = new TaskEquipmentPlanner(itemPlanner);

            //create the TaskPlan to return
            TaskPlan plan = new TaskPlan(this);

            //create a list of the items that still need to be sold and the equipment that still needs to be sold
            ItemList itemsLeftToSell = new ItemList();
            ItemList equipmentLeftToSell = new ItemList();
            foreach (GameItemType itemType in m_whatToSell.ItemTypes)
            {
                if (itemType.Class == ItemClass.Equipment)
                {
                    equipmentLeftToSell.SetItemCount(itemType, m_whatToSell.GetItemCount(itemType));
                }
                else
                {
                    itemsLeftToSell.SetItemCount(itemType, m_whatToSell.GetItemCount(itemType));
                }
            }

            //find the delivery area building
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

            //we will want to get all the items we sell from buildings near to the store or near to the prefered source if one is specified
            Location getItemsNear = store.LocationOn;
            if (m_preferedSource != null)
            {
                getItemsNear = m_preferedSource.LocationOn;
            }

            //the worker to have sell something next
            int workerNum = 0;

            //keep going until all the items that need to be sold have been planned for
            while (itemsLeftToSell.ItemTypes.Count > 0)
            {
                //have each worker sell as much as it can hold
                ItemList willSell = new ItemList();
                int spaceLeft = Worker.INVENTORY_SIZE;

                //foreach type that still needs to be sold
                foreach (GameItemType itemType in itemsLeftToSell.ItemTypes)
                {
                    //get the size of the item
                    int itemSize = itemType.Size;

                    //get the amount of this item that still needs to be sold
                    int amountToSell = itemsLeftToSell.GetItemCount(itemType);

                    //get the amount the worker can fit (or if more than the amount left get the amount left)
                    int amountCanFit = spaceLeft / itemSize;
                    if (amountCanFit > amountToSell) { amountCanFit = amountToSell; }

                    //have the worker add that to the list of items he will sell
                    if (amountCanFit > 0)
                    {
                        //add to workers will buy list
                        willSell.AddItem(itemType);
                        willSell.SetItemCount(itemType, amountCanFit);

                        //reduce space worker has left
                        spaceLeft -= (itemSize * amountCanFit);

                        //remove that amount from left to sell
                        itemsLeftToSell.DecreaseItemCount(itemType, amountCanFit);
                    }

                    //remove the item from leftToSell if all have been bought
                    if (itemsLeftToSell.GetItemCount(itemType) == 0)
                    {
                        itemsLeftToSell.RemoveItem(itemType);
                    }

                    //if the worker cant carry any more stop looking for items for him to carry
                    if (spaceLeft == 0)
                    {
                        break;
                    }
                }

                //use the item planner to plan locations to get the items that we will sell
                itemPlanner.PlanToGetItems(plan, workerNum, willSell, getItemsNear);

                //create action to sell the items, and add to the plan
                SellItemsAction sellItemsAction = new SellItemsAction(store);
                plan.AddAction(workerNum, sellItemsAction);
                
                //have the next worker find something to sell now
                workerNum++;
                if (workerNum >= m_numberOfWorkers)
                {
                    workerNum = 0;
                }
            } //while items left to sell




            //keep going until all the equipment that needs to be sold has been planned for
            while (equipmentLeftToSell.ItemTypes.Count > 0)
            {
                //each worker will sell a tractor, and a tow if possible.
                //they might sell just a tractor if no tows left, or just a tow if no tractors left.
                EquipmentType tractorToSell = null;
                EquipmentType towToSell = null;

                //foreach type of equipmnet that still needs to be sold
                foreach (GameItemType itemType in equipmentLeftToSell.ItemTypes)
                {
                    EquipmentType equipmentType = Program.DataFiles.EquipmentFile.GetEquipmentType(itemType);

                    if (equipmentType.IsVehicle && tractorToSell == null)
                    {
                        tractorToSell = equipmentType;
                        equipmentLeftToSell.DecreaseItemCount(itemType, 1);
                    }
                    else if (equipmentType.IsVehicle == false && towToSell == null)
                    {
                        towToSell = equipmentType;
                        equipmentLeftToSell.DecreaseItemCount(itemType, 1);
                    }

                    //remove the equipmnet from leftToSell if all have been bought
                    if (equipmentLeftToSell.GetItemCount(itemType) == 0)
                    {
                        equipmentLeftToSell.RemoveItem(itemType);
                    }
                }

                //if worker is selling both tractor and tow he can get them both in one trip
                if (tractorToSell != null && towToSell != null)
                {
                    //find a place to get the tractor
                    itemPlanner.PlanToGetEquipment(plan, workerNum, tractorToSell, getItemsNear);

                    //find a place to get the tow
                    itemPlanner.PlanToGetEquipment(plan, workerNum, towToSell, getItemsNear);

                    //sell the tow first
                    SellEquipmentAction sellTow = new SellEquipmentAction(store, towToSell);
                    plan.AddAction(workerNum, sellTow);

                    //sell the tractor next
                    SellEquipmentAction sellTractor = new SellEquipmentAction(store, tractorToSell);
                    plan.AddAction(workerNum, sellTractor);
                }
                //worker is just selling a tractor
                else if (tractorToSell != null && towToSell == null)
                {
                    //find a place to get the tractor
                    itemPlanner.PlanToGetEquipment(plan, workerNum, tractorToSell, getItemsNear);
                    
                    //sell the tractor
                    SellEquipmentAction sellTractor = new SellEquipmentAction(store, tractorToSell);
                    plan.AddAction(workerNum, sellTractor);
                }
                //worker is just selling a tow
                else if (tractorToSell == null && towToSell != null)
                {
                    //get the worker a tractor is they dont have one, so they can use it to move the tow
                    equipmentPlanner.PlanToGetEquipmentIfNeeded(plan, workerNum, "Tractor", store.LocationOn);

                    //find a place to get the tow
                    itemPlanner.PlanToGetEquipment(plan, workerNum, towToSell, getItemsNear);

                    //sell the tow
                    SellEquipmentAction sellTow = new SellEquipmentAction(store, towToSell);
                    plan.AddAction(workerNum, sellTow);
                }
                
                //have the next worker find something to sell now
                workerNum++;
                if (workerNum >= m_numberOfWorkers)
                {
                    workerNum = 0;
                }
            } //while equipment left to sell


            //put all equipmwnt that was gotten for the task back
            equipmentPlanner.PlanToPutAllEquipmentBack(plan);


            return plan;
        }


        public override string LongDescription()
        {
            return "Sell Items";
        }

        public override string ShortDescription()
        {
            return "Sell Items";
        }

        public override Color TaskColor()
        {
            return Color.Black;
        }


        


        
        public override void WriteState(ObjectState state)
        {
            base.WriteState(state);
            state.SetValue("PreferedSource", m_preferedSource);
            state.WriteSubState("WhatToSell", m_whatToSell);            
        }

        public override void ReadState(ObjectState state)
        {
            base.ReadState(state);
            m_preferedSource = state.GetValue<StorageBuilding>("PreferedSource");
            m_whatToSell = state.ReadSubState<ItemList>("WhatToSell");
        }
    }
}
