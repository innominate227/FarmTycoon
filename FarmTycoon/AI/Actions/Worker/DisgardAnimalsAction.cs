using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Worker disgards all animals in their posetion (into the abyss)
    /// </summary>
    public class DisgardAnimalsAction : OneLocationAction<Worker>
    {
        /// <summary>
        /// Where the worker should be when the items are disguarded
        /// </summary>
        private Location m_disgruardAt;

        /// <summary>
        /// List of animals to disguard
        /// </summary>
        private ItemList m_toDisguard;

        /// <summary>
        /// True to ignore the to disguard list and disguard all items
        /// </summary>
        private bool m_diguardAll = false;
        

        /// <summary>
        /// Constructor used for reading object from state
        /// </summary>
        public DisgardAnimalsAction() { }


        /// <summary>
        /// Create a new disguard animals action
        /// </summary>
        public DisgardAnimalsAction(Location disguardAt, List<Animal> toDisguard)
        {
            m_disgruardAt = disguardAt;
            m_toDisguard = toDisguard;
            m_diguardAll = false;
        }

        /// <summary>
        /// Create a new disguard animals action
        /// </summary>
        public DisgardAnimalsAction(Location disguardAt)
        {
            m_disgruardAt = disguardAt;
            m_toDisguard = new List<Animal>();
            m_diguardAll = true;
        }

        /// <summary>
        /// Where the worker should be when the items are disguarded
        /// </summary>
        public Location DisgruardAt
        {
            get { return m_disgruardAt; }
        }

        /// <summary>
        /// List of animals to disguard
        /// </summary>
        public List<Animal> ToDisguard
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
            List<Animal> animalsToDisguard = new List<Animal>();

            if (m_diguardAll)
            {
                //delete all animals the worker has 
                animalsToDisguard.AddRange(m_actor.FollowingAnimals);
            }
            else
            {
                //delete the animals we were set to disguard
                animalsToDisguard.AddRange(m_toDisguard);
            }

            foreach (Animal animal in animalsToDisguard)
            {
                m_actor.RemoveAnimal(animal);
                animal.Delete();
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
            state.SetListValues<Animal>("ToDisguard", m_toDisguard);
            state.SetValue("DiguardAll", m_diguardAll);
        }

        public override void ReadState(ObjectState state)
        {
            base.ReadState(state);
            m_disgruardAt = state.GetValue<Location>("DisgruardAt");
            m_toDisguard = state.GetListValues<Animal>("ToDisguard");
            m_diguardAll = state.GetValue<bool>("DiguardAll");
        }
    }
}
