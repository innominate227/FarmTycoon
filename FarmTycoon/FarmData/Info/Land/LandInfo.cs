using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace FarmTycoon
{
    /// <summary>
    /// Land info object contains traits for a each peice of land
    /// </summary>
    public class LandInfo : IInfo, ITraitsInfo, ITexturesInfo
    {
        
        /// <summary>
        /// Unique name for the LandInfo.
        /// </summary>
        public const string UNIQUE_NAME = "Land_Land";

        /// <summary>
        /// Maximum allowed height for land
        /// </summary>
        private int _maxHeight = 20;

        /// <summary>
        /// Minimum allowed height for land
        /// </summary>
        private int _minHeight = 0;

        
        /// <summary>
        /// The traits that the land has
        /// </summary>
        private TraitInfoSet _traits;

        /// <summary>
        /// The textures this land can show
        /// </summary>
        private TexturesInfoSet _textures;



        public LandInfo(XmlReader reader, FarmData farmInfo)
        {
            reader.ReadToFollowing("Land");
            if (reader.MoveToAttribute("MaxHeight"))
            {
                _maxHeight = reader.ReadContentAsInt();
            }
            if (reader.MoveToAttribute("MinHeight"))
            {
                _minHeight = reader.ReadContentAsInt();
            }


            _traits = new TraitInfoSet(this);
            _textures = new TexturesInfoSet(this);

            while (reader.ReadNextElement())
            {
                _traits.ReadElement(reader, farmInfo);
                _textures.ReadElement(reader, farmInfo);
            }

            _traits.InitSet();
        }


        /// <summary>
        /// Maximum allowed height for land
        /// </summary>
        public int MaxHeight
        {
            get { return _maxHeight; }
        }

        /// <summary>
        /// Minimum allowed height for land
        /// </summary>
        public int MinHeight
        {
            get { return _minHeight; }
        }
        
        /// <summary>
        /// All the traits that the land has
        /// </summary>
        public TraitInfoSet Traits
        {
            get { return _traits; }
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
            get { return UNIQUE_NAME; }
        }

    }
}
