using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    /// <summary>
    /// Holds a reference to all windows that have been opened by the 
    /// </summary>
    public class WindowManager
    {

        /// <summary>
        /// All the windows
        /// </summary>
        private HashSet<TycoonWindow> _windows = new HashSet<TycoonWindow>();


        public void AddWindow(TycoonWindow window)
        {
            _windows.Add(window);
            Program.UserInterface.Graphics.AddWindow(window);
        }

        public void RemoveWindow(TycoonWindow window)
        {
            if (_windows.Contains(window))
            {
                _windows.Remove(window);
                Program.UserInterface.Graphics.RemoveWindow(window);
            }
        }

        /// <summary>
        /// Create all toolbar windows that should be shown during gameplay
        /// </summary>
        public void CreateToolbars()
        {            
            new BottomLeftStatus();
            new BottomRightStatus();
            new IssueOverlay();
            new EditorToolbar();
            new RightToolbar();
        }



        /// <summary>
        /// Delete all windows
        /// </summary>
        public void DeleteAllWindows()
        {
            foreach (TycoonWindow window in _windows.ToArray())
            {
                window.CloseWindow();
                RemoveWindow(window);                
            }
        }


        /// <summary>
        /// Delete all sub toolbar windows except the one passed
        /// </summary>
        public void DeleteSubToolbars()
        {
            DeleteSubToolbars(null);
        }

        /// <summary>
        /// Delete all sub toolbar windows except the one passed
        /// </summary>
        public void DeleteSubToolbars(ToolBarWindow excpet)
        {
            foreach (TycoonWindow window in _windows.ToArray())
            {
                if (window is ToolBarWindow && (window as ToolBarWindow).IsMainToolbar() == false && window != excpet)
                {
                    window.CloseWindow();
                    RemoveWindow(window);
                }
            }
        }



    }
}
