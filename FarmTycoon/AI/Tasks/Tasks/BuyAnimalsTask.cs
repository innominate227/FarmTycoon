using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace FarmTycoon
{
    public class BuyAnimalsTask : Task
    {
        /// <summary>
        /// The building that is the prefered destination for the animals that are to be bought, null for no prefered destination
        /// </summary>
        protected Pasture m_preferedDestination;

        /// <summary>
        /// Animals to buy
        /// </summary>
        protected List<Animal> m_whatToBuy = new List<Animal>();
        
        /// <summary>
        /// Create a new task to buy animals, call Setup or ReadState before using
        /// </summary>
        public BuyAnimalsTask() : base() { }
        

        /// <summary>
        /// Animals to buy
        /// </summary>
        public List<Animal> WhatToBuy
        {            
            get { return m_whatToBuy; }            
        }
                
        /// <summary>
        /// The building that is the prefered destination for the items that are to be bought, null for no prefered destination
        /// </summary>
        public Pasture PreferedDestination
        {
            get { return m_preferedDestination; }
            set { m_preferedDestination = value; }
        }

        /// <summary>
        /// Plan out the task
        /// </summary>
        protected override TaskPlan PlanTaskInner()
        {

            //used to plan what each worker should get on each trip from the delivery area
            TaskAnimalDivider animalDivider = new TaskAnimalDivider(m_whatToBuy);
            //used to plan where to take the purchased items
            AnimalPlacementPlanner animalPlanner = new AnimalPlacementPlanner();

            //create the TaskPlan to return
            TaskPlan plan = new TaskPlan(this);
                        
            //find the delivery area building
            DeliveryArea deliveryArea = Program.Game.Tools.GameObjectFinder.FindClosestObjectMeetingPredicate<DeliveryArea>(null, delegate(DeliveryArea building) { return true; });
            if (deliveryArea == null)
            {
                //if no delivery area dont try planning
                plan.AddIssue("No Delivery Area.", true);
                return plan;
            }
                        
            //we will want to put all the animals we buy in pastures near to the delivery area or near to the prefered destination if one is specified
            Location putItemsNear = deliveryArea.ActionLocation;
            if (m_preferedDestination != null)
            {
                putItemsNear = m_preferedDestination.ActionLocation;
            }
            
            //the worker to have get something from the delivery area next
            int workerNum = 0;

            //keep getting loads until none are left
            List<Animal> toGet = animalDivider.NextLoad();
            while (toGet != null)
            {
                //determine if we have a list of normal items or a list of equipment                                    
                PlanAnimalItemsTrip(plan, animalPlanner, workerNum, putItemsNear, deliveryArea, toGet);
                
                //have the next worker find something to get
                workerNum++;
                if (workerNum >= m_numberOfWorkers)
                {
                    workerNum = 0;
                }

                //figure out what the next worker will be getting
                toGet = animalDivider.NextLoad();
            }
                        
            return plan;
        }

        /// <summary>
        /// Plan a trip for getting normal items
        /// </summary>
        private void PlanAnimalItemsTrip(TaskPlan plan, AnimalPlacementPlanner animalPlanner, int workerNum, Location putItemsNear, DeliveryArea deliveryArea, List<Animal> toGet)
        {
            //create action to get the animals needed from the delivery area, and add to the plan
            GetAnimalsAction purchaseItemsAction = new GetAnimalsAction(deliveryArea, toGet);
            plan.AddAction(workerNum, purchaseItemsAction);

            //use the item planner to plan locations to put the animals
            animalPlanner.PlanToPutAnimals(plan, workerNum, toGet, putItemsNear);
        }
        
       
        


        /// <summary>
        /// List of the animals that still need to be gotten for this task.
        /// Used so that if the task is aborted the animals that were not gotten can be refunded.
        /// </summary>
        private List<Animal> m_leftToGet = new List<Animal>();

        /// <summary>
        /// The amount paid for each animal when it was bought
        /// </summary>
        private Dictionary<Animal, int> m_amountPaidForAnimal = new Dictionary<Animal, int>();

        /// <summary>
        /// After the task is setup, remove the animals from the store inventory, and pay for the animals
        /// </summary>
        protected override void DoneWithSetupInner()
        {
            base.DoneWithSetupInner();

            //find the delivery area
            DeliveryArea deliveryArea = Program.Game.Tools.GameObjectFinder.FindClosestObjectMeetingPredicate<DeliveryArea>(null, delegate(DeliveryArea building) { return true; });

            //start the left to get list with everything
            m_leftToGet.AddRange(m_whatToBuy);

            //and pay for the animals we are going to buy and then add them to the delivery area
            foreach (Animal animal in m_whatToBuy)
            {
                //remove animal froms stores stock                
                Program.Game.Store.Animals.Remove(animal);

                //determine the current cost of the item, and remeber the amount paid for it
                int animalCost = Program.Game.Prices.GetPrice(animal.AnimalItemType);
                m_amountPaidForAnimal.Add(animal, animalCost);

                //pay for the amount we bought
                Program.Game.Treasury.Buy(SpendingCatagory.ItemsPurchase, animalCost);

                //put the animal bought into the delivery area                
                deliveryArea.AddAnimal(animal);
            }                        
        }


        public override void ActionFinished(ActionBase<Worker> action)
        {
            base.ActionFinished(action);

            //if it was a get animals action (that we got from the delivery area), remove the animals gotten from left to get list
            if (action is GetAnimalsAction && (action as GetAnimalsAction).GetFrom is DeliveryArea)
            {                
                foreach (Animal animalGotten in (action as GetAnimalsAction).AnimalsToGet)
                {
                    //remove the animal form the left to get list
                    m_leftToGet.Remove(animalGotten);
                }                
            }
        }

        protected override void AfterAborted(bool wasStarted)
        {
            //find the delivery area
            DeliveryArea deliveryArea = Program.Game.Tools.GameObjectFinder.FindClosestObjectMeetingPredicate<DeliveryArea>(null, delegate(DeliveryArea building) { return true; });
            
            //all the animals we never got should be removed from the delivery area, and put back into the stores inventory
            foreach (Animal animal in m_leftToGet)
            {
                //put back into stores                
                Program.Game.Store.Animals.Add(animal);
                                
                //refund the animal
                int amountPaidForAnimal = m_amountPaidForAnimal[animal];
                Program.Game.Treasury.Sell(SpendingCatagory.ItemsPurchase, amountPaidForAnimal);

                //remove the animal we didnt get from the delivery area
                deliveryArea.RemoveAnimal(animal);
            }          
        }


        public override string LongDescription()
        {
            string description = "Buy Animals";            
            
            if (m_preferedDestination == null)
            {
                description += " and place in nearest pasture.";
            }
            else
            {
                description += " and place in " + m_preferedDestination.Name + ".";
            }
            
            return description;
        }

        public override string ShortDescription()
        {
            string description = "Buy Animals";            
            return description;
        }

        public override Color TaskColor()
        {
            return Color.Black;
        }
        


        public override void WriteState(ObjectState state)
        {
            base.WriteState(state);
            state.SetValue("PreferedDestination", m_preferedDestination);
            state.SetListValues<Animal>("WhatToBuy", m_whatToBuy);            
            state.SetListValues<Animal>("LeftToGet", m_leftToGet);
            state.SetDictionaryValues<Animal, int>("AmountsPaid", m_amountPaidForAnimal);
        }

        public override void ReadState(ObjectState state)
        {
            base.ReadState(state);
            m_preferedDestination = state.GetValue<Pasture>("PreferedDestination");
            m_whatToBuy = state.GetListValues<Animal>("WhatToBuy");
            m_leftToGet = state.GetListValues<Animal>("LeftToGet");
            m_amountPaidForAnimal = state.GetDictionaryValues<Animal, int>("AmountsPaid");
        }

    }
}
