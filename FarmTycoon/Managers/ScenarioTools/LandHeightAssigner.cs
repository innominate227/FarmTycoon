using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    public class LandHeightAssigner
    {

        Random _rnd = new Random();

        public LandHeightAssigner()
        {
        }


        /// <summary>
        /// Assign random locations as hills.
        /// </summary>
        public void RandomHillPoints(int hillPoints, int minHeight, int maxHeight)
        {
            Program.Game.PathFinder.StartBatchInvalidate();

            List<Land> allLand = GameState.Current.MasterObjectList.FindAll<Land>();            
            
            for (int i = 0; i < hillPoints; i++)
            {
                //peice of land to raise
                Land hillCenter = allLand[_rnd.Next(allLand.Count)];

                //height to set the location to
                int hillHeight = _rnd.Next(minHeight, maxHeight);

                //corner to raise
                LandCorner raiseCorner = LandCorner.Center;
                if (_rnd.Next(2) == 0)
                {
                    raiseCorner = (LandCorner)_rnd.Next(1, 5);
                }

                //adjust the land
                AdjustLandToHeight(hillCenter, hillHeight, raiseCorner);
                                                
            }

            Program.Game.PathFinder.EndBatchInvalidate();
        }

        /// <summary>
        /// Adjust the land passed to height selected
        /// </summary>
        private void AdjustLandToHeight(Land land, int height, LandCorner corner)
        {
            //get the current height 
            int currentHeiht = land.LocationOn.Z;
            if (corner == LandCorner.North)
            {
                currentHeiht = land.GetHeight(CardinalDirection.North);
            }
            else if (corner == LandCorner.South)
            {
                currentHeiht = land.GetHeight(CardinalDirection.South);
            }
            else if (corner == LandCorner.West)
            {
                currentHeiht = land.GetHeight(CardinalDirection.West);
            }
            else if (corner == LandCorner.East)
            {
                currentHeiht = land.GetHeight(CardinalDirection.East);
            }
            
            //adjustment we will make
            int adjustment = height - currentHeiht;
            
            //adjust the land
            LandHeightAdjuster heightAdjuster = new LandHeightAdjuster();
            if (adjustment > 0)
            {
                for (int adjust = 0; adjust < adjustment; adjust++)
                {
                    heightAdjuster.RaiseLand(land, true, corner);
                }
            }
            else if (adjustment < 0)
            {
                for (int adjust = 0; adjust < adjustment*-1; adjust++)
                {
                    heightAdjuster.LowerLand(land, true, corner);
                }
            }
        }

        /// <summary>
        /// Adjust land height using the midpoint displacement algorithm
        /// </summary>
        public void ModpointDisplacement(float roughness, float smoothFactor, int minZ, int maxZ)
        {
            Program.Game.PathFinder.StartBatchInvalidate();

            //array to hold height of each peice of land
            int size = GameState.Current.Locations.Size;
            double[][] altitudes = new double[size + 2][];
            for(int i=0;i<size+2; i++)
            {
                altitudes[i] = new double[size + 2];
                for (int j = 0; j < size+2; j++)
                {
                    altitudes[i][j] = -1;
                }
            }
            
            //recursive divide the grid using the midpoint displacement algorithm
            double c1 = _rnd.NextDouble();
            double c2 = _rnd.NextDouble();
            double c3 = _rnd.NextDouble();
            double c4 = _rnd.NextDouble();
            DivideGrid(altitudes, roughness, smoothFactor, 0, 0, size + 2, size + 2, c1, c2, c3, c4);

            //get the range of the z values allowed
            int zRange = maxZ - minZ;

            //set land height based on the height of each cell
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Debug.Assert(altitudes[x + 1][y + 1] != -1);

                    //get height as an integer from min to max alt
                    int z = minZ + (int)Math.Round(altitudes[x+1][y+1] * zRange);                    

                    //adjust height of the land
                    Land land = GameState.Current.Locations.GetLocation(x, y).Find<Land>();
                    AdjustLandToHeight(land, z, LandCorner.North);

                    if (x == size - 1)
                    {                        
                        z = minZ + (int)Math.Round(altitudes[x + 2][y + 1] * zRange);                        
                        AdjustLandToHeight(land, z, LandCorner.East);
                    }
                    if (y == size - 1)
                    {
                        z = minZ + (int)Math.Round(altitudes[x + 1][y + 2] * zRange);
                        AdjustLandToHeight(land, z, LandCorner.West);
                    }
                    if (y == size - 1 && x == size - 1)
                    {
                        z = minZ + (int)Math.Round(altitudes[x + 2][y + 2] * zRange);
                        AdjustLandToHeight(land, z, LandCorner.South);
                    }

                }
            }


            Program.Game.PathFinder.EndBatchInvalidate();
        }
        
        /// <summary>
        /// Adjust land height using the midpoint displacement algorithm
        /// </summary>
        private void DivideGrid(double[][] altitudes, double roughness, double smooth, int x, int y, int width, int height, double c1, double c2, double c3, double c4)
        {            
            if (width > 1 || height > 1)
            {
                //mid points for each line segmnet between a pair of two corner, takes the height of the average of the two corners
                double mid1 = ((c1 + c2) / 2);
                double mid2 = ((c2 + c3) / 2);
                double mid3 = ((c3 + c4) / 2);
                double mid4 = ((c4 + c1) / 2);

                //ceter is the average of all 4 plus a random offset
                double randomOffset = (_rnd.NextDouble() - 0.5) * roughness;
                double center = ((c1 + c2 + c3 + c4) / 4) + randomOffset;
                if (center < 0) { center = 0; }
                if (center > 1) { center = 1; }

                //smooth out random offset size as we go ddeper
                roughness = roughness / smooth;
                
                //cut region in half
                int newWidth = width / 2;
                int newHeight = height / 2;

                //but if it does not cut perfectly in half some sub regions will be one bigger
                int biggerNewWidth = width - newWidth;
                int biggerNewHeight = height - newHeight;

                //repeate on subgrids           
                DivideGrid(altitudes, roughness, smooth, x,            y,             newWidth,       newHeight,       c1, mid1, center, mid4);
                DivideGrid(altitudes, roughness, smooth, x + newWidth, y,             biggerNewWidth, newHeight,       mid1, c2, mid2, center);
                DivideGrid(altitudes, roughness, smooth, x + newWidth, y + newHeight, biggerNewWidth, biggerNewHeight, center, mid2, c3, mid3);
                DivideGrid(altitudes, roughness, smooth, x,            y + newHeight, newWidth,       biggerNewHeight, mid4, center, mid3, c4);
            }
            else    
            {
                //we cannot sub divide further
                //set the point to the average of the 4 corners
                if (x > 0 && x < altitudes.Length && y > 0 && y < altitudes.Length)
                {
                    altitudes[x][y] = (c1 + c2 + c3 + c4) / 4;
                }
            }
        }


        /// <summary>
        /// creates a river or ridge by walking the path between start land and end land, and setting tiles to have height between max and min.
        /// The size of the square adjusted is between min and max,  the change that the river will go left or right instead of toward the destination is curve probability
        /// </summary
        public void RiverAlgorithm(Land startLand, Land endLand, int minZ, int maxZ, int minSize, int maxSize, double curveProbability)
        {
            Land landOn = startLand;
            while (landOn != endLand)
            {
                //randomly choose height to adjust land to, and size of land square to adjust
                int height = _rnd.Next(minZ, maxZ + 1);
                int size = _rnd.Next(minSize, maxSize + 1);

                //collect the land to adjust
                List<Location> collectedLocations = LocationUtils.CollectLocations(landOn.LocationOn, CardinalDirection.North, size);
                List<Land> collectedLands = LocationUtils.FindAll<Land>(collectedLocations);

                //adjust it to that height
                foreach (Land collectedLand in collectedLands)
                {
                    AdjustLandToHeight(collectedLand, height, LandCorner.Center);
                }

                //find the direction we should travel to get to the end land.
                //we travel in one of the two direction that we could travel to get there with the probablitly that we travel in that direction being equal to how much we have to travel in that direction
                OrdinalDirection directionToTravel;
                int xDiff = landOn.LocationOn.X - endLand.LocationOn.X;
                int yDiff = landOn.LocationOn.Y - endLand.LocationOn.Y;
                bool travelX = _rnd.Next(Math.Abs(xDiff) + Math.Abs(yDiff)) < Math.Abs(xDiff);

                if (travelX)
                {
                    if (xDiff > 0) { directionToTravel = OrdinalDirection.NorthWest; }
                    else { directionToTravel = OrdinalDirection.SouthEast; }
                }
                else
                {
                    if (yDiff > 0) { directionToTravel = OrdinalDirection.NorthEast; }
                    else { directionToTravel = OrdinalDirection.SouthWest; }
                }


                
                //we travel in that direction unless we randomly decide to curve                
                double curve = _rnd.NextDouble();
                if (curve < curveProbability)
                {
                    //propose a new direction with even change of either
                    OrdinalDirection proposedDirection;
                    if (curve < curveProbability / 2)
                    {
                        proposedDirection = DirectionUtils.ClockwiseOne(directionToTravel);
                    }
                    else
                    {
                        proposedDirection = DirectionUtils.CounterClockwiseOne(directionToTravel);
                    }

                    //only take the turn if it does not wrap us around the edge of the world
                    if (LocationUtils.WrapsAroundWorldEdge(landOn.LocationOn, proposedDirection) == false)
                    {
                        directionToTravel = proposedDirection;
                    }
                }

                Debug.Assert(LocationUtils.WrapsAroundWorldEdge(landOn.LocationOn, directionToTravel) == false);


                //get the land in that direction
                landOn = landOn.GetAdjacent(directionToTravel);
            }
        }



    }
}
