using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{

    /// <summary>
    /// Handles GameObjects that want to be deleted, but could not be deleted right away because there is an action
    /// going on that is using the object
    /// </summary>    
    public class ObjectsInLimboManager : ISavable
    {

        #region Member Vars

        /// <summary>
        /// List of all the objects waiting to be deleted
        /// (we save these deep because they will no longer be in the MasterObjectList when they are in here)
        /// </summary>
        private List<IGameObject> _objects = new List<IGameObject>();
                        
        /// <summary>
        /// Check every day to try to delete the objects that are waiting to be deleted
        /// </summary>
        private Notification _notification;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a ObjectsInLimboManager, call Setup or ReadState
        /// </summary>
        public ObjectsInLimboManager() { }

        /// <summary>
        /// Setup the task list
        /// </summary>
        public void Setup()
        {
            //listen to the clock to check again if objects can be deleted every few days
            _notification = Program.GameThread.Clock.RegisterNotification(TryToDeleteObjectsInLimbo, 0.5, true);
        }

        public void Delete()
        {
            //remove expire notification
            Program.GameThread.Clock.RemoveNotification(_notification);
        }

        #endregion
        
        #region Logic

        /// <summary>
        /// Add an object that could not be deleted.
        /// Objects will be removed again every time an attempt is made to deleted them, so they should be added back again if they could not be deleted
        /// </summary>
        public void AddObject(IGameObject obj)
        {
            _objects.Add(obj);            
        }


        /// <summary>
        /// Try to delete any object that were waiting to be deleted
        /// </summary>
        public void TryToDeleteObjectsInLimbo()
        {
            //copy then clear the objects list (objects will be added back if they could not be delted still)
            List<IGameObject> waitingToDelete = new List<IGameObject>(_objects);
            _objects.Clear();
            
            //try and delete the objects again
            foreach (IGameObject obj in waitingToDelete)
            {
                obj.Delete();
            }
        }

        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteObjectList<IGameObject>(_objects);
            writer.WriteNotification(_notification);
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _objects = reader.ReadObjectList<IGameObject>();
            _notification = reader.ReadNotification(TryToDeleteObjectsInLimbo);
        }

        public void AfterReadStateV1()
        {
        }
        #endregion
    }
}

