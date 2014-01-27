using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FarmTycoon
{
    
    /// <summary>
    /// Plays through a scenario script, by listening for date changes and playing the correct section of the script.
    /// </summary>
    public class ScriptPlayer : ISavable
    {
        /// <summary>
        /// Script to play
        /// </summary>
        private ScenarioScript m_script;

        /// <summary>
        /// Dictionary of script variables and their current values
        /// </summary>
        private Dictionary<string, int> m_scriptVariables = new Dictionary<string, int>();
        
        /// <summary>
        /// Create a script player, Call Setup, or Read State before using
        /// </summary>
        public ScriptPlayer()
        {

        }


        /// <summary>
        /// Create a new script play to play the script on the game passed
        /// </summary>
        public void SetUp()
        {
            Program.Game.Calandar.DateChanged += new Action(Calandar_DateChanged);

            //create empty script
            m_script = new ScenarioScript("");
        }

        /// <summary>
        /// Raised when the date changes
        /// </summary>
        private void Calandar_DateChanged()
        {
            int date = Program.Game.Calandar.Date;

            //get the sections that should be played on that date
            List<ScriptSection> sectionsToPlay = m_script.GetSectionsForDate(date);

            //play each section
            foreach (ScriptSection section in sectionsToPlay)
            {
                section.DoSection();

                //if the section will not play again remove it
                if (date + section.RepeatInterval > section.EndDate)
                {
                    m_script.RemoveSection(section);
                }
            }
        }

        /// <summary>
        /// Load a script from a file
        /// </summary>
        public void LoadScript(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            string scriptText = reader.ReadToEnd();
            reader.Close();

            m_script = new ScenarioScript(scriptText);
        }

        /// <summary>
        /// Save script to a file
        /// </summary>
        public void SaveScript(string fileName)
        {
            StreamWriter writer = new StreamWriter(fileName);
            writer.Write(m_script.ScriptText);
            writer.Close();
        }

        /// <summary>
        /// Get the value of a script variable
        /// </summary>
        public int GetVariableValue(string variableName)
        {
            if (m_scriptVariables.ContainsKey(variableName) == false) { return 0; }
            return m_scriptVariables[variableName];
        }

        /// <summary>
        /// Set the value of a script variable
        /// </summary>
        public void SetVariableValue(string variableName, int variableValue)
        {
            if (m_scriptVariables.ContainsKey(variableName) == false) 
            {
                m_scriptVariables.Add(variableName, 0);
            }
            m_scriptVariables[variableName] = variableValue;
        }



        public void WriteState(ObjectState state)
        {
             
            state.SetValue("ScenarioScript", m_script.ScriptText);

            state.SetValue("ScriptVairableCount", m_scriptVariables.Count);
            int varNum = 0;
            foreach (string variableName in m_scriptVariables.Keys)
            {
                int variableValue = m_scriptVariables[variableName];
                state.SetValue("ScriptVairableName" + varNum.ToString(), variableName);
                state.SetValue("ScriptVairableValue" + varNum.ToString(), variableValue);
                varNum++;
            }
        }

        public void ReadState(ObjectState state)
        {
             
            Program.Game.Calandar.DateChanged += new Action(Calandar_DateChanged);

            string scriptText = state.GetValue<string>("ScenarioScript");
            m_script = new ScenarioScript(scriptText);
            
            int variableCount = state.GetValue<int>("ScriptVairableCount");
            for(int i=0; i<variableCount; i++)            
            {                
                string variableName = state.GetValue<string>("ScriptVairableName" + i.ToString());
                int variableValue = state.GetValue<int>("ScriptVairableValue" + i.ToString());
                m_scriptVariables.Add(variableName, variableValue);
            }
            
            //get rid of sections that have already passed
            int date = Program.Game.Calandar.Date;
            foreach (ScriptSection section in m_script.AllSections())
            {
                //if the section will not play again remove it
                if (date > section.EndDate)
                {
                    m_script.RemoveSection(section);
                }
            }
        }
    }
}
