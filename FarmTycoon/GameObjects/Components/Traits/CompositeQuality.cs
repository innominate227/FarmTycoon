using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// A Composite Quality groups together several quality objects and shows average values for all of them
    /// </summary>
    public class CompositeQuality : ISavable, IQuality
    {
        #region Member Vars
     
        /// <summary>
        /// All the quality that make up the composite quality
        /// </summary>
        private List<Quality> _qualitiesInComposite = new List<Quality>();

        #endregion

        #region Setup

        /// <summary>
        /// Create a CompositeQuality.
        /// </summary>
        public CompositeQuality()
        {
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// The current average quality of the group of objects
        /// </summary>
        public int CurrentQuality
        {
            get
            {
                if (_qualitiesInComposite.Count == 0) { return 0; }
                int qualitySum = 0;
                foreach (Quality quality in _qualitiesInComposite)
                {
                    qualitySum += quality.CurrentQuality;
                }
                return (int)Math.Round((double)qualitySum / _qualitiesInComposite.Count);
            }
        }

        /// <summary>
        /// The value of a trait with the traitId passed that determines the quality
        /// </summary>        
        public int GetTraitValue(int traitId)
        {            
            if (_qualitiesInComposite.Count == 0) { return 0; }
            int traitSum = 0;
            foreach (Quality quality in _qualitiesInComposite)
            {
                traitSum += quality.GetTraitValue(traitId);
            }
            return (int)Math.Round((double)traitSum / _qualitiesInComposite.Count);
        }


        /// <summary>
        /// Sets the value of all traits in the composite.  This is only used in scenario edit mode, as normally each trait value is modified individualy.
        /// </summary>        
        public void SetTraitValue(int traitId, int value)
        {   
            foreach (Quality quality in _qualitiesInComposite)
            {
                quality.SetTraitValue(traitId, value);
            }
        }

        
        /// <summary>
        /// The value of a trait with the traitId passed that determines the quality
        /// </summary>        
        public int[] TraitIds
        {
            get
            {
                if (_qualitiesInComposite.Count == 0) { return new int[0]; }
                return _qualitiesInComposite[0].TraitIds; 
            }
        }
        
        /// <summary>
        /// The value of a trait with the traitId passed that determines the quality
        /// </summary>        
        public TraitInfo GetTraitInfo(int traitId)
        {
            if (_qualitiesInComposite.Count == 0) { return null; }
            return _qualitiesInComposite[0].GetTraitInfo(traitId); 
        }



        #endregion

        #region Logic

        /// <summary>
        /// Add a quality object to the quality composite
        /// </summary>
        public void AddQuality(Quality quality)
        {            
            //add to list of all qualities
            _qualitiesInComposite.Add(quality);
        }


        /// <summary>
        /// Remove a quality object from the quality composite
        /// </summary>
        public void RemoveQuality(Quality quality)
        {
            _qualitiesInComposite.Remove(quality);
        }
        
        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteObjectList<Quality>(_qualitiesInComposite);
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _qualitiesInComposite = reader.ReadObjectList<Quality>();    
        }

        public void AfterReadStateV1()
        {
        }
        #endregion


    }
}
