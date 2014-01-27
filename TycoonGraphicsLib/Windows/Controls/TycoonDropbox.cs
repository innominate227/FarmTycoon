
using System;
using System.Drawing;
using System.Collections.Generic;

namespace TycoonGraphicsLib
{


    public class TycoonDropbox : TycoonControl
    {

        /// <summary>
        /// Event raised when the text of the control is changed
        /// </summary>
        public event Action<TycoonControl> TextChanged;

        #region GUI Properties

        /// <summary>
        /// String for the selected by the drop box
        /// </summary>
        private TycoonString _text = new TycoonString("Button");

        /// <summary>
        /// Main color of the dropbox
        /// </summary>
        private Safe<Color> _backColor = new Safe<Color>(Color.Red);

        /// <summary>
        /// Color of the light drop shadow
        /// </summary>
        private Safe<Color> _shadowLightColor = new Safe<Color>(Color.Pink);

        /// <summary>
        /// Color of the dark drop shaodw
        /// </summary>
        private Safe<Color> _shadowDarkColor = new Safe<Color>(Color.DarkRed);
        
        /// <summary>
        /// Color of the text in the dropped portion of the dropbox
        /// </summary>
        private Color _dropTextColor = Color.White;

        /// <summary>
        /// Color of the item selected in the drop box
        /// </summary>
        private Color _selectionColor = Color.Blue;

        /// <summary>
        /// The texture for the drop arrow
        /// </summary>
        private volatile string _dropArrowTexture = "arrowdown";

        /// <summary>
        /// The height of the drop down region
        /// </summary>
        private int _dropHeight = 50;

        /// <summary>
        /// List of drop box items
        /// </summary>
        private List<string> _items = new List<string>();


        /// <summary>
        /// String for the selected by the drop box
        /// </summary>
        public string Text
        {
            get { return _text.Text; }
            set 
            {
                if (_text.Text == value) { return; }

                _text.Text = value;
                StringTextureChanged(_text);
                if (TextChanged != null)
                {
                    TextChanged(this);
                }
            }
        }

        /// <summary>
        /// Color for text shown in the dropbox
        /// </summary>
        public Color TextColor
        {
            get { return _text.Color; }
            set { _text.Color = value; StringTextureChanged(_text); }
        }

        /// <summary>
        /// Font for text shown in the dropbox
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
        /// Main color of the dropbox
        /// </summary>
        public Color BackColor
        {
            get { return _backColor.Value; }
            set { _backColor.Value = value; RebufferWindowNextFrame(); }
        }

        /// <summary>
        /// Color of the light drop shadow
        /// </summary>
        public Color ShadowLightColor
        {
            get { return _shadowLightColor.Value; }
            set { _shadowLightColor.Value = value; RebufferWindowNextFrame(); }
        }

        /// <summary>
        /// Color of the dark drop shaodw
        /// </summary>
        public Color ShadowDarkColor
        {
            get { return _shadowDarkColor.Value; }
            set { _shadowDarkColor.Value = value; RebufferWindowNextFrame(); }
        }

        /// <summary>
        /// Color of the item selected in the drop box
        /// </summary>
        public Color SelectionColor
        {
            get { return _selectionColor; }
            set { _selectionColor = value; }
        }


        /// <summary>
        /// Color of the text in the dropped portion of the dropbox
        /// </summary>
        public Color DropTextColor
        {
            get { return _dropTextColor; }
            set { _dropTextColor = value; }
        }
        
        /// <summary>
        /// The texture for the drop arrow
        /// </summary>
        public string DropArrowTexture
        {
            get { return _dropArrowTexture; }
            set { _dropArrowTexture = value; RebufferWindowNextFrame(); }
        }
        
        /// <summary>
        /// The height of the drop down region
        /// </summary>
        public int DropHeight
        {
            get { return _dropHeight; }
            set { _dropHeight = value; }
        }
                
        /// <summary>
        /// List of drop box items
        /// </summary>
        public List<string> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        #endregion

        #region Mouse Handeling (Droping the dropbox)

        internal override void LocationClicked(int x, int y)
        {
            //check if the drop button was clicked
            if (x > Width - 17)
            {
                DropDown();
            }
            else
            {
                base.LocationClicked(x, y);
            }
        }


