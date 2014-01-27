using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public interface ITraitsInfo : IInfo
    {    
        
        /// <summary>
        /// All the trait infos that the info object has
        /// </summary>
        TraitInfoSet Traits
        {
            get;
        }
    }
}
