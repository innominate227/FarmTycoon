using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    public class BuyEquipmentAction : OneLocationAction
    {
        /// <summary>
        /// Equipment type to buy
        /// </summary>
        private EquipmentType m_toBuy;
        
        /// <summary>
        /// Building we are geting it from
        /// </summary>
        private Building m_building;


        public BuyEquipmentAction() { }

        public BuyEquipmentAction(Building building, EquipmentType toBuy)
        {
            Debug.Assert(toBuy.IsNullType() == false);
            m_building = building;
            m_toBuy = toBuy;
        }
        
        /// <summary>
        /// equipment type to buy
        /// </summary>
        public EquipmentType ToBuy
        {
            get { return m_toBuy; }
        }

        public override Location TheLocation()
        {
            return m_building.ActionLocation;
        }
        
        public override double GetActionTime(EquipmentType expectedTractor, EquipmentType expectedTow)
        {
            return 1.0 / 24.0;
        }

        public override void DoAction()
        {
            //determine how much the item cost
            int itemCost = m_worker.Game.Market.GetItemPrice(m_toBuy.ItemType);

            //pay for the items
            m_worker.Game.Treasury.Transaction(SpendingCatagory.ItemsPurchase, -1*itemCost);

            //give that equipment to the worker
            m_worker.SetEquipment(m_toBuy);
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
            
            return "Buying Equipment";                        
        }



        public override void WriteState(ObjectState state)
        {
            base.WriteState(state);
            state.SetValue("Building", m_building);
            state.SetValue("ToBuy", m_toBuy.ItemType.Name);
        }

        public override void ReadState(ObjectState state)
        {
            base.ReadState(state);
            m_building = state.GetValue<Building>("Building");
            m_toBuy = Program.DataFiles.EquipmentFile.GetEquipmentType(state.GetValue<string>("ToBuy"));
        }
    }
}
