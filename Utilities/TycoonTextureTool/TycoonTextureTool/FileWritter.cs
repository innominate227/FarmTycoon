using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using Packing;
using System.Diagnostics;
using Packers;
using System.Windows.Forms;

namespace TycoonTextureTool
{
    public delegate void ProgressCallbackHandler(int progress);

    public class FileWriter
    {
        public void WriteWindowsTexturesFile()
        {
            string texturesFile = TextureTool.Instance.WorkingDirectory + "wintextures.txt";

            //first make a copy of old textures.txt
            File.Copy(texturesFile, texturesFile + ".bak");

            //write the file
            StreamWriter wintexturesFileWriter = new StreamWriter(texturesFile);
            foreach (Texture wintexture in TextureTool.Instance.Textures.Values)
            {
                if (wintexture.TextureSheet != TextureSheet.Window) { continue; }

                wintexturesFileWriter.Write(wintexture.Name + "=");
                wintexturesFileWriter.Write(wintexture.Left + ",");
                wintexturesFileWriter.Write(wintexture.Top + ",");
                wintexturesFileWriter.Write(wintexture.Width + ",");
                wintexturesFileWriter.Write(wintexture.Height + ",");
                wintexturesFileWriter.Write(wintexture.Catagory);
                wintexturesFileWriter.WriteLine();
            }
            wintexturesFileWriter.Close();

            //delete backup
            File.Delete(texturesFile + ".bak");
        }

        public void WriteTexturesFile()
        {
            string texturesFile = TextureTool.Instance.WorkingDirectory + "textures.txt";

            //first make a copy of old textures.txt
            File.Copy(texturesFile, texturesFile + ".bak", true);
            
            //write the file
            StreamWriter texturesFileWriter = new StreamWriter(texturesFile);
            foreach (Texture texture in TextureTool.Instance.Textures.Values)
            {
                if (texture.TextureSheet != TextureSheet.Game) { continue; }

                texturesFileWriter.Write(texture.Name + "=");
                texturesFileWriter.Write(texture.Left + ",");
                texturesFileWriter.Write(texture.Top + ",");
                texturesFileWriter.Write(texture.Width + ",");
                texturesFileWriter.Write(texture.Height + ",");                
                texturesFileWriter.Write(texture.CenterOffsetY + ",");
                texturesFileWriter.Write(texture.CenterOffsetX + ",");
                texturesFileWriter.Write(texture.Catagory);
                texturesFileWriter.WriteLine();                
            }
            texturesFileWriter.Close();
            
            //delete backup
            File.Delete(texturesFile + ".bak");
        }

        public void WriteQuartetsFile()
        {
            string quartetsFile = TextureTool.Instance.WorkingDirectory + "quartets.txt";

            //first make a copy of old quartets.txt
            File.Copy(quartetsFile, quartetsFile + ".bak");

            //write the file
            StreamWriter quartetsFileWriter = new StreamWriter(quartetsFile);
            foreach (Quartet quartet in TextureTool.Instance.Quartets.Values)
            {
                quartetsFileWriter.Write(quartet.Name + "=");
                quartetsFileWriter.Write(quartet.North.Name + ",");
                quartetsFileWriter.Write(quartet.East.Name + ",");
                quartetsFileWriter.Write(quartet.South.Name + ",");
                quartetsFileWriter.Write(quartet.West.Name + ",");
                quartetsFileWriter.Write(quartet.Catagory);
                quartetsFileWriter.WriteLine();
            }
            quartetsFileWriter.Close();

            //delete backup
            File.Delete(quartetsFile + ".bak");
        }
                
