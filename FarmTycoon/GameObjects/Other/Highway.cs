using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Diagnostics;

namespace FarmTycoon
{



    /// <summary>
    /// A Highway allows workers to get to where they need to be much faster.  But it takes up two tiles of space.
    /// </summary>
    public class Highway : GameObject
    {
        #region Member Vars

        /// <summary>
        /// Tile for the highway
        /// </summary>
        private GameTile _tile;
        
        /// <summary>
        /// When we delete the highway we need to tell our neightbors that we were deleted.
        /// But once we are deleted we no longer have a location.  So we need to keep track of our
        /// location seperate from GameObject for that situation
        /// </summary>
        private Location _locationOn2;

        /// <summary>
        /// Travel direction for this highway
        /// </summary>
        private OrdinalDirection _travelDirection;
                                
        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a Highway call LoadState or Setup first
        /// </summary>
        public Highway() : base() 
        {
        }
        
        /// <summary>
        /// Create a Highway given the location the Highway should be built on (and the direction of travel)
        /// </summary>
        public void Setup(Location locaton, OrdinalDirection travelDirection)
        {
            _travelDirection = travelDirection;

            //set the location we are on
            AddLocationOn(locaton);            

            //rember them ourself to (see def for why)
            _locationOn2 = locaton;            

            //we want workers to walk on highways
            PathEffect = ObjectsEffectOnPath.DoWalkPlus;

            //set the prefered direction of travel for the object
            PathEffect |= DirectionUtils.TravelDirection(travelDirection);
        }
                       
        /// <summary>
        /// Called when the road is deleted
        /// </summary>
        protected override void DeleteInner()
        {
            if (_tile != null)
            {
                _tile.Delete();
            }
            
            UpdateNeighborRoadAndHighwayTiles(false);
        }
        
        #endregion

        #region Propeties

        /// <summary>
        /// Direction of travel for this highway 
        /// </summary>
        public OrdinalDirection TravelDirection
        {
            get
            {
                return _travelDirection;
            }
        }

        /// <summary>
        /// The direction the other land of the highwat it in
        /// </summary>
        public OrdinalDirection OtherLaneDirection
        {
            get
            {
                return DirectionUtils.CounterClockwiseOne(_travelDirection);                
            }
        }

        /// <summary>
        /// The Other Lane for this highway
        /// </summary>
        public Highway OtherLane
        {
            get
            {   
                return LocationOn.GetAdjacent(OtherLaneDirection).Find<Highway>();                
            }
        }


        /// <summary>
        /// Is highway is part of an intersection
        /// </summary>
        public bool IsPartOfIntersection
        {
            get
            {
                return DetermineIfPartOfIntersection();
            }
        }




        #endregion

        #region Logic

