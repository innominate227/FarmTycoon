using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FarmTycoon
{
    public partial class SprayTask : Task
    {
        #region Member Vars

        /// <summary>
        /// The area object that is being sprayed
        /// </summary>
        private Field _fieldBeingSprayed;
        
        /// <summary>
        /// What to spray in the field
        /// </summary>
        private ItemType _whatToSpray = null;

        /// <summary>
        /// Should the workers use equipment when spraying the objects
        /// </summary>
        private bool _useEquipment = false;

        #endregion

        #region Setup

        /// <summary>
        /// Create a new SprayTask.          
        /// </summary>
        public SprayTask() : base() { }

        /// <summary>
        /// Clone the task (only the planning state of the task needs to be cloned)
        /// </summary>
        protected override Task CloneInner()
        {
            SprayTask clone = new SprayTask();
            clone._fieldBeingSprayed = _fieldBeingSprayed;
            clone._whatToSpray = _whatToSpray;
            clone._useEquipment = _useEquipment;
            return clone;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The area object that is being sprayed
        /// </summary>
        public Field AreaBeingSprayed
        {
            set { _fieldBeingSprayed = value; }
            get { return _fieldBeingSprayed; }
        }

        /// <summary>
        /// What item to spray in the field
        /// </summary>
        public ItemType WhatToSpray
        {
            get { return _whatToSpray; }
            set { _whatToSpray = value; }
        }

        /// <summary>
        /// Should the workers use equipment when spraying the objects
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

            //if using equipment plan for each worker to get a tractor and sprayer
            if (_useEquipment)
            {
                for (int workerNum = 0; workerNum < _numberOfWorkers; workerNum++)
                {
                    equipmentPlanner.PlanToGetEquipmentIfNeeded(plan, workerNum, EquipmentType.Tractor, _fieldBeingSprayed.ActionLocation);
                    equipmentPlanner.PlanToGetEquipmentIfNeeded(plan, workerNum, EquipmentType.Sprayer, _fieldBeingSprayed.ActionLocation);
                }
            }


            //trip planner to help split the field among workers
            TaskTripPlanner<IHasActionLocation> tripPlanner = new TaskTripPlanner<IHasActionLocation>();
            tripPlanner.NumberOfWorkers = _numberOfWorkers;
            tripPlanner.ObjectsToVisit = _fieldBeingSprayed.Crops.Cast<IHasActionLocation>().ToList();
            for (int workerNum = 0; workerNum < _numberOfWorkers; workerNum++)
            {
                int capacity = equipmentPlanner.CurrentExpectedCapacity(workerNum);
                tripPlanner.SetMaxObjectsPerTrip(workerNum, capacity / _whatToSpray.Size);
            }
            tripPlanner.SetPlanTripCallback(new PlanTripCallback<IHasActionLocation>(delegate(int workerNum, List<IHasActionLocation> objectsForTrip)
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


        private void CheckForFieldIssues(TaskPlan plan)
        {
            if (GameState.Current.MasterTaskList.IsActiveTaskOfTypeDependingOn<PlantTask>(_fieldBeingSprayed))
            {
                plan.AddIssue("Cannot spray while being planted.", false);
            }
            if (_whatToSpray == null)
            {
                plan.AddIssue("Must select item to spray.", true);
            }
        }
        

        private void PlanTrip(TaskPlan plan, int workerNum, List<IHasActionLocation> tripObjects, TaskItemPlanner itemPlanner)
        {
            //how many objects to spray this tip
            int amountOfSprayForThisTrip = tripObjects.Count;

            //create a list of the spray we need to get
            ItemList sprayToGet = new ItemList();
            sprayToGet.IncreaseItemCount(_whatToSpray, amountOfSprayForThisTrip);

            //plan where to get that much spray from
            itemPlanner.PlanToGetItems(plan, workerNum, sprayToGet, tripObjects[0].ActionLocation);

            //create spray field action
            SprayAction sprayFieldTask = new SprayAction(tripObjects, _whatToSpray, _fieldBeingSprayed);
            plan.AddAction(workerNum, sprayFieldTask);
        }

        #endregion

        #region Other

        public override bool DependsOnObject(IGameObject obj)
        {
            if (obj == _fieldBeingSprayed) { return true; }
            return false;
        }

        public override string Description()
        {
            int sprayCount = _fieldBeingSprayed.Crops.Count;
            return "Spray " + _whatToSpray.FullName + "(" + sprayCount.ToString() + ") in " + _fieldBeingSprayed.Name;
        }

        #endregion

        #endregion

        #region Save Load
		public override void WriteStateV1(StateWriterV1 writer)
		{
			base.WriteStateV1(writer);
			writer.WriteObject(_fieldBeingSprayed);
			writer.WriteObject(_whatToSpray);
			writer.WriteBool(_useEquipment);
		}
		
		public override void ReadStateV1(StateReaderV1 reader)
		{
			base.ReadStateV1(reader);
			_fieldBeingSprayed = reader.ReadObject<Field>();
			_whatToSpray = reader.ReadObject<ItemType>();
			_useEquipment = reader.ReadBool();
		}
		
		public override void AfterReadStateV1()
		{
			base.AfterReadStateV1();
		}
		#endregion

    }
}
