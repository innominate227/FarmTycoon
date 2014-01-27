using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Base abstract class where the task involves performing the same action on several game objects.
    /// For instance spraying all crops in a field.  The class assits in dividing the work across multiple trips where as many crops as possible are sprayed in each trip.
    /// </summary>
    public abstract class MultiObjectTask : Task
    {
        /// <summary>
        /// Should the worker use equipment to complete the task
        /// </summary>
        protected bool m_useEquipment = false;
        
        /// <summary>
        /// Create a new MultiObjectTask.  
        /// Call either Setup, and ReadState on the task after creating it.
        /// </summary>
        public MultiObjectTask() : base() { }
        

        /// <summary>
        /// Should the worker use equipment to complete the task
        /// </summary>
        public bool UseEquipment
        {
            set { m_useEquipment = value; }
            get { return m_useEquipment; }
        }

        /// <summary>
        /// Return the IHasActionLocation objects that should be worked for this task
        /// </summary>
        protected abstract List<IHasActionLocation> GetObjectsToWork();

        /// <summary>
        /// Determine how many objects in the worker can handel on each trip
        /// </summary>
        protected abstract int GetMaxObjectsPerTrip(int workerNum, TaskItemPlanner itemPlanner, TaskEquipmentPlanner equipmentPlanner);
        
        /// <summary>
        /// Called before any trips are planned.  Allows derived classes to plan what to do before any trip is made (such as get items), or check for issues.
        /// </summary>
        protected virtual void PlanBeforeTrips(TaskPlan plan, TaskItemPlanner itemPlanner, TaskEquipmentPlanner equipmentPlanner) { }

        /// <summary>
        /// Plan one trip to the set of objects for one worker
        /// </summary>
        protected abstract void PlanTrip(TaskPlan plan, int workerNum, List<IHasActionLocation> objectsForTrip, TaskItemPlanner itemPlanner, TaskEquipmentPlanner equipmentPlanner);

        /// <summary>
        /// Called after all trips are planned.  Allows derived classes to plan what to do after all trips are made.
        /// </summary>
        protected virtual void PlanAfterTrips(TaskPlan plan, TaskItemPlanner itemPlanner, TaskEquipmentPlanner equipmentPlanner) { }
        
        
        /// <summary>
        /// Plan out the task
        /// </summary>
        protected override TaskPlan PlanTaskInner()
        {
            //create a task plan to return
            TaskPlan plan = new TaskPlan(this);

            //create an itemplann and equipmnet planner, for derived classes to use through the planning process
            TaskItemPlanner itemPlanner = new TaskItemPlanner();
            TaskEquipmentPlanner equipmentPlanner = new TaskEquipmentPlanner(itemPlanner);
            
            //do any planning that needs to be done before starting the first trip
            PlanBeforeTrips(plan, itemPlanner, equipmentPlanner);

            //if we can no longer calculate the expected time stop trying to plan the rest of the task
            if (plan.CanCalculateExpectedTime == false) { return plan; }

            //get the land in the field we need to visit for this field task (it should never be the case that there is no land for us to visit)
            List<Land> landForTask = DetermineLandThatNeedsToBeVisited();

            //if we determined that no land needed to be visited, we must be planning the task before its possible to complete, just pretend that we would visit all the land
            if (landForTask.Count == 0)
            {
                landForTask = m_field.Enclosure.OrderedLand.ToList();
            }
                                                
            //determin the land that each worker is responsible for
            Dictionary<int, List<Land>> workersResponsibilities = new Dictionary<int, List<Land>>();
            for (int workerNum = 0; workerNum < m_numberOfWorkers; workerNum++)
            {                
                List<Land> workerResponsibility = CalculateWorkerResponsiblity(m_numberOfWorkers, workerNum, landForTask);
                workersResponsibilities.Add(workerNum, workerResponsibility);
            }
            
            //continues having each worker plan a trip until all workers have planned all trips
            int tripNum = 0;
            bool allWorkersDonePlannedAllTrips = false;
            while (allWorkersDonePlannedAllTrips == false)
            {
                allWorkersDonePlannedAllTrips = true;

                //have each worker plan a trip
                for (int workerNum = 0; workerNum < m_numberOfWorkers; workerNum++)
                {
                    //determine the maximum number of tiles we can do each trip. (this should always be greater than 0)
                    int maxTilesPerTrip = DetermineMaxTilesPerTrip(workerNum, itemPlanner, equipmentPlanner);
                    Debug.Assert(maxTilesPerTrip > 0);

                    //get the section of the field this worker is responsible for                
                    List<Land> workerResponsibility = workersResponsibilities[workerNum];

                    //how many tiles of the field the worker is responsible for
                    int numberOfTileWorkerResponsibleFor = workerResponsibility.Count;

                    //determine how many trips the worker will need to make                
                    int tripsNeeded = (int)Math.Ceiling((double)(numberOfTileWorkerResponsibleFor) / (double)maxTilesPerTrip);

                    //plan the trip unless the worker has already made all the trips they needed to
                    if (tripNum < tripsNeeded)
                    {
                        //we are not done with planning yet
                        allWorkersDonePlannedAllTrips = false;

                        //determine how many tiles the worker will handel on this trip (do as much as possible, except on the last trip do whats left)
                        int numberOfTilesForThisTrip = maxTilesPerTrip;
                        if (tripNum == tripsNeeded - 1)
                        {
                            numberOfTilesForThisTrip = numberOfTileWorkerResponsibleFor % maxTilesPerTrip;
                            if (numberOfTilesForThisTrip == 0) { numberOfTilesForThisTrip = maxTilesPerTrip; }
                        }

                        //determine the land range for this trip (we did as many tiles as possible on each trip before this one)
                        int startIndexForThisTrip = maxTilesPerTrip * tripNum;
                        List<Land> tilesThisTrip = new List<Land>();
                        for (int tripTileIndex = startIndexForThisTrip; tripTileIndex < startIndexForThisTrip + numberOfTilesForThisTrip; tripTileIndex++)
                        {
                            tilesThisTrip.Add(workerResponsibility[tripTileIndex]);
                        }

                        //plan the one trip for the worker
                        PlanTrip(plan, workerNum, tilesThisTrip, itemPlanner, equipmentPlanner);
                    }
                } //for each worker

                //increase number of the trip being planned
                tripNum++;
            } //while worker is still planning
            
            //do any planning that needs to be done after planning all trips
            PlanAfterTrips(plan, itemPlanner, equipmentPlanner);

            return plan;
        }
        
        /// <summary>
        /// Determine what land in the field needs to be worked for this task
        /// </summary>
        /// <returns></returns>
        private List<Land> DetermineLandThatNeedsToBeVisited()
        {
            bool withCrop = WorkTilesWithCrop();

            List<Land> landToVisit = new List<Land>();
            foreach (Land fieldLand in m_field.Enclosure.OrderedLand)
            {
                if ((withCrop && fieldLand.LocationOn.Contains<Crop>()) ||
                    (withCrop == false && fieldLand.LocationOn.Contains<Crop>() == false))
                {
                    landToVisit.Add(fieldLand);
                }
            }
            return landToVisit;
        }
        
        /// <summary>
        /// Determine what areas the field a worker is responsible for based on the total number of workers and their worker number (0 based).
        /// And passed a list of all land in the field that needs to be acted on for this task
        /// </summary>
        private List<Land> CalculateWorkerResponsiblity(int totalWorkers, int workerNumber, List<Land> allLandToVisit)
        {
            int allLandToVisitCount = allLandToVisit.Count;
            
            //how many land tiles this worker will need to work
            int numberOfTilesToWork = allLandToVisitCount / totalWorkers;
            if (workerNumber < allLandToVisitCount % totalWorkers)
            {
                numberOfTilesToWork++;
            }

            //determine what index this worker shold start on
            int startIndex = workerNumber * (allLandToVisitCount / totalWorkers);
            startIndex += (allLandToVisitCount % totalWorkers);
            if (workerNumber < (allLandToVisitCount % totalWorkers))
            {
                startIndex -= ((allLandToVisitCount % totalWorkers) - workerNumber);
            }
            
            //create the list of land this worker should visit
            List<Land> landForThisWorker = new List<Land>();
            for (int i = startIndex; i < startIndex + numberOfTilesToWork; i++)
            {
                landForThisWorker.Add(allLandToVisit[i]);
            }
            return landForThisWorker;
        }

        

        #region Read Write

        public override void WriteState(ObjectState state)
        {
            base.WriteState(state);
            state.SetValue("UseEquipment", m_useEquipment);
            state.SetValue("Field", m_field);
        }

        public override void ReadState(ObjectState state)
        {
            base.ReadState(state);
            m_useEquipment = state.GetValue<bool>("UseEquipment");
            m_field = state.GetValue<Field>("Field");
        }

        #endregion
    }
}
