using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Script event to add to a buildings inventory
    /// </summary>
    public class AddToBuildingInventoryEvent : ScriptEvent
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "ADDTOBUILDINGINVENTORY";

        
        /// <summary>
        /// Name of the building to add to
        /// </summary>
        private ScriptString m_buildingName;

        /// <summary>
        /// What item should be adjusted
        /// </summary>
        private ScriptString m_itemToAdd;
        
        /// <summary>
        /// The amount to increase or decrease the item.
        /// </summary>
        private ScriptNumber m_amount;
        
        
        
        /// <summary>
        /// Create a AddToBuildingInventoryEvent event
        /// </summary>
        public AddToBuildingInventoryEvent(string[] actionParams)
        {
            Debug.Assert(actionParams.Length == 3);

            m_buildingName = new ScriptString(actionParams[0]);
            m_itemToAdd = new ScriptString(actionParams[1]);
            m_amount = new ScriptNumber(actionParams[2]);
        }


        public override void  DoEvent()
        {
            //find the building to add to
            string buildingName = m_buildingName.GetValue();
            IStorageBuilding addTo = null;
            foreach (SolidObject building in Program.Game.MasterObjectList.FindAll<SolidObject>())
            {
                if (building.Name == buildingName && building is IStorageBuilding)
                {
                    addTo = (IStorageBuilding)building;
                    break;
                }
            }

            //make sure a building was found and it was a storage building            
            Debug.Assert(addTo != null);
            Debug.Assert(addTo is StorageBuilding);

            //get the item to add, and amount to add
            ItemType itemToAdd = Program.Game.FarmData.GetItemType(m_itemToAdd.GetValue());
            int amount = m_amount.GetValue();

            //add to the buildings inventory
            addTo.Inventory.AddToInvetory(itemToAdd, amount);            
        }
 

    }
    

}
