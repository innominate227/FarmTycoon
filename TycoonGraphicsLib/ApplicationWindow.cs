using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace TycoonGraphicsLib
{


    internal class ApplicationWindow : GameWindow
    {
        /// <summary>
        /// Reference to the world we are drawing
        /// </summary>
        private World _world;  

        /// <summary>
        /// Reference to the current primary world view
        /// </summary>
        private WorldView _primaryView;        

        /// <summary>
        /// Reference to the window manager
        /// </summary>
        private WindowManager _windowManager;
        
       

        /// <summary>
        /// Set the world to show, and the primary view of that world, and the window manager that the main window will be rendering.
        /// </summary>
        public void SetRenderObjects(World world, WorldView primaryView, WindowManager windowManager)
        {
            _world = world;
            _primaryView = primaryView;
            _windowManager = windowManager;            
        }



        public ApplicationWindow(int width, int height)
            : base(width, height)
        {
            this.VSync = VSyncMode.On;
            WindowSettings.Height = height;
            WindowSettings.Width = width;     
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.ScissorTest);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Lighting);

            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.EdgeFlagArray);
            GL.DisableClientState(ArrayCap.FogCoordArray);
            GL.DisableClientState(ArrayCap.IndexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.SecondaryColorArray);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            WindowSettings.Height = Height;
            WindowSettings.Width = Width;
            
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
        }
        
                
        Stopwatch stopwatch = new Stopwatch();
        int renderCount = 0;
        double renderTot = 0;
        
        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            renderCount++;
            renderTot += this.RenderFrequency;

            if (renderCount >= 100)
            {
                stopwatch.Stop();
                long millisecs = stopwatch.ElapsedMilliseconds;

                stopwatch.Reset();
                stopwatch.Start();

                this.Title = (renderTot / renderCount).ToString() + "   MilliSecs Per Frame:" + (millisecs / (float)renderCount).ToString() + "  Layers: " + TycoonGraphics.DEBUG_CurrentMaxLayer.ToString() + "   Max Layers Ever:" + TycoonGraphics.DEBUG_AllTimeMaxLayer.ToString();
                renderCount = 0;
                renderTot = 0;
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                

            //process all added/deleted/changed tiles
            _world.TileManger.DelayedTileProcessList.Process();

            //update tile layers based on the changea
            _world.TileManger.TileLayerManager.DoDelayedLayerUpdates();

            //render the main view to the screen
            _primaryView.RenderView();                

            //render all windows to the screen
            _windowManager.DrawingManager.DrawWindows();

            SwapBuffers();            
        }

    }
}


