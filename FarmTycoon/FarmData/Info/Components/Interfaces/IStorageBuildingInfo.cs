using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{
    public interface IStorageBuildingInfo : IInventoryInfo, IInfo
    {

        /// <summary>
        /// Delay for when items are retrieved from the building
        /// </summary>
        double GetDelayMultiplier
        {
            get;
        }

        /// <summary>
        /// Delay for when items are place into the building
        /// </summary>
        double PutDelayMultiplier
        {
            get;
        }
        
    }
}
