using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Action where an animal visits a trough and applies items in it to its self.      
    /// </summary>
    public partial class VisitTroughAction : OneLocationAction<Animal>
    {
        #region Member Vars

        /// <summary>
        /// The Trough the animal will consume items from.
        /// </summary>
        private Trough _trough;

        /// <summary>
        /// Items in the Trough the animal will apply to itself.
        /// </summary>
        private ItemList _items;

        #endregion

        #region Setup

        /// <summary>
        /// Used on Load
        /// </summary>
        public VisitTroughAction()
        {            
        }

        /// <summary>
        /// Create an action to visit the Trough and consume the items in the list passed
        /// </summary>
        public VisitTroughAction(Trough trough, ItemList items)
        {
            _trough = trough;
            _items = items;
        }

        #endregion

        #region Logic

        /// <summary>
        /// We arrived at the location where we will do the action
        /// </summary>
        public override void ArrivedAtAction()
        {
            //set temp textures VistTrough on animal
            _actor.TextureManager.SetTextureForActionOrEvent(ActionOrEventType.VisitTrough);
        }
        
        /// <summary>
        /// Return the location we need to visit for the action
        /// </summary>        
        public override Location TheLocation()
        {
            return _trough.ActionLocation;
        }

        public override void  DoAction()
        {
            //remove the items from the trough
            foreach (ItemType itemType in _items.ItemTypes)
            {
                //get how many of the item to get
                int itemTypeCount = _items.GetItemCount(itemType);
                
                //remove from the troughs inventory
                _trough.Inventory.RemoveFromInvetory(itemType, itemTypeCount);

                //apply the items to the animal                
                _actor.Traits.ApplyItemToTraits(itemType, itemTypeCount);                
            }

            //apply traits for the VistTrough action to the animal
            _actor.Traits.ApplyActionOrEventToTraits(ActionOrEventType.VisitTrough);
        }

        public override double  GetActionTime(DelaySet delaySet)
        {            
            return delaySet.GetDelay(ActionOrEventType.VisitTrough);
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
        ///reserve the items in the trough for the animal
        /// </summary>
        private void ReserveItems()
        {
            foreach (ItemType itemType in _items.ItemTypes)
            {
                _trough.Inventory.ReserveItems(itemType, _items.GetItemCount(itemType));
            }
        }

        /// <summary>
        /// free the reserved items
        /// </summary>
        private void FreeReservedItems()
        {
            foreach (ItemType itemType in _items.ItemTypes)
            {
                _trough.Inventory.FreeReservedItems(itemType, _items.GetItemCount(itemType));
            }
        }

        #endregion

        public override bool  IsObjectInvolved(IGameObject obj)
        {
            return (obj == _trough);
        }        

        public override string  Description()
        {
            return "Trough";
        }

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteObject(_trough);
            writer.WriteObject(_items);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _trough = reader.ReadObject<Trough>();
            _items = reader.ReadObject<ItemList>();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion

    }
}
       