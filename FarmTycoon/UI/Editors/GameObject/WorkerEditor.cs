using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    /// <summary>
    /// Create workers by listening to the mouse
    /// </summary>
    public class WorkerEditor : Editor
    {
        /// <summary>
        /// The worker in progress of being palced
        /// </summary>
        private Worker _inProgress = null;

        /// <summary>
        /// Window showing the cost of whats being placed
        /// </summary>
        private CostWindow _costWindow = null;
        
        /// <summary>
        /// Create a new Worker Editor for placing worker
        /// </summary>
        public WorkerEditor() : base() { }
        

        /// <summary>
        /// Start editing workers
        /// </summary>
        protected override void StartEditingInner()
        {
            Program.UserInterface.Graphics.Events.KeyDown += new KeyboardEventHandler(Graphics_KeyDown);
            Program.UserInterface.Graphics.Events.MouseDown += new MouseEventHandler(Graphics_MouseDown);
            Program.UserInterface.Graphics.Events.MouseMoved += new MouseEventHandler(Graphics_MouseMove);
        }

        private void Graphics_KeyDown(Key key)
        {
            if (key == Key.Escape)
            {
                this.StopEditing();
            }
        }

        
        /// <summary>
        /// Stop editing land
        /// </summary>
        protected override void StopEditingInner()
        {
            if (_costWindow != null)
            {
                _costWindow.CloseWindow();
                _costWindow = null;
            }
            if (_inProgress != null)
            {
                _inProgress.Delete();
                _inProgress = null;
            }

            Program.UserInterface.Graphics.Events.KeyDown -= new KeyboardEventHandler(Graphics_KeyDown);
            Program.UserInterface.Graphics.Events.MouseDown -= new MouseEventHandler(Graphics_MouseDown);
            Program.UserInterface.Graphics.Events.MouseMoved -= new MouseEventHandler(Graphics_MouseMove);
        }
        

        /// <summary>
        /// User lowered mouse button
        /// </summary>
        private void Graphics_MouseDown(ClickInfo clickInfo)
        {
            if (clickInfo.Button != MouseButton.Left && clickInfo.Button != MouseButton.Right) { return; } 
            Land landOn = clickInfo.GetLandClicked();
            if (landOn != null && _inProgress != null)
            {
                if (_costWindow != null)
                {
                    Program.UserInterface.WindowManager.RemoveWindow(_costWindow);
                    _costWindow = null;
                }

                //buy the worker
                int workerHireCost = GameState.Current.Prices.GetPrice(PriceType.Worker);
                GameState.Current.Treasury.Buy(Treasury.CONSTRUCTION_CATAGORY, "New Hire", workerHireCost);

                //name the worker based on how many other workers there are
                _inProgress.Name = "Worker" + GameState.Current.MasterObjectList.FindAll<Worker>().Count.ToString();                    

                //start the worker
                _inProgress.DoneWithPlacement();
                _inProgress = null;

                ////hack to build workers faster
                //for (int i = 0; i < 49; i++)
                //{
                //    _inProgress = new Worker();
                //    _inProgress.Setup(landOn.LocationOn);
                //    _inProgress.Name = "Worker" + GameState.Current.MasterObjectList.FindAll<Worker>().Count.ToString();
                //    _inProgress.DoneWithPlacement();
                //    _inProgress = null;
                //}
                

                //this.StopEditing();
            }
        }
            
        
        /// <summary>
        /// User moved mouse button
        /// </summary>
        private void Graphics_MouseMove(ClickInfo clickInfo)
        {
            Land landClicked = clickInfo.GetLandClicked();
            if (landClicked != null)
            {
                if (_inProgress != null)
                {
                    _inProgress.Delete();
                    _inProgress = null;
                }

                if (_costWindow == null)
                {
                    _costWindow = new CostWindow(GameState.Current.Treasury);
                    int workerHireCost = GameState.Current.Prices.GetPrice(PriceType.Worker);
                    _costWindow.Cost = workerHireCost;
                }

                if (WorkerEditor.CanWalkOn(landClicked.LocationOn))
                {
                    _inProgress = new Worker();
                    _inProgress.Setup(landClicked.LocationOn);
                    _costWindow.Visible = true;
                }
                else
                {
                    _costWindow.Visible = false;
                }
                
            }
        }


        /// <summary>
        /// Return if a worker will be able to walk on the location passed,        
        /// </summary>
        public static bool CanWalkOn(Location location)
        {
            //workers cant walk on buildings, troughs, etc
            if (location.Contains<StorageBuilding>()) { return false; }
            if (location.Contains<ProductionBuilding>()) { return false; }
            if (location.Contains<Scenery>()) { return false; }
            if (location.Contains<Trough>()) { return false; }



            //looks like we can walk here
            return true;
        }


    }
}
