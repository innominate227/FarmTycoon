using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{


    public partial class ScriptGameInterface
    {
        
        
        public int GetOwnedLandCount()
        {
            //TODO:
            return -1;
        }
        public int GetWorkerCount()
        {
            return GameState.Current.MasterObjectList.TypeCount<Worker>();
        }
        public int GetAvailableWorkerCount()
        {
            return GameState.Current.WorkerAssigner.NumberOfAvailableWorkers;
        }
        public int GetRoadCount()
        {
            return GameState.Current.MasterObjectList.TypeCount<Road>();
        }
        public int GetFieldCount()
        {
            return GameState.Current.MasterObjectList.TypeCount<Field>();
        }
        public int GetCropCount()
        {
            return GameState.Current.MasterObjectList.TypeCount<Crop>();
        }
        public int GetPastureCount()
        {
            return GameState.Current.MasterObjectList.TypeCount<Pasture>();
        }
        public int GetProductionBuildingCount()
        {
            return GameState.Current.MasterObjectList.TypeCount<ProductionBuilding>();
        }
        public int GetStorageBuildingCount()
        {
            return GameState.Current.MasterObjectList.TypeCount<StorageBuilding>();
        }
        public int GetTroughCount()
        {
            return GameState.Current.MasterObjectList.TypeCount<Trough>();
        }
        public int GetSceneryCount()
        {
            return GameState.Current.MasterObjectList.TypeCount<Scenery>();
        }






        public int GetFieldCount(string seedName)
        {
            int count = 0;
            foreach (Field field in GameState.Current.MasterObjectList.FindAll<Field>())
            {
                if (field.CropInfo != null && field.CropInfo.Seed.Name == seedName)
                {
                    count++;
                }
            }
            return count;
        }
        public int GetCropCount(string seedName)
        {
            int count = 0;
            foreach (Crop crop in GameState.Current.MasterObjectList.FindAll<Crop>())
            {
                if (crop.CropInfo.Seed.Name == seedName)
                {
                    count++;
                }
            }
            return count;
        }

        public int GetPastureCount(string animalName)
        {
            int count = 0;
            foreach (Pasture pasture in GameState.Current.MasterObjectList.FindAll<Pasture>())
            {
                if (pasture.AnimalInfo != null && pasture.AnimalInfo.AnimalType.Name == animalName)
                {
                    count++;
                }
            }
            return count;
        }
        public int GetProductionBuildingCount(string buildingName)
        {
            int count = 0;
            foreach (ProductionBuilding building in GameState.Current.MasterObjectList.FindAll<ProductionBuilding>())
            {
                if (building.BuildingInfo.Name == buildingName)
                {
                    count++;
                }
            }
            return count;
        }
        public int GetStorageBuildingCount(string buildingName)
        {
            int count = 0;
            foreach (StorageBuilding building in GameState.Current.MasterObjectList.FindAll<StorageBuilding>())
            {
                if (building.BuildingInfo.Name == buildingName)
                {
                    count++;
                }
            }
            return count;
        }
        public int GetTroughCount(string buildingName)
        {
            int count = 0;
            foreach (Trough building in GameState.Current.MasterObjectList.FindAll<Trough>())
            {
                if (building.TroughInfo.Name == buildingName)
                {
                    count++;
                }
            }
            return count;
        }
        public int GetSceneryCount(string buildingName)
        {
            int count = 0;
            foreach (Scenery building in GameState.Current.MasterObjectList.FindAll<Scenery>())
            {
                if (building.SceneryInfo.Name == buildingName)
                {
                    count++;
                }
            }
            return count;
        }





    }
}
