using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// A level 2 path node inside a cluster.
    /// </summary>
    public class Level2Node
    {
        /// <summary>
        /// The location of this level node in the game world (level 1)
        /// </summary>
        private Location _location;

        /// <summary>
        /// The cluster this Level2Node resides in.
        /// </summary>
        private Cluster _cluster;
        
        /// <summary>
        /// Nodes adjacent to this level 2 node, and the edge that leads to them
        /// </summary>
        private Dictionary<Level2Node, Level2Edge> _adjacent = new Dictionary<Level2Node, Level2Edge>();


        /// <summary>
        /// Create a new level 2 node, in the cluster passed at the location passed
        /// </summary>
        public Level2Node(Cluster cluster, Location location)
        {
            _cluster = cluster;
            _location = location;
        }

        
        /// <summary>
        /// The location of this level node in the game world (level 1)
        /// </summary>
        public Location Location
        {
            get{ return _location;}
        }

        /// <summary>
        /// The cluster this Level2Node resides in.
        /// </summary>
        public Cluster Cluster
        {
            get{ return _cluster;}
        }
        
        /// <summary>
        /// Get a list of all edges leaving this node
        /// </summary>
        public ICollection<Level2Edge> Edges
        {
            get { return _adjacent.Values; }
        }
        
        /// <summary>
        /// Add an edge to another level 2 node
        /// </summary>
        public void AddEdge(Level2Edge edge)
        {
            _adjacent.Add(edge.Destination, edge);
        }
        
        /// <summary>
        /// Remove the edge to another level 2 node
        /// </summary>
        public void RemoveEdge(Level2Node edgeDestination)
        {
            _adjacent.Remove(edgeDestination);
        }


        /// <summary>
        /// Delete this level2 Node.  Remove the node from all nodes it is adjacent to.
        /// </summary>
        public void Delete()
        {
            foreach (Level2Node adjacentNode in _adjacent.Keys.ToArray())
            {
                //remove the edges to this node from adjacent nodes
                adjacentNode.RemoveEdge(this);

                //remove this nodes edges to adjacent nodes
                this.RemoveEdge(adjacentNode);
            }
        }

    }
}
