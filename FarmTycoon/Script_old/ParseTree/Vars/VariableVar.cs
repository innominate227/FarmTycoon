using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Var for the value of a script variable
    /// </summary>
    public class VariableVar : ScriptVar
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "VARIABLE";
        
        /// <summary>
        /// The name of the variable to get the value of
        /// </summary>
        private ScriptString m_name;



        public VariableVar(string[] actionParams)
        {
            Debug.Assert(actionParams.Length == 1);
            m_name = new ScriptString(actionParams[0]);
        }


        public override int GetValue()
        {
            string variableName = m_name.GetValue();
            return Program.Game.ScriptPlayer.GetVariableValue(variableName);
        }
 

    }
    

}
