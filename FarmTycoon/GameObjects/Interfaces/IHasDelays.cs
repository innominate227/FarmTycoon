using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public interface IHasDelays : IGameObject
    {
        DelaySet Delays
        {
            get;
        }
    }
}
