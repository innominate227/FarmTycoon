using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    /// <summary>
    /// Updates a object/tiles position via the PositionManager to follow behind another Position Manager
    /// </summary>
    public partial class FollowMover : ISavable
    {
        #region Member Vars

        /// <summary>
        /// Position manager that controls the position for the tile/object being moved
        /// </summary>
        private PositionManager _positionManager;
                
        /// <summary>
        /// Position manager controling the object/tile we are following
        /// </summary>
        private PositionManager _positionToFollow;

        /// <summary>
        /// Have we started following or are we waiting for them to move far enough away still
        /// </summary>
        private bool _startedFollowing = false;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create an ActionMover
        /// </summary>        
        public FollowMover()
        {
        }
        
        /// <summary>
        /// Setup a follow mover.  Pass the position manager to update, and the position manager to follow
        /// </summary>
        public void Setup(PositionManager positionManager, PositionManager positionToFollow)
        {
            _positionManager = positionManager;
            _positionToFollow = positionToFollow;
            _positionToFollow.PositionUpdated += new Action(PositionToFollow_PositionUpdated);

            //start on the same position, dont start following until they have moved a whole tile away
            _positionManager.Going = _positionToFollow.Going;
            _positionManager.Leaving = _positionToFollow.Leaving;
        }

        public void Delete()
        {
            _positionToFollow.PositionUpdated -= new Action(PositionToFollow_PositionUpdated);
        }
        
        #endregion
        
        #region Properties

        /// <summary>
        /// Position manager we are following
        /// </summary>
        public PositionManager PositionToFollow
        {
            get { return _positionToFollow; }
        }

        #endregion

        #region Logic

        private void PositionToFollow_PositionUpdated()
        {
            //we are always the same distance between two tiles as who we are following
            _positionManager.DistToGoing = _positionToFollow.DistToGoing;

            //we should always be going to the location that who we are following is leaving
            //if we are not that means they started leaving a new location so we need to start going to that location and leaving the location we used to be going to
            if (_positionManager.Going != _positionToFollow.Leaving)
            {
                _positionManager.Leaving = _positionManager.Going;
                _positionManager.Going = _positionToFollow.Leaving;
                _positionManager.SetWalkingDirection();
                _startedFollowing = true;
            }

            //so long as we have started following update our position
            if (_startedFollowing)
            {
                _positionManager.UpdatePosition();
            }
        }

        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteObject(_positionManager);
            writer.WriteObject(_positionToFollow);
            writer.WriteBool(_startedFollowing);
        }

        public void ReadStateV1(StateReaderV1 reader)
		{
			_positionManager = reader.ReadObject<PositionManager>();
			_positionToFollow = reader.ReadObject<PositionManager>();
			_startedFollowing = reader.ReadBool();
		}

        public void AfterReadStateV1()
        {
            _positionToFollow.PositionUpdated += new Action(PositionToFollow_PositionUpdated);
        }
        #endregion

    }
}

