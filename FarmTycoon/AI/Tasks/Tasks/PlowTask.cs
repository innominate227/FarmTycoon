using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FarmTycoon
{
    public class PlowTask : Task
    {
        #region Member Vars

        /// <summary>
        /// The planted area being plowed
        /// </summary>
        private Field _field;

        /// <summary>
        /// Should the workers use equipment when planting the field
        /// </summary>
        private bool _useEquipment = false;

        #endregion

        #region Setup

        /// <summary>
        /// Create a new PlowTask.          
        /// </summary>
        public PlowTask() : base() { }

        /// <summary>
        /// Clone the task (only the planning state of the task needs to be cloned)
        /// </summary>
        protected override Task CloneInner()
        {
            PlowTask clone = new PlowTask();
            clone._field = _field;
            clone._useEquipment = _useEquipment;
            return clone;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the field the task will act on
        /// </summary>
        public Field PlantedArea
        {
            set { _field = value; }
            get { return _field; }
        }

        /// <summary>
        /// Should the workers use equipment when planting the field
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

            //check for issues that would prevent planning the task
            CheckForFieldIssues(plan);
            if (plan.CanCalculateExpectedTime == false) { return plan; }

            //item planner to plan where to get items from, and equipment planner to plan where to get equipment
            TaskItemPlanner itemPlanner = new TaskItemPlanner();
            TaskEquipmentPlanner equipmentPlanner = new TaskEquipmentPlanner(itemPlanner);
            
            //if using equipment plan for each worker to get a tractor and plow
            if (_useEquipment)
            {
                for (int workerNum = 0; workerNum < _numberOfWorkers; workerNum++)
                {
                    equipmentPlanner.PlanToGetEquipmentIfNeeded(plan, workerNum, EquipmentType.Tractor, _field.EntryLand.LocationOn);
                    equipmentPlanner.PlanToGetEquipmentIfNeeded(plan, workerNum, EquipmentType.Plow, _field.EntryLand.LocationOn);
                }
            }


            //trip planner to help split the area among workers
            TaskTripPlanner<Land> tripPlanner = new TaskTripPlanner<Land>();
            tripPlanner.NumberOfWorkers = _numberOfWorkers;
            tripPlanner.ObjectsToVisit = _field.OrderedLand;
            tripPlanner.SetMaxObjectsPerTripForAll(int.MaxValue); //no limit to the number of spaces each worker can plow on a trip
            tripPlanner.SetPlanTripCallback(new PlanTripCallback<Land>(delegate(int workerNum, List<Land> objectsForTrip)
            {
                PlanTrip(plan, workerNum, objectsForTrip);
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


        private void CheckForFieldIssues(TaskPlan plan)
        {
            if (_field.Crops.Count > 0)
            {
                plan.AddIssue("Cannot plow while crops are planted.", false);
            }
            if (GameState.Current.MasterTaskList.IsActiveTaskOfTypeDependingOn<PlantTask>(_field))
            {
                plan.AddIssue("Cannot plow while being planted.", false);
            }
            if (GameState.Current.MasterTaskList.IsActiveTaskOfTypeDependingOn<PickTask>(_field))
            {
                plan.AddIssue("Cannot plant while being harvested.", false);
            }

        }


        private void PlanTrip(TaskPlan plan, int workerNum, List<Land> tripLand)
        {
            //add an action to plow that section of the field
            plan.AddAction(workerNum, new PlowAction(tripLand, _field));
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
            return "Plow " + _field.Name;
        }

        #endregion

        #endregion

        #region Save Load
		public override void WriteStateV1(StateWriterV1 writer)
		{
			base.WriteStateV1(writer);
			writer.WriteObject(_field);
			writer.WriteBool(_useEquipment);
		}
		
		public override void ReadStateV1(StateReaderV1 reader)
		{
			base.ReadStateV1(reader);
			_field = reader.ReadObject<Field>();
			_useEquipment = reader.ReadBool();
		}
		
		public override void AfterReadStateV1()
		{
			base.AfterReadStateV1();
		}
		#endregion

    }
}
