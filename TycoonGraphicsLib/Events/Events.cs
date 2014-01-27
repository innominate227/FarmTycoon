using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TycoonGraphicsLib
{
    /// <summary>
    /// Delegate for handeling mouse events
    /// </summary>
    /// <param name="x">The x position of the mouse</param>
    /// <param name="y">The y position of the mouse</param>
    /// <param name="tile">The tile that was under the mouse, or null if no tile was under the mouse (the mouse is over a window, or an area where no tile was drawn)</param>
    /// <param name="clickLocationX">A number between 0 and 1 where 0 means the mouse is over the very left of the tile, and 1 means the very right</param>
    /// <param name="clickLocationY">A number between 0 and 1 where 0 means the mouse is over the very top of the tile, and 1 means the very bottom</param>
    public delegate void MouseEventHandler(ClickInfo clickInfo);

    /// <summary>
    /// Delegate for handeling mouse wheel event
    /// </summary>
    /// <param name="delta">The amount the mouse wheel moved</param>
    public delegate void MouseWheelEventHandler(float delta);

    /// <summary>
    /// Delegate for handeling keyboard events
    /// </summary>
    /// <param name="key">The key that was pressed</param>
    public delegate void KeyboardEventHandler(Key key);
    

    
    /// <summary>
    /// Handels rasing the main events raised by tycoon graphics.
    /// Events are raised on a seperate thread determine by the user of tycoonGraphicsLib by calling the RaiseEvents method form that thread.
    /// </summary>
    public class Events
    {
        #region Events
        
        /// <summary>
        /// Event raised when the mouse goes down in the window.
        /// </summary>
        public event MouseEventHandler MouseDown;

        /// <summary>
        /// Event raised when the mouse goes up in the window.
        /// </summary>
        public event MouseEventHandler MouseUp;

        /// <summary>
        /// Event raised when the mouse moves around in the window.
        /// If there are multiple move event queued up this only gets raised for the latest event.
        /// </summary>
        public event MouseEventHandler MouseMoved;

        /// <summary>
        /// Event raised when the mouse moves around in the window.
        /// If there are multiple move event queued up this only gets raised for all events.
        /// </summary>
        public event MouseEventHandler MouseMoved_All;

        /// <summary>
        /// Event raised when the mouse wheel moves
        /// </summary>
        public event MouseWheelEventHandler MouseWheel;
        
        /// <summary>
        /// Event raised when a key on the keyboard is pressed
        /// </summary>
        public event KeyboardEventHandler KeyDown;

        /// <summary>
        /// Event raised when a key on the keyboard is raised
        /// </summary>
        public event KeyboardEventHandler KeyUp;
                
        /// <summary>
        /// Window size changed
        /// </summary>
        public event Action WindowSizeChanged;

        #endregion

        /// <summary>
        /// List of queued events
        /// </summary>
        private List<QueuedEvent> _queuedEvents = new List<QueuedEvent>();
        
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
        private ApplicationWindow _window;
                

        /// <summary>
        /// Create a new Events object
        /// </summary>
        internal Events(ApplicationWindow window)
        {
            _window = window;
                        
            //handel window events
            _window.Mouse.ButtonDown += new EventHandler<OpenTK.Input.MouseButtonEventArgs>(Mouse_ButtonDown);
            _window.Mouse.ButtonUp += new EventHandler<OpenTK.Input.MouseButtonEventArgs>(Mouse_ButtonUp);
            _window.Mouse.WheelChanged += new EventHandler<OpenTK.Input.MouseWheelEventArgs>(Mouse_WheelChanged);

            _window.Mouse.Move += new EventHandler<OpenTK.Input.MouseMoveEventArgs>(Mouse_Move);
            _window.Keyboard.KeyDown += new EventHandler<OpenTK.Input.KeyboardKeyEventArgs>(Keyboard_KeyDown);
            _window.Keyboard.KeyUp += new EventHandler<OpenTK.Input.KeyboardKeyEventArgs>(Keyboard_KeyUp);

            //handel window size changed event
            WindowSettings.SettingsChanged += new Action(WindowSettings_SettingsChanged);            
        }


        /// <summary>
        /// Give the event manager a reference to the window manager so it can determine what window is being clicked
        /// </summary>
        internal void SetWindowManager(WindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        /// <summary>
        /// Give the event manager a reference to the primary view so it can determine what tile the mouse is being moved over
        /// </summary>
        internal void SetPrimaryView(WorldView primaryView)
        {
            _primaryView = primaryView;
        }


        /// <summary>
        /// Raises the queued events on the thread called from. 
        /// This must be called periodicly from the game thread so that control click handlers, and MouseEvent handlers can be raised.
        /// </summary>
        public void RaiseQueuedEvents(double secondsPassed)
        {
            List<QueuedEvent> queuedEventsCopy;
            lock (_queuedEvents)
            {
                //create a copy of the queued events list so we can process the queued events without leaving the list locked

                //Big long story about a deadlock that happened once
                //When window event queue used to be locked for the full processing of the evnets I got a deadlock once where I was locked in the gui thread, trying to add
                //another event to the queue, which makes since.  The game thread said it was locked on _recreateLevelLock in the window drawer, which dose not make since
                //we cant be drawing window on the gui thread and at the same time trying to add an event on the gui thread (gui thread stack did not appear to be anywhere 
                //where the _recreateLevelLock was lock?).  The event was WindowResized, the game thread was trying to set the top of a window (because of a previous
                //WindowResized event).  It all happened when making the window full screen.
                //Still not sure what happened but either wat making a copy before handeling should prevent any possible future dealocks involving the _queuedEvents list
                queuedEventsCopy = _queuedEvents.ToList();

                //clear evnet queue
                _queuedEvents.Clear();
            }

            //find the last mouse moved event in the list
            QueuedEvent lastMouseMovedEvent = null;
            for (int i = queuedEventsCopy.Count - 1; i >= 0; i--)
            {
                if (queuedEventsCopy[i].Type == EventType.MouseMoved)
                {
                    lastMouseMovedEvent = queuedEventsCopy[i];
                    break;
                }
            }


            //tell window manager about time passed
            _windowManager.ClickManager.HandleTimePassed(secondsPassed);

            //raise all the queued events
            foreach (QueuedEvent queuedEvent in queuedEventsCopy)
            {
                if (queuedEvent.Type == EventType.MouseMoved)
                {
                    bool isLatest = (queuedEvent == lastMouseMovedEvent);
                    RaiseMouseMove(queuedEvent, isLatest);
                }
                else if (queuedEvent.Type == EventType.MouseDown)
                {
                    RaiseMouseDown(queuedEvent);
                }
                else if (queuedEvent.Type == EventType.MouseUp)
                {
                    RaiseMouseUp(queuedEvent);
                }
                else if (queuedEvent.Type == EventType.MouseScroll)
                {
                    RaiseMouseWheel(queuedEvent);
                }
                else if (queuedEvent.Type == EventType.KeyDown)
                {
                    RaiseKeyDown(queuedEvent);
                }
                else if (queuedEvent.Type == EventType.KeyUp)
                {
                    RaiseKeyUp(queuedEvent);
                }
                else if (queuedEvent.Type == EventType.WindowSizeChanged)
                {
                    RaiseWindowSizeChanged(queuedEvent);
                }
                else
                {
                    //need to handel all evnet types
                    Debug.Assert(false);
                }
                
            }
        }


               

        #region Main Window Event Handeling

        //These methods handel an event on the GUI thread and put it into the event queue
        
        private void Mouse_Move(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            //determine if we clicked a control
            bool clickedControl = _windowManager.ClickManager.IsControlAtPosition(e.X, e.Y);
                                    
            //if no window control was below the mouse, and we are actually handeling mouse moved then determin what tiles
            //were moved over.  Basicly we only want to do this if we are pretty sure were going to use it.
            TileClickInfo[] movedOverTiles = new TileClickInfo[0];
            if (clickedControl == false && _primaryView != null && MouseMoved != null)
            {
                //determine what tiles the mouse moved over                    
                movedOverTiles = _primaryView.GetTilesAtPosition(e.X, e.Y).ToArray();                    
            }

            //create queued evnet and add it the queue
            QueuedEvent queuedEvent = new QueuedEvent();
            queuedEvent.Type = EventType.MouseMoved;
            queuedEvent.Arg1 = e;
            queuedEvent.Arg2 = movedOverTiles;
            lock (_queuedEvents)
            {
                _queuedEvents.Add(queuedEvent);
            }                        
        }
                
        private void Mouse_ButtonDown(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            //raise mouse move events for the button location before raising the button event
            Mouse_Move(sender, new OpenTK.Input.MouseMoveEventArgs(e.X, e.Y, 0, 0));

            //determine if we clicked a control
            bool clickedControl = _windowManager.ClickManager.IsControlAtPosition(e.X, e.Y);

            //if no window control was below the mouse, and we are actually handeling mouse moved then determin what tiles
            //were moved over.  Basicly we only want to do this if we are pretty sure were going to use it.
            TileClickInfo[] movedOverTiles = new TileClickInfo[0];
            if (clickedControl == false && _primaryView != null && MouseDown != null)
            {
                //determine what tiles the mouse moved over                    
                movedOverTiles = _primaryView.GetTilesAtPosition(e.X, e.Y).ToArray();
            }

            //create queued evnet and add it the queue
            QueuedEvent queuedEvent = new QueuedEvent();
            queuedEvent.Type = EventType.MouseDown;
            queuedEvent.Arg1 = e;
            queuedEvent.Arg2 = movedOverTiles;
            lock (_queuedEvents)
            {
                _queuedEvents.Add(queuedEvent);
            } 
        }

        private void Mouse_ButtonUp(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            //raise mouse move events for the button location before raising the button event
            Mouse_Move(sender, new OpenTK.Input.MouseMoveEventArgs(e.X, e.Y, 0, 0));

            //determine if we clicked a control
            bool clickedControl = _windowManager.ClickManager.IsControlAtPosition(e.X, e.Y);

            //if no window control was below the mouse, and we are actually handeling mouse moved then determin what tiles
            //were moved over.  Basicly we only want to do this if we are pretty sure were going to use it.
            TileClickInfo[] movedOverTiles = new TileClickInfo[0];
            if (clickedControl == false && _primaryView != null && MouseUp != null)
            {
                //determine what tiles the mouse moved over                    
                movedOverTiles = _primaryView.GetTilesAtPosition(e.X, e.Y).ToArray();
            }

            //create queued evnet and add it the queue
            QueuedEvent queuedEvent = new QueuedEvent();
            queuedEvent.Type = EventType.MouseUp;
            queuedEvent.Arg1 = e;
            queuedEvent.Arg2 = movedOverTiles;
            lock (_queuedEvents)
            {
                _queuedEvents.Add(queuedEvent);
            } 
        }


        private void Mouse_WheelChanged(object sender, OpenTK.Input.MouseWheelEventArgs e)
        {
            //create queued evnet and add it the queue
            QueuedEvent queuedEvent = new QueuedEvent();
            queuedEvent.Type = EventType.MouseScroll;
            queuedEvent.Arg1 = e;
            lock (_queuedEvents)
            {
                _queuedEvents.Add(queuedEvent);
            } 
        }


        private void Keyboard_KeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            //create queued evnet and add it the queue
            QueuedEvent queuedEvent = new QueuedEvent();
            queuedEvent.Type = EventType.KeyUp;
            queuedEvent.Arg1 = e;
            lock (_queuedEvents)
            {
                _queuedEvents.Add(queuedEvent);
            } 
        }

        private void Keyboard_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            //create queued evnet and add it the queue
            QueuedEvent queuedEvent = new QueuedEvent();
            queuedEvent.Type = EventType.KeyDown;
            queuedEvent.Arg1 = e;
            lock (_queuedEvents)
            {
                _queuedEvents.Add(queuedEvent);
            } 
        }
        

        private void WindowSettings_SettingsChanged()
        {
            //create queued evnet and add it the queue
            QueuedEvent queuedEvent = new QueuedEvent();
            queuedEvent.Type = EventType.WindowSizeChanged;            
            lock (_queuedEvents)
            {
                _queuedEvents.Add(queuedEvent);
            }
        }

        #endregion



        #region Raise Events

        //these methods raise and event that was taken out of the event queue
        
        private void RaiseMouseMove(QueuedEvent queuedEvent, bool isLatest)
        {
            //get the args that were queued
            OpenTK.Input.MouseMoveEventArgs e = (OpenTK.Input.MouseMoveEventArgs)queuedEvent.Arg1;
            TileClickInfo[] movedOverTiles = (TileClickInfo[])queuedEvent.Arg2;

            //tell the window manager about the mouse movement
            TycoonControl controlOver = _windowManager.ClickManager.HandleMouseMove(e.X, e.Y);

            //raise the mouse moved event if handeling
            if (isLatest && MouseMoved != null)
            {   
                MouseMoved(new ClickInfo(e.X, e.Y, MouseButton.None, movedOverTiles, controlOver));                
            }

            //raise the mouse moved all event if handeling
            if (MouseMoved_All != null)
            {
                MouseMoved_All(new ClickInfo(e.X, e.Y, MouseButton.None, movedOverTiles, controlOver));
            }
        }

        private void RaiseMouseDown(QueuedEvent queuedEvent)
        {
            //get the args that were queued
            OpenTK.Input.MouseButtonEventArgs e = (OpenTK.Input.MouseButtonEventArgs)queuedEvent.Arg1;
            TileClickInfo[] movedOverTiles = (TileClickInfo[])queuedEvent.Arg2;
            
            //tell the window manager about the mouse down, if was one of the normal buttons
            TycoonControl controlOver = null;
            if (e.Button == OpenTK.Input.MouseButton.Left || e.Button == OpenTK.Input.MouseButton.Right)
            {
                controlOver = _windowManager.ClickManager.HandleMouseDown(e.X, e.Y);
            }

            //raise the clicked event
            if (MouseDown != null)
            {                                    
                MouseDown(new ClickInfo(e.X, e.Y, (MouseButton)e.Button, movedOverTiles, controlOver));                
            }
        }

        private void RaiseMouseUp(QueuedEvent queuedEvent)
        {
            //get the args that were queued
            OpenTK.Input.MouseButtonEventArgs e = (OpenTK.Input.MouseButtonEventArgs)queuedEvent.Arg1;
            TileClickInfo[] movedOverTiles = (TileClickInfo[])queuedEvent.Arg2;

            //tell the window manager about the mouse up, if was one of the normal buttons
            TycoonControl controlOver = null;
            if (e.Button == OpenTK.Input.MouseButton.Left || e.Button == OpenTK.Input.MouseButton.Right)
            {
                controlOver = _windowManager.ClickManager.HandleMouseUp(e.X, e.Y);
            }

            //raise the clicked event
            if (MouseUp != null)
            {                
                MouseUp(new ClickInfo(e.X, e.Y, (MouseButton)e.Button, movedOverTiles, controlOver));                
            }
        }

        private void RaiseMouseWheel(QueuedEvent queuedEvent)
        {
            //get the args that were queued
            OpenTK.Input.MouseWheelEventArgs e = (OpenTK.Input.MouseWheelEventArgs)queuedEvent.Arg1;
            
            //raise the mouse wheel event
            if (MouseWheel != null)
            {
                MouseWheel(e.DeltaPrecise);
            }
        }

        private void RaiseKeyUp(QueuedEvent queuedEvent)
        {
            //get the args that were queued
            OpenTK.Input.KeyboardKeyEventArgs e = (OpenTK.Input.KeyboardKeyEventArgs)queuedEvent.Arg1;
            TileClickInfo[] movedOverTiles = (TileClickInfo[])queuedEvent.Arg2;

            //tell window manager
            bool consumed = _windowManager.ClickManager.HandleKeyUp((Key)e.Key);
            if (consumed) { return; }

            //raise the event
            if (KeyUp != null)
            {
                KeyUp((Key)e.Key);
            }
        }

        private void RaiseKeyDown(QueuedEvent queuedEvent)
        {
            //get the args that were queued
            OpenTK.Input.KeyboardKeyEventArgs e = (OpenTK.Input.KeyboardKeyEventArgs)queuedEvent.Arg1;
            TileClickInfo[] movedOverTiles = (TileClickInfo[])queuedEvent.Arg2;
            
            //tell window manager
            bool consumed = _windowManager.ClickManager.HandleKeyDown((Key)e.Key);
            if (consumed) { return; }

            //raise the event
            if (KeyDown != null)
            {
                KeyDown((Key)e.Key);
            }
        }

        private void RaiseWindowSizeChanged(QueuedEvent queuedEvent)
        {
            //raise the event
            if (WindowSizeChanged != null)
            {
                WindowSizeChanged();
            }
        }

        #endregion

    }
}
