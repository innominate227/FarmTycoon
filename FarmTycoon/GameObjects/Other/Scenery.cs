using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    public partial class Scenery : GameObject, IHasInfo
    {
        #region Member Vars

        /// <summary>
        /// Info about the scenery
        /// </summary>
        private SceneryInfo _sceneryInfo;
        
        /// <summary>
        /// The tile for the scenery
        /// </summary>
        private MobileGameTile _tile;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a new scenery object in the game world passed.
        /// Setup or ReadState must be called after the scenery is created.
        /// </summary>
        public Scenery() : base() { }

        /// <summary>
        /// Setup the scenery.
        /// </summary>
        public void Setup(Location centerLocation, SceneryInfo sceneryInfo)
        {
            _sceneryInfo = sceneryInfo;

            PathEffect = ObjectsEffectOnPath.Solid;

            //add the scenery to the locations it ocupies
            AddLocationsOn(LocationUtils.GetLocationList(centerLocation, sceneryInfo.LandOn));

            //setup the scenery tile
            SetupTile();

            UpdateTiles();
        }

        /// <summary>
        /// Create the tile for the scenery
        /// </summary>
        private void SetupTile()
        {
            _tile = new MobileGameTile(this);
            _tile.Texture = _sceneryInfo.Texture;
        }

        /// <summary>
        /// Delete the Scenery object
        /// </summary>
        protected override void DeleteInner()
        {
            if (_tile != null)
            {
                _tile.Delete();
            }
        }
        
        #endregion
        
        #region Properties

        /// <summary>
        /// Info for the scenery
        /// </summary>
        public SceneryInfo SceneryInfo
        {
            get { return _sceneryInfo; }
        }
        
        /// <summary>
        /// Info for the scenery
        /// </summary>
        public IInfo Info
        {
            get { return _sceneryInfo; }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Update the tile for the scenery
        /// </summary>
        public override void UpdateTiles()
        {            
            _tile.MoveToLocation(LocationOn);
            _tile.Update();
        }
        
        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteInfo(_sceneryInfo);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
			_sceneryInfo = reader.ReadInfo<SceneryInfo>();
		}

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
            SetupTile();
        }
        #endregion

    }
}
