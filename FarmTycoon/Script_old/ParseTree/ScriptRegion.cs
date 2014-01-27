using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// A region in the scenario script.  A region is an area that starts and ends with a { and }.
    /// A region contains multiple expressions.
    /// </summary>
    public class ScriptRegion
    {

        /// <summary>
        /// Events in the region
        /// </summary>
        private List<ScriptEvent> m_events = new List<ScriptEvent>();


        /// <summary>
        /// Create a Script Section by parsing the section
        /// </summary>
        public ScriptRegion(string region)
        {
            string[] lines = region.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            
            //to create events from their text representations
            EventFactory eventFactory = new EventFactory();
            
            //parse each line of the region
            int lineNum = 0;
            while (lineNum < lines.Length)
            {
                string line = lines[lineNum].Trim();

                //check that the line is not empty (or basicly empty)
                if (line == "{" || line == "}" || line == "")
                {
                    lineNum++;
                    continue;
                }

                //the event we will create
                ScriptEvent newEvent = null;
                if (line.ToUpper().StartsWith(ChooseEvent.NAME))
                {                    
                    //get the full text for the choose event
                    string chooseEventText = line;
                    lineNum++;
                    chooseEventText += ScriptUtils.ReadUntilCloseBrace(ref lineNum, lines);
                    
                    //create a new choose event
                    newEvent = new ChooseEvent(chooseEventText);
                }
                else if (line.ToUpper().StartsWith(IfEvent.NAME))
                {
                    //get the full text for the if event
                    string ifEventText = line;
                    lineNum++;
                    ifEventText += ScriptUtils.ReadUntilCloseBrace(ref lineNum, lines);

                    //create a new choose event
                    newEvent = new IfEvent(ifEventText);
                }
                else
                {
                    //create a new normal event
                    newEvent = eventFactory.Create(line);
                }
                
                //add to list of events
                m_events.Add(newEvent);

                //read next line
                lineNum++;
            }
        }


        /// <summary>
        /// Do all the events in the region
        /// </summary>
        /// <param name="game"></param>
        public void DoRegion()
        {
            foreach (ScriptEvent scriptEvent in m_events)
            {
                scriptEvent.DoEvent();
            }
        }

    }
    


}

