using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Default action that an animal does just wander to a random land tile in the pasture
    /// </summary>
    public class AnimalWanderAction : ActionBase<Animal>
    {
        #region Member Vars

        /// <summary>
        /// Location to wander too
        /// </summary>        
        private Location _wanderTo;

        #endregion

        #region Setup

        public AnimalWanderAction()
        {            
        }

        #endregion

        #region Logic

        /// <summary>
        /// The first peice of land the animal should go to for this action
        /// </summary>
        public override Location FirstLocation()
        {
            return _wanderTo;
        }

        /// <summary>
        /// The next peice of land the animal should go to for this action
        /// </summary>
        protected override Location NextLocationInnrer()
        {
            return _wanderTo;
        }

        /// <summary>
        /// Called after an animal has been assigned to do this action
        /// </summary>
        protected override void AfterAssigned()
        {
            _wanderTo = GetWanderLocation();
        }
        
        /// <summary>
        /// Worker has arrived at that land it was moving to
        /// </summary>
        public override void DoLocationAction(Location location)
        {   
            //get next location to wander to
            _wanderTo = GetWanderLocation();                        
        }

        /// <summary>
        /// Animal arrived at a detination
        /// </summary>
        public override double ArrivedAtDestination(Location location)
        {
            return 0;
        }

        /// <summary>
        /// The last peice of land the animal should go to for this action
        /// </summary>
        public override Location LastLocation()
        {
            //this should never be getting called, since the animal action is never finished
            Debug.Assert(false);
            return null;
        }

        public override double ExpectedTime(DelaySet expectedDelays)
        {
            //Expected tine should not be getting called on this action, becase it should not be part of a task
            Debug.Assert(false);
            return 0;
        }
        

        /// <summary>
        /// Get a location to wander to
        /// </summary>
        private Location GetWanderLocation()
        {
            bool buildingOnLocation = true;
            Location wanderLocation = null;
            while (buildingOnLocation)
            {
                int numberOfChoices = _actor.Pasture.OrderedLand.Count;
                int choice = Program.Game.Random.Next(numberOfChoices);
                wanderLocation = _actor.Pasture.OrderedLand[choice].LocationOn;
                buildingOnLocation = wanderLocation.Contains<Trough>();
            }
            return wanderLocation;
        }
        
        public override bool IsObjectInvolved(IGameObject obj)
        {
            //no objects involved in the action
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
            writer.WriteObject(_wanderTo);
        }

        public override void ReadStateV1(StateReaderV1 reader)
		{
			base.ReadStateV1(reader);
			_wanderTo = reader.ReadObject<Location>();
		}

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion
        
    }
}
