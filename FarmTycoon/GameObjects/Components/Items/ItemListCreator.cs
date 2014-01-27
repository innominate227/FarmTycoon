using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// Utility class to create lists containing all items, or all items of some type
    /// </summary>
    public class ItemListCreator
    {
        /// <summary>
        /// cache for a full list of all items
        /// </summary>
        private static ItemList m_fullList;
        
        /// <summary>
        /// cache for a partial list of items
        /// </summary>
        private static Dictionary<string, ItemList> m_subList = new Dictionary<string,ItemList>();
        
        /// <summary>
        /// all the game item types
        /// </summary>
        private static Dictionary<string, GameItemType> m_itemTypes = new Dictionary<string, GameItemType>();



        /// <summary>
        /// return a full list of all items
        /// </summary>
        public static ItemList FullItemList()
        {
            //return the full list if its not null
            if (m_fullList != null)
            {
                return m_fullList;
            }

            //create list to return 
            m_fullList = new ItemList();

            //if the items types list has not been read yet load it
            if (m_itemTypes.Count == 0)
            {
                LoadItemTypes();
            }

            //add one of each type of item to the full list
            foreach(GameItemType itemType in m_itemTypes.Values)
            {
                m_fullList.AddItem(itemType);
            }

            return m_fullList;
        }

        /// <summary>
        /// return an item list that contains all the items for the class
        /// </summary>
        public static ItemList ItemListForClasses(string[] classes)
        {
            //make classes uppercase
            for (int i = 0; i < classes.Length; i++)
            {
                classes[i] = classes[i].ToUpper();
            }

            //create a string that concatinates all the classes
            List<string> classesList = new List<string>(classes);
            classesList.Sort();
            string classesString = "";
            foreach (string className in classesList)
            {
                classesString += ("," + className);
            }

            //return the item list if it was already created
            if (m_subList.ContainsKey(classesString))
            {
                return m_subList[classesString];
            }


            //we need to create the list

            //get the full item list
            ItemList fullList = FullItemList();

            //create a sub list
            ItemList subList = new ItemList();
            foreach (GameItemType itemType in fullList.ItemTypes)
            {
                if (classes.Contains(itemType.Class.ToUpper()))
                {
                    subList.AddItem(itemType);
                }
            }

            //add to the cache
            m_subList.Add(classesString, subList);

            //return the sub supply
            return subList;
        }


        public static List<GameItemType> AllTypes()
        {
            return FullItemList().ItemTypes;
        }

        /// <summary>
        /// Get an item type given its name
        /// </summary>
        public static GameItemType GetItemTypeByName(string name)
        {
            //if the items types list has not been read yet load it
            if (m_itemTypes.Count == 0)
            {
                LoadItemTypes();
            }

            //return the item with the name passed
            return m_itemTypes[name.ToUpper()];
        }


        /// <summary>
        /// Load in the list of item types
        /// </summary>
        private static void LoadItemTypes()
        {
            //if the items types list has not been read yet
            if (m_itemTypes.Count == 0)
            {
                //read in all the items from the data file
                DataFile itemsDataFile = Program.DataFiles.ItemsFile;
                foreach (string typeName in itemsDataFile.DataItems)
                {
                    string typeClass = itemsDataFile.GetParameterForItem(typeName, 0);
                    string subclass = itemsDataFile.GetParameterForItem(typeName, 1);
                    int size = int.Parse(itemsDataFile.GetParameterForItem(typeName, 2));
                    int cost = int.Parse(itemsDataFile.GetParameterForItem(typeName, 3));
                    string icon = itemsDataFile.GetParameterForItem(typeName, 4);
                    string descirption = itemsDataFile.GetParameterForItem(typeName, 5);

                    //create an item and add it to the dictionary
                    GameItemType itemType = new GameItemType(typeName, typeClass, subclass, size, cost, icon, descirption);
                    m_itemTypes.Add(typeName.ToUpper(), itemType);
                }
            }
        }
        

    }
}
