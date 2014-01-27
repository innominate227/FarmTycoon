using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Interface for a quality, allows treating a single quality or a composite of traits the same
    /// </summary>
    public interface IQuality : ITraitSet
    {
                    
        /// <summary>
        /// The current quality of the object
        /// </summary>
        int CurrentQuality
        {
            get;
        }

        

    }
}
