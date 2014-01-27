using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TycoonGraphicsLib
{

    /// <summary>
    /// Window settings
    /// </summary>
    internal class WindowSettings
    {
        /// <summary>
        /// Event raised when any window settings change
        /// </summary>
        public static event Action SettingsChanged;

        /// <summary>
        /// Width of the game window
        /// </summary>
        private static int _width;

        /// <summary>
        /// Height of the game window
        /// </summary>
        private static int _height;


        /// <summary>
        /// Width of the game window
        /// </summary>
        public static int Width
        {
            get { return _width; }
            set 
            {                
                _width = value;
                //this fixes an issue where tiles would not render pixel perfect at odd numbered window sizes
                if (_width % 2 == 1) { _width -= 1; }
                RaiseSettingsChanged();
                GraphicsDebug.Debug1 = "width:" + _width.ToString() + "  PPPX: " + PointsPerPixelX.ToString("R");
            }
        }

        /// <summary>
        /// Height of the game window
        /// </summary>
        public static int Height
        {
            get { return _height; }
            set 
            {
                _height = value;
                //this fixes an issue where tiles would not render pixel perfect at odd numbered window sizes
                if (_height % 2 == 1) { _height -= 1; }
                RaiseSettingsChanged();
                GraphicsDebug.Debug2 = "height:" + _height.ToString() + "  PPPY: " + PointsPerPixelY.ToString("R");
            }
        }

        /// <summary>
        /// Number of open gl points for each pixel in the X direction 
        /// </summary>
        public static float PointsPerPixelX
        {
            get { return 2.0f / _width; }
        }

        /// <summary>
        /// Number of open gl points for each pixel in the Y direction 
        /// </summary>
        public static float PointsPerPixelY
        {
            get { return 2.0f / _height; }
        }
        

        private static void RaiseSettingsChanged()
        {
            if (SettingsChanged != null)
            {
                SettingsChanged();
            }
        }

    }
}
