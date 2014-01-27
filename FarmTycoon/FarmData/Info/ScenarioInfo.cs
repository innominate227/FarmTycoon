using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace FarmTycoon
{
    /// <summary>
    /// Info about the scnario the player is playing
    /// </summary>
    public partial class ScenarioInfo : IInfo
    {
        /// <summary>
        /// Unique name for the LandInfo.
        /// </summary>
        public const string UNIQUE_NAME = "ScenarioInfo_ScenarioInfo";


        /// <summary>
        /// Scenario name
        /// </summary>        
        private string _name;
        
        /// <summary>
        /// Scenario description
        /// </summary>
        private string _description;
        
        /// <summary>
        /// Scenario objective
        /// </summary>
        private string _objective;

        /// <summary>
        /// The folder containing the textures the scenario uses
        /// </summary>
        private string _textures;


        public ScenarioInfo(XmlReader reader)
        {
            reader.ReadToFollowing("Scenario");
            reader.ReadToFollowing("Name");
            _name = reader.ReadInnerXml().Trim();
            reader.ReadToFollowing("Descirption");
            _description = reader.ReadInnerXml().Trim();
            reader.ReadToFollowing("Objective");
            _objective = reader.ReadInnerXml().Trim();
            reader.ReadToFollowing("Textures");
            _textures = reader.ReadInnerXml().Trim();
        }



        /// <summary>
        /// Scenario name
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Scenario description
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Scenario objective
        /// </summary>
        public string Objective
        {
            get { return _objective; }
        }

        /// <summary>
        /// The folder containing the textures the scenario uses
        /// </summary>
        public string Textures
        {
            get { return _textures; }
        }


        public string UniqueName
        {
            get { return UNIQUE_NAME; }
        }
    }
}
