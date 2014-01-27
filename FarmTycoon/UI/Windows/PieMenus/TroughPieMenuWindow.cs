using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    public class TroughPieMenuWindow : PieMenuWindow
    {
        public TroughPieMenuWindow(Trough trough, Point centerPoint)            
        {
            base.Init(new string[] { "Fill", "Inventory", "Change Name" }, centerPoint);
            
            base.ToolClicked += new Action<string, Point>(delegate(string tool, Point toolLoc)
            {
                TycoonWindow poppedUpWindow = null;

                if (tool == "Fill")
                {
                    SimpleTaskWindow window = new SimpleTaskWindow();
                    window.SetupForFillTroughTask(trough);
                    poppedUpWindow = window;
                }
                else if (tool == "Inventory")
                {
                    poppedUpWindow = new InventoryWindow(trough.Inventory, true);
                    (poppedUpWindow as InventoryWindow).SetWindowName(trough);
                }
                else if (tool == "Change Name")
                {
                    poppedUpWindow = new ChangeNameWindow(trough);   
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
