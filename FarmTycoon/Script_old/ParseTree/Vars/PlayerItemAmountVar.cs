using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Var for the amount of stock of an item that the player has
    /// </summary>
    public class PlayerItemAmountVar : ScriptVar
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "PLAYERITEMAMOUNT";
        
        /// <summary>
        /// What item we should get the price of should
        /// </summary>
        private List<ScriptString> m_items = new List<ScriptString>();


        public PlayerItemAmountVar(string[] actionParams)
        {
            Debug.Assert(actionParams.Length >= 1);

            foreach (string actionParam in actionParams)
            {
                m_items.Add(new ScriptString(actionParam));
            }
        }


        public override int GetValue()
        {
            int totalCount = 0;
            foreach (ScriptString itemString in m_items)
            {
                ItemType item = Program.Game.FarmData.GetItemType(itemString.GetValue());
                totalCount += Program.Game.MasterItemList.GetItemCount(item);
            }
            return totalCount;
        }
 

    }
    

}
