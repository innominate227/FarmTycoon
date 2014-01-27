using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// Used to read the state of game objects
    /// </summary>
    public class StateReaderV1
    {
        /// <summary>
        /// Binary reader to read the actual state from disk
        /// </summary>
        private BinaryReader _reader;

        /// <summary>
        /// Array of farm info indexed by their farm info id
        /// </summary>
        private IInfo[] _farmInfo;

        /// <summary>
        /// Array of all saveable object indexed by their id
        /// </summary>
        private Dictionary<int, ISavable> _idToObjMap = new Dictionary<int, ISavable>();
        
        /// <summary>
        /// The number of game objects that have been processed so far.
        /// we count the number of game objects processed because it is a good indicator of the percent of objects processed overall, and we easily know the total number of game objects.
        /// </summary>
        private int _numberOfGameObjectsProcessed;

        /// <summary>
        /// The total number of game objects
        /// </summary>
        private int _totalNumberOfGameObjects;

        /// <summary>
        /// Call back that reports overall progress.
        /// </summary>
        private Action<double> _progressCallback;


        /// <summary>
        /// Create a new state reader that uses the binary reader proveded to reader the game state from disk
        /// </summary>        
        public StateReaderV1(BinaryReader reader, FarmData farmData, Action<double> progressCallback)
        {
            _reader = reader;
            _progressCallback = progressCallback;

            //read the farm data id mapping table
            ReadFarmDataIdMappingTable(farmData);

            //read in the total number of game objects
            _totalNumberOfGameObjects = reader.ReadInt32();
        }
        
        /// <summary>
        /// Write a table that maps a farm data id to a unique name.
        /// The rest of the save is then able to just save the id instead of the full name
        /// </summary>
        private void ReadFarmDataIdMappingTable(FarmData farmData)
        {
            int idCount = _reader.ReadInt32();
            _farmInfo = new IInfo[idCount];
            for (int idNum = 0; idNum < idCount; idNum++)
            {
                string uniqueName = _reader.ReadString();
                _farmInfo[idNum] = farmData.GetInfo(uniqueName);
            }
        }

        /// <summary>
        /// Read an enumeration
        /// </summary>        
        public T ReadEnum<T>()
        {
            return (T)Enum.Parse(typeof(T), _reader.ReadString());
        }

        /// <summary>
        /// Read a string
        /// </summary>        
        public string ReadString()
        {
            return _reader.ReadString();
        }

        /// <summary>
        /// Read an int
        /// </summary>        
        public int ReadInt()
        {
            return _reader.ReadInt32();
        }

        /// <summary>
        /// Read an bool
        /// </summary>        
        public bool ReadBool()
        {
            return _reader.ReadBoolean();
        }

        /// <summary>
        /// Reader a float
        /// </summary>        
        public float ReadFloat()
        {
            return _reader.ReadSingle();
        }

        /// <summary>
        /// Write a double
        /// </summary>        
        public double ReadDouble()
        {
            return _reader.ReadDouble();
        }
        
        /// <summary>
        /// Read a list of objects of a type
        /// </summary>        
        public List<T> ReadObjectList<T>() where T : ISavable
        {
            List<T> ret = new List<T>();
            int count = _reader.ReadInt32();
            for (int objNum = 0; objNum < count; objNum++)
            {
                ret.Add(ReadObject<T>());
            }
            return ret;
        }
        
        /// <summary>
        /// Read an array of objects of a type
        /// </summary>        
        public T[] ReadObjectArray<T>() where T : ISavable
        {   
            return ReadObjectList<T>().ToArray();
        }
        
        /// <summary>
        /// Read a saveable object, and cast it to the type T
        /// </summary>        
        public T ReadObject<T>() where T : ISavable
        {
            return (T)ReadObject();
        }

        /// <summary>
        /// Read a saveable object.
        /// </summary>        
        public ISavable ReadObject()
        {
            //are we reading a refernce or the full object
            char writeType = _reader.ReadChar();

            if (writeType == 'N')
            {
                return null;
            }
            else if (writeType == 'R')
            {
                //read in the id of the object, then get the object from the lookup
                int id = _reader.ReadInt32();
                return _idToObjMap[id];
            }
            else if (writeType == 'O')
            {
                //read the id of the object
                int id = _reader.ReadInt32();

                //read in the type of object
                string objTypeString = _reader.ReadString();
                Type objType = Type.GetType(objTypeString);
                
                //create a new instance of the object
                ISavable obj = (ISavable)objType.GetConstructor(new Type[] { }).Invoke(new object[] { });

                //put the object in the mapping
                _idToObjMap.Add(id, obj);

                //read the state of the object
                obj.ReadStateV1(this);

                //we count the number of game objects processed because it is a good indicator of the percent of objects processed overall, and we easily know the total number of game objects.
                if (obj is IGameObject)
                {
                    //we have processed one more game object
                    _numberOfGameObjectsProcessed++;

                    //do progress callback
                    if (_progressCallback != null)
                    {
                        _progressCallback(_numberOfGameObjectsProcessed / (double)_totalNumberOfGameObjects);
                    }
                }
                
                //return the object
                return obj;
            }
            else
            {
                //should only be one of the three above
                Debug.Assert(false);
                return null;
            }
        }

        /// <summary>
        /// Read a FarmInfo object of type T
        /// </summary>
        public T ReadInfo<T>() where T:IInfo
        {
            return (T)ReadInfo();
        }
        
        /// <summary>
        /// Read a Info object
        /// </summary>
        public IInfo ReadInfo()
        {
            //read the name  of the info object
            int farmInfoId = _reader.ReadInt32();
            IInfo info = _farmInfo[farmInfoId];
            return info;
        }
        
        /// <summary>
        /// Read a notification, the notification will notifiy the method passed
        /// </summary>
        public Notification ReadNotification(TimeElapsedNotificationHandler method)
        {
            return Program.GameThread.Clock.ReadNotificationState(_reader, method);
        }

        /// <summary>
        /// Call after read state on all objects that were just read in by the state reader
        /// </summary>
        public void DoAfterReadState()
        {
            foreach (ISavable obj in _idToObjMap.Values)
            {
                obj.AfterReadStateV1();
            }
        }

    }
}
