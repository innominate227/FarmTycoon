using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public interface IEventsInfo : IInfo
    {    
        
        /// <summary>
        /// All the event infos that the info object has
        /// </summary>
        ObjectEventInfoSet Events
        {
            get;
        }
    }
}