        public void DropDown()
        {
            //get the position of the dropbox in absolute screen units
            int top, left;
            GetPositionAbsolute(out left, out top);

            //create the drop window
            TycoonWindow dropWindow = new TycoonWindow();
            dropWindow.TitleText = "Dropbox";
            dropWindow.Tag = "Dropbox";
            dropWindow.TitleBar = false;
            dropWindow.BackColor = _backColor.Value;
            dropWindow.BorderColor = _shadowLightColor.Value;
            dropWindow.Width = Width;
            dropWindow.Left = left;
            dropWindow.Top = top + Height - 1;
            dropWindow.ScrollLightColor = _shadowLightColor.Value;
            dropWindow.ScrollDarkColor = _shadowDarkColor.Value;

            //determine the height needed for the drop region, and if it should have a scroll bar
            int neededHeight = _items.Count * 12 + 2;
            if (neededHeight < _dropHeight)
            {
                dropWindow.Height = neededHeight;
                dropWindow.Scrollable = false;
            }
            else
            {
                dropWindow.Height = _dropHeight;
                dropWindow.Scrollable = true;
            }

            //create a label for each item you can choose from in the drop box
            int labelTop = 1;
            foreach (string item in _items)
            {
                TycoonLabel choice = new TycoonLabel();
                choice.Top = labelTop;
                choice.Left = 1;

                //width of choice depends on if there is a scroll bar or not
                if (dropWindow.Scrollable)
                {
                    choice.Width = dropWindow.Width - 14;
                }
                else
                {
                    choice.Width = dropWindow.Width - 2;
                }
                choice.Height = 12;
                choice.Text = item;
                choice.TextColor = _dropTextColor;
                choice.TextFont = _text.Font;
                //if its the current item selected set it to be the selected color
                if (item == _text.Text)
                {
                    choice.BorderColor = _selectionColor;
                    choice.BackColor = _selectionColor;
                }
                else
                {
                    choice.BorderColor = _backColor.Value;
                    choice.BackColor = _backColor.Value;
                }

                //when an item is hovered over set its color to the selected color, and the color of all others to normal color
                choice.MouseOver += new Action<TycoonControl>(delegate
                {
                    foreach (TycoonControl otherChoice in choice.Parent.Children)
                    {
                        ((TycoonLabel)otherChoice).BorderColor = _backColor.Value;
                        ((TycoonLabel)otherChoice).BackColor = _backColor.Value;
                    }
                    choice.BorderColor = _selectionColor;
                    choice.BackColor = _selectionColor;
                });

                //when an item is clicked set it as the selected text, and hide the dropbox window
                choice.Clicked += new Action<TycoonControl>(delegate
                {
                    _parentWindow.WindowManager.DropboxWindow = null;
                    this.Text = choice.Text;
                });

                //add the dropbox choice to the window
                dropWindow.AddChild(choice);
                labelTop += 12;
            }

            //tell the window manager what the current dropbox window is
            _parentWindow.WindowManager.DropboxWindow = dropWindow;
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
        /// <param name="textureSheetBuilder"></param>
        internal override void AddLocalTextures(TextureSheetBuilder textureSheetBuilder)
        {
            _text.Width = Width - 15;
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

            //get the position of the dropbox
            float top, left, bottom, right;
            GetPositionPoints(out left, out top, out right, out bottom);

            //positions one pixel from the left,top,right,bottom
            float almostLeft = left + 1 * WindowSettings.PointsPerPixelX;
            float almostRight = right - 1 * WindowSettings.PointsPerPixelX;
            float almostTop = top - 1 * WindowSettings.PointsPerPixelY;
            float almostBottom = bottom + 1 * WindowSettings.PointsPerPixelY;

            //add the dark shadow for the main box
            int shadowDarkSlot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(shadowDarkSlot, left, top, right, bottom, _shadowDarkColor.Value);
            
            //add the light shadow for the main box
            int shadowLightSlot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(shadowLightSlot, almostLeft, almostTop, right, bottom, _shadowLightColor.Value);
            
            //add the main text area
            int buttonSlot = linesBuffer.GetNextFreeSlot();
            linesBuffer.SetSlotValues(buttonSlot, almostLeft, almostTop, almostRight, almostBottom, _backColor.Value);
                        
            //add the drop button
            int closeSlot = commonTexturesBuffer.GetNextFreeSlot();            
            float dropLeft = right - 13 * WindowSettings.PointsPerPixelX;
            float dropRight = right - 2 * WindowSettings.PointsPerPixelX;
            float dropTop = top - 2 * WindowSettings.PointsPerPixelY;
            float dropBottom = top - 12 * WindowSettings.PointsPerPixelY;
            Texture closeTexture = commonTextures.GetTexture(_dropArrowTexture);
            commonTexturesBuffer.SetSlotValues(closeSlot, dropLeft, dropTop, dropRight, dropBottom, closeTexture);
            
            //add the text
            int stringSlot = localTexturesBuffer.GetNextFreeSlot();                           
            float textBottom = bottom + 2 * WindowSettings.PointsPerPixelY;
            float textRight = right - 14 * WindowSettings.PointsPerPixelX;
            Texture textTexture = localTextures.GetTexture(_text.SheetTextureName);
            localTexturesBuffer.SetSlotValues(stringSlot, almostLeft, top, textRight, textBottom, textTexture);
        }

        #endregion
    }
}
