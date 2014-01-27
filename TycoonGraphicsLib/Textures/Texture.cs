using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TycoonGraphicsLib
{

    /// <summary>
    /// A texture in the texture sheet
    /// </summary>
    internal class Texture
    {
        /// <summary>
        /// the name of the texture
        /// </summary>
        private string _name;

        /// <summary>
        /// the texture sheet the texture is in
        /// </summary>
        private TextureSheet _sheet;

        /// <summary>
        /// the top position of texture in the texture sheet. (where 0 is top 1 is bottom)
        /// </summary>
        private float _top;

        /// <summary>
        /// the left position of texture in the texture sheet. (where 0 is left 1 is right)
        /// </summary>
        private float _left;

        /// <summary>
        /// the bottom position of texture in the texture sheet. (where 0 is top 1 is bottom)
        /// </summary>
        private float _bottom;

        /// <summary>
        /// the right position of texture in the texture sheet. (where 0 is left 1 is right)
        /// </summary>
        private float _right;
        
        /// <summary>
        /// Height of the texture in pixels
        /// </summary>
        private int _height;

        /// <summary>
        /// Width of the texture in pixels
        /// </summary>
        private int _width;


        /// <summary>
        /// Create a new texture object
        /// </summary>
        public Texture(string name, TextureSheet sheet, float left, float top, float right, float bottom, int width, int height)
        {
            _name = name;
            _sheet = sheet;
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
            _width = width;
            _height = height;
        }


        /// <summary>
        /// the name of the texture
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// the texture sheet the texture is in
        /// </summary>
        public TextureSheet Sheet
        {
            get { return _sheet; }
        }

        /// <summary>
        /// the top position of texture in the texture sheet. In texture sheet units. (where 0 is top 1 is bottom)
        /// </summary>
        public float Top
        {
            get { return _top; }
        }

        /// <summary>
        /// the left position of texture in the texture sheet. In texture sheet units. (where 0 is left 1 is right)
        /// </summary>
        public float Left
        {
            get { return _left; }
        }

        /// <summary>
        /// the bottom position of texture in the texture sheet. In texture sheet units. (where 0 is top 1 is bottom)
        /// </summary>
        public float Bottom
        {
            get { return _bottom; }
        }

        /// <summary>
        /// the right position of texture in the texture sheet. In texture sheet units. (where 0 is left 1 is right)
        /// </summary>
        public float Right
        {
            get { return _right; }
        }
        
        /// <summary>
        /// Height of the texture in pixels
        /// </summary>
        public int Height
        {
            get { return _height; }
        }

        /// <summary>
        /// Width of the texture in pixels
        /// </summary>
        public int Width
        {
            get { return _width; }
        }

    }
}

