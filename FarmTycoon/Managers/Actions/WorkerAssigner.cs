using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Keeps track of avaiable workers and assigns tasks to the worker best suited for the task
    /// </summary>
    public class WorkerAssigner : ISavable
    {
        #region Events

        public event Action NumberOfAvaiableWorkersChanged;

        #endregion

        #region Member Vars

        /// <summary>
        /// List of all avaiable workers, kept sorted by the energy level of the workers
        /// </summary>
        private LinkedList<Worker> _avaiableWorkers = new LinkedList<Worker>();

        #endregion

        #region Properties

        /// <summary>
        /// Returns the number of avaiable workers
        /// </summary>        
        public int NumberOfAvailableWorkers
        {
            get
            {
                return _avaiableWorkers.Count;
            }
        }

        /// <summary>
        /// The next worker that is most likly going to be assigned by the Worker assigner
        /// </summary>
        public Worker NextWorkerThatWillBeAssigned
        {
            get
            {
                if (_avaiableWorkers.Count == 0)
                {
                    return null;
                }
                else
                {
                    return _avaiableWorkers.First.Value;
                }
            }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Find the best worker to do each action seqeuence in the taskplan passed.        
        /// Return the list of workers assigned.
        /// This should only be called if it is known that enough workers are avialbe to do the task.  
        /// Use NumberOfAvailableWorkers to determine if enough are avaialble
        /// </summary>
        public List<Worker> AssignWorkers(TaskPlan taskPlan)
        {
            //how many workers do we need to assigned (only assign workers for action sequences that actually have actions)
            int workersNeeded = 0;
            foreach (ActionSequence<Worker> actionSequence in taskPlan.ActionSequences)
            {
                if (actionSequence.Actions.Count > 0)
                {
                    workersNeeded++;
                }
            }
            
            //make sure we have enough workers available
            Debug.Assert(workersNeeded <= _avaiableWorkers.Count);

            //the list of workers that will be assigned to do the task
            List<Worker> workersToAssign = new List<Worker>();
                        
            //find perferred workers to assign to the task
            List<Worker> preferredWorkers = taskPlan.Task.PreferredWorkers;
            FindPerferredWorkers(workersNeeded, preferredWorkers, workersToAssign);

            //find additional workers to assign to the task
            FindAdditionalWorkers(workersNeeded, workersToAssign);
            
            //assigned each worker to an action sequence seqeuence
            int workerNum = 0;
            foreach (ActionSequence<Worker> actionSequence in taskPlan.ActionSequences)
            {
                //ignore empty action sequences
                if (actionSequence.Actions.Count == 0) { continue; }                

                //have the worker start the action sequence
                Worker toAssign = workersToAssign[workerNum];
                toAssign.DoActionSequenceForTask(taskPlan.Task, actionSequence);
                workerNum++;
            }

            //less workers are avaiable now
            RaiseNumberOfAvaiableWorkersChanged();

            //return the list of workers that were assigned
            return workersToAssign;
        }


        /// <summary>
        /// Assign up to workersNeeded preferred workers that from the available list.
        /// Adds the workers to the workersToAssign list.
        /// If not enough perferred workers are available it will assign as many as are available
        /// </summary>
        private void FindPerferredWorkers(int workersNeeded, List<Worker> preferredWorkers, List<Worker> workersToAssign)
        {
            //see if there are any preffered workers
            if (preferredWorkers.Count > 0)
            {
                //walk the avilable workers list in order and see if any are preferred
                LinkedListNode<Worker> workerNode = _avaiableWorkers.First;
                while (workerNode != null)
                {
                    LinkedListNode<Worker> nextWorkerNode = workerNode.Next;
                    Worker worker = workerNode.Value;

                    //if we found a preffered worker
                    if (preferredWorkers.Contains(worker))
                    {
                        //add to list of workers to assign
                        workersToAssign.Add(worker);

                        //remove worker from available list
                        _avaiableWorkers.Remove(workerNode);

                        //if we have found enough workers we are done
                        if (workersToAssign.Count == workersNeeded)
                        {
                            break;
                        }
                    }

                    //go to next worker node
                    workerNode = nextWorkerNode;
                }
            }
        }
        
        /// <summary>
        /// Assign up to workersNeeded workers that to the available list.
        /// Adds the workers to the workersToAssign list.
        /// </summary>
        private void FindAdditionalWorkers(int workersNeeded, List<Worker> workersToAssign)
        {
            //if we already have enough workers we dont need to find any more
            if (workersToAssign.Count == workersNeeded)
            {
                return;
            }

            //walk the avilable workers list in order and assign the workers (fastest workers are at the front of the list)
            LinkedListNode<Worker> workerNode = _avaiableWorkers.First;
            while (workerNode != null)
            {
                LinkedListNode<Worker> nextWorkerNode = workerNode.Next;
                Worker worker = workerNode.Value;

                //add to list of workers to assign
                workersToAssign.Add(worker);

                //remove worker from available list
                _avaiableWorkers.Remove(workerNode);

                //if we have found enough workers we are done
                if (workersToAssign.Count == workersNeeded)
                {
                    break;
                }                

                //go to next worker node
                workerNode = nextWorkerNode;
            }
            
        }


        /// <summary>
        /// Inform the assigner that a worker has just become avaiable
        /// </summary>
        public void WorkerNowAvaiable(Worker newAvaiableWorker)
        {
            //get the energy level for the new worker
            int newAvaiableWorkerEnergy = newAvaiableWorker.Traits.GetTraitValue(SpecialTraits.ENERGY_TRAIT);

            
            //walk the avilable workers list in reverse-order 
            //because this worker probably just did a task and will have lower energy
            bool workerAdded = false;
            LinkedListNode<Worker> otherWorkerNode = _avaiableWorkers.Last;
            while (otherWorkerNode != null)
            {
                LinkedListNode<Worker> prevOtherWorkerNode = otherWorkerNode.Previous;
                Worker otherWorker = otherWorkerNode.Value;

                //get the energy level of the other worker                
                int otherWorkerEnergy = otherWorker.Traits.GetTraitValue(SpecialTraits.ENERGY_TRAIT);

                //if we have equal or less energy we put ourselfs after the other worker in the list
                if (newAvaiableWorkerEnergy <= otherWorkerEnergy)
                {
                    _avaiableWorkers.AddAfter(otherWorkerNode, newAvaiableWorker);
                    workerAdded = true;
                    break;
                }

                //go to previous worker node
                otherWorkerNode = prevOtherWorkerNode;
            }

            //worker was not added after any other then it belongs in the front
            if (workerAdded == false)
            {
                _avaiableWorkers.AddFirst(newAvaiableWorker);
            }            


            //raise aviable workers changed event
            RaiseNumberOfAvaiableWorkersChanged();
        }
        
        /// <summary>
        /// Raise the number of available workers changed event
        /// </summary>
        private void RaiseNumberOfAvaiableWorkersChanged()
        {
            if (NumberOfAvaiableWorkersChanged != null)
            {
                NumberOfAvaiableWorkersChanged();
            }
        }

        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
		{
			writer.WriteObjectList<Worker>(new List<Worker>(_avaiableWorkers));			
		}

        public void ReadStateV1(StateReaderV1 reader)
        {
            List<Worker> workers = reader.ReadObjectList<Worker>();
            _avaiableWorkers = new LinkedList<Worker>(workers);            
		}

        public void AfterReadStateV1()
        {
        }
        #endregion
    }
}
