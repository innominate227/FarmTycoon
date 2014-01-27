using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;
using System.Security;

namespace FarmTycoon
{
    public class GameFile
    {
        /// <summary>
        /// Create a new game, and make it the current game
        /// </summary>
        public static void New(int size)
        {
            //make sure a game is not currently loaded
            Debug.Assert(Program.Game == null);

            //default game files
            string farmDataFile = Program.Settings.DataFolder + Path.DirectorySeparatorChar + "defaultFarmData.xml";            
            string scenarioScriptFile = Program.Settings.DataFolder + Path.DirectorySeparatorChar + "ScenarioScript.cs";
            
            //load the correct textures            
            Program.UserInterface.SetupGameWorld("DefaultTextures", size);

            //create a new game state
            GameState gameState = new GameState();
            gameState.Setup();

            //cretae farm data using default data
            FarmData farmData = new FarmData(File.ReadAllText(farmDataFile));
            FarmData.Current = farmData;

            //create the script using default script
            ScriptPlayer script = new ScriptPlayer(File.ReadAllText(scenarioScriptFile));
                        
            //create locations
            gameState.Locations.CreateLocations(size);

            //Build some land
            LandBuilder landBuilder = new LandBuilder();
            landBuilder.BuildLand(size);

            //create the game
            Game game = new Game(gameState, farmData, script, true, "");

            //set the game as the current game
            Program.SetCurrentGame(game);            
                                    
            //set the camera back to center of map
            Program.UserInterface.Graphics.ViewX = size;
            Program.UserInterface.Graphics.ViewY = size;
            Program.UserInterface.Graphics.Scale = 1;

            //do the things we need to do after loading
            AfterLoadOrNew();
        }
        
        /// <summary>
        /// Save the current game
        /// </summary>
        /// <param name="fileName"></param>
        public static void Save(string fileName)
        {
            //make sure a game is currently loaded
            Debug.Assert(Program.Game != null);
            
            //pause the game
            bool wasPaused = Program.GameThread.ClockDriver.Paused;
            Program.GameThread.ClockDriver.Paused = true;

            //create a window to show save progress
            ProgressWindow progressWindow = new ProgressWindow("Saving");
            progressWindow.MaxValue = 1000;
            
            //special pre save for scenario mode
            if (Program.Game.ScenarioEditMode)
            {
                BeforeSaveScenarioMode();
            }

            //remember the file name we used
            GameState.Current.LastUsedValues.SaveName = Path.GetFileNameWithoutExtension(fileName);
            
            //binary writer to actually write to the file
            BinaryWriter binaryWriter = GetStreamWriter(fileName);

            //write the version number for the file
            binaryWriter.Write(1);
                                    
            //create save info, and populate with current game info
            SaveInfo saveInfo = new SaveInfo();
            saveInfo.PopulateSaveInfo();

            //write the save info            
            saveInfo.WriteStateV1(binaryWriter);
                        
            //write the farm data the game uses
            binaryWriter.Write(Program.Game.FarmData.FarmDataXml);

            //write the scenario script the game uses
            binaryWriter.Write(Program.Game.ScriptPlayer.ScriptText);

            //write the state of the scenario script
            binaryWriter.Write(Program.Game.ScriptPlayer.GetScriptState());

            //get the number of game objects that will be saved
            int numberOfGameObject = Program.Game.GameState.MasterObjectList.FindAll<IGameObject>().Count;

            //progress call back will update the progress window
            Action<double> progressCallback = delegate(double progress)
            {
                progressWindow.Progress = (int)(progressWindow.MaxValue * progress);
            };

            //create a StateWriter version 1 
            StateWriterV1 writer = new StateWriterV1(binaryWriter, Program.Game.FarmData, numberOfGameObject, progressCallback);

            //write the state of the game
            writer.WriteObject(Program.Game.GameState);
                                                
            //close the underlying binary writer
            binaryWriter.Close();

            //close the progress window
            progressWindow.CloseWindow();
            
            //resume the game if it was paused
            Program.GameThread.ClockDriver.Paused = wasPaused;
        }
                                
