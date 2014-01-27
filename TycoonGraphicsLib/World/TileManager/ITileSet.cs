using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace TycoonGraphicsLib
{
    /// <summary>
    /// A tile set is a set of tiles that all get drawn at the same time
    /// </summary>
    internal interface ITileSet : IEnumerable<Tile>
    {
        /// <summary>
        /// Add a tile to the tile set
        /// </summary>
        /// <param name="tile"></param>
        void AddTile(Tile tile);
        
        /// <summary>
        /// Remove a tile from this tile set
        /// </summary>
        /// <param name="tile"></param>
        void RemoveTile(Tile tile);
        
        /// <summary>
        /// Render all tiles in the tile set
        /// </summary>
        void Render();

        /// <summary>
        /// Free resources used by the tile set
        /// </summary>
        void Delete();
        
        /// <summary>
        /// Get values for the tile
        /// </summary>
        void GetTileRenderValues(Tile tile, out float left, out float top, out float right, out float bottom, out float texLeft, out float texTop, out float texRight, out float texBottom);
        
    }
}
