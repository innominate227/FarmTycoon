using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class SellItemsAction : OneLocationAction
    {
        /// <summary>
        /// Building to go to to sell the items
        /// </summary>
        private Building m_building;


        public SellItemsAction() { }

        /// <summary>
        /// Create a sell items action
        /// </summary>
        public SellItemsAction(Building building)
        {
            m_building = building;
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

            //have the worker sell everything in their inventory
            foreach (GameItemType itemType in m_worker.Inventory.Types)
            {
                //determine the amount to sell
                int amountToSell = m_worker.Inventory.GetTypeCount(itemType);
                                
                //amount of money the worker will make
                int itemCost = m_worker.Game.Market.GetItemPrice(itemType);
                int profit = (int)(amountToSell * itemCost);

                //put the money in the users treasury
                m_worker.Game.Treasury.Transaction(SpendingCatagory.ItemSales, profit);

                //remove the items from the workers inventory
                m_worker.Inventory.RemoveFromInvetory(itemType, amountToSell);
            }            
        }


        
        

        public override string Description()
        {
            return "Selling Items";                        
        }



        public override void WriteState(ObjectState state)
        {
            base.WriteState(state);
            state.SetValue("Building", m_building);
        }

        public override void ReadState(ObjectState state)
        {
            base.ReadState(state);
            m_building = state.GetValue<Building>("Building");
        }
    }
}
