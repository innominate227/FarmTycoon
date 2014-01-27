using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TycoonGraphicsLib;

namespace FarmTycoon
{

    public partial class Animal
    {
        #region Member Vars

        /// <summary>
        /// The events that drive the animals actions
        /// </summary>
        private ObjectEventSet _events;
        
        /// <summary>
        /// Events that are waiting to be done, either because we are not in a pasture or we are doing another event right now
        /// </summary>
        private ObjectEventQueue _eventQueue;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Setup animal events. (Called when animal is created)
        /// </summary>
        private void SetupEvents()
        {
            //create queue for events
            _eventQueue = new ObjectEventQueue();

            //create the event set for the animal
            _events = new ObjectEventSet();
            _events.Setup(_animalInfo, this);                        
        }

        private void DeleteEvents()
        {
            _events.Delete();            
        }

        #endregion
        
        #region Logic
        

        /// <summary>
        /// Do the next event waiting in the event queue if there is one.
        /// If there is not it will do a wander action
        /// </summary>
        private void DoNextEventInQueue()
        {
            //no one should be telling us to do event from the queue if we are not in a pasture
            Debug.Assert(_pastrue != null);

            //we should not be told get an event from the queue if we are doing a non-wander action
            Debug.Assert(CurrentlyDoingNonWanderAction() == false);

            //get the next event to try an complete
            ObjectEvent nextEvent = _eventQueue.DequeueEvent();
            while (nextEvent != null)
            {
                //try and process the event
                ProcessEvent(nextEvent);

                if (CurrentlyDoingNonWanderAction())
                {
                    //we found something to do stop searching
                    return;
                }
                else
                {
                    //that event did not cause a action to start so try to do the next event too
                    nextEvent = _eventQueue.DequeueEvent();
                }
            }

            //we went through the whole queue, and either there was nothing to do,
            //or we were unable to start doing it, so just wander
            DoWanderAction();
            
        }


        /// <summary>
        /// Do the event passed, or if it can not be done right now add it to the event queue        
        /// </summary>
        public void ProcessEvent(ObjectEvent objectEvent)
        {
            //if not in a pasture we will not be able to do any events until we get back into a pasture            
            if (_pastrue == null) 
            {
                _eventQueue.EnqueueEvent(objectEvent);
                return; 
            }

            //check the type of event it is
            if (objectEvent.EventInfo.EventType == ActionOrEventType.Die)
            {                
                //dont allow the animal to die if an action currently exsists where it is going to be used.
                if (GameState.Current.ActiveActionList.IsObjectInvolvedWithActiveAction(this) == false)
                {
                    //remove from pastures inventory if in pasture
                    if (_pastrue != null)
                    {                        
                        _pastrue.Inventory.RemoveFromInvetory(_animalItemType, 1);
                    }

                    //delete the animal
                    this.Delete();
                }
            }
            else if (objectEvent.EventInfo.EventType == ActionOrEventType.Baby)
            {
                //Make a baby                
                DoBabyEvent();
            }
            else if (objectEvent.EventInfo.EventType == ActionOrEventType.Consume)
            {
                //we are already working on another action (doing anything but wandering) we want to let that finish first, so queue the event
                if (CurrentlyDoingNonWanderAction())
                {
                    _eventQueue.EnqueueEvent(objectEvent);
                    return;
                }
                else
                {
                    //do an action to consume the nessisary items (if possible)
                    DoConsumeAction(objectEvent);
                }                
            }
            else
            {
                //we are supposed to do an event we dont know how to do
                Debug.Assert(false);
            }
        }
              

        /// <summary>
        /// Do the baby evemt (create a new Animal of this type, and add to the pasture)
        /// </summary>
        private void DoBabyEvent()
        {
            //create the baby and add it to the same pasture
            Animal baby = new Animal();
            ItemType babyItemType = GameState.Current.ItemPool.GetNewItemType(_animalItemType.BaseName);
            baby.Setup(_animalInfo, babyItemType, this.LocationOn);
            _pastrue.Inventory.AddToInvetory(babyItemType, 1);            
        }
        
        #endregion

        #region Save Load
        private void WriteStateV1Events(StateWriterV1 writer)
        {
            writer.WriteObject(_events);
            writer.WriteObject(_eventQueue);
        }

        private void ReadStateV1Events(StateReaderV1 reader)
		{
			_events = reader.ReadObject<ObjectEventSet>();
			_eventQueue = reader.ReadObject<ObjectEventQueue>();
		}

        #endregion

    }
}

