using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;

namespace FarmTycoon
{

    /// <summary>
    /// Contains all Info objects that have been read from the farmdata xml file.
    /// Info objects contains properties shared between different instance of the same building/crop/etc
    /// For instance all Barns reference an StorageBuilding info object with proeprties of the Barn building.
    /// </summary>
    public partial class FarmData
    {
        #region Member Vars

        /// <summary>
        /// Ids of info objects that are referenced by Id instead of name
        /// </summary>
        private InfoIds _infoIds = new InfoIds();

        /// <summary>
        /// All Info objects keyed by their unique name
        /// </summary>
        private Dictionary<string, IInfo> _infos = new Dictionary<string, IInfo>();

        /// <summary>
        /// All Info objects by type
        /// </summary>
        private Dictionary<Type, List<IInfo>> _infoSets = new Dictionary<Type, List<IInfo>>();

        /// <summary>
        /// Mapping between ItemTypes and EquipmentInfos
        /// </summary>
        private Dictionary<ItemTypeInfo, EquipmentInfo> _itemTypeInfoToEquipmentInfo = new Dictionary<ItemTypeInfo, EquipmentInfo>();

        /// <summary>
        /// Mapping between ItemTypes and AnimalInfos
        /// </summary>
        private Dictionary<ItemTypeInfo, AnimalInfo> _itemTypeInfoToAnimalInfo = new Dictionary<ItemTypeInfo, AnimalInfo>();

        #endregion

        #region Properties

        /// <summary>
        /// Ids of info objects that are referenced by Id instead of name
        /// </summary>
        public InfoIds InfoIds
        {
            get { return _infoIds; }
        }


        #region Single Info Properties

        /// <summary>
        /// Get the ScenarioInfo
        /// </summary>
        public ScenarioInfo ScenarioInfo
        {
            get { return (ScenarioInfo)GetInfo(ScenarioInfo.UNIQUE_NAME); }
        }
        
        /// <summary>
        /// Get the DeliveryAreaInfo
        /// </summary>
        public DeliveryAreaInfo DeliveryAreaInfo
        {
            get { return (DeliveryAreaInfo)GetInfo(DeliveryAreaInfo.UNIQUE_NAME); }
        }

        /// <summary>
        /// Get the PastureInfo
        /// </summary>
        public PastureInfo PastureInfo
        {
            get { return (PastureInfo)GetInfo(PastureInfo.UNIQUE_NAME); }
        }

        /// <summary>
        /// Get the LandInfo
        /// </summary>
        public LandInfo LandInfo
        {
            get { return (LandInfo)GetInfo(LandInfo.UNIQUE_NAME); }
        }
        

        /// <summary>
        /// Get the WorkerInfo
        /// </summary>
        public WorkerInfo WorkerInfo
        {
            get { return (WorkerInfo)GetInfo(WorkerInfo.UNIQUE_NAME); }
        }
        
        #endregion


        /// <summary>
        /// Get the Info object with the unique name passed
        /// </summary>
        public IInfo GetInfo(string uniqueName)
        {
            if (_infos.ContainsKey(uniqueName) == false) { return null; }
            return _infos[uniqueName];
        }


        /// <summary>
        /// Get all info objects of all types
        /// </summary>
        public List<IInfo> GetAllInfos()
        {            
            return new List<IInfo>(_infos.Values);
        }


        /// <summary>
        /// Get all info objects of type T
        /// </summary>
        public List<T> GetInfos<T>()
        {
            List<T> ret = new List<T>();
            foreach (IInfo info in _infoSets[typeof(T)])
            {
                ret.Add((T)info);
            }
            return ret;
        }


        /// <summary>
        /// Get crop info based on the name of the crop
        /// </summary>
        public CropInfo GetCropInfoForSeed(ItemTypeInfo seedType)
        {
            foreach (CropInfo cropInfo in _infoSets[typeof(CropInfo)])
            {
                if (cropInfo.Seed == seedType)
                {
                    return cropInfo;
                }
            }
            return null;
        }


        /// <summary>
        /// If there is equipment info associated with the item type info passed, then return the equipment info
        /// if not return null;
        /// </summary>
        public EquipmentInfo GetEquipmentInfoForItemInfo(ItemTypeInfo itemTypeInfo)
        {
            if (_itemTypeInfoToEquipmentInfo.ContainsKey(itemTypeInfo) == false) { return null; }
            return _itemTypeInfoToEquipmentInfo[itemTypeInfo];
        }


        /// <summary>
        /// If there is animal info associated with the item type info passed, then return the equipment info
        /// if not return null;
        /// </summary>
        public AnimalInfo GetAnimalInfoForItemInfo(ItemTypeInfo itemTypeInfo)
        {
            if (_itemTypeInfoToAnimalInfo.ContainsKey(itemTypeInfo) == false) { return null; }
            return _itemTypeInfoToAnimalInfo[itemTypeInfo];
        }


        /// <summary>
        /// get all EquipmentInfo for equipment of the type passed
        /// </summary>
        public List<EquipmentInfo> GetEquipmentOfType(EquipmentType equipmentType)
        {
            List<EquipmentInfo> ret = new List<EquipmentInfo>();
            foreach (EquipmentInfo equipmentInfo in _infoSets[typeof(EquipmentInfo)])
            {
                if (equipmentInfo.EquipmentType == equipmentType)
                {
                    ret.Add(equipmentInfo);
                }
            }
            return ret;
        }


        #endregion

        #region Logic

        /// <summary>
        /// Add the info object passed to the InfoObject dictionary, as well as any Info objects that are members of the Info object passed
        /// </summary>
        private void AddInfo(IInfo info)
        {
            //determine the type of info this is
            Type infoType = info.GetType();
            
            //it might already be in there because of the way we add things
            //if it is do nothing
            if (_infos.ContainsKey(info.UniqueName))
            {
                if (_infos[info.UniqueName] != info)
                {
                    throw new Exception("Two Info objects have the same name '" + info.UniqueName + "'");
                }
                else
                {
                    return;
                }
            }

            //add to dictionary of info
            _infos.Add(info.UniqueName, info);

            //add to dictionary of infosets
            if (_infoSets.ContainsKey(info.GetType()) == false)
            {
                _infoSets.Add(info.GetType(), new List<IInfo>());
            }
            _infoSets[info.GetType()].Add(info);

            //add to itemTypeInfo->EquipmentInfo mapping if it EquipmentInfo
            if (info is EquipmentInfo)
            {
                _itemTypeInfoToEquipmentInfo.Add((info as EquipmentInfo).ItemTypeInfo, (info as EquipmentInfo));
            }

            //add to itemTypeInfo->AnimalInfo mapping if it AnimalInfo
            if (info is AnimalInfo)
            {
                _itemTypeInfoToAnimalInfo.Add((info as AnimalInfo).AnimalType, (info as AnimalInfo));
            }

            //check if it has sub Info objects that need to be added
            if (info is ITraitsInfo)
            {
                foreach (IInfo subInfo in (info as ITraitsInfo).Traits.TraitInfoList)
                {
                    AddInfo(subInfo);
                }
            }
            if (info is IEventsInfo)
            {
                foreach (IInfo subInfo in (info as IEventsInfo).Events.Events)
                {
                    AddInfo(subInfo);
                }
            }
            if (info is ITexturesInfo)
            {
                foreach (IInfo subInfo in (info as ITexturesInfo).Textures.Textures)
                {
                    AddInfo(subInfo);
                }
                foreach (IInfo subInfo in (info as ITexturesInfo).Textures.TempTextures.Values)
                {
                    AddInfo(subInfo);
                }
            }
            
        }

        #endregion

        

    }
}
