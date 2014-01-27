using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{
    /// <summary>
    /// This describes the realtionship between an ItemTypeInfo object, and the one or more ItemTypes that use that Info object
    /// See ItemType class comment for better descirptions
    /// </summary>
    public enum ItemTypeRelation
    {
        One,  //There is one ItemType that uses this ItemTypeInfo (the type has no assoicated quality)
        Qualities, //There are ten ItemTypes that uses this ItemTypeInfo.  One each for quality levels 0-9.
        Many //There are many ItemTypes that uses this ItemTypeInfo.  One for each object this item represents.  For instance each cow has its own ItemType.
    }


    /// <summary>
    /// ItemTypeInfo contains info shared between realted ItemsTypes, for instance Wheat_1 and Wheat_2 share the same ItemBaseType.
    /// Most properties of the item are part of the ItemTypeInfo.
    /// </summary>
    public class ItemTypeInfo : IInfo
    {

        /// <summary>
        /// Prepended to the unique name of all ItemTypeInfos
        /// </summary>
        public const string UNIQUE_PREPEND = "Item_";

        /// <summary>
        /// The name for the ItemTypeInfo.  For intance "Wheat" is the ItemTypeInfo name of ItemType "Wheat_2"
        /// </summary>
        private string _name = "";
        
        /// <summary>
        /// icon for the item type.
        /// </summary>
        private string _icon = "";

        /// <summary>
        /// the size of the item type.  The amount of space it takes up in a building.
        /// </summary>
        private int _size = 1;

        /// <summary>
        /// Tags for the item.
        /// </summary>
        private HashSet<string> _tags = new HashSet<string>();

        /// <summary>
        /// relationship between an ItemTypeInfo and the ItemTypes using the info
        /// </summary>
        private ItemTypeRelation _itemTypeRelation = ItemTypeRelation.One;
        


        public ItemTypeInfo(XmlReader reader)
        {
            reader.ReadToFollowing("Item");
            if (reader.MoveToAttribute("Name"))
            {
                _name = reader.ReadContentAsString();
            }
            if (reader.MoveToAttribute("Icon"))
            {
                _icon = reader.ReadContentAsString();
            }
            if (reader.MoveToAttribute("Size"))
            {
                _size = reader.ReadContentAsInt();
            }
            if (reader.MoveToAttribute("Tags"))
            {
                string tags = reader.ReadContentAsString();
                foreach (string tag in tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    _tags.Add(tag.Trim());
                }
            }
            if (reader.MoveToAttribute("SubType"))
            {
                _itemTypeRelation = (ItemTypeRelation)Enum.Parse(typeof(ItemTypeRelation), reader.ReadContentAsString());
            }

            //everything is always has the tag "All"
            _tags.Add(SpecialTags.ALL_TAG);
        }


        /// <summary>
        /// The name for the ItemTypeInfo.  For intance "Wheat" is the ItemTypeInfo name of ItemType "Wheat_2"
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        
        /// <summary>
        /// Icon for the item.
        /// </summary>
        public string Icon
        {
            get { return _icon; }
        }

        /// <summary>
        /// The size of the item.  The amount of space it takes up in a building.
        /// </summary>
        public int Size
        {
            get { return _size; }
        }
        
        /// <summary>
        /// relationship between an ItemTypeInfo and the ItemTypes using the info
        /// </summary>
        public ItemTypeRelation ItemTypeRelation
        {
            get { return _itemTypeRelation; }
        }


        /// <summary>
        /// Tags for the item
        /// </summary>
        public HashSet<string> Tags
        {
            get { return _tags; }
        }

        /// <summary>
        /// Return if the ItemTypeInfo has the tag passed
        /// </summary>
        public bool HasTag(string tag)
        {
            return _tags.Contains(tag);
        }

        /// <summary>
        /// UniqueName for the IInfo object
        /// </summary>
        public string UniqueName
        {
            get { return UNIQUE_PREPEND + Name; }
        }
    }
}
