using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Var for a count of objects that exists in the game world
    /// </summary>
    public class ObjectCountVar : ScriptVar
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "OBJECTCOUNT";
        
        /// <summary>
        /// What kind of object are we counting
        /// </summary>
        private ScriptString m_kind;
        
        /// <summary>
        /// What type of of that kind are we counting, or null for all
        /// </summary>
        private ScriptString m_type;


        public ObjectCountVar(string[] actionParams)
        {
            Debug.Assert(actionParams.Length == 1 || actionParams.Length == 2);

            m_kind = new ScriptString(actionParams[0]);
            if (actionParams.Length == 2)
            {
                m_type = new ScriptString(actionParams[1]);
            }
        }


        public override int GetValue()
        {
            string kindToMakeAvailable = m_kind.GetValue();
            string typeToMakeAvailable = "";
            if (m_type != null)
            {
                typeToMakeAvailable = m_type.GetValue();
            }
            
            if (kindToMakeAvailable.ToUpper() == "OBJECT")
            {
                return CountSolidObjects(typeToMakeAvailable);
            }
            else if (kindToMakeAvailable.ToUpper() == "FIELD")
            {
                return CountFields(typeToMakeAvailable);
            }
            else if (kindToMakeAvailable.ToUpper() == "CROP")
            {
                return CountCrops(typeToMakeAvailable);
            }
            else if (kindToMakeAvailable.ToUpper() == "WORKER")
            {
                return CountWorkers(typeToMakeAvailable);
            }
            else if (kindToMakeAvailable.ToUpper() == "ROAD")
            {
                return CountRoads(typeToMakeAvailable);
            }
            else if (kindToMakeAvailable.ToUpper() == "FENCE")
            {
                return CountFences(typeToMakeAvailable);
            }
            else
            {
                Debug.Assert(false);
                return 0;
            }

        }


        private int CountSolidObjects(string type)
        {
            if (type == "")
            {
                return Program.Game.MasterObjectList.TypeCount<SolidObject>();
            }
            else
            {
                int count = 0;
                foreach (SolidObject solidObject in Program.Game.MasterObjectList.FindAll<SolidObject>())
                {
                    if (solidObject.SolidObjectInfo.Name.ToUpper() == type.ToUpper())
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        

        private int CountFields(string type)
        {
            if (type == "")
            {
                return Program.Game.MasterObjectList.TypeCount<PlantedArea>();
            }
            else
            {
                int count = 0;
                foreach (PlantedArea field in Program.Game.MasterObjectList.FindAll<PlantedArea>())
                {
                    if (field.TypePlanted.ToUpper() == type.ToUpper())
                    {
                        count++;
                    }
                }
                return count;
            }
        }


        private int CountCrops(string type)
        {
            if (type == "")
            {
                return Program.Game.MasterObjectList.TypeCount<Crop>();
            }
            else
            {
                int count = 0;
                foreach (Crop crop in Program.Game.MasterObjectList.FindAll<Crop>())
                {
                    if (crop.Name.ToUpper() == type.ToUpper())
                    {
                        count++;
                    }
                }
                return count;
            }
        }


        private int CountWorkers(string type)
        {
            return Program.Game.MasterObjectList.TypeCount<Worker>();            
        }

        private int CountRoads(string type)
        {
            return Program.Game.MasterObjectList.TypeCount<Road>();
        }

        private int CountFences(string type)
        {
            return Program.Game.MasterObjectList.TypeCount<Fence>();
        }
    }
    

}
