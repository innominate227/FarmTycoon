using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FarmTycoon
{
    /// <summary>
    /// Some info that needs to get saved when the game is saved.  this info can be loaded from a save file without loading the entire file
    /// </summary>
    public partial class SaveInfo
    {
        #region Member Vars


        /// <summary>
        /// Name of the folder containing textures bitmaps, and location files.
        /// </summary>
        private string _texturesFolder = "DefaultTextures";

        /// <summary>
        /// Size of the game world
        /// </summary>
        private int _gameSize;

        /// <summary>
        /// Camera X location on save
        /// </summary>
        private float _viewX;

        /// <summary>
        /// Camera Y location on save
        /// </summary>
        private float _viewY;
        
        /// <summary>
        /// Camera Zoom on save
        /// </summary>
        private float _scale;


        
        /// <summary>
        /// Game date that the game was last saved
        /// </summary>
        private string _gameDateLastSave;


        /// <summary>
        /// Real date that the game was last saved
        /// </summary>        
        private string _realDateLastSave;

        /// <summary>
        /// Scenario name
        /// </summary>    
        private string _name;

        /// <summary>
        /// Scenario description
        /// </summary>
        private string _description;

        /// <summary>
        /// Scenario objective
        /// </summary>
        private string _objective;

        #endregion


        public SaveInfo()
        {
        }

        /// <summary>
        /// populate save info fields with info about the current state
        /// </summary>
        public void PopulateSaveInfo()
        {
            GameSize = GameState.Current.Locations.Size;
            ScenarioName = FarmData.Current.ScenarioInfo.Name;
            ScenarioDescription = FarmData.Current.ScenarioInfo.Description;
            ScenarioObjective = FarmData.Current.ScenarioInfo.Objective;
            TexturesFolder = FarmData.Current.ScenarioInfo.Textures;
            ViewX = Program.UserInterface.Graphics.ViewX;
            ViewY = Program.UserInterface.Graphics.ViewY;
            Scale = Program.UserInterface.Graphics.Scale;
            GameDateLastSave = Calandar.DateAsString(GameState.Current.Calandar.Date);
            RealDateLastSave = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
        }


        #region Properties


        /// <summary>
        /// Name of the folder containing textures bitmaps, and location files.
        /// </summary>        
        public string TexturesFolder
        {
            get { return _texturesFolder; }
            set { _texturesFolder = value; }
        }

        /// <summary>
        /// Size of the game world
        /// </summary>        
        public int GameSize
        {
            get { return _gameSize; }
            set { _gameSize = value; }
        }


        /// <summary>
        /// Camera X location on save
        /// </summary>
        public float ViewX
        {
            get { return _viewX; }
            set { _viewX = value; }
        }

        /// <summary>
        /// Camera Y location on save
        /// </summary>
        public float ViewY
        {
            get { return _viewY; }
            set { _viewY = value; }
        }
        
        /// <summary>
        /// Camera Zoom on save
        /// </summary>
        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }
        

        /// <summary>
        /// Game date that the game was last saved
        /// </summary>
        public string GameDateLastSave
        {
            get { return _gameDateLastSave; }
            set { _gameDateLastSave = value; }
        }


        /// <summary>
        /// Real date that the game was last saved
        /// </summary>
        public string RealDateLastSave
        {
            get { return _realDateLastSave; }
            set { _realDateLastSave = value; }
        }


        /// <summary>
        /// Scenario name
        /// </summary>
        public string ScenarioName
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Scenario description
        /// </summary>
        public string ScenarioDescription
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Scenario objective
        /// </summary>
        public string ScenarioObjective
        {
            get { return _objective; }
            set { _objective = value; }
        }

        #endregion

        #region Read Write

        public void WriteStateV1(BinaryWriter writer)
        {
            writer.Write(_texturesFolder);
            writer.Write(_gameSize);
            writer.Write(_viewX);
            writer.Write(_viewY);
            writer.Write(_scale);
            writer.Write(_gameDateLastSave);
            writer.Write(_realDateLastSave);
            writer.Write(_name);
            writer.Write(_description);
            writer.Write(_objective);
        }

        public void ReadStateV1(BinaryReader reader)
        {
            _texturesFolder = reader.ReadString();
            _gameSize = reader.ReadInt32();
            _viewX = reader.ReadSingle();
            _viewY = reader.ReadSingle();
            _scale = reader.ReadSingle();
            _gameDateLastSave = reader.ReadString();
            _realDateLastSave = reader.ReadString();
            _name = reader.ReadString();
            _description = reader.ReadString();
            _objective = reader.ReadString();
        }

        #endregion
    }
}
