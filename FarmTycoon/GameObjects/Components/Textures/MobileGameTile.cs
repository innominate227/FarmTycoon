using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Wraps a mobile tile.  Not mobile does not nessisarily mean moves in posisiton (it may move only its layer)
    /// Manages the texture for a tile whos texture is made by concatnation three strings Prepend + Texture + Append.    
    /// </summary>
    public class MobileGameTile : GameTile
    {
        
        /// <summary>
        /// Create a new MobileGameTile
        /// </summary>
        public MobileGameTile(GameObject obj, float x, float y, float z)
        {
            MobileTile mobileTile = Program.UserInterface.Graphics.NewMobileTile();
            mobileTile.Tag = this;
            mobileTile.X = x;
            mobileTile.Y = y;
            mobileTile.Z = z;
            _tile = mobileTile;
            _gameObj = obj;  
        }
        
        /// <summary>
        /// Create a new MobileGameTile
        /// </summary>
        public MobileGameTile(GameObject obj, Location location)
            : this(obj, location.X, location.Y, location.Z)
        {
        }

        /// <summary>
        /// Create a new MobileGameTile
        /// </summary>
        public MobileGameTile(GameObject obj)
            : this(obj, 0, 0, 0)
        {
        }

        /// <summary>
        /// Set the X,Y,Z of the tile to be the same as the location passed
        /// </summary>
        public void MoveToLocation(Location location)
        {
            MobileTile mobileTile = (_tile as MobileTile);
            mobileTile.X = location.X;
            mobileTile.Y = location.Y;
            mobileTile.Z = location.Z;
        }

        /// <summary>
        /// Get or set the X position for the tile
        /// </summary>
        public float X
        {
            get { return (_tile as MobileTile).X; }
            set { (_tile as MobileTile).X = value; }
        }


        /// <summary>
        /// Get or set the Y position for the tile
        /// </summary>
        public float Y
        {
            get { return (_tile as MobileTile).Y; }
            set { (_tile as MobileTile).Y = value; }
        }

        /// <summary>
        /// Get or set the Z position for the tile
        /// </summary>
        public float Z
        {
            get { return (_tile as MobileTile).Z; }
            set { (_tile as MobileTile).Z = value; }
        }

        /// <summary>
        /// When comparing the Y positions of a tile with a non 0 edge factor.  
        /// The edge factor is multiplied by the difference between the screen X positions of the two tiles.
        /// This is used for tiles such as fences that exist between normal tile locations.
        /// This must be set before the first call to Update.
        /// </summary>
        public float EdgeFactor
        {
            get { return (_tile as MobileTile).EdgeFactor; }
            set { (_tile as MobileTile).EdgeFactor = value; }
        }

        /// <summary>
        /// The type of forced layering that applies to this tile.
        /// This must be set before the first call to Update.
        /// </summary>
        public ForcedLayerType ForcedLayering
        {
            get { return (_tile as MobileTile).ForcedLayering; }
            set { (_tile as MobileTile).ForcedLayering = value; }
        }
    }
}
