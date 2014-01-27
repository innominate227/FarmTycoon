using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace TycoonGraphicsLib
{
    internal class WorldView
    {
        /// <summary>
        /// X location being viewed in world units.
        /// </summary>
        private SimpleDelayedValue<float> _x = new SimpleDelayedValue<float>(0);
            
        /// <summary>
        /// Y location being viewed in world units.
        /// </summary>
        private SimpleDelayedValue<float> _y = new SimpleDelayedValue<float>(0);
        
        /// <summary>
        /// Z location being viewed in world units.
        /// </summary>
        private SimpleDelayedValue<float> _z = new SimpleDelayedValue<float>(0);
		
		/// <summary>
		/// Render location X in pixels
		/// </summary>
		private int _renderLocX;
		
		/// <summary>
        /// Render location Y in pixels
		/// </summary>
		private int _renderLocY;
		
		/// <summary>
        /// Render location width in pixels
		/// </summary>
		private int _renderWidth;
		
		/// <summary>
        /// Render location Heights in pixels
		/// </summary>
		private int _renderHeight;
		
		/// <summary>
		/// Amount of extra rows to draw
		/// All rows will be drawn such that object with Z=0 will be on the screen
		/// but if you have objects with Z > 0 more rows will need to be drawn so that
		/// those object appear on the screen 
		/// </summary>
		private int _overdraw;
        
        /// <summary>
        /// World being viewed.
        /// </summary>
        private World _world;
        
        /// <summary>
        /// Create a world view to view the world passed.
        /// </summary>
        /// <param name="world"></param>
        public WorldView(World world)
        {
            _world = world;
        }
        
        /// <summary>
        /// X location being viewed.
        /// </summary>
        public float X
        {
            get { return _x.Delayed; }
            set { _x.Delayed = value; }
        }

        /// <summary>
        /// Y location being viewed.
        /// </summary>
        public float Y
        {
            get { return _y.Delayed; }
            set { _y.Delayed = value; }
        }

        /// <summary>
        /// Z location being viewed.
        /// </summary>
        public float Z
        {
            get { return _z.Delayed; }
            set { _z.Delayed = value; }
        }
				
		/// <summary>
		/// Render location X (The X pixel of the screen where the view is rendered)
		/// </summary>
		public int RenderLocX
		{
			get{ return _renderLocX; }
			set{ _renderLocX = value; }
		}
		
		/// <summary>
        /// Render location Y (The Y pixel of the screen where the view is rendered)
		/// </summary>
		public int RenderLocY
		{
			get{ return _renderLocY; }
			set{ _renderLocY = value; }
		}
		
		/// <summary>
		/// Render location width (The width of the area of the screen to render at in pixels)
		/// </summary>
		public int RenderWidth
		{
			get{ return _renderWidth; }
			set{ _renderWidth = value; }
		}
		
		/// <summary>
        /// Render location Heights (The height of the area of the screen to render at in pixels)
		/// </summary>
		public int RenderHeight
		{
			get{ return _renderHeight; }
			set{ _renderHeight = value; }
		}
			
		/// <summary>
		/// Amount of extra rows to draw
		/// All rows will be drawn such that object with Z=0 will be on the screen
		/// but if you have objects with Z > 0 more rows will need to be drawn so that
		/// those object appear on the screen.
		/// </summary>
		public int Overdraw
		{
			get { return _overdraw; }
			set { _overdraw = value; }	
		}
        
        /// <summary>
        /// Direction the world is being viewed from.
        /// Note this property is shared by all world view of the same world.
        /// </summary>
        public ViewDirection Direction
        {
            get { return _world.SharedWorldViewSettings.Direction; }
            set { _world.SharedWorldViewSettings.Direction = value; }
        }

        /// <summary>
        /// Scale the world is being viewed at.
        /// Note this property is shared by all world view of the same world.
        /// </summary>
        public float Scale
        {
            get { return _world.SharedWorldViewSettings.Scale; }
            set { _world.SharedWorldViewSettings.Scale = value; }
        }
                
        /// <summary>
        /// Render the world view to the screen
        /// </summary>
        public void RenderView()
        {
            //if any view settings were changed on another thread make sure were using the latest values
            _x.UseDelayed();
            _y.UseDelayed();
            _z.UseDelayed();
            _world.SharedWorldViewSettings.UseDelayedValues();

            //enable things needed for texture drawing
            GL.Enable(EnableCap.Texture2D);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            //set the first texture sheet to active
            _world.TileTextureManager.CurrentTextureSheetIndex = 0;
                                        		
			//restrict area being drawn in to the draw region
			GL.Scissor(_renderLocX, WindowSettings.Height - _renderLocY - _renderHeight, _renderWidth, _renderHeight);
            						
			//draw black background
			GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.Zero);
            GL.Begin(BeginMode.Quads);
            GL.Color4(0f, 0f, 0f, 1f); GL.Vertex2(-1.0f, -1.0f);
            GL.Color4(0f, 0f, 0f, 1f); GL.Vertex2(1.0f, -1.0f);
            GL.Color4(0f, 0f, 0f, 1f); GL.Vertex2(1.0f, 1.0f);
            GL.Color4(0f, 0f, 0f, 1f); GL.Vertex2(-1.0f, 1.0f);
            GL.End();
			
			//need this so the textures ddont draw as black
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			GL.Color4(1f, 1f, 1f, 1f);

            //draw region in world view units
            int drawRegionLeft, drawRegionTop, drawRegionRight, drawRegionBottom;
            float transX, transY;
            CalculateDrawingValues(out drawRegionLeft, out drawRegionTop, out drawRegionRight, out drawRegionBottom, out transX, out transY);

            //translate so we can see what we are drawing
            GL.PushMatrix();
            GL.Translate(transX, transY, 0);

            //get the segments that the draw region coresponds to
            int leftSegmentNum = drawRegionLeft / _world.WorldSettings.SegmentSize;
            int rightSegmentNum = drawRegionRight / _world.WorldSettings.SegmentSize;
            int topSegmentNum = drawRegionTop / _world.WorldSettings.SegmentSize;
            int bottomSegmentNum = drawRegionBottom / _world.WorldSettings.SegmentSize;

            //STATRT DEBUG
            int DEBUG_currentMaxLayer = -1;
            //END DEBUG

            //render each layer from 0 to number of layers
            for (int layer = 0; layer < _world.WorldSettings.Layers; layer++)
            {
                ////STATRT DEBUG
                if (TycoonGraphics.DEBUG_ShowOnlyLayer != -1 && TycoonGraphics.DEBUG_ShowOnlyLayer < layer) { continue; }
                ////END DEBUG

                //foreach texture sheet (on most modern graphics cards this will just be 1)
                for (int textureSheetIndex = 0; textureSheetIndex < _world.TileTextureManager.TextureSheets.Count; textureSheetIndex++)
                {
                    //foreach y segment (generally we only see about 2x2 segments)
                    for (int ySegment = topSegmentNum; ySegment <= bottomSegmentNum; ySegment++)
                    {
                        //for each x segemnt
                        for (int xSegment = leftSegmentNum; xSegment <= rightSegmentNum; xSegment++)
                        {
                            //get the tile set to draw, and render it
                            TileSet tileSet = _world.TileManger.GetTileSetForSegment(xSegment, ySegment, layer, textureSheetIndex);

                            //can be null if our segment is outside of segments that actually exsist (player has moved view away from game area)
                            if (tileSet != null)
                            {

                                ////STATRT DEBUG
                                if (tileSet.NotEmpty && layer > DEBUG_currentMaxLayer)
                                {
                                    DEBUG_currentMaxLayer = layer;
                                }
                                if (tileSet.NotEmpty && layer > TycoonGraphics.DEBUG_AllTimeMaxLayer)
                                {
                                    TycoonGraphics.DEBUG_AllTimeMaxLayer = layer;
                                }
                                ////END DEBUG


                                //only render the tile buffer if there is something in it
                                if (tileSet.NotEmpty)
                                {
                                    //switch to the correct texture sheet if we are not already using the correct sheet
                                    if (_world.TileTextureManager.CurrentTextureSheetIndex != textureSheetIndex)
                                    {
                                        _world.TileTextureManager.CurrentTextureSheetIndex = textureSheetIndex;
                                    }

                                    //render the buffer
                                    tileSet.Render();
                                }
                            }
                        }//x
                    }//y
                }//texture sheet
            }//layer


            //STATRT DEBUG
            TycoonGraphics.DEBUG_CurrentMaxLayer = DEBUG_currentMaxLayer;
            //END DEBUG

            GL.PopMatrix();
        }
		    
        /// <summary>
        /// Get the tiles that are being rendered at the position specified.  
        /// Also returns where each tile was clicked (by way of percentX, and percentY).
        /// </summary>
        public List<TileClickInfo> GetTilesAtPosition(int mouseX, int mouseY)
        {
            List<TileClickInfo> toReturn = new List<TileClickInfo>();

            //get drawing settings
			int drawRegionLeft, drawRegionTop, drawRegionRight, drawRegionBottom;				
			float transX, transY;
			CalculateDrawingValues(out drawRegionLeft, out drawRegionTop, out drawRegionRight, out drawRegionBottom, out transX, out transY);	
			
            //get the point clicked in points (openGL units)
			float pointXClicked = -1 - transX + (mouseX * WindowSettings.PointsPerPixelX);
            float pointYClicked = 1 - transY - (mouseY * WindowSettings.PointsPerPixelY);

            //get the X point clicked in world units (the pointXClick + 1 because -1 is far left of the screen)
            int pointXClickedWorldUnits = (int)((1 + pointXClicked) / _world.SharedWorldViewSettings.PointsPerWorldUnitX);
            //what segment the x point clicked is in
            int xSegmentIn = pointXClickedWorldUnits / _world.WorldSettings.SegmentSize;

            //we only need to search segments that are around the same x as the mouse
            //default x start and end search areas to be the one segment
            int xSearchSegmentStart = xSegmentIn;
            int xSearchSegmentEnd = xSegmentIn;

            //we need to increase the size because we can be on the border of two segments.  
            //Each point is in a singel segment, but the tile can overlap into neighboring segments.  
            const int MAX_TILE_SIZE = 5;

            //if x is near front of segment search the segment beofore this one too
            if ((pointXClickedWorldUnits - MAX_TILE_SIZE) / _world.WorldSettings.SegmentSize < xSearchSegmentStart)
            {
                xSearchSegmentStart -= 1;
            }
            //if x is near end of segment search the segment after this one too
            if ((pointXClickedWorldUnits + MAX_TILE_SIZE) / _world.WorldSettings.SegmentSize > xSearchSegmentStart)
            {
                xSearchSegmentEnd += 1;
            }

            //search all y segments being drawn (there is no limit to how high the texture could be, or how large its Z could be)
            int ySearchSegmentTop = drawRegionTop / _world.WorldSettings.SegmentSize;
            int ySearchSegmentBottom = drawRegionBottom / _world.WorldSettings.SegmentSize;

            //for each layer
            for (int layer = _world.WorldSettings.Layers - 1; layer >= 0; layer--)
            {
                for (int textureSheetIndex = 0; textureSheetIndex < _world.TileTextureManager.TextureSheets.Count; textureSheetIndex++)
                {
                    //for each y segment
                    for (int ySegment = ySearchSegmentTop; ySegment <= ySearchSegmentBottom; ySegment++)
                    {
                        //for each x segment
                        for (int xSegment = xSearchSegmentStart; xSegment <= xSearchSegmentEnd; xSegment++)
                        {
                            //get the tile set for this area
                            TileSet tileSet = _world.TileManger.GetTileSetForSegment(xSegment, ySegment, layer, textureSheetIndex);
                            if (tileSet != null)
                            {
                                //look at each tile in that tile set
                                foreach (Tile tile in tileSet)
                                {
                                    float left, top, right, bottom, texLeft, texTop, texRight, texBottom;
                                    tileSet.GetTileRenderValues(tile, out left, out top, out right, out bottom, out texLeft, out texTop, out texRight, out texBottom);

                                    if (left <= pointXClicked && pointXClicked <= right && top >= pointYClicked && pointYClicked >= bottom)
                                    {
                                        TileTexture texture = tile.TileTexture;

                                        float tilePointClickedXPercent = (pointXClicked - left) / (right - left);
                                        float tilePointClickedYPercent = (top - pointYClicked) / (top - bottom);

                                        int textureSheetImageWidth = texture.Texture.Sheet.TextureSheetImage.Width;
                                        int textureSheetImageHeight = texture.Texture.Sheet.TextureSheetImage.Height;

                                        float bitmapPointClickedXPixel = (texture.Texture.Left * textureSheetImageWidth) + ((texture.Texture.Right - texture.Texture.Left) * textureSheetImageWidth * tilePointClickedXPercent);
                                        float bitmapPointClickedYPixel = (texture.Texture.Top * textureSheetImageHeight) + ((texture.Texture.Bottom - texture.Texture.Top) * textureSheetImageHeight * tilePointClickedYPercent);

                                        if (texture.Texture.Sheet.TextureSheetImage.GetPixel((int)bitmapPointClickedXPixel, (int)bitmapPointClickedYPixel).A == 255)
                                        {
                                            toReturn.Add(new TileClickInfo(tile, tilePointClickedXPercent, tilePointClickedYPercent));
                                        }
                                    }
                                }
                            }
                        }//x
                    }//y
                }//texture sheet
            } //layer
            
            return toReturn;
        }
			
		/// <summary>
		/// Calculates several values used for both drawing the view, and for getting info about a mouse position.
        /// Draw region values are in world units, 
		/// </summary>
		private void CalculateDrawingValues(out int drawRegionLeft, out int drawRegionTop, out int drawRegionRight, out int drawRegionBottom, out float transX, out float transY)
		{	
			//get the view x and y corrected for rotation
            float viewX, viewY;
            _world.SharedWorldViewSettings.GetXYForRotation(_x.Current, _y.Current, out viewX, out viewY);

            //correct view y to take view z into account
            viewY -= (_z.Current / 2.0f);
			
			//get the height and width of the rendered portion in world units
            float renderWidthWorldUnits = _world.SharedWorldViewSettings.WindowWidthWorldUnits * ((float)_renderWidth / WindowSettings.Width);
            float renderHeightWorldUnits = _world.SharedWorldViewSettings.WindowHeightWorldUnits * ((float)_renderHeight / WindowSettings.Height);
			
			//need to adjust view x, and view y so it is centerd in the window, to center it needs to be offset
			//half the width/height minus half the width/height of a tile (each tile is 2 world units wide/tall).
			//for Y need to add an additional 1 to take into account the default tiles rise of standard tile wich is half the tile height. 
            //(Maybe user should take this into account when specifing position instead?)
			float centerOffsetXWorldUnit = (renderWidthWorldUnits / 2.0f) - 1.0f;
            float centerOffsetYWorldUnit = (renderHeightWorldUnits / 2.0f) - 1.0f + 1.0f;
			
			//determined the top left position that drawing will need to start at in order for view X and Y to be centered in the window 
            float drawRegionLeftF = viewX - centerOffsetXWorldUnit;
            float drawRegionTopF = viewY - centerOffsetYWorldUnit;

			//determine how far to translate so we see the tiles we are drawing (translates such that the center of what we are viewing is in the very center of the window)
            transX = _world.SharedWorldViewSettings.PointsPerWorldUnitX * -1 * drawRegionLeftF;
            transY = _world.SharedWorldViewSettings.PointsPerWorldUnitY * drawRegionTopF;

            //add the rendered x and y to the translate values (adds to translate such that the center of what we are viewing is in the center of the render region)
            transX += _renderLocX * WindowSettings.PointsPerPixelX;
            transY -= _renderLocY * WindowSettings.PointsPerPixelY;            
			
            //determine what world unit is at the very bottom of the draw region, and the very right
            //also for the very right add an extra segments lenght (this is need because otherwise the drawing may end a partial segment before the right of the screen)
            //and for the bottom add overdraw which it for tiles with large Z positions
			float drawRegionRightF = drawRegionLeftF + renderWidthWorldUnits + _world.WorldSettings.SegmentSize;
            float drawRegionBottomF = drawRegionTopF + renderHeightWorldUnits + _overdraw;

			
			//convert the draw region into ints
            //need the minus 1 and 2 there or else will see black regions on the screen
			drawRegionLeft = (int)drawRegionLeftF - 1;			
			drawRegionTop = (int)drawRegionTopF - 1;
			drawRegionRight = (int)drawRegionRightF;
			drawRegionBottom = (int)drawRegionBottomF + 1;
			
		}
			
			
    }
}
