using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class BuyLandEditorSizeToolbar : ToolBarWindow
    {
        /// <summary>
        /// Buy Land editor
        /// </summary>
        private BuyLandEditor _buyLandEditor;
        
        public BuyLandEditorSizeToolbar(int dotPosition)            
        {
            if (Program.Game.ScenarioEditMode)
            {
                base.Init(new string[] { "1x1", "2x2", "3x3", "4x4", "5x5", "6x6", "7x7", "8x8", "MoveEntrance" }, dotPosition);
            }
            else
            {
                base.Init(new string[] { "1x1", "2x2", "3x3", "4x4", "5x5", "6x6", "7x7", "8x8" }, dotPosition);
            }

            _buyLandEditor = new BuyLandEditor();
            _buyLandEditor.Size = 1;
            _buyLandEditor.StartEditing();

            this.SelectTool("1x1");

            this.Top = 5;
            this.Left = 36;
            this.ToolClicked += new Action<string, int>(ToolClickedHandler);                      
        }
        

        private void ToolClickedHandler(string tool, int position)
        {
            this.SelectTool(tool);

            if (tool == "MoveEntrance")
            {
                SetFarmEntranceEditor setEntrance = new SetFarmEntranceEditor();
                setEntrance.StartEditing();
            }
            else
            {
                //get the size clicked, and inform the editor
                _buyLandEditor.StartEditing();
                int size = int.Parse(tool.Substring(0, 1));
                _buyLandEditor.Size = size;
            }
        }


    }
}
