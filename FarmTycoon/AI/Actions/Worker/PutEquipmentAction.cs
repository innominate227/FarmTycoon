using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// Action to put equipment into a storgae building.  Action needs to be told what is going to be moved so it can reserve space in the building.    
    /// </summary>
    public class PutEquipmentAction : OneLocationAction<Worker>
    {        
        /// <summary>
        /// Equipmnet to put in the builidng
        /// </summary>
        private EquipmentType m_whatToPut;

        /// <summary>
        /// Storage building to move the items to
        /// </summary>
        private IStorageBuilding m_storageBuilding;

        /// <summary>
        /// If we are getting and putting equipment in the same action then this is the action where we got the equipment.
        /// We need to do things slightly different when putting back something we got earlier in the same action.  
        /// We dont want to reserve space until the get action is finished.
        /// </summary>
        private GetEquipmentAction m_corespondingGetEquipmentAction = null;

        public PutEquipmentAction() { }
        
        /// <summary>
        /// Create a new put equipment action to put equipmnet into the storage building passed.
        /// </summary>
        public PutEquipmentAction(IStorageBuilding storageBuilding, EquipmentType whatToPut)
        {
            m_corespondingGetEquipmentAction = null;
            m_storageBuilding = storageBuilding;
            m_whatToPut = whatToPut;
        }

        /// <summary>
        /// Create a new put equipment action to put equipmnet back that was gotten this same task
        /// </summary>
        public PutEquipmentAction(GetEquipmentAction actionEquipmentWasGottenDuring)
        {
            m_corespondingGetEquipmentAction = actionEquipmentWasGottenDuring;
            m_corespondingGetEquipmentAction.SetCorespondingPutEquipmentAction(this);
            m_storageBuilding = actionEquipmentWasGottenDuring.StorageBuilding;
            m_whatToPut = actionEquipmentWasGottenDuring.WhatToGet;
        }


        /// <summary>
        /// Equipmnet to put in the builidng
        /// </summary>
        public EquipmentType WhatToPut
        {
            get { return m_whatToPut; }
        }


        /// <summary>
        /// Called by the CorespondingGetEquipmentAction when it gets the equipment that this action will be putting back.
        /// This should only be called by GetEquipmnetAction 
        /// </summary>
        public void CorespondingGetEquipmentActionGotEquipment()
        {
            //reserve space to put the equipment back now
            ReserveSpace();
        }


        public override Location TheLocation()
        {
            return m_storageBuilding.ActionLocation;
        }

        public override double GetActionTime(double actionDelayMultiplier)
        {
            //for now always takes the same amount of time to get items no matter how much the worker is getting
            return 1.0 / 24.0;
        }


        public override void DoAction()
        {
            //the actor must be a worker if this is an equipment action
            Worker worker = (Worker)m_actor;

            //make sure the worker has that equipment
            Debug.Assert(worker.HasEquipment(m_whatToPut));

            //have the worker get off the equipmnet
            worker.GetOffEquipment(m_whatToPut);

            //put the equipment into the building
            m_storageBuilding.Inventory.AddToInvetory(m_whatToPut.ItemType, 1);
        }


        #region Free and Reserve Space


        protected override void AfterAssigned()
        {
            //dont imediantly reserve space for the equipment if we are getting it this task.
            //because if we are getting it this task than right now the equipment is already in the builing
            //we will reserve space for the equipment when we get it instead
            if (m_corespondingGetEquipmentAction == null)
            {
                ReserveSpace();
            }
        }
        protected override void AfterFinished()
        {
            FreeReservedSpace();
        }
        protected override void AfterAborted(bool wasStarted)
        {            
            FreeReservedSpace();            
        }

        /// <summary>
        /// Only free space if we actually reserved space
        /// </summary>
        private bool m_freeSpace = false;

        /// <summary>
        /// Reserve Space in the building to place the items
        /// </summary>
        private void ReserveSpace()
        {            
            m_storageBuilding.Inventory.ReserveCapacityFor(m_whatToPut.ItemType, 1);
            m_freeSpace = true;
        }

        /// <summary>
        /// Free Reserved Space in the building
        /// </summary>
        private void FreeReservedSpace()
        {
            //we might not have reserved space yet (see AfterAssigned method)
            if (m_freeSpace)
            {
                m_storageBuilding.Inventory.FreeReservedCapacityFor(m_whatToPut.ItemType, 1);
            }
        }


        #endregion


        public override List<IGameObject> InvolvedObjects()
        {
            List<IGameObject> ret = new List<IGameObject>();
            ret.Add(m_storageBuilding);
            return ret;
        }

        public override string Description()
        {
            return "Puting Equipment into " + m_storageBuilding.Name;                        
        }
               


        public override void WriteState(ObjectState state)
        {
            base.WriteState(state);
            state.SetValue("StorageBuilding", m_storageBuilding);
            state.SetValue("WhatToPut", m_whatToPut.ItemType.Name);
            state.SetValue("FreeSpace", m_freeSpace);
            state.SetValue("CorespondingGetEquipmentAction", m_corespondingGetEquipmentAction);
        }

        public override void ReadState(ObjectState state)
        {
            base.ReadState(state);
            m_storageBuilding = state.GetValue<IStorageBuilding>("StorageBuilding");
            m_whatToPut = Program.Game.FarmData.GetEquipmentType(state.GetValue<string>("WhatToPut"));
            m_freeSpace = state.GetValue<bool>("FreeSpace");
            m_corespondingGetEquipmentAction = state.GetValue<GetEquipmentAction>("CorespondingGetEquipmentAction");
        }
    }
}
