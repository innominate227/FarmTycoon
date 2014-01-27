using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public interface IScenarioScript
    {
        /// <summary>
        /// Called once each game day to allow script to examine and effect the game
        /// </summary>
        void DoScript(int day, ScriptGameInterface game);

        /// <summary>
        /// Called when game is saved so that script can save any nessisary state
        /// </summary>
        string SaveState();

        /// <summary>
        /// Called when game is loaded so that script can load any nessisary state
        /// </summary>
        void LoadState(string state);
    }
}
