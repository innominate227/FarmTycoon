using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// Builds all location objects for an area of a certain size and heigth
    /// </summary>
    public class LocationBuilder
    {
        /// <summary>
        /// The columns keyed on y then x
        /// </summary>
        private Dictionary<int, Dictionary<int, Column>> m_columns = new Dictionary<int, Dictionary<int, Column>>();

        /// <summary>
        /// Get the column at x,y
        /// </summary>
        public Column GetColumn(int x, int y)
        {
            return m_columns[y][x];
        }


        /// <summary>
        /// Get the location at x,y,z
        /// </summary>
        public Location GetLocation(int x, int y, int z)
        {
            return m_locations[y][x][z];
        }


        /// <summary>
        /// Build location for a grid of size, with height height
        /// </summary>
        public void BuildLocations(int size, int height)
        {
            //make sure size is even (Size must be even, or wrap around wont work)
            Debug.Assert(size % 2 == 0);
            
            BuildColumns(size);
            BuildLocations2(size, height);
        }


        private void BuildColumns(int size)
        {
            //create columns first
            for (int y = 0; y < size; y++)
            {
                m_columns.Add(y, new Dictionary<int, Column>());
                for (int x = (y % 2); x < size; x += 2)
                {
                    Column newColumn = new Column(x, y);
                    m_columns[y].Add(x, newColumn);
                }
            }

            //set the adjacnet columns
            for (int y = 0; y < size; y++)
            {
                for (int x = (y % 2); x < size; x += 2)
                {
                    Column northWest = null;
                    Column northEast = null;
                    Column southWest = null;
                    Column southEast = null;
                    if (y > 0)
                    {
                        if (x > 0)
                        {
                            northWest = m_columns[y - 1][x - 1];
                        }
                        else
                        {
                            northWest = m_columns[y - 1][size - 1];
                        }
                        if (x < size - 1)
                        {
                            northEast = m_columns[y - 1][x + 1];
                        }
                        else
                        {
                            northEast = m_columns[y - 1][0];
                        }
                    }
                    else
                    {
                        if (x > 0)
                        {
                            northWest = m_columns[size - 1][x - 1];
                        }
                        else
                        {
                            northWest = m_columns[size - 1][size - 1];
                        }
                        if (x < size - 1)
                        {
                            northEast = m_columns[size - 1][x + 1];
                        }
                        else
                        {
                            northEast = m_columns[size - 1][0];
                        }
                    }
                    
                    if (y < size - 1)
                    {
                        if (x > 0)
                        {
                            southWest = m_columns[y + 1][x - 1];
                        }
                        else
                        {
                            southWest = m_columns[y + 1][size - 1];
                        }
                        if (x < size - 1)
                        {
                            southEast = m_columns[y + 1][x + 1];
                        }
                        else
                        {
                            southEast = m_columns[y + 1][0];
                        }
                    }
                    else
                    {
                        if (x > 0)
                        {
                            southWest = m_columns[0][x - 1];
                        }
                        else
                        {
                            southWest = m_columns[0][size - 1];
                        }
                        if (x < size - 1)
                        {
                            southEast = m_columns[0][x + 1];
                        }
                        else
                        {
                            southEast = m_columns[0][0];
                        }
                    }

                    m_columns[y][x].SetAdjacent(northEast, northWest, southEast, southWest);
                }
            }
        }

        private void BuildLocations2(int size, int height)
        {
            for (int y = 0; y < size; y++)
            {
                m_locations.Add(y, new Dictionary<int, Dictionary<int, Location>>());
                for (int x = (y % 2); x < size; x += 2)
                {
                    m_locations[y].Add(x, new Dictionary<int, Location>());
                    for (int z = 0; z < height; z++)
                    {
                        Location newLocation = new Location(x,y,z, m_columns[y][x]);
                        m_locations[y][x].Add(z, newLocation);
                    }
                }
            }
            

            //set the adjacnet
            for (int y = 0; y < size; y++)
            {
                for (int x = (y % 2); x < size; x += 2)
                {
                    for (int z = 0; z < height; z++)
                    {
                        Location northWest = null;
                        Location northEast = null;
                        Location southWest = null;
                        Location southEast = null;
                        Location above = null;
                        Location below = null;
                        if (y > 0)
                        {
                            if (x > 0)
                            {
                                northWest = m_locations[y - 1][x - 1][z];
                            }
                            else
                            {
                                northWest = m_locations[y - 1][size - 1][z];
                            }
                            if (x < size - 1)
                            {
                                northEast = m_locations[y - 1][x + 1][z];
                            }
                            else
                            {
                                northEast = m_locations[y - 1][0][z];
                            }
                        }
                        else
                        {
                            if (x > 0)
                            {
                                northWest = m_locations[size - 1][x - 1][z];
                            }
                            else
                            {
                                northWest = m_locations[size - 1][size - 1][z];
                            }
                            if (x < size - 1)
                            {
                                northEast = m_locations[size - 1][x + 1][z];
                            }
                            else
                            {
                                northEast = m_locations[size - 1][0][z];
                            }
                        }

                        if (y < size - 1)
                        {
                            if (x > 0)
                            {
                                southWest = m_locations[y + 1][x - 1][z];
                            }
                            else
                            {
                                southWest = m_locations[y + 1][size - 1][z];
                            }
                            if (x < size - 1)
                            {
                                southEast = m_locations[y + 1][x + 1][z];
                            }
                            else
                            {
                                southEast = m_locations[y + 1][0][z];
                            }
                        }
                        else
                        {
                            if (x > 0)
                            {
                                southWest = m_locations[0][x - 1][z];
                            }
                            else
                            {
                                southWest = m_locations[0][size - 1][z];
                            }
                            if (x < size - 1)
                            {
                                southEast = m_locations[0][x + 1][z];
                            }
                            else
                            {
                                southEast = m_locations[0][0][z];
                            }
                        }

                        if (z > 0)
                        {
                            below = m_locations[y][x][z - 1];
                        }
                        else
                        {
                            below = m_locations[y][x][height - 1];
                        }
                        if (z < height - 1)
                        {
                            above = m_locations[y][x][z + 1];                            
                        }
                        else
                        {
                            above = m_locations[y][x][0];
                        }

                        m_locations[y][x][z].SetAdjacent(northEast, northWest, southEast, southWest, above, below);
                    }
                }
            }



        }

    }
}
