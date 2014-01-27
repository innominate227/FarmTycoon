using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;
using TycoonGraphicsLib;

namespace FarmTycoon
{
    /// <summary>
    /// The effect an object has on a path
    /// </summary>
    public enum ObjectsEffectOnPath
    {
        None = 0, //the object has not effects on paths.
        Solid = 1, //the object cannot be walked through (the location the object is on will be un-walkable)
        DontWalk = 2, //the object CAN be walked through, but it is perferable that you dont walk on the object
        DoWalk = 4, //the object CAN be walked through, and the worker should try and walk on it (This is used for roads)
        DoWalkPlus = 8, //the object CAN be walked through, and the worker should try and really walk on it (This is used for highways)
        BlocksNE = 16, //the object prevents walking toward the northeast
        BlocksSE = 32, //the object prevents walking toward the southeast
        BlocksSW = 64, //the object prevents walking toward the southwest
        BlocksNW = 128, //the object prevents walking toward the northwest

        TravelNE = 256, //the direction of travel for the object is toward the northeast, traveling the opposite way is treated as DontWalk
        TravelSE = 512, //the direction of travel for the object is toward the southeast, traveling the opposite way is treated as DontWalk
        TravelSW = 1024, //the direction of travel for the object is toward the southwest, traveling the opposite way is treated as DontWalk
        TravelNW = 2048, //the direction of travel for the object is toward the northwest, traveling the opposite way is treated as DontWalk
    }

    /// <summary>
    /// Placement state of an object
    /// </summary>
    public enum PlacementState { BeingPlaced, Placed, WaitingToDelete, Deleted }

    /// <summary>
    /// Properties shared by all gameobjects.  GameObject are things seen in the game world.  They exsist on one or more peices of land.
    /// This is different than GameItems which are just seen in building inventories.
    /// </summary>
    public abstract class GameObject : ISavable, IGameObject
    {
        #region Events

        /// <summary>
        /// Raised when the name of the object changes
        /// </summary>
        public event Action NameChanged;

        #endregion

        #region Member Vars
        
        /// <summary>
        /// The name the user has given the game object.  
        /// All objects have names, some are just never seen by the user.
        /// TODO: should all objects have names or should this get thrown into an IHasName
        /// </summary>
        protected string _name = "";

        /// <summary>
        /// The effect the object has on paths
        /// </summary>
        private ObjectsEffectOnPath _pathEffect = ObjectsEffectOnPath.None;
                
        /// <summary>
        /// The locations that this game object is currently at.
        /// Derived classes should used AddLocationOn, and ClearLocationsOn to modify.
        /// </summary>
        private List<Location> _locations = new List<Location>();
                
        /// <summary>
        /// What is the state of placement is the game object in
        /// </summary>
        protected PlacementState _placementState = PlacementState.BeingPlaced;

        #endregion

        #region Setup

        /// <summary>
        /// GameObject constructor.  Setup, or ReadState should be called on the GameObject after is is created.        
        /// </summary>
        public GameObject()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The locations of the game object.
        /// If the game object covers multiple locations this the center or main location.
        /// </summary>
        public Location LocationOn
        {
            get 
            {
                if (_locations.Count == 0) { return null; }
                return _locations[0]; 
            }
        }
        
        /// <summary>
        /// The locations that the game object is currently on.        
        /// Do not edit this list diectly
        /// </summary>
        public List<Location> AllLocationsOn
        {
            get { return _locations; }
        }
        
