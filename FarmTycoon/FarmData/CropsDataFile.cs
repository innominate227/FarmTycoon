using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class CropsDataFile : DataFile
    {
        /// <summary>
        /// mapping from seeds to the type of crop it grows
        /// </summary>
        private Dictionary<ItemType, string> m_seeds = new Dictionary<ItemType, string>();

        /// <summary>
        /// mapping from crop to the time to grow it
        /// </summary>
        private Dictionary<string, double> m_growTimes = new Dictionary<string, double>();

        /// <summary>
        /// All the trait info for each crop
        /// </summary>
        private Dictionary<string, Dictionary<TraitName, TraitInfo>> m_allTraitInfo = new Dictionary<string, Dictionary<TraitName, TraitInfo>>();



        public CropsDataFile(string dataFileText)
            : base(dataFileText)
        {
        }

        public override void ParseFile()
        {
            m_seeds.Clear();
            m_growTimes.Clear();
            m_allTraitInfo.Clear();

            DataFileReader dataFile = new DataFileReader(m_dataFileText);


            foreach (string crop in dataFile.DataItems)
            {
                m_allTraitInfo.Add(crop, new Dictionary<TraitName, TraitInfo>());

                string seedName = dataFile.GetParameterForItem(crop, 0);
                m_seeds.Add(Program.Game.DataFiles.ItemsFile.GetItemTypeByName(seedName), crop);

                double growTime = double.Parse(dataFile.GetParameterForItem(crop, 1));
                m_growTimes.Add(crop, growTime);

                double waterOptimal = double.Parse(dataFile.GetParameterForItem(crop, 2));  
                double waterStart = double.Parse(dataFile.GetParameterForItem(crop, 3));                
                double waterUseSpeed = double.Parse(dataFile.GetParameterForItem(crop, 4));
                string waterItems = dataFile.GetParameterForItem(crop, 5);
                string waterRanges = dataFile.GetParameterForItem(crop, 6);
                TraitInfo waterTraitInfo = new TraitInfo(crop, TraitName.Water, waterOptimal, waterStart, waterUseSpeed, waterItems, waterRanges);
                m_allTraitInfo[crop].Add(TraitName.Water, waterTraitInfo);

                double fertilizerOptimal = double.Parse(dataFile.GetParameterForItem(crop, 7));  
                double fertilizerStart = double.Parse(dataFile.GetParameterForItem(crop, 8));                
                double fertilizerUseSpeed = double.Parse(dataFile.GetParameterForItem(crop, 9));
                string fertilizerItems = dataFile.GetParameterForItem(crop, 10);
                string fertilizerRanges = dataFile.GetParameterForItem(crop, 11);
                TraitInfo fertilizerTraitInfo = new TraitInfo(crop, TraitName.Fertilizer, fertilizerOptimal, fertilizerStart, fertilizerUseSpeed, fertilizerItems, fertilizerRanges);
                m_allTraitInfo[crop].Add(TraitName.Fertilizer, fertilizerTraitInfo);
                
                double sunlightOptimal = double.Parse(dataFile.GetParameterForItem(crop, 12));
                double sunlightStart = double.Parse(dataFile.GetParameterForItem(crop, 13));
                double sunlightUseSpeed = double.Parse(dataFile.GetParameterForItem(crop, 14));
                string sunlightItems = dataFile.GetParameterForItem(crop, 15);
                string sunlightRanges = dataFile.GetParameterForItem(crop, 16);
                TraitInfo sunlightTraitInfo = new TraitInfo(crop, TraitName.Sunlight, sunlightOptimal, sunlightStart, sunlightUseSpeed, sunlightItems, sunlightRanges);
                m_allTraitInfo[crop].Add(TraitName.Sunlight, sunlightTraitInfo);
                
                double landSlopeOptimal = double.Parse(dataFile.GetParameterForItem(crop, 17));
                double landSlopeStart = double.Parse(dataFile.GetParameterForItem(crop, 18));
                double landSlopeUseSpeed = double.Parse(dataFile.GetParameterForItem(crop, 19));
                string landSlopeItems = dataFile.GetParameterForItem(crop, 20);
                string landSlopeRanges = dataFile.GetParameterForItem(crop, 21);
                TraitInfo landSlopeTraitInfo = new TraitInfo(crop, TraitName.Slope, landSlopeOptimal, landSlopeStart, landSlopeUseSpeed, landSlopeItems, landSlopeRanges);
                m_allTraitInfo[crop].Add(TraitName.Slope, landSlopeTraitInfo);
                
                double soilOptimal = double.Parse(dataFile.GetParameterForItem(crop, 22));
                double soilStart = double.Parse(dataFile.GetParameterForItem(crop, 23));
                double soilUseSpeed = double.Parse(dataFile.GetParameterForItem(crop, 24));
                string soilItems = dataFile.GetParameterForItem(crop, 25);
                string soilRanges = dataFile.GetParameterForItem(crop, 26);
                TraitInfo soilTraitInfo = new TraitInfo(crop, TraitName.Soil, soilOptimal, soilStart, soilUseSpeed, soilItems, soilRanges);
                m_allTraitInfo[crop].Add(TraitName.Soil, soilTraitInfo);
            }

        }


        public string GetCropType(ItemType seed)
        {
            return m_seeds[seed];
        }

        public double GetGrowTime(string crop)
        {
            return m_growTimes[crop];
        }


        public TraitInfo GetTraitInfo(string crop, TraitName traitName)
        {
            return m_allTraitInfo[crop][traitName];
        }

    }
}
