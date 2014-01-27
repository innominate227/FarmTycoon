using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace FarmTycoon
{
    public class PickTask : Task
    {
        #region Member Vars

        /// <summary>
        /// The field the task is for
        /// </summary>
        private Field _field;

        /// <summary>
        /// Should the workers use equipment when picking/harvesting
        /// </summary>
        private bool _useEquipment = false;

        /// <summary>
        /// Should we harvest (true) or just pick (false)
        /// </summary>
        private bool _harvest = false;

        #endregion

        #region Setup

        /// <summary>
        /// Create a new PickTask.          
        /// </summary>
        public PickTask() : base() { }

        /// <summary>
        /// Clone the task (only the planning state of the task needs to be cloned)
        /// </summary>
        protected override Task CloneInner()
        {
            PickTask clone = new PickTask();
            clone._field = _field;
            clone._useEquipment = _useEquipment;
            clone._harvest = _harvest;
            return clone;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the Field the task will act on
        /// </summary>
        public Field Field
        {
            set { _field = value; }
            get { return _field; }
        }

        /// <summary>
        /// Should the workers use equipment when picking/harvesting
        /// </summary>
        public bool UseEquipment
        {
            get { return _useEquipment; }
            set { _useEquipment = value; }
        }

        /// <summary>
        /// Should we harvest (true) or just pick (false)
        /// </summary>
        public bool Harvest
        {
            get { return _harvest; }
            set { _harvest = value; }
        }

        #endregion

        #region Logic
        
        #region Planning

        /// <summary>
        /// Plan out the task
        /// </summary>
        protected override TaskPlan PlanTaskInner()
        {
            //create task plan
            TaskPlan plan = new TaskPlan(this);

            //no equipment for picking only for harvesting
            if (_harvest == false)
            {
                _useEquipment = false;
            }

            //check for issues with the field that would prevent planning the task
            CheckForFieldIssues(plan);
            if (plan.CanCalculateExpectedTime == false) { return plan; }
            
            //item planner to plan where to get items from
            TaskItemPlanner itemPlanner = new TaskItemPlanner();
            TaskEquipmentPlanner equipmentPlanner = new TaskEquipmentPlanner(itemPlanner);

            //if using equipment plan for each worker to get a harvester and trailer
            if (_useEquipment)
            {
                for (int workerNum = 0; workerNum < _numberOfWorkers; workerNum++)
                {                    
                    equipmentPlanner.PlanToGetEquipmentIfNeeded(plan, workerNum, EquipmentType.Harvester, _field.EntryLand.LocationOn);
                    equipmentPlanner.PlanToGetEquipmentIfNeeded(plan, workerNum, EquipmentType.Trailer, _field.EntryLand.LocationOn);
                }
            }

            //objects in the field to visit
            IList<Crop> toVisit = DetermineObjectsToVisit();

            //if nothing to visit that mean there are tree in the field but they are not ready to harvest yet (we check for tree in the CheckForFieldIssues function)
            if (toVisit.Count == 0)
            {
                plan.AddIssue("Nothing in field is ready to be picked.", true);
                return plan;
            }
            else if (toVisit.Count < _field.Crops.Count)
            {
                int diff = _field.Crops.Count - toVisit.Count;
                plan.AddWarning(diff.ToString() + " crops in the field are NOT ready to be picked.");
            }
            
            //get the size of what we will be picking/harvesting
            int sizeOfCropInField = 1;
            if (_field.CropInfo != null)
            {
                if (_harvest && _field.CropInfo.HarvestItem != null)
                {
                    sizeOfCropInField = _field.CropInfo.HarvestItem.Size;
                }
                else if (_field.CropInfo.PickItem != null)
                {
                    sizeOfCropInField = _field.CropInfo.PickItem.Size;
                }
            }

            //trip planner to help split the crops to pick among workers
            TaskTripPlanner<Crop> tripPlanner = new TaskTripPlanner<Crop>();
            tripPlanner.NumberOfWorkers = _numberOfWorkers;
            tripPlanner.ObjectsToVisit = toVisit;
            for (int workerNum = 0; workerNum < _numberOfWorkers; workerNum++)
            {
                int capacity = equipmentPlanner.CurrentExpectedCapacity(workerNum);
                tripPlanner.SetMaxObjectsPerTrip(workerNum, capacity / sizeOfCropInField);
            }
            tripPlanner.SetPlanTripCallback(new PlanTripCallback<Crop>(delegate(int workerNum, List<Crop> objectsForTrip)
            {
                PlanTrip(plan, workerNum, objectsForTrip, itemPlanner);
            }));
            tripPlanner.PlanTrips();

            //plan to put all the equipment back
            if (_useEquipment)
            {
                equipmentPlanner.PlanToPutAllEquipmentBack(plan);
            }

            //return the plan
            return plan;
        }


        /// <summary>
        /// Determine the list of crops in the field to be pick/harvest
        /// </summary>
        private IList<Crop> DetermineObjectsToVisit()
        {            
            if (_harvest)
            {
                //retrun all the crops all will be harvested
                return _field.Crops;
            }
            else
            {
                //just return the crops that are ready to be picked
                List<Crop> readyCrops = new List<Crop>();
                foreach (Crop crop in _field.Crops)
                {
                    if (crop.CanPick)
                    {
                        readyCrops.Add(crop);
                    }
                }                
                return readyCrops;
            }            
        }


        private void CheckForFieldIssues(TaskPlan plan)
        {
            string verbString = "pick";
            string verbPastString = "picked";
            if (_harvest)
            {
                verbString = "harvest";
                verbPastString = "harvested";
            }

            //see if we are not doing something that would prevent the task from happening            
            if (_field.Crops.Count == 0)
            {
                plan.AddIssue("Nothing is planted in field so it cannot be " + verbPastString + ".", true);
            }
            if (GameState.Current.MasterTaskList.IsActiveTaskOfTypeDependingOn<PlowTask>(_field))
            {
                plan.AddIssue("Cannot " + verbString + " while being plowing.", false);
            }
            if (GameState.Current.MasterTaskList.IsActiveTaskOfTypeDependingOn<PlantTask>(_field))
            {
                plan.AddIssue("Cannot " + verbString + " while being planted.", false);
            }
            if (GameState.Current.MasterTaskList.IsActiveTaskOfTypeDependingOn<PickTask>(_field))
            {
                plan.AddIssue("Cannot " + verbString + " while being picked.", false);
            }
        }
        
        private void PlanTrip(TaskPlan plan, int workerNum, List<Crop> tripCrops, TaskItemPlanner itemPlanner)
        {
            //create harvest field action
            PickAction harvestFieldAction = new PickAction(tripCrops, _field, _harvest);
            plan.AddAction(workerNum, harvestFieldAction);

            //get what type of item that will be harvested 
            //it doesnt matter what quality level we get for it, the put event will subsitute others if needed
            string typeNameGottenFromHarvest = _field.CropInfo.PickItem.Name;            
            if (_harvest)
            {
                typeNameGottenFromHarvest = _field.CropInfo.HarvestItem.Name;                
            }
            ItemType cropGottenFromHarvest = GameState.Current.ItemPool.GetItemType(typeNameGottenFromHarvest, 0);
                        
            //create a item list of the crop we will get from the field, we will get one unit of crop for each tile harvested
            ItemList cropToPut = new ItemList();
            cropToPut.IncreaseItemCount(cropGottenFromHarvest, tripCrops.Count);

            //plan to put the items into a building
            itemPlanner.PlanToPutItems(plan, workerNum, cropToPut, true, _field.EntryLand.LocationOn);
        }

        #endregion

        #region Other

        public override bool DependsOnObject(IGameObject obj)
        {
            if (obj == _field) { return true; }
            return false;
        }

        public override string Description()
        {
            string cropInFieldString = _field.TypePlanted;
            int cropCount = _field.Crops.Count;
            return "Harvest " + cropInFieldString + "(" + cropCount.ToString() + ") from " + _field.Name;
        }

        #endregion

        #endregion

        #region Save Load
		public override void WriteStateV1(StateWriterV1 writer)
		{
			base.WriteStateV1(writer);
			writer.WriteObject(_field);
			writer.WriteBool(_useEquipment);
			writer.WriteBool(_harvest);
		}
		
		public override void ReadStateV1(StateReaderV1 reader)
		{
			base.ReadStateV1(reader);
			_field = reader.ReadObject<Field>();
			_useEquipment = reader.ReadBool();
			_harvest = reader.ReadBool();
		}
		
		public override void AfterReadStateV1()
		{
			base.AfterReadStateV1();
		}
		#endregion

    }
}
