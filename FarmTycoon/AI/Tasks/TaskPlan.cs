using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// A plan for how to do a task.  
    /// Consits of a list of ActionSequences for each worker working on the task.
    /// Or contains a list of issues that prevent the task from being done.
    /// </summary>
    public class TaskPlan
    {
        /// <summary>
        /// List of action seqeuences for the plan.  There will be the same number of seqeuences as there are workers assigned to the task.        
        /// </summary>
        private List<ActionSequence<Worker>> _actionSequences = new List<ActionSequence<Worker>>();

        /// <summary>
        /// List of issues that prevent the task from being completed.
        /// These are issues that are not realted to lack of items which are tracked seperatly below
        /// </summary>
        private List<string> _otherIssues = new List<string>();

        /// <summary>
        /// Items that we are lacking that prevent us from starting the task and the amoun that is lacking.
        /// Keyed on the name of the item thats missing
        /// </summary>
        private Dictionary<string, int> _lackingItems = new Dictionary<string, int>();
        
        /// <summary>
        /// Items that can not be placed and are lacking that prevent us from starting the task
        /// Keyed on the name of the item that needs space
        /// </summary>
        private Dictionary<string, int> _lackingSpace = new Dictionary<string, int>();

        /// <summary>
        /// List of warnings that the player should know about, but that dont prevent the task from being completed.
        /// </summary>
        private List<string> _warnings = new List<string>();

        /// <summary>
        /// Set to true if there was an issue that will cause the expected time calculated to be inacurate.        
        /// </summary>
        private bool _issuePreventsExpectedTimeCalculation = false;

        /// <summary>
        /// The task this plan is for
        /// </summary>
        private Task _task;
        
        
        /// <summary>
        /// Create a task plan for the task passed
        /// </summary>        
        public TaskPlan(Task task) : this(task, 0)
        {
        }

        /// <summary>
        /// Create a task plan for the task passed
        /// </summary>        
        public TaskPlan(Task task, int extraDelay)
        {
            _task = task;

            //to prevent all workers from starting at the same time (and being right on top of each other)
            //we will first add a Delay to the action sequence equal to the worker number
            //the value below will cause the works to be about a square apart (depending on terian type, and energy level) when they start
            double actionStartDelay = FarmData.Current.WorkerInfo.Delays.GetDelayInfo(ActionOrEventType.Move).MaximumValue * 16;

            //create an action sequence for each worker            
            for (int workerNum = 0; workerNum < task.NumberOfWorkers; workerNum++)
            {
                ActionSequence<Worker> actionSequenceForWorker = new ActionSequence<Worker>();
                actionSequenceForWorker.AddAction(new DelayAction((workerNum + extraDelay) * actionStartDelay));
                _actionSequences.Add(actionSequenceForWorker);
            }
        }


        /// <summary>
        /// The task this plan is for
        /// </summary>
        public Task Task
        {
            get { return _task; }
        }
        

        /// <summary>
        /// Return all the action seqeucnes in the plan.
        /// </summary>
        public IList<ActionSequence<Worker>> ActionSequences
        {
            get { return _actionSequences.AsReadOnly(); }
        }

        /// <summary>
        /// Add an action to the plan.
        /// </summary>
        public void AddAction(int workerToDoAction, ActionBase<Worker> action)
        {
            action.Task = _task;
            _actionSequences[workerToDoAction].AddAction(action);
        }

        /// <summary>
        /// Add an issue to the task plan, and optionaly set that we stopped trying to create the plan (becayse the issue prevent planning further)
        /// </summary>
        public void AddIssue(string issue, bool issuePreventsExpectedTimeCalculation)
        {
            _otherIssues.Add(issue);
            if (issuePreventsExpectedTimeCalculation)
            {
                _issuePreventsExpectedTimeCalculation = true;
            }
        }

        /// <summary>
        /// Add an issue that an not enough of an item can be found.  This will always set the unable to estimate time flag
        /// </summary>
        public void AddMissingItemsIssue(ItemType item, int amountNeeded)
        {
            if (_lackingItems.ContainsKey(item.BaseName) == false)
            {
                _lackingItems.Add(item.BaseName, 0);
            }
            _lackingItems[item.BaseName] += amountNeeded;
            _issuePreventsExpectedTimeCalculation = true;
        }

        /// <summary>
        /// Add an issue that not enough of a type of equipmnet can be found.  This will always set the unable to estimate time flag
        /// </summary>
        public void AddMissingEquipmentIssue(EquipmentType equipmentType, int amountNeeded)
        {
            if (_lackingItems.ContainsKey(equipmentType.ToString()) == false)
            {
                _lackingItems.Add(equipmentType.ToString(), 0);
            }
            _lackingItems[equipmentType.ToString()] += amountNeeded;
            _issuePreventsExpectedTimeCalculation = true;
        }

        /// <summary>
        /// Add an issue that an not enough space could be found to store an item.  This will always set the unable to estimate time flag
        /// </summary>
        public void AddMissingSpaceIssue(ItemType item, int amountNeededToBePlaced)
        {
            if (_lackingSpace.ContainsKey(item.BaseName) == false)
            {
                _lackingSpace.Add(item.BaseName, 0);
            }
            _lackingSpace[item.BaseName] += amountNeededToBePlaced;
            _issuePreventsExpectedTimeCalculation = true;
        }
        
        /// <summary>
        /// Add a waring to the task plan.  A warning is something the user should know but does not prevent the task form being completed.
        /// </summary>
        public void AddWarning(string warning)
        {
            _warnings.Add(warning);
        }

        /// <summary>
        /// Return if the plan succeeded (there were no issues)
        /// </summary>        
        public bool PlanSucceeded
        {
            get { return _otherIssues.Count == 0 && _lackingItems.Count == 0 && _lackingSpace.Count == 0; }
        }

        /// <summary>
        /// Get all the issues that ocurred when planning the task as a string
        /// </summary>
        public string IssuesString()
        {
            StringBuilder issuesString = new StringBuilder();
            foreach (string typeMissing in _lackingItems.Keys)
            {
                int amountMissing = _lackingItems[typeMissing];
                string s = "s"; if (amountMissing == 1) { s = ""; }
                issuesString.Append("Need " + amountMissing.ToString() + " more " + typeMissing + s + ".");
                issuesString.Append(" ");
            }
            foreach (string typeNeedsSpace in _lackingSpace.Keys)
            {
                int amountOfSpaceNeeded = _lackingSpace[typeNeedsSpace];
                string s = "s"; if (amountOfSpaceNeeded == 1) { s = ""; }
                issuesString.Append("Need space for " + amountOfSpaceNeeded.ToString() + " more " + typeNeedsSpace + s + ".");
                issuesString.Append(" ");
            }
            foreach (string issue in _otherIssues)
            {
                issuesString.Append(issue);
                issuesString.Append(" ");
            }
            if (issuesString.Length > 0)
            {
                issuesString.Append("Task will be started as soon as issues are resolved.");
            }
            return issuesString.ToString().Trim();
        }

        /// <summary>
        /// Get all the warnings for the task as a string
        /// </summary>
        public string WarningsString()
        {
            StringBuilder warningsString = new StringBuilder();
            foreach (string warning in _warnings)
            {
                warningsString.Append(warning);
                warningsString.Append(" ");
            }
            return warningsString.ToString().Trim();
        }

        public bool CanCalculateExpectedTime
        {
            get
            {
                return _issuePreventsExpectedTimeCalculation == false;
            }
        }

        /// <summary>
        /// Return the time it is expected to take to complete the task according to this task plan (int number of days)
        /// Will return -1 if the expected time cannot be determined
        /// </summary>
        public int CalculateExpectedTime()
        {
            //if there was an issue that made it so we cant do expected time then return -1
            if (_issuePreventsExpectedTimeCalculation) { return -1; }
            
            //determine the max time of all the action sequences
            double maxExpectedTime = 0;
            foreach (ActionSequence<Worker> actionSequence in _actionSequences)
            {
                double expectedTimeForActionSequence = actionSequence.ExpectedTime();
                if (expectedTimeForActionSequence > maxExpectedTime)
                {
                    maxExpectedTime = expectedTimeForActionSequence;
                }
                //enfore maximum expected time of 360 days
                if (maxExpectedTime >= 360) { maxExpectedTime = 360; break; }
            }                        
            return (int)Math.Ceiling(maxExpectedTime);
        }

    }
}
