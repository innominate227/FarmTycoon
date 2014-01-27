using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;

namespace TycoonGraphicsLib
{
    /// <summary>
    /// A tile set is a set of tiles that all get drawn at the same time
    /// </summary>
    internal class TileSet : IEnumerable<Tile>
    {
        /// <summary>
        /// opengl buffer used to render the tiles in this tile set.
        /// </summary>
        private QuadTextureBuffer _buffer = new QuadTextureBuffer();

        /// <summary>
        /// The tiles that are currently in this tile set, mapped to their location in the tile buffer
        /// </summary>
        private HashSet<Tile> _tiles = new HashSet<Tile>();

        /// <summary>
        /// Is the tile set empty, faster than doing _tiles.Count > 0.
        /// We want to know most the time if its not empty as opposed to empty so we remeber that
        /// </summary>
        private bool _notEmpty = false;


        /// <summary>
        /// Add a tile to the tile set
        /// </summary>
        public void AddTile(Tile tile, float screenLeft, float screenTop, float screenRight, float screenBottom, Texture texture)
        {
            //get a slot in the buffer to add the tile to
            int bufferSlot = _buffer.GetNextFreeSlot();
            
            //set the values for its buffer slot                
            _buffer.SetSlotValues(bufferSlot, screenLeft, screenTop, screenRight, screenBottom, texture);

            //set the buffer slot for the tile
            tile.BufferSlot = bufferSlot;

            //add to our collection of all tiles in the set
            _tiles.Add(tile);

            //not empty now
            _notEmpty = true;
        }


        /// <summary>
        /// Remove a tile from this tile set
        /// </summary>
        public void RemoveTile(Tile tile)
        {
            //free the slot in the buffer
            _buffer.FreeSlot(tile.BufferSlot);

            //remove the tile form our list
            _tiles.Remove(tile);

            //check if were not empty now
            if (_tiles.Count == 0)
            {
                _notEmpty = false;
            }
        }


        /// <summary>
        /// Render all tiles in the tile set
        /// </summary>
        public void Render()
        {
            _buffer.Render();
        }

        /// <summary>
        /// Free resources used by the tile set
        /// </summary>
        public void Delete()
        {
            _buffer.Delete();
        }

        /// <summary>
        /// Get values for the tile
        /// </summary>
        public void GetTileRenderValues(Tile tile, out float left, out float top, out float right, out float bottom, out float texLeft, out float texTop, out float texRight, out float texBottom)
        {
            int bufferSlot = tile.BufferSlot;
            _buffer.GetSlotValues(bufferSlot, out left, out top, out right, out bottom, out texLeft, out texTop, out texRight, out texBottom);
        }

        /// <summary>
        /// True if the tile set is not empty
        /// </summary>
        public bool NotEmpty
        {
            get { return _notEmpty; }
        }

        /// <summary>
        /// Returns the number of tiles in the tile set
        /// </summary>
        public int Count
        {
            get { return _tiles.Count; }
        }


        /// <summary>
        /// Get an enumerator that enumerates over all tiles in the set
        /// </summary>
        public IEnumerator<Tile> GetEnumerator()
        {
            return _tiles.GetEnumerator();
        }

        /// <summary>
        /// Get an enumerator that enumerates over all tiles in the set
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _tiles.GetEnumerator();
        }


    }
}
