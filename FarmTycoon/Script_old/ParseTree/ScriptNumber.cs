using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// A number in the script.  All numbers can be precise or can be random
    /// </summary>
    public class ScriptNumber
    {
        /// <summary>
        /// Possible choices for the number
        /// </summary>
        private List<int> m_choices = new List<int>();

        /// <summary>
        /// Create a script number form text
        /// </summary>
        public ScriptNumber(string numberText)
        {
            //split the string up into choices
            string[] choiceTokens = numberText.Split(';');

            //look at each choice
            foreach (string choiceToken in choiceTokens)
            {                
                if (choiceToken.Contains(":"))
                {
                    //its a range of choices
                    int choiceStart = int.Parse(choiceToken.Split(':')[0]);
                    int choiceEnd = int.Parse(choiceToken.Split(':')[1]);
                    for (int choice = choiceStart; choice <= choiceEnd; choice++)
                    {
                        m_choices.Add(choice);
                    }
                }
                else
                {
                    //its a singel number choice
                    int choice = int.Parse(choiceToken);
                    m_choices.Add(choice);
                }
            }
        }

        /// <summary>
        /// Get the value for the number, (pass a reference to the game so we can use its randomizer)
        /// </summary>
        public int GetValue()
        {
            if (m_choices.Count == 1) { return m_choices[0]; }

            int choiceIndex = Program.Game.Random.Next(m_choices.Count);
            
            return m_choices[choiceIndex];
        }

    }
}
