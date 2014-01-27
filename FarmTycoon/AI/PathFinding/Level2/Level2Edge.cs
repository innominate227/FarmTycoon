using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// A level 2 path node inside a cluster.
    /// </summary>
    public class Level2Edge
    {
        /// <summary>
        /// The destination of this edge
        /// </summary>
        private Level2Node _destination;

        /// <summary>
        /// The cost to take this edge.
        /// </summary>
        private int _cost;

        /// <summary>
        /// Head of the level 1 path that can be taken to go from source to the destination
        /// </summary>
        private LocationPathNode _level1Path;



        /// <summary>
        /// Create a new level 2 edge.
        /// </summary>
        public Level2Edge(Level2Node destination, int cost, LocationPathNode level1PathHead)
        {
            _destination = destination;
            _cost = cost;
            _level1Path = level1PathHead;
        }
        

        /// <summary>
        /// The destination of this edge
        /// </summary>
        public Level2Node Destination
        {
            get { return _destination; }
        }

        /// <summary>
        /// The cost to take this edge.
        /// </summary>
        public int Cost
        {
            get { return _cost; }
        }
        
        /// <summary>
        /// Head of the level 1 path that can be taken to go from source to the destination.
        /// Null here will means that the two level 2 nodes connected have the same location.        
        /// </summary>
        public LocationPathNode Level1Path
        {
            get { return _level1Path; }
        }


    }
}
