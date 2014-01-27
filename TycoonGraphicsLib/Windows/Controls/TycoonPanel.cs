
using System;
using System.Drawing;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace TycoonGraphicsLib
{	

	public class TycoonPanel : TycoonControl
    {
        #region GUI Properties

        /// <summary>
        /// Primary color.
        /// </summary>
        private Safe<Color> _backColor = new Safe<Color>(Color.White);

        /// <summary>
        /// Color for the border
        /// </summary>
        private Safe<Color> _borderColor = new Safe<Color>();

        /// <summary>
        /// Color for the light part of the scroll bar shadow
        /// </summary>
        private Safe<Color> _scrollLightColor = new Safe<Color>();

        /// <summary>
        /// Color for the dark part of the scroll bar shadow
        /// </summary>
        private Safe<Color> _scrollDarkColor = new Safe<Color>();

        /// <summary>
        /// Should the panel be scrollable (a scroll bar will always be shown)
        /// </summary>
        private volatile bool _scrollable = false;

        /// <summary>
        /// Should the panel always show the scroll bar (even when theres nothing to scroll)
        /// </summary>
        private volatile bool _alwaysShowScroll = false;
        
        /// <summary>
        /// Should the panel have a border
        /// </summary>
        private volatile bool _border = true;

        /// <summary>
        /// The area of the scrollable panel being shown.  ( Controls with Top=ScrollPos will be seen at the very top of the window)
        /// </summary>
        private volatile int _scrollPosition = 0;

        /// <summary>
        /// The texture for the scroll up button
        /// </summary>
        private volatile string _scrollUpTexture = "arrowup";

        /// <summary>
        /// The texture for the scroll down button
        /// </summary>
        private volatile string _scrollDownTexture = "arrowdown";
        
        /// <summary>
        /// Primary color.
        /// </summary>
        public Color BackColor
		{
            get { return _backColor.Value; }
            set { _backColor.Value = value; RebufferWindowNextFrame(); }	
		}

        /// <summary>
        /// Color for the border
        /// </summary>
        public Color BorderColor
        {
            get { return _borderColor.Value; }
            set 
            {
                _borderColor.Value = value; 
                RebufferWindowNextFrame();
            }
        }
        
        /// <summary>
        /// Color for the light part of the scroll bar shadow
        /// </summary>
        public Color ScrollLightColor
        {
            get { return _scrollLightColor.Value; }
            set { _scrollLightColor.Value = value; RebufferWindowNextFrame(); }
        }

        /// <summary>
        /// Color for the dark part of the scroll bar shadow
        /// </summary>
        public Color ScrollDarkColor
        {
            get { return _scrollDarkColor.Value; }
            set { _scrollDarkColor.Value = value; RebufferWindowNextFrame(); }
        }

        /// <summary>
        /// Should the panel be scrollable (a scroll bar will always be shown)
        /// </summary>
        public bool Scrollable
        {
            get { return _scrollable; }
            set 
            {
                _scrollable = value;
                if (value)
                {
                    _childrenRegionOffsetRight = 12;
                }
                else
                {
                    if (_border)
                    {
                        _childrenRegionOffsetRight = 1;
                    }
                    else
                    {
                        _childrenRegionOffsetRight = 0;
                    }
                }
                RecalcScissorsNextFrame();
                RebufferWindowNextFrame();                
            }
        }
        
        /// <summary>
        /// Should the panel always show the scroll bar (even when theres nothing to scroll)
        /// </summary>
        public bool AlwaysShowScroll
        {
            get { return _alwaysShowScroll; }
            set { _alwaysShowScroll = value; }
        }

        /// <summary>
        /// Should the panel have a border
        /// </summary>
        public bool Border
        {
            get { return _border; }
            set 
            {
                //adjust children offset, this dose not take croll bars, or window title into account, but they should never be present on a border less window
                if (value == false)
                {
                    _childrenRegionOffsetRight = 0;
                    _childrenRegionOffsetLeft = 0;
                    _childrenRegionOffsetTop = 0;
                    _childrenRegionOffsetBottom = 0;
                }
                else
                {
                    _childrenRegionOffsetRight = 1;
                    _childrenRegionOffsetLeft = 1;
                    _childrenRegionOffsetTop = 1;
                    _childrenRegionOffsetBottom = 1;
                }

                _border = value;
                RecalcScissorsNextFrame();
                RebufferWindowNextFrame(); 
            }
        }

        /// <summary>
        /// The area of the scrollable panel being shown.  ( Controls with Top=ScrollPos will be seen at the very top of the window)
        /// </summary>
        public int ScrollPosition
        {
            get { return _scrollPosition; }
            set { _scrollPosition = value; RebufferWindowNextFrame(); }
        }

        /// <summary>
        /// The texture for the scroll up button
        /// </summary>
        public string ScrollUpTexture
        {
            get { return _scrollUpTexture; }
            set { _scrollUpTexture = value; }
        }

        /// <summary>
        /// The texture for the scroll down button
        /// </summary>
        public string ScrollDownTexture
        {
            get { return _scrollDownTexture; }
            set { _scrollDownTexture = value; }
        }

        #endregion

        #region Children

        /// <summary>
        /// The children of this control
        /// </summary>
        private List<TycoonControl> _children = new List<TycoonControl>();

        /// <summary>
        /// Get the list of the children of the panel.  Returns a copy of the list incase another thread is added or removing children.
        /// Use AddChild, and RemoveChild to add and remove children from the panel.
        /// </summary>
		public IList<TycoonControl> Children
		{
			get 
            {
                lock (_children) { return _children.ToArray(); }
            }
	
		}
        
        /// <summary>
        /// Add a child control to this panel
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(TycoonControl child)
        {
            if (child.Parent != null)
            {
                throw new Exception("Cannot add a control that already has already been added to another control");
            }
            if (child is TycoonWindow)
            {
                throw new Exception("Cannot add a window as a child control");
            }

            
            child.ParentWindow = this.ParentWindow;
            child.Parent = this;
            if (this.ParentWindow != null && this.ParentWindow.Drawer != null)
            {
                //if this panel is being drawn we need to get locks to make sure we dont 
                //add children to the window while buffers are being created for the window
                lock (this.ParentWindow.Drawer.CreatingBuffersLock)
                {
                    //also while being managed by TycoonGraphicsLib we need to make srue we dont add children while 
                    //someone is trying to get the list of controls children.  Order here is important.  Drawer locks in the same order.
                    lock (_children) 
                    {
                        _children.Add(child); 
                    }

                    //tell the control it has been added to a panel
                    child.AfterAddedToParent();
                }
            }
            else
            {
                _children.Add(child);
            }

        }
        
        /// <summary>
        /// Remove a child control from this panel
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(TycoonControl child)
        {
            if (this.ParentWindow != null && this.ParentWindow.Drawer != null)
            {
                //if this panel is being drawn we need to get locks to make sure we dont 
                //remove children from the window while buffers are being created for the window
                lock (this.ParentWindow.Drawer.CreatingBuffersLock)
                {
                    //also while being managed by TycoonGraphicsLib we need to make srue we dont remove children while 
                    //someone is trying to get the list of current children.  Order is important becayse Drawer locks in the same order
                    lock (_children)
                    {
                        _children.Remove(child);
                    }
                }

                //just need to rebuffer
                RebufferWindowNextFrame();
            }
            else
            {
                _children.Remove(child);
            }
        }

        /// <summary>
        /// Called after the control has been added to its parent
        /// </summary>
        public override void AfterAddedToParent()
        {
            //tell all children about being added
            foreach (TycoonControl child in _children)
            {
                child.AfterAddedToParent();
            }
        }


        /// <summary>
        /// Offset from the top of the panel where child controls are drawn
        /// </summary>
        internal int _childrenRegionOffsetTop = 1;

        /// <summary>
        /// Offset from the bottom of the panel where child controls are drawn
        /// </summary>
        internal int _childrenRegionOffsetBottom = 1;

        /// <summary>
        /// Offset from the left of the panel where child controls are drawn
        /// </summary>
        internal int _childrenRegionOffsetLeft = 1;

        /// <summary>
        /// Offset from the right of the panel where child controls are drawn
        /// </summary>
        internal int _childrenRegionOffsetRight = 1;



        /// <summary>
        /// Offset from the top of the panel where child controls are drawn
        /// </summary>
        internal int ChildrenRegionOffsetTop
        {
            get { return _childrenRegionOffsetTop; }
        }

        /// <summary>
        /// Offset from the bottom of the panel where child controls are drawn
        /// </summary>
        internal int ChildrenRegionOffsetBottom
        {
            get { return _childrenRegionOffsetBottom; }
        }

        /// <summary>
        /// Offset from the left of the panel where child controls are drawn
        /// </summary>
        internal int ChildrenRegionOffsetLeft
        {
            get { return _childrenRegionOffsetLeft; }
        }

        /// <summary>
        /// Offset from the right of the panel where child controls are drawn
        /// </summary>
        internal int ChildrenRegionOffsetRight
        {
            get { return _childrenRegionOffsetRight; }
        }



        /// <summary>
        /// The window that is the parent of this control
        /// </summary>
        public override TycoonWindow ParentWindow
        {
            get { return base.ParentWindow; }
            internal set 
            { 
                base.ParentWindow = value; 
                //set all children to have the same parent window
                foreach (TycoonControl child in _children)
                {
                    child.ParentWindow = value;
                }
            }
        }

        #endregion

        #region Add to buffers (including adding scroll bar to buffers)

        /// <summary>
        /// Called to allow this control to add any strings or icons it needs to add to the local texture sheet
        /// </summary>
        /// <param name="textureSheetBuilder"></param>
        internal override void AddLocalTextures(TextureSheetBuilder textureSheetBuilder)
        {
        }        

        /// <summary>
        /// Called to allow this control to add itself to the panel buffers.
        /// </summary>        
        internal override void AddToBuffers(QuadColorBuffer linesBuffer, QuadTextureBuffer commonTexturesBuffer, QuadTextureBuffer localTexturesBuffer, TextureSheet commonTextures, TextureSheet localTextures)
        {
            //NOTE: unlike other controls Windows and Panels still add to the buffer when invisible, because instead of not being in the buffer the entire Window/Panel will not be drawn

            //get the position of the panel
            float top, left, bottom, right;
            GetPositionPoints(out left, out top, out right, out bottom);

            //positions one pixel from the left,top,right,bottom
            float almostLeft = left + 1 * WindowSettings.PointsPerPixelX;
            float almostRight = right - 1 * WindowSettings.PointsPerPixelX;
            float almostTop = top - 1 * WindowSettings.PointsPerPixelY;
            float almostBottom = bottom + 1 * WindowSettings.PointsPerPixelY;

            //should the panel have a border
            if (_border)
            {
                //add the panels border
                int windowBorderSlot = linesBuffer.GetNextFreeSlot();
                linesBuffer.SetSlotValues(windowBorderSlot, left, top, right, bottom, _borderColor.Value);

                //add the panel
                int panelSlot = linesBuffer.GetNextFreeSlot();
                linesBuffer.SetSlotValues(panelSlot, almostLeft, almostTop, almostRight, almostBottom, _backColor.Value);
            }
            else
            {
                //add the panel bigger
                int panelSlot = linesBuffer.GetNextFreeSlot();
                linesBuffer.SetSlotValues(panelSlot, left, top, right, bottom, _backColor.Value);
            }
            
            //add the scroll bars items if they are to be shown
            if (_scrollable)
            {
                AddScrollBarToBuffers(linesBuffer, commonTexturesBuffer, localTexturesBuffer, commonTextures, localTextures);                
            }
        }
        
        /// <summary>
        /// Called to add the scroll bar to the panels buffer
        /// </summary>
        private void AddScrollBarToBuffers(QuadColorBuffer linesBuffer, QuadTextureBuffer iconsBuffer, QuadTextureBuffer stringsBuffer, TextureSheet iconTextures, TextureSheet stringTextures)
        {
            //get the position of the panel
            float top, left, bottom, right;
            GetPositionPoints(out left, out top, out right, out bottom);
            
            //calculate where the grip of the scroll bar should be rendered, and what the max Y location to scroll to is
            int maxScroll, scrollGripTopPixels, scrollGripBottomPixels; float scrollPixelsPerScrollbarPixel;
            ScrollCalculations(out maxScroll, out scrollGripTopPixels, out scrollGripBottomPixels, out scrollPixelsPerScrollbarPixel);

            //no scroll bar is needed so return
            if (maxScroll == 0 && _alwaysShowScroll == false)
            {
                //adjust child control region, if needed
                if ((_childrenRegionOffsetRight != 1 && _border) || (_childrenRegionOffsetRight != 0 && _border == false))
                {
                    _childrenRegionOffsetRight = 1;
                    if (_border == false)
                    {
                        _childrenRegionOffsetRight = 0;
                    }
                    RecalcScissorsNextFrame();
                }
                return;
            }
            else
            {
                //adjust child control region, if needed
                if (_childrenRegionOffsetRight != 12)
                {
                    _childrenRegionOffsetRight = 12;
                    RecalcScissorsNextFrame();
                }
            }

            //determe where the sroll bar should be rendered
            float scrollBarLeft = right - 12 * WindowSettings.PointsPerPixelX;
            float scrollBarRight = right - 1 * WindowSettings.PointsPerPixelX;
            float scrollBarTop = top - _childrenRegionOffsetTop * WindowSettings.PointsPerPixelY;
            float scrollBarBottom = bottom + _childrenRegionOffsetBottom * WindowSettings.PointsPerPixelY; 

            //determine where the scroll grip should be rendered (it shares left and right with the scroll bar)
            float scrollGripTop = top - scrollGripTopPixels * WindowSettings.PointsPerPixelY;
            float scrollGripBottom = top - scrollGripBottomPixels * WindowSettings.PointsPerPixelY;

            //determine locations one pixel in from the scroll grip
            float almostScrollGripLeft = scrollBarLeft + 1 * WindowSettings.PointsPerPixelX;
            float almostScrollGripRight = scrollBarRight - 1 * WindowSettings.PointsPerPixelX;
            float almostScrollGripTop = scrollGripTop - 1 * WindowSettings.PointsPerPixelY;
            float almostScrollGripBottom = scrollGripBottom + 1 * WindowSettings.PointsPerPixelY;

            //add the scroll bar
            int scrollLightSlot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(scrollLightSlot, scrollBarLeft, scrollBarTop, scrollBarRight, scrollBarBottom, _scrollLightColor.Value);
            
            //add the dark part of the scroll grip button
            int scrollButtonDark = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(scrollButtonDark, scrollBarLeft, scrollGripTop, scrollBarRight, scrollGripBottom, _scrollDarkColor.Value);
            
            //add the light part of the scroll grip button
            int scrollButtonLight = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(scrollButtonLight, scrollBarLeft, scrollGripTop, almostScrollGripRight, almostScrollGripBottom, _scrollLightColor.Value);
            

            //add the scroll grip button
            int scrollButton = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(scrollButton, almostScrollGripLeft, almostScrollGripTop, almostScrollGripRight, almostScrollGripBottom, _backColor.Value);
            

            //add the scroll up button
            int scrollUpSlot = iconsBuffer.GetNextFreeSlot();            
            float upButtonBottom = scrollBarTop - UP_AND_DOWN_BUTTON_SIZE * WindowSettings.PointsPerPixelY;
            Texture upTexture = iconTextures.GetTexture(_scrollUpTexture);
            iconsBuffer.SetSlotValues(scrollUpSlot, scrollBarLeft, scrollBarTop, scrollBarRight, upButtonBottom, upTexture);
            

            //add the scroll down button
            int scrollDownSlot = iconsBuffer.GetNextFreeSlot();            
            float downButtonTop = scrollBarBottom + UP_AND_DOWN_BUTTON_SIZE * WindowSettings.PointsPerPixelY;
            Texture downTexture = iconTextures.GetTexture(_scrollDownTexture);
            iconsBuffer.SetSlotValues(scrollDownSlot, scrollBarLeft, downButtonTop, scrollBarRight, scrollBarBottom, downTexture);
            
        }

        /// <summary>
        /// Allows the panel to preform a special render action, after all the normal rendering
        /// </summary>
        internal virtual void DoSpecialPanelRender()
        {
        }

        #endregion
        
        #region Anchor / Resizing

        /// <summary>
        /// Width of the control
        /// </summary>
        public override int Width
        {
            get { return base.Width; }
            set
            {
                int widthChange = value - Width;

                //change the width of child controls based on their anchors
                foreach (TycoonControl childControl in _children)
                {
                    if (childControl.AnchorLeft && childControl.AnchorRight)
                    {
                        childControl.Width += widthChange;
                    }
                    else if (childControl.AnchorLeft == false && childControl.AnchorRight)
                    {
                        childControl.Left += widthChange;
                    }
                }

                base.Width = value;
            }
        }



        /// <summary>
        /// Height of the control
        /// </summary>
        public override int Height
        {
            get { return base.Height; }
            set
            {
                int heightChange = value - Height;

                //change the height of child controls based on their anchors
                foreach (TycoonControl childControl in _children)
                {
                    if (childControl.AnchorTop && childControl.AnchorBottom)
                    {
                        childControl.Height += heightChange;
                    }
                    else if (childControl.AnchorTop == false && childControl.AnchorBottom)
                    {
                        childControl.Top += heightChange;
                    }
                }

                base.Height = value;
            }
        }

        #endregion

        #region Scrolling

        /// <summary>
        /// The total amout the Y value of the mouse position has changed since the scrolling began
        /// </summary>
        private int _totalDeltaY;

        /// <summary>
        /// The ScrollPosition at the time the scrolling started
        /// </summary>
        private int _scrollPosAtTimeOfClick;
        
        /// <summary>
        /// Handel the location passed being clicked
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal override void LocationClicked(int x, int y)
        {
            base.LocationClicked(x, y);

            if (_scrollable)
            {
                if (x > Width - 12)
                {
                    //calculate where the grip of the scroll bar is
                    int maxScroll, scrollGripTopPixels, scrollGripBottomPixels; float scrollPixelsPerScrollbarPixel;
                    ScrollCalculations(out maxScroll, out scrollGripTopPixels, out scrollGripBottomPixels, out scrollPixelsPerScrollbarPixel);

                    if (y > _childrenRegionOffsetTop && y < UP_AND_DOWN_BUTTON_SIZE + _childrenRegionOffsetTop)
                    {
                        //scroll down button clicked
                        ScrollPosition -= 2;
                    }
                    else if (y > Height - UP_AND_DOWN_BUTTON_SIZE - _childrenRegionOffsetBottom && y < Height - _childrenRegionOffsetBottom)
                    {
                        //scroll up button clicked
                        ScrollPosition += 2;
                    }
                    else if (y >= scrollGripTopPixels && y <= scrollGripBottomPixels)
                    {
                        //scroll grip was clicked
                        _totalDeltaY = 0;
                        _scrollPosAtTimeOfClick = ScrollPosition;
                        _parentWindow.WindowManager.ClickManager.MouseMoved += new WindowManagerMouseMovedHandler(ClickManager_MouseMoved);
                        _parentWindow.WindowManager.ClickManager.MouseUp += new WindowManagerMouseUpHandler(ClickManager_MouseUp);
                    }
                }
            }
        }

        private void ClickManager_MouseUp()
        {            
            _parentWindow.WindowManager.ClickManager.MouseMoved -= new WindowManagerMouseMovedHandler(ClickManager_MouseMoved);
            _parentWindow.WindowManager.ClickManager.MouseUp -= new WindowManagerMouseUpHandler(ClickManager_MouseUp);
        }

        private void ClickManager_MouseMoved(int deltaX, int deltaY)
        {
            //adjust the total delta Y
            _totalDeltaY += deltaY;
            
            //calculate the number of pixel we need to scroll the window per pixel of the scroll bar scrolled
            int maxScroll, scrollGripTopPixels, scrollGripBottomPixels; float scrollPixelsPerScrollbarPixel;
            ScrollCalculations(out maxScroll, out scrollGripTopPixels, out scrollGripBottomPixels, out scrollPixelsPerScrollbarPixel);

            //set the scroll position to the original position + the amout the mouse has moved * the amout the needs to be scrolled for each pixel
            ScrollPosition = _scrollPosAtTimeOfClick + (int)(_totalDeltaY * scrollPixelsPerScrollbarPixel);
        }

        #endregion
        
        #region Scroll bar location calculations
                
        //the height of the scroll up and down buttons on each end of the scroll bar
        private const int UP_AND_DOWN_BUTTON_SIZE = 10;


        /// <summary>
        /// Calculates values needed for drawing the scroll bar, and for determining how far to scroll when draggin the scroll bar
        /// </summary>
        /// <param name="scrollGripTop">location in pixels from the top of the control that the top of the scroll grip should be</param>
        /// <param name="scrollGripBottom">loction in pixels from the top of the control that the bottom of the scroll grip should be</param>
        /// <param name="scrollPixelsPerScrollbarPixel">How many pixels of window should be scrolled for each pixel of bar that is scrolled</param>
        private void ScrollCalculations(out int maxScroll, out int scrollGripTop, out int scrollGripBottom, out float scrollPixelsPerScrollbarPixel)
        {
            
            //determine the max y position in the window
            int maxY = 0;
            foreach (TycoonControl child in _children)
            {
                //only take visible children into account
                if (child.Visible)
                {
                    if (maxY < child.Top + child.Height)
                    {
                        maxY = child.Top + child.Height;
                    }
                }
            }

            //the amount of height the children get to be viewed in the window
            int myHeight = Height;
            if (myHeight < 0) { myHeight = 0; }
            int childrensHeight = myHeight - _childrenRegionOffsetBottom;

            //point that the top can be showing such that the bottom of the panel is showing the max y
            maxScroll = maxY - childrensHeight;
            if (maxScroll < 0)
            {
                maxScroll = 0;
            }

            //make sure they havent tried to scroll past what is allowable
            if (_scrollPosition > maxScroll)
            {
                _scrollPosition = maxScroll;
            }
            if (_scrollPosition < 0)
            {
                _scrollPosition = 0;
            }

            //scroll percent (how close are we to the bottom of the scroll area)
            float scrollPercent = (float)_scrollPosition / maxScroll;
            if (maxScroll == 0)
            {
                //no scroll is needed, so center the scroll drag area
                scrollPercent = 0.5f;
            }

            //percent of the panel we are seeing
            float percentShown = (float)childrensHeight / maxY;
            if (percentShown > 1)
            {
                percentShown = 1;
            }

            //maximum height of the scroll grip (at most it can be the area of the scroll bar is rendered minus the height of the two scroll up/down buttons)
            int scrollGripHeightMax = Height - _childrenRegionOffsetTop - _childrenRegionOffsetBottom - (UP_AND_DOWN_BUTTON_SIZE * 2);

            //size we should make the scroll button (enforce a minimum height)
            int scrollGripHeight = (int)(scrollGripHeightMax * percentShown);
            if (scrollGripHeight < 5)
            {
                scrollGripHeight = 5;
            }

            //determine the location for the top of the scroll grip
            scrollGripTop = (int)(_childrenRegionOffsetTop + UP_AND_DOWN_BUTTON_SIZE + scrollPercent * (scrollGripHeightMax - scrollGripHeight));

            //determine the location for the bottom of the scroll grip
            scrollGripBottom = scrollGripTop + scrollGripHeight;
            
            //maximum spot that can be scrolled to / number of pixels on the scroll bar can move = amout each pixel or movement should scroll the window
            scrollPixelsPerScrollbarPixel = maxScroll / (float)(scrollGripHeightMax - scrollGripHeight);
        }

        #endregion
    }
}
