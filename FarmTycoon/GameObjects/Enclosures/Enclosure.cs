using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// An Enclosure is an area surrounded by fences.  
    /// Inside the enclosure will be either a Pasture, or a PlantedArea.
    /// A enclosure is defined by a list of land tiles that make it up, and the fences that surround that land.
    /// </summary>
    public class Enclosure : GameObject, IHasActionLocation
    {        
        #region Member Vars

        /// <summary>
        /// List of fences that surrond the enclosure
        /// </summary>
        private List<Fence> _borderFences;

        /// <summary>
        /// The fence that serves as the gate to get into the enclosure
        /// </summary>
        private Fence _gate;

        /// <summary>
        /// Entry land for the enclosure, this is the land just insdie the ensloure near the gate.
        /// </summary>
        private Land _entryLand;

        /// <summary>
        /// Exit land for the enclosure, this is the land just outside the ensloure near the gate.
        /// </summary>
        private Land _exitLand;

        /// <summary>
        /// List of all land in the enclosure.
        /// The list is ordered in such a way that walking between the land in order is an efficent way to walk to all land in the enclosrue.
        /// </summary>
        private List<Land> _allLandOn = new List<Land>();
        
        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a new enclosure call setup or load before using
        /// </summary>
        public Enclosure(): base() 
        {
        }
         
        /// <summary>
        /// Create a enslousre given the land the makes up the enslousre and the fences that surround it
        /// </summary>
        public virtual void Setup(List<Land> allLand, List<Fence> borderFences)
        {
            _allLandOn.AddRange(allLand);
            
            //location for the enclosure will be the same as the land it is on
            foreach (Land land in allLand)
            {
                AddLocationOn(land.LocationOn);
            }
                        
            //setup the fences
            _borderFences = borderFences;
            foreach (Fence borderFence in _borderFences)
            {
                borderFence.AddBorderingEnclosure(this);
            }

            //make one of the fences a gate            
            Gate = borderFences[0];     

            //enclosure object is not created until it is completly done so the it is immedantly considered done with placement
            this.DoneWithPlacement();    
        }

        /// <summary>
        /// Called when the enclosure is deleted
        /// </summary>
        protected override void DeleteInner()
        {
            //unhandle fence events
            if (_gate != null)
            {
                _gate.BorderingEnclosuresChanged -= new Action(FenceChanged);
            }
            else
            {
                foreach (Fence fence in _borderFences)
                {
                    fence.BorderingEnclosuresChanged -= new Action(FenceChanged);
                }
            }


            //tell all the fences arround this enclosure that they no longer border it
            foreach (Fence borderFence in _borderFences)
            {
                //the fence may have all ready been deleted if we are doing a Delete All right now (in that case dont worry about telling it anything)
                if (borderFence.PlacementState != FarmTycoon.PlacementState.Deleted)
                {
                    borderFence.RemoveBorderingEnclosure(this);

                    //if there is no other field arround it any more then delete the fence too
                    if (borderFence.EnclosuresBordered.Count == 0)
                    {
                        borderFence.Delete();
                    }
                }
            }
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// List of fences that surround this enclosure
        /// </summary>
        public List<Fence> BorderFences
        {
            get { return _borderFences; }
        }

        /// <summary>
        /// Entry land for the enclosure, this is the land just insdie the ensloure near the gate.
        /// </summary>
        public Land EntryLand
        {
            get { return _entryLand; }
        }

        /// <summary>
        /// Exit land for the enclosure, this is the land just outside the ensloure near the gate.
        /// </summary>
        public Land ExitLand
        {
            get { return _exitLand; }
        }
        
        /// <summary>
        /// Action Location for the enclosure, same as entry land
        /// </summary>
        public Location ActionLocation
        {
            get { return _entryLand.LocationOn; }
        }

        /// <summary>
        /// The fence that serves as the gate to get into the enclosure.
        /// Note: is this is set to a Fence that is not a valid gate another Fence will be chosen.
        /// </summary>
        public Fence Gate
        {
            get { return _gate; }
            set
            {
                //tell the old gate its not the gate anymore
                RemoveOldGate();

                //set the new gate
                _gate = value;

                //check that the gate is in a valid place
                ValidateGateLocation();

                //Tell the fence that is now the gate that its the gate, and update the entry land
                UpdateGate();
            }
        }

        /// <summary>
        /// List of all land in the enclosure.
        /// The list is ordered in such a way that walking between the land in order is an efficent way to walk to all land in the enclosrue.
        /// Note: this changes when the gate location changes.
        /// </summary>
        public List<Land> OrderedLand
        {
            get { return _allLandOn; }
        }

        #endregion

        #region Logic
                                
        #region Fences/Gate
            
        /// <summary>
        /// Make sure the gate is currently in a valid location.  If not moves the gate to a valid location.
        /// If there is no valid location gate will be null
        /// </summary>
        private void ValidateGateLocation()
        {
            //get the index of the location of the current gate
            int gateIndex = 0;                        
            if (_gate != null)
            {
                gateIndex = _borderFences.IndexOf(_gate);
            }            

            //if we get back to the index we started at and there is still no place for the gate then set it to null
            int firstGateIndex = gateIndex;

            //the gate cannot be place on a fence that two fields both border
            while (_gate.EnclosuresBordered.Count == 2)
            {
                //get the index of the next gate (wrap arround if nessisary)
                gateIndex++;
                if (gateIndex == _borderFences.Count)
                {
                    gateIndex = 0;
                }

                //if we tried all the fences and nothing can be the gate
                if (gateIndex == firstGateIndex)
                {
                    _gate = null;
                    break;
                }

                //try another fence as the gate
                _gate = _borderFences[gateIndex];
            }            
        }

        /// <summary>
        /// Tell the fence that is currently the gate (if there is such a fence) that it is no longer the gate
        /// </summary>
        private void RemoveOldGate()
        {
            if (_gate != null)
            {
                _gate.BorderingEnclosuresChanged -= new Action(FenceChanged);                
                _gate.Gate = false;                
            }
        }

        /// <summary>
        /// Tell the fence that is now the gate that its the gate, updates the entry land.
        /// </summary>
        private void UpdateGate()
        {
            //if a fence was chosen to be the gate
            if (_gate != null)
            {
                //set it to draw as a gate
                _gate.Gate = true;

                //need to know if the gate fence becomes adjacent to another field, because if so it will have to be moved
                _gate.BorderingEnclosuresChanged += new Action(FenceChanged);

                //determine the land at the entrance to the enclosure.
                //it going to be either the land the gate fence is on, or the land across from it, whichever one is not in the enclosure
                Land landGateOn = _gate.LandOn;
                Land landOnOtherSideOfGate = _gate.LandOn.GetAdjacent(_gate.SideOn);
                _entryLand = landGateOn;
                _exitLand = landOnOtherSideOfGate;
                if (_allLandOn.Contains(landOnOtherSideOfGate))
                {
                    _entryLand = landOnOtherSideOfGate;
                    _exitLand = landGateOn;
                }

                //redetermine a good order to walk the enclosure in
                DetermineOrderedLand();
            }
            else
            {
                _entryLand = null;
                _exitLand = null;

                //listen to all fences any changes we may be able to put the gate there
                foreach (Fence fence in _borderFences)
                {
                    fence.BorderingEnclosuresChanged += new Action(FenceChanged);
                }
            } 
        }
        
        /// <summary>
        /// Called when one of the fences changes the number of enclosures that border it.
        /// We need to make sure the location of our gate is still good, or if we dont have
        /// gate because there was no good place then we might have a good place for a gate now.
        /// </summary>
        private void FenceChanged()
        {
            if (_gate != null)
            {
                //make sure that the gate is still valid by setting it to itself
                //if its not valid anymore then when its set it will pick a different valid gate
                Gate = _gate;
            }
            else
            {
                //stop listening to all fences
                foreach (Fence fence in _borderFences)
                {
                    fence.BorderingEnclosuresChanged -= new Action(FenceChanged);
                }

                //there may be a valid spot for a gate now, set to the first fence, and if that one is not
                //valid a valid one will be chosen
                Gate = _borderFences[0];
            }
        }
        
        #endregion

        #region Land Ordering
            
        /// <summary>
        /// determine a good order to do work on the land in, starting from where the gate is and walking in a way that hugs the fence
        /// </summary>
        private void DetermineOrderedLand()
        {   
            //find the land thats right inside from the Gate                        
            Land landNearGateInField = _gate.LandOn;
            OrdinalDirection arrivingDirection = DirectionUtils.OppositeDirection(_gate.SideOn);
            if (_allLandOn.Contains(_gate.LandOn) == false)
            {
                landNearGateInField = _gate.LandOn.GetAdjacent(_gate.SideOn);
                arrivingDirection = _gate.SideOn;
            }
            
            //walk the field land in order to determine a good order for workers to go
            DetermineOrderedLandWalk(landNearGateInField, arrivingDirection);
        }

        /// <summary>
        /// Walk around enclosure land huging to the left, to create the order land list
        /// </summary>
        private void DetermineOrderedLandWalk(Land visit, OrdinalDirection walking)
        {
            //hash set of land we have visited so that we can quickly tell what we have visted or not
            HashSet<Land> vistedList = new HashSet<Land>();
            HashSet<Land> allLandOnSet = new HashSet<Land>(_allLandOn);

            //clear the current ordered land list
            _allLandOn.Clear();

            //this gets a stack over flow using recursion, so use stacks.
            Stack<Land> stack = new Stack<Land>();
            Stack<OrdinalDirection> dirStack = new Stack<OrdinalDirection>();
            stack.Push(visit);
            dirStack.Push(walking);

            while (stack.Count > 0)
            {
                visit = stack.Pop();
                walking = dirStack.Pop();

                //dont go this way if we have already visted this land
                if (vistedList.Contains(visit)) { continue; }

                //dont visit land thats not in the field
                if (allLandOnSet.Contains(visit) == false) { continue; }

                //add the land we arrived at
                _allLandOn.Add(visit);
                vistedList.Add(visit);

                //try and visit to the right last (were pushing this to a stack so the first one we push gets poped last)
                OrdinalDirection right = DirectionUtils.ClockwiseOne(walking);
                Land toRight = visit.GetAdjacent(right);
                stack.Push(toRight);
                dirStack.Push(right);

                //then try and visit straight in front                
                Land toFront = visit.GetAdjacent(walking);
                stack.Push(toFront);
                dirStack.Push(walking);

                //try and visit land to the left first
                OrdinalDirection left = DirectionUtils.CounterClockwiseOne(walking);
                Land toLeft = visit.GetAdjacent(left);
                stack.Push(toLeft);
                dirStack.Push(left);

                //no need to visit back the way we came from b/c we have already visited that land
            }
        }
        
        #endregion
        
        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteObjectList<Fence>(_borderFences);
            writer.WriteObject(_gate);
            writer.WriteObject(_entryLand);
            writer.WriteObject(_exitLand);
            writer.WriteObjectList<Land>(_allLandOn);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
			_borderFences = reader.ReadObjectList<Fence>();
			_gate = reader.ReadObject<Fence>();
			_entryLand = reader.ReadObject<Land>();
			_exitLand = reader.ReadObject<Land>();
			_allLandOn = reader.ReadObjectList<Land>();
		}

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();

            //create handler to listen for change to either all the fences, or just the gate
            if (_gate != null)
            {
                //need to know if the gate fence becomes adjacent to another field, because if so it will have to be moved
                _gate.BorderingEnclosuresChanged += new Action(FenceChanged);
            }
            else
            {
                //listen to all fences any changes we may be able to put the gate there
                foreach (Fence fence in _borderFences)
                {
                    fence.BorderingEnclosuresChanged += new Action(FenceChanged);
                }
            }
        }
        #endregion

    }
}
