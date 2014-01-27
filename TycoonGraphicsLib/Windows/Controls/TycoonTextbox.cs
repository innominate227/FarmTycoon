
using System;
using System.Drawing;

namespace TycoonGraphicsLib
{


    public class TycoonTextbox : TycoonControl
    {

        #region GUI Properties

        public event Action<TycoonTextbox> TextChanged;

        /// <summary>
        /// String for the textbox that is visible
        /// </summary>
        private TycoonString _text = new TycoonString("");

        /// <summary>
        /// Full text entered into the textbox
        /// </summary>
        private string _fullText = "";

        /// <summary>
        /// Main color of the textbox
        /// </summary>
        private Safe<Color> _backColor = new Safe<Color>(Color.White);

        /// <summary>
        /// Color of the border of the textbox
        /// </summary>
        private Safe<Color> _borderColor = new Safe<Color>(Color.Pink);

        /// <summary>
        /// Should the box only allow numbers
        /// </summary>
        private bool _numbersOnly = false;

        /// <summary>
        /// the maximum number of characters in the box
        /// </summary>
        private int _maxLenght = int.MaxValue;
                
        /// <summary>
        /// The string in the textbox
        /// </summary>
        public string Text
        {
            get { return _fullText; }
            set 
            {
                if (value == null) { value = ""; }
                _fullText = value;
                RebuildLocalTexturesSheetNextFrame();
                if (TextChanged != null)
                {
                    TextChanged(this);
                }
            }
        }

        /// <summary>
        /// The color of the button text
        /// </summary>
        public Color TextColor
        {
            get { return _text.Color; }
            set { _text.Color = value; StringTextureChanged(_text); }
        }

        /// <summary>
        /// The font of the textbox text
        /// </summary>
        public Font TextFont
        {
            get { return _text.Font; }
            set { _text.Font = value; StringTextureChanged(_text); }
        }
                        
        /// <summary>
        /// Main color of the button
        /// </summary>
        public Color BackColor
        {
            get { return _backColor.Value; }
            set { _backColor.Value = value; RebufferWindowNextFrame(); }
        }

        /// <summary>
        /// Color of the border of the textbox
        /// </summary>
        public Color BorderColor
        {
            get { return _borderColor.Value; }
            set { _borderColor.Value = value; RebufferWindowNextFrame(); }
        }
        
        /// <summary>
        /// Should the box only allow numbers
        /// </summary>
        public bool NumbersOnly
        {
            get { return _numbersOnly; }
            set { _numbersOnly = value; }
        }

        /// <summary>
        /// the maximum number of characters in the box
        /// </summary>
        public int MaxLenght
        {
            get { return _maxLenght; }
            set { _maxLenght = value; }
        }

        public TycoonTextbox()
        {
            _text.VerticelAlignment = StringAlignment.Center;
        }
        
        #endregion
        

        #region Rendering
        
        /// <summary>
        /// Set true if the text box is focused (blinky line on the textbox be visible)
        /// </summary>
        private bool _isFocused;

        /// <summary>
        /// Set true if the text box is focused (blinky line on the textbox be visible)
        /// </summary>
        public bool IsFocused
        {
            get { return _isFocused; }
            internal set { _isFocused = value; RebufferWindowNextFrame(); }
        }
        
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
        /// <param name="textureSheetBuilder"></param>
        internal override void AddLocalTextures(TextureSheetBuilder textureSheetBuilder)
        {
            int characters;
            _text.Width = CalculateStringWidth(out characters);
            _text.Text = _fullText.Substring(0, characters);
            _text.Height = Height - 2;
            textureSheetBuilder.AddString(_text);
        }

        /// <summary>
        /// Called to allow this control to add itself to the panel buffers.
        /// </summary>
        /// <param name="linesBuffer"></param>
        /// <param name="commonTexturesBuffer"></param>
        /// <param name="localTexturesBuffer"></param>
        /// <param name="commonTextures"></param>
        /// <param name="localTextures"></param>
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

            //add the border color
            int borderSlot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(borderSlot, left, top, right, bottom, _borderColor.Value);

            //add the main color
            int mainColorSlot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(mainColorSlot, almostLeft, almostTop, almostRight, almostBottom, _backColor.Value);
            
            //add the text
            int stringSlot = localTexturesBuffer.GetNextFreeSlot();
            Texture textTexture = localTextures.GetTexture(_text.SheetTextureName);
            localTexturesBuffer.SetSlotValues(stringSlot, almostLeft, almostTop, almostLeft + (textTexture.Width * WindowSettings.PointsPerPixelX), almostBottom, textTexture);

            if (_isFocused)
            {
                //add the line after the text is the blink is on
                int lineSlot = linesBuffer.GetNextFreeSlot();
                linesBuffer.SetSlotValues(lineSlot, almostLeft + (textTexture.Width * WindowSettings.PointsPerPixelX), almostTop - 2 * WindowSettings.PointsPerPixelY, almostLeft + ((textTexture.Width + 1) * WindowSettings.PointsPerPixelX), almostBottom + 2 * WindowSettings.PointsPerPixelY, Color.Black);
            }            
        }

        /// <summary>
        /// Calculate the width of the string that will be in the textbox
        /// </summary>
        private int CalculateStringWidth(out int characters)
        {
            //not used by required by function
            int lines;

            //need to mesaure string this bitmap is just for that
            Bitmap tmpBitmap = new Bitmap(1, 1);
            Graphics graphics = Graphics.FromImage(tmpBitmap);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            //meausre the width of the string
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = _text.Alignment;
            stringFormat.LineAlignment = _text.VerticelAlignment;
            SizeF size = graphics.MeasureString(_fullText.Trim(), _text.Font, new SizeF(this.Width+5, 5), stringFormat, out characters, out lines);

            //return the width of the string
            return ((int)size.Width) + 1;
        }

        #endregion
    }
}
