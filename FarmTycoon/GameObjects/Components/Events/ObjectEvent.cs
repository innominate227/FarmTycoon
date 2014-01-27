using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// A event that can ocurr on an object when a set of conditions is met
    /// </summary>
    public class ObjectEvent : ISavable
    {
        #region Member Vars

        /// <summary>
        /// Info for the event
        /// </summary>
        private ObjectEventInfo _eventInfo;
        
        /// <summary>
        /// Gameobject that this event effects
        /// </summary>
        private IHasEvents _gameObject;
        
        /// <summary>
        /// Notification for checking the conditions of the event
        /// </summary>
        private Notification _notification;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a new ObjectEvent call Setup or ReadState before using
        /// </summary>
        public ObjectEvent()
        {
        }
        
        /// <summary>
        /// create a new ObjectEvent
        /// </summary>
        public void Setup(ObjectEventInfo eventInfo, IHasEvents gameObject)
        {
            _eventInfo = eventInfo;
            _gameObject = gameObject;
            _notification = Program.GameThread.Clock.RegisterNotification(CheckEvent, eventInfo.CheckRate, true);
        }
        
        /// <summary>
        /// Delete the desire object.
        /// Unhandel notifications it uses
        /// </summary>
        public void Delete()
        {            
            Program.GameThread.Clock.RemoveNotification(_notification);           
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Info on the Event
        /// </summary>
        public ObjectEventInfo EventInfo
        {
            get { return _eventInfo; }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Check if an event can be done, and do it if it can
        /// </summary>
        public void CheckEvent()
        {
            bool conditionsMet = CheckEventConditions();
            if (conditionsMet)
            {
                DoEvent();
            }
        }

        /// <summary>
        /// Check if the events conditions are met
        /// </summary>
        private bool CheckEventConditions()
        {            
            //check if all the conditions are met
            foreach (ConditionInfo condition in _eventInfo.Conditions)
            {
                //if the condition is not met return false
                if (condition.ConditionMet(_gameObject) == false)
                {
                    return false;
                }
            }

            //all conditions met
            return true;
        }

        /// <summary>
        /// Do the event.
        /// Should be called when the conditions of the event are met.
        /// </summary>
        private void DoEvent()
        {
            //set a temp texture based on the event
            if (_gameObject is IHasTextureManager)
            {
                (_gameObject as IHasTextureManager).TextureManager.SetTextureForActionOrEvent(_eventInfo.EventType);
            }

            //apply the event to the traits            
            _gameObject.Traits.ApplyActionOrEventToTraits(_eventInfo.EventType);
                        
            //have the object process this event
            _gameObject.ProcessEvent(this);
        }

        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteInfo(_eventInfo);
            writer.WriteObject(_gameObject);
            writer.WriteNotification(_notification);
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _eventInfo = reader.ReadInfo<ObjectEventInfo>();
            _gameObject = reader.ReadObject<IHasEvents>();
            _notification = reader.ReadNotification(CheckEvent);
        }

        public void AfterReadStateV1()
        {
            //if event is no loger valid remove the notifcation
            if (_eventInfo == null)
            {
                Program.GameThread.Clock.RemoveNotification(_notification);
            }
        }
        #endregion

    }
}
