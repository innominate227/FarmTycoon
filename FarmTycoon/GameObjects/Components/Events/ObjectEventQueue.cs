using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// A queue of evnets that are waiting to be processed by an object
    /// </summary>
    public partial class ObjectEventQueue : ISavable
    {
        #region Member Vars

        /// <summary>
        /// list of all events that are waiting to be processed
        /// </summary>
        private Queue<ObjectEvent> _queuedEvents = new Queue<ObjectEvent>();

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a ObjectEventQueue.        
        /// </summary>
        public ObjectEventQueue() : base() { }

        #endregion

        #region Logic

        /// <summary>
        /// A evebts conditions have been met, and it needs to be process by the object        
        /// </summary>
        public void EnqueueEvent(ObjectEvent objEvent)
        {
            //dont allow to many events to backlog
            if (_queuedEvents.Count > 20)
            {
                return;
            }

            //add event to queue
            _queuedEvents.Enqueue(objEvent);
        }


        /// <summary>
        /// Get the next evemt form the event queue or null if there are no more events
        /// </summary>
        public ObjectEvent DequeueEvent()
        {
            if (_queuedEvents.Count == 0)
            {
                return null;
            }
            else
            {
                return _queuedEvents.Dequeue();
            }
        }

        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteInt(_queuedEvents.Count);            
            foreach (ObjectEvent objEvent in _queuedEvents)
            {
                writer.WriteObject(objEvent);
            }
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            int queuedEventCount = reader.ReadInt();
            for (int eventNum = queuedEventCount-1; eventNum > 0; eventNum--)
            {
                ObjectEvent objEvent = reader.ReadObject<ObjectEvent>();
                _queuedEvents.Enqueue(objEvent);
            }
        }

        public void AfterReadStateV1()
        {
        }
        #endregion

    }
}
