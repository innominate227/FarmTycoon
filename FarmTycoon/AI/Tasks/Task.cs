using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace FarmTycoon
{
    public abstract class Task : ISavable
    {
        #region Member Vars

        /// <summary>
        /// The date the task should start
        /// </summary>
        protected int _desiredStartDate = -1;

        /// <summary>
        /// The number of workers that will be doing the task
        /// </summary>
        protected int _numberOfWorkers = 1;

        /// <summary>
        /// Workers that are preferred to work on this task
        /// </summary>
        protected List<Worker> _preferredWorkers = new List<Worker>();

        /// <summary>
        /// The state the task is in
        /// </summary>
        protected TaskState _taskState = TaskState.Planning;
        
        /// <summary>
        /// List of workers that are currently working on the task
        /// </summary>
        protected List<Worker> _workersDoingTask = new List<Worker>();
        
        #endregion

        #region Setup

        /// <summary>
        /// Create a task, call Setup or ReadState before using
        /// </summary>
        public Task()
        {
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// The date the task should start
        /// </summary>        
        public int DesiredStartDate
        {
            get { return _desiredStartDate; }
            set { _desiredStartDate = value; }
        }


        /// <summary>
        /// The number of workers that will be doing the task
        /// </summary>
        public int NumberOfWorkers
        {
            get { return _numberOfWorkers; }
            set
            {
                //number of workers can only be changed while the task is in the planning phase
                Debug.Assert(_taskState == TaskState.Planning);
                _numberOfWorkers = value;
            }
        }
        
        /// <summary>
        /// Workers that are preferred to work on this task, this list should be edited directly
        /// </summary>        
        public List<Worker> PreferredWorkers
        {
            get { return _preferredWorkers; }
        }
        
        /// <summary>
        /// Get the current state of the task
        /// </summary>
        public TaskState TaskState
        {
            get { return _taskState; }
        }

        #endregion

        #region Abstract
        
        /// <summary>
        /// Plan how to do the task.         
        /// This method should be overrideen by all derived classes to do the actial planning.
        /// Dont worry about making sure enough workers are available to do the task, that is handeled by the base class.
        /// </summary>
        protected abstract TaskPlan PlanTaskInner();

        /// <summary>
        /// Clone the task.        
        /// </summary>
        protected abstract Task CloneInner();

        /// <summary>
        /// Get a string that describes the task
        /// </summary>
        public abstract string Description();

        /// <summary>
        /// Return true if the task requires that the game object passed exsist in order to be started.
        /// </summary>
        public abstract bool DependsOnObject(IGameObject obj);


        #region State Changed

        /// <summary>
        /// Called after the task has been setup
        /// </summary>
        protected virtual void DoneWithSetupInner() { }

        /// <summary>
        /// Called just before the task is started
        /// </summary>
        protected virtual void BeforeStarted() { }

        /// <summary>
        /// Called after the task has been started
        /// </summary>
        protected virtual void AfterStarted() { }

        /// <summary>
        /// Called after the task has been finished
        /// </summary>
        protected virtual void AfterFinished() { }

        /// <summary>
        /// Called after the task has been aborted. Pass if the task was actually started, or just scehduled to be started.
        /// </summary>
        protected virtual void AfterAborted(bool wasStarted) { }

        /// <summary>
        /// Called by an action created by this task after it has been finished
        /// </summary>        
        public virtual void ActionFinished(ActionBase<Worker> action) { }

        #endregion
                
        #endregion

        #region Logic


        /// <summary>
        /// Clone the task. 
        /// The task should not have been started yet.
        /// </summary>
        public Task Clone()
        {
            Debug.Assert(_taskState == TaskState.Planning);

            Task clone = CloneInner();
            clone._numberOfWorkers = _numberOfWorkers;
            clone._preferredWorkers = _preferredWorkers;
            return clone;
        }

        /// <summary>
        /// Call to try and start the task.        
        /// </summary>
        public void TryToStart()
        {
            //we should be in planning state when we are told to try and start
            Debug.Assert(_taskState == TaskState.Planning);

            //let derived classes know the task is done being setup
            DoneWithSetupInner();

            //task is done being setup, and is now waiting to be started
            _taskState = TaskState.Waiting;
            
            //try and start the task, the starter will keep trying to start the task if it could not be started right away
            GameState.Current.TaskStarter.StartTask(this);
        }

        /// <summary>
        /// Called by the task starter when the task should be started.
        /// If you are not the task starter you should call TryToStart() instead.
        /// </summary>
        public void Start()
        {
            //we should be in waiting state when we actually start
            Debug.Assert(_taskState == TaskState.Waiting);

            //plan out the task
            TaskPlan taskPlan = PlanTask();

            //we should never be told to do the task if its not currently possible to start it
            Debug.Assert(taskPlan.PlanSucceeded);

            //let derived classes know the task is just about to start
            BeforeStarted();

            //assign workers to do the plan
            _workersDoingTask = GameState.Current.WorkerAssigner.AssignWorkers(taskPlan);
            
            //set the state to started
            _taskState = TaskState.Started;
                        
            //let derived classes know the task is started
            AfterStarted();

            //add to the master task list of running tasks
            GameState.Current.MasterTaskList.AddActiveTask(this);

            //if no workers were assigned to the task we are done right away
            if (_workersDoingTask.Count == 0)
            {
                TaskFinished();
            }
        }

        /// <summary>
        /// Plan how to do the task.
        /// </summary>
        public TaskPlan PlanTask()
        {
            //have the derviered class create a plan
            TaskPlan taskPlan = PlanTaskInner();

            //make sure there are enough workers            
            if (_numberOfWorkers > GameState.Current.WorkerAssigner.NumberOfAvailableWorkers)
            {
                int numberOfWorkersNeeded = _numberOfWorkers - GameState.Current.WorkerAssigner.NumberOfAvailableWorkers;
                string workersIssue = "Not enough workers currently avaialble, need " + numberOfWorkersNeeded.ToString() + " more";

                //figure out if the player will need to hire more
                if (_numberOfWorkers > GameState.Current.MasterObjectList.FindAll<Worker>().Count)
                {
                    workersIssue += " (You will need to hire more).";
                }
                else
                {
                    workersIssue += ".";
                }

                //add the workers issue to the plan
                taskPlan.AddIssue(workersIssue, false);
            }

            //return the task plan
            return taskPlan;
        }
        
        /// <summary>
        /// Called by a worker when it has finished its portion of the task
        /// </summary>
        public void WorkerFinished(Worker worker)
        {
            Debug.Assert(_taskState == TaskState.Started);

            //the worker is no longer doing the task
            _workersDoingTask.Remove(worker);

            if (_workersDoingTask.Count == 0)
            {
                TaskFinished();
            }
        }

        /// <summary>
        /// Called when the task has ben finished
        /// </summary>
        private void TaskFinished()
        {
            //task is finished
            _taskState = TaskState.Finished;

            //let derived classes know the task is finished
            AfterFinished();

            //remove from the master task list of running tasks
            GameState.Current.MasterTaskList.RemoveActiveTask(this);
        }

        /// <summary>
        /// Abort the task
        /// </summary>
        public void Abort()
        {
            //make sure the task was waiting, or was started when abort was called
            Debug.Assert(_taskState == TaskState.Waiting || _taskState == TaskState.Started);

            bool wasStarted = (_taskState == TaskState.Started);

            //task is aborted
            _taskState = TaskState.Aborted;

            //if the task had been satrted tell all workers to abort the task
            if (wasStarted)
            {                
                foreach (Worker worker in _workersDoingTask)
                {
                    worker.AbortTask();
                }
            }
                        
            //stop trying to start the task
            GameState.Current.TaskStarter.GiveUpOnTask(this);
            
            //let derived classes know the task was aborted
            AfterAborted(wasStarted);
            
            //remove from the master task list of running tasks
            GameState.Current.MasterTaskList.RemoveActiveTask(this);
        }
        
                
        #endregion

        #region Save Load
        public virtual void WriteStateV1(StateWriterV1 writer)
		{
			writer.WriteInt(_desiredStartDate);
			writer.WriteInt(_numberOfWorkers);
			writer.WriteObjectList<Worker>(_preferredWorkers);
			writer.WriteEnum(_taskState);
			writer.WriteObjectList<Worker>(_workersDoingTask);
		}

        public virtual void ReadStateV1(StateReaderV1 reader)
		{
			_desiredStartDate = reader.ReadInt();
			_numberOfWorkers = reader.ReadInt();
			_preferredWorkers = reader.ReadObjectList<Worker>();
			_taskState = reader.ReadEnum<TaskState>();
			_workersDoingTask = reader.ReadObjectList<Worker>();			
		}
		
		public virtual void AfterReadStateV1()
		{
		}
		#endregion

    }
}
