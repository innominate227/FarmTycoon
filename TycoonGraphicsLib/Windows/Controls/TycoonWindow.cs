
using System;
using System.Drawing;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;

namespace TycoonGraphicsLib
{

    public class TycoonWindow : TycoonPanel
    {
        #region Events

        /// <summary>
        /// Event raised when the close button on the window is clicked
        /// </summary>
        public event Action<TycoonWindow> CloseClicked;

        #endregion

        #region GUI Properties

        private static int TITLE_BAR_HEIGHT = 12;
        
        /// <summary>
        /// String for the title of the window
        /// </summary>
        private TycoonString _title = new TycoonString("Window");

        /// <summary>
        /// Color of the windows title bar
        /// </summary>
        private Safe<Color> _titleBarColor = new Safe<Color>();
        
        /// <summary>
        /// True if the windows title bar should be drawn
        /// </summary>
        private volatile bool _titlebar = true;

        /// <summary>
        /// The minimum width for the window
        /// </summary>
        private int _minimumWidth = 0;

        /// <summary>
        /// The minimum height for the window
        /// </summary>
        private int _minimumHeight = 0;

        /// <summary>
        /// The maximum width for the window
        /// </summary>
        private int _maximumWidth = int.MaxValue;

        /// <summary>
        /// The maximum height for the window
        /// </summary>
        private int _maximumHeight = int.MaxValue;

        /// <summary>
        /// Is the window resizable
        /// </summary>
        private bool _resizable = true;
                
        /// <summary>
        /// String for the title text of the window
        /// </summary>
		public string TitleText
		{
			get { return _title.Text; }
            set { _title.Text = value; StringTextureChanged(_title); }
		}

        /// <summary>
        /// Color for the title text of the window
        /// </summary>
        public Color TitleTextColor
        {
            get { return _title.Color; }
            set { _title.Color = value; StringTextureChanged(_title); }
        }

        /// <summary>
        /// Font for the title text of the window
        /// </summary>
        public Font TitleTextFont
        {
            get { return _title.Font; }
            set { _title.Font = value; StringTextureChanged(_title); }
        }

        /// <summary>
        /// The alignment of the label text
        /// </summary>
        public StringAlignment TitleTextAlignment
        {
            get { return _title.Alignment; }
            set { _title.Alignment = value; StringTextureChanged(_title); }
        }

        /// <summary>
        /// Color of the windows title bar
        /// </summary>
        public Color TitleBarColor
		{
            get { return _titleBarColor.Value; }
            set { _titleBarColor.Value = value; StringTextureChanged(_title); }	
		}
        
        /// <summary>
        /// True if the windows title bar should be drawn
        /// </summary>
        public bool TitleBar
        {
            get { return _titlebar; }
            set 
            { 
                _titlebar = value; 
                RebufferWindowNextFrame();

                if (_titlebar)
                {
                    _childrenRegionOffsetTop = 1 + TITLE_BAR_HEIGHT + 1; //1 for the border on each side of the title bar
                }
                else
                {
                    _childrenRegionOffsetTop = 1;
                }                
            }
        }

        /// <summary>
        /// The minimum width for the window
        /// </summary>
        public int MinimumWidth
        {
            get { return _minimumWidth; }
            set 
            { 
                _minimumWidth = value;
                if (Width < _minimumWidth)
                {
                    Width = _minimumWidth;
                }
            }
        }

        /// <summary>
        /// The minimum height for the window
        /// </summary>
        public int MinimumHeight
        {
            get { return _minimumHeight; }
            set
            {
                _minimumHeight = value;
                if (Height < _minimumHeight)
                {
                    Height = _minimumHeight;
                }
            }
        }

        /// <summary>
        /// The maximum width for the window
        /// </summary>
        public int MaximumWidth
        {
            get { return _maximumWidth; }
            set
            {
                _maximumWidth = value;
                if (Width > _maximumWidth)
                {
                    Width = _maximumWidth;
                }
            }
        }

        /// <summary>
        /// The maximum height for the window
        /// </summary>
        public int MaximumHeight
        {
            get { return _maximumHeight; }
            set
            {
                _maximumHeight = value;
                if (Height > _maximumHeight)
                {
                    Height = _maximumHeight;
                }
            }
        }

        /// <summary>
        /// The height of the window
        /// </summary>
        public override int Height
        {
            get { return base.Height; }
            set
            {
                if (value > _maximumHeight) { value = _maximumHeight; }
                if (value < _minimumHeight) { value = _minimumHeight; }
                base.Height = value;                
            }
        }

        /// <summary>
        /// The width of the window
        /// </summary>
        public override int Width
        {
            get { return base.Width; }
            set
            {
                if (value > _maximumWidth) { value = _maximumWidth; }
                if (value < _minimumWidth) { value = _minimumWidth; }
                base.Width = value;
            }
        }

        /// <summary>
        /// Is the window resizable
        /// </summary>
        public bool Resizable
        {
            get { return _resizable; }
            set { _resizable = value; }
        }

        #endregion
        
