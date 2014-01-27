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
    public class PriceOfVar : ScriptVar
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "PRICEOF";

        /// <summary>
        /// The kind of thing to get the price of
        /// </summary>
        private ScriptString m_kind;

        /// <summary>
        /// The item/building/scnery to get the price of
        /// </summary>
        private ScriptString m_name;



        public PriceOfVar(string[] actionParams)
        {
            Debug.Assert(actionParams.Length == 2);

            m_kind = new ScriptString(actionParams[0]);
            m_name = new ScriptString(actionParams[1]);
        }


        public override int GetValue()
        {
            string kind = m_kind.GetValue().ToUpper();
            
            if (kind == "ITEM")
            {
                ItemType item = Program.Game.FarmData.GetItemType(m_name.GetValue());
                return Program.Game.Prices.GetPrice(item);
            }
            else if (kind == "OBJECT")
            {
                IGameObjectInfo objectInfo = Program.Game.FarmData.GetObjectInfo(m_name.GetValue());
                return Program.Game.Prices.GetPrice(objectInfo);
            }
            else if (kind == "OTHER")
            {
                OtherPrice otherPrice = (OtherPrice)Enum.Parse(typeof(OtherPrice), m_name.GetValue());
                return Program.Game.Prices.GetPrice(otherPrice);
            }
            else
            {
                Debug.Assert(false);
                return 0;
            }
        }
 

    }
    

}
