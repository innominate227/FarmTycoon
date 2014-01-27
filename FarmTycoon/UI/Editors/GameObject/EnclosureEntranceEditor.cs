using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    /// <summary>
    /// Editor for selecting where the entrance to a field should be
    /// </summary>
    public class EnclosureEntranceEditor : Editor
    {

        private Enclosure _enclosure;

        /// <summary>
        /// Start editing
        /// </summary>
        protected override void StartEditingInner()
        {
            Program.UserInterface.Graphics.Events.MouseDown += new MouseEventHandler(Graphics_MouseDown);
            Program.UserInterface.Graphics.Events.MouseMoved += new MouseEventHandler(Graphics_MouseMoved);
        }

        /// <summary>
        /// Stop editing
        /// </summary>
        protected override void StopEditingInner()
        {
            Program.UserInterface.Graphics.Events.MouseDown -= new MouseEventHandler(Graphics_MouseDown);
            Program.UserInterface.Graphics.Events.MouseMoved -= new MouseEventHandler(Graphics_MouseMoved);
        }

        /// <summary>
        /// Create a new field entrace editor passing the field whos entrance is to be edited
        /// </summary>
        public EnclosureEntranceEditor(Enclosure enclosure)
            : base()
        {
            _enclosure = enclosure;
        }


        /// <summary>
        /// User moved mouse button
        /// </summary>
        private void Graphics_MouseMoved(ClickInfo clickInfo)
        {            
            if (clickInfo.GetGameObjectClicked() != null && clickInfo.GetGameObjectClicked() is Fence)
            {
                Fence fenceClicked = (Fence)clickInfo.GetGameObjectClicked();
                foreach (Fence fence in _enclosure.BorderFences)
                {
                    //if the fence clicked was a border fence for this field and only this field
                    if (fenceClicked == fence && fenceClicked.EnclosuresBordered.Count == 1)
                    {
                        //get location where the gate used to be
                        Land oldGateLocation = _enclosure.Gate.LandOn;

                        //set that to be the new gate
                        _enclosure.Gate = fenceClicked;

                        //TODO: need to invlidiate the path caches in gates old location and new location

                    }
                }
            }
        }

        /// <summary>
        /// User lowered mouse button
        /// </summary>
        private void Graphics_MouseDown(ClickInfo clickInfo)
        {
            if (clickInfo.Button != MouseButton.Left && clickInfo.Button != MouseButton.Right) { return; }
            if (clickInfo.TileClicked)
            {
                //stop editing
                this.StopEditing();
            }
        }

        

    }
}
