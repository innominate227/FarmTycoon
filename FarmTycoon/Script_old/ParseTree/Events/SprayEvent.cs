using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Spray all crops on the map with items (generally used to spray with wether items)
    /// </summary>
    public class SprayEvent : ScriptEvent
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "SPRAY";


        /// <summary>
        /// What type to spray
        /// </summary>
        private ScriptString m_whatToSpray;
        
        /// <summary>
        /// What item types to spray
        /// </summary>
        private List<ScriptString> m_typesToSpray = new List<ScriptString>();
        
        
        /// <summary>
        /// Create a spray event
        /// </summary>
        public SprayEvent(string[] actionParams)
        {
            m_whatToSpray = new ScriptString(actionParams[0]);

            for (int typeToSprayIndex = 1; typeToSprayIndex < actionParams.Length; typeToSprayIndex++)
            {
                ScriptString typeSprayed = new ScriptString(actionParams[typeToSprayIndex]);
                m_typesToSpray.Add(typeSprayed);
            }
        }


        public override void  DoEvent()
        {
            string whatToSpray = m_whatToSpray.GetValue().ToUpper();
            
            //spary all the types
            foreach (ScriptString typeSprayed in m_typesToSpray)
            {
                //get the type to spray
                ItemType typeToSpray = Program.Game.FarmData.GetItemType(typeSprayed.GetValue());

                if (whatToSpray == "CROP" || whatToSpray == "ALL")
                {
                    //spray on to all crops
                    foreach (Crop crop in Program.Game.MasterObjectList.FindAll<Crop>())
                    {
                        crop.Traits.ApplyToTraits(typeToSpray);
                    }
                }
                if (whatToSpray == "LAND" || whatToSpray == "ALL")
                {
                    //spray on to all land
                    foreach (Land land in Program.Game.MasterObjectList.FindAll<Land>())
                    {
                        land.Traits.ApplyToTraits(typeToSpray);
                    }
                }
            }

        }
 

    }
    

}
