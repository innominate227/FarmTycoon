using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    public class EquipmentDataFile : DataFile
    {
        /// <summary>
        /// info for each peice of equipment
        /// </summary>
        private Dictionary<ItemType, EquipmentType> m_equipment = new Dictionary<ItemType, EquipmentType>();

        /// <summary>
        /// Represent when the worker does not actuall have a vehicle, and its just the worker itslef
        /// </summary>
        private static EquipmentType m_nullVehicle = new EquipmentType(null, true, "man1", 1.0, 1.0, 1.0);

        /// <summary>
        /// Represent when the worker does not actuall have a tow, and its just the worker itslef
        /// </summary>
        private static EquipmentType m_nullTow = new EquipmentType(null, false, "empty", 1.0, 1.0, 1.0);


        public EquipmentDataFile(string dataFileText)
            : base(dataFileText)
        {
        }

        public override void ParseFile()
        {
            m_equipment.Clear();


            DataFileReader dataFile = new DataFileReader(m_dataFileText);
                        
            foreach (string equipment in dataFile.DataItems)
            {
                ItemType equipmentType = Program.Game.DataFiles.ItemsFile.GetItemTypeByName(equipment);
                Debug.Assert(equipmentType.Class == ItemClass.Equipment);

                bool isVehicle = dataFile.GetParameterForItem(equipment, 0).ToUpper() == "VEHICLE";
                string texture = dataFile.GetParameterForItem(equipment, 1);
                double moveSpeedMultipler = double.Parse(dataFile.GetParameterForItem(equipment, 2));
                double actionSpeedMultipler = double.Parse(dataFile.GetParameterForItem(equipment, 3));
                double inventorySizeMultipler = double.Parse(dataFile.GetParameterForItem(equipment, 4));

                m_equipment.Add(equipmentType, new EquipmentType(equipmentType, isVehicle, texture, moveSpeedMultipler, actionSpeedMultipler, inventorySizeMultipler));
            }
        }


        /// <summary>
        /// Represent when the worker does not actuall have a vehicle, and its just the worker itslef
        /// </summary>
        public static EquipmentType NullVehicle
        {
            get { return m_nullVehicle; }
        }


        /// <summary>
        /// Represent when the worker does not actuall have a tow, and its just the worker itslef
        /// </summary>
        public static EquipmentType NullTow
        {
            get { return m_nullTow; }
        }

        /// <summary>
        /// Get equipment type given the item type for that equipmnet
        /// </summary>
        public EquipmentType GetEquipmentType(ItemType equipmentType)
        {
            Debug.Assert(equipmentType.Class == ItemClass.Equipment);

            return m_equipment[equipmentType];
        }

        /// <summary>
        /// Get equipment type given the item type for that equipmnet
        /// </summary>
        public EquipmentType GetEquipmentType(string equipmentType)
        {
            return GetEquipmentType(Program.Game.DataFiles.ItemsFile.GetItemTypeByName(equipmentType));
        }

    }
}
