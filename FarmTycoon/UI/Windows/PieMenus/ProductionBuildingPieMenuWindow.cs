﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    public class ProductionBuildingPieMenuWindow : PieMenuWindow
    {
        public ProductionBuildingPieMenuWindow(ProductionBuilding building, Point centerPoint)            
        {
            base.Init(new string[] { "Move", "Use", "Assign Workers", "Inventory", "Workers", "Change Name" }, centerPoint);
            
            base.ToolClicked += new Action<string, Point>(delegate(string tool, Point toolLoc)
            {
                TycoonWindow poppedUpWindow = null;
                if (tool == "Move")
                {
                    MoveItemsTaskWindow window = new MoveItemsTaskWindow();
                    window.SetupForMoveTask(building);
                    poppedUpWindow = window;
                }
                else if (tool == "Use")
                {
                    MoveItemsTaskWindow window = new MoveItemsTaskWindow();
                    window.SetupForUseTask(building);
                    poppedUpWindow = window;
                }
                else if (tool == "Assign Workers")
                {
                    poppedUpWindow = new ProductionWindow(building);                    
                }
                else if (tool == "Inventory")
                {
                    poppedUpWindow = new InventoryWindow(building.Inventory, true);
                    (poppedUpWindow as InventoryWindow).SetWindowName(building);
                }
                else if (tool == "Workers")
                {
                    poppedUpWindow = new ProductionWorkersWindow(building);                    
                }
                else if (tool == "Change Name")
                {
                    poppedUpWindow = new ChangeNameWindow(building);   
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