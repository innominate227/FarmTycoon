using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// Assits in planning a task by keeping tack where equipment was gotten from, and making it easy to put back all equipment a worker has gotten so far in the task.      
    /// Uses the TaskItemPlanner to determine where to get equipment.
    /// This should only be used if you get equipmnet with the intention to use it and return it.  
    /// If you are getting it to sell it (or not putting it back for some other reason use task item planner directly).
    /// </summary>
    public class TaskEquipmentPlanner
    {

        /// <summary>
        /// Item planner used to find places to get the equipment from
        /// </summary>
        private TaskItemPlanner _itemPlanner;

        /// <summary>        
        /// for each worker number the action the worker used to get a tractor.  
        /// If the worker has not planned to get a tractor yet then this will not have an entry for that worker num.
        /// </summary>
        private Dictionary<int, GetItemsAction> _getVehicleAction = new Dictionary<int, GetItemsAction>();

        /// <summary>        
        /// for each worker number the action the worker used to get a tow.  
        /// If the worker has not planned to get a tow yet then this will not have an entry for that worker num.
        /// </summary>
        private Dictionary<int, GetItemsAction> _getTowAction = new Dictionary<int, GetItemsAction>();


        /// <summary>
        /// Create a Task equpment planner, that uses the task item planner passed to plan where to get equipment from
        /// </summary>
        public TaskEquipmentPlanner(TaskItemPlanner itemPlanner)
        {
            _itemPlanner = itemPlanner;
        }

        /// <summary>
        /// The equipment info for the type of tractor worker workerNUm is expected to have at this point in the planning
        /// </summary>
        private EquipmentInfo CurrentExpectedVehicle(int workerNum)
        {
            if (_getVehicleAction.ContainsKey(workerNum) == false) { return null; }
            ItemType equipmentItemType = _getVehicleAction[workerNum].GetList.ItemTypes[0];
            return FarmData.Current.GetEquipmentInfoForItemInfo(equipmentItemType.BaseType);
        }

        /// <summary>
        /// The equipment info for the type of tow worker workerNUm is expected to have at this point in the planning
        /// </summary>
        private EquipmentInfo CurrentExpectedTow(int workerNum)
        {
            if (_getTowAction.ContainsKey(workerNum) == false) { return null; }
            ItemType equipmentItemType = _getTowAction[workerNum].GetList.ItemTypes[0];
            return FarmData.Current.GetEquipmentInfoForItemInfo(equipmentItemType.BaseType);
        }

        /// <summary>
        /// The expected capacity based on the equipmnet the worker is expected to have at this point
        /// </summary>
        public int CurrentExpectedCapacity(int workerNum)
        {
            int baseCapacity = (FarmData.Current.GetInfo(WorkerInfo.UNIQUE_NAME) as WorkerInfo).Capacity;

            double multiplier = 1.0;
            EquipmentInfo expectedVehicle =  CurrentExpectedVehicle(workerNum);            
            if (expectedVehicle != null)
            {
                multiplier *= expectedVehicle.InventorySizeMultiplier;
            }
            EquipmentInfo expectedTow = CurrentExpectedTow(workerNum);
            if (expectedTow != null)
            {
                multiplier *= expectedTow.InventorySizeMultiplier;
            }

            return (int)(baseCapacity * multiplier);
        }



        /// <summary>
        /// Add actions to the TaskPlan for the worker, workerNum, to get any vehicle equipment type passed from a storage building, near to the land passed.
        /// If the worker will already has any type of vehicle at this point the method does nothing.
        /// If it can not find somewhere to get a vehicle it adds an issue into the task plan.
        /// It keeps track of where the worker got the equipment so they can put it back.
        /// </summary>
        public void PlanToGetVehicleIfNeeded(TaskPlan plan, int workerNum, Location near)
        {
            //see if the worker will already has a vehicle
            if (CurrentExpectedVehicle(workerNum) != null)
            {
                return;
            }
            
            //we do need to get the equipment
            //use itemPlanner to plan where to get it
            GetItemsAction getEquipmentAction = _itemPlanner.PlanToGetEquipmentItem(plan, workerNum, EquipmentTypeUtils.AllVehicleTypes, near);

            //remeber the action we used to get the equipment
            if (getEquipmentAction != null)
            {                
                _getVehicleAction.Add(workerNum, getEquipmentAction);                
            }
        }


        /// <summary>
        /// Add actions to the TaskPlan for the worker, workerNum, to get the equipment of the type passed from a storage building, near to the land passed.
        /// If the worker will already has that equipment at this point the method does nothing.
        /// If it can not find somewhere to get the equpment it adds an issue into the task plan.
        /// It keeps track of where the worker got the equipment so they can put it back.
        /// </summary>
        public void PlanToGetEquipmentIfNeeded(TaskPlan plan, int workerNum, EquipmentType equipmentType, Location near)
        {            
            if (EquipmentTypeUtils.IsVehicle(equipmentType))
            {
                //see if the worker will already has that vehicle
                if (CurrentExpectedVehicle(workerNum) != null && CurrentExpectedVehicle(workerNum).EquipmentType == equipmentType)
                {
                    return;
                }

                //if they are expected to have another type of vehicle already they cant get this type
                Debug.Assert(CurrentExpectedVehicle(workerNum) == null);
            }
            else
            {
                //see if the worker will already has that tow
                if (CurrentExpectedTow(workerNum) != null && CurrentExpectedTow(workerNum).EquipmentType == equipmentType)
                {
                    return;
                }

                //if they are expected to have another type of tow already they cant get this type
                Debug.Assert(CurrentExpectedTow(workerNum) == null);
            }
            
            //we do need to get the equipment
            //use itemPlanner to plan where to get it
            GetItemsAction getEquipmentAction = _itemPlanner.PlanToGetEquipmentItem(plan, workerNum, equipmentType, near);

            //remeber the action we used to get the equipment
            if (getEquipmentAction != null)
            {
                if (EquipmentTypeUtils.IsVehicle(equipmentType))
                {
                    _getVehicleAction.Add(workerNum, getEquipmentAction);
                }
                else
                {
                    _getTowAction.Add(workerNum, getEquipmentAction);
                }
            }
        }


        /// <summary>
        /// For each worker:
        /// Plan to put back both the tow (if the worker has one)
        /// And to put back the tracotor (if the worker has one)
        /// </summary>
        public void PlanToPutAllEquipmentBack(TaskPlan plan)
        {
            for (int workerNum = 0; workerNum < plan.Task.NumberOfWorkers; workerNum++)
            {
                PlanToPutEquipmentBack(plan, workerNum);
            }            
        }
        
        /// <summary>
        /// Plan to put back both the tow (if the worker has one)
        /// And to put back the tracotor (if the worker has one)
        /// </summary>
        public void PlanToPutEquipmentBack(TaskPlan plan, int workerNum)
        {
            PlanToPutTowBack(plan, workerNum);
            PlanToPutVehicleBack(plan, workerNum);
        }

        /// <summary>
        /// Add actions to the TaskPlan for the worker to put the vehicle back where he got it from.
        /// If the worker will not have a vehicle at this point this does nothing
        /// </summary>
        public void PlanToPutVehicleBack(TaskPlan plan, int workerNum)
        {
            //if we never got a vehicle do nothing
            if (_getVehicleAction.ContainsKey(workerNum) == false)
            {
                return;
            }

            //get the action we used to get the vehicle
            GetItemsAction actionThatGotVehicle = _getVehicleAction[workerNum];

            //create an action to put the vehicle back
            PutItemsAction putVehicleBack = new PutItemsAction(actionThatGotVehicle);
            plan.AddAction(workerNum, putVehicleBack);

            //the worker no longer has a vehicle
            _getVehicleAction.Remove(workerNum);
        }
        
        /// <summary>
        /// Add actions to the TaskPlan for the worker to put the tow back where he got it from.
        /// If the worker will not have a tow at this point this does nothing
        /// </summary>
        public void PlanToPutTowBack(TaskPlan plan, int workerNum)
        {
            //if we never got a tow do nothing
            if (_getTowAction.ContainsKey(workerNum) == false)
            {
                return;
            }

            //get the action we used to get the tow
            GetItemsAction actionThatGotTow = _getTowAction[workerNum];

            //create an action to put the tow back
            PutItemsAction putTowBack = new PutItemsAction(actionThatGotTow);
            plan.AddAction(workerNum, putTowBack);

            //the worker no longer has a tow
            _getTowAction.Remove(workerNum);
        }



        


    }
}
