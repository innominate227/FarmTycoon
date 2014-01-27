using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Diagnostics;

namespace FarmTycoon
{
    public class EnclosureEditor : Editor
    {
        /// <summary>
        /// The type of game object we are making an enclusre for
        /// </summary>
        private EnclosureType _typeToEnclose = EnclosureType.Field;

        /// <summary>
        /// Window showing how much it will cost to bild the roads in progress
        /// </summary>
        private CostWindow _costWindow;

        /// <summary>
        /// The fences that have been placed so far for the field in progress
        /// </summary>
        private List<Fence> _fencesPlacedForField = new List<Fence>();

        /// <summary>
        /// The fences in progress of being placed
        /// </summary>
        private List<Fence> _fencesInProgress = new List<Fence>();

        /// <summary>
        /// are we curently draggin a lenht of fences
        /// </summary>
        private bool _dragging = false;

        /// <summary>
        /// Peice of land the fence were currently draggin start on
        /// </summary>
        private Land _startTile;

        /// <summary>
        /// Side of the land the fence were currently draggin start on
        /// </summary>
        private OrdinalDirection _startSide;

        /// <summary>
        /// Peice of land the fence were currently draggin ends on
        /// </summary>
        private Land _endTile;

        /// <summary>
        /// Side of the land the fence were currently draggin ends on
        /// </summary>
        private OrdinalDirection _endSide;

        /// <summary>
        /// is the shift key down
        /// </summary>
        private bool _shiftDown = false;
        
        /// <summary>
        /// Create a new road editor for adding roads
        /// </summary>
        public EnclosureEditor(EnclosureType typeToEnclose)
            : base()
        {
            _typeToEnclose = typeToEnclose;
        }

        /// <summary>
        /// Start editing land
        /// </summary>
        protected override void StartEditingInner()
        {
            _fencesPlacedForField.Clear();
            _fencesInProgress.Clear();
            _startTile = null;
            _endTile = null;

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
                RefreshFence();
            }
        }

        private void Graphics_KeyDown(Key key)
        {
            if (key == Key.ShiftLeft || key == Key.ShiftRight)
            {
                _shiftDown = true;
                RefreshFence();
            }
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
                Program.UserInterface.WindowManager.RemoveWindow(_costWindow);
                _costWindow = null;
            }
            
            //remove all fences in progress
            foreach (Fence fencePlacedForField in _fencesPlacedForField)
            {
                fencePlacedForField.Delete();
            }
            _fencesPlacedForField.Clear();            
            foreach (Fence fenceInProgress in _fencesInProgress)
            {
                fenceInProgress.Delete();
            }
            _fencesInProgress.Clear();

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

            //treat right button same as shift
            if (clickInfo.Button == MouseButton.Right)
            {
                _shiftDown = false;
            }

