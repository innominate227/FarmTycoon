using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    /// <summary>
    /// Editor that just allows you to select items on the screen, and opens the correct window for the item
    /// </summary>
    public class SelectionEditor : Editor
    {

        /// <summary>
        /// Create a new selection editor.
        /// </summary>
        public SelectionEditor() : base() { }

        /// <summary>
        /// Start editing
        /// </summary>
        protected override void StartEditingInner()
        {
            Program.UserInterface.Graphics.Events.MouseDown += new MouseEventHandler(Graphics_MouseDown);
        }
        
        /// <summary>
        /// Stop editing
        /// </summary>
        protected override void StopEditingInner()
        {
            Program.UserInterface.Graphics.Events.MouseDown -= new MouseEventHandler(Graphics_MouseDown);
            
        }


        /// <summary>
        /// User lowered mouse button
        /// </summary>
        private void Graphics_MouseDown(ClickInfo clickInfo)
        {
            if (clickInfo.Button != MouseButton.Left && clickInfo.Button != MouseButton.Right) { return; }
            if (clickInfo.TopMostTile != null)
            {                
                GameObject objectClicked = (clickInfo.TopMostTile.Tag as GameTile).GameObject;

                //we clicked an object but we may mean to actually select something else, for instance in clicking a crop we mean to select the field
                GameObject objectSelected = null;
                if (objectClicked.PlacementState != PlacementState.BeingPlaced)
                {

                    if (objectClicked is Crop)
                    {
                        objectSelected = ((Crop)objectClicked).Field;
                    }
                    else if (objectClicked is Fence && (objectClicked as Fence).EnclosuresBordered.Count > 0)
                    {
                        objectSelected = ((Fence)objectClicked).EnclosuresBordered[0];
                    }
                    else if (objectClicked is Land && objectClicked.LocationOn.Contains<Field>())
                    {
                        objectSelected = ((Land)objectClicked).LocationOn.Find<Field>();
                    }
                    else if (objectClicked is Land && objectClicked.LocationOn.Contains<Pasture>())
                    {
                        objectSelected = objectClicked.LocationOn.Find<Pasture>();
                    }
                    else
                    {
                        objectSelected = objectClicked;
                    }
                }

                Point pointClicked = new Point(clickInfo.X, clickInfo.Y);

                if (objectSelected != null)
                {
                    if (objectSelected is Worker)
                    {
                        new WorkerPieMenuWindow((Worker)objectSelected, pointClicked);
                    }
                    else if (objectSelected is DeliveryArea)
                    {
                        new StorePieMenuWindow(pointClicked);
                    }
                    else if (objectSelected is ProductionBuilding)
                    {
                        new ProductionBuildingPieMenuWindow((ProductionBuilding)objectSelected, pointClicked);
                    }
                    else if (objectSelected is StorageBuilding)
                    {
                        new StorageBuildingPieMenuWindow((StorageBuilding)objectSelected, pointClicked);
                    }
                    else if (objectSelected is Field)
                    {
                        new FieldPieMenuWindow((Field)objectSelected, pointClicked);
                    }
                    else if (objectSelected is Pasture)
                    {
                        new PasturePieMenuWindow((Pasture)objectSelected, pointClicked);
                    }
                    else if (objectSelected is Trough)
                    {
                        new TroughPieMenuWindow((Trough)objectSelected, pointClicked);
                    }
                    else if (objectSelected is BreakHouse)
                    {
                        new BreakHousePieMenuWindow((BreakHouse)objectSelected, pointClicked);
                    }
                }
            }

            
        }

        


    }
}
