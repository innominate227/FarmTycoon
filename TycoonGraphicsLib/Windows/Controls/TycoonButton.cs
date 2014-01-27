
using System;
using System.Drawing;

namespace TycoonGraphicsLib
{


    public class TycoonButton : TycoonControl
    {

        #region GUI Properties

        /// <summary>
        /// String for the title of the window
        /// </summary>
        private TycoonString _text = new TycoonString("Button");

        /// <summary>
        /// Main color of the button
        /// </summary>
        private Safe<Color> _backColor = new Safe<Color>(Color.Red);

        /// <summary>
        /// Color of the drop shaodw above and to the left of the button
        /// </summary>
        private Safe<Color> _shadowLightColor = new Safe<Color>(Color.Pink);

        /// <summary>
        /// Color of the drop shaodw to the right and below the button
        /// </summary>
        private Safe<Color> _shadowDarkColor = new Safe<Color>(Color.DarkRed);

        /// <summary>
        /// icon texture used for the button
        /// </summary>
        private volatile string _iconTexture = "";

        /// <summary>
        /// is the button depressed
        /// </summary>
        private volatile bool _depressed = false;
        
        /// <summary>
        /// The string on the button
        /// </summary>
        public string Text
        {
            get { return _text.Text; }
            set 
            {
                //do nothing if already set
                if (_text.Text == value) { return; }

                //set text
                _text.Text = value;
                
                //rebuild strings if we dont already have a texture with the same name
                StringTextureChanged(_text); 
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
        /// The font of the button text
        /// </summary>
        public Font TextFont
        {
            get { return _text.Font; }
            set { _text.Font = value; StringTextureChanged(_text); }
        }

        /// <summary>
        /// The alignment of the label text
        /// </summary>
        public StringAlignment TextAlignment
        {
            get { return _text.Alignment; }
            set { _text.Alignment = value; StringTextureChanged(_text); }
        }

        /// <summary>
        /// The verticle alignment of the label text
        /// </summary>
        public StringAlignment TextVerticelAlignment
        {
            get { return _text.VerticelAlignment; }
            set { _text.VerticelAlignment = value; StringTextureChanged(_text); }
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
        /// Color of the drop shaodw above and to the left of the button
        /// </summary>
        public Color ShadowLightColor
        {
            get { return _shadowLightColor.Value; }
            set { _shadowLightColor.Value = value; RebufferWindowNextFrame(); }
        }

        /// <summary>
        /// Color of the drop shaodw to the right and below the button
        /// </summary>
        public Color ShadowDarkColor
        {
            get { return _shadowDarkColor.Value; }
            set { _shadowDarkColor.Value = value; RebufferWindowNextFrame(); }
        }

        /// <summary>
        /// The icon texture used for the button.
        /// </summary>
        public string IconTexture
        {
            get { return _iconTexture; }
            set { _iconTexture = value; RebufferWindowNextFrame(); }
        }

        /// <summary>
        /// is the button depressed
        /// </summary>
        public bool Depressed
        {
            get { return _depressed; }
            set { _depressed = value; RebufferWindowNextFrame(); }
        }

        #endregion


        #region Rendering

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
            _text.Width = Width - 2;
            _text.Height = Height - 2;
            textureSheetBuilder.AddString(_text);
        }
                
        /// <summary>
        /// Called to allow this control to add itself to the window buffers.
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

            //add the shadow dark
            int shadowDarkSlot = linesBuffer.GetNextFreeSlot();
            Color shadowDarkColor = _shadowDarkColor.Value;
            if (_depressed) { shadowDarkColor = _shadowLightColor.Value; }
            linesBuffer.SetSlotValues(shadowDarkSlot, left, top, right, bottom, shadowDarkColor);
            
            //add the shadow light
            int shadowLightSlot = linesBuffer.GetNextFreeSlot();
            Color shadowLightColor = _shadowLightColor.Value;
            if (_depressed) { shadowLightColor = _shadowDarkColor.Value; }
            linesBuffer.SetSlotValues(shadowLightSlot, left, top, almostRight, almostBottom, shadowLightColor);
            
            //add the button
            int buttonSlot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(buttonSlot, almostLeft, almostTop, almostRight, almostBottom, _backColor.Value);

            //add the button icon
            if (_iconTexture != null && _iconTexture != "")
            {
                int buttonImageSlot = commonTexturesBuffer.GetNextFreeSlot();
                Texture iconTexture = commonTextures.GetTexture(_iconTexture);
                float iconLeft = left + ((Width / 2) - (iconTexture.Width / 2)) * WindowSettings.PointsPerPixelX;
                float iconTop = top - ((Height / 2) - (iconTexture.Height / 2)) * WindowSettings.PointsPerPixelY;
                float iconRight = iconLeft + iconTexture.Width * WindowSettings.PointsPerPixelX;
                float iconBottom = iconTop - iconTexture.Height * WindowSettings.PointsPerPixelY;
                commonTexturesBuffer.SetSlotValues(buttonImageSlot, iconLeft, iconTop, iconRight, iconBottom, iconTexture);
            }

            //add the text
            int stringSlot = localTexturesBuffer.GetNextFreeSlot();            
            float textBottom = bottom + 2 * WindowSettings.PointsPerPixelY;
            Texture textTexture = localTextures.GetTexture(_text.SheetTextureName);
            localTexturesBuffer.SetSlotValues(stringSlot, almostLeft, top, almostRight, textBottom, textTexture);
        }

        #endregion
    }
}
