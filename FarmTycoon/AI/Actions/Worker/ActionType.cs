using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// Type of Actions.
    /// There is not a 1-1 mapping between ActionTypes, and Action classes
    /// For instance GetItems, and PutItems both use the move items action class
    /// </summary>
    public enum ActionType
    {
        Move,
        Plow,
        GetItems,
        PutItems,
        Plant,
        Spray, //spray with Water/Fertilizer/Pesticide/etc
        Harvest,
        Purchase,
        Sell
    }
}
