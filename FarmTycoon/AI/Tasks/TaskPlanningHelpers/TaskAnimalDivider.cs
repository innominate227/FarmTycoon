using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    public class TaskAnimalDivider
    {
        
        /// <summary>
        /// Animals left to be assigned to a worker to get
        /// </summary>
        private List<Animal> m_animalsLeftToGet = new List<Animal>();
                

        /// <summary>
        /// Create a new ItemDivider to divide out the responsibility of getting all the items in the list passed
        /// </summary>
        public TaskAnimalDivider(List<Animal> animalsLeftToGet)
        {
            m_animalsLeftToGet = new List<Animal>(animalsLeftToGet);

            //sort the animals by type
            m_animalsLeftToGet.Sort(delegate(Animal a1, Animal a2)
            {
                return a1.AnimalInfo.Name.CompareTo(a2.AnimalInfo.Name);
            });
        }


        /// <summary>
        /// Return the next load of items that a worker should get from the list of all items.
        /// Each load will be such that a worker should be able to manage the load in one trip.
        /// When there is nothing left to be gotten null is returned.
        /// </summary>
        public List<Animal> NextLoad()
        {
            //if nothing left to get return null
            if (m_animalsLeftToGet.Count == 0)
            {
                return null;
            }

            //create a list for what to get this load.  The worker will get as much as its inventory size allows
            List<Animal> thisLoad = new List<Animal>();
            int spaceLeft = Program.Game.FarmData.WorkerInfo.Capacity;

            //foreach animal that still needs to be gotten
            foreach (Animal animal in m_animalsLeftToGet.ToArray())
            {
                //get the size of the animal
                int itemSize = animal.AnimalInfo.AnimalType.Size;

                //if the animal will fit
                if (itemSize < spaceLeft)
                {
                    //add to this load and remove from left to get
                    thisLoad.Add(animal);
                    m_animalsLeftToGet.Remove(animal);
                }
                else
                {
                    //if hold any more then break
                    break;
                }
            }
            
            return thisLoad; 
        }



    }
}