        /// <summary>
        /// Load a Game fomr the file passed, and make the game the current game
        /// </summary>
        public static void Load(string fileName, bool scanarioEditMode)
        {
            //make sure a game is not currently loaded
            Debug.Assert(Program.Game == null);
                        
            //get binary reader for the file
            BinaryReader reader = GetStreamReader(fileName);
            
            //read the version of the file
            int versionNumer = reader.ReadInt32();
            
            //read the save info
            SaveInfo saveInfo = new SaveInfo();
            if (versionNumer == 1)
            {
                saveInfo.ReadStateV1(reader);
            }

            //load the correct textures for the save
            Program.UserInterface.SetupGameWorld(saveInfo.TexturesFolder, saveInfo.GameSize);

            //set the camera back to the correct location
            Program.UserInterface.Graphics.ViewX = saveInfo.ViewX;
            Program.UserInterface.Graphics.ViewY = saveInfo.ViewY;
            Program.UserInterface.Graphics.Scale = saveInfo.Scale;

            //create a window to show load progress
            ProgressWindow progressWindow = new ProgressWindow("Loading");
            progressWindow.MaxValue = 1000;
            
            //progress call back will update the progress window
            Action<double> progressCallback = delegate(double progress)
            {
                progressWindow.Progress = (int)(progressWindow.MaxValue * progress);
            };
            
            //load game
            Game game = null;
            if (versionNumer == 1)
            {
                //load a version 1 game save file
                game = LoadV1(fileName, reader, scanarioEditMode, progressCallback);
            }

            //close the game reader
            reader.Close();
                                    
            //set the currently loaded game
            Program.SetCurrentGame(game);
                        
            //close the progress window
            progressWindow.CloseWindow();
                        
            //do the things we need to do after loading or file new
            AfterLoadOrNew();
        }



        /// <summary>
        /// Load a Game, and SaveInfo from a version 1 game file
        /// </summary>
        private static Game LoadV1(string fileName, BinaryReader binaryReader, bool scanarioEditMode, Action<double> progressCallback)
        {            
            //read the farm data the game uses
            string farmDataXml = binaryReader.ReadString();
            FarmData farmData = new FarmData(farmDataXml);
            FarmData.Current = farmData;

            //read the scenario script data the game uses
            string scenarioScript = binaryReader.ReadString();
            ScriptPlayer script = new ScriptPlayer(scenarioScript);

            //read the state of the scenario script
            string scriptState = binaryReader.ReadString();
            script.SetScriptState(scriptState);

            
            //create a StateReader version 1 
            StateReaderV1 reader = new StateReaderV1(binaryReader, farmData, progressCallback);

            //read the game state
            GameState gameState = reader.ReadObject<GameState>();

            //call after read on all objects just read in
            reader.DoAfterReadState();

            //create a new Game with what was read
            Game game = new Game(gameState, farmData, script, scanarioEditMode, fileName);

            //return the game we read
            return game;
        }
                
        /// <summary>
        /// Load a SaveInfo for the file passed
        /// </summary>
        public static SaveInfo LoadInfo(string fileName)
        {
            //get binary reader for the file
            BinaryReader reader = GetStreamReader(fileName);

            //read the version of the file
            int versionNumer = reader.ReadInt32();

            //create saveinfo object we will read state into
            SaveInfo saveInfo = new SaveInfo();

            //load different based on save file version number
            if (versionNumer == 1)
            {
                //read the save info       
                saveInfo.ReadStateV1(reader);
            }

            reader.Close();

            //return the loaded save info
            return saveInfo;
        }

