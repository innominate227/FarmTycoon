using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class CalandarWindow : TycoonWindow
    {
        /// <summary>
        /// Filter to decide what tasks to show on the calandar 
        /// </summary>
        private TaskFilter m_filter = null;

        /// <summary>
        /// The window will either have a static name, or take the name of a gameobject.  
        /// If the objects name changes we need to update the windows title
        /// </summary>
        private GameObject m_objToUseNameOf = null;

        /// <summary>
        /// The task list of tasks to show
        /// </summary>
        private MasterTaskList m_taskList;

        public CalandarWindow(MasterTaskList taskList, TaskFilter filter)
        {
            InitializeComponent();

            m_taskList = taskList;
            m_filter = filter;
            eventCalandarPanel.Setup(taskList, filter);

            //handel resize event
            this.SizeChanged += new Action<TycoonControl>(delegate
            {
                eventCalandarPanel.Width = (int)((this.Width - 15) * (2.0 / 3.0));
                taskDetailsPanel.Width = (int)((this.Width - 15) * (1.0 / 3.0));

                verticelLine.Left = eventCalandarPanel.Left + eventCalandarPanel.Width + 5;
                taskDetailsPanel.Left = verticelLine.Left + 1;
            });
                        
            //refresh details if date changes or tasks change
            eventCalandarPanel.SelectedDateChanged += new Action(RefreshDetails);
            m_taskList.TaskListChanged += new Action(RefreshDetails);
            RefreshDetails();

            //delete window on close
            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                eventCalandarPanel.Delete();
                m_taskList.TaskListChanged -= new Action(RefreshDetails);
                if (m_objToUseNameOf != null)
                {
                    m_objToUseNameOf.NameChanged -= new Action(RefreshWindowName);
                }
                Program.UserInterface.WindowManager.RemoveWindow(this);
            });

            //add thw window
            Program.UserInterface.WindowManager.AddWindow(this);
        }

        public void SetWindowName(string name)
        {
            this.TitleText = name + " Schedule";
        }
        public void SetWindowName(GameObject objToUseNameOf)
        {
            m_objToUseNameOf = objToUseNameOf;
            m_objToUseNameOf.NameChanged += new Action(RefreshWindowName);
            RefreshWindowName();
        }
        

        private void RefreshWindowName()
        {
            this.TitleText = m_objToUseNameOf.Name + " Schedule";
        }

        private void RefreshDetails()
        {
            //remove current child controls
            foreach (TycoonControl con in taskDetailsPanel.Children.ToArray())
            {
                taskDetailsPanel.RemoveChild(con);
            }

            //get all the tasks on that date
            int dateSelected = eventCalandarPanel.DateSelected;
            List<Task> tasksOnDate = m_taskList.GetTasksStartingOn(dateSelected);

            int taskNum = 0;
            foreach (Task task in tasksOnDate)
            {
                if (m_filter != null && m_filter(task) == false)
                {
                    continue;
                }

                EventDetailsPanel taskDetails = new EventDetailsPanel();
                taskDetails.SetTask(task);
                taskDetails.Left = 0;
                taskDetails.Top = taskNum * 75;
                taskDetails.Width = taskDetailsPanel.Width;
                taskDetails.Height = 75;
                taskDetails.AnchorRight = true;
                taskDetailsPanel.AddChild(taskDetails);

                taskNum++;
            }
        }

    }
}

