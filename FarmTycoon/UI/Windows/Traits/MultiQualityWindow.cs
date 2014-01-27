using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class MultiQualityWindow : TycoonWindow
    {
        private IHasQuality _obj;

        public MultiQualityWindow(IHasQuality obj)
        {
            InitializeComponent();
            qualityCaption.Visible = false;
            qualityGauge.Visible = false;
            showTraitsButton.Depressed = true;
            showSubQualitiesButton.Depressed = false;
            traitsPanel.Visible = true;
            subQualitiesPanel.Visible = false;


            _obj = obj;

            if (_obj is Pasture)
            {
                cropPlantedTitle.Text = "Animal:";
            }

            RefreshName();
            //m_obj.NameChanged +=new Action(RefreshName);
            
            RefreshStats();
            


            showSubQualitiesButton.Clicked += new Action<TycoonControl>(delegate
            {
                showTraitsButton.Depressed = false;
                showSubQualitiesButton.Depressed = true;
                traitsPanel.Visible = false;
                subQualitiesPanel.Visible = true;
                RefreshStats();
            });
            showTraitsButton.Clicked += new Action<TycoonControl>(delegate
            {
                showTraitsButton.Depressed = true;
                showSubQualitiesButton.Depressed = false;
                traitsPanel.Visible = true;
                subQualitiesPanel.Visible = false;
                RefreshStats();
            });


            Program.GameThread.RefreshTimePassed += new Action(RefreshStats);

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                //m_obj.NameChanged -= new Action(RefreshName);
                Program.GameThread.RefreshTimePassed -= new Action(RefreshStats);
                Program.UserInterface.WindowManager.RemoveWindow(this);                
            });

            Program.UserInterface.WindowManager.AddWindow(this);
        }


        private void RefreshName()
        {
            this.TitleText = _obj.Name + " Status";
        }


        
        private void RefreshStats()
        {
            //refresh the some stuff based on what we are showing quality of
            if (_obj is Field)
            {
                RefreshField();
            }
            else if (_obj is Pasture)
            {
                RefreshPasture();
            }

            //refresh overall quality
            if (_obj.Quality != null)
            {
                qualityGauge.Value = _obj.Quality.CurrentQuality;
                qualityGauge.Quality = _obj.Quality.CurrentQuality;

                if (qualityCaption.Visible == false)
                {
                    qualityCaption.Visible = true;
                    qualityGauge.Visible = true;

                    traitsPanel.SetTraits(_obj.Quality);
                }
            }
            else
            {
                if (qualityCaption.Visible == true)
                {
                    qualityCaption.Visible = false;
                    qualityGauge.Visible = false;

                    traitsPanel.SetTraits(null);
                }                
            }

            //what height should the window be
            int height = 67;
            int childrenCount = 0;
            if (qualityCaption.Visible) { height += 30; }

            //only refresh the panel that visisble
            if (subQualitiesPanel.Visible)
            {
                //refresh subqualites
                subQualitiesPanel.Refresh();
                childrenCount = subQualitiesPanel.Children.Count;
                height += (Math.Min(childrenCount, 10) * 30);
            }
            else
            {
                //refresh traits 
                traitsPanel.Refresh();
                childrenCount = traitsPanel.Children.Count;
                height += (Math.Min(childrenCount, 10) * 30);
            }

            if (this.Height != height)
            {
                this.Height = height;
                if (childrenCount > 10)
                {
                    qualityGauge.Width = this.Width - 82 - 15;                    
                }
                else
                {
                    qualityGauge.Width = this.Width - 82;
                }
            }
            
        }


        //if there are more objects on the next refresh we need to re point to the quality list too
        private int _subQualitiesLastTime = 0;
        
        private void RefreshField()
        {
            Field field = (Field)_obj;

            fieldSizeLabel.Text = _obj.AllLocationsOn.Count.ToString();            
            cropPlantedLabel.Text = field.TypePlanted;

            //if the number of planted objects changed then re assign the quality to show
            if (field.Crops.Count != _subQualitiesLastTime)
            {
                _subQualitiesLastTime = field.Crops.Count;                   
                                
                //sub qualities
                int cropNum = 1;
                Dictionary<string, IQuality> subQualities = new Dictionary<string, IQuality>();
                foreach(Crop crop in field.Crops)
                {
                    subQualities.Add(field.TypePlanted + " " + cropNum.ToString(), crop.Quality);
                    cropNum++;
                }
                
                subQualitiesPanel.SetSubQualities(subQualities);
            }
        }
        
        private void RefreshPasture()
        {
            Pasture pasture = (Pasture)_obj;

            fieldSizeLabel.Text = _obj.AllLocationsOn.Count.ToString();
            cropPlantedLabel.Text = pasture.AnimalType;

            //if the number of planted objects changed then re assign the quality to show
            if (pasture.Animals.Count != _subQualitiesLastTime)
            {
                _subQualitiesLastTime = pasture.Animals.Count;

                //sub qualities
                int animalNum = 1;
                Dictionary<string, IQuality> subQualities = new Dictionary<string, IQuality>();
                foreach (Animal animal in pasture.Animals)
                {
                    subQualities.Add(pasture.AnimalType + " " + animalNum.ToString(), animal.Quality);
                    animalNum++;
                }

                subQualitiesPanel.SetSubQualities(subQualities);
            }
        }



    }
}
