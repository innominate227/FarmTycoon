using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{

    /// <summary>
    /// Trait panel shows trait stats, or growth stats.
    /// </summary>
    public partial class TraitPanel : TycoonPanel
    {
        /// <summary>
        /// the trait id for the trait the panel is showing trait stats for
        /// </summary>
        private int _traitId;

        /// <summary>
        /// The trait set that the trait is in
        /// </summary>
        private ITraitSet _traitSet;

        /// <summary>
        /// Value of the trait when we last updated it,
        /// (used to prevent unessisary refreshes)
        /// </summary>
        private int _lastValue = -1;


        /// <summary>
        /// In scenario edit mode the user can edit the value of the trait
        /// </summary>
        private TycoonTextbox _traitEditBox;

        

        public TraitPanel()
        {
            //intilize
            InitializeComponent();

            //in scenario edit mode we add a textbox that allows the user to edit the value of the trait
            if (Program.Game.ScenarioEditMode)
            {
                AddEditValueBox();
            }
            
            //refresh if window size changes
            this.SizeChanged += new Action<TycoonControl>(delegate(TycoonControl con)
            {
                RefreshTraitInfo();
            });
        }

        private void AddEditValueBox()
        {
            traitGauge.Width -= 60;

            _traitEditBox = new TycoonTextbox();
            _traitEditBox.Name = "traitEditBox";
            _traitEditBox.Tag = null;
            _traitEditBox.Visible = true;
            _traitEditBox.Width = 40;
            _traitEditBox.Height = 15;
            _traitEditBox.Top = 4;
            _traitEditBox.Left = traitGauge.Left + traitGauge.Width + 15;
            _traitEditBox.AnchorTop = true;
            _traitEditBox.AnchorLeft = true;
            _traitEditBox.AnchorRight = false;
            _traitEditBox.AnchorBottom = false;          
            _traitEditBox.TextColor = System.Drawing.Color.FromArgb(255, 0, 0, 0);
            _traitEditBox.TextFont = new System.Drawing.Font("Segoe UI", 8f, System.Drawing.FontStyle.Regular);
            _traitEditBox.BackColor = System.Drawing.Color.FromArgb(255, 255, 255, 255);
            _traitEditBox.BorderColor = System.Drawing.Color.FromArgb(255, 224, 128, 128);
            _traitEditBox.NumbersOnly = true;
            this.AddChild(_traitEditBox);


            TycoonButton traitEditButton = new TycoonButton();
            traitEditButton.Name = "traitEditButton";
            traitEditButton.Tag = null;
            traitEditButton.Visible = true;
            traitEditButton.Width = 10;
            traitEditButton.Height = 15;
            traitEditButton.Top = 4;
            traitEditButton.Left = _traitEditBox.Left + _traitEditBox.Width+3;
            traitEditButton.AnchorTop = true;
            traitEditButton.AnchorLeft = true;
            traitEditButton.AnchorRight = false;
            traitEditButton.AnchorBottom = false;
            traitEditButton.Text = "E";
            traitEditButton.TextColor = System.Drawing.Color.FromArgb(255, 0, 0, 0);
            traitEditButton.TextFont = new System.Drawing.Font("Segoe UI", 8f, System.Drawing.FontStyle.Regular);
            traitEditButton.BackColor = System.Drawing.Color.FromArgb(255, 192, 64, 64);
            traitEditButton.ShadowLightColor = System.Drawing.Color.FromArgb(255, 224, 128, 128);
            traitEditButton.ShadowDarkColor = System.Drawing.Color.FromArgb(255, 96, 32, 0);
            traitEditButton.Clicked += new Action<TycoonControl>(TraitEditButton_Clicked);
            this.AddChild(traitEditButton);

        }
        

        /// <summary>
        /// Set the trait stats to show
        /// </summary>
        public void SetTrait(ITraitSet traitSet, int traitId)
        {
            _traitSet = traitSet;
            _traitId = traitId;
            RefreshTraitInfo();
            Refresh();
        }
        
        public void Refresh()
        {
            if (_traitSet != null)
            {
                //get the traits value, and check if it actually changed
                int traitValue = _traitSet.GetTraitValue(_traitId);
                if (traitValue == _lastValue)
                {
                    return;
                }
                _lastValue = traitValue;

                //if scenario edit mode (edit textbox has been created) set the value in the textbox if its not in focus
                if (_traitEditBox != null && _traitEditBox.IsFocused == false)
                {
                    _traitEditBox.Text = traitValue.ToString();
                }                

                //get the quality.  prefer to show running quality over instanious quality                
                TraitInfo traitInfo = _traitSet.GetTraitInfo(_traitId);
                int traitQuality;
                if (traitInfo.RunningQualityInfo != null)
                {
                    traitQuality = traitInfo.RunningQualityInfo.GetQuality(traitValue);                    
                }
                else if (traitInfo.InstantaneousQualityInfo != null)
                {
                    traitQuality = traitInfo.InstantaneousQualityInfo.GetQuality(traitValue);                    
                }
                else
                {
                    traitQuality = 100;
                }
                
                traitGauge.Value = traitValue;
                traitGauge.Quality = traitQuality;
            }
        }


        /// <summary>
        /// Refresh the trait info shown
        /// </summary>
        private void RefreshTraitInfo()
        {
            if (_traitSet != null)
            {
                TraitInfo traitInfo = _traitSet.GetTraitInfo(_traitId);

                traitName.Text = traitInfo.Name + ":";
                int minValue = traitInfo.MinimumValue;
                int maxValue = traitInfo.MaximumValue;
                traitGauge.MinValue = minValue;
                traitGauge.MaxValue = maxValue;

                //get quality info to determine guage ranges (prefer running quality)
                TraitQualityInfo qualityInfo = null;
                if (traitInfo.RunningQualityInfo != null)
                {
                    qualityInfo = traitInfo.RunningQualityInfo;
                }
                else if (traitInfo.InstantaneousQualityInfo != null)
                {
                    qualityInfo = traitInfo.InstantaneousQualityInfo;
                }

                if (qualityInfo != null)
                {
                    int goodColorStart = CalculateValueBelowForQuality(66, qualityInfo);
                    int goodColorEnd = CalculateValueAboveForQuality(66, qualityInfo);
                    int midColorStart = CalculateValueBelowForQuality(33, qualityInfo);
                    int midColorEnd = CalculateValueAboveForQuality(33, qualityInfo);

                    if (goodColorStart < minValue) { goodColorStart = minValue; }
                    if (goodColorStart > maxValue) { goodColorStart = maxValue; }
                    if (goodColorEnd < minValue) { goodColorEnd = minValue; }
                    if (goodColorEnd > maxValue) { goodColorEnd = maxValue; }
                    if (midColorStart < minValue) { midColorStart = minValue; }
                    if (midColorStart > maxValue) { midColorStart = maxValue; }
                    if (midColorEnd < minValue) { midColorEnd = minValue; }
                    if (midColorEnd > maxValue) { midColorEnd = maxValue; }

                    traitGauge.MidColorStart = midColorStart;
                    traitGauge.MidColorEnd = midColorEnd;
                    traitGauge.GoodColorStart = goodColorStart;
                    traitGauge.GoodColorEnd = goodColorEnd;
                }
                else
                {
                    traitGauge.MidColorStart = minValue;
                    traitGauge.MidColorEnd = maxValue;
                    traitGauge.GoodColorStart = minValue;
                    traitGauge.GoodColorEnd = maxValue;
                }
            }
        }

        /// <summary>
        /// Determine what the value of the trait would need to be for the quality to be the value passed,
        /// where the value of the trait is above the optimal value
        /// </summary>
        private int CalculateValueAboveForQuality(int quality, TraitQualityInfo traitQualityInfo)
        {
            return (int)Math.Round((Math.Pow((quality - 100.0) / -100.0, 1.0 / traitQualityInfo.LeniencyAbove) * traitQualityInfo.MaxAbove) + traitQualityInfo.Optimal);
        }

        /// <summary>
        /// Determine what the value of the trait would need to be for the quality to be the value passed,
        /// where the value of the trait is below the optimal value
        /// </summary>
        private int CalculateValueBelowForQuality(int quality, TraitQualityInfo traitQualityInfo)
        {
            return (int)Math.Round(-1 * (Math.Pow((quality - 100.0) / -100.0, 1.0 / traitQualityInfo.LeniencyBelow) * traitQualityInfo.MaxBelow) + traitQualityInfo.Optimal);
        }


        /// <summary>
        /// Trait value edit button was clicked set the value of the trait
        /// </summary>
        private void TraitEditButton_Clicked(TycoonControl obj)
        {
            int value;
            if (int.TryParse(_traitEditBox.Text, out value))
            {
                _traitSet.SetTraitValue(_traitId, value);
            }
        }


    }
}
