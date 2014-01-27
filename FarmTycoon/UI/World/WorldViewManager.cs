using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Diagnostics;
using System.Drawing;

namespace FarmTycoon
{
    /// <summary>
    /// Handel input that handels the view of the world
    /// </summary>
    public class WorldViewManager
    {
        //how many nano seconds to wait between moves
        private static long MOVE_FREQUENCY = (long)(0.1 * 1000000000);
        private static float MOVE_DISTANCE = 0.5f;

        private long _moveTimeCounter;

        private float _moveAmountX;

        private float _moveAmountY;

        private bool _moving = false;
        private bool _movingMouse = false;

        private bool _mouseMiddleDown = false;
        private Point _middleDownPoint;

        /// <summary>
        /// Manage the view of the world
        /// </summary>
        public WorldViewManager()
        {
            Program.UserInterface.Graphics.Events.KeyDown += new KeyboardEventHandler(Graphics_KeyDown);
            Program.UserInterface.Graphics.Events.KeyUp += new KeyboardEventHandler(Graphics_KeyUp);
            Program.UserInterface.Graphics.Events.MouseWheel += new MouseWheelEventHandler(Graphics_MouseWheel);

            Program.UserInterface.Graphics.Events.MouseMoved += new MouseEventHandler(Events_MouseMoved);


            Program.UserInterface.Graphics.Events.MouseDown += new MouseEventHandler(Events_MouseDown);
            Program.UserInterface.Graphics.Events.MouseUp += new MouseEventHandler(Events_MouseUp);

            Program.GameThread.TimePassed += new Action<long>(Graphics_UpdateFrame);
        }



        private void Events_MouseDown(ClickInfo clickInfo)
        {
            if (clickInfo.Button == MouseButton.Middle)
            {
                _mouseMiddleDown = true;
                _middleDownPoint = new Point(clickInfo.X, clickInfo.Y);
            }
        }


        private void Events_MouseUp(ClickInfo clickInfo)
        {
            if (clickInfo.Button == MouseButton.Middle)
            {
                _mouseMiddleDown = false;
                _movingMouse = false;
                _moveAmountX = 0;
                _moveAmountY = 0;
            }
        }

        private void Events_MouseMoved(ClickInfo clickInfo)
        {
            if (_mouseMiddleDown)
            {
                int deltaX = _middleDownPoint.X - clickInfo.X;
                int deltaY = _middleDownPoint.Y - clickInfo.Y;

                float speedX = deltaX / 50.0f;
                float speedY = deltaY / 50.0f;


                _moveAmountX = -1 * speedX * MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                _moveAmountY = -1 * speedY * MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                _movingMouse = true;
            }
            else
            {
                //if (Program.UserInterface.Graphics.ViewRotation == ViewDirection.North)
                //{
                //    _movingMouse = false;
                //    if (clickInfo.X < 10)
                //    {
                //        _moveAmountX = -1 * MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                //        _movingMouse = true;
                //    }
                //    else if (clickInfo.X > Program.UserInterface.Graphics.WindowWidth - 10)
                //    {
                //        _moveAmountX = MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                //        _movingMouse = true;
                //    }
                //    else
                //    {
                //        _moveAmountX = 0;
                //    }

                //    if (clickInfo.Y < 10)
                //    {
                //        _moveAmountY = -2 * MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                //        _movingMouse = true;
                //    }
                //    else if (clickInfo.Y > Program.UserInterface.Graphics.WindowHeight - 10)
                //    {
                //        _moveAmountY = 2 * MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                //        _movingMouse = true;
                //    }
                //    else
                //    {
                //        _moveAmountY = 0;
                //    }

                //}
            }
        }

        private void Graphics_MouseWheel(float delta)
        {         
            if (delta > 0)
            {
                Program.UserInterface.Graphics.Scale *= 2.0f;
            }
            else if (delta < 0)
            {
                Program.UserInterface.Graphics.Scale *= 0.5f;
            }
        }

