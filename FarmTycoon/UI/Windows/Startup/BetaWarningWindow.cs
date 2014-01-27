using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;
using System.IO;

namespace FarmTycoon
{
    public partial class BetaWarningWindow : TycoonWindow
    {

        public BetaWarningWindow()
        {
            InitializeComponent();
            
            this.Top = Program.UserInterface.Graphics.WindowHeight - this.Height;
            this.Width = Program.UserInterface.Graphics.WindowWidth;
            this.Left = 0;

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                Program.UserInterface.WindowManager.RemoveWindow(this);
            });
            Program.UserInterface.WindowManager.AddWindow(this);
        }
   
    


    }
}
