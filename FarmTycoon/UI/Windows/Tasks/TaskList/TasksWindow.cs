using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class TasksWindow : TycoonWindow
    {
        /// <summary>
        /// Task pabels that have been keyed sorted by the task they were created for.
        /// For scheduled task this task is the task template
        /// </summary>
        private Dictionary<Task, TaskPanel> _taskPanels = new Dictionary<Task,TaskPanel>();

        /// <summary>
        /// The task that is selected in the window
        /// </summary>
        private Task _selectedTask = null;

        /// <summary>
        /// The index of the selected task in the window
        /// </summary>
        private int _selectedTaskIndex = 0;



        public TasksWindow()
        {
            InitializeComponent();
            
            this.Top = (Program.UserInterface.Graphics.WindowHeight / 2) - (this.Height / 2);
            this.Left = (Program.UserInterface.Graphics.WindowWidth / 2) - (this.Width / 2);
            
            GameState.Current.MasterTaskList.ActiveTaskListChanged += new Action(RefreshWindow);
            GameState.Current.MasterTaskList.ScheudledTasksListChanged += new Action(RefreshWindow);
            GameState.Current.TaskStarter.WaitingTaskListChanged += new Action(RefreshWindow);


            ActiveTasksButton.Depressed = false;
            ScheduledTasksButton.Depressed = false;
            DealyedTaskButton.Depressed = true;
            DateLabel.Text = "Expected";

            RefreshWindow();

            ActiveTasksButton.Clicked += new Action<TycoonControl>(delegate
            {
                ActiveTasksButton.Depressed = true;
                ScheduledTasksButton.Depressed = false;
                DealyedTaskButton.Depressed = false;
                TaskLabel.Text = "Task";
                DateLabel.Text = "Started On";
                RefreshWindow();
            });
            ScheduledTasksButton.Clicked += new Action<TycoonControl>(delegate
            {
                ActiveTasksButton.Depressed = false;
                ScheduledTasksButton.Depressed = true;
                DealyedTaskButton.Depressed = false;
                TaskLabel.Text = "Task Schedule";
                DateLabel.Text = "Next Start";
                RefreshWindow();
            });
            DealyedTaskButton.Clicked += new Action<TycoonControl>(delegate
            {
                ActiveTasksButton.Depressed = false;
                ScheduledTasksButton.Depressed = false;
                DealyedTaskButton.Depressed = true;
                TaskLabel.Text = "Task";
                DateLabel.Text = "Expected";
                RefreshWindow();
            });


            AbortButton.Clicked += new Action<TycoonControl>(AbortButton_Clicked);

            
            this.CloseClicked += new Action<TycoonWindow>(delegate
            {

                GameState.Current.MasterTaskList.ActiveTaskListChanged -= new Action(RefreshWindow);
                GameState.Current.MasterTaskList.ScheudledTasksListChanged -= new Action(RefreshWindow);
                GameState.Current.TaskStarter.WaitingTaskListChanged -= new Action(RefreshWindow);

                Program.UserInterface.WindowManager.RemoveWindow(this);
            });
                 
            Program.UserInterface.WindowManager.AddWindow(this);
        }
        

        private void RefreshWindow()
        {
            //collect a list of tasks or task schedules to show
            List<Task> toShow = new List<Task>();

            //for tasks that are still scheduled and should be shown these are the schedules
            Dictionary<Task, ScheduledTask> toShowScheduled = new Dictionary<Task, ScheduledTask>();

            //decxide which tasks to show based on the filter
            if (ActiveTasksButton.Depressed)
            {
                toShow.AddRange(GameState.Current.MasterTaskList.ActiveTasks);
            }
            else if (DealyedTaskButton.Depressed)
            {
                toShow.AddRange(GameState.Current.MasterTaskList.WaitingTasks);
            }
            else if (ScheduledTasksButton.Depressed)
            {
                foreach(ScheduledTask schedualedTask in GameState.Current.MasterTaskList.TaskScheudles)
                {
                    toShow.Add(schedualedTask.TemplateTask);
                    toShowScheduled.Add(schedualedTask.TemplateTask, schedualedTask);
                }
            }
            

            //hide any panels that are not visible
            foreach (Task taskWithPanel in _taskPanels.Keys)
            {
                if (toShow.Contains(taskWithPanel) == false)
                {
                    _taskPanels[taskWithPanel].Visible = false;
                }
            }

            //we need to select a new task if the one that was selected is no longer being shown
            if (toShow.Contains(_selectedTask) == false)
            {
                _selectedTask = null;
                if (toShow.Count > 0)
                {
                    if (toShow.Count - 1 < _selectedTaskIndex)
                    {
                        _selectedTaskIndex = toShow.Count - 1;
                    }
                    _selectedTask = toShow[_selectedTaskIndex];
                }
            }
            


            //make/show the panel for each task
            int taskNum = 0;
            foreach (Task task in toShow)
            {
                //get or create the task panel
                if (_taskPanels.ContainsKey(task) == false)
                {
                    TaskPanel newTaskPanel = new TaskPanel();
                    newTaskPanel.Left = 0;                    
                    newTaskPanel.Width = TasksPanel.Width;
                    newTaskPanel.Height = 30;
                    newTaskPanel.Selected += new Action<TaskPanel>(TaskPanel_Selected);
                    TasksPanel.AddChild(newTaskPanel);
                    _taskPanels.Add(task, newTaskPanel);
                }

                //determine if this is the selected task
                bool selected = false;
                if (task == _selectedTask)
                {
                    selected = true;
                    _selectedTaskIndex = taskNum;
                }

                //move task panel to the right spot
                TaskPanel taskPanel = _taskPanels[task];
                taskPanel.Top = taskNum * taskPanel.Height;                
                taskPanel.Visible = true;
                taskPanel.IsSelected = selected;
                taskPanel.Tag = taskNum;
                if (toShowScheduled.ContainsKey(task))
                {
                    //show the task
                    taskPanel.ShowScheduledTask(toShowScheduled[task]);
                }
                else
                {
                    //show the task
                    taskPanel.ShowTask(task);
                }

                //next one will be further down
                taskNum += 1;                
            }
        }


        /// <summary>
        /// Called when the selected task panel changes
        /// </summary>
        private void TaskPanel_Selected(TaskPanel selected)
        {
            //do nothing if already selected
            if (selected.Task == _selectedTask) { return; }

            //unselect old and select new
            _taskPanels[_selectedTask].IsSelected = false;
            _taskPanels[selected.Task].IsSelected = true;

            //update currently selected item, and panel
            _selectedTask = selected.Task;
            _selectedTaskIndex = (int)_taskPanels[selected.Task].Tag;
        }




        private void AbortButton_Clicked(TycoonControl button)
        {
            if (_selectedTask != null)
            {
                ScheduledTask scheduledTask = _taskPanels[_selectedTask].ScheduledTask;
                if (scheduledTask != null)
                {
                    scheduledTask.AbortSchedule();
                }
                else
                {
                    _selectedTask.Abort();
                }
            }
        }
    }
}
