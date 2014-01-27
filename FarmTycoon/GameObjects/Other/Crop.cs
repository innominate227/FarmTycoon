using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// A crop that is grown in a field.  Crops can be either harvested (wheat) or picked (apples).  When the crop is harvested/picked it creates a game item of
    /// a certain type and qaulity based on the CropInfo.  Crops have traits and the traits determine the quality of the crop.  A crop can die if its traits are below a certain value.
    /// </summary>
    public class Crop : GameObject, IHasQuality, IHasTraits, IHasActionLocation, IHasEvents, IHasTextureManager, IHasInfo
    {
        #region Member Vars

        /// <summary>
        /// Notification called everytime a day has passed
        /// </summary>
        private Notification _dayPassedNotification;

        /// <summary>
        /// Tile for the crop
        /// </summary>
        private MobileGameTile _tile;

        /// <summary>
        /// The land the crop is on
        /// </summary>
        private Land _landOn;

        /// <summary>
        /// The crops roots may travel into other land.
        /// The traits of these land tiles will be made to match the traits of the land the crop is on.  That way we dont get a stange pattern of land with far different traits then neighboring land.
        /// </summary>
        private List<Land> _rootsLand = new List<Land>();

        /// <summary>
        /// Field the crop is in
        /// </summary>
        private Field _field;

        /// <summary>
        /// Info for the crop
        /// </summary>
        private CropInfo _cropInfo;
                
        /// <summary>
        /// Has the crop been picked this cycle
        /// </summary>
        private bool _pickedThisCycle = false;

        /// <summary>
        /// Traits that the crop has
        /// </summary>
        private TraitSet _traits;

        /// <summary>
        /// Quality for the crop
        /// </summary>
        private Quality _quality;

        /// <summary>
        /// Events that determine when the crop will die
        /// </summary>
        private ObjectEventSet _events;

        /// <summary>
        /// The texture manager for this crop
        /// </summary>
        private TextureManager _textureManager;
        
        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a crop.
        /// Setup or ReadState must be called after the crop is created.
        /// </summary>
        public Crop() : base()
        {
        }


        /// <summary>
        /// Create a crop on the passed land, as part of the passed planted area, using the passed crop info
        /// </summary>
        public void Setup(Land landOn, Field field, CropInfo cropInfo)
        {
            _landOn = landOn;
            _cropInfo = cropInfo;
            _field = field;
                        
            //location of the crop the will be the same as the land its on
            AddLocationOn(_landOn.LocationOn);

            //setup crop traits
            _traits = new TraitSet();
            _traits.Setup(cropInfo);
            _traits.Pair(landOn.Traits);

            //setup crop quality
            _quality = new Quality();
            _quality.Setup(_traits);
            
            //add the crop to the field (needs to be done after the crops quality is setup)
            _field.AddCrop(this);

            //setup crop events
            _events = new ObjectEventSet();
            _events.Setup(cropInfo, this);
            
            //setup texture manager. Set the correct texture of the tile based on crop traits
            _textureManager = new TextureManager();
            _textureManager.Setup(_cropInfo, this);   
                        
            //create day passed handler for the crop.
            //this is only needed if either there is going to be root land, or if it can be picked without being harvested
            if (_cropInfo.PickItem != null || _cropInfo.NeedsSpace)
            {
                _dayPassedNotification = Program.GameThread.Clock.RegisterNotification(DayPassed, 1.0, true);
            }

            //setup the list of land that contains the crops roots
            SetupRootsLand();

            //if this crop needs space then we want to try and not walk on it
            if (_cropInfo.NeedsSpace)
            {
                PathEffect = ObjectsEffectOnPath.DontWalk;
            }
            
            //create tile for the crop
            SetupTile();
                        
            //crops are never in a "placing" state so we are done with placement
            this.DoneWithPlacement();

            //update the tile
            UpdateTiles();

        }

        /// <summary>
        /// Create the tile for the crop
        /// </summary>
        private void SetupTile()
        {                        
            _tile = new MobileGameTile(this);
            if (_cropInfo.NeedsSpace == false)
            {
                //force tightly packed crop tiles to be behind worker tiles, and make it so they do not order with each other
                _tile.ForcedLayering = ForcedLayerType.Behind | ForcedLayerType.Same;
            }
            _textureManager.SetTileToUpdate(_tile);
        }

        /// <summary>
        /// We need to adjust the nutriants on surrounding tiles as well, or else the field will be an odd pattern of land tiles with all nutraints and those with no nutraitns
        /// and if the field gets converted to crops this will make for a strange harvest.  All land ortangonally or diagonaly adjancent that is not already adjacnet to a crop is considered
        /// the crops roots land.
        /// </summary>
        private void SetupRootsLand()
        {
            //if the crop does not need extra space then it does not have any root land
            if (_cropInfo.NeedsSpace == false)
            {
                return;
            }

            //collect all adjacent land to check if it already has a crop adajcent to it (it part of that crops root land)
            List<Land> landToCheck = new List<Land>();
            foreach (OrdinalDirection dir in DirectionUtils.AllOrdinalDirections)
            {
                landToCheck.Add(_landOn.GetAdjacent(dir));
                landToCheck.Add(_landOn.GetAdjacent(dir).GetAdjacent(DirectionUtils.ClockwiseOne(dir)));
            }

            //check if the land has any other adjacent crops
            foreach (Land land in landToCheck)
            {
                if (_field.OrderedLand.Contains(land) == false) { continue; }

                foreach (OrdinalDirection dir in DirectionUtils.AllOrdinalDirections)
                {
                    if (land.GetAdjacent(dir).LocationOn.Contains<Crop>()) { continue; }
                    if (land.GetAdjacent(dir).GetAdjacent(DirectionUtils.ClockwiseOne(dir)).LocationOn.Contains<Crop>()) { continue; }
                }
                _rootsLand.Add(land);
            }
            
        }
                
        /// <summary>
        /// Called when the crop is deleted
        /// </summary>
        protected override void DeleteInner()
        {
            _field.RemoveCrop(this);
            _tile.Delete();
            _traits.Delete();
            _quality.Delete();
            _events.Delete();
            _textureManager.Delete();
            if (_dayPassedNotification != null)
            {
                Program.GameThread.Clock.RemoveNotification(_dayPassedNotification);
            }
        }
        
        /// <summary>
        /// Called when the crop should have been deleted but it could not be deleted because there was an ongoing action that used it
        /// </summary>
        protected override void WaitingToDeleteInner()
        {
            _tile.Hidden = true;
            _tile.Update();
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Info for the crop
        /// </summary>
        public CropInfo CropInfo
        {
            get { return _cropInfo; }
        }

        /// <summary>
        /// Info for the crop
        /// </summary>
        public IInfo Info
        {
            get { return _cropInfo; }
        }

                
        /// <summary>
        /// Field the crop is in
        /// </summary>
        public Field Field
        {
            get { return _field; }
        }

        /// <summary>
        /// The quality for the crop
        /// </summary>
        public IQuality Quality
        {
            get { return _quality; }
        }

        /// <summary>
        /// The traits of the crop
        /// </summary>
        public TraitSet Traits
        {
            get { return _traits; }
        }
        
        /// <summary>
        /// Retrn true if the crop can be picked (it has not been picked yet this cycle, and its pick-trait is within the pick-range)
        /// </summary>
        public bool CanPick
        {
            get 
            {
                if (_pickedThisCycle) { return false; }
                return CropInPickingRange();                
            }
        }

        /// <summary>
        /// Retrn true if the crop has been picked this cycle
        /// </summary>
        public bool PickedThisCycle
        {
            get { return _pickedThisCycle; }
        }

        /// <summary>
        /// Action location for the crop
        /// </summary>
        public Location ActionLocation
        {
            get
            {
                if (_cropInfo.NeedsSpace)
                {
                    //for crops the need space we go to a location beside the crop, make sure that location is in the field
                    foreach (OrdinalDirection direction in DirectionUtils.AllOrdinalDirections)
                    {
                        Land landInDirection = _landOn.GetAdjacent(direction);
                        Field fieldForThatLand = landInDirection.LocationOn.Find<Field>();
                        if (fieldForThatLand == _field)
                        {
                            return landInDirection.LocationOn;
                        }
                    }

                    //if the crop was planted properly this should never happen
                    Debug.Assert(false);
                    return null;
                }
                else
                {
                    //for non space needing crops go right where the crop is
                    return _landOn.LocationOn;
                }
            }
        }

        /// <summary>
        /// The texture manager for this crop
        /// </summary>
        public TextureManager TextureManager
        {
            get { return _textureManager; }
        }

        #endregion

        #region Logic
                
        /// <summary>
        /// Update the crop tiles.
        /// </summary>
        public override void UpdateTiles()
        {
            //ignore if tile not yet created
            if (_tile == null) { return; }
                        
            //move the tile (incase the land it was on changed)
            _tile.MoveToLocation(LocationOn);

            //prepend with the lands height code
            _tile.Prepend = _landOn.HeightCode + "_";

            //have the texture manager redertermine the texture (which also causes the tile to update)
            _textureManager.Refresh();
        }

        /// <summary>
        /// Do the event passed
        /// </summary>
        public void ProcessEvent(ObjectEvent objectEvent)
        {
            if (objectEvent.EventInfo.EventType == ActionOrEventType.Die)
            {                
                //delete the crop
                this.Delete();
            }
            else
            {
                //we are supposed to do an event we dont know how to do
                Debug.Assert(false);
            }
        }
                                           
        /// <summary>
        /// Called when a day has passed
        /// </summary>
        private void DayPassed()
        {            
            //if the crop is NOT with-in picking range set picked this cycle back to false
            if (CropInPickingRange() == false)
            {                
                _pickedThisCycle = false;
            }

            //set traits of all land in the roots land list to be the same as the trait for the land the tree is on
            foreach (int rootLandTraitId in _landOn.Traits.TraitIds)
            {
                //except for SLOPE dont set that
                if (rootLandTraitId == SpecialTraits.SLOPE_TRAIT) { continue; }

                //get the id of the trait
                int traitValue = _landOn.Traits.GetTraitValue(rootLandTraitId);

                //set value for all other root land
                foreach (Land rootsLand in _rootsLand)
                {
                    //set the value to be the same as the main land
                    rootsLand.Traits.SetTraitValue(rootLandTraitId, traitValue);
                }
            }                                          
        }

        /// <summary>
        /// Return true if the pickable trait of the crop is within the picking range for the crop
        /// </summary>
        private bool CropInPickingRange()
        {
            int pickableTraitValue = _traits.GetTraitValue(_cropInfo.PickableTraitId);            
            return _cropInfo.PickableRange.IsInRange(pickableTraitValue);
        }
        
        /// <summary>
        /// Call when the crop is picked so the CanPicked crop can be set
        /// </summary>
        public void Picked()
        {
            //its possible we try to pick right after the crop got deleted (died), in that case just return.
            if (_placementState == FarmTycoon.PlacementState.Deleted) { return; }

            //the crop has been picked this cycle, will be set to false again when the next cycle starts
            _pickedThisCycle = true;


            //update tile to show its been picked
            UpdateTiles();
        }
        
        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteNotification(_dayPassedNotification);
            writer.WriteObject(_landOn);
            writer.WriteObjectList<Land>(_rootsLand);
            writer.WriteObject(_field);
            writer.WriteInfo(_cropInfo);
            writer.WriteBool(_pickedThisCycle);
            writer.WriteObject(_traits);
            writer.WriteObject(_quality);
            writer.WriteObject(_events);
            writer.WriteObject(_textureManager);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
			_dayPassedNotification = reader.ReadNotification(DayPassed);
			_landOn = reader.ReadObject<Land>();
			_rootsLand = reader.ReadObjectList<Land>();
			_field = reader.ReadObject<Field>();
			_cropInfo = reader.ReadInfo<CropInfo>();
			_pickedThisCycle = reader.ReadBool();
			_traits = reader.ReadObject<TraitSet>();
			_quality = reader.ReadObject<Quality>();
			_events = reader.ReadObject<ObjectEventSet>();
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
