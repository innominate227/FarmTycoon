using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace TycoonTextureTool
{
    public class FileReader
    {

        public void ReadTextures()
        {
            ReadWindowsTexturesFile();
            ReadTexturesFile();
            ReadQuartetsFile();
        }

        public void CreateIndividualImages(TextureSheet sheet, string fileName)
        {
            //read in full bitmap
            Bitmap fullMap;
            using (FileStream fs = new System.IO.FileStream(TextureTool.Instance.WorkingDirectory + fileName, System.IO.FileMode.Open))
            {
                Bitmap bmp = new Bitmap(fs);
                fullMap = (Bitmap)bmp.Clone();
            }
            
            //go though each texture
            foreach (Texture texture in TextureTool.Instance.Textures.Values)
            {
                if (texture.TextureSheet != sheet) { continue; }

                //get texture properties
                string name = texture.Name;
                int left = texture.Left;
                int top = texture.Top;
                int width = texture.Width;
                int height = texture.Height;

                //create Bitmap and save it
                Bitmap textureBitmap = new Bitmap(width, height);
                Graphics drawTexture = Graphics.FromImage(textureBitmap);
                drawTexture.DrawImage(fullMap, new Rectangle(0, 0, width, height), new Rectangle(left, top, width, height), GraphicsUnit.Pixel);
                
                drawTexture.Dispose();
                textureBitmap.Save(texture.FullFileName);
            }
        }
        
        private void ReadWindowsTexturesFile()
        {
            string texturesFile = TextureTool.Instance.WorkingDirectory + "wintextures.txt";

            //read the textures file into memory            
            StreamReader wintexturesFileReader = new StreamReader(texturesFile);
            string wintexturesFileContents = wintexturesFileReader.ReadToEnd();
            wintexturesFileReader.Close();

            //parse the textures from the textures file
            foreach (string wintextureFileLine in wintexturesFileContents.Split('\n'))
            {
                //skip blank lines and lines with comments
                if (wintextureFileLine.Trim() == "" || wintextureFileLine.Trim().StartsWith("#"))
                {
                    continue;
                }

                //split into tokens
                string[] textureTokens = wintextureFileLine.Split(new char[]{',', '='}, StringSplitOptions.RemoveEmptyEntries);
                string name = textureTokens[0].Trim();
                int left = int.Parse(textureTokens[1]);
                int top = int.Parse(textureTokens[2]);
                int width = int.Parse(textureTokens[3]);
                int height = int.Parse(textureTokens[4]);                

                string catagory = "Catagory";
                if (textureTokens.Length > 5)
                {
                    catagory = textureTokens[5].Trim();
                }

                //create Texture, and add to list
                Texture texture = new Texture();
                texture.TextureSheet = TextureSheet.Window;
                texture.Name = name;
                texture.Catagory = catagory;
                texture.Left = left;
                texture.Top = top;
                texture.Width = width;
                texture.Height = height;
                TextureTool.Instance.AddTexture(texture);
                
                
            }
        }

        private void ReadTexturesFile()
        {
            string texturesFile = TextureTool.Instance.WorkingDirectory + "textures.txt";

            //read the textures file into memory            
            StreamReader texturesFileReader = new StreamReader(texturesFile);
            string texturesFileContents = texturesFileReader.ReadToEnd();
            texturesFileReader.Close();

            //parse the textures from the textures file
            foreach (string textureFileLine in texturesFileContents.Split('\n'))
            {
                //skip blank lines and lines with comments
                if (textureFileLine.Trim() == "" || textureFileLine.Trim().StartsWith("#"))
                {
                    continue;
                }

                //split into tokens
                string[] textureTokens = textureFileLine.Split(new char[]{',', '='}, StringSplitOptions.RemoveEmptyEntries);
                string name = textureTokens[0].Trim();
                int left = int.Parse(textureTokens[1]);
                int top = int.Parse(textureTokens[2]);
                int width = int.Parse(textureTokens[3]);
                int height = int.Parse(textureTokens[4]);
                int offsetY = int.Parse(textureTokens[5]);
                int offsetX = int.Parse(textureTokens[6]);
                string catagory = textureTokens[7].Trim();
                

                //create Texture, and add to list
                Texture texture = new Texture();
                texture.TextureSheet = TextureSheet.Game;
                texture.Name = name;
                texture.Catagory = catagory;
                texture.Left = left;
                texture.Top = top;
                texture.Width = width;
                texture.Height = height;
                texture.CenterOffsetX = offsetX;
                texture.CenterOffsetY = offsetY;
                TextureTool.Instance.AddTexture(texture);
                
                
            }
        }
        
        private void ReadQuartetsFile()
        {
            string quartetsFile = TextureTool.Instance.WorkingDirectory + "quartets.txt";

            //read the quartets file into memory            
            StreamReader quartetsFileReader = new StreamReader(quartetsFile);
            string quartetsFileContents = quartetsFileReader.ReadToEnd();
            quartetsFileReader.Close();

            //parse the quartets from the quartets file
            foreach (string quartetFileLine in quartetsFileContents.Split('\n'))
            {
                //skip blank lines and lines with comments
                if (quartetFileLine.Trim() == "" || quartetFileLine.Trim().StartsWith("#"))
                {
                    continue;
                }

                //split into tokens
                string[] quartetTokens = quartetFileLine.Split(new char[] { ',', '=' }, StringSplitOptions.RemoveEmptyEntries);
                string name = quartetTokens[0].Trim();
                string north = TextureSheet.Game.ToString() + "_" + quartetTokens[1].Trim();
                string east = TextureSheet.Game.ToString() + "_" + quartetTokens[2].Trim();
                string south = TextureSheet.Game.ToString() + "_" + quartetTokens[3].Trim();
                string west = TextureSheet.Game.ToString() + "_" + quartetTokens[4].Trim();
                string catagory = "Catagory";
                if (quartetTokens.Length > 5)
                {
                    catagory = quartetTokens[5].Trim();
                }

                //create Quartet, and add to list
                Quartet quartet = new Quartet();
                quartet.Name = name;
                quartet.Catagory = catagory;
                quartet.North = TextureTool.Instance.Textures[north];
                quartet.East = TextureTool.Instance.Textures[east];
                quartet.South = TextureTool.Instance.Textures[south];
                quartet.West = TextureTool.Instance.Textures[west];
                TextureTool.Instance.AddQuartet(quartet);
            }
        }
    }
}
