using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// Creates a maintains a list of all Location objects.  
    /// </summary>
    public class LocationManager : ISavable
    {
        #region Member Vars

        /// <summary>
        /// All location objects keyed by their x,y location
        /// </summary>
        private Location[][] _locations;
        
        /// <summary>
        /// Size of the map, if a location larger than size if requested it wraps around
        /// </summary>
        private int _size;
        
        #endregion

        #region Setup

        /// <summary>
        /// Create the LocationManager. Setup, or ReadState should be called before using
        /// </summary>
        public LocationManager() { }
        
        /// <summary>
        /// Create locations for the size passed.  This should only be called once after the location manager is first created.
        /// </summary>
        public void CreateLocations(int size)
        {
            _size = size;
            
            //create the locations
            _locations = new Location[size][];
            for (int x = 0; x < size; x++)
            {
                _locations[x] = new Location[size];
                for (int y = 0; y < size; y++)
                {
                    _locations[x][y] = new Location();                    
                }
            }

            //setup the locations            
            for (int x = 0; x < size; x++)
            {                
                for (int y = 0; y < size; y++)
                {
                    _locations[x][y].Setup(x, y);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Size of the map, if a location larger than size if requested it wraps around
        /// </summary>
        public int Size
        {
            get { return _size; }
        }

        
        /// <summary>
        /// Get a location
        /// </summary>
        public Location GetLocation(int x, int y)
        {
            if (x < 0) { x = _size + x; }
            if (y < 0) { y = _size + y; }
            if (x >= _size) { x = x - _size; }
            if (y >= _size) { y = y - _size; }

            //return the location            
            return _locations[x][y];
        }
        
        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {            
            writer.WriteInt(_size);
            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    writer.WriteObject(_locations[x][y]);
                }
            }
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _size = reader.ReadInt();
            _locations = new Location[_size][];
            for (int x = 0; x < _size; x++)
            {
                _locations[x] = new Location[_size];
                for (int y = 0; y < _size; y++)
                {
                    _locations[x][y] = reader.ReadObject<Location>();
                }
            }
        }

        public void AfterReadStateV1()
        {
        }

        #endregion

    }
}
