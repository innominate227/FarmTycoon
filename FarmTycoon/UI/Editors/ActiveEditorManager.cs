using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// Manages which editor is active, makes the old editor inactive when a new one is made active
    /// </summary>
    public class ActiveEditorManager
    {
        public event Action ActiveEditorChanged;
        
        /// <summary>
        /// The default editor
        /// </summary>
        private Editor _defaultEditor;

        /// <summary>
        /// The active editor
        /// </summary>
        private Editor _activeEditor;
        
        /// <summary>
        /// Get or set the default editor
        /// </summary>
        public Editor DefaultEditor
        {
            get { return _defaultEditor; }            
            set 
            {
                _defaultEditor = value;

                //if there isnt currently an active editor, make the default editor active
                if (_activeEditor == null)
                {
                    _defaultEditor.StartEditing();
                }
            }            
        }

        /// <summary>
        /// Get the active editor, this is set by calling StartEditing() on an editor
        /// </summary>
        public Editor ActiveEditor
        {
            get { return _activeEditor; }            
        }


        /// <summary>
        /// Set the currently active editor, this method should only be called by the Editor class.
        /// return false if the editor was already active
        /// </summary>
        public bool MakeEditorActive(Editor activeEditor)
        {
            //editor is already the active editor so do nothing
            if (_activeEditor == activeEditor)
            {
                return false;
            }

            //remeber who the old editor was
            Editor oldActiveEditor = _activeEditor;
            
            //set the newly active editor
            _activeEditor = activeEditor;

            //tell the old editor to stop (this will cause it to call MakeEditorInactive, but the method will do nothing as it is no longer the active editor)
            if (oldActiveEditor != null)
            {
                oldActiveEditor.StopEditing();
            }

            //raise active editor changed event
            if (ActiveEditorChanged != null)
            {
                ActiveEditorChanged();
            }

            return true;
        }



        /// <summary>
        /// Set the editor passed as no longer being active, if that editor was the active editor the active editor will be set to the default editor
        /// </summary>
        public void MakeEditorInactive(Editor editor)
        {
            //cant make the defualt editor inactive
            if (editor == _defaultEditor) { return; }

            //if the editor thats no longer active was the active editor then set the active editor to the default editor
            if (editor == _activeEditor)
            {
                _defaultEditor.StartEditing();
            }
        }



    }
}
