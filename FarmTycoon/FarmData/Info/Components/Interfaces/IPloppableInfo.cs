using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{
    /// <summary>
    /// An info object that can be plopped on the map.
    /// Must define the area that the object takes up as well as a name for the object
    /// </summary>
    public interface IPloppableInfo : IInfo
    {

        /// <summary>
        /// Name of the object
        /// </summary>
        string Name
        {
            get;
        }
        
        
        /// <summary>
        /// Locations the object is on (relative to the center location)
        /// </summary>
        List<RelativeLocation> LandOn
        {
            get;
        }


        /// <summary>
        /// Price type for checking the price to plop the building
        /// </summary>
        PriceType PriceType
        {
            get;
        }
    }
}
