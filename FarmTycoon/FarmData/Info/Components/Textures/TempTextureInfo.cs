using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace FarmTycoon
{
    /// <summary>
    /// Info on a Texture that is shown temporaily during an action
    /// </summary>
    public class TempTextureInfo : TextureInfoBase, IInfo
    {
        /// <summary>
        /// Prepended to the unique name of all TempTextureInfo
        /// </summary>
        public const string UNIQUE_PREPEND = "TempTexture_";


        /// <summary>
        /// Name of the object that has this texture + the name of the texture
        /// </summary>
        private string _fullName = "";

        /// <summary>
        /// Name of the tenp texture
        /// </summary>
        private string _name = "";


        /// <summary>
        /// Name of the action (or event) that causes the texture to be seen
        /// </summary>
        private ActionOrEventType _actionOrEvent;

                

        public TempTextureInfo(string parentName, XmlReader reader)
        {            
            reader.ReadToFollowing("TempTexture");            

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
            if (reader.MoveToAttribute("Event"))
            {
                string eventString = reader.ReadContentAsString();
                _actionOrEvent = (ActionOrEventType)Enum.Parse(typeof(ActionOrEventType), eventString);
            }
            if (reader.MoveToAttribute("Action"))
            {
                string eventString = reader.ReadContentAsString();
                _actionOrEvent = (ActionOrEventType)Enum.Parse(typeof(ActionOrEventType), eventString);
            }       
        }
        
        /// <summary>
        /// Name of the tenp texture
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Name of the texture to apply
        /// </summary>
        public string Texture
        {
            get { return _texture; }
        }

        /// <summary>
        /// The action or event that causes the texture to be seen
        /// </summary>
        public ActionOrEventType ActionOrEvent
        {
            get { return _actionOrEvent; }
        }
        

        public string UniqueName
        {
            get { return UNIQUE_PREPEND + _fullName; }
        }

    }
}
