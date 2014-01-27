using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TycoonGraphicsLib
{

    /// <summary>
    /// A node for a tile in the layer graph
    /// </summary>
    public class LayerGraphNode
    {
        /// <summary>
        /// Tile this node determines position for
        /// </summary>
        private MobileTile _tile;

        /// <summary>
        /// Tiles that are in front of this tile  
        /// (List is actually faster than HashSet for very small sizes less than 20)
        /// </summary>
        private List<LayerGraphNode> _tilesInFrontOfThis = new List<LayerGraphNode>();
        //private HashSet<LayerGraphNode> _tilesInFrontOfThis = new HashSet<LayerGraphNode>();

        /// <summary>
        /// Tiles that are behind this tile   
        /// </summary>
        private List<LayerGraphNode> _tilesBehindThis = new List<LayerGraphNode>();
        //private HashSet<LayerGraphNode> _tilesBehindThis = new HashSet<LayerGraphNode>();

        /// <summary>
        /// Set to true when Updateing the tiles layer, and false again when leaving to help detect ordering cycles.
        /// If an ordering cycle is found we stop trying to order further on the path (we pretend that the edge that caused the cycle does not exist)
        /// </summary>
        private bool _orderCycleDetect = false;

        /// <summary>
        /// Create a new layer graph node
        /// </summary>
        public LayerGraphNode(MobileTile tile)
        {
            _tile = tile;
        }
        
        /// <summary>
        /// The Tile the node is for
        /// </summary>
        public MobileTile Tile
        {
            get { return _tile; }
        }
        
        /// <summary>
        /// Tiles that are in front of this tile  
        /// </summary>
        public List<LayerGraphNode> TilesInFrontOfThis
        {
            get { return _tilesInFrontOfThis; }
        }

        /// <summary>
        /// Tiles that are behind this tile
        /// </summary>
        public List<LayerGraphNode> TilesBehindThis
        {
            get { return _tilesBehindThis; }
        }

        

        /// <summary>
        /// Set the layer for this tile based on all tiles behind it.
        /// If this tiles layer is updated it will update the layers of the tiles in front of it.
        /// </summary>
        public void UpdateLayer()
        {
            //if we enter this method again it means we have a cycle
            _orderCycleDetect = true;

            //determine the layer for this node
            bool layerChanged = DetermineLayer();

            //if we hit a cycle we need to do a full layer check on our next update
            bool fullCheck = false;

            //see if our layer actually changed
            while (layerChanged)
            {
                //will be set back to true, if we hit a cycle and need to redermine our layer
                layerChanged = false;
                
                //tell each tile in front of this one to update, but they only need to recheck based on the change made in this tile, not changes in other layers
                foreach (LayerGraphNode tileInFrontOfThis in _tilesInFrontOfThis.ToArray())
                {
                    bool cycle = tileInFrontOfThis.UpdateLayerInner(this, fullCheck);
                    if (cycle)
                    {
                        //redertmine my layer
                        DetermineLayer();

                        //we will need to update children again
                        layerChanged = true;

                        //we need to do a full layer check this time.
                        fullCheck = true;

                        //break so we can start updating from the start of the list of children again
                        break;
                    }
                }

            }

            //we are exiting the method so clear cycle detect flag
            _orderCycleDetect = false;
        }
        


        /// <summary>
        /// Set the layer for this tile.  
        /// If this tiles layer is updated it will update the layers of the tiles in front of it.
        /// </summary>
        private bool UpdateLayerInner(LayerGraphNode parentThatChanged, bool fullCheck)
        {
            //we have found a cycle, remove the bad edge
            //note that this method is somewhere on the call stack right now.  When the call stack gets back to there it will still set order cycle detect to false.
            if (_orderCycleDetect)
            {
                //the node that called us is no longer considered behind us
                _tilesBehindThis.Remove(parentThatChanged);

                //we are no longer considered in front of the node that called us
                parentThatChanged._tilesInFrontOfThis.Remove(this);
                                
                //determine the layer for this node again since the edge we removed was probably causing a fast layer rise
                DetermineLayer();

                //retun true since we got into a cycle
                return true;
            }
            
            //if we enter this method again it means we have a cycle
            _orderCycleDetect = true;

            //set to true if we get into a cycle
            bool wasCycle = false;

            //set true if our layer changes
            bool layerChanged = false;

            if (fullCheck)
            {
                //if we need to do a full check look at all our parents to determine our layer
                layerChanged = DetermineLayer();
            }
            else if (this.Tile.Layer <= parentThatChanged.Tile.Layer && parentThatChanged.Tile.Layer < 299)
            {                
                //other wise we just increase our layer is the parent that changed has a layer higher or euqal to our current
                this.Tile.UpdateLayer(parentThatChanged.Tile.Layer + 1);                
                layerChanged = true;
            }

                        
            if (layerChanged)
            {
                //tell each tile in front of this one to update based on the fact that we just changed                
                foreach (LayerGraphNode tileInFrontOfThis in _tilesInFrontOfThis.ToArray())
                {
                    bool cycle = tileInFrontOfThis.UpdateLayerInner(this, fullCheck);
                    
                    //remember that we hit a cycle, and stop trying to fix children
                    if (cycle)
                    {
                        wasCycle = true;
                        break;
                    }
                }
            }

            //we are exiting the method so clear cycle detect flag
            _orderCycleDetect = false;

            return wasCycle;
        }




        /// <summary>
        /// Determine the layer for this node by looking at the layer of all parent nodes.
        /// Return if the layer for this node changed.
        /// </summary>
        private bool DetermineLayer()
        {
            //determine the maximum layer of the tiles behind this one
            int maxLayerOfTileBehindThis = 4;
            foreach (LayerGraphNode tileBehindThis in _tilesBehindThis)
            {
                if (maxLayerOfTileBehindThis < tileBehindThis.Tile.Layer)
                {
                    maxLayerOfTileBehindThis = tileBehindThis.Tile.Layer;
                }
            }

            //we should be one layer higher than that (since were in front)
            int layerForThisTile = maxLayerOfTileBehindThis + 1;

            //see if our layer actually changed
            if (_tile.Layer != layerForThisTile && layerForThisTile < 300)
            {
                //update the layer for the actual tile
                _tile.UpdateLayer(layerForThisTile);                

                //the layer changed
                return true;
            }

            //the layer did no change
            return false;
        }

    }
}
