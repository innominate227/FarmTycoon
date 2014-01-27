using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FarmTycoon
{
    public class FillTroughTask : Task
    {
        #region Member Vars

        /// <summary>
        /// The trough to fill
        /// </summary>
        private Trough _trough;
                
        /// <summary>
        /// What type to fill the trough with
        /// </summary>
        private ItemType _whatToFillWith = null;

        #endregion

        #region Setup

        /// <summary>
        /// Create a new FillTroughTask.          
        /// </summary>
        public FillTroughTask() : base() { }

        /// <summary>
        /// Clone the task (only the planning state of the task needs to be cloned)
        /// </summary>
        protected override Task CloneInner()
        {
            FillTroughTask clone = new FillTroughTask();
            clone._trough = _trough;
            clone._whatToFillWith = _whatToFillWith;
            return clone;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The trough that is being filled
        /// </summary>
        public Trough Trough
        {
            set { _trough = value; }
            get { return _trough; }
        }
        
        /// <summary>
        /// What item to fill the troughs with
        /// </summary>
        public ItemType WhatToFill
        {
            get { return _whatToFillWith; }
            set { _whatToFillWith = value; }
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
            CheckForIssues(plan);
            if (plan.CanCalculateExpectedTime == false) { return plan; }

            //item planner to plan where to get items from
            TaskItemPlanner itemPlanner = new TaskItemPlanner();

            //get a list of items the workers need to move to the troughs
            int amountOfItemThatCanFit = _trough.Inventory.AmountThatWillFitAfterReservedCapacity(_whatToFillWith);            
            ItemList toMove = new ItemList();
            toMove.IncreaseItemCount(_whatToFillWith, amountOfItemThatCanFit);
            
            
            //used to plan what each worker should move on each trip
            TaskItemDivider itemDivider = new TaskItemDivider(toMove);
                        
            //the worker to have move something next
            int workerNum = 0;

            //keep moving loads until nothing left to move
            ItemList load = itemDivider.NextLoad();
            while (load != null)
            {
                //plan the load
                PlanTrip(plan, itemPlanner, load, workerNum);

                //have the next worker find something to get
                workerNum++;
                if (workerNum >= _numberOfWorkers)
                {
                    workerNum = 0;
                }

                //figure out what the next worker will be getting
                load = itemDivider.NextLoad();
            }

            //return the plan
            return plan;
        }


        private void CheckForIssues(TaskPlan plan)
        {            
            if (_whatToFillWith == null)
            {
                plan.AddIssue("Must select item to fill troughs with.", true);
            }

            if (_trough.Inventory.Types.Count > 0)
            {                
                if (_whatToFillWith.Tags.Contains(SpecialTags.ANIMAL_WATER_TAG) && _trough.Inventory.Types[0].Tags.Contains(SpecialTags.ANIMAL_FOOD_TAG))
                {
                    plan.AddWarning("The food in the trough will be removed.");
                }
                else if (_whatToFillWith.Tags.Contains(SpecialTags.ANIMAL_FOOD_TAG) && _trough.Inventory.Types[0].Tags.Contains(SpecialTags.ANIMAL_WATER_TAG))
                {
                    plan.AddWarning("The water in the trough will be removed.");
                }
            }
        }

        protected override void AfterStarted()
        {
            base.AfterStarted();

            //determin if we need to clear the trough current inventory
            //clear the trough if the current inventory is incompatiable
            bool clearTrough = false;
            if (_trough.Inventory.Types.Count > 0)
            {
                if (_whatToFillWith.Tags.Contains(SpecialTags.ANIMAL_WATER_TAG) && _trough.Inventory.Types[0].Tags.Contains(SpecialTags.ANIMAL_FOOD_TAG))
                {
                    clearTrough = true;
                }
                else if (_whatToFillWith.Tags.Contains(SpecialTags.ANIMAL_FOOD_TAG) && _trough.Inventory.Types[0].Tags.Contains(SpecialTags.ANIMAL_WATER_TAG))
                {
                    clearTrough = true;
                }
            }

            if (clearTrough)
            {
                //cancel an animals that are doing action involing the trough
                foreach (IActionSequence actionInvolvingTrough in GameState.Current.ActiveActionList.ActionSequencesInvolving(_trough))
                {
                    if (actionInvolvingTrough is VisitTroughAction)
                    {
                        (actionInvolvingTrough as VisitTroughAction).Actor.AbortCurrentActionSequence();
                    }
                }

                //empty the troughs inventory
                foreach (ItemType itemInInventory in _trough.Inventory.Types)
                {
                    _trough.Inventory.RemoveFromInvetory(itemInInventory, _trough.Inventory.GetTypeCount(itemInInventory));
                }
            }
        }
        
        private void PlanTrip(TaskPlan plan, TaskItemPlanner itemPlanner, ItemList load, int workerNum)
        {
            //find somewhere to get the items from (near the pastures entrance)
            itemPlanner.PlanToGetItems(plan, workerNum, load, _trough.LocationOn);

            //put the items in the trough
            PutItemsAction putItems = new PutItemsAction(_trough, load);
            plan.AddAction(workerNum, putItems);

        }

        #endregion
                
        #region Other

        public override bool DependsOnObject(IGameObject obj)
        {
            if (obj == _trough) { return true; }
            return false;
        }

        public override string Description()
        {
            return "Fill Troughs";
        }


        #endregion

        #endregion
        
        #region Save Load
		public override void WriteStateV1(StateWriterV1 writer)
		{
			base.WriteStateV1(writer);
			writer.WriteObject(_trough);
			writer.WriteObject(_whatToFillWith);
		}
		
		public override void ReadStateV1(StateReaderV1 reader)
		{
			base.ReadStateV1(reader);
			_trough = reader.ReadObject<Trough>();
			_whatToFillWith = reader.ReadObject<ItemType>();
		}
		
		public override void AfterReadStateV1()
		{
			base.AfterReadStateV1();
		}
		#endregion

    }
}
