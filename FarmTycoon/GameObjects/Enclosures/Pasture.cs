using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// A pasture holds Animals it acts like storage building, and also like a Planted Area.
    /// It tracks the composite quality for all the animals inside it.
    /// </summary>
    public class Pasture : Enclosure, IHasQuality, IHasActionLocation, IStorageBuilding
    {
        #region Member Vars
        
        /// <summary>
        /// Inventory of the animals in the pasture
        /// </summary>
        private Inventory _inventory;

        /// <summary>
        /// Composite quality averaging the quality of all animals the pasture
        /// null if the pasture is empty.
        /// </summary>
        private CompositeQuality _quality;

        /// <summary>
        /// Animals in the pasture
        /// </summary>
        private List<Animal> _animals = new List<Animal>();

        /// <summary>
        /// Troughs in the pasture
        /// </summary>
        private List<Trough> _troughs = new List<Trough>();

        #endregion
        
        #region Setup Delete

        public Pasture() : base() 
        {
        }
                
        /// <summary>
        /// Create a pasture given the enclosure that encloses it
        /// </summary>
        public override void Setup(List<Land> allLand, List<Fence> borderFences)
        {
            base.Setup(allLand, borderFences);                
                    
            //create inventory
            _inventory = new Inventory();
            _inventory.SetUp((PastureInfo)FarmData.Current.PastureInfo);

            _quality = new CompositeQuality();

            //handel when animals are added/removed from the pasture
            _inventory.UnderlyingList.ItemAdded += new Action<ItemType>(AnimalAdded);
            _inventory.UnderlyingList.ItemRemoved += new Action<ItemType>(AnimalRemoved); 
        }

        /// <summary>
        /// Called when the planted area is deleted
        /// </summary>
        protected override void DeleteInner()
        {
            //delete all trough, and animals still in the pasture
            foreach (Trough trough in _troughs)
            {
                trough.Delete();
            }
            foreach (Animal animal in _animals)
            {
                animal.Delete();
            }

            _inventory.Delete();
            base.DeleteInner();
        }
        
        #endregion

        #region Properties
        
        /// <summary>
        /// Inventory of the animals in the pasture
        /// </summary>
        public Inventory Inventory
        {
            get { return _inventory; }
        }
       
        /// <summary>
        /// All animal objects int the pasture (read only)
        /// Do not modify this list, instead add the animal to the Pastures inventory.  And it will appear in the list.
        /// </summary>
        public List<Animal> Animals
        {
            get { return _animals; }
        }
        
        /// <summary>
        /// Troughs in the pasture. (Edit this list directly)
        /// </summary>        
        public List<Trough> Troughs
        {
            get { return _troughs; }
        }

        /// <summary>
        /// Return true or false depending on if the pasture has animals in it
        /// </summary>
        public bool HasAnimals
        {
            get { return (_animals.Count > 0); }
        }

        /// <summary>
        /// The name of animals in the pasture, or "None" if no animals
        /// </summary>
        public string AnimalType
        {
            get
            {
                if (_animals.Count == 0) { return "None"; }
                return _animals[0].AnimalInfo.AnimalType.Name;
            }
        }

        /// <summary>
        /// Info for the animals in the pasture or null if no animals
        /// </summary>
        public AnimalInfo AnimalInfo
        {
            get
            {
                if (_animals.Count == 0) { return null; }
                return _animals[0].AnimalInfo;
            }
        }

        /// <summary>
        /// Average Quality of all animals in the pasture
        /// </summary>
        public IQuality Quality
        {
            get { return _quality; }
        }
        
        /// <summary>
        /// StorageBuildingInfo for this pasture
        /// </summary>  
        public IStorageBuildingInfo StorageBuildingInfo
        {
            get { return FarmData.Current.PastureInfo; }
        }

        #endregion

        #region Logic
        
        public override void UpdateTiles()
        {
            //pasture has no tiles.
        }        

        /// <summary>
        /// Called when an animal is added to the pastures inventory
        /// </summary>
        private void AnimalAdded(ItemType animalType)
        {
            Animal animal = (Animal)animalType.ItemObject;

            //make sure its the same type as other animals in the pasture
            if (_animals.Count > 0)
            {
                Debug.Assert(_animals[0].AnimalInfo == animal.AnimalInfo);
            }
            
            //add quality to overal quality            
            _quality.AddQuality((Quality)animal.Quality);

            //tell the anumal it was added to this pasture
            animal.AddedToPasture(this);
            
            //add to list of animals
            _animals.Add(animal); 
   
            //update the "Space" trait for all animals in pasture
            UpdateAnimalSpace();
        }

        /// <summary>
        /// Called when an animal is removed from the pastures inventory
        /// </summary>
        private void AnimalRemoved(ItemType animalType)
        {
            Animal animal = (Animal)animalType.ItemObject;

            //remove from overall quality
            _quality.RemoveQuality((Quality)animal.Quality);
            
            //remove from pasture
            _animals.Remove(animal);
            
            //update the "Space" trait for all animals in pasture
            UpdateAnimalSpace();
        }

        /// <summary>
        /// update the "Space" trait for all animals in pasture, based on current number of animals and total tiles in pasture
        /// </summary>
        private void UpdateAnimalSpace()
        {
            //make sure we have animals
            if (_animals.Count == 0) { return; }

            //calculate space per animal
            int space = (int)Math.Round((double)(base.OrderedLand.Count * 10) / _animals.Count);

            foreach (Animal animal in _animals)
            {
                bool hasSpaceTrait = animal.Traits.HasTrait(SpecialTraits.SPACE_TRAIT);
                if (hasSpaceTrait)
                {
                    animal.Traits.SetTraitValue(SpecialTraits.SPACE_TRAIT, space);                    
                }
            }
        }

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteObject(_inventory);
            writer.WriteObject(_quality);
            writer.WriteObjectList<Animal>(_animals);
            writer.WriteObjectList<Trough>(_troughs);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _inventory = reader.ReadObject<Inventory>();
            _quality = reader.ReadObject<CompositeQuality>();
            _animals = reader.ReadObjectList<Animal>();
            _troughs = reader.ReadObjectList<Trough>();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();

            //handel when animals are added/removed from the pasture
            _inventory.UnderlyingList.ItemAdded += new Action<ItemType>(AnimalAdded);
            _inventory.UnderlyingList.ItemRemoved += new Action<ItemType>(AnimalRemoved); 
        }
        #endregion

    }
}

