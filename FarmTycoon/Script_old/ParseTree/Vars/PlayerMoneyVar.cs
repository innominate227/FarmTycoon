using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// Variable for how much money the player currently has
    /// </summary>
    public class PlayerMoneyVar : ScriptVar
    {
        /// <summary>
        /// Name of the var in the script file
        /// </summary>
        public const string NAME = "PLAYERMONEY";
        

        public PlayerMoneyVar(string[] varParams)
        {
            Debug.Assert(varParams.Length == 0);
        }

        public override int GetValue()
        {
            return Program.Game.Treasury.CurrentMoney;
        }

    }
}
