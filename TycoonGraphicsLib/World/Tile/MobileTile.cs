using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;

namespace TycoonGraphicsLib
{
    public enum ForcedLayerType
    {
        /// <summary>
        /// No forced layering for this tile
        /// </summary>
        None = 0,
        
        /// <summary>
        /// This tile will always be behind any tile with a ForcedLayerType set to InFront
        /// </summary>
        Behind = 1,

        /// <summary>
        /// This tile will always be in front of any tile with a ForcedLayerType set to Behind        
        /// </summary>
        InFront = 2,

        /// <summary>
        /// No ordering will be applied between this tile and another tile that have a ForcedLayeringType set to Same
        /// </summary>
        Same = 4,


        
    }


    public sealed class MobileTile : Tile
    {

        /// <summary>
        /// The X poition of the tile in game units
        /// </summary>
        private DelayedValue<float> _gameX = new DelayedValue<float>(0);

        /// <summary>
        /// The Y poition of the tile in game units
        /// </summary>
        private DelayedValue<float> _gameY = new DelayedValue<float>(0);

        /// <summary>
        /// The Z poition of the tile in game units
        /// </summary>
        private DelayedValue<float> _gameZ = new DelayedValue<float>(0);

        /// <summary>
        /// The type of forced layering that applies to this tile
        /// </summary>
        private ForcedLayerType _forcedLayering = ForcedLayerType.None;

        /// <summary>
        /// When comparing the Y positions of a tile with a non 0 edge factor.  
        /// The edge factor is multiplied by the difference between the screen X positions of the two tiles.
        /// This is used for tiles such as fences that exist between normal tile locations
        /// </summary>
        private float _edgeFactor = 0;



        /// <summary>
        /// The layer the moving tile is currently in.
        /// This will change as the tile is moved between layers
        /// </summary>
        private int _layer;

        


        #region Cached Values

        /// <summary>
        /// The left position where the tile should appear on the screen.
        /// This value is cached because it does not need to be redetermined when only the layer of the tile changes.
        /// </summary>
        private float _screenLeft;

        /// <summary>
        /// The right position where the tile should appear on the screen.
        /// This value is cached because it does not need to be redetermined when only the layer of the tile changes.
        /// </summary>
        private float _screenRight;

        /// <summary>
        /// The top position where the tile should appear on the screen.
        /// This value is cached because it does not need to be redetermined when only the layer of the tile changes.
        /// </summary>
        private float _screenTop;

        /// <summary>
        /// The bottom position where the tile should appear on the screen.
        /// This value is cached because it does not need to be redetermined when only the layer of the tile changes.
        /// </summary>
        private float _screenBottom;

        /// <summary>
        /// The X position of the tile in world units.
        /// This value is cached because it does not need to be redetermined when only the layer of the tile changes.
        /// </summary>
        private float _worldCenterX;

        /// <summary>
        /// The Y position of the tile in world units.
        /// This value is cached because it does not need to be redetermined when only the layer of the tile changes.
        /// </summary>
        private float _worldCenterY;

        /// <summary>
        /// The X position of the tile in world units.  Rotation is taken into account.        
        /// This value is cached because it does not need to be redetermined when only the layer of the tile changes.
        /// </summary>
        private float _worldX;

        /// <summary>
        /// The Y position of the tile in world units (without additional Y from the tiles Z or base level taken into account).  Rotation is taken into account.        
        /// This value is cached because it does not need to be redetermined when only the layer of the tile changes.
        /// </summary>
        private float _worldY;

        /// <summary>
        /// The left position where the tile should appear in the world.
        /// This value is cached because it does not need to be redetermined when only the layer of the tile changes.
        /// </summary>
        private float _worldLeft;

        /// <summary>
        /// The right position where the tile should appear in the world.
        /// This value is cached because it does not need to be redetermined when only the layer of the tile changes.
        /// </summary>
        private float _worldRight;

        /// <summary>
        /// The top position where the tile should appear in the world.
        /// This value is cached because it does not need to be redetermined when only the layer of the tile changes.
        /// </summary>
        private float _worldTop;

