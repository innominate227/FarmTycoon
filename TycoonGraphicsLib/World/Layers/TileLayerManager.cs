using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TycoonGraphicsLib
{

    public class TileLayerManager
    {
        /// <summary>
        /// All tiles that have been ordered mapped by position on the screen
        /// allows us to quickly determine what tiles we need to check ordering against
        /// </summary>
        private QuickMap _tilePositions;

        /// <summary>
        /// Graph of each tile where an edge between two tiles A->B means that tile A is in front of tile B and should be on a higher layer.
        /// </summary>
        private LayerGraph _layerGraph;

        /// <summary>
        /// World the tiles are a part of
        /// </summary>
        private World _world;


        internal TileLayerManager(World world)
        {                        
            _tilePositions = new QuickMap(world.WorldSettings.WorldSize, world.WorldSettings.WorldSize);            
            _layerGraph = new LayerGraph();
            _world = world;
        }


        /// <summary>
        /// Do delayed layering updates for all the tiles that had their layering changed
        /// </summary>
        public void DoDelayedLayerUpdates()
        {
            _layerGraph.DoDelayedLayerUpdates();
        }

        /// <summary>
        /// Delete layering info for the tile.  (This is safe to call even if the tile was never added to the layer manager)        
        /// </summary>
        public void DeleteTileLayerInfo(MobileTile tile)
        {
            //get the node for this tile            
            LayerGraphNode tileNode = tile.LayerGraphNode;

            //delete the current orderings for the tile, and make sure it is not in the delayed ordering list
            _layerGraph.DeleteOrderings(tileNode);
            
            //remove from the tile position structure
            _tilePositions.Remove(tile);
        }


        /// <summary>
        /// Update the layering info for the tile, and add (if it wasnt alreadt) to the layer manager so other tiles can move in front of it if nessisary
        /// </summary>
        public void UpdateTilesLayer(MobileTile tile)
        {
            //get the node for this tile            
            LayerGraphNode tileNode = tile.LayerGraphNode;

            //move this tile to the correct location in the position structure 
            //(will do an insert if the tile is not in the position structure yet)            
            ICollection<MobileTile> overlappingTiles = _tilePositions.MoveAndQuery(tile);
               
            //clear current orderings for the tile
            _layerGraph.ClearOrderings(tileNode);
                        
            //set the tile as infront or behind each overlapping tile
            foreach (MobileTile overlappingTile in overlappingTiles)
            {
                //ignore self
                if (overlappingTile == tile) { continue; }
                
                //add any ordering between the tile, and the tile is overlaps with
                AddOrdering(tile, overlappingTile);                                              
            }
        }

        /// <summary>
        /// Determine if tile should be in front of or behind overlapping tile.
        /// Then add the ordering to the layer graph.
        /// </summary>
        private void AddOrdering(MobileTile tile, MobileTile overlappingTile)
        {            
            //is one of us in front of the other
            bool meInFront = false;
            bool theyInFront = false;
            
            //see if there is a forced ordering
            if (tile.ForcedLayering.HasFlag(ForcedLayerType.Same) && overlappingTile.ForcedLayering.HasFlag(ForcedLayerType.Same))
            {
                //no one is in front
            }
            else if (tile.ForcedLayering.HasFlag(ForcedLayerType.InFront) && overlappingTile.ForcedLayering.HasFlag(ForcedLayerType.Behind))
            {
                //we are forced in front
                meInFront = true;
            }
            else if (tile.ForcedLayering.HasFlag(ForcedLayerType.Behind) && overlappingTile.ForcedLayering.HasFlag(ForcedLayerType.InFront))
            {
                //they are forced in front
                theyInFront = true;
            }
            else
            {
                //there is no forced order so do normal ordering

                //the difference between my X and their X (and the opposite)
                float myXdiff = tile.WorldX - overlappingTile.WorldX;
                float theirXdiff = overlappingTile.WorldX - tile.WorldX;
                
                //my and their Y loction in the world (account for edge factor)
                float myScreenY = tile.WorldY + (myXdiff * tile.EdgeFactor);
                float theirScreenY = overlappingTile.WorldY + (theirXdiff * overlappingTile.EdgeFactor);

                //whom ever has a higher Y is in front
                if (myScreenY > theirScreenY)
                {
                    meInFront = true;
                }
                else if (theirScreenY > myScreenY)
                {
                    theyInFront = true;
                }                
            }
            

            //if we decided on an ordering add the ordering
            if (meInFront)
            {
                _layerGraph.AddOrdering(tile.LayerGraphNode, overlappingTile.LayerGraphNode);
            }
            else if (theyInFront)
            {
                _layerGraph.AddOrdering(overlappingTile.LayerGraphNode, tile.LayerGraphNode);
            } 
            
        }
           

    }
}
