using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// Action for putting animals into a IHoldsAnimals
    /// </summary>
    public class PutAnimalsAction : OneLocationAction<Worker>
    {
        /// <summary>
        /// The animals we are going to put into the area
        /// </summary>
        private List<Animal> m_animalsToPut = new List<Animal>();

        /// <summary>
        /// The area to put the animals into
        /// </summary>
        private IHoldsAnimals m_putInto;

        public PutAnimalsAction() { }


        public PutAnimalsAction(IHoldsAnimals putInto, List<Animal> animalsToPut)            
        {
            m_putInto = putInto;
            m_animalsToPut = animalsToPut;
        }

        
        public override Location TheLocation()
        {
            return m_putInto.ActionLocation;
        }

        public override void DoAction()
        {
            //put all the animals
            foreach (Animal animal in m_animalsToPut)
            {
                //put the animal into the location
                m_putInto.AddAnimal(animal);
                
                //remove from list of workers animals
                m_actor.RemoveAnimal(animal);
            }   

            //adjust the follow chain for remaining following anumals
            PositionManager whoToFollow = m_actor.WorkerPosition;
            foreach (Animal animal in m_actor.FollowingAnimals)
            {
                animal.StartFollowing(whoToFollow);
                whoToFollow = animal.Position;
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
            objs.Add(m_putInto);
            return objs;
        }

        public override string Description()
        {
            return "Put Animals";
        }
    }
}
