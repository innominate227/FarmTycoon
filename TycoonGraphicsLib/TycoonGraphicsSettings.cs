using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TycoonGraphicsLib
{
    
    /// <summary>
    /// All the settings passed to the TycoonGraphics constructor
    /// </summary>
    public class TycoonGraphicsSettings
    {
        private string _windowsCommonTextureBitmapFile;
        private string _windowsCommonTextureRegionsFile;
        
        private string _worldTexturesBitmapFile;
        private string _worldTexturesRegionsFile;
        private string _worldTexturesQuartetsFile;
        
        private int _maxZ;

        private int _gameSize;
        private int _segmentLenght;
        private int _layers;

        private bool _forceMinTextureSize;


        /// <summary>
        /// File containing icons and string textures used by all windows
        /// </summary>
        public string WindowIconsBitmapFile
        {
            get { return _windowsCommonTextureBitmapFile; }
            set { _windowsCommonTextureBitmapFile = value; }
        }
        
        /// <summary>
        /// File containing the locations of the texture in the WindowIconsBitmapFile
        /// </summary>
        public string WindowIconsRegionsFile
        {
            get { return _windowsCommonTextureRegionsFile; }
            set { _windowsCommonTextureRegionsFile = value; }
        }



        /// <summary>
        /// File containing textures used in the game world
        /// </summary>
        public string TextureBitmapFile
        {
            get { return _worldTexturesBitmapFile; }
            set { _worldTexturesBitmapFile = value; }
        }

        /// <summary>
        /// File containing the locations of the textures in the TextureBitmapFile
        /// </summary>
        public string TextureRegionsFile
        {
            get { return _worldTexturesRegionsFile; }
            set { _worldTexturesRegionsFile = value; }
        }

        /// <summary>
        /// File containing groups of four textures, which specifies which texture to use based on if the world is being viewed from the North, South, East or West
        /// </summary>
        public string TextureQuartetsFile
        {
            get { return _worldTexturesQuartetsFile; }
            set { _worldTexturesQuartetsFile = value; }
        }

        /// <summary>
        /// Maximum allowed Z value for any tile
        /// </summary>
        public int MaxZ
        {
            get { return _maxZ; }
            set { _maxZ = value; }
        }

        /// <summary>
        /// Size of the game world in game units
        /// </summary>
        public int GameSize
        {
            get { return _gameSize; }
            set { _gameSize = value; }
        }
        
        /// <summary>
        /// The game world is divided into segments, and only visible segments are rendered.  This is the size of the segments.
        /// </summary>
        public int SegmentLenght
        {
            get { return _segmentLenght; }
            set { _segmentLenght = value; }
        }

        /// <summary>
        /// This is the maximum number of layers that can exsist in the game world.
        /// </summary>
        public int Layers
        {
            get { return _layers; }
            set { _layers = value; }
        }

        /// <summary>
        /// If this is set to true the game world textures will be broken up into 1024x1024 size textures sheets regardless of what the graphics hardware claims it can support
        /// </summary>
        public bool ForceMinTextureSize
        {
            get { return _forceMinTextureSize; }
            set { _forceMinTextureSize = value; }
        }
        
    }
}
