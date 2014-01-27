using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Script event to adjust weather
    /// </summary>
    public class WeatherEvent : ScriptEvent
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "WEATHER";
        

        /// <summary>
        /// Text description of the weather for the user to see
        /// </summary>
        private ScriptString m_weatherDescription;
                
        
        /// <summary>
        /// Create a weather event
        /// </summary>
        public WeatherEvent(string[] actionParams)
        {
            Debug.Assert(actionParams.Length == 1);

            m_weatherDescription = new ScriptString(actionParams[0]);
            
        }


        public override void  DoEvent()
        {
            string weatherDescription = m_weatherDescription.GetValue();
            Program.Game.Weather.SetWeather(weatherDescription);
            
        }
 

    }
    

}
