using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.IO;
using System.Security;

namespace FarmTycoon
{
    public class Program
    {
        /// <summary>
        /// The currently loaded game
        /// </summary>
        private static Game _game;
                
        /// <summary>
        /// Contains references to several classes relating the the game UI
        /// </summary>
        private static UserInterface _userInterface;
        
        /// <summary>
        /// The thread that runs the game
        /// </summary>
        private static GameThread _gameThread;
                
        /// <summary>
        /// Application settings
        /// </summary>
        private static Settings _settings;



        /// <summary>
        /// The currently loaded game
        /// </summary>
        public static Game Game
        {
            get { return _game; }
        }

        /// <summary>
        /// Contains references to several classes relating the UI
        /// </summary>
        public static UserInterface UserInterface
        {
            get { return _userInterface; }
        }
        
        /// <summary>
        /// The thread that runs the game, includes a reference to the Clock that drives the game objects at game speed.
        /// </summary>
        public static GameThread GameThread
        {
            get { return _gameThread; }
        }

        /// <summary>
        /// Application settings
        /// </summary>
        public static Settings Settings
        {
            get { return _settings; }
        }
        
        /// <summary>
        /// Set the currently loaded game
        /// </summary>
        public static void SetCurrentGame(Game game)
        {
            _game = game;
        }

        
        static void Main(string[] args)
        {
            //get path of the exe
            string exeDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar;
            
            //read in settings
            string settingsFile = exeDir + "Settings.xml";
            _settings = new Settings(settingsFile);
                     
            //create the game thread
            _gameThread = new GameThread();
            
            //create UI            
            _userInterface = new UserInterface();
            _userInterface.Setup();

            //not actually setting up a full game world, we just need to load in default textures for the icons, and so we have a window manager
            _userInterface.SetupGameWorld("DefaultTextures", 10);

            
            if (args.Length == 0)
            {
                //open the start window
                new StartWindow();
            }
            else if (args[0] == "D")
            {
                StartDebug();
            }
            else if (args[0] == "P")
            {
                StartPathFindingTest();
                return;
            }
            
            //start the game thread (starts on its own thread)
            _gameThread.Start();
            
            //collect any setup garbage
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //start graphics runs on this thread
            _userInterface.Graphics.Start();

            //if we have returned from start its beacuse user has closed the window,
            //stop the game thread
            _gameThread.Stop();
                        
        }


        private static void StartPathFindingTest()
        {
            _userInterface.Graphics.WindowWidth = 200;
            _userInterface.Graphics.WindowHeight = 200;

            //create a game             
            GameFile.New(250);

            //how many times to find a path
            int tests = 10000;
            Random rnd = new Random();
            
            //get all the land
            List<Land> allLand = _game.GameState.MasterObjectList.FindAll<Land>();

            Console.WriteLine("Starting Test");

            //choose two random lands and find a path
            DateTime start = DateTime.Now;
            for (int i = 0; i < tests; i++)
            {
                Location loc1 = allLand[rnd.Next(allLand.Count)].LocationOn;
                Location loc2 = allLand[rnd.Next(allLand.Count)].LocationOn;
                                
                _game.PathFinder.FindPathCost(loc1, loc2);
            }

            //figure out how long it took and print that out
            DateTime end = DateTime.Now;
            double msecs = (end - start).TotalMilliseconds;
            double msecsPer = msecs / tests;
            Console.WriteLine(msecs.ToString() + "ms total");
            Console.WriteLine(msecsPer.ToString() + "ms per path");
            Console.ReadLine();
            return;
        }

        private static void StartDebug()
        {
            //create a new game            
            GameFile.New(100);
            

            //create delivery area 
            DeliveryArea deliveryArea = new DeliveryArea();
            deliveryArea.Setup(GameState.Current.Locations.GetLocation(30, 50));
            deliveryArea.DoneWithPlacement();


            StorageBuilding building1 = new StorageBuilding();
            building1.Setup(GameState.Current.Locations.GetLocation(39, 63), (StorageBuildingInfo)FarmData.Current.GetInfo("StorageBuilding_Barn"));
            building1.DoneWithPlacement();
            StorageBuilding building2 = new StorageBuilding();
            building2.Setup(GameState.Current.Locations.GetLocation(61, 63), (StorageBuildingInfo)FarmData.Current.GetInfo("StorageBuilding_Barn"));
            building2.DoneWithPlacement();
            StorageBuilding building3 = new StorageBuilding();
            building3.Setup(GameState.Current.Locations.GetLocation(38, 35), (StorageBuildingInfo)FarmData.Current.GetInfo("StorageBuilding_Barn"));
            building3.DoneWithPlacement();
            StorageBuilding building4 = new StorageBuilding();
            building4.Setup(GameState.Current.Locations.GetLocation(57, 35), (StorageBuildingInfo)FarmData.Current.GetInfo("StorageBuilding_Barn"));
            building4.DoneWithPlacement();

            //for (int i = 39; i <= 61; i++)
            //{
            //    Road road = new Road();
            //    road.Setup(GameState.Current.Locations.GetLocation(i, 66));
            //    road.DoneWithPlacement();
            //}
            //for (int i = 38; i <= 57; i++)
            //{
            //    Road road = new Road();
            //    road.Setup(GameState.Current.Locations.GetLocation(i, 38));
            //    road.DoneWithPlacement();
            //}
            //for (int i = 39; i <= 65; i++)
            //{
            //    Road road = new Road();
            //    road.Setup(GameState.Current.Locations.GetLocation(51, i));
            //    road.DoneWithPlacement();
            //}

            
            //have the window manager show the toolbar windows for the game
            Program.UserInterface.WindowManager.CreateToolbars();

            //set the default editor to the selection editor
            Program.UserInterface.ActiveEditorManager.DefaultEditor = new SelectionEditor();

            //set 8x game speed, and unpause
            Program.GameThread.ClockDriver.DesiredRate = 8.0;
            Program.GameThread.ClockDriver.Paused = false;

            Program.UserInterface.Graphics.ViewX = 150;
            Program.UserInterface.Graphics.ViewY = 150;
        }
        

    }
}
