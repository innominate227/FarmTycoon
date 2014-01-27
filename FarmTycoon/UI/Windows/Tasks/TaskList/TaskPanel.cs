using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;
using System.Diagnostics;

namespace FarmTycoon
{

    public partial class TaskPanel : TycoonPanel
    {
        public event Action<TaskPanel> Selected;


        /// <summary>
        /// The task shown in the panel.
        /// </summary>
        private Task _task;

        /// <summary>
        /// The scheduled task shown in the panel. null if showing a normal Task
        /// </summary>
        private ScheduledTask _scheduledTask;


        /// <summary>
        /// The task shown in the panel.
        /// </summary>
        public Task Task
        {
            get { return _task; }            
        }

        /// <summary>
        /// The scheduled task shown in the panel. null if showing a normal Task
        /// </summary>
        public ScheduledTask ScheduledTask
        {
            get { return _scheduledTask; }            
        }



        public TaskPanel()
        {
            //intilize
            InitializeComponent();

            this.Clicked += new Action<TycoonControl>(Task_Clicked);
            NameLabel.Clicked += new Action<TycoonControl>(Task_Clicked);
            DateLabel.Clicked += new Action<TycoonControl>(Task_Clicked);
            WorkersLabel.Clicked += new Action<TycoonControl>(Task_Clicked);
        }

        private void Task_Clicked(TycoonControl obj)
        {
            if (this.IsSelected == false)
            {
                this.IsSelected = true;
                if (Selected != null)
                {
                    Selected(this);
                }
            }
        }


        public void ShowTask(Task task)
        {
            _task = task;
            NameLabel.Text = task.Description();
            WorkersLabel.Text = task.NumberOfWorkers.ToString();
            DateLabel.Text = Calandar.DateAsString(task.DesiredStartDate);  
        }

        public void ShowScheduledTask(ScheduledTask scheduledTask)
        {
            _scheduledTask = scheduledTask;
            _task = scheduledTask.TemplateTask;
            NameLabel.Text = scheduledTask.TemplateTask.Description();
            WorkersLabel.Text = scheduledTask.TemplateTask.NumberOfWorkers.ToString();
            int nextRunDate = scheduledTask.NextRunDate;
            if (nextRunDate == -1)
            {
                DateLabel.Text = "";
            }
            else
            {
                DateLabel.Text = Calandar.DateAsString(nextRunDate);
            }
        }

        /// <summary>
        /// Should the panel show as selected
        /// </summary>
        public bool IsSelected
        {
            get { return this.BackColor == Color.Blue; }
            set
            {
                Color colorToSet = Color.FromArgb(192, 64, 64);
                if (value)
                {
                    colorToSet = Color.Blue;
                }

                this.BackColor = colorToSet;
                DateLabel.BackColor = colorToSet;                
                DateLabel.BorderColor = colorToSet;
                NameLabel.BackColor = colorToSet;
                NameLabel.BorderColor = colorToSet;
                WorkersLabel.BackColor = colorToSet;
                WorkersLabel.BorderColor = colorToSet;                
            }
        }
        

    }
}
