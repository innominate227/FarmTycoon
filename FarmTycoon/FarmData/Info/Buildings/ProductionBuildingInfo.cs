using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{
    public class ProductionBuildingInfo : IPloppableInfo, IInventoryInfo, IInfo, ITexturesInfo, IStorageBuildingInfo
    {
        /// <summary>
        /// Prepended to the unique name of all ProductionBuildingInfo
        /// </summary>
        public const string UNIQUE_PREPEND = "ProductionBuilding_";

        /// <summary>
        /// Location of the storage building where an action will take place
        /// </summary>
        private RelativeLocation _actionLocation = new RelativeLocation("C");
                
        /// <summary>
        /// Capacity of the inventory in the building
        /// </summary>
        private int _capacity = int.MaxValue;

        /// <summary>
        /// interval at wich the building produces
        /// </summary>
        private double _interval = 1.0;

        /// <summary>
        /// How many works can work at the building.
        /// Building will produce at an interval of Interval*(Number of workers / max workers).
        /// Unless Max workers is 0 in wich case it will always produce at interval
        /// </summary>
        private int _maxWorkers = 0;
                
        /// <summary>
        /// inputs to the production building
        /// </summary>
        private Dictionary<ItemTypeInfo, int> _inputs = new Dictionary<ItemTypeInfo, int>();
        
        /// <summary>
        /// outputs from the production building
        /// </summary>
        private Dictionary<ItemTypeInfo, int> _outputs = new Dictionary<ItemTypeInfo, int>();

        /// <summary>
        /// Types of items allowed in the building
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
		
		



        public ProductionBuildingInfo(XmlReader reader, FarmData farmInfo)
        {
            reader.ReadToFollowing("ProductionBuilding");
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
            if (reader.MoveToAttribute("Interval"))
            {
                _interval = reader.ReadContentAsDouble();
            }
            if (reader.MoveToAttribute("MaxWorkers"))
            {
                _maxWorkers = reader.ReadContentAsInt();
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

                if (reader.Name == "Input")
                {
                    reader.MoveToAttribute("Name");
                    ItemTypeInfo itemType = reader.ReadContentAsItemTypeInfo(farmInfo);
                    reader.MoveToAttribute("Amount");
                    int amount = reader.ReadContentAsInt();
                    _inputs.Add(itemType, amount);
                    _allowedTypes.Add(itemType);
                }
                else if (reader.Name == "Output")
                {
                    reader.MoveToAttribute("Name");
                    ItemTypeInfo itemType = reader.ReadContentAsItemTypeInfo(farmInfo);
                    reader.MoveToAttribute("Amount");
                    int amount = reader.ReadContentAsInt();
                    _outputs.Add(itemType, amount);
                    _allowedTypes.Add(itemType);
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
        /// interval at wich the building produces
        /// </summary>
        public double Interval
        {
            get { return _interval; }
        }
        
        /// <summary>
        /// How many works can work at the building.
        /// Building will produce at an interval of Interval*(Number of workers / max workers).
        /// Unless Max workers is 0 in wich case it will always produce at interval
        /// </summary>
        public int MaxWorkers
        {
            get { return _maxWorkers; }
        }

        /// <summary>
        /// inputs to the production building
        /// </summary>
        public Dictionary<ItemTypeInfo, int> Inputs
        {
            get { return _inputs; }
        }

        /// <summary>
        /// outputs from the production building
        /// </summary>
        public Dictionary<ItemTypeInfo, int> Outputs
        {
            get { return _outputs; }
        }

        /// <summary>
        /// Types of items allowed in the building
        /// A value of null indicates all types are allowed
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
            get { return false; }
        }


        public string UniqueName
        {
            get { return UNIQUE_PREPEND + _name; }
        }

        public PriceType PriceType
        {
            get { return PriceType.ProductionBuilding; }
        }
    }
}
