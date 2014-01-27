using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;

namespace TycoonGraphicsLib
{

    public abstract class Tile
    {
        /// <summary>
        /// The name of the texture quartet the tile will use.   This should really be protected and internal.
        /// </summary>
        internal DelayedValue<string> _textureQuartet = new DelayedValue<string>("");

        /// <summary>
        /// The world the tile is in.   This should really be protected and internal.
        /// </summary>
        internal World _world;

        /// <summary>
        /// The tile set the tile is in.  This should really be protected and internal.
        /// </summary>
        internal TileSet _setIn;

        /// <summary>
        /// The index of the tile in the tile buffer.  This should really be protected and internal.
        /// </summary>
        internal int _bufferSlot;

        /// <summary>
        /// The texture of the tile.  This should really be protected and internal.        
        /// </summary>
        internal TileTexture _tileTexture;

        /// <summary>
        /// Value assoicated with the tile.  This will generally be the game object the tile was created to show.
        /// </summary>
        private object _tag;


        /// <summary>
        /// Internal consturctor to ensure it is only inhertied from by other classes inside Tycoon Graphics Lib
        /// </summary>
        internal Tile(World world)
        {
            _world = world;
        }


        /// <summary>
        /// Get or set the texture for the tile
        /// </summary>
        public string Texture
        {
            set { _textureQuartet.Delayed = value; }
            get { return _textureQuartet.Delayed; }
        }

        /// <summary>
        /// Value assoicated with the tile.  This will generally be the game object the tile was created to show.
        /// </summary>
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }


        /// <summary>
        /// The index of the tile in the tile buffer
        /// </summary>
        internal int BufferSlot
        {
            get { return _bufferSlot; }
            set { _bufferSlot = value; }
        }


        /// <summary>
        /// The texture of the tile. 
        /// </summary>
        internal TileTexture TileTexture
        {
            get { return _tileTexture; }            
        }
        
        /// <summary>
        /// Call (on the Game thread) to update the tile with the latest settings (on the next frame).
        /// </summary>
        public void Update()
        {
            //if we are in a change set remeber this tile, and wait to process changes until later.
            if (_inChangeSet)
            {
                _changeSet.Add(this);
                return;
            }

            //add the tile to the delayed tile process list
            _world.TileManger.DelayedTileProcessList.QueueForChange(this);
        }


        /// <summary>
        /// Call (on the Game thread) to delete the tile (on the next frame).
        /// </summary>
        public void Delete()
        {
            //if we are in a change set remeber this tile, and wait to process changes until later.
            if (_inChangeSet)
            {
                _deleteSet.Add(this);
                return;
            }

            //add the tile to the delayed tile process list
            _world.TileManger.DelayedTileProcessList.QueueForDeletion(this);
        }

                
        /// <summary>
        /// Called in the derived class (on the game threat) when the tile has been added to the delayed tile process list for changes.
        /// Passes if changes should be made ready using change stage A or B.
        /// </summary>
        internal abstract void MakeChangeReady(bool readyA);

        /// <summary>
        /// Called  in the derived class (on the GUI threat) when the tile has had changed made that need to be used.
        /// Passes if changes from change stage A or B should be used.
        /// </summary>
        internal abstract void ProcessChanges(bool processA);

        /// <summary>
        /// Called  in the derived class (on the GUI threat) when the tile has been deleted.        
        /// </summary>
        internal abstract void ProcessDeletion();
        
