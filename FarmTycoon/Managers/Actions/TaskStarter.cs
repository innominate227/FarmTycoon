using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// In charge of starting a task, if the task cannot be started right away it will attempt to start again until it is started
    /// </summary>    
    public class TaskStarter : ISavable
    {
        #region Events


        /// <summary>
        /// Rasied when the list of waiting tasks changes
        /// </summary>
        public event Action WaitingTaskListChanged;

        #endregion

        #region Member Vars

        /// <summary>
        /// List of tasks that are waiting for some issue (not enough workers, nessary items dont exsist ect).
        /// </summary>        
        private List<Task> _waitingForIssue = new List<Task>();
        
        #endregion

        #region Setup

        /// <summary>
        /// Setup the TaskStarter
        /// </summary>
        public void Setup()
        {           
            SetupRetryEvents();  
        }


        /// <summary>        
        /// handle all events that could possibly cause a waiting task to be doable now
        /// </summary>
        private void SetupRetryEvents()
        {            
            GameState.Current.WorkerAssigner.NumberOfAvaiableWorkersChanged += new Action(TryToDoTasksWaitingForIssues);
            GameState.Current.PlayersItemsList.ListChanged += new Action(TryToDoTasksWaitingForIssues);            
            GameState.Current.Calandar.DateChanged += new Action(TryToDoTasksWaitingForIssues);
            GameState.Current.MasterTaskList.ActiveTaskListChanged += new Action(TryToDoTasksWaitingForIssues);
            GameState.Current.MasterObjectList.ItemAdded += new Action<Type>(delegate
            {
                TryToDoTasksWaitingForIssues();
            });
            GameState.Current.MasterObjectList.ItemRemoved += new Action<Type>(delegate
            {
                TryToDoTasksWaitingForIssues();
            });
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Get the List of tasks that are waiting for some issue (not enough workers, nessary items dont exsist ect).
        /// (do not modifiy this list)
        /// </summary>                
        public List<Task> WaitingForIssue
        {
            get { return _waitingForIssue; }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Try and start the task passed.
        /// </summary>
        public void StartTask(Task task)
        {
            //task should be in the waiting state 
            Debug.Assert(task.TaskState == TaskState.Waiting);

            //try and do the task, it will be added to the list if it cant be started
            TryToDoTask(task);            
        }
        
        /// <summary>
        /// Stop trying to start the task passed
        /// </summary>
        public void GiveUpOnTask(Task task)
        {
            if (_waitingForIssue.Contains(task))
            {
                _waitingForIssue.Remove(task);
                if (WaitingTaskListChanged != null)
                {
                    WaitingTaskListChanged();
                }
            }
            GameState.Current.IssueManager.ClearIssue(task, "Cant Start");
        }
                
        /// <summary>
        /// Called to try and do a task, if the task could not be done it will be added to the waiting list.        
        /// </summary>
        private void TryToDoTask(Task task)
        {
            //try and plan the task
            TaskPlan taskPlan = task.PlanTask();
            if (taskPlan.PlanSucceeded == false)
            {
                //add the task to the issues list if it is not already in it
                if (_waitingForIssue.Contains(task) == false)
                {
                    _waitingForIssue.Add(task);
                    if (WaitingTaskListChanged != null)
                    {
                        WaitingTaskListChanged();
                    }
                }

                //report issues to issue manager
                GameState.Current.IssueManager.ReportIssue(task, "Cant Start", task.Description() + " - " + taskPlan.IssuesString());
            }
            else //the task plan succeeded
            {
                //if it was in the waiting for issues list then remove it
                if (_waitingForIssue.Contains(task))
                {
                    _waitingForIssue.Remove(task);
                    if (WaitingTaskListChanged != null)
                    {
                        WaitingTaskListChanged();
                    }
                }

                //clear issue in issue manager
                GameState.Current.IssueManager.ClearIssue(task, "Cant Start");
                                
                //start the task
                task.Start();                
            }
        }

        /// <summary>
        /// We need to know if were already working on doing tasks waiting for issues because if so we are we dont want to try and do them again.
        /// (prevents infinate loop)
        /// </summary>
        private bool _tryingToDoTasksWaitingForIssues = false;

        /// <summary>
        /// Try and do all the tasks that are waiting on an issue.        
        /// </summary>
        private void TryToDoTasksWaitingForIssues()
        {
            //if we are already trying to do task waiting for issues, dont try and do them all again
            if (_tryingToDoTasksWaitingForIssues)
            {
                return;
            }

            //barrier to ensure we dont enter this method twice
            _tryingToDoTasksWaitingForIssues = true;

            //create list of tasks to try, sort by task desired start date, 
            //as we want to try and do that task that have been waiting for the longest first
            List<Task> tasksToTry = new List<Task>();            
            tasksToTry.AddRange(_waitingForIssue);
            tasksToTry.Sort(delegate(Task t1, Task t2)
            {
                return t1.DesiredStartDate - t2.DesiredStartDate;
            });

            //try each task
            foreach (Task taskToTry in tasksToTry)
            {
                TryToDoTask(taskToTry);
            }

            _tryingToDoTasksWaitingForIssues = false;
        }

        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteObjectList<Task>(_waitingForIssue);
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _waitingForIssue = reader.ReadObjectList<Task>();
        }

        public void AfterReadStateV1()
        {
            SetupRetryEvents();
        }
        #endregion

    }
}
