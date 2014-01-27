using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// Workers go to a BreakHouse when they are not doing anything else in order to rest
    /// </summary>
    public partial class BreakHouse : GameObject, IHasTextureManager, IHasInfo, IHoldsWorkers, IHasActionLocation
    {
        #region Member Vars

        /// <summary>
        /// Info for this BreakHouse
        /// </summary>
        private BreakHouseInfo _breakHouseInfo;
        
        /// <summary>
        /// The tile for the BreakHouse
        /// </summary>
        private MobileGameTile _tile;

        /// <summary>
        /// Texture manager for the BreakHouse
        /// </summary>
        private TextureManager _textureManager;

        /// <summary>
        /// List of workers inside the breakhouse and/or that have reserved a spot inside the breakhouse
        /// </summary>
        private WorkersInsideList _workersInside = new WorkersInsideList();

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a new BreakHouse
        /// Setup or ReadState must be called after creation.
        /// </summary>
        public BreakHouse() : base() { }

        /// <summary>
        /// Setup the tile for the BreakHouse
        /// </summary>
        private void SetupTile()
        {
            //create the tile
            _tile = new MobileGameTile(this);
            _textureManager.SetTileToUpdate(_tile);
        }

        /// <summary>
        /// Setup the BreakHouse
        /// </summary>
        public void Setup(Location centerLocation, BreakHouseInfo breakHouseInfo)
        {
            _breakHouseInfo = breakHouseInfo;

            //add the breakhouse to the locations it ocupies
            AddLocationsOn(LocationUtils.GetLocationList(centerLocation, breakHouseInfo.LandOn));

            //setup texture manager
            _textureManager = new TextureManager();
            _textureManager.Setup(breakHouseInfo, this);

            //setup tle for the breakhouse
            SetupTile();

            //do not allow walking through break house
            PathEffect = ObjectsEffectOnPath.Solid;

            //update the tile
            UpdateTiles();
        }


        /// <summary>
        /// Called when the break house is waiting to be deleted.
        /// </summary>
        protected override void WaitingToDeleteInner()
        {
            //all workers in the break house should restart their default action sequencer
            //they will rest again, but this break house is no longer in master object list (since it is in waiting to be deleted state).
            //so they will go to another break house
            //we can then delete all objects in limbo wich will cause this break house to be deleted, since there are no longer actions using it.

            foreach (Worker worker in _workersInside.WorkersInside.ToArray())
            {
                worker.RestartDefaultActionSequence();
            }

            GameState.Current.ObjectsInLimbo.TryToDeleteObjectsInLimbo();
        }

        /// <summary>
        /// Called when the break house is deleted
        /// </summary>
        protected override void DeleteInner()
        {
            base.DeleteInner();
            _tile.Delete();
            _textureManager.Delete();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The location a worker should travel to in order to vist the break house
        /// </summary>
        public Location ActionLocation
        {
            get { return _breakHouseInfo.ActionLocation.GetRealtiveLocation(LocationOn); }
        }
        
        /// <summary>
        /// Texture Manager for the building
        /// </summary>
        public TextureManager TextureManager
        {
            get { return _textureManager; }
        }

        /// <summary>
        /// BreakHouseInfo for this BreakHouse
        /// </summary>        
        public BreakHouseInfo BreakHouseInfo
        {
            get { return _breakHouseInfo; }
        }
        
        /// <summary>
        /// Info for this BreakHouse
        /// </summary>        
        public IInfo Info
        {
            get { return _breakHouseInfo; }
        }
        
        /// <summary>
        /// List of workers inside the breakhouse and/or that have reserved a spot inside the breakhouse
        /// </summary>
        public WorkersInsideList WorkersInside
        {
            get { return _workersInside; }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Update the tile for the trough
        /// </summary>
        public override void UpdateTiles()
        {
            _tile.MoveToLocation(LocationOn);
            _textureManager.Refresh();
        }
          
        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteInfo(_breakHouseInfo);
            writer.WriteObject(_textureManager);
            writer.WriteObject(_workersInside);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _breakHouseInfo = reader.ReadInfo<BreakHouseInfo>();
            _textureManager = reader.ReadObject<TextureManager>();
            _workersInside = reader.ReadObject<WorkersInsideList>();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
            SetupTile();
        }
        #endregion
    }
}
