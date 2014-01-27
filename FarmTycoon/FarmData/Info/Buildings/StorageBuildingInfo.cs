using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{
    public class StorageBuildingInfo : IPloppableInfo, IInventoryInfo, IInfo, ITexturesInfo, IStorageBuildingInfo
    {
        /// <summary>
        /// Prepended to the unique name of all StorageBuildingInfo
        /// </summary>
        public const string UNIQUE_PREPEND = "StorageBuilding_";



        /// <summary>
        /// Location of the storage building where an action will take place
        /// </summary>
        private RelativeLocation _actionLocation = new RelativeLocation("C");
        
        /// <summary>
        /// Capacity of the inventory in the building
        /// </summary>
        private int _capacity = int.MaxValue;
        
        /// <summary>
        /// Types of items allowed in teh building
        /// </summary>
        private HashSet<ItemTypeInfo> _allowedTypes = new HashSet<ItemTypeInfo>();
        
        /// <summary>
        /// Locations the object is on (relative to the center location)
        /// </summary>
        protected List<RelativeLocation> _landOn = new List<RelativeLocation>();

        /// <summary>
        /// Name of the info object
        /// </summary>
        protected string _name = "";

        /// <summary>
        /// The textures this building can show
        /// </summary>
        private TexturesInfoSet _textures;
        
        /// <summary>
        /// Delay for when items are retrieved from the building
        /// </summary>
        private double _getDelayMultiplier = 1.0;

        /// <summary>
        /// Delay for when items are place into the building
        /// </summary>
        private double _putDelayMultiplier = 1.0;



        public StorageBuildingInfo(XmlReader reader, FarmData farmInfo)
        {
            reader.ReadToFollowing("StorageBuilding");
            if (reader.MoveToAttribute("Name"))
            {
                _name = reader.ReadContentAsString();
            }
            if (reader.MoveToAttribute("ActionLocation"))
            {
                _actionLocation = reader.ReadContentAsRelativeLocation();
            }
            if (reader.MoveToAttribute("LandOn"))
            {
                _landOn = reader.ReadContentAsRelativeLocationList();
            }
            if (reader.MoveToAttribute("Capacity"))
            {
                _capacity = reader.ReadContentAsInt();
            }
            if (reader.MoveToAttribute("GetDelayMultiplier"))
            {
                _getDelayMultiplier = reader.ReadContentAsInt();
            }
            if (reader.MoveToAttribute("PutDelayMultiplier"))
            {
                _putDelayMultiplier = reader.ReadContentAsInt();
            }

            _textures = new TexturesInfoSet(this);

            while (reader.ReadNextElement())
            {
                _textures.ReadElement(reader, farmInfo);

                if (reader.Name == "ItemTag")
                {
                    reader.MoveToAttribute("Name");
                    foreach (ItemTypeInfo itemTypeInfo in reader.ReadContentAsItemTypeInfosContainingTag(farmInfo))
                    {
                        _allowedTypes.Add(itemTypeInfo);                     
                    }
                }
                else if (reader.Name == "Item")
                {
                    reader.MoveToAttribute("Name");
                    _allowedTypes.Add(reader.ReadContentAsItemTypeInfo(farmInfo));
                }
            }

        }

        /// <summary>
        /// Name of the info object
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Locations the object is on (relative to the center location)
        /// </summary>
        public List<RelativeLocation> LandOn
        {
            get { return _landOn; }
        }


        /// <summary>
        /// Location of the storage building where an action will take place
        /// </summary>
        public RelativeLocation ActionLocation
        {
            get { return _actionLocation; }
        }
        
        /// <summary>
        /// Capacity of the inventory in the building
        /// </summary>
        public int Capacity
        {
            get { return _capacity; }
        }

        /// <summary>
        /// Types of items allowed in the building
        /// </summary>
        public HashSet<ItemTypeInfo> AllowedTypes
        {
            get { return _allowedTypes; }
        }


        public bool OneBaseTypeInventory
        {
            get { return false; }
        }

        /// <summary>
        /// The textures this building can show
        /// </summary>
        public TexturesInfoSet Textures
        {
            get { return _textures; }
        }


        /// <summary>
        /// Delay for when items are retrieved from the building
        /// </summary>
        public double GetDelayMultiplier
        {
            get { return _getDelayMultiplier; }
        }

        /// <summary>
        /// Delay for when items are place into the building
        /// </summary>
        public double PutDelayMultiplier
        {
            get { return _putDelayMultiplier; }
        }


        /// <summary>
        /// If true when items are added to this inventory they should be added to the global players items list
        /// </summary>
        public bool UpdatePlayerItemsList
        {
            get { return true; }
        }

        /// <summary>
        /// If true when items are added to this inventory they should be added to the global players sellable items list.
        /// </summary>
        public bool UpdatePlayerSellableItemsList
        {
            get { return true; }
        }


        public string UniqueName
        {
            get { return UNIQUE_PREPEND + _name; }
        }

        public PriceType PriceType
        {
            get { return PriceType.StorageBuilding; }
        }
    }
}
