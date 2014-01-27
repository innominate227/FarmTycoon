using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    
    /// <summary>
    /// Trough hold food or water for an animal,
    /// It is like a storage building, with a few difference:
    /// Workers can not remove items from it (only Animals), it can only be built in a pasture.
    /// Animals look for Troughs when trying to do a Consume event.
    /// This class is very similar to StorageBuilding, but other class will interact with a Trough differently than a StorageBuilding.
    /// </summary>
    public partial class Trough : GameObject, IStorageBuilding, IHasTextureManager, IHasInventory, IHasInfo, IHasActionLocation
    {
        #region Member Vars

        /// <summary>
        /// TroughInfo for this Trough
        /// </summary>
        private TroughInfo _troughInfo;

        /// <summary>
        /// The inventory of the building
        /// </summary>
        private Inventory _inventory;
        
        /// <summary>
        /// The tile for the trough
        /// </summary>
        private MobileGameTile _tile;

        /// <summary>
        /// Texture manager for the delivery area
        /// </summary>
        private TextureManager _textureManager;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a new trough
        /// Setup or ReadState must be called after creation.
        /// </summary>
        public Trough() : base() { }
        
        /// <summary>
        /// Setup the tile for the troughs
        /// </summary>
        private void SetupTile()
        {
            //create the tile
            _tile = new MobileGameTile(this);
            _textureManager.SetTileToUpdate(_tile);        
        }

        /// <summary>
        /// Setup the trough
        /// </summary>
        public void Setup(Location centerLocation, TroughInfo troughInfo)
        {
            _troughInfo = troughInfo;

            //create inventory for the trough
            _inventory = new Inventory();
            _inventory.SetUp(troughInfo);

            //add the trough to the locations it ocupies
            AddLocationsOn(LocationUtils.GetLocationList(centerLocation, troughInfo.LandOn));
            
            //setup texture manager
            _textureManager = new TextureManager();
            _textureManager.Setup(troughInfo, this);

            //setup tle for the troughs
            SetupTile();

            //try and not walk through troughs
            PathEffect = ObjectsEffectOnPath.DontWalk;

            //update the tile
            UpdateTiles();
        }


        /// <summary>
        /// Called when the trough is deleted
        /// </summary>
        protected override void DeleteInner()
        {
            base.DeleteInner();
            _inventory.Delete();
            _textureManager.Delete();
            _tile.Delete();
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// The location a worker/animal should travel to in order to perform an action on this trough
        /// </summary>
        public Location ActionLocation
        {
            get { return _troughInfo.ActionLocation.GetRealtiveLocation(LocationOn); }
        }

        /// <summary>
        /// The inventory of the trough
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
        /// TroughInfo for this Trough
        /// </summary>        
        public TroughInfo TroughInfo
        {
            get { return _troughInfo; }
        }

        /// <summary>
        /// StorageBuildingInfo for this Trough
        /// </summary>        
        public IStorageBuildingInfo StorageBuildingInfo
        {
            get { return _troughInfo; }
        }
        
        /// <summary>
        /// Info for this Trough
        /// </summary>        
        public IInfo Info
        {
            get { return _troughInfo; }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Update the tile for the trough
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
            writer.WriteInfo(_troughInfo);
            writer.WriteObject(_inventory);
            writer.WriteObject(_textureManager);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _troughInfo = reader.ReadInfo<TroughInfo>();
            _inventory = reader.ReadObject<Inventory>();
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