        /// <summary>
        /// The bottom position where the tile should appear in the world.
        /// This value is cached because it does not need to be redetermined when only the layer of the tile changes.
        /// </summary>
        private float _worldBottom;
        
        #endregion
        

        /// <summary>
        /// Create a new fixed tile
        /// </summary>
        internal MobileTile(World world) : base(world)
        {
            _layerGraphNode = new LayerGraphNode(this);
        }


        /// <summary>
        /// Get or set the X position for the tile
        /// </summary>
        public float X
        {
            set { _gameX.Delayed = value; }
            get { return _gameX.Delayed; }
        }

        /// <summary>
        /// Get or set the Y position for the tile
        /// </summary>
        public float Y
        {
            set { _gameY.Delayed = value; }
            get { return _gameY.Delayed; }
        }

        /// <summary>
        /// Get or set the Z position for the tile
        /// </summary>
        public float Z
        {
            set { _gameZ.Delayed = value; }
            get { return _gameZ.Delayed; }
        }



        /// <summary>
        /// The type of forced layering that applies to this tile.
        /// Multiple forced layer types can be applied using |
        /// This must be set before the first call to Update.
        /// </summary>
        public ForcedLayerType ForcedLayering
        {
            set { _forcedLayering = value; }
            get { return _forcedLayering; }
        }

        /// <summary>
        /// When comparing the Y positions of a tile with a non 0 edge factor.  
        /// The edge factor is multiplied by the difference between the screen X positions of the two tiles.
        /// This is used for tiles such as fences that exist between normal tile locations.
        /// This must be set before the first call to Update.
        /// </summary>
        public float EdgeFactor
        {
            set { _edgeFactor = value; }
            get { return _edgeFactor; }
        }



        #region Internal Position Properties

        /// <summary>
        /// The X poition of the tile in game units
        /// </summary>
        internal float GameX { get { return _gameX.Current; } }

        /// <summary>
        /// The Y poition of the tile in game units
        /// </summary>
        internal float GameY { get { return _gameY.Current; } }

        /// <summary>
        /// The Z poition of the tile in game units
        /// </summary>
        internal float GameZ { get { return _gameZ.Current; } }

        /// <summary>
        /// The layer the moving tile is currently in.
        /// This will change as the tile is moved between layers.
        /// </summary>
        internal int Layer { get { return _layer; } }
        
        /// <summary>
        /// The X position of the tile in world units.
        /// </summary>
        internal float WorldCenterX { get { return _worldCenterX; } }

        /// <summary>
        /// The Y position of the tile in world units.
        /// </summary>
        internal float WorldCenterY { get { return _worldCenterY; } }

        /// <summary>
        /// The Y position of the tile in world units (without additional Y from the tiles Z or base level taken into account).  Rotation is taken into account.        
        /// This value is cached because it does not need to be redetermined when only the layer of the tile changes.
        /// </summary>
        internal float WorldY { get { return _worldY; } }

        /// <summary>
        /// The X position of the tile in world units.  Rotation is taken into account.
        /// This value is cached because it does not need to be redetermined when only the layer of the tile changes.
        /// </summary>
        internal float WorldX { get { return _worldX; } }

        /// <summary>
        /// The left position where the tile should appear in the World.        
        /// </summary>
        internal float WorldLeft { get { return _worldLeft; } }

        /// <summary>
        /// The right position where the tile should appear in the World.
        /// </summary>
        internal float WorldRight { get { return _worldRight; } }

        /// <summary>
        /// The top position where the tile should appear in the World.
        /// </summary>
        internal float WorldTop { get { return _worldTop; } }

        /// <summary>
        /// The bottom position where the tile should appear in the World.
        /// </summary>
        internal float WorldBottom { get { return _worldBottom; } }
                
        #endregion
                
        #region Layer Graph Helper Properties


        /// <summary>
        /// This flag can be used to mark a tile when searching tiles.
        /// </summary>
        private bool _searchBool = false;

