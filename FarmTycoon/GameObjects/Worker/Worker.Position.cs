using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TycoonGraphicsLib;

namespace FarmTycoon
{

    public partial class Worker
    {
        #region Member Vars

        /// <summary>
        /// The main tile for the worker
        /// </summary>
        private MobileGameTile _workerTile;

        /// <summary>
        /// The tile for the object being towed by the worker
        /// </summary>
        private MobileGameTile _towTile;

        /// <summary>
        /// The position of the worker, and the main worker tile.
        /// </summary>
        private PositionManager _workerPosition;

        /// <summary>
        /// The position of the tow the worker is pulling
        /// </summary>
        private PositionManager _towPosition;

        /// <summary>
        /// The moves the workers position in order to complete the actions in an action sequence.
        /// </summary>
        private ActionMover<Worker> _workerMover;

        /// <summary>
        /// The position of the tow the worker is pulling
        /// </summary>
        private FollowMover _towMover;

        /// <summary>
        /// Texture manager for the worker (when it is not in a vehicle)
        /// </summary>
        private TextureManager _textureManager;

        /// <summary>
        /// Set to the building a worker is inside, while a worker is inside a building, otherwise set to null
        /// </summary>
        private IHoldsWorkers _buildingInside = null;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Setup worker position. (Called when worker is created, after init tiles)
        /// </summary>
        private void SetupPosition()
        {
            //create manager for the position of the worker, managers the worker tile, and the worker game objects position
            _workerPosition = new PositionManager();
            _workerPosition.Setup(LocationOn, this);
            
            //create manager for the position of the tow, managers the tow tiles position, but not game object
            //(the Worker game objects LocationOn will not include the position for the tow, but I think that should be ok)
            _towPosition = new PositionManager();
            _towPosition.Setup(LocationOn);

            //action mover to move the worker game object, and tile on a path determine by an ActionSequence
            _workerMover = new ActionMover<Worker>();
            _workerMover.Setup(_workerPosition, this);
            _workerMover.FinishedAssignedSequence += new Action<ActionSequence<Worker>>(FinishedAssignedActionSequence);

            //tow mover set up to move the tow on a path such that it follows the worker.
            _towMover = new FollowMover();
            _towMover.Setup(_towPosition, _workerPosition);

            //setup texture manager for the worker
            _textureManager = new TextureManager();
            _textureManager.Setup((WorkerInfo)FarmData.Current.GetInfo(WorkerInfo.UNIQUE_NAME), this);

            //setup the tiles for the worker and the tow
            SetupTiles();

            //update position of the worker tile
            _workerPosition.UpdatePosition();
        }
        
        /// <summary>
        /// Create the workers tiles
        /// </summary>        
        private void SetupTiles()
        {
            //create tile for the rendering the worker
            _workerTile = new MobileGameTile(this);
            _workerTile.ForcedLayering = ForcedLayerType.InFront; //force worker tiles to be in front of crop tiles            
            _workerPosition.SetTileToMove(_workerTile);
            if (_vehicle != null)
            {
                _vehicle.TextureManager.SetTileToUpdate(_workerTile);
            }
            else
            {
                _textureManager.SetTileToUpdate(_workerTile);
            }
            
            //create tile for the rendering the tow
            _towTile = new MobileGameTile(this);
            _towTile.ForcedLayering = ForcedLayerType.InFront; //force worker tiles to be in front of crop tiles            
            _towPosition.SetTileToMove(_towTile);
            if (_tow != null)
            {
                _tow.TextureManager.SetTileToUpdate(_towTile);
                _towTile.Hidden = false;
            }
            else
            {
                _towTile.Hidden = true;
            }

            UpdateTiles();
        }
        
        /// <summary>
        /// Called when worker is delted
        /// </summary>
        private void DeletePosition()
        {
            _workerMover.Delete();
            _towMover.Delete();
            _workerTile.Delete();
            _towTile.Delete();            
            _textureManager.Delete();
        }
        
        #endregion

        #region Propeties

        /// <summary>
        /// The position of the worker, and the main worker tile.
        /// </summary>
        public PositionManager WorkerPosition
        {
            get { return _workerPosition; }
        }
        
