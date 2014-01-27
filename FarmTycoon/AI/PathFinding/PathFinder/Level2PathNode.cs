using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    

    public class Level2PathNode
    {
        /// <summary>
        /// Clusts that the Path is within
        /// </summary>
        private List<Cluster> _clusters = new List<Cluster>();


        /// <summary>
        /// Clustser that the Path is within.
        /// Normally this will be 1 cluster if level 2 edge we witin a single cluster, or 2 cluster if the level2 edge was a door edge.
        /// This could be more for the special small distance case where we create the path at level 1 and then create a fake level 2 path.
        /// </summary>
        public List<Cluster> Clusters
        {
            get { return _clusters; }
        }


        /// <summary>
        /// The head of the level1 path we should take to get between the two level 2 path nodes
        /// </summary>
        public LocationPathNode Level1Path
        {
            get;
            set;
        }

        /// <summary>
        /// The next level 2 path node (or null if this is the last node in the path)
        /// </summary>
        public Level2PathNode Next
        {
            get;
            set;
        }

    }

    
}
