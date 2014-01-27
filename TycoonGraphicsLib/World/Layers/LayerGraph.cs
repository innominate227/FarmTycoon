using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TycoonGraphicsLib
{
    /// <summary>
    /// Graph of what tiles are in front of what other tiles.  
    /// The parents a node determines the layer that a tile should be on.  Nodes with no parents are on layer 0.  
    /// Other nodes are on a layer 1 greater than the gratest layer of their parents.
    /// The graph is "acyclic".  Where acyclic is in quotes because cycles can be added, they are just removed eventually before layers are redetermined.    
    /// </summary>
    internal class LayerGraph
    {
        /// <summary>
        /// Graph nodes that need to have their layer (depth) updated.  
        /// We delay updating depth so that several tree modifications can be made before the depth needs to be redetermined.
        /// </summary>
        private HashSet<LayerGraphNode> _delayedLayerUpdate = new HashSet<LayerGraphNode>();


        
        /// <summary>
        /// Preform delayed layer updates based on the ordering added or removed
        /// </summary>
        public void DoDelayedLayerUpdates()
        {
            //update layers
            foreach(LayerGraphNode layerNode in _delayedLayerUpdate)
            {
                layerNode.UpdateLayer();
            }

            //clear dealyed layers
            _delayedLayerUpdate.Clear();
        }





        /// <summary>
        /// Set the tile "inFront" to be in front of the tile "behind" in the layer graph.
        /// </summary>
        public void AddOrdering(LayerGraphNode inFrontNode, LayerGraphNode behindNode)
        {
            //add the "in front node" to the list of node in front of the "behind node"
            behindNode.TilesInFrontOfThis.Add(inFrontNode);

            //add the "behind node" to the list of nodes in behind the "in front node" 
            inFrontNode.TilesBehindThis.Add(behindNode);

            //we need to check if the layer on the tile that is in front is still correct.
            //but we want to wait for several orderings to be added before we check.
            if (_delayedLayerUpdate.Contains(inFrontNode) == false)
            {
                _delayedLayerUpdate.Add(inFrontNode);
            }
        }


        /// <summary>
        /// Remove all orderings for the tile passed.  
        /// It will no longer be considered infront of, or behind any tiles until AddOrder is called with the tile.
        /// It will be deleted completly from the layer graph.  The layer of the tile will NOT be update on the next DoDelayedLayerUpdates()
        /// or any subsequent DoDelayedLayerUpdates()
        /// </summary>
        public void DeleteOrderings(LayerGraphNode tileNode)
        {
            //remove tiles ordering with other tiles
            RemoveOrderings(tileNode);

            //if we were planning to redtermine the layer for this tile dont
            if (_delayedLayerUpdate.Contains(tileNode))
            {
                _delayedLayerUpdate.Remove(tileNode);
            }
        }


        /// <summary>
        /// Remove all orderings for the tile passed.  
        /// It will no longer be considered infront of, or behind any tiles until AddOrder is called with the tile.
        /// It will still be "in" the layer graph, and the layer of the tile will be updated on the next DoDelayedLayerUpdates()
        /// </summary>
        public void ClearOrderings(LayerGraphNode tileNode)
        {
            //remove tiles ordering with other tiles
            RemoveOrderings(tileNode);

            //we need to redtermine the layer for this tile            
            if (_delayedLayerUpdate.Contains(tileNode) == false)
            {
                _delayedLayerUpdate.Add(tileNode);
            }   
        }


        /// <summary>
        /// Remove all orderings for the tile passed.  
        /// It will no longer be considered infront of, or behind any tiles until AddOrder is called with the tile.        
        /// </summary>
        public void RemoveOrderings(LayerGraphNode tileNode)
        {
            //foreach tile in behind this tile, this tile is no longer in in front of them.
            foreach (LayerGraphNode tileBehindThis in tileNode.TilesBehindThis)
            {
                tileBehindThis.TilesInFrontOfThis.Remove(tileNode);
            }

            //foreach tile in front of this tile, this tile is no longer behind of them.
            foreach (LayerGraphNode tileInFrontOfThis in tileNode.TilesInFrontOfThis)
            {
                tileInFrontOfThis.TilesBehindThis.Remove(tileNode);

                //these tiles will need to redetermine their layer, as they may be able to move forward a layer now
                //but wait to redertermine until several operations have happened
                if (_delayedLayerUpdate.Contains(tileInFrontOfThis) == false)
                {
                    _delayedLayerUpdate.Add(tileInFrontOfThis);
                }
            }

            //this tile is no longer in front of or behind any other tiles
            tileNode.TilesBehindThis.Clear();
            tileNode.TilesInFrontOfThis.Clear();
        }

    }
}
