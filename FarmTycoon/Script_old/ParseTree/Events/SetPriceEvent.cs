using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Script event to set the price of an item in the store, or a building, or scenery
    /// </summary>
    public class SetPriceEvent : ScriptEvent
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "SETPRICE";

        /// <summary>
        /// The kind of thing to set the price of
        /// </summary>
        private ScriptString m_kind;

        /// <summary>
        /// The item to adjust the price of
        /// </summary>
        private ScriptString m_name;

        /// <summary>
        /// The amount to set the price to
        /// </summary>
        private ScriptNumber m_price;
        
        
        /// <summary>
        /// Create a SetItemPriceEvent
        /// </summary>
        public SetPriceEvent(string[] actionParams)
        {
            Debug.Assert(actionParams.Length == 3);

            m_kind = new ScriptString(actionParams[0]);
            m_name = new ScriptString(actionParams[1]);
            m_price = new ScriptNumber(actionParams[2]);
        }


        public override void DoEvent()
        {
            string kind = m_kind.GetValue().ToUpper();
            int price = m_price.GetValue();

            if (kind == "ITEM")
            {
                ItemType item = Program.Game.FarmData.GetItemType(m_name.GetValue());                
                Program.Game.Prices.SetPrice(item, price);
            }
            else if (kind == "OBJECT")
            {
                IGameObjectInfo objectInfo = Program.Game.FarmData.GetObjectInfo(m_name.GetValue());
                Program.Game.Prices.SetPrice(objectInfo, price);
            }
            else if (kind == "OTHER")
            {
                OtherPrice otherPrice = (OtherPrice)Enum.Parse(typeof(OtherPrice), m_name.GetValue());
                Program.Game.Prices.SetPrice(otherPrice, price);
            }
        }



 

    }
    

}
