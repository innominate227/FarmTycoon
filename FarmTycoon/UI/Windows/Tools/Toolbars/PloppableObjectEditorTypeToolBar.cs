using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    public class PloppableObjectEditorTypeToolBar : ToolBarWindow
    {
        /// <summary>
        /// editor
        /// </summary>
        private PloppableEditor _ploppableEditor;


        private Dictionary<string, IPloppableInfo> _infosForTools = new Dictionary<string, IPloppableInfo>();

        public PloppableObjectEditorTypeToolBar(List<IPloppableInfo> objectInfos, int dotPosition, int left)            
        {
            List<string> typeNames = new List<string>();
            foreach (IPloppableInfo objectInfo in objectInfos)
            {
                typeNames.Add(objectInfo.Name);
                _infosForTools.Add(objectInfo.Name, objectInfo);
            }

            base.Init(typeNames.ToArray(), dotPosition);
            
            this.Top = 5;
            this.Left = left;
            this.ToolClicked += new Action<string, int>(ToolClickedHandler);

            if (objectInfos.Count == 0) { return; }

            this.SelectTool(typeNames[0]);

            //start editing that building
            _ploppableEditor = new PloppableEditor();
            _ploppableEditor.SetObjectInfo(objectInfos[0]);
            _ploppableEditor.StartEditing();
        }
        

        private void ToolClickedHandler(string tool, int position)
        {
            this.SelectTool(tool);

            //get the object type to create
            IPloppableInfo objectInfo = _infosForTools[tool];

            //set building type to place
            _ploppableEditor.SetObjectInfo(objectInfo);            
        }


    }
}
