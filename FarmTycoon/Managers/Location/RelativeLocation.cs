using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// The class is used to find a location relative to another location.
    /// It is a list of directions that can be applied to a location to get another location.
    /// </summary>
    public class RelativeLocation
    {
        /// <summary>
        /// The list of directions that are applied to apply the realtive location
        /// </summary>
        private List<OrdinalDirection> _directions = new List<OrdinalDirection>();

        /// <summary>
        /// Create a realtive location form a relative location string
        /// </summary>
        public RelativeLocation(string realtiveLocationString)
        {
            foreach (string realtiveLocationStringPart in realtiveLocationString.Split('-'))
            {
                if (realtiveLocationStringPart.Trim().ToUpper() == "C") { continue; }
                _directions.Add(DirectionUtils.AbreviationToOrdinalDirection(realtiveLocationStringPart.Trim().ToUpper()));
            }
        }

        /// <summary>
        /// Get the realtive location given a starting location
        /// </summary>
        public Location GetRealtiveLocation(Location startLocation)
        {
            Location location = startLocation;
            foreach (OrdinalDirection direction in _directions)
            {
                location = location.GetAdjacent(direction);
            }
            return location;
        }

        public static List<RelativeLocation> CreateRealativeLocationList(string relativeLocationStrings)
        {
            List<RelativeLocation> relativeLocations = new List<RelativeLocation>();
            foreach (string relativeLocationString in relativeLocationStrings.Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries))
            {
                relativeLocations.Add(new RelativeLocation(relativeLocationString.Trim()));
            }
            return relativeLocations;
        }

    }
}
