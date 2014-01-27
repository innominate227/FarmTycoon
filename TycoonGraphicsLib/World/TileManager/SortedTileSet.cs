using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace TycoonGraphicsLib
{
    /// <summary>
    /// A tile set is a set of tiles that all get drawn at the same time.  Tile added to this set will be drawn in the order specified by their Sort property.
    /// </summary>
    internal class SortedTileSet : ITileSet, IEnumerable<Tile>
    {
        /// <summary>
        /// All the tiles, mapped to what bucket they are in
        /// </summary>
        private Dictionary<Tile, int> m_tileBucketLoc = new Dictionary<Tile, int>();

        /// <summary>
        /// A dictionary of buckets
        /// </summary>
        private Dictionary<int, HashSet<Tile>> m_buckets = new Dictionary<int,HashSet<Tile>>();

        /// <summary>
        /// Sorted list of bucket numbers
        /// </summary>
        private List<int> m_bucketNumbers = new List<int>();


        /// <summary>
        /// Are the buffer, and tiles sortedTiles list up to date currently sorted
        /// </summary>
        private bool m_sorted = false;        

        /// <summary>
        /// opengl buffer used to render the tiles in this tile set.
        /// </summary>
        private QuadTextureBuffer m_buffer = new QuadTextureBuffer();

        /// <summary>
        /// Mapping from each tiles to its location in the tile buffer (if m_sorted=false this may not contain all tiles)
        /// </summary>
        private Dictionary<Tile, int> m_tileLocations = new Dictionary<Tile, int>();

        /// <summary>
        /// List the tiles sorted (if m_sorted=false this may not contain all tiles)
        /// </summary>
        private List<Tile> m_sortedTiles = new List<Tile>();
          


                
        /// <summary>
        /// Add a tile to the tile set
        /// </summary>
        /// <param name="tile"></param>
        public void AddTile(Tile tile)
        {
            //create bucket if its not done yet
            if (m_buckets.ContainsKey(tile.CurrentSort) == false)
            {
                m_bucketNumbers.Add(tile.CurrentSort);
                m_bucketNumbers.Sort();

                m_buckets.Add(tile.CurrentSort, new HashSet<Tile>());
            }

            //remember what bucket we put the tile in
            m_tileBucketLoc.Add(tile, tile.CurrentSort);

            //add tile to the bucket
            m_buckets[tile.CurrentSort].Add(tile);

            //mark the list unsorted
            m_sorted = false;
        }


        /// <summary>
        /// Remove a tile from this tile set
        /// </summary>
        /// <param name="tile"></param>
        public void RemoveTile(Tile tile)
        {
            //remove the tile from the bucket
            m_buckets[m_tileBucketLoc[tile]].Remove(tile);

            //remove the tile from our bucket location list
            m_tileBucketLoc.Remove(tile);

            //mark the list unsorted
            //Note the buffer is actually still sorted at this point, (it just has a hole), but its simpler to just mark unsorted so the buffer, and sorted tiles list get rebuilt
            m_sorted = false;
        }


        /// <summary>
        /// Rebuilds the buffer, and sorted list (if nessiary)
        /// </summary>
        private void Rebuild()
        {
            if (m_sorted == false)
            {
                m_sorted = true;
                m_buffer.Clear();
                m_sortedTiles.Clear();
                m_tileLocations.Clear();

                //create the sorted tiles list by adding all tiles from each bucket
                foreach (int bucketNum in m_bucketNumbers)
                {
                    m_sortedTiles.AddRange(m_buckets[bucketNum]);
                }

                //create the buffer
                foreach (Tile tile in m_sortedTiles)
                {
                    //get a slot in the buffer to add the tile to
                    int bufferSlot = m_buffer.GetNextFreeSlot();

                    //get the location for the tile in points
                    float left, top, right, bottom;
                    tile.GetLocationPoints(out left, out top, out right, out bottom);

                    //get the current texture for the tile
                    Texture texture = tile.GetTexture();

                    //set the values for its buffer slot                
                    m_buffer.SetSlotValues(bufferSlot, left, top, right, bottom, texture);

                    //keep track of where each tile is in the buffer
                    m_tileLocations.Add(tile, bufferSlot);
                }
            }
        }


        /// <summary>
        /// Render all tiles in the tile set
        /// </summary>
        public void Render()
        {
            Rebuild();
            m_buffer.Render();
        }

        /// <summary>
        /// Free resources used by the tile set
        /// </summary>
        public void Delete()
        {
            m_buffer.Delete();
        }
        
        /// <summary>
        /// Get values for the tile
        /// </summary>
        public void GetTileRenderValues(Tile tile, out float left, out float top, out float right, out float bottom, out float texLeft, out float texTop, out float texRight, out float texBottom)
        {
            Rebuild();
            int tileLocation = m_tileLocations[tile];
            m_buffer.GetSlotValues(tileLocation, out left, out top, out right, out bottom, out texLeft, out texTop, out texRight, out texBottom);
        }

        
        /// <summary>
        /// Get an enumerator that enumerates over all tiles in the set
        /// </summary>
        public IEnumerator<Tile> GetEnumerator()
        {
            Rebuild();
            return m_sortedTiles.Reverse<Tile>().GetEnumerator();
        }

        /// <summary>
        /// Get an enumerator that enumerates over all tiles in the set
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            Rebuild();
            return m_sortedTiles.Reverse<Tile>().GetEnumerator();
        }


    }
}
