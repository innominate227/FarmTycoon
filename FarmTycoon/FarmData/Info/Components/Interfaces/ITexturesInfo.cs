using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public interface ITexturesInfo : IInfo
    {            
        /// <summary>
        /// Contains info on the normal an temp textures for this object
        /// </summary>
        TexturesInfoSet Textures
        {
            get;
        }

    }
}
