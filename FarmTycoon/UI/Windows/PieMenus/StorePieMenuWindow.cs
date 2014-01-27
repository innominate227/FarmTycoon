using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    public class StorePieMenuWindow : PieMenuWindow
    {
        public StorePieMenuWindow(Point centerPoint)            
        {
            base.Init(new string[] { "Buy", "Sell", "Inventory" }, centerPoint);

            base.ToolClicked += new Action<string, Point>(delegate(string tool, Point toolLoc)
            {
                TycoonWindow poppedUpWindow = null;

                DeliveryArea deliveryArea = GameState.Current.MasterObjectList.Find<DeliveryArea>();
                if (tool == "Buy")
                {
                    MoveItemsTaskWindow window = new MoveItemsTaskWindow();
                    window.SetupForBuyTask(null);
                    poppedUpWindow = window;
                }
                else if (tool == "Sell")
                {
                    MoveItemsTaskWindow window = new MoveItemsTaskWindow();
                    window.SetupForSellTask(null);
                    poppedUpWindow = window;
                }
                else if (tool == "Inventory")
                {
                    poppedUpWindow = new InventoryWindow(deliveryArea.Inventory, true);
                    (poppedUpWindow as InventoryWindow).SetWindowName(deliveryArea);
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
