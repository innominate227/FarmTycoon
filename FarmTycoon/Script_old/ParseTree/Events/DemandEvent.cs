using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Script event to adjust demand in the market
    /// </summary>
    public class DemandEvent : ScriptEvent
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "DEMAND";


        /// <summary>
        /// What subclass does this effect demand on
        /// </summary>
        private ScriptString m_effectedSubclass;
        
        /// <summary>
        /// The amount to increase demand.
        /// </summary>
        private ScriptNumber m_amount;
        
        
        /// <summary>
        /// Create a demand event
        /// </summary>
        public DemandEvent(string[] actionParams)
        {
            Debug.Assert(actionParams.Length == 2);

            m_effectedSubclass = new ScriptString(actionParams[0]);
            m_amount = new ScriptNumber(actionParams[1]);            
        }


        public override void  DoEvent(Game game)
        {
            string effectedSubclass = m_effectedSubclass.GetValue(game);
            int amount = m_amount.GetValue(game);
            game.Store.AddDemand(effectedSubclass, amount);
        }
 

    }
    

}
