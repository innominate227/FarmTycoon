
using System;
using System.Windows.Forms;

namespace TycoonGraphicsLib
{


	public abstract class TycoonControl
    {
        #region Events

        /// <summary>
        /// Event raised when the size of the control changes
        /// </summary>
        public event Action<TycoonControl> SizeChanged;

        /// <summary>
        /// Event raised when the visibility of the control changes
        /// </summary>
        public event Action<TycoonControl> VisibilityChanged;

        /// <summary>
        /// Event raised when the control is clicked
        /// </summary>
        public event Action<TycoonControl> Clicked;

        /// <summary>
        /// Event raised when the mouse is over the control
        /// </summary>
        public event Action<TycoonControl> MouseOver;

        #endregion

        #region GUI Properties

        /// <summary>
        /// Name of the control
        /// </summary>
        private string _name = "";

        /// <summary>
        /// tag
        /// </summary>
        private object _tag;

        /// <summary>
        /// Is the window visible
        /// </summary>
        private volatile bool _visible = true;

		/// <summary>
		/// Width of the control
		/// </summary>
        private volatile int _width;

        /// <summary>
        /// Height of the control
        /// </summary>
        private volatile int _height;
		
        /// <summary>
        /// Top position of the control
        /// </summary>
        private volatile int _top;

        /// <summary>
        /// Left position of the control
        /// </summary>
        private volatile int _left;
        
        /// <summary>
        /// Anchor to the top of the window
        /// </summary>
        private bool _anchorTop = true;

        /// <summary>
        /// Anchor to the left of the window
        /// </summary>
        private bool _anchorLeft = true;

        /// <summary>
        /// Anchor to the right of the window
        /// </summary>
        private bool _anchorRight = false;

        /// <summary>
        /// Anchor to the bottom of the window
        /// </summary>
        private bool _anchorBottom = false;

        /// <summary>
        /// Tool tip for the control
        /// </summary>
        private string _tooltip = "";

        /// <summary>
        /// Time to wait until bringing up the tool tip
        /// </summary>
        private double _tooltipTime = 1.0;
        