            if (_fencesInProgress.Count > 0)
            {
                //all fences in progress will be part of the field
                _fencesPlacedForField.AddRange(_fencesInProgress);

                //determine if one of the feces placed made an enclosure                
                List<Fence> enclosurePath = new List<Fence>();
                bool newFenceCreatedEnclosure = false;
                foreach (Fence newFence in _fencesInProgress)
                {
                    enclosurePath = new List<Fence>();
                    if (FenceIsPartOfEnclousre(newFence, enclosurePath))
                    {
                        newFenceCreatedEnclosure = true;
                        break;
                    }
                }
                                
                //if the fence places made an enclosure we need to make a field
                if (newFenceCreatedEnclosure)
                {
                    //remove any fences placed in making the field that are not part of the enclosure                    
                    foreach (Fence fencePlacedForField in _fencesPlacedForField.ToArray())
                    {
                        if (enclosurePath.Contains(fencePlacedForField) == false)
                        {
                            fencePlacedForField.Delete();
                            _fencesPlacedForField.Remove(fencePlacedForField);
                        }
                    }

                    //take a fence in the enclosure path, either the land the fence is on, or the land adajcent
                    //to it on the other side of the fence must be part of the field                    
                    Land partOfField = null;
                    if (LandInsideFences(enclosurePath[0].LandOn, enclosurePath) && enclosurePath[0].LocationOn.Contains<Field>() == false)
                    {
                        partOfField = enclosurePath[0].LandOn;
                    }
                    else
                    {
                        partOfField = enclosurePath[0].LandOn.GetAdjacent(enclosurePath[0].SideOn);
                    }

                    //get all the land that is part of the field
                    List<Land> landInField = new List<Land>();
                    CollectLandInsideFence(partOfField, landInField);
                    
                    //decide if equivelent fences are better (equivlent fence is between the same two land tiles but sits on the other peice of land)
                    //we want to fences to be on the land that is part of the enclosure
                    //we only check the fences place just for this field because the others will not be any better or worse if moved to this field
                    DecideIfEquivelentFencesAreBetter(_fencesPlacedForField, landInField);

                    //get the fences that border the land (this may be different from enclousre path because the enclosure path could contain fences that belong to another field) 
                    List<Fence> fieldFences = new List<Fence>();
                    CollectBorderingFences(landInField, fieldFences);

                    //count the number of new fence this is the number of border fences that do not already part of a field
                    int newFencesCount = 0;
                    foreach (Fence fence in fieldFences)
                    {
                        if (fence.EnclosuresBordered.Count == 0)
                        {
                            newFencesCount++;
                        }
                    }
                    
                    //make sure there is not an illegal object in the field
                    bool allowableSpotForField = true;
                    foreach (Land fieldLand in landInField)
                    {
                        if (CanBePartOfField(fieldLand) == false)
                        {
                            allowableSpotForField = false;
                        }
                    }

                    if (allowableSpotForField)
                    {
                        int perFenceCost = GameState.Current.Prices.GetPrice(PriceType.PastureFence);
                        GameState.Current.Treasury.Buy(Treasury.CONSTRUCTION_CATAGORY, "Fences", newFencesCount * perFenceCost);

                        //we will invalidate several locations with all the fences, and the field that get created. Handle them as a batch
                        Program.Game.PathFinder.StartBatchInvalidate();

                        //mark the fences as done with placement
                        foreach (Fence fence in _fencesPlacedForField)
                        {
                            fence.DoneWithPlacement();
                        }
                                                
                        //create the game object to go inside the enclsure of the type the player wants
                        Enclosure newEnclosure = null;
                        if (_typeToEnclose == EnclosureType.Field)
                        {
                            //create a field in the enclosure
                            Field newField = new Field();
                            newField.Setup(landInField, fieldFences);
                            newField.Name = "Field " + GameState.Current.MasterObjectList.FindAll<Field>().Count.ToString();
                            newEnclosure = newField;
                        }
                        else if (_typeToEnclose == EnclosureType.Pasture)
                        {
                            //create a Pasture in the enclosure
                            Pasture newPasture = new Pasture();
                            newPasture.Setup(landInField, fieldFences);
                            newPasture.Name = "Pasture" + GameState.Current.MasterObjectList.FindAll<Pasture>().Count.ToString();
                            newEnclosure = newPasture;
                        }

                        Program.UserInterface.WindowManager.RemoveWindow(_costWindow);
                        _costWindow = null;
                        _fencesPlacedForField.Clear();
                        _fencesInProgress.Clear();
                        this.StopEditing();
                        
                        //done invalidating locations
                        Program.Game.PathFinder.EndBatchInvalidate();

                    }
                    else
                    {
                        //remove all fences
                        foreach (Fence fencePlacedForField in _fencesPlacedForField.ToArray())
                        {
                            fencePlacedForField.Delete();
                        }
                        

                        Program.UserInterface.WindowManager.RemoveWindow(_costWindow);
                        _costWindow = null;
                        _fencesPlacedForField.Clear();
                        _fencesInProgress.Clear();
                        this.StopEditing();
                    }
                }               

            }

            _dragging = false;
            _fencesInProgress.Clear();
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

