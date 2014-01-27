using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Script event to make a building or scnery available
    /// </summary>
    public class MakeAvailableEvent : ScriptEvent
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "MAKEAVAILABLE";

        
        /// <summary>
        /// Either the string "Building", or "Scenery" depending on what kind of object make avaialble is
        /// </summary>
        private ScriptString m_kind;

        /// <summary>
        /// What building/scenery should be made available
        /// </summary>
        private ScriptString m_type;
        
        
        /// <summary>
        /// Create a MakeAvailableEvent
        /// </summary>
        public MakeAvailableEvent(string[] actionParams)
        {
            Debug.Assert(actionParams.Length == 2);

            m_kind = new ScriptString(actionParams[0]);
            m_type = new ScriptString(actionParams[1]);
        }


        public override void DoEvent()
        {            
            string typeToMakeAvailable = m_type.GetValue();
            IGameObjectInfo objectInfo = Program.Game.FarmData.GetObjectInfo(typeToMakeAvailable);
            Program.Game.AvailableObjects.MakeAvaiable(objectInfo);
        }
 

    }
    

}
