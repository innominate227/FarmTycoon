using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// Action to get equipment from a storgae building.  Action needs to be told what is going to be getting so it can reserve the item in the building
    /// </summary>
    public class GetEquipmentAction : OneLocationAction<Worker>
    {        
        /// <summary>
        /// The peice of equipmnet to get from the builidng
        /// </summary>
        private Equipment m_whatToGet;

        /// <summary>
        /// Storage building to get the equipmnet from
        /// </summary>
        private IStorageBuilding m_storageBuilding;

        /// <summary>
        /// If we are getting and putting equipment in the same action then this is the action where we will put the equipment back.
        /// We want to tell the put action when we get the equipment so it knows not to reserve space to put it back until we get it.
        /// </summary>
        private PutEquipmentAction m_corespondingPutEquipmentAction = null;

        public GetEquipmentAction() { }
        
        /// <summary>
        /// Create a new get equipment
        /// </summary>
        public GetEquipmentAction(IStorageBuilding storageBuilding, Equipment whatToGet)
        {        
            m_storageBuilding = storageBuilding;
            m_whatToGet = whatToGet;
        }


        /// <summary>
        /// Equipmnet to get from the builidng
        /// </summary>
        public Equipment WhatToGet
        {
            get { return m_whatToGet; }
        }

        /// <summary>
        /// Storage building to get the equipmnet from
        /// </summary>
        public IStorageBuilding StorageBuilding
        {
            get { return m_storageBuilding; }
        }


        /// <summary>
        /// Set the action where we put the equipment we are going to get back into the same storage.
        /// We need to tell it when we take the equipment out so it doesnt reserve space until the equipment is out of the storage building.
        /// This should only be called by PutEquipmnetAction class.
        /// </summary>        
        public void SetCorespondingPutEquipmentAction(PutEquipmentAction corespondingPuttEquipmentAction)
        {
            m_corespondingPutEquipmentAction = corespondingPuttEquipmentAction;
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
            //remove the equipment from the building
            m_storageBuilding.Inventory.RemoveFromInvetory(m_whatToGet.ItemType, 1);

            //tell the corespond put action that we have gotten the equipment
            if (m_corespondingPutEquipmentAction != null)
            {
                m_corespondingPutEquipmentAction.CorespondingGetEquipmentActionGotEquipment();
            }
            
            //give that equipment to the worker
            m_actor.GetOnEquipment(m_whatToGet);
        }


        #region Reserve and Free space

        protected override void AfterAssigned()
        {
            ReserveItems();
        }
        protected override void AfterFinished()
        {
            FreeReservedItems();
        }
        protected override void AfterAborted(bool wasStarted)
        {
            FreeReservedItems();
        }


        /// <summary>
        ///reserve the items int eh inventory of the storage building we are getting the items from
        /// </summary>
        private void ReserveItems()
        {
            m_storageBuilding.Inventory.ReserveItems(m_whatToGet.ItemType, 1);            
        }

        /// <summary>
        /// free the reserved items
        /// </summary>
        private void FreeReservedItems()
        {
            m_storageBuilding.Inventory.FreeReservedItems(m_whatToGet.ItemType, 1);            
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
            return "Getting Equipment from " + m_storageBuilding.Name;                        
        }
               


        public override void WriteState(ObjectState state)
        {
            base.WriteState(state);
            state.SetValue("StorageBuilding", m_storageBuilding);
            state.SetValue("WhatToGet", m_whatToGet);
            state.SetValue("CorespondingPutEquipmentAction", m_corespondingPutEquipmentAction);
        }

        public override void ReadState(ObjectState state)
        {
            base.ReadState(state);
            m_storageBuilding = state.GetValue<IStorageBuilding>("StorageBuilding");
            m_whatToGet = state.GetValue<Equipment>("WhatToGet");
            m_corespondingPutEquipmentAction = state.GetValue<PutEquipmentAction>("CorespondingPutEquipmentAction");
        }

    }
}
