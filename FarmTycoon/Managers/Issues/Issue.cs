using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// An Issue being managed by the issue manager
    /// </summary>    
    public class Issue : ISavable
    {
        #region Member Vars

        /// <summary>
        /// The object that has this issue
        /// </summary>
        private ISavable _hasIssue;

        /// <summary>
        /// Unique key for the issue (unique to the object having the issue)
        /// This allow us to distinguish issues where more an object has more than one issue.
        /// </summary>
        private string _key;

        /// <summary>
        /// Text about the issue
        /// </summary>
        private string _description;

        /// <summary>
        /// If the issue has location data this is the location, otherwise it is null
        /// </summary>
        private Location _location;

        #endregion

        #region Constructor
        
        /// <summary>
        /// This constructor should only be used for loading game state
        /// </summary>
        public Issue()
        {
        }

        /// <summary>
        /// Create a new issue
        /// </summary>
        public Issue(ISavable hasIssue, string key, string description, Location location)
        {
            _hasIssue = hasIssue;
            _key = key;
            _description = description;
            _location = location;
        }

        #endregion


        #region Properties

        /// <summary>
        /// The object that has this issue
        /// </summary>
        public ISavable ObjectWithIssue
        {
            get { return _hasIssue; }
        }

        /// <summary>
        /// Unique key for the issue (unique to the object having the issue)
        /// This allow us to distinguish issues where more an object has more than one issue.
        /// </summary>
        public string Key
        {
            get { return _key; }
        }

        /// <summary>
        /// Text about the issue
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// If the issue has location data this is the location, otherwise it is null
        /// </summary>
        public Location Location
        {
            get { return _location; }
        }

        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteObject(_hasIssue);
            writer.WriteString(_key);
            writer.WriteString(_description);
            writer.WriteObject(_location);
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _hasIssue = reader.ReadObject();
            _key = reader.ReadString();
            _description = reader.ReadString();
            _location = reader.ReadObject<Location>();
        }

        public void AfterReadStateV1()
        {
        }
        #endregion        
    }
}
