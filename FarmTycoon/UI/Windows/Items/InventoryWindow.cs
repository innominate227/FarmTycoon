using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class InventoryWindow : TycoonWindow
    {
        /// <summary>
        /// The window will use the name of a gameobject.  
        /// If the objects name changes we need to update the windows title
        /// </summary>
        private GameObject _objToUseNameOf;

        /// <summary>
        /// invnetory were showing
        /// </summary>
        private Inventory _inventory;
        


        /// <summary>
        /// Create a new invetory window for the inventory passed.
        /// Specify if the user should be given the option to see Free\Reserved\All of the inventory
        /// </summary>
        public InventoryWindow(Inventory inventory, bool showReserved)
        {
            InitializeComponent();

            _inventory = inventory;
            
            //hide space progress bar if infinate space
            if (inventory.InventoryInfo.Capacity == int.MaxValue)
            {
                SpaceProgress.Visible = false;
                this.Height -= 20;
            }
            

            //set up inventory panel
            if (showReserved)
            {
                itemsPanel.SetColumns("Count", "Reserved", "Size");
            }
            else
            {
                itemsPanel.SetColumns("Count", "Size");
            }
            itemsPanel.AllowSelection = false;
            itemsPanel.Inventory = _inventory;
            
            
            //listen to changes to reserved items/space and refresh as approprate
            _inventory.ReservedSpaceChanged += new Action(Refresh);
            _inventory.ReservedItemsChanged += new Action(Refresh);
            _inventory.UnderlyingList.ListChanged += new Action(Refresh);

            Refresh();
            
            //delete window on close
            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                itemsPanel.Delete();

                _inventory.ReservedSpaceChanged += new Action(Refresh);
                _inventory.ReservedItemsChanged += new Action(Refresh);
                _inventory.UnderlyingList.ListChanged += new Action(Refresh);

                if (_objToUseNameOf != null)
                {
                    _objToUseNameOf.NameChanged -= new Action(RefreshWindowName);
                }
                Program.UserInterface.WindowManager.RemoveWindow(this);
            });

            //add thw window
            Program.UserInterface.WindowManager.AddWindow(this);
        }



        private void Refresh()
        {
            //refresh space progress
            int realCapacitry = _inventory.Capacity - _inventory.ExtraCapacity;
            int usedSpace = realCapacitry - _inventory.FreeSpace;
            SpaceProgress.MaxValue = realCapacitry;
            SpaceProgress.Progress = usedSpace;
            SpaceProgress.Text = usedSpace.ToString() + " / " + realCapacitry.ToString();
        }
        

        public void SetWindowName(GameObject objToUseNameOf)
        {
            _objToUseNameOf = objToUseNameOf;
            _objToUseNameOf.NameChanged += new Action(RefreshWindowName);
            RefreshWindowName();
        }
        
        private void RefreshWindowName()
        {
            this.TitleText = _objToUseNameOf.Name + " Inventory";
        }


    }
}

