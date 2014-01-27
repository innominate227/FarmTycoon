using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Threading;

namespace TycoonGraphicsLib
{
    internal class WindowDrawer
    {
        /// <summary>
        /// Texture sheet for the icons and string shared by all windows
        /// </summary>
        private TextureSheet _commonTextureSheet;

        /// <summary>
        /// Texture sheet for the string textures in this window
        /// </summary>
        private TextureSheet _localTextureSheet;
        
        /// <summary>
        /// Panel drawer for the top level panel of the window
        /// </summary>
        private PanelDrawer _mainPanelDrawer;

        /// <summary>
        /// The window being drawn
        /// </summary>
        private TycoonWindow _window;

        /// <summary>
        /// Object locked while the window is creating its buffers. (and while its building local textures, and determineing scissor regions)
        /// Controls cannot be added or removed during this time because we may try and render a control that got added after the local buffers for that
        /// control got created.  Its still safe to change control properties though as its ok if we render older properties of a control in one frame, we
        /// will end up rendering the newer ones in the next frame.  We may end up rendering half older properties, and half newer but that should be ok for
        /// a control.  (its not ok, and is prevented for tiles)
        /// </summary>
        public Object CreatingBuffersLock = new Object();

        /// <summary>
        /// Level enum tells what we need to recreate next frame.  
        /// If we need to do something we also need to do everything below it:
        /// if we need to create Buffers, we also need to to scissors
        /// if we need to create local textures, we also need to recreate buffers, and to do scissors
        /// </summary>
        private enum RecreateLevel
        {
            None = 0,
            ScissorRegions = 1,
            Buffers = 2,
            LocalTextures = 3,
        }

        /// <summary>
        /// What needs to be recreated next frame
        /// </summary>
        private volatile RecreateLevel _recreateLevel = RecreateLevel.LocalTextures;
        private Object _recreateLevelLock = new Object();
                
        /// <summary>
        /// Create a new Window Drawer to draw the window specified.
        /// </summary>
        public WindowDrawer(TycoonWindow window, TextureSheet commonTextureSheet)
        {
            _window = window;
            _commonTextureSheet = commonTextureSheet;
            _mainPanelDrawer = new PanelDrawer(_window, null);
        }


        /// <summary>
        /// Texture sheet for the icons and strings shared by all windows
        /// </summary>
        public TextureSheet CommonTextureSheet
        {
            get { return _commonTextureSheet; }
        }


        /// <summary>
        /// Build or Rebuild the Local Texture Sheet on the next frame.
        /// Note this will also cause the buffers to be recreated, and the scissor regions to be recalculated.
        /// </summary>
        public void ReBuildLocalTextureSheetNextFrame()
        {
            lock (_recreateLevelLock)
            {
                _recreateLevel = RecreateLevel.LocalTextures;
            }
        }

        /// <summary>
        /// Create or Recreate the buffers for drawing this window.
        /// Note this will also cause the scissor regions to be recalculated.
        /// </summary>
        public void ReCreateBuffersNextFrame()
        {
            lock (_recreateLevelLock)
            {
                //if we were going to recreate strings next frame dont lower it to recreate only buffers
                if (_recreateLevel < RecreateLevel.Buffers)
                {
                    _recreateLevel = RecreateLevel.Buffers;
                }
            }
        }

        /// <summary>
        /// Calculates or Recalculate scissor regions for the windows/panels
        /// </summary>
        public void ReCalculateScissorRegionsNextFrame()
        {
            lock (_recreateLevelLock)
            {
                //if we were going to recreate local textures, or buffers next frame dont lower it to recreate only scissor regions
                if (_recreateLevel < RecreateLevel.ScissorRegions)
                {
                    _recreateLevel = RecreateLevel.ScissorRegions;
                }
            }
        }


