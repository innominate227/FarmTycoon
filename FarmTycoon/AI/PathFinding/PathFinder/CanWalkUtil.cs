using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{

    /// <summary>
    /// Constain methods to determine if two adjacent locations can be walked between, and the cost to walk between them
    /// </summary>
    public class CanWalkUtil
    {

        /// <summary>
        /// Cost to walk between two locations that both have highways
        /// </summary>
        public const int HIGHWAY_COST = 10;

        /// <summary>
        /// Cost to walk between two locations that both have roads
        /// </summary>
        public const int ROAD_COST = 15;

        /// <summary>
        /// Cost to walk between two locations that do not both have roads
        /// </summary>
        public const int NORMAL_COST = 20;

        /// <summary>
        /// Cost to walk between two locations where one of them is a prefered dont walk zone
        /// </summary>
        public const int DONT_WALK_COST = 500;



        /// <summary>
        /// Return if you can walk from start to the location in the direction passed.
        /// This assumes the start location does not contains a solid object
        /// </summary>
        public static bool CanWalkTo(Location start, OrdinalDirection direction, out int cost)
        {
            //return cost as max if we find that we cant walk between
            cost = int.MaxValue;

            //get the adjacent location
            Location end = start.GetAdjacent(direction);

            //the cost it will be to walk assuming we can walk
            int costToWalk = NORMAL_COST;
            
            //get all start location effects
            ObjectsEffectOnPath startLocationEffects = start.CumulativeEffectOnPath;
            
            //need to check that start location to make sure no object blocks walking toward the direction we want to go
            ObjectsEffectOnPath startBlockingBorderEffect = DirectionUtils.BlocksDirection(direction);

            //if a object in the start location prevents walking in that direction we cannot walk                
            if (startLocationEffects.HasFlag(startBlockingBorderEffect)) { return false; }
            
            //get all end location effects
            ObjectsEffectOnPath endLocationEffects = end.CumulativeEffectOnPath;
            
            //if end location contains solid object we cannot walk there
            if (endLocationEffects.HasFlag(ObjectsEffectOnPath.Solid)) { return false; }
            
            //need to check the the end location does not block walking in from the direction we are coming from
            ObjectsEffectOnPath endBlockingBorderEffect = DirectionUtils.BlocksDirection(DirectionUtils.OppositeDirection(direction));
            
            //if end location contains object that blocks that direction we cannot walk there
            if (endLocationEffects.HasFlag(endBlockingBorderEffect)) { return false; }

            //determine the effect for the direction of travel, and opposite direction of travel from the direction we are going            
            ObjectsEffectOnPath oppositeDirectionOfTravel = DirectionUtils.TravelDirection(DirectionUtils.OppositeDirection(direction));

            //if we are walking the opposide of the direction of travel treat it as a dont walk
            if (endLocationEffects.HasFlag(oppositeDirectionOfTravel))
            {
                costToWalk = DONT_WALK_COST; 
            }
            //if there is an object that wants you to NOT walk then raise cost
            else if (endLocationEffects.HasFlag(ObjectsEffectOnPath.DontWalk)) 
            {
                costToWalk = DONT_WALK_COST; 
            }
            //if there is an object that wants you TO walk then lower cost
            else if (endLocationEffects.HasFlag(ObjectsEffectOnPath.DoWalk))
            {
                costToWalk = ROAD_COST;
            }
            //if we are on highway lower the cost alot
            else if (endLocationEffects.HasFlag(ObjectsEffectOnPath.DoWalkPlus))
            {
                costToWalk = HIGHWAY_COST;
            }
                          
            cost = costToWalk;
            return true;
        }



        /// <summary>
        /// Return if you can walk from start to the location in the direction passed.
        /// This also checks that the start location does not contains a solid object
        /// </summary>
        public static bool CanWalkBetween(Location start, OrdinalDirection direction)
        {
            int unused;
            if (CanWalkTo(start, direction, out unused) == false)
            {
                return false;
            }
            
            //check that there are no solid objects in that start location            
            foreach (GameObject obj in start.AllObjects)
            {                
                if (obj.PathEffect.HasFlag(ObjectsEffectOnPath.Solid)) { return false; }
            }

            //we can walk between these tiles
            return true;
        } 
    }
}
