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
    public class LoseEvent : ScriptEvent
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "LOSE";

        public LoseEvent(string[] actionParams)
        {
            Debug.Assert(actionParams.Length == 0);
        }

        public override void DoEvent()
        {
            new MessageWindow("You Lose", "You Lose :(", false, 150, 100);
        } 
 

    }   

}