        /// <summary>
        /// Return if the local texture sheet has a texture with the name passed
        /// </summary>
        public bool HasLocalTexture(string textureName)
        {
            lock (CreatingBuffersLock)
            {
                if (_localTextureSheet == null) { return false; }
                return _localTextureSheet.HasTexture(textureName);
            }
        }



        /// <summary>
        /// Recreate what needs to be recreated for the next frame
        /// </summary>
        private void RecreateForFrame()
        {
            //dont allow cetain things while recreating buffers (see comment on lock)
            lock (CreatingBuffersLock)
            {

                //get the level we need to recreate and set the value back to none atomicly                
                RecreateLevel levelToRereate;
                lock(_recreateLevelLock)
                {
                    levelToRereate = _recreateLevel;
                    _recreateLevel = RecreateLevel.None;
                }
                
                //recreate as specified
                if (levelToRereate == RecreateLevel.LocalTextures)
                {
                    BuildLocalTextureSheet();
                }
                if (levelToRereate >= RecreateLevel.Buffers)
                {
                    _mainPanelDrawer.CreateBuffers(_commonTextureSheet, _localTextureSheet);
                }
                if (levelToRereate >= RecreateLevel.ScissorRegions)
                {
                    _mainPanelDrawer.RecalculateScissor();
                }
            }
        }

        /// <summary>
        /// Build or Rebuild the Local Texture Sheet.
        /// </summary>
        private void BuildLocalTextureSheet()
        {
            //delete the old local texture sheet
            if (_localTextureSheet != null)
            {
                _localTextureSheet.Delete();
            }

            //create a new local texture sheet
            TextureSheetBuilder sheetBuilder = new TextureSheetBuilder();
            BuildLocalTextureSheetResursive(sheetBuilder, _window);
            _localTextureSheet = sheetBuilder.CreateTextureSheet();            
        }
        
        /// <summary>
        /// Method to recursivly call AddLocalTextures on each Control in the window
        /// </summary>
        private void BuildLocalTextureSheetResursive(TextureSheetBuilder sheetBuilder, TycoonControl control)
        {
            control.AddLocalTextures(sheetBuilder);
            if (control is TycoonPanel)
            {
                foreach (TycoonControl childControl in ((TycoonPanel)control).Children)
                {
                    BuildLocalTextureSheetResursive(sheetBuilder, childControl);
                }
            }
        }

        
        /// <summary>
        /// Draw this window
        /// </summary>
        public void RenderWindow()
        {
            //dont try and render if the window is invisible
            if (_window.Visible == false)
            {
                return;
            }
                        
            //update anything that is needed for this frame
            RecreateForFrame();
            
            float transX = _window.Left * WindowSettings.PointsPerPixelX;
            float transY = -1 * _window.Top * WindowSettings.PointsPerPixelY;

            GL.PushMatrix();
            GL.Translate(transX, transY, 0);

            //switch everything so we are drawing colors
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.Zero);
            GL.Disable(EnableCap.Texture2D);

            //draw the window lines            
            _mainPanelDrawer.RenderLines();

            //switch everything back to drawing textures
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Texture2D);

            //draw shared textures
            GL.BindTexture(TextureTarget.Texture2D, _commonTextureSheet.TextureSheetId);
            _mainPanelDrawer.RenderSharedTextures();

            //draw the local textures
            GL.BindTexture(TextureTarget.Texture2D, _localTextureSheet.TextureSheetId);
            _mainPanelDrawer.RenderLocalTextures();
                        
            GL.PopMatrix();

            //perform the special rendering
            _mainPanelDrawer.RenderSpecial();
        }
        
        /// <summary>
        /// Frees all resoucres used by the window drawer
        /// </summary>
        public void Delete()
        {
            //delete the local texture sheet
            if (_localTextureSheet != null)
            {
                _localTextureSheet.Delete();
            }

            //NOTE dont delete _commonTextureSheet it is shared between all WindowDrawers

            //have the panel drawers delete their resources
            _mainPanelDrawer.Delete();
        }


    }
}
