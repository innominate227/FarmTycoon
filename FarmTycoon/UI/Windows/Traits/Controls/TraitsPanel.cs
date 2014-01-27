using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{

    /// <summary>
    /// Shows all traits in a trait set
    /// </summary>
    public partial class TraitsPanel : TycoonPanel
    {        
        /// <summary>
        /// Traits being shown.
        /// </summary>
        private ITraitSet _traits;
        
        /// <summary>
        /// Panels for traits
        /// </summary>
        private List<TraitPanel> _traitPanels = new List<TraitPanel>();


        public TraitsPanel()
        {
            //intilize
            InitializeComponent();
                                    
            this.SizeChanged += new Action<TycoonControl>(delegate
            {      
                int scrollBarRoomTraits = 0;
                if ((this.Children.Count * 25) > this.Height + 5)
                {
                    scrollBarRoomTraits = 15;
                }
                foreach (TycoonControl control in this.Children)
                {
                    control.Width = this.Width - scrollBarRoomTraits;
                }
            });

        }

        /// <summary>
        /// Set the traits to show
        /// </summary>
        public void SetTraits(ITraitSet traits)
        {
            //clear out old panels
            foreach (TraitPanel traitPanel in _traitPanels.ToArray())
            {
                this.RemoveChild(traitPanel);
                _traitPanels.Remove(traitPanel);
            }


            _traits = traits;
            if (traits == null)
            {
                return;
            }
                        
            int scrollBarRoom = 0;
            if ((traits.TraitIds.Length * 30) > this.Height + 5)
            {
                scrollBarRoom = 15;
            }

            int traitNum = 0;
            foreach (int traitId in _traits.TraitIds)
            {
                //skip hidden traits
                if (_traits.GetTraitInfo(traitId).Hidden) { continue; }

                TraitPanel traitPanel = new TraitPanel();                
                traitPanel.Tag = null;
                traitPanel.Visible = true;
                traitPanel.Width = this.Width - scrollBarRoom;
                traitPanel.Height = 27;
                traitPanel.Top = 3 + (traitNum * 30);
                traitPanel.Left = 0;
                traitPanel.AnchorTop = true;
                traitPanel.AnchorLeft = true;
                traitPanel.AnchorRight = true;
                traitPanel.AnchorBottom = false;
                traitPanel.BackColor = System.Drawing.Color.FromArgb(255, 192, 64, 64);
                traitPanel.BorderColor = System.Drawing.Color.FromArgb(255, 255, 255, 0);
                traitPanel.ScrollLightColor = System.Drawing.Color.FromArgb(255, 224, 128, 128);
                traitPanel.ScrollDarkColor = System.Drawing.Color.FromArgb(255, 96, 32, 0);
                traitPanel.Scrollable = false;
                traitPanel.AlwaysShowScroll = false;
                traitPanel.Border = false;
                traitPanel.SetTrait(_traits, traitId);
                this.AddChild(traitPanel);
                _traitPanels.Add(traitPanel);
                traitNum++;
            }

            Refresh();
        }

        public void Refresh()
        {
            //nothing to refresh
            if (_traits == null) { return; }
                        
            foreach (TraitPanel traitPanel in _traitPanels)
            {
                traitPanel.Refresh();
            }            
        }



    }
}
