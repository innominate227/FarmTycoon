using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace TycoonGraphicsLib
{

    /// <summary>
    /// Manages all the window being shown
    /// </summary>
    internal class WindowManager
    {
        /// <summary>
        /// Manages drawing the windows
        /// </summary>
        private WindowDrawingManager _drawingManager;

        /// <summary>
        /// Manages clicks and other mouse movement realted to the windows
        /// </summary>
        private WindowClickManager _clickManager;

        /// <summary>
        /// List of the windows in their draw order
        /// </summary>
        private List<TycoonWindow> _windows = new List<TycoonWindow>();

        /// <summary>
        /// The drop box window (or null if no drop box is dropped)
        /// </summary>
        private TycoonWindow _dropBoxWindow = null;
        private Object _dropBoxWindowLock = new Object();

        /// <summary>
        /// The tool tip window (or null if no tool tip is displayed)
        /// </summary>
        private TycoonWindow _toolTipWindow = null;
        private Object _toolTipWindowLock = new Object();
                
        /// <summary>
        /// Create a new window mamanger
        /// </summary>
        public WindowManager(string windowCommonTexturesBitmapFile, string windowCommonTexturesRegionsFile)
        {
            _drawingManager = new WindowDrawingManager(this, windowCommonTexturesBitmapFile, windowCommonTexturesRegionsFile);
            _clickManager = new WindowClickManager(this);
        }
        
        /// <summary>
        /// Manages drawing the windows
        /// </summary>
        public WindowDrawingManager DrawingManager
        {
            get { return _drawingManager; }
        }

        /// <summary>
        /// Manages clicks and other mouse movement realted to the windows
        /// </summary>
        public WindowClickManager ClickManager
        {
            get { return _clickManager; }
        }
        
        /// <summary>
        /// List of the windows in their draw order (use add/remove window to edit this list).        
        /// Returns a copy of the window list, so that windows can be added and removed while currnet window are being drawn
        /// </summary>
        public List<TycoonWindow> Windows
        {
            get 
            {
                lock (_windows)
                {
                    return _windows.ToList();
                }
            }
        }
                

        /// <summary>
        /// add a window to the window manager
        /// </summary>
        /// <param name="window"></param>
        public void AddWindow(TycoonWindow window)
        {
            //tell the window who its window manager is
            window.WindowManager = this;
                        
            //add to the list of all windows
            lock (_windows)
            {
                _windows.Add(window);
            }
        }
        
        /// <summary>
        /// remove a window from the window manager
        /// </summary>
        public void RemoveWindow(TycoonWindow window)
        {            
            lock (_windows)
            {
                //do nothing if window already removed
                if (_windows.Contains(window) == false) { return; }
                
                //remove from the list of all windows            
                _windows.Remove(window);

                //delete the drawer for the old window (important to do this after it is removed form the list so we dont try and draw it)
                _drawingManager.DeleteWindowDrawer(window);
            }
        }
        
        /// <summary>
        /// The drop box window (or null if no drop box is dropped)
        /// </summary>
        public TycoonWindow DropboxWindow
        {
            get
            {                
                return _dropBoxWindow;                 
            }
            set
            {                
                //remeber the old dropbox window
                TycoonWindow oldDropboxWindow = _dropBoxWindow;
                    
                //if dropbox was not set to null
                if (value != null)
                {
                    //tell the window who its window manager is
                    value.WindowManager = this;
                }

                //set dropbox window to the new one
                _dropBoxWindow = value;

                //delete the drawer for the old one (imporant to do after we set _dropBoxWindow so we dont try and draw the deleted one)
                if (oldDropboxWindow != null)
                {
                    //destory the drawer for the old dropbox window
                    _drawingManager.DeleteWindowDrawer(oldDropboxWindow);
                }                
            }
        }
        
        /// <summary>
        /// The tool tip window (or null if no tool tip is displayed)
        /// </summary>
        public TycoonWindow ToolTipWindow
        {
            get 
            {                
                return _toolTipWindow;                
            }
            set
            {
                //remeber the old tooltip window
                TycoonWindow oldTooltipWindow = _toolTipWindow;

                //if tooltip was not set to null
                if (value != null)
                {
                    //tell the window who its window manager is
                    value.WindowManager = this;
                }

                //set tooltip window to the new one
                _toolTipWindow = value;

                //delete the drawer for the old one (imporant to do after we set _toolTipWindow so we dont try and draw the deleted one)
                if (oldTooltipWindow != null)
                {
                    //destory the drawer for the old tolltip window
                    _drawingManager.DeleteWindowDrawer(oldTooltipWindow);
                }               
            }
        }
        
        /// <summary>
        /// move the window passed to the front
        /// </summary>
        /// <param name="window"></param>
        public void MoveWindowToFront(TycoonWindow window)
        {
            lock (_windows)
            {
                //remove the window from where ever it currently is
                _windows.Remove(window);

                //add it back to the list so its at the back now (back of list is the last one rendered, so it ends up being the front)
                _windows.Add(window);
            }
        }

        /// <summary>
        /// Delete the resorces used by the window manager.
        /// There should only be one thread running while this is called.
        /// No other methods will be called after this method is called.
        /// </summary>
        public void Delete()
        {
            _drawingManager.Delete();
        }

    }
}
