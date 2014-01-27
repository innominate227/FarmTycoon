using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class RightAnglePathFinder
    {
        /// <summary>
        /// Find a right angle path from the startLand to the endLand.  pass a bool to get the other right angle path between the two lands
        /// 
        /// TODO: this should really act on locations instead of land (this would allow remocing adjacent data from Land object wich is taking up memory)
        /// can probably just be moved into location utils at that time
        /// </summary>
        public List<Land> FindPath(Land startLand, Land endLand, bool path2)
        {
            Land turnLand;
            bool straightPath;
            OrdinalDirection firstDirection, secondDirection;
            return FindPath(startLand, endLand, path2, out turnLand, out straightPath, out firstDirection, out secondDirection);
        }

        /// <summary>
        /// Find a right angle path from the startLand to the endLand
        /// </summary>
        public List<Land> FindPath(Land startLand, Land endLand, bool path2, out Land turnLand, out bool straightPath, out OrdinalDirection firstDirection, out OrdinalDirection secondDirection)
        {
            //determine the direction we need to go in the X 
            OrdinalDirection xDir = OrdinalDirection.SouthEast;
            if (endLand.LocationOn.X < startLand.LocationOn.X)
            {
                xDir = OrdinalDirection.NorthWest;
            }

            //determine the direction we need to go in the Y
            OrdinalDirection yDir = OrdinalDirection.SouthWest;
            if (endLand.LocationOn.Y < startLand.LocationOn.Y)
            {
                yDir = OrdinalDirection.NorthEast;
            }

            //determine which way we will go first, and wich way next
            firstDirection = xDir;
            secondDirection = yDir;
            if (path2)
            {
                firstDirection = yDir;
                secondDirection = xDir;
            }

            //list of the land to return
            List<Land> toRet = new List<Land>();
            toRet.Add(startLand);

            //go first direction until X is the same (or Y same for path2)
            Land landOn = startLand;
            bool movedInFirstDirection = false;
            while((landOn.LocationOn.X != endLand.LocationOn.X && path2 == false) || (landOn.LocationOn.Y != endLand.LocationOn.Y && path2))
            {
                movedInFirstDirection = true;
                landOn = landOn.GetAdjacent(firstDirection);
                toRet.Add(landOn);
            }

            //this is the turn land
            turnLand = landOn;
            
            //go second direction until we are at the end point            
            bool movedInSecondDirection = false;
            while(landOn != endLand)
            {
                movedInSecondDirection = true;
                landOn = landOn.GetAdjacent(secondDirection);
                toRet.Add(landOn);
            }

            //if we did not move in both directions its a straight path
            straightPath = (movedInFirstDirection == false || movedInSecondDirection == false);

            //return the path
            return toRet;
        }




    }
}
