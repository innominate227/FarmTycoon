using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TycoonGraphicsLib
{

    public class ClickInfo
    {
        /// <summary>
        /// X location of the screen clicked
        /// </summary>
        private int _x;

        /// <summary>
        /// Y location of the screen clicked
        /// </summary>
        private int _y;

        /// <summary>
        /// Mouse button used for the lick
        /// </summary>
        private MouseButton _button;
        
        /// <summary>
        /// The tiles clicked (where index 0 is the tile in the front)
        /// </summary>
        private TileClickInfo[] _tilesClicked;

        /// <summary>
        /// The control that was clicked, or null if no control was clicked
        /// </summary>
        private TycoonControl _controlClicked;
        
        
        /// <summary>
        /// Create a new ClickInfo object
        /// </summary>
        public ClickInfo(int x, int y, MouseButton button, TileClickInfo[] tilesClicked, TycoonControl controlClicked)
        {
            _x = x;
            _y = y;
            _button = button;
            _tilesClicked = tilesClicked;
            _controlClicked = controlClicked;
        }

        /// <summary>
        /// X location of the screen clicked
        /// </summary>
        public int X
        {
            get { return _x; }
        }
        
        /// <summary>
        /// Y location of the screen clicked
        /// </summary>
        public int Y
        {
            get { return _y; }
        }

        /// <summary>
        /// Mouse button used for the lick
        /// </summary>
        public MouseButton Button
        {
            get { return _button; }
        }

        /// <summary>
        /// The tiles clicked (where index 0 is the tile in the front)
        /// </summary>
        public TileClickInfo[] TilesClicked
        {
            get { return _tilesClicked; }
        }

        public bool TileClicked
        {
            get
            {
                return (_tilesClicked.Length > 0);
            }
        }

        /// <summary>
        /// The top most tile that was clicked
        /// </summary>
        public Tile TopMostTile
        {
            get 
            {
                if (_tilesClicked.Length > 0)
                {
                    return _tilesClicked[0].Tile;
                }
                else
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// The control that was clicked, or null if no control was clicked
        /// </summary>
        public TycoonControl ControlClicked
        {
            get { return _controlClicked; }
        }

    }


    public class TileClickInfo
    {
        /// <summary>
        /// The tile clicked
        /// </summary>
        private Tile _tile;

        /// <summary>
        /// Number between 0=left, and 1=right that tells what portion of the tile was clicked
        /// </summary>
        private float _clickLocationX;

        /// <summary>
        /// Number between 0=top, and 1=botom that tells what portion of the tile was clicked
        /// </summary>
        private float _clickLocationY;

        /// <summary>
        /// Create a new TileClickInfo object
        /// </summary>
        public TileClickInfo(Tile tile, float clickLocationX, float clickLocationY)
        {
            _tile = tile;
            _clickLocationX = clickLocationX;
            _clickLocationY = clickLocationY;
        }

        /// <summary>
        /// The tile clicked
        /// </summary>
        public Tile Tile
        {
            get{ return _tile; }
        }

        /// <summary>
        /// Number between 0=left, and 1=right that tells what portion of the tile was clicked
        /// </summary>
        public float ClickLocationX
        {
            get { return _clickLocationX; }
        }

        /// <summary>
        /// Number between 0=top, and 1=botom that tells what portion of the tile was clicked
        /// </summary>
        public float ClickLocationY
        {
            get { return _clickLocationY; }
        }

    }
}
