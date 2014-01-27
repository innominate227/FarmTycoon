using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Diagnostics;

namespace FarmTycoon
{

    public partial class Land : GameObject, IHasActionLocation, IHasTraits
    {
        #region Member Vars

        /// <summary>
        /// Manages the texture of the main land tile
        /// </summary>
        private TextureManager _textureManager;

        /// <summary>
        /// What corner of the tile should be shown as selected
        /// </summary>
        private LandCorner _cornerToHighlight = LandCorner.None;
                
        /// <summary>
        /// The tile that shows part of the land as being selected (null if land not selected)
        /// </summary>
        private GameTile _selectionTile;

        /// <summary>
        /// The main tile for the square of land
        /// </summary>        
        private GameTile _tile;

        /// <summary>
        /// The tile for the water of the land (null if land is above water level)
        /// </summary>
        private GameTile _waterTile;
        
        /// <summary>
        /// Tile because this land is on the border of the land owned by the player
        /// </summary>
        private List<GameTile> _borderTiles = new List<GameTile>();

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a new land tile at the location passed
        /// </summary>
        private void SetupTiles()
        {   
            //setup the texture manager for the main tile
            _textureManager = new TextureManager();
            _textureManager.Setup((LandInfo)FarmData.Current.GetInfo(LandInfo.UNIQUE_NAME), this);
            _textureManager.SetTileToUpdate(_tile);
        }
                
