using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Creates the correct var given a string representation of the var
    /// </summary>
    public class VarFactory
    {
        /// <summary>
        /// Create a var given the string for the var.
        /// </summary>
        public ScriptVar Create(string varString)
        {
            //if its just a number not, then treat it as a constant var
            if (varString.Contains("(") == false)
            {
                int unused;
                if (int.TryParse(varString.Trim(), out unused))
                {
                    //if its numeric create a constant var
                    return new ConstantVar(new string[] { varString.Trim() });
                }
                else
                {
                    //if its non-numeric create a variable var
                    return new VariableVar(new string[] { varString.Trim() });
                }
            }
            
            //get the name of the var
            string varName = varString.Substring(0, varString.IndexOf("(")).Trim().ToUpper();

            //get params in a string[] and trim extra spaces
            string varParamsText = varString.Substring(varString.IndexOf("(") + 1).Trim();
            varParamsText = varParamsText.Substring(0, varParamsText.Length - 1);
            string[] varParams = varParamsText.Split2(',', '\\');
            for (int paramNum = 0; paramNum < varParams.Length; paramNum++)
            {
                varParams[paramNum] = varParams[paramNum].Trim();
            }

            //if we have one empty string paramter, we actualy had no parameters
            if (varParams.Length == 1 && varParams[0].Trim() == "")
            {
                varParams = new string[0];
            }

            //create the correct var
            ScriptVar newVar = null;
            if (varName == ConstantVar.NAME)
            {
                newVar = new ConstantVar(varParams);
            }
            else if (varName == PriceOfVar.NAME)
            {
                newVar = new PriceOfVar(varParams);
            }
            else if (varName == PlayerItemAmountVar.NAME)
            {
                newVar = new PlayerItemAmountVar(varParams);
            }
            else if (varName == StoreItemAmountVar.NAME)
            {
                newVar = new StoreItemAmountVar(varParams);
            }
            else if (varName == PlayerMoneyVar.NAME)
            {
                newVar = new PlayerMoneyVar(varParams);
            }
            else if (varName == ObjectCountVar.NAME)
            {
                newVar = new ObjectCountVar(varParams);
            }
            else if (varName == VariableVar.NAME)
            {
                newVar = new VariableVar(varParams);
            }
            else if (varName == DateVar.NAME)
            {
                newVar = new DateVar(varParams);
            }
            else
            {
                Debug.Assert(false);
            }

            //return the newly created event
            return newVar;
        }

    }
}
