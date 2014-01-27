using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// A list of workers that are inside a building, and workers that have reserved a spot within the building
    /// </summary>
    public class WorkersInsideList : ISavable
    {
        /// <summary>
        /// Raised when workers inside the building changes, workers heading toward the building changes, or when workers with spots reserved changes
        /// </summary>
        public event Action Changed;



        #region Member Vars

        /// <summary>
        /// The list of workers that have reserved a spot in the building
        /// </summary>
        private List<Worker> _workersWithSpotReserved = new List<Worker>();

        /// <summary>
        /// The list of workers that are inside the building
        /// </summary>
        private List<Worker> _workersInside = new List<Worker>();

        /// <summary>
        /// The list of workers that are heading toward the building
        /// </summary>
        private List<Worker> _workersHeadingToward = new List<Worker>();

        #endregion


        #region Properties


        /// <summary>
        /// The list of workers that have reserved a spot in the building.  Do not modify this list directly.
        /// </summary>
        public List<Worker> WorkersWithSpotReserved
        {
            get { return _workersWithSpotReserved; }
        }

        /// <summary>
        /// The list of workers that are inside the building.  Do not modify this list directly.
        /// </summary>
        public List<Worker> WorkersInside
        {
            get { return _workersInside; }
        }

        /// <summary>
        /// The list of workers that are heading toward the building.  Do not modify this list directly.
        /// </summary>
        public List<Worker> WorkersHeadingToward
        {
            get { return _workersHeadingToward; }
        }
        
        #endregion


        #region Logic
        
        /// <summary>
        /// Reserve a spot in the building for the worker passed
        /// </summary>
        public void ReserveSpotFor(Worker worker)
        {
            _workersWithSpotReserved.Add(worker);
            if (Changed != null) { Changed(); }
        }

        /// <summary>
        /// Free the spot in the building that was reserved for the worker passed
        /// </summary>
        public void FreeSpotFor(Worker worker)
        {
            _workersWithSpotReserved.Remove(worker);
            if (Changed != null) { Changed(); }
        }


        /// <summary>
        /// Add a worker into the building
        /// Note: worker automatically does this when you call EnterBuilding().
        /// </summary>
        public void AddWorker(Worker worker)
        {
            _workersInside.Add(worker);
            if (Changed != null) { Changed(); }
        }

        /// <summary>
        /// Remove a worker from the building.
        /// Note: worker automatically does this when you call ExitBuilding().
        /// </summary>
        public void RemoveWorker(Worker worker)
        {
            _workersInside.Remove(worker);
            if (Changed != null) { Changed(); }
        }


        /// <summary>
        /// Add a worker into the building
        /// </summary>
        public void AddWorkerHeadingToward(Worker worker)
        {
            _workersHeadingToward.Add(worker);
            if (Changed != null) { Changed(); }
        }

        /// <summary>
        /// Remove a worker from the building
        /// </summary>
        public void RemoveWorkerHeadingToward(Worker worker)
        {
            _workersHeadingToward.Remove(worker);
            if (Changed != null) { Changed(); }
        }

        #endregion

        #region Save Load

        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteObjectList<Worker>(_workersHeadingToward);
            writer.WriteObjectList<Worker>(_workersInside);
            writer.WriteObjectList<Worker>(_workersWithSpotReserved);
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _workersHeadingToward = reader.ReadObjectList<Worker>();
            _workersInside = reader.ReadObjectList<Worker>();
            _workersWithSpotReserved = reader.ReadObjectList<Worker>();
        }

        public void AfterReadStateV1()
        {            
        }

        #endregion

    }
}
