using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TycoonGraphicsLib
{

    /// <summary>
    /// Settings dealing with the size of the world and how the tiles that make up the world are stored in video memory.
    /// These setting can not be changed without recreating the world object
    /// </summary>
    internal class WorldSettings
    {
        /// <summary>
        /// Value added when convert from game unit to world unit.  Based on the maximum allowed Z.  This offset keeps all world units positive.
        /// </summary>
        private int _worldUnitOffset;

        /// <summary>
        /// The maximum allowable Z value for a tile
        /// </summary>
        private int _maxZ;

        /// <summary>
        /// width, and height of the world (in game units)
        /// </summary>
        private int _gameSize;

        /// <summary>
        /// width, and height of the world (in world units).        
        /// </summary>
        private int _worldSize;

        /// <summary>
        /// Number of layers in the world
        /// </summary>
        private int _layers;
                
        /// <summary>
        /// Size of each segment that the world gets divided into (in world units).
        /// </summary>
        private int _segmentSize;
        		
		/// <summary>
		/// Path to the bitmap file of textures
		/// </summary>
        private string _texturesBitmapFile;

        /// <summary>
        /// Path to the file defineing texture regions
        /// </summary>
        private string _textureRegionsFile;

        /// <summary>
        /// Path to the file defineing texture quartets
        /// </summary>
        private string _textureQuartetsFile;

        /// <summary>
        /// Force the size of all textures to be 1024
        /// </summary>
        private bool _forceMinTextureSize;
        
        /// <summary>
        /// Create a new world settings object
        /// </summary>
        public WorldSettings(int gameSize, int maxZ, int layers, int segmentSize, string texturesBitmapFile, string textureRegionsFile, string textureQuartetsFile, bool forceMinTextureSize)
        {
            _layers = layers;
            _gameSize = gameSize;
            _maxZ = maxZ;            
            _worldUnitOffset = maxZ + 10;
            _worldSize = (gameSize * 2) + (_worldUnitOffset * 2);
            _segmentSize = segmentSize;
            _texturesBitmapFile = texturesBitmapFile;
            _textureRegionsFile = textureRegionsFile;
            _textureQuartetsFile = textureQuartetsFile;
            _forceMinTextureSize = forceMinTextureSize;
        }

        /// <summary>
        /// Value added when convert from game unit to world unit.  Based on the maximum allowed Z.  This offset keeps all world units positive.
        /// </summary>
        public int WorldUnitOffset
        {
            get { return _worldUnitOffset; }
        }

        /// <summary>
        /// The maximum allowable Z value for a tile
        /// </summary>
        public int MaxZ
        {
            get { return _maxZ; }
        }

        /// <summary>
        /// width, and height of the world (in game units)
        /// </summary>
        public int GameSize
        {
            get { return _gameSize; }
        }

        /// <summary>
        /// width, and height of the world (in world units).
        /// This will be 2*GameSize
        /// </summary>
        public int WorldSize
        {
            get { return _worldSize; }
        }

        /// <summary>
        /// Number of layers in the world
        /// </summary>
        public int Layers
        {
            get { return _layers; }        
        }

        /// <summary>
        /// Size of each segment that the world gets divided into (in world units).
        /// </summary>
        public int SegmentSize
        {
            get { return _segmentSize; }
        }

        /// <summary>
        /// Path to the bitmap file of textures
		/// </summary>
        public string TexturesBitmapFile
		{
            get { return _texturesBitmapFile; }
		}

        /// <summary>
        /// Path to the file defineing texture regions
        /// </summary>
        public string TextureRegionsFile
        {
            get { return _textureRegionsFile; }
        }

        /// <summary>
        /// Path to the file defineing texture quartets
        /// </summary>
        public string TextureQuartetsFile
        {
            get { return _textureQuartetsFile; }
        }
        
        /// <summary>
        /// Force the size of all textures to be 1024
        /// </summary>
        public bool ForceMinTextureSize
        {
            get { return _forceMinTextureSize; }
        }
        
        /// <summary>
        /// Get the number of segments each row/column of the world is divided into
        /// </summary>
        public int SegmentDivisions
        {
            get
            {
                //calculate how many segments the row/cols can be divided into
                int segmentsDivisions = _worldSize / _segmentSize;

                //if not divided evenly add one segment
                if (_worldSize % _segmentSize != 0)
                {
                    segmentsDivisions++;
                }
                return segmentsDivisions;
            }
        }


    }
}
