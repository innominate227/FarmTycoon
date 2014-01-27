using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class BuildingsDataFile : DataFile
    {
        /// <summary>
        /// mapping from building names to building info objects
        /// </summary>
        private Dictionary<string, BuildingInfo> m_buildings = new Dictionary<string, BuildingInfo>();


        public BuildingsDataFile(string dataFileText)
            : base(dataFileText)
        {
        }

        public override void ParseFile()
        {
            m_buildings.Clear();

            DataFileReader dataFile = new DataFileReader(m_dataFileText);


            foreach (string buildingType in dataFile.DataItems)
            {
                string buildingCatagoryStr = dataFile.GetParameterForItem(buildingType, 0);
                BuildingCatagory buildingCatagory = (BuildingCatagory)Enum.Parse(typeof(BuildingCatagory), buildingCatagoryStr);

                string texture = dataFile.GetParameterForItem(buildingType, 1);
                string height = dataFile.GetParameterForItem(buildingType, 2);
                string landOn = dataFile.GetParameterForItem(buildingType, 3);
                string walkableLand = dataFile.GetParameterForItem(buildingType, 4);
                string actionLand = dataFile.GetParameterForItem(buildingType, 5);

                if (buildingCatagory == BuildingCatagory.Storage)
                {
                    string capacity = dataFile.GetParameterForItem(buildingType, 6);
                    string allowedItems = dataFile.GetParameterForItem(buildingType, 7);

                    BuildingInfo building = new StorageBuildingInfo(buildingCatagory, buildingType, texture, height, landOn, walkableLand, actionLand, capacity, allowedItems);
                    m_buildings.Add(buildingType, building);
                }
                else if (buildingCatagory == BuildingCatagory.Production)
                {
                    string capacity = dataFile.GetParameterForItem(buildingType, 6);
                    string allowedItems = dataFile.GetParameterForItem(buildingType, 7);
                    string inputs = dataFile.GetParameterForItem(buildingType, 8);
                    string outputs = dataFile.GetParameterForItem(buildingType, 9);
                    string speed = dataFile.GetParameterForItem(buildingType, 10);

                    BuildingInfo building = new ProdcutionBuildingInfo(buildingCatagory, buildingType, texture, height, landOn, walkableLand, actionLand, capacity, allowedItems, inputs, outputs, speed);
                    m_buildings.Add(buildingType, building);
                }
                else if (buildingCatagory == BuildingCatagory.DeliveryArea)
                {
                    string allTypesString = "";
                    foreach(ItemType type in Program.Game.DataFiles.ItemsFile.FullItemList.ItemTypes)
                    {
                        allTypesString += "Item:" + type.Name + ";";
                    }
                    allTypesString = allTypesString.Trim(';');
                    BuildingInfo building = new StorageBuildingInfo(buildingCatagory, buildingType, texture, height, landOn, walkableLand, actionLand, int.MaxValue.ToString(), allTypesString);
                    m_buildings.Add(buildingType, building);
                }
                else if (buildingCatagory == BuildingCatagory.BreakHouse)
                {                    
                    BuildingInfo building = new BuildingInfo(buildingCatagory, buildingType, texture, height, landOn, walkableLand, actionLand);
                    m_buildings.Add(buildingType, building);
                }

            }
        }


        public BuildingInfo[] GetAllBuildingsInfo()
        {
            return m_buildings.Values.ToArray();
        }

        public BuildingInfo GetBuildingInfo(string buildingType)
        {
            return m_buildings[buildingType];
        }

    }
}
