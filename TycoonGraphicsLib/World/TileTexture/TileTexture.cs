using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TycoonGraphicsLib
{

    /// <summary>
    /// A texures for a tile.  This wraps a Texture object, and contains info about the how the texture will be rendered in the world.  
    /// It includes the height, and width of the texture in world units.  And the center X, and Y offset of the texture in world units.
    /// Also includes the number for the tile texture sheet that the texture belongs to.
    /// </summary>
    internal class TileTexture
    {
        /// <summary>
        /// The height of the texture in world units
        /// </summary>
        private float _height;

        /// <summary>
        /// The width of the texture in world units
        /// </summary>
        private float _width;
                
        /// <summary>
        /// Distance in world units from the bottom of the texture that is the "center" of the texture
        /// </summary>
        private float _centerYOffset;

        /// <summary>
        /// Distance in world units from the left of the texture that is the "center" of the texture
        /// </summary>
        private float _centerXOffset;
        
        /// <summary>
        /// The index for the tile texture sheet this texture is in.
        /// </summary>
        private int _tileTextureSheetIndex;

        

        /// <summary>
        /// The texture.
        /// </summary>
        private Texture _texture;


        /// <summary>
        /// Create a new TileTexture
        /// </summary>
        public TileTexture(Texture texture, float height, float width, float centerYOffset, float centerXOffset, int tileTextureSheetNumber)
        {
            _texture = texture;
            _height = height;
            _width = width;
            _centerYOffset = centerYOffset;
            _centerXOffset = centerXOffset;
            _tileTextureSheetIndex = tileTextureSheetNumber;
        }
        

        /// <summary>
        /// The texture.
        /// </summary>
        public Texture Texture
        {
            get { return _texture; }
        }

        /// <summary>
        /// The height of the texture in world units
        /// </summary>
        public float Height
        {
            get { return _height; }
        }

        /// <summary>
        /// The width of the texture in world units
        /// </summary>
        public float Width
        {
            get { return _width; }
        }
        
        /// <summary>
        /// Distance in world units from the bottom of the texture that is the "center" of the texture
        /// </summary>
        public float CenterYOffset
        {
            get { return _centerYOffset; }
        }
        
        /// <summary>
        /// Distance in world units from the left of the texture that is the "center" of the texture
        /// </summary>
        public float CenterXOffset
        {
            get { return _centerXOffset; }
        }

        /// <summary>
        /// The index for the tile texture sheet this texture is in.
        /// </summary>
        public int TileTextureSheetIndex
        {
            get { return _tileTextureSheetIndex; }
        }



    }
}

