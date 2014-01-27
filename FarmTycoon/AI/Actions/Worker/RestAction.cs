using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Has the worker go to the nearest rest house
    /// </summary>
    public partial class RestAction : ActionBase<Worker>
    {
        #region Member Vars

        /// <summary>
        /// BreakHouse we are going to (or are in)
        /// </summary>        
        private BreakHouse _breakHouse = null;

        /// <summary>
        /// Did we reserve a spot in the break house
        /// </summary>        
        private bool _reservedSpotInBreakHouse = false;

        /// <summary>
        /// Are we inside a break house
        /// </summary>
        private bool _insideBreakHouse = false;
        
        #endregion
        
        #region Setup

        public RestAction()
        {            
        }

        #endregion

        #region Logic

        /// <summary>
        /// Called after a worker has been assigned to do this action
        /// </summary>
        protected override void AfterAssigned()
        {
            FindBreakHouse();
        }
        
        /// <summary>
        /// Return the next location the worker will go to
        /// </summary>
        protected override Location NextLocationInnrer()
        {
            //we could not find a break house just stand still
            if (_breakHouse == null)
            {
                return _actor.LocationOn;
            }

            //go to the break house we found
            return _breakHouse.ActionLocation;
        }

        /// <summary>
        /// Worker has arrived, and finsihed waiting at that location it was moving to
        /// </summary>
        public override void DoLocationAction(Location location)
        {
            if (_reservedSpotInBreakHouse)
            {
                //if we are in a break house and resting apply rest action to worker
                _actor.ApplyActionOrEventToTraits(ActionOrEventType.Rest);
            }
            else
            {
                //othereise find if there is a break house for us to go to now               
                FindBreakHouse();
            }
        }

        /// <summary>
        /// Called when the worker arrives at its destination
        /// </summary>
        public override double ArrivedAtDestination(Location location)
        {
            if (_breakHouse != null && _insideBreakHouse == false)
            {
                //we were going toward a breakhouse, that we are not yet inside
                
                //we are inside now
                _insideBreakHouse = true;

                //enter the breakhouse (hide the worker)
                _actor.EnterBuilding(_breakHouse);

                //add to list of workers inside the break hosue
                _breakHouse.WorkersInside.AddWorker(_actor);                
    
                //remove from list of workers going toward the break house
                _breakHouse.WorkersInside.RemoveWorkerHeadingToward(_actor);                
            }
            
            //we either wait 1/4 and do a rest action at the end (if we are in the break house and have a space reserved)
            //or we will look for somewhere else after 1/4 day
            return 0.25;
        }

        /// <summary>
        /// When we abort the rest action (we leave the house) we need to lower the houses worker count.
        /// </summary>
        protected override void AfterAborted(bool wasStarted)
        {
            base.AfterAborted(wasStarted);

            //if were going toward the break house (but were no inside it yet)
            if (_breakHouse != null && _insideBreakHouse == false)
            {
                //remove from workers going toward
                _breakHouse.WorkersInside.WorkersHeadingToward.Remove(_actor);

                //if we had a spot reserved in the break house we were going toward we no longer do
                if (_reservedSpotInBreakHouse)
                {
                    _breakHouse.WorkersInside.WorkersWithSpotReserved.Remove(_actor);
                }
            }

            //NOTE: that if we were actually inside the break house (wether or not we had a spot reserved)
            //we will no actually exit the break house until a delay has passed.  See the Delay action for where the worker actually exits the break house.

        }

        
        /// <summary>
        /// Search for a breakhouse to go to.
        /// If one is found the worker will reserve a spot in it, and add itself to the list of workers heading toward it.       
        /// </summary>        
        private void FindBreakHouse()
        {
            //all break houses
            List<BreakHouse> allBreakHouses = GameState.Current.MasterObjectList.FindAll<BreakHouse>();

            //find all break houses with spots left
            List<BreakHouse> nonFullBreakHouses = new List<BreakHouse>();
            foreach (BreakHouse breakHouse in allBreakHouses)
            {
                if (breakHouse.WorkersInside.WorkersWithSpotReserved.Count < breakHouse.BreakHouseInfo.Capacity)
                {
                    nonFullBreakHouses.Add(breakHouse);
                }
            }
                        
            if (nonFullBreakHouses.Count > 0)
            {
                //there was at least one that was not full

                //get all break houses by distance
                List<BreakHouse> breakHouses = GameState.Current.MasterObjectList.SortObjectsByDistance(_actor.LocationOn, nonFullBreakHouses);

                //find the nearest non-full one
                foreach (BreakHouse house in breakHouses)
                {
                    if (house.WorkersInside.WorkersWithSpotReserved.Count < house.BreakHouseInfo.Capacity)
                    {

                        //if we were inside another breakhouse already we exit that one (if it is not the same one we just found space in)
                        if (_insideBreakHouse && _breakHouse != house)
                        {
                            _insideBreakHouse = false;
                            _breakHouse.WorkersInside.RemoveWorker(_actor);
                        }

                        //break house we are going to now
                        _breakHouse = house;
                                                                        
                        //we reserved a spot in it 
                        _reservedSpotInBreakHouse = true;
                        _breakHouse.WorkersInside.ReserveSpotFor(_actor);

                        //we will start heading toward it (if not already in it)
                        if (_insideBreakHouse == false)
                        {
                            _breakHouse.WorkersInside.AddWorkerHeadingToward(_actor);
                        }

                        //stop searching
                        break;
                    }
                }
            }
            else
            {
                //we could not find a non-full one


                if (_insideBreakHouse)
                {
                    //if we are already waiting at one keep waiting there
                    //(do nothing)                    
                }
                else
                {
                    //we are not already inside one

                    if (allBreakHouses.Count > 0)
                    {
                        //if there is at least one break house, go to one at random
                        _breakHouse = allBreakHouses[Program.Game.Random.Next(allBreakHouses.Count)];
                        _reservedSpotInBreakHouse = false;                        

                        //we will start heading toward it
                        _breakHouse.WorkersInside.AddWorkerHeadingToward(_actor);
                    }
                    else
                    {
                        //if not we will just go nowhere
                        _breakHouse = null;
                        _reservedSpotInBreakHouse = false;                        
                    }
                }
            }

            //if no break house had room that is an issue
            if (_reservedSpotInBreakHouse == false)
            {
                GameState.Current.IssueManager.ReportIssue(_actor, "BreakHouse", _actor.Name + " could not find a break house with room");
            }
            else
            {
                GameState.Current.IssueManager.ReportIssue(_actor, "BreakHouse", "");
            }

        }
                
        
        public override bool IsObjectInvolved(IGameObject obj)
        {
            return (obj == _breakHouse);
        }

        public override string Description()
        {
            return "Resting";
        }

        #endregion
        
        #region Not Called

        /// <summary>
        /// The first peice of land the worker should go to for this action
        /// </summary>
        public override Location FirstLocation()
        {
            //this should never be getting called on the rest action, since the idle action is never finished
            Debug.Assert(false);
            return null;
        }

        /// <summary>
        /// The last peice of land the worker should go to for this action
        /// </summary>
        public override Location LastLocation()
        {
            //this should never be getting called on the rest action, since the idle action is never finished
            Debug.Assert(false);
            return null;
        }

        public override double ExpectedTime(DelaySet expectedDelays)
        {
            //Expected tine should not be getting called on the rest action, becase it should not be part of a task
            Debug.Assert(false);
            return 0;
        }

        #endregion
        
        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteObject(_breakHouse);
            writer.WriteBool(_reservedSpotInBreakHouse);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _breakHouse = reader.ReadObject<BreakHouse>();
            _reservedSpotInBreakHouse = reader.ReadBool();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion

    }
}
