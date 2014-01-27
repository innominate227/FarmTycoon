using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Diagnostics;

namespace TycoonGraphicsLib
{

    /// <summary>
    /// Manages all the window being shown
    /// </summary>
    internal class WindowDrawingManager
    {
        /// <summary>
        /// Texture sheet for the icons, and other texture shared by all windows
        /// </summary>
        private TextureSheet _commonTextureSheet;

        /// <summary>
        /// List of the window drawers
        /// </summary>
        private Dictionary<TycoonWindow, WindowDrawer> _windowDrawers = new Dictionary<TycoonWindow, WindowDrawer>();
        
        /// <summary>
        /// Window drawers to be deleted on the next draw
        /// </summary>
        private List<TycoonWindow> _dealyedDeleteWindowDrawers = new List<TycoonWindow>();
        

        /// <summary>
        /// Window manager
        /// </summary>
        private WindowManager _windowManager;
        
        /// <summary>
        /// Create a new WindowDrawingManager
        /// </summary>
        /// <param name="windowTexturesFolder"></param>
        public WindowDrawingManager(WindowManager windowManager, string windowCommonTexturesBitmapFile, string windowCommonTexturesRegionsFile)
        {
            _windowManager = windowManager;

            CreateCommonTextureSheet(windowCommonTexturesBitmapFile, windowCommonTexturesRegionsFile);
            
            //recreate all window buffers if the main window size changes
            WindowSettings.SettingsChanged += new Action(ReCreateAllWindowBuffersNextFrame);            
        }

        /// <summary>
        /// Recreate all window buffers next frame, this is needed if the application window changes size
        /// </summary>
        private void ReCreateAllWindowBuffersNextFrame()
        {
            foreach (WindowDrawer drawer in _windowDrawers.Values)
            {
                drawer.ReCreateBuffersNextFrame();
            }             
        }


        /// <summary>
        /// Create the common texture sheet for the windows
        /// </summary>
        private void CreateCommonTextureSheet(string windowCommonTexturesBitmapFile, string windowCommonTexturesRegionsFile)
        {                            
            //load icons bitmap
            Bitmap textureSheetImage = new Bitmap(windowCommonTexturesBitmapFile);
            textureSheetImage.MakeTransparent(Color.Blue);

            //read the icons locations file into memory            
            StreamReader texturesFileReader = new StreamReader(windowCommonTexturesRegionsFile);
            string texturesFileContents = texturesFileReader.ReadToEnd();
            texturesFileReader.Close();

            //parse the locations of each icon from the file            
            List<TextureSheetLocation> textureSheetLocations = new List<TextureSheetLocation>();
            foreach (string textureFileLine in texturesFileContents.Split('\n'))
            {
                //skip blank lines and lines with comments
                if (textureFileLine.Trim() != "" && textureFileLine.Trim().StartsWith("#") == false)
                {
                    TextureSheetLocation textureSheetLocation = new TextureSheetLocation();
                    textureSheetLocation.ParseFromTexturesFileLine(textureFileLine);
                    textureSheetLocations.Add(textureSheetLocation);
                }
            }

            //create common texture sheet
            _commonTextureSheet = new TextureSheet(textureSheetImage, textureSheetLocations);
            
        }


        /// <summary>
        /// Do delayed deletes of window drawers
        /// </summary>
        private void DoDelayedDeletes()
        {
            lock (_dealyedDeleteWindowDrawers)
            {
                foreach (TycoonWindow window in _dealyedDeleteWindowDrawers)
                {
                    //it seems to be possible that a window drawer was never created for the window
                    if (_windowDrawers.ContainsKey(window) == false){ continue;}

                    //delete the drawer for the window if it exists 
                    WindowDrawer drawer = _windowDrawers[window];
                    drawer.Delete();

                    //remove from the list of all windows            
                    _windowDrawers.Remove(window);   
                }
                _dealyedDeleteWindowDrawers.Clear();                
            }
        }
                
        /// <summary>
        /// destory the window drawer for the window passed on the next refresh of the windows.
        /// This should not be called unitl the window has been removed from the Windows list
        /// </summary>
        public void DeleteWindowDrawer(TycoonWindow window)
        {
            lock (_dealyedDeleteWindowDrawers)
            {
                _dealyedDeleteWindowDrawers.Add(window);
            }       
        }
        
        /// <summary>
        /// Draw all the windows
        /// </summary>
        public void DrawWindows()
        {
            //delete drawers of windows that were removed
            DoDelayedDeletes();

            //get all the windows (expcet for dropbox, and tooltip windows)
            //this returns a copy of the windows list so its ok to modify
            List<TycoonWindow> allWindows = _windowManager.Windows;

            //add dropbox window if there is one
            TycoonWindow dropBoxWindow = _windowManager.DropboxWindow;
            if (dropBoxWindow != null)
            {
                allWindows.Add(dropBoxWindow);
            }

            //add tooltip window last if there is one
            TycoonWindow toolTipWindow = _windowManager.ToolTipWindow;
            if (toolTipWindow != null)
            {
                allWindows.Add(toolTipWindow);
            }
            
            //draw each window
            foreach (TycoonWindow window in allWindows)
            {
                //if a drawer has not been created for the window create one
                if (_windowDrawers.ContainsKey(window) == false)
                {
                    WindowDrawer newDrawer = new WindowDrawer(window, _commonTextureSheet);
                    window.Drawer = newDrawer;
                    _windowDrawers.Add(window, newDrawer);
                }

                //get the window drawer, and draw the window                
                _windowDrawers[window].RenderWindow();                    
            }
            
        }
        
        /// <summary>
        /// Delete the resorces used by the window manager.
        /// There should only be one thread running while this is called.
        /// No other methods will be called after this method is called.
        /// </summary>
        public void Delete()
        {
            WindowSettings.SettingsChanged -= new Action(ReCreateAllWindowBuffersNextFrame);

            _commonTextureSheet.Delete();
            foreach (WindowDrawer windowDrawer in _windowDrawers.Values)
            {
                windowDrawer.Delete();
            }
        }

    }
}
