using System;
//using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Base class for an action where the same thing is done at several locations.  For instance spraying several locations containing crops.
    /// </summary>
    public abstract class MultiLocationAction<T> : ActionBase<T> where T : IActor
    {
        #region Member Vars

        /// <summary>
        /// The gameobjects that we will be visting to preform the actions
        /// </summary>
        protected List<IHasActionLocation> _actionLocations;
                
        /// <summary>
        /// The index of land we are working on visiting.
        /// This is used while the action is actually running, so we know what actionland we are visiting
        /// </summary>
        private int _indexVisiting = 0;

        /// <summary>
        /// If the worker is using a tow, they need to go to the action land, but they wont actual do the action there, they will instead go one tile toward the
        /// next place they need to be, and then do the action, this way their tow is on the action land.  This will be true while they are going to the action land
        /// where they will not immedantly do the action
        /// </summary>
        private bool _extraMoveForTow = false;
        
        #endregion

        #region Setup

        /// <summary>
        /// Must be called in the constructor of inhertiing classes
        /// </summary>
        /// <param name="actionLocations"></param>
        protected void Setup(List<IHasActionLocation> actionLocations)
        {
            _actionLocations = actionLocations;

            //make sure we are acting on at least one location
            System.Diagnostics.Debug.Assert(actionLocations.Count > 0);
        }

        #endregion

        #region Abstract 
        
        /// <summary>
        /// Overriding class should return the amount of time that will be spent at each action location.
        /// Given either the expected delay set, or the actual delay set.
        /// </summary>
        public abstract double ExpectedTimeAtLocation(DelaySet delaySet);

        /// <summary>
        /// Called when the has arriveed at the destination and waited the correct amount of time. The worker should do the action there.
        /// </summary>
        protected abstract void DoActionAtLocation(IHasActionLocation arrivedAt);

        /// <summary>
        /// Called when the worker arrives at the location where they will do the action.
        /// Dont actually do the action yet, but apply textures or other pre-action things.
        /// </summary>
        public virtual void ArrivedAtLocation(IHasActionLocation arrivedAt) { }
        
        #endregion

        #region Logic

        public override Location FirstLocation()
        {
            return _actionLocations[0].ActionLocation;
        }

        public override Location LastLocation()
        {
            return _actionLocations[_actionLocations.Count - 1].ActionLocation;
        }

        public override double ExpectedTime(DelaySet expectedDelays)
        {            
            //time spent at each land tile
            double expectedTime = _actionLocations.Count * ExpectedTimeAtLocation(expectedDelays);

            if (_actionLocations.Count > 2)
            {
                //estimate time for walking between each tile in the action list.                  
                int distBetweenTwoActionLocations = Program.Game.PathFinder.FindPathCost(_actionLocations[0].ActionLocation, _actionLocations[1].ActionLocation);

                //assume all our about that far apart to set expected time                
                expectedTime += ((_actionLocations.Count - 1) * distBetweenTwoActionLocations * expectedDelays.GetDelay(ActionOrEventType.Move));
            }
                                    
            #region Slower More Exact Time
            ////time to walk between the tiles
            //for (int i = 1; i < _actionLand.Count; i++)
            //{
            //    Land land1 = _actionLand[i - 1];
            //    Land land2 = _actionLand[i];

            //    //add the time to get between the peices of land
            //    double timeBetweenLand;
            //    Program.PathFinder.FindPath(land1, land2, out timeBetweenLand);
            //    expextedTime += timeBetweenLand;
            //}
            #endregion
                        
            //return estimated time
            return expectedTime;
        }
        
        public override double ArrivedAtDestination(Location location)
        {
            //dont wait if this is the special extra move needed for each action when towing
            if (_extraMoveForTow) { return 0.0; }

            //let inherters do stuff on arrival
            ArrivedAtLocation(_actionLocations[_indexVisiting]);

            //otherwise wait the amount determined by the derived class
            return ExpectedTimeAtLocation(_actor.Delays);
        }
        
        protected override void AfterStarted()
        {
            //if worker is on a tow the first move he does will be the extra move for the tow            
            if (_actor is Worker && (_actor as Worker).Tow != null)
            {
                _extraMoveForTow = true;
            }
        }

        protected override Location NextLocationInnrer()
        {
            //check if no more action land to visit return null
            if (_indexVisiting == _actionLocations.Count)
            {
                return null;
            }

            if (_actor is Worker == false || (_actor as Worker).Tow == null)
            {
                //we are on foot visit the next peice of land
                Location nextLocation = _actionLocations[_indexVisiting].ActionLocation;
                Debug.Assert(nextLocation != null);
                return nextLocation;
            }
            else
            {
                //if not extra move for tow visits the next action land
                if (_extraMoveForTow)
                {
                    Location nextLocation = _actionLocations[_indexVisiting].ActionLocation;
                    Debug.Assert(nextLocation != null);
                    return nextLocation;
                }
                else
                {
                    //the land for where we do the actual action will be the land 1 unit in the direction of the next place we need to be
                                        
                    Location nextActionLocation = null;
                    if (_indexVisiting + 1 < _actionLocations.Count)
                    {
                        //next action land is the next action land is there is one
                        nextActionLocation = _actionLocations[_indexVisiting + 1].ActionLocation;
                    }
                    else
                    {
                        //if not and we are in an enclosure then go toward the entrance
                        Enclosure enclosure = _actionLocations[_indexVisiting].ActionLocation.Find<Enclosure>();
                        if (enclosure != null)
                        {
                            nextActionLocation = enclosure.EntryLand.LocationOn;
                        }
                    }

                    //get the path that leads to the next action location
                    Location nextWalkLocation = null;
                    if (nextActionLocation != null)
                    {                        
                        nextWalkLocation = Program.Game.PathFinder.FindPath(_actionLocations[_indexVisiting].ActionLocation, nextActionLocation, _actor);
                    }

                    //in rare case where there is no path just try and go to action land like normal
                    if (nextWalkLocation == null)
                    {
                        return _actionLocations[_indexVisiting].ActionLocation;
                    }

                    //return the second peice of land in the path (the first peice is the action land)
                    return nextWalkLocation;
                }
            }   
        }

        public override void DoLocationAction(Location arrivedAt)
        {
            //it was not an extra move for towing
            if (_extraMoveForTow == false)
            {
                //do the action at the land
                DoActionAtLocation(_actionLocations[_indexVisiting]);

                //increment land to visit
                _indexVisiting++;

                //if we have a tow the next move will be an extra move
                if (_actor is Worker && (_actor as Worker).Tow == null)
                {
                    _extraMoveForTow = true;
                }
            }
            else
            {
                //it was the extra move for the tow, the next will not be
                _extraMoveForTow = false;
            }             
        }


        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
		{
			base.WriteStateV1(writer);
			writer.WriteObjectList<IHasActionLocation>(_actionLocations);
			writer.WriteInt(_indexVisiting);
			writer.WriteBool(_extraMoveForTow);
		}

        public override void ReadStateV1(StateReaderV1 reader)
		{
			base.ReadStateV1(reader);
			_actionLocations = reader.ReadObjectList<IHasActionLocation>();
			_indexVisiting = reader.ReadInt();
			_extraMoveForTow = reader.ReadBool();
		}

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion

    }
}
