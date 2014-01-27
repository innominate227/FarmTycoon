using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Moves an Actor in order to complete actions in an action sequence
    /// </summary>        
    public partial class ActionMover<T> : ISavable where T:IActor
    {
        #region Events

        /// <summary>
        /// Event raised when the action sequence has been completed
        /// </summary>
        public event Action<ActionSequence<T>> FinishedAssignedSequence;

        #endregion

        #region Member Vars

        /// <summary>
        /// Action sequence the mover is working on
        /// </summary>        
        private ActionSequence<T> _currentActionSequence;

        /// <summary>
        /// Action the actor is currently working on
        /// </summary>        
        private ActionBase<T> _currentAction;

        /// <summary>
        /// The T that is going to perform the actions
        /// </summary>        
        private T _actor;

        #endregion

        #region Setup

        /// <summary>
        /// Setup the ActionMover
        /// </summary>
        public void Setup(PositionManager positionManager, T actor)
        {
            _positionManager = positionManager;
            _actor = actor;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Action sequence the actor is working on
        /// </summary>
        public ActionSequence<T> CurrentActionSequence
        {
            get { return _currentActionSequence; }
        }

        /// <summary>
        /// Action the actor is currently working on
        /// </summary>
        public ActionBase<T> CurrentAction
        {
            get { return _currentAction; }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Abort the current action sequence (if the actor is working on one)
        /// </summary>
        public void AbortActionSequence()
        {
            //check if the actor is currently working on a action sequence
            if (_currentActionSequence != null)
            {
                //have the actor stop trying to go to the current destination (it will also stop waiting if it was waiting)
                AbortDestination();

                //abort the current sequence
                _currentActionSequence.Abort();

                //clear current action, and sequence
                _currentAction = null;
                _currentActionSequence = null;

                //clear current destination
                _destination = null;
            }
        }
        
        /// <summary>
        /// Tell the actor to start doing the actions in the action sequence passed.        
        /// </summary>
        public void DoActionSequence(ActionSequence<T> actionSequence)
        {
            //all passed should have at least one action
            Debug.Assert(actionSequence.Actions.Count > 0);

            //we should not be told to do a sequence when we are already doing one
            Debug.Assert(_currentActionSequence == null && _currentAction == null);

            //set the new action sequence
            _currentActionSequence = actionSequence;

            //tell the sequence that we have started it
            _currentActionSequence.Start(_actor);
        }
        
        /// <summary>
        /// Called when a destination has been arrived at return how long to wait at the destination
        /// </summary>
        private double ArrivedAtDestination(Location arrivedAt)
        {
            //make sure they arrived at where they were supposed to be going
            Debug.Assert(arrivedAt == _currentAction.NextLocation());

            //tell the action we arrived at the destination and
            //get how long to wait at the current land we arrived at
            double timeToWaitAtNewLocation = _currentAction.ArrivedAtDestination(arrivedAt);

            //return how long to wait at that Location
            return timeToWaitAtNewLocation;
        }

        /// <summary>
        /// Called when we are finished waiting at a destination return the new destination or null, if no new destination
        /// </summary>
        private Location FinishedWaitingAtDestination(Location finishedWaitingAt)
        {
            //make sure they finished waiting at where they were supposed to be going
            Debug.Assert(finishedWaitingAt == _currentAction.NextLocation());

            //do the action at this location
            _currentAction.DoLocationAction(finishedWaitingAt);

            //if there is no next land then we are donw with this action.  start the next action in the action sequence
            if (_currentAction.NextLocation() == null)
            {
                //go to the next action
                return StartNextAction();
            }
            else
            {
                //move the next location for this action
                return _currentAction.NextLocation();
            }
        }
        
        /// <summary>
        /// Have the worker start the next action in the action sequence,
        /// or the first action if the current action is null.
        /// return the destination the worker should travel to in order to do this
        /// </summary>
        private Location StartNextAction()
        {
            //make sure we have a sequence to start
            Debug.Assert(_currentActionSequence != null);

            //go to the next action (if current action is null this will return the first action in the sequence)
            _currentAction = _currentActionSequence.GetNextAction(_currentAction);

            //if there is no next action we have completed all the actions in the sequence
            if (_currentAction == null)
            {
                //we have completed the sequence
                return ActionSequenceCompleted();
            }
            else
            {
                //tell the action we have started it
                _currentAction.Started();

                //move toward the next location for the action (which will be the first location)
                return _currentAction.NextLocation();
            }
        }
        
        /// <summary>
        /// Called when the worker has completed its current action sequence.
        /// If we get assigned a new sequence return the first location we should travel to for that sequence
        /// </summary>
        private Location ActionSequenceCompleted()
        {
            //tell the action sequence it is completed
            _currentActionSequence.Finished();

            //remeber what sequence we just finised
            ActionSequence<T> actionSequenceFinished = _currentActionSequence;

            //we are no longer doing that action sequence
            _currentActionSequence = null;
            _currentAction = null;

            //report that we finished the action sequence 
            //(need to do this after setting current sequence to null, so we can be assigned a new sequence)
            if (FinishedAssignedSequence != null)
            {
                FinishedAssignedSequence(actionSequenceFinished);
            }

            //if we were assigned a new action sequence by who ever handled the event above then return the first destination for that.
            //if not the call to CheckForDestination will return null
            return CheckForDestination();            
        }
        
        /// <summary>
        /// Called when there currently is no destination.  If we have been assigned an ActionSequence and now have a destination then return that.
        /// Otherwise return null.
        /// </summary>
        private Location CheckForDestination()
        {
            //if we were assigned a new action sequence the start it
            if (_currentActionSequence != null)
            {
                return StartNextAction();
            }
            else
            {
                //we have nothing to do
                return null;
            }
        }

        #endregion
        
        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
		{
            WriteStateV1Actions(writer);
            WriteStateV1Movement(writer);
		}

        public void ReadStateV1(StateReaderV1 reader)
        {
            ReadStateV1Actions(reader);
            ReadStateV1Movement(reader);
		}

        public void AfterReadStateV1()
        {
        }

        


        private void WriteStateV1Actions(StateWriterV1 writer)
        {
            writer.WriteObject(_currentActionSequence);
            writer.WriteObject(_currentAction);
            writer.WriteObject(_actor);
        }

        private void ReadStateV1Actions(StateReaderV1 reader)
        {
            _currentActionSequence = reader.ReadObject<ActionSequence<T>>();
            _currentAction = reader.ReadObject<ActionBase<T>>();
            _actor = reader.ReadObject<T>();
        }

        #endregion

    }
}
