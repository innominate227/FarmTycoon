using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TycoonGraphicsLib
{
    internal class SharedWorldViewSettings
    {
        /// <summary>
        /// Number of pixels for one world unit in the X
        /// </summary>
        public static float WORLD_UNIT_WIDTH = 32.0f;

        /// <summary>
        /// Number of pixels for one world unit in the Y
        /// </summary>
        public static float WORLD_UNIT_HEIGHT = 16.0f; 


        /// <summary>
        /// Direction the world is being viewed from
        /// </summary>
        private SimpleDelayedValue<ViewDirection> _direction = new SimpleDelayedValue<ViewDirection>(ViewDirection.North);

        /// <summary>
        /// Scale the world is being viewed at
        /// </summary>
        private SimpleDelayedValue<float> _scale = new SimpleDelayedValue<float>(1.0f);

        /// <summary>
        /// World the setting are being shared for.
        /// </summary>
        private World _world;
                

        /// <summary>
        /// Create a new SharedWorldViewSettings
        /// </summary>
        public SharedWorldViewSettings(World world)
        {
            _world = world;            
        }

        /// <summary>
        /// Direction the world is being viewed from (use the from the drawing thread)
        /// </summary>
        public ViewDirection CurrentDirection
        {
            get { return _direction.Current; }
        }

        /// <summary>
        /// Direction the world is being viewed from
        /// </summary>
        public ViewDirection Direction
        {
            get { return _direction.Delayed; }
            set { _direction.Delayed = value; }
        }
        
        /// <summary>
        /// Scale the world is being viewed at (use the from the drawing thread)
        /// </summary>
        public float CurrentScale
        {
            get { return _scale.Delayed; }
        }

        /// <summary>
        /// Scale the world is being viewed at
        /// </summary>
        public float Scale
        {
            get { return _scale.Delayed; }
            set { _scale.Delayed = value; }
        }

        /// <summary>
        /// Use the values that were set, but we have delayed using them because we might have been rendering a frame
        /// </summary>
        public void UseDelayedValues()
        {
            //update scale and direction to be what the Game thread has set them to be
            bool scaleChanged = _scale.UseDelayed();
            bool directionChanged = _direction.UseDelayed();

            //if they change recalculate all tiles
            if (scaleChanged || directionChanged)
            {
                _world.TileManger.ReCalcTiles();
            }
        }

		
		/// <summary>
		/// Number of open gl points for world unit in the X direction 
		/// </summary>
		public float PointsPerWorldUnitX
		{
            get { return WindowSettings.PointsPerPixelX * WORLD_UNIT_WIDTH * _scale.Current; }	
		}
				
		/// <summary>
		/// Number of open gl points for world unit in the Y direction 
		/// </summary>
		public float PointsPerWorldUnitY
		{
            get { return WindowSettings.PointsPerPixelY * WORLD_UNIT_HEIGHT * _scale.Current; }	
		}

		/// <summary>
		/// Number of open gl points for world unit in the Z direction 
		/// </summary>
		public float PointsPerWorldUnitZ
		{
            get { return WindowSettings.PointsPerPixelY * (WORLD_UNIT_HEIGHT / 2.0f) * _scale.Current; }	
		}
		/// <summary>
		/// Number of world units wide the window is 
		/// </summary>
		public float WindowWidthWorldUnits
		{
            get { return WindowSettings.Width / (WORLD_UNIT_WIDTH * _scale.Current); }
		}
				
		/// <summary>
		/// Number of world units tall the window is 
		/// </summary>
		public float WindowHeightWorldUnits
		{
            get { return WindowSettings.Height / (WORLD_UNIT_HEIGHT * _scale.Current); }
		}
		
		
		/// <summary>
		/// Get the x, and y of a world unit location adjusted for the rotation of the view
		/// </summary>
        internal void GetXYForRotation(float x, float y, out float outX, out float outY)
        {
            if (_direction.Current == ViewDirection.North)
            {
                outX = x;
                outY = y;
            }
            else if (_direction.Current == ViewDirection.East)
            {
                outX = y;
                outY = _world.WorldSettings.WorldSize - x - 1;
            }
            else if (_direction.Current == ViewDirection.South)
            {
                outX = _world.WorldSettings.WorldSize - x - 1;
                outY = _world.WorldSettings.WorldSize - y - 1;
            }
            else //if (_direction.Current == Direction.West)
            {
                outX = _world.WorldSettings.WorldSize - y - 1;
                outY = x;
            }
        }


    }
}
