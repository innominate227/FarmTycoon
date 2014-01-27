using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Master list of all task schedules.
    /// Also holds the list of active tasks.
    /// Once a day calls each task schedule to see if it needs to start a task
    /// </summary>
    public class MasterTaskList : ISavable
    {
        #region Events

        /// <summary>
        /// Rasied when the list of active tasks changes
        /// </summary>
        public event Action ScheudledTasksListChanged;

        /// <summary>
        /// Rasied when the list of active tasks changes
        /// </summary>
        public event Action ActiveTaskListChanged;

        #endregion

        #region Member Vars

        /// <summary>
        /// List of all scheduled tasks
        /// </summary>
        private List<ScheduledTask> _scheduledTasks = new List<ScheduledTask>();

        /// <summary>
        /// List of active tasks
        /// </summary>
        private List<Task> _activeTasks = new List<Task>();
                
        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a tasklist, call Setup or ReadState
        /// </summary>
        public MasterTaskList() { }

        /// <summary>
        /// Setup the task list
        /// </summary>
        public void Setup()
        {            
            GameState.Current.Calandar.DateChanged += new Action(Calandar_DateChanged);
        }

        /// <summary>
        /// Abort all scheduled and active tasks (used when closing the current game)
        /// </summary>
        public void AbortAll()
        {
            foreach (Task task in _activeTasks.ToArray())
            {
                task.Abort();
            }
            foreach (ScheduledTask scheduledTask in _scheduledTasks.ToArray())
            {
                scheduledTask.AbortSchedule();
            }
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// List of tasks schedules
        /// (do not modify this list directly)
        /// </summary>
        public List<ScheduledTask> TaskScheudles
        {
            get { return _scheduledTasks; }
        }

        /// <summary>
        /// Return a list of all active tasks.
        /// (do not modify this list directly)
        /// </summary>
        public List<Task> ActiveTasks
        {
            get { return _activeTasks; }
        }

        /// <summary>
        /// Return a list of all active tasks that are waiting to be started.
        /// (Note this is the same as the TaskStarter.WaitingForIssue it is just repeated here for convince)
        /// (do not modify this list directly)
        /// </summary>
        public List<Task> WaitingTasks
        {
            get { return GameState.Current.TaskStarter.WaitingForIssue; }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Add a task schdule.
        /// </summary>
        public void AddScheduledTask(ScheduledTask scheduleTask)
        {
            _scheduledTasks.Add(scheduleTask);

            if (ScheudledTasksListChanged != null)
            {
                ScheudledTasksListChanged();
            }            
        }

        /// <summary>
        /// Remove a task schdule.
        /// </summary>
        public void RemoveScheduledTask(ScheduledTask scheduleTask)
        {
            _scheduledTasks.Remove(scheduleTask);

            if (ScheudledTasksListChanged != null)
            {
                ScheudledTasksListChanged();
            }
        }
        
        /// <summary>
        /// Return all scheduleds tasks that depend on the object passed
        /// </summary>
        public List<ScheduledTask> GetScheduledTaskDependingOn(IGameObject obj)
        {
            List<ScheduledTask> toRet = new List<ScheduledTask>();
            foreach (ScheduledTask scheduledTask in _scheduledTasks)
            {
                if (scheduledTask.TemplateTask.DependsOnObject(obj))
                {
                    toRet.Add(scheduledTask);
                }
            }
            return toRet;
        }

        /// <summary>
        /// Abort any scheduled tasks that depend on the object passed
        /// </summary>
        public void AbortScheduledTasksDependingOn(IGameObject obj)
        {
            List<ScheduledTask> toRemove = GetScheduledTaskDependingOn(obj);
            foreach (ScheduledTask taskScheduleToRemove in toRemove)
            {
                taskScheduleToRemove.AbortSchedule();
            }
        }




        /// <summary>
        /// Return if there is an active task that depends on the game object passed in order to start.
        /// Note: that a Task Dependence is different than action dependence.  
        /// Task dependce looks at what is needed in order to start the task, no what the task actually operates on.
        /// So for instance to start a Spray Task the task needs the field being sprayed to exists, but it does not care if Crops in the field exist.
        /// </summary>
        public bool IsTaskDependingOn(IGameObject obj)
        {
            foreach (Task task in _activeTasks)
            {
                if (task.DependsOnObject(obj))
                {
                    return true;
                }
            }
            return false;
        }



        /// <summary>
        /// Return if there is an active task of type T that depends on the game object passed in order to start.
        /// Note: that a Task Dependence is different than action dependence.  
        /// Task dependce looks at what is needed in order to start the task, no what the task actually operates on.
        /// So for instance to start a Spray Task the task needs the field being sprayed to exists, but it does not care if Crops in the field exist.
        /// </summary>
        public bool IsActiveTaskOfTypeDependingOn<T>(IGameObject obj) where T : Task
        {
            foreach (Task task in _activeTasks)
            {
                if ((task is T) == false) { continue; }
                if (task.DependsOnObject(obj))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Return all active tasks that depends on the game object passed in order to start.
        /// Note: that a Task Dependence is different than Action Dependence.  
        /// Task dependce looks at what is needed in order to start the task, no what the task actually operates on.
        /// So for instance to start a Spray Task the task needs the field being sprayed to exists, but it does not care if Crops in the field exist.
        /// </summary>
        public List<T> AllTasksOfTypeInvolvingObject<T>(IGameObject obj) where T : Task
        {
            List<T> ret = new List<T>();
            foreach (Task task in _activeTasks)
            {
                //only look at those of type T
                if ((task is T) == false) { continue; }

                //if we depend on it add it to the list
                if (task.DependsOnObject(obj))
                {
                    ret.Add((T)task);
                }
            }
            return ret;
        }
        




        /// <summary>
        /// Add a active task .
        /// </summary>
        public void AddActiveTask(Task activeTask)
        {
            _activeTasks.Add(activeTask);
            if (ActiveTaskListChanged != null)
            {
                ActiveTaskListChanged();
            }
        }

        /// <summary>
        /// Remove a active task.
        /// </summary>
        public void RemoveActiveTask(Task activeTask)
        {
            _activeTasks.Remove(activeTask);
            if (ActiveTaskListChanged != null)
            {
                ActiveTaskListChanged();
            }
        }


        /// <summary>
        /// Called when the date changes
        /// </summary>
        private void Calandar_DateChanged()
        {
            //check each task scheduled to see if it should start
            foreach (ScheduledTask scheduledTask in _scheduledTasks.ToArray())
            {
                scheduledTask.CheckSchedule(GameState.Current.Calandar.Date);
            }
        }


        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteObjectList<ScheduledTask>(_scheduledTasks);
            writer.WriteObjectList<Task>(_activeTasks);
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _scheduledTasks = reader.ReadObjectList<ScheduledTask>();
            _activeTasks = reader.ReadObjectList<Task>();
        }

        public void AfterReadStateV1()
        {
            GameState.Current.Calandar.DateChanged += new Action(Calandar_DateChanged);
        }
        #endregion
    }
}
