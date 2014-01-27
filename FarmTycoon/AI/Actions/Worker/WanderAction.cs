using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Default action just has the worker wander around the farm
    /// </summary>
    public partial class WanderAction : ActionBase<Worker>
    {
        #region Member Vars

        /// <summary>
        /// Location to wander too
        /// </summary>        
        private Location _wanderTo;

        #endregion

        #region Setup

        public WanderAction()
        {            
        }

        #endregion

        #region Logic

        protected override void AfterAssigned()
        {
            _wanderTo = GetWanderLocation();
        }

        /// <summary>
        /// The first peice of land the worker should go to for this action
        /// </summary>
        public override Location FirstLocation()
        {
            return _wanderTo;
        }

        protected override Location NextLocationInnrer()
        {
            return _wanderTo;
        }

        /// <summary>
        /// Worker has arrived at that land it was moving to
        /// </summary>
        public override void DoLocationAction(Location location)
        {   
            //get next location to wander to
            _wanderTo = GetWanderLocation();         
        }

        public override double ArrivedAtDestination(Location location)
        {
            return 0;
        }

        
        /// <summary>
        /// The last peice of land the worker should go to for this action
        /// </summary>
        public override Location LastLocation()
        {
            //this should never be getting called on the idle action, since the idle action is never finished
            Debug.Assert(false);
            return null;
        }

        public override double ExpectedTime(DelaySet expectedDelays)
        {
            //Expected tine should not be getting called on the idle action, becase it should not be part of a task
            Debug.Assert(false);
            return 0;
        }
             
        /// <summary>
        /// Get a location to wander to
        /// </summary>
        /// <returns></returns>
        private Location GetWanderLocation()
        {

            int numberOfChoices = 0;
            foreach (StorageBuilding building in GameState.Current.MasterObjectList.FindAll<StorageBuilding>())
            {
                numberOfChoices++;
            }
            foreach (Field field in GameState.Current.MasterObjectList.FindAll<Field>())
            {
                numberOfChoices++;
            }
            foreach (Pasture pasture in GameState.Current.MasterObjectList.FindAll<Pasture>())
            {
                numberOfChoices++;
            }

            //must be at least two choice or we will just keep going to the same peuce of land over and over
            if (numberOfChoices == 0 || numberOfChoices == 1)
            {
                numberOfChoices = 2;
            }

            int choice = Program.Game.Random.Next(numberOfChoices);
            int choiceOn = 0;

            foreach (StorageBuilding building in GameState.Current.MasterObjectList.FindAll<StorageBuilding>())
            {
                if (choice == choiceOn)
                {
                    return building.ActionLocation;
                }
                choiceOn++;

            }
            foreach (Field field in GameState.Current.MasterObjectList.FindAll<Field>())
            {
                if (choice == choiceOn)
                {
                    return field.EntryLand.LocationOn;
                }
                choiceOn++;
            }
            foreach (Pasture pasture in GameState.Current.MasterObjectList.FindAll<Pasture>())
            {
                if (choice == choiceOn)
                {
                    return pasture.EntryLand.LocationOn;
                }
                choiceOn++;
            }

            Random rnd = new Random();

            return GameState.Current.MasterObjectList.FindAll<Land>()[rnd.Next(GameState.Current.MasterObjectList.TypeCount<Land>())].LocationOn;
        }
        
        public override bool IsObjectInvolved(IGameObject obj)
        {
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
