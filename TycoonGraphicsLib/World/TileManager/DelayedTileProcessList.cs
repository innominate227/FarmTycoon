using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TycoonGraphicsLib
{
    /// <summary>
    /// A list of tiles that have been added/deleted/changed since the last frame.  
    /// The changes will need to be processed on the next frame.
    /// When a tile is added ProcessAddition/ProcessDeletion/ProcessChange will be called on the next frame.
    /// </summary>
    internal class DelayedTileProcessList
    {
        /// <summary>
        /// Reference to the game world the tile process list if a part of
        /// </summary>
        private World _world;


        /// <summary>
        /// List of tiles that have been deleted.
        /// We process deletions from one list while adding newly deleted tiles to the other.
        /// </summary>
        private HashSet<Tile> _deletedListA = new HashSet<Tile>();

        /// <summary>
        /// List of tiles that have been deleted.
        /// We process deletions from one list while adding newly deleted tiles to the other.
        /// </summary>
        private HashSet<Tile> _deletedListB = new HashSet<Tile>();

        /// <summary>
        /// List of tiles that have been changed.
        /// We process changes from one list while adding newly changed tiles to the other.
        /// </summary>
        private HashSet<Tile> _changeListA = new HashSet<Tile>();

        /// <summary>
        /// List of tiles that have been changed.
        /// We process changes from one list while adding newly changed tiles to the other.
        /// </summary>
        private HashSet<Tile> _changeListB = new HashSet<Tile>();

        
        /// <summary>
        /// Is list A the one that should be processed on the next frame.
        /// </summary>
        private volatile bool _processListA = true;


        /// <summary>
        /// Create a new DelayedTileProcessList to process changes to tiles in the world passed
        /// </summary>
        public DelayedTileProcessList(World world)
        {
            _world = world;
        }





        /// <summary>
        /// Add a tile to the list of tile that have changed.
        /// </summary>
        public void QueueForChange(Tile tile)
        {
            lock (this)
            {
                //add it to the list were not processing
                HashSet<Tile> changeList = _changeListA;
                if (_processListA) { changeList = _changeListB; }

                //tell the tile to make the changes ready for processing on the list were adding to (which is the list we are not processing)
                tile.MakeChangeReady(!_processListA);

                //dont add if its already in there
                if (changeList.Contains(tile) == false)
                {
                    changeList.Add(tile);
                }
            }
        }


        /// <summary>
        /// Add a tile to the list of tiles that have been deleted.
        /// </summary>
        public void QueueForDeletion(Tile tile)
        {
            lock (this)
            {
                //add it to the list were not processing
                HashSet<Tile> deleteList = _deletedListA;
                if (_processListA) { deleteList = _deletedListB; }

                //dont add if its already in there
                if (deleteList.Contains(tile) == false)
                {
                    deleteList.Add(tile);
                }
            }
        }




        /// <summary>
        /// Process all additions/deletions/changes that were delayed
        /// </summary>
        public void Process()
        {
            HashSet<Tile> changeList;
            HashSet<Tile> deletionList;
            lock (this)
            {
                //process the list that others have been adding to (and have them start adding to the other one)
                _processListA = !_processListA;

                //determine list to process
                changeList = _changeListA;
                deletionList = _deletedListA;
                if (_processListA == false)
                {
                    changeList = _changeListB;
                    deletionList = _deletedListB;
                }
            }

            //process changes
            foreach (Tile tile in changeList)
            {
                tile.ProcessChanges(_processListA);
            }

            //process deletions
            foreach (Tile tile in deletionList)
            {
                tile.ProcessDeletion();
            }
            

            //clear process lists            
            changeList.Clear();
            deletionList.Clear();
            
        }

        /// <summary>        
        /// Allows adding multiple items to the added/changed/deleted list with out the list being processed. 
        /// This way if multiple tiles must be processed together that can be done
        /// </summary>
        public void StartAddMultiple()
        {
            System.Threading.Monitor.Enter(this);
        }

        /// <summary>        
        /// Allows adding multiple items to the added/changed/deleted list with out the list being processed. 
        /// This way if multiple tiles must be processed together that can be done
        /// </summary>
        public void EndAddMultiple()
        {
            System.Threading.Monitor.Exit(this);
        }

    }
}
