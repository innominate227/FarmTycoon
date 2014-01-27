using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{   

    /// <summary>
    /// The delivery area.  Items appear in this building when the player purchases them.
    /// There should be one and exactly one DeliveryArea in the Game.
    /// </summary>
    public class DeliveryArea : GameObject, IStorageBuilding, IHasInventory, IHasTextureManager, IHasInfo, IHasActionLocation
    {
        #region Member Vars

        /// <summary>
        /// The inventory of the delivery area.
        /// This is items that have been purchased and delivered, but not yet picked up by a worker.
        /// </summary>
        private Inventory _inventory;
        
        /// <summary>
        /// The tile for the delivery area
        /// </summary>
        private MobileGameTile _tile;

        /// <summary>
        /// Texture manager for the delivery area
        /// </summary>
        private TextureManager _textureManager;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a new delivery area
        /// Setup or ReadState must be called after the building is created.
        /// </summary>
        public DeliveryArea() : base() { }

        /// <summary>
        /// Setup the tile for the building
        /// </summary>
        private void SetupTile()
        {            
            //create the delivery area tile            
            _tile = new MobileGameTile(this);
            _textureManager.SetTileToUpdate(_tile);
        }

        /// <summary>
        /// Setup the delivery area
        /// </summary>
        public void Setup(Location centerLocation)
        {
            //make sure there is not already a delivery area
            Debug.Assert(GameState.Current.MasterObjectList.Contains<DeliveryArea>() == false);

            //get info for the delivery area
            DeliveryAreaInfo deliveryAreaInfo = FarmData.Current.DeliveryAreaInfo;

            //setup inventory
            _inventory = new Inventory();
            _inventory.SetUp(deliveryAreaInfo);
            _inventory.UnderlyingList.ItemAdded += new Action<ItemType>(ItemAddedToInventory);

            //add the delivery area to the locations it ocupies
            AddLocationsOn(LocationUtils.GetLocationList(centerLocation, deliveryAreaInfo.LandOn));
                        
            //setup texture manager
            _textureManager = new TextureManager();
            _textureManager.Setup(deliveryAreaInfo, this);

            //create the tile for the building
            SetupTile();

            //do not allow walking through delivery area
            PathEffect = ObjectsEffectOnPath.Solid;

            //update the tile
            UpdateTiles();
        }
        
        /// <summary>
        /// Called when the delivery area is being deleted
        /// </summary>
        protected override void DeleteInner()
        {
            _tile.Delete();
            _inventory.Delete();
            _textureManager.Delete();
        }
        
        #endregion

        #region Properties
        
        /// <summary>
        /// The location a worker should travel to in order to perform an action at the delivery area
        /// </summary>
        public Location ActionLocation
        {
            get { return FarmData.Current.DeliveryAreaInfo.ActionLocation.GetRealtiveLocation(LocationOn); }            
        }

        /// <summary>
        /// The inventory of the delivery area
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
        /// StorageBuildingInfo for this delivery area
        /// </summary>        
        public IStorageBuildingInfo StorageBuildingInfo
        {
            get { return FarmData.Current.DeliveryAreaInfo; }
        }

        /// <summary>
        /// Info for this delivery area
        /// </summary>        
        public IInfo Info
        {
            get { return FarmData.Current.DeliveryAreaInfo; }
        }


        #endregion

        #region Logic

        /// <summary>
        /// called when an new item is added to the Delivery Areas Inventory
        /// </summary>
        private void ItemAddedToInventory(ItemType itemType)
        {
            //for items with associated object the object should be created now
            itemType.CreateAssociatedObject(ActionLocation);
        }
        
        /// <summary>
        /// Update the tile for the delivery area
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
            writer.WriteObject(_inventory);
            writer.WriteObject(_textureManager);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _inventory = reader.ReadObject<Inventory>();
            _textureManager = reader.ReadObject<TextureManager>();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
            _inventory.UnderlyingList.ItemAdded += new Action<ItemType>(ItemAddedToInventory);
            SetupTile();
        }
        #endregion

    }
}
