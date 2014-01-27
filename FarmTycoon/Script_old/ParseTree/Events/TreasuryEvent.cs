using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Script event to adjust the players treasury
    /// </summary>
    public class TreasuryEvent : ScriptEvent
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "TREASURY";
        
        /// <summary>
        /// The amount to increase treasury.
        /// </summary>
        private ScriptNumber m_amount;
        
        
        /// <summary>
        /// Create a treasury evnet
        /// </summary>
        public TreasuryEvent(string[] actionParams)
        {
            Debug.Assert(actionParams.Length == 1);

            m_amount = new ScriptNumber(actionParams[0]);
        }


        public override void DoEvent()
        {            
            int amount = m_amount.GetValue();
            if (amount >= 0)
            {
                Program.Game.Treasury.Sell(SpendingCatagory.IncomeEvemts, amount);
            }
            else
            {
                Program.Game.Treasury.Buy(SpendingCatagory.ExpenseEvemts, -1 * amount);
            }
        }
 

    }
    

}
