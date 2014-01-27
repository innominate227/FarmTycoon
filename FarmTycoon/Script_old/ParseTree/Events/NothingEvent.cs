using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Script event that does nothing
    /// </summary>
    public class NothingEvent : ScriptEvent
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "NOTHING";

        /// <summary>
        /// Create a calculate prices event
        /// </summary>
        public NothingEvent(string[] actionParams)
        {
            Debug.Assert(actionParams.Length == 0);
        }

        public override void DoEvent()
        {
            //do nothing
        }
 

    }   

}
