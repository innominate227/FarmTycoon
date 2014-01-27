using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace TycoonGraphicsLib
{
    internal delegate void WindowManagerMouseMovedHandler(int deltaX, int deltaY);
    internal delegate void WindowManagerMouseUpHandler();

    /// <summary>
    /// Handles sending clicks to the correct window to process.  Also alows windows to subscribe to mouse move, and mouse up.
    /// </summary>
    internal class WindowClickManager
    {
        /// <summary>
        /// Event rasied when the mouse is moved anywhere with in the window, reports how far the mouse has moved sine the last moved, or the last mouse click
        /// </summary>
        public event WindowManagerMouseMovedHandler MouseMoved;

        /// <summary>
        /// Event rasied when the mouse is lifted anywhere in the window
        /// </summary>
        public event WindowManagerMouseUpHandler MouseUp;
        
        /// <summary>
        /// How close to the edge of a window is considered to be the window.
        /// This fudge factor allows for window grips to be extened out past the window a bit.
        /// </summary>
        public const int NEAR_FACTOR = 3;
        
        /// <summary>
        /// Window manager
        /// </summary>
        private WindowManager _windowManager;

        /// <summary>
        /// Control being hovered over
        /// </summary>
        private TycoonControl _hoveringOver;

        /// <summary>
        /// The textbox in focus (or null if no textbox is in focus)
        /// </summary>
        private TycoonTextbox _focusedTextbox = null;
        
        /// <summary>
        /// Time we have spent hovering in ms
        /// </summary>
        private double _timeHovering;

        /// <summary>
        /// Time we have spent since the line in the focused textbox was last blinked
        /// </summary>
        private double _lineBlinkTime;
                
        /// <summary>
        /// The last mouse loation during a dragging operation
        /// </summary>
        private int _lastX;

        /// <summary>
        /// The last mouse loation during a dragging operation
        /// </summary>
        private int _lastY;
        
        /// <summary>
        /// Create a new WindowClickManager
        /// </summary>
        /// <param name="windowManager"></param>
        public WindowClickManager(WindowManager windowManager)
        {
            _windowManager = windowManager;
        }
        
        /// <summary>
        /// Handel a mouse move event, informing the approprate control that the mouse was moved over it.
        /// Returns true if the mouse position is over a control, or false or not.
        /// </summary>
        public TycoonControl HandleMouseMove(int x, int y)
        {
            //determine how much the x, and y has changed since last time
            int xDelta = (x - _lastX);
            int yDelta = (y - _lastY);

            //set new last x, and y
            _lastX = x;
            _lastY = y;

            //tell any controls who wants to know about mouse movement
            if (MouseMoved != null)
            {
                MouseMoved(xDelta, yDelta);
            }

            //tell the contorl that was moved on
            int xOffset, yOffset;
            TycoonControl control = GetControlAtPosition(x, y, out xOffset, out yOffset);
            
            //update the contorl being hovered over if it changes and reset to time hovering 
            if (_hoveringOver != control)
            {
                _windowManager.ToolTipWindow = null;
                _hoveringOver = control;
                _timeHovering = 0;
            }

            //tell the control being convered over about it
            if (control != null)
            {                
                Cursor cursor = control.LocationHoveredOver(xOffset, yOffset);
                Cursor.Current = cursor;
                return control;
            }
            else
            {
                Cursor.Current = Cursors.Default;
                return null;
            }

        }
           
        /// <summary>
        /// Handle a mouse down evnet, informing the approprate control that the mouse was clicked on it
        /// Returns true if the mouse position is over a control, or false or not.
        /// </summary>
        public TycoonControl HandleMouseDown(int x, int y)
        {
            //set the last x, and y
            _lastX = x;
            _lastY = y;

            //find which control was clicked
            int xOffset, yOffset;
            TycoonControl control = GetControlAtPosition(x, y, out xOffset, out yOffset);
            if (control != null)
            {
                if (control.ParentWindow != _windowManager.DropboxWindow)
                {
                    //move the window the contorl is on to the foregorund (as long as it not the dropbox window which will always be in the front
                    _windowManager.MoveWindowToFront(control.ParentWindow);

                    //if the drop box window was not the one clicked it needs to be hidden
                    _windowManager.DropboxWindow = null;
                }

                //tell the control it was clicked
                control.LocationClicked(xOffset, yOffset);
                                
                if (control is TycoonTextbox)
                {
                    //if the control click was a textbox that is the focued textbox
                    if (_focusedTextbox != null) { _focusedTextbox.IsFocused = false; }
                    _lineBlinkTime = 0;
                    _focusedTextbox = (TycoonTextbox)control;
                }
                else
                {
                    //the control wasnt a textbox so no textbox is in focus now
                    if (_focusedTextbox != null) { _focusedTextbox.IsFocused = false; }
                    _focusedTextbox = null;
                }

                return control;
            }
            else
            {
                //no control was clicked so no textbox is in focus now
                if (_focusedTextbox != null) { _focusedTextbox.IsFocused = false; }
                _focusedTextbox = null;
                return null;
            }
        }

        /// <summary>
        /// Handle a mouse down evnet, informing the approprate control that the mouse was clicked on it
        /// Returns true if the mouse position is over a control, or false or not.
        /// </summary>
        public TycoonControl HandleMouseUp(int x, int y)
        {
            //tell any controls that want to know about the mouse up
            if (MouseUp != null)
            {
                MouseUp();
            }

            //determine if the mouse was raised above a control
            int xOffset, yOffset;
            TycoonControl control = GetControlAtPosition(x, y, out xOffset, out yOffset);
            if (control != null)
            {
                //Controls dont currently care about mouse up happening over them, but might want to add it
                return control;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Handle that an amount of time has passed since the last frame
        /// </summary>
        public void HandleTimePassed(double secondsPassed)
        {
            _timeHovering += secondsPassed;
            _lineBlinkTime += secondsPassed;

            if (_hoveringOver != null && _hoveringOver.Tooltip != "" && _timeHovering > _hoveringOver.TooltipTime)
            {
                //get the location for the tool tip
                int left, top;
                _hoveringOver.GetPositionAbsolute(out left, out top);
                left += _hoveringOver.Width / 2;
                top += _hoveringOver.Height / 2;

                //Create a tool tip window
                TycoonWindow toolTipWindow = new TycoonWindow();
                toolTipWindow.MinimumHeight = 0;
                toolTipWindow.MinimumWidth = 0;                
                toolTipWindow.Left = left;
                toolTipWindow.Top = top;
                toolTipWindow.BackColor = Color.White;
                toolTipWindow.BorderColor = Color.Transparent;
                toolTipWindow.Resizable = false;
                toolTipWindow.Scrollable = false;
                toolTipWindow.TitleBar = false;
                toolTipWindow.Visible = true;

                //Tool Tip Window Label
                TycoonLabel toolTipLabel = new TycoonLabel();
                toolTipLabel.Width = toolTipWindow.Width;
                toolTipLabel.Height = toolTipWindow.Height;
                toolTipLabel.Left = 0;
                toolTipLabel.Top = 0;
                toolTipLabel.BackColor = Color.White;
                toolTipLabel.Text = _hoveringOver.Tooltip;
                toolTipLabel.TextColor = Color.Black;
                toolTipLabel.TextAlignment = StringAlignment.Near;
                toolTipLabel.Visible = true;
                toolTipLabel.AnchorBottom = true;
                toolTipLabel.AnchorLeft = true;
                toolTipLabel.AnchorRight = true;
                toolTipLabel.AnchorTop = true;
                toolTipWindow.AddChild(toolTipLabel);
                                
                //measure what width, and height the tool tip needs to be
                Graphics grapics = Graphics.FromImage(new Bitmap(1, 1));
                SizeF stringSize = grapics.MeasureString(_hoveringOver.Tooltip, toolTipLabel.TextFont);
                toolTipWindow.Width = (int)Math.Ceiling(stringSize.Width);
                toolTipWindow.Height = (int)Math.Ceiling(stringSize.Height) - 2;

                //set the tool tip window
                _windowManager.ToolTipWindow = toolTipWindow;
            }

            //if the line blink time has elapsed and we have a textbox in focus flip the blink
            if (_focusedTextbox != null && _lineBlinkTime > 0.5)
            {
                _lineBlinkTime = 0;
                _focusedTextbox.IsFocused = !_focusedTextbox.IsFocused;
            }
        }

        /// <summary>
        /// Set true when the shift key is pressed
        /// </summary>
        private bool _shiftDown = false;

        /// <summary>
        /// Handle that a key was unpressed, return true if the window manager consumed the key up (a textbox was selected)
        /// </summary>
        public bool HandleKeyUp(Key key)
        {
            //if no textbox is focused the window manager does not consume the press
            if (_focusedTextbox == null)
            {
                return false;
            }

            //not that the shift key is no longer pressed
            if (key == Key.ShiftLeft || key == Key.ShiftRight || key == Key.RShift || key == Key.LShift)
            {
                _shiftDown = false;
            }

            return true;
        }
        
        /// <summary>
        /// Handle that a key was pressed, return true if the window manager consumed the key up (a textbox was selected)
        /// </summary>
        public bool HandleKeyDown(Key key)
        {
            //if no textbox is focused the window manager does not consume the press
            if (_focusedTextbox == null)
            {
                return false;
            }

            //remeber if shit is pressed
            if (key == Key.ShiftLeft || key == Key.ShiftRight || key == Key.RShift || key == Key.LShift)
            {
                _shiftDown = true;
            }


            if (key == Key.Delete)
            {
                //clear on delted
                _focusedTextbox.Text = "";
            }
            else if (key == Key.BackSpace)
            {
                //remove one character on backspace
                if (_focusedTextbox.Text.Length > 0)
                {
                    _focusedTextbox.Text = _focusedTextbox.Text.Substring(0, _focusedTextbox.Text.Length - 1);
                }
            }
            else if (_focusedTextbox.Text.Length < _focusedTextbox.MaxLenght)
            {
                if (key.ToString().Length == 1 && _focusedTextbox.NumbersOnly == false)
                {
                    //write the key pressed in upper or lower case based on if shift is down or not
                    if (_shiftDown)
                    {
                        _focusedTextbox.Text += key.ToString().ToUpper();
                    }
                    else
                    {
                        _focusedTextbox.Text += key.ToString().ToLower();
                    }
                }
                else if (key == Key.Space && _focusedTextbox.NumbersOnly == false)
                {
                    //space
                    _focusedTextbox.Text += " ";
                }
                else if (key.ToString().StartsWith("Number"))
                {
                    //a normal number
                    _focusedTextbox.Text += key.ToString().Replace("Number", "");
                }
                else if (key.ToString().StartsWith("Keypad") && key.ToString().Replace("Keypad", "").Length == 1)
                {
                    //a number from the key pad
                    _focusedTextbox.Text += key.ToString().Replace("Keypad", "");
                }
                else if (key == Key.Minus || key == Key.KeypadMinus)
                {
                    _focusedTextbox.Text += "-";
                }
                else if (key == Key.Period)
                {
                    _focusedTextbox.Text += ".";
                }
            }

            return true;
        }

        /// <summary>
        /// Get the window at the position or null if none is at that position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private TycoonWindow GetWindowAtPosition(int x, int y)
        {
            //first check thw dropbox window if there is one
            if (_windowManager.DropboxWindow != null)
            {
                TycoonWindow window = _windowManager.DropboxWindow;

                //check if the click was within thw window
                if (window.Top <= y && window.Top + window.Height >= y && window.Left <= x && window.Left + window.Width >= x)
                {
                    return window;
                }
            }

            //look at the windows in reverse order
            IList<TycoonWindow> windows = _windowManager.Windows;
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                TycoonWindow window = windows[i];

                int nearFactor = NEAR_FACTOR;
                if (window.Resizable == false)
                {
                    nearFactor = 0;
                }

                //check if the click was within a visible window
                if (window.Visible)
                {
                    if (window.Top - nearFactor <= y && window.Top + window.Height + nearFactor >= y && window.Left - nearFactor <= x && window.Left + window.Width + nearFactor >= x)
                    {
                        return window;
                    }
                }
            }

            return null;
        }
        
        /// <summary>
        /// Return if a control is at the position passed
        /// </summary>
        public bool IsControlAtPosition(int x, int y)
        {
            int xOffset, yOffset;
            return (GetControlAtPosition(x, y, out xOffset, out yOffset) != null);
        }

        /// <summary>
        /// Get the control located at a position on the screen
        /// </summary>
        private TycoonControl GetControlAtPosition(int x, int y, out int xOffset, out int yOffset)
        {
            //frist get the window at that position
            TycoonWindow window = GetWindowAtPosition(x, y);
            if (window == null)
            {
                xOffset = 0;
                yOffset = 0;
                return null;
            }

            TycoonControl control = GetControlAtPositionRecursive(x - window.Left, y - window.Top, window, out xOffset, out yOffset);

            //if the user has clicked a transparent window pretend like they didnt click anything
            if (control is TycoonWindow && (control as TycoonWindow).BackColor == Color.Transparent)
            {
                xOffset = 0;
                yOffset = 0;
                return null;
            }

            return control;
        }

        /// <summary>
        /// Recursive method used by GetControlAtPosition. x, and y in this method are such that 0, 0 is the top left of the control.
        /// </summary>
        private TycoonControl GetControlAtPositionRecursive(int x, int y, TycoonControl control, out int xOffset, out int yOffset)
        {
            
            //check if the position falls onto one of the child controls
            if (control is TycoonPanel)
            {
                TycoonPanel panel = (TycoonPanel)control;
                
                //see if the click was in the the child controls region of the panel
                if (x > panel.ChildrenRegionOffsetLeft && x < panel.Width - panel.ChildrenRegionOffsetRight &&
                    y > panel.ChildrenRegionOffsetTop && y < panel.Height - panel.ChildrenRegionOffsetBottom)
                {

                    //adjust x, and y based on the children control offsets
                    int xAdjust = x - panel.ChildrenRegionOffsetLeft;
                    int yAdjust = y - panel.ChildrenRegionOffsetTop;
                
                    //check if the click should go to a child control (check in rever order because end of the list is drawn last)
                    foreach (TycoonControl childControl in panel.Children.Reverse())
                    {
                        //make sure the control is visible
                        if (childControl.Visible)
                        {
                            //see if the click is within the control
                            if (childControl.Left <= xAdjust && childControl.Left + childControl.Width >= xAdjust && childControl.Top - panel.ScrollPosition <= yAdjust && childControl.Top + childControl.Height - panel.ScrollPosition >= yAdjust)
                            {
                                return GetControlAtPositionRecursive(xAdjust - childControl.Left, yAdjust - childControl.Top - panel.ScrollPosition, childControl, out xOffset, out yOffset);
                            }
                        }
                    }

                }                
            }

            //it wasnt a panel or wasnt one of the children of the panel
            xOffset = x;
            yOffset = y;
                        
            return control;            
        }


    }
}
