using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace FarmTycoon
{
    /// <summary>
    /// Info on a Texture
    /// </summary>
    public class TextureInfo : TextureInfoBase, IInfo
    {
        /// <summary>
        /// Prepended to the unique name of all TextureInfo
        /// </summary>
        public const string UNIQUE_PREPEND = "Texture_";


        /// <summary>
        /// Name of the object that has this texture + the name of the texture
        /// </summary>
        private string _fullName = "";

        /// <summary>
        /// Name of the texture info
        /// </summary>
        private string _name = "";

        
        /// <summary>
        /// Conditions that should be met for the texture to be applied
        /// </summary>
        private List<ConditionInfo> _conditions = new List<ConditionInfo>();



        public TextureInfo(string parentName, XmlReader reader, FarmData farmInfo)
        {            
            reader.ReadToFollowing("Texture");            

            //read trait of the event
            if (reader.MoveToAttribute("Name"))
            {
                _name = reader.ReadContentAsString();
                _fullName = parentName + "_" + _name;
            }
            if (reader.MoveToAttribute("Texture"))
            {
                _texture = reader.ReadContentAsString();
                if (_name == "")
                {
                    //name is same as texture if not specified
                    _name = _texture;
                    _fullName = parentName + "_" + _name;
                }
            }
            if (reader.MoveToAttribute("Frames"))
            {
                _frames = reader.ReadContentAsInt();
            }
            if (reader.MoveToAttribute("FrameRate"))
            {
                _frameRate = reader.ReadContentAsDouble();
            }      
                                    
            while (reader.ReadNextElement())
            {
                if (reader.Name == "Condition")
                {
                    ConditionInfo condition = ConditionInfo.ReadCondition(reader, farmInfo);
                    _conditions.Add(condition);
                }
            }
        }
                

        /// <summary>
        /// Name of the texture info
        /// </summary>
        public string Name
        {
            get { return _name; }
        }


        
        /// <summary>
        /// Conditions that should be met for the event to take place
        /// </summary>
        public List<ConditionInfo> Conditions
        {
            get { return _conditions; }
        }



        public string UniqueName
        {
            get { return UNIQUE_PREPEND + _fullName; }
        }

    }
}
