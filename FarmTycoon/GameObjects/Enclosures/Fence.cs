using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    /// <summary>
    /// A fence that surrounds an enclosure.  Fences also exist on their own for a small amount of time while an enclosure is being created.
    /// Fences are mostly just visible, they also effect where the worker can walk.
    /// </summary>
    public class Fence : GameObject
    {
        #region Actions

        /// <summary>
        /// Action raised when the number of enclosures that border the fence changes
        /// </summary>
        public event Action BorderingEnclosuresChanged;

        #endregion

        #region Member Vars

        /// <summary>
        /// List of enclosures that this fence is a border for
        /// </summary>
        private List<Enclosure> _enclosuresBordered = new List<Enclosure>();     

        /// <summary>
        /// List of fences adjacent to this fence
        /// </summary>
        private List<Fence> _adjacentFences = new List<Fence>();        
        
        /// <summary>
        /// The tile for the fence
        /// </summary>
        public MobileGameTile _tile;

        /// <summary>
        /// The land the fence is on
        /// TODO: probably dont need to store, can just get based on location
        /// </summary>
        private Land _landOn;

        /// <summary>
        /// Side the land the fence is on
        /// </summary>
        private OrdinalDirection _sideOn;

        /// <summary>
        /// Is this fence a gate
        /// </summary>
        private bool _gate = false;

        /// <summary>
        /// The type of enclosure the fence is for.  Determines the color of the fence.
        /// </summary>
        private EnclosureType _typeFor = EnclosureType.Field;
        
        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a fence 
        /// Setup or ReadState must be called after the fence is created.
        /// </summary>
        public Fence() : base() { }

        /// <summary>
        /// Create a fence given the land is was built on, and the side
        /// </summary>
        public void Setup(Land landOn, OrdinalDirection sideOn)
        {
            _landOn = landOn;
            _sideOn = sideOn;

            AddLocationOn(landOn.LocationOn);

            //set the effect the fence has on paths
            UpdatePathEffect();
            
            //update the fence tile
            UpdateTiles();

            //determine the fences we are adjacnet to, and have our neighboring fences do the same
            RefreshAdjacentFences();
            RefreshNeighbors();
        }
        
        /// <summary>
        /// Called when the fence is being deleted
        /// </summary>
        protected override void DeleteInner()
        {
            //tell adjacent fence to update their adjacency list
            foreach (Fence adjacentFence in _adjacentFences)
            {
                adjacentFence.RefreshAdjacentFences();
            }
            if (_tile != null)
            {
                _tile.Delete();
            }
        }
        
        #endregion

        #region Properties
        
        /// <summary>
        /// The type of enclosure the fence is for.  Determines the color of the fence.
        /// </summary>
        public EnclosureType TypeFor
        {
            get { return _typeFor; }
            set
            {
                _typeFor = value;
                UpdateTiles();
            }
        }

        /// <summary>
        /// The land the fence is on
        /// </summary>
        public Land LandOn
        {
            get { return _landOn; }
        }

        /// <summary>
        /// Side the fence is on
        /// </summary>
        public OrdinalDirection SideOn
        {
            get { return _sideOn; }
        }

        /// <summary>
        /// List of enclosures that this fence borders
        /// </summary>
        public IList<Enclosure> EnclosuresBordered
        {
            get { return _enclosuresBordered.AsReadOnly(); }
        }
        
        /// <summary>
        /// List of fences adjacent to this fence. (A post of the fences touch)
        /// </summary>
        public IList<Fence> AdjacentFences
        {
            get { return _adjacentFences.AsReadOnly(); }
        }
        
        /// <summary>
        /// Is this fence a gate
        /// </summary>
        public bool Gate
        {
            get { return _gate; }
            set 
            {
                _gate = value; 
                UpdateTiles();
                //update the effect on paths
                UpdatePathEffect();
            }
        }

        #endregion

        #region Logic
                
        #region Enclosures

        /// <summary>
        /// Add a enclosures to the list of bordering enclosures
        /// </summary>
        public void AddBorderingEnclosure(Enclosure bordering)
        {
            _enclosuresBordered.Add(bordering);

            //raise event that the number of bordering enclosures has changed
            if (BorderingEnclosuresChanged != null)
            {
                BorderingEnclosuresChanged();
            }
        }
        /// <summary>
        /// Remove a enclosures from the list of bordering enclosures
        /// </summary>
        public void RemoveBorderingEnclosure(Enclosure bordering)
        {
            _enclosuresBordered.Remove(bordering);

            //make sure we are the correct fence type if we only border one enclosure
            if (_enclosuresBordered.Count == 1)
            {
                bool fieldInsideEnclodusre = _enclosuresBordered[0].LocationOn.Contains<Field>();
                if (fieldInsideEnclodusre)
                {
                    TypeFor = EnclosureType.Field;
                }
                else
                {
                    TypeFor = EnclosureType.Pasture;
                }
            }

            //raise event that the number of bordering enclosures has changed
            if (BorderingEnclosuresChanged != null)
            {
                BorderingEnclosuresChanged();
            }
        }

        #endregion

        #region Adjacnet Fences

        /// <summary>
        /// Have all fences in neighboring tiles refresh their adjacent fences list
        /// </summary>
        private void RefreshNeighbors()
        {
            List<Location> neighboringLocations = new List<Location>();
            neighboringLocations.Add(LocationOn);
            foreach (OrdinalDirection direction in DirectionUtils.AllOrdinalDirections)
            {
                OrdinalDirection directionClockwise1 = DirectionUtils.ClockwiseOne(direction);
                neighboringLocations.Add(LocationOn.GetAdjacent(direction));
                neighboringLocations.Add(LocationOn.GetAdjacent(direction).GetAdjacent(directionClockwise1));
            }
            foreach (Location neighbor in neighboringLocations)
            {
                foreach (Fence fence in neighbor.FindAll<Fence>())
                {
                    fence.RefreshAdjacentFences();
                }
            }
        }

        /// <summary>
        /// refresh my adjacent fences list
        /// </summary>
        private void RefreshAdjacentFences()
        {
            //clear current adjacent fences list
            _adjacentFences.Clear();

            //any fence in the same tile thats not on the opposite side, and not the same fence (on the same side) is adjacent
            foreach (Fence fence in LocationOn.FindAll<Fence>())
            {
                if (fence.SideOn == DirectionUtils.ClockwiseOne(_sideOn) || fence.SideOn == DirectionUtils.CounterClockwiseOne(_sideOn)) 
                {
                    _adjacentFences.Add(fence);
                }
            }
            
            //for the land on the side the fence is on do that same checks (not opposite, and not the same side, where it would be the same fence)
            foreach (Fence fence in LocationOn.GetAdjacent(_sideOn).FindAll<Fence>())
            {
                if (fence.SideOn == DirectionUtils.ClockwiseOne(_sideOn) || fence.SideOn == DirectionUtils.CounterClockwiseOne(_sideOn))
                {
                    _adjacentFences.Add(fence);
                }
            }
            
            //for the land clockwise from the side the fence is on check:
            //side fence is on, and side counter-clockwise to side fence is on
            foreach (Fence fence in LocationOn.GetAdjacent(DirectionUtils.ClockwiseOne(_sideOn)).FindAll<Fence>())
            {
                if (fence.SideOn == _sideOn || fence.SideOn == DirectionUtils.CounterClockwiseOne(_sideOn))
                {
                    _adjacentFences.Add(fence);
                }
            }

            //for the land counter-clockwise from the side the fence is on check:
            //side fence is on, and side counter-clockwise to side fence is on
            foreach (Fence fence in LocationOn.GetAdjacent(DirectionUtils.CounterClockwiseOne(_sideOn)).FindAll<Fence>())
            {
                if (fence.SideOn == _sideOn || fence.SideOn == DirectionUtils.ClockwiseOne(_sideOn))
                {
                    _adjacentFences.Add(fence);
                }
            }

            //for the land at side of fence + clockwise check:
            //opposite side, and side counter-clockwise
            foreach (Fence fence in LocationOn.GetAdjacent(_sideOn).GetAdjacent(DirectionUtils.ClockwiseOne(_sideOn)).FindAll<Fence>())
            {
                if (fence.SideOn == DirectionUtils.OppositeDirection(_sideOn) || fence.SideOn == DirectionUtils.CounterClockwiseOne(_sideOn))
                {
                    _adjacentFences.Add(fence);
                }
            }

            //for the land at side of fence + counter-clockwise check:
            //opposite side, and side clockwise
            foreach (Fence fence in LocationOn.GetAdjacent(_sideOn).GetAdjacent(DirectionUtils.CounterClockwiseOne(_sideOn)).FindAll<Fence>())
            {
                if (fence.SideOn == DirectionUtils.OppositeDirection(_sideOn) || fence.SideOn == DirectionUtils.ClockwiseOne(_sideOn))
                {
                    _adjacentFences.Add(fence);
                }
            }
        }
        
        #endregion

        #region Location / Tile
        
        private void UpdatePathEffect()
        {
            if (_gate)
            {
                PathEffect = ObjectsEffectOnPath.None;
            }
            else
            {
                //the object prevents walking toward the side its on
                PathEffect = DirectionUtils.BlocksDirection(_sideOn);
            }
        }

        /// <summary>
        /// Update the tile rendering the fence
        /// </summary>
        public override void UpdateTiles()
        {
            if (_gate)
            {
                //remove tile if its a gate
                if (_tile != null)
                {
                    _tile.Delete();
                    _tile = null;
                }
            }
            else
            {
                //create tile if nessisary
                if (_tile == null)
                {
                    _tile = new MobileGameTile(this);
                }

                //location of the fence on X, and Y is the average of the two land locations
                Location adjacentLocation = LocationOn.GetAdjacent(_sideOn);
                float locX = (LocationOn.X + adjacentLocation.X) / 2.0f;
                float locY = (LocationOn.Y + adjacentLocation.Y) / 2.0f;

                //Z location is more complicated
                float locZ = 0;
                if (LocationOn.Z == adjacentLocation.Z)
                {
                    locZ = LocationOn.Z;

                    //for Z if both are the same Z value it is that Z value
                    //except if one of the corners involved is two higher we need to raise Z by 1 
                    int corner1ExtraHeight = _landOn.GetExtraHeight(DirectionUtils.ClockwiseOneCardinal(_sideOn));
                    int corner2ExtraHeight = _landOn.GetExtraHeight(DirectionUtils.CounterClockwiseOneCardinal(_sideOn));
                    if (corner1ExtraHeight == 2 || corner2ExtraHeight == 2)
                    {
                        locZ += 1;
                    }
                }
                else
                {
                    //otherwise it is the higher of the two Z values
                    locZ = Math.Max(LocationOn.Z, adjacentLocation.Z);
                }
                
                //set the tile to the correct location
                _tile.X = locX;
                _tile.Y = locY;
                _tile.Z = locZ;

                //fence tiles have and edge factor so that when an object is in front of the fence (greater X) the Y position of the fence is lower
                //and when an object is behind the fence smaller X the Y position of the fence is higher
                _tile.EdgeFactor = -0.5f;
                if (_sideOn == OrdinalDirection.NorthWest || _sideOn == OrdinalDirection.SouthEast)
                {
                    _tile.EdgeFactor = 0.5f;
                }

                ////fence tiles should not be ordered with other fence tiles
                //_tile.ForcedLayering = ForcedLayerType.DontOrder;
                
                //determine if it in an A (bottom left to top right) fence OR a B (top left to bottom right) fence
                string fenceSide = "a";
                if (_sideOn == OrdinalDirection.NorthWest || _sideOn == OrdinalDirection.SouthEast)
                {
                    fenceSide = "b";
                }

                //determine the height for each corner of the fence
                int corner1Height = _landOn.GetHeight(DirectionUtils.ClockwiseOneCardinal(_sideOn));
                int corner2Height = _landOn.GetHeight(DirectionUtils.CounterClockwiseOneCardinal(_sideOn));
                if (_sideOn == OrdinalDirection.NorthWest || _sideOn == OrdinalDirection.SouthWest)
                {
                    corner1Height = _landOn.GetHeight(DirectionUtils.CounterClockwiseOneCardinal(_sideOn));
                    corner2Height = _landOn.GetHeight(DirectionUtils.ClockwiseOneCardinal(_sideOn));
                }

                //fence grade is based on the height of each corner
                string fenceGrade = "1";
                if (corner1Height > corner2Height)
                {
                    fenceGrade = "0";
                }
                else if (corner2Height > corner1Height)
                {
                    fenceGrade = "2";
                }
                
                //different fence texture for differenet enclosure types
                string fenceType = "fence";
                if (_typeFor == EnclosureType.Pasture)
                {
                    fenceType = "fence2";
                }

                //determine texture for the fence                
                _tile.Texture = fenceType + "_" + fenceSide + fenceGrade;

                //order the tile (but dont order it with any fences, as fences draw fine in any order)
                _tile.Update();
            }
        }

        #endregion

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteObjectList<Enclosure>(_enclosuresBordered);
            writer.WriteObjectList<Fence>(_adjacentFences);
            writer.WriteObject(_landOn);
            writer.WriteEnum(_sideOn);
            writer.WriteBool(_gate);
            writer.WriteEnum(_typeFor);            
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
			_enclosuresBordered = reader.ReadObjectList<Enclosure>();
			_adjacentFences = reader.ReadObjectList<Fence>();			
			_landOn = reader.ReadObject<Land>();
			_sideOn = reader.ReadEnum<OrdinalDirection>();
			_gate = reader.ReadBool();
			_typeFor = reader.ReadEnum<EnclosureType>();
		}

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion
    }
}
