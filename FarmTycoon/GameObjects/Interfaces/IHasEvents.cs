using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// A game object that has events occurr on it
    /// </summary>
    public interface IHasEvents : IGameObject, IHasTraits
    {
        /// <summary>
        /// Have the object process the event passed
        /// </summary>
        void ProcessEvent(ObjectEvent objectEvent);
    }
}
