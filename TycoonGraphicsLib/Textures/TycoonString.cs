using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TycoonGraphicsLib
{
    public class TycoonString
    {
        /// <summary>
        /// The uniqe texture name for the string
        /// </summary>
        private volatile string _textureName = null;
        
        /// <summary>
        /// texture name to use if one is not explcity assigned.  based of all the proeprties of the string
        /// </summary>
        private volatile string _defaultTextureName = "";

        /// <summary>
        /// The texture name used when this string was last added to a texture sheet.   
        /// 
        /// More compilcated... we decide not to add to the sheet if something in sheet has same name...
        /// but that means we dont update the sheet texture name.
        /// 
        /// can we just update it when we deicde the texture is already present in the sheet...
        /// 
        /// </summary>
        private volatile string _sheetTextureName = null;

        /// <summary>
        /// The text of the string
        /// </summary>
        private volatile string _text = "";

        /// <summary>
        /// The color of the string
        /// </summary>
        private Safe<Color> _color = new Safe<Color>(Color.White);

        /// <summary>
        /// The font of the string
        /// </summary>
        private volatile Font _font = new Font("Segoe UI", 8, FontStyle.Regular);

        /// <summary>
        /// How the string is aligned
        /// </summary>
        private volatile StringAlignment _alignment = StringAlignment.Near;

        /// <summary>
        /// How the string is aligned verticely
        /// </summary>
        private volatile StringAlignment _alignmentVerticel = StringAlignment.Near;

        /// <summary>
        /// The width of the string
        /// </summary>
        private int _width;

        /// <summary>
        /// The height of the string
        /// </summary>
        private int _height;

        /// <summary>
        /// Create a new Tycoon string setting the initial text
        /// </summary>
        /// <param name="text"></param>
        public TycoonString(string text)
        {
            _text = text;
        }
        
        /// <summary>
        /// The uniqe texture name for the string,
        /// If not explicitly set then defaults to a string based on all the properties of this string. (Text + "," + Width + "," ...).
        /// That way two strings will all the same properties will have the same texture name and can share the same texture
        /// </summary>
        public string TextureName
        {
            get 
            {
                if (_textureName == null)
                {
                    return _defaultTextureName;                    
                }
                else
                {
                    return _textureName; 
                }
            }
            set { _textureName = value; }
        }
        
        /// <summary>
        /// The texture name used when this string was last added to a texture sheet, or when it found that there was already a texture in the texture sheet with the correct name.
        /// This is needed because we allow chaning other properties of the string while the window is being drawn, and the TextureName will change as the properties do.
        /// </summary>
        public string SheetTextureName
        {
            get { return _sheetTextureName; }
            set { _sheetTextureName = value; }
        }
        
        /// <summary>
        /// The text of the string
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; RedetermineDefaultTextureName(); }
        }


        /// <summary>
        /// The color of the string
        /// </summary>
        public Color Color
        {
            get { return _color.Value; }
            set { _color.Value = value; RedetermineDefaultTextureName(); }
        }

        /// <summary>
        /// The font of the string
        /// </summary>
        public Font Font
        {
            get { return _font; }
            set { _font = value; RedetermineDefaultTextureName(); }
        }

        /// <summary>
        /// How the string is aligned verticely
        /// </summary>
        public StringAlignment VerticelAlignment
        {
            get { return _alignmentVerticel; }
            set { _alignmentVerticel = value; RedetermineDefaultTextureName(); }
        }
        
        /// <summary>
        /// How the string is aligned
        /// </summary>
        public StringAlignment Alignment
        {
            get { return _alignment; }
            set { _alignment = value; RedetermineDefaultTextureName(); }
        }
        
        /// <summary>
        /// The width of the string
        /// </summary>
        public int Width
        {
            get { return _width; }
            set { _width = value; RedetermineDefaultTextureName(); }
        }

        /// <summary>
        /// The height of the string
        /// </summary>
        public int Height
        {
            get { return _height; }
            set { _height = value; RedetermineDefaultTextureName(); }
        }


        private void RedetermineDefaultTextureName()
        {
            _defaultTextureName = _text + "," + _width.ToString() + "," + _height.ToString() + "," + _alignment.ToString() + "," + _alignmentVerticel.ToString() + "," + _font.ToString() + "," + _color.ToString();
        }

    }
}
