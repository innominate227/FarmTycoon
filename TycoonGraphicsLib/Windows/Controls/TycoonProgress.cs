
using System;
using System.Drawing;

namespace TycoonGraphicsLib
{


    public class TycoonProgress : TycoonLabel
    {
        #region GUI Properties

        /// <summary>
        /// number between 1 and 100 that tells the progress
        /// </summary>
        private volatile int _progress;

        /// <summary>
        /// max value for the progress bar
        /// </summary>
        private volatile int _maxValue = 100;
        
        /// <summary>
        /// Progress color
        /// </summary>
        private Safe<Color> _progressColor = new Safe<Color>(Color.Black);
        

        /// <summary>
        /// number between 0 and MaxValue that tells the progress
        /// </summary>
        public int Progress
        {
            get { return _progress; }
            set 
            {
                _progress = value;
                if (_progress > _maxValue) { _progress = _maxValue; }
                if (_progress < 0) { _progress = 0; }
                RebufferWindowNextFrame(); 
            }
        }


        /// <summary>
        /// max value for the progress bar
        /// </summary>
        public int MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; RebufferWindowNextFrame(); }
        }

        /// <summary>
        /// Progress color
        /// </summary>
        public Color ProgressColor
        {
            get { return _progressColor.Value; }
            set { _progressColor.Value = value; RebufferWindowNextFrame(); }
        }

        #endregion

        #region Render
        

        /// <summary>
        /// Called to allow this control to add itself to the panel buffers.
        /// </summary>
        internal override void AddToBuffers(QuadColorBuffer linesBuffer, QuadTextureBuffer commonTexturesBuffer, QuadTextureBuffer localTexturesBuffer, TextureSheet commonTextures, TextureSheet localTextures)
        {
            //add nothing to the buffer if were invisible
            if (Visible == false)
            {
                return;
            }

            //add base to buffer
            base.AddToBuffers(linesBuffer, commonTexturesBuffer, localTexturesBuffer, commonTextures, localTextures);

            //get the position of the label
            float top, left, bottom, right;
            GetPositionPoints(out left, out top, out right, out bottom);

            //positions one pixel from the left,top,right,bottom
            float almostLeft = left + 1 * WindowSettings.PointsPerPixelX;
            float almostRight = right - 1 * WindowSettings.PointsPerPixelX;
            float almostTop = top - 1 * WindowSettings.PointsPerPixelY;
            float almostBottom = bottom + 1 * WindowSettings.PointsPerPixelY;

            //determine where the progress bar should end
            float totalLeftToRight = almostRight - almostLeft;
            float progressRight = almostLeft + (totalLeftToRight * (_progress / (float)_maxValue));

            //add the progress
            int progressSlot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(progressSlot, almostLeft, almostTop, progressRight, almostBottom, _progressColor.Value);
                                 
        }

        #endregion
	}
}
