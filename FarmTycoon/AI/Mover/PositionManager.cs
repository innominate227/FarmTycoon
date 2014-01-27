using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{

    /// <summary>
    /// Manages the position of a tile (and optionally a game object) that travels between locations in the game world.    
    /// </summary>
    public class PositionManager : ISavable
    {
        #region Events

        /// <summary>
        /// Raised every time the position of the tile and or game object being managed is changed.
        /// </summary>
        public event Action PositionUpdated;

        #endregion

        #region Member Vars

        /// <summary>
        /// Location leaving, this is always adjacent to _going
        /// </summary>
        private Location _leaving;

        /// <summary>
        /// Location going, this is always adjacent to _leaving
        /// </summary>
        private Location _going;
        
        /// <summary>
        /// The direction facing
        /// </summary>
        private OrdinalDirection _direction;
        
        /// <summary>
        /// number between 1 and 16 that tells how close we are to _going
        /// </summary>
        private int _distToGoing = 16;
        
        /// <summary>
        /// The tile to move.
        /// </summary>
        private MobileGameTile _tile;

        /// <summary>
        /// The game objects whos position should be updated, this can be null
        /// </summary>
        private GameObject _object;
        
        #endregion

        #region Setup

        /// <summary>
        /// Create an object mover.  Call Setup or ReadState before using
        /// </summary>        
        public PositionManager()
        {
        }
        
        /// <summary>
        /// Setup the position manager to move a tile, set the tile using SetTileToMove
        /// </summary>        
        public void Setup(Location startLocation)
        {
            Setup(startLocation, null);
        }
        
        /// <summary>
        /// Setup the position manager to move the gameobject passed (and possible a tile is set using SetTileToMove)
        /// </summary>        
        public void Setup(Location startLocation, GameObject gameObject)
        {
            _object = gameObject;

            //set the initial leaving and going
            _leaving = startLocation.GetAdjacent(OrdinalDirection.SouthEast);
            _going = startLocation;
            _distToGoing = 16;
        }

        /// <summary>
        /// Set the tile to be moved by the position manager.      
        /// </summary>
        public void SetTileToMove(MobileGameTile tile)
        {
            _tile = tile;
        }
        
        #endregion
        
        #region Properties

        /// <summary>
        /// number between 1 and 16 that tells how close we are to _going
        /// </summary>
        public int DistToGoing
        {
            get { return _distToGoing; }
            set { _distToGoing = value; }
        }
        
        /// <summary>
        /// Location going, this must always be adjacent to _leaving
        /// </summary>
        public Location Going
        {
            get { return _going; }
            set { _going = value; }
        }

        /// <summary>
        /// Location leaving, this must always be adjacent to _going  
        /// </summary>
        public Location Leaving
        {
            get { return _leaving; }
            set { _leaving = value; }
        }
        
        /// <summary>
        /// The direction facing
        /// </summary>
        public OrdinalDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Sets Direction to the direction one would normally face when walking from going to leaving.
        /// The direction that faces going.
        /// </summary>
        public void SetWalkingDirection()
        {            
            foreach (OrdinalDirection possibleDirection in DirectionUtils.AllOrdinalDirections)
            {
                if (_leaving.GetAdjacent(possibleDirection) == _going)
                {
                    _direction = possibleDirection;
                    return;
                }
            }

            //if we got here going is not adjacent to leaving
            Debug.Assert(false);
        }


                
        /// <summary>
        /// Update the position of the tile and the game object
        /// </summary>
        public void UpdatePosition()
        {
            UpdateTilePosition();
            if (_object != null)
            {
                UpdateGameObjectPosition();
            }
            if (PositionUpdated != null)
            {
                PositionUpdated();
            }
        }

        /// <summary>
        /// Update the position of the tile
        /// </summary>
        private void UpdateTilePosition()
        {
            //determine the location for the tile
            float locX = _leaving.X + ((_going.X - _leaving.X) / 16.0f * _distToGoing);
            float locY = _leaving.Y + ((_going.Y - _leaving.Y) / 16.0f * _distToGoing);
            float locZ = _leaving.Z + ((_going.Z - _leaving.Z) / 16.0f * _distToGoing);
            
            //determine the two letters for the direction were facing
            string direction_facing = "_" + DirectionUtils.OrdinalDirectionToAbreviation(_direction);            

            //update rendering position for the tile
            _tile.X = locX;
            _tile.Y = locY;
            _tile.Z = locZ;
            _tile.Append = direction_facing;            
            _tile.Update();
        }
        
        /// <summary>
        /// Update the position of the game object
        /// </summary>
        private void UpdateGameObjectPosition()
        {
            _object.ClearLocationsOn();
            _object.AddLocationOn(_going);
            _object.AddLocationOn(_leaving);
        }

        #endregion
        
        #region Save Load
		public void WriteStateV1(StateWriterV1 writer)
		{
			writer.WriteObject(_leaving);
			writer.WriteObject(_going);
			writer.WriteEnum(_direction);
			writer.WriteInt(_distToGoing);
			writer.WriteObject(_object);
		}
		
		public void ReadStateV1(StateReaderV1 reader)
		{
			_leaving = reader.ReadObject<Location>();
			_going = reader.ReadObject<Location>();
			_direction = reader.ReadEnum<OrdinalDirection>();
			_distToGoing = reader.ReadInt();
			_object = reader.ReadObject<GameObject>();
		}
		
		public void AfterReadStateV1()
		{
		}
		#endregion

    }
}

