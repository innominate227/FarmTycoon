using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Represents an x, y location on the map.  Contains a list of all game objects at that location..
    /// </summary>
    public partial class Location : ISavable
    {
        #region Member Vars

        /// <summary>
        /// Logical X position for the location.  
        /// This is the X position if you were to turn the game world by 45 degreess such that it was a square instead of a diamond.
        /// The North corner is 0,0;  The East is Max,0; The South corner is Max,Max;  The West is 0,Max;
        /// </summary>
        private int _x;
            
        /// <summary>
        /// Logical Y position for the location.  
        /// This is the T position if you were to turn the game world by 45 degreess such that it was a square instead of a diamond.
        /// The North corner is 0,0;  The East is Max,0; The South corner is Max,Max;  The West is 0,Max;
        /// </summary>
        private int _y;

        /// <summary>
        /// The Z position for the location.
        /// A location will have its Z position changed based based on the height of the land at the location.
        /// </summary>
        private int _z;


        /// <summary>
        /// Locations adjacent to this location in the 4 ordinal directions
        /// </summary>
        private Location[] _adjacent = new Location[4];
                
        /// <summary>
        /// All objects in this location (should be a very small number of items so List should be faster than Hashset here)
        /// </summary>
        private List<GameObject> _objects = new List<GameObject>();
        
        #endregion

        #region Setup

        /// <summary>
        /// Create a new location, call Setup, or ReadState before using
        /// </summary>
        public Location(){ }

        /// <summary>
        /// Create a new location object
        /// </summary>
        public void Setup(int x, int y)
        {
            _x = x;
            _y = y;

            FindNeighbors();
        }

        /// <summary>
        /// Set the adjcent array with the neighbors of this Location
        /// </summary>
        private void FindNeighbors()
        {
            _adjacent[(int)OrdinalDirection.NorthEast] = GameState.Current.Locations.GetLocation(_x, _y - 1);
            _adjacent[(int)OrdinalDirection.SouthEast] = GameState.Current.Locations.GetLocation(_x + 1, _y);
            _adjacent[(int)OrdinalDirection.SouthWest] = GameState.Current.Locations.GetLocation(_x, _y + 1);
            _adjacent[(int)OrdinalDirection.NorthWest] = GameState.Current.Locations.GetLocation(_x - 1, _y);
        }


        #endregion

        #region Properties

        /// <summary>
        /// Logical X position for the location.  
        /// This is the X position if you were to turn the game world by 45 degreess such that it was a square instead of a diamond.
        /// The North corner is 0,0;  The East is Max,0; The South corner is Max,Max;  The West is 0,Max;
        /// </summary>
        public int X
        {
            get { return _x; }        
        }

        /// <summary>
        /// Logical Y position for the location.  
        /// This is the T position if you were to turn the game world by 45 degreess such that it was a square instead of a diamond.
        /// The North corner is 0,0;  The East is Max,0; The South corner is Max,Max;  The West is 0,Max;
        /// </summary>
        public int Y
        {
            get { return _y; }
        }


        /// <summary>
        /// The Z position for the location.
        /// A location will have its Z position changed based based on the height of the land at the location.
        /// </summary>
        public int Z
        {
            get { return _z; }            
            set { _z = value; }
        }


        /// <summary>
        /// Get a location that is adjacent to this location in an cardinal direction
        /// </summary>
        public Location GetAdjacent(CardinalDirection direction)
        {
            OrdinalDirection dir1 = DirectionUtils.ClockwiseOneOrdinal(direction);
            OrdinalDirection dir2 = DirectionUtils.CounterClockwiseOneOrdinal(direction);

            return GetAdjacent(dir1).GetAdjacent(dir2);
        }

        /// <summary>
        /// Get a location that is adjacent to this location in an ordianl direction
        /// </summary>
        public Location GetAdjacent(OrdinalDirection direction)
        {
            return _adjacent[(int)direction];   
        }
        
        #endregion

        #region Logic

        /// <summary>
        /// Add a object to this location.
        /// </summary>
        public void AddObject(GameObject obj)
        {
            _objects.Add(obj);
        }

        /// <summary>
        /// Remove a object from this location.
        /// </summary>
        public void RemoveObject(GameObject obj)
        {
            _objects.Remove(obj);
        }
                
        /// <summary>
        /// Get all the objects in this location
        /// </summary>
        public List<GameObject> AllObjects
        {
            get { return _objects; }
        }

        /// <summary>
        /// Does the location contain type T
        /// </summary>
        public bool Contains<T>() where T : IGameObject
        {
            foreach (IGameObject obj in _objects)
            {
                if (obj is T)
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Return if the object passed is present at the location
        /// </summary>
        public bool ContainsObject(GameObject obj)
        {
            return _objects.Contains(obj);
        }
        
        /// <summary>
        /// Get an object in this location of type T
        /// </summary>
        public T Find<T>() where T : IGameObject
        {
            foreach (IGameObject obj in _objects)
            {
                if (obj is T)
                {
                    return (T)obj;
                }
            }
            return default(T);
        }

        /// <summary>
        /// Get all objects in this location of type T
        /// </summary>        
        public List<T> FindAll<T>() where T : IGameObject
        {
            List<T> objs = new List<T>();
            foreach (IGameObject obj in _objects)
            {
                if (obj is T)
                {
                    objs.Add((T)obj);
                }
            }
            return objs;
        }

        /// <summary>
        /// The effect that all objects in the location have on the path.
        /// This is the or-ing of the path effect of each object in the location
        /// </summary>
        public ObjectsEffectOnPath CumulativeEffectOnPath
        {
            get
            {
                ObjectsEffectOnPath locationEffects = ObjectsEffectOnPath.None;
                foreach (GameObject obj in _objects)
                {
                    locationEffects |= obj.PathEffect;
                }
                return locationEffects;
            }
        }

        #endregion

        #region Save Load
		public void WriteStateV1(StateWriterV1 writer)
		{
			writer.WriteInt(_x);
			writer.WriteInt(_y);
			writer.WriteInt(_z);
			writer.WriteObjectList<GameObject>(_objects);
            //we dont save neighbors, because 1 it is redundant we can just redetermine them and 2 it causes a stack overflow to save like that (each neighbor saves its neightbors and so on)
		}
		
		public void ReadStateV1(StateReaderV1 reader)
		{
			_x = reader.ReadInt();
			_y = reader.ReadInt();
			_z = reader.ReadInt();
			_objects = reader.ReadObjectList<GameObject>();
		}
		
		public void AfterReadStateV1()
		{
            FindNeighbors();
		}
		
		#endregion

    }
}
