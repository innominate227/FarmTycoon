using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace FarmTycoon
{
    public partial class MoveItemsTask : Task
    {
        #region Member Vars

        /// <summary>
        /// The building that is the source for the items to be moved to
        /// </summary>
        private IStorageBuilding _source;

        /// <summary>
        /// The building that is the prefered destination for the items to be moved to
        /// </summary>
        private IStorageBuilding _preferedDestination;

        /// <summary>
        /// Items to move
        /// </summary>
        private ItemList _whatToMove = new ItemList();

        /// <summary>
        /// Are we using the items (or if not we are moving them).
        /// Moving means the items will going into a stroage building, while using means the items will go into a prodcution budiling
        /// </summary>
        private bool _useTask = false;


        #endregion

        #region Setup

        /// <summary>
        /// Create a new task to move items, call Setup or ReadState before using
        /// </summary>
        public MoveItemsTask() : base() { }

        /// <summary>
        /// Clone the task (only the planning state of the task needs to be cloned)
        /// </summary>
        protected override Task CloneInner()
        {
            MoveItemsTask clone = new MoveItemsTask();
            clone._source = _source;
            clone._preferedDestination = _preferedDestination;
            clone._whatToMove = _whatToMove;
            clone._useTask = _useTask;
            return clone;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Items to move
        /// </summary>
        public ItemList WhatToMove
        {
            get { return _whatToMove; }            
        }
        
        /// <summary>
        /// The building that is the source for the items to be moved to
        /// </summary>
        public IStorageBuilding Source
        {
            get { return _source; }
            set { _source = value; }
        }

        /// <summary>
        /// The building that is the prefered destination for the items that are to be moved
        /// </summary>
        public IStorageBuilding PreferedDestination
        {
            get { return _preferedDestination; }
            set { _preferedDestination = value; }
        }

        /// <summary>
        /// Are we using the items (or if not we are moving them).
        /// Moving means the items will going into a stroage building, while using means the items will go into a prodcution budiling
        /// </summary>
        public bool UseTask
        {
            get { return _useTask; }
            set { _useTask = value; }
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


            // Determine the list of items that should actually be moved, based on what is still in the source building
            ItemList whatToMoveAdjusted = DetermineActualMoveList(plan);


            //we will want to put all the items we move in buildings near to the source area or near to the prefered destination if one is specified
            Location putItemsNear = _source.ActionLocation;
            if (_preferedDestination != null)
            {
                putItemsNear = _preferedDestination.ActionLocation;
            }


            //Determine what predicate to use to filter to possible building we could take the items to.
            GameObjectPredicate<IStorageBuilding> whereToPutItemsPredicate = DetermineDestinationPredicate();


            //used to plan what each worker should move on each trip
            TaskItemDivider itemDivider = new TaskItemDivider(whatToMoveAdjusted);
            //used to plan where to take the items
            TaskItemPlanner itemPlanner = new TaskItemPlanner();
            //if we are moving tows without tractores we need to plan where to get tractors to move the tows
            TaskEquipmentPlanner equipmentPlanner = new TaskEquipmentPlanner(itemPlanner);
            
                                                                
            //the worker to have move something next
            int workerNum = 0;

            //keep getting loads to move until none are left
            ItemList toMove = itemDivider.NextLoad();
            while (toMove != null)
            {
                //determine if we have a list of normal items or a list of equipment
                bool isEquipment = (FarmData.Current.GetEquipmentInfoForItemInfo(toMove.ItemTypes[0].BaseType) != null);
                if (isEquipment)
                {
                    PlanEquipmentItemsTrip(plan, itemPlanner, equipmentPlanner, workerNum, toMove, putItemsNear, whereToPutItemsPredicate);                    
                }
                else
                {
                    PlanNormalItemsTrip(plan, itemPlanner, workerNum, toMove, putItemsNear, whereToPutItemsPredicate);
                }

                //have the next worker find something to get
                workerNum++;
                if (workerNum >= _numberOfWorkers)
                {
                    workerNum = 0;
                }

                //figure out what the next worker will be getting
                toMove = itemDivider.NextLoad();
            }
            
            //put any tractors back that were used to tow a tow back where we got them
            equipmentPlanner.PlanToPutAllEquipmentBack(plan);
            
            return plan;
        }


        /// <summary>
        /// Determine the list of items that should actually be moved, based on what is still in the source building
        /// </summary>
        private ItemList DetermineActualMoveList(TaskPlan plan)
        {
            //its possible that items have been reserved in the building or, removed from it since we set up the move task.
            //in that case we just want to move as much as possible
            bool reducedMoveList = false;
            ItemList whatToMoveAdjusted = new ItemList();
            foreach (ItemType itemType in _whatToMove.ItemTypes)
            {
                int amountToMove = _whatToMove.GetItemCount(itemType);
                int amountAvailable = _source.Inventory.GetTypeCountThatsFree(itemType);
                if (amountToMove > amountAvailable)
                {
                    amountToMove = amountAvailable;
                    reducedMoveList = true;
                }
                if (amountToMove > 0)
                {
                    whatToMoveAdjusted.SetItemCount(itemType, amountToMove);
                }
            }

            //if we had to reduce add a warning
            if (reducedMoveList)
            {
                plan.AddWarning("Unable to move all the items requested because items are no longer in the building or reserved");
            }

            //returnt he adjusted list
            return whatToMoveAdjusted;
        }


        /// <summary>
        /// Determine what predicate to use to filter to possible building we could take the items to.
        /// </summary>
        private GameObjectPredicate<IStorageBuilding> DetermineDestinationPredicate()
        {
            GameObjectPredicate<IStorageBuilding> predicate = null;
            if (_useTask == false)
            {
                //if this is a normal move task we must put the items into a StorageBuilding or Pasture.
                predicate = delegate(IStorageBuilding building)
                {
                    return (building is StorageBuilding || building is Pasture);
                };
            }
            else
            {
                //this is a use task we must put the items into a Production building.
                //if a prefered production building was selected it should be a production building of the same type.
                if (_preferedDestination != null)
                {
                    predicate = delegate(IStorageBuilding building)
                    {
                        return (building is ProductionBuilding && (building as ProductionBuilding).BuildingInfo == (_preferedDestination as ProductionBuilding).BuildingInfo);
                    };
                }
                else
                {
                    predicate = delegate(IStorageBuilding building)
                    {
                        return (building is ProductionBuilding);
                    };
                }
            }
            return predicate;
        }


        /// <summary>
        /// Plan a trip for getting normal items
        /// </summary>
        private void PlanNormalItemsTrip(TaskPlan plan, TaskItemPlanner itemPlanner, int workerNum, ItemList toGet, Location putItemsNear, GameObjectPredicate<IStorageBuilding> whereToPutItemsPredicate)
        {
            //create action to get the items needed from the source area, and add to the plan
            GetItemsAction purchaseItemsAction = new GetItemsAction(_source, toGet);
            plan.AddAction(workerNum, purchaseItemsAction);

            //use the item planner to plan locations to put the items
            itemPlanner.PlanToPutItems(plan, workerNum, toGet, false, putItemsNear, whereToPutItemsPredicate);
        }
        

        /// <summary>
        /// Plan a trip for getting equipment items
        /// </summary>
        private void PlanEquipmentItemsTrip(TaskPlan plan, TaskItemPlanner itemPlanner, TaskEquipmentPlanner equipmentPlanner, int workerNum, ItemList toGet, Location putItemsNear, GameObjectPredicate<IStorageBuilding> whereToPutItemsPredicate)
        {
            //each worker will get a tractor, and a tow if possible.
            ItemType vehicleToMove = null;
            ItemType towToMove = null;
                
            //get the vheicle and tow they are moving
            foreach (ItemType itemType in toGet.ItemTypes)
            {
                EquipmentInfo equipmentInfo = FarmData.Current.GetEquipmentInfoForItemInfo(itemType.BaseType);
                if (equipmentInfo.IsVehicle)
                {
                    vehicleToMove = itemType;
                }
                else
                {
                    towToMove = itemType;                    
                }
            }

            //create an item list with just the two/just the vehicle, because the GetItemsAction wants a list
            ItemList vehicleToMoveList = new ItemList();
            if (vehicleToMove != null)
            {
                vehicleToMoveList.IncreaseItemCount(vehicleToMove, 1);
            }
            ItemList towToMoveList = new ItemList();
            if (towToMove != null)
            {
                towToMoveList.IncreaseItemCount(towToMove, 1);
            }
            
            //if worker is moving both tractor and tow he can move them both in one trip
            if (vehicleToMove != null && towToMove != null)
            {
                //get the tractor first
                GetItemsAction getTractor = new GetItemsAction(_source, vehicleToMoveList);
                plan.AddAction(workerNum, getTractor);
                    
                //get the tow next
                GetItemsAction getTow = new GetItemsAction(_source, towToMoveList);
                plan.AddAction(workerNum, getTow);

                //find a place for the tow
                itemPlanner.PlanToPutItems(plan, workerNum, towToMoveList, false, putItemsNear, whereToPutItemsPredicate);

                //find a place for the tractor
                itemPlanner.PlanToPutItems(plan, workerNum, vehicleToMoveList, false, putItemsNear, whereToPutItemsPredicate);
            }
            //worker is just moving a tractor
            else if (vehicleToMove != null && towToMove == null)
            {
                //get the tractor
                GetItemsAction getTractor = new GetItemsAction(_source, vehicleToMoveList);
                plan.AddAction(workerNum, getTractor);

                //find a place for the tractor
                itemPlanner.PlanToPutItems(plan, workerNum, vehicleToMoveList, false, putItemsNear, whereToPutItemsPredicate);
            }
            //worker is just moving a tow
            else if (vehicleToMove == null && towToMove != null)
            {
                //get the worker a vehicle if they dont have one
                equipmentPlanner.PlanToGetVehicleIfNeeded(plan, workerNum, putItemsNear);
                    
                //get the tow
                GetItemsAction getTow = new GetItemsAction(_source, towToMoveList);
                plan.AddAction(workerNum, getTow);

                //find a place to put the tow   
                itemPlanner.PlanToPutItems(plan, workerNum, towToMoveList, false, putItemsNear, whereToPutItemsPredicate);                     
            }  
        }

        #endregion

        #region Other

        public override bool DependsOnObject(IGameObject obj)
        {
            if (obj == _source) { return true; }
            if (obj == _preferedDestination) { return true; }
            return false;
        }

        public override string Description()
        {
            string description = "Move";
            foreach (ItemType itemType in _whatToMove.ItemTypes)
            {
                int amount = _whatToMove.GetItemCount(itemType);
                description += " " + itemType.FullName + "(" + amount.ToString() + ")";
            }

            description += " from " + _source.Name;

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
			writer.WriteObject(_source);
			writer.WriteObject(_preferedDestination);
			writer.WriteObject(_whatToMove);
			writer.WriteBool(_useTask);
		}
		
		public override void ReadStateV1(StateReaderV1 reader)
		{
			base.ReadStateV1(reader);
			_source = reader.ReadObject<IStorageBuilding>();
			_preferedDestination = reader.ReadObject<IStorageBuilding>();
			_whatToMove = reader.ReadObject<ItemList>();
			_useTask = reader.ReadBool();
		}
		
		public override void AfterReadStateV1()
		{
			base.AfterReadStateV1();
		}
		#endregion

    }
}
