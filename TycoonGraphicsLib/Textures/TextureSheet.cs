using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace TycoonGraphicsLib
{
    internal class TextureSheet
    {
        /// <summary>
        /// Dictionary mapping texture name to the textures that make up the texture sheet
        /// </summary>
        private Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();
        
        /// <summary>
        /// Open GL id for the texture sheet
        /// </summary>
        private uint _textureSheetId;

        /// <summary>
        /// Image of the texture sheet 
        /// </summary>
        private Bitmap _textureSheetImage;
        
        /// <summary>
        /// Open GL id for the texture sheet
        /// </summary>
        public uint TextureSheetId
        {
            get { return _textureSheetId; }
        }

        /// <summary>
        /// Image of the texture sheet 
        /// </summary>
        public Bitmap TextureSheetImage
        {
            get { return _textureSheetImage; }
        }
                
        /// <summary>
        /// create a texture sheet by passing a texture image, and a list of textures
        /// </summary>
        /// <param name="textureSheetImage"></param>
        /// <param name="textureSheetTextures"></param>
        public TextureSheet(Bitmap textureSheetImage, List<TextureSheetLocation> textureSheetTextures)
        {
            CreateTextureSheet(textureSheetImage, textureSheetTextures);
        }
        
        /// <summary>
        /// Create the texture sheet, called by all constructors
        /// </summary>
        private void CreateTextureSheet(Bitmap textureSheetImage, List<TextureSheetLocation> textureSheetTextures)
        {
            //generate an id for the texture map                        
            _textureSheetId = (uint)GL.GenTexture();
            
            //create the texture in open gl
            _textureSheetImage = textureSheetImage;
            GL.BindTexture(TextureTarget.Texture2D, _textureSheetId);
            BitmapData data = _textureSheetImage.LockBits(new Rectangle(0, 0, _textureSheetImage.Width, _textureSheetImage.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            _textureSheetImage.UnlockBits(data);            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            
            //create Texure objects for each texture in the texture sheet
            foreach(TextureSheetLocation textureInfo in textureSheetTextures)
            {            
                //calculate the left, top, bottom, and right from 0.0 to 1.0 where 
                //0.0 is left/top    and 1.0 is bottom/right
                float left = textureInfo.Left / (float)_textureSheetImage.Width;
                float top = textureInfo.Top / (float)_textureSheetImage.Height;
                float right = left + textureInfo.Width / (float)_textureSheetImage.Width;
                float bottom = top + textureInfo.Height / (float)_textureSheetImage.Height;
                
                //create a tile texture
                Texture texture = new Texture(textureInfo.Name, this, left, top, right, bottom, textureInfo.Width, textureInfo.Height);
                _textures.Add(textureInfo.Name, texture);                
            }
        }

        /// <summary>
        /// Free resources used by the texture sheet
        /// </summary>
        public void Delete()
        {
            GL.DeleteTexture(_textureSheetId);
        }


        /// <summary>
        /// Get a texture by name
        /// </summary>
        public Texture GetTexture(string name)
        {
            if (_textures.ContainsKey(name) == false) { return _textures["bad"]; }
            return _textures[name];
        }

        /// <summary>
        /// Return if the texture sheet has a texture with the name passed
        /// </summary>
        public bool HasTexture(string name)
        {
            return _textures.ContainsKey(name);
        }
        
    }
}
