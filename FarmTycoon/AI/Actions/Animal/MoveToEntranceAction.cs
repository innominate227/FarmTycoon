using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Action that an animal when it is going to be picked up (move to the Pasture action location to meet the worker)
    /// </summary>
    public partial class MoveToEntranceAction : ActionBase<Animal>
    {
        #region Member Vars

        /// <summary>
        /// Pasture to move to the entrance of
        /// </summary>        
        private Pasture _moveToEntranceOf;

        #endregion

        #region Setup

        public MoveToEntranceAction()
        {            
        }

        public MoveToEntranceAction(Pasture moveToEntranceOf)
        {
            _moveToEntranceOf = moveToEntranceOf;
        }

        #endregion

        #region Logic

        /// <summary>
        /// The first peice of land the animal should go to for this action
        /// </summary>
        public override Location FirstLocation()
        {
            return _moveToEntranceOf.ActionLocation;
        }

        /// <summary>
        /// The next peice of land the animal should go to for this action
        /// </summary>
        protected override Location NextLocationInnrer()
        {
            return _moveToEntranceOf.ActionLocation;
        }
                
        /// <summary>
        /// animal has arrived at that land it was moving to
        /// </summary>
        public override void DoLocationAction(Location location)
        {                          
        }

        /// <summary>
        /// Animal arrived at a detination retrun how long to wait
        /// </summary>
        public override double ArrivedAtDestination(Location location)
        {
            return 100;
        }

        /// <summary>
        /// The last peice of land the animal should go to for this action
        /// </summary>
        public override Location LastLocation()
        {
            //this should never be getting called, since the move to entrance action is never finished
            Debug.Assert(false);
            return null;
        }

        public override double ExpectedTime(DelaySet expectedDelays)
        {
            //Expected tine should not be getting called on this action, becase it should not be part of a task
            Debug.Assert(false);
            return 0;
        }
        

        public override bool IsObjectInvolved(IGameObject obj)
        {
            //no objects involved in the action
            return false;
        }
        
        public override string Description()
        {
            return "Move To Entrance";
        }

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteObject(_moveToEntranceOf);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _moveToEntranceOf = reader.ReadObject<Pasture>();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion
        
    }
}
