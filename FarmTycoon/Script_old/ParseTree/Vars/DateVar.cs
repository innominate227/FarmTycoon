using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// Variable for the current date
    /// </summary>
    public class DateVar : ScriptVar
    {
        /// <summary>
        /// Name of the var in the script file
        /// </summary>
        public const string NAME = "DATE";


        public DateVar(string[] varParams)
        {
            Debug.Assert(varParams.Length == 0);
        }

        public override int GetValue()
        {
            return Program.Game.Calandar.Date;
        }

    }
}