        /// <summary>
        /// Delete the tile created for the land
        /// </summary>
        private void DeleteTiles()
        {
            _textureManager.Delete();

            if (_tile != null)
            {
                _tile.Delete();
                _tile = null;
            }
            if (_selectionTile != null)
            {
                _selectionTile.Delete();
                _selectionTile = null;
            }
            if (_waterTile != null)
            {
                _waterTile.Delete();
                _waterTile = null;
            }
            foreach (GameTile borderTile in _borderTiles)
            {
                borderTile.Delete();
            }
            _borderTiles.Clear();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Manages the texture of the main land tile
        /// </summary>
        public TextureManager TextureManager
        {
            get { return _textureManager; }
        }

        /// <summary>
        /// What corner of the tile should be drawn as highlighted
        /// </summary>
        public LandCorner CornerToHighlight
        {
            get { return _cornerToHighlight; }
            set 
            {
                _cornerToHighlight = value;
                UpdateSelectionTile();
            }
        }
        
        /// <summary>
        /// Get the 4 digit number represent the slope of the land, and it used in texture names
        /// </summary>
        public string HeightCode
        {
            get
            {
                int extraNorth = this.GetExtraHeight(CardinalDirection.North);
                int extraEast = this.GetExtraHeight(CardinalDirection.East);
                int extraSouth = this.GetExtraHeight(CardinalDirection.South);
                int extraWest = this.GetExtraHeight(CardinalDirection.West);
                return extraNorth.ToString() + extraEast.ToString() + extraSouth.ToString() + extraWest.ToString();
            }
        }

        #endregion
        
        #region Logic

        /// <summary>
        /// Refresh all tiles created for the land
        /// </summary>
        public override void UpdateTiles()
        {            
            UpdateMainTile();
            UpdateSelectionTile();
            UpdateWaterTile();
            UpdateBorderTiles();
        }
        
        /// <summary>
        /// update the main tile to be using the correct texture at the correct height
        /// </summary>
        private void UpdateMainTile()
        {
            if (_tile != null)
            {
                _tile.Delete();
            }

            //create a new tile for the land, at its new location.  Land is fixed at layer 0
            _tile = new FixedGameTile(this, LocationOn, 0);
             
            _tile.Prepend = this.HeightCode + "_";
            _textureManager.SetTileToUpdate(_tile);
            _textureManager.Refresh();
            _tile.Update();
        }

        /// <summary>
        /// Update the selection tile
        /// </summary>
        private void UpdateSelectionTile()
        {
            //remove the old selection (if there was one)
            if (_selectionTile != null)
            {
                _selectionTile.Delete();
                _selectionTile = null;
            }

            //highlight the corener to highlight
            if (_cornerToHighlight != LandCorner.None)
            {
                string highlightSuffix = "";
                if (_cornerToHighlight == LandCorner.North) { highlightSuffix = "N"; }
                else if (_cornerToHighlight == LandCorner.South) { highlightSuffix = "S"; }
                else if (_cornerToHighlight == LandCorner.East) { highlightSuffix = "E"; }
                else if (_cornerToHighlight == LandCorner.West) { highlightSuffix = "W"; }
                else if (_cornerToHighlight == LandCorner.Center) { highlightSuffix = "C"; }
                
                _selectionTile = new MobileGameTile(this, LocationOn);                             
                _selectionTile.Texture = HeightCode + "_Sel" + highlightSuffix;
                _selectionTile.Update();                
            }




            //remove the old selection (if there was one)
            if (_cornerToHighlight2_gameTile_DEBUG != null)
            {
                _cornerToHighlight2_gameTile_DEBUG.Delete();
                _cornerToHighlight2_gameTile_DEBUG = null;
            }

            //highlight the corener to highlight
            if (_cornerToHighlight2_DEBUG != LandCorner.None)
            {
                string highlightSuffix = "";
                if (_cornerToHighlight2_DEBUG == LandCorner.North) { highlightSuffix = "N"; }
                else if (_cornerToHighlight2_DEBUG == LandCorner.South) { highlightSuffix = "S"; }
                else if (_cornerToHighlight2_DEBUG == LandCorner.East) { highlightSuffix = "E"; }
                else if (_cornerToHighlight2_DEBUG == LandCorner.West) { highlightSuffix = "W"; }
                else if (_cornerToHighlight2_DEBUG == LandCorner.Center) { highlightSuffix = "C"; }

                _cornerToHighlight2_gameTile_DEBUG = new MobileGameTile(this, LocationOn);
                _cornerToHighlight2_gameTile_DEBUG.Texture = HeightCode + "_Sel" + highlightSuffix;
                _cornerToHighlight2_gameTile_DEBUG.Update();
            }
        }

        public LandCorner _cornerToHighlight2_DEBUG = LandCorner.None;
        private MobileGameTile _cornerToHighlight2_gameTile_DEBUG = null;
                
        /// <summary>
        /// Update the water on the tile.
        /// </summary>
        private void UpdateWaterTile()
        {
            if (_waterTile != null)
            {
                _waterTile.Delete();
                _waterTile = null;
            }
            

            //are we at water level
            if (this.MinHeight == FarmData.Current.LandInfo.MinHeight)
            {
                if (this.HeightCode == "0000" || this.HeightCode == "1000" || this.HeightCode == "0100" || this.HeightCode == "0010" || this.HeightCode == "0001")
                {
                    //create a fixed for the water on the land
                    _waterTile = new FixedGameTile(this, LocationOn, 1);                    

                    _waterTile.Texture = this.HeightCode + "Wet";                    
                    _waterTile.Update();
                }
            }
        }

        /// <summary>
        /// Add (or remove) border tiles for this land tile
        /// </summary>
        private void UpdateBorderTiles()
        {
            //TODO:

            ////delete the current border tiles
            //foreach (GameObjectTile borderTile in _borderTiles)
            //{
            //    borderTile.Delete();
            //}
            //_borderTiles.Clear();
            
            ////if this land is not owned we will not be drawing any border
            ////also if this is the entry we will not draw any border
            //if (_owned == false || _entry)
            //{
            //    return;
            //}

            ////check if border tlie is needed for each side
            //foreach (OrdinalDirection dir in DirectionUtils.AllOrdinalDirections)
            //{
            //    //if adjacent in that direction is also owned do nothing
            //    if (GetAdjacent(dir).Owned) { continue; }

            //    //draw a border in that direction
            //    GameObjectTile fenceTile = new GameObjectTile(this);
            //    fenceTile.RenderingInfo.X = LocationOn.ScreenX;
            //    fenceTile.RenderingInfo.Y = LocationOn.ScreenY;
            //    fenceTile.RenderingInfo.Z = MinHeight; 

            //    //TODO: some duplication here with Fence object
            //    if (dir == OrdinalDirection.NorthEast)
            //    {
            //        fenceTile.TilePosition.XCenter = LocationOn.X;
            //        fenceTile.TilePosition.YCenter = LocationOn.Y - 0.5f;
            //        fenceTile.TilePosition.ZBase = LocationOn.Z;
            //        fenceTile.TilePosition.XSize = 0.5f;
            //        fenceTile.TilePosition.YSize = 0.1f;
            //        fenceTile.TilePosition.ZHeigth = 2;
            //    }
            //    else if (dir == OrdinalDirection.SouthEast)
            //    {
            //        fenceTile.TilePosition.XCenter = LocationOn.X + 0.5f;
            //        fenceTile.TilePosition.YCenter = LocationOn.Y;
            //        fenceTile.TilePosition.ZBase = LocationOn.Z;
            //        fenceTile.TilePosition.XSize = 0.1f;
            //        fenceTile.TilePosition.YSize = 0.5f;
            //        fenceTile.TilePosition.ZHeigth = 2;
            //    }
            //    else if (dir == OrdinalDirection.NorthWest)
            //    {
            //        fenceTile.TilePosition.XCenter = LocationOn.X - 0.5f;
            //        fenceTile.TilePosition.YCenter = LocationOn.Y;
            //        fenceTile.TilePosition.ZBase = LocationOn.Z;
            //        fenceTile.TilePosition.XSize = 0.1f;
            //        fenceTile.TilePosition.YSize = 0.5f;
            //        fenceTile.TilePosition.ZHeigth = 2;
            //    }
            //    else if (dir == OrdinalDirection.SouthWest)
            //    {
            //        fenceTile.TilePosition.XCenter = LocationOn.X;
            //        fenceTile.TilePosition.YCenter = LocationOn.Y + 0.5f;
            //        fenceTile.TilePosition.ZBase = LocationOn.Z;
            //        fenceTile.TilePosition.XSize = 0.5f;
            //        fenceTile.TilePosition.YSize = 0.1f;
            //        fenceTile.TilePosition.ZHeigth = 2;
            //    }


            //    string side = DirectionUtils.OrdinalDirectionToAbreviation(dir);
            //    fenceTile.Texture = HeightCode + "_fence_short_" + side;
            //    fenceTile.Update();

            //    _borderTiles.Add(fenceTile);
            //}
        }

        #endregion

        #region Save Load
        private void WriteStateV1Tiles(StateWriterV1 writer)
        {
            writer.WriteObject(_textureManager);
        }

        private void ReadStateV1Tiles(StateReaderV1 reader)
        {
            _textureManager = reader.ReadObject<TextureManager>();
        }
        #endregion

    }
}