        /// <summary>
        /// The name the user has given the game object.  
        /// All objects have names, some are just never seen by the player.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                if (NameChanged != null)
                {
                    NameChanged();
                }
            }
        }

        /// <summary>
        /// Is the game object being place, placed, or deleted
        /// </summary>
        public PlacementState PlacementState
        {
            get { return _placementState; }            
        }
        
        /// <summary>
        /// The effect the object has on paths
        /// </summary>        
        public ObjectsEffectOnPath PathEffect
        {
            get { return _pathEffect; }
            protected set 
            {
                _pathEffect = value;

                if (_placementState != FarmTycoon.PlacementState.BeingPlaced)
                {
                    //we have a different effect on paths now so invalidate any paths that pass through locations we are on
                    Program.Game.PathFinder.InvalidateLocations(AllLocationsOn);
                }
            }
        }
        
        #endregion

        #region Logic

        /// <summary>
        /// Remove the game object
        /// </summary>
        public void Delete()
        {
            //if it wasnt already delted, dont double delete it
            if (_placementState == FarmTycoon.PlacementState.Deleted) { return; }

            //if we already started the deletion process, then we dont need to do some things
            if (_placementState != FarmTycoon.PlacementState.WaitingToDelete)
            {
                //abort any scheudled tasks that depended on the game object
                GameState.Current.MasterTaskList.AbortScheduledTasksDependingOn(this);
                
                //remove from the master object list (it would not be there yet if it was not fully placed yet)
                if (_placementState == PlacementState.Placed)
                {
                    GameState.Current.MasterObjectList.Remove(this);
                }
            }
            
            //make sure the object can actually be deleted.
            //it can not be deleted if an action is currently being preformed that involves it
            if (GameState.Current.ActiveActionList.IsObjectInvolvedWithActiveAction(this))
            {
                //we are now in a waiting to delete state
                _placementState = FarmTycoon.PlacementState.WaitingToDelete;

                //add to list of objects in limbo, every few days the limbo manager will attempt to delete the object again
                GameState.Current.ObjectsInLimbo.AddObject(this);

                //call waiting to delete inner, incase the object wants to do something special when it gets into this state
                WaitingToDeleteInner();
                
                //dont actually delete the object yet
                return;
            }
            
            //set state as deleted
            PlacementState oldPlacementState = _placementState;
            _placementState = PlacementState.Deleted;
            
            //remove the object from the locations it was on, but remeber what it was on for later
            List<Location> locationTheObjectWasOn = AllLocationsOn;
            ClearLocationsOn();

            //call the delete method in the derived class
            DeleteInner();
            
            //if this object was effecting paths then invalidate paths in its area
            if (_pathEffect != ObjectsEffectOnPath.None && oldPlacementState != FarmTycoon.PlacementState.BeingPlaced)
            {
                Program.Game.PathFinder.InvalidateLocations(AllLocationsOn);
            }
        }

        /// <summary>
        /// Called when an object can not be deleted right away because there is an active action sequence that is using the object.
        /// The object may want to make itself invisible or something until it can actually be deleted.
        /// </summary>
        protected virtual void WaitingToDeleteInner()
        {
        }

        /// <summary>
        /// Called right after the object is deleted (called even if the object is still in the beingplaced state)
        /// </summary>
        protected virtual void DeleteInner()
        {
        }

        /// <summary>
        /// Called to have the game object update the tiles it maintains.
        /// </summary>
        public virtual void UpdateTiles()
        {
            //not all game objects have tiles associated, so by default do nothing.  Game objects with tiles should override
        }


        /// <summary>
        /// Add a locations to the list of locations this object is on.
        /// The center, or main location should be added first.
        /// </summary>
        public void AddLocationsOn(ICollection<Location> locationsOn)
        {
            foreach (Location location in locationsOn)
            {
                AddLocationOn(location);
            }
        }
                
        /// <summary>
        /// Add a locations to the list of locations this object is on.
        /// The center, or main location should be added first.
        /// </summary>
        public void AddLocationOn(Location locationOn)
        {
            _locations.Add(locationOn);

            //add the object to that location
            locationOn.AddObject(this);
        }

        /// <summary>
        /// Clear the list of land this object is on.
        /// This should be used in conjuntion with AddLandOn when an object is moving.
        /// </summary>
        public void ClearLocationsOn()
        {            
            //remove the object from the locations it was on
            foreach (Location locationOn in _locations)
            {
                locationOn.RemoveObject(this);
            }            

            //clear location on list
            _locations.Clear();
        }
                        
        /// <summary>
        /// Call when the game object is done being placed.
        /// Game object is added to master object list
        /// </summary>
        public virtual void DoneWithPlacement()
        {
            //make sure the object was not already placed
            Debug.Assert(_placementState == PlacementState.BeingPlaced);
            
            //add to master objects list
            GameState.Current.MasterObjectList.Add(this);
               
            //set state to placed
            _placementState = PlacementState.Placed;  
          
            //if this object effect paths the invalidate paths in its area            
            if (_pathEffect != ObjectsEffectOnPath.None)
            {
                Program.Game.PathFinder.InvalidateLocations(AllLocationsOn);
            }
        }
        
        #endregion

        #region Save Load
        public virtual void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteString(_name);
            writer.WriteEnum(_pathEffect);
            writer.WriteObjectList<Location>(_locations);
            writer.WriteEnum(_placementState);
        }

        public virtual void ReadStateV1(StateReaderV1 reader)
        {
            _name = reader.ReadString();
            _pathEffect = reader.ReadEnum<ObjectsEffectOnPath>();
            _locations = reader.ReadObjectList<Location>();
            _placementState = reader.ReadEnum<PlacementState>();
        }

        public virtual void AfterReadStateV1()
        {
        }
        #endregion

    }
}
