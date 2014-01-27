using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FarmTycoon
{
    public interface ISavable
    {
        /// <summary>
        /// Write the state of the gameobject to the state writer
        /// </summary>
        void WriteStateV1(StateWriterV1 writer);

        /// <summary>
        /// Read the state of the gameobject from state reader
        /// </summary>
        void ReadStateV1(StateReaderV1 reader);

        /// <summary>
        /// Called after the state of all objects has been read in
        /// </summary>
        void AfterReadStateV1();
        
    }
}
