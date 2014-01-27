using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace FarmTycoon
{
    public class DeliveryAreaInfo : IPloppableInfo, IInventoryInfo, IInfo, ITexturesInfo, IStorageBuildingInfo
    {
        /// <summary>
        /// Unique name for the delivery area info object.
        /// </summary>
        public const string UNIQUE_NAME = "Delivery_Delivery";

        /// <summary>
        /// Location of the delivery area where an action will take place
        /// </summary>
        private RelativeLocation _actionLocation = new RelativeLocation("C");

        /// <summary>
        /// Height of the building
        /// </summary>
        protected int _height = 0;

        /// <summary>
        /// Locations the object is on (relative to the center location)
        /// </summary>
        protected List<RelativeLocation> _landOn = new List<RelativeLocation>();

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
		



        public DeliveryAreaInfo(XmlReader reader, FarmData farmInfo)
        {
            reader.ReadToFollowing("DeliveryArea");    
            if (reader.MoveToAttribute("LandOn"))
            {
                _landOn = reader.ReadContentAsRelativeLocationList();
            }
            if (reader.MoveToAttribute("ActionLocation"))
            {
                _actionLocation = reader.ReadContentAsRelativeLocation();
            }
            if (reader.MoveToAttribute("GetDelayMultiplier"))
            {
                _getDelayMultiplier = reader.ReadContentAsInt();
            }
            if (reader.MoveToAttribute("PutDelayMultiplier"))
            {
                _putDelayMultiplier = reader.ReadContentAsInt();
            }

            //read in the textures for the building
            _textures = new TexturesInfoSet(this);
            while (reader.ReadNextElement())
            {                
                _textures.ReadElement(reader, farmInfo);
            }
        }


        /// <summary>
        /// Location of the storage building where an action will take place
        /// </summary>
        public RelativeLocation ActionLocation
        {
            get { return _actionLocation; }
        }


        /// <summary>
        /// The textures this building can show
        /// </summary>
        public TexturesInfoSet Textures
        {
            get { return _textures; }
        }


        /// <summary>
        /// Locations the object is on (relative to the center location)
        /// </summary>
        public List<RelativeLocation> LandOn
        {
            get { return _landOn; }
        }
        
        /// <summary>
        /// Capacity of the inventory in the building
        /// </summary>
        public int Capacity
        {
            get { return int.MaxValue; }
        }

        /// <summary>
        /// Types of items allowed in the building 
        /// A value of null indicates all types are allowed
        /// </summary>
        public HashSet<ItemTypeInfo> AllowedTypes
        {
            get { return null; }
        }


        public bool OneBaseTypeInventory
        {
            get { return false; }
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

        public string UniqueName
        {
            get { return UNIQUE_NAME; }
        }

        public string Name
        {
            get { return "Delivery Area"; }
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
            get { return false ; }
        }


        public PriceType PriceType
        {            
            get 
            {
                //this should not matter since the the delivery area is placed in scenario edit mode, and will be free                
                return PriceType.StorageBuilding; 
            }
        }
    }
}
