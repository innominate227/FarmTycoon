using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class BreakHouseWindow : TycoonWindow
    {
        /// <summary>
        /// Breakhouse we are showing the workers inside for
        /// </summary>
        private BreakHouse _breakHouse;
        

        /// <summary>
        /// Create a window to show workers inside the breakhouse
        /// </summary>
        public BreakHouseWindow(BreakHouse breakHouse)
        {
            InitializeComponent();

            _breakHouse = breakHouse;
            

            //set up wokers panel
            WorkersPanel.SetColumns("Status", "Energy");
            WorkersPanel.AllowSelection = false;
            WorkersPanel.Building = breakHouse;


            _breakHouse.WorkersInside.Changed += new Action(Refresh);
            _breakHouse.NameChanged += new Action(RefreshWindowName);

            Refresh();
            RefreshWindowName();

            //delete window on close
            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                WorkersPanel.Delete();

                _breakHouse.WorkersInside.Changed -= new Action(Refresh);
                _breakHouse.NameChanged -= new Action(RefreshWindowName);

                Program.UserInterface.WindowManager.RemoveWindow(this);
            });

            //add thw window
            Program.UserInterface.WindowManager.AddWindow(this);
        }



        private void Refresh()
        {
            //refresh space progress
            int capacitry = _breakHouse.BreakHouseInfo.Capacity;
            int usedSpace = _breakHouse.WorkersInside.WorkersWithSpotReserved.Count;
            SpaceProgress.MaxValue = capacitry;
            SpaceProgress.Progress = usedSpace;
            SpaceProgress.Text = usedSpace.ToString() + " / " + capacitry.ToString();
        }
                        
        private void RefreshWindowName()
        {
            this.TitleText = _breakHouse.Name;
        }


    }
}

