using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// A section of a scenario script.  A section is a group of events that ocurs on a certain date, or at a certain interval.
    /// </summary>
    public class ScriptSection
    {
        /// <summary>
        /// Interval to repeate at
        /// </summary>
        private int m_repeatInterval;

        /// <summary>
        /// start date for the section
        /// </summary>
        private int m_startDate;

        /// <summary>
        /// start date for the section
        /// </summary>
        private int m_endDate;

        /// <summary>
        /// Region for the section
        /// </summary>
        private ScriptRegion m_scriptRegion;


        /// <summary>
        /// Create a Script Section by parsing the section
        /// </summary>
        public ScriptSection(string section)
        {
            //get tokens for the section from line 1 of the section
            string line1 = section.Substring(0, section.IndexOf("\r\n"));
            string[] line1Tokens = line1.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            //parse the first line with the frequency info            
            m_startDate = int.Parse(line1Tokens[1]);
            m_repeatInterval = int.Parse(line1Tokens[3]);
            m_endDate = int.Parse(line1Tokens[5]);

            //make sure the frequency makes since
            Debug.Assert(m_startDate >= 0 && m_endDate >= 0 && m_repeatInterval > 0 && m_endDate >= m_startDate);

            //get the remaining lines
            string remaininglines = section.Substring(section.IndexOf("\r\n"));
            m_scriptRegion = new ScriptRegion(remaininglines);
        }


        public void DoSection()
        {
            m_scriptRegion.DoRegion();
        }


        /// <summary>
        /// Interval to repeate at
        /// </summary>
        public int RepeatInterval
        {
            get { return m_repeatInterval; }
        }

        /// <summary>
        /// start date for the section
        /// </summary>
        public int StartDate
        {
            get { return m_startDate; }
        }

        /// <summary>
        /// end date for the section
        /// </summary>
        public int EndDate
        {
            get { return m_endDate; }
        }

    }
    

    
}
