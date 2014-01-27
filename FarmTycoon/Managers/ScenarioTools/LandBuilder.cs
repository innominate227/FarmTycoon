using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// Builds a section of land of a certain size
    /// </summary>
    public class LandBuilder
    {

        /// <summary>
        /// Create a new land building to build land in the current game
        /// </summary>
        public LandBuilder()
        {
        }

        /// <summary>
        /// Build a section of land of a certain size
        /// </summary>
        public void BuildLand(int size)
        {
            ProgressWindow progressWindow = new ProgressWindow("Building Map");
            progressWindow.MaxValue = size * size;
            progressWindow.Progress = 0;

            //create land tiles at each location
            List<Land> createdLand = new List<Land>();
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Location locationForLand = GameState.Current.Locations.GetLocation(x, y);

                    Land newLand = new Land();
                    newLand.Setup(locationForLand);

                    createdLand.Add(newLand);

                    progressWindow.Progress += 1;
                }
            }
            
            //now set the slope and update tiles for all the land
            foreach (Land land in createdLand)
            {
                land.AdjustSlopeTrait();

                //refresh all land not that they know who their neighbors are
                land.UpdateTiles();                
            }

            progressWindow.CloseWindow();
        }

    }
}
