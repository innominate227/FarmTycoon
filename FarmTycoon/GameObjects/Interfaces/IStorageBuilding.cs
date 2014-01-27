using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public interface IStorageBuilding : IHasActionLocation, IHasInventory, IGameObject
    {

        IStorageBuildingInfo StorageBuildingInfo
        {
            get;
        }

    }
}
