using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// Workers move arround the game world performing action sequences assigned to them as part of a task created by the player.
    /// </summary>
    public partial class Worker : GameObject, IActor, IHasTraits, IHasDelays, IHasInfo
    {
        /// <summary>
        /// Amount of space that is needed for the worker to walk
        /// </summary>
        public static int WORKER_HEIGHT = 4;

        #region Memeber Vars
                                
        /// <summary>
        /// The workers inventory
        /// </summary>
        private Inventory _inventory;

        /// <summary>
        /// List of animals following the worker
        /// </summary>
        private List<Animal> _followingAnimals;

        /// <summary>
        /// Traits for the worker
        /// </summary>
        private TraitSet _traits;

        /// <summary>
        /// Delays for the worker
        /// </summary>
        private DelaySet _delays;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a new worker call Setup or LoadState after creation
        /// </summary>
        public Worker() : base() 
        {
        }

        /// <summary>
        /// Create a worker starting at a location passed
        /// </summary>
        public void Setup(Location location)
        {
            WorkerInfo workerInfo = FarmData.Current.WorkerInfo;

            AddLocationOn(location);            
            _inventory = new Inventory();
            _inventory.SetUp(workerInfo);

            _followingAnimals = new List<Animal>();

            _traits = new TraitSet();
            _traits.Setup(workerInfo);

            _delays = new DelaySet();
            _delays.Setup(workerInfo, this);

            SetupPosition();
        }
        
        /// <summary>
        /// Start the worker actually moving and doing work
        /// </summary>        
        public override void DoneWithPlacement()
        {            
            base.DoneWithPlacement();

            //start having the worker do the default plan
            DoDefaultActionSequence();

            //have the worker start moving
            _workerMover.StartMoving();

            //the new worker is avaiable to do a task
            GameState.Current.WorkerAssigner.WorkerNowAvaiable(this);
        }


        /// <summary>
        /// Called when the object is deleted
        /// </summary>
        protected override void DeleteInner()
        {
            DeletePosition();
            _inventory.Delete();
            _traits.Delete();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The workers inventory
        /// </summary>
        public Inventory Inventory
        {
            get { return _inventory; }            
        }

        /// <summary>
        /// List of animals following the worker.  Animals will add/remove themself to this list as they are told to follow the worker
        /// </summary>
        public List<Animal> FollowingAnimals
        {
            get { return _followingAnimals; }
        }
        
        /// <summary>
        /// Traits for the worker
        /// </summary>
        public TraitSet Traits
        {
            get { return _traits; }
        }

        /// <summary>
        /// Delays for the worker
        /// </summary>
        public DelaySet Delays
        {
            get { return _delays; }
        }
        
        /// <summary>
        /// Info for the worker
        /// </summary>
        public IInfo Info
        {
            get { return FarmData.Current.WorkerInfo; }
        }


        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteObject(_inventory);
            writer.WriteObjectList<Animal>(_followingAnimals);
            writer.WriteObject(_traits);
            writer.WriteObject(_delays);
            WriteStateV1Equipment(writer);
            WriteStateV1Position(writer);
            WriteStateV1Task(writer);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _inventory = reader.ReadObject<Inventory>();
            _followingAnimals = reader.ReadObjectList<Animal>();
            _traits = reader.ReadObject<TraitSet>();
            _delays = reader.ReadObject<DelaySet>();
            ReadStateV1Equipment(reader);
            ReadStateV1Position(reader);
            ReadStateV1Task(reader);
        }

        #endregion
    }
}
