using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    public partial class PlantAction : MultiLocationAction<Worker>
    {
        #region Member Vars

        /// <summary>
        /// Item type for the crop to plant
        /// </summary>        
        private ItemType _typeToPlant;

        /// <summary>
        /// The field that is being planted
        /// </summary>        
        private Field _field;

        #endregion

        #region Setup

        /// <summary>
        /// only used for loading game state
        /// </summary>
        public PlantAction(){ }

        /// <summary>
        /// Create a new Plant Field Action
        /// </summary>        
        public PlantAction(List<Land> landToPlantOn, ItemType typeToPlant, Field field)
        {
            _typeToPlant = typeToPlant;
            _field = field;
            base.Setup(new List<IHasActionLocation>(landToPlantOn));
        }

        #endregion

        #region Logic

        public override double ExpectedTimeAtLocation(DelaySet delaySet)
        {            
            double delayMultiplier = 1.0;
            if (_field.CropInfo != null)
            {
                delayMultiplier = _field.CropInfo.PlantDelayMultiplier;
            }
            return delayMultiplier * delaySet.GetDelay(ActionOrEventType.Plant);                
        }
        
        /// <summary>
        /// We arrived at the location where we will do the action
        /// </summary>
        public override void ArrivedAtLocation(IHasActionLocation arrivedAt)
        {
            //get the land we arrived at
            Land landArrivedAt = (Land)arrivedAt;
            
            //apply to action of everything involved
            _actor.SetTextureForActionOrEvent(ActionOrEventType.Plant);
            landArrivedAt.TextureManager.SetTextureForActionOrEvent(ActionOrEventType.Plant);            
        }

        protected override void DoActionAtLocation(IHasActionLocation arrivedAt)
        {
            Land landArrivedAt = (Land)arrivedAt;
                        
            //make sure worker has the seed to plant that crop
            Debug.Assert(_actor.Inventory.GetTypeCount(_typeToPlant) >= 1);
                       
            //make sure there is not already a crop there
            Debug.Assert(landArrivedAt.LocationOn.Contains<Crop>() == false);

            //get crop info for the crop that will be made form this seed
            CropInfo cropInfo = FarmData.Current.GetCropInfoForSeed(_typeToPlant.BaseType);

            //create a new crop
            Crop newCrop = new Crop();
            newCrop.Setup(landArrivedAt, _field, cropInfo);
           
            //remove seed from worker inventory
            _actor.Inventory.RemoveFromInvetory(_typeToPlant, 1);
            
            //apply plant ation to the land, crop, workers traits
            landArrivedAt.Traits.ApplyActionOrEventToTraits(ActionOrEventType.Plant);            
            newCrop.Traits.ApplyActionOrEventToTraits(ActionOrEventType.Plant);
            _actor.ApplyActionOrEventToTraits(ActionOrEventType.Plant);

            //clear action textures on objects they were started on
            landArrivedAt.TextureManager.ClearTextureForActionOrEvent();
            _actor.ClearTextureForActionOrEvent();
        }



        public override bool IsObjectInvolved(IGameObject obj)
        {
            //is it the field 
            if (obj == _field) { return true; }

            return false;
        }
        
        public override string Description()
        {
            return "Planting " + _typeToPlant.FullName + " in " + _field.Name;            
        }

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteObject(_typeToPlant);
            writer.WriteObject(_field);
        }

        public override void ReadStateV1(StateReaderV1 reader)
		{
			base.ReadStateV1(reader);
			_typeToPlant = reader.ReadObject<ItemType>();
			_field = reader.ReadObject<Field>();
		}

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion

    }
}
