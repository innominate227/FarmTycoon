using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{

    /// <summary>
    /// Used to edit the height of land
    /// </summary>
    public class LandEditor : Editor
    {        
        /// <summary>
        /// List of land tiles that are selected
        /// </summary>
        private List<Land> _selectedLand = new List<Land>();        

        /// <summary>
        /// The y location of the mouse when the drag started
        /// </summary>
        private int _lastMouseY = 0;

        /// <summary>
        /// The corener of the land being dragged (only relvant for 1x1 draggin)
        /// </summary>
        private LandCorner _raisingCorner;

        /// <summary>
        /// Is the land currently being raised (or lowered)
        /// </summary>
        private bool _raising = false;

        /// <summary>
        /// The size of square of land being raised;
        /// </summary>
        private int _size = 1;

        /// <summary>
        /// Should land that is raised or lowered be smoothed
        /// </summary>
        private bool _smoothed = true;

        /// <summary>
        /// Total spent so far adjusting land
        /// </summary>
        private int _totalSpent = 0;

        /// <summary>
        /// Window showing the cost of the edits
        /// </summary>
        private CostWindow _costWindow = null;

        
        /// <summary>
        /// Create a new land editor for editing the height of land.
        /// Pass the game to edit.
        /// </summary>
        public LandEditor() : base() { }
        

        /// <summary>
        /// Start editing land
        /// </summary>
        protected override void StartEditingInner()
        {
            Program.UserInterface.Graphics.Events.MouseDown += new MouseEventHandler(Graphics_MouseDown);
            Program.UserInterface.Graphics.Events.MouseMoved += new MouseEventHandler(Graphics_MouseMoved);
            Program.UserInterface.Graphics.Events.MouseUp += new MouseEventHandler(Graphics_MouseUp);
            Program.UserInterface.Graphics.Events.KeyDown += new KeyboardEventHandler(Graphics_KeyDown);
        }
        
        /// <summary>
        /// Stop editing land
        /// </summary>
        protected override void StopEditingInner()
        {
            //close cost window
            if (_costWindow != null)
            {
                _costWindow.CloseWindow();
                _costWindow = null;
            }

            //clear all curent selection
            foreach (Land oldLand in _selectedLand)
            {
                oldLand.CornerToHighlight = LandCorner.None;
            }
            _selectedLand.Clear();


            Program.UserInterface.Graphics.Events.MouseDown -= new MouseEventHandler(Graphics_MouseDown);
            Program.UserInterface.Graphics.Events.MouseMoved -= new MouseEventHandler(Graphics_MouseMoved);
            Program.UserInterface.Graphics.Events.MouseUp -= new MouseEventHandler(Graphics_MouseUp);
            Program.UserInterface.Graphics.Events.KeyDown -= new KeyboardEventHandler(Graphics_KeyDown);
        }

        /// <summary>
        /// The size of the land being edited
        /// </summary>
        public int Size
        {
            get{ return _size; }
            set { _size = value; }
        }

        /// <summary>
        /// Should the land be smoothed
        /// </summary>
        public bool Smoothed
        {
            get { return _smoothed; }
            set { _smoothed = value; }
        }
        
        private void Graphics_KeyDown(Key key)
        {
            if (key == Key.Escape)
            {
                this.StopEditing();
            }
        }

        /// <summary>
        /// User raised mouse button
        /// </summary>
        private void Graphics_MouseUp(ClickInfo clickInfo)
        {
            if (clickInfo.Button != MouseButton.Left && clickInfo.Button != MouseButton.Right) { return; }
            //stop raiseing the land
            _raising = false;

            //close cost window
            if (_costWindow != null)
            {
                _costWindow.CloseWindow();
                _costWindow = null;
            }
        }

        /// <summary>
        /// User lowered mouse button
        /// </summary>
        private void Graphics_MouseDown(ClickInfo clickInfo)
        {
            if (clickInfo.Button != MouseButton.Left && clickInfo.Button != MouseButton.Right) { return; }
            _raising = true;
            _lastMouseY = clickInfo.Y;
            _totalSpent = 0;
        }
        
        /// <summary>
        /// User moved mouse
        /// </summary>
        private void Graphics_MouseMoved(ClickInfo clickInfo)
        {
            //cost to show int he cost window
            int costToShow = 0;

            //how much each adjustment will cost
            int costPerAdjustment = GameState.Current.Prices.GetPrice(PriceType.LandRaise);

            //check if were in the process of raising land
            if (_raising)
            {
                //if the user has moved the mouse up 8 picels since the last rise
                if (clickInfo.Y < _lastMouseY - 8)
                {
                    LandHeightAdjuster heightAdjuster = new LandHeightAdjuster();
                    int numberOfTilesChanged = heightAdjuster.RaiseLand(_selectedLand, _smoothed, _raisingCorner);                    
                    _lastMouseY = clickInfo.Y;

                    int totalCostForThisAdjustment = costPerAdjustment * numberOfTilesChanged;
                    _totalSpent += totalCostForThisAdjustment;
                    GameState.Current.Treasury.Buy(Treasury.CONSTRUCTION_CATAGORY, "Landscaping", totalCostForThisAdjustment);
                }
                //if the user has moved the mouse down 8 picels since the last rise
                else if (clickInfo.Y > _lastMouseY + 8)
                {
                    LandHeightAdjuster heightAdjuster = new LandHeightAdjuster();
                    int numberOfTilesChanged = heightAdjuster.LowerLand(_selectedLand, _smoothed, _raisingCorner);
                    _lastMouseY = clickInfo.Y;

                    int totalCostForThisAdjustment = costPerAdjustment * numberOfTilesChanged;
                    _totalSpent += totalCostForThisAdjustment;
                    GameState.Current.Treasury.Buy(Treasury.CONSTRUCTION_CATAGORY, "Landscaping", totalCostForThisAdjustment);
                }
                
                //show the total amount spent so far
                costToShow = _totalSpent;
            }
            else
            {
                //select the land that the mouse is over
                SelectLand(clickInfo);
                
                //show how much it will be to make one raise/lower
                costToShow = _selectedLand.Count * costPerAdjustment;
            }
            
            if (_costWindow == null)
            {
                _costWindow = new CostWindow(GameState.Current.Treasury);                                
            }
            _costWindow.Cost = costToShow;

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



            //make sure the mouse if over land            
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

                if (_size == 1)
                {
                    land.CornerToHighlight = corenerOfLand;
                    _selectedLand.Add(land);
                    _raisingCorner = corenerOfLand;
                                        
                    #region Adjust Corner For Rotation
                    //Need to adjust the corner to change based on rotation                        
                    if (Program.UserInterface.Graphics.ViewRotation == ViewDirection.East)
                    {
                        if (corenerOfLand == LandCorner.North)
                        {
                            _raisingCorner = LandCorner.East;
                        }
                        else if (corenerOfLand == LandCorner.East)
                        {
                            _raisingCorner = LandCorner.South;
                        }
                        else if (corenerOfLand == LandCorner.South)
                        {
                            _raisingCorner = LandCorner.West;
                        }
                        else if (corenerOfLand == LandCorner.West)
                        {
                            _raisingCorner = LandCorner.North;
                        }
                    }
                    else if (Program.UserInterface.Graphics.ViewRotation == ViewDirection.South)
                    {
                        if (corenerOfLand == LandCorner.North)
                        {
                            _raisingCorner = LandCorner.South;
                        }
                        else if (corenerOfLand == LandCorner.East)
                        {
                            _raisingCorner = LandCorner.West;
                        }
                        else if (corenerOfLand == LandCorner.South)
                        {
                            _raisingCorner = LandCorner.North;
                        }
                        else if (corenerOfLand == LandCorner.West)
                        {
                            _raisingCorner = LandCorner.East;
                        }
                    }
                    if (Program.UserInterface.Graphics.ViewRotation == ViewDirection.West)
                    {
                        if (corenerOfLand == LandCorner.North)
                        {
                            _raisingCorner = LandCorner.West;
                        }
                        else if (corenerOfLand == LandCorner.East)
                        {
                            _raisingCorner = LandCorner.North;
                        }
                        else if (corenerOfLand == LandCorner.South)
                        {
                            _raisingCorner = LandCorner.East;
                        }
                        else if (corenerOfLand == LandCorner.West)
                        {
                            _raisingCorner = LandCorner.South;
                        }
                    }
                    #endregion
                }
                else
                {
                    _selectedLand = CollectLand(land, corenerOfLand, _size);
                    foreach (Land selectedLand in _selectedLand)
                    {
                        selectedLand.CornerToHighlight = LandCorner.Center;
                    }
                }
            }
        }
         
       
               
        /// <summary>
        /// Collect several peices of land surrounding a center peice of land.
        /// </summary>
        private List<Land> CollectLand(Land centerLand, LandCorner preferedDirection, int size)
        {
            Location centerLocation = centerLand.LocationOn;
            CardinalDirection dir = CardinalDirection.North;
            if (preferedDirection == LandCorner.East) { dir = CardinalDirection.East; }
            else if (preferedDirection == LandCorner.West) { dir = CardinalDirection.West; }
            else if (preferedDirection == LandCorner.South) { dir = CardinalDirection.South; }

            List<Location> collectedLocations = LocationUtils.CollectLocations(centerLocation, dir, size);
            return LocationUtils.FindAll<Land>(collectedLocations);
        }

        ///// <summary>
        ///// Collect several peices of land surrounding a center peice of land.
        ///// </summary>
        //private void CollectLand(Land centerLand, LandCorner preferedDirection, List<Land> collectedLand, int size, Dictionary<Land, int> examined)
        //{
        //    //dont try and examine the same peice of land twice
        //    if (examined.ContainsKey(centerLand) && examined[centerLand] >= size)
        //    {
        //        return;
        //    }

        //    //rember that we examined this tile and remeber how far
        //    if (examined.ContainsKey(centerLand))
        //    {
        //        examined[centerLand] = size;
        //    }
        //    else
        //    {
        //        examined.Add(centerLand, size);
        //    }

        //    //collect this tile
        //    if (collectedLand.Contains(centerLand) == false)
        //    {
        //        collectedLand.Add(centerLand);
        //    }

        //    if (size == 1)
        //    {
        //        //if size if one there is nothing else to collect
        //        return;
        //    }
        //    else if (size == 2)
        //    {
        //        if (preferedDirection == LandCorner.East || preferedDirection == LandCorner.Center)
        //        {                    
        //            CollectLand(centerLand.NorthEast, preferedDirection, collectedLand, size - 1, examined);                    
        //            CollectLand(centerLand.SouthEast, preferedDirection, collectedLand, size - 1, examined);
        //            CollectLand(centerLand.NorthEast.SouthEast, preferedDirection, collectedLand, size - 1, examined);                    
        //        }
        //        else if (preferedDirection == LandCorner.West)
        //        {                    
        //            CollectLand(centerLand.NorthWest, preferedDirection, collectedLand, size - 1, examined);                    
        //            CollectLand(centerLand.SouthWest, preferedDirection, collectedLand, size - 1, examined);                    
        //            CollectLand(centerLand.NorthWest.SouthWest, preferedDirection, collectedLand, size - 1, examined);                    
        //        }
        //        else if (preferedDirection == LandCorner.North)
        //        {                    
        //            CollectLand(centerLand.NorthWest, preferedDirection, collectedLand, size - 1, examined);                    
        //            CollectLand(centerLand.NorthEast, preferedDirection, collectedLand, size - 1, examined);                    
        //            CollectLand(centerLand.NorthWest.NorthEast, preferedDirection, collectedLand, size - 1, examined);                    
        //        }
        //        else if (preferedDirection == LandCorner.South)
        //        {                    
        //            CollectLand(centerLand.SouthWest, preferedDirection, collectedLand, size - 1, examined);                    
        //            CollectLand(centerLand.SouthEast, preferedDirection, collectedLand, size - 1, examined);                    
        //            CollectLand(centerLand.SouthWest.SouthEast, preferedDirection, collectedLand, size - 1, examined);                    
        //        }
        //    }
        //    else //size 3 or greater
        //    {                
        //        CollectLand(centerLand.NorthEast, preferedDirection, collectedLand, size - 2, examined);                
        //        CollectLand(centerLand.SouthEast, preferedDirection, collectedLand, size - 2, examined);                
        //        CollectLand(centerLand.NorthWest, preferedDirection, collectedLand, size - 2, examined);                
        //        CollectLand(centerLand.SouthWest, preferedDirection, collectedLand, size - 2, examined);                
        //        CollectLand(centerLand.NorthEast.SouthEast, preferedDirection, collectedLand, size - 2, examined);
        //        CollectLand(centerLand.SouthEast.SouthWest, preferedDirection, collectedLand, size - 2, examined);
        //        CollectLand(centerLand.NorthWest.SouthWest, preferedDirection, collectedLand, size - 2, examined);
        //        CollectLand(centerLand.NorthEast.NorthWest, preferedDirection, collectedLand, size - 2, examined);                
        //    }
        //}
    }
}
