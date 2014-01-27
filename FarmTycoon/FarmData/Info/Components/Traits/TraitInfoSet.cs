using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace FarmTycoon
{
    /// <summary>
    /// A set of trait info object quickly accesable by their id
    /// </summary>
    public class TraitInfoSet
    {
        /// <summary>
        /// The info object that owns this trait info set
        /// </summary>
        private ITraitsInfo _infoSetOwner;
        
        /// <summary>
        /// The the info objects that make up this set of info objects
        /// </summary>
        private List<TraitInfo> _traitInfoList; 

        /// <summary>
        /// The info objects index by their trait id minus the min id of any trait in the set
        /// </summary>
        private TraitInfo[] _traitInfos; 
        
        /// <summary>
        /// The min id of any trait in the set
        /// </summary>
        private int _minTraitId;

        /// <summary>
        /// The max id of any trait in the set
        /// </summary>
        private int _maxTraitId;
        
        /// <summary>
        /// The trait ids for each trait in the set
        /// </summary>        
        private int[] _traitIds;

                
        /// <summary>
        /// Create a trait info set, ReadElement and InitSet need to be called before the set is used.
        /// </summary>        
        public TraitInfoSet(ITraitsInfo infoSetOwner)
        {
            _infoSetOwner = infoSetOwner;
            _traitInfoList = new List<TraitInfo>();
        }

        /// <summary>
        /// Read the element the xml reader is currently on, if the element is a trait add it to the trait info set
        /// </summary>        
        public void ReadElement(XmlReader reader, FarmData farmInfo)
        {
            if (reader.Name == "Trait")
            {
                TraitInfo trait = new TraitInfo(_infoSetOwner, reader.ReadSubtree(), farmInfo);
                _traitInfoList.Add(trait);
            }
        }
        
        /// <summary>
        /// Initlize the set after all DelayInfos have been read
        /// </summary>
        public void InitSet()
        {
            //determine the min and max id of all the traits
            _maxTraitId = -1;
            _minTraitId = int.MaxValue;
            foreach (TraitInfo traitInfo in _traitInfoList)
            {
                _minTraitId = Math.Min(_minTraitId, traitInfo.Id);
                _maxTraitId = Math.Max(_maxTraitId, traitInfo.Id);
            }

            //create array to hold trait infos
            if (_maxTraitId > 0)
            {
                _traitInfos = new TraitInfo[_maxTraitId + 1 - _minTraitId];
            }

            //put the trait infos into the array
            foreach (TraitInfo traitInfo in _traitInfoList)
            {
                _traitInfos[traitInfo.Id - _minTraitId] = traitInfo;                
            }
            
            //initilize the array of all valid trait ids
            _traitIds = new int[_traitInfoList.Count];
            int index = 0;
            foreach (TraitInfo traitInfo in _traitInfoList)
            {
                _traitIds[index] = traitInfo.Id;
                index++;
            }

        }


        /// <summary>
        /// The info object that owns this trait info set
        /// </summary>
        public ITraitsInfo InfoSetOwner
        {
            get { return _infoSetOwner; }
        }

        /// <summary>
        /// The trait info object in this set
        /// </summary>
        public List<TraitInfo> TraitInfoList
        {
            get { return _traitInfoList; }
        }

        /// <summary>
        /// Get a trait info given the id of the trait.
        /// The id passed is not checked for validity.
        /// </summary>        
        public TraitInfo GetTraitInfo(int traitInfoId)
        {
            return _traitInfos[traitInfoId - _minTraitId];
        }



        /// <summary>
        /// The min id of any trait in the set
        /// </summary>
        public int MinTraitId
        {
            get { return _minTraitId; }
        }

        /// <summary>
        /// The max id of any trait in the set
        /// </summary>
        public int MaxTraitId
        {
            get { return _maxTraitId; }
        }

        /// <summary>
        /// The trait ids for each trait in the set
        /// </summary>        
        public int[] TraitIds
        {
            get { return _traitIds; }
        }



    }
}
