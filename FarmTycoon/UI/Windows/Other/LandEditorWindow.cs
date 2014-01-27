using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace FarmTycoon
{
    public partial class LandEditorWindow : TycoonWindow
    {
        public LandEditorWindow()
        {
            InitializeComponent();
                                    
            //center
            this.Top = (Program.UserInterface.Graphics.WindowHeight / 2) - (this.Height / 2);
            this.Left = (Program.UserInterface.Graphics.WindowWidth / 2) - (this.Width / 2);

            RandomLandButton.Clicked += new Action<TycoonControl>(RandomLandButton_Clicked);
            MidpointDisplacementButton.Clicked += new Action<TycoonControl>(MidpointDisplacementButton_Clicked);
            RiverGoButton.Clicked += new Action<TycoonControl>(RiverGoButton_Clicked);

            PickLand1Button.Clicked += new Action<TycoonControl>(delegate
            {
                Land oldLand = (Land)RiverLand1Label.Tag;
                if (oldLand != null)
                {
                    oldLand.CornerToHighlight = LandCorner.None;
                }

                PickLandEditor pickLand = new PickLandEditor();
                pickLand.StartEditing();
                pickLand.LandPicked += new Action<Land>(delegate(Land landPicked)
                {
                    landPicked.CornerToHighlight = LandCorner.Center;
                    RiverLand1Label.Tag = landPicked;
                    RiverLand1Label.Text = landPicked.LocationOn.X.ToString() + "," + landPicked.LocationOn.Y.ToString();
                });
            });
            PickLand2Button.Clicked += new Action<TycoonControl>(delegate
            {
                Land oldLand = (Land)RiverLand2Label.Tag;
                if (oldLand != null)
                {
                    oldLand.CornerToHighlight = LandCorner.None;
                }

                PickLandEditor pickLand = new PickLandEditor();
                pickLand.StartEditing();
                pickLand.LandPicked += new Action<Land>(delegate(Land landPicked)
                {
                    landPicked.CornerToHighlight = LandCorner.Center;
                    RiverLand2Label.Tag = landPicked;
                    RiverLand2Label.Text = landPicked.LocationOn.X.ToString() + "," + landPicked.LocationOn.Y.ToString();
                });
            });

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                Program.UserInterface.WindowManager.RemoveWindow(this);
            });
            Program.UserInterface.WindowManager.AddWindow(this);
        }

        
        private void RandomLandButton_Clicked(TycoonControl obj)
        {
            int adjustPoints;
            int maxHeight;
            int minHeight;

            bool goodParse = int.TryParse(RandomAdjustPointsTextbox.Text, out adjustPoints);
            bool goodParse2 = int.TryParse(RandomMaxHeightTextbox.Text, out maxHeight);
            bool goodParse3 = int.TryParse(RandomMinHeightTextbox.Text, out minHeight);

            if (goodParse && goodParse2 && goodParse3)
            {
                LandHeightAssigner landHeight = new LandHeightAssigner();
                landHeight.RandomHillPoints(adjustPoints, minHeight, maxHeight);
            }
        }
        
        private void MidpointDisplacementButton_Clicked(TycoonControl obj)
        {
            float roughness;
            float smooth;
            int maxHeight;
            int minHeight;

            bool goodParse = float.TryParse(MidpointRoughnessTextbox.Text, out roughness);
            bool goodParse2 = float.TryParse(MidpointSmoothTextbox.Text, out smooth);
            bool goodParse3 = int.TryParse(MidpointMaxHeightTextbox.Text, out maxHeight);
            bool goodParse4 = int.TryParse(MidpointMinHeightTextbox.Text, out minHeight);

            if (goodParse && goodParse2 && goodParse3 && goodParse4)
            {
                LandHeightAssigner landHeight = new LandHeightAssigner();
                landHeight.ModpointDisplacement(roughness, smooth, minHeight, maxHeight);
            }
        }
        
        private void RiverGoButton_Clicked(TycoonControl obj)
        {
            
            double curve;
            int maxHeight;
            int minHeight;
            int maxSize;
            int minSize;
            Land land1 = (Land)RiverLand1Label.Tag;
            Land land2 = (Land)RiverLand2Label.Tag;

            bool goodParse = double.TryParse(RiverCurveTextbox.Text, out curve);
            bool goodParse2 = int.TryParse(RiverMaxSizeTextbox.Text, out maxSize);
            bool goodParse3 = int.TryParse(RiverMinSizeTextbox.Text, out minSize);
            bool goodParse4 = int.TryParse(RiverMaxHeightTextbox.Text, out maxHeight);
            bool goodParse5 = int.TryParse(RiverMinHeightTextbox.Text, out minHeight);


            if (goodParse && goodParse2 && goodParse3 && goodParse4 && goodParse5 && land1 != null && land2 != null)
            {
                LandHeightAssigner landHeight = new LandHeightAssigner();
                landHeight.RiverAlgorithm(land1, land2, minHeight, maxHeight, minSize, maxSize, curve);
            }
        }



    }
}