        /// <summary>
        /// Create a new tycoon window
        /// </summary>
        public TycoonWindow()
        {
            _parentWindow = this;
            _parent = null;
            _childrenRegionOffsetTop = 1 + TITLE_BAR_HEIGHT + 1; //1 for the border on each side of the title bar
            _childrenRegionOffsetBottom = 1;
            _childrenRegionOffsetLeft = 1;
            _childrenRegionOffsetRight = 1;
        }

        #region Rendering

        /// <summary>
        /// Called to allow this control to add any strings or icons it needs to add to the local texture sheet
        /// </summary>
        internal override void AddLocalTextures(TextureSheetBuilder textureSheetBuilder)
        {
            _title.Width = Width - 2;
            _title.Height = 12;
            textureSheetBuilder.AddString(_title);
        }
        
        /// <summary>
        /// Called to allow this control to add itself to the panel buffers.
        /// </summary>
        internal override void AddToBuffers(QuadColorBuffer linesBuffer, QuadTextureBuffer commonTexturesBuffer, QuadTextureBuffer localTexturesBuffer, TextureSheet commonTextures, TextureSheet localTextures)
        {   
            //draw the underlying panel 
            base.AddToBuffers(linesBuffer, commonTexturesBuffer, localTexturesBuffer, commonTextures, localTextures);

            //if the window does not have a title bar were done
            if (_titlebar == false)
            {
                return;
            }

            //get the position of the window
            float top, left, bottom, right;
            GetPositionPoints(out left, out top, out right, out bottom);

            //positions one pixel from the left,top,right,bottom
            float almostLeft = left + 1 * WindowSettings.PointsPerPixelX;
            float almostRight = right - 1 * WindowSettings.PointsPerPixelX;
            float almostTop = top - 1 * WindowSettings.PointsPerPixelY;
            float almostBottom = bottom + 1 * WindowSettings.PointsPerPixelY;
                        
            //add the window title bar
            int windowTitleSlot = linesBuffer.GetNextFreeSlot();
            if (windowTitleSlot != -1)
            {
                float titleBarBottom = top - 13 * WindowSettings.PointsPerPixelY;
                linesBuffer.SetSlotValues(windowTitleSlot, almostLeft, almostTop, almostRight, titleBarBottom, _titleBarColor.Value);
            }
            
            //add the extra border below the title bar
            int windowTitleBorderSlot = linesBuffer.GetNextFreeSlot();
            if (windowTitleBorderSlot != -1)
            {
                float titleBorderTop = top - 13 * WindowSettings.PointsPerPixelY;
                float titleBorderBottom = top - 14 * WindowSettings.PointsPerPixelY;
                linesBuffer.SetSlotValues(windowTitleBorderSlot, almostLeft, titleBorderTop, almostRight, titleBorderBottom, BorderColor);
            }
            
            //add the window X button
            int closeSlot = commonTexturesBuffer.GetNextFreeSlot();
            if (closeSlot != -1)
            {                
                float xLeft = right - 12 * WindowSettings.PointsPerPixelX;
                float xRight = right - 2 * WindowSettings.PointsPerPixelX;
                float xTop = top - 2 * WindowSettings.PointsPerPixelY;
                float xBottom = top - 12 * WindowSettings.PointsPerPixelY;                                
                Texture closeTexture = commonTextures.GetTexture("close");
                commonTexturesBuffer.SetSlotValues(closeSlot, xLeft, xTop, xRight, xBottom, closeTexture);
            }
            
            //add the window title text
            int windowTextSlot = localTexturesBuffer.GetNextFreeSlot();
            if (windowTextSlot != -1)
            {
                float titleTextBottom = top - 12 * WindowSettings.PointsPerPixelY; //NOTE: title bar text is one pixel higher than the title bar because it draws better that way
                Texture textTexture = localTextures.GetTexture(_title.SheetTextureName);
                localTexturesBuffer.SetSlotValues(windowTextSlot, almostLeft, top, almostRight, titleTextBottom, textTexture);
            }
        }

        #endregion

        #region Resizing / Movement

        private bool _draggingLeftBorder = false;
        private bool _draggingTopBorder = false;
        private bool _draggingRightBorder = false;
        private bool _draggingBottomBorder = false;
        private bool _draggingTitle = false;
        
        internal override Cursor LocationHoveredOver(int x, int y)
        {
            int nearFactor = WindowClickManager.NEAR_FACTOR;

            if (_resizable == false || _titlebar == false)
            {
                return Cursors.Default;
            }

            if (y <= nearFactor && x <= nearFactor)
            {
                return Cursors.SizeNWSE;
            }
            else if (y <= nearFactor && x >= Width - nearFactor)
            {
                return Cursors.SizeNESW;
            }
            else if (y >= Height - nearFactor && x <= nearFactor)
            {
                return Cursors.SizeNESW;
            }
            else if (y >= Height - nearFactor && x >= Width - nearFactor)
            {
                return Cursors.SizeNWSE;
            }
            else if (y <= nearFactor)
            {
                return Cursors.SizeNS;
            }
            else if (y >= Height - nearFactor)
            {
                return Cursors.SizeNS;
            }
            else if (x <= nearFactor)
            {
                return Cursors.SizeWE;
            }
            else if (x >= Width - nearFactor)
            {
                return Cursors.SizeWE;
            }
            return Cursors.Default;
        }

