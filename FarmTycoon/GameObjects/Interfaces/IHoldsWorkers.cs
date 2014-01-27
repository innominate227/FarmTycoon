using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// A building that can hold workers inside of it
    /// </summary>
    public interface IHoldsWorkers : IGameObject
    {


        /// <summary>
        /// The list of workers that are inside the break house.
        /// </summary>
        WorkersInsideList WorkersInside
        {
            get;
        }
        
        
    }
}
