using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace FarmTycoon
{
    /// <summary>
    /// A set of delay info object quickly accesable by the action or event the delay is for
    /// </summary>
    public class DelayInfoSet
    {        
        /// <summary>
        /// The info object that owns this delay info set
        /// </summary>
        private IDelaysInfo _infoSetOwner;
        
        /// <summary>
        /// The delay info object that make up this set
        /// </summary>
        private List<DelayInfo> _delayInfoList; 

        /// <summary>
        /// The delay info objects index by their Action/Event type, null if there is no delay info for a particular type
        /// </summary>
        private DelayInfo[] _delayInfos; 
        

        /// <summary>
        /// Create a delay info set, ReadElement and InitSet need to be called before the set is used.
        /// </summary>        
        public DelayInfoSet(IDelaysInfo infoSetOwner)
        {
            _infoSetOwner = infoSetOwner;
            _delayInfoList = new List<DelayInfo>();
        }

        /// <summary>
        /// Read the element the xml reader is currently on, if the element is a delay add it to the delay info set
        /// </summary>        
        public void ReadElement(XmlReader reader, FarmData farmInfo)
        {
            if (reader.Name == "Delay")
            {
                DelayInfo delayInfo = new DelayInfo(_infoSetOwner.UniqueName, reader.ReadSubtree(), farmInfo);
                _delayInfoList.Add(delayInfo);
            }
        }

        /// <summary>
        /// Initlize the set after all DelayInfos have been read
        /// </summary>
        public void InitSet()
        {
            
            //create array to hold delay infos            
            int numberOfActionOrEventTypes = Enum.GetNames(typeof(ActionOrEventType)).Length;
            _delayInfos = new DelayInfo[numberOfActionOrEventTypes];
            
            //put the delay infos into the array
            foreach (DelayInfo delayInfo in _delayInfoList)
            {
                _delayInfos[(int)delayInfo.Action] = delayInfo;
            }            
        }


        /// <summary>
        /// The info object that owns this delay info set
        /// </summary>
        public IDelaysInfo InfoSetOwner
        {
            get { return _infoSetOwner; }
        }

        /// <summary>
        /// The delay info object in this set
        /// </summary>
        public List<DelayInfo> DelayInfoList
        {
            get { return _delayInfoList; }
        }

        /// <summary>
        /// Get a info for a delay given the action or event it is for        
        /// </summary>        
        public DelayInfo GetDelayInfo(ActionOrEventType action)
        {
            return _delayInfos[(int)action];
        }
                

    }
}
