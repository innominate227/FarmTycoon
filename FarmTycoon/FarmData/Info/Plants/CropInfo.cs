using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace FarmTycoon
{
    public class CropInfo : ITraitsInfo, IEventsInfo, ITexturesInfo, IInfo
    {
        /// <summary>
        /// Prepended to the unique name of all CropInfo
        /// </summary>
        public const string UNIQUE_PREPEND = "Crop_";
        
        /// <summary>
        /// ItemTypeInfo for the item gotten when the crop is harvested
        /// </summary>
        private ItemTypeInfo _harvestItem = null;

        /// <summary>
        /// ItemTypeInfo for the item gotten when the crop is picked 
        /// (null if the crop is not pickable)
        /// </summary>
        private ItemTypeInfo _pickItem = null;

        /// <summary>
        /// Type of item that is the seed for the crop
        /// </summary>
        private ItemTypeInfo _seed = null;

        /// <summary>
        /// When this crop is planted the plant delay is multiplied by this multiplier
        /// </summary>
        private double _plantDelayMultiplier = 1.0;

        /// <summary>
        /// When this crop is harvested the harvest delay is multiplied by this multiplier
        /// </summary>
        private double _harvestDelayMultiplier = 1.0;

        /// <summary>
        /// When this crop is picked the pick delay is multiplied by this multiplier
        /// </summary>
        private double _pickDelayMultiplier = 1.0;
        
        /// <summary>
        /// If true the crop needs space between it an other crops in the field
        /// </summary>
        private bool _needsSpace = false;

        /// <summary>
        /// Id of the trait checked to determine if the crop is pickable.
        /// the value of the trait must be within pickable range
        /// </summary>
        private int _pickableTraitId = -1;
        
        /// <summary>
        /// Range the pickable trait must be within in order for the crop to be considered pickable.
        /// It can be picked once each time it falls between this range, it must go out of the range, and back within in order to be picked again.
        /// </summary>
        private Range _pickableRange = new Range(0, false, 0, false);
        
        /// <summary>
        /// The traits that effect the quality of the crop
        /// </summary>
        private TraitInfoSet _traits;

        /// <summary>
        /// The events that effect what the crop does
        /// </summary>
        private ObjectEventInfoSet _events;

        /// <summary>
        /// The textures this crop can show
        /// </summary>
        private TexturesInfoSet _textures;


        public CropInfo(XmlReader reader, FarmData farmInfo)
        {
            reader.ReadToFollowing("Crop");            
            if (reader.MoveToAttribute("Seed"))
            {
                _seed = reader.ReadContentAsItemTypeInfo(farmInfo);
            }
            if (reader.MoveToAttribute("PlantDelayMultiplier"))
            {
                _plantDelayMultiplier = reader.ReadContentAsDouble();
            }
            if (reader.MoveToAttribute("HarvestDelayMultiplier"))
            {
                _harvestDelayMultiplier = reader.ReadContentAsDouble();
            }
            if (reader.MoveToAttribute("PickDelayMultiplier"))
            {
                _pickDelayMultiplier = reader.ReadContentAsDouble();
            }
            if (reader.MoveToAttribute("HarvestItem"))
            {
                _harvestItem = reader.ReadContentAsItemTypeInfo(farmInfo);
            }
            if (reader.MoveToAttribute("PickItem"))
            {
                _pickItem = reader.ReadContentAsItemTypeInfo(farmInfo);
            }
            if (reader.MoveToAttribute("NeedsSpace"))
            {
                _needsSpace = reader.ReadContentAsBoolean();
            }
            if (reader.MoveToAttribute("PickableTrait"))
            {
                _pickableTraitId = reader.ReadContentAsTraitId(farmInfo);
            }
            if (reader.MoveToAttribute("PickableRange"))
            {
                _pickableRange = reader.ReadContentAsRange();
            }

            _traits = new TraitInfoSet(this);
            _textures = new TexturesInfoSet(this);
            _events = new ObjectEventInfoSet(this);
            
            while (reader.ReadNextElement())
            {
                _traits.ReadElement(reader, farmInfo);
                _textures.ReadElement(reader, farmInfo);
                _events.ReadElement(reader, farmInfo);  
            }

            _traits.InitSet();
        }

        
        /// <summary>
        /// ItemTypeInfo for the item gotten when the crop is harvested
        /// </summary>
        public ItemTypeInfo HarvestItem
        {
            get { return _harvestItem; }
        }

        /// <summary>
        /// ItemTypeInfo for the item gotten when the crop is picked 
        /// (null if the crop is not pickable)
        /// </summary>
        public ItemTypeInfo PickItem
        {
            get { return _pickItem; }
        }

        /// <summary>
        /// Type of item that is the seed for the crop
        /// </summary>
        public ItemTypeInfo Seed
        {
            get { return _seed; }
        }


        /// <summary>
        /// When this crop is planted the plant delay is multiplied by this multiplier
        /// </summary>
        public double PlantDelayMultiplier
        {
            get { return _plantDelayMultiplier; }
        }

        /// <summary>
        /// When this crop is harvested the harvest delay is multiplied by this multiplier
        /// </summary>
        public double HarvestDelayMultiplier
        {
            get { return _harvestDelayMultiplier; }
        }

        /// <summary>
        /// When this crop is picked the pick delay is multiplied by this multiplier
        /// </summary>
        public double PickDelayMultiplier
        {
            get { return _pickDelayMultiplier; }
        }


        /// <summary>
        /// If true the crop needs space between it an other crops in the field
        /// </summary>
        public bool NeedsSpace
        {
            get { return _needsSpace; }
        }

        /// <summary>
        /// Id of the trait checked to determine if the crop is pickable.
        /// the value of the trait must be within pickable range
        /// </summary>
        public int PickableTraitId
        {
            get { return _pickableTraitId; }
        }

        /// <summary>
        /// Range the pickable trait must be within in order for the crop to be considered pickable.
        /// It can be picked once each time it falls between this range, it must go out of the range, and back within in order to be picked again.
        /// </summary>
        public Range PickableRange
        {
            get { return _pickableRange; }
        }


        /// <summary>
        /// All the traits that determine the crops quality
        /// </summary>
        public TraitInfoSet Traits
        {
            get { return _traits; }
        }

        /// <summary>
        /// The events that effect what the animal does
        /// </summary>
        public ObjectEventInfoSet Events
        {
            get { return _events; }
        }

        /// <summary>
        /// The textures this crop can show
        /// </summary>
        public TexturesInfoSet Textures
        {
            get { return _textures; }
        }

        public string UniqueName
        {
            get { return UNIQUE_PREPEND + _seed.Name; }
        }
    }
}
