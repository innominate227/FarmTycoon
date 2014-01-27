using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    public partial class SprayAction : MultiLocationAction<Worker>
    {
        #region Member Vars

        /// <summary>
        /// Item type to spray
        /// </summary>        
        private ItemType _typeToSpray;

        /// <summary>
        /// The area that is being planted its name will be used in the description for this action
        /// </summary>        
        private Field _areaBeingSprayed;

        #endregion

        #region Setup

        /// <summary>
        /// Only used when loading game
        /// </summary>
        public SprayAction() { }

        /// <summary>
        /// Create a new spray action
        /// </summary>
        /// <param name="objectsToSpray">List of objects that need to be spraryed</param>
        /// <param name="typeToSpray">The item type to spray</param>
        /// <param name="areaBeingSprayed">Object whos area is being sprayed.</param>
        public SprayAction(List<IHasActionLocation> objectsToSpray, ItemType typeToSpray, Field areaBeingSprayed)
        {
            _typeToSpray = typeToSpray;
            _areaBeingSprayed = areaBeingSprayed;
            base.Setup(objectsToSpray);
        }

        #endregion

        #region Logic

        public override double ExpectedTimeAtLocation(DelaySet delaySet)
        {                       
            return delaySet.GetDelay(ActionOrEventType.Spray);
        }

        /// <summary>
        /// We arrived at the location where we will do the action
        /// </summary>
        public override void ArrivedAtLocation(IHasActionLocation arrivedAt)
        {
            //apply the action to the textures for the spray and crop
            //Note if the object has died since we started moving towrd it then dont try and do anything to it
            if (arrivedAt.PlacementState != PlacementState.Deleted)
            {
                Crop crop = arrivedAt.LocationOn.Find<Crop>();
                crop.TextureManager.SetTextureForActionOrEvent(ActionOrEventType.Spray);
                Land land = arrivedAt.LocationOn.Find<Land>();
                land.TextureManager.SetTextureForActionOrEvent(ActionOrEventType.Spray);
            }

            //set action texture for worker
            _actor.SetTextureForActionOrEvent(ActionOrEventType.Spray);
        }

        protected override void DoActionAtLocation(IHasActionLocation arrivedAt)
        {                        
            //worker should have in inventory what they are supposed to spray
            Debug.Assert(_actor.Inventory.GetTypeCount(_typeToSpray) >= 1);

            //the crop we are going to spray
            Crop crop = (Crop)arrivedAt; 

            //apply the spray to the crop and land at that location
            //Note if the object has died since we started moving towrd it then it will not have a location, so dont try and spary it
            if (arrivedAt.PlacementState != PlacementState.Deleted)
            {
                //apply item, and evnet to the crop traits.  Then clear the action texture                
                crop.Traits.ApplyItemToTraits(_typeToSpray);
                crop.Traits.ApplyActionOrEventToTraits(ActionOrEventType.Spray);
                crop.TextureManager.ClearTextureForActionOrEvent();

                //apply item, and event to the land traits.  Then clear the action texture
                Land land = crop.LocationOn.Find<Land>();
                land.Traits.ApplyItemToTraits(_typeToSpray);
                land.Traits.ApplyActionOrEventToTraits(ActionOrEventType.Spray);
                land.TextureManager.ClearTextureForActionOrEvent();
            }

            //remove the spray from the workers inventory
            _actor.Inventory.RemoveFromInvetory(_typeToSpray, 1);

            //apply to spray action worker traits, and clear the action texture
            _actor.ApplyActionOrEventToTraits(ActionOrEventType.Spray);
            _actor.ClearTextureForActionOrEvent();
        }


        public override bool IsObjectInvolved(IGameObject obj)
        {
            //is it the field 
            if (obj == _areaBeingSprayed) { return true; }

            if (obj is Crop)
            {
                //is it one of the crops we are going to spray
                if (_actionLocations.Contains(obj))
                {
                    return true;
                }
            }
            return false;
        }

        public override string Description()
        {
            return "Spraying " + _typeToSpray.FullName + " in " + _areaBeingSprayed.Name;            
        }

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteObject(_typeToSpray);
            writer.WriteObject(_areaBeingSprayed);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _typeToSpray = reader.ReadObject<ItemType>();
            _areaBeingSprayed = reader.ReadObject<Field>();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion

    }
}
