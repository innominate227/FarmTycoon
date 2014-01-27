using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public enum EquipmentType
    {
        Tractor,
        Plow,
        Sprayer,
        Planter,
        Harvester,
        Trailer
    }

    public class EquipmentTypeUtils
    {
        /// <summary>
        /// Return true if the equipment type passed is a Vehicle type, 
        /// return false if its a tow type
        /// </summary>
        public static bool IsVehicle(EquipmentType equipmentType)
        {
            return AllVehicleTypes.Contains(equipmentType);
        }

        /// <summary>
        /// A list of all equipmnet types that are vehicles
        /// </summary>
        public static List<EquipmentType> AllVehicleTypes = new List<EquipmentType>() { EquipmentType.Tractor, EquipmentType.Harvester };
    }

}