        /// <summary>
        /// Name of the control
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Tag for the control, has no function.
        /// </summary>
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        /// <summary>
        /// Is the window visible
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value; RebufferWindowNextFrame();
                if (this is TycoonPanel == false)
                {
                    //dont need to rebuffer because if its a window or a panel because if its invisible we can just skip drawning the whole buffer
                    RebufferWindowNextFrame();
                }
                if (VisibilityChanged != null)
                {
                    VisibilityChanged(this);
                }
            }
        }

        /// <summary>
        /// Width of the control
        /// </summary>
        public virtual int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                //raise size changed event
                if (SizeChanged != null)
                {
                    SizeChanged(this);
                }             
            }
        }

        /// <summary>
        /// Height of the control
        /// </summary>
        public virtual int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                //raise size changed event
                if (SizeChanged != null)
                {
                    SizeChanged(this);
                }            
            }
        }

        /// <summary>
        /// Top position of the control
        /// </summary>
        public virtual int Top
        {
            get { return _top; }
            set
            {
                _top = value;
                if (this is TycoonWindow == false)
                {
                    RebufferWindowNextFrame();
                }
                else
                {
                    RecalcScissorsNextFrame();
                }
            }
        }

        /// <summary>
        /// Left position of the control
        /// </summary>
        public virtual int Left
        {
            get { return _left; }
            set
            {
                _left = value;
                if (this is TycoonWindow == false)
                {
                    RebufferWindowNextFrame();
                }
                else
                {
                    RecalcScissorsNextFrame();
                }
            }
        }

        /// <summary>
        /// Anchor to the top of the window
        /// </summary>
        public bool AnchorTop
        {
            get { return _anchorTop; }
            set { _anchorTop = value; }
        }

        /// <summary>
        /// Anchor to the left of the window
        /// </summary>
        public bool AnchorLeft
        {
            get { return _anchorLeft; }
            set { _anchorLeft = value; }
        }

        /// <summary>
        /// Anchor to the right of the window
        /// </summary>
        public bool AnchorRight
        {
            get { return _anchorRight; }
            set { _anchorRight = value; }
        }

        /// <summary>
        /// Anchor to the bottom of the window
        /// </summary>
        public bool AnchorBottom
        {
            get { return _anchorBottom; }
            set { _anchorBottom = value; }
        }
        
        /// <summary>
        /// Tool tip for the control
        /// </summary>
        public string Tooltip
        {
            get { return _tooltip; }
            set { _tooltip = value; }
        }
        
        /// <summary>
        /// Time to wait until bringing up the tool tip
        /// </summary>
        public double TooltipTime
        {
            get { return _tooltipTime; }
            set { _tooltipTime = value; }
        }

        #endregion

        #region Parent

        /// <summary>
        /// Parent of the control
        /// </summary>
        protected TycoonPanel _parent = null;
        
        /// <summary>
        /// Parent window of the control
        /// </summary>
        protected TycoonWindow _parentWindow = null;
        		
        /// <summary>
        /// The control that is the parent of this control
        /// </summary>
		public TycoonPanel Parent
		{
			get { return _parent; }
			internal set { _parent = value; }
		}

        /// <summary>
        /// The window that is the parent of this control
        /// </summary>
        public virtual TycoonWindow ParentWindow
        {
            get { return _parentWindow; }
            internal set { _parentWindow = value; }
        }

        /// <summary>
        /// Called after the control has been added to a parent control        
        /// </summary>
        public virtual void AfterAddedToParent()
        {
        }

        #endregion

        #region Position Helpers

        /// <summary>
        /// Method to get the controls relative position with in the window (in points)
        /// </summary>
        internal void GetPositionPoints(out float left, out float top, out float right, out float bottom)
        {
            int leftPixels, topPixels, rightPixels, bottomPixels;
            GetPosition(out leftPixels, out topPixels, out rightPixels, out bottomPixels);

            left = -1 + (leftPixels * WindowSettings.PointsPerPixelX);
            top = 1 - (topPixels * WindowSettings.PointsPerPixelY);

            right = -1 + (rightPixels * WindowSettings.PointsPerPixelX);
            bottom = 1 - (bottomPixels * WindowSettings.PointsPerPixelY);
        }
        
        /// <summary>
        /// Method to get the controls relative position with in the window
        /// </summary>
        public void GetPosition(out int left, out int top, out int right, out int bottom)
        {
            GetPosition(out left, out top);
            right = left + _width;
            bottom = top + _height;
        }
		
        /// <summary>
        /// Method to get the controls relative position with in the window
        /// </summary>
		public void GetPosition(out int left, out int top)
		{
			if (Parent == null)
			{
				left = 0;
				top = 0;	
			}
			else
			{
				int parentLeft, parentTop;
				Parent.GetPosition(out parentLeft, out parentTop);
				left = parentLeft + _left + Parent.ChildrenRegionOffsetLeft;
                top = parentTop + _top - Parent.ScrollPosition + Parent.ChildrenRegionOffsetTop;
			}
		}
        
        /// <summary>
        /// Method to get the controls absolution position
        /// </summary>
        public void GetPositionAbsolute(out int left, out int top, out int right, out int bottom)
        {
            GetPositionAbsolute(out left, out top);
            right = left + _width;
            bottom = top + _height;
        }

        /// <summary>
        /// Method to get the controls absolute position
        /// </summary>
        public void GetPositionAbsolute(out int left, out int top)
        {
            if (Parent == null)
            {
                left = _left;
                top = _top;
            }
            else
            {
                int parentLeft, parentTop;
                Parent.GetPositionAbsolute(out parentLeft, out parentTop);
                left = parentLeft + _left + Parent.ChildrenRegionOffsetLeft;
                top = parentTop + _top - Parent.ScrollPosition + Parent.ChildrenRegionOffsetTop;
            }
        }

        #endregion
        
        #region Rebuild Helpers
        
        /// <summary>
        /// Recalculates the scissor region for the window (if the window is currently being managed)
        /// </summary>
        internal void RecalcScissorsNextFrame()
        {
            if (_parentWindow != null)
            {
                if (_parentWindow.Drawer != null)
                {
                    _parentWindow.Drawer.ReCalculateScissorRegionsNextFrame(); 
                }
            }
        }

        /// <summary>
        /// Remakes the buffers for the window (if the window is currently being managed)
        /// </summary>
        internal void RebufferWindowNextFrame()
        {
            if (_parentWindow != null)
            {
                if (_parentWindow.Drawer != null)
                {
                    _parentWindow.Drawer.ReCreateBuffersNextFrame();
                }
            }
        }

        /// <summary>
        /// Remakes the local textures for the window, and rebuffers the window (if the window is currently being managed)
        /// </summary>
        internal void RebuildLocalTexturesSheetNextFrame()
        {
            if (_parentWindow != null)
            {
                if (_parentWindow.Drawer != null)
                {
                    _parentWindow.Drawer.ReBuildLocalTextureSheetNextFrame();
                    _parentWindow.Drawer.ReCreateBuffersNextFrame();
                }
            }
        }
        
        /// <summary>
        /// Call when a string changes. If needed it will cause ReCreateLocalBuffer to be called.
        /// If there is already a good texture for the string it will not need to call ReCreateLocalBuffers.
        /// </summary>
        internal void StringTextureChanged(TycoonString tycoonString)
        {
            if (_parentWindow != null)
            {
                if (_parentWindow.Drawer != null)
                {
                    lock (_parentWindow.Drawer.CreatingBuffersLock)
                    {
                        if (_parentWindow.Drawer.HasLocalTexture(tycoonString.TextureName) == false)
                        {
                            //we will need to rebuild the local texture sheet next frame in order to add the string texture
                            _parentWindow.Drawer.ReBuildLocalTextureSheetNextFrame();
                        }
                        else
                        {
                            //update the name to use when getting a texture from the texture sheet
                            tycoonString.SheetTextureName = tycoonString.TextureName;
                        }
                        _parentWindow.Drawer.ReCreateBuffersNextFrame();
                    }
                }
            }
        }
        
        #endregion

        #region Default Mouse Handlers

        /// <summary>
        /// Handel the location passed being hovered over on this control
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal virtual Cursor LocationHoveredOver(int x, int y)
        {
            if (MouseOver != null)
            {
                MouseOver(this);
            }

            return Cursors.Default;
        }

        /// <summary>
        /// Handel the location passed being clicked on this control
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal virtual void LocationClicked(int x, int y)
        {
            if (Clicked != null)
            {
                Clicked(this);
            }
        }
                
        #endregion

        #region Rendering

        /// <summary>
        /// Called to allow this control to add any local strings / icons it needs to add to the local texture sheet
        /// </summary>
		internal abstract void AddLocalTextures(TextureSheetBuilder textureSheetBuilder);
        
        /// <summary>
        /// Called to allow this control to add itself to the panel buffers.
        /// </summary>
        internal abstract void AddToBuffers(QuadColorBuffer linesBuffer, QuadTextureBuffer commonTexturesBuffer, QuadTextureBuffer localTexturesBuffer, TextureSheet commonTextures, TextureSheet localTextures);

        #endregion

    }
}
