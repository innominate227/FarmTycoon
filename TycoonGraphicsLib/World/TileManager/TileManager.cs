using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace TycoonGraphicsLib
{
    internal class TileManager
    {        
        /// <summary>
        /// Array of tile sets used to display the tiles
        /// </summary>
        private TileSet[] _tileSet;
                		
		/// <summary>
		/// World the tiles belong to
		/// </summary>
		private World _world;

        /// <summary>
        /// List of tiles that have been changed since the last frame, and need to be updated before the next frame.
        /// </summary>
        private DelayedTileProcessList _delayedTileProcessList;

        /// <summary>
        /// Manages what layer each tile should appear on.
        /// </summary>
        private TileLayerManager _tileLayerManager;
                
        /// <summary>
        /// Create a new tile manager to manage the tiles that make up the world passed
        /// </summary>
		public TileManager(World world)
        {
			_world = world;
            _delayedTileProcessList = new DelayedTileProcessList(world);
            _tileLayerManager = new TileLayerManager(world);
            CreateTileBuffers();

            //need to recalulate tile positions if the window settings change
            WindowSettings.SettingsChanged += new Action(ReCalcTiles);
        }


        /// <summary>
        /// Keeps track of tiles that have been changed since the last frame, and need to be updated on the next frame.
        /// </summary>
        public DelayedTileProcessList DelayedTileProcessList
        {
            get { return _delayedTileProcessList; }
        }

        /// <summary>
        /// Manages what layer each tile should appear on.
        /// </summary>
        public TileLayerManager TileLayerManager
        {
            get { return _tileLayerManager; }
        }

		
        /// <summary>
        /// Create a new Fixed Tile.  This is called on the Game Thread.
        /// </summary>
        public FixedTile CreateFixedTile()
        {
            return new FixedTile(_world);
        }

        /// <summary>
        /// Create a new Mobile Tile.  This is called on the Game Thread.
        /// </summary>
        public MobileTile CreateMobileTile()
        {
            return new MobileTile(_world);            
        }
        
        
        /// <summary>
        /// Recalculate the position of all tiles.  This needs to be done when the rotation, or scale is changed, or when the window is resized.
        /// </summary>
        public void ReCalcTiles()
        {
            List<Tile> allTiles = new List<Tile>();
            for (int i = 0; i < _tileSet.Length; i++)
            {
                allTiles.AddRange(_tileSet[i]);
            }

            //remove each tile then add back so that it appears in the correct location after the size change
            foreach (Tile tile in allTiles)
            {
                tile.UpdateTileLocation();                
            }
            
        }

        #region Values Cache
        /// <summary>
        /// Value of _world.WorldSettings.SegmentDivisions
        /// Caching this here improves performance signigicantly
        /// </summary>
        private int _segmentDivisionsCache;

        /// <summary>
        /// Value of _world.WorldSettings.SegmentDivisions * _world.WorldSettings.SegmentDivisions * _world.TileTextureManager.TextureSheets.Count
        /// Caching this here improves performance signigicantly
        /// </summary>
        private int _layerMultiplierCache;

        /// <summary>
        /// Value of _world.WorldSettings.SegmentDivisions * _world.WorldSettings.SegmentDivisions
        /// Caching this here improves performance signigicantly
        /// </summary>
        private int _sheetMultiplierCache;
        #endregion

        /// <summary>
        /// Create the tilesets to hold the tiles
        /// </summary>
        private void CreateTileBuffers()
        {
            //calculate the number of tile sets that need to be created
            int tileSetCount = _world.WorldSettings.SegmentDivisions * _world.WorldSettings.SegmentDivisions * _world.WorldSettings.Layers * _world.TileTextureManager.TextureSheets.Count;
            
            //create the tileset array
            _tileSet = new TileSet[tileSetCount];

            //create each tile set
            for (int i = 0; i < tileSetCount; i++)
            {
                _tileSet[i] = new TileSet();
            }          
  
            //set cached values
            _segmentDivisionsCache = _world.WorldSettings.SegmentDivisions;
            _layerMultiplierCache = _world.WorldSettings.SegmentDivisions * _world.WorldSettings.SegmentDivisions * _world.TileTextureManager.TextureSheets.Count;
            _sheetMultiplierCache = _world.WorldSettings.SegmentDivisions * _world.WorldSettings.SegmentDivisions;
        }

        /// <summary>
        /// Get the TileBuffer that should be used to store a tile at a particular location (and for a particular texture sheet)
        /// </summary>
        public TileSet GetTileBuffer(float worldX, float worldY, int layer, int textureSheetIndex)
        {
            int xSegment = (int)(worldX / _world.WorldSettings.SegmentSize);
            int ySegment = (int)(worldY / _world.WorldSettings.SegmentSize);

            return GetTileSetForSegment(xSegment, ySegment, layer, textureSheetIndex);
        }

        /// <summary>
        /// Get the TileBuffer for the x segment y segemnt and layer passed
        /// </summary>
        public TileSet GetTileSetForSegment(int xSegment, int ySegment, int layer, int textureSheetIndex)
        {            
            if (xSegment < 0 || ySegment < 0 || xSegment >= _segmentDivisionsCache || ySegment >= _segmentDivisionsCache) { return null; }

            int tileSetIndex = (layer * _layerMultiplierCache) + (textureSheetIndex * _sheetMultiplierCache) + (ySegment * _segmentDivisionsCache) + xSegment;
            return _tileSet[tileSetIndex];
        }


        

        /// <summary>
        /// Delete all resources used by the tile manager
        /// </summary>
        public void Delete()
        {
            WindowSettings.SettingsChanged -= new Action(ReCalcTiles);

            //delete all tile buffers
            foreach (TileSet tileSet in _tileSet)
            {
                tileSet.Delete();
            }
        }
       


    }
}
