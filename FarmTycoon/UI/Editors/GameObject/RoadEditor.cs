using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    /// <summary>
    /// Create roads, or highways by listening to the mouse
    /// </summary>
    public class RoadEditor : Editor
    {
        /// <summary>
        /// Window showing how much it will cost to bild the roads in progress
        /// </summary>
        private CostWindow _costWindow;

        /// <summary>
        /// Build highway instead of road
        /// </summary>
        private bool _buildHighway = false;

        /// <summary>
        /// Roads in progress of being built
        /// </summary>
        private List<GameObject> _roadsInProgress = new List<GameObject>();

        /// <summary>
        /// The tile of land the road being created starts on
        /// </summary>
        private Land _draggingStart;

        /// <summary>
        /// The tile of land the road being created ends on
        /// </summary>
        private Land _draggingEnd;

        /// <summary>
        /// is the user draggin out a lenght of road
        /// </summary>
        private bool _draggingRoad = false;

        /// <summary>
        /// is shift down
        /// </summary>
        private bool _shiftDown = false;
        
        
        /// <summary>
        /// Create a new road editor for adding roads
        /// </summary>
        public RoadEditor(bool buildHighway)
            : base() 
        {
            _buildHighway = buildHighway;
        }
        

        /// <summary>
        /// Start editing
        /// </summary>
        protected override void StartEditingInner()
        {
            Program.UserInterface.Graphics.Events.MouseDown += new MouseEventHandler(Graphics_MouseDown);
            Program.UserInterface.Graphics.Events.MouseMoved += new MouseEventHandler(Graphics_MouseMoved);
            Program.UserInterface.Graphics.Events.MouseUp += new MouseEventHandler(Graphics_MouseUp);
            Program.UserInterface.Graphics.Events.KeyDown += new KeyboardEventHandler(Graphics_KeyDown);
            Program.UserInterface.Graphics.Events.KeyUp += new KeyboardEventHandler(Graphics_KeyUp);
        }

        private void Graphics_KeyUp(Key key)
        {
            if (key == Key.ShiftLeft || key == Key.ShiftRight)
            {
                _shiftDown = false;
                RefreshRoadInProgress();
            }
        }

        private void Graphics_KeyDown(Key key)
        {
            if (key == Key.Escape)
            {
                this.StopEditing();
            }

            if (key == Key.ShiftLeft || key == Key.ShiftRight)
            {
                _shiftDown = true;
                RefreshRoadInProgress();
            }
        }
        
        /// <summary>
        /// Stop editing land
        /// </summary>
        protected override void StopEditingInner()
        {
            //delete roads in progress
            foreach (GameObject oldRoad in _roadsInProgress)
            {
                oldRoad.Delete();
            }
            _roadsInProgress.Clear();

            //remove cost window
            if (_costWindow!=null)
            {
                Program.UserInterface.WindowManager.RemoveWindow(_costWindow);
                _costWindow = null;
            }

            Program.UserInterface.Graphics.Events.MouseDown -= new MouseEventHandler(Graphics_MouseDown);
            Program.UserInterface.Graphics.Events.MouseMoved -= new MouseEventHandler(Graphics_MouseMoved);
            Program.UserInterface.Graphics.Events.MouseUp -= new MouseEventHandler(Graphics_MouseUp);
            Program.UserInterface.Graphics.Events.KeyDown -= new KeyboardEventHandler(Graphics_KeyDown);
            Program.UserInterface.Graphics.Events.KeyUp -= new KeyboardEventHandler(Graphics_KeyUp);            
        }
        
        /// <summary>
        /// User raised mouse button
        /// </summary>
        private void Graphics_MouseUp(ClickInfo clickInfo)
        {
            if (clickInfo.Button != MouseButton.Left && clickInfo.Button != MouseButton.Right) { return; }
            //stop draggin the road
            _draggingRoad = false;

            //treat right button same as shift
            if (clickInfo.Button == MouseButton.Right)
            {
                _shiftDown = false;
            }

            if (clickInfo.TileClicked && _costWindow != null)
            {
                if (_buildHighway)
                {
                    GameState.Current.Treasury.Buy(Treasury.CONSTRUCTION_CATAGORY, "Roads", _costWindow.Cost);
                }
                else
                {
                    GameState.Current.Treasury.Buy(Treasury.CONSTRUCTION_CATAGORY, "Highway", _costWindow.Cost);
                }

                //start batch location invalidation
                //locations will become invalid when roads are done with placement
                Program.Game.PathFinder.StartBatchInvalidate();

                //mark all road in progress as done with placement
                foreach (GameObject road in _roadsInProgress)
                {
                    road.DoneWithPlacement();
                }
                
                //end batch location invalidation
                Program.Game.PathFinder.EndBatchInvalidate();
                
                //roads are no longer in progress of being built
                _roadsInProgress.Clear();
                
                Program.UserInterface.WindowManager.RemoveWindow(_costWindow);
                _costWindow = null;
            }
        }

        /// <summary>
        /// User lowered mouse button
        /// </summary>
        private void Graphics_MouseDown(ClickInfo clickInfo)
        {
            if (clickInfo.Button != MouseButton.Left && clickInfo.Button != MouseButton.Right) { return; }

            //treat right button the same as shift left
            if (clickInfo.Button == MouseButton.Right)
            {                
                _shiftDown = true;
            }

            Land landOn = clickInfo.GetLandClicked();
            if (landOn != null)
            {
                _draggingStart = landOn;
                _draggingEnd = landOn;
                _draggingRoad = true;

                RefreshRoadInProgress();
            }
        }

        /// <summary>
        /// User moved mouse
        /// </summary>
        private void Graphics_MouseMoved(ClickInfo clickInfo)
        {
            Land landOn = clickInfo.GetLandClicked();

            //check if were in the draggin out a road, mouse is over a land tile, and it is a different land tile
            if (_draggingRoad && landOn != null && _draggingEnd != landOn)
            {
                _draggingEnd = landOn;
                RefreshRoadInProgress();             
            }
            else if (landOn != null && _draggingEnd != landOn)
            {
                //we are no in draggin mode, set both start and end
                _draggingStart = landOn;
                _draggingEnd = landOn;
                RefreshRoadInProgress();
            }
        }


        private void RefreshRoadInProgress()
        {
            Tile.StartChangeSet();

            //delete the old roads.  deleting old after creating new prevents flicker cause where the 
            //old road is deleted, and then a couple frames are drawn before the new road is created
            foreach (GameObject oldRoad in _roadsInProgress)
            {
                oldRoad.Delete();
            }
            _roadsInProgress.Clear();

            if (_buildHighway == false)
            {
                //building normal roads
                BuildNormalRoads();
            }
            else
            {
                //building highways
                BuildHighways();

                //update the highways and surrounding roads/highways that were already placed
                foreach (Highway highway in _roadsInProgress)
                {
                    highway.UpdateTiles();
                    highway.UpdateNeighborRoadAndHighwayTiles(true);
                }
            }

            Tile.EndChangeSet();

                        
            //show the cost window
            if (_costWindow == null)
            {
                _costWindow = new CostWindow(GameState.Current.Treasury);
            }
            int perRoadCost = GameState.Current.Prices.GetPrice(PriceType.Road);
            if (_buildHighway)
            {
                perRoadCost = GameState.Current.Prices.GetPrice(PriceType.Highway);
            }

            _costWindow.Cost = _roadsInProgress.Count * perRoadCost;
        }



        /// <summary>
        /// Create normal roads and add to roads in progress
        /// </summary>
        private void BuildNormalRoads()
        {
            //determine the path the road should take         
            RightAnglePathFinder rightAnglePath = new RightAnglePathFinder();
            List<Land> roadPath = rightAnglePath.FindPath(_draggingStart, _draggingEnd, _shiftDown);
            
            //add a road on each tile in the path that is acceptable for a road                
            foreach (Land roadPathLand in roadPath)
            {
                if (LocationAcceptableForRoad(roadPathLand.LocationOn))
                {
                    Road newRoad = new Road();
                    newRoad.Setup(roadPathLand.LocationOn);
                    _roadsInProgress.Add(newRoad);
                }
            }
        }


        /// <summary>
        /// Create highways and add to roads in progress
        /// </summary>
        private void BuildHighways()
        {
            //determine the path the road should take         
            RightAnglePathFinder rightAnglePath = new RightAnglePathFinder();
            Land turnLand; bool straightPath; OrdinalDirection dir1; OrdinalDirection dir2;
            List<Land> roadPath = rightAnglePath.FindPath(_draggingStart, _draggingEnd, _shiftDown, out turnLand, out straightPath, out dir1, out dir2);

            //if it was a striaght path and the turn land is the first land in the path then use dir2
            //(dont want to change the FindPath function because not sure if something else expects it to be that way)
            if (straightPath && turnLand == roadPath[0])
            {
                dir1 = dir2;
            }
            
            //if the path is not straight, we shiould check if it is just 1 land before the turn land then we should just ignore the turn
            if (straightPath == false && roadPath.Count >= 2)
            {
                if (roadPath[1] == turnLand)
                {
                    roadPath.RemoveAt(0);
                    turnLand = null;
                    dir1 = dir2;
                    straightPath = true;
                }
                else if (roadPath[roadPath.Count - 2] == turnLand)
                {
                    roadPath.RemoveAt(roadPath.Count - 1);
                    turnLand = null;                    
                    straightPath = true;
                }
            }



            //is the turn a left turn
            bool leftTurn = DirectionUtils.CounterClockwiseOne(dir1) == dir2;

            //are we after the turn
            bool afterTurn = false;

            //we need to skip a peice of land just after the turn if we are doing a left turn
            bool skipNext = false;

            //add a road on each tile in the path that is acceptable for a road
            foreach (Land roadPathLand in roadPath)
            {
                //skip this one if needed
                if (skipNext)
                {
                    skipNext = false;
                    continue;
                }

                //the highway takes up two locations this is the main location (the other location is determined based on the direction of travel)
                Location mainLoc = roadPathLand.LocationOn;
                OrdinalDirection directionOfTravel = dir1;

                //if after turn we need to update direction of travel and maybe mainLoc
                if (afterTurn)
                {
                    directionOfTravel = dir2;

                    //for a left turn we choose a peice of land clockwise one from the direction of travel so we do not end up with a strange looking corner
                    if (leftTurn == false)
                    {
                        mainLoc = mainLoc.GetAdjacent(DirectionUtils.ClockwiseOne(directionOfTravel));
                    }
                }

                //get the direction opposite to direction of travel (direction for the other lane)
                OrdinalDirection oppositeDirectionOfTravel = DirectionUtils.OppositeDirection(directionOfTravel);

                //get the other location the road will be on(wich is counter clockwise from the direction of travel)
                Location secLoc = mainLoc.GetAdjacent(DirectionUtils.CounterClockwiseOne(directionOfTravel));
                                
                if (LocationAcceptableForRoad(mainLoc) && LocationAcceptableForRoad(secLoc))
                {
                    //both locations are ok, make the highway
                    Highway newRoad = new Highway();
                    newRoad.Setup(mainLoc, directionOfTravel);
                    _roadsInProgress.Add(newRoad);

                    Highway newRoad2 = new Highway();
                    newRoad2.Setup(secLoc, oppositeDirectionOfTravel);
                    _roadsInProgress.Add(newRoad2);
                }
                else if (mainLoc.Contains<Highway>() && secLoc.Contains<Highway>() == false && mainLoc.Find<Highway>().TravelDirection != directionOfTravel && mainLoc.Find<Highway>().TravelDirection != oppositeDirectionOfTravel)
                {
                    //one location contains highway and the other does not, make a highway perpendicular

                    Location specialLoc1 = secLoc;
                    Location specialLoc2 = secLoc.GetAdjacent(directionOfTravel);
                    OrdinalDirection specialDir1 = DirectionUtils.ClockwiseOne(directionOfTravel);
                    OrdinalDirection specialDir2 = DirectionUtils.CounterClockwiseOne(directionOfTravel);

                    Highway newRoad = new Highway();
                    newRoad.Setup(specialLoc1, specialDir1);
                    _roadsInProgress.Add(newRoad);

                    Highway newRoad2 = new Highway();
                    newRoad2.Setup(specialLoc2, specialDir2);
                    _roadsInProgress.Add(newRoad2);

                    //need to skip the next because we made an ortangonal one wich took two spots on our path
                    skipNext = true;
                }
                else if (secLoc.Contains<Highway>() && mainLoc.Contains<Highway>() == false && secLoc.Find<Highway>().TravelDirection != directionOfTravel && secLoc.Find<Highway>().TravelDirection != oppositeDirectionOfTravel)
                {
                    //one location contains highway and the other does not, make a highway perpendicular

                    Location specialLoc1 = mainLoc;
                    Location specialLoc2 = mainLoc.GetAdjacent(directionOfTravel);
                    OrdinalDirection specialDir1 = DirectionUtils.ClockwiseOne(directionOfTravel);
                    OrdinalDirection specialDir2 = DirectionUtils.CounterClockwiseOne(directionOfTravel);

                    Highway newRoad = new Highway();
                    newRoad.Setup(specialLoc1, specialDir1);
                    _roadsInProgress.Add(newRoad);

                    Highway newRoad2 = new Highway();
                    newRoad2.Setup(specialLoc2, specialDir2);
                    _roadsInProgress.Add(newRoad2);

                    //need to skip the next because we made an ortangonal one wich took two spots on our path
                    skipNext = true;
                }


                //if this was the turn land we are now after the turn
                if (straightPath == false && roadPathLand == turnLand)
                {
                    afterTurn = true;

                    //if its a left turn we need to skip the next one
                    if (leftTurn)
                    {
                        skipNext = true;
                    }
                }
            }
        }



        /// <summary>
        /// Determines if a road can be added to the location
        /// </summary>
        private bool LocationAcceptableForRoad(Location location)
        {            
            List<Location> roadLocations = new List<Location>();
            roadLocations.Add(location);            
            bool aboveLandAndEmpty = LocationUtils.IsGoodLocationToBuild(roadLocations, BuildRequirementFlags.None);
            if (aboveLandAndEmpty == false) { return false; }

            //make sure the land type is acceptable
            Land land = location.Find<Land>();            
            string landType = land.HeightCode;
            if (!(landType == "0000" || landType == "0011" || landType == "1100" || landType == "1001" || landType == "0110"))
            {
                return false;
            }

            return true;
        }


    }
}
