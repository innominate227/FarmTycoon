using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Script event to show the user a message
    /// </summary>
    public class MessageEvent : ScriptEvent
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "MESSAGE";

        /// <summary>
        /// Width of the window
        /// </summary>
        private ScriptNumber m_width;

        /// <summary>
        /// Height of the window
        /// </summary>
        private ScriptNumber m_height;

        /// <summary>
        /// Text for the title
        /// </summary>
        private ScriptString m_title;

        /// <summary>
        /// Text of the message
        /// </summary>
        private ScriptString m_message;
        
        /// <summary>
        /// Should we pause when showing the message
        /// </summary>
        private ScriptString m_pause;
        
        
        /// <summary>
        /// Create a message event
        /// </summary>
        public MessageEvent(string[] actionParams)
        {
            Debug.Assert(actionParams.Length == 4 || actionParams.Length == 5);

            m_width = new ScriptNumber(actionParams[0]);
            m_height = new ScriptNumber(actionParams[1]);
            m_title = new ScriptString(actionParams[2]);
            m_message = new ScriptString(actionParams[3]);
            if (actionParams.Length == 5)
            {
                m_pause = new ScriptString(actionParams[4]);
            }
            else
            {
                m_pause = new ScriptString("False");
            }
        }


        public override void  DoEvent()
        {
            int width = m_width.GetValue();
            int heigth = m_height.GetValue();
            string title = m_title.GetValue();
            string message = m_message.GetValue();
            bool pause = (m_pause.GetValue().ToUpper() == "TRUE");

            new MessageWindow(title, message, pause, width, heigth);
        }
 

    }
    

}
