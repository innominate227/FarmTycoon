using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Performs an action sequence by making calls on a Mover in order to Move an object to the locations nessisary to perform the actions
    /// </summary>
    public class ActionPerformer
    {
        /// <summary>
        /// Action sequence the mover is working on
        /// </summary>
        private ActionSequence m_currentActionSequence;

        /// <summary>
        /// Action the actor is currently working on
        /// </summary>
        private ActionBase m_currentAction;

        /// <summary>
        /// Mover used to move the object around in order to perform the action
        /// </summary>
        private Mover m_mover;

        /// <summary>
        /// The actor that is going to perform the actions
        /// </summary>
        private IActor m_actor;

        /// <summary>
        /// Create a new ActionPerformer
        /// </summary>
        public ActionPerformer()
        {
        }

        /// <summary>
        /// Setup the ActionPerformer
        /// </summary>
        public void Setup(IActor actor, Mover mover)
        {
            m_actor = actor;
            m_mover = mover;

            m_mover.SetHandlers(ArrivedAtDestination, FinishedWaiting);
        }


        /// <summary>
        /// Action sequence the actor is working on
        /// </summary>
        public ActionSequence CurrentActionSequence
        {
            get { return m_currentActionSequence; }
        }

        /// <summary>
        /// Action the actor is currently working on
        /// </summary>
        public ActionBase CurrentAction
        {
            get { return m_currentAction; }
        }


        /// <summary>
        /// Abort the current action sequence (if the actor is working on one)
        /// </summary>
        public void AbortActionSequence()
        {
            //check if the actor is currently working on a action sequence
            if (m_currentActionSequence != null)
            {
                //have the actor stop waiting (incase the current action had him waiting)
                m_mover.AbortWaiting();                

                //abort the current sequence
                m_currentActionSequence.Abort();

                //clear current action, and sequence
                m_currentAction = null;
                m_currentActionSequence = null;
            }
        }


        /// <summary>
        /// Tell the actor to start doing the actions in the action sequence passed.        
        /// </summary>
        public void DoActionSequence(ActionSequence actionSequence)
        {
            //all sequences the actor should ever be assigned to do should have at least one action
            Debug.Assert(actionSequence.Actions.Count > 0);

            //we should not be told to do a sequence when we are already doing one
            Debug.Assert(m_currentActionSequence == null && m_currentAction == null);
            
            //set the new action sequence
            m_currentActionSequence = actionSequence;

            //tell the sequence that we have started it
            m_currentActionSequence.AssignWorker(m_actor);

            //start the next action for the plan (which will be the first since the current action is null)
            StartNextAction();
        }


        /// <summary>
        /// Called by the mover when it has arrived at a destination
        /// </summary>
        private void ArrivedAtDestination(Location arrivedAt)
        {
            //make sure they arrived at where they were supposed to be going
            Debug.Assert(arrivedAt == m_currentAction.NextLocation());

            //get how long to wait at the current land we arrived at
            double timeToWaitAtNewLocation = m_currentAction.GetLocationWaitTime(m_mover.Destination);

            //wait at that Location
            m_mover.Wait(timeToWaitAtNewLocation);
        }

        /// <summary>
        /// Called by the mover when it is done waiting at a destination
        /// </summary>
        private void FinishedWaiting(Location finishedWaitingAt)
        {
            //make sure they finished waiting at where they were supposed to be going
            Debug.Assert(finishedWaitingAt == m_currentAction.NextLocation());

            //do the action at this location
            m_currentAction.DoLocationAction(m_mover.Destination);

            //if there is no next land then we are donw with this action.  start the next action in the action sequence
            if (m_currentAction.NextLocation() == null)
            {
                //go to the next action
                StartNextAction();
            }
            else
            {
                //move toward the next location
                m_mover.MoveToward(m_currentAction.NextLocation());
            }
        }


        /// <summary>
        /// Have the worker start the next action in the action sequence,
        /// or the first action if the current action is null
        /// </summary>
        private void StartNextAction()
        {
            //make sure we have a sequence to start
            Debug.Assert(m_currentActionSequence != null);

            //go to the next action (if current action is null this will return the first action in the sequence)
            m_currentAction = m_currentActionSequence.GetNextAction(m_currentAction);
            
            //if there is no next action we have completed all the actions in the sequence
            if (m_currentAction == null)
            {
                //we have completed the sequence
                ActionSequenceCompleted();
            }
            else
            {
                //tell the action we have started it
                m_currentAction.Started();

                //move toward the next location for the action (which will be the first location)
                m_mover.MoveToward(m_currentAction.NextLocation());
            }
        }


        /// <summary>
        /// Called when the worker has completed its current action sequence
        /// </summary>
        private void ActionSequenceCompleted()
        {
            //remeber what sequence we just finised
            ActionSequence actionSequenceFinished = m_currentActionSequence;

            //we are no longer doing that action sequence
            m_currentActionSequence = null;
            m_currentAction = null;

            //report that we finished the action sequence 
            //(need to do this after setting current sequence to null, so we can be assigned a new sequence by the task manager)
            FinishedAssignedActionSequence(actionSequenceFinished);
        }



        #region Save State

        public void WriteActionState(ObjectState state)
        {
            state.SetValue("CurrentAction", m_currentAction);
            state.SetValue("CurrentActionSequenceNull", m_currentActionSequence == null);
            if (m_currentActionSequence != null)
            {
                state.WriteSubState("CurrentActionSequence", m_currentActionSequence);
            }
        }

        public void ReadActionState(ObjectState state)
        {
            m_currentAction = state.GetValue<ActionBase>("CurrentAction");
            bool currentActionSequenceIsNull = state.GetValue<bool>("CurrentActionSequenceNull");
            if (currentActionSequenceIsNull == false)
            {
                m_currentActionSequence = state.ReadSubState<ActionSequence>("CurrentActionSequence");
            }
        }

        #endregion
    }
}
