using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{

    /// <summary>
    /// Used to delete objects on selected land
    /// </summary>
    public class DeleteEditor : Editor
    {
        /// <summary>
        /// List of land tiles that are selected
        /// </summary>
        private List<Land> _selectedLand = new List<Land>();
        
        /// <summary>
        /// The size of square of land being raised;
        /// </summary>
        private int _size = 1;

        /// <summary>
        /// Create a new delte editor. pass the game to edit
        /// </summary>
        public DeleteEditor() : base() { }
        


        /// <summary>
        /// Start delete editor
        /// </summary>
        protected override void StartEditingInner()
        {
            Program.UserInterface.Graphics.Events.MouseDown += new MouseEventHandler(Graphics_MouseDown);
            Program.UserInterface.Graphics.Events.MouseMoved += new MouseEventHandler(Graphics_MouseMoved);
            Program.UserInterface.Graphics.Events.KeyDown += new KeyboardEventHandler(Graphics_KeyDown);
        }

        /// <summary>
        /// Stop delete editor
        /// </summary>
        protected override void StopEditingInner()
        {
            //clear all curent selection
            foreach (Land oldLand in _selectedLand)
            {
                oldLand.CornerToHighlight = LandCorner.None;
            }
            _selectedLand.Clear();


            Program.UserInterface.Graphics.Events.MouseDown -= new MouseEventHandler(Graphics_MouseDown);
            Program.UserInterface.Graphics.Events.MouseMoved -= new MouseEventHandler(Graphics_MouseMoved);
            Program.UserInterface.Graphics.Events.KeyDown -= new KeyboardEventHandler(Graphics_KeyDown);
        }


        /// <summary>
        /// The size of the land being edited
        /// </summary>
        public int Size
        {
            get { return _size; }
            set
            {
                _size = value;
            }
        }


        private void Graphics_KeyDown(Key key)
        {
            if (key == Key.Escape)
            {
                this.StopEditing();
            }
        }
        
        /// <summary>
        /// User lowered mouse button
        /// </summary>
        private void Graphics_MouseDown(ClickInfo clickInfo)
        {
            if (clickInfo.Button != MouseButton.Left && clickInfo.Button != MouseButton.Right) { return; }
            if (clickInfo.TileClicked)
            {
                //get all the objects on the land
                List<GameObject> allObjectsOnLand = new List<GameObject>();
                foreach (Land landSelected in _selectedLand)
                {
                    foreach(GameObject gameObjectOnLand in landSelected.LocationOn.AllObjects)
                    {
                        if (allObjectsOnLand.Contains(gameObjectOnLand) == false)
                        {
                            allObjectsOnLand.Add(gameObjectOnLand);
                        }
                    }
                }

                //delete each object
                foreach(GameObject gameObj in allObjectsOnLand)
                {
                    //remove all roads, scenery
                    if(gameObj is Road || gameObj is Scenery)
                    {
                        gameObj.Delete();
                    }

                    //ask the user before removing storage buildings, production buildings, planted areas
                    if (gameObj is StorageBuilding || gameObj is ProductionBuilding || gameObj is BreakHouse || gameObj is Trough || gameObj is Field || gameObj is Pasture)
                    {
                        new DeleteWindow(gameObj);
                    }

                    //other types are not deleted
                }                
            }
        }

        /// <summary>
        /// User moved mouse
        /// </summary>
        private void Graphics_MouseMoved(ClickInfo clickInfo)
        {            
            //select the land that the mouse is over
            SelectLand(clickInfo);            
        }

        /// <summary>
        /// Select the land that the mouse if over
        /// </summary>
        private void SelectLand(ClickInfo clickInfo)
        {
            //clear all curent selection
            foreach (Land oldLand in _selectedLand)
            {
                oldLand.CornerToHighlight = LandCorner.None;
            }
            _selectedLand.Clear();
            

            //make sure the mouse is over land            
            TileClickInfo landClickedInfo = clickInfo.GetLandClickedInfo();
            if (landClickedInfo != null)
            {
                Land land = clickInfo.GetLandClicked();
                float tileLocationX = landClickedInfo.ClickLocationX;
                float tileLocationY = landClickedInfo.ClickLocationY;

                //determine what corner of land the mouse is over
                LandCorner corenerOfLand = LandCorner.Center;
                if (tileLocationX < 0.3f)
                {
                    corenerOfLand = LandCorner.West;
                }
                else if (tileLocationX > 0.7f)
                {
                    corenerOfLand = LandCorner.East;
                }
                else if (tileLocationY < 0.3f)
                {
                    corenerOfLand = LandCorner.North;
                }
                else if (tileLocationY > 0.7f)
                {
                    corenerOfLand = LandCorner.South;
                }

                CollectLand(land, corenerOfLand, _selectedLand, _size, new Dictionary<Land, int>());
                foreach (Land selectedLand in _selectedLand)
                {
                    selectedLand.CornerToHighlight = LandCorner.Center;
                }
                
            }
        }


        /// <summary>
        /// Collect several peices of land surrounding a center peice of land.
        /// </summary>
        private void CollectLand(Land centerLand, LandCorner preferedDirection, List<Land> collectedLand, int size, Dictionary<Land, int> examined)
        {
            //dont try and examine the same peice of land twice
            if (examined.ContainsKey(centerLand) && examined[centerLand] >= size)
            {
                return;
            }

            //rember that we examined this tile and remeber how far
            if (examined.ContainsKey(centerLand))
            {
                examined[centerLand] = size;
            }
            else
            {
                examined.Add(centerLand, size);
            }

            //collect this tile
            if (collectedLand.Contains(centerLand) == false)
            {
                collectedLand.Add(centerLand);
            }

            if (size == 1)
            {
                //if size if one there is nothing else to collect
                return;
            }
            else if (size == 2)
            {
                if (preferedDirection == LandCorner.East || preferedDirection == LandCorner.Center)
                {
                    CollectLand(centerLand.NorthEast, preferedDirection, collectedLand, size - 1, examined);
                    CollectLand(centerLand.SouthEast, preferedDirection, collectedLand, size - 1, examined);
                    CollectLand(centerLand.NorthEast.SouthEast, preferedDirection, collectedLand, size - 1, examined);
                }
                else if (preferedDirection == LandCorner.West)
                {
                    CollectLand(centerLand.NorthWest, preferedDirection, collectedLand, size - 1, examined);
                    CollectLand(centerLand.SouthWest, preferedDirection, collectedLand, size - 1, examined);
                    CollectLand(centerLand.NorthWest.SouthWest, preferedDirection, collectedLand, size - 1, examined);
                }
                else if (preferedDirection == LandCorner.North)
                {
                    CollectLand(centerLand.NorthWest, preferedDirection, collectedLand, size - 1, examined);
                    CollectLand(centerLand.NorthEast, preferedDirection, collectedLand, size - 1, examined);
                    CollectLand(centerLand.NorthWest.NorthEast, preferedDirection, collectedLand, size - 1, examined);
                }
                else if (preferedDirection == LandCorner.South)
                {
                    CollectLand(centerLand.SouthWest, preferedDirection, collectedLand, size - 1, examined);
                    CollectLand(centerLand.SouthEast, preferedDirection, collectedLand, size - 1, examined);
                    CollectLand(centerLand.SouthWest.SouthEast, preferedDirection, collectedLand, size - 1, examined);
                }
            }
            else //size 3 or greater
            {
                CollectLand(centerLand.NorthEast, preferedDirection, collectedLand, size - 2, examined);
                CollectLand(centerLand.SouthEast, preferedDirection, collectedLand, size - 2, examined);
                CollectLand(centerLand.NorthWest, preferedDirection, collectedLand, size - 2, examined);
                CollectLand(centerLand.SouthWest, preferedDirection, collectedLand, size - 2, examined);
                CollectLand(centerLand.NorthEast.SouthEast, preferedDirection, collectedLand, size - 2, examined);
                CollectLand(centerLand.SouthEast.SouthWest, preferedDirection, collectedLand, size - 2, examined);
                CollectLand(centerLand.NorthWest.SouthWest, preferedDirection, collectedLand, size - 2, examined);
                CollectLand(centerLand.NorthEast.NorthWest, preferedDirection, collectedLand, size - 2, examined);
            }
        }
    }
}
