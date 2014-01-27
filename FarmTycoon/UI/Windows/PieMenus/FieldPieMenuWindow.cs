using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    public class FieldPieMenuWindow : PieMenuWindow
    {
        private Field _field;

        public FieldPieMenuWindow(Field field, Point centerPoint)            
        {
            
            CropInfo cropInfo = field.CropInfo;
            if (cropInfo == null)
            {
                base.Init(new string[] { "Plow", "Plant", "Change Name", "Move Gate", "Stats" }, centerPoint);
            }
            else
            {
                if (cropInfo.PickItem != null)
                {
                    base.Init(new string[] { "Spray", "Pick", "Harvest", "Change Name", "Move Gate", "Stats" }, centerPoint);
                }
                else
                {
                    base.Init(new string[] { "Spray", "Harvest", "Change Name", "Move Gate", "Stats" }, centerPoint);
                }
            }                
            
            _field = field;

            base.ToolClicked += new Action<string, Point>(delegate(string tool, Point toolLoc)
            {
                TycoonWindow poppedUpWindow = null;

                if (tool == "Plow")
                {
                    SimpleTaskWindow window = new SimpleTaskWindow();
                    window.SetupForPlowTask(field);
                    poppedUpWindow = window;                    
                }
                else if (tool == "Plant")
                {
                    SimpleTaskWindow window = new SimpleTaskWindow();
                    window.SetupForPlantTask(field);
                    poppedUpWindow = window;
                }
                else if (tool == "Spray")
                {
                    SimpleTaskWindow window = new SimpleTaskWindow();
                    window.SetupForSprayTask(field);
                    poppedUpWindow = window;
                }
                else if (tool == "Harvest")
                {
                    SimpleTaskWindow window = new SimpleTaskWindow();
                    window.SetupForHarvestTask(field);
                    poppedUpWindow = window;                 
                }
                else if (tool == "Change Name")
                {
                    poppedUpWindow = new ChangeNameWindow(_field);                    
                }
                else if (tool == "Move Gate")
                {
                    EnclosureEntranceEditor entranceEditor = new EnclosureEntranceEditor(_field);
                    entranceEditor.StartEditing();
                }
                else if (tool == "Stats")
                {
                    poppedUpWindow = new MultiQualityWindow(_field);                    
                }
                

                if (poppedUpWindow != null)
                {
                    poppedUpWindow.Top = toolLoc.Y - poppedUpWindow.Height / 2;
                    poppedUpWindow.Left = toolLoc.X - poppedUpWindow.Width / 2;

                    if (poppedUpWindow.Top + poppedUpWindow.Height > Program.UserInterface.Graphics.WindowHeight) { poppedUpWindow.Top = Program.UserInterface.Graphics.WindowHeight - poppedUpWindow.Height; }
                    if (poppedUpWindow.Left + poppedUpWindow.Width > Program.UserInterface.Graphics.WindowWidth) { poppedUpWindow.Left = Program.UserInterface.Graphics.WindowWidth - poppedUpWindow.Width; }                    
                    if (poppedUpWindow.Top < 0) { poppedUpWindow.Top = 0; }
                    if (poppedUpWindow.Left < 0) { poppedUpWindow.Left = 0; }
                }

            });
        }

    }
}
