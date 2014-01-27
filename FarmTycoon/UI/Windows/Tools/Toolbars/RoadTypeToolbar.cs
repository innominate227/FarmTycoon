using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class RoadTypeToolbar : ToolBarWindow
    {

        public RoadTypeToolbar(int dotPosition)            
        {
            base.Init(new string[] { "Road", "Highway" }, dotPosition);
            
            this.Top = 5;
            this.Left = 36;
            this.ToolClicked += new Action<string, int>(ToolClickedHandler);

            this.SelectTool("Road");
        }
        

        private void ToolClickedHandler(string tool, int position)
        {
            this.SelectTool(tool);

            if (tool == "Road")
            {
                //edit roads
                RoadEditor roadEditor = new RoadEditor(false);
                roadEditor.StartEditing();
            }
            else
            {
                //edit highways
                RoadEditor roadEditor = new RoadEditor(true);
                roadEditor.StartEditing();
            }
        }


    }
}
