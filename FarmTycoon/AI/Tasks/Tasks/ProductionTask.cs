using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace FarmTycoon
{
    public partial class ProductionTask : Task
    {
        #region Member Vars

        /// <summary>
        /// The production building where workers are to work
        /// </summary>
        private ProductionBuilding _productionBuilding;

        /// <summary>
        /// If multiple production tasks are starting at the same time (multi workers were assigned to do production) then we add delay to the task.
        /// </summary>
        private int _extraDelay = 0;

        #endregion

        #region Setup

        /// <summary>
        /// Create a new ProductionTask.  
        /// Call either Setup, and ReadState on the task after creating it.
        /// </summary>
        public ProductionTask() : base() { }

        /// <summary>
        /// Clone the task (only the planning state of the task needs to be cloned)
        /// </summary>
        protected override Task CloneInner()
        {
            ProductionTask clone = new ProductionTask();
            clone._productionBuilding = _productionBuilding;
            clone._extraDelay = _extraDelay; 
            return clone;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The production building where workers are to work
        /// </summary>
        public ProductionBuilding ProductionBuilding
        {
            set { _productionBuilding = value; }
            get { return _productionBuilding; }
        }
        
        /// <summary>
        /// If multiple production tasks are starting at the same time (multi workers were assigned to do production) then we add delay to the task.
        /// </summary>
        public int ExtraDelay
        {
            get { return _extraDelay; }
            set { _extraDelay = value; }
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
            TaskPlan plan = new TaskPlan(this, _extraDelay);
            
            //A ProductionTask should only ever have 1 worker,  multiple ProductionTasks are made when assigning multiple workers to the building.
            //This way when the Task is Aborted so the worker can leave it can be done with granuality of 1.
            Debug.Assert(_numberOfWorkers == 1);
            
            //add action to tell the worker to go to the production building                            
            plan.AddAction(0, new ProductionAction(_productionBuilding));            

            //return the plan
            return plan;
        }
             
        #endregion

        #region Other

        public override bool DependsOnObject(IGameObject obj)
        {
            if (obj == _productionBuilding) { return true; }
            return false;
        }

        public override string Description()
        {
            return "Production Task";
        }

        #endregion

        #endregion

        #region Save Load
		public override void WriteStateV1(StateWriterV1 writer)
		{
			base.WriteStateV1(writer);
            writer.WriteObject(_productionBuilding);
            writer.WriteInt(_extraDelay);
		}
		
		public override void ReadStateV1(StateReaderV1 reader)
		{
			base.ReadStateV1(reader);
            _productionBuilding = reader.ReadObject<ProductionBuilding>();
            _extraDelay  = reader.ReadInt();
		}
		
		public override void AfterReadStateV1()
		{
			base.AfterReadStateV1();
		}
		#endregion

    }
}
