using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{

    /// <summary>
    /// Assits in planning a task by determining what pastures animals can be taken to.  
    /// Keeps track of what previous actions in the task will be putting animals so that it will not plan to put the a different type of animal in the same pasture as another type
    /// </summary>
    public class AnimalPlacementPlanner
    {
        /// <summary>
        /// Mapping from Pasture to the animal type that is expected to be there.
        /// Expected animals are decided based on what the planner has previously planned to put there, or what was there when the planner started planning
        /// </summary>
        private Dictionary<Pasture, ItemTypeInfo> m_expectedAnimalTypes = new Dictionary<Pasture, ItemTypeInfo>();




        /// <summary>
        /// Add actions to the TaskPlan for the worker, workerNum, to get the animals in the list passed from the Pastures they are in        
        /// The Pasture furthest away from orderHint will be visted first, and the one closest will be visited last.        
        /// </summary>
        public void PlanToGetAnimals(TaskPlan plan, int workerNum, List<Animal> animalsToGet, Location orderHint)
        {
            //break the list into sub list by Pasture
            Dictionary<Pasture, List<Animal>> animalsToGetByPasture = new Dictionary<Pasture, List<Animal>>();
            foreach (Animal animal in animalsToGet)
            {
                if (animalsToGetByPasture.ContainsKey(animal.Pasture) == false)
                {
                    animalsToGetByPasture.Add(animal.Pasture, new List<Animal>());
                }
                animalsToGetByPasture[animal.Pasture].Add(animal);
            }
            
            //create a list of the pastures to get animals from sorted by distance from order hint
            List<Pasture> pasturesToGetAnimalsFrom = new List<Pasture>(animalsToGetByPasture.Keys);
            pasturesToGetAnimalsFrom = Program.Game.Tools.GameObjectFinder.SortObjectsByDistance<Pasture>(orderHint, pasturesToGetAnimalsFrom);

            //create an action to put the items into each building
            foreach (Pasture pastureToGetAnimalsFrom in pasturesToGetAnimalsFrom.Reverse<Pasture>())
            {
                //create action to get the animals from that pasture
                GetAnimalsAction getAnimalsAction = new GetAnimalsAction(pastureToGetAnimalsFrom, animalsToGetByPasture[pastureToGetAnimalsFrom]);
                plan.AddAction(workerNum, getAnimalsAction);
            }
        }
             

        /// <summary>
        /// Add actions to the TaskPlan for the worker, workerNum, to put the animals in the list passed into Pastures near to the location passed.
        /// All animals that are allowed in the closest pasture will be placed into that pasture, then if animals reamain all animals will be placed in the next closest pasture, etc.
        /// If it can not find somewhere to put all animals it adds an issue into the task plan.
        /// </summary>
        public void PlanToPutAnimals(TaskPlan plan, int workerNum, List<Animal> animalsToPut, Location near)
        {
            //break the list into sub list bu type
            Dictionary<ItemTypeInfo, List<Animal>> animalsToPutByType = new Dictionary<ItemTypeInfo, List<Animal>>();
            foreach (Animal animal in animalsToPut)
            {
                if (animalsToPutByType.ContainsKey(animal.AnimalInfo.AnimalType) == false)
                {
                    animalsToPutByType.Add(animal.AnimalInfo.AnimalType, new List<Animal>());
                }
                animalsToPutByType[animal.AnimalInfo.AnimalType].Add(animal);
            }

            //the pastures we are going to put things in and the animal type we are going to put in that pasture
            Dictionary<Pasture, ItemTypeInfo> animalTypeToPutInPasture = new Dictionary<Pasture, ItemTypeInfo>();

            //find a place to take each type of animal
            foreach (ItemTypeInfo animalType in animalsToPutByType.Keys)
            {
                //find nearby pasture buildings that can hold that animal
                Pasture pastureThatCanHoldAnimal = Program.Game.Tools.GameObjectFinder.FindClosestObjectMeetingPredicate<Pasture>(near,
                    delegate(Pasture pasture)
                    {
                        //create expected animal type for the building if we dont already have one
                        if (m_expectedAnimalTypes.ContainsKey(pasture) == false)
                        {
                            if (pasture.AnimalInfo == null)
                            {
                                m_expectedAnimalTypes.Add(pasture, null);
                            }
                            else
                            {
                                m_expectedAnimalTypes.Add(pasture, pasture.AnimalInfo.AnimalType);
                            }
                        }

                        //return true if we the expected inventory alrady has this type of animal or has no type of animal
                        return m_expectedAnimalTypes[pasture] == null || m_expectedAnimalTypes[pasture] == animalType;
                    });

                if (pastureThatCanHoldAnimal == null)
                {
                    string issue = "No pasture available for " + animalType.Name + ".";
                    plan.AddIssue(issue, true);
                }
                else
                {
                    //put the animals of that type in the pasture we found
                    animalTypeToPutInPasture.Add(pastureThatCanHoldAnimal, animalType);

                    //note in expected animal type that we plan to put that animal type in there
                    m_expectedAnimalTypes[pastureThatCanHoldAnimal] = animalType;
                }
            }

            //create a list of buildings to put items in sorted by distance to "near"
            List<Pasture> pasturesToPutAnimalsIn = new List<Pasture>(animalTypeToPutInPasture.Keys);
            pasturesToPutAnimalsIn = Program.Game.Tools.GameObjectFinder.SortObjectsByDistance<Pasture>(near, pasturesToPutAnimalsIn);

            //create an action to put the items into each building
            foreach (Pasture pastureToPutAnimalsIn in pasturesToPutAnimalsIn)
            {
                //get teh animal type were putting in that pasture
                ItemTypeInfo animalTypeForPasture = animalTypeToPutInPasture[pastureToPutAnimalsIn];

                //create action to put animals into that pasture
                PutAnimalsAction putAnimalsAction = new PutAnimalsAction(pastureToPutAnimalsIn, animalsToPutByType[animalTypeForPasture]);
                plan.AddAction(workerNum, putAnimalsAction);
            }

        }



        /// <summary>
        /// Add actions to the ActionSequence for the worker, workerNum, to put the animals in the list passed into Pastures near to the location passed.
        /// All animals that are allowed in the closest pasture will be placed into that pasture, then if animals reamain all animals will be placed in the next closest pasture, etc.
        /// If it can not find somewhere to put all animals it adds an issue into the task plan.
        /// </summary>
        public void PlanToPutAnimals(ActionSequence<Worker> actionSequence, List<Animal> animalsToPut, Location near)
        {
            //TODO: this shares alot of code with the PlanToPutAnimals(TaskPlan, ...) they something should be done about that


            //break the list into sub list bu type
            Dictionary<ItemTypeInfo, List<Animal>> animalsToPutByType = new Dictionary<ItemTypeInfo, List<Animal>>();
            foreach (Animal animal in animalsToPut)
            {
                if (animalsToPutByType.ContainsKey(animal.AnimalInfo.AnimalType) == false)
                {
                    animalsToPutByType.Add(animal.AnimalInfo.AnimalType, new List<Animal>());
                }
                animalsToPutByType[animal.AnimalInfo.AnimalType].Add(animal);
            }

            //the pastures we are going to put things in and the animal type we are going to put in that pasture
            Dictionary<Pasture, ItemTypeInfo> animalTypeToPutInPasture = new Dictionary<Pasture, ItemTypeInfo>();

            //find a place to take each type of animal
            foreach (ItemTypeInfo animalType in animalsToPutByType.Keys)
            {
                //find nearby pasture buildings that can hold that animal
                Pasture pastureThatCanHoldAnimal = Program.Game.Tools.GameObjectFinder.FindClosestObjectMeetingPredicate<Pasture>(near,
                    delegate(Pasture pasture)
                    {
                        //create expected animal type for the building if we dont already have one
                        if (m_expectedAnimalTypes.ContainsKey(pasture) == false)
                        {
                            if (pasture.AnimalInfo == null)
                            {
                                m_expectedAnimalTypes.Add(pasture, null);
                            }
                            else
                            {
                                m_expectedAnimalTypes.Add(pasture, pasture.AnimalInfo.AnimalType);
                            }
                        }

                        //return true if we the expected inventory alrady has this type of animal or has no type of animal
                        return m_expectedAnimalTypes[pasture] == null || m_expectedAnimalTypes[pasture] == animalType;
                    });

                if (pastureThatCanHoldAnimal == null)
                {
                    //put the animals of that type in the pasture we found
                    animalTypeToPutInPasture.Add(pastureThatCanHoldAnimal, animalType);

                    //note in expected animal type that we plan to put that animal type in there
                    m_expectedAnimalTypes[pastureThatCanHoldAnimal] = animalType;
                }
            }

            //create a list of buildings to put items in sorted by distance to "near"
            List<Pasture> pasturesToPutAnimalsIn = new List<Pasture>(animalTypeToPutInPasture.Keys);
            pasturesToPutAnimalsIn = Program.Game.Tools.GameObjectFinder.SortObjectsByDistance<Pasture>(near, pasturesToPutAnimalsIn);

            //create an action to put the items into each building
            foreach (Pasture pastureToPutAnimalsIn in pasturesToPutAnimalsIn)
            {
                //get teh animal type were putting in that pasture
                ItemTypeInfo animalTypeForPasture = animalTypeToPutInPasture[pastureToPutAnimalsIn];

                //create action to put animals into that pasture
                PutAnimalsAction putAnimalsAction = new PutAnimalsAction(pastureToPutAnimalsIn, animalsToPutByType[animalTypeForPasture]);
                actionSequence.AddAction(putAnimalsAction);
            }

        }
             


    }
}
