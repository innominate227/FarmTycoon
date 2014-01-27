using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FarmTycoon
{
    
    /// <summary>
    /// A srcipt controls when events ocur suring a scenario
    /// </summary>
    public class ScenarioScript
    {
        /// <summary>
        /// Sections of the game script sorted by their start date
        /// </summary>
        private List<ScriptSection> m_sections = new List<ScriptSection>();
        
        /// <summary>
        /// Full text of the script.  This can be used to easily recreate the script.
        /// </summary>
        private string m_scriptText;
                        
        /// <summary>
        /// Create a script object from the script text
        /// </summary>        
        public ScenarioScript(string scriptText)
        {            
            m_scriptText = scriptText;           
            
            //create file without all the comment lines
            string scriptNoComments = "";
            foreach (string line in scriptText.Split('\n'))
            {
                if (line.Trim().StartsWith("#") == false && line.Trim() != "")
                {
                    scriptNoComments += (line + "\n");
                }
            }
                        
            string[] lines = scriptNoComments.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            int lineNum = 0;
            while (lineNum < lines.Length)
            {
                string line = lines[lineNum];

                //skip blank lines
                if (line.Trim() == "")
                {
                    lineNum++;
                    continue;
                }

                //read section header
                string sectionString = line;

                //read section region
                lineNum++;
                sectionString += ScriptUtils.ReadUntilCloseBrace(ref lineNum, lines);

                //create Prices section
                ScriptSection section = new ScriptSection(sectionString);

                //add to list of sections
                m_sections.Add(section);

                //read next line
                lineNum++;
            }

            //section number for each section, used as a tiw breaker to make the sort by date stable
            Dictionary<ScriptSection, int> sectionNumbers = new Dictionary<ScriptSection, int>();
            int sectionNumber = 0;
            foreach (ScriptSection section in m_sections)
            {
                sectionNumbers.Add(section, sectionNumber);
                sectionNumber++;
            }

            //sort the sections list
            m_sections.Sort(delegate(ScriptSection s1, ScriptSection s2)
            {
                if (s1.StartDate == s2.StartDate)
                {
                    return sectionNumbers[s1] - sectionNumbers[s2];
                }
                return s1.StartDate - s2.StartDate;
            });
        }

        /// <summary>
        /// Full text of the Prices script.  This can be saved and then used to easily recreate the script.
        /// </summary>
        public string ScriptText
        {
            get { return m_scriptText; }
        }

        /// <summary>
        /// Return all sections int he script
        /// </summary>
        public List<ScriptSection> AllSections()
        {
            return m_sections.ToList();
        }

        /// <summary>
        /// Get a list of script sections that should take place on the date passed
        /// </summary>
        public List<ScriptSection> GetSectionsForDate(int date)
        {
            List<ScriptSection> toRet = new List<ScriptSection>();
            for (int sectionNum = 0; sectionNum < m_sections.Count; sectionNum++)
            {
                ScriptSection section = m_sections[sectionNum];
                if (section.StartDate <= date && section.EndDate >= date && (date - section.StartDate) % section.RepeatInterval == 0)
                {
                    toRet.Add(section);
                }

                if (section.StartDate > date)
                {
                    //sections are in order by start date so once we find one whos start date is after the date passed, all the rest will be after the date passed
                    break;
                }
            }
            return toRet;
        }

        /// <summary>
        /// Removes a section from the script, this is in order to get rid of section that will never be played again so they dont need to be checked any more
        /// </summary>
        public void RemoveSection(ScriptSection section)
        {
            m_sections.Remove(section);
        }


    }
}
