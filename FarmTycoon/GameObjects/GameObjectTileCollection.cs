using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{

    /// <summary>
    /// A collection of gameobject tiles, and methods for querying the collection
    /// </summary>
    public class GameObjectTileCollection : IEnumerable<GameObjectTile>
    {
        /// <summary>
        /// Raised when a tile is added
        /// </summary>
        public event Action<GameObjectTile> TileAdded;

        /// <summary>
        /// Raised when a tile is removed
        /// </summary>
        public event Action<GameObjectTile> TileRemoved;
                    


        /// <summary>
        /// all the GameObjectTiles in lists keyed by Z position
        /// </summary>
        private Dictionary<int, HashSet<GameObjectTile>> m_tilesByZ = new Dictionary<int, HashSet<GameObjectTile>>();

        /// <summary>
        /// all the GameObjectTiles in the list
        /// </summary>
        private HashSet<GameObjectTile> m_tiles = new HashSet<GameObjectTile>();
        
        /// <summary>
        /// Add a game object tile to the collection
        /// </summary>
        public void Add(GameObjectTile tile)
        {
            m_tiles.Add(tile);

            if (m_tilesByZ.ContainsKey(tile.Z) == false)
            {
                m_tilesByZ.Add(tile.Z, new HashSet<GameObjectTile>());
            }
            m_tilesByZ[tile.Z].Add(tile);

            if (TileAdded != null)
            {
                TileAdded(tile);
            }
        }

        /// <summary>
        /// Remove a game object tile from the collection
        /// </summary>
        public void Remove(GameObjectTile tile)
        {
            m_tiles.Remove(tile);

            m_tilesByZ[tile.Z].Remove(tile);
            if (m_tilesByZ[tile.Z].Count == 0)
            {
                m_tilesByZ.Remove(tile.Z);
            }

            if (TileRemoved != null)
            {
                TileRemoved(tile);
            }
        }

        /// <summary>
        /// The number of game object tiles in the collection
        /// </summary>
        public int Count
        {
            get { return m_tiles.Count; }
        }


        /// <summary>
        /// Get all tiles in the list with a z at or above the z passed
        /// </summary>
        public List<GameObjectTile> GetAtOrAbove(int z)
        {
            List<GameObjectTile> toRet = new List<GameObjectTile>();
            foreach (int zKey in m_tilesByZ.Keys)
            {
                if (zKey >= z)
                {
                    toRet.AddRange(m_tilesByZ[zKey]);
                }
            }
            return toRet;
        }


        /// <summary>
        /// Get all tiles in the list with a z above the z passed
        /// </summary>
        public List<GameObjectTile> GetAbove(int z)
        {
            List<GameObjectTile> toRet = new List<GameObjectTile>();
            foreach (int zKey in m_tilesByZ.Keys)
            {
                if (zKey > z)
                {
                    toRet.AddRange(m_tilesByZ[zKey]);
                }
            }
            return toRet;
        }


        /// <summary>
        /// Get all tiles in the list with a z at or below the z passed
        /// </summary>
        public List<GameObjectTile> GetAtOrBelow(int z)
        {
            List<GameObjectTile> toRet = new List<GameObjectTile>();
            foreach (int zKey in m_tilesByZ.Keys)
            {
                if (zKey <= z)
                {
                    toRet.AddRange(m_tilesByZ[zKey]);
                }
            }
            return toRet;
        }


        /// <summary>
        /// Get all tiles in the list with a z below the z passed
        /// </summary>
        public List<GameObjectTile> GetBelow(int z)
        {
            List<GameObjectTile> toRet = new List<GameObjectTile>();
            foreach (int zKey in m_tilesByZ.Keys)
            {
                if (zKey < z)
                {
                    toRet.AddRange(m_tilesByZ[zKey]);
                }
            }
            return toRet;
        }


        IEnumerator<GameObjectTile> IEnumerable<GameObjectTile>.GetEnumerator()
        {
            return m_tiles.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_tiles.GetEnumerator();
        }

    }
}
