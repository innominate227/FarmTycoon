using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Worker disgards all equipment in their posetion (into the abyss)
    /// </summary>
    public class DisgardEquipmentAction : OneLocationAction<Worker>
    {
        /// <summary>
        /// Where the worker should be when the equipment is disguarded
        /// </summary>
        private Location m_disgruardAt;

        /// <summary>
        /// Equipment type to disguard
        /// </summary>
        private EquipmentType m_toDisguard;

        /// <summary>
        /// True to ignore the to disguard variable and disguard all equipment
        /// </summary>
        private bool m_diguardAll = false;

        public DisgardEquipmentAction() { }

        /// <summary>
        /// Create a new disguard equipment action
        /// </summary>
        public DisgardEquipmentAction(Location disguardAt, EquipmentType toDisguard)
        {
            m_disgruardAt = disguardAt;
            m_toDisguard = toDisguard;
            m_diguardAll = false;
        }

        /// <summary>
        /// Create a new disguard equipment action, to disguard all equipment
        /// </summary>
        public DisgardEquipmentAction(Location disguardAt)
        {
            m_disgruardAt = disguardAt;
            m_toDisguard = null;
            m_diguardAll = true;
        }


        /// <summary>
        /// Where the worker should be when the equipment is disguarded
        /// </summary>
        public Location DisgruardAt
        {
            get { return m_disgruardAt; }
        }

        /// <summary>
        /// Equipment type to disguard
        /// </summary>
        public EquipmentType ToDisguard
        {
            get { return m_toDisguard; }
        }


        public override Location TheLocation()
        {
            return m_disgruardAt;
        }

        public override double GetActionTime(double actionDelayMultiplier)
        {
            return 0.0;
        }

        public override void DoAction()
        {
            if (m_diguardAll)
            {
                //have the worker get of its tow
                if (m_actor.TowType.IsNullType() == false)
                {
                    m_actor.GetOffEquipment(m_actor.TowType);
                }

                //have the worker get off its tractor
                if (m_actor.VehicleType.IsNullType() == false)
                {
                    m_actor.GetOffEquipment(m_actor.VehicleType);
                }
            }
            else
            {
                //make sure the worker has that equipment
                Debug.Assert(m_actor.HasEquipment(m_toDisguard));

                //have the worker get of the equipment to disguard
                m_actor.GetOffEquipment(m_toDisguard);
            }
        }



        public override List<IGameObject> InvolvedObjects()
        {
            return new List<IGameObject>();
        }


        public override string Description()
        {   
            return "";                        
        }


        public override void WriteState(ObjectState state)
        {
            base.WriteState(state);
            state.SetValue("DisgruardAt", m_disgruardAt);
            state.SetValue("ToDisguard", m_toDisguard.ItemType.Name);
            state.SetValue("DiguardAll", m_diguardAll);
        }

        public override void ReadState(ObjectState state)
        {
            base.ReadState(state);
            m_disgruardAt = state.GetValue<Location>("DisgruardAt");
            m_toDisguard = Program.Game.FarmData.GetEquipmentType(state.GetValue<string>("DisgruardAt"));
            m_diguardAll = state.GetValue<bool>("DiguardAll");
        }

    }
}
