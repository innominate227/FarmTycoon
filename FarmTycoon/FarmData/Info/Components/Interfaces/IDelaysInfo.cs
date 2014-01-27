using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public interface IDelaysInfo : IInfo
    {    
        
        /// <summary>
        /// All the dealys infos that the info object has
        /// </summary>
        DelayInfoSet Delays
        {
            get;
        }
    }
}
