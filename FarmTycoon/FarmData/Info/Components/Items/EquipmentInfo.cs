using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{
    
    /// <summary>
    /// Info on a type of equipment loaded form the config file
    /// </summary>
    public class EquipmentInfo : IInfo, ITraitsInfo, ITexturesInfo, IDelaysInfo
    {
        /// <summary>
        /// Prepended to the unique name of all EquipmentInfos
        /// </summary>
        public const string UNIQUE_PREPEND = "Equipment_";

        /// <summary>
        /// ItemTypeInfo for the equipment
        /// </summary>
        private ItemTypeInfo _itemTypeInfo = null;
                
        /// <summary>
        /// type of equipment this is
        /// </summary>
        private EquipmentType _equipmentType;
                
        /// <summary>
        /// inventory size multiplier for the equipment
        /// </summary>
        private double _inventorySizeMultiplier = 1.0;

        /// <summary>
        /// The traits that effect the quality of the animal
        /// </summary>
        private TraitInfoSet _traits;

        /// <summary>
        /// The textures this equipment can show
        /// </summary>
        private TexturesInfoSet _textures;

        /// <summary>
        /// Delays that determine how fast the equipmnet moves, and does actions
        /// </summary>
        private DelayInfoSet _delays;


        /// <summary>
        /// Return if the equipment is Vehicle
        /// </summary>
        public bool IsVehicle
        {
            get { return EquipmentTypeUtils.IsVehicle(_equipmentType); }
        }

        
        /// <summary>
        /// Create a Game Item type.  There should only be one instance for each type of item.
        /// GameItemType class should only be created by itemsDataFile.
        /// </summary>
        public EquipmentInfo(XmlReader reader, FarmData farmInfo)
        {
            reader.ReadToFollowing("Equipment");
            if (reader.MoveToAttribute("Item"))
            {
                _itemTypeInfo = reader.ReadContentAsItemTypeInfo(farmInfo);
            }
            if (reader.MoveToAttribute("EquipmentType"))
            {
                _equipmentType = (EquipmentType)Enum.Parse(typeof(EquipmentType), reader.ReadContentAsString());
            }
            if (reader.MoveToAttribute("InventoryMultiplier"))
            {
                _inventorySizeMultiplier = reader.ReadContentAsDouble();
            }
            
            _traits = new TraitInfoSet(this);
            _delays = new DelayInfoSet(this);
            _textures = new TexturesInfoSet(this);

            while (reader.ReadNextElement())
            {
                _traits.ReadElement(reader, farmInfo);
                _delays.ReadElement(reader, farmInfo);
                _textures.ReadElement(reader, farmInfo);                  
            }

            _traits.InitSet();
            _delays.InitSet();

        }

        /// <summary>
        /// Equipment has the sane name as its associated ItemTypeInfo
        /// </summary>
        public string Name
        {
            get { return _itemTypeInfo.Name; }
        }

        /// <summary>
        /// ItemTypeInfo for the equipment
        /// </summary>
        public ItemTypeInfo ItemTypeInfo
        {
            get { return _itemTypeInfo; }
        }


        /// <summary>
        /// kind of equipment this is
        /// </summary>
        public EquipmentType EquipmentType
        {
            get { return _equipmentType; }
        }
        
        /// <summary>
        /// inventory size multiplier for the equipment
        /// </summary>
        public double InventorySizeMultiplier
        {
            get { return _inventorySizeMultiplier; }
        }
        
        /// <summary>
        /// UniqueName for the IInfo object
        /// </summary>
        public string UniqueName
        {
            get { return UNIQUE_PREPEND + Name; }
        }

        /// <summary>
        /// The traits that effect the quality of the animal
        /// </summary>
        public TraitInfoSet Traits
        {
            get { return _traits; }
        }

        /// <summary>
        /// The textures this worker can show
        /// </summary>
        public TexturesInfoSet Textures
        {
            get { return _textures; }
        }

        /// <summary>
        /// Delays that determine how fast the equipmnet moves, and does actions
        /// </summary>
        public DelayInfoSet Delays
        {
            get { return _delays; }
        }




    }
}
