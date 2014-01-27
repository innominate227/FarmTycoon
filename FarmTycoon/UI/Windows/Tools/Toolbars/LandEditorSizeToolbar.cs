using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class LandEditorSizeToolbar : ToolBarWindow
    {
        /// <summary>
        /// Land editor
        /// </summary>
        private LandEditor _landEditor;


        public LandEditorSizeToolbar(int dotPosition)            
        {
            base.Init(new string[] { "1x1", "2x2", "3x3", "4x4", "5x5", "6x6", "7x7", "8x8" }, dotPosition);

            _landEditor = new LandEditor();
            _landEditor.Smoothed = true;
            _landEditor.StartEditing();

            this.Top = 5;
            this.Left = 36;
            this.ToolClicked += new Action<string, int>(ToolClickedHandler);

            _landEditor.Size = 1;
            this.SelectTool("1x1");
        }
        

        private void ToolClickedHandler(string tool, int position)
        {
            this.SelectTool(tool);

            //get the size clicked, and inform the editor
            int size = int.Parse(tool.Substring(0, 1));
            _landEditor.Size = size;
        }


    }
}
