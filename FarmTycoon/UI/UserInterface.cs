using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.IO;
using System.Drawing;

namespace FarmTycoon
{
    /// <summary>
    /// Contains references to several classes relating the the game UI
    /// </summary>
    public class UserInterface
    {
        /// <summary>
        /// Graphics engine of the game
        /// </summary>
        private TycoonGraphics _graphics;

        /// <summary>
        /// Which game editor is currently active.
        /// </summary>
        private ActiveEditorManager _activeEditorManager;

        /// <summary>
        /// Manage all the windows that have been created
        /// </summary>
        private WindowManager _windowManager;

        /// <summary>
        /// Manages our view of the game world
        /// </summary>
        private WorldViewManager _worldViewManager;


        public UserInterface()
        {
        }

        /// <summary>
        /// Should be called right after the constructor, seperate from contructor so that UserInterface reference from Program has been set already
        /// </summary>
        public void Setup()
        {
            //create tycoon graphics engine
            _graphics = new TycoonGraphics(Program.Settings.MultiThread == false, Program.Settings.LimitTo60fps);
            
            //create a window manager
            _windowManager = new WindowManager();
            
            //create active editor manager (default editor is no editor)
            _activeEditorManager = new ActiveEditorManager();
            _activeEditorManager.DefaultEditor = new NullEditor();

            ///create world view manager
            _worldViewManager = new WorldViewManager();
        }


        /// <summary>
        /// Load Game Textures, pass the folder containing the window and game world texture files
        /// </summary>
        public void SetupGameWorld(string texturesFolder, int gameSize)
        {
            string fullTexturesFolder = Program.Settings.DataFolder + Path.DirectorySeparatorChar + texturesFolder;

            //setup the graphics with the next textures
            TycoonGraphicsSettings settings = new TycoonGraphicsSettings();
            settings.WindowIconsBitmapFile = fullTexturesFolder + Path.DirectorySeparatorChar + "wintexturemap.bmp";
            settings.WindowIconsRegionsFile = fullTexturesFolder + Path.DirectorySeparatorChar + "wintextures.txt";
            settings.GameSize = gameSize;
            settings.MaxZ = 50;
            settings.Layers = 300;
            settings.SegmentLenght = 50;
            settings.TextureBitmapFile = fullTexturesFolder + Path.DirectorySeparatorChar + "texturemap.bmp";
            settings.TextureRegionsFile = fullTexturesFolder + Path.DirectorySeparatorChar + "textures.txt";
            settings.TextureQuartetsFile = fullTexturesFolder + Path.DirectorySeparatorChar + "quartets.txt";
            settings.ForceMinTextureSize = Program.Settings.ForceMinTextureSize;
            _graphics.SetupGraphics(settings);

        }

        
        
        /// <summary>
        /// Graphics engine of the game
        /// </summary>
        public TycoonGraphics Graphics
        {
            get { return _graphics; }
        }

        /// <summary>
        /// Which game editor is currently active.
        /// </summary>
        public ActiveEditorManager ActiveEditorManager
        {
            get { return _activeEditorManager; }
        }

        /// <summary>
        /// Manage all the windows that have been created
        /// </summary>
        public WindowManager WindowManager
        {
            get { return _windowManager; }
        }

        /// <summary>
        /// Manages our view of the game world
        /// </summary>
        public WorldViewManager WorldViewManager
        {
            get { return _worldViewManager; }
        }

    }
}
