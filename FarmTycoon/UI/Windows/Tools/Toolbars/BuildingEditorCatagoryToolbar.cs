using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    public class BuildingEditorCatagoryToolBar : ToolBarWindow
    {
        public BuildingEditorCatagoryToolBar(int dotPosition)            
        {
            //get all the catagories with at least 1 building type
            List<string> catagoriesWithBuildings = new List<string>();
            if (GetAllowedToBuildOfType<StorageBuildingInfo>().Count > 0)
            {
                catagoriesWithBuildings.Add("Storage");
            }
            if (GetAllowedToBuildOfType<ProductionBuildingInfo>().Count > 0)
            {
                catagoriesWithBuildings.Add("Production");
            }
            if (GetAllowedToBuildOfType<TroughInfo>().Count > 0)
            {
                catagoriesWithBuildings.Add("Animal");
            }
            if (GetAllowedToBuildOfType<SceneryInfo>().Count > 0)
            {
                catagoriesWithBuildings.Add("Scenery");
            }
            if (GetAllowedToBuildOfType<BreakHouseInfo>().Count > 0)
            {
                catagoriesWithBuildings.Add("WorkerBuildings");
            }
            if (Program.Game.ScenarioEditMode)
            {
                catagoriesWithBuildings.Add("Special");
            }

            base.Init(catagoriesWithBuildings.ToArray(), dotPosition);
            
            this.Top = 5;
            this.Left = 36;
            this.ToolClicked += new Action<string, int>(ToolClickedHandler);

            //go back to default editor until we pick a building to build
            Program.UserInterface.ActiveEditorManager.DefaultEditor.StartEditing();
        }




        private void ToolClickedHandler(string tool, int position)
        {
            Program.UserInterface.WindowManager.DeleteSubToolbars(this);

            this.SelectTool(tool);
                        
            if(tool == "Storage")
            {
                new PloppableObjectEditorTypeToolBar(GetAllowedToBuildOfType<StorageBuildingInfo>(), position, 77);
            }
            else if(tool == "Production")
            {
                new PloppableObjectEditorTypeToolBar(GetAllowedToBuildOfType<ProductionBuildingInfo>(), position, 77);
            }
            else if (tool == "Animal")
            {
                new PloppableObjectEditorTypeToolBar(GetAllowedToBuildOfType<TroughInfo>(), position, 77);
            }
            else if (tool == "Scenery")
            {
                new PloppableObjectEditorTypeToolBar(GetAllowedToBuildOfType<SceneryInfo>(), position, 77);
            }
            else if (tool == "WorkerBuildings")
            {
                new PloppableObjectEditorTypeToolBar(GetAllowedToBuildOfType<BreakHouseInfo>(), position, 77);
            }
            else if(tool == "Special")
            {
                List<IPloppableInfo> specialObjType = new List<IPloppableInfo>();
                specialObjType.Add((DeliveryAreaInfo)FarmData.Current.GetInfo(DeliveryAreaInfo.UNIQUE_NAME));
                new PloppableObjectEditorTypeToolBar(specialObjType, position, 77);
            }            
        }

        public List<IPloppableInfo> GetAllowedToBuildOfType<T>() where T : IPloppableInfo
        {
            List<IPloppableInfo> ret = new List<IPloppableInfo>();
            List<T> buildingsOfTypeT = FarmData.Current.GetInfos<T>();
            foreach (T buildingOfTypeT in buildingsOfTypeT)
            {
                if (GameState.Current.Prices.GetPrice(buildingOfTypeT) >= 0)
                {
                    ret.Add(buildingOfTypeT);
                }
            }
            return ret;
        }

        

    }
}
