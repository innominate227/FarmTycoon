
using System;
using System.Drawing;

namespace TycoonGraphicsLib
{


    public class TycoonGauge : TycoonControl
    {
        #region GUI Properties
        
        /// <summary>
        /// Good color of the gauge
        /// </summary>
        private Safe<Color> _goodColor = new Safe<Color>(Color.FromArgb(181, 230, 29));

        /// <summary>
        /// middle color of the gauge
        /// </summary>
        private Safe<Color> _midColor = new Safe<Color>(Color.FromArgb(255, 242, 0));

        /// <summary>
        /// Bad color of the gauge
        /// </summary>
        private Safe<Color> _badColor = new Safe<Color>(Color.FromArgb(241, 86, 91));

        /// <summary>
        /// Color of the gauge border
        /// </summary>
        private Safe<Color> _borderColor = new Safe<Color>(Color.FromArgb(0, 0, 0));

        /// <summary>
        /// Quality to show in the gauge (between 0 and 99)
        /// </summary>
        private volatile int _quality = 99;

        /// <summary>
        /// Value to show in the gauge
        /// </summary>
        private volatile int _value = 50;

        /// <summary>
        /// Min value possible for the gauge
        /// </summary>
        private volatile int _minValue = 0;

        /// <summary>
        /// Max value possible for the gauge
        /// </summary>
        private volatile int _maxValue = 100;

        /// <summary>
        /// Value where the mid color starts
        /// </summary>
        private volatile int _midColorStart = 25;

        /// <summary>
        /// Value where the mid color ends
        /// </summary>
        private volatile int _midColorEnd = 75;

        /// <summary>
        /// Value where the good color starts
        /// </summary>
        private volatile int _goodColorStart = 40;

        /// <summary>
        /// Value where the good color ends
        /// </summary>
        private volatile int _goodColorEnd = 60;


        /// <summary>
        /// Tycoon Gauge Height must be 15
        /// </summary>
        public override int Height
        {
            get { return base.Height; }
            set { base.Height = 17; }
        }




        /// <summary>
        /// Quality to show in the gauge
        /// </summary>
        public int Quality
        {
            get { return _quality; }
            set 
            {
                if (value > 99) { value = 99; }
                if (value < 0) { value = 0; }
                _quality = value; 
                RebufferWindowNextFrame(); 
            }
        }

        /// <summary>
        /// Value for the gauge
        /// </summary>
        public int Value
        {
            get { return _value; }
            set { _value = value; RebufferWindowNextFrame(); }
        }

        /// <summary>
        /// Min value possible for the gauge
        /// </summary>
        public int MinValue
        {
            get { return _minValue; }
            set { _minValue = value; RebufferWindowNextFrame(); }
        }

