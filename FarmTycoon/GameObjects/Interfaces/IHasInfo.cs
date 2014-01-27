using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public interface IHasInfo : IGameObject
    {
        IInfo Info
        {
            get;
        }
    }
}
