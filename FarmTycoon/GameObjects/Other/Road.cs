using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{



    /// <summary>
    /// A Road allows workers to get to where they need to be faster.  Each road takes up one tile of space.    
    /// </summary>
    public class Road : GameObject
    {
        #region Member Vars

        /// <summary>
        /// The tile for the road
        /// </summary>
        private GameTile _tile;

        /// <summary>
        /// When we delete the road we need to tell our neightbors that we were deleted.
        /// But once we are deleted we no longer have a location.  So we need to keep track of our
        /// location seperate from GameObject for that situation
        /// </summary>
        private Location _locationOn2;
                                
        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a road call LoadState or Setup first
        /// </summary>
        public Road() : base() 
        {
        }
        
        /// <summary>
        /// Create a road given the location the road should be built on
        /// </summary>
        public void Setup(Location locaton)
        {            
            AddLocationOn(locaton);
            _locationOn2 = locaton;

            //we want workers to walk on roads
            PathEffect = ObjectsEffectOnPath.DoWalk;

            UpdateNeighborRoadTiles();
            UpdateTiles();
        }
                       
        /// <summary>
        /// Called when the road is deleted
        /// </summary>
        protected override void DeleteInner()
        {
            _tile.Delete();
            UpdateNeighborRoadTiles();
        }
        
        #endregion
                
        #region Logic
                
        /// <summary>
        /// Returns a 4 digit binary string for the road based on if there are adjacent roads (or highways) in the adjacent locations
        /// </summary>
        private string DetermineAdjacenciesString()
        {     
            //if we are on sloped land then our adjaceny string will always be the same
            Land landOn = LocationOn.Find<Land>();
            if (landOn.HeightCode == "1100")
            {                
                return "1010";
            }
            else if (landOn.HeightCode == "0011")
            {
                return "1010";
            }
            else if (landOn.HeightCode == "0110")
            {
                return "0101";
            }
            else if (landOn.HeightCode == "1001")
            {
                return "0101";
            }            

            //check for a neighbor in each direction (note the direction order in All Directions matches the texture name order)
            string adjacencyCode = "";
            foreach(OrdinalDirection direction in DirectionUtils.AllOrdinalDirections)
            {   
                if (LocationOn.GetAdjacent(direction).Contains<Road>() || LocationOn.GetAdjacent(direction).Contains<Highway>())
                {
                    adjacencyCode += "1";
                }
                else
                {
                    adjacencyCode += "0";
                }
            }
            return adjacencyCode;
            
        }
                
        /// <summary>
        /// Updates the roads tile to ensure its using the correct one
        /// </summary>
        public override void UpdateTiles()
        {
            //delete old tile
            if (_tile != null)
            {
                _tile.Delete();
            }

            //create new tile for the road (road is always on layer 1 right above the land)
            _tile = new FixedGameTile(this, LocationOn, 1); 
           
            Land landOn = LocationOn.Find<Land>();
            if (landOn == null) { return; } //landOn could be null if we are deleting all objects right now
            string tileName = landOn.HeightCode;
            string roadsName = DetermineAdjacenciesString();
            _tile.Texture = "road_" + tileName + "_" + roadsName;                        
            _tile.Update();
        }


        /// <summary>
        /// Updates the tiles for all neighboring roads
        /// </summary>
        private void UpdateNeighborRoadTiles()
        {
            foreach (OrdinalDirection direction in DirectionUtils.AllOrdinalDirections)
            {
                Road otherRoad = _locationOn2.GetAdjacent(direction).Find<Road>();
                if (otherRoad != null)
                {
                    otherRoad.UpdateTiles();
                }
            }
        }

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteObject(_locationOn2);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _locationOn2 = reader.ReadObject<Location>();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion

    }
}
