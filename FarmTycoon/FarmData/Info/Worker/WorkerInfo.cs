using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{
    /// <summary>
    /// Info shared by all workers
    /// </summary>
    public class WorkerInfo : IInventoryInfo, ITraitsInfo, /*IEventsInfo,*/ ITexturesInfo, IInfo, IDelaysInfo
    {
        /// <summary>
        /// Unique name for the WorkerInfo
        /// </summary>
        public const string UNIQUE_NAME = "Worker_Worker";

        /// <summary>
        /// Size of the workers inventory
        /// </summary>
        private int _inventorySize = 100;
        


        /// <summary>
        /// The traits that effect the worker
        /// </summary>
        private TraitInfoSet _traits;

        /// <summary>
        /// The events that effect what the animal does
        /// </summary>
        private List<ObjectEventInfo> _events = new List<ObjectEventInfo>();
        
        /// <summary>
        /// Delays that determine how fast the worker walks, and does actions
        /// </summary>
        private DelayInfoSet _delays;

        /// <summary>
        /// The textures this worker can show
        /// </summary>
        private TexturesInfoSet _textures;
        
        private WorkerInfo()
        {
        }


        public WorkerInfo(XmlReader reader, FarmData farmInfo)
        {
            reader.ReadToFollowing("Worker");
            if (reader.MoveToAttribute("InventorySize"))
            {
                _inventorySize = reader.ReadContentAsInt();
            }

            _traits = new TraitInfoSet(this);
            _delays = new DelayInfoSet(this);
            _textures = new TexturesInfoSet(this);

            while (reader.ReadNextElement())
            {
                _traits.ReadElement(reader, farmInfo);
                _delays.ReadElement(reader, farmInfo);
                _textures.ReadElement(reader, farmInfo);   

                if (reader.Name == "Event")
                {
                    ObjectEventInfo objectEvent = new ObjectEventInfo(UniqueName, reader.ReadSubtree(), farmInfo);
                    _events.Add(objectEvent);
                }
            }

            _traits.InitSet();
            _delays.InitSet();
            
        }

        /// <summary>
        /// Capacity of the inventory of the worker
        /// </summary>
        public int Capacity
        {
            get { return _inventorySize; }
        }
        

        /// <summary>
        /// Types of items allowed on the worker
        /// A value of null indicates all types are allowed
        /// </summary>
        public HashSet<ItemTypeInfo> AllowedTypes
        {
            get { return null; }
        }
                
        public bool OneBaseTypeInventory
        {
            get { return false; }
        }


        /// <summary>
        /// The traits that effect the quality of the worker
        /// </summary>
        public TraitInfoSet Traits
        {
            get { return _traits; }
        }


        ///// <summary>
        ///// The desires that effect what the worker does
        ///// </summary>
        //public List<ObjectEventInfo> Events
        //{
        //    get { return _events; }
        //}

        /// <summary>
        /// The textures this worker can show
        /// </summary>
        public TexturesInfoSet Textures
        {
            get { return _textures; }
        }

        /// <summary>
        /// Delays that determine how fast the worker walks, and does actions
        /// </summary>
        public DelayInfoSet Delays
        {
            get { return _delays; }
        }


        /// <summary>
        /// If true when items are added to this inventory they should be added to the global players items list
        /// </summary>
        public bool UpdatePlayerItemsList
        {
            get { return true; }
        }

        /// <summary>
        /// If true when items are added to this inventory they should be added to the global players sellable items list.
        /// </summary>
        public bool UpdatePlayerSellableItemsList
        {
            get { return false; }
        }


        public string UniqueName
        {
            get { return UNIQUE_NAME; }
        }

    }
}
