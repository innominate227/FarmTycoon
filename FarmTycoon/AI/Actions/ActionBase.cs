using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Possible states the action can be in
    /// </summary>
    public enum ActionState
    {
        Planning,   //action has been created, but has not been assigned to a worker yet, it may never get assigned to a worker
                    //Possible Next States: Assigned, or unreferenced

        Assigned,   //The action has been assigned to a worker,  The worker has started the action sequence this action is a part of
                    //but this particular action has not been started yet.  
                    //Possible Next States: Aborted, Started  

        Started,    //worker has started preforming this action, they are walking to the start land for the action, or possibly further along than that
                    //Possible Next States: Aborted, Finished
   
        Finished,   //worker has finished preforming this action
                    //Possible Next States: unreferenced
        
        Aborted     //the action was aborted
                    //Possible Next States: unreferenced
    }

    /// <summary>
    /// An Action that is perfored by a T.  This is the base class for all specific Action classes.
    /// </summary>
    public abstract class ActionBase<T> : IAction where T : IActor
    {
        #region Member Vars

        /// <summary>
        /// The state the action is currently in
        /// </summary>
        protected ActionState _state = ActionState.Planning;

        /// <summary>
        /// The T that will be preforming the action
        /// </summary>
        protected T _actor;

        /// <summary>
        /// The task the action is part of.  Will be null if the action is not part of a task
        /// </summary>
        protected Task _task;

        #endregion

        #region Abstract Methods

        /// <summary>
        /// The first location the IActor should go to when doing this action.  Returning null indicates the worker does not need to go anywhere to preform this action (such Actions should return the workers current position for GetNextLocation).
        /// This method will be called during the planning phase of the action so it should not relay on the action having a IActor assigned.
        /// </summary>
        public abstract Location FirstLocation();

        /// <summary>
        /// The last location the IActor should go to when doing this action.  Returning null indicates the worker does not need to go anywhere to preform this action (such Actions should return the workers current position for GetNextLocation).
        /// This method will be called during the planning phase of the action so it should not relay on the action having a IActor assigned.
        /// </summary>
        public abstract Location LastLocation();

        /// <summary>
        /// The time expected to complete the action assuming
        /// This method will be called during the planning phase of the action so it should not relay on the action having a IActor assigned.
        /// </summary>        
        public abstract double ExpectedTime(DelaySet expectedDelays);

        /// <summary>
        /// Return true if the game objects passed is needed to complete the action.        
        /// </summary>
        public abstract bool IsObjectInvolved(IGameObject obj);

        /// <summary>
        /// Description of the action.
        /// This method will be called during the planning phase of the action so it should not relay on the action having a IActor assigned.
        /// </summary>  
        public abstract string Description();



        /// <summary>
        /// Called when the actor has arrived at the next desitionation for the action.
        /// Return the number of seconds that the IActor should wait at the location passed.  
        /// This method will only be called after the action has been started.
        /// </summary>
        public abstract double ArrivedAtDestination(Location location);


        /// <summary>
        /// Called when the IActor has arrived at the location passed and waited for the approprate amount of time.
        /// The IActor should complete the part of this action that needs to be completed at this location.
        /// This method will only be called after the action has been started.
        /// </summary>
        public abstract void DoLocationAction(Location location);

        /// <summary>
        /// The location the IActor should go to next.
        /// This method will only be called after the action has been started.
        /// Return null if there are no more locations to go to.
        /// This will be called multiple times before the IActor actually arrives at the location.
        /// </summary>
        protected abstract Location NextLocationInnrer();
        
        #region Optional State Changed Methods

        /// <summary>
        /// Called after the action has been assigned to a IActor.  
        /// </summary>
        protected virtual void AfterAssigned() { }

        /// <summary>
        /// Called after the action has been started by a IActor.  
        /// </summary>
        protected virtual void AfterStarted() { }

        /// <summary>
        /// Called after the action has been finished by a IActor.  
        /// </summary>
        protected virtual void AfterFinished() { }

        /// <summary>
        /// Called after the action has been aborted.  The action must have been assigned in order to have been aborted.
        /// The passed value tells if the action was stared or not.  If it was finished this will not be called
        /// </summary>
        protected virtual void AfterAborted(bool wasStarted) { }

        #endregion

        #endregion

        #region Properties
        
        /// <summary>
        /// Get/Set the task the action is part of. 
        /// </summary>        
        public Task Task
        {
            get { return _task; }
            set { _task = value; }
        }
        
        /// <summary>
        /// The T that will be preforming the action
        /// </summary>
        public T Actor
        {
            get { return _actor; }            
        }

        #endregion

        #region Logic

        /// <summary>
        /// Assign the IActor who will be doing this action.
        /// The Action will move into the assigned state, and it is expected that the action will be either completed or aborted at some point the the future.
        /// </summary>
        public void AssignWorker(T actor)
        {
            //make sure no one else has alreadt been assigned this action
            Debug.Assert(_actor == null);

            //we should be in planning phase when the action is assigned
            Debug.Assert(_state == ActionState.Planning);

            _actor = actor;
            _state = ActionState.Assigned;

            AfterAssigned();
        }
        
        /// <summary>
        /// Called by the IActor doing this action right before they start working on the action
        /// </summary>
        public void Started()
        {            
            Debug.Assert(_state == ActionState.Assigned);
            _state = ActionState.Started;
            AfterStarted();
        }
        
        /// <summary>
        /// Call if the IActor has aborted trying to preform the action.  The action should have been assigned if this is to be call, but it has not nessisarily been started.
        /// Do not call Abort on an action that never left the planning state.
        /// </summary>
        public void Abort()
        {
            Debug.Assert(_state == ActionState.Assigned || _state == ActionState.Started || _state == ActionState.Finished);

            if (_state == ActionState.Finished)
            {
                //if action was finished it remains finished
                return;
            }

            //see if the action had been started by the worker or not
            bool wasStarted = (_state == ActionState.Started);

            //action is not aborted
            _state = ActionState.Aborted;
            AfterAborted(wasStarted);
        }
        
        /// <summary>
        /// The location the IActor should go to next.
        /// This method will only be called after the action has been started.
        /// Return null if there are no more locations to go to.
        /// This will be called multiple times before the IActor actually arrives at the location.   
        /// </summary>
        public Location NextLocation()
        {
            Location nextLocation = NextLocationInnrer();

            //if there is no next land we are done with the action
            if (nextLocation == null)
            {
                _state = ActionState.Finished;
                AfterFinished();

                //if there is an associated task inform the task we are finished
                if (_task != null)
                {
                    _task.ActionFinished((ActionBase<Worker>)(object)this);
                }
            }
            return nextLocation;
        }

        #endregion

        #region Save Load
        public virtual void WriteStateV1(StateWriterV1 writer)
		{
			writer.WriteEnum(_state);
			writer.WriteObject(_actor);
			writer.WriteObject(_task);
		}

        public virtual void ReadStateV1(StateReaderV1 reader)
		{
			_state = reader.ReadEnum<ActionState>();
			_actor = reader.ReadObject<T>();
			_task = reader.ReadObject<Task>();			
		}
		
		public virtual void AfterReadStateV1()
		{
		}
		#endregion
        
    }
}
