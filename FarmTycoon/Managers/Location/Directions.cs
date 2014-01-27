using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    /// <summary>
    /// The four ordinal directions
    /// </summary>
    public enum OrdinalDirection
    {
        NorthEast,
        SouthEast,
        SouthWest,
        NorthWest
    }


    /// <summary>
    /// The four cardinal directions
    /// </summary>
    public enum CardinalDirection
    {
        North,
        East,
        South,
        West
    }



    public class DirectionUtils
    {
        public static OrdinalDirection[] AllOrdinalDirections = { OrdinalDirection.NorthEast, OrdinalDirection.SouthEast, OrdinalDirection.SouthWest, OrdinalDirection.NorthWest };

        public static CardinalDirection[] AllCardinalDirections = { CardinalDirection.North, CardinalDirection.East, CardinalDirection.South, CardinalDirection.West };
        
        /// <summary>
        /// Return an ordianl driection given its abreviation
        /// </summary>
        public static OrdinalDirection AbreviationToOrdinalDirection(string direction)
        {
            if (direction == "NE") { return OrdinalDirection.NorthEast; }
            else if (direction == "SE") { return OrdinalDirection.SouthEast; }
            else if (direction == "SW") { return OrdinalDirection.SouthWest; }
            else { return OrdinalDirection.NorthWest; }
        }


        /// <summary>
        /// Return the abrevation for the ordinal direction
        /// </summary>
        public static string OrdinalDirectionToAbreviation(OrdinalDirection direction)
        {
            if (direction == OrdinalDirection.NorthEast) { return "NE"; }
            else if (direction == OrdinalDirection.SouthEast) { return "SE"; }
            else if (direction == OrdinalDirection.SouthWest) { return "SW"; }
            else { return "NW"; }
        }

        /// <summary>
        /// Return the abrevation for the cardinal direction
        /// </summary>
        public static string CardinalDirectionFirstLetter(CardinalDirection direction)
        {
            return direction.ToString().Substring(0, 1);
        }


        /// <summary>
        /// Get the PathEffect Blocks flag that block walking toward the dirstion passed
        /// </summary>
        public static ObjectsEffectOnPath BlocksDirection(OrdinalDirection direction)
        {
            if (direction == OrdinalDirection.NorthEast)
            {
                return ObjectsEffectOnPath.BlocksNE;
            }
            else if (direction == OrdinalDirection.NorthWest)
            {
                return ObjectsEffectOnPath.BlocksNW;
            }
            else if (direction == OrdinalDirection.SouthEast)
            {
                return ObjectsEffectOnPath.BlocksSE;
            }
            else //if (direction == LandDirection.SouthWest)
            {
                return ObjectsEffectOnPath.BlocksSW;
            }
        }


        /// <summary>
        /// Get the PathEffect flag that says the prefered direction of travel for the object
        /// </summary>
        public static ObjectsEffectOnPath TravelDirection(OrdinalDirection direction)
        {
            if (direction == OrdinalDirection.NorthEast)
            {
                return ObjectsEffectOnPath.TravelNE;
            }
            else if (direction == OrdinalDirection.NorthWest)
            {
                return ObjectsEffectOnPath.TravelNW;
            }
            else if (direction == OrdinalDirection.SouthEast)
            {
                return ObjectsEffectOnPath.TravelSE;
            }
            else //if (direction == LandDirection.SouthWest)
            {
                return ObjectsEffectOnPath.TravelSW;
            }
        }

        public static OrdinalDirection OppositeDirection(OrdinalDirection direction)
        {
            if (direction == OrdinalDirection.NorthEast)
            {
                return OrdinalDirection.SouthWest;
            }
            else if (direction == OrdinalDirection.NorthWest)
            {
                return OrdinalDirection.SouthEast;
            }
            else if (direction == OrdinalDirection.SouthEast)
            {
                return OrdinalDirection.NorthWest;
            }
            else //if (direction == LandDirection.SouthWest)
            {
                return OrdinalDirection.NorthEast;
            }
        }

        public static CardinalDirection OppositeDirection(CardinalDirection direction)
        {
            if (direction == CardinalDirection.North)
            {
                return CardinalDirection.South;
            }
            else if (direction == CardinalDirection.West)
            {
                return CardinalDirection.East;
            }
            else if (direction == CardinalDirection.East)
            {
                return CardinalDirection.West;
            }
            else //if (direction == LandDirection.South)
            {
                return CardinalDirection.North;
            }
        }

        public static ViewDirection ClockwiseOne(ViewDirection direction)
        {
            return (ViewDirection)Enum.Parse(typeof(ViewDirection), ClockwiseOne((CardinalDirection)Enum.Parse(typeof(CardinalDirection), direction.ToString())).ToString());
        }
        public static ViewDirection CounterClockwiseOne(ViewDirection direction)
        {
            return (ViewDirection)Enum.Parse(typeof(ViewDirection), CounterClockwiseOne((CardinalDirection)Enum.Parse(typeof(CardinalDirection), direction.ToString())).ToString());
        }

        public static CardinalDirection ClockwiseOne(CardinalDirection direction)
        {
            if (direction == CardinalDirection.North)
            {
                return CardinalDirection.East;
            }
            else if (direction == CardinalDirection.East)
            {
                return CardinalDirection.South;
            }
            else if (direction == CardinalDirection.South)
            {
                return CardinalDirection.West;
            }
            else 
            {
                return CardinalDirection.North;
            }
        }

        public static CardinalDirection CounterClockwiseOne(CardinalDirection direction)
        {
            if (direction == CardinalDirection.North)
            {
                return CardinalDirection.West;
            }
            else if (direction == CardinalDirection.East)
            {
                return CardinalDirection.North;
            }
            else if (direction == CardinalDirection.South)
            {
                return CardinalDirection.East;
            }
            else 
            {
                return CardinalDirection.South;
            }
        }

        public static OrdinalDirection ClockwiseOne(OrdinalDirection direction)
        {
            if (direction == OrdinalDirection.NorthEast)
            {
                return OrdinalDirection.SouthEast;
            }
            else if (direction == OrdinalDirection.NorthWest)
            {
                return OrdinalDirection.NorthEast;
            }
            else if (direction == OrdinalDirection.SouthEast)
            {
                return OrdinalDirection.SouthWest;
            }
            else //if (direction == LandDirection.SouthWest)
            {
                return OrdinalDirection.NorthWest;
            }
        }

        public static OrdinalDirection CounterClockwiseOne(OrdinalDirection direction)
        {
            if (direction == OrdinalDirection.NorthEast)
            {
                return OrdinalDirection.NorthWest;
            }
            else if (direction == OrdinalDirection.NorthWest)
            {
                return OrdinalDirection.SouthWest;
            }
            else if (direction == OrdinalDirection.SouthEast)
            {
                return OrdinalDirection.NorthEast;
            }
            else //if (direction == LandDirection.SouthWest)
            {
                return OrdinalDirection.SouthEast;
            }
        }




        public static CardinalDirection ClockwiseOneCardinal(OrdinalDirection direction)
        {
            if (direction == OrdinalDirection.NorthEast)
            {
                return CardinalDirection.East;
            }
            else if (direction == OrdinalDirection.NorthWest)
            {
                return CardinalDirection.North;
            }
            else if (direction == OrdinalDirection.SouthEast)
            {
                return CardinalDirection.South;
            }
            else //if (direction == LandDirection.SouthWest)
            {
                return CardinalDirection.West;
            }
        }

        public static CardinalDirection CounterClockwiseOneCardinal(OrdinalDirection direction)
        {
            if (direction == OrdinalDirection.NorthEast)
            {
                return CardinalDirection.North;
            }
            else if (direction == OrdinalDirection.NorthWest)
            {
                return CardinalDirection.West;
            }
            else if (direction == OrdinalDirection.SouthEast)
            {
                return CardinalDirection.East;
            }
            else //if (direction == LandDirection.SouthWest)
            {
                return CardinalDirection.South;
            }
        }




        public static OrdinalDirection ClockwiseOneOrdinal(CardinalDirection direction)
        {
            if (direction == CardinalDirection.North)
            {
                return OrdinalDirection.NorthEast;
            }
            else if (direction == CardinalDirection.West)
            {
                return OrdinalDirection.NorthWest;
            }
            else if (direction == CardinalDirection.South)
            {
                return OrdinalDirection.SouthWest;
            }
            else //if (direction == CardinalDirection.East)
            {
                return OrdinalDirection.SouthEast;
            }
        }

        public static OrdinalDirection CounterClockwiseOneOrdinal(CardinalDirection direction)
        {
            if (direction == CardinalDirection.North)
            {
                return OrdinalDirection.NorthWest;
            }
            else if (direction == CardinalDirection.West)
            {
                return OrdinalDirection.SouthWest;
            }
            else if (direction == CardinalDirection.South)
            {
                return OrdinalDirection.SouthEast;
            }
            else //if (direction == CardinalDirection.East)
            {
                return OrdinalDirection.NorthEast;
            }
        }
    }
}
