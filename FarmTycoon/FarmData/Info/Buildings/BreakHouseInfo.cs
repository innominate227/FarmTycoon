using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{
    public class BreakHouseInfo : IPloppableInfo, IInfo, ITexturesInfo
    {
        /// <summary>
        /// Prepended to the unique name of all BreakHouseInfo
        /// </summary>
        public const string UNIQUE_PREPEND = "BreakHouse_";

        /// <summary>
        /// Location of the storage building where an action will take place
        /// </summary>
        private RelativeLocation _actionLocation = new RelativeLocation("C");

        /// <summary>
        /// Capacity of the the breakhouse in number of workers
        /// </summary>
        private int _capacity = int.MaxValue;

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


        public BreakHouseInfo(XmlReader reader, FarmData farmInfo)
        {
            reader.ReadToFollowing("BreakHouse");
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
            
            _textures = new TexturesInfoSet(this);
            while (reader.ReadNextElement())
            {
                _textures.ReadElement(reader, farmInfo);
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
        /// The textures this building can show
        /// </summary>
        public TexturesInfoSet Textures
        {
            get { return _textures; }
        }
        
        public string UniqueName
        {
            get { return UNIQUE_PREPEND + _name; }
        }


        public PriceType PriceType
        {
            get { return PriceType.BreakHouse; }
        }
    }
}
