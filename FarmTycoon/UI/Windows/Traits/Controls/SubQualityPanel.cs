using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{

    /// <summary>
    /// SubQuality panel shows overall quality managed by a quality manager, and a button that allows the user to click to see full quality info
    /// </summary>
    public partial class SubQualityPanel : TycoonPanel
    {        
        /// <summary>
        /// Quality being shown.
        /// </summary>
        private IQuality _quality;

        /// <summary>
        /// Value of the quality when we last updated it,
        /// (used to prevent unessisary refreshes)
        /// </summary>
        private int _lastQuality = -1;


        public SubQualityPanel()
        {
            //intilize
            InitializeComponent();            
        }
        

        /// <summary>
        /// Set the quality to show
        /// </summary>
        public void SetQuality(IQuality quality, string subQualityName)
        {
            itemButton.Text = subQualityName;
            _quality = quality;
            Refresh();

            itemButton.Clicked += new Action<TycoonControl>(delegate
            {
                new SingleQualityWindow(_quality, itemButton.Text);
            });
        }

        public void Refresh()
        {
            int quality = _quality.CurrentQuality;
            if (quality == _lastQuality) { return; }

            _lastQuality = quality;
            qualityGauge.Value = quality;            
            qualityGauge.Quality = quality;                        

        }



    }
}