        /// <summary>
        /// Called after every frame
        /// </summary>        
        private void Graphics_UpdateFrame(long timeSinceLastUpdate)
        {
            //do nothing if not moving
            if (_moving == false && _movingMouse == false)
            {
                return;
            }

            _moveTimeCounter += timeSinceLastUpdate;
            if (_moveTimeCounter >= MOVE_FREQUENCY)
            {
                _moveTimeCounter = _moveTimeCounter - MOVE_FREQUENCY;

                Program.UserInterface.Graphics.ViewX += _moveAmountX;
                Program.UserInterface.Graphics.ViewY += _moveAmountY;
            }
        }

        /// <summary>
        /// User raised a key
        /// </summary>        
        private void Graphics_KeyUp(Key key)
        {
            HandelPanningKeyUp(key);           
        }

        /// <summary>
        /// User lowered a ket
        /// </summary>
        private void Graphics_KeyDown(Key key)
        {

            if (key == Key.Tilde)
            {
                TycoonGraphics.DEBUG_ShowOnlyLayer = -1;
            }
            if (key == Key.Number0)
            {
                TycoonGraphics.DEBUG_ShowOnlyLayer = 0;
            }
            if (key == Key.Number1)
            {
                TycoonGraphics.DEBUG_ShowOnlyLayer = 1;
            }
            if (key == Key.Number2)
            {
                TycoonGraphics.DEBUG_ShowOnlyLayer = 2;
            }
            if (key == Key.Number3)
            {
                TycoonGraphics.DEBUG_ShowOnlyLayer = 3;
            }
            if (key == Key.Number4)
            {
                TycoonGraphics.DEBUG_ShowOnlyLayer = 4;
            }
            if (key == Key.Number5)
            {
                TycoonGraphics.DEBUG_ShowOnlyLayer = 5;
            }
            if (key == Key.Number6)
            {
                TycoonGraphics.DEBUG_ShowOnlyLayer = 6;
            }
            if (key == Key.Number7)
            {
                TycoonGraphics.DEBUG_ShowOnlyLayer = 7;
            }
            if (key == Key.Number8)
            {
                TycoonGraphics.DEBUG_ShowOnlyLayer = 8;
            }
            if (key == Key.Number9)
            {
                TycoonGraphics.DEBUG_ShowOnlyLayer = 9;
            }
            if (key == Key.RBracket)
            {
                TycoonGraphics.DEBUG_ShowOnlyLayer++;
            }
            if (key == Key.LBracket)
            {
                TycoonGraphics.DEBUG_ShowOnlyLayer--;
            }
            

            if (TycoonGraphics.DEBUG_ShowOnlyLayer == -1)
            {
                DebugToolWindow.VisibleLayers = "All Layers";
            }
            else
            {
                DebugToolWindow.VisibleLayers = "Layer 0-" + TycoonGraphics.DEBUG_ShowOnlyLayer.ToString();
            }
            

            if (key == Key.F5)
            {
                new DebugToolWindow();
            }


            HandelZoomKeyDown(key);
            HandelRotationKeyDown(key);
            HandelPanningKeyDown(key);
        }

        private void HandelZoomKeyDown(Key key)
        {
            if (key == Key.Plus || key == Key.KeypadPlus)
            {
                Program.UserInterface.Graphics.Scale *= 2;
            }
            else if (key == Key.Minus || key == Key.KeypadMinus)
            {
                Program.UserInterface.Graphics.Scale *= 0.5f;
            }
        }

        private void HandelRotationKeyDown(Key key)
        {
            //if (key == Key.N)
            //{
            //    Program.UserInterface.Graphics.ViewRotation = ViewDirection.North;
            //}
            //else if (key == Key.S)
            //{
            //    Program.UserInterface.Graphics.ViewRotation = ViewDirection.South;
            //}
            //else if (key == Key.E)
            //{
            //    Program.UserInterface.Graphics.ViewRotation = ViewDirection.East;
            //}
            //else if (key == Key.W)
            //{
            //    Program.UserInterface.Graphics.ViewRotation = ViewDirection.West;
            //}
        }
        
