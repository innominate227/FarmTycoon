using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class SellEquipmentAction : OneLocationAction
    {
        /// <summary>
        /// Equipmen type to sell
        /// </summary>
        private EquipmentType m_toSell;
        
        /// <summary>
        /// Building we are sell it at
        /// </summary>
        private Building m_building;


        public SellEquipmentAction() { }

        public SellEquipmentAction(Building building, EquipmentType toSell)
        {
            m_building = building;
            m_toSell = toSell;
        }


        /// <summary>
        /// equipment type to sell
        /// </summary>
        public EquipmentType ToSell
        {
            get { return m_toSell; }
        }


        public override Location TheLocation()
        {
            return m_building.ActionLocation;
        }

        public override double GetActionTime(EquipmentType expectedTractor, EquipmentType expectedTow)
        {
            //for now always takes the same amount of time to buy items no matter how much the worker is getting
            return 1.0 / 24.0;
        }


        public override void DoAction()
        {
            //determine how much the equipment costs
            int itemCost = m_worker.Game.Market.GetItemPrice(m_toSell.ItemType);

            //get money for the equipment
            m_worker.Game.Treasury.Transaction(SpendingCatagory.ItemSales, itemCost);

            //have the worker get off the equipment sold
            m_worker.GetOffEquipment(m_toSell);
        }   

        

        public override string Description()
        {
            //string itemList = "";
            //foreach (GameItemType itemType in m_buyList.ItemTypes)
            //{
            //    itemList += itemType.Name;                
            //    itemList += "(" + m_buyList.GetItemCount(itemType).ToString() + ") ,";                
            //}
            //if (itemList == ""){ itemList= "Nothing";}
            
            return "Selling Equipment";                        
        }



        public override void WriteState(ObjectState state)
        {
            base.WriteState(state);
            state.SetValue("Building", m_building);
            state.SetValue("ToSell", m_toSell.ItemType.Name);
        }

        public override void ReadState(ObjectState state)
        {
            base.ReadState(state);
            m_building = state.GetValue<Building>("Building");
            m_toSell = Program.DataFiles.EquipmentFile.GetEquipmentType(state.GetValue<string>("ToSell"));
        }
    }
}