        public void CreateTextureMap(ProgressCallbackHandler progressCallback, bool quick, TextureSheet sheet, string fileName)
        {
            //read all texture bitmaps into memory (also set widhts and heights)
            Dictionary<Texture, Bitmap> textureBitmaps = new Dictionary<Texture, Bitmap>();
            int totalTextures = 0;
            foreach (Texture texture in TextureTool.Instance.Textures.Values)
            {
                //this is not the texture map we are creating right now
                if (texture.TextureSheet != sheet) { continue; }

                totalTextures++;

                //get the image                                
                Bitmap imageFile = new Bitmap(texture.FullFileName);                
                Bitmap image = new Bitmap(imageFile.Width, imageFile.Height);

                //draw the image          
                Graphics drawer = Graphics.FromImage(image);
                drawer.DrawImage(imageFile, 0, 0);
                drawer.Dispose();
                                    
                //keep the texture bitmap
                textureBitmaps.Add(texture, image);
                texture.Width = image.Width;
                texture.Height = image.Height;                         
            }

            //we will make multiple submaps of a certain size            
            int subMapSize = 1024;
            List<Bitmap> subMaps = new List<Bitmap>();
            Dictionary<Bitmap, List<Texture>> subMapTextures = new Dictionary<Bitmap, List<Texture>>();
            Dictionary<Texture, Bitmap> texturesSubMaps = new Dictionary<Texture, Bitmap>();

            //create the first submap
            Bitmap currentSubMap = new Bitmap(subMapSize, subMapSize);
            subMapTextures.Add(currentSubMap, new List<Texture>());
            Graphics currentSubMapDrawer = Graphics.FromImage(currentSubMap);
            RectanglePacker currentPacker = GetPacker(subMapSize);
            
            //keep track of progress
            int textureOn = 0;

            //go though each texture by group
            foreach (string catagory in TextureTool.Instance.TextureCatagories.Keys)
            {
                //sort textures in the group by height (tallest first)
                List<Texture> sortedTextures = new List<Texture>(TextureTool.Instance.TextureCatagories[catagory]);
                sortedTextures.Sort(new Comparison<Texture>(delegate(Texture t1, Texture t2)
                {
                    return -1 * t1.Height.CompareTo(t2.Height);
                }));

                foreach (Texture texture in sortedTextures)
                {
                    //make sure the texture is int he correct sheet
                    if (texture.TextureSheet != sheet) { continue; }
                    
                    //report progress
                    textureOn++;
                    if (progressCallback != null)
                    {
                        progressCallback((int)((double)textureOn / (double)totalTextures * 100.0));
                    }

                    //first see if the image is already a subimage of an exsisting Map
                    bool foundExsisting = false;
                    if (quick == false)
                    {
                        foreach (Texture alreadyPlacedTexture in texturesSubMaps.Keys)
                        {
                            //ignore unless same width (for speed, for now)
                            if (alreadyPlacedTexture.Width != texture.Width || alreadyPlacedTexture.Catagory != texture.Catagory)
                            {
                                continue;
                            }

                            ImageChecker checkForSubImages = new ImageChecker(textureBitmaps[alreadyPlacedTexture], textureBitmaps[texture]);
                            Point subpoint = checkForSubImages.bigContainsSmall();
                            checkForSubImages.big_image.Unlock();
                            checkForSubImages.small_image.Unlock();
                            if (subpoint != ImageChecker.CHECKFAILED)
                            {
                                //set the texture to the point where the subimage already exists
                                Bitmap placedTextureSubmap = texturesSubMaps[alreadyPlacedTexture];
                                subMapTextures[placedTextureSubmap].Add(texture);
                                texturesSubMaps.Add(texture, placedTextureSubmap);
                                texture.Left = alreadyPlacedTexture.Left + subpoint.X;
                                texture.Top = alreadyPlacedTexture.Top + subpoint.Y;
                                foundExsisting = true;
                                break;
                            }
                        }
                    }

                    //the image is already in out tileset we dont need to find a spot for it
                    if (foundExsisting)
                    {
                        continue;
                    }


                    //try and pack the texture
                    Point packPoint;
                    bool didPack = currentPacker.TryPack(texture.Width + 2, texture.Height + 2, out packPoint);

                    //if it didnt pack, make a new subMap
                    if (didPack == false)
                    {
                        if (sheet == TextureSheet.Window)
                        {
                            MessageBox.Show("Warning: Window texture sheet greater than " + subMapSize.ToString());
                        }

                        //put current submap into list and get a new one
                        currentSubMapDrawer.Dispose();
                        subMaps.Add(currentSubMap);
                        currentSubMap = new Bitmap(subMapSize, subMapSize);
                        subMapTextures.Add(currentSubMap, new List<Texture>());
                        currentSubMapDrawer = Graphics.FromImage(currentSubMap);
                        currentPacker = GetPacker(subMapSize);

                        //we should be able to pack now
                        didPack = currentPacker.TryPack(texture.Width + 2, texture.Height + 2, out packPoint);
                        if (didPack == false)
                        {
                            MessageBox.Show("Warning: There is a texture over " + subMapSize.ToString());
                        }
                    }

                    packPoint.X += 1;
                    packPoint.Y += 1;

                    //add the bitmap to the submap, and tell the texture where it went
                    currentSubMapDrawer.DrawImage(textureBitmaps[texture], packPoint);
                    subMapTextures[currentSubMap].Add(texture);
                    texturesSubMaps.Add(texture, currentSubMap);
                    texture.Left = packPoint.X;
                    texture.Top = packPoint.Y;

                }
            }

            //add final submap to the list
            currentSubMapDrawer.Dispose();
            subMaps.Add(currentSubMap);

            //now put all the subMaps into a big map
            //first determine how many submaps width and tall we need to be
            int subMapCount = subMaps.Count;
            int subMapsWide = 1;
            while (subMapsWide * subMapsWide < subMapCount)
            {
                subMapsWide++;
            }

            //create the full map
            Bitmap fullMap = new Bitmap(subMapsWide * subMapSize, subMapsWide * subMapSize);
            Graphics fullMapDrawer = Graphics.FromImage(fullMap);

            //place each submap into the full Map
            int subMapX = 0;
            int subMapY = 0;
            foreach (Bitmap subMap in subMaps)
            {
                //determine offsets in pixels
                int xOffset = subMapX * subMapSize;
                int yOffset = subMapY * subMapSize;

                //draw the submap
                fullMapDrawer.DrawImage(subMap, xOffset, yOffset);

                //adjust texture offsets to be the location in the full map
                foreach (Texture texture in subMapTextures[subMap])
                {
                    texture.Top += yOffset;
                    texture.Left += xOffset;
                }

                //decide where to put the next submap
                subMapX += 1;
                if (subMapX == subMapsWide)
                {
                    subMapX = 0;
                    subMapY += 1;
                }
            }
            fullMapDrawer.Dispose();

            //save the fullMap bitmap
            fullMap.Save(TextureTool.Instance.WorkingDirectory + fileName);
        }

        private RectanglePacker GetPacker(int size)
        {
            return new ArevaloRectanglePacker(size, size);
            //return new SimpleRectanglePacker(size, size);
            //return new CygonRectanglePacker(size, size);
        }

    }
}