        /// <summary>
        /// Texture manager for the worker (when it is not in a vehicle)
        /// </summary>
        public TextureManager TextureManager
        {
            get { return _textureManager; }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Refresh the workers tiles
        /// </summary>
        public override void UpdateTiles()
        {
            if (_vehicle != null)
            {
                _vehicle.TextureManager.Refresh();
            }
            else
            {
                _textureManager.Refresh();
            }
            if (_tow != null)
            {
                _tow.TextureManager.Refresh();
            }
            
            _workerPosition.UpdatePosition();
            _towPosition.UpdatePosition();
        }


        /// <summary>
        /// Have the worker enter the building passed 
        /// Note: the worker is not added to the buildings worker list by this method, this method just hides the worker tile, and sets the BuildingInside property to the building passed)
        /// </summary>
        public void EnterBuilding(IHoldsWorkers building)
        {
            //set the building the worker is inside            
            _buildingInside = building;

            //hide tile
            _workerTile.Hidden = true;
            _workerTile.Update();
            _towTile.Hidden = true;
            _towTile.Update();            
        }


        /// <summary>
        /// Have the worker exit the building it is in
        /// Note: the worker is not removed from the buildings worker list by this method, this method just shows the worker tile, and sets the BuildingInside property to null)
        /// </summary>
        public void ExitBuilding()
        {
            //no longer inside the building        
            _buildingInside = null;
            
            //show worker tile
            _workerTile.Hidden = false;
            _workerTile.Update();
            if (_tow != null)
            {
                _towTile.Hidden = false;
                _towTile.Update();
            }
        }

        /// <summary>
        /// If the worker is inside a building then that building. Otherwise null.
        /// </summary>
        public IHoldsWorkers BuildingInside
        {
            get { return _buildingInside; }
        }
        



        /// <summary>
        /// Passes the action or event type to the worker, or the vechle and tow depending on who is in charge of the textures right now
        /// </summary>
        public void SetTextureForActionOrEvent(ActionOrEventType actinoOrEventType)
        {
            if (_vehicle == null)
            {
                _textureManager.SetTextureForActionOrEvent(actinoOrEventType);
            }
            else
            {
                _vehicle.TextureManager.SetTextureForActionOrEvent(actinoOrEventType);
            }
            if (_tow != null)
            {
                _tow.TextureManager.SetTextureForActionOrEvent(actinoOrEventType);
            }            
        }

        /// <summary>
        /// Clears the action texture that was last sent to the worker, or the vechle and tow depending on who is in charge of the textures right now
        /// </summary>
        public void ClearTextureForActionOrEvent()
        {
            if (_vehicle == null)
            {
                _textureManager.ClearTextureForActionOrEvent();
            }
            else
            {
                _vehicle.TextureManager.ClearTextureForActionOrEvent();
            }
            if (_tow != null)
            {
                _tow.TextureManager.ClearTextureForActionOrEvent();
            }
        }
        #endregion

        #region Save Load
        private void WriteStateV1Position(StateWriterV1 writer)
        {
            writer.WriteObject(_workerPosition);
            writer.WriteObject(_towPosition);
            writer.WriteObject(_workerMover);
            writer.WriteObject(_towMover);
            writer.WriteObject(_textureManager);
            writer.WriteObject(_buildingInside);            
        }

        private void ReadStateV1Position(StateReaderV1 reader)
        {
            _workerPosition = reader.ReadObject<PositionManager>();
            _towPosition = reader.ReadObject<PositionManager>();
            _workerMover = reader.ReadObject<ActionMover<Worker>>();
            _towMover = reader.ReadObject<FollowMover>();
            _textureManager = reader.ReadObject<TextureManager>();
            _buildingInside = reader.ReadObject<IHoldsWorkers>();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();

            _workerMover.FinishedAssignedSequence += new Action<ActionSequence<Worker>>(FinishedAssignedActionSequence);
            SetupTiles();

            //hide tiles if in a building
            if (_buildingInside != null)
            {
                EnterBuilding(_buildingInside);
            }
        }
        #endregion

    }
}

