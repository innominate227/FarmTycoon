using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{
    /// <summary>
    /// Info object for a game object.  All game objects of a similar type share an InfoObject.
    /// For example all barns will share a StorageBuilding info object with the properties shared by all barnes.
    /// The Info objects are created by parsing the FarmData.xml file.
    /// </summary>
    public class GameObjectInfo : IInfo
    {
        /// <summary>
        /// Name of the info object
        /// </summary>
        protected string m_name = "";

        
        /// <summary>
        /// Name of the info object
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }        
    }
}
