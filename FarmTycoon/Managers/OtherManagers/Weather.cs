using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{

    /// <summary>
    /// UI Strings that are set by the game script
    /// </summary>
    public partial class UIStrings : ISavable
    {
        #region Events

        /// <summary>
        /// Event raised when the weather changes
        /// </summary>
        public event Action WeatherChanged;

        /// <summary>
        /// Event raised when the vitory progress changes
        /// </summary>
        public event Action VictoryProgressChanged;

        #endregion

        #region Member Vars

        /// <summary>
        /// The current wether conditions
        /// </summary>
        private string _weather = "";

        /// <summary>
        /// The current victory progress
        /// </summary>
        private string _victoryProgress = "";

        #endregion

        #region Setup

        public UIStrings()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The current game wether conditions
        /// </summary>
        public string Weather
        {
            get { return _weather; }
            set 
            { 
                _weather = value;
                if (WeatherChanged != null)
                {
                    WeatherChanged();
                }
            }
        }

        /// <summary>
        /// The current victory progress
        /// </summary>
        public string VictoryProgress
        {
            get { return _victoryProgress; }
            set
            {
                _victoryProgress = value;
                if (VictoryProgressChanged != null)
                {
                    VictoryProgressChanged();
                }
            }
        }

        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteString(_weather);
            writer.WriteString(_victoryProgress);
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _weather = reader.ReadString();
            _victoryProgress = reader.ReadString();
        }

        public void AfterReadStateV1()
        {
        }
        #endregion
    }
}
