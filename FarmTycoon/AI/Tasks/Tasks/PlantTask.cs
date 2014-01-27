using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FarmTycoon
{
    public class PlantTask : Task
    {
        #region Member Vars

        /// <summary>
        /// The PlantedArea the task is for
        /// </summary>
        private Field _plantedArea;

        /// <summary>
        /// What to plant in the area
        /// </summary>
        private ItemType _seedToPlant = null;

        /// <summary>
        /// Should the workers use equipment when planting the field
        /// </summary>
        private bool _useEquipment = false;

        #endregion

        #region Setup

        /// <summary>
        /// Create a new PlantTask.          
        /// </summary>
        public PlantTask() : base() { }

        /// <summary>
        /// Clone the task (only the planning state of the task needs to be cloned)
        /// </summary>
        protected override Task CloneInner()
        {
            PlantTask clone = new PlantTask();
            clone._plantedArea = _plantedArea;
            clone._seedToPlant = _seedToPlant;
            clone._useEquipment = _useEquipment;
            return clone;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the PlantedArea the task will act on
        /// </summary>
        public Field PlantedArea
        {
            set { _plantedArea = value; }
            get { return _plantedArea; }
        }

        /// <summary>
        /// What to plant in the area
        /// </summary>
        public ItemType SeedToPlant
        {
            get { return _seedToPlant; }
            set { _seedToPlant = value; }
        }

        /// <summary>
        /// Should the workers use equipment when planting
        /// </summary>
        public bool UseEquipment
        {
            get { return _useEquipment; }
            set { _useEquipment = value; }
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

            //check for issues with the field that would prevent planning the task
            CheckForFieldIssues(plan);
            if (plan.CanCalculateExpectedTime == false) { return plan; }

            //item planner to plan where to get items from, and equipment planner to plan where to get equipment
            TaskItemPlanner itemPlanner = new TaskItemPlanner();
            TaskEquipmentPlanner equipmentPlanner = new TaskEquipmentPlanner(itemPlanner);
            
            //if using equipment plan for each worker to get a tractor and planter
            if (_useEquipment)
            {
                for (int workerNum = 0; workerNum < _numberOfWorkers; workerNum++)
                {
                    equipmentPlanner.PlanToGetEquipmentIfNeeded(plan, workerNum, EquipmentType.Tractor, _plantedArea.EntryLand.LocationOn);
                    equipmentPlanner.PlanToGetEquipmentIfNeeded(plan, workerNum, EquipmentType.Planter, _plantedArea.EntryLand.LocationOn);
                }
            }


            //trip planner to help split the field among workers
            TaskTripPlanner<Land> tripPlanner = new TaskTripPlanner<Land>();
            tripPlanner.NumberOfWorkers = _numberOfWorkers;
            tripPlanner.ObjectsToVisit = LandToPlant();
            for (int workerNum = 0; workerNum < _numberOfWorkers; workerNum++)
            {
                int capacity = equipmentPlanner.CurrentExpectedCapacity(workerNum);
                tripPlanner.SetMaxObjectsPerTrip(workerNum, capacity / _seedToPlant.Size);
            }
            tripPlanner.SetPlanTripCallback(new PlanTripCallback<Land>(delegate(int workerNum, List<Land> objectsForTrip)
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
        /// Check for issues that prevent planting
        /// </summary>
        /// <param name="plan"></param>
        private void CheckForFieldIssues(TaskPlan plan)
        {
            if (_plantedArea.Crops.Count == _plantedArea.AllLocationsOn.Count)
            {
                plan.AddIssue("Cannot plant while the field is already full.", false);
            }
            if (_plantedArea.Crops.Count > 0 && _plantedArea.CropInfo != null && _plantedArea.CropInfo.Seed != _seedToPlant.BaseType)
            {
                plan.AddIssue("Another type of crop is alreadt in the field.", false);
            }
            if (GameState.Current.MasterTaskList.IsActiveTaskOfTypeDependingOn<PlantTask>(_plantedArea))
            {
                plan.AddIssue("Cannot plant while being planted.", false);
            }
            if (GameState.Current.MasterTaskList.IsActiveTaskOfTypeDependingOn<PlowTask>(_plantedArea))
            {
                plan.AddIssue("Cannot plant while being plowed.", false);
            }
            if (GameState.Current.MasterTaskList.IsActiveTaskOfTypeDependingOn<PickTask>(_plantedArea))
            {
                plan.AddIssue("Cannot plant while being harvested.", false);
            }
            if (_seedToPlant == null || FarmData.Current.GetCropInfoForSeed(_seedToPlant.BaseType) == null)             
            {
                plan.AddIssue("Must select seed to plant.", true);
            }

            ////check if its been a long time since land was plowed
            //foreach (Land land in _field.OrderedLand)
            //{
            //    if (land.DaysSincePlowed > Land.MAX_DAYS_SINCE_PLOWED * 3 / 4)
            //    {
            //        plan.AddWarning("The land has not been plowed for a long time, you may want to plow the land first.");
            //        break;
            //    }
            //}

            ////check if soil is very low on nutraints
            //foreach (Land land in _field.OrderedLand)
            //{
            //    if (land.SoilNutriants < Land.MAX_NUTRIANTS * 1 / 4)
            //    {
            //        plan.AddWarning("The soil is very low on nutraints, you may want to let the field lay fallow for a while.");
            //        break;
            //    }
            //}

        }

        /// <summary>
        /// Get the land int the planted area to plant
        /// </summary>
        private List<Land> LandToPlant()
        {
            //get info for the crop thats going to be planted
            CropInfo cropInfo = FarmData.Current.GetCropInfoForSeed(_seedToPlant.BaseType);
                        
            //list of land we need to plant crops on
            List<Land> landToPlant = new List<Land>();

            //if the crop does not need extra space
            if (cropInfo.NeedsSpace == false)
            {
                //all the land that does not have a crop should be planted
                foreach (Land land in _plantedArea.OrderedLand)
                {
                    if (land.LocationOn.Contains<Crop>()) { continue; }
                    landToPlant.Add(land);
                }
            }
            else
            {
                //for need space crops we want to plant them in a staggard manner

                //determine what arrangement of land in the field allows us to plant the most crops
                bool even, divBy4;
                DetermineBestLandType(out even, out divBy4);

                //for all land that does not already have a crop, and also only land where the x and y are even and add up to a number divisible by 4 (leves space between trees)                
                foreach (Land land in _plantedArea.OrderedLand)
                {
                    if (land.LocationOn.Contains<Crop>()) { continue; }
                    if ((land.LocationOn.X % 2 != 0 || land.LocationOn.Y % 2 != 0) && even) { continue; }
                    if ((land.LocationOn.X % 2 != 1 || land.LocationOn.Y % 2 != 1) && even == false) { continue; }
                    if ((land.LocationOn.X + land.LocationOn.Y) % 4 != 0 && divBy4) { continue; }
                    if ((land.LocationOn.X + land.LocationOn.Y) % 4 != 2 && divBy4 == false) { continue; }
                    landToPlant.Add(land);                    
                }
            }
            return landToPlant;
        }

        /// <summary>        
        /// for spaced crops we can only plant crops on one of these 4 subcatagories of land, determine which one will be able to hold the most crops
        /// </summary>
        private void DetermineBestLandType(out bool even, out bool divBy4)
        {
            //we can only plant crop on one of these 4 subcatagories of land, determine which one will be able to hold the most crops
            int evenDiv4LandCount = 0;
            int evenNonDiv4LandCount = 0;
            int oddDiv4LandCount = 0;
            int oddNonDiv4LandCount = 0;
            foreach (Land land in _plantedArea.OrderedLand)
            {
                if (land.LocationOn.X % 2 == 0 && land.LocationOn.Y % 2 == 0 && (land.LocationOn.X + land.LocationOn.Y) % 4 == 0)
                {
                    evenDiv4LandCount++;
                }
                if (land.LocationOn.X % 2 == 0 && land.LocationOn.Y % 2 == 0 && (land.LocationOn.X + land.LocationOn.Y) % 4 == 2)
                {
                    evenNonDiv4LandCount++;
                }
                if (land.LocationOn.X % 2 == 1 && land.LocationOn.Y % 2 == 1 && (land.LocationOn.X + land.LocationOn.Y) % 4 == 0)
                {
                    oddDiv4LandCount++;
                }
                if (land.LocationOn.X % 2 == 1 && land.LocationOn.Y % 2 == 1 && (land.LocationOn.X + land.LocationOn.Y) % 4 == 2)
                {
                    oddNonDiv4LandCount++;
                }
            }

            if (evenDiv4LandCount > evenNonDiv4LandCount && evenDiv4LandCount > oddDiv4LandCount && evenDiv4LandCount > oddNonDiv4LandCount)
            {
                even = true;
                divBy4 = true;
            }
            else if (evenNonDiv4LandCount > oddDiv4LandCount && evenNonDiv4LandCount > oddNonDiv4LandCount)
            {
                even = true;
                divBy4 = false;
            }
            else if (oddDiv4LandCount > oddNonDiv4LandCount)
            {
                even = false;
                divBy4 = true;
            }
            else
            {
                even = false;
                divBy4 = false;
            }
        }
        
        /// <summary>
        /// Plan one trip for the worker passed, that plants the land passed
        /// </summary>
        private void PlanTrip(TaskPlan plan, int workerNum, List<Land> tripLand, TaskItemPlanner itemPlanner)
        {
            //the amount of seed we will need for this trip
            int amountOfSeedForThisTrip = tripLand.Count;

            //create a list of the seed we need to get
            ItemList seedToGet = new ItemList();
            seedToGet.IncreaseItemCount(_seedToPlant, amountOfSeedForThisTrip); 

            //plan where to get that much seed from
            itemPlanner.PlanToGetItems(plan, workerNum, seedToGet, _plantedArea.EntryLand.LocationOn);
            
            //create plant field action
            PlantAction plantFieldTask = new PlantAction(tripLand, _seedToPlant, _plantedArea);
            plan.AddAction(workerNum, plantFieldTask);
        }

        #endregion

        #region Other

        public override bool DependsOnObject(IGameObject obj)
        {
            if (obj == _plantedArea) { return true; }
            return false;
        }

        public override string Description()
        {
            int plantCount = _plantedArea.OrderedLand.Count - _plantedArea.Crops.Count;
            return "Plant " + _seedToPlant.FullName + "(" + plantCount.ToString() + ") in " + _plantedArea.Name;
        }
        

        #endregion

        #endregion

        #region Save Load
		public override void WriteStateV1(StateWriterV1 writer)
		{
			base.WriteStateV1(writer);
			writer.WriteObject(_plantedArea);
			writer.WriteObject(_seedToPlant);
			writer.WriteBool(_useEquipment);
		}
		
		public override void ReadStateV1(StateReaderV1 reader)
		{
			base.ReadStateV1(reader);
			_plantedArea = reader.ReadObject<Field>();
			_seedToPlant = reader.ReadObject<ItemType>();
			_useEquipment = reader.ReadBool();
		}
		
		public override void AfterReadStateV1()
		{
			base.AfterReadStateV1();
		}
		#endregion

    }
}
