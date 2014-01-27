using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// Abstract convince class for actions that take place at a singel location
    /// </summary>
    public abstract partial class OneLocationAction<T> : ActionBase<T> where T:IActor
    {
        #region Member Vars

        /// <summary>
        /// Has the worker done the action.
        /// </summary>
        private bool _didAction = false;

        #endregion

        #region Abstract

        /// <summary>
        /// The location the action should take place at
        /// </summary>
        public abstract Location TheLocation();

        /// <summary>
        /// Called when the worker arrives at that location, so the worker can do the action
        /// </summary>        
        public abstract void DoAction();

        /// <summary>
        /// Called when the worker arrives at the location where they will do the action.
        /// Dont actually do the action yet, but apply textures or other pre-action things.
        /// </summary>
        public virtual void ArrivedAtAction() { }

        /// <summary>
        /// Return the amount of time it will take to do the one location action.
        /// Will be passed either an expected delay set (during planning) or the workers actualy delay set suring execution
        /// </summary>
        public abstract double GetActionTime(DelaySet delaySet);

        #endregion

        #region Logic

        public override double ArrivedAtDestination(Location location)
        {            
            ArrivedAtAction();
            return GetActionTime(_actor.Delays);
        }

        public override double ExpectedTime(DelaySet expectedDelays)
        {
            return GetActionTime(expectedDelays);
        }

        public override Location FirstLocation()
        {
            return TheLocation();
        }

        public override Location LastLocation()
        {
            return TheLocation();
        }
        
        public override void DoLocationAction(Location location)
        {            
            DoAction();
            _didAction = true;
        }

        protected override Location NextLocationInnrer()
        {
            //we did the action so there is no next location
            if (_didAction)
            {
                return null;
            }
            return TheLocation();
        }

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteBool(_didAction);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _didAction = reader.ReadBool();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion

    }
}