        /// <summary>
        /// Close the current game
        /// </summary>
        public static void Close()
        {
            //delete all windows
            Program.UserInterface.WindowManager.DeleteAllWindows();
            Program.UserInterface.ActiveEditorManager.DefaultEditor = new NullEditor();
            Program.UserInterface.ActiveEditorManager.DefaultEditor.StartEditing();

            //clear references to old farm data
            FarmData.Current = null;
            
            //delete the old game
            GameState.Current.Delete();
            Program.Game.ScriptPlayer.UnloadScript();
            Program.SetCurrentGame(null);

            //there should be no notifications left
            Debug.Assert(Program.GameThread.Clock.NotificationsCount == 0);

            //collect old game garbage
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        
        /// <summary>
        /// Called after load game or new game
        /// </summary>
        private static void AfterLoadOrNew()
        {
            //special load for scenario mode
            if (Program.Game.ScenarioEditMode)
            {
                AfterLoadOrNewScenarioMode();
            }

            TycoonGraphicsLib.Tile.StartChangeSet();

            //refresh all the GameObject's tiles
            foreach (GameObject obj in GameState.Current.MasterObjectList.FindAll<GameObject>())
            {
                obj.UpdateTiles();
            }

            TycoonGraphicsLib.Tile.EndChangeSet();
                        
            //have the window manager show the toolbar windows for the game
            Program.UserInterface.WindowManager.CreateToolbars();

            //set the default editor to the selection editor
            Program.UserInterface.ActiveEditorManager.DefaultEditor = new SelectionEditor();
            Program.UserInterface.ActiveEditorManager.DefaultEditor.StartEditing();

            //set 1x game speed, and unpause
            Program.GameThread.ClockDriver.DesiredRate = 1.0;
            Program.GameThread.ClockDriver.Paused = false;

            //tell the scenario script to listen to date change event 
            //(dont do this in scenario edit mode because we do not want the script runnin in that mode)
            if (Program.Game.ScenarioEditMode == false)
            {                
                Program.Game.ScriptPlayer.HandleDateChanged();
            }
            
            //run day0, if we are just starting the scenario
            if (GameState.Current.Calandar.Date == -1)
            {
                GameState.Current.Calandar.DoDay0();
            }
        }
        
        /// <summary>
        /// Called before saving when in scenario mode
        /// </summary>
        private static void BeforeSaveScenarioMode()
        {
            //clear out store inventory
            GameState.Current.StoreStock.Clear();
        }

        /// <summary>
        /// Called after loading when in scenario mode
        /// </summary>
        private static void AfterLoadOrNewScenarioMode()
        {
            //add items to store inventory
            foreach (ItemTypeInfo itemTypeInfo in FarmData.Current.GetInfos<ItemTypeInfo>())
            {
                if (itemTypeInfo.ItemTypeRelation == ItemTypeRelation.One)
                {
                    ItemType itemType = GameState.Current.ItemPool.GetItemType(itemTypeInfo.Name);
                    GameState.Current.StoreStock.IncreaseItemCount(itemType, 999999);
                }
                else if (itemTypeInfo.ItemTypeRelation == ItemTypeRelation.Qualities)
                {
                    for (int quality = 0; quality < 10; quality++)
                    {
                        ItemType itemType = GameState.Current.ItemPool.GetItemType(itemTypeInfo.Name, quality);
                        GameState.Current.StoreStock.IncreaseItemCount(itemType, 999999);
                    }
                }
                else if (itemTypeInfo.ItemTypeRelation == ItemTypeRelation.Many)
                {
                    for (int animalNum = 0; animalNum < 100; animalNum++)
                    {                        
                        ItemType itemType = GameState.Current.ItemPool.GetItemType(itemTypeInfo.Name, Guid.NewGuid().ToString().Replace("-", ""));
                        GameState.Current.StoreStock.IncreaseItemCount(itemType, 1);
                    }
                }
            }
        }
        
        /// <summary>
        /// Get a BinaryWriter for the file passed that will write the file in normal or compressed mode depending on settings
        /// </summary>
        private static BinaryWriter GetStreamWriter(string file)
        {
            //write the states to disk
            FileStream fileStream = new FileStream(file, FileMode.Create);
            
            //return normal writer or add a GZipStream inbetween
            if (Program.Settings.CompressSaves)
            {
                GZipStream gzStream = new GZipStream(fileStream, CompressionMode.Compress);
                return new BinaryWriter(gzStream);
            }
            else
            {
                return new BinaryWriter(fileStream);
            }
        }
        
        /// <summary>
        /// Get a StreamReader for the file passed that will correctly read the file if it is either compressed or uncompressed
        /// </summary>
        private static BinaryReader GetStreamReader(string file)
        {
            //load a game state object from the disk
            FileStream fileStream = new FileStream(file, FileMode.Open);

            //read in the magic number to determine if it was compressed or not
            BinaryReader reader = new BinaryReader(fileStream);
            int magicNumber = reader.ReadUInt16();
            
            //go back to the front of the file
            fileStream.Position = 0;

                      
            if (magicNumber == 0x8b1f)
            {
                //if the magic number is of a GZip file use a GZipStream to read  
                GZipStream gzStream = new GZipStream(fileStream, CompressionMode.Decompress);
                return new BinaryReader(gzStream);
            }
            else
            {
                //otherwise use normal binary reader
                return new BinaryReader(fileStream);
            }
        }

    }
}
