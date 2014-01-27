using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// The master collection of gameobjects, and methods for querying the collection
    /// </summary>
    public class MasterObjectList : ISavable
    {
        #region Events

        /// <summary>
        /// Raised when an item is added passes the type of item
        /// </summary>
        public event Action<Type> ItemAdded;
        /// <summary>
        /// Raised when an item is removed passes the type of item
        /// </summary>
        public event Action<Type> ItemRemoved;

        #endregion

        #region Member Vars

        /// <summary>
        /// all the gameobjects in lists keyed by type
        /// </summary>
        private Dictionary<Type, HashSet<IGameObject>> _objectsByType = new Dictionary<Type, HashSet<IGameObject>>();

        /// <summary>
        /// all the gameobjects in the list
        /// </summary>
        private HashSet<IGameObject> _objects = new HashSet<IGameObject>();

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a new MasterObjectList
        /// </summary>
        public MasterObjectList() { }

        /// <summary>
        /// Delete all objects in the master object list
        /// </summary>
        public void DeleteAll()
        {
            //read the state for all objects of that type
            foreach (GameObject obj in _objects.ToArray())
            {
                //delete the object
                obj.Delete();
            }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Add a game object to the master object list
        /// </summary>
        public void Add(IGameObject obj)
        {
            _objects.Add(obj);

            Type objType = obj.GetType();
            foreach(Type interfaceType in objType.GetInterfaces())
            {
                if (_objectsByType.ContainsKey(interfaceType) == false)
                {
                    _objectsByType.Add(interfaceType, new HashSet<IGameObject>());
                }
                _objectsByType[interfaceType].Add(obj);
            }
            while (objType != null)
            {
                if (_objectsByType.ContainsKey(objType) == false)
                {
                    _objectsByType.Add(objType, new HashSet<IGameObject>());
                }
                _objectsByType[objType].Add(obj);

                objType = objType.BaseType;
            }

            if (ItemAdded != null)
            {
                ItemAdded(obj.GetType());
            }
        }

        /// <summary>
        /// Remove a game object from the master object list
        /// </summary>
        public void Remove(IGameObject obj)
        {
            _objects.Remove(obj);

            Type objType = obj.GetType();
            foreach (Type interfaceType in objType.GetInterfaces())
            {
                _objectsByType[interfaceType].Remove(obj);
            }
            while (objType != null)
            {
                _objectsByType[objType].Remove(obj);
                objType = objType.BaseType;
            }

            if (ItemRemoved != null)
            {
                ItemRemoved(obj.GetType());
            }
        }
        
        /// <summary>
        /// Find a GameObject of type T (or return null if none is found)
        /// </summary>
        public T Find<T>()
        {
            Type objType = typeof(T);
            
            //there is no method to get an abritary item from a hashset, this works im not sure how efficent it is (not sure how hashset enumerator works)
            if (_objectsByType.ContainsKey(objType))
            {
                foreach (GameObject obj in _objectsByType[objType])
                {
                    return (T)(object)obj;
                }
            }

            return default(T);
        }
        
        /// <summary>
        /// Find all GameObjects of type T (or return empty list if none is found).
        /// </summary>
        public List<T> FindAll<T>()
        {
            List<T> toRet = new List<T>();

            Type objType = typeof(T);
            if (_objectsByType.ContainsKey(objType))
            {
                foreach (GameObject obj in _objectsByType[objType])
                {                    
                    toRet.Add((T)(object)obj);                    
                }
            }

            return toRet;
        }

        /// <summary>
        /// Return the number of GameObjects of type T in the list.
        /// </summary>
        public int TypeCount<T>()
        {
            Type objType = typeof(T);
            if (_objectsByType.ContainsKey(objType))
            {
                return _objectsByType[objType].Count;                
            }
            return 0;
        }
        
        /// <summary>
        /// Returns true if there is at least 1 game object of type T in the collection
        /// </summary>
        public bool Contains<T>()
        {
            Type objType = typeof(T);
            if (_objectsByType.ContainsKey(objType) == false || _objectsByType[objType].Count == 0)
            {
                return false;
            }

            return true;
        }

        #region Nearest Objects

        /// <summary>
        /// Find game object closest to near that meets the predicate
        /// </summary>
        public T FindClosestObjectMeetingPredicate<T>(Location near, GameObjectPredicate<T> predicate) where T : IGameObject
        {
            List<T> gameObjects = FindClosestObjectsMeetingPredicate(near, predicate);
            if (gameObjects.Count == 0) { return default(T); }
            return gameObjects[0];
        }

        /// <summary>
        /// Find game objects close to near that meet the predicate
        /// </summary>
        public List<T> FindClosestObjectsMeetingPredicate<T>(Location near, GameObjectPredicate<T> predicate) where T : IGameObject
        {
            //collect all the objects to search
            List<T> allObjects = FindAll<T>();

            //the objects that meet the predicate
            List<T> objectsMeetingPredicate = new List<T>();

            //check object to see if it meets the predicate
            foreach (T gameObject in allObjects)
            {
                //check if the object meets the predicate
                if (predicate(gameObject))
                {
                    objectsMeetingPredicate.Add(gameObject);
                }
            }

            //return list of objects that meet the predicate sorted
            return SortObjectsByDistance(near, objectsMeetingPredicate);
        }

        /// <summary>
        /// Sort the game objects passed by their distance from the location passed. (Sorts the list passed)
        /// If the land passed is null the objects may return in any order.
        /// </summary>
        public List<T> SortObjectsByDistance<T>(Location distanceFrom, List<T> gameObjects) where T : IGameObject
        {
            //the distance from the land to each gameobject
            Dictionary<T, int> objectDistances = new Dictionary<T, int>();

            //determine the distance each gameobject is away from the land
            foreach (T gameObj in gameObjects)
            {
                //determine the disctance of the object
                int distanceAway = 1;
                if (distanceFrom != null)
                {
                    Location locationOfGameObject = gameObj.LocationOn;
                    //for game objects with an action location use that instead
                    if (gameObj is IHasActionLocation)
                    {
                        locationOfGameObject = (gameObj as IHasActionLocation).ActionLocation;
                    }
                    distanceAway = Program.Game.PathFinder.FindPathCost(distanceFrom, locationOfGameObject);                    
                }
                objectDistances.Add(gameObj, distanceAway);
            }

            Debug.Assert(gameObjects.Contains(default(T)) == false);

            //sort the game objects by distance            
            gameObjects.Sort(new Comparison<T>(delegate(T obj1, T obj2)
            {
                return objectDistances[obj1] - objectDistances[obj2];
            }));

            //return list of clost objects
            return gameObjects;
        }


        #endregion

        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteInt(_objects.Count);            
            foreach(GameObject obj in _objects)
            {
                writer.WriteObject(obj);
            }            
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            int numOfObjects = reader.ReadInt();
            for (int i = 0; i < numOfObjects; i++)
            {
                IGameObject obj = reader.ReadObject<IGameObject>();
                Add(obj);
            }
        }

        public void AfterReadStateV1()
        {
        }
        #endregion

    }
}
