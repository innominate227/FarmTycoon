using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// This action simple delays the worker from doing anything else for the time passed (only if the worker is in a building).
    /// This is used so that if two workers are assigned to a task and are in the same building they do not come out right on top of each other.
    /// </summary>
    public partial class DelayAction : ActionBase<Worker>
    {
        #region Member Vars

        /// <summary>
        /// How long to delay to worker
        /// </summary>        
        private double _delay;

        /// <summary>
        /// true once to worker has delayed
        /// </summary>
        private bool _didDelay = false;

        #endregion

        #region Setup

        public DelayAction()
        {            
        }

        public DelayAction(double delay)
        {
            _delay = delay;
        }

        #endregion

        #region Logic
        
        public override Location FirstLocation()
        {
            //null here mean we dont care where we are when preforming this acton
            return null;
        }
        public override Location LastLocation()
        {
            //null here mean we dont care where we are when preforming this acton
            return null;
        }


        protected override Location NextLocationInnrer()
        {
            if (_didDelay)
            {
                //if we did the delay already we are done
                return null;
            }
            else
            {
                //go to where to worker currently is
                return _actor.LocationOn;
            }
        }

        public override void DoLocationAction(Location location)
        {
            _didDelay = true;

            //if we are inside a building we need to exit it now
            IHoldsWorkers buildingIn = _actor.BuildingInside;
            if (buildingIn != null)
            {
                //if we had a space reserved in the building we no longer do
                if (buildingIn.WorkersInside.WorkersWithSpotReserved.Contains(_actor))
                {
                    buildingIn.WorkersInside.FreeSpotFor(_actor);
                }

                //we are no longer in the building
                buildingIn.WorkersInside.RemoveWorker(_actor);

                //make the worker leaving the building it was in (make it visisble)
                _actor.ExitBuilding();
            }
        }

        public override double ArrivedAtDestination(Location location)
        {
            //only delay if we are actually inside a building
            if (_actor.BuildingInside != null)
            {
                return _delay;
            }
            else
            {
                return 0;
            }
        }
                

        public override double ExpectedTime(DelaySet expectedDelays)
        {
            return _delay;
        }
            
        public override bool IsObjectInvolved(IGameObject obj)
        {
            return false;
        }

        public override string Description()
        {
            return "Waking Up";
        }

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteDouble(_delay);
            writer.WriteBool(_didDelay);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _delay = reader.ReadDouble();
            _didDelay = reader.ReadBool();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion

    }
}
