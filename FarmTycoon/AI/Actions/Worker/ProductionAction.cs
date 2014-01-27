using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Active where the worker will go to a production building and stay until the action is Aborted.
    /// The production building is told when the worker gets to it, and will cause the building to start producing.
    /// </summary>
    public partial class ProductionAction : ActionBase<Worker>
    {
        #region Member Vars

        /// <summary>
        /// Location to wander too
        /// </summary>        
        private ProductionBuilding _productionBuilding;

        /// <summary>
        /// True once the worker is in the production building
        /// </summary>        
        private bool _inBuilding = false;

        #endregion

        #region Setup

        public ProductionAction()
        {            
        }

        public ProductionAction(ProductionBuilding productionBuilding)
        {
            _productionBuilding = productionBuilding;
        }

        #endregion

        #region Logic

        protected override void AfterAssigned()
        {
            base.AfterAssigned();

            //we are now going to the production building we were assigned to
            _productionBuilding.WorkersInside.AddWorkerHeadingToward(_actor);
        }

        /// <summary>
        /// The first peice of land the worker should go to for this action
        /// </summary>
        public override Location FirstLocation()
        {
            return _productionBuilding.ActionLocation;
        }

        protected override Location NextLocationInnrer()
        {
            return _productionBuilding.ActionLocation;
        }

        /// <summary>
        /// Worker has arrived at the location it was moving to
        /// </summary>
        public override void DoLocationAction(Location location)
        {
            //if worker is not in the building yet put him in there
            if (_inBuilding == false)
            {
                //we are no longer going toward that production building
                _productionBuilding.WorkersInside.RemoveWorkerHeadingToward(_actor);

                //we are not inside the building
                _productionBuilding.WorkersInside.AddWorker(_actor);                
                _inBuilding = true;
                
                //make worker invisible
                _actor.EnterBuilding(_productionBuilding);
            }
        }

        public override double ArrivedAtDestination(Location location)
        {
            if (_inBuilding == false)
            {
                //not in building yet dont wait at all to go into the building.
                return 0;
            }
            else
            {
                //we arrived at the building again, we are already in it though, just wait a long time, and keep waiting a long time everytime this is called.
                //cant return Infinate here, so we just return a large number.
                //When the time elapses and we need to go to the next location we go right back to the prodcution building anyway.
                return 100;
            }
        }

        
        /// <summary>
        /// The last peice of land the worker should go to for this action
        /// </summary>
        public override Location LastLocation()
        {
            return _productionBuilding.ActionLocation;
        }

        public override double ExpectedTime(DelaySet expectedDelays)
        {
            //This action never ends.  Just return 0 as the expected time, the window for this action should hide expected time.
            return 0;
        }

        protected override void AfterAborted(bool wasStarted)
        {
            if (_inBuilding)
            {
                //if we were in the building we are not anymore
                _productionBuilding.WorkersInside.RemoveWorker(_actor);

                //make the worker visible again
                _actor.ExitBuilding();
            }
            else
            {
                //we are no longer going toward that production building
                _productionBuilding.WorkersInside.RemoveWorkerHeadingToward(_actor);
            }
            base.AfterAborted(wasStarted);
        }


        public override bool IsObjectInvolved(IGameObject obj)
        {            
            if (obj == _productionBuilding) { return true; }
            return false;
        }

        public override string Description()
        {
            return "Wandering";
        }

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteObject(_productionBuilding);
            writer.WriteBool(_inBuilding);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _productionBuilding = reader.ReadObject<ProductionBuilding>();
            _inBuilding = reader.ReadBool();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion


    }
}
