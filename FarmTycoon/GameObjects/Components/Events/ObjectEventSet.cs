using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// A set of events that can happen on a game object
    /// </summary>
    public class ObjectEventSet : ISavable
    {
        #region Member Vars

        /// <summary>
        /// list of all events that can take place on the object
        /// </summary>
        private List<ObjectEvent> _events = new List<ObjectEvent>();

        /// <summary>
        /// Info about the events in the set
        /// </summary>
        private IEventsInfo _eventsInfo;

        /// <summary>
        /// Game object whos events these are
        /// </summary>
        private IHasEvents _gameObject;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a EventSet.
        /// Setup or ReadState must be called after created.
        /// </summary>
        public ObjectEventSet() : base() { }
        
        /// <summary>
        /// Setup the eventt set for the object passed
        /// </summary>
        public void Setup(IEventsInfo eventsInfo, IHasEvents gameObject)
        {
            _eventsInfo = eventsInfo;
            _gameObject = gameObject;

            //create events
            foreach (ObjectEventInfo eventInfo in eventsInfo.Events.Events)
            {
                ObjectEvent newEvent = new ObjectEvent();
                newEvent.Setup(eventInfo, gameObject);
                _events.Add(newEvent);
            }

            //check all the events (do the ones that are currently met)
            foreach (ObjectEvent objectEvent in _events)
            {
                objectEvent.CheckEvent();
            }
        }
        
        /// <summary>
        /// Delete the trait set
        /// </summary>
        public void Delete()
        {
            foreach (ObjectEvent objEvent in _events)
            {
                objEvent.Delete();
            }
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// The evnets in the set
        /// </summary>
        public List<ObjectEvent> Events
        {
            get { return _events; }
        }
        
        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteObjectList<ObjectEvent>(_events);
            writer.WriteInfo(_eventsInfo);
            writer.WriteObject(_gameObject);
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _events = reader.ReadObjectList<ObjectEvent>();
            _eventsInfo = reader.ReadInfo<IEventsInfo>();
            _gameObject = reader.ReadObject<IHasEvents>();
        }

        public void AfterReadStateV1()
        {
            //remove any events that are no longer valid from the set
            foreach (ObjectEvent objectEvent in _events.ToArray())
            {
                if (objectEvent.EventInfo == null)
                {
                    _events.Remove(objectEvent);
                }
            }

            //if there are new events then add the new events
            if (_eventsInfo != null) //event could be null if entire crop/animal type deleted
            {
                HashSet<string> currentEvents = new HashSet<string>();
                foreach (ObjectEvent objectEvent in _events.ToArray())
                {
                    currentEvents.Add(objectEvent.EventInfo.Name);
                }
                foreach (ObjectEventInfo eventInfo in _eventsInfo.Events.Events)
                {
                    if (currentEvents.Contains(eventInfo.Name))
                    {
                        ObjectEvent newEvent = new ObjectEvent();
                        newEvent.Setup(eventInfo, _gameObject);
                        _events.Add(newEvent);
                    }
                }
            }
        }
        #endregion

    }
}
