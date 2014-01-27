using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{

    /// <summary>
    /// This class contains name for special traits.
    /// an object can have a trait with any name, but certain componets of the game will look for traits with these names.
    /// </summary>
    public class SpecialTraits
    {

        /// <summary>
        /// Trait that tells the age of a animal or other object (age is special because it is shown in the items details, and can be set on object creation).
        /// </summary>
        public static int AGE_TRAIT = -1;


        /// <summary>
        /// Trait that tells the energy level of a worker
        /// </summary>
        public static int ENERGY_TRAIT = -1;


        /// <summary>
        /// Trait that tells the slope of a peice of land
        /// </summary>
        public static int SLOPE_TRAIT = -1;


        /// <summary>
        /// Trait that tells the amount of space in a pasture
        /// </summary>
        public static int SPACE_TRAIT = -1;


        /// <summary>
        /// Set the ids for specual traits
        /// </summary>        
        public static void SetSpecialTraitIds(InfoIds infoIds)
        {
            AGE_TRAIT = infoIds.GetTraitId("Age");
            ENERGY_TRAIT = infoIds.GetTraitId("Energy");
            SLOPE_TRAIT = infoIds.GetTraitId("Slope");
            SPACE_TRAIT = infoIds.GetTraitId("Space");
        }



    }
}