        /// <summary>
        /// Returns a 4 digit binary string for the highway based on if there are highways in the adjacent locations
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
                Highway adjacentHighway = LocationOn.GetAdjacent(direction).Find<Highway>();
                if (adjacentHighway == null)
                {
                    adjacencyCode += "0";
                }
                else
                {
                    //does the highway travel in the opposite direction
                    bool travelsInOppositeDirection = (adjacentHighway.TravelDirection == DirectionUtils.OppositeDirection(TravelDirection));

                    //id it our OtherLand
                    bool isOtherLand = (adjacentHighway == OtherLane);

                    //if is travels in other direction and is not our other land do not consider it adjacnet
                    if (travelsInOppositeDirection && isOtherLand == false)
                    {
                        adjacencyCode += "0";
                    }
                    else
                    {
                        adjacencyCode += "1";
                    }
                }
            }
            return adjacencyCode;
            
        }

        
        /// <summary>
        /// Returns a 4 digit binary string for the highway based on if there are highways in the adjacent caridnal locations
        /// </summary>
        private string DetermineCardinalAdjacenciesString()
        {     
            //if we are on sloped land then our adjaceny string will always be the same
            Land landOn = LocationOn.Find<Land>();
            
            //check for a neighbor in each direction (note the direction order in All Directions matches the texture name order)
            string adjacencyCode = "";
            foreach(CardinalDirection direction in DirectionUtils.AllCardinalDirections)
            {
                OrdinalDirection subDirection1 = DirectionUtils.CounterClockwiseOneOrdinal(direction);
                OrdinalDirection subDirection2 = DirectionUtils.ClockwiseOneOrdinal(direction);

                bool adjacentDir = LocationOn.GetAdjacent(direction).Contains<Highway>();
                bool adjacentSubDir1 = LocationOn.GetAdjacent(subDirection1).Contains<Highway>();
                bool adjacentSubDir2 = LocationOn.GetAdjacent(subDirection2).Contains<Highway>();
                
                if (adjacentDir && adjacentSubDir1 && adjacentSubDir2)
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
        /// Retrun if this highway is maybe part of an intersection.
        /// See Part of Inersection for part two of the check
        /// </summary>
        private bool DetermineIfPartOfIntersection()
        {
            //we must be maybe part of the intersection
            if (DetermineIfMaybePartOfIntersection() == false)
            {
                return false;
            }

            //and a neightbor that travels in the same direction must be "maybe part of the intersection"
            foreach (OrdinalDirection dir in DirectionUtils.AllOrdinalDirections)
            {
                Highway adjacentHighway = LocationOn.GetAdjacent(dir).Find<Highway>();
                if (adjacentHighway != null && adjacentHighway.TravelDirection == TravelDirection && adjacentHighway.DetermineIfMaybePartOfIntersection())
                {
                    return true;
                }
            }

            //other wise we are not part of the intersection
            return false;
        }

        /// <summary>
        /// Retrun if this highway is maybe part of an intersection.
        /// See Part of Inersection for part two of the check
        /// </summary>
        private bool DetermineIfMaybePartOfIntersection()
        {
            //if the highway is only half built so far then say no
            if (OtherLane == null)
            {
                return false;
            }

            //if i have a highway adjacent to me in that has a direction of travel that is perpendicular to mine
            foreach (OrdinalDirection dir in DirectionUtils.AllOrdinalDirections)
            {
                Highway adjacentHighway = LocationOn.GetAdjacent(dir).Find<Highway>();
                if (adjacentHighway != null && adjacentHighway.TravelDirection != TravelDirection && adjacentHighway.TravelDirection != DirectionUtils.OppositeDirection(TravelDirection))
                {
                    return true;
                }
            }

            //if my other lane has a highway adjacent to it in that has a direction of travel that is perpendicular to it (wich means perpendicular to me too)
            foreach (OrdinalDirection dir in DirectionUtils.AllOrdinalDirections)
            {
                Highway adjacentHighway = OtherLane.LocationOn.GetAdjacent(dir).Find<Highway>();
                if (adjacentHighway != null && adjacentHighway.TravelDirection != TravelDirection && adjacentHighway.TravelDirection != DirectionUtils.OppositeDirection(TravelDirection))
                {
                    return true;
                }
            }

            return false;
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

            LocationOn.Find<Land>()._cornerToHighlight2_DEBUG = LandCorner.None;
            LocationOn.Find<Land>().CornerToHighlight = LandCorner.None;

            //we have not made the other land yet, dont try and draw
            if (OtherLane == null)
            {
                return;
            }
                        
            //find the land each of the two highway peices is on
            Land landOn = LocationOn.Find<Land>();            
            if (landOn == null) { return; } //landOn could be null if we are deleting all objects right now
            
            //determine adjacencies for the tile
            string adjacencies = DetermineAdjacenciesString();                   
            
            //determine the texture for both tile
            string texture = "highway_" + landOn.HeightCode + "_" + adjacencies + "_" + DirectionUtils.OrdinalDirectionToAbreviation(OtherLaneDirection);
            
            //if part of an intersection the tile is different, we need to know Carindl adjacencies have roads
            if (IsPartOfIntersection)
            {                
                adjacencies = DetermineCardinalAdjacenciesString();
                texture = "highway_intersection_" + adjacencies;                
            }


            //if (_travelDirection == OrdinalDirection.NorthEast)
            //{
            //    LocationOn.Find<Land>()._cornerToHighlight2_DEBUG = LandCorner.East;
            //    LocationOn.Find<Land>().CornerToHighlight = LandCorner.North;
            //}
            //else if (_travelDirection == OrdinalDirection.SouthEast)
            //{
            //    LocationOn.Find<Land>()._cornerToHighlight2_DEBUG = LandCorner.East;
            //    LocationOn.Find<Land>().CornerToHighlight = LandCorner.South;
            //}
            //else if (_travelDirection == OrdinalDirection.SouthWest)
            //{
            //    LocationOn.Find<Land>()._cornerToHighlight2_DEBUG = LandCorner.West;
            //    LocationOn.Find<Land>().CornerToHighlight = LandCorner.South;
            //}
            //else if (_travelDirection == OrdinalDirection.NorthWest)
            //{
            //    LocationOn.Find<Land>()._cornerToHighlight2_DEBUG = LandCorner.West;
            //    LocationOn.Find<Land>().CornerToHighlight = LandCorner.North;
            //}
            
            //create new tile for the highway (highway is always on layer 1 right above the land)
            _tile = new FixedGameTile(this, AllLocationsOn[0], 1);                                    
            _tile.Texture = texture;            
            _tile.Update();                        
        }


        /// <summary>
        /// Updates the tiles for all neighboring roads, and highways.
        /// Optinoally only update for neighbors that are (fully placed opposed to inprogress)
        /// </summary>
        public void UpdateNeighborRoadAndHighwayTiles(bool placedOnly)
        {
            List<Location> nearByLocations = LocationUtils.CollectLocations(_locationOn2, CardinalDirection.North, 5);


            foreach (Location nearBy in nearByLocations)
            {
                Road otherRoad = nearBy.Find<Road>();
                if (otherRoad != null && (placedOnly == false || otherRoad.PlacementState == FarmTycoon.PlacementState.Placed))
                {
                    otherRoad.UpdateTiles();
                }

                Highway otherHighway = nearBy.Find<Highway>();
                if (otherHighway != null && otherHighway != this && (placedOnly == false || otherHighway.PlacementState == FarmTycoon.PlacementState.Placed))
                {
                    otherHighway.UpdateTiles();
                }
            }         
        }

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteObject(_locationOn2);
            writer.WriteEnum(_travelDirection);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _locationOn2 = reader.ReadObject<Location>();
            _travelDirection = reader.ReadEnum<OrdinalDirection>();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion

    }
}
