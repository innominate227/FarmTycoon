using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace FarmTycoon
{
    public class AnimalInfo : ITraitsInfo, IEventsInfo, ITexturesInfo, IInfo, IDelaysInfo
    {
        /// <summary>
        /// Prepended to the unique name of all AnimalInfos
        /// </summary>
        public const string UNIQUE_PREPEND = "Animal_";
        

        /// <summary>
        /// Item type for the animal
        /// </summary>
        private ItemTypeInfo _animalType = null;
                                             
        /// <summary>
        /// The traits that effect the quality of the animal
        /// </summary>
        private TraitInfoSet _traits;

        /// <summary>
        /// The events that effect what the animal does
        /// </summary>
        private ObjectEventInfoSet _events;

        /// <summary>
        /// The textures this animal can show
        /// </summary>
        private TexturesInfoSet _textures;

        /// <summary>
        /// Delays that determine how fast the animal walks, and does actions
        /// </summary>
        private DelayInfoSet _delays;


        public AnimalInfo(XmlReader reader, FarmData farmInfo)
        {
            reader.ReadToFollowing("Animal");            
            if (reader.MoveToAttribute("AnimalType"))
            {
                _animalType = reader.ReadContentAsItemTypeInfo(farmInfo);
            }

            _traits = new TraitInfoSet(this);
            _delays = new DelayInfoSet(this);
            _textures = new TexturesInfoSet(this);
            _events = new ObjectEventInfoSet(this);

            while (reader.ReadNextElement())
            {
                _traits.ReadElement(reader, farmInfo);
                _delays.ReadElement(reader, farmInfo);
                _textures.ReadElement(reader, farmInfo);
                _events.ReadElement(reader, farmInfo);
            }

            _traits.InitSet();
            _delays.InitSet();

        }
        
        /// <summary>
        /// Item type for the animal
        /// </summary>
        public ItemTypeInfo AnimalType
        {
            get { return _animalType; }
        }
                

        /// <summary>
        /// The traits that effect the quality of the animal
        /// </summary>
        public TraitInfoSet Traits
        {
            get { return _traits; }
        }

        
        /// <summary>
        /// The textures this worker can show
        /// </summary>
        public TexturesInfoSet Textures
        {
            get { return _textures; }
        }


        /// <summary>
        /// Delays that determine how fast the animal walks, and does actions
        /// </summary>
        public DelayInfoSet Delays
        {
            get { return _delays; }
        }


        /// <summary>
        /// The events that effect what the animal does
        /// </summary>
        public ObjectEventInfoSet Events
        {
            get { return _events; }
        }




        public string UniqueName
        {
            get { return UNIQUE_PREPEND + _animalType.Name; }
        }
    }
}
