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
        /// Dealys for the animal
        /// </summary>
        private DelaySet _delays;

        /// <summary>
        /// Moves the animal when it is doing actions, set to null while it is folowing a worker
        /// </summary>
        private ActionMover<Animal> _actionMover;

        /// <summary>
        /// Moves the animal when it is following a worker, set to null while it is doing actions
        /// </summary>
        private FollowMover _followMover;

        /// <summary>
        /// Pasture the animal is currently in, or null if its not in one
        /// </summary>
        private Pasture _pastrue;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Setup animal actions. (Called when animal is created)
        /// </summary>
        private void SetupActions()
        {
            //create queue for events
            _delays = new DelaySet();
            _delays.Setup(_animalInfo, this);
        }
        
        /// <summary>
        /// Delete the animals tiles, and movers
        /// </summary>
        private void DeleteActions()
        {
            //the animal should not be in a pasture when it is deleted
            Debug.Assert(_pastrue == null);            
            if (_actionMover != null)
            {
                _actionMover.FinishedAssignedSequence -= new Action<ActionSequence<Animal>>(ActionMover_FinishedAssignedSequence);
                _actionMover.Delete();
                _actionMover = null;
            }
            if (_followMover != null)
            {
                _followMover.Delete();
                _followMover = null;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Delays for the animal
        /// </summary>
        public DelaySet Delays
        {
            get { return _delays; }
        }

        #endregion
        
        #region Logic
        
        /// <summary>
        /// Called by Pasture when the animal is added into the pasture have it start doing actions in the pasture
        /// Start the animal doing actions in a pasture
        /// </summary>
        public void AddedToPasture(Pasture pasture)
        {
            //set the pasture we are in
            _pastrue = pasture;

            //this is a bit of a hack to get the cow to be in the correct position before switching them from follow mover to action mover.
            //When the cow that was following the worker is added it needs to change Going to be the workers going and its DestToGoing to 1, or
            //else it will appear to jump back before going into the pasture.  The others will be updated when the first one calls update position.
            if (_followMover.PositionToFollow.DistToGoing == 16)
            {
                _position.Leaving = _position.Going;
                _position.Going = _followMover.PositionToFollow.Going;
                _position.DistToGoing = 1;
                _position.SetWalkingDirection();
                _position.UpdatePosition();
            }

            //stop following the worker
            StopFollowing();
            
            //create an action mover to move the animal around
            _actionMover = new ActionMover<Animal>();
            _actionMover.Setup(_position, this);
            _actionMover.FinishedAssignedSequence += new Action<ActionSequence<Animal>>(ActionMover_FinishedAssignedSequence);
            _actionMover.StartMoving();

            //process the next event in the animals event queue (or wanders if there is nothing)
            DoNextEventInQueue();
        }
        
        /// <summary>
        /// Called once done with an action sequence
        /// </summary>
        private void ActionMover_FinishedAssignedSequence(ActionSequence<Animal> actionSequence)
        {
            //start working on the next event
            DoNextEventInQueue();
        }
                
        /// <summary>
        /// Attempt to plan and start an action sequence to fulfil a Consume event.
        /// Retruns true if action is started, or false if it can be started.
        /// </summary>
        private bool DoConsumeAction(ObjectEvent consumeEvent)
        {
            //make sure they passed the correct event type
            Debug.Assert(consumeEvent.EventInfo.EventType == ActionOrEventType.Consume);

            //get what trait we are going to adjust            
            TraitInfo toAdjustTraitInfo = _traits.GetTraitInfoUnsafe(consumeEvent.EventInfo.ConsumeTraitId);
            int desiredAdjustment = consumeEvent.EventInfo.ConsumeChange;
            if (desiredAdjustment == 0){ return false; }
            
            //find all the troughs in the pasture (sorted by nearest to the animal)
            List<Trough> nearByTroughs = GameState.Current.MasterObjectList.SortObjectsByDistance<Trough>(this.LocationOn, _pastrue.Troughs);

            //create action sequence to visit Troughs until we have adjusted the trait we want to adjust 
            ActionSequence<Animal> adjustTraitActionSequence = new ActionSequence<Animal>();

            //amount the trait will be adjusted based on what we are going to consume so far
            int adjustmentSoFar = 0;
            bool finishedAdjusting = false;

            //go through each trough in the list
            foreach (Trough trough in nearByTroughs)
            {
                //a list of items to consume from the trough
                ItemList toConsume = new ItemList();
                                
                //look at each item in the trough
                foreach (ItemType itemType in trough.Inventory.Types)
                {                    
                    //determine how much that item will adjust our trait by
                    int amountItemAdjusts = toAdjustTraitInfo.AmountItemEffectsTrait(itemType.BaseType);
                    //dont consume things that take us in the wrong direction (or do nothing)
                    if ((amountItemAdjusts == 0) || (amountItemAdjusts < 0 && desiredAdjustment > 0) || (amountItemAdjusts > 0 && desiredAdjustment < 0)) { continue; }
                    
                    //get the amount of that item type that is avaialble in the trough
                    int amountAvailable = trough.Inventory.GetTypeCountThatsFree(itemType);
                        
                    //how much more do we need to adjust the trait by
                    int amountLeftToAdjust = desiredAdjustment - adjustmentSoFar;

                    //how much more item do we need to eat to adjust it by that amount
                    int itemAmountToAdjustThatMuch = (int)Math.Ceiling(amountLeftToAdjust / (double)amountItemAdjusts);

                    //consume that amount that will cause the trait to be adjusted enough, or as much as there is
                    int amountToConsume = itemAmountToAdjustThatMuch;
                    if (amountToConsume > amountAvailable)
                    {
                        amountToConsume = amountAvailable;
                    }

                    //add to consume list
                    toConsume.IncreaseItemCount(itemType, amountToConsume);
                    
                    //keep track of how much we have adjusted the trait so far
                    adjustmentSoFar += (amountItemAdjusts * amountToConsume);

                    //check if we have adjusted enough
                    if ((desiredAdjustment < 0 && adjustmentSoFar <= desiredAdjustment) || (desiredAdjustment > 0 && adjustmentSoFar >= desiredAdjustment)) 
                    {
                        finishedAdjusting = true;
                        break; 
                    }
                }
                            

                //add an action to visit that trough (if we found items there to consume)
                if (toConsume.ItemTypes.Count > 0)
                {
                    adjustTraitActionSequence.AddAction(new VisitTroughAction(trough, toConsume));
                }

                //check if we have adjusted enough
                if (finishedAdjusting) { break; }
            }

            //if we found nothing to comsume then dont try and start an action
            if (adjustTraitActionSequence.Actions.Count == 0)
            {
                return false;
            }

            //abort current action (should be wandering, or doing nothing)
            Debug.Assert(_actionMover.CurrentAction is AnimalWanderAction || _actionMover.CurrentAction == null);
            _actionMover.AbortActionSequence();

            //start the action to visit the troughs
            _actionMover.DoActionSequence(adjustTraitActionSequence);

            //return that we started an action
            return true;
        }
        
        /// <summary>
        /// Plan and start an action to wander around the pasture
        /// </summary>
        private void DoWanderAction()
        {
            //if we are already doing to wander action do nothing
            if ((_actionMover.CurrentAction is AnimalWanderAction) || (_actionMover.CurrentActionSequence != null && _actionMover.CurrentActionSequence.Actions[0] is AnimalWanderAction)) { return; }

            ActionSequence<Animal> wanderActionSequence = new ActionSequence<Animal>();
            wanderActionSequence.AddAction(new AnimalWanderAction());
            _actionMover.DoActionSequence(wanderActionSequence);
        }
        
        /// <summary>
        /// Return true if we are doing an action that is not a wander action
        /// </summary>
        private bool CurrentlyDoingNonWanderAction()
        {
            return (((_actionMover.CurrentAction != null) && (_actionMover.CurrentAction is AnimalWanderAction) == false) ||
                    ((_actionMover.CurrentActionSequence != null) && (_actionMover.CurrentActionSequence.Actions[0] is AnimalWanderAction) == false));
        }

        /// <summary>
        /// Aborts the animals current action, the animal will do its next event in the event queue which could result in another action or a wander action
        /// </summary>
        public void AbortCurrentActionSequence()
        {
            //we are trying to abort an action, but we are currently following a worker, not doing an action
            Debug.Assert(_actionMover != null);

            //abort current action
            _actionMover.AbortActionSequence();

            //start the next event in the queue, or if there are none this will start the wander action
            DoNextEventInQueue();
        }


        /// <summary>
        /// Have the animal move to the entrance of the pasture it is in
        /// </summary>
        public void GoToPastureEntrance()
        {
            //make sure we are in a pasture
            Debug.Assert(_pastrue != null);

            //abort current action
            _actionMover.AbortActionSequence();

            //move to the entrance
            ActionSequence<Animal> moveToEntranceActionSequence = new ActionSequence<Animal>();
            moveToEntranceActionSequence.AddAction(new MoveToEntranceAction(_pastrue));
            _actionMover.DoActionSequence(moveToEntranceActionSequence);
        }

        /// <summary>
        /// If the animal was going to the entrance of the pasture have it abort that, and go back to normal tasks
        /// </summary>
        public void AbortGoingToPastureEntrance()
        {
            //abort move to entrance action
            _actionMover.AbortActionSequence();

            //do next event in queue (or wander)
            DoNextEventInQueue();
        }

        
        #endregion

        #region Save Load
        private void WriteStateV1Actions(StateWriterV1 writer)
        {
            writer.WriteObject(_delays);
            writer.WriteObject(_actionMover);
            writer.WriteObject(_followMover);
            writer.WriteObject(_pastrue);
        }

        private void ReadStateV1Actions(StateReaderV1 reader)
		{
			_delays = reader.ReadObject<DelaySet>();
			_actionMover = reader.ReadObject<ActionMover<Animal>>();
			_followMover = reader.ReadObject<FollowMover>();
			_pastrue = reader.ReadObject<Pasture>();
		}

        #endregion

    }
}

