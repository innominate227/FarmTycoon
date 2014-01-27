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
        /// The tile for the animal
        /// </summary>
        private MobileGameTile _tile;
                
        /// <summary>
        /// The position of the animal, and the animal tile.
        /// </summary>
        private PositionManager _position;
        
        /// <summary>
        /// Manages the texture of the animals tile
        /// </summary>
        private TextureManager _textureManager;

        #endregion

        #region Setup
        
        /// <summary>
        /// Setup animal position. (Called when animal is created)
        /// </summary>
        private void SetupPosition()
        {
            //create manager for the position of the animal, managers the animal tile, and the animal game objects position
            _position = new PositionManager();
            _position.Setup(LocationOn, this);            

            //setup the texture manager, texture manager will tell the position manager what texture to use instead of setting the texture on the tile itself
            _textureManager = new TextureManager();
            _textureManager.Setup(_animalInfo, this);

            //setup the tile
            SetupTile();
        }
        
        /// <summary>
        /// Create the animals tile
        /// </summary>        
        private void SetupTile()
        {
            //create tile for the rendering the animal (the position manager will manage the tiles position)
            _tile = new MobileGameTile(this);

            //tell texture manager and position manager about the tile
            _textureManager.SetTileToUpdate(_tile);
            _position.SetTileToMove(_tile);
            _position.UpdatePosition();
        }

        /// <summary>
        /// Hide the animals tile.        
        /// </summary>
        public void Hide()
        {
            _tile.Hidden = true;
            _tile.Update();
        }

        /// <summary>
        /// Delete the animals tiles, and movers
        /// </summary>
        private void DeletePosition()
        {            
            _textureManager.Delete();
            _tile.Delete();
        }


        #endregion

        #region Properties

        /// <summary>
        /// The position of the animal, and the animal tile.
        /// </summary>
        public PositionManager Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Pasture the animal is in
        /// </summary>
        public Pasture Pasture
        {
            get { return _pastrue; }
        }
        
        /// <summary>
        /// Manages the texture of the animals tile
        /// </summary>
        public TextureManager TextureManager
        {
            get { return _textureManager; }
        }

        #endregion

        #region Logic

        public override void UpdateTiles()
        {
            _position.UpdatePosition();
            _textureManager.Refresh();            
        }

        #endregion

        #region Save Load
        private void WriteStateV1Position(StateWriterV1 writer)
        {
            writer.WriteObject(_position);
            writer.WriteObject(_textureManager);
        }

        private void ReadStateV1Position(StateReaderV1 reader)
        {
            _position = reader.ReadObject<PositionManager>();
            _textureManager = reader.ReadObject<TextureManager>();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();

            _actionMover.FinishedAssignedSequence += new Action<ActionSequence<Animal>>(ActionMover_FinishedAssignedSequence);

            SetupTile();
        }
        #endregion

    }
}

