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
    /// <summary>
    /// Builds a texture sheet consisting of strings and images
    /// </summary>
    internal class TextureSheetBuilder
    {
        /// <summary>
        /// All the strings added to the builder keyed on the strings texture name
        /// </summary>
        private Dictionary<string, TycoonString> _strings = new Dictionary<string, TycoonString>();

        /// <summary>
        /// All bitmaps added to the builder keyed on the texture name
        /// </summary>
        private Dictionary<string, Bitmap> _icons = new Dictionary<string, Bitmap>();

        /// <summary>
        /// Add a string to be built by the texture sheet builder.
        /// If a TyconnString with the same texturename has already been added this does nothing.
        /// </summary>        
        public void AddString(TycoonString tycoonString)
        {
            //get the texture name for the string right now
            string sheetTextureName = tycoonString.TextureName;

            //this texture name will be the texture name that gets used for the texture sheet
            tycoonString.SheetTextureName = sheetTextureName;

            if (_strings.ContainsKey(sheetTextureName) == false)
            {
                _strings.Add(sheetTextureName, tycoonString);
            }
        }

        /// <summary>
        /// Add an icon to the texture sheet builder
        /// </summary>
        public void AddIcon(string textureName, Bitmap icon)
        {
            _icons.Add(textureName, icon);
        }
       
		
        /// <summary>
        /// Create a texture sheet containing all the strings and icons added
        /// </summary>
        public TextureSheet CreateTextureSheet()
        {
            //list of texture sheet info objects that tell where the texture is in the sheet
            List<TextureSheetLocation> textureInfoList = new List<TextureSheetLocation>();

            //maximum width for the texture
            const int MAX_WIDTH = 1024;

            //next x and y to place a texture
            int nextX = 0;
            int nextY = 0;
            
            //ma height of any texture on the current line
            int maxHeightThisLine = 0;


            //find a location for all the icon textures
            foreach (string iconTextureName in _icons.Keys)
            {
                Bitmap icon = _icons[iconTextureName];

                //try and put it on the same line if there is room, otherwise go down a line
                if (nextX + icon.Width > MAX_WIDTH)
                {
                    nextX = 0;
                    nextY += maxHeightThisLine;
                }

                //create a TextureSheetTextureInfo for the string
                TextureSheetLocation textureInfo = new TextureSheetLocation();
                textureInfo.Name = iconTextureName;
                textureInfo.Left = nextX;
                textureInfo.Top = nextY;
                textureInfo.Width = icon.Width;
                textureInfo.Height = icon.Height;
                textureInfoList.Add(textureInfo);

                //check if this is the max height for the line
                if (icon.Height > maxHeightThisLine)
                {
                    maxHeightThisLine = icon.Height;
                }

                //adjust next x
                nextX += textureInfo.Width;
            }


            //find a location for all the string textures
            foreach (TycoonString tycoonString in _strings.Values)
            {
                //try and put it on the same line if there is room, otherwise go down a line
                if (nextX + tycoonString.Width > MAX_WIDTH)
                {
                    nextX = 0;
                    nextY += maxHeightThisLine;
                }

                //create a TextureSheetTextureInfo for the string
                TextureSheetLocation textureInfo = new TextureSheetLocation();
                textureInfo.Name = tycoonString.SheetTextureName;
                textureInfo.Left = nextX;
                textureInfo.Top = nextY;
                textureInfo.Width = tycoonString.Width;
                textureInfo.Height = tycoonString.Height;
                textureInfoList.Add(textureInfo);
                
                //check if this is the max height for the line
                if (tycoonString.Height > maxHeightThisLine)
                {
                    maxHeightThisLine = tycoonString.Height;
                }

                //adjust next x
                nextX += textureInfo.Width;
            }

            //find how big of a bitmap we need to create for the texture sheet
            int bitmapWidth = 0;
            int bitmapHeight = 0;
            foreach (TextureSheetLocation textureInfo in textureInfoList)
            {
                if (bitmapWidth < textureInfo.Left + textureInfo.Width)
                {
                    bitmapWidth = textureInfo.Left + textureInfo.Width;
                }
                if (bitmapHeight < textureInfo.Top + textureInfo.Height)
                {
                    bitmapHeight = textureInfo.Top + textureInfo.Height;
                }
            }

            //create the texture image
            Bitmap textureSheetImage = new Bitmap(bitmapWidth, bitmapHeight);

            //create drawer for the bitmap 
            Graphics graphics = Graphics.FromImage(textureSheetImage);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            graphics.Clear(Color.Transparent);
            

            //draw each icon/string to the bitmap            
            foreach (TextureSheetLocation textureInfo in textureInfoList)
            {
                if (_icons.ContainsKey(textureInfo.Name))
                {
                    //draw the icon to the image
                    Bitmap icon = _icons[textureInfo.Name];
                    graphics.DrawImage(icon, textureInfo.Left, textureInfo.Top);
                }
                else
                {
                    //get settings for the string to draw
                    string text = _strings[textureInfo.Name].Text;
                    SolidBrush color = new SolidBrush(_strings[textureInfo.Name].Color);
                    Font font = _strings[textureInfo.Name].Font;
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = _strings[textureInfo.Name].Alignment;
                    stringFormat.LineAlignment = _strings[textureInfo.Name].VerticelAlignment;

                    //draw the string (double drawing makes it easier to read, i dont understand why)
                    graphics.DrawString(text, font, color, new RectangleF(textureInfo.Left, textureInfo.Top, textureInfo.Width, textureInfo.Height), stringFormat);
                    graphics.DrawString(text, font, color, new RectangleF(textureInfo.Left, textureInfo.Top, textureInfo.Width, textureInfo.Height), stringFormat);
                }
            }
            graphics.Dispose();


            //StreamWriter temp = new StreamWriter("C:\\Users\\Innominate\\Temp\\wintextures.txt");
            //foreach (TextureSheetLocation textureInfo in textureInfoList)
            //{
            //    temp.WriteLine(textureInfo.Name+ "=" + textureInfo.Left.ToString()  + "," + textureInfo.Top.ToString()   + "," + textureInfo.Width.ToString()  + "," + textureInfo.Height.ToString() + ",Required");
            //}
            //temp.Close();
            //textureSheetImage.Save("C:\\Users\\Innominate\\Temp\\wintexturemap.bmp");


            //create a texture sheet and return it
            return new TextureSheet(textureSheetImage, textureInfoList);
        }

    }
}
