using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class SingleTraitsWindow : TycoonWindow
    {

        public SingleTraitsWindow(TraitSet traits, string label)
        {
            InitializeComponent();

            this.TitleText = label + " Status";

            //main quality
            traitsPanel.SetTraits(traits);

            Program.GameThread.RefreshTimePassed += new Action(Refresh);

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                Program.GameThread.RefreshTimePassed -= new Action(Refresh);
                Program.UserInterface.WindowManager.RemoveWindow(this);                
            });

            Program.UserInterface.WindowManager.AddWindow(this);
        }


        private void Refresh()
        {
            //refresh statistics
            traitsPanel.Refresh();

            //what height should the window be
            int height = 16 + Math.Min(traitsPanel.Children.Count, 10) * 30;            
            if (this.Height != height)
            {
                this.Height = height;                
            }

        }
        
    }
}
