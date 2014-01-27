using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{

    /// <summary>
    /// Properties for a Type of game item.    
    /// Game items are manages using ItemList, or Inventories.  
    /// For most game items there will exsist more than one of the same item.  For instace you may have 500 bushels of "Quality 10 Wheat".
    /// There is just one "Quality 10 Wheat" ItemType object Inventories determine the amount of "Quality 10 Wheat" a building or a worker has.
    /// Item types that are similar share the same ItemTypeInfo.  For instance "Quality 10 Wheat" and "Quality 5 Wheat" share the same ItemTypeInfo.
    /// Sometimes when looking at an items type you may only care if it has a certain "ItemTypeInfo".
    /// There are three main ways ItemTypes relate to ItemTypeInfos:
    /// 1. There is just one ItemType for the ItemTypeInfo.  For instance "Wheat Seed", this is for items where there is no quality involved.
    /// 2. There are 10 ItemTypes for the ItemTypeInfo. For insatance "Quality 1 Wheat"-"Quality 10 Wheat".  The ItemType differ only by quality
    ///    Generally the normvalture for these ItemTypes is "[ItemTypeInfoName]_[Quality]"
    /// 3. There are Many ItemTypes for the ItemTypeInfo. For insatance "Betsy The Cow", "Bob The Cow".  The ItemType differ by quality, and each ItemType
    ///    has a directly associated GameObject (in this case a cow).  When this is the case you will only ever see 0 or 1 of these ItemTypes in an 
    ///    ItemList\Inventory.  Generally the nomenclature for these ItemTypes is "[ItemTypeInfoName]_[RandomLetterString]". 
    /// The ItemTypeInfo is also called the BaseType of the item.
    /// </summary>
    public partial class ItemType : ISavable
    {

        #region Member Vars

        /// <summary>
        /// Info object for the item type, shared between similar item types.
        /// </summary>
        private ItemTypeInfo _typeInfo;

        /// <summary>
        /// Quality for items of this type, this value can change or be constant depending on the type of item.
        /// For instance "Wheat_2" will always have quality 2.  The item type "Wheat_2" is for quality 2 wheat.
        /// But for animals "Cow_ABCDE" will have its quality change over time.  (Each cow has its own unique item type.)
        /// </summary>
        private int _quality = 0;

        /// <summary>
        /// Items have a name and a unique name.  For instance "Wheat", and "Wheat_2"
        /// This would be "2" in that case.
        /// </summary>
        private string _uniquePartOfName;

        /// <summary>
        /// Some game items (like animals) have an associated object        
        /// </summary>
        private IGameObject _itemObject = null;

        /// <summary>
        /// Animals and Tractors have an age trait assoicated with the object, but we also need to know the value for this trait when the 
        /// item exsists only as an item and not as an object (before the item has been bought).  So we keep the age here as well.
        /// This is the age in days
        /// </summary>
        private int _age = 0;

        #endregion

        #region Setup

        /// <summary>
        /// Create a Game Item type.  Setup or load state should be called before using.
        /// </summary>
        public ItemType() { }

        /// <summary>
        /// Setup the item type
        /// </summary>
        public void Setup(ItemTypeInfo baseType, string uniquePartOfName)
        {
            _typeInfo = baseType;
            _quality = 0;
            _uniquePartOfName = uniquePartOfName;
        }

        #endregion

        #region Propeties

        #region ItemTypeInfo Properties
        
        /// <summary>
        /// The name of the item type, with out the unique part.
        /// For isntance "Wheat" instead of "Wheat_2"
        /// </summary>
        public string BaseName
        {
            get { return _typeInfo.Name; }
        }

        /// <summary>
        /// icon for the item.
        /// </summary>
        public string Icon
        {
            get { return _typeInfo.Icon; }
        }

        /// <summary>
        /// the size of the item.  The amount of space it takes up in a building.
        /// </summary>
        public int Size
        {
            get { return _typeInfo.Size; }
        }

        /// <summary>
        /// relationship between an ItemTypeInfo and the ItemTypes using the info
        /// </summary>
        public ItemTypeRelation ItemTypeRelation
        {
            get { return _typeInfo.ItemTypeRelation; }
        }

        /// <summary>
        /// Tags for the item
        /// </summary>
        public HashSet<string> Tags
        {
            get { return _typeInfo.Tags; }
        }

        /// <summary>
        /// Return if the item has the tag passed
        /// </summary>
        public bool HasTag(string tag)
        {
            return _typeInfo.HasTag(tag);
        }

        /// <summary>
        /// base item type for this item type.
        /// </summary>
        public ItemTypeInfo BaseType
        {
            get { return _typeInfo; }
        }
        
        #endregion
        
        /// <summary>
        /// The full name of the item type. Inclusing the unique part.
        /// For isntance "Wheat_2" instead of "Wheat"
        /// </summary>
        public string FullName
        {
            get { return _typeInfo.Name + "_" + _uniquePartOfName; }
        }
        
        /// <summary>
        /// Quality for items of this type between 0 and 9
        /// this value can change or may be constant depending on the type of item.
        /// For instance "Wheat_2" will always have quality 2.  The item type "Wheat_2" is for quality 2 wheat.
        /// But for animals "Cow_ABCDE" will have its quality change over time.  Each cow has its own unique item type.
        /// For items without quality this will always be 0
        /// </summary>
        public int Quality
        {
            get { return _quality; }
            set { _quality = value; }
        }

        /// <summary>
        /// Set the 0-9 scale wuality of this item based on the 0-100 scale wuality passed
        /// </summary>
        public void SetQualityFrom100Scale(int oneHundredScaleQuality)
        {
            _quality = (int)Math.Min(9, oneHundredScaleQuality / 100.0);
        }


        /// <summary>
        /// Some game items have an associated object
        /// This is that object
        /// </summary>
        public IGameObject ItemObject
        {
            get { return _itemObject; }
            set { _itemObject = value; }
        }


        /// <summary>
        /// Animals and Tractors have an age trait assoicated with the object, but we also need to know the value for this trait when the 
        /// item exsists only as an item and not as an object (before the item has been bought).  So we keep the age here as well.
        /// This is the age in days
        /// </summary>        
        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }

        #endregion

        #region Logic

        /// <summary>
        /// For an ItemType that has an associated game object (Animals and Equipment) create the associated game object.
        /// If the item type has no associated game object nothing happens.
        /// </summary>
        public void CreateAssociatedObject(Location location)
        {
            //if the item is for an animal create an animal game object for it
            AnimalInfo animalInfo = FarmData.Current.GetAnimalInfoForItemInfo(_typeInfo);
            if (animalInfo != null)
            {
                Animal animal = new Animal();
                animal.Setup(animalInfo, this, location);
                _itemObject = animal;
                return;
            }

            //if the item is for equipmnet create equipmnet game object for it
            EquipmentInfo equipmentInfo = FarmData.Current.GetEquipmentInfoForItemInfo(_typeInfo);
            if (equipmentInfo != null)
            {
                Equipment equipment = new Equipment();
                equipment.Setup(equipmentInfo, this);
                _itemObject = equipment;
                return;
            }
        }

        #endregion

        #region Save Load
		public void WriteStateV1(StateWriterV1 writer)
		{
			writer.WriteInfo(_typeInfo);
			writer.WriteInt(_quality);
			writer.WriteString(_uniquePartOfName);
			writer.WriteObject(_itemObject);
			writer.WriteInt(_age);
		}
		
		public void ReadStateV1(StateReaderV1 reader)
		{
			_typeInfo = reader.ReadInfo<ItemTypeInfo>();
			_quality = reader.ReadInt();
			_uniquePartOfName = reader.ReadString();
			_itemObject = reader.ReadObject<IGameObject>();
			_age = reader.ReadInt();
		}
		
		public void AfterReadStateV1()
		{
		}
		#endregion

    }
}
