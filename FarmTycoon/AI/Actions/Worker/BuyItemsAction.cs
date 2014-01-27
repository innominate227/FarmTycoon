using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class BuyItemsAction : OneLocationAction
    {
        /// <summary>
        /// List of items to buy
        /// </summary>
        private ItemList m_buyList;
        
        /// <summary>
        /// Building we are geting the items from
        /// </summary>
        private Building m_building;


        public BuyItemsAction() { }
                
        /// <summary>
        /// Create a purchase items action
        /// </summary>
        public BuyItemsAction(Building building, ItemList buyList)
        {
            m_building = building;
            m_buyList = buyList;
        }
        
        /// <summary>
        /// List of items to buy
        /// </summary>
        public ItemList BuyList
        {
            get { return m_buyList; }
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

            //buy the correct amount of each item type in the buy list
            foreach (GameItemType itemType in m_buyList.ItemTypes)
            {
                //determine the amount to buy
                int amountToBuy = m_buyList.GetItemCount(itemType);
                
                //determine how much the worker getting the items can hold
                int amountThatCanFit = m_worker.Inventory.AmountThatWillFit(itemType);                

                //check if it more than the amount the worker can hold
                if (amountToBuy > amountThatCanFit)
                {
                    throw new Exception("Worker can not carry that much");   
                }

                //determine how much the item cost
                int itemCost = m_worker.Game.Market.GetItemPrice(itemType);
                int costForThisItemType = amountToBuy * itemCost;
                
                //pay for the items
                m_worker.Game.Treasury.Transaction(SpendingCatagory.ItemSales, -1*costForThisItemType);

                //give that amount to the worker
                m_worker.Inventory.AddToInvetory(itemType, amountToBuy);                
            }            
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
            
            return "Buying Items";                        
        }



        public override void WriteState(ObjectState state)
        {
            base.WriteState(state);
            state.SetValue("Building", m_building);
            state.WriteSubState("BuyList", m_buyList);
        }

        public override void ReadState(ObjectState state)
        {
            base.ReadState(state);
            m_building = state.GetValue<Building>("Building");
            m_buyList = state.ReadSubState<ItemList>("BuyList");
        }

    }
}
