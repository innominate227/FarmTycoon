using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{
    /// <summary>
    /// Interface that must be implemented by all Info objects
    /// </summary>
    public interface IInfo
    {        
        /// <summary>
        /// Unique name of the info object
        /// </summary>
        string UniqueName
        {
            get;
        }  
      
    }
}
