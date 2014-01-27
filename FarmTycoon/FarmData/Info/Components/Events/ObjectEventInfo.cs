using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace FarmTycoon
{
    /// <summary>
    /// Info on an ObjectEvent
    /// </summary>
    public class ObjectEventInfo : IInfo
    {
        /// <summary>
        /// Prepended to the unique name of all ObjectEventInfo
        /// </summary>
        public const string UNIQUE_PREPEND = "Event_";

        /// <summary>
        /// Name of the object does the event + the name of the event
        /// </summary>
        private string _fullName = "";

        /// <summary>
        /// Name of the event
        /// </summary>
        private string _name = "";

        /// <summary>
        /// How often the desire is checked in days 
        /// </summary>
        private double _checkRate = 1;

        /// <summary>
        /// The type of event that should ocurr
        /// </summary>
        private ActionOrEventType _eventType;
        
        /// <summary>
        /// Id of the trait that the animal will try to adjust by consuming
        /// (only relevent for Consume event type)
        /// </summary>
        private int _consumeTraitId = -1;

        /// <summary>
        /// Amount the animal will attempt to change the trait by when this event becomes active
        /// (only relevent for Consume event type)
        /// </summary>
        private int _consumeChange = 0;
        
        /// <summary>
        /// Conditions that should be met for the desire to be acted on
        /// </summary>
        private List<ConditionInfo> _conditions = new List<ConditionInfo>();



        public ObjectEventInfo(string parentName, XmlReader reader, FarmData farmInfo)
        {            
            reader.ReadToFollowing("Event");            

            //read trait of the event
            if (reader.MoveToAttribute("Name"))
            {
                _name = reader.ReadContentAsString();
                _fullName = parentName + "_" + _name;
            }
            if (reader.MoveToAttribute("CheckRate"))
            {
                _checkRate = reader.ReadContentAsDouble();
            }
            if (reader.MoveToAttribute("Type"))
            {
                string eventTypeString = reader.ReadContentAsString();
                _eventType = (ActionOrEventType)Enum.Parse(typeof(ActionOrEventType), eventTypeString);
            }
            if (reader.MoveToAttribute("ConsumeTrait"))
            {
                _consumeTraitId = reader.ReadContentAsTraitId(farmInfo);
            }
            if (reader.MoveToAttribute("ConsumeChange"))
            {
                _consumeChange = reader.ReadContentAsInt();
            }
            
                                    
            while (reader.ReadNextElement())
            {
                if (reader.Name == "Condition")
                {
                    ConditionInfo condition = ConditionInfo.ReadCondition(reader, farmInfo);
                    _conditions.Add(condition);
                }
            }
        }
        

        
        /// <summary>
        /// Name of the event
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        

        /// <summary>
        /// How often the desire is checked in days 
        /// </summary>
        public double CheckRate
        {
            get { return _checkRate; }
        }

        /// <summary>
        /// The type of event that should ocurr
        /// </summary>
        public ActionOrEventType EventType
        {
            get { return _eventType; }
        }

        /// <summary>
        /// Id of the trait that the animal will try to adjust by consuming
        /// (only relevent for Consume event type)
        /// </summary>
        public int ConsumeTraitId
        {
            get { return _consumeTraitId; }
        }

        /// <summary>
        /// Amount the animal will attempt to change the trait by when this event becomes active
        /// (only relevent for Consume event type)
        /// </summary>
        public int ConsumeChange
        {
            get { return _consumeChange; }
        }


        /// <summary>
        /// Conditions that should be met for the event to take place
        /// </summary>
        public List<ConditionInfo> Conditions
        {
            get { return _conditions; }
        }


        public string UniqueName
        {
            get { return UNIQUE_PREPEND + _fullName; }
        }
    }
}
