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
    /// Properties shared by all gameobjects.  GameObject are things seen in the game world.  They exsist on one or more peices of land.
    /// This is different than GameItems which are just seen in building inventories.
    /// </summary>
    public interface IGameObject : ISavable
    {
                        
        /// <summary>
        /// The locations of the game object.
        /// If the game object covers multiple locations this the center or main location.
        /// </summary>
        Location LocationOn
        {
            get;
        }
        
        /// <summary>
        /// The locations that the game object is currently on.        
        /// </summary>
        List<Location> AllLocationsOn
        {
            get;
        }
        
        /// <summary>
        /// The name the user has given the game object.  
        /// All objects have names, some are just never seen by the user.
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Is the game object being place, placed, or deleted
        /// </summary>
        PlacementState PlacementState
        {
            get;
        }


        /// <summary>
        /// Delete the game object
        /// </summary>
        void Delete();

        
    }
}
