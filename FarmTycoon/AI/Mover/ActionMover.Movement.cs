using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    public partial class ActionMover<T> : ISavable
    {
        #region Member Vars

        /// <summary>
        /// Position manager that controls the position for the tile/object being moved
        /// </summary>
        private PositionManager _positionManager;
        
        /// <summary>
        /// The detination the object is moving to
        /// </summary>
        private Location _destination;
        
        /// <summary>
        /// Is the object currently waiting (not moving)
        /// </summary>
        private bool _waiting = false;
                        
        /// <summary>
        /// true if the object is unable to read the next desitnation
        /// </summary>
        private bool _unableToReachDestination = false;
                
        /// <summary>
        /// The notification the object moves on each notification
        /// </summary>
        private Notification _notification;

        #endregion
        
        #region Delete

        /// <summary>
        /// Call before unreferencing the movement manager
        /// </summary>
        public void Delete()
        {
            //if we are doing an action sequence abort it
            AbortActionSequence();

            //we only regiter for notification once the worker is actually placed so this may be null
            if (_notification != null)
            {
                Program.GameThread.Clock.RemoveNotification(_notification);
            }

            //drop any cached paths
            Program.Game.PathFinder.RemoveTraveller(_actor);
        }

        #endregion

        #region Logic

        /// <summary>
        /// Start moving the object
        /// </summary>
        public void StartMoving()
        {
            //set the notification interval to the workers movement delay
            double moveDelay = _actor.Delays.GetDelay(ActionOrEventType.Move);
            _notification = Program.GameThread.Clock.RegisterNotification(TimeNotification, moveDelay, false);

            //move once, so everything get set correctly
            Move();
        }


        /// <summary>
        /// Abort trying to go to the current destination
        /// Also if the object was waiting, have it abort that wait, and start moving again
        /// </summary>
        private void AbortDestination()
        {
            _destination = null;

            if (_waiting)
            {
                //stop waiting
                _waiting = false;

                //set the notification interval back to the workers movement delay
                double moveDelay = _actor.Delays.GetDelay(ActionOrEventType.Move);
                _notification = Program.GameThread.Clock.UpdateNotification(_notification, moveDelay);
            }
        }  


        /// <summary>
        /// Called by the clock when an amount of time has pased, based on speed that it needs to move.
        /// Or when waiting that the waiting time has passed
        /// </summary>
        private void TimeNotification()
        {
            //if we were waiting we, are not longer waiting, update the interval to the new speed
            if (_waiting)
            {
                _waiting = false;

                //we are done waiting at the desintion, get the next destination
                _destination = FinishedWaitingAtDestination(_positionManager.Going);                

                //set the notification interval back to the workers movement delay
                double moveDelay = _actor.Delays.GetDelay(ActionOrEventType.Move);
                _notification = Program.GameThread.Clock.UpdateNotification(_notification, moveDelay);
            }
            else
            {
                Move();
            }
        }


        /// <summary>
        /// move the worker 1/16 of the way between two tiles
        /// </summary>
        private void Move()
        {
            //one more step toward the destination land
            _positionManager.DistToGoing += 1;
            if (_positionManager.DistToGoing > 16)
            {
                //if we reached the destination land then reset the count
                _positionManager.DistToGoing = 1;

                //determine the location we should go to next (we may also end up determining that we need to wait at the current land for some amount of time)
                bool locationDetermined = DetermineNextLocation();

                //if were unable to determine the next location to go to or we started waiting then dont move, also setting _distToDest to 16 to 
                //force recalculating where to go preventing a double move when we start moving again
                if (locationDetermined == false)
                {
                    _positionManager.DistToGoing = 16;
                    return;
                }
            }

            //update the position of objects tile
            Tile.StartChangeSet();
            _positionManager.UpdatePosition();
            Tile.EndChangeSet();
        }
        

        /// <summary>
        /// Determine the next location to go to, and update leaving and going based on the next location.
        /// Return true if the next location was updated, or false if the worker does not have a next location, or cant get there, or is waiting.        
        /// </summary>
        private bool DetermineNextLocation()
        {

            //if we dont have a destination check for a destination
            if (_destination == null)
            {
                _destination = CheckForDestination();
            }
            
            //if we got to the destination we need to raise the ArriveAtDestination event
            //the event may send us back to the same destination in which case we should raise the event again
            while (_destination == _positionManager.Going)
            {
                //we arrived at destination, determine how long we need to wait there
                double waitTime = ArrivedAtDestination(_positionManager.Going);
                                
                //when we arrive at a destination we might have started waiting, if we did then dont try and determine the next location
                if (waitTime > 0)
                {
                    //we are waiting now
                    _waiting = true;

                    //set the notification interval to the wait time
                    _notification = Program.GameThread.Clock.UpdateNotification(_notification, waitTime);

                    //we did not determine a next location
                    return false;
                }
                else
                {
                    //there was no wait time so we are done waiting, get the next destination                    
                    _destination = FinishedWaitingAtDestination(_positionManager.Going);
                }
            }
            
            //if a new destination has been set
            if (_destination != null)
            {
                //if there is a destination, then the object should move toward that destination

                //we should not get here id the detinstion is the game as where we are going (that case should have been handled above)
                Debug.Assert(_positionManager.Going != _destination);

                //find next location on the path to the destination
                Location nextLocationOnPathToDestination = Program.Game.PathFinder.FindPath(_positionManager.Going, _destination, _actor);
                
                if (nextLocationOnPathToDestination == null)
                {
                    //object cant reach the destination
                    _unableToReachDestination = true;
                    GameState.Current.IssueManager.ReportIssue(_actor, "Cant Reach", _actor.Name + " cannot reach destination.:" + _destination.X + "," + _destination.Y + "," + _destination.Z);
                }
                else
                {
                    _unableToReachDestination = false;
                    GameState.Current.IssueManager.ClearIssue(_actor, "Cant Reach");

                    //we are leaving where we were going before, and update where we were going                                        
                    _positionManager.Leaving = _positionManager.Going;
                    _positionManager.Going = nextLocationOnPathToDestination;
                    _positionManager.SetWalkingDirection();
                }
            }

            //if there is not a destination based on the task (or the worker can not reach it), then
            if (_destination == null || _unableToReachDestination)
            {
                if (_unableToReachDestination)
                {
                    //to determine that we are unable to reach a destination we do a search on the whole map which is very slow.  
                    //Raise the notification interval to stop the game from lagging to a halt while a path cannot be found                    
                    _notification = Program.GameThread.Clock.UpdateNotification(_notification, 1.0);
                }

                //we did not determine the next location.  We will try again on the next notification.
                return false;
            }
            
            //get the movement delay interval for the worker
            double movementDelay = _actor.Delays.GetDelay(ActionOrEventType.Move);

            //check both locations for path effect that increase travel speed
            bool goingDoWalkPlus = _positionManager.Going.CumulativeEffectOnPath.HasFlag(ObjectsEffectOnPath.DoWalkPlus);
            bool leavingDoWalkPlus = _positionManager.Leaving.CumulativeEffectOnPath.HasFlag(ObjectsEffectOnPath.DoWalkPlus);
            bool goingDoWalk = _positionManager.Going.CumulativeEffectOnPath.HasFlag(ObjectsEffectOnPath.DoWalk);
            bool leavingDoWalk = _positionManager.Leaving.CumulativeEffectOnPath.HasFlag(ObjectsEffectOnPath.DoWalk);

            if (goingDoWalkPlus && leavingDoWalkPlus)
            {
                //full speed
            }
            else if ((goingDoWalkPlus || goingDoWalk) && (leavingDoWalkPlus || leavingDoWalk))
            {
                //half speed
                movementDelay *= 2.0;
            }
            else
            {
                //very slow speed
                movementDelay *= 4.0;
            }
                        
            //changed notification to have the new delay            
            _notification = Program.GameThread.Clock.UpdateNotification(_notification, movementDelay);
            
            //we did find the next location to go to.
            return true;
        }

        #endregion
        

        #region Save Load
        private void WriteStateV1Movement(StateWriterV1 writer)
		{
			writer.WriteObject(_positionManager);
			writer.WriteObject(_destination);
			writer.WriteBool(_waiting);
			writer.WriteBool(_unableToReachDestination);
			writer.WriteNotification(_notification);
		}

        private void ReadStateV1Movement(StateReaderV1 reader)
		{
			_positionManager = reader.ReadObject<PositionManager>();
			_destination = reader.ReadObject<Location>();
			_waiting = reader.ReadBool();
			_unableToReachDestination = reader.ReadBool();
            _notification = reader.ReadNotification(TimeNotification);
		}

        #endregion

    }
}

