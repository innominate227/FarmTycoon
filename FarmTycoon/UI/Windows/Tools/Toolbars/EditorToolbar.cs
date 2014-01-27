using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class EditorToolbar : ToolBarWindow
    {
        /// <summary>
        /// Create new editor toolbar.
        /// </summary>
        public EditorToolbar()            
        {
            string[] editors = new string[] { "Hand", "Land", "Road", "Enclosure", "Building", "Worker", "Delete", "BuyLand", "Goal" };
            if (Program.Game.ScenarioEditMode)
            {
                editors = new string[] { "Hand", "Land", "Road", "Enclosure", "Building", "Worker", "Delete", "BuyLand", "Scenario", "LandGen", "Goal" };
            }
            base.Init(editors, -1);
            
            this.Top = 5;
            this.Left = 5;
            this.ToolClicked += new Action<string, int>(ToolClickedHandler);

            ToolClickedHandler("Hand", 0);
        }


        private void ToolClickedHandler(string tool, int position)
        {
            Program.UserInterface.WindowManager.DeleteSubToolbars();

            this.SelectTool(tool);

            if (tool == "Hand")
            {
                Program.UserInterface.ActiveEditorManager.DefaultEditor.StartEditing();
            }
            else if (tool == "Land")
            {
                //create a land smoothness toolbar
                new LandEditorSizeToolbar(position);
            }
            else if (tool == "Road")
            {
                new RoadTypeToolbar(position);
            }
            else if (tool == "Enclosure")
            {                
                new EnclosureTypeToolbar(position);
            }
            else if (tool == "Building")
            {                
                //create a Building Types toolbar
                new BuildingEditorCatagoryToolBar(position);
            }
            else if (tool == "Worker")
            {
                WorkerEditor workerEditor = new WorkerEditor();
                workerEditor.StartEditing();
            }
            else if (tool == "Delete")
            {
                //create delete editor toolbar
                new DeleteEditorSizeToolbar(position);
            }
            else if (tool == "BuyLand")
            {
                //create buy land editor toolbar
                new BuyLandEditorSizeToolbar(position);
            }
            else if (tool == "Scenario")
            {
                //open scenario settings window
                new ScenarioSettingsWindow();
            }
            else if (tool == "LandGen")
            {
                //open land editor window
                new LandEditorWindow();
            }
            else if (tool == "Goal")
            {
                //open goals window
                new GoalWindow();
                this.SelectTool("Hand");
            }
        }


    }
}

