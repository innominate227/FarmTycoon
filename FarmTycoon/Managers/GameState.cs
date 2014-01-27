using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;

namespace FarmTycoon
{
    /// <summary>
    /// Top level object that contains reference to everything related to the state of the game
    /// </summary>    
    public class GameState : ISavable
    {
        #region Current
        /// <summary>
        /// Reference to the currently loaded game state
        /// </summary>
        private static GameState _current;

        /// <summary>
        /// Reference to the currently loaded game state
        /// </summary>
        public static GameState Current
        {
            get
            {
                return _current;
            }
        }
        #endregion


        #region Member Vars
        
        /// <summary>
        /// Manages the game clandar, (the current date)
        /// </summary>        
        private Calandar _calandar;

        /// <summary>
        /// All Locations that exsis in the Game
        /// </summary>
        private LocationManager _locations;

        /// <summary>
        /// List containing all tasks that have been created in the game
        /// </summary>
        private MasterTaskList _masterTaskList;
        
        /// <summary>
        /// List containing all actions that are currently being done
        /// </summary>
        private ActiveActionList _activeActionList;

        /// <summary>
        /// List contains all objects that have been created in the game
        /// </summary>
        private MasterObjectList _masterObjectList;
        
        /// <summary>
        /// List of objects that could not be deleted rirght away because an action was being done that involved the object
        /// </summary>
        private ObjectsInLimboManager _objectsInLimbo;
        
        /// <summary>
        /// List of all items that exsits in one of the players workers or buildings
        /// </summary>
        private ItemList _playersItemsList;

        /// <summary>
        /// List of all items that exsits in one of the players buildings, and can currently be sold.
        /// Meaning it is in a storage building, and is not currently reserved by a worker.
        /// </summary>
        private ItemList _playersSellableItemsList;

        /// <summary>
        /// Contains all ItemTypes created.  Creates new ItemTypes.
        /// </summary>
        private ItemTypePool _itemPool;

        /// <summary>
        /// Handel issues that the workers, tasks, or other game objects are having that need to be reported to the user
        /// </summary>
        private IssueManager _issueManager;

        /// <summary>
        /// Handels assigning workers to tasks.
        /// </summary>
        private WorkerAssigner _workerAssigner;

        /// <summary>
        /// Handels starting tasks at the approprate time
        /// </summary>
        private TaskStarter _taskStarter;
                
        /// <summary>
        /// Manages how much money the player has created, and how much was spent on what.
        /// </summary>
        private Treasury _treasury;
        
        /// <summary>
        /// The game prices
        /// </summary>
        private Prices _prices;
        
        /// <summary>
        /// Game weather
        /// </summary>
        private UIStrings _uiStrings;

        /// <summary>
        /// Items available in the store
        /// </summary>
        private ItemList _storeStock;
                        
        /// <summary>
        /// Tracks the values the player last used for various dialog in the game.
        /// </summary>
        private LastUsedValues _lastUsedValues;
        
        #endregion
        
        #region Setup Delete

        /// <summary>
        /// Create a game.  Setup, or ReadState should be called on the Game before it is used.
        /// </summary>
        public GameState()
        {
            //the newly created state is the current game state
            _current = this;
        }
        
        /// <summary>
        /// Init the Game object
        /// </summary>
        public void Setup()
        {
            _calandar = new Calandar();
            _calandar.Setup();

            _locations = new LocationManager();         
            _masterObjectList = new MasterObjectList();
            _objectsInLimbo = new ObjectsInLimboManager();
            _objectsInLimbo.Setup();
            _masterTaskList = new MasterTaskList();
            _masterTaskList.Setup();
            _activeActionList = new ActiveActionList();
            _playersItemsList = new ItemList();
            _playersSellableItemsList = new ItemList();
            _itemPool = new ItemTypePool();

            _uiStrings = new UIStrings();
            _treasury = new Treasury();
            _treasury.Setup();
            _prices = new Prices();
            _storeStock = new ItemList();            

            _issueManager = new IssueManager();
            _workerAssigner = new WorkerAssigner();
            _taskStarter = new TaskStarter();
            _taskStarter.Setup();

            _lastUsedValues = new LastUsedValues();
        }
        

