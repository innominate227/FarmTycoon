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
        /// The worker we are currently following or null if were not following one
        /// </summary>
        private Worker _workerFollowing;

        #endregion

        #region Logic

        public void StopFollowing()
        {
            //if we were already not following do nothing
            if (_workerFollowing == null) { return; }
                        
            //delete the follow mover
            _followMover.Delete();
            _followMover = null;
            
            //remove from list of animals following worker, and set worker to null
            _workerFollowing.FollowingAnimals.Remove(this);
            _workerFollowing = null;
        }


        /// <summary>
        /// Start following the worker passed, or the last animal that is following the worker passed
        /// </summary>
        public void StartFollowing(Worker workerToFollow)
        {
            //no longer in a pasture
            _pastrue = null;

            //stop following if already following someone else
            if (_workerFollowing != null)
            {
                StopFollowing();
            }
            _workerFollowing = workerToFollow;

            if (workerToFollow.FollowingAnimals.Count == 0)
            {
                //if not animals currently following worker follow worker
                StartFollowing(workerToFollow.WorkerPosition);
            }
            else
            {
                //if animal currently following worker follow last animal
                StartFollowing(workerToFollow.FollowingAnimals[workerToFollow.FollowingAnimals.Count-1].Position);
            }

            //add self to list of following animals
            workerToFollow.FollowingAnimals.Add(this);
        }


        /// <summary>
        /// Start following the position manager passed, position manager should be for a worker or another aniaml
        /// </summary>
        private void StartFollowing(PositionManager whatToFollow)
        {
            //delete the action mover
            if (_actionMover != null)
            {
                _actionMover.FinishedAssignedSequence -= new Action<ActionSequence<Animal>>(ActionMover_FinishedAssignedSequence);
                _actionMover.Delete();
                _actionMover = null;
            }

            //if we are following something else stop following it
            if (_followMover != null)
            {                
                if (_followMover.PositionToFollow == whatToFollow)
                {
                    //if we are already following then do nothing
                    return;
                }
                else
                {
                    //delete current follower
                    _followMover.Delete();
                    _followMover = null;
                }
            }

            //create a mover for the animal to follow the position manager passed
            _followMover = new FollowMover();
            _followMover.Setup(_position, whatToFollow);
        }

        #endregion

        #region Save Load
        private void WriteStateV1Follow(StateWriterV1 writer)
        {
            writer.WriteObject(_workerFollowing);
        }

        private void ReadStateV1Follow(StateReaderV1 reader)
        {
            _workerFollowing = reader.ReadObject<Worker>();
        }
        #endregion
    }
}

