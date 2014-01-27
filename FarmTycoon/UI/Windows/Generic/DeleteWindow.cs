using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class DeleteWindow : TycoonWindow
    {
        private bool _wasPaused;

        private GameObject _toDelete;

        public DeleteWindow(GameObject toDelete)
        {
            InitializeComponent();
            _toDelete = toDelete;
            
            _wasPaused = Program.GameThread.ClockDriver.Paused;            
            Program.GameThread.ClockDriver.Paused = true;
            
            //set congfirmation label
            this.TitleText = "Delete " + toDelete.Name + "?";
            messageLabel.Text = "Are you sure you want to delete " + toDelete.Name + "?";            
            if (toDelete is Field)
            {
                messageLabel.Text += "\n" + "Any plants in the field will be destoryed.";
            }
            else if (toDelete is Pasture)
            {
                messageLabel.Text += "\n" + "Any animals in the pasture will be destoryed.";
            }
            else if (toDelete is IStorageBuilding)
            {
                messageLabel.Text += "\n" + "Any items in the building will be destoryed.";
            }

            //mention if scheduled tasks will be ended because of this
            int scheduledTasks = GameState.Current.MasterTaskList.GetScheduledTaskDependingOn(toDelete).Count;
            if (scheduledTasks > 0)
            {
                messageLabel.Text += "\n" + scheduledTasks.ToString() + " scheduled tasks involving this building will be stopped.";
            }

            //find actions involving the object
            bool invlovedWithAction = GameState.Current.ActiveActionList.IsObjectInvolvedWithActiveAction(toDelete);
            if (invlovedWithAction)
            {
                string areaType = "object";
                string areaTypeCap = "Object";
                if (toDelete is IStorageBuilding)
                {
                    areaType = "building";
                    areaTypeCap = "Building";   
                }
                else if (toDelete is Field)
                {
                    areaType = "field";
                    areaTypeCap = "Field";
                }
                else if (toDelete is Pasture)
                {
                    areaType = "pasture";
                    areaTypeCap = "Pastrue";
                }
                messageLabel.Text += "\n\n" + areaTypeCap + " will be destoryed once currently active tasks involving the " + areaType + " are finsihed.";
            }
            
            this.Top = (Program.UserInterface.Graphics.WindowHeight / 2) - (this.Height / 2);
            this.Left = (Program.UserInterface.Graphics.WindowWidth / 2) - (this.Width / 2);
                

            yesButton.Clicked += new Action<TycoonControl>(delegate
            {
                _toDelete.Delete(); 
                this.CloseWindow();
            });
            noButton.Clicked += new Action<TycoonControl>(delegate
            {
                this.CloseWindow();
            });

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                if (_wasPaused == false)
                {
                    Program.GameThread.ClockDriver.Paused = false;
                }
                Program.UserInterface.WindowManager.RemoveWindow(this);
            });
                 
            Program.UserInterface.WindowManager.AddWindow(this);
        }
            


    }
}
