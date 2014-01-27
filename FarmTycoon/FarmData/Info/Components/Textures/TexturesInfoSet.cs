using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{
    public class TexturesInfoSet
    {
        /// <summary>
        /// The normal textures this game object can show
        /// </summary>
        private List<TextureInfo> _textures = new List<TextureInfo>();

        /// <summary>
        /// The temp-textures this game object can show while an action or event is done on it
        /// </summary>
        private Dictionary<ActionOrEventType, TempTextureInfo> _tempTextures = new Dictionary<ActionOrEventType, TempTextureInfo>();
                
        /// <summary>
        /// The info object that owns this textures info object
        /// </summary>
        private ITexturesInfo _infoSetOwner;
        

                
        /// <summary>
        /// Create a TexturesInfoSet, ReadElement and InitSet need to be called before used.
        /// </summary>        
        public TexturesInfoSet(ITexturesInfo infoSetOwner)
        {
            _infoSetOwner = infoSetOwner;
        }

        /// <summary>
        /// Read the element the xml reader is currently on, if the element is a trait add it to the trait info set
        /// </summary>        
        public void ReadElement(XmlReader reader, FarmData farmInfo)
        {
            if (reader.Name == "Texture")
            {
                TextureInfo textureInfo = new TextureInfo(_infoSetOwner.UniqueName, reader.ReadSubtree(), farmInfo);
                _textures.Add(textureInfo);
            }
            else if (reader.Name == "TempTexture")
            {
                TempTextureInfo tempTextureInfo = new TempTextureInfo(_infoSetOwner.UniqueName, reader.ReadSubtree());
                _tempTextures.Add(tempTextureInfo.ActionOrEvent, tempTextureInfo);
            } 

        }


        /// <summary>
        /// The info object that owns this textures info object
        /// </summary>
        public ITexturesInfo InfoSetOwner
        {
            get { return _infoSetOwner; }
        }

        /// <summary>
        /// The normal textures this game object can show
        /// </summary>
        public List<TextureInfo> Textures
        {
            get { return _textures; }
        }

        /// <summary>
        /// The temp-textures this game object can show while doing an action
        /// </summary>
        public Dictionary<ActionOrEventType, TempTextureInfo> TempTextures
        {
            get { return _tempTextures; }
        }



    }
}
