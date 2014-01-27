using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class SceneryDataFile : DataFile
    {
        /// <summary>
        /// mapping from scenery names to scenery info objects
        /// </summary>
        private Dictionary<string, SceneryInfo> m_scenery = new Dictionary<string, SceneryInfo>();


        public SceneryDataFile(string dataFileText)
            : base(dataFileText)
        {
        }

        public override void ParseFile()
        {
            m_scenery.Clear();

            DataFileReader dataFile = new DataFileReader(m_dataFileText);

            foreach (string sceneryType in dataFile.DataItems)
            {
                string texture = dataFile.GetParameterForItem(sceneryType, 0);
                string height = dataFile.GetParameterForItem(sceneryType, 1);
                string landOn = dataFile.GetParameterForItem(sceneryType, 2);

                SceneryInfo sceneryInfo = new SceneryInfo(sceneryType, texture, height, landOn);
                m_scenery.Add(sceneryType, sceneryInfo);
            }
        }

        public SceneryInfo[] GetAllSceneryInfo()
        {
            return m_scenery.Values.ToArray();
        }

        public SceneryInfo GetSceneryInfo(string sceneryType)
        {
            return m_scenery[sceneryType];
        }


    }
}
