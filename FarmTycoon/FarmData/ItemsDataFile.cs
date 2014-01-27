using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{

    public class ItemsDataFile : DataFile
    {
        /// <summary>
        /// Full list of all items
        /// </summary>
        private ItemList m_fullList;
        
        /// <summary>
        /// all the game item types, in a dictionary keyed by their name
        /// </summary>
        private Dictionary<string, ItemType> m_itemTypes = new Dictionary<string, ItemType>();

        /// <summary>
        /// all the game item types, grouped by their class
        /// </summary>
        private Dictionary<ItemClass, List<ItemType>> m_itemTypesByClass = new Dictionary<ItemClass, List<ItemType>>();

        /// <summary>
        /// all the game item types, grouped by their subclass
        /// </summary>
        private Dictionary<string, List<ItemType>> m_itemTypesBySubclass = new Dictionary<string, List<ItemType>>();



        /// <summary>
        /// Read the Items Data File into memory
        /// </summary>
        public ItemsDataFile(string dataFileText)
            : base(dataFileText)
        {
        }


        public override void ParseFile()
        {
            m_itemTypes.Clear();
            m_itemTypesByClass.Clear();
            m_itemTypesBySubclass.Clear();

            DataFileReader dataFile = new DataFileReader(m_dataFileText);

            //read the items types into a dictionaty
            foreach (string typeName in dataFile.DataItems)
            {
                ItemClass itemClass = (ItemClass)Enum.Parse(typeof(ItemClass), dataFile.GetParameterForItem(typeName, 0));
                string subclass = dataFile.GetParameterForItem(typeName, 1);
                int size = int.Parse(dataFile.GetParameterForItem(typeName, 2));
                string icon = dataFile.GetParameterForItem(typeName, 3);                
                string descirption = dataFile.GetParameterForItem(typeName, 4);

                //create an item and add it to the dictionary of all item types
                ItemType itemType = new ItemType(typeName, itemClass, subclass, size, icon, descirption);
                m_itemTypes.Add(typeName.ToUpper(), itemType);

                //add to items class list
                if (m_itemTypesByClass.ContainsKey(itemClass) == false)
                {
                    m_itemTypesByClass.Add(itemClass, new List<ItemType>());
                }
                m_itemTypesByClass[itemClass].Add(itemType);

                //add to items subclass list
                if (m_itemTypesBySubclass.ContainsKey(subclass.ToUpper()) == false)
                {
                    m_itemTypesBySubclass.Add(subclass.ToUpper(), new List<ItemType>());
                }
                m_itemTypesBySubclass[subclass.ToUpper()].Add(itemType);
            }

            //create the full list
            m_fullList = new ItemList();            
            foreach (ItemType itemType in m_itemTypes.Values)
            {
                m_fullList.AddItem(itemType);
            }
        }


        /// <summary>
        /// Get an item type given its name
        /// </summary>
        public ItemType GetItemTypeByName(string name)
        {
            //return the item with the name passed
            return m_itemTypes[name.ToUpper()];
        }

        /// <summary>
        /// Get a list of item types given their class name
        /// </summary>
        public IList<ItemType> GetItemTypesByClass(ItemClass className)
        {
            //return the items with the name passed
            return m_itemTypesByClass[className].AsReadOnly();
        }

        /// <summary>
        /// Get a list of item types given their subclass name
        /// </summary>
        public IList<ItemType> GetItemTypesBySubclass(string subclassName)
        {
            //return the items with the name passed
            return m_itemTypesBySubclass[subclassName.ToUpper()].AsReadOnly();
        }

        /// <summary>
        /// return a full list of all items
        /// </summary>
        public ItemList FullItemList
        {
            get { return m_fullList; }
        }

        /// <summary>
        /// return an item list that contains 1 of each items in the classes passed
        /// </summary>
        public ItemList ItemListForClass(ItemClass itemClass)
        {                        
            //create a list of all item in the class
            ItemList list = new ItemList();
            foreach (ItemType itemTypeInClass in m_itemTypesByClass[itemClass])
            {
                list.AddItem(itemTypeInClass);
            }
                        
            return list;
        }

        

    }
}
