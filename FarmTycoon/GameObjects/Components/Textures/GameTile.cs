using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Wraps a tile.  Manages the texture for a tile whos texture is made by concatnation three strings Prepend + Texture + Append.    
    /// </summary>
    public class GameTile
    {
        /// <summary>
        /// String to prepend to the tiles texture name
        /// </summary>
        private string _prepend = "";

        /// <summary>
        /// String to append to the tiles texture name
        /// </summary>
        private string _append = "";

        /// <summary>
        /// String to append to the tiles texture name for the frame (after the normal append)
        /// </summary>
        private string _frame = "";
        
        /// <summary>
        /// String for the tiles texture name
        /// </summary>
        private string _texture = "";

        /// <summary>
        /// If the tile is hidden its texture will always be "empty" no matter what else it gets set to until it is unhidden again
        /// </summary>
        private bool _hidden = false;

        /// <summary>
        /// The tile wrapped
        /// </summary>
        protected Tile _tile;

        /// <summary>
        /// The game object the game tile is for
        /// </summary>
        protected GameObject _gameObj;



        /// <summary>
        /// The game object the game tile is for
        /// </summary>
        public GameObject GameObject
        {
            get { return _gameObj; }
        }
        

        /// <summary>
        /// String to prepend to the tiles texture name
        /// </summary>
        public string Prepend
        {
            get { return _prepend; }
            set { _prepend = value; SetTextureName(); }
        }

        /// <summary>
        /// String to append to the tiles texture name
        /// </summary>
        public string Append
        {
            get { return _append; }
            set { _append = value; SetTextureName(); }
        }

        /// <summary>
        /// String to append to the tiles texture name for the frame (after the normal append)
        /// </summary>
        public string Frame
        {
            get { return _frame; }
            set { _frame = value; SetTextureName(); }
        }

        /// <summary>
        /// The texture for the tile.
        /// This should be used as opposed to ".RenderingInfo.TextureQuartet" so that the Append and Prepend strings can be applied to create the full texture name
        /// </summary>
        public string Texture
        {
            get { return _texture; }
            set { _texture = value; SetTextureName(); }
        }


        /// <summary>
        /// If the tile is hidden its texture will always be "empty" no matter what else it gets set to until it is unhidden again
        /// </summary>
        public bool Hidden
        {
            get { return _hidden; }
            set { _hidden = value; SetTextureName(); }
        }

        /// <summary>
        /// Set the texture name on the tile when the Prepend/Append/Texture property is changed
        /// </summary>
        private void SetTextureName()
        {
            if (_hidden)
            {
                //TODO: instead of hidding delete the tile from the game world, and add it back again when unhidden
                _tile.Texture= "empty";                
            }
            else
            {
                _tile.Texture = _prepend + _texture + _append + _frame;                
            }
        }

        /// <summary>
        /// Update the tile to have the latest values.  Add the tile to the game world if not already added.
        /// </summary>
        public void Update()
        {
            _tile.Update();            
        }

        /// <summary>
        /// Delete the tile. (remove if from the gameworld if added)
        /// </summary>
        public void Delete()
        {
            _tile.Delete();
        }

    }
}
