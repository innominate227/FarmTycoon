using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// Action for getting animals from a IHoldsAnimals
    /// </summary>
    public class GetAnimalsAction : OneLocationAction<Worker>
    {
        /// <summary>
        /// The animals we are going to get from the pasture
        /// </summary>
        private List<Animal> m_animalsToGet = new List<Animal>();

        /// <summary>
        /// The area to get the animals from
        /// </summary>
        private IHoldsAnimals m_getFrom;

        public GetAnimalsAction() { }


        public GetAnimalsAction(IHoldsAnimals getFrom, List<Animal> animalsToGet)            
        {
            m_getFrom = getFrom;
            m_animalsToGet = animalsToGet;
        }

        /// <summary>
        /// The animals we are going to get from the pasture
        /// </summary>
        public IList<Animal> AnimalsToGet
        {
            get { return m_animalsToGet.AsReadOnly(); }
        }

        /// <summary>
        /// The area to get the animals from
        /// </summary>
        public IHoldsAnimals GetFrom
        {
            get { return m_getFrom; }
        }

        
        public override Location TheLocation()
        {
            return m_getFrom.ActionLocation;
        }

        public override void DoAction()
        {
            //check that all the animals will fit into the inventory            
            int amountThatCanFit = m_actor.Inventory.AmountThatWillFit(m_animalsToGet[0].AnimalInfo.AnimalType.GetItemTypeWithQuality(0));
            Debug.Assert(m_animalsToGet.Count <= amountThatCanFit);

            //the first animal we create should follow the worker, unless the worker already has animals following, in which case it should follow the last animal
            PositionManager whoToFollow = m_actor.WorkerPosition;
            if (m_actor.FollowingAnimals.Count > 0)
            {
                whoToFollow = m_actor.FollowingAnimals[m_actor.FollowingAnimals.Count - 1].Position;
            }

            //move all the animals
            foreach (Animal animal in m_animalsToGet)
            {
                //remove the animal from the location
                m_getFrom.RemoveAnimal(animal);

                //follow the worker or the last animal created
                animal.StartFollowing(whoToFollow);

                //next animal will follow the this animal
                whoToFollow = animal.Position;

                //add to list of workers animals
                m_actor.AddAnimal(animal);
            }   
        }

        public override double GetActionTime(double actionDelayMultiplier)
        {
            //1 day + action delay
            return 1.0 * actionDelayMultiplier;
        }

        public override List<IGameObject> InvolvedObjects()
        {
            List<IGameObject> objs = new List<IGameObject>();
            objs.Add(m_getFrom);
            return objs;
        }

        public override string Description()
        {
            return "Get Animals";
        }
    }
}
