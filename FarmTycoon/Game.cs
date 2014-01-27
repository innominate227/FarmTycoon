using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// Top most object, contains the state of the game, farm data (data files) used by the game, and the scenario script the game runs.    
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Contains the state of all objects in the game
        /// </summary>
        private GameState _gameState;

        /// <summary>
        /// Farm data used by game objects
        /// </summary>
        private FarmData _farmData;

        /// <summary>
        /// Runs the scenario script
        /// </summary>
        private ScriptPlayer _scriptPlayer;
        
        /// <summary>
        /// Used by game objects to find paths between locations in the gameworld
        /// </summary>
        private FastPathFinder _pathFinder;

        /// <summary>
        /// True if the game has been loaded in scenario edit mode
        /// </summary>
        private bool _scenarioEditMode;

        /// <summary>
        /// The file that the Game was loaded from (or empty string if this was a new scenario)
        /// </summary>
        private string _gameFile;

        /// <summary>
        /// Randomizer shared by all game objects
        /// </summary>
        private Random _random;


        /// <summary>
        /// Create a new game
        /// </summary>
        public Game(GameState gameState, FarmData farmData, ScriptPlayer scriptPlayer, bool scenarioEditMode, string gameFile)
        {
            _gameState = gameState;
            _farmData = farmData;
            _scriptPlayer = scriptPlayer;
            _scenarioEditMode = scenarioEditMode;
            _gameFile = gameFile;

            _pathFinder = new FastPathFinder();
            _pathFinder.Setup();
            _random = new Random();

        }




        /// <summary>
        /// Contains the state of all objects in the game
        /// </summary>
        public GameState GameState
        {
            get { return _gameState; }
        }

        /// <summary>
        /// Farm data parsed from data files
        /// </summary>
        public FarmData FarmData
        {
            get { return _farmData; }
        }

        /// <summary>
        /// Plays the script for the current scenario
        /// </summary>
        public ScriptPlayer ScriptPlayer
        {
            get { return _scriptPlayer; }
        }

        /// <summary>
        /// Used by to find paths between locations in the gameworld
        /// </summary>
        public FastPathFinder PathFinder
        {
            get { return _pathFinder; }
        }


        /// <summary>
        /// True if the game has been loaded in scenario edit mode
        /// </summary>
        public bool ScenarioEditMode
        {
            get { return _scenarioEditMode; }
        }

        /// <summary>
        /// The file that the Game was loaded from (or empty string if this was a new scenario)
        /// </summary>
        public string GameFile
        {
            get { return _gameFile; }
        }


        /// <summary>
        /// Randomizer shared by all game objects
        /// </summary>
        public Random Random
        {
            get { return _random; }
        }


    }
}
