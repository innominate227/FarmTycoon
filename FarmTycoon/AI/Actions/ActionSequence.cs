using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{

    /// <summary>
    /// An ordered list of actions that a T will do.    
    /// </summary>    
    public class ActionSequence<T> : IActionSequence where T : IActor
    {
        #region Member Vars

        /// <summary>
        /// The list of actions in the sequence in order
        /// </summary>
        private List<ActionBase<T>> _actions = new List<ActionBase<T>>();

        #endregion

        #region Setup

        /// <summary>
        /// Create a new action sequence
        /// </summary>
        public ActionSequence()
        {            
        }

        #endregion

        #region Properties

        /// <summary>
        /// Read only list of actions in the sequence, use AddAction, and RemoveAction to edit the lsit of actions
        /// </summary>
        public IList<ActionBase<T>> Actions
        {
            get { return _actions.AsReadOnly(); }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Add an action to the end of the action sequence
        /// </summary>
        public void AddAction(ActionBase<T> action)
        {
            _actions.Add(action);
        }
        
        /// <summary>
        /// Get the expected time it will take to do all the actions in the action sequence.
        /// Assuming the actor has started at the start location for the first action
        /// </summary>
        public double ExpectedTime()
        {
            //if no actions it will take no time
            if (_actions.Count == 0)
            {
                return 0;
            }

            //find the location for the first action that has a location
            Location firstActionFirstLocation = null;
            foreach (ActionBase<T> action in _actions)
            {
                firstActionFirstLocation = action.FirstLocation();
                if (firstActionFirstLocation != null) { break; }
            }

            //note if all actions had null locations the first location may be null here.  
            //However Expected time will never actually use the location we pass since all actions have null locations. 
            //determine expected time
            return ExpectedTime(firstActionFirstLocation);
        }
        
        /// <summary>
        /// Get the expected time it will take to do all the actions in the action sequence.
        /// Assuming the actor has started on foot from the startLand
        /// </summary>
        public double ExpectedTime(Location startLocation)
        {
            //for the first action we are coming from the start location, for subsequent action this is the end location of the previous action
            Location comingFrom = startLocation;

            //worker to use to estimate dealys (could be null, but expected delays is able to handle that)
            Worker worker = GameState.Current.WorkerAssigner.NextWorkerThatWillBeAssigned;

            //keeep track of expected delays for things
            DelaySet expectedDelays = new DelaySet();
            expectedDelays.Setup(FarmData.Current.WorkerInfo, worker);

            //delay sets for the vehicle and tow we are going to have
            DelaySet currentVehicleDelays = null;
            DelaySet currentTowDelays = null;

            double totalTime = 0;
            foreach (ActionBase<T> action in _actions)
            {
                //add that time it will take to get to the action (if it has a specific location)
                Location actionFirstLocation = action.FirstLocation();
                if (actionFirstLocation != null)
                {
                    int weightedLength = Program.Game.PathFinder.FindPathCost(comingFrom, actionFirstLocation);
                    totalTime += (weightedLength * expectedDelays.GetDelay(ActionOrEventType.Move));
                }
             
                //add the time it will take to complete this action to the total                
                totalTime += action.ExpectedTime(expectedDelays);

                //set the land we are coming from to the last location for that action so we know how long it takes to get to the next action (only set if the action has a specific location)
                Location actionLastLocation = action.LastLocation();
                if (actionLastLocation != null)
                {
                    comingFrom = actionLastLocation;
                }
                
                //once it gets really huge just tell the player is huge
                if (totalTime > 360)
                {
                    totalTime = 360;
                    break;
                }

                //if we are geting equipment during this action modifiy expcted delays
                if (action is GetItemsAction)
                {
                    //see if any of the items we are getting are equipment
                    foreach (ItemType item in (action as GetItemsAction).GetList.ItemTypes)
                    {
                        //if there is an associated EquipmentType then we have found some equipment
                        EquipmentInfo equipmentInfo = FarmData.Current.GetEquipmentInfoForItemInfo(item.BaseType);
                        if (equipmentInfo != null)
                        {
                            if (equipmentInfo.IsVehicle)
                            {
                                currentVehicleDelays = new DelaySet();
                                currentVehicleDelays.Setup(equipmentInfo, null);
                                expectedDelays.AddEffectingDelaySet(currentVehicleDelays);
                            }
                            else
                            {
                                currentTowDelays = new DelaySet();
                                currentTowDelays.Setup(equipmentInfo, null);
                                expectedDelays.AddEffectingDelaySet(currentTowDelays);
                            }
                        }
                    }
                }

                //if we are puting equipment back during this action keep tract of what are puting back
                if (action is PutItemsAction)
                {
                    //see if any of the items we are putting are equipment
                    foreach (ItemType item in (action as PutItemsAction).PutList.ItemTypes)
                    {
                        //if there is an associated EquipmentType then we have found some equipment
                        EquipmentInfo equipmentInfo = FarmData.Current.GetEquipmentInfoForItemInfo(item.BaseType);
                        if (equipmentInfo != null)
                        {
                            if (equipmentInfo.IsVehicle)
                            {
                                expectedDelays.RemoveEffectingDelaySet(currentVehicleDelays);                                
                                currentVehicleDelays = null;
                            }
                            else
                            {
                                expectedDelays.RemoveEffectingDelaySet(currentTowDelays); 
                                currentTowDelays = null;
                            }
                        }
                    }
                }
            }

            return totalTime;
        }
        
        /// <summary>
        /// Inform the action sequence that it is being started by the actor passed.        
        /// </summary>
        public void Start(T worker)
        {
            //tell all the actions who is assigned to do them
            foreach (ActionBase<T> action in _actions)
            {
                action.AssignWorker(worker);
            }

            //add to the active action list
            GameState.Current.ActiveActionList.AddActiveActionSequence(this);
        }
        
        /// <summary>
        /// Inform the action sequence that the actor has aborted the sequence. 
        /// </summary>
        public void Abort()
        {
            //tell all the actions we aborted
            foreach (ActionBase<T> action in _actions)
            {
                action.Abort();
            }

            //do the smae things we would do if we had finished normally
            Finished();
        }
        
        /// <summary>
        /// Inform the action sequence that the actor has finished the sequence
        /// </summary>
        public void Finished()
        {
            //remove from the active action list
            GameState.Current.ActiveActionList.RemoveActionSequence(this);
        }
        
        /// <summary>
        /// Get the next action in the sequence, that comes after the action passed (or return null if its the last action in the plan)
        /// </summary>
        public ActionBase<T> GetNextAction(ActionBase<T> action)
        {
            int actionIndex = _actions.IndexOf(action);
            if (actionIndex == _actions.Count - 1)
            {
                return null;
            }
            else
            {
                return _actions[actionIndex + 1];
            }
        }

        /// <summary>
        /// Is the game object passed involved with any of the actions in the sequence
        /// </summary>
        public bool IsObjectInvolved(IGameObject obj)
        {
            foreach (ActionBase<T> action in _actions)
            {
                if (action.IsObjectInvolved(obj))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteObjectList<ActionBase<T>>(_actions);
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _actions = reader.ReadObjectList<ActionBase<T>>();
        }

        public void AfterReadStateV1()
        {
        }
        #endregion

    }
}
