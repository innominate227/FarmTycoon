using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// For efficeny instead of string names, int ids are used for indentifing some info objects.  
    /// This class maps names to ids.  Note the id of an info could change between loads.  
    /// So Info object Ids should not be saved as part of the game state.  
    /// Also this is different from Save ID wich is just assigned and used at save time.
    /// </summary>
    public class InfoIds
    {
        /// <summary>
        /// Trait named mapped to its id
        /// </summary>
        private Dictionary<string, int> _ids = new Dictionary<string, int>();

        /// <summary>
        /// Get an ID for a trait with the name passed
        /// </summary>
        public int GetTraitId(string traitName)
        {
            if (_ids.ContainsKey(traitName) == false)
            {
                _ids.Add(traitName, _ids.Count);
            }
            return _ids[traitName];
        }

    }
}
