using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class ProgressWindow : TycoonWindow
    {
        public ProgressWindow(string message)
        {
            InitializeComponent();
            this.TitleBar = false;
            this.Height -= 13;
            
            progress.Text = message;

            this.Top = (Program.UserInterface.Graphics.WindowHeight / 2) - (this.Height / 2);
            this.Left = (Program.UserInterface.Graphics.WindowWidth / 2) - (this.Width / 2);
               
            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                Program.UserInterface.WindowManager.RemoveWindow(this);
            });
                 
            Program.UserInterface.WindowManager.AddWindow(this);
        }

        /// <summary>
        /// Max value of the progress bar
        /// </summary>
        public int MaxValue
        {
            get { return progress.MaxValue; }
            set { progress.MaxValue = value; }
        }

        /// <summary>
        /// Progress of the bar
        /// </summary>
        public int Progress
        {
            get { return progress.Progress; }
            set { progress.Progress = value; }
        }
   
    


    }
}