        /// <summary>
        /// Delete the game
        /// </summary>
        public void Delete()
        {
            //abourt all tasks and actions (so that actions do not stop objects from being delted)
            _masterTaskList.AbortAll();
            _activeActionList.AbortAll();

            _masterObjectList.DeleteAll();
            _calandar.Delete();
            _objectsInLimbo.Delete();

            //make is so there is no longer a current game state (this should kill the reference to game state and allow all game object to be garbage collected)
            _current = null;
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// All locations that exsis in the Game
        /// </summary>
        public LocationManager Locations
        {
            get { return _locations; }
        }

        /// <summary>
        /// List containing task sceudles, and tasks that have been satrted (or attmpted to be started)
        /// </summary>
        public MasterTaskList MasterTaskList
        {
            get { return _masterTaskList; }
        }

        /// <summary>
        /// List containing all actions that are currently being done
        /// </summary>
        public ActiveActionList ActiveActionList
        {
            get { return _activeActionList; }
        }
        
        /// <summary>
        /// List contains all objects that have been created in the game
        /// </summary>
        public MasterObjectList MasterObjectList
        {
            get { return _masterObjectList; }
        }


        /// <summary>
        /// List of objects that could not be deleted rirght away because an action was being done that involved the object
        /// </summary>
        public ObjectsInLimboManager ObjectsInLimbo
        {
            get { return _objectsInLimbo; }
        }

        /// <summary>
        /// List of all items that exsits in one of the players workers or buildings
        /// </summary>
        public ItemList PlayersItemsList
        {
            get { return _playersItemsList; }
        }

        /// <summary>
        /// List of all items that exsits in one of the players buildings, and can currently be sold.
        /// Meaning it is in a storage building, and is not currently reserved by a worker.
        /// </summary>
        public ItemList PlayersSellableItemsList
        {
            get { return _playersSellableItemsList; }
        }
        
        /// <summary>
        /// Contains all ItemTypes created.  Creates new ItemTypes.
        /// </summary>
        public ItemTypePool ItemPool
        {
            get { return _itemPool; }
        }
        
        /// <summary>
        /// Handel issues that the workers, tasks, or other game objects are having that need to be reported to the user
        /// </summary>
        public IssueManager IssueManager
        {
            get { return _issueManager; }
        }

        /// <summary>
        /// Handels starting tasks at the approprate time
        /// </summary>  
        public TaskStarter TaskStarter
        {
            get { return _taskStarter; }
        }

        /// <summary>
        /// Handels assigning workers to tasks.
        /// </summary>  
        public WorkerAssigner WorkerAssigner
        {
            get { return _workerAssigner; }
        }

        /// <summary>
        /// Manages the game clandar, (the current date)
        /// </summary>
        public Calandar Calandar
        {
            get { return _calandar; }
        }

        /// <summary>
        /// The games prices
        /// </summary>
        public Prices Prices
        {
            get { return _prices; }
        }
        
        /// <summary>
        /// Manages how much money the player has created, and how much was spent on what.
        /// </summary>
        public Treasury Treasury
        {
            get { return _treasury; }
        }

        /// <summary>
        /// UI Strings that are set by the game script
        /// </summary>
        public UIStrings UIStrings
        {
            get { return _uiStrings; }
        }

        /// <summary>
        /// Items available in the store.
        /// </summary>
        public ItemList StoreStock
        {
            get { return _storeStock; }
        }
                            
        /// <summary>
        /// Tracks the values the player last used for various dialog in the game.
        /// </summary>
        public LastUsedValues LastUsedValues
        {
            get { return _lastUsedValues; }
        }


        #endregion

        #region Save Load
		public void WriteStateV1(StateWriterV1 writer)
		{
			writer.WriteObject(_calandar);
			writer.WriteObject(_locations);
			writer.WriteObject(_masterTaskList);
			writer.WriteObject(_activeActionList);
			writer.WriteObject(_masterObjectList);
			writer.WriteObject(_objectsInLimbo);
			writer.WriteObject(_playersItemsList);
			writer.WriteObject(_playersSellableItemsList);
			writer.WriteObject(_itemPool);
			writer.WriteObject(_issueManager);
			writer.WriteObject(_workerAssigner);
			writer.WriteObject(_taskStarter);
			writer.WriteObject(_treasury);
			writer.WriteObject(_prices);
			writer.WriteObject(_uiStrings);
			writer.WriteObject(_storeStock);
			writer.WriteObject(_lastUsedValues);
		}
		
		public void ReadStateV1(StateReaderV1 reader)
		{
			_calandar = reader.ReadObject<Calandar>();
			_locations = reader.ReadObject<LocationManager>();
			_masterTaskList = reader.ReadObject<MasterTaskList>();
			_activeActionList = reader.ReadObject<ActiveActionList>();
			_masterObjectList = reader.ReadObject<MasterObjectList>();
			_objectsInLimbo = reader.ReadObject<ObjectsInLimboManager>();
			_playersItemsList = reader.ReadObject<ItemList>();
			_playersSellableItemsList = reader.ReadObject<ItemList>();
			_itemPool = reader.ReadObject<ItemTypePool>();
			_issueManager = reader.ReadObject<IssueManager>();
			_workerAssigner = reader.ReadObject<WorkerAssigner>();
			_taskStarter = reader.ReadObject<TaskStarter>();
			_treasury = reader.ReadObject<Treasury>();
			_prices = reader.ReadObject<Prices>();
			_uiStrings = reader.ReadObject<UIStrings>();
			_storeStock = reader.ReadObject<ItemList>();
			_lastUsedValues = reader.ReadObject<LastUsedValues>();
		}
		
		public void AfterReadStateV1()
		{
            //check all game objects that use Info to make sure the Info was loaded.
            foreach (IHasInfo gameObject in _masterObjectList.FindAll<IHasInfo>())
            {
                //if the info could not be loaded delete the object
                if (gameObject.Info == null)
                {
                    gameObject.Delete();
                }
            }
		}
		
		#endregion        
    }
}