        /// <summary>
        /// Max value possible for the gauge
        /// </summary>
        public int MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; RebufferWindowNextFrame(); }
        }


        /// <summary>
        /// Value where the mid color starts
        /// </summary>
        public int MidColorStart
        {
            get { return _midColorStart; }
            set { _midColorStart = value; RebufferWindowNextFrame(); }
        }
        
        /// <summary>
        /// Value where the mid color ends
        /// </summary>
        public int MidColorEnd
        {
            get { return _midColorEnd; }
            set { _midColorEnd = value; RebufferWindowNextFrame(); }
        }

        /// <summary>
        /// Value where the good color starts
        /// </summary>
        public int GoodColorStart
        {
            get { return _goodColorStart; }
            set { _goodColorStart = value; RebufferWindowNextFrame(); }
        }

        /// <summary>
        /// Value where the good color ends
        /// </summary>
        public int GoodColorEnd
        {
            get { return _goodColorEnd; }
            set { _goodColorEnd = value; RebufferWindowNextFrame(); }
        }


               



        /// <summary>
        /// Good color of the gauge
        /// </summary>
        public Color GoodColor
        {
            get { return _goodColor.Value; }
            set { _goodColor.Value = value; RebufferWindowNextFrame(); }
        }
        
        /// <summary>
        /// Middle color of the gauge
        /// </summary>
        public Color MidColor
        {
            get { return _midColor.Value; }
            set { _midColor.Value = value; RebufferWindowNextFrame(); }
        }
        
        /// <summary>
        /// Bad color of the gauge
        /// </summary>
        public Color BadColor
        {
            get { return _badColor.Value; }
            set { _badColor.Value = value; RebufferWindowNextFrame(); }
        }

        /// <summary>
        /// Color of the gauges border
        /// </summary>
        public Color BorderColor
        {
            get { return _borderColor.Value; }
            set { _borderColor.Value = value; RebufferWindowNextFrame(); }
        }


        #endregion

        #region Render

        /// <summary>
        /// Called to allow this control to add any strings or icons it needs to add to the local texture sheet
        /// </summary>
        internal override void AddLocalTextures(TextureSheetBuilder textureSheetBuilder)
        {            
            //we never use local textures the numbers we show are always added to the common textures             
        }



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

            //get the position of the label
            float top, left, bottom, right;
            GetPositionPoints(out left, out top, out right, out bottom);

            //positions one pixel from the left,top,right,bottom
            float almostLeft = left + 1 * WindowSettings.PointsPerPixelX;
            float almostRight = right - 1 * WindowSettings.PointsPerPixelX;
            float almostTop = top - 1 * WindowSettings.PointsPerPixelY;
            float almostBottom = bottom + 1 * WindowSettings.PointsPerPixelY;

            //add the gauge border
            int borderSlot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(borderSlot, left, top, right, bottom, _borderColor.Value);

            //determine color ranges
            int midStartPixel = (int)Math.Round((this.Width - 2) * ((double)(_midColorStart - _minValue) / (_maxValue - _minValue)));
            int midEndPixel = (int)Math.Round((this.Width - 2) * ((double)(_midColorEnd - _minValue) / (_maxValue - _minValue)));
            int goodStartPixel = (int)Math.Round((this.Width - 2) * ((double)(_goodColorStart - _minValue) / (_maxValue - _minValue)));
            int goodEndPixel = (int)Math.Round((this.Width - 2) * ((double)(_goodColorEnd - _minValue) / (_maxValue - _minValue)));
            
            //these fixes issue where the range is drawn a little over, or not quite to the end of the gauge
            if (midStartPixel == 0) { midStartPixel = 1; }
            if (goodStartPixel == 0) { goodStartPixel = 1; }
            if (midEndPixel == this.Width - 2) { midEndPixel = this.Width - 1; } 
            if (goodEndPixel == this.Width - 2) { goodEndPixel = this.Width - 1; }
            float midStart = left + midStartPixel * WindowSettings.PointsPerPixelX;
            float midEnd = left + midEndPixel * WindowSettings.PointsPerPixelX;
            float goodStart = left + goodStartPixel * WindowSettings.PointsPerPixelX;
            float goodEnd = left + goodEndPixel * WindowSettings.PointsPerPixelX;
            
            //add the bad color
            int badColorSlot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(badColorSlot, almostLeft, almostTop, almostRight, almostBottom, _badColor.Value);

            //add the mid color
            int midColorSlot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(midColorSlot, midStart, almostTop, midEnd, almostBottom, _midColor.Value);

            //add the good color
            int goodColorSlot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(goodColorSlot, goodStart, almostTop, goodEnd, almostBottom, _goodColor.Value);

            //determine where the value slider should be
            int valuePixel = (int)Math.Round((this.Width - 2) * ((double)(_value - _minValue) / (_maxValue - _minValue)));

            //make sure the value slider is no so close to the side that the text goes past the border
            int sideSpace = 7;            
            if (valuePixel > this.Width - sideSpace) { valuePixel = this.Width - sideSpace; }
            if (valuePixel < sideSpace) { valuePixel = sideSpace; }

            //determine points to create the slider arrows
            float valueFloat = left + valuePixel * WindowSettings.PointsPerPixelX;
            float valueFloat2 = left + (valuePixel + 1) * WindowSettings.PointsPerPixelX;
            float valueFloat3 = left + (valuePixel - 1) * WindowSettings.PointsPerPixelX;
            float valueFloat4 = left + (valuePixel + 2) * WindowSettings.PointsPerPixelX;
            float valueFloat5 = left + (valuePixel - 2) * WindowSettings.PointsPerPixelX;
            float valueFloat6 = left + (valuePixel + 3) * WindowSettings.PointsPerPixelX;
            float almostTop2 = top - 2 * WindowSettings.PointsPerPixelY;
            float almostTop3 = top - 3 * WindowSettings.PointsPerPixelY;
            float almostTop4 = top - 4 * WindowSettings.PointsPerPixelY;
            float almostBottom2 = bottom + 2 * WindowSettings.PointsPerPixelY;
            float almostBottom3 = bottom + 3 * WindowSettings.PointsPerPixelY;
            float almostBottom4 = bottom + 4 * WindowSettings.PointsPerPixelY;

            //draw lines for the slider arrows                        
            int gaugeLine1Slot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(gaugeLine1Slot, valueFloat, almostTop, valueFloat2, almostTop2, _borderColor.Value);
            int gaugeLine2Slot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(gaugeLine2Slot, valueFloat3, almostTop2, valueFloat4, almostTop3, _borderColor.Value);
            int gaugeLine3Slot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(gaugeLine3Slot, valueFloat5, almostTop3, valueFloat6, almostTop4, _borderColor.Value);
            int gaugeLine4Slot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(gaugeLine4Slot, valueFloat, almostBottom, valueFloat2, almostBottom2, _borderColor.Value);
            int gaugeLine5Slot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(gaugeLine5Slot, valueFloat3, almostBottom2, valueFloat4, almostBottom3, _borderColor.Value);
            int gaugeLine6Slot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(gaugeLine6Slot, valueFloat5, almostBottom3, valueFloat6, almostBottom4, _borderColor.Value);


            //determine text location
            int valueTextLeftPixel = valuePixel - 8;
            int valueTextRightPixel = valuePixel + 8;
            float valueTextLeft = left + valueTextLeftPixel * WindowSettings.PointsPerPixelX;
            float valueTextRight = left + valueTextRightPixel * WindowSettings.PointsPerPixelX;
            float valueTextTop = top - 3 * WindowSettings.PointsPerPixelY;
            float valueTextBottom = top - 13 * WindowSettings.PointsPerPixelY;

            //add the text
            int stringSlot = commonTexturesBuffer.GetNextFreeSlot();
            Texture textTexture = commonTextures.GetTexture("gauge_" + _quality.ToString());
            commonTexturesBuffer.SetSlotValues(stringSlot, valueTextLeft, valueTextTop, valueTextRight, valueTextBottom, textTexture);
        }

        #endregion
    }
}
