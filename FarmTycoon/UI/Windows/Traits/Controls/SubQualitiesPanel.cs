using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{

    public partial class SubQualitiesPanel : TycoonPanel
    {        
        /// <summary>
        /// Panels for sub quality
        /// </summary>
        private List<SubQualityPanel> _subQualityPanels = new List<SubQualityPanel>();

        public SubQualitiesPanel()
        {
            //intilize
            InitializeComponent();
                                    
            this.SizeChanged += new Action<TycoonControl>(delegate
            {                
                int scrollBarRoomSubQualitites = 0;
                if ((this.Children.Count * 30) > this.Height + 5)
                {
                    scrollBarRoomSubQualitites = 15;
                }
                foreach (TycoonControl control in this.Children)
                {
                    control.Width = this.Width - scrollBarRoomSubQualitites;
                }
            });

        }
        

        /// <summary>
        /// Set a list of subqualities to show
        /// </summary>
        public void SetSubQualities(Dictionary<string, IQuality> subQualities)
        {
            //clear out old panels
            foreach (SubQualityPanel subQualityPanel in _subQualityPanels.ToArray())
            {
                this.RemoveChild(subQualityPanel);
                _subQualityPanels.Remove(subQualityPanel);
            }

            int scrollBarRoom = 0;
            if ((subQualities.Count * 30) > this.Height + 5)
            {
                scrollBarRoom = 15;
            }
            
            int subQualityNum = 0;
            foreach (string subQualityName in subQualities.Keys)
            {
                IQuality subQuality = subQualities[subQualityName];

                SubQualityPanel subQualityPanel = new SubQualityPanel();
                subQualityPanel.Tag = null;
                subQualityPanel.Visible = true;
                subQualityPanel.Width = this.Width - scrollBarRoom;
                subQualityPanel.Height = 27;
                subQualityPanel.Top = 3 + (subQualityNum * 30);
                subQualityPanel.Left = 0;
                subQualityPanel.AnchorTop = true;
                subQualityPanel.AnchorLeft = true;
                subQualityPanel.AnchorRight = true;
                subQualityPanel.AnchorBottom = false;
                subQualityPanel.BackColor = System.Drawing.Color.FromArgb(255, 192, 64, 64);
                subQualityPanel.BorderColor = System.Drawing.Color.FromArgb(255, 255, 255, 0);
                subQualityPanel.ScrollLightColor = System.Drawing.Color.FromArgb(255, 224, 128, 128);
                subQualityPanel.ScrollDarkColor = System.Drawing.Color.FromArgb(255, 96, 32, 0);
                subQualityPanel.Scrollable = false;
                subQualityPanel.AlwaysShowScroll = false;
                subQualityPanel.Border = false;
                subQualityPanel.SetQuality(subQuality, subQualityName);
                this.AddChild(subQualityPanel);
                _subQualityPanels.Add(subQualityPanel);
                subQualityNum++;
            }

            Refresh();

        }



        public void Refresh()
        {
            foreach (SubQualityPanel subQualityPanel in _subQualityPanels)
            {
                subQualityPanel.Refresh();
            }            
        }



    }
}
