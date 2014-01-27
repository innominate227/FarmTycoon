using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TycoonTextureTool
{
    public class Texture
    {

        /// <summary>
        /// The texture sheet this texture belongs in
        /// </summary>
        public TextureSheet TextureSheet
        {
            get;
            set;
        }




        /// <summary>
        /// Center offset for the texture in the X direction
        /// </summary>
        public int CenterOffsetX
        {
            get;
            set;
        }

        /// <summary>
        /// Center offset for the texture in the Y direction
        /// </summary>
        public int CenterOffsetY
        {
            get;
            set;
        }
        
        /// <summary>
        /// Catagory string
        /// </summary>
        public string Catagory
        {
            get;
            set;
        }

        /// <summary>
        /// Name string
        /// </summary>
        public string FullName
        {
            get { return TextureSheet.ToString() + "_" + Name; }            
        }

        /// <summary>
        /// Name string
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Full path to the image file
        /// </summary>
        public string FullFileName
        {
            get
            {
                return TextureTool.Instance.TexturesDirectory + FullName + ".bmp";                
            }
        }

        /// <summary>
        /// X position in texture map
        /// </summary>
        public int Left
        {
            get;
            set;
        }

        /// <summary>
        /// Y position in texture map
        /// </summary>
        public int Top
        {
            get;
            set;
        }

        /// <summary>
        /// Width in texture map
        /// </summary>
        public int Width
        {
            get;
            set;
        }

        /// <summary>
        /// Height in texture map
        /// </summary>
        public int Height
        {
            get;
            set;
        }



        public Texture Clone()
        {
            Texture clone = new Texture();
            clone.Catagory = this.Catagory;
            clone.CenterOffsetX = this.CenterOffsetX;
            clone.CenterOffsetY = this.CenterOffsetY;
            clone.Height = this.Height;
            clone.Width = this.Width;
            clone.TextureSheet = this.TextureSheet;

            //find a unique name for the copy
            int copyNum = 1;
            string name = this.Name + " Copy" + copyNum.ToString();
            while (TextureTool.Instance.Textures.ContainsKey(name))
            {
                name = this.Name + " Copy" + copyNum.ToString();
                copyNum++;
            }

            clone.Name = name;

            //copy the bitmap file for the texture
            File.Copy(this.FullFileName, clone.FullFileName);

            //give the clone to the main data strcuture
            TextureTool.Instance.AddTexture(clone);

            return clone;
        }

    }
}
