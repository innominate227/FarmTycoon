using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// A peice of farm equipment (Tractor, Plow etc)
    /// The actually rendering, and moving of the equipment in the game world is handeled by the Worker object.
    /// This class tracks the traits and quality for a peice of equipment, it manages the delay multiplies for the equipment.
    /// Also the worker will use the Equipments TextureManager to update the workers texture while the worker is on equipment.
    /// </summary>
    public class Equipment : GameObject, ISavable, IHasDelays, IHasQuality, IHasTraits
    {
        #region Member Vars
                
        /// <summary>
        /// Traits for the equipment
        /// </summary>
        private TraitSet _traits;

        /// <summary>
        /// Quality for the equipment
        /// </summary>
        private Quality _quality;
        
        /// <summary>
        /// The texture manager for the equipment.  The worker uses this texture manager instead of its own when it is using equipment.
        /// </summary>
        private TextureManager _textureManager;

        /// <summary>
        /// Info for the equipment
        /// </summary>
        private EquipmentInfo _equipmentInfo;

        /// <summary>
        /// The item for this peice of equipment
        /// </summary>
        private ItemType _itemType;

        /// <summary>
        /// Delays for the equipment
        /// </summary>
        private DelaySet _delays;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create an Equipment.
        /// Setup or ReadState must be called after created.
        /// </summary>
        public Equipment() : base() { }
        
        /// <summary>
        /// Create a peice of equipment
        /// </summary>
        public void Setup(EquipmentInfo equipmentInfo, ItemType equipmentItem)
        {
            _equipmentInfo = equipmentInfo;
            _itemType = equipmentItem;
            _itemType.ItemObject = this;

            //setup the traits
            _traits = new TraitSet();
            _traits.Setup(equipmentInfo);

            //set up quality
            _quality = new Quality();
            _quality.Setup(_traits, _itemType);        
    
            //setup delays
            _delays = new DelaySet();
            _delays.Setup(equipmentInfo, this);

            //setup texture manager
            _textureManager = new TextureManager();
            _textureManager.Setup(_equipmentInfo, this);
            
            //equipment is never placed.            
            base.DoneWithPlacement();

            //we dont bother updating the LocationOn for the equipment GameObject.
            //it is going to always be the same location as a worker, or a Building.
            //So for instance if we are building something there is no need to check if there is Equipment
            //blocking us from building in that location because checking for a worker is sufficent.
        }

        /// <summary>
        /// Called when the equipment is deleted
        /// </summary>
        protected override void  DeleteInner()
        {
            _quality.Delete();
            _textureManager.Delete();            
        }

        #endregion

        #region Properties

        /// <summary>
        /// The texture manager for the equipment.  The worker uses this texture manager instead of its own when it is using equipment.
        /// </summary>
        public TextureManager TextureManager
        {
            get { return _textureManager; }
        }

        /// <summary>
        /// Info for the equipment
        /// </summary>
        public EquipmentInfo EquipmentInfo
        {
            get { return _equipmentInfo; }
        }

        /// <summary>
        /// The item for this peice of equipment
        /// </summary>
        public ItemType ItemType
        {
            get { return _itemType; }
        }

        /// <summary>
        /// Delays for the equipment
        /// </summary>
        public DelaySet Delays
        {
            get { return _delays; }
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

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteObject(_traits);
            writer.WriteObject(_quality);
            writer.WriteObject(_textureManager);
            writer.WriteInfo(_equipmentInfo);
            writer.WriteObject(_itemType);
            writer.WriteObject(_delays);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _traits = reader.ReadObject<TraitSet>();
            _quality = reader.ReadObject<Quality>();
            _textureManager = reader.ReadObject<TextureManager>();
            _equipmentInfo = reader.ReadInfo<EquipmentInfo>();
            _itemType = reader.ReadObject<ItemType>();
            _delays = reader.ReadObject<DelaySet>();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();

            //if equipment is no longer valid delte it
            if (_equipmentInfo == null)
            {
                Delete();
            }
        }
        #endregion

    }
}

