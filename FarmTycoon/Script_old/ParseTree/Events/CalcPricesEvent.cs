using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Script event to adjust supply and demand in the market
    /// </summary>
    public class CalcPricesEvent : ScriptEvent
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "CALCPRICES";

        /// <summary>
        /// Create a calculate prices event
        /// </summary>
        public CalcPricesEvent(string[] actionParams)
        {
            Debug.Assert(actionParams.Length == 0);
        }


        public override void DoEvent(Game game)
        {
            game.Store.RecalculatePrices();
        }
 

    }
    

}
