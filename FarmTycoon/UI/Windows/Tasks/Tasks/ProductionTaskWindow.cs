using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class ProductionWindow : TycoonWindow
    {   
        /// <summary>
        /// The task being planned
        /// </summary>
        private ProductionBuilding _productionBuilding;

        /// <summary>
        /// The number of workers currently working in the production building
        /// </summary>
        private int _currentNumberOfWorkers;


        public ProductionWindow(ProductionBuilding productionBuilding)
        {
            InitializeComponent();

            _productionBuilding = productionBuilding;
            this.TitleText = "Work in " + productionBuilding.Name;

            //this task is slightly different, workers never complete it.
            //for number of workers show the current number of workers in the building (or going to the building).
            //Note: there is only every one worker per ProductionTask
            _currentNumberOfWorkers = productionBuilding.WorkersInside.WorkersInside.Count + productionBuilding.WorkersInside.WorkersHeadingToward.Count;
            CurrentWorkersLabel.Text = _currentNumberOfWorkers.ToString();
            MaxWorkersLabel.Text = productionBuilding.BuildingInfo.MaxWorkers.ToString();
            
            //setup number of workers panel
            numberOfWorkersPanel.Allow0Workers = true;
            numberOfWorkersPanel.NumberOfWorkers = _currentNumberOfWorkers;
            numberOfWorkersPanel.PreferredList = new List<Worker>();
            numberOfWorkersPanel.NumberOfWorkersChanged += new Action(delegate
            {                
                RefreshTime();
                RefreshWarnings();
                issuesAndTimePanel.Refresh();
            });

            //setup issues panel (note we just pass it a task because it has to have one, we override the time and warning messages)
            ProductionTask unused = new ProductionTask();
            unused.ProductionBuilding = _productionBuilding;
            issuesAndTimePanel.SetTask(unused);

            //refresh the expected time and warnings
            RefreshTime();
            RefreshWarnings();
            issuesAndTimePanel.Refresh();
            
            okButton.Clicked += new Action<TycoonControl>(OkButton_Clicked);
            cancelButton.Clicked += new Action<TycoonControl>(delegate { CloseWindow(); });
            Program.UserInterface.Graphics.Events.MouseDown += new MouseEventHandler(Graphics_MouseDown);

            //add window to game
            Program.UserInterface.WindowManager.AddWindow(this);


            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                Program.UserInterface.Graphics.Events.MouseDown -= new MouseEventHandler(Graphics_MouseDown);
                Program.UserInterface.WindowManager.RemoveWindow(this); 	        
            });
        }


        private void RefreshTime()
        {
            int maxWorkers = _productionBuilding.BuildingInfo.MaxWorkers;
            int proposedWorkers = numberOfWorkersPanel.NumberOfWorkers;
            double interval = _productionBuilding.BuildingInfo.Interval;

            if (proposedWorkers == 0)
            {
                issuesAndTimePanel.TimeOverride = "Production Building Inactive";
            }
            else
            {
                double actualInterval = interval;
                if (proposedWorkers < maxWorkers)
                {
                    actualInterval = interval * ((double)maxWorkers / (double)proposedWorkers);
                }
                issuesAndTimePanel.TimeOverride = "Time: " + String.Format("{0:0.00}", actualInterval) + " days per Production";
            }
        }

        private void RefreshWarnings()
        {
            int maxWorkers = _productionBuilding.BuildingInfo.MaxWorkers;
            int proposedWorkers = numberOfWorkersPanel.NumberOfWorkers;
            int diff = proposedWorkers - _currentNumberOfWorkers;

            //create worker movement string
            string issuesString = "";
            if (diff == 0)
            {
                issuesString = "All workers will reaming working.";
            }
            else if (diff > 0)
            {
                if (diff == 1)
                {
                    issuesString = "1 worker will start working.";
                }
                else
                {
                    issuesString = diff.ToString() + " workers will start working.";
                }
            }
            else if (diff < 0)
            {
                if (diff == -1)
                {
                    issuesString = "1 worker will stop working.";
                }
                else
                {
                    issuesString = (-1 * diff).ToString() + " workers will stop working.";
                }                
            }

            //add overstaffed/understaffed string
            if (proposedWorkers > maxWorkers)
            {
                issuesString += "  Building will be over-staffed.";
            }
            else if (proposedWorkers == maxWorkers)
            {
                issuesString += "  Building will be  fully-staffed.";
            }
            else if (proposedWorkers < maxWorkers)
            {
                issuesString += "  Building will be under-staffed.";
            }
            issuesAndTimePanel.IssuesOverride = issuesString;
        }

        private void Graphics_MouseDown(ClickInfo clickInfo)
        {
            //close window if something that is not in this window was clicked
            //dont close if event is from pie menu button clicked to create this window, or from control clicked in dropbox window
            if (clickInfo.ControlClicked == null || (clickInfo.ControlClicked.ParentWindow != this && clickInfo.ControlClicked.ParentWindow is PieMenuWindow == false && clickInfo.ControlClicked.ParentWindow.TitleText != "Dropbox"))
            {
                CloseWindow();
            }
        }
        
        private void OkButton_Clicked(TycoonControl obj)
        {
            int proposedWorkers = numberOfWorkersPanel.NumberOfWorkers;
            int diff = proposedWorkers - _currentNumberOfWorkers;

            if (diff > 0)
            {
                for (int newTaskNum = 0; newTaskNum < diff; newTaskNum++)
                {
                    //start task to get more workers there
                    ProductionTask productionTask = new ProductionTask();
                    productionTask.ExtraDelay = newTaskNum;
                    productionTask.NumberOfWorkers = 1;
                    productionTask.PreferredWorkers.AddRange(numberOfWorkersPanel.PreferredList);
                    productionTask.ProductionBuilding = _productionBuilding;
                    productionTask.DesiredStartDate = GameState.Current.Calandar.Date;
                    productionTask.TryToStart();
                }
            }
            else if (diff < 0)
            {
                //how many we still need to abort
                int numLeftToAbort = diff * -1;

                //first abort workers that are still walking toward the building
                foreach (Worker worker in _productionBuilding.WorkersInside.WorkersHeadingToward.ToArray())
                {
                    //dnoe if none left to abort
                    if (numLeftToAbort == 0)
                    {
                        break;
                    }

                    //abort this one
                    worker.AbortTask();

                    //keep track of number left to abort
                    numLeftToAbort--;                    
                }

                //now abort workers that are already in the building
                foreach (Worker worker in _productionBuilding.WorkersInside.WorkersInside.ToArray())
                {
                    //dnoe if none left to abort
                    if (numLeftToAbort == 0)
                    {
                        break;
                    }

                    //abort this one
                    worker.AbortTask();

                    //keep track of number left to abort
                    numLeftToAbort--;                    
                }
            }
            //if diff is 0 do nothing
            
            CloseWindow();
        }

    }
}