        private void HandelPanningKeyDown(Key key)
        {

            //if (key == Key.Left)
            //{
            //    Program.UserInterface.Graphics.WindowWidth -= 1;
            //}
            //else if (key == Key.Right)
            //{
            //    Program.UserInterface.Graphics.WindowWidth += 1;
            //}
            //else if (key == Key.Up)
            //{
            //    Program.UserInterface.Graphics.WindowHeight -= 1;
            //}
            //else if (key == Key.Down)
            //{
            //    Program.UserInterface.Graphics.WindowHeight += 1;
            //}
            //return;

            if (Program.UserInterface.Graphics.ViewRotation == ViewDirection.North)
            {
                if (key == Key.Up)
                {
                    _moveAmountY = -1 * 2 * MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                    _moving = true;
                }
                else if (key == Key.Down)
                {
                    _moveAmountY = 2 * MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                    _moving = true;
                }
                else if (key == Key.Left)
                {
                    _moveAmountX = -1 * MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                    _moving = true;
                }
                else if (key == Key.Right)
                {
                    _moveAmountX = MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                    _moving = true;
                }
            }
            else if (Program.UserInterface.Graphics.ViewRotation == ViewDirection.South)
            {
                if (key == Key.Up)
                {
                    _moveAmountY = 2 * MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                    _moving = true;
                }
                else if (key == Key.Down)
                {
                    _moveAmountY = -1 * 2 * MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                    _moving = true;
                }
                else if (key == Key.Left)
                {
                    _moveAmountX = MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                    _moving = true;
                }
                else if (key == Key.Right)
                {
                    _moveAmountX = -1 * MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                    _moving = true;
                }
            }
            else if (Program.UserInterface.Graphics.ViewRotation == ViewDirection.East)
            {
                if (key == Key.Up)
                {
                    _moveAmountX = 2 * MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                    _moving = true;
                }
                else if (key == Key.Down)
                {
                    _moveAmountX = -1 * 2 * MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                    _moving = true;
                }
                else if (key == Key.Left)
                {
                    _moveAmountY = -1 * MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                    _moving = true;
                }
                else if (key == Key.Right)
                {
                    _moveAmountY = MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                    _moving = true;
                }
            }
            else if (Program.UserInterface.Graphics.ViewRotation == ViewDirection.West)
            {
                if (key == Key.Up)
                {
                    _moveAmountX = -1 * 2 * MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                    _moving = true;
                }
                else if (key == Key.Down)
                {
                    _moveAmountX = 2 * MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                    _moving = true;
                }
                else if (key == Key.Left)
                {
                    _moveAmountY = MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                    _moving = true;
                }
                else if (key == Key.Right)
                {
                    _moveAmountY = -1 * MOVE_DISTANCE * (1.0f / Program.UserInterface.Graphics.Scale);
                    _moving = true;
                }
            }
        }

        private void HandelPanningKeyUp(Key key)
        {
            if (Program.UserInterface.Graphics.ViewRotation == ViewDirection.North || Program.UserInterface.Graphics.ViewRotation == ViewDirection.South)
            {
                if (key == Key.Up || key == Key.Down)
                {
                    _moveAmountY = 0.0f;
                }
                else if (key == Key.Left || key == Key.Right)
                {
                    _moveAmountX = 0.0f;
                }
            }
            else if (Program.UserInterface.Graphics.ViewRotation == ViewDirection.East || Program.UserInterface.Graphics.ViewRotation == ViewDirection.West)
            {
                if (key == Key.Up || key == Key.Down)
                {
                    _moveAmountX = 0.0f;
                }
                else if (key == Key.Left || key == Key.Right)
                {
                    _moveAmountY = 0.0f;
                }
            }

            if ((int)_moveAmountX == 0 && (int)_moveAmountY == 0)
            {
                _moving = false;
            }
        }

    }
}
