using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Script event to adjust the stock of the store
    /// </summary>
    public class StoreStockEvent : ScriptEvent
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "STORESTOCK";


        /// <summary>
        /// What item should be adjusted
        /// </summary>
        private ScriptString m_effectedItem;
        
        /// <summary>
        /// The amount to increase or decrease the item.
        /// </summary>
        private ScriptNumber m_amount;
        
        /// <summary>
        /// The min amount the item should be after the change (or null if no min)
        /// </summary>
        private ScriptNumber m_min;
        
        /// <summary>
        /// The max amount the item should be after the change (or null if no max)
        /// </summary>
        private ScriptNumber m_max;
        
        
        /// <summary>
        /// Create a demand event
        /// </summary>
        public StoreStockEvent(string[] actionParams)
        {
            Debug.Assert(actionParams.Length == 2 || actionParams.Length == 4);

            m_effectedItem = new ScriptString(actionParams[0]);
            m_amount = new ScriptNumber(actionParams[1]);

            if (actionParams.Length == 4)
            {
                m_min = new ScriptNumber(actionParams[2]);
                m_max = new ScriptNumber(actionParams[3]);
            }
            else
            {
                //default min and max are 0, and int.Max
                m_min = new ScriptNumber("0");
                m_max = new ScriptNumber(int.MaxValue.ToString());
            }
        }


        public override void  DoEvent()
        {
            ItemType effectedItem = Program.Game.FarmData.GetItemType(m_effectedItem.GetValue());
            int amount = m_amount.GetValue();
            Program.Game.Store.Stock.IncreaseItemCount(effectedItem, amount);
            
            //make sure we didnt go below min or above max
            int max = m_max.GetValue();
            int min = m_min.GetValue();
            if (Program.Game.Store.Stock.GetItemCount(effectedItem) < min)
            {
                Program.Game.Store.Stock.SetItemCount(effectedItem, min);
            }
            else if (Program.Game.Store.Stock.GetItemCount(effectedItem) > max)
            {
                Program.Game.Store.Stock.SetItemCount(effectedItem, max);
            }
            
        }
 

    }
    

}
