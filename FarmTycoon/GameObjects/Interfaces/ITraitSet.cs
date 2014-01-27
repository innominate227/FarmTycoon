using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Interface for a trait set that could actually be a quality, or composite quality object
    /// </summary>
    public interface ITraitSet
    {

        /// <summary>
        /// Get a list of all trait ids in the set
        /// </summary>
        int[] TraitIds
        {
            get;
        }
              
        /// <summary>
        /// Get the value of a trait
        /// </summary>
        int GetTraitValue(int traitId);

        /// <summary>
        /// Set the value of a trait
        /// </summary>
        void SetTraitValue(int traitId, int value);

        /// <summary>
        /// Get the info for a trait
        /// </summary>
        TraitInfo GetTraitInfo(int traitId);
    }
}
