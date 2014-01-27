using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    /// <summary>
    /// Editor to used to place buildings, scenery, troughs, and other ploppable game objects into the game world
    /// </summary>
    public class PloppableEditor : Editor
    {
        /// <summary>
        /// Window showing how much it will cost to bild the building in progress
        /// </summary>
        private CostWindow _costWindow;

        /// <summary>
        /// GameObject in progress of being placed
        /// </summary>
        private GameObject _objectInProgress;
        
        /// <summary>
        /// Info for the game object being placed
        /// </summary>
        private IPloppableInfo _objectInfo;
                
        /// <summary>
        /// Create a editor for placing ploppable game objects into the game world
        /// </summary>
        public PloppableEditor() : base() { }
        
        /// <summary>
        /// Sets the IPloppableInfo object which determines the type of object that is going to be plopped
        /// </summary>
        public void SetObjectInfo(IPloppableInfo objectInfo)
        {
            _objectInfo = objectInfo;
        }

        /// <summary>
        /// Start editing 
        /// </summary>
        protected override void StartEditingInner()
        {
            Program.UserInterface.Graphics.Events.KeyDown += new KeyboardEventHandler(Graphics_KeyDown);
            Program.UserInterface.Graphics.Events.MouseDown += new MouseEventHandler(Graphics_MouseDown);
            Program.UserInterface.Graphics.Events.MouseMoved += new MouseEventHandler(Graphics_MouseMoved);
        }

        private void Graphics_KeyDown(Key key)
        {
            if (key == Key.Escape)
            {
                this.StopEditing();
            }
        }
                
        /// <summary>
        /// Stop editing 
        /// </summary>
        protected override void StopEditingInner()
        {
            if (_objectInProgress != null)
            {
                _objectInProgress.Delete();
                _objectInProgress = null;
            }
            if (_costWindow != null)
            {
                Program.UserInterface.WindowManager.RemoveWindow(_costWindow);
                _costWindow = null;
            }

            Program.UserInterface.Graphics.Events.KeyDown -= new KeyboardEventHandler(Graphics_KeyDown);
            Program.UserInterface.Graphics.Events.MouseDown -= new MouseEventHandler(Graphics_MouseDown);
            Program.UserInterface.Graphics.Events.MouseMoved -= new MouseEventHandler(Graphics_MouseMoved);
        }


        /// <summary>
        /// User lowered mouse button
        /// </summary>
        private void Graphics_MouseDown(ClickInfo clickInfo)
        {
            if (clickInfo.Button != MouseButton.Left && clickInfo.Button != MouseButton.Right) { return; }
            if (clickInfo.TileClicked)
            {
                if (_objectInProgress != null && _costWindow != null)
                {
                    //buy the object
                    GameState.Current.Treasury.Buy(Treasury.CONSTRUCTION_CATAGORY, _objectInfo.Name, _costWindow.Cost);

                    //find a valid name for the building
                    string namebase = _objectInfo.Name;
                    int numberToUse = 1;
                    string nameToUse = "";
                    bool foundGoodNumber = false;
                    while (foundGoodNumber == false)
                    {
                        foundGoodNumber = true;
                        nameToUse = namebase + " " + numberToUse.ToString();
                        foreach (GameObject gameObject in GameState.Current.MasterObjectList.FindAll<GameObject>())
                        {
                            if (gameObject.Name.Trim().ToUpper() == nameToUse.ToUpper())
                            {
                                foundGoodNumber = false;
                                numberToUse++;
                                break;
                            }
                        }
                    }

                    //set the name for the object                
                    _objectInProgress.Name = nameToUse;

                    //building has been placed
                    _objectInProgress.DoneWithPlacement();
                    _objectInProgress = null;                    
                }
            }
        }

        /// <summary>
        /// User moved mouse
        /// </summary>
        private void Graphics_MouseMoved(ClickInfo clickInfo)
        {
            Tile.StartChangeSet();
            if (_objectInProgress != null)
            {
                _objectInProgress.Delete();
                _objectInProgress = null;
            }
            
            Land landOn = clickInfo.GetLandClicked();
            if (landOn != null)
            {
                //create the cost window if not created yet
                if (_costWindow == null)
                {
                    _costWindow = new CostWindow(GameState.Current.Treasury);
                }
                                       
                                                
                //show the solid object if the land on is acceptable
                if (LocationAcceptableForSolidObject(landOn.LocationOn, _objectInfo))
                {
                    //create the building
                    if (_objectInfo is SceneryInfo)
                    {
                        Scenery sceneryInProgress = new Scenery();
                        sceneryInProgress.Setup(landOn.LocationOn, (SceneryInfo)_objectInfo);
                        _objectInProgress = sceneryInProgress;
                    }
                    else if (_objectInfo is StorageBuildingInfo)
                    {
                        StorageBuilding buildingInProgress = new StorageBuilding();
                        buildingInProgress.Setup(landOn.LocationOn, (StorageBuildingInfo)_objectInfo);
                        _objectInProgress = buildingInProgress;
                    }
                    else if (_objectInfo is ProductionBuildingInfo)
                    {
                        ProductionBuilding buildingInProgress = new ProductionBuilding();
                        buildingInProgress.Setup(landOn.LocationOn, (ProductionBuildingInfo)_objectInfo);
                        _objectInProgress = buildingInProgress;
                    }
                    else if (_objectInfo is DeliveryAreaInfo && GameState.Current.MasterObjectList.Contains<DeliveryArea>() == false)
                    {
                        DeliveryArea buildingInProgress = new DeliveryArea();
                        buildingInProgress.Setup(landOn.LocationOn);
                        _objectInProgress = buildingInProgress;
                    }
                    else if (_objectInfo is TroughInfo)
                    {
                        Trough buildingInProgress = new Trough();
                        buildingInProgress.Setup(landOn.LocationOn, (TroughInfo)_objectInfo);
                        _objectInProgress = buildingInProgress;
                    }
                    else if (_objectInfo is BreakHouseInfo)
                    {
                        BreakHouse buildingInProgress = new BreakHouse();
                        buildingInProgress.Setup(landOn.LocationOn, (BreakHouseInfo)_objectInfo);
                        _objectInProgress = buildingInProgress;
                    }

                    _costWindow.Cost = GameState.Current.Prices.GetPrice(_objectInfo);
                    _costWindow.Visible = true;
                }
                else
                {
                    _costWindow.Visible = false;
                }                                
            }

            Tile.EndChangeSet();
        }




        /// <summary>
        /// Determines if a solid object can be added to the location
        /// </summary>
        private bool LocationAcceptableForSolidObject(Location centerLocation, IPloppableInfo IPloppableInfo)
        {
            //get the locations the building is on
            List<Location> buildingLocations = LocationUtils.GetLocationList(centerLocation, IPloppableInfo.LandOn);
                        
            if (IPloppableInfo is TroughInfo)
            {
                //if this is an animal building make sure its in a pasture, and there are no workers
                return LocationUtils.IsGoodLocationToBuild(buildingLocations, BuildRequirementFlags.NoWorkers | BuildRequirementFlags.InsidePasture);                
            }
            else
            {
                //if this is not an animal building, just make sure there are no wokrers there
                return LocationUtils.IsGoodLocationToBuild(buildingLocations, BuildRequirementFlags.NoWorkers);                
            }

        }


    }
}