            _dragging = true;
        }

        /// <summary>
        /// User moved mouse
        /// </summary>
        private void Graphics_MouseMoved(ClickInfo clickInfo)
        {
            //dertemine what land tile was moved over, and what side of it was moved over
            Land landOn = clickInfo.GetLandClicked();
            OrdinalDirection sideOn = OrdinalDirection.NorthEast;
            if (landOn != null)
            {                
                TileClickInfo landClickInfo = clickInfo.GetLandClickedInfo();
                if (landClickInfo.ClickLocationX <= 0.5f && landClickInfo.ClickLocationY <= 0.5f)
                {
                    sideOn = OrdinalDirection.NorthWest;
                }
                else if (landClickInfo.ClickLocationX > 0.5f && landClickInfo.ClickLocationY <= 0.5f)
                {
                    sideOn = OrdinalDirection.NorthEast;
                }
                else if (landClickInfo.ClickLocationX <= 0.5f && landClickInfo.ClickLocationY > 0.5f)
                {
                    sideOn = OrdinalDirection.SouthWest;
                }
                else if (landClickInfo.ClickLocationX > 0.5f && landClickInfo.ClickLocationY > 0.5f)
                {
                    sideOn = OrdinalDirection.SouthEast;
                }
            }


            if (landOn != null)
            {
                if (_dragging == false)
                {
                    _startTile = landOn;
                    _startSide = sideOn;
                    _endTile = landOn;
                    _endSide = sideOn;                    
                }
                else
                {                    
                    _endTile = landOn;
                    _endSide = sideOn;                    
                }
            }

            if (_startTile != null && _endTile != null)
            {
                RefreshFence();
            }
            
        }

        /// <summary>
        /// Refreshes the fences in progress of being placed
        /// </summary>
        private void RefreshFence()
        {
            //remove any fences in progress
            foreach (Fence fenceInProgress in _fencesInProgress)
            {
                fenceInProgress.Delete();
            }
            _fencesInProgress.Clear();

            //find both possible paths for the fence to take from the start tile to the end tile            
            OrdinalDirection firstDirectionPath1, firstDirectionPath2, secondDirectionPath1, secondDirectionPath2;
            OrdinalDirection startSidePath1, endSidePath1, startSidePath2, endSidePath2;
            Land trunLandPath1, trunLandPath2;
            bool straightPath1, straightPath2;
            List<Land> path1 = FindPossiblePath(false, out firstDirectionPath1, out secondDirectionPath1, out startSidePath1, out endSidePath1, out trunLandPath1, out straightPath1);
            List<Land> path2 = FindPossiblePath(true, out firstDirectionPath2, out secondDirectionPath2, out startSidePath2, out endSidePath2, out trunLandPath2, out straightPath2);

            //determine which path didnt require changing the start side
            List<Land> bestPath = path1;
            OrdinalDirection bestPathStartSide = startSidePath1;
            OrdinalDirection bestPathEndSide = endSidePath1;
            OrdinalDirection bestPathFirstDirection = firstDirectionPath1;
            OrdinalDirection bestPathSecondDirection = secondDirectionPath1; 
            Land bestPathTurnLand = trunLandPath1;
            bool bestPathStraight = straightPath1;
            if ((startSidePath2 == _startSide && _shiftDown == false) || (startSidePath2 != _startSide && _shiftDown))
            {
                bestPath = path2;
                bestPathStartSide = startSidePath2;
                bestPathEndSide = endSidePath2;
                bestPathFirstDirection = firstDirectionPath2;
                bestPathSecondDirection = secondDirectionPath2; 
                bestPathTurnLand = trunLandPath2;
                bestPathStraight = straightPath2;
            }

            //create fences fences up until the turnLand will be on the start side
            OrdinalDirection side = bestPathStartSide;
            foreach (Land land in bestPath)
            {
                if (land == bestPathTurnLand && bestPathStraight == false)
                {
                    //after the turn land fence will be on the end side, turn land fences will be added later
                    side = bestPathEndSide;
                }
                else
                {
                    //add a fence
                    AddFenceToProgressIfPossible(land, side);
                }
            }

            //now create 0,1,or 2 fences on the turn land 
            if (bestPathStraight == false)
            {
                if (bestPathEndSide == bestPathFirstDirection && bestPathStartSide == DirectionUtils.OppositeDirection(bestPathSecondDirection))
                {
                    AddFenceToProgressIfPossible(bestPathTurnLand, bestPathStartSide);
                    AddFenceToProgressIfPossible(bestPathTurnLand, bestPathEndSide);
                }
                else if (bestPathEndSide == DirectionUtils.OppositeDirection(bestPathFirstDirection) && bestPathStartSide == DirectionUtils.OppositeDirection(bestPathSecondDirection))
                {
                    AddFenceToProgressIfPossible(bestPathTurnLand, bestPathEndSide);
                }
                else if (bestPathEndSide == bestPathFirstDirection && bestPathStartSide == bestPathSecondDirection)
                {
                    AddFenceToProgressIfPossible(bestPathTurnLand, bestPathStartSide);
                }
            }

            if (_costWindow == null)
            {
                _costWindow = new CostWindow(GameState.Current.Treasury);
            }
            int perFenceCost = GameState.Current.Prices.GetPrice(PriceType.PastureFence);
            _costWindow.Cost = (_fencesPlacedForField.Count + _fencesInProgress.Count) * perFenceCost;

        }

        /// <summary>
        /// Simple helper method that checks to make sure a fence is valid beofre adding it the in-progress list
        /// </summary>
        private void AddFenceToProgressIfPossible(Land land, OrdinalDirection side)
        {
            if (FenceCanBePlaced(land, side))
            {
                Fence fence = new Fence();
                fence.Setup(land, side);
                fence.TypeFor = _typeToEnclose;
                _fencesInProgress.Add(fence);
            }
        }
        
        /// <summary>
        /// Finds one of the two possible paths that a dragged fence may be intended to take.
        /// Retuns info about the path, what side of the land the first lenght of fences is on, what side the second is on.
        /// What directions the fences run first, and what direction they run next. What land the fences turn on, or if the fences are straight.
        /// </summary>
        private List<Land> FindPossiblePath(bool path2, out OrdinalDirection firstDirection, out OrdinalDirection secondDirection, out OrdinalDirection startSideFix, out OrdinalDirection endSideFix, out Land turnLand, out bool straightPath)
        {
            //find the paths for the fence to take from the start tile to the end tile  
            RightAnglePathFinder rightAngleFinder = new RightAnglePathFinder();
            List<Land> path = rightAngleFinder.FindPath(_startTile, _endTile, path2, out turnLand, out straightPath, out firstDirection, out secondDirection);
            
            //default to using the start side, and end side that the user had his mouse at
            startSideFix = _startSide;
            endSideFix = _endSide;

            //if the path is more than one tile long the start side might need to be changed
            if (_startTile != _endTile)
            {
                if ((firstDirection == OrdinalDirection.NorthEast || firstDirection == OrdinalDirection.SouthWest) && (_startSide == OrdinalDirection.SouthWest || _startSide == OrdinalDirection.NorthEast))
                {
                    startSideFix = OrdinalDirection.SouthEast;
                }
                if ((firstDirection == OrdinalDirection.NorthWest || firstDirection == OrdinalDirection.SouthEast) && (_startSide == OrdinalDirection.SouthEast || _startSide == OrdinalDirection.NorthWest))
                {
                    startSideFix = OrdinalDirection.SouthWest;
                }
            }

            //the end side may need to be fixed so it can work with the start side
            if ((startSideFix == OrdinalDirection.NorthEast || startSideFix == OrdinalDirection.SouthWest) && (endSideFix == OrdinalDirection.NorthEast || endSideFix == OrdinalDirection.SouthWest))
            {
                endSideFix = OrdinalDirection.SouthEast;
            }
            if ((startSideFix == OrdinalDirection.SouthEast || startSideFix == OrdinalDirection.NorthWest) && (endSideFix == OrdinalDirection.SouthEast || endSideFix == OrdinalDirection.NorthWest))
            {
                endSideFix = OrdinalDirection.SouthWest;
            }

            return path;
        }
        
        /// <summary>
        /// Determines if the fence passed is part of an enclosure, and if it is
        /// add the enclosure path to enclosurePath list.
        /// </summary>
        private bool FenceIsPartOfEnclousre(Fence fence, List<Fence> enclosurePath)
        {
            //first check that the fence has at least 2 adajcent fences
            if (fence.AdjacentFences.Count < 2)
            {
                return false;
            }
            
            //now check that there is at least one fence coming in at each post.            
            //all fences coming in at the same post will be adjacent to one another so find list of fences adjacent to post A, and another list adjacent to its post B
            HashSet<Fence> adjacentToPostA = new HashSet<Fence>();
            HashSet<Fence> adjacentToPostB = new HashSet<Fence>();

            //arbitrailty choose the post that fence 0 is adjacent to as post A
            Fence postAFence1 = fence.AdjacentFences[0];
            adjacentToPostA.Add(postAFence1);

            //look at all the fences in the adjacnet list
            foreach (Fence adjacentFence in fence.AdjacentFences)
            {
                //dont add postAFence1 again
                if (adjacentFence == postAFence1)
                {
                    continue;
                }

                //if the fence is adjacent to postAFence1 it is adjcent on post A so add to the post A list
                if (adjacentFence.AdjacentFences.Contains(postAFence1))
                {
                    adjacentToPostA.Add(adjacentFence);
                }
                //oteher wise it must be adajcent to post B
                else 
                {
                    adjacentToPostB.Add(adjacentFence);
                }
            }

            //if there is not atleast one fence adajcent to post B then this fence can not be part of an enclosure
            if (adjacentToPostB.Count == 0)
            {
                return false;
            }
            
            //now search for a fence in the post B list, by starting at a fence in the post A list.
            //But first add the passed fence to the visited list so we wont search though it to get to post B.
            HashSet<Fence> visitList = new HashSet<Fence>();
            visitList.Add(fence);
            
            //do the search, and get a list of fences involved in the path 
            bool foundPostBFence = SearchFences(postAFence1, visitList, adjacentToPostB, enclosurePath);

            //dont bother with other calculations if we didnt find a path
            if (foundPostBFence == false) { return false; }
            
            //add the start fence to the enclosure path
            enclosurePath.Add(fence);

            //some unnisary fences may be in the enclouser path,  fences that jutt off from corners could be in there, as well as fences that form figure 8s
            // ___________  (jutt off)    
            // | Field |
            // |       |
            // |_______|___  (figure 8)  
            //         |   |
            //         |___|
            //

            //remove fence that jutt out from corners
            RemoveJuttOffFences(enclosurePath);
            
            //remove fence that form figure 8s
            RemoveFigure8Fences(enclosurePath);

            //return that we found an enclosure or not
            return true;
        }

        /// <summary>
        /// Search through adjacent fences from the fence passed, trying to find one of the fences in the search for list.
        /// Dont visit fences in the visited list (they have already been searched).  
        /// If a path is found from the search list to a fence in search for list it will be added to the path list passed.
        /// </summary>
        private bool SearchFences(Fence search, HashSet<Fence> visitList, HashSet<Fence> searchForList, List<Fence> path)
        {
            //check if we found the fence were searching for
            if (searchForList.Contains(search))
            {
                path.Add(search);
                return true;
            }

            //add fence to the lsit of fences visted
            visitList.Add(search);

            //search all adjacent fences
            foreach (Fence adjacentFence in search.AdjacentFences.Reverse<Fence>())
            {
                //dont revist a fence that was already visted, dont search through gates
                if (visitList.Contains(adjacentFence) == false)
                {
                    bool foundEnd = SearchFences(adjacentFence, visitList, searchForList, path);
                    if (foundEnd)
                    {
                        path.Add(search);
                        return true;
                    }
                }
            }

            //none of the fences adjacent to me led to the search for fence
            return false;
        }
        
        /// <summary>
        /// Removes jutt off fences from a fence path
        /// </summary>        
        private void RemoveJuttOffFences(List<Fence> enclosurePath)
        {
            //                                                ____
            //                                               |   | 
            //this removes fences that jutt off at corners   |___|_ <-  

            
            //collect a list of unnessisary fences
            List<Fence> unnessisaryFences = new List<Fence>();

            //walk through each fence in the path
            for (int i = 0; i < enclosurePath.Count; i++)
            {
                //get the fence were examining, as well as the previous fence and the next fence
                Fence toCheck = enclosurePath[i];
                Fence prev = enclosurePath[(i - 1 + enclosurePath.Count) % enclosurePath.Count];
                Fence next = enclosurePath[(i + 1) % enclosurePath.Count];

                //if the prvious fence is adjacent to the next fence we have found a jutt off fence
                if (prev.AdjacentFences.Contains(next))
                {
                    unnessisaryFences.Add(toCheck);
                }
            }

            //remove the unnisary fences
            foreach (Fence unnessisaryFence in unnessisaryFences)
            {
                enclosurePath.Remove(unnessisaryFence);
            }
        }
        
        /// <summary>
        /// Removes one half of the fences involved in a figure 8 fence path
        /// </summary>
        private void RemoveFigure8Fences(List<Fence> enclosurePath)
        {
            //                                                ____
            //                                               |   | 
            //this removes fences involved in figure 8s      |___|____  
            //                                                   |   |
            //                                                   |___|


            //
            //                                                ____
            //                                               |   | 
            //enclosure paths like this are imposible        |___|
            //beacuse the fence in the middel must have      |   |
            //been visted twice to form such a path          |___|
            


            //there may be multiple firgure 8s keep doing remove passes until none are found
            while (true) 
            {
                //collect a list of unnessisary fences
                List<Fence> unnessisaryFences = new List<Fence>();

                //loop until we dont remove a figure 8
                bool removedFigure8 = false;

                //walk each fence on the path
                for (int i = 0; i < enclosurePath.Count; i++)
                {
                    //the fence at this index
                    Fence toCheck = enclosurePath[i];

                    //the fence previous to this index
                    Fence prev = enclosurePath[(i - 1 + enclosurePath.Count) % enclosurePath.Count];

                    //starting two fences ahead of this one go until we reach the previous fence. if we reach a fence adjacent to this fence before reaching the previous fence then there is a figure 8.
                    //(cant start one fence ahead as it should be ajacent to this fence)
                    int numberAhead = 2;
                    Fence fenceAhead = enclosurePath[(i + numberAhead) % enclosurePath.Count];
                    while (fenceAhead != prev)
                    {
                        //if the fence at least 2 ahead of this fence (that is not the previous fence) is adjacent to this fence then there is a figure 8
                        if (toCheck.AdjacentFences.Contains(fenceAhead))
                        {
                            //remove all fences from one ahead of this fence till the fence adjacent, these are all the fences involved in the figure 8
                            for (int j = 1; j <= numberAhead; j++)
                            {
                                unnessisaryFences.Add(enclosurePath[(i + j) % enclosurePath.Count]);
                            }

                            //we found and marked a figure 8, so stop searching for one
                            removedFigure8 = true;
                            break;
                        }

                        //search the next fence ahead of this fence
                        numberAhead++;
                        fenceAhead = enclosurePath[(i + numberAhead) % enclosurePath.Count];
                    }

                    //if we removed a figure 8 the are done with this pass
                    if (removedFigure8)
                    {
                        break;
                    }
                }

                //remove the unnisary fences involved in the figure 8
                foreach (Fence unnessisaryFence in unnessisaryFences)
                {
                    enclosurePath.Remove(unnessisaryFence);
                }
                unnessisaryFences.Clear();

                //if no figure 8 was removed were done, other wise we will do another pass
                if (removedFigure8 == false)
                {
                    break;
                }
            }
        }
        
        /// <summary>
        /// Determine if a fence can be placed on the side of the land passed
        /// </summary>
        private bool FenceCanBePlaced(Land land, OrdinalDirection side)
        {
            //cant place fence on land that is already a field
            if (land.LocationOn.Contains<Field>())
            {
                return false;
            }

            //find all the fences on the peice of land
            List<Fence> fencesOnLand = land.LocationOn.FindAll<Fence>();

            //if there is a fence on the same peice of land on the same side, then a fence is already present so it cant be placed
            foreach (Fence fenceOnLand in fencesOnLand)
            {
                if (fenceOnLand.SideOn == side)
                {
                    return false;
                }
            }

            //find all fence on the peice of land, adjacent to this land on the side of the fence
            List<Fence> fencesOnAjacentLand = land.GetAdjacent(side).LocationOn.FindAll<Fence>();

            //if there is a fence on the adjacent land on the opposite side then an equivellent fence is already present so it cant be placed
            foreach (Fence fenceOnAjacentLand in fencesOnAjacentLand)
            {
                if (fenceOnAjacentLand.SideOn == DirectionUtils.OppositeDirection(side))
                {
                    return false;
                }
            }

            //if none of the above are problems the fence can be placed
            return true;
        }
        
        /// <summary>
        /// Return is the land passed is inside the list of fences passed
        /// </summary>
        private bool LandInsideFences(Land land, List<Fence> fences)
        {
            //go north west until the edge of the map is reached, 
            //if an even number of fences are crossed the land is not inside,
            //if an odd number of fences is creossed the land is inside
            int fencesCrossed = 0;

            //start on the land passed
            Land landOn = land;
            
            //move northwest unil we loop around at the edge of the map            
            while (true)
            {
                //get all the fences on the land
                List<Fence> fencesOnLand = landOn.LocationOn.FindAll<Fence>();

                
                foreach (Fence fenceOnLand in fencesOnLand)
                {
                    //see if the fence is on the nortwest, and if its one of the fences we are taking into account
                    if (fenceOnLand.SideOn == OrdinalDirection.NorthWest && fences.Contains(fenceOnLand))
                    {
                        //we crossed this fence
                        fencesCrossed++;
                    }
                    //see if the fence is on the southeast, and if its one of the fences we are taking into account, And we are not at the start land 
                    else if (fenceOnLand.SideOn == OrdinalDirection.SouthEast && fences.Contains(fenceOnLand) && landOn != land)
                    {
                        //we crossed this fence
                        fencesCrossed++;
                    }
                }

                //check if the movement causes us to loop around the 
                if (landOn.NorthWest.LocationOn.Y > landOn.LocationOn.Y || landOn.NorthWest.LocationOn.X > landOn.LocationOn.X)
                {
                    //if so were done
                    break;
                }
                else
                {
                    //move to the tile to the northwest of here
                    landOn = landOn.NorthWest;
                }                
            }
            

            //return true for an odd number
            return (fencesCrossed % 2 == 1);
        }
        
        /// <summary>
        /// Collect land that is inside of a fence, starting from the land passed
        /// </summary>
        private void CollectLandInsideFence(Land land, List<Land> landInsideFence)
        {
            //just the same as  landInsideFence, but in a hashset to speed up .Contains
            HashSet<Land> visitedLand = new HashSet<Land>();

            //this function used to be recursive but was causing stack overflow, so using heap stack instead
            Stack<Land> stack = new Stack<Land>();
            stack.Push(land);
            while(stack.Count > 0)
            {
                land = stack.Pop();

                //if land already added to list then skip
                if (visitedLand.Contains(land))
                {
                    continue;
                }

                //add the current land
                landInsideFence.Add(land);
                visitedLand.Add(land);

                //get the fences on this land
                List<Fence> fencesOnLand = land.LocationOn.FindAll<Fence>();

                //foreach other direction add that land if a fence doesnt prevent it
                foreach (OrdinalDirection direction in new OrdinalDirection[] { OrdinalDirection.NorthEast, OrdinalDirection.NorthWest, OrdinalDirection.SouthEast, OrdinalDirection.SouthWest })
                {
                    //get the adjacent land
                    Land adjacentLand = land.GetAdjacent(direction);

                    //get the fences on the adjacent land
                    List<Fence> fencesOnAdjacentLand = adjacentLand.LocationOn.FindAll<Fence>();
                    
                    //check if there is a fence on this land preventing us from getting to the adjacent land
                    bool fencePreventsTravel = false;
                    foreach (Fence fenceOnLand in fencesOnLand)
                    {
                        if (fenceOnLand.SideOn == direction)
                        {
                            fencePreventsTravel = true;
                            break;
                        }
                    }

                    //check if there is a fence on the adjacentland preventing us from getting to the adjacent land                
                    foreach (Fence fenceOnAdjacentLand in fencesOnAdjacentLand)
                    {
                        if (fenceOnAdjacentLand.SideOn == DirectionUtils.OppositeDirection(direction))
                        {
                            fencePreventsTravel = true;
                            break;
                        }
                    }

                    //if a fence does not prevent travel explore the adjacent land
                    if (fencePreventsTravel == false)
                    {
                        stack.Push(adjacentLand);                        
                    }
                }
            }
        }
                
        /// <summary>
        /// Collect fences the enclose the list of land passed
        /// </summary>
        private void CollectBorderingFences(List<Land> land, List<Fence> fences)
        {
            foreach (Land landTile in land)
            {

                //add all fences on this peice of land
                foreach (Fence fenceOnLand in landTile.LocationOn.FindAll<Fence>())
                {
                    if (fences.Contains(fenceOnLand) == false)
                    {
                        fences.Add(fenceOnLand);
                    }
                }

                //add all fences on adjacent land that border this peice of land                
                foreach (OrdinalDirection direction in new OrdinalDirection[] { OrdinalDirection.NorthEast, OrdinalDirection.NorthWest, OrdinalDirection.SouthEast, OrdinalDirection.SouthWest })
                {
                    //get the adjacent land
                    Land adjacentLand = landTile.GetAdjacent(direction);

                    //get the fences on the adjacent land
                    List<Fence> fencesOnAdjacentLand = adjacentLand.LocationOn.FindAll<Fence>();
                    
                    //check if the fence is on the side that borders this peice of land                
                    foreach (Fence fenceOnAdjacentLand in fencesOnAdjacentLand)
                    {
                        if (fenceOnAdjacentLand.SideOn == DirectionUtils.OppositeDirection(direction))
                        {
                            if (fences.Contains(fenceOnAdjacentLand) == false)
                            {
                                fences.Add(fenceOnAdjacentLand);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determine if the land passed can be part of a field.
        /// </summary>
        private bool CanBePartOfField(Land land)
        {
            //only make sure the things inside the field are allowed inside the field

            foreach (GameObject obj in land.LocationOn.AllObjects)
            {
                if ((obj is Worker || obj is Animal || obj is Land || obj is Fence) == false)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// When a fence borders only 1 field then it is better to have the fence be on that fields tile.  
        /// This method takes care of moving fences so this is the case.        
        /// Pass a list of the new fences place and the land that makes up the new field
        /// The fences list may contains the new fence objects when returned
        /// </summary>
        private void DecideIfEquivelentFencesAreBetter(List<Fence> fencesJustPlaced, List<Land> landInEnclosure)
        {
            for (int fenceIndex = 0; fenceIndex < fencesJustPlaced.Count; fenceIndex++)
            {
                //get the current fence
                Fence currentFence = fencesJustPlaced[fenceIndex];
                                
                //if the fence is not on the field land
                if (landInEnclosure.Contains(currentFence.LandOn) == false)
                {
                    //create new fence on the land that is in the field
                    Fence newFence = new Fence();
                    Land newFenceLand = currentFence.LandOn.GetAdjacent(currentFence.SideOn);
                    Debug.Assert(landInEnclosure.Contains(newFenceLand));
                    OrdinalDirection newFenceSide = DirectionUtils.OppositeDirection(currentFence.SideOn);
                    newFence.Setup(newFenceLand, newFenceSide);
                    newFence.TypeFor = _typeToEnclose;

                    //delete the old fence
                    currentFence.Delete();

                    //remove from fences list
                    fencesJustPlaced.Remove(currentFence);
                    fencesJustPlaced.Insert(fenceIndex, newFence);
                }
            }
        }
    }
}
