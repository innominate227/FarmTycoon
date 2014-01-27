using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    public partial class PickAction : MultiLocationAction<Worker>
    {

        #region Member Vars

        /// <summary>
        /// Field being picked/harvested
        /// </summary>        
        private Field _field;

        /// <summary>
        /// Set to true to harvest or false to pick
        /// </summary>        
        private bool _harvest = false;

        #endregion

        #region Setup

        /// <summary>
        /// Only used for Loading game state
        /// </summary>
        public PickAction(){ }

        /// <summary>
        /// Create a new pick action
        /// </summary>
        public PickAction(List<Crop> cropToHarvest, Field field, bool harvest)
        {
            _field = field;
            _harvest = harvest;
            base.Setup(new List<IHasActionLocation>(cropToHarvest));
        }

        #endregion

        #region Logic

        public override double ExpectedTimeAtLocation(DelaySet delaySet)
        {
            if (_harvest)
            {
                double delayMultiplier = 1.0;
                if (_field.CropInfo != null)
                {
                    delayMultiplier = _field.CropInfo.HarvestDelayMultiplier;
                }
                return delayMultiplier * delaySet.GetDelay(ActionOrEventType.Harvest);
            }
            else
            {
                double delayMultiplier = 1.0;
                if (_field.CropInfo != null)
                {
                    delayMultiplier = _field.CropInfo.PickDelayMultiplier;
                }
                return delayMultiplier * delaySet.GetDelay(ActionOrEventType.Pick);
            }
        }

        /// <summary>
        /// We arrived at the location where we will do the action
        /// </summary>
        public override void  ArrivedAtLocation(IHasActionLocation arrivedAt)
        {
            //get the crop we arrived at
            Crop cropArrivedAt = (Crop)arrivedAt;

            //what type of action should we apply
            ActionOrEventType toApply = ActionOrEventType.Pick;
            if (_harvest)
            {
                toApply = ActionOrEventType.Harvest;
            }

            //apply action textures of everything involved
            _actor.SetTextureForActionOrEvent(toApply);
            cropArrivedAt.TextureManager.SetTextureForActionOrEvent(toApply);
            cropArrivedAt.LocationOn.Find<Land>().TextureManager.SetTextureForActionOrEvent(toApply);
        }

        protected override void DoActionAtLocation(IHasActionLocation arrivedAt)
        {
            //we should have arrived a crop
            Debug.Assert(arrivedAt is Crop);
            
            //get the crop we arrived at
            Crop cropArrivedAt = (Crop)arrivedAt;

            //get the quality of the crop (0-100) scale
            int objectQuality = cropArrivedAt.Quality.CurrentQuality;
            
            //the crop has died, and was only not deleted yet because of this action, lower itemQuality to 0
            if (cropArrivedAt.PlacementState == PlacementState.WaitingToDelete)
            {
                objectQuality = 0;
            }

            //type info for the type we will end up producing
            ItemTypeInfo typeInfoProduced = null;

            //are we harvesting the crop, or just picking it
            if (_harvest)
            {
                //get the item type that will be created on harvest
                typeInfoProduced = cropArrivedAt.CropInfo.HarvestItem;

                //apply harvest action to the land, and actor (we dont apply Harvest top crop since its going to get deleted)
                cropArrivedAt.LocationOn.Find<Land>().Traits.ApplyActionOrEventToTraits(ActionOrEventType.Harvest);
                _actor.ApplyActionOrEventToTraits(ActionOrEventType.Harvest);

                //remove the crop
                cropArrivedAt.Delete();

                //clear action textures of everything involved (that was not deleted)                
                cropArrivedAt.LocationOn.Find<Land>().TextureManager.ClearTextureForActionOrEvent();
                _actor.ClearTextureForActionOrEvent();
            }
            else
            {
                //get the item type that will be created on pick
                typeInfoProduced = cropArrivedAt.CropInfo.PickItem;

                //we took soo long to arrive at the crop that we are not even supposed to be allowed to pick it any more, set the quality of what is going to be picked to be as low as possible
                if (cropArrivedAt.CanPick == false)
                {
                    objectQuality = 0;
                }

                //apply picked action to traits 
                cropArrivedAt.Traits.ApplyActionOrEventToTraits(ActionOrEventType.Pick);
                cropArrivedAt.LocationOn.Find<Land>().Traits.ApplyActionOrEventToTraits(ActionOrEventType.Pick);
                _actor.ApplyActionOrEventToTraits(ActionOrEventType.Pick);
                                
                //Tell it that we picked it so it doesnt allow us to pick it again in the same cycle
                cropArrivedAt.Picked();

                //clear action textures of everything involved                
                cropArrivedAt.TextureManager.ClearTextureForActionOrEvent();
                cropArrivedAt.LocationOn.Find<Land>().TextureManager.ClearTextureForActionOrEvent();
                _actor.ClearTextureForActionOrEvent();
            }

            //get the item type to produce
            int itemQuality = (int)Math.Min(9, objectQuality / 100.0);
            ItemType typeProduced = GameState.Current.ItemPool.GetItemType(typeInfoProduced.Name, itemQuality);

            //set the quality for that type, this only needs to be done once, but it hard to know if we did it yet so might as well just do it every time
            typeProduced.Quality = itemQuality;

            //add that to the workers inventory
            _actor.Inventory.AddToInvetory(typeProduced, 1);
        }


        public override bool IsObjectInvolved(IGameObject obj)
        {
            //is it the field 
            if (obj == _field) { return true; }

            if (obj is Crop)
            {
                //is it one of the crops we are going to pick
                if (_actionLocations.Contains(obj))
                {
                    return true;
                }
            }
            return false;
        }

        public override string Description()
        {
            return "Harvesting " + _field.Name;            
        }

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteObject(_field);
            writer.WriteBool(_harvest);
        }

        public override void ReadStateV1(StateReaderV1 reader)
		{
			base.ReadStateV1(reader);
			_field = reader.ReadObject<Field>();
			_harvest = reader.ReadBool();
		}

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion

    }
}
