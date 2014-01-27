
using System;
using System.Drawing;

namespace TycoonGraphicsLib
{


    public class TycoonLabel : TycoonControl
    {
        #region GUI Properties

        /// <summary>
        /// For labels that show a numeric value we can render quicker by using common textures.
        /// This is especialy faster when the numbers change often.  Set to true to go into numeric value mode
        /// </summary>
        private volatile bool _drawNumericValue = false;

        /// <summary>
        /// For labels that show a numeric value should we show a $ sign in front of the value
        /// </summary>
        private volatile bool _drawDollarSign = false;

        /// <summary>
        /// For labels that show a numeric value should we show the value as 0 to 5 stars
        /// </summary>
        private volatile bool _drawAsStars = false;

        /// <summary>
        /// For labels that show a numeric value this is the value they should show
        /// </summary>
        private volatile int _numericValue = 0;

        /// <summary>
        /// For labels that show a numeric value if this is not -1 then a decimal is shown with that number
        /// </summary>
        private volatile int _decimalValue = -1;

        /// <summary>
        /// String for the label
        /// </summary>
        private TycoonString _text = new TycoonString("Label");

        /// <summary>
        /// Main color of the label
        /// </summary>
        private Safe<Color> _backColor = new Safe<Color>(Color.FromArgb(192, 64, 64));

        /// <summary>
        /// Color of the labels border
        /// </summary>
        private Safe<Color> _borderColor = new Safe<Color>(Color.FromArgb(192, 64, 64));



        /// <summary>
        /// For labels that show a numeric value we can render quicker by using common textures.
        /// This is especialy faster when the numbers change often.  Set to true to go into numeric value mode
        /// </summary>
        public bool DrawNumericValue
        {
            get { return _drawNumericValue; }
            set
            {
                _drawNumericValue = value;
                RebufferWindowNextFrame();
            }
        }

        /// <summary>
        /// For labels that show a numeric value should we show a $ sign in front of the value
        /// </summary>
        public bool DrawDollarSign
        {
            get { return _drawDollarSign; }
            set
            {
                _drawDollarSign = value;
                RebufferWindowNextFrame();
            }
        }

        /// <summary>
        /// For labels that show a numeric value should we show the values 0-9 as 0 to 5 stars
        /// </summary>
        public bool DrawAsStars
        {
            get { return _drawAsStars; }
            set
            {
                _drawAsStars = value;
                RebufferWindowNextFrame();
            }
        }

        /// <summary>
        /// For labels that show a numeric value this is the value they should show
        /// </summary>
        public int NumericValue
        {
            get { return _numericValue; }
            set 
            {
                //dont allow negative values
                if (value < 0) { value = 0; }
                //if value did not change do nothing
                if (_numericValue == value) { return; }
                _numericValue = value;
                RebufferWindowNextFrame();
            }
        }

        /// <summary>
        /// For labels that show a numeric value if this is not -1 then a decimal is shown with that number
        /// </summary>
        public int DecimalValue
        {
            get { return _decimalValue; }
            set
            {
                //if value did not change do nothing
                if (_decimalValue == value) { return; }
                _decimalValue = value;
                RebufferWindowNextFrame();
            }
        }


        /// <summary>
        /// The string on the label
        /// </summary>
        public string Text
        {
            get { return _text.Text; }
            set
            {
                //dont rerender if text is already that
                if (_text.Text == value) { return; }

                //update the text
                _text.Text = value;
                
                StringTextureChanged(_text); 
            }
        }

        /// <summary>
        /// The color of the label text
        /// </summary>
        public Color TextColor
        {
            get { return _text.Color; }
            set 
            {
                _text.Color = value;
                StringTextureChanged(_text);
            }
        }

        /// <summary>
        /// The font of the label text
        /// </summary>
        public Font TextFont
        {
            get { return _text.Font; }
            set
            {
                _text.Font = value;
                StringTextureChanged(_text);
            }
        }

        /// <summary>
        /// The alignment of the label text
        /// </summary>
        public StringAlignment TextAlignment
        {
            get { return _text.Alignment; }
            set
            {
                _text.Alignment = value;
                StringTextureChanged(_text);
            }
        }

        /// <summary>
        /// The verticle alignment of the label text
        /// </summary>
        public StringAlignment TextVerticelAlignment
        {
            get { return _text.VerticelAlignment; }
            set
            {
                _text.VerticelAlignment = value;
                StringTextureChanged(_text);
            }
        }

        /// <summary>
        /// Main color of the label
        /// </summary>
        public Color BackColor
        {
            get { return _backColor.Value; }
            set { _backColor.Value = value; RebufferWindowNextFrame(); }
        }


