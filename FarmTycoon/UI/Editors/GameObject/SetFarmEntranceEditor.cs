using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{

    /// <summary>
    /// Used to select land that will be the entrance to the farm
    /// </summary>
    public class SetFarmEntranceEditor : Editor
    {       

        public SetFarmEntranceEditor() : base() { }
        
        /// <summary>
        /// Start editor
        /// </summary>
        protected override void StartEditingInner()
        {
            Program.UserInterface.Graphics.Events.MouseDown += new MouseEventHandler(Graphics_MouseDown);
        }

        /// <summary>
        /// Stop farm land editor
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
            Land landClicked = clickInfo.GetLandClicked();
            if (landClicked != null && landClicked.Owned)
            {
                //start batch for path change and tile updates
                Tile.StartChangeSet();
                Program.Game.PathFinder.StartBatchInvalidate();

                //make the land that was the entry not the entry
                foreach (Land land in GameState.Current.MasterObjectList.FindAll<Land>())
                {
                    if (land.Entry)
                    {
                        land.Entry = false;

                        //refersh tile and surroundings
                        land.UpdateTiles();
                        foreach (OrdinalDirection direction in DirectionUtils.AllOrdinalDirections)
                        {
                            land.GetAdjacent(direction).UpdateTiles();                            
                        }
                    }
                }

                //make the new land the entry
                landClicked.Entry = true;

                //refersh tile and surroundings
                landClicked.UpdateTiles();
                foreach (OrdinalDirection direction in DirectionUtils.AllOrdinalDirections)
                {
                    landClicked.GetAdjacent(direction).UpdateTiles();
                }
                
                //end batch
                Tile.EndChangeSet();
                Program.Game.PathFinder.EndBatchInvalidate();
            }
        }

    }
}
