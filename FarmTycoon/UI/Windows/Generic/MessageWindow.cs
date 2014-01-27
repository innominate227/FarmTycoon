using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class MessageWindow : TycoonWindow
    {
        private bool _wasPaused;

        public MessageWindow(string title, string message, bool pause, int width, int height)
        {
            InitializeComponent();

            this.Width = width;
            this.Height = height;

            _wasPaused = Program.GameThread.ClockDriver.Paused;
            if (pause)
            {
                Program.GameThread.ClockDriver.Paused = true;
            }

            this.TitleText = title;
            messageLabel.Text = message;

            this.Top = (Program.UserInterface.Graphics.WindowHeight / 2) - (this.Height / 2);
            this.Left = (Program.UserInterface.Graphics.WindowWidth / 2) - (this.Width / 2);
                

            okButton.Clicked += new Action<TycoonControl>(delegate
            {
                this.CloseWindow();
            });
            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                if (pause && _wasPaused == false)
                {
                    Program.GameThread.ClockDriver.Paused = false;
                }

                Program.UserInterface.WindowManager.RemoveWindow(this);
            });
                 
            Program.UserInterface.WindowManager.AddWindow(this);
        }
   
    


    }
}
