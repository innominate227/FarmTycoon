using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{


    /// <summary>
    /// An editor captures all mouse and keyboard input, and manipulates the game world in some way depending on the editor type
    /// </summary>
    public abstract class Editor
    {
        /// <summary>
        /// Event raised when editing has stopped
        /// </summary>
        public event Action EditingStopped;
        
        /// <summary>
        /// Create a new editor that edits the current game.
        /// </summary>
        public Editor()
        {
        }

        
        /// <summary>
        /// Have the editor start editing.  This will make it the active editor.
        /// </summary>
        public void StartEditing()
        {
            //set yourself as the active editor
            bool newlyActive = Program.UserInterface.ActiveEditorManager.MakeEditorActive(this);

            //if it was already active we dont need to call the Start Editing method
            if (newlyActive)
            {
                //call start editing in the concrete class
                StartEditingInner();
            }
        }

        /// <summary>
        /// Have the editor stop editing.  If this editor was active, the default editor will become active instead.
        /// </summary>
        public void StopEditing()
        {
            //call stop editing in the concrete class
            StopEditingInner();
            
            //set yourself as no longer active
            Program.UserInterface.ActiveEditorManager.MakeEditorInactive(this);

            //raise editing stopped event
            if (EditingStopped != null)
            {
                EditingStopped();
            }
        }

        
        protected abstract void StartEditingInner();
        protected abstract void StopEditingInner();
    }
}
