using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace TycoonGraphicsLib
{
    internal class PanelDrawer
    {
        /// <summary>
        /// Panel this drawer is drawing
        /// </summary>
        private TycoonPanel _panel;
        
        /// <summary>
        /// Buffer for drawing the lines of the controls in the panel
        /// </summary>
        private QuadColorBuffer _controlLinesBuffer = new QuadColorBuffer();
        
        /// <summary>
        /// Buffer for drawing the textures that are common for the controls in the panel
        /// </summary>
        private QuadTextureBuffer _controlCommonTexturesBuffer = new QuadTextureBuffer();

        /// <summary>
        /// Buffer for drawing the textures specific for just this panel
        /// </summary>
        private QuadTextureBuffer _controlLocalTexturesBuffer = new QuadTextureBuffer();
        
        /// <summary>
        /// Buffer for drawing the lines of the window
        /// </summary>
        private QuadColorBuffer _windowLinesBuffer = new QuadColorBuffer();

        /// <summary>
        /// Buffer for drawing the textures that are common to this window and other windows
        /// </summary>
        private QuadTextureBuffer _windowCommonTexturesBuffer = new QuadTextureBuffer();

        /// <summary>
        /// Buffer for drawing the textures specific for just this window
        /// </summary>
        private QuadTextureBuffer _windowLocalTexturesBuffer = new QuadTextureBuffer();

        /// <summary>
        /// X position of the scissor region for the controls
        /// </summary>
        private int _controlsScissorX;

        /// <summary>
        /// Y position of the scissor region for the controls
        /// </summary>
        private int _controlsScissorY;

        /// <summary>
        /// Width of the scissor region for the controls
        /// </summary>
        private int _controlsScissorWidth;

        /// <summary>
        /// Height of the scissor region for the controls
        /// </summary>
        private int _controlsScissorHeight;

        /// <summary>
        /// X position of the scissor region for the window
        /// </summary>
        private int _windowScissorX;

        /// <summary>
        /// Y position of the scissor region for the window
        /// </summary>
        private int _windowScissorY;

        /// <summary>
        /// Width of the scissor region for the window
        /// </summary>
        private int _windowScissorWidth;

        /// <summary>
        /// Height of the scissor region for the window
        /// </summary>
        private int _windowScissorHeight;

        /// <summary>
        /// Drawers for panels that are children of the panel (keyed on the child panels)
        /// </summary>
        private Dictionary<TycoonPanel, PanelDrawer> _childDrawers = new Dictionary<TycoonPanel, PanelDrawer>();

        /// <summary>
        /// The PanelDrawer that is the parent to this panel drawer, or null if it doe not have a parent.
        /// </summary>
        private PanelDrawer _parentDrawer;

        /// <summary>
        /// Create a new Panel drawer to draw the panel passed
        /// </summary>
        public PanelDrawer(TycoonPanel panel, PanelDrawer parent)
        {
            _panel = panel;
            _parentDrawer = parent;
        }



        /// <summary>
        /// Renders the windows lines
        /// </summary>
        public void RenderLines()
        {
            //dont render if invisible
            if (_panel.Visible == false)
            {
                return;
            }

            //restrict area being drawn in to the area of the panel
            GL.Scissor(_windowScissorX, _windowScissorY, _windowScissorWidth, _windowScissorHeight);

            //render the lines
            _windowLinesBuffer.Render();

            //restrict area being drawn in to the controls area of the panel
            GL.Scissor(_controlsScissorX, _controlsScissorY, _controlsScissorWidth, _controlsScissorHeight);

            //render the lines for the controls
            _controlLinesBuffer.Render();

            //render the panels children
            foreach (PanelDrawer child in _childDrawers.Values)
            {
                child.RenderLines();
            }
        }

        /// <summary>
        /// Renders the shared textures for this panel and its children
        /// </summary>
        public void RenderSharedTextures()
        {
            //dont render if invisible
            if (_panel.Visible == false)
            {
                return;
            }

            //restrict area being drawn in to the area of the panel
            GL.Scissor(_windowScissorX, _windowScissorY, _windowScissorWidth, _windowScissorHeight);

            //render the common textures for the panel
            _windowCommonTexturesBuffer.Render();

            //restrict area being drawn in to the controls area of the panel
            GL.Scissor(_controlsScissorX, _controlsScissorY, _controlsScissorWidth, _controlsScissorHeight);

            //render the common textures for the controls
            _controlCommonTexturesBuffer.Render();

            //render the panels children
            foreach (PanelDrawer child in _childDrawers.Values)
            {
                child.RenderSharedTextures();
            }
        }

        /// <summary>
        /// Renders the local textures for this panel and its children
        /// </summary>
        public void RenderLocalTextures()
        {
            //dont render if invisible
            if (_panel.Visible == false)
            {
                return;
            }

            //restrict area being drawn in to the area of the panel 
            GL.Scissor(_windowScissorX, _windowScissorY, _windowScissorWidth, _windowScissorHeight);

            //render the local textures for the window
            _windowLocalTexturesBuffer.Render();
            
            //restrict area being drawn in to the controls area of the panel 
            GL.Scissor(_controlsScissorX, _controlsScissorY, _controlsScissorWidth, _controlsScissorHeight);

            //render the local textures for the controls
            _controlLocalTexturesBuffer.Render();

            //render the panels children
            foreach (PanelDrawer child in _childDrawers.Values)
            {
                child.RenderLocalTextures();
            }
        }


        /// <summary>
        /// Allows panels to preform their special rendering
        /// </summary>
        public void RenderSpecial()
        {
            //dont render if invisible
            if (_panel.Visible == false)
            {
                return;
            }

            _panel.DoSpecialPanelRender();
            
            //render the panels children
            foreach (PanelDrawer child in _childDrawers.Values)
            {
                child.RenderSpecial();
            }
        }

        /// <summary>
        /// Recalculates the scissor region for the panel
        /// </summary>
        public void RecalculateScissor()
        {
            //calculate the scissor region for the window
            int left, top, right, bottom;
            _panel.GetPositionAbsolute(out left, out top, out right, out bottom);
            _windowScissorX = left;
            _windowScissorY = WindowSettings.Height - bottom;
            _windowScissorWidth = right - left;
            _windowScissorHeight = bottom - top;

            //make sure we dont have greater bounds than our patents control region
            if (_parentDrawer != null)
            {
                if (_windowScissorX < _parentDrawer._controlsScissorX)
                {
                    _windowScissorX = _parentDrawer._controlsScissorX;
                }
                if (_windowScissorY < _parentDrawer._controlsScissorY)
                {
                    _windowScissorY = _parentDrawer._controlsScissorY;
                }
                if (_windowScissorX + _windowScissorWidth > _parentDrawer._controlsScissorX + _parentDrawer._controlsScissorWidth)
                {
                    _windowScissorWidth = _parentDrawer._controlsScissorX + _parentDrawer._controlsScissorWidth - _windowScissorX;
                }
                if (_windowScissorY + _windowScissorHeight > _parentDrawer._controlsScissorY + _parentDrawer._controlsScissorHeight)
                {
                    _windowScissorHeight = _parentDrawer._controlsScissorY + _parentDrawer._controlsScissorHeight - _windowScissorY;
                }
            }
                                    

            //calculate the scissor region for the windows children
            _controlsScissorX = _windowScissorX + _panel.ChildrenRegionOffsetLeft;
            _controlsScissorY = _windowScissorY + _panel.ChildrenRegionOffsetBottom;
            _controlsScissorWidth = _windowScissorWidth - _panel.ChildrenRegionOffsetLeft - _panel.ChildrenRegionOffsetRight;
            _controlsScissorHeight = _windowScissorHeight - _panel.ChildrenRegionOffsetTop - _panel.ChildrenRegionOffsetBottom;
            
            //also have the children panel drawers recalculate the scissor
            foreach (PanelDrawer childDrawer in _childDrawers.Values)
            {
                childDrawer.RecalculateScissor();
            }

        }

        /// <summary>
        /// Creates or Recreates the buffers for this panel, and the panels child panels
        /// </summary>
        public void CreateBuffers(TextureSheet commonTextures, TextureSheet localTextures)
        {
            //clear all buffers
            _windowLinesBuffer.Clear();
            _windowCommonTexturesBuffer.Clear();
            _windowLocalTexturesBuffer.Clear();
            _controlLinesBuffer.Clear();
            _controlCommonTexturesBuffer.Clear();
            _controlLocalTexturesBuffer.Clear();
            
            //have the panel add what it needs to draw to its buffer
            _panel.AddToBuffers(_windowLinesBuffer, _windowCommonTexturesBuffer, _windowLocalTexturesBuffer, commonTextures, localTextures);

            //dictionary used to check that all the panels that were children of this panel are still children
            Dictionary<TycoonPanel, bool> panelStillExsits = new Dictionary<TycoonPanel, bool>();
            foreach (TycoonPanel childPanel in _childDrawers.Keys)
            {
                panelStillExsits.Add(childPanel, false);
            }

            //look at each child control of the panel
            foreach (TycoonControl child in _panel.Children)
            {
                if (child is TycoonPanel == false)
                {
                    //children that are not panels themselves are added as  well
                    child.AddToBuffers(_controlLinesBuffer, _controlCommonTexturesBuffer, _controlLocalTexturesBuffer, commonTextures, localTextures);
                }
                else
                {
                    //children that are panel have a PanelDrawer created for them (if one does not exsits)
                    TycoonPanel childPanel = (TycoonPanel)child;
                    if (_childDrawers.ContainsKey(childPanel) == false)
                    {
                        //need to create a drawer for the new panel
                        _childDrawers.Add(childPanel, new PanelDrawer(childPanel, this));
                    }
                    else
                    {
                        panelStillExsits[childPanel] = true;
                    }
                    
                    //have the child panel recreate its buffers
                    _childDrawers[childPanel].CreateBuffers(commonTextures, localTextures);
                }
            }

            //destroy and panel drawers for panels that dont exsit any more
            foreach (TycoonPanel childPanel in panelStillExsits.Keys)
            {
                if (panelStillExsits[childPanel] == false)
                {
                    _childDrawers[childPanel] .Delete();
                    _childDrawers.Remove(childPanel);
                }
            }
            
        }


        /// <summary>
        /// Frees all resoucres used by the panel drawer, and any child panel drawers
        /// </summary>
        public void Delete()
        {
            //delete all buffers
            _windowLinesBuffer.Delete();
            _windowCommonTexturesBuffer.Delete();
            _windowLocalTexturesBuffer.Delete();
            _controlLinesBuffer.Delete();
            _controlCommonTexturesBuffer.Delete();
            _controlLocalTexturesBuffer.Delete();

            //have the children delete their buffers
            foreach (PanelDrawer childPanelDrawer in _childDrawers.Values)
            {
                childPanelDrawer.Delete();
            }
        }




    }
}
