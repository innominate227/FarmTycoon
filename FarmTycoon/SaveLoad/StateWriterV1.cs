using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FarmTycoon
{

    /// <summary>
    /// Used to write the state of game objects
    /// </summary>
    public class StateWriterV1
    {
        /// <summary>
        /// Binary writer to write the actual state to disk
        /// </summary>
        private BinaryWriter _writer;

        /// <summary>
        /// Keep track of the id of any game object that has already had it state written
        /// </summary>
        private Dictionary<ISavable, int> _objToIdMap = new Dictionary<ISavable, int>();

        /// <summary>
        /// Mapping with the id of all Info objects
        /// </summary>
        private Dictionary<IInfo, int> _infoIds = new Dictionary<IInfo, int>();

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
        /// Create a new state writer that uses the binary writer proveded to write game state to disk
        /// </summary>        
        public StateWriterV1(BinaryWriter writer, FarmData farmData, int totalNumberOfGameObjects, Action<double> progressCallback)
        {
            _writer = writer;
            _totalNumberOfGameObjects = totalNumberOfGameObjects;
            _progressCallback = progressCallback;

            //wrtie table table of the unique names for all farm data objects, so we just have to write there ids later
            WriteFarmDataIdMappingTable(farmData);

            //write the number of game objects
            _writer.Write(totalNumberOfGameObjects);
        }

        /// <summary>
        /// Write a table that maps a farm data id to a unique name.
        /// The rest of the save is then able to just save the id instead of the full name
        /// </summary>
        private void WriteFarmDataIdMappingTable(FarmData farmData)
        {            
            _writer.Write(farmData.GetAllInfos().Count);
            foreach (IInfo info in farmData.GetAllInfos())
            {
                _infoIds.Add(info, _infoIds.Count);                
                _writer.Write(info.UniqueName);                
            }
        }
        
        /// <summary>
        /// Write an enumeration
        /// </summary>        
        public void WriteEnum(Enum val)
        {
            _writer.Write(val.ToString());
        }

        /// <summary>
        /// Write a string
        /// </summary>        
        public void WriteString(string val)
        {
            _writer.Write(val);
        }

        /// <summary>
        /// Write an int
        /// </summary>        
        public void WriteInt(int val)
        {
            _writer.Write(val);
        }

        /// <summary>
        /// Write a bool
        /// </summary>        
        public void WriteBool(bool val)
        {
            _writer.Write(val);
        }

        /// <summary>
        /// Write a float
        /// </summary>        
        public void WriteFloat(float val)
        {
            _writer.Write(val);
        }

        /// <summary>
        /// Write a double
        /// </summary>        
        public void WriteDouble(double val)
        {
            _writer.Write(val);
        }
        
        /// <summary>
        /// Write a list of objects of a type
        /// </summary>        
        public void WriteObjectList<T>(List<T> objs) where T : ISavable
        {
            _writer.Write(objs.Count);
            foreach (T obj in objs)
            {
                WriteObject(obj);
            }
        }
        
        /// <summary>
        /// Write an array of objects of a type
        /// </summary>        
        public void WriteObjectArray<T>(T[] objs) where T : ISavable
        {
            WriteObjectList(objs.ToList());
        }
        
        /// <summary>
        /// Write a saveable object.  Either a reference will be written (if the object has already been written) or just an id will be written.
        /// </summary>        
        public void WriteObject(ISavable obj)
        {
            if (obj == null)
            {
                //its a null object just write N to remeber it was a null reference
                _writer.Write('N');                
            }
            else if (_objToIdMap.ContainsKey(obj))
            {
                //if we have already written the object just wrtie a reference to the object
                _writer.Write('R');
                _writer.Write(_objToIdMap[obj]);
            }
            else
            {
                //get an id for the object
                int id = _objToIdMap.Count;
                _objToIdMap.Add(obj, id);

                //write the entire object
                _writer.Write('O');
                _writer.Write(id);                                
                _writer.Write(obj.GetType().FullName);                                
                obj.WriteStateV1(this);
                
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

            }
        }


        /// <summary>
        /// Write a reference to a Farm Info object.
        /// </summary>
        public void WriteInfo(IInfo info)
        {
            //write the id of the info object
            _writer.Write(_infoIds[info]);
        }


        /// <summary>
        /// Write the state of the notification
        /// </summary>
        public void WriteNotification(Notification notification)
        {
            Program.GameThread.Clock.WriteNotificationState(_writer, notification);
        }

    }
}
