using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class YesNoWindow : TycoonWindow
    {
        private bool _wasPaused;

        /// <summary>
        /// Action to when if user presses yes
        /// </summary>
        private Action _yesAction;

        /// <summary>
        /// Action to when if user presses no
        /// </summary>
        private Action _noAction;


        public YesNoWindow(string title, string message, string yesText, string noText, bool pause, int width, int height, Action yesAction, Action noAction)
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
            yesButton.Text = yesText;
            noButton.Text = noText;

            yesButton.Width = (this.Width - 15) / 2;
            noButton.Width = (this.Width - 15) / 2;
            noButton.Left = yesButton.Left + yesButton.Width + 5;

            _yesAction = yesAction;
            _noAction = noAction;

            this.Top = (Program.UserInterface.Graphics.WindowHeight / 2) - (this.Height / 2);
            this.Left = (Program.UserInterface.Graphics.WindowWidth / 2) - (this.Width / 2);
                

            yesButton.Clicked += new Action<TycoonControl>(delegate
            {
                if (_yesAction != null)
                {
                    _yesAction();
                }
                _yesAction = null;
                _noAction = null;
                this.CloseWindow();
            });
            noButton.Clicked += new Action<TycoonControl>(delegate
            {
                if (_noAction != null)
                {
                    _noAction();
                }
                _yesAction = null;
                _noAction = null;
                this.CloseWindow();
            });

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                if (_noAction != null)
                {
                    _noAction();
                }

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
