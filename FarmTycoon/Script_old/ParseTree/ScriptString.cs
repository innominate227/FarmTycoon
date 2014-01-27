using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// A number in the script.  All numbers can be precise or can be random
    /// </summary>
    public class ScriptString
    {
        /// <summary>
        /// Possible choices for the string
        /// </summary>
        private List<string> m_choices = new List<string>();

        /// <summary>
        /// Create a script string form text
        /// </summary>
        public ScriptString(string stringText)
        {
            //split the string on ';' to get the choices with the escape characters still in them
            string[] choicesEscaped = stringText.Split2(';', '\\');

            //remove escape characters form each of them
            foreach (string choiceEscaped in choicesEscaped)
            {
                //create choice fixed by copying each character except the escaped characters (escapt for escaped escaped characters)
                string choiceFixed = "";
                bool prevWasEscape = false;
                foreach (char chr in choiceEscaped)
                {
                    if (chr == '\\' && prevWasEscape == false)
                    {                        
                        //this will not be met if we have two escape characters in a row
                        //remeber that the previous was an escape character
                        prevWasEscape = true;                        
                    }
                    else if (chr == 'n' && prevWasEscape)
                    {
                        //copy a new line
                        choiceFixed += "\n";
                        prevWasEscape = false;

                    }
                    else
                    {
                        //copy character
                        choiceFixed += chr;
                        prevWasEscape = false;
                    }
                }

                //add fixed choice to the list
                m_choices.Add(choiceFixed);

            }
            
        }

        /// <summary>
        /// Get the value for the string, (pass a reference to the game so we can use its randomizer)
        /// </summary>
        public string GetValue()
        {
            int choiceIndex = Program.Game.Random.Next(m_choices.Count);
            return m_choices[choiceIndex];
        }

    }
}
