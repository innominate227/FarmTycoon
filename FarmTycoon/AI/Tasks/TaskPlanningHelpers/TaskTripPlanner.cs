using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace FarmTycoon
{

    public delegate int MaxObjectsEachTripCallback(int workerNum);

    public delegate void PlanTripCallback<T>(int workerNum, List<T> tripObjects);


    public class TaskTripPlanner<T>
    {
        /// <summary>
        /// List of objects that we need to visit
        /// </summary>
        private IList<T> _objectsToVisit;

        /// <summary>
        /// Number of workers the trips should be split across
        /// </summary>
        private int _numberOfWorkers;

        /// <summary>
        /// The number of objects the worker can manage on each trip for each worker (keyed on worker number)
        /// </summary>
        private Dictionary<int, int> _workersObjectsPerTrip = new Dictionary<int, int>();
        
        /// <summary>
        /// Called to determine the maximum possible objects that a worker would be able to tend to in one trip
        /// </summary>
        private PlanTripCallback<T> _planTripCallback;


        

        public TaskTripPlanner() { }


        /// <summary>
        /// List of objects that we need to visit
        /// </summary>
        public IList<T> ObjectsToVisit
        {
            set { _objectsToVisit = value; }
            get { return _objectsToVisit; }
        }


        /// <summary>
        /// The number of workers to split the trips between
        /// </summary>
        public int NumberOfWorkers
        {
            set { _numberOfWorkers = value; }
            get { return _numberOfWorkers; }
        }


        /// <summary>
        /// Set the number of objects the workers can handle on each trip.
        /// </summary>
        public void SetMaxObjectsPerTripForAll(int maxObjects)
        {
            for (int workerNum = 0; workerNum < _numberOfWorkers; workerNum++)
            {
                SetMaxObjectsPerTrip(workerNum, maxObjects);
            }
        }

        /// <summary>
        /// Set the number of objects the worker workerNum can handle on each trip.
        /// If different workers have different equipment they may be able to handle a different number of objects.
        /// </summary>
        public void SetMaxObjectsPerTrip(int workerNumber, int maxObjects)
        {
            if (_workersObjectsPerTrip.ContainsKey(workerNumber) == false)
            {
                _workersObjectsPerTrip.Add(workerNumber, maxObjects);
            }
            else
            {
                _workersObjectsPerTrip[workerNumber] = maxObjects;
            }
        }

        /// <summary>
        /// Set the function to call to plan a trip for a worker
        /// </summary>
        public void SetPlanTripCallback(PlanTripCallback<T> callback)
        {
            _planTripCallback = callback;
        }
                
        
        /// <summary>
        /// Plan out the task
        /// </summary>
        public void PlanTrips()
        {
            //nothing to visit then were done
            if (_objectsToVisit.Count == 0)
            {
                return;
            }
                                                 
            //have each worker plan a trip
            for (int workerNum = 0; workerNum < _numberOfWorkers; workerNum++)
            {
                //get the maximum number of objects this worker can do each trip
                int maxObjectsWorkerCanDoEachTrip = _workersObjectsPerTrip[workerNum];

                //get the objects this worker is responsible for                
                List<T> workerResponsibility = CalculateWorkerResponsiblity(workerNum);

                //count of the objects that the worker is responsible for
                int numberOfObjectsWorkerIsResponsibleFor = workerResponsibility.Count;

                //determine how many trips the worker will need to make                
                int tripsNeeded = (int)Math.Ceiling((double)(numberOfObjectsWorkerIsResponsibleFor) / (double)maxObjectsWorkerCanDoEachTrip);

                //plan each trip for this worker
                for (int tripNum = 0; tripNum < tripsNeeded; tripNum++)
                {
                    //determine how many objects the worker will handel on this trip (do as much as possible, except on the last trip do whats left)
                    int numberOfObjectsForThisTrip = maxObjectsWorkerCanDoEachTrip;
                    if (tripNum == tripsNeeded - 1)
                    {
                        numberOfObjectsForThisTrip = numberOfObjectsWorkerIsResponsibleFor % maxObjectsWorkerCanDoEachTrip;
                        if (numberOfObjectsForThisTrip == 0) { numberOfObjectsForThisTrip = maxObjectsWorkerCanDoEachTrip; }
                    }

                    //determine the object range for this trip (we did as many objects as possible on each trip before this one)
                    int startIndexForThisTrip = maxObjectsWorkerCanDoEachTrip * tripNum;
                    List<T> objectsThisTrip = new List<T>();
                    for (int tripObjectIndex = startIndexForThisTrip; tripObjectIndex < startIndexForThisTrip + numberOfObjectsForThisTrip; tripObjectIndex++)
                    {
                        objectsThisTrip.Add(workerResponsibility[tripObjectIndex]);
                    }

                    //plan the one trip for the worker
                    _planTripCallback(workerNum, objectsThisTrip);
                }
            } //for each worker

        }
        
        
        /// <summary>
        /// Determine what areas the field a worker is responsible for based on the total number of workers and their worker number (0 based).
        /// And passed a list of all land in the field that needs to be acted on for this task
        /// </summary>
        private List<T> CalculateWorkerResponsiblity(int workerNumber)
        {
            int allObjectsToVisitCount = _objectsToVisit.Count;
            
            //how many land tiles this worker will need to work
            int numberOfTilesToWork = allObjectsToVisitCount / _numberOfWorkers;
            if (workerNumber < allObjectsToVisitCount % _numberOfWorkers)
            {
                numberOfTilesToWork++;
            }

            //determine what index this worker shold start on
            int startIndex = workerNumber * (allObjectsToVisitCount / _numberOfWorkers);
            startIndex += (allObjectsToVisitCount % _numberOfWorkers);
            if (workerNumber < (allObjectsToVisitCount % _numberOfWorkers))
            {
                startIndex -= ((allObjectsToVisitCount % _numberOfWorkers) - workerNumber);
            }
            
            //create the list of objects this worker should visit
            List<T> objectsForThisWorker = new List<T>();
            for (int i = startIndex; i < startIndex + numberOfTilesToWork; i++)
            {
                objectsForThisWorker.Add(_objectsToVisit[i]);
            }
            return objectsForThisWorker;
        }
                
    }
}
