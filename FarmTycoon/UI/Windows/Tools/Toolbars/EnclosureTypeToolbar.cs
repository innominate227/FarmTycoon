using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class EnclosureTypeToolbar : ToolBarWindow
    {
        public EnclosureTypeToolbar(int dotPosition)            
        {
            base.Init(new string[] { "Field", "Pasture" }, dotPosition);

            this.SelectTool("Field");

            EnclosureEditor enclosureEditor = new EnclosureEditor(EnclosureType.Field);
            enclosureEditor.StartEditing();

            this.Top = 5;
            this.Left = 36;
            this.ToolClicked += new Action<string, int>(ToolClickedHandler);                      
        }
        

        private void ToolClickedHandler(string tool, int position)
        {
            this.SelectTool(tool);

            if (tool == "Field")
            {
                EnclosureEditor enclosureEditor = new EnclosureEditor(EnclosureType.Field);
                enclosureEditor.StartEditing();
            }
            else if (tool == "Pasture")
            {
                EnclosureEditor enclosureEditor = new EnclosureEditor(EnclosureType.Pasture);
                enclosureEditor.StartEditing();
            }
            
        }


    }
}
