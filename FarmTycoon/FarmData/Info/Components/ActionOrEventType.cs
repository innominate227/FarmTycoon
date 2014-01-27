using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// Types of action/events that can have texture associated with them or effect traits
    /// </summary>
    public enum ActionOrEventType
    {
        Move, //note the Move "action" is NOT applied to traits/texture on every move but a Move delay can be specifed.
        Rest,
        Plow,
        Plant,
        Pick,
        Harvest, //this is just pick action with harvest flag set.
        Spray,
        PutItems,
        GetItems,
        DisguardItems,
        VisitTrough,

        Baby,
        Consume,
        Die
    }
}