        /// <summary>
        /// Color of the labels border
        /// </summary>
        public Color BorderColor
        {
            get { return _borderColor.Value; }
            set { _borderColor.Value = value; RebufferWindowNextFrame(); }
        }
        

        #endregion

        #region Render

        
        /// <summary>
        /// Called after the control has been added to its parent
        /// </summary>
        public override void AfterAddedToParent()
        {
            StringTextureChanged(_text); 
        }


        /// <summary>
        /// Called to allow this control to add any strings or icons it needs to add to the local texture sheet
        /// </summary>
        internal override void AddLocalTextures(TextureSheetBuilder textureSheetBuilder)
        {
            //if we are using a common texture for our text dont create a location texture for the text
            if (_drawNumericValue == false)
            {
                _text.Width = Width;
                _text.Height = Height;
                textureSheetBuilder.AddString(_text);
            }
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

            //add the label border
            int borderSlot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(borderSlot, left, top, right, bottom, _borderColor.Value);

            //add the label background
            int labelSlot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(labelSlot, almostLeft, almostTop, almostRight, almostBottom, _backColor.Value);

            //add the string for the label
            float textTop = top + 1 * WindowSettings.PointsPerPixelY;
            float textBottom = bottom + 1 * WindowSettings.PointsPerPixelY;
            if (_drawNumericValue == false)
            {
                //add the non common text
                int stringSlot = localTexturesBuffer.GetNextFreeSlot();
                Texture textTexture = localTextures.GetTexture(_text.SheetTextureName);
                localTexturesBuffer.SetSlotValues(stringSlot, left, textTop, right, textBottom, textTexture);
            }
            else if (_drawAsStars)
            {
                //draw the stars
                if (_numericValue > 9) { _numericValue = 9; }
                float starLeft = right - 50 * WindowSettings.PointsPerPixelX;
                textBottom = textTop - 15 * WindowSettings.PointsPerPixelY;
                int starsSlot = commonTexturesBuffer.GetNextFreeSlot();
                Texture starsTexture = commonTextures.GetTexture("star" + _numericValue.ToString());
                commonTexturesBuffer.SetSlotValues(starsSlot, starLeft, textTop, right, textBottom, starsTexture);
            }
            else
            {
                textBottom = textTop - 15 * WindowSettings.PointsPerPixelY;

                //how far off from the right is the next part we are printing
                int rightOffset = 0;

                //draw decimal if needed
                if (_decimalValue != -1)
                {
                    Texture decimalTexture = commonTextures.GetTexture("decimal_" + _decimalValue.ToString());

                    //get location to draw
                    float textRight = right - rightOffset * WindowSettings.PointsPerPixelX;
                    float textleft = textRight - decimalTexture.Width * WindowSettings.PointsPerPixelX;

                    //draw the text
                    int stringSlot = commonTexturesBuffer.GetNextFreeSlot();
                    commonTexturesBuffer.SetSlotValues(stringSlot, textleft, textTop, textRight, textBottom, decimalTexture);

                    rightOffset = 9;
                }


                //get number as a string
                string numericString = _numericValue.ToString();

                //index for the last two digits of the string
                int stringIndex = numericString.Length - 2;

                while (true)
                {
                    //if at index -1 that means for the last part we have just 1 digit
                    string drawingPart;
                    if (stringIndex == -1)
                    {
                        stringIndex = 0;
                        drawingPart = numericString.Substring(stringIndex, 1);
                    }
                    else
                    {
                        drawingPart = numericString.Substring(stringIndex, 2);
                    }

                    //get texture to draw
                    string drawingTexture = "num_" + drawingPart;
                    if (_drawDollarSign && stringIndex == 0)
                    {
                        drawingTexture = "cost_" + drawingPart;
                    }
                    Texture digitsTexture = commonTextures.GetTexture(drawingTexture);

                    //get location to draw
                    float textRight = right - rightOffset * WindowSettings.PointsPerPixelX;
                    float textleft = textRight - digitsTexture.Width * WindowSettings.PointsPerPixelX;

                    //draw the text
                    int stringSlot = commonTexturesBuffer.GetNextFreeSlot();
                    commonTexturesBuffer.SetSlotValues(stringSlot, textleft, textTop, textRight, textBottom, digitsTexture);

                    //if we just index 0 we are done
                    if (stringIndex == 0)
                    {
                        break;
                    }

                    //go down two more in index
                    stringIndex -= 2;

                    //update offset for next two digits
                    rightOffset += 12;
                }
            }

        }

        #endregion
	}
}
