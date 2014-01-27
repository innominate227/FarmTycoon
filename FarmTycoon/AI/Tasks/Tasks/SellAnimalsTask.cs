using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace FarmTycoon
{
    public class SellAnimalsTask : Task
    {
        /// <summary>
        /// The pasture that is the prefered source for the animals that are to be sold
        /// </summary>
        protected Pasture m_preferedSource;

        /// <summary>
        /// Animals to sell
        /// </summary>
        protected List<Animal> m_whatToSell = new List<Animal>();
        
        /// <summary>
        /// Create a new SellAnimalsTask.  
        /// </summary>
        public SellAnimalsTask() : base() { }

        /// <summary>
        /// Animals to sell
        /// </summary>
        public List<Animal> WhatToSell
        {
            set { m_whatToSell = value; }
            get { return m_whatToSell; }
        }

        /// <summary>
        /// The pasture that is the prefered source for the animals that are to be sold
        /// </summary>
        public Pasture PreferedSource
        {
            get { return m_preferedSource; }
            set { m_preferedSource = value; }
        }

        /// <summary>
        /// Plan out the task
        /// </summary>
        protected override TaskPlan PlanTaskInner()
        {
            //used to plan what each worker should get before each trip to the delivery area
            TaskAnimalDivider animalDivider = new TaskAnimalDivider(m_whatToSell);
            //used to plan where to get the animals we are selling
            AnimalPlacementPlanner animalPlanner = new AnimalPlacementPlanner();

            //create the TaskPlan to return
            TaskPlan plan = new TaskPlan(this);
            
            //find the delivery area building
            DeliveryArea deliveryArea = Program.Game.Tools.GameObjectFinder.FindClosestObjectMeetingPredicate<DeliveryArea>(null, delegate(DeliveryArea building) { return true; });
            if (deliveryArea == null)
            {
                //if no deliveryArea dont try planning
                plan.AddIssue("No Delivery Area", true);
                return plan;
            }

            //we will want to get all the animals we sell from pastures near to the deliveryArea or near to the prefered source if one is specified
            Location getItemsNear = deliveryArea.ActionLocation;
            if (m_preferedSource != null)
            {
                getItemsNear = m_preferedSource.ActionLocation;
            }

            //the worker to have sell something next
            int workerNum = 0;

            //keep selling loads until none are left
            List<Animal> toSell = animalDivider.NextLoad();
            while (toSell != null)
            {
                //plan a trip
                PlanTrip(plan, animalPlanner, workerNum, getItemsNear, deliveryArea, toSell);
                
                //have the next worker find something to sell
                workerNum++;
                if (workerNum >= m_numberOfWorkers)
                {
                    workerNum = 0;
                }

                //figure out what the next worker will be selling
                toSell = animalDivider.NextLoad();
            }
            
            return plan;
        }





        /// <summary>
        /// Plan a trip for selling animals
        /// </summary>
        private void PlanTrip(TaskPlan plan, AnimalPlacementPlanner animalPlanner, int workerNum, Location getItemsNear, DeliveryArea deliveryArea, List<Animal> toSell)
        {
            //use the animal planner to plan locations to get the animal that we will sell
            animalPlanner.PlanToGetAnimals(plan, workerNum, toSell, getItemsNear);

            //create action to disguard the animals we are selling at the delivery area (we dont want to just put them back in our delivery area) and add to the plan
            DisgardAnimalsAction disgardItemsAction = new DisgardAnimalsAction(deliveryArea.ActionLocation, toSell);
            plan.AddAction(workerNum, disgardItemsAction);
        }













        /// <summary>
        /// List of the items that still need to be sold
        /// Used so that if the task is aborted the animals that were not sold can be refunded.
        /// </summary>
        private List<Animal> m_leftToSell = new List<Animal>();

        /// <summary>
        /// The amount made for each animal type when the task started
        /// </summary>
        private Dictionary<Animal, int> m_amountMadeForAnimal = new Dictionary<Animal, int>();

        /// <summary>
        /// After the task is setup, add the items to the store inventory, and get the money for the items
        /// </summary>
        protected override void DoneWithSetupInner()
        {
            base.DoneWithSetupInner();

            //start the left to sell list with everything
            m_leftToSell.AddRange(m_whatToSell);

            //get the money for the animals being sold, and remeber how much we got for each item type
            foreach (Animal animal in m_whatToSell)
            {
                //determine the current cost of the animal, and remeber the amount it was sold for
                int animalCost = Program.Game.Prices.GetPrice(animal.AnimalItemType);
                m_amountMadeForAnimal.Add(animal, animalCost);
                
                //get money for the amount we sold
                Program.Game.Treasury.Sell(SpendingCatagory.ItemSales, animalCost);
            }
        }


        public override void ActionFinished(ActionBase<Worker> action)
        {
            base.ActionFinished(action);

            //if it was a disguard animals action (we got rid of animals we were selling), then remove from left to sell.  and add the item to the stores inventory
            if (action is DisgardAnimalsAction)
            {
                List<Animal> animalsJustSold = (action as DisgardAnimalsAction).ToDisguard;
                foreach (Animal animalTypeSold in animalsJustSold)
                {
                    //remove the animal from the left to sell list
                    m_leftToSell.Remove(animalTypeSold);

                    //add to store inventory
                    Program.Game.Store.Animals.Add(animalTypeSold);
                }
            }
        }

        protected override void AfterAborted(bool wasStarted)
        {
            //all the animals we never sold we should give back the money we got for them
            foreach (Animal animal in m_leftToSell)
            {
                //refund the animal
                int amountMadeForAnimal = m_amountMadeForAnimal[animal];
                Program.Game.Treasury.Buy(SpendingCatagory.ItemSales, amountMadeForAnimal);
            }
        }






        public override string LongDescription()
        {
            string description = "Sell Animals";
            
            if (m_preferedSource == null)
            {
                description += " get from nearest building.";
            }
            else
            {
                description += " get from " + m_preferedSource.Name + ".";
            }

            return description;
        }

        public override string ShortDescription()
        {
            string description = "Sell Animals";
            
            return description;
        }

        public override Color TaskColor()
        {
            return Color.Black;
        }


        


        
        public override void WriteState(ObjectState state)
        {
            base.WriteState(state);
            state.SetValue("PreferedSource", m_preferedSource);
            state.SetListValues<Animal>("WhatToSell", m_whatToSell);
            state.SetListValues<Animal>("LeftToSellList", m_leftToSell);

            state.SetDictionaryValues<Animal,int>("PricesPaidDictionary", m_amountMadeForAnimal);            
        }

        public override void ReadState(ObjectState state)
        {
            base.ReadState(state);
            m_preferedSource = state.GetValue<Pasture>("PreferedSource");
            m_whatToSell = state.GetListValues<Animal>("WhatToSell");
            m_leftToSell = state.GetListValues<Animal>("LeftToSellList");

            m_amountMadeForAnimal = state.GetDictionaryValues<Animal, int>("PricesPaidDictionary");
        }
    }
}