        /// <summary>
        /// This flag can be used to mark a tile when searching tiles.
        /// This is add as a bool on the object as opposed to a hashmap in the QuickMap in order to increase preformance.
        /// </summary>
        internal bool SearchBool
        {
            get { return _searchBool; }
            set { _searchBool = value; }
        }


        /// <summary>
        /// Positions this tile is currently located at in the quick map.
        /// This allows for quickly removing the tile from all it positions in the quickmap.
        /// This exsists on the object as opposed to in a dictionay in order to increase preformance.
        /// </summary>
        private List<Point> _quickMapPositions = new List<Point>();
        
        /// <summary>
        /// Positions this tile is currently located at in the quick map.
        /// This allows for quickly removing the tile from all it positions in the quickmap.
        /// </summary>
        internal List<Point> QuickMapPositions
        {
            get { return _quickMapPositions; }
        }


        /// <summary>
        /// Node for this tile in the tile layer graph
        /// </summary>
        private LayerGraphNode _layerGraphNode;

        /// <summary>
        /// Node for this tile in the tile layer graph.
        /// This exsists on the object as opposed to in a dictionay in order to increase preformance.
        /// </summary>
        internal LayerGraphNode LayerGraphNode
        {
            get { return _layerGraphNode; }
        }

        #endregion
               


        /// <summary>
        /// Called (on the game threat) when the tile has been added to the delayed tile process list for changes.
        /// Passes if changes should be made ready using change stage A or B.
        /// </summary>
        internal override void MakeChangeReady(bool readyA)
        {
            _gameX.MakeReady(readyA);
            _gameY.MakeReady(readyA);
            _gameZ.MakeReady(readyA);
            _textureQuartet.MakeReady(readyA);
        }
        
        /// <summary>
        /// Called (on the GUI threat) when the tile has had changed made that needs to be used.
        /// Passes if changes from change stage A or B should be used.
        /// </summary>
        internal override void ProcessChanges(bool processA)
        {
            //use the changes made ready
            _gameX.UseReady(processA);
            _gameY.UseReady(processA);
            _gameZ.UseReady(processA);
            _textureQuartet.UseReady(processA);

            //update tiles location
            UpdateTileLocation();
        }

        /// <summary>
        /// Called (on the GUI threat) to have the tile remove and readd itself to the world
        /// </summary>
        internal override void UpdateTileLocation()
        {
            //remove the tile from its old tile set
            RemoveFromTileSet();

            //calculate the position of the tile in the world, and on the screen
            //store the results in memeber variables
            DoTileCalculations(_textureQuartet.Current, _gameX.Current, _gameY.Current, _gameZ.Current, out _worldX, out _worldY, out _worldLeft, out _worldTop, out _worldRight, out _worldBottom, out _worldCenterX, out _worldCenterY, out _screenLeft, out _screenTop, out _screenRight, out _screenBottom);
            
            //move the tile to the correct layer
            _world.TileManger.TileLayerManager.UpdateTilesLayer(this);

            //add to the correct tile buffer
            AddToTileSet(_worldCenterX, _worldCenterY, _layer, _screenLeft, _screenTop, _screenRight, _screenBottom);
        }
        

        /// <summary>
        /// Called by the tile layer manager in order to update the layer of a tile is updated
        /// </summary>
        internal void UpdateLayer(int newLayer)
        {
            //remove the tile from its old tile set
            RemoveFromTileSet();

            //update the layer
            _layer = newLayer;

            //add to the correct tile buffer based on the new layer
            //note we do not need to recaulte world position or screen poition
            AddToTileSet(_worldCenterX, _worldCenterY, _layer, _screenLeft, _screenTop, _screenRight, _screenBottom);
        }

        /// <summary>
        /// Called (on the GUI threat) when the tile is removed from the world
        /// </summary>
        internal override void ProcessDeletion()
        {
            //remove from layer manager            
            _world.TileManger.TileLayerManager.DeleteTileLayerInfo(this);

            //remove from tile set
            RemoveFromTileSet();
        }

    }
}
