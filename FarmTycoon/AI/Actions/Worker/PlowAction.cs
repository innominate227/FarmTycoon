using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public partial class PlowAction : MultiLocationAction<Worker>
    {
        #region Member Vars

        /// <summary>
        /// Planted area being plowed
        /// </summary>        
        private Field _field;

        #endregion

        #region Setup

        /// <summary>
        /// only used for loading game state
        /// </summary>
        public PlowAction() { }

        public PlowAction(List<Land> landToPlow, Field field)
        {
            _field = field;
            base.Setup(new List<IHasActionLocation>(landToPlow));
        }

        #endregion

        #region Logic

        public override double ExpectedTimeAtLocation(DelaySet delaySet)
        {
            return delaySet.GetDelay(ActionOrEventType.Plow);
        }

        /// <summary>
        /// We arrived at the location where we will do the action
        /// </summary>
        public override void ArrivedAtLocation(IHasActionLocation arrivedAt)
        {
            //get the land we arrived at
            Land landArrivedAt = (Land)arrivedAt;

            //apply action textures of everything involved
            _actor.SetTextureForActionOrEvent(ActionOrEventType.Plow);
            landArrivedAt.TextureManager.SetTextureForActionOrEvent(ActionOrEventType.Plow);
        }

        protected override void DoActionAtLocation(IHasActionLocation arrivedAt)
        {
            //get the land we arrived at
            Land landArrivedAt = (Land)arrivedAt;

            //apply plow ation to traits at that land, and to worker
            landArrivedAt.Traits.ApplyActionOrEventToTraits(ActionOrEventType.Plow);
            _actor.ApplyActionOrEventToTraits(ActionOrEventType.Plow);

            //clear action textures on objects 
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
            return "Plowing " + _field.Name;
        }

        #endregion

        #region Save Load
        public override void WriteStateV1(StateWriterV1 writer)
        {
            base.WriteStateV1(writer);
            writer.WriteObject(_field);
        }

        public override void ReadStateV1(StateReaderV1 reader)
        {
            base.ReadStateV1(reader);
            _field = reader.ReadObject<Field>();
        }

        public override void AfterReadStateV1()
        {
            base.AfterReadStateV1();
        }
        #endregion

    }
}
