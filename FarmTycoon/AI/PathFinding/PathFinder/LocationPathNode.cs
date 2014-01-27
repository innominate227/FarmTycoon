using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    

    public class LocationPathNode
    {
        /// <summary>
        /// The Location
        /// </summary>
        public Location Location
        {
            get;
            set;
        }

        /// <summary>
        /// The next path node (or null if this is the last node in the path)
        /// </summary>
        public LocationPathNode Next
        {
            get;
            set;
        }


        ///// <summary>
        ///// Get the node at the start of the path that is the reserve of this path
        ///// </summary>
        //public LocationPathNode Reverse()
        //{
        //    //the new node we have created
        //    LocationPathNode newPathNodeOn = null;
        //    LocationPathNode newPathNodePrev = null;

        //    //walk the path until we get to the end
        //    LocationPathNode pathNodeOn = this;
        //    while (pathNodeOn != null)
        //    {
        //        //remeber the previous node 
        //        newPathNodePrev = newPathNodeOn;

        //        //create path node in the new path for the same location as this path
        //        newPathNodeOn = new LocationPathNode();
        //        newPathNodeOn.Location = pathNodeOn.Location;
        //        newPathNodeOn.Next = newPathNodePrev;

        //        //go to next node in path                
        //        pathNodeOn = pathNodeOn.Next;
        //    }

        //    //the last new path node created
        //    return newPathNodeOn;
        //}

    }


    
}
