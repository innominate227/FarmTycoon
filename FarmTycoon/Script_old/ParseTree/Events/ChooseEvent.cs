using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Special event that causes on of a list of event to be chosen from and executed
    /// </summary>
    public class ChooseEvent : ScriptEvent
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "CHOOSE";
        
        /// <summary>
        /// Script events
        /// </summary>
        private List<ScriptRegion> m_choices = new List<ScriptRegion>();
                
        /// <summary>
        /// Create a supply evnet
        /// </summary>
        public ChooseEvent(string chooseSection)
        {
            //get line 1
            string line1 = chooseSection.Substring(0, chooseSection.IndexOf("\r\n"));
            Debug.Assert(line1.Trim().ToUpper() == NAME);
            
            //get all but line 1,2 and the last line, which should be a bunch of regions
            string choicesSection = chooseSection.Substring(chooseSection.IndexOf("{") + 2);
            choicesSection = choicesSection.Substring(0, choicesSection.LastIndexOf("}") - 1);


            string[] choicesLines = choicesSection.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            //to create events from their text representations
            EventFactory eventFactory = new EventFactory();

            //parse each line of the scetion
            for (int lineNum = 0; lineNum < choicesLines.Length; lineNum++)
            {
                string line = choicesLines[lineNum].Trim();

                //check that the line is not empty (or is one of the lines thats part of the choose structure
                if (line == "")
                {
                    continue;
                }

                //create an region for each choice
                string choice = ScriptUtils.ReadUntilCloseBrace(ref lineNum, choicesLines);                
                ScriptRegion choiceRegion = new ScriptRegion(choice);
                m_choices.Add(choiceRegion);
            }

        }


        public override void  DoEvent()
        {
            //chose an event and do it
            int choiceIndex = Program.Game.Random.Next(m_choices.Count);
            ScriptRegion choice = m_choices[choiceIndex];
            choice.DoRegion();
        }
 

    }
    

}
