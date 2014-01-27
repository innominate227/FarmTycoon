using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    public enum WorkerTaskState
    {
        DoingTask,      //the worker is doing a task
        Aborting,       //the worker is aborting a task (putting items back that it got during the task)
        DefaultAction   //the worker is doing default action
    }


    public partial class Worker
    {
        #region Member Vars

        /// <summary>
        /// Current task the worker is working on (or null if not working on a task at the moment)
        /// </summary>
        private Task _currentTask;

        /// <summary>
        /// Current task state (are were working on a task, or are we working on aborting a task, or just wandering)
        /// </summary>
        private WorkerTaskState _taskState;

        #endregion

        #region Logic

        /// <summary>
        /// Have the worker abort the task they are currently working on
        /// </summary>
        public void AbortTask()
        {
            //make sure the worker was actually working on a task
            Debug.Assert(_taskState == WorkerTaskState.DoingTask);
            
            //have the worker abort their current action sequence
            _workerMover.AbortActionSequence();            

            //not working on a task anymore
            _currentTask = null;

            //have thw worker do their abort action sequence (where they put back items they have in their inventory)
            DoAbortActionSequence();
        }

        /// <summary>
        /// Tell the worker to start working on a task, by passing the task, and the action sequence they should be doing for the task.
        /// </summary>
        public void DoActionSequenceForTask(Task task, ActionSequence<Worker> actionSequence)
        {
            //make sure the worker was not busy
            Debug.Assert(_taskState == WorkerTaskState.DefaultAction); 
                                   
            //abort the default action sequence
            _workerMover.AbortActionSequence();

            //rember the task were working
            _currentTask = task;

            //tell the action manager to start on the action sequqence we need to do for that task
            _workerMover.DoActionSequence(actionSequence);
            
            //the worker is now working on a task
            _taskState = WorkerTaskState.DoingTask;
        }


        /// <summary>
        /// Handler called when the ActionMover reports that the worker has finished doing all the actions in the action sequence he was instructed to do.
        /// </summary>
        private void FinishedAssignedActionSequence(ActionSequence<Worker> plan)
        {
            if (_taskState == WorkerTaskState.DoingTask)
            {
                //if we were doing a task tell the task we are done
                _currentTask.WorkerFinished(this);
            }
            else if (_taskState == WorkerTaskState.Aborting)
            {
                //we have finished doing the actions nessisary to abort a task
            }
            else if (_taskState == WorkerTaskState.DefaultAction)
            {
                //we should never finish the default action sequence.
                Debug.Assert(false);
            }

            //we are done with the current task
            _currentTask = null;

            //set our state to default, so that we can be assigned a task to do (dont actually start wandering yet though because we may be assigned a task to do right away)
            _taskState = WorkerTaskState.DefaultAction;

            //report to the task assigner that we can do a task now.  It may assign us a task during this call (by calling our DoActionSequenceForTask method)
            GameState.Current.WorkerAssigner.WorkerNowAvaiable(this);

            //if task assigner did not assign us a task then start on default action sequence
            if (_currentTask == null)
            {
                DoDefaultActionSequence();
            }
        }


        /// <summary>
        /// Have the worker do an abort action sequence
        /// </summary>
        private void DoAbortActionSequence()
        {
            //create abort action sequence, which will have the worker put any items in his inventory back into the closest building
            ActionSequence<Worker> abortActionSequence = new ActionSequence<Worker>();
            
            //plan where to reserve space to put our items back
            TaskItemPlanner itemPlanner = new TaskItemPlanner();

            //plan to put the items the worker has back into some storage
            itemPlanner.PlanToPutItems(abortActionSequence, Inventory.UnderlyingList, false, LocationOn);
                                    
            //if the worker has a tow, put the tow back somewhere
            if (_tow != null)
            {
                ItemList towList = new ItemList();
                towList.IncreaseItemCount(_tow.ItemType, 1);
                itemPlanner.PlanToPutItems(abortActionSequence, towList, false, LocationOn);
            }

            //if the worker has a tractor, put the tractor back somewhere
            if (_vehicle != null)
            {
                ItemList vehicleList = new ItemList();
                vehicleList.IncreaseItemCount(_vehicle.ItemType, 1);
                itemPlanner.PlanToPutItems(abortActionSequence, vehicleList, false, LocationOn);
            }

            //add an action to drop all the items/equipment the worker was not able to put away
            Location dropAt = LocationOn;
            if (abortActionSequence.Actions.Count > 0)
            {
                dropAt = abortActionSequence.Actions[abortActionSequence.Actions.Count - 1].LastLocation();
            }
            DisgardItemsAction disguardItems = new DisgardItemsAction(dropAt);
            abortActionSequence.AddAction(disguardItems);

            //set state to aborting
            _taskState = WorkerTaskState.Aborting;

            //have the worker do the action sequence
            _workerMover.DoActionSequence(abortActionSequence);
        }

        
        /// <summary>
        /// Have the worker restart their default action sequence.
        /// This needs to be called if an object the worker was using as part of their default action sequence was deleted.
        /// </summary>
        public void RestartDefaultActionSequence()
        {
            //we should be doing default action when this is called
            Debug.Assert(_taskState == WorkerTaskState.DefaultAction);

            //abort the default action sequence
            _workerMover.AbortActionSequence();

            //restart default action sequence
            DoDefaultActionSequence();
        }

        /// <summary>
        /// Have the worker do the default action sequence.
        /// </summary>
        private void DoDefaultActionSequence()
        {
            //we should not be doing the default plan if there is something else to do
            Debug.Assert(_currentTask == null);

            //create the default action sequence
            ActionSequence<Worker> defaultActionSequence = new ActionSequence<Worker>();
            defaultActionSequence.AddAction(new RestAction());

            //set state to doing default action sequence
            _taskState = WorkerTaskState.DefaultAction; 

            //do the default action sequence
            _workerMover.DoActionSequence(defaultActionSequence);  
        }

        #endregion

        #region Save Load
        private void WriteStateV1Task(StateWriterV1 writer)
        {
            writer.WriteObject(_currentTask);
            writer.WriteEnum(_taskState);
        }

        private void ReadStateV1Task(StateReaderV1 reader)
        {
            _currentTask = reader.ReadObject<Task>();
            _taskState = reader.ReadEnum<WorkerTaskState>();
        }

        #endregion

    }
}