        /// <summary>
        /// Called (on the GUI threat) to have the tile remove and readd itself to the world
        /// </summary>
        internal abstract void UpdateTileLocation();
        
            
        /// <summary>
        /// Calculate tile bounds, and world position given its texture and game location.  This should really be protected and internal.
        /// </summary>
        internal void DoTileCalculations(string textureQuartet, float gameX, float gameY, float gameZ, out float worldX, out float worldY, out float worldLeft, out float worldTop, out float worldRight, out float worldBottom, out float worldCenterX, out float worldCenterY, out float screenLeft, out float screenTop, out float screenRight, out float screenBottom)
        {
            //determine the texture the tile will use
            TileTextureQuartet quartet = _world.TileTextureManager.GetTextureQuartet(_textureQuartet.Current);
            _tileTexture = quartet.GetTextureForDirection(_world.SharedWorldViewSettings.CurrentDirection);

            //get the poition of the tile in world units (adjusts for our 45 deg view of the world)
            float worldNoRotationX = _world.WorldSettings.WorldUnitOffset + _world.WorldSettings.GameSize + gameX - gameY;
            float worldNoRotationY = _world.WorldSettings.WorldUnitOffset + gameX + gameY;

            //fix the x and y location based on rotation                        
            _world.SharedWorldViewSettings.GetXYForRotation(worldNoRotationX, worldNoRotationY, out worldX, out worldY);

            //worldZ is the same as gameZ
            float worldZ = gameZ;

            //get the left/top/right/bottom in world untis
            worldLeft = worldX - _tileTexture.CenterXOffset;
            worldTop = worldY - (_tileTexture.Height - _tileTexture.CenterYOffset) - (worldZ / 2.0f);
            worldRight = worldLeft + _tileTexture.Width;
            worldBottom = worldTop + _tileTexture.Height;

            //get the worldCenterX, and worldCenterY
            worldCenterX = (worldLeft + _tileTexture.Width / 2.0f);
            worldCenterY = (worldTop + _tileTexture.Height / 2.0f);
            
            
            //get the left/top/right/bottom in open GL points            
            screenLeft = -1.0f + worldLeft * _world.SharedWorldViewSettings.PointsPerWorldUnitX;
            screenTop = 1.0f - worldTop * _world.SharedWorldViewSettings.PointsPerWorldUnitY;
            screenRight = -1.0f + worldRight * _world.SharedWorldViewSettings.PointsPerWorldUnitX;
            screenBottom = 1.0f - worldBottom * _world.SharedWorldViewSettings.PointsPerWorldUnitY;
        }
        
        /// <summary>
        /// Add the tile to the correct tile buffer.  This should really be protected and internal.
        /// </summary>
        internal void AddToTileSet(float worldCenterX, float worldCenterY, int layer, float screenLeft, float screenTop, float screenRight, float screenBottom)
        {            
            //get the tile set to add the tile to
            _setIn = _world.TileManger.GetTileBuffer(worldCenterX, worldCenterY, layer, _tileTexture.TileTextureSheetIndex);

            //add the tile to the set
            _setIn.AddTile(this, screenLeft, screenTop, screenRight, screenBottom, _tileTexture.Texture);
        }
                
        /// <summary>
        /// Remove the tile from the tile set it is in if it is in one.  This should really be protected and internal.
        /// </summary>
        internal void RemoveFromTileSet()
        {
            if (_setIn != null)
            {
                _setIn.RemoveTile(this);
                _setIn = null;
            }
        }



        #region Change Set

        /// <summary>
        /// True is start change set has been called
        /// </summary>
        private static bool _inChangeSet = false;

        /// <summary>
        /// List of tiles changed during the change set
        /// </summary>
        private static List<Tile> _changeSet = new List<Tile>();

        /// <summary>
        /// List of tiles deleted during the change set
        /// </summary>
        private static List<Tile> _deleteSet = new List<Tile>();   


        /// <summary>
        /// After StartChangeSet is called all calls to tile.Update will be buffered.
        /// When EndChangeSet is called all changes will be made with in the same frame.
        /// </summary>
        public static void StartChangeSet()
        {
            Debug.Assert(_inChangeSet == false);

            //set to in a change set
            _inChangeSet = true;
        }

        /// <summary>
        /// After StartChangeSet is called all calls to tile.Update will be buffered.
        /// When EndChangeSet is called all changes will be made with in the same frame.
        /// </summary>
        public static void EndChangeSet()
        {
            Debug.Assert(_inChangeSet);

            //mo longer in a change set
            _inChangeSet = false;

            //ignore if change set is empty and delete set is emptu
            if (_changeSet.Count == 0 && _deleteSet.Count == 0)
            {
                return;
            }

            //all the tile will be in the same tile manager, we just need one of them so we can get a reference to it
            TileManager tileManager = null;
            if (_changeSet.Count > 0)
            {
                tileManager = _changeSet[0]._world.TileManger;
            }
            else
            {
                tileManager = _deleteSet[0]._world.TileManger;
            }

            //start adding multiple to the change list
            tileManager.DelayedTileProcessList.StartAddMultiple();

            //process each change
            foreach (Tile changedTile in _changeSet)
            {
                changedTile.Update();
            }

            //process each delete
            foreach (Tile changedTile in _deleteSet)
            {
                changedTile.Delete();
            }

            //end adding multiple to the change list
            tileManager.DelayedTileProcessList.EndAddMultiple();

            //clear the change set
            _changeSet.Clear();

            //clear the delete set
            _deleteSet.Clear();

        }

        #endregion


    }
}
