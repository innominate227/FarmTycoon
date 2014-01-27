using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class IssuesWindow : TycoonWindow
    {
        /// <summary>
        /// Issue labels that have been keyed by the issue they were created for.        
        /// </summary>
        private Dictionary<Issue, TycoonLabel> _issueLabels = new Dictionary<Issue, TycoonLabel>();

        /// <summary>
        /// The issue that is selected in the window
        /// </summary>
        private Issue _selectedIssue = null;

        /// <summary>
        /// The index of the selected issue in the window
        /// </summary>
        private int _selectedIssueIndex = 0;



        public IssuesWindow()
        {
            InitializeComponent();
            
            this.Top = (Program.UserInterface.Graphics.WindowHeight / 2) - (this.Height / 2);
            this.Left = (Program.UserInterface.Graphics.WindowWidth / 2) - (this.Width / 2);

            GameState.Current.IssueManager.IssuesListChanged += new Action(RefreshWindow);
            
            RefreshWindow();

            this.ActionButton.Clicked += new Action<TycoonControl>(ActionButton_Clicked);
            
            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                GameState.Current.IssueManager.IssuesListChanged -= new Action(RefreshWindow);

                Program.UserInterface.WindowManager.RemoveWindow(this);
            });
                 
            Program.UserInterface.WindowManager.AddWindow(this);
        }
                

        private void RefreshWindow()
        {
            //get the list of issues to show
            List<Issue> issues = GameState.Current.IssueManager.AllIssues();
            
            //delete any labels that are not visible
            foreach (Issue issueWithLabel in _issueLabels.Keys)
            {
                if (issues.Contains(issueWithLabel) == false)
                {
                    IssuesPanel.RemoveChild(_issueLabels[issueWithLabel]);
                }
            }

            //we need to select a new issue if the one that was selected is no longer being shown
            if (issues.Contains(_selectedIssue) == false)
            {
                _selectedIssue = null;
                if (issues.Count > 0)
                {
                    if (issues.Count - 1 < _selectedIssueIndex)
                    {
                        _selectedIssueIndex = issues.Count - 1;
                    }
                    _selectedIssue = issues[_selectedIssueIndex];
                }
            }
            

            //make/show the label for each issue
            int issueNum = 0;
            foreach (Issue issue in issues)
            {
                //get or create the issue label
                if (_issueLabels.ContainsKey(issue) == false)
                {
                    TycoonLabel newIssueLabel = new TycoonLabel();
                    newIssueLabel.Left = 0;
                    newIssueLabel.Width = IssuesPanel.Width;
                    newIssueLabel.Height = 30;
                    newIssueLabel.Clicked += new Action<TycoonControl>(Issue_Selected);
                    newIssueLabel.Text = issue.Description;
                    IssuesPanel.AddChild(newIssueLabel);
                    _issueLabels.Add(issue, newIssueLabel);
                }

                //determine if this is the selected task
                Color backColor = IssuesPanel.BackColor;
                if (issue == _selectedIssue)
                {
                    backColor = Color.Blue;
                    _selectedIssueIndex = issueNum;
                }

                //move issue label to the right spot
                TycoonLabel issueLabel = _issueLabels[issue];
                issueLabel.Top = issueNum * issueLabel.Height;                
                issueLabel.Visible = true;
                issueLabel.BackColor = backColor;
                issueLabel.Tag = issue;
                
                //next one will be further down
                issueNum += 1;                
            }

            UpdateActionButtonForSelectedIssue();
        }


        /// <summary>
        /// Called when the selected issue has changed
        /// </summary>
        private void Issue_Selected(TycoonControl selectedLabel)
        {
            Issue selected = selectedLabel.Tag as Issue;

            //do nothing if already selected
            if (selected == _selectedIssue) { return; }

            //unselect old and select new
            _issueLabels[_selectedIssue].BackColor = IssuesPanel.BackColor;
            _issueLabels[selected].BackColor = Color.Blue;

            //update currently selected issue
            _selectedIssue = selected;
            _selectedIssueIndex = selectedLabel.Top / selectedLabel.Height;

            //update action button
            UpdateActionButtonForSelectedIssue();
        }


        private void UpdateActionButtonForSelectedIssue()
        {
            if (_selectedIssue == null)
            {
                ActionButton.Visible = false;
            }
            else if (_selectedIssue.ObjectWithIssue is Task)
            {
                ActionButton.Text = "Abort";
                ActionButton.Visible = true;
            }
            else if (_selectedIssue.Location != null)
            {
                ActionButton.Text = "Go To";
                ActionButton.Visible = true;
            }
            else
            {
                ActionButton.Visible = false;
            }
        }



        private void ActionButton_Clicked(TycoonControl obj)
        {
            if (_selectedIssue != null)
            {
                if (_selectedIssue.ObjectWithIssue is Task)
                {
                    (_selectedIssue.ObjectWithIssue as Task).Abort();
                }
                else if (_selectedIssue.Location != null)
                {
                    Program.UserInterface.Graphics.ViewX = _selectedIssue.Location.X;
                    Program.UserInterface.Graphics.ViewY = _selectedIssue.Location.Y;
                    Program.UserInterface.Graphics.ViewZ = _selectedIssue.Location.Z;
                }
            }
        }
    }
}
