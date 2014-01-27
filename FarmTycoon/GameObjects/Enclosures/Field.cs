using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// A field contains several Crops.
    /// It keeps track of the composite quality of all the crops within the field.
    /// </summary>
    public class Field : Enclosure, IHasQuality, IHasActionLocation
    {
        #region Member Vars
                
        /// <summary>
        /// Composite quality averaging the quality of all planted objects in the planted area.
        /// null if the planted area is empty.
        /// </summary>
        private CompositeQuality _quality;

        /// <summary>
        /// Crops in the field
        /// </summary>
        private List<Crop> _crops;

        /// <summary>
        /// Are the crops in the list ordered to corespond with the land in the enclosure
        /// </summary>
        private bool _cropsAreOrdered;
        
        #endregion

        #region Setup Delete

        public Field() : base() 
        {
        }
                
        /// <summary>
        /// Create a field given the land that makes it up and the fences that border it
        /// </summary>
        public override void Setup(List<Land> allLand, List<Fence> borderFences)
        {
            _quality = new CompositeQuality();
            _crops = new List<Crop>();
            _cropsAreOrdered = false;
            base.Setup(allLand, borderFences);                
        }
        
        /// <summary>
        /// Called when the field is deleted
        /// </summary>
        protected override void DeleteInner()
        {
            base.DeleteInner();

            //delete any crops in the field
            foreach (Crop crop in _crops.ToArray())
            {
                crop.Delete();
            }
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// All crops in the field.
        /// Do not add to this list directly, use AddCrop to add a crop
        /// The crops are returned in such an order that they can be quickly walked between
        /// </summary>
        public List<Crop> Crops
        {
            get
            {
                if (_cropsAreOrdered == false) { OrderCrops(); }
                return _crops;
            }
        }
        
        /// <summary>
        /// The name of crop planted in the field, or "None" if no crops are planted
        /// </summary>
        public string TypePlanted
        {
            get
            {
                if (_crops.Count == 0) { return "None"; }
                return _crops[0].CropInfo.Seed.Name.Replace("Seed","").Trim();
            }
        }

        /// <summary>
        /// Info for the crop in the field or null if none are planted
        /// </summary>
        public CropInfo CropInfo
        {
            get
            {
                if (_crops.Count == 0) { return null; }
                return _crops[0].CropInfo;
            }
        }

        /// <summary>
        /// Average Quality of all crops in the field
        /// </summary>
        public IQuality Quality
        {
            get { return _quality; }
        }

        #endregion

        #region Logic
                                
        /// <summary>
        /// Add a crop to the field
        /// </summary>
        public void AddCrop(Crop crop)
        {
            _quality.AddQuality((Quality)crop.Quality);
            _crops.Add(crop);
            _cropsAreOrdered = false;
        }

        /// <summary>
        /// Remove a crop from the planted area
        /// </summary>
        public void RemoveCrop(Crop crop)
        {
            _quality.RemoveQuality((Quality)crop.Quality);
            _crops.Remove(crop);            
        }

        /// <summary>
        /// Order the crops to be in the same order as the land in the enclosure
        /// </summary>
        private void OrderCrops()
        {
            List<Crop> newCropsList = new List<Crop>();
            foreach (Land land in base.OrderedLand)
            {
                Crop plantedOnLand = land.LocationOn.Find<Crop>();
                if (plantedOnLand != null)
                {
                    Debug.Assert(_crops.Contains(plantedOnLand));
                    newCropsList.Add(plantedOnLand);
                }
            }

            Debug.Assert(newCropsList.Count == _crops.Count);
            _crops = newCropsList;
            _cropsAreOrdered = true;
        }

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
			writer.WriteObject(_quality);
			writer.WriteObjectList<Crop>(_crops);
			writer.WriteBool(_cropsAreOrdered);			
		}

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
			_quality = reader.ReadObject<CompositeQuality>();
			_crops = reader.ReadObjectList<Crop>();
			_cropsAreOrdered = reader.ReadBool();			
		}

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion
        
    }
}

