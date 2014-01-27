using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    public class RightToolbar : ToolBarWindow
    {

        public RightToolbar()            
        {
            base.Init(new string[] { "S+", "S-", "Pause", "VL", "VR", "V+", "V-", "Save", "Exit" }, -1);
            
            MoveToTopLeft();
            Program.UserInterface.Graphics.Events.WindowSizeChanged += new Action(MoveToTopLeft);

            this.CloseClicked += new Action<TycoonGraphicsLib.TycoonWindow>(delegate
            {
                Program.UserInterface.Graphics.Events.WindowSizeChanged -= new Action(MoveToTopLeft);
            });

            this.ToolClicked += new Action<string, int>(ToolClickedHandler);
        }

        private void MoveToTopLeft()
        {
            this.Top = 5;
            this.Left = Program.UserInterface.Graphics.WindowWidth - 35;
        }


        private void ToolClickedHandler(string tool, int position)
        {
            if (tool == "S+")
            {
                Program.GameThread.ClockDriver.DesiredRate *= 2.0;
            }
            else if (tool == "S-")
            {
                Program.GameThread.ClockDriver.DesiredRate *= 0.5;                
            }
            else if (tool == "Pause")
            {
                Program.GameThread.ClockDriver.Paused = !Program.GameThread.ClockDriver.Paused;
            }
            else if (tool == "VL")
            {
                Program.UserInterface.Graphics.ViewRotation = DirectionUtils.ClockwiseOne(Program.UserInterface.Graphics.ViewRotation);
            }
            else if (tool == "VR")
            {
                Program.UserInterface.Graphics.ViewRotation = DirectionUtils.CounterClockwiseOne(Program.UserInterface.Graphics.ViewRotation);
            }
            else if (tool == "V+")
            {
                Program.UserInterface.Graphics.Scale *= 2.0f;
            }
            else if (tool == "V-")
            {
                Program.UserInterface.Graphics.Scale *= 0.5f;
            }
            else if (tool == "Save")
            {
                if (Program.Game.ScenarioEditMode)
                {
                    new SaveGameWindow(Program.Settings.ScenariosFolder);
                }
                else
                {
                    new SaveGameWindow(Program.Settings.SavesFolder);
                }
            }
            else if (tool == "Exit")
            {
                YesNoWindow confirmWindow = new YesNoWindow("Exit?", "Are you sure you want to Exit?", "Yes", "No", true, 200, 65, delegate
                {
                    //close the game
                    GameFile.Close();

                    //show start window
                    new StartWindow();
                }, delegate { });                
            }
        }


    }
}

