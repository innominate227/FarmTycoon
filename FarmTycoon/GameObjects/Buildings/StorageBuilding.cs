using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{

    /// <summary>
    /// Storage building holds items.
    /// Different storage building can hold different amounts, and types of items
    /// </summary>
    public class StorageBuilding : GameObject, IStorageBuilding, IHasTextureManager, IHasInventory, IHasInfo, IHasActionLocation
    {
        #region Member Vars

        /// <summary>
        /// BuildingInfo for this building
        /// </summary>
        private StorageBuildingInfo _buildingInfo;
        
        /// <summary>
        /// The inventory of the building
        /// </summary>
        private Inventory _inventory;
        
        /// <summary>
        /// The tile for the building
        /// </summary>
        private MobileGameTile _tile;

        /// <summary>
        /// Texture manager for the delivery area
        /// </summary>
        private TextureManager _textureManager;

        #endregion
        
        #region Setup

        /// <summary>
        /// Create a new StorageBuilding
        /// Setup or ReadState must be called after creation.
        /// </summary>
        public StorageBuilding() : base() { }
        
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
        /// Setup the Storage Building
        /// </summary>
        public void Setup(Location centerLocation, StorageBuildingInfo buildingInfo)
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

            //setup the tile for the building
            SetupTile();

            //the object can not be walked through
            PathEffect = ObjectsEffectOnPath.Solid;

            //update the tile
            UpdateTiles();
        }


        /// <summary>
        /// Called when the storage buidling is delted
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
        /// BuildingInfo for this building
        /// </summary>        
        public StorageBuildingInfo BuildingInfo
        {
            get { return _buildingInfo; }
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

        #endregion

        #region Logic

        /// <summary>
        /// Update the tile for the storage buidling
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
            writer.WriteObject(_textureManager);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _buildingInfo = reader.ReadInfo<StorageBuildingInfo>();
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
