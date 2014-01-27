using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace FarmTycoon
{

    /// <summary>
    /// Contains all Info objects that have been read from the farmdata xml file.
    /// Info objects contains properties shared between different instance of the same building/crop/etc
    /// For instance all Barns reference an StorageBuilding info object with proeprties of the Barn building.
    /// </summary>
    public partial class FarmData
    {
        #region Current
        /// <summary>
        /// Reference to the currently loaded farm data
        /// </summary>
        private static FarmData _current;

        /// <summary>
        /// Reference to the currently loaded farm data
        /// </summary>
        public static FarmData Current
        {
            get
            {
                return _current;
            }
            set
            {
                _current = value;
            }
        }
        #endregion

        #region Member Vars

        /// <summary>
        /// The xml string of the farm data
        /// </summary>        
        private string _farmDataXml;

        #endregion

        #region Setup

        /// <summary>
        /// Create new FarmData object
        /// </summary>
        public FarmData(string farmDataXml)
        {
            _farmDataXml = farmDataXml;
            ParseRawData();
        }
        
        #endregion
        
        #region Propeties
        
        /// <summary>
        /// The xml string of the farm data.
        /// Note setting this does not reaload the farm info objects, but setting it will cause the new farm data to be saved, and new farm info object will be created on the next load.
        /// </summary>        
        public string FarmDataXml
        {
            get { return _farmDataXml; }
            set { _farmDataXml = value; }
        }

        #endregion
        
        #region Logic

        ///// <summary>
        ///// Load farm data from the file passed
        ///// </summary>
        //public void LoadFarmDataFromFile(string fileName)
        //{
        //    StreamReader readFarmData = new StreamReader(fileName);
        //    _farmDataXml = readFarmData.ReadToEnd();
        //    readFarmData.Close();
        //    ParseRawData();            
        //}

        ///// <summary>
        ///// Re-loads only the scenario info text.  But does not reload info objects.
        ///// The updated text will be saved and new info object will be loaded next time the user saves and loads.
        ///// </summary>
        //public void LoadFarmDataFromFileDontReloadInfoObject(string fileName)
        //{
        //    StreamReader readFarmData = new StreamReader(fileName);
        //    _farmDataXml = readFarmData.ReadToEnd();
        //    readFarmData.Close();
        //}


        ///// <summary>
        ///// Save farm data to the file passed
        ///// </summary>
        //public void SaveFarmDataToFile(string fileName)
        //{
        //    StreamWriter writeFarmData = new StreamWriter(fileName);
        //    writeFarmData.Write(_farmDataXml);
        //    writeFarmData.Close();
        //}
        
        /// <summary>
        /// Parse the farmDataRaw text into farm data
        /// </summary>
        private void ParseRawData()
        {
            XmlReader reader = XmlReader.Create(new StringReader(_farmDataXml));                        
            while (reader.ReadNextElement())
            {
                if (reader.Name == "Scenario")
                {
                    ScenarioInfo scenarioInfo = new ScenarioInfo(reader.ReadSubtree());
                    AddInfo(scenarioInfo);
                }
                else if (reader.Name == "Item")
                {
                    ItemTypeInfo itemTypeInfo = new ItemTypeInfo(reader.ReadSubtree());
                    AddInfo(itemTypeInfo);
                }
                else if (reader.Name == "Land")
                {
                    LandInfo landInfo = new LandInfo(reader.ReadSubtree(), this);
                    AddInfo(landInfo);
                }
                else if (reader.Name == "Worker")
                {
                    WorkerInfo workerInfo = new WorkerInfo(reader.ReadSubtree(), this);
                    AddInfo(workerInfo);
                }
                else if (reader.Name == "Crop")
                {
                    CropInfo cropInfo = new CropInfo(reader.ReadSubtree(), this);
                    AddInfo(cropInfo);
                }
                else if (reader.Name == "Animal")
                {
                    AnimalInfo animalInfo = new AnimalInfo(reader.ReadSubtree(), this);
                    AddInfo(animalInfo);
                }
                else if (reader.Name == "Equipment")
                {
                    EquipmentInfo equipmentInfo = new EquipmentInfo(reader.ReadSubtree(), this);
                    AddInfo(equipmentInfo);
                }
                else if (reader.Name == "StorageBuilding")
                {
                    StorageBuildingInfo storageBuildingInfo = new StorageBuildingInfo(reader.ReadSubtree(), this);
                    AddInfo(storageBuildingInfo);
                }
                else if (reader.Name == "ProductionBuilding")
                {
                    ProductionBuildingInfo productionBuildingInfo = new ProductionBuildingInfo(reader.ReadSubtree(), this);
                    AddInfo(productionBuildingInfo);
                }
                else if (reader.Name == "Scenery")
                {
                    SceneryInfo sceneryInfo = new SceneryInfo(reader.ReadSubtree());
                    AddInfo(sceneryInfo);
                }
                else if (reader.Name == "DeliveryArea")
                {
                    DeliveryAreaInfo deliveryAreaInfo = new DeliveryAreaInfo(reader.ReadSubtree(), this);
                    AddInfo(deliveryAreaInfo);
                }
                else if (reader.Name == "Trough")
                {
                    TroughInfo troughInfo = new TroughInfo(reader.ReadSubtree(), this);
                    AddInfo(troughInfo);
                }
                else if (reader.Name == "BreakHouse")
                {
                    BreakHouseInfo breakHouseInfo = new BreakHouseInfo(reader.ReadSubtree(), this);
                    AddInfo(breakHouseInfo);
                }
            }

            //TODO: right now nothing is actually configurable about pasture info, but it seems strange for this to be sperate like it is
            AddInfo(new PastureInfo(this));

            //set the Ids of the special traits
            SpecialTraits.SetSpecialTraitIds(_infoIds);            

            //pre compute the effect for all traits
            foreach (TraitInfo traitInfo in _infoSets[typeof(TraitInfo)])
            {
                traitInfo.ComputeTraitValueEffects();
            }

        }




        #endregion

    }
}
