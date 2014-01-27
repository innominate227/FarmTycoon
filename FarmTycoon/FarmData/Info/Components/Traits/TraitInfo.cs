using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace FarmTycoon
{
    /// <summary>
    /// Info on a trait
    /// </summary>
    public partial class TraitInfo : IInfo
    {
        /// <summary>
        /// Prepended to the unique name of all TraitInfo
        /// </summary>
        public const string UNIQUE_PREPEND = "Trait_";


        /// <summary>
        /// The info object with traits that is a parent to this trait info object
        /// </summary>
        private ITraitsInfo _parent;

        /// <summary>
        /// Name of the info object that has this trait + the name of the trait (this should be unique to the trait where as name would not be)
        /// </summary>
        private string _fullName = "";

        /// <summary>
        /// Id of the trait.  Two traits with the same name will have the same id as well, it is just faster to compare ids (and index on ids) then it is strings.        
        /// </summary>
        private int _id;

        /// <summary>
        /// Name of the trait
        /// </summary>
        private string _name = "";
                
        /// <summary>
        /// Start value of the trait
        /// </summary>
        private int _startValue = 50;

        /// <summary>
        /// minimum possible value of the trait
        /// </summary>
        private int _minimumValue = 0;

        /// <summary>
        /// maximum possible value of the trait
        /// </summary>
        private int _maximumValue = 100;

        /// <summary>
        /// True if the trait should be be shown in the gui
        /// </summary>
        private bool _hidden = false;

        /// <summary>
        /// How the traits instantaneous quality is calculated, or null if the trait does not have instantaneous quality
        /// </summary>
        private TraitQualityInfo _instantaneousQuality = null;

        /// <summary>
        /// How the traits running quality is calculated, or null if the trait does not have running quality
        /// </summary>
        private TraitQualityInfo _runningQuality = null;



        public TraitInfo(ITraitsInfo parent, XmlReader reader, FarmData farmInfo)
        {
            _parent = parent;
            reader.ReadToFollowing("Trait");
            if (reader.MoveToAttribute("Name"))
            {
                _name = reader.ReadContentAsString();
                _fullName = _parent.UniqueName + "_" + _name;
                _id = farmInfo.InfoIds.GetTraitId(_name);
            }
            if (reader.MoveToAttribute("Start"))
            {
                _startValue = reader.ReadContentAsInt();
            }
            if (reader.MoveToAttribute("Min"))
            {
                _minimumValue = reader.ReadContentAsInt();
                if (_minimumValue < 0) { _minimumValue = 0; }
            }
            if (reader.MoveToAttribute("Max"))
            {
                _maximumValue = reader.ReadContentAsInt();
                if (_maximumValue > 10000) { _maximumValue = 10000; }
            }
            if (reader.MoveToAttribute("Hidden"))
            {
                _hidden = reader.ReadContentAsBoolean();
            }
                                    
            while (reader.ReadNextElement())
            {
                if (reader.Name == "EffectingItem")
                {
                    ReadEffectingItem(reader, farmInfo);
                }
                else if (reader.Name == "EffectingRange")
                {
                    ReadEffectingRange(reader, farmInfo);
                }
                else if (reader.Name == "EffectingEvent" ||
                         reader.Name == "EffectingAction")
                {
                    ReadEffectingEvent(reader);
                }
                else if (reader.Name == "InstantaneousQuality")
                {
                    ReadQuality(reader, true);
                }
                else if (reader.Name == "RunningQuality")
                {
                    ReadQuality(reader, false);
                }
            }
        }

        private void ReadQuality(XmlReader reader, bool instantaneous)
        {
            //parse the quality values
            int optimal = 0;
            int weight = 0;
            int maxAbove = (_maximumValue - _minimumValue) * 4; //by default allow being above by signifacntly more than the entire range, so going above never lowers quality.  (dont want Int.MaxValue b/c of issue rolling over the top)
            int maxBelow = (_maximumValue - _minimumValue) * 4;
            double leniencyAbove = 1000; //by default leincy is super high which essentally means we stay at quality 100 until we hit max above
            double leniencyBelow = 1000;            
            if (reader.MoveToAttribute("Optimal"))
            {
                optimal = reader.ReadContentAsInt();
            }
            if (reader.MoveToAttribute("Weight"))
            {
                weight = reader.ReadContentAsInt();
            }
            if (reader.MoveToAttribute("MaxAbove"))
            {
                maxAbove = reader.ReadContentAsInt();
            }
            if (reader.MoveToAttribute("MaxBelow"))
            {
                maxBelow = reader.ReadContentAsInt();
            }
            if (reader.MoveToAttribute("LeniencyAbove"))
            {
                leniencyAbove = reader.ReadContentAsDouble();
            }
            if (reader.MoveToAttribute("LeniencyBelow"))
            {
                leniencyBelow = reader.ReadContentAsDouble();
            }

            //create the quality and assign it to the proper quality reference
            TraitQualityInfo traitQualityInfo = new TraitQualityInfo(optimal, weight, maxBelow, maxAbove, leniencyBelow, leniencyAbove);
            if (instantaneous)
            {
                _instantaneousQuality = traitQualityInfo;
            }
            else
            {
                _runningQuality = traitQualityInfo;
            }
        }


        /// <summary>
        /// Id of the trait.  Two traits with the same name will have the same id.
        /// </summary>
        public int Id
        {
            get { return _id; }
        }
                        
        /// <summary>
        /// Name of the trait
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
               

        /// <summary>
        /// Start value of the trait
        /// </summary>
        public int StartValue
        {
            get { return _startValue; }
        }


        /// <summary>
        /// minimum possible value of the trait
        /// </summary>
        public int MinimumValue
        {
            get { return _minimumValue; }
        }


        /// <summary>
        /// maximum possible value of the trait
        /// </summary>
        public int MaximumValue
        {
            get { return _maximumValue; }
        }


        /// <summary>
        /// True if the trait should be be shown in the gui
        /// </summary>
        public bool Hidden
        {
            get { return _hidden; }
        }


        /// <summary>
        /// How the traits instantaneous quality is calculated, or null if the trait does not have instantaneous quality
        /// </summary>
        public TraitQualityInfo InstantaneousQualityInfo
        {
            get { return _instantaneousQuality; }
        }

        /// <summary>
        /// How the traits running quality is calculated, or null if the trait does not have running quality
        /// </summary>
        public TraitQualityInfo RunningQualityInfo
        {
            get { return _runningQuality; }
        }


        public string UniqueName
        {
            get { return UNIQUE_PREPEND + _fullName; }
        }
    }
}
