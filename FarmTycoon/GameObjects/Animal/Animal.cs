using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;

namespace FarmTycoon
{

    /// <summary>
    /// Represnet a farm animal.  Animals can be bought, sold, or sent into a production building to be turned into another item type.
    /// Animals have traits.  The traits change based on time, and items.  An Animal has a quality determined by it the current and past values of it traits.
    /// An animal is driven by events wich allow it to decide to Consume Items, Have Babys, or Die.
    /// Each instance of an Animal has its own ItemType, and the quailty of that item type matches the quality of the animal.  Also the item exsists for
    /// the duration (or longer) then the exsistance of the Animal.  This differs from Crops where the item is produced when the crop is deleted, and multiple 
    /// Crop objects representing the same type of crop will create the same ItemType.
    /// </summary>
    public partial class Animal : GameObject, IHasQuality, IHasTraits, IActor, IHasEvents, IHasTextureManager, IHasDelays, IHasInfo
    {                
        #region Member Vars

        /// <summary>
        /// Info for the animal
        /// </summary>
        private AnimalInfo _animalInfo;

        /// <summary>
        /// The item type associated with this animal,
        /// each animal has its own item type
        /// </summary>
        private ItemType _animalItemType;
        
        #endregion

        #region Setup Delete

        /// <summary>
        /// Create an animal.
        /// Setup or ReadState must be called after the animal is created.
        /// </summary>
        public Animal() : base() 
        {
        }

        /// <summary>
        /// Create a animal using the passed animal info
        /// </summary>
        public void Setup(AnimalInfo animalInfo, ItemType animalsItemType, Location intialLocation)
        {
            _animalInfo = animalInfo;
            _animalItemType = animalsItemType;
            _animalItemType.ItemObject = this;

            AddLocationOn(intialLocation);

            SetupPosition();
            SetupQuality();
            SetupEvents();  //events needs to be after quality. Because it will check conditions for events on creation.
            SetupActions();
            
            //refresh tile
            UpdateTiles();
            
            //animals are never in a placing state so we are done with placement
            this.DoneWithPlacement();
        }
        
        /// <summary>
        /// Called when the Animal is deleted
        /// </summary>
        protected override void DeleteInner()
        {
            DeleteEvents();
            DeleteActions();
            DeletePosition();
            DeleteQuality();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Info for the animal
        /// </summary>
        public IInfo Info
        {
            get { return _animalInfo; }
        }

        /// <summary>
        /// Info for the animal
        /// </summary>
        public AnimalInfo AnimalInfo
        {
            get { return _animalInfo; }
        }

        /// <summary>
        /// The item type associated with this animal
        /// </summary>
        public ItemType AnimalItemType
        {
            get { return _animalItemType; }
        }
        
        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteInfo(_animalInfo);
            writer.WriteObject(_animalItemType);
            WriteStateV1Actions(writer);
            WriteStateV1Events(writer);
            WriteStateV1Follow(writer);
            WriteStateV1Position(writer);
            WriteStateV1Quality(writer);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _animalInfo = reader.ReadInfo<AnimalInfo>();
            _animalItemType = reader.ReadObject<ItemType>();
            ReadStateV1Actions(reader);
            ReadStateV1Events(reader);
            ReadStateV1Follow(reader);
            ReadStateV1Position(reader);
            ReadStateV1Quality(reader);
        }

        #endregion

    }
}
