using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Script event that makes the player win the scenario
    /// </summary>
    public class WinEvent : ScriptEvent
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "WIN";

        public WinEvent(string[] actionParams)
        {
            Debug.Assert(actionParams.Length == 0);
        }

        public override void DoEvent()
        {
            new MessageWindow("You WIN", "You WIN!", false, 150, 100);
        } 

    }   

}
