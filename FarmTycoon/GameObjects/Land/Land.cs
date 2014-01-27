using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// Each column in the game world has a peice of land in it.  
    /// The land can be at a differnet height, and the height can be changed by the player.
    /// Land has traits, when a crop is planted on the land it shares its traits with the traits of the land.
    /// In addition to the land grass tile, tiles are created for the dirt when the land creates a cliff, and for water if the land is low enough
    /// </summary>
    public partial class Land : GameObject, IHasActionLocation, IHasTraits, IHasTextureManager, IHasInfo
    {
        #region Setup Delete

        /// <summary>
        /// Create a land tile
        /// </summary>
        public Land() : base() 
        {
        }

        /// <summary>
        /// Create a new land tile at the location passed
        /// </summary>
        public void Setup(Location location)
        {
            SetupLocation(location);            
            SetupTiles();
            SetupTraits();
            
            //land is never in a being placed state
            this.DoneWithPlacement();            
        }

        protected override void DeleteInner()
        {
            DeleteTiles();
            DeleteTraits();
        }

        #endregion
        
        /// <summary>
        /// Get the info for this land
        /// </summary>
        public IInfo Info
        {
            get { return FarmData.Current.LandInfo; }
        }

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            WriteStateV1Location(writer);
            WriteStateV1Tiles(writer);
            WriteStateV1Traits(writer);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            ReadStateV1Location(reader);
            ReadStateV1Tiles(reader);
            ReadStateV1Traits(reader);
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion

    }
}
