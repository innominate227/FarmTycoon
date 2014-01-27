using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace FarmTycoon
{
    /// <summary>
    /// A set of event info objects
    /// </summary>
    public class ObjectEventInfoSet
    {        
        /// <summary>
        /// The info object that owns this event info set
        /// </summary>
        private IEventsInfo _infoSetOwner;
        
        /// <summary>
        /// The events that effect what the object does
        /// </summary>
        private List<ObjectEventInfo> _events = new List<ObjectEventInfo>();
                

        /// <summary>
        /// Create a event info set, ReadElement needs to be called before the set is used.
        /// </summary>        
        public ObjectEventInfoSet(IEventsInfo infoSetOwner)
        {
            _infoSetOwner = infoSetOwner;
        }

        /// <summary>
        /// Read the element the xml reader is currently on, if the element is a delay add it to the delay info set
        /// </summary>        
        public void ReadElement(XmlReader reader, FarmData farmInfo)
        {
            if (reader.Name == "Event")
            {
                ObjectEventInfo objectEvent = new ObjectEventInfo(_infoSetOwner.UniqueName, reader.ReadSubtree(), farmInfo);
                _events.Add(objectEvent);
            }
        }



        /// <summary>
        /// The info object that owns this event info set
        /// </summary>
        public IEventsInfo InfoSetOwner
        {
            get { return _infoSetOwner; }
        }

        /// <summary>
        /// The desires that effect what the animal does
        /// </summary>
        public List<ObjectEventInfo> Events
        {
            get { return _events; }
        }
                

    }
}
