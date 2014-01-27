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
    public class AdjustPriceEvent : ScriptEvent
    {
        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "ADJUSTPRICE";

        /// <summary>
        /// The kind of thing to set the price of
        /// </summary>
        private ScriptString m_kind;

        /// <summary>
        /// The item to adjust the price of
        /// </summary>
        private ScriptString m_name;

        /// <summary>
        /// The amount to adjust the price to
        /// </summary>
        private ScriptNumber m_adjustment;

        
        /// <summary>
        /// The min amount the item should be after the change (or null if no min)
        /// </summary>
        private ScriptNumber m_min;
        
        /// <summary>
        /// The max amount the item should be after the change (or null if no max)
        /// </summary>
        private ScriptNumber m_max;


        /// <summary>
        /// Create a SetItemPriceEvent
        /// </summary>
        public AdjustPriceEvent(string[] actionParams)
        {
            Debug.Assert(actionParams.Length == 3 || actionParams.Length == 5);

            m_kind = new ScriptString(actionParams[0]);
            m_name = new ScriptString(actionParams[1]);
            m_adjustment = new ScriptNumber(actionParams[2]);
            
            if (actionParams.Length == 5)
            {
                m_min = new ScriptNumber(actionParams[3]);
                m_max = new ScriptNumber(actionParams[4]);
            }
            else
            {
                //default min and max are 0, and int.Max
                m_min = new ScriptNumber("0");
                m_max = new ScriptNumber(int.MaxValue.ToString());
            }
        }


        public override void DoEvent()
        {
            string kind = m_kind.GetValue().ToUpper();
            int adjustment = m_adjustment.GetValue();
            int max = m_max.GetValue();
            int min = m_min.GetValue();

            if (kind == "ITEM")
            {
                ItemType item = Program.Game.FarmData.GetItemType(m_name.GetValue());
                Program.Game.Prices.AdjustPrice(item, adjustment);

                if (Program.Game.Prices.GetPrice(item) < min)
                {
                    Program.Game.Prices.SetPrice(item, min);
                }
                else if (Program.Game.Prices.GetPrice(item) > max)
                {
                    Program.Game.Prices.SetPrice(item, max);
                }
            }
            else if (kind == "OBJECT")
            {
                IGameObjectInfo objectInfo = Program.Game.FarmData.GetObjectInfo(m_name.GetValue());
                Program.Game.Prices.AdjustPrice(objectInfo, adjustment);

                if (Program.Game.Prices.GetPrice(objectInfo) < min)
                {
                    Program.Game.Prices.SetPrice(objectInfo, min);
                }
                else if (Program.Game.Prices.GetPrice(objectInfo) > max)
                {
                    Program.Game.Prices.SetPrice(objectInfo, max);
                }
            }
            else if (kind == "OTHER")
            {
                OtherPrice otherPrice = (OtherPrice)Enum.Parse(typeof(OtherPrice), m_name.GetValue());
                Program.Game.Prices.AdjustPrice(otherPrice, adjustment);

                if (Program.Game.Prices.GetPrice(otherPrice) < min)
                {
                    Program.Game.Prices.SetPrice(otherPrice, min);
                }
                else if (Program.Game.Prices.GetPrice(otherPrice) > max)
                {
                    Program.Game.Prices.SetPrice(otherPrice, max);
                }
            }
        }





    }


}
