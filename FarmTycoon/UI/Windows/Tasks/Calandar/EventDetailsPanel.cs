using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class EventDetailsPanel : TycoonPanel
    {
        private Task m_task;

        public EventDetailsPanel()
        {
            InitializeComponent();

            cancelButton.Clicked += new Action<TycoonControl>(delegate
            {
                m_task.Abort();
            });
        }

        public void SetTask(Task task)
        {
            m_task = task;
            eventTitleLabel.Text = task.ShortDescription();
            eventDetailsLabel.Text = task.LongDescription();
            cancelButton.Visible = (task.TaskState != TaskState.Finished);
        }
    }
}

