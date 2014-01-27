using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FarmTycoon
{
    public static class XmlReaderExtenstions
    {

        public static int ReadContentAsTraitId(this XmlReader reader, FarmData farmInfo)
        {
            string traitName = reader.ReadContentAsString();            
            return farmInfo.InfoIds.GetTraitId(traitName);
        }

        public static ItemTypeInfo ReadContentAsItemTypeInfo(this XmlReader reader, FarmData farmInfo)
        {
            string itemName = reader.ReadContentAsString();
            string itemInfoUniqueName = ItemTypeInfo.UNIQUE_PREPEND + itemName;
            return (ItemTypeInfo)farmInfo.GetInfo(itemInfoUniqueName);
        }


        public static List<ItemTypeInfo> ReadContentAsItemTypeInfosContainingTag(this XmlReader reader, FarmData farmInfo)
        {
            //read tag
            string tag = reader.ReadContentAsString();

            //check all farm info objects for the tag
            List<ItemTypeInfo> infos = new List<ItemTypeInfo>();
            foreach (ItemTypeInfo itemTypeInfo in farmInfo.GetInfos<ItemTypeInfo>())
            {
                if (itemTypeInfo.HasTag(tag))
                {
                    infos.Add(itemTypeInfo);
                }
            }
            return infos;
        }
        
        
        public static Range ReadContentAsRange(this XmlReader reader)
        {
            //get the range text string
            string rangeText = reader.ReadContentAsString().Trim();

            //start and end are wide as possible until we find another value
            int start = int.MinValue;
            int end = int.MaxValue;

            //determine if we are inclusive or exclusive
            bool startInclusive = rangeText.StartsWith("[");
            bool endInclusive = rangeText.EndsWith("]");
            
            //there should be a comma, if not we will just have an infinate range
            if (rangeText.Contains(','))
            {
                string justRange = rangeText.Replace("[", "").Replace("(", "").Replace(")", "").Replace("]", "");
                string[] rangeComponents = justRange.Split(',');

                //parse start value if able
                int startVal;
                if (int.TryParse(rangeComponents[0], out startVal))
                {
                    start = startVal;
                }

                //parse end value if able
                int endVal;
                if (int.TryParse(rangeComponents[1], out endVal))
                {
                    end = endVal;
                }
            }

            //create the range object
            return new Range(start, startInclusive, end, endInclusive);
        }

        public static RelativeLocation ReadContentAsRelativeLocation(this XmlReader reader)
        {
            string relativeLocationText = reader.ReadContentAsString();
            return new RelativeLocation(relativeLocationText);
        }

        public static List<RelativeLocation> ReadContentAsRelativeLocationList(this XmlReader reader)
        {            
            string relativeLocationsText = reader.ReadContentAsString();
            return RelativeLocation.CreateRealativeLocationList(relativeLocationsText);
        }

        public static bool ReadNextElement (this XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
