using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{

    /// <summary>
    /// Number of workers panel
    /// </summary>
    public partial class IssuesAndTimePanel : TycoonPanel
    {
        private Task _task;

        /// <summary>
        /// Time string that would normally be shown is overridden with this string if its not blank
        /// </summary>
        private string _timeOverride = "";

        /// <summary>
        /// Issues that would normally be shown are overrideen with this string is its not blank.
        /// </summary>
        private string _issuesOverride = "";
        
        
        public IssuesAndTimePanel()
        {
            //intilize
            InitializeComponent();
        }

        public void SetTask(Task task)
        {
            _task = task;
            Refresh();
        }


        /// <summary>
        /// Time string that would normally be shown is overridden with this string if its not blank
        /// </summary>
        public string TimeOverride
        {
            get { return _timeOverride; }
            set { _timeOverride = value; }
        }

        /// <summary>
        /// Issues that would normally be shown are overrideen with this string is its not blank.
        /// </summary>
        public string IssuesOverride
        {
            get { return _issuesOverride; }
            set { _issuesOverride = value; }
        }
                

        public void Refresh()
        {            
            TaskPlan taskPlan = _task.PlanTask();
            int time = taskPlan.CalculateExpectedTime();
            string issues = taskPlan.IssuesString();
            string warnings = taskPlan.WarningsString();

            string issuesAndWarnings = "";
            if (issues != "")
            {
                issuesAndWarnings = "Issues: " + issues;
            }
            if (warnings != "")
            {
                issuesAndWarnings += "  Warnings: " + warnings;
                issuesAndWarnings = issuesAndWarnings.Trim();
            }
            if (_issuesOverride != "")
            {
                issuesAndWarnings = _issuesOverride;
            }


            //set the issues
            if (issuesAndWarnings == "")
            {
                issuesLabel.Text = "";
                if (issuesLabel.Visible)
                {
                    issuesLabel.Visible = false;
                    this.Height -= 80;

                    //adjust parent window to be shorter
                    this.ParentWindow.Height -= 80;

                    //fix controls that were below this one
                    foreach (TycoonControl con in this.ParentWindow.Children)
                    {
                        if (con.Top > this.Top)
                        {
                            con.Top -= 80;
                        }                        
                    }
                }
            }
            else
            {
                issuesLabel.Text = issuesAndWarnings;
                if (issuesLabel.Visible == false)
                {
                    issuesLabel.Visible = true;
                    this.Height += 80;

                    //adjust parent window to be shorter
                    this.ParentWindow.Height += 80;

                    //fix controls that were below this one
                    foreach (TycoonControl con in this.ParentWindow.Children)
                    {
                        if (con.Top > this.Top)
                        {
                            con.Top += 80;
                        }
                    }
                }
            }

            //set the time label
            if (_timeOverride != "")
            {
                expectedTimeLabel.Text = _timeOverride;
            }
            else if (time == -1)
            {
                expectedTimeLabel.Text = "Expected Time: ??? days";
            }
            else if (time == 1)
            {
                expectedTimeLabel.Text = "Expected Time: " + time.ToString() + " day";
            }
            else
            {
                expectedTimeLabel.Text = "Expected Time: " + time.ToString() + " days";
            }
        }


    }
}
