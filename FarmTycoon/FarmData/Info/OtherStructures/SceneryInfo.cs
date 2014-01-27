using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{
    public class SceneryInfo : IPloppableInfo, IInfo
    {
        /// <summary>
        /// Prepended to the unique name of all SceneryInfo
        /// </summary>
        public const string UNIQUE_PREPEND = "Scenery_";


        /// <summary>
        /// Name of the info object
        /// </summary>
        protected string _name = "";

        /// <summary>
        /// Texture for the building
        /// </summary>
        protected string _texture = "";
        
        /// <summary>
        /// Locations the object is on (relative to the center location)
        /// </summary>
        protected List<RelativeLocation> _landOn = new List<RelativeLocation>();



        public SceneryInfo(XmlReader reader)
        {
            reader.ReadToFollowing("Scenery");
            if (reader.MoveToAttribute("Name"))
            {
                _name = reader.ReadContentAsString();
            }
            if (reader.MoveToAttribute("Texture"))
            {
                _texture = reader.ReadContentAsString();
            }
            if (reader.MoveToAttribute("LandOn"))
            {
                _landOn = reader.ReadContentAsRelativeLocationList();
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
        /// Texture for the solid object
        /// </summary>
        public string Texture
        {
            get { return _texture; }
        }
        

        /// <summary>
        /// Locations the object is on (relative to the center location)
        /// </summary>
        public List<RelativeLocation> LandOn
        {
            get { return _landOn; }
        }


        public string UniqueName
        {
            get { return UNIQUE_PREPEND + Name; }
        }


        public PriceType PriceType
        {
            get { return PriceType.Scenery; }
        }
    }
}
