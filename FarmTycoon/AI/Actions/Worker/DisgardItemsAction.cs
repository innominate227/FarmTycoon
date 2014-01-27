using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Worker disgards all items in their posetion (into the abyss)
    /// </summary>
    public partial class DisgardItemsAction : OneLocationAction<Worker>
    {
        #region Member Vars

        /// <summary>
        /// Where the worker should be when the items are disguarded
        /// </summary>
        private Location _disgruardAt;

        /// <summary>
        /// List of items to disguard
        /// </summary>
        private ItemList _toDisguard;

        /// <summary>
        /// True to ignore the to disguard list and disguard all items
        /// </summary>
        private bool _diguardAll = false;

        #endregion

        #region Setup

        /// <summary>
        /// Constructor used for reading object from state
        /// </summary>
        public DisgardItemsAction() { }
        
        /// <summary>
        /// Create a new disguard items action
        /// </summary>
        public DisgardItemsAction(Location disguardAt, ItemList toDisguard)
        {
            _disgruardAt = disguardAt;
            _toDisguard = toDisguard;
            _diguardAll = false;
        }

        /// <summary>
        /// Create a new disguard items action
        /// </summary>
        public DisgardItemsAction(Location disguardAt)
        {
            _disgruardAt = disguardAt;
            _toDisguard = new ItemList();
            _diguardAll = true;
        }

        #endregion

        #region Propeties

        /// <summary>
        /// Where the worker should be when the items are disguarded
        /// </summary>
        public Location DisgruardAt
        {
            get { return _disgruardAt; }
        }

        /// <summary>
        /// List of items to disguard
        /// </summary>
        public ItemList ToDisguard
        {
            get { return _toDisguard; }
        }

        #endregion

        #region Logic

        public override Location TheLocation()
        {
            return _disgruardAt;
        }

        public override double GetActionTime(DelaySet delaySet)
        {
            return 0.0;
        }

        /// <summary>
        /// We arrived at the location where we will do the action
        /// </summary>
        public override void ArrivedAtAction()
        {
            //set action textures on objects
            _actor.SetTextureForActionOrEvent(ActionOrEventType.DisguardItems);
        }

        public override void DoAction()
        {
            //get the list of what to disguard, either everything or the toDisguard list
            ItemList disguardList = new ItemList();
            if (_diguardAll)
            {
                disguardList.AddItems(_actor.Inventory.UnderlyingList);
            }
            else
            {
                disguardList.AddItems(_toDisguard);
            }

            //disguard each item in the list
            foreach (ItemType itemType in disguardList.ItemTypes)
            {
                int amountToDisguard = disguardList.GetItemCount(itemType);

                //make sure worker has that much
                int amountWorkerHas = _actor.Inventory.GetTypeCount(itemType);
                Debug.Assert(amountToDisguard >= amountWorkerHas);

                //remove the amount for the workers inventory
                _actor.Inventory.RemoveFromInvetory(itemType, amountToDisguard);


                //if the item has an object attached we might need to do something special with it
                if (itemType.ItemObject != null)
                {
                    if (itemType.ItemObject is Animal)
                    {
                        //if we are disguarding an animal, have the animal stop following the worker, and delete it
                        Animal animalDisgaurding = (Animal)itemType.ItemObject;
                        animalDisgaurding.StopFollowing();
                        animalDisgaurding.Delete();
                    }
                    else if (itemType.ItemObject is Equipment)
                    {
                        //if we are disguarding equipment, have the worker get off it and delete it
                        Equipment equipmentDisgaurding = (Equipment)itemType.ItemObject;
                        _actor.GetOffEquipment(equipmentDisgaurding);
                        equipmentDisgaurding.Delete();
                    }
                }
            }
            
            //apply the action to traits
            _actor.ApplyActionOrEventToTraits(ActionOrEventType.DisguardItems);

            //clear action textures on objects
            _actor.ClearTextureForActionOrEvent();
        }
              
        public override bool IsObjectInvolved(IGameObject obj)
        {
            return false;
        }

        public override string Description()
        {   
            return "";                        
        }

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteObject(_disgruardAt);
            writer.WriteObject(_toDisguard);
            writer.WriteBool(_diguardAll);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _disgruardAt = reader.ReadObject<Location>();
            _toDisguard = reader.ReadObject<ItemList>();
            _diguardAll = reader.ReadBool();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion

    }
}
