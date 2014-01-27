using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    public enum Operator
    {
        None,
        Plus,
        Minus,
        Multiply,
        Divide,
        Modulus
    }

    /// <summary>
    /// Script event to set the value of a script variable
    /// </summary>
    public class SetVariableEvent : ScriptEvent
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "SETVARIABLE";

        /// <summary>
        /// The variable to set
        /// </summary>
        private ScriptString m_name;

        /// <summary>
        /// The left hind side of the epxression to set it to (or if there are not two side the entire expression)
        /// </summary>
        private ScriptVar m_left;

        /// <summary>
        /// The right hind side of the epxression to set it to (or null if there are not two side to the expression)
        /// </summary>
        private ScriptVar m_right;

        /// <summary>
        /// Operator the epxression uses
        /// </summary>
        private Operator m_operator;
        
        
        /// <summary>
        /// Create a SetItemPriceEvent
        /// </summary>
        public SetVariableEvent(string[] actionParams)
        {
            Debug.Assert(actionParams.Length == 2);
            m_name = new ScriptString(actionParams[0]);
            
            //determine what type of expression it is and the character that will split it
            string exprerssion = actionParams[1];
            char splitChar = '\0';
            if (exprerssion.Contains("+"))
            {
                m_operator = Operator.Plus;
                splitChar = '+';
            }
            else if (exprerssion.Contains("-"))
            {
                m_operator = Operator.Plus;
                splitChar = '-';
            }
            else if (exprerssion.Contains("*"))
            {
                m_operator = Operator.Multiply;
                splitChar = '*';
            }
            else if (exprerssion.Contains("/"))
            {
                m_operator = Operator.Divide;
                splitChar = '/';
            }
            else if (exprerssion.Contains("%"))
            {
                m_operator = Operator.Modulus;
                splitChar = '%';
            }
            else
            {
                m_operator = Operator.None;
                splitChar = '\0';
            }

            
            //split the epxression into a left half and a right half
            string leftHandSide = "";
            string rightHandSide = "";
            if (splitChar == '\0')
            {
                leftHandSide = exprerssion;
            }
            else
            {
                leftHandSide = exprerssion.Split(splitChar)[0];
                rightHandSide = exprerssion.Split(splitChar)[1];
            }
                        

            //create Var for each side
            VarFactory varFactory = new VarFactory();
            m_left = varFactory.Create(leftHandSide);
            if (rightHandSide != "")
            {
                m_right = varFactory.Create(rightHandSide);
            }
            else
            {
                m_right = null;
            }
        }


        public override void DoEvent()
        {
            int leftHandSide = m_left.GetValue();
            int rightHandSide = 0;
            if (m_right != null)
            {
                rightHandSide = m_right.GetValue();
            }

            //get the value after appluing the operator
            int reultValue = 0;
            if (m_operator == Operator.None)
            {
                reultValue = leftHandSide;
            }
            else if (m_operator == Operator.Plus)
            {
                reultValue = leftHandSide + rightHandSide;
            }
            else if (m_operator == Operator.Minus)
            {
                reultValue = leftHandSide - rightHandSide;
            }
            else if (m_operator == Operator.Multiply)
            {
                reultValue = leftHandSide * rightHandSide;
            }
            else if (m_operator == Operator.Divide)
            {
                reultValue = leftHandSide / rightHandSide;
            }
            else if (m_operator == Operator.Modulus)
            {
                reultValue = leftHandSide % rightHandSide;
            }
            
            //set the value of the variable
            string variable = m_name.GetValue();
            Program.Game.ScriptPlayer.SetVariableValue(variable, reultValue);
        }



 

    }
    

}
