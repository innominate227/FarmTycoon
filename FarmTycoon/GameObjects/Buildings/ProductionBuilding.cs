using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// Production building turn one type of item into another type.
    /// A worker can be assigned to work at a production building.
    /// The number of workers working the building determines how fast production happens.
    /// </summary>
    public class ProductionBuilding : GameObject, IStorageBuilding, IHasTextureManager, IHasInventory, IHasInfo, IHoldsWorkers, IHasActionLocation
    {
        #region Member Vars

        /// <summary>
        /// ProductionBuildingInfo for this building
        /// </summary>
        private ProductionBuildingInfo _buildingInfo;
        
        /// <summary>
        /// The inventory of the production building
        /// </summary>
        private Inventory _inventory;

        /// <summary>
        /// The notification this production building uses to decide if it should produce
        /// </summary>
        private Notification _notification;
                
        /// <summary>
        /// The tile for the building
        /// </summary>
        private MobileGameTile _tile;

        /// <summary>
        /// Texture manager for the delivery area
        /// </summary>
        private TextureManager _textureManager;

        /// <summary>
        /// List of workers inside the production building (or on their way to the production building)
        /// </summary>
        private WorkersInsideList _workersInside = new WorkersInsideList();

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a new Production Building
        /// Setup or ReadState must be called after creation.
        /// </summary>
        public ProductionBuilding() : base() { }
        
        /// <summary>
        /// Setup the tile for the building
        /// </summary>
        private void SetupTile()
        {
            //create the tile
            _tile = new MobileGameTile(this);
            _textureManager.SetTileToUpdate(_tile);
        }

        /// <summary>
        /// Setup the Production Building
        /// </summary>
        public void Setup(Location centerLocation, ProductionBuildingInfo buildingInfo)
        {            
            _buildingInfo = buildingInfo;

            //create inventory for the building
            _inventory = new Inventory();
            _inventory.SetUp(buildingInfo);

            //add the building to the locations it ocupies
            AddLocationsOn(LocationUtils.GetLocationList(centerLocation, buildingInfo.LandOn));
                        
            //setup texture manager
            _textureManager = new TextureManager();
            _textureManager.Setup(buildingInfo, this);

            //setup tile for the building
            SetupTile();

            //the object can not be walked through
            PathEffect = ObjectsEffectOnPath.Solid;

            //update the tile
            UpdateTiles();
        }
                
        /// <summary>
        /// Building is finished being placed
        /// </summary>
        public override void DoneWithPlacement()
        {
            base.DoneWithPlacement();

            //have the building start production
            _notification = Program.GameThread.Clock.RegisterNotification(ProductionIntervalElapsed, _buildingInfo.Interval, true);
            UpdateNotificationInterval();
        }

        /// <summary>
        /// Called when the production building is being deleted
        /// </summary>
        protected override void DeleteInner()
        {
            base.DeleteInner();
            _tile.Delete();
            _inventory.Delete();
            _textureManager.Delete();
            if (_notification != null)
            {
                //remove notification if we created it
                Program.GameThread.Clock.RemoveNotification(_notification);
            }
        }
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// BuildingInfo for this building
        /// </summary>
        public ProductionBuildingInfo BuildingInfo
        {
            get { return _buildingInfo; }
        }

        /// <summary>
        /// The location a worker should travel to in order to perform an action on this building
        /// </summary>
        public Location ActionLocation
        {
            get { return _buildingInfo.ActionLocation.GetRealtiveLocation(LocationOn); }
        }

        /// <summary>
        /// The inventory of the building
        /// </summary>
        public Inventory Inventory
        {
            get { return _inventory; }
        }

        /// <summary>
        /// Texture Manager for the building
        /// </summary>
        public TextureManager TextureManager
        {
            get { return _textureManager; }
        }
                
        /// <summary>
        /// StorageBuildingInfo for this building
        /// </summary>        
        public IStorageBuildingInfo StorageBuildingInfo
        {
            get { return _buildingInfo; }
        }

        /// <summary>
        /// Info for this building
        /// </summary>        
        public IInfo Info
        {
            get { return _buildingInfo; }
        }

        /// <summary>
        /// List of workers inside the production building (or on their way to the production building)
        /// </summary>
        public WorkersInsideList WorkersInside
        {
            get { return _workersInside; }
        }
        
        #endregion
        
        #region Logic

        /// <summary>
        /// Check if the production building has become overstaffed,
        /// and if so add an issue to the issue manager
        /// </summary>
        private void CheckIfOverWorked()
        {
            if (_workersInside.WorkersInside.Count > _buildingInfo.MaxWorkers)
            {
                GameState.Current.IssueManager.ReportIssue(this, "Overstaffed", _name + " is overstaffed");
            }
            else
            {
                GameState.Current.IssueManager.ClearIssue(this, "Overstaffed");
            }
        }

        /// <summary>
        /// Update the notification interval for the building based on the number of workers the are working in the building
        /// </summary>
        private void UpdateNotificationInterval()
        {
            //determine the interval.  Note if the building needs workers and has zero works, the interval determined 
            //does not matter the TimePassedHandler() will not allow production.         

            //the max interval for the building
            double interval = _buildingInfo.Interval;

            //the building has workers, and needs workers
            if (_buildingInfo.MaxWorkers > 0 && _workersInside.WorkersInside.Count > 0)
            {
                //determine the interval based on how staffed the building is
                interval = _buildingInfo.Interval * ((double)_buildingInfo.MaxWorkers / (double)_workersInside.WorkersInside.Count);

                //dont allow it to go faster then the max interval, incase the building is overstaffed
                if (interval < _buildingInfo.Interval)
                {
                    interval = _buildingInfo.Interval;
                }
            }

            //if we dont round the interval to a resonable number of decimal places we could loose percision when using grouping, and it trips an assert
            interval = Math.Round(interval, 2);

            //update the notifications interval
            _notification = Program.GameThread.Clock.UpdateNotification(_notification, interval);
        }
               
        /// <summary>
        /// Raised everytime the production interval has passed
        /// </summary>
        private void ProductionIntervalElapsed()
        {
            //make sure we dont have the building over staffed (if we do we tell the player)
            CheckIfOverWorked();

            //if we require workers but dont have any, then do nothing
            if (_workersInside.WorkersInside.Count == 0 && _buildingInfo.MaxWorkers > 0)
            {
                return;
            }
            
            //check if we have the inputs
            ItemList inputs = new ItemList();
            bool haveInputs = CollectInputs(inputs);

            //if we had everything make the outputs
            if (haveInputs)
            {
                MakeOutputs(inputs);
            }
        }

        /// <summary>
        /// look through the buildings inventory to find the inputs it needs to make its output.
        /// return true if they were found, and fill the inputs list with the items to use as inputs
        /// If true is returned the items in the inputs list are removed from the inventory
        /// </summary>
        private bool CollectInputs(ItemList inputs)
        {            
            //look at each input type needed
            foreach (ItemTypeInfo inputType in _buildingInfo.Inputs.Keys)
            {
                int amountOfTypeNeeded = _buildingInfo.Inputs[inputType];

                //look at each item type in the building
                foreach (ItemType itemTypeInBuilding in _inventory.Types)
                {
                    //if that item is what we need use it
                    if (itemTypeInBuilding.BaseType == inputType)
                    {
                        //get total amount we have of that type we will need to use
                        int amountOfItemInBuildingToUse = _inventory.GetTypeCount(itemTypeInBuilding);
                        if (amountOfItemInBuildingToUse > amountOfTypeNeeded)
                        {
                            amountOfItemInBuildingToUse = amountOfTypeNeeded;
                        }

                        //add to list of items to use, and decrease from amount needed.
                        inputs.IncreaseItemCount(itemTypeInBuilding, amountOfItemInBuildingToUse);
                        amountOfTypeNeeded -= amountOfItemInBuildingToUse;

                        //if we dont need any more of that type stop looking
                        if (amountOfTypeNeeded == 0) { break; }
                    }                    
                }

                //if we were unable to find all of one of the types need we must not have all the items
                if (amountOfTypeNeeded > 0)
                {
                    return false;
                }
            }

            //if we got here we must have all the items we need
            //remove each item used as input from the invnentory
            foreach(ItemType itemTypeToUse in inputs.ItemTypes)
            {
                _inventory.RemoveFromInvetory(itemTypeToUse, inputs.GetItemCount(itemTypeToUse));
            }

            //we were able to find all needed items
            return true;            
        }

        /// <summary>
        /// Create the buildings outputs given the list of inputs.
        /// Adds the outputs to the buildings inventory
        /// </summary>        
        private void MakeOutputs(ItemList inputs)
        {
            //calculate the average quality of all inputs
            double inputsAverageQualityTotal = 0;
            int inputsCountTotal = 0;
            foreach (ItemType inputType in inputs.ItemTypes)
            {
                inputsAverageQualityTotal += (inputType.Quality * inputs.GetItemCount(inputType));
                inputsCountTotal += inputs.GetItemCount(inputType);
            }
            int inputsAverageQuality = 0;
            if (inputsCountTotal > 0)
            {
                inputsAverageQuality = (int)Math.Round(inputsAverageQualityTotal / inputsCountTotal);
            }
            
            //now add the output resources all having the average quality of the inputs
            foreach (ItemTypeInfo outputType in _buildingInfo.Outputs.Keys)
            {
                int amountMade = _buildingInfo.Outputs[outputType];

                //get the output type to create
                ItemType toCreate = null;
                if (outputType.ItemTypeRelation == ItemTypeRelation.One)
                {
                    toCreate = GameState.Current.ItemPool.GetItemType(outputType.Name);
                }
                else if (outputType.ItemTypeRelation == ItemTypeRelation.Qualities)
                {
                    toCreate = GameState.Current.ItemPool.GetItemType(outputType.Name, inputsAverageQuality);
                }
                else
                {
                    Debug.Assert(false);
                }

                //if we made more than will fit just put what will fit
                int amountToPut = amountMade;
                if (amountToPut > _inventory.AmountThatWillFit(toCreate))
                {
                    amountToPut = _inventory.AmountThatWillFit(toCreate);
                }

                //put the items into the inventory
                _inventory.AddToInvetory(toCreate, amountToPut);
            }
        }

        /// <summary>
        /// Update the tile for the production building
        /// </summary>
        public override void UpdateTiles()
        {
            _tile.MoveToLocation(LocationOn);
            _textureManager.Refresh();
        }

        #endregion 
        
        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteInfo(_buildingInfo);
            writer.WriteObject(_inventory);
            writer.WriteNotification(_notification);
            writer.WriteObject(_workersInside);            
            writer.WriteObject(_textureManager);            
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
			_buildingInfo = reader.ReadInfo<ProductionBuildingInfo>();
			_inventory = reader.ReadObject<Inventory>();
			_notification = reader.ReadNotification(ProductionIntervalElapsed);
            _workersInside = reader.ReadObject<WorkersInsideList>();
			_textureManager = reader.ReadObject<TextureManager>();			
		}

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
            SetupTile();
        }
        #endregion

    }
}
