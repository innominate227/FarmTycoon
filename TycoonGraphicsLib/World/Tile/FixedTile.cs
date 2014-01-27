using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;

namespace TycoonGraphicsLib
{

    public sealed class FixedTile : Tile
    {
        /// <summary>
        /// The X poition of the tile in game unit
        /// </summary>
        private float _gameX;

        /// <summary>
        /// The Y poition of the tile in game unit
        /// </summary>
        private float _gameY;

        /// <summary>
        /// The Z poition of the tile in game unit
        /// </summary>
        private float _gameZ;

        /// <summary>
        /// The layer for the background tile
        /// </summary>
        private int _layer;



        /// <summary>
        /// Create a new background tile
        /// </summary>
        internal FixedTile(World world) : base(world)
        {
        }



        /// <summary>
        /// The X poition of the tile in game unit.
        /// This MUST be set before calling Update() on this tile for the first time. 
        /// Or the tile could end up in a strange state.
        /// </summary>
        public float X
        {
            get { return _gameX; }
            set { _gameX = value; }
        }

        /// <summary>
        /// The Y poition of the tile in game unit.
        /// This MUST be set before calling Update() on this tile for the first time. 
        /// Or the tile could end up in a strange state.
        /// </summary>
        public float Y
        {
            get { return _gameY; }
            set { _gameY = value; }
        }

        /// <summary>
        /// The Z poition of the tile in game unit.
        /// This MUST be set before calling Update() on this tile for the first time. 
        /// Or the tile could end up in a strange state.
        /// </summary>
        public float Z
        {
            get { return _gameZ; }
            set { _gameZ = value; }
        }

        /// <summary>
        /// The layer for the background tile
        /// This MUST be set before calling Update() on this tile for the first time. 
        /// Or the tile could end up in a strange state.
        /// </summary>
        public int Layer
        {
            get { return _layer; }
            set { _layer = value; }
        }

                

        /// <summary>
        /// Called (on the game threat) when the tile has been added to the delayed tile process list for changes.
        /// Passes if changes should be made ready using change stage A or B.
        /// </summary>
        internal override void MakeChangeReady(bool readyA)
        {
            _textureQuartet.MakeReady(readyA);
        }
        

        /// <summary>
        /// Called (on the GUI threat) when the tile has had changed made that needs to be used.
        /// Passes if changes from change stage A or B should be used.
        /// </summary>
        internal override void ProcessChanges(bool processA)
        {
            //ready the changes to the texture
            _textureQuartet.UseReady(processA);

            //move the tile to the correcy place in the world
            UpdateTileLocation();
        }

        /// <summary>
        /// Called (on the GUI threat) to have the tile remove and readd itself to the world
        /// </summary>
        internal override void UpdateTileLocation()
        {
            //remove the tile from where it used to be
            ProcessDeletion();

            //calculate the position of the tile in the world, and on the screen
            float worldX; float worldY; float worldLeft; float worldTop; float worldRight; float worldBottom; float worldCenterX; float worldCenterY; float screenLeft; float screenTop; float screenRight; float screenBottom;
            DoTileCalculations(_textureQuartet.Current, _gameX, _gameY, _gameZ, out worldX, out worldY, out worldLeft, out worldTop, out worldRight, out worldBottom, out worldCenterX, out worldCenterY, out screenLeft, out screenTop, out screenRight, out screenBottom);

            //add to the correct tile buffer
            AddToTileSet(worldCenterX, worldCenterY, _layer, screenLeft, screenTop, screenRight, screenBottom);
        }

        /// <summary>
        /// Called (on the GUI threat) when the tile is removed from the world
        /// </summary>
        internal override void ProcessDeletion()
        {
            //remove from tile set
            RemoveFromTileSet();
        }
        
    }
}
