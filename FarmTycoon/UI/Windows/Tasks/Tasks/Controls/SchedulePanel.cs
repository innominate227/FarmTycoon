using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{

    public partial class SchedulePanel : TycoonPanel
    {
        /// <summary>
        /// The schedule being shown
        /// </summary>
        private ScheduledTask _schedule = new ScheduledTask();

        public SchedulePanel()
        {
            //intilize
            InitializeComponent();
            
            EditScheduleButton.Clicked += new Action<TycoonControl>(delegate
            {
                TaskScheduleWindow window = new TaskScheduleWindow(_schedule);
                window.ScheduleChanged += new Action(Refresh);
            });

            Refresh();
        }

        public ScheduledTask Schedule
        {
            get { return _schedule; }
            set { _schedule = value; }
        }

        private void Refresh()
        {
            //start time
            string starting = "now";
            if (_schedule.StartDelay > 0)
            {
                starting = "in " + _schedule.StartDelay.ToString() + " days";
            }
            else if (_schedule.StartDelay == -1)
            {
                starting = "on " + Calandar.DateAsString(_schedule.StartOn);
            }
            
            //interval
            string interval = "???";
            if (_schedule.Interval == 1)
            {
                interval = "Daily";
            }
            else if (_schedule.Interval == 7)
            {
                interval = "Weekly";
            }
            else if (_schedule.Interval == 30)
            {
                interval = "Monthly";
            }
            else if (_schedule.Interval == 360)
            {
                interval = "Yearly";
            }
            else if (_schedule.Interval != -1)
            {
                interval = "Every " + _schedule.Interval.ToString() + " days";
            }

            //end time
            string ending = "???";
            if (_schedule.EndOn != -1)
            {
                ending = "until " + Calandar.DateAsString(_schedule.EndOn);
            }
            else if (_schedule.TimesToRepeate == int.MaxValue && _schedule.EndDelay == int.MaxValue)
            {
                ending = "forever";
            }
            else if (_schedule.TimesToRepeate != int.MaxValue)
            {
                ending = "for " + _schedule.TimesToRepeate.ToString() + " times";
            }
            else if (_schedule.EndDelay != int.MaxValue)
            {
                ending = "for " + _schedule.EndDelay.ToString() + " days";
            }


            if (_schedule.TimesToRepeate == 1)
            {
                //for once we dont show end time
                ScheduleLabel.Text = "Once, starting " + starting;
            }            
            else
            {
                //other wise show full string
                ScheduleLabel.Text = interval + ", starting " + starting + ", " + ending + ".";
            }

        }
    }
}
