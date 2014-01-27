using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace FarmTycoon
{
    /// <summary>
    /// Info on a delay
    /// </summary>
    public partial class DelayInfo : IInfo
    {
        /// <summary>
        /// Prepended to the unique name of all DelayInfo
        /// </summary>
        public const string UNIQUE_PREPEND = "Delay_";
        

        /// <summary>
        /// Name of the info object that has this delay + the name of the action the delay if for
        /// </summary>
        private string _fullName = "";

        /// <summary>
        /// Action that this info specifies the delay for
        /// </summary>
        private ActionOrEventType _action;

        /// <summary>
        /// minimum value for the delay
        /// </summary>
        private double _minimumValue = 0.1;
        
        /// <summary>
        /// maximum value for the dealy
        /// </summary>
        private double _maximumValue = 1.0;

        /// <summary>
        /// Id of the trait whos instantanious quality determines exactly the value of the delay.
        /// </summary>
        private int _traitId;

        /// <summary>
        /// If true when a second delay is applied to this delay, it overrides it instead of being multiplied by it.
        /// </summary>
        private bool _otherDelayFullyOverrides = false;

        /// <summary>
        /// Cache of delay values calcualted for qualities
        /// </summary>
        private Dictionary<int, double> _cache = new Dictionary<int, double>();



        public DelayInfo(string parentName, XmlReader reader, FarmData farmInfo)
        {
            reader.ReadToFollowing("Delay");
            if (reader.MoveToAttribute("Action"))
            {
                string actionName = reader.ReadContentAsString();
                _action = (ActionOrEventType)Enum.Parse(typeof(ActionOrEventType), actionName);
                _fullName = parentName + "_" + actionName;
            }
            if (reader.MoveToAttribute("Min"))
            {
                _minimumValue = reader.ReadContentAsDouble();
            }
            if (reader.MoveToAttribute("Max"))
            {
                _maximumValue = reader.ReadContentAsDouble();
            }
            if (reader.MoveToAttribute("Val"))
            {
                //for delay with a fixed value instead of being determined by a trait
                _minimumValue = reader.ReadContentAsDouble();
                _maximumValue = reader.ReadContentAsDouble();
            }
            if (reader.MoveToAttribute("Trait"))
            {
                _traitId = reader.ReadContentAsTraitId(farmInfo);
            }
            if (reader.MoveToAttribute("OverrideType"))
            {
                string delayType = reader.ReadContentAsString();
                _otherDelayFullyOverrides = (delayType.ToUpper() == "OVERRIDE");
            }
        }


        /// <summary>
        /// Get the delay given the quality of the trait
        /// </summary>
        public double GetDelay(int quality)
        {
            if (_cache.ContainsKey(quality) == false)
            {
                //difference between min and max delay (at most delay can increase by this much)
                double delayDiff = _maximumValue - _minimumValue;

                //qualiy factor if 0 for pefect quality to 1 for worse possible qaulity.
                double qualityFactor = (100.0 - quality) / 100.0;

                //calculate the delay, and add to cache
                double delay = _minimumValue + qualityFactor * delayDiff;
                _cache.Add(quality, delay);
            }

            return _cache[quality];
        }


        /// <summary>
        /// Action that this info specifies the delay for
        /// </summary>
        public ActionOrEventType Action
        {
            get { return _action; }
        }

        /// <summary>
        /// minimum value for the delay
        /// </summary>
        public double MinimumValue
        {
            get { return _minimumValue; }
        }

        /// <summary>
        /// maximum value for the dealy
        /// </summary>
        public double MaximumValue
        {
            get { return _maximumValue; }
        }

        /// <summary>
        /// Id of the trait whos instantanious quality determines exactly the value of the delay.
        /// </summary>
        public int TraitId
        {
            get { return _traitId; }
        }
        
        /// <summary>
        /// If true when a second delay is applied to this delay, it overrides it instead of being multiplied by it.
        /// </summary>
        public bool OtherDelaysFullyOverrides
        {
            get { return _otherDelayFullyOverrides; }
        }

        public string UniqueName
        {
            get { return UNIQUE_PREPEND + _fullName; }
        }
    }
}
