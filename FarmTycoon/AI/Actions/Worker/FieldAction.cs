using System;
//using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    public abstract class FieldAction : ActionBase
    {        
        /// <summary>
        /// The field we are doing an action on
        /// </summary>
        protected Field m_field;

        /// <summary>
        /// The land in the field to do the actions on
        /// </summary>
        protected List<Land> m_actionLand;
                
        /// <summary>
        /// The index of land we are working on visiting.
        /// This is used while the action is actually running, so we know what actionland we are visiting
        /// </summary>
        private int m_indexVisiting = 0;

        /// <summary>
        /// If the worker is using a tow, they need to go to the action land, but they wont actual do the action there, they will instead go one tile toward the
        /// next place they need to be, and then do the action, this way there tow is on the action land.  This will be true while they are going to the action land
        /// where they will not immedantly do the action
        /// </summary>
        private bool m_extraMoveForTow = false;

        public FieldAction() { }

        public FieldAction(Field field, List<Land> actionLand)
        {
            m_field = field;
            m_actionLand = actionLand;

            //make sure we are acting on at least on peice of land
            System.Diagnostics.Debug.Assert(actionLand.Count > 0);
        }
        
        public override Location FirstLocation()
        {
            return m_actionLand[0].LocationOn;
        }

        public override Location LastLocation()
        {
            return m_actionLand[m_actionLand.Count - 1].LocationOn;
        }

        public override double ExpectedTime(EquipmentType expectedTractor, EquipmentType expectedTow)
        {
            double moveSpeedMultiplier = expectedTractor.MoveSpeedMultiplier * expectedTow.MoveSpeedMultiplier;
            
            //time spent at each land tile
            double expectedTime = m_actionLand.Count * ExpectedTimeAtEachFieldTile(expectedTractor, expectedTow);
            
            //estimate time for walking between each tile in the action list.  
            //(assumes all action list tiles are adjacent which is true most of the time, and should be ok since this is just expected time)
            expectedTime += ((m_actionLand.Count - 1) * Worker.OFF_PATH_BASE_SPEED * moveSpeedMultiplier);

            #region Slower More Exact Time
            ////time to walk between the tiles
            //for (int i = 1; i < m_actionLand.Count; i++)
            //{
            //    Land land1 = m_actionLand[i - 1];
            //    Land land2 = m_actionLand[i];

            //    //add the time to get between the peices of land
            //    double timeBetweenLand;
            //    Program.Tools.FastestPathFinder.FindPath(land1, land2, out timeBetweenLand);
            //    expextedTime += timeBetweenLand;
            //}
            #endregion
                        
            //return estimated time
            return expectedTime;
        }
        
        public override double GetLocationWaitTime(Location location)
        {
            //dont wait if this is the special extra move needed for each action when towing
            if (m_extraMoveForTow) { return 0.0; }

            //otherwise wait the amount determined by the subclass
            return ExpectedTimeAtEachFieldTile(m_worker.VehicleType, m_worker.TowType);
        }

        public abstract double ExpectedTimeAtEachFieldTile(EquipmentType expectedTractor, EquipmentType expectedTow);

        protected override void AfterStarted()
        {
            //if worker is on a tow the first move he does will be the extra move for the tow            
            if (m_worker.TowType.IsNullType() == false)
            {
                m_extraMoveForTow = true;
            }
        }

        protected override Location NextLocationInnrer()
        {
            //check if no more action land to visit return null
            if (m_indexVisiting == m_actionLand.Count)
            {
                return null;
            }

            if (m_worker.TowType.IsNullType())
            {
                //we are on foot visit the next peice of land
                return m_actionLand[m_indexVisiting].LocationOn;
            }
            else
            {
                //if not extra move for tow visits the next action land
                if (m_extraMoveForTow)
                {
                    return m_actionLand[m_indexVisiting].LocationOn;
                }
                else
                {
                    //the land for where we do the actual action will be the land 1 unit in the direction of the next place we need to be

                    //next action land is the next action land unless there isnt one, then the next action land is the entrance
                    Land nextActionLand = m_field.Enclosure.EntryLand;
                    if (m_indexVisiting + 1 < m_actionLand.Count)
                    {
                        nextActionLand = m_actionLand[m_indexVisiting + 1];
                    }

                    //get the path that leads to the next action
                    List<Location> path = Program.Game.Tools.FastestPathFinder.FindPath(m_actionLand[m_indexVisiting].LocationOn, nextActionLand.LocationOn);

                    //in rare case where there is no path just try and go to action land like normal
                    if (path == null)
                    {
                        return m_actionLand[m_indexVisiting].LocationOn;
                    }

                    //return the second peice of land in the path (the first peice is the action land)
                    return path[1];
                }
            }   
        }

        public override void DoLocationAction(Location arrivedAt)
        {
            //Debug.Assert(m_actionLand[m_indexVisiting].LocationOn == arrivedAt);

            //it was not an extra move for towing
            if (m_extraMoveForTow == false)
            {
                //do the action at the land
                DoActionAtFieldLand(m_actionLand[m_indexVisiting]);

                //increment land to visit
                m_indexVisiting++;

                //if we have a tow the next move will be an extra move
                if (m_worker.TowType.IsNullType() == false)
                {
                    m_extraMoveForTow = true;
                }
            }
            else
            {
                //it was the extra move for the tow, the next will not be
                m_extraMoveForTow = false;
            }             
        }



        protected abstract void DoActionAtFieldLand(Land arrivedAt);
        



        public override void WriteState(ObjectState state)
        {
            base.WriteState(state);
            state.SetValue("Field", m_field);
            state.SetValue("ActionLandCount", m_actionLand.Count);
            for (int i = 0; i < m_actionLand.Count; i++)
            {
                state.SetValue("ActionLand" + i.ToString(), m_actionLand[i]);
            }
            state.SetValue("IndexLastVisited", m_indexVisiting);
            state.SetValue("ExtraMoveForTow", m_extraMoveForTow);
        }

        public override void ReadState(ObjectState state)
        {
            base.ReadState(state);
            m_field = state.GetValue<Field>("Field");
            int actionLandCount = state.GetValue<int>("ActionLandCount");
            m_actionLand = new List<Land>();
            for (int i = 0; i < actionLandCount; i++)
            {
                m_actionLand.Add(state.GetValue<Land>("ActionLand" + i.ToString()));                
            }
            m_indexVisiting = state.GetValue<int>("IndexLastVisited");
            m_extraMoveForTow = state.GetValue<bool>("ExtraMoveForTow");
        }
    }
}
