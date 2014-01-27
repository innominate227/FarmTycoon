using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{

    /// <summary>
    /// Used to let the user pick one square of land
    /// </summary>
    public class PickLandEditor : Editor
    {        
        public event Action<Land> LandPicked;

        /// <summary>
        /// Land tiles that was selected
        /// </summary>
        private Land _selectedLand = null;        
        
                
        public PickLandEditor() : base() { }
        

        /// <summary>
        /// Start editing land
        /// </summary>
        protected override void StartEditingInner()
        {
            Program.UserInterface.Graphics.Events.MouseDown += new MouseEventHandler(Graphics_MouseDown);
            Program.UserInterface.Graphics.Events.MouseMoved += new MouseEventHandler(Graphics_MouseMoved);
            Program.UserInterface.Graphics.Events.KeyDown += new KeyboardEventHandler(Graphics_KeyDown);
        }
        
        /// <summary>
        /// Stop editing land
        /// </summary>
        protected override void StopEditingInner()
        {            
            if (_selectedLand != null)
            {
                _selectedLand.CornerToHighlight = LandCorner.None;
                _selectedLand = null;
            }

            Program.UserInterface.Graphics.Events.MouseDown -= new MouseEventHandler(Graphics_MouseDown);
            Program.UserInterface.Graphics.Events.MouseMoved -= new MouseEventHandler(Graphics_MouseMoved);
            Program.UserInterface.Graphics.Events.KeyDown -= new KeyboardEventHandler(Graphics_KeyDown);
        }

        
        private void Graphics_KeyDown(Key key)
        {
            if (key == Key.Escape)
            {
                this.StopEditing();
            }
        }


        /// <summary>
        /// User lowered mouse button
        /// </summary>
        private void Graphics_MouseDown(ClickInfo clickInfo)
        {
            if (clickInfo.ControlClicked != null || (clickInfo.Button != MouseButton.Left && clickInfo.Button != MouseButton.Right)) { return; }

            Land selectedLand = _selectedLand;

            //stop editing first because the LandPicked handler wants to put its own mark on the land selected
            this.StopEditing();

            if (selectedLand != null && LandPicked != null)
            {
                LandPicked(selectedLand);
            }
        }
        
        /// <summary>
        /// User moved mouse
        /// </summary>
        private void Graphics_MouseMoved(ClickInfo clickInfo)
        {
            if (_selectedLand != null)
            {
                _selectedLand.CornerToHighlight = LandCorner.None;
                _selectedLand = null;
            }


            _selectedLand = clickInfo.GetLandClicked();
            if (_selectedLand != null)
            {
                _selectedLand.CornerToHighlight = LandCorner.Center;
            }

        }
        

    }
}
