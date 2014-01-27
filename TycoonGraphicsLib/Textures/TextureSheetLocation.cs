using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TycoonGraphicsLib
{

    /// <summary>
    /// Info about where a texture is in the texture sheet, and the name of the texture.
    /// Used when creating a TextureSheet object
    /// </summary>
    internal class TextureSheetLocation
    {
        /// <summary>
        /// the name of the texture
        /// </summary>
        private string _name;

        /// <summary>
        /// the top position of texture in the texture sheet in pixels.
        /// </summary>
        private int _top;

        /// <summary>
        /// the left position of texture in the texture sheet in pixels.
        /// </summary>
        private int _left;

        /// <summary>
        /// the width of the texture in the texture sheet in pixels.
        /// </summary>
        private int _width;

        /// <summary>
        /// the height of the texture in the texture sheet in pixels.
        /// </summary>
        private int _height;


        /// <summary>
        /// Set all values for the TextureSheetLocation by copying the value of another
        /// </summary>
        public void SetValues(TextureSheetLocation other)
        {
            _name = other.Name;
            _left = other.Left;
            _top = other.Top;
            _width = other.Width;
            _height = other.Height;
        }
        
        /// <summary>
        /// Set all values for the TextureSheetLocation
        /// </summary>
        public void SetValues(string name, int top, int left, int width, int height)
        {
            _name = name;
            _left = left;
            _top = top;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Parse TextureSheetLocation from a line in the texture file.
        /// </summary>
        /// <param name="textureFileLine"></param>
        public void ParseFromTexturesFileLine(string textureFileLine)
        {            
            _name = textureFileLine.Split('=')[0].Trim();            
            _left = int.Parse(textureFileLine.Split('=')[1].Split(',')[0]);
            _top = int.Parse(textureFileLine.Split('=')[1].Split(',')[1]);
            _width = int.Parse(textureFileLine.Split('=')[1].Split(',')[2]);
            _height = int.Parse(textureFileLine.Split('=')[1].Split(',')[3]);
        }



        /// <summary>
        /// the name of the texture
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// the top position of texture in the texture sheet in pixels.
        /// </summary>
        public int Top
        {
            get { return _top; }
            set { _top = value; }
        }

        /// <summary>
        /// the left position of texture in the texture sheet in pixels.
        /// </summary>
        public int Left
        {
            get { return _left; }
            set { _left = value; }
        }

        /// <summary>
        /// the width of the texture in the texture sheet in pixels.
        /// </summary>
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>
        /// the height of the texture in the texture sheet in pixels.
        /// </summary>
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

    }
}
