using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class IssueOverlay: TycoonWindow
    {
        public IssueOverlay()
        {
            InitializeComponent();
                        
            //when clicked show all issue window
            StatusLabel.Clicked += new Action<TycoonControl>(delegate
            {
                ShowAllIssuesWindow();
            });
            XMoreLabel.Clicked += new Action<TycoonControl>(delegate
            {
                ShowAllIssuesWindow();
            });
            
            //handle when action button is clicked cancel the task
            ActionButton.Clicked += new Action<TycoonControl>(ActionButton_Clicked);

            //keep window in the bottom center
            MoveToBottomCenter();
            Program.UserInterface.Graphics.Events.WindowSizeChanged += new Action(MoveToBottomCenter);

            //refresh the window as issues change
            Refresh();
            GameState.Current.IssueManager.IssuesListChanged += new Action(Refresh);

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                Program.UserInterface.Graphics.Events.WindowSizeChanged -= new Action(MoveToBottomCenter);
                GameState.Current.IssueManager.IssuesListChanged -= new Action(Refresh);
            });
            
            Program.UserInterface.WindowManager.AddWindow(this);
        }


        private void MoveToBottomCenter()
        {
            this.Width = Program.UserInterface.Graphics.WindowWidth / 2;
            this.Height = 50;
            this.Left = Program.UserInterface.Graphics.WindowWidth / 2 - this.Width / 2;
            this.Top = Program.UserInterface.Graphics.WindowHeight - this.Height;
        }


        private void ActionButton_Clicked(TycoonControl control)
        {
            Issue issue = (control.Tag as Issue);

            if (issue.ObjectWithIssue is Task)
            {
                (issue.ObjectWithIssue as Task).Abort();
            }
            else if (issue.Location != null)
            {
                //TODO:
                //Program.UserInterface.Graphics.ViewX = issue.Location.ScreenX;
                //Program.UserInterface.Graphics.ViewY = issue.Location.ScreenY;
                //Program.UserInterface.Graphics.ViewZ = issue.Location.ScreenZ;
            }
        }


        private void Refresh()
        {
            //get a list of all current issues
            List<Issue> issues = GameState.Current.IssueManager.AllIssues();

            if (issues.Count == 0)
            {
                //if no issue hide overlay
                this.Visible = false;
            }
            else
            {
                //show overlay
                this.Visible = true;

                //show/hide more issye label
                if (issues.Count > 1)
                {
                    XMoreLabel.Text = "(" + (issues.Count - 1).ToString() + " more)";
                }
                else
                {
                    XMoreLabel.Text = "";
                }

                //set startus label for the top issue
                Issue issue = issues[0];                
                StatusLabel.Text = issue.Description;
                ActionButton.Tag = issue;


                //change action button depedning on the type of issue
                if (issue.ObjectWithIssue is Task)
                {                 
                    ActionButton.Text = "Abort";
                    ActionButton.Visible = true;                    
                }
                else if (issue.Location != null)
                {                
                    ActionButton.Text = "Go To";
                    ActionButton.Visible = true;                    
                }
                else
                {
                    ActionButton.Visible = false;
                }


            }
        }


        private void ShowAllIssuesWindow()
        {
            new IssuesWindow();
        }


        
    }
}
