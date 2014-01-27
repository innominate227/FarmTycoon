using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// Var that is just a constant number
    /// </summary>
    public class ConstantVar : ScriptVar
    {
        /// <summary>
        /// Name of the var in the script file
        /// </summary>
        public const string NAME = "CONSTANT";

        /// <summary>
        /// Value of the constant
        /// </summary>
        private ScriptNumber m_value;


        public ConstantVar(string[] varParams)
        {
            Debug.Assert(varParams.Length == 1);

            m_value = new ScriptNumber(varParams[0]);
        }

        public override int GetValue()
        {
            return m_value.GetValue();
        }

    }
}
