using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// Manages the toolbar windows, and handel creating the correct window, or starting the correct editor when a tool bar button is clicked
    /// </summary>
    public class ToolbarManager
    {
        //editors activated by the toolbar
        private RoadEditor m_roadEditor = new RoadEditor();
        private WorkerEditor m_workerEditor = new WorkerEditor();
        private FieldEditor m_fieldEditor = new FieldEditor();
        private BuildingEditor m_buildingEditor = new BuildingEditor();
        private LandEditor m_landEditor = new LandEditor();
        private DeleteEditor m_deleteEditor = new DeleteEditor();
        
        //the tool bars
        private ToolBarWindow m_rightToolBar;
        private ToolBarWindow m_mainToolBar;
        private ToolBarWindow m_landSmoothToolBar;
        private ToolBarWindow m_landSizeToolBar;
        private ToolBarWindow m_buildingTypesToolBar;
        private ToolBarWindow m_buildingsToolBar;
        private ToolBarWindow m_deleteSizeToolBar;


        public ToolbarManager()
        {            
            //listen to all clicks to determine if we need to hide the tool bar windows
            Program.Graphics.Events.MouseDown += new TycoonGraphicsLib.MouseEventHandler(Graphics_MouseDown);

            //when any of the editors stops nothing should be selected in the toolbar any longer
            m_roadEditor.EditingStopped += new Action(delegate { m_mainToolBar.SelectTool("Hand"); });
            m_workerEditor.EditingStopped += new Action(delegate { m_mainToolBar.SelectTool("Hand"); });
            m_fieldEditor.EditingStopped += new Action(delegate { m_mainToolBar.SelectTool("Hand"); });
            m_buildingEditor.EditingStopped += new Action(delegate { m_mainToolBar.SelectTool("Hand"); });
            m_landEditor.EditingStopped += new Action(delegate { m_mainToolBar.SelectTool("Hand"); HideToolbars(); });
            m_deleteEditor.EditingStopped += new Action(delegate { m_mainToolBar.SelectTool("Hand"); HideToolbars(); });

            CreateMainToolbar();
            CreateRightToolbar();
        }

        private void CreateMainToolbar()
        {
            //create the main toolbar
            m_mainToolBar = new ToolBarWindow(new string[] { "Hand", "Land", "Road", "Field", "Building", "Worker", "Delete" }, -1);
            m_mainToolBar.Top = 5;
            m_mainToolBar.Left = 5;
            m_mainToolBar.ToolClicked += new Action<string, int>(MainToolBar_ToolClicked);
        }

        private void CreateRightToolbar()
        {
            //create the right toolbar
            m_rightToolBar = new ToolBarWindow(new string[] { "S+","S-", "Pause", "VL", "VR", "V+", "V-", "Save", "Load" }, -1);
            m_rightToolBar.Top = 5;
            m_rightToolBar.Left = Program.Graphics.WindowWidth - 35;
            Program.Graphics.Events.WindowSizeChanged += new Action(delegate
            {
                m_rightToolBar.Left = Program.Graphics.WindowWidth - 35;
            });
            m_rightToolBar.ToolClicked += new Action<string, int>(RightToolBar_ToolClicked);
        }

        #region Hide Toolbars

        private void Graphics_MouseDown(TycoonGraphicsLib.ClickInfo clickInfo)
        {
            //we need to hide some toolbar windows when the mouse is clicked on something thats not the toolbar
            //when somehting thats not the tool bar window is clicked
            //first figure out if what was clicked is not a toolbar window
            bool hideToolbarWindows = false;
            if (clickInfo.ControlClicked == null)
            {
                hideToolbarWindows = true;
            }
            else
            {
                if (clickInfo.ControlClicked.ParentWindow is ToolBarWindow == false)
                {
                    hideToolbarWindows = true;
                }
            }

            //hide all the toolbar windows
            if (hideToolbarWindows)
            {
                //we dont want to hide land size, or land smooth toolbars.
                HideToolbarsExcept(m_landSizeToolBar, m_landSmoothToolBar, m_deleteSizeToolBar);

                //if we were placing a building also unselect the building toolbar button
                if (m_mainToolBar.ToolSelected == "Building")
                {
                    m_mainToolBar.SelectTool("Hand");
                }
            }
        }


        /// <summary>
        /// Hides all toolbars but the main toolbar
        /// </summary>
        private void HideToolbars()
        {
            HideToolbarsExcept();
        }

        /// <summary>
        /// Hides all toolbars but the main toolbar
        /// </summary>
        private void HideToolbarsExcept(params ToolBarWindow[] dontHide)
        {
            if (m_buildingTypesToolBar != null && dontHide.Contains(m_buildingTypesToolBar) == false)
            {
                m_buildingTypesToolBar.Visible = false;
                Program.Graphics.RemoveWindow(m_buildingTypesToolBar);
                m_buildingTypesToolBar.ToolClicked -= new Action<string, int>(BuildingTypesToolBar_ToolClicked);
                m_buildingTypesToolBar = null;
            }

            if (m_buildingsToolBar != null && dontHide.Contains(m_buildingsToolBar) == false)
            {
                m_buildingsToolBar.Visible = false;
                Program.Graphics.RemoveWindow(m_buildingsToolBar);
                m_buildingsToolBar.ToolClicked -= new Action<string, int>(BuildingsToolBar_ToolClicked);
                m_buildingsToolBar = null;
            }

            if (m_landSmoothToolBar != null && dontHide.Contains(m_landSmoothToolBar) == false)
            {
                m_landSmoothToolBar.Visible = false;
                Program.Graphics.RemoveWindow(m_landSmoothToolBar);
                m_landSmoothToolBar.ToolClicked -= new Action<string, int>(LandSmoothToolBar_ToolClicked);
                m_landSmoothToolBar = null;
            }

            if (m_landSizeToolBar != null && dontHide.Contains(m_landSizeToolBar) == false)
            {
                m_landSizeToolBar.Visible = false;
                Program.Graphics.RemoveWindow(m_landSizeToolBar);
                m_landSizeToolBar.ToolClicked -= new Action<string, int>(LandSizeToolBar_ToolClicked);
                m_landSizeToolBar = null;
            }

            if (m_deleteSizeToolBar != null && dontHide.Contains(m_deleteSizeToolBar) == false)
            {
                m_deleteSizeToolBar.Visible = false;
                Program.Graphics.RemoveWindow(m_deleteSizeToolBar);
                m_deleteSizeToolBar.ToolClicked -= new Action<string, int>(DeleteSizeToolBar_ToolClicked);
                m_deleteSizeToolBar = null;
            }
        }
        
        #endregion

        public void MainToolBar_ToolClicked(string tool, int position)
        {
            //hide any other toolbars
            HideToolbars();
            
            if (tool == "Hand")
            {
                Program.ActiveEditorManager.DefaultEditor.StartEditing();
                m_mainToolBar.SelectTool(tool);
            }
            else if (tool == "Land")
            {
                //go back to default editor until we pick smooth or cliff
                Program.ActiveEditorManager.DefaultEditor.StartEditing();
                m_mainToolBar.SelectTool(tool);

                //create a land smoothness toolbar
                m_landSmoothToolBar = new ToolBarWindow(new string[] { "Cliff", "Smooth" }, position);
                m_landSmoothToolBar.Top = 5;
                m_landSmoothToolBar.Left = 35;
                m_landSmoothToolBar.ToolClicked += new Action<string, int>(LandSmoothToolBar_ToolClicked);                                
            }
            else if (tool == "Road")
            {
                m_roadEditor.StartEditing();
                m_mainToolBar.SelectTool(tool);
            }
            else if (tool == "Field")
            {
                m_fieldEditor.StartEditing();
                m_mainToolBar.SelectTool(tool);
            }
            else if (tool == "Building")
            {
                //go back to default editor until we pick a building to build
                Program.ActiveEditorManager.DefaultEditor.StartEditing();
                m_mainToolBar.SelectTool(tool);

                //create a Building Types toolbar
                m_buildingTypesToolBar = new ToolBarWindow(new string[] { "Storage", "Production", "Animal", "Special" }, position);
                m_buildingTypesToolBar.Top = 5;
                m_buildingTypesToolBar.Left = 35;
                m_buildingTypesToolBar.ToolClicked += new Action<string, int>(BuildingTypesToolBar_ToolClicked);                
            }
            else if (tool == "Worker")
            {
                m_workerEditor.StartEditing();
                m_mainToolBar.SelectTool(tool);
            }
            else if (tool == "Delete")
            {
                m_deleteEditor.Size = 1;
                m_deleteEditor.StartEditing();
                m_mainToolBar.SelectTool(tool);

                //create a delete size toolbar
                m_deleteSizeToolBar = new ToolBarWindow(new string[] { "1x1", "2x2", "3x3", "4x4", "5x5", "6x6" }, position);
                m_deleteSizeToolBar.Top = 5;
                m_deleteSizeToolBar.Left = 35;
                m_deleteSizeToolBar.SelectTool("1x1");
                m_deleteSizeToolBar.ToolClicked += new Action<string, int>(DeleteSizeToolBar_ToolClicked);                
            }
        }


        public void RightToolBar_ToolClicked(string tool, int position)
        {
            //hide any other toolbars
            HideToolbars();

            if (tool == "S+")
            {
                Program.GameThread.ClockDriver.DesiredRate *= 2.0;
            }
            else if (tool == "S-")
            {
                Program.GameThread.ClockDriver.DesiredRate *= 0.5;
            }
            else if (tool == "Pause")
            {
                Program.GameThread.ClockDriver.Paused = !Program.GameThread.ClockDriver.Paused;
            }
            else if (tool == "VL")
            {
                Program.Graphics.ViewRotation = DirectionUtils.CounterClockwiseOne(Program.Graphics.ViewRotation);
            }
            else if (tool == "VR")
            {
                Program.Graphics.ViewRotation = DirectionUtils.ClockwiseOne(Program.Graphics.ViewRotation); 
            }
            else if (tool == "V+")
            {
                Program.Graphics.Scale *= 2.0f;
            }
            else if (tool == "V-")
            {
                Program.Graphics.Scale *= 0.5f;
            }
            else if (tool == "Save")
            {
                Program.ActiveEditorManager.DefaultEditor.StartEditing();
                GameSaver saver = new GameSaver();

                string basePath = @"C:\Users\Innominate\SkyDrive\Projects\FarmTycoon\Saves\save";
                int num = 0;
                while (System.IO.File.Exists(basePath + num.ToString()))
                {
                    num++;
                }
                saver.Save(basePath + num.ToString());
            }
            else if (tool == "Load")
            {
                
                string basePath = @"C:\Users\Innominate\SkyDrive\Projects\FarmTycoon\Saves\save";
                int num = 0;
                while (System.IO.File.Exists(basePath + num.ToString()))
                {
                    num++;
                }

                Program.ActiveEditorManager.DefaultEditor.StartEditing();
                GameSaver saver = new GameSaver();
                saver.Load(basePath + (num-1).ToString());
                
            }
        }

        #region Land Sub Toolbars
        private void LandSmoothToolBar_ToolClicked(string tool, int position)
        {
            //hide toolbars that are not this one
            HideToolbarsExcept(m_landSmoothToolBar);

            //set land editor to smooth if it was selected
            m_landSmoothToolBar.SelectTool(tool);
            m_landEditor.Smoothed = (tool == "Smooth");

            //start editing land at default 1x1 size
            m_landEditor.Size = 1;
            m_landEditor.StartEditing();

            //create a land size toolbar
            m_landSizeToolBar = new ToolBarWindow(new string[] { "1x1", "2x2", "3x3", "4x4", "5x5", "6x6" }, position);
            m_landSizeToolBar.Top = 5;
            m_landSizeToolBar.Left = 75;
            m_landSizeToolBar.SelectTool("1x1");
            m_landSizeToolBar.ToolClicked += new Action<string, int>(LandSizeToolBar_ToolClicked);
        }
        private void LandSizeToolBar_ToolClicked(string tool, int position)
        {
            m_landSizeToolBar.SelectTool(tool);

            //get the size clicked, and inform the editor
            int size = int.Parse(tool.Substring(0, 1));
            m_landEditor.Size = size;
        }
        #endregion

        #region Buildings Sub Toolbars

        private void BuildingTypesToolBar_ToolClicked(string tool, int position)
        {            
            //hide toolbars that are not this one
            HideToolbarsExcept(m_buildingTypesToolBar);

            m_buildingTypesToolBar.SelectTool(tool);
                        
            //get a list of all the buildings for the catagory clicked
            List<string> buildingsInToolBar = new List<string>();
            BuildingCatagory catagoryClicked = (BuildingCatagory)Enum.Parse(typeof(BuildingCatagory), tool);
            DataFile buildingsDataFile = Program.DataFiles.BuildingsFile;            
            foreach (string building in buildingsDataFile.DataItems)
            {
                //get the type of building
                string buildingCatagoryParameter = buildingsDataFile.GetParameterForItem(building, 0);
                BuildingCatagory buildingCatagory = (BuildingCatagory)Enum.Parse(typeof(BuildingCatagory), buildingCatagoryParameter);                
                if (buildingCatagory == catagoryClicked)
                {
                    buildingsInToolBar.Add(building);
                }
            }

            //create a buildings toolbar
            m_buildingsToolBar = new ToolBarWindow(buildingsInToolBar.ToArray(), position);
            m_buildingsToolBar.Top = 5;
            m_buildingsToolBar.Left = 75;
            m_buildingsToolBar.ToolClicked += new Action<string, int>(BuildingsToolBar_ToolClicked);
        }
        
        private void BuildingsToolBar_ToolClicked(string tool, int position)
        {            
            m_buildingsToolBar.SelectTool(tool);

            //start placing that building
            m_buildingEditor.SetBuildingType(tool);
            m_buildingEditor.StartEditing();
        }

        #endregion

        #region Delete Sub Toolbars
        private void DeleteSizeToolBar_ToolClicked(string tool, int position)
        {
            m_deleteSizeToolBar.SelectTool(tool);

            //get the size clicked, and inform the editor
            int size = int.Parse(tool.Substring(0, 1));
            m_deleteEditor.Size = size;
        }
        #endregion
    }
}
