using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace TycoonGraphicsLib
{

    /// <summary>
    /// Manages the TileTextures, and the TileTextureQuartets 
    /// </summary>
    internal class TileTextureManager
    {        
        /// <summary>
        /// Texture sheet for tile textures
        /// </summary>
        private List<TextureSheet> _textureSheets = new List<TextureSheet>();
        
        /// <summary>
        /// The index of the current tile texture sheet being used to render tiles
        /// </summary>
        private int _currentTextureSheetIndex;
        
        /// <summary>
        /// Dictionary mapping tile texture name to textureQuartet
        /// </summary>
        private Dictionary<string, TileTexture> _textures = new Dictionary<string, TileTexture>();

		/// <summary>
        /// Dictionary mapping texture quartet name to textureQuartet
        /// </summary>
        private Dictionary<string, TileTextureQuartet> _quartets = new Dictionary<string, TileTextureQuartet>();

        /// <summary>
        /// The world that the tile texture manager contains tile textures for.
        /// </summary>
        private World _world;

        /// <summary>
        /// Create a new TileTextureManager to manage tile textures for this world
        /// </summary>
        /// <param name="world"></param>
        public TileTextureManager(World world)
		{
            _world = world;
            LoadTextures();
		}			

        /// <summary>
        /// Texture sheet containing the tile textures
        /// </summary>
        public List<TextureSheet> TextureSheets
        {
            get { return _textureSheets; }
        }

        /// <summary>
        /// The index of the current tile texture sheet being used to render tiles.
        /// Setting this also makes the OpenGL calls to make that texture sheet active.
        /// Note that the window manager also binds texture sheets, so the current texture sheet may not match the current open gl texture sheet after rendering windows.
        /// </summary>
        public int CurrentTextureSheetIndex
        {
            get { return _currentTextureSheetIndex; }
            set 
            {
                _currentTextureSheetIndex = value;
                GL.BindTexture(TextureTarget.Texture2D, _textureSheets[_currentTextureSheetIndex].TextureSheetId);
            }
        }

        /// <summary>
        /// Load the texture sheet used by the World
        /// </summary>
        private void LoadTextures()
        {
            //determine the path for the texture files, and the texturemap
            string textureSheetImageFile = _world.WorldSettings.TexturesBitmapFile;
            string texturesFile = _world.WorldSettings.TextureRegionsFile;
            string quartetsFile = _world.WorldSettings.TextureQuartetsFile;
			
            //load texture map bitmap
            Bitmap textureSheetImageFull = new Bitmap(textureSheetImageFile);

            //make sure the dimension of the map are valid
            Debug.Assert(textureSheetImageFull.Width == textureSheetImageFull.Height);
            Debug.Assert(textureSheetImageFull.Width % 1024 == 0);

            //determine the max texture size supported by the hardware, and how many times we will need to split our texture sheet
            int maxTextureSize = DetermineMaxTextureSize(textureSheetImageFull.Width);
            int numberOfSplits = textureSheetImageFull.Width / maxTextureSize;
            
            //read the textures file into memory            
            StreamReader texturesFileReader = new StreamReader(texturesFile);
            string texturesFileContents = texturesFileReader.ReadToEnd();
            texturesFileReader.Close();
            
            //parse the locations of each texture from the textures file (locations are not yet adjusted for the sheet being split into multiple)
            List<TextureSheetLocation> allTextureSheetTextureLocations = new List<TextureSheetLocation>();
            foreach (string textureFileLine in texturesFileContents.Split('\n'))
            {
                //skip blank lines and lines with comments
                if (textureFileLine.Trim() != "" && textureFileLine.Trim().StartsWith("#") == false)
                {
                    TextureSheetLocation textureSheetLocation = new TextureSheetLocation();
                    textureSheetLocation.ParseFromTexturesFileLine(textureFileLine);
                    allTextureSheetTextureLocations.Add(textureSheetLocation);                    
                }
            }

            //dictionary of all textures in all sheets by name
            Dictionary<string, Texture> texturesByName = new Dictionary<string, Texture>();

            //create the needed number of texture sheets
            for (int sheetIndexX = 0; sheetIndexX < numberOfSplits; sheetIndexX++)
            {
                for (int sheetIndexY = 0; sheetIndexY < numberOfSplits; sheetIndexY++)
                {
                    //get the top left point of the sheet to make
                    int sheetLeft = sheetIndexX * maxTextureSize;
                    int sheetTop = sheetIndexY * maxTextureSize;

                    //get an image of this sheet by copying a sub image from the full image
                    Bitmap sheetImage = new Bitmap(maxTextureSize, maxTextureSize);
                    Graphics sheetImageDrawer = Graphics.FromImage(sheetImage);
                    sheetImageDrawer.DrawImage(textureSheetImageFull, new Rectangle(0, 0, maxTextureSize, maxTextureSize), new Rectangle(sheetLeft, sheetTop, maxTextureSize, maxTextureSize), GraphicsUnit.Pixel);
                    sheetImageDrawer.Dispose();

                    //create a list of texture with locations in this sheet
                    List<TextureSheetLocation> textureSheetTextureLocationsForThisSheet = new List<TextureSheetLocation>();

                    //check all the locations that were loaded
                    foreach (TextureSheetLocation textureSheetTextureLocation in allTextureSheetTextureLocations)
                    {
                        //check if the texture is with in the bounds of this texture sheet
                        if ((textureSheetTextureLocation.Left >= sheetLeft && textureSheetTextureLocation.Left < sheetLeft + maxTextureSize) &&
                            (textureSheetTextureLocation.Top >= sheetTop && textureSheetTextureLocation.Top < sheetTop + maxTextureSize))
                        {
                            //create a copy of the location object, change the top and left to be relative to this sheet instead of the entire image
                            TextureSheetLocation textureSheetTextureLocationCopy = new TextureSheetLocation();
                            textureSheetTextureLocationCopy.SetValues(textureSheetTextureLocation);
                            textureSheetTextureLocationCopy.Left %= maxTextureSize;
                            textureSheetTextureLocationCopy.Top %= maxTextureSize;

                            //add to the list of locations for this sheet
                            textureSheetTextureLocationsForThisSheet.Add(textureSheetTextureLocationCopy);
                        }
                    }

                    //create the textures sheet, and add to the list of sheets
                    sheetImage.MakeTransparent(Color.Blue);
                    TextureSheet textureSheet = new TextureSheet(sheetImage, textureSheetTextureLocationsForThisSheet);
                    _textureSheets.Add(textureSheet);

                    //add each texture in the sheet to the textuires by name dictionary
                    foreach (TextureSheetLocation textureSheetTextureLocationForThisSheet in textureSheetTextureLocationsForThisSheet)
                    {
                        Texture texture = textureSheet.GetTexture(textureSheetTextureLocationForThisSheet.Name);
                        texturesByName.Add(texture.Name, texture);
                    }
                }
            }
                        


            //read through each line of the texture file again, and create a tile texture object for it
            foreach (string textureFileLine in texturesFileContents.Split('\n'))
            {
                //skip blank lines and lines with comments
                if (textureFileLine.Trim() != "" && textureFileLine.Trim().StartsWith("#") == false)
            	{
                    //parse the texture file line
                    string name = textureFileLine.Split('=')[0].Trim();
                    string[] textureLineTokens = textureFileLine.Split('=')[1].Trim().Split(',');

                    //get the texture for that line
                    Texture texture = texturesByName[name];
                    
                    //get the texture properties for a TileTexture                   
                    int centerOffsetYPixels = int.Parse(textureLineTokens[4].Trim());                    
                    int centerOffsetXPixels = int.Parse(textureLineTokens[5].Trim());     
                    
                    //get the height, width, and center offset in world units
                    float height = texture.Height / SharedWorldViewSettings.WORLD_UNIT_HEIGHT;
                    float width = texture.Width / SharedWorldViewSettings.WORLD_UNIT_WIDTH;
                    float centerOffsetY = centerOffsetYPixels / SharedWorldViewSettings.WORLD_UNIT_HEIGHT;
                    float centerOffsetX = centerOffsetXPixels / SharedWorldViewSettings.WORLD_UNIT_WIDTH;
                                                           
                    //get the texture sheet index for the texture
                    int textureSheetIndex = _textureSheets.IndexOf(texture.Sheet);

                    //create a tile texture
                    TileTexture tileTexture = new TileTexture(texture, height, width, centerOffsetY, centerOffsetX, textureSheetIndex);
                    _textures.Add(name, tileTexture);
				}
			}
					

            //read each line of the texture quartet file, and create a texture quartet object for it
            StreamReader quartetFileReader = new StreamReader(quartetsFile);
            string quartetFile = quartetFileReader.ReadToEnd();
            foreach (string quartetFileLine in quartetFile.Split('\n'))
            {
            	if (quartetFileLine.Trim() != "" && quartetFileLine.Trim().StartsWith("#") == false)
            	{
                    string name = quartetFileLine.Split('=')[0].Trim();
                    string northName = quartetFileLine.Split('=')[1].Split(',')[0].Trim();
                    string eastName = quartetFileLine.Split('=')[1].Split(',')[1].Trim();
                    string southName = quartetFileLine.Split('=')[1].Split(',')[2].Trim();
                    string westName = quartetFileLine.Split('=')[1].Split(',')[3].Trim();
                    TileTexture north = _textures[northName];
                    TileTexture east = _textures[eastName];
                    TileTexture south = _textures[southName];
                    TileTexture west = _textures[westName];
			
					TileTextureQuartet quartet = new TileTextureQuartet(name, north, east, south, west);
                	_quartets.Add(quartet.Name, quartet);
				}
			}			
		}

        /// <summary>
        /// Determine the maximum texture size supported by the graphics hardware (that is also a multiple of 1024)
        /// </summary>
        private int DetermineMaxTextureSize(int textureSheetSize)
        {
            //if we are forceing the min texture size return that
            if (_world.WorldSettings.ForceMinTextureSize)
            {
                return 1024;
            }

            //get the max texture size supported by the hardware
            int maxTextureSize;
            GL.GetInteger(GetPName.MaxTextureSize, out maxTextureSize);

            //get the highest multiple of 1024 that is less than or equal to the maxTextureSize
            int powerOfTwo = 1024;
            while (true)
            {
                if (powerOfTwo * 2 > maxTextureSize)
                {
                    break;
                }
                powerOfTwo = powerOfTwo * 2;
            }

            //use that as the max texture size
            maxTextureSize = powerOfTwo;

            //if that is larger than the size of the entire image, use the size of the entire image
            if (maxTextureSize > textureSheetSize)
            {
                maxTextureSize = textureSheetSize;
            }

            return maxTextureSize;            
        }
        
        /// <summary>
        /// Free all resources used by the TileTextureManager
        /// </summary>
        public void Delete()
        {
            //delete the texture sheets with the textures
            foreach (TextureSheet textureSheet in _textureSheets)
            {
                textureSheet.Delete();
            }
        }
			
        /// <summary>
        /// Get a texture by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TileTexture GetTexture(string name)
        {
            return _textures[name];
        }
        	
        /// <summary>
        /// Get a texture quartet by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TileTextureQuartet GetTextureQuartet(string name)
        {            
            if (name == null || _quartets.ContainsKey(name) == false)
            {
                return _quartets["bad"];
            }

            return _quartets[name];
        }

    }
}
