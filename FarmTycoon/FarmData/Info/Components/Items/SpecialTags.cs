using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{

    /// <summary>
    /// This class contains tags for items that have special meaning.
    /// and Item can have any tag, but certain componets of the game will look for these tags.
    /// </summary>
    public class SpecialTags
    {
        
        /// <summary>
        /// Tag that all ItemTypes will always have
        /// </summary>
        public const string ALL_TAG = "All";

        /// <summary>
        /// Can be put in a trough that does not already have AnimalFood
        /// </summary>
        public const string ANIMAL_FOOD_TAG = "AnimalFood";
        
        /// <summary>
        /// Can be put in a trough that does not already have AnimalWater
        /// </summary>
        public const string ANIMAL_WATER_TAG = "AnimalWater";

        /// <summary>
        /// Can be put sprayed onto a crop
        /// </summary>
        public const string SPRAY_TAG = "Spray";

        /// <summary>
        /// Is a seed (can be planted)
        /// </summary>
        public const string SEED_TAG = "Seed";

        /// <summary>
        /// Shows up in the store.  Will be grouped into sections pased on the part of the tag after the underscore.
        /// </summary>
        public const string STORE_TAG_PREFIX = "Store_";
    }
}
