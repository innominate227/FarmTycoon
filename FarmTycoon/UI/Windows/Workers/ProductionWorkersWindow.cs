using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class ProductionWorkersWindow : TycoonWindow
    {
        /// <summary>
        /// ProductionBuilding we are showing the workers inside for
        /// </summary>
        private ProductionBuilding _productionBuilding;
        

        /// <summary>
        /// Create a window to show workers inside the ProductionBuilding
        /// </summary>
        public ProductionWorkersWindow(ProductionBuilding productionBuilding)
        {
            InitializeComponent();

            _productionBuilding = productionBuilding;
            

            //set up wokers panel
            WorkersPanel.SetColumns("Status", "Energy");
            WorkersPanel.AllowSelection = false;
            WorkersPanel.Building = productionBuilding;


            _productionBuilding.WorkersInside.Changed += new Action(Refresh);
            _productionBuilding.NameChanged += new Action(RefreshWindowName);

            Refresh();
            RefreshWindowName();

            //delete window on close
            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                WorkersPanel.Delete();

                _productionBuilding.WorkersInside.Changed -= new Action(Refresh);
                _productionBuilding.NameChanged -= new Action(RefreshWindowName);

                Program.UserInterface.WindowManager.RemoveWindow(this);
            });

            //add thw window
            Program.UserInterface.WindowManager.AddWindow(this);
        }



        private void Refresh()
        {
            //refresh space progress
            int capacitry = _productionBuilding.BuildingInfo.MaxWorkers;
            int usedSpace = _productionBuilding.WorkersInside.WorkersInside.Count + _productionBuilding.WorkersInside.WorkersHeadingToward.Count;
            SpaceProgress.MaxValue = capacitry;
            SpaceProgress.Progress = usedSpace;
            SpaceProgress.Text = usedSpace.ToString() + " / " + capacitry.ToString();
        }
                        
        private void RefreshWindowName()
        {
            this.TitleText = _productionBuilding.Name;
        }


    }
}

