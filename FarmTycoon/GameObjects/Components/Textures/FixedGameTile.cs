using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Wraps a fixed tile.  Manages the texture for a tile whos texture is made by concatnation three strings Prepend + Texture + Append.    
    /// </summary>
    public class FixedGameTile : GameTile
    {
        
        /// <summary>
        /// Create a new FixedGameTile
        /// </summary>
        public FixedGameTile(GameObject obj, float x, float y, float z, int layer)
        {
            FixedTile fixedTile = Program.UserInterface.Graphics.NewFixedTile();
            fixedTile.Tag = this;
            fixedTile.X = x;
            fixedTile.Y = y;
            fixedTile.Z = z;
            fixedTile.Layer = layer;
            _tile = fixedTile;
            _gameObj = obj;            
        }


        /// <summary>
        /// Create a new FixedGameTile
        /// </summary>
        public FixedGameTile(GameObject obj, Location location, int layer) : this(obj, location.X, location.Y, location.Z, layer)
        {
        }
        


    }
}
