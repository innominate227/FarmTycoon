using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace TycoonGraphicsLib
{
    

    public class TycoonGraphics
    {
        /// <summary>
        /// Event raised after every render to allow the game to update state on the same thread as the render, only rasied when tycoon graphics is started in single threaded mode
        /// </summary>
        public event Action SingleThreadUpdate;

        public static int DEBUG_ShowOnlyLayer = -1;
        public static int DEBUG_CurrentMaxLayer = -1;
        public static int DEBUG_AllTimeMaxLayer = -1;

        #region Member Vars

        /// <summary>
        /// The current game world
        /// </summary>
        private World _world;

        /// <summary>
        /// The primary view of the game world
        /// </summary>
        private WorldView _primaryView;
        
        /// <summary>
        /// The window manager
        /// </summary>
        private WindowManager _windowManager;

        /// <summary>
        /// Main window of the application
        /// </summary>
        private ApplicationWindow _appWindow;

        /// <summary>
        /// Events raised by the Tycoon Graphics
        /// </summary>
        private Events _events;

        /// <summary>
        /// Are we running in single thread mode
        /// </summary>
        private bool _singleThreadMode = false;

        /// <summary>
        /// Has the graphic engine been started
        /// </summary>
        private bool _started = false;

        /// <summary>
        /// Graphics settings being used
        /// </summary>
        private TycoonGraphicsSettings _settings;
        
        /// <summary>
        /// Raised after the new graphics objects have been loaded on the UI thread
        /// </summary>
        private AutoResetEvent _waitForGraphicsToupdateEvent = new AutoResetEvent(false);

        #endregion
        
        #region Setup
        
        /// <summary>
        /// Create a new TycoonGraphics object
        /// </summary>        
        public TycoonGraphics(bool singleThreadedMode, bool limitTo60fps)
        {
            _singleThreadMode = singleThreadedMode;

            //setup the main window for the application
            _appWindow = new ApplicationWindow(1024, 768);
            _appWindow.WindowState = OpenTK.WindowState.Maximized;

            //limit frame rate if requested
            if (limitTo60fps)
            {
                _appWindow.VSync = OpenTK.VSyncMode.On;
            }
            else
            {
                _appWindow.VSync = OpenTK.VSyncMode.Off;
            }
            
            //setup event manager
            _events = new Events(_appWindow);

            //handel same thread update even if we need to
            if (singleThreadedMode)
            {
                _appWindow.UpdateFrame += new EventHandler<OpenTK.FrameEventArgs>(delegate
                {
                    if (SingleThreadUpdate != null)
                    {
                        SingleThreadUpdate();
                    }
                });
            }
            
            //width and height for the primary view should always be the same as for the window
            WindowSettings.SettingsChanged += new Action(delegate
            {
                if (_primaryView != null)
                {
                    _primaryView.RenderWidth = WindowSettings.Width;
                    _primaryView.RenderHeight = WindowSettings.Height;
                }
            });

        }
        
        /// <summary>
        /// Setup the graphics with the settings passed.
        /// This must be called after creating Tycoon Graphics and before calling Start().
        /// This can be called again to modify graphics settings, all windows and all tiles will be deleted when this is called again.
        /// </summary>
        public void SetupGraphics(TycoonGraphicsSettings settings)
        {
            _settings = settings;
                        
            if (_started == false || _singleThreadMode == true)
            {
                //if we have not yet started the graphic thread, or we are running in single thread mode then we can load objects on this thread
                SetupGraphicInternal();
            }
            else
            {
                //otherwise handel an update frame event wich will be on the UI thread
                _appWindow.UpdateFrame += new EventHandler<OpenTK.FrameEventArgs>(SetupGraphicInternalUIThreat);

                //wait for the graphics to be updated
                _waitForGraphicsToupdateEvent.WaitOne();
            }
        }
        
        /// <summary>
        /// Used when we need to update the graphics objects on the UI thread
        /// </summary>
        private void SetupGraphicInternalUIThreat(object sender, OpenTK.FrameEventArgs e)
        {
            //reload the graphics
            SetupGraphicInternal();

            //stop handeling the update frame event
            _appWindow.UpdateFrame -= new EventHandler<OpenTK.FrameEventArgs>(SetupGraphicInternalUIThreat);

            //the graphics have been updated
            _waitForGraphicsToupdateEvent.Set();
        }

        /// <summary>
        /// create or recreate the window manager and the world, based on the current graphics settings
        /// </summary>
        private void SetupGraphicInternal()
        {     
            //delete old window manager if there is one
            if (_windowManager != null)
            {
                _windowManager.Delete();
            }

            //delete old world if there is one
            if (_world != null)
            {
                _world.Delete();
            }

            //set up window manager
            _windowManager = new WindowManager(_settings.WindowIconsBitmapFile, _settings.WindowIconsRegionsFile);

            //create the world
            WorldSettings worldSettings = new WorldSettings(_settings.GameSize, _settings.MaxZ, _settings.Layers, _settings.SegmentLenght, _settings.TextureBitmapFile, _settings.TextureRegionsFile, _settings.TextureQuartetsFile, _settings.ForceMinTextureSize);
            _world = new World(worldSettings);

            //create a primary view for the world
            _primaryView = new WorldView(_world);
            _primaryView.Direction = ViewDirection.North;
            _primaryView.Scale = 1.0f;
            _primaryView.Overdraw = 5;
            _primaryView.RenderLocX = 0;
            _primaryView.RenderLocY = 0;
            _primaryView.RenderWidth = WindowSettings.Width;
            _primaryView.RenderHeight = WindowSettings.Height;
            _primaryView.X = 0;
            _primaryView.Y = 0;

            //tell event about the new primary view, and window manager
            _events.SetPrimaryView(_primaryView);
            _events.SetWindowManager(_windowManager);

            //tell the main window to start using the new object to render
            _appWindow.SetRenderObjects(_world, _primaryView, _windowManager);
            
        }
        
        /// <summary>
        /// Start the main window running
        /// </summary>
        public void Start()
        {
            _started = true;
            _appWindow.Run();
        }

        #endregion
             
        #region World
        
        /// <summary>
        /// Set the X location of the world to view
        /// </summary>
        public float ViewX
        {
            get { return _primaryView.X; }
            set { _primaryView.X = value; }
        }

        /// <summary>
        /// Set the Y location of the world to view
        /// </summary>
        public float ViewY
        {
            get { return _primaryView.Y; }
            set { _primaryView.Y = value; }
        }

        /// <summary>
        /// Set the Z location of the world to view
        /// </summary>
        public float ViewZ
        {
            get { return _primaryView.Z; }
            set { _primaryView.Z = value; }
        }

        /// <summary>
        /// Set the direction the world is viewed from
        /// </summary>
        public ViewDirection ViewRotation
        {
            get { return _primaryView.Direction; }
            set { _primaryView.Direction = value; }
        }

        /// <summary>
        /// Set the scale the world is viewed at
        /// </summary>
        public float Scale
        {
            get { return _primaryView.Scale; }
            set { _primaryView.Scale = value; }
        }

        /// <summary>
        /// Create a new fixed tile in the current world.
        /// </summary>
        public FixedTile NewFixedTile()
        {
            return _world.TileManger.CreateFixedTile();
        }

        /// <summary>
        /// Create a new mobile tile in the current world.
        /// </summary>
        public MobileTile NewMobileTile()
        {
            return _world.TileManger.CreateMobileTile();
        }
        
        /// <summary>
        /// Width of the application window
        /// </summary>
        public int WindowWidth
        {
            get { return _appWindow.Width; }
            set { _appWindow.Width = value; }
        }

        /// <summary>
        /// Height of the application window
        /// </summary>
        public int WindowHeight
        {
            get { return _appWindow.Height; }
            set { _appWindow.Height = value; }
        }
        
        #endregion
        
        #region Windows
        
        /// <summary>
        /// Events raised by the Tycoon Graphics.  
        /// By default events are queued and raised when you call RaiseQueuedEvents on the thread that you call the method from
        /// </summary>
        public Events Events
        {
            get { return _events; }
        }
        
        /// <summary>
        /// Add a window to be shown on the screen
        /// </summary>
        public void AddWindow(TycoonWindow window)
        {
            window.World = _world;
            _windowManager.AddWindow(window);
        }
        
        /// <summary>
        /// Remove a window
        /// </summary>
        public void RemoveWindow(TycoonWindow window)
        {
            _windowManager.RemoveWindow(window);            
        }
        
        #endregion

    }
}
