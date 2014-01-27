using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class SingleQualityWindow : TycoonWindow
    {

        private IQuality _quality;

        public SingleQualityWindow(IQuality quality, string label)
        {
            InitializeComponent();

            _quality = quality;
            this.TitleText = label + " Status";

            //main quality
            traitsPanel.SetTraits(quality);

            Program.GameThread.RefreshTimePassed += new Action(Refresh);

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                Program.GameThread.RefreshTimePassed -= new Action(Refresh);
                Program.UserInterface.WindowManager.RemoveWindow(this);                
            });

            Program.UserInterface.WindowManager.AddWindow(this);

            Refresh();
        }
        

        private void Refresh()
        {
            qualityGauge.Value = _quality.CurrentQuality;
            qualityGauge.Quality = _quality.CurrentQuality;

            //refresh traits
            traitsPanel.Refresh();
            
            //what height should the window be
            int height = 46 + (Math.Min(traitsPanel.Children.Count, 10) * 30);                        
            if (this.Height != height)
            {
                this.Height = height;
                if (traitsPanel.Children.Count > 10)                    
                {
                    qualityGauge.Width = this.Width - 82 - 15;
                }
                else
                {
                    qualityGauge.Width = this.Width - 82;
                }
            }

        }
        
    }
}
