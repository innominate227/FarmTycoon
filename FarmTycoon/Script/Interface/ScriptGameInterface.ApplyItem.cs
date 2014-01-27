using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public partial class ScriptGameInterface
    {

        public void ApplyItemToAll(string itemName)
        {
            ApplyItemToWorkers(itemName);
            ApplyItemToEquipment(itemName);
            ApplyItemToCrops(itemName);
            ApplyItemToLand(itemName);
            ApplyItemToAnimals(itemName);
        }


        public void ApplyItemToWorkers(string itemName)
        {
            //get the type to apply
            ItemTypeInfo typeToSpray = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);

            //apply on to all workers
            foreach (Worker worker in GameState.Current.MasterObjectList.FindAll<Worker>())
            {
                worker.Traits.ApplyItemToTraits(typeToSpray);
            }
        }

        public void ApplyItemToEquipment(string itemName)
        {
            //get the type to apply
            ItemTypeInfo typeToSpray = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);

            //apply on to all equipmnet
            foreach (Equipment equipment in GameState.Current.MasterObjectList.FindAll<Equipment>())
            {
                equipment.Traits.ApplyItemToTraits(typeToSpray);
            }
        }

        public void ApplyItemToCrops(string itemName)
        {
            //get the type to apply
            ItemTypeInfo typeToSpray = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);

            //spray on to all crops
            foreach (Crop crop in GameState.Current.MasterObjectList.FindAll<Crop>())
            {
                crop.Traits.ApplyItemToTraits(typeToSpray);
            }
        }
        
        public void ApplyItemToLand(string itemName)
        {
            //get the type to apply
            ItemTypeInfo typeToSpray = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);

            //spray on to all animals
            foreach (Animal animal in GameState.Current.MasterObjectList.FindAll<Animal>())
            {
                animal.Traits.ApplyItemToTraits(typeToSpray);
            }
        }
        
        public void ApplyItemToAnimals(string itemName)
        {
            //get the type to apply
            ItemTypeInfo typeToSpray = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);

            //spray on to all land
            foreach (Land land in GameState.Current.MasterObjectList.FindAll<Land>())
            {
                land.Traits.ApplyItemToTraits(typeToSpray);
            }
        }







        public void ApplyItemToSomeWorkers(string itemName, int count)
        {
            //get the type to apply
            ItemTypeInfo typeToSpray = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);

            //apply to count workers
            List<Worker> worker = GameState.Current.MasterObjectList.FindAll<Worker>();
            for(int i=0; i<count; i++)
            {                
                worker[Program.Game.Random.Next(worker.Count)].Traits.ApplyItemToTraits(typeToSpray);
            }
        }

        public void ApplyItemToSomeEquipment(string itemName, int count)
        {
            //get the type to apply
            ItemTypeInfo typeToSpray = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);

            //apply to count equipmnet
            List<Equipment> equipment = GameState.Current.MasterObjectList.FindAll<Equipment>();
            for (int i = 0; i < count; i++)
            {
                equipment[Program.Game.Random.Next(equipment.Count)].Traits.ApplyItemToTraits(typeToSpray);
            }
        }

        public void ApplyItemToSomeCrops(string itemName, int count)
        {
            //get the type to apply
            ItemTypeInfo typeToSpray = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);

            //apply to count  crops
            List<Crop> crops = GameState.Current.MasterObjectList.FindAll<Crop>();
            for(int i=0; i<count; i++)
            {                
                crops[Program.Game.Random.Next(crops.Count)].Traits.ApplyItemToTraits(typeToSpray);
            }
        }

        public void ApplyItemToSomeLand(string itemName, int count)
        {
            //get the type to apply
            ItemTypeInfo typeToSpray = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);

            //apply to count  animals
            List<Animal> animals = GameState.Current.MasterObjectList.FindAll<Animal>();
            for (int i = 0; i < count; i++)
            {
                animals[Program.Game.Random.Next(animals.Count)].Traits.ApplyItemToTraits(typeToSpray);
            }
        }

        public void ApplyItemToSomeAnimals(string itemName, int count)
        {
            //get the type to apply
            ItemTypeInfo typeToSpray = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);

            //apply to count  land
            List<Land> lands = GameState.Current.MasterObjectList.FindAll<Land>();
            for (int i = 0; i < count; i++)
            {
                lands[Program.Game.Random.Next(lands.Count)].Traits.ApplyItemToTraits(typeToSpray);
            }
        }



    }
}
