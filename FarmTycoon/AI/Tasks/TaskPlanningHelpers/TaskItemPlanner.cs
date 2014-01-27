using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{

    public delegate bool GameObjectPredicate<T>(T obj) where T : IGameObject;


    /// <summary>
    /// Assits in planning a task by determining where items can be found and taken to.  
    /// Keeps track of what previous actions in the task will be putting/taking items so that it will not plan to take items from a builidng that will not have them.
    /// </summary>
    public class TaskItemPlanner
    {
        /// <summary>
        /// Mapping from normal game inventories to the expected inventories.
        /// Expected inventories are created based on what the planner has previously planned to get/put in places
        /// </summary>
        private Dictionary<Inventory, ExpectedInventory> _expectedInventories = new Dictionary<Inventory, ExpectedInventory>();

        
        /// <summary>
        /// Add actions to the TaskPlan for the worker, workerNum, to get the items in the itemList passed, near to the land passed.
        /// Storage buildings will be tested against the default predicate (it will not put into DeliveryArea storage buildings, or into trough storage buildings)
        /// As many items as possible will be gotten from the closest building, then if all cannot be gotten from that building the next closest.
        /// Buildings will be visited in order of the furthest away first followed by the next closest, etc.
        /// If it can not find somewhere to get the items it adds an issue into the task plan.
        /// </summary>
        public void PlanToGetItems(TaskPlan plan, int workerNum, ItemList itemsToGet, Location near)
        {
            PlanToGetItems(plan, workerNum, itemsToGet, near,
                delegate(IStorageBuilding building)
                {
                    //dont get it from the delivery area
                    if (building is DeliveryArea) { return false; }

                    //dont get it from a trough
                    if (building is Trough) { return false; }

                    //other building types are ok
                    return true;
                });

        }

        /// <summary>
        /// Add actions to the TaskPlan for the worker, workerNum, to get the items in the itemList passed, near to the land passed.
        /// Storage buildings will be tested to make sure they meet the predicate passed 
        /// As many items as possible will be gotten from the closest building, then if all cannot be gotten from that building the next closest.
        /// Buildings will be visited in order of the furthest away first followed by the next closest, etc.
        /// If it can not find somewhere to get the items it adds an issue into the task plan.
        /// </summary>
        public void PlanToGetItems(TaskPlan plan, int workerNum, ItemList itemsToGet, Location near, GameObjectPredicate<IStorageBuilding> storageBuildingPredicate)
        {
            //the items to get at each builing, we will be getting items into
            Dictionary<IStorageBuilding, ItemList> itemsToGetFromBuildings = new Dictionary<IStorageBuilding, ItemList>();

            //find a place or places to get each item from
            foreach (ItemType itemType in itemsToGet.ItemTypes)
            {
                //find nearby storage buildings that have the item
                List<IStorageBuilding> buildingsThatHaveItem = GameState.Current.MasterObjectList.FindClosestObjectsMeetingPredicate<IStorageBuilding>(near,
                    delegate(IStorageBuilding building)
                    {
                        //dont get it from buildings that dont meet the predicate
                        if (storageBuildingPredicate(building) == false) { return false; }

                        //create expected inventory for the building if we dont already have one
                        if (_expectedInventories.ContainsKey(building.Inventory) == false)
                        {
                            _expectedInventories.Add(building.Inventory, new ExpectedInventory(building.Inventory));
                        }

                        //return true if we the expected inventory has at least one of that item
                        return _expectedInventories[building.Inventory].GetTypeCountThatsFree(itemType) > 0;
                    });


                //get items from the closest buildings until all have been gotten
                int leftToGet = itemsToGet.GetItemCount(itemType);
                foreach (IStorageBuilding buildingThatHasItem in buildingsThatHaveItem)
                {
                    //get as much as there is there
                    int amountToGetFromThatBuilding = _expectedInventories[buildingThatHasItem.Inventory].GetTypeCountThatsFree(itemType);

                    //or if more than the amount left to get, get the amount left to get
                    if (amountToGetFromThatBuilding > leftToGet) { amountToGetFromThatBuilding = leftToGet; }

                    //create a list of items to get from that building if one does not exsist
                    if (itemsToGetFromBuildings.ContainsKey(buildingThatHasItem) == false)
                    {
                        itemsToGetFromBuildings.Add(buildingThatHasItem, new ItemList());
                    }

                    //add items to the list of items to get from that building
                    itemsToGetFromBuildings[buildingThatHasItem].IncreaseItemCount(itemType, amountToGetFromThatBuilding);

                    //tell the expected inventory that we plan to get the items from it
                    _expectedInventories[buildingThatHasItem.Inventory].PlanToRemove(itemType, amountToGetFromThatBuilding);

                    //that much less to get
                    leftToGet -= amountToGetFromThatBuilding;
                    if (leftToGet == 0) { break; }
                }

                //if we didnt manage to get all the items from somewhere
                if (leftToGet > 0)
                {                    
                    plan.AddMissingItemsIssue(itemType, leftToGet);
                }
            }

            //create a list of buildings to get items from sorted by distance to "near"
            List<IStorageBuilding> buildingsToGetItemsFrom = new List<IStorageBuilding>(itemsToGetFromBuildings.Keys);
            buildingsToGetItemsFrom = GameState.Current.MasterObjectList.SortObjectsByDistance<IStorageBuilding>(near, buildingsToGetItemsFrom);

            //create an action to get the items from each building (in reverse order, we want to get the closest items first)
            foreach (IStorageBuilding buildingToGetItemsFrom in buildingsToGetItemsFrom.Reverse<IStorageBuilding>())
            {
                ItemList itemsToGetFromTheBuilding = itemsToGetFromBuildings[buildingToGetItemsFrom];

                //create action to get the items from the building
                GetItemsAction getItemsAction = new GetItemsAction(buildingToGetItemsFrom, itemsToGetFromTheBuilding);
                plan.AddAction(workerNum, getItemsAction);
            }
        }

        

        /// <summary>
        /// Add actions to the TaskPlan for the worker, workerNum, to put the items in the itemList passed into buildings near to the land passed.
        /// Storage buildings will be tested against the default predicate (it will not put into DeliveryArea storage buildings, or into trough storage buildings)
        /// All items that can fit in the closest building will be placed into that building, then if items reamain all items will be placed in the next closest building, etc.
        /// If it can find somewhere to put all items it adds an issue into the task plan.
        /// </summary>
        public void PlanToPutItems(TaskPlan plan, int workerNum, ItemList itemsToPut, bool allowSubsitutions, Location near)
        {
            PlanToPutItems(plan, workerNum, itemsToPut, allowSubsitutions, near,
                delegate(IStorageBuilding building)
                {
                    //dont put it into the delivery area
                    if (building is DeliveryArea) { return false; }

                    //dont put it into a trough
                    if (building is Trough) { return false; }

                    //other building types are ok
                    return true;
                });
        }

        /// <summary>
        /// Add actions to the TaskPlan for the worker, workerNum, to put the items in the itemList passed into buildings near to the land passed.
        /// Storage buildings will be tested to make sure they meet the predicate passed 
        /// All items that can fit in the closest building will be placed into that building, then if items reamain all items will be placed in the next closest building, etc.
        /// If it can find somewhere to put all items it adds an issue into the task plan.
        /// </summary>
        public void PlanToPutItems(TaskPlan plan, int workerNum, ItemList itemsToPut, bool allowSubsitutions, Location near, GameObjectPredicate<IStorageBuilding> storageBuildingPredicate)
        {
            PlanToPutItemsInner(plan, workerNum, null, itemsToPut, allowSubsitutions, near, storageBuildingPredicate);
        }
        

        /// <summary>
        /// Add actions to the action sequence to put the items in the itemList passed into buildings near to the land passed.
        /// All items that can fit in the closest building will be placed into that building, then if items reamain all items will be placed in the next closest building, etc.        
        /// </summary>
        public void PlanToPutItems(ActionSequence<Worker> actionSequence, ItemList itemsToPut, bool allowSubsitutions, Location near)
        {
            PlanToPutItemsInner(null, 0, actionSequence, itemsToPut, allowSubsitutions, near, delegate(IStorageBuilding building)
                {
                    //dont put it into the delivery area
                    if (building is DeliveryArea) { return false; }

                    //dont put it into a trough
                    if (building is Trough) { return false; }

                    //other building types are ok
                    return true;
                });
        }


        /// <summary>
        /// Used by the two PlanToPutItem methods.  either plan or action sequence will be null
        /// </summary>
        private void PlanToPutItemsInner(TaskPlan plan, int workerNum, ActionSequence<Worker> actionSequence, ItemList itemsToPut, bool allowSubsitutions, Location near, GameObjectPredicate<IStorageBuilding> storageBuildingPredicate)
        {
            //the items to put at each builing, we will be putting items into
            Dictionary<IStorageBuilding, ItemList> itemsToPutInBuildings = new Dictionary<IStorageBuilding, ItemList>();

            //find a place or places to take each item type
            foreach (ItemType itemType in itemsToPut.ItemTypes)
            {

                //find nearby storage buildings that can hold at least one of that type of item
                List<IStorageBuilding> buildingsThatCanHoldItem = GameState.Current.MasterObjectList.FindClosestObjectsMeetingPredicate<IStorageBuilding>(near,
                    delegate(IStorageBuilding building)
                    {
                        //dont put it into the storage building if it does not meet the predicate
                        if (storageBuildingPredicate(building) == false) { return false; }

                        //create expected inventory for the building if we dont already have one
                        if (_expectedInventories.ContainsKey(building.Inventory) == false)
                        {
                            _expectedInventories.Add(building.Inventory, new ExpectedInventory(building.Inventory));
                        }

                        //return true if we the expected inventory can hold at least one more item
                        return _expectedInventories[building.Inventory].AmountThatWillFitAfterReservedCapacity(itemType) > 0;
                    });


                //put items in the closest buildings until all have been put
                int leftToPut = itemsToPut.GetItemCount(itemType);
                foreach (IStorageBuilding buildingThatCanHoldItem in buildingsThatCanHoldItem)
                {
                    //put as much as will fit in there
                    int amountToPutInThatBuilding = _expectedInventories[buildingThatCanHoldItem.Inventory].AmountThatWillFitAfterReservedCapacity(itemType);

                    //or if more than the amount left to put, put the amount left to put
                    if (amountToPutInThatBuilding > leftToPut) { amountToPutInThatBuilding = leftToPut; }

                    //create a list of items to put in that building if one does not exsist
                    if (itemsToPutInBuildings.ContainsKey(buildingThatCanHoldItem) == false)
                    {
                        itemsToPutInBuildings.Add(buildingThatCanHoldItem, new ItemList());
                    }

                    //add items to the list of items to put in that building
                    itemsToPutInBuildings[buildingThatCanHoldItem].IncreaseItemCount(itemType, amountToPutInThatBuilding);

                    //tell the expected inventory that we plan to put the items into it
                    _expectedInventories[buildingThatCanHoldItem.Inventory].PlanToAdd(itemType, amountToPutInThatBuilding);

                    //that must less to put
                    leftToPut -= amountToPutInThatBuilding;
                    if (leftToPut == 0) { break; }
                }

                //if we didnt manage to put all the items somewhere
                if (leftToPut > 0 && plan != null)
                {
                    plan.AddMissingSpaceIssue(itemType, leftToPut);
                }
            }

            //create a list of buildings to put items in sorted by distance to "near"
            List<IStorageBuilding> buildingsToPutItemsIn = new List<IStorageBuilding>(itemsToPutInBuildings.Keys);
            buildingsToPutItemsIn = GameState.Current.MasterObjectList.SortObjectsByDistance<IStorageBuilding>(near, buildingsToPutItemsIn);

            //create an action to put the items into each building
            foreach (IStorageBuilding buildingToPutItemsIn in buildingsToPutItemsIn)
            {
                ItemList itemsToPutInTheBuilding = itemsToPutInBuildings[buildingToPutItemsIn];

                //create action to put items into the building
                PutItemsAction putItemsAction = new PutItemsAction(buildingToPutItemsIn, itemsToPutInTheBuilding, allowSubsitutions);
                if (plan != null)
                {
                    plan.AddAction(workerNum, putItemsAction);
                }
                else
                {
                    actionSequence.AddAction(putItemsAction);
                }
            }

        }

        
        /// <summary>
        /// Add an actions to the TaskPlan for the worker, workerNum, to get equipment of the type passed from a storage building near to the land passed.        
        /// If it can not find somewhere to get the equpment it adds an issue into the task plan.
        /// Return the Action used to get the equipment if it was found.
        /// </summary>
        public GetItemsAction PlanToGetEquipmentItem(TaskPlan plan, int workerNum, EquipmentType equipmentType, Location near)
        {
            return PlanToGetEquipmentItem(plan, workerNum, new List<EquipmentType>() { equipmentType }, near);
        }


        /// <summary>
        /// Add an actions to the TaskPlan for the worker, workerNum, to get equipment of any on of the types passed from a storage building near to the land passed.        
        /// If it can not find somewhere to get the equpment it adds an issue into the task plan.
        /// Return the Action used to get the equipment if it was found.
        /// </summary>
        public GetItemsAction PlanToGetEquipmentItem(TaskPlan plan, int workerNum, List<EquipmentType> equipmentTypes, Location near)
        {
            //create a list of item type infos for the type of equipment we want to get
            List<ItemTypeInfo> itemTypeInfosForEquipment = new List<ItemTypeInfo>();
            foreach (EquipmentType equipmentType in equipmentTypes)
            {
                foreach (EquipmentInfo equipmentInfo in FarmData.Current.GetEquipmentOfType(equipmentType))
                {
                    itemTypeInfosForEquipment.Add(equipmentInfo.ItemTypeInfo);
                }
            }

            //this gets set in the delegate below to the item type we found that was of that equipment type
            ItemType equipmentTypeInTheBuilding = null;

            //find a nearby storage buildings that has the item
            IStorageBuilding buildingThatHasItem = GameState.Current.MasterObjectList.FindClosestObjectMeetingPredicate<IStorageBuilding>(near,
                delegate(IStorageBuilding building)
                {
                    //dont get it from the delivery area
                    if (building is DeliveryArea) { return false; }

                    //increase speed by checking if this building is even able to hold Equipment
                    bool buildingCanHoldEquipmnet = false;
                    foreach(ItemTypeInfo equipmentItemTypeInfo in itemTypeInfosForEquipment)
                    {
                        if (building.Inventory.InventoryInfo.AllowedTypes.Contains(equipmentItemTypeInfo))
                        {
                            buildingCanHoldEquipmnet = true;
                        }
                    }
                    if (buildingCanHoldEquipmnet == false) { return false; }
                    
                    //create expected inventory for the building if we dont already have one
                    if (_expectedInventories.ContainsKey(building.Inventory) == false)
                    {
                        _expectedInventories.Add(building.Inventory, new ExpectedInventory(building.Inventory));
                    }

                    //look at all the types available in the building and see if any are equipment and have the equipment type passed
                    foreach(ItemType availableItemType in _expectedInventories[building.Inventory].TypesAvailable())
                    {
                        //if its ItemTypeInfo is one of the ones we are looking for for that equipment type then we found a good building
                        if (itemTypeInfosForEquipment.Contains(availableItemType.BaseType))
                        {
                            equipmentTypeInTheBuilding = availableItemType;
                            return true;
                        }
                    }

                    //building did not have any equipment
                    return false;
                });


            //make sure we found a building
            if (buildingThatHasItem != null)
            {
                //tell the expected inventory that we plan to get some equipment from it
                _expectedInventories[buildingThatHasItem.Inventory].PlanToRemove(equipmentTypeInTheBuilding, 1);

                //create action to get the equipment
                ItemList equipmentToGet = new ItemList();
                equipmentToGet.IncreaseItemCount(equipmentTypeInTheBuilding, 1);
                GetItemsAction getEquipmentAction = new GetItemsAction(buildingThatHasItem, equipmentToGet);
                plan.AddAction(workerNum, getEquipmentAction);
                return getEquipmentAction;
            }
            else
            {                
                plan.AddMissingEquipmentIssue(equipmentTypes[0], 1);
                return null;
            }
        }




    }
}