        internal override void LocationClicked(int x, int y)
        {
            base.LocationClicked(x, y);

            int nearFactor = WindowClickManager.NEAR_FACTOR;

            //what part or parts of the window was clicked
            _draggingLeftBorder = false;
            _draggingTopBorder = false;
            _draggingRightBorder = false;
            _draggingBottomBorder = false;
            _draggingTitle = false;


            //no resize, or moving clicks to check on panels without title bar
            if (_titlebar == false)
            {
                return;
            }

            //if the window is not resiable just check for title bar click
            if (_resizable == false)
            {
                if (y <= 14 && y >= 0)
                {
                    if (x >= Width - 12)
                    {
                        //clicked the close button
                        if (CloseClicked != null)
                        {
                            CloseClicked(this);
                        }
                    }
                    else
                    {
                        //clicked the title bar
                        _draggingTitle = true;
                    }
                }
            }
            else
            {

                if (y <= nearFactor && x <= nearFactor)
                {
                    _draggingLeftBorder = true;
                    _draggingTopBorder = true;
                }
                else if (y <= nearFactor && x >= Width - nearFactor)
                {
                    _draggingRightBorder = true;
                    _draggingTopBorder = true;
                }
                else if (y >= Height - nearFactor && x <= nearFactor)
                {
                    _draggingLeftBorder = true;
                    _draggingBottomBorder = true;
                }
                else if (y >= Height - nearFactor && x >= Width - nearFactor)
                {
                    _draggingRightBorder = true;
                    _draggingBottomBorder = true;
                }
                else if (y <= nearFactor)
                {
                    _draggingTopBorder = true;
                }
                else if (y >= Height - nearFactor)
                {
                    _draggingBottomBorder = true;
                }
                else if (x <= nearFactor)
                {
                    _draggingLeftBorder = true;
                }
                else if (x >= Width - nearFactor)
                {
                    _draggingRightBorder = true;
                }
                else if (y <= 14)
                {
                    if (x >= Width - 12)
                    {
                        //clicked the close button
                        if (CloseClicked != null)
                        {
                            CloseClicked(this);
                        }
                    }
                    else
                    {
                        //clicked the title bar
                        _draggingTitle = true;
                    }
                }
            }

            if (_draggingBottomBorder || _draggingLeftBorder || _draggingRightBorder || _draggingTopBorder || _draggingTitle)
            {
                //we need to know about all mouse movement so we can resize as the mouse moves
                _manager.ClickManager.MouseMoved += new WindowManagerMouseMovedHandler(ClickManager_MouseMoved);
                _manager.ClickManager.MouseUp += new WindowManagerMouseUpHandler(ClickManager_MouseUp);
            }
        }

        private void ClickManager_MouseUp()
        {
            //unregister events
            _manager.ClickManager.MouseMoved -= new WindowManagerMouseMovedHandler(ClickManager_MouseMoved);
            _manager.ClickManager.MouseUp -= new WindowManagerMouseUpHandler(ClickManager_MouseUp);
        }

        private void ClickManager_MouseMoved(int deltaX, int deltaY)
        {
            if (_draggingTitle)
            {
                Top += deltaY;
                Left += deltaX;
            }
            if (_draggingLeftBorder)
            {
                int widthBefore = Width;
                Width -= deltaX;                
                Left += (widthBefore - Width);
            }
            if (_draggingRightBorder)
            {
                Width += deltaX;
            }
            if (_draggingTopBorder)
            {
                int heightBefore = Height;
                Height -= deltaY;
                Top += (heightBefore - Height);
            }
            if (_draggingBottomBorder)
            {                
                Height += deltaY;
            }
        }

        #endregion

        #region Window Mangement

        /// <summary>
        /// The drawer that draws this window
        /// </summary>
        private WindowDrawer _drawer;

        /// <summary>
        /// The drawer that draws this window
        /// </summary>
        private WindowManager _manager;

        /// <summary>
        /// Reference to the world the window is in, this is needed by the world view panel
        /// </summary>
        private World _world;

        /// <summary>
        /// The drawer that draws this window
        /// </summary>
        internal WindowDrawer Drawer
        {
            get { return _drawer; }
            set { _drawer = value; }
        }

        /// <summary>
        /// The manager that managers this window
        /// </summary>
        internal WindowManager WindowManager
        {
            get { return _manager; }
            set { _manager = value; }
        }


        /// <summary>
        /// Reference to the world the window is in, this is needed by the world view panel
        /// </summary>
        internal World World
        {
            get { return _world; }
            set { _world = value; }
        }

        /// <summary>
        /// Move the window to the front
        /// </summary>
        public void ToFront()
        {
            if (_manager != null)
            {
                _manager.MoveWindowToFront(this);
            }
        }

        /// <summary>
        /// Close the window (raises the close clicked event)
        /// </summary>
        public void CloseWindow()
        {
            //clicked the close button
            if (CloseClicked != null)
            {
                CloseClicked(this);
            }
        }

        #endregion

    }
}
