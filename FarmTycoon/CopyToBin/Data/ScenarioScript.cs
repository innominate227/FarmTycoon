using System;
using System.IO;
using FarmTycoon;

/*
	
	Methods for adjusting the amount of items available for purchase at the store.        
	The item name is the name from the FarmData file.
    For items with SubType of "One" just pass the name.
    For items with SubType of "Qualities" also pass the quality to adjust.
    For items with SubType of "Many" pass the unique name for the many item.
    Setting an amount to less than 0 will cause the amount to be 0, for unique items setting an amount greater than 1 will cause the amount to be 1.
	The "GetStoreAmountAll" works for all SubTypes it gets all the items of the type passed regardless of quality/unique name
    The Adjust methods return the new amount after adjustment   
    For Animal/Equipment items you need to set the Quality and Age of the item after adding it to the store (or they will always be 0)
       
	void SetStoreAmount(string itemName, int amount)
    void SetStoreAmount(string itemName, int quality, int amount)
    void SetStoreAmount(string itemName, string uniqueName, int amount)
        	
	int GetStoreAmount(string itemName)
    int GetStoreAmount(string itemName, int quality)
    int GetStoreAmount(string itemName, string uniqueName)
    int GetStoreAmountAll(string itemName)
    	
	int AdjustStoreAmount(string itemName, int adjustment)
    int AdjustStoreAmount(string itemName, int quality, int adjustment)
    int AdjustStoreAmount(string itemName, string uniqueName, int adjustment)
    
    void SetItemQuality(string itemName, string uniqueName, int quality)
    void SetItemAge(string itemName, string uniqueName, int age)
    
    
    
    Methods for getting or adjusting how many of an item a player has
    The rules for the Get functions are just like for the ones that get store amount
    The Total amount is the amount in any building/pasture/held by worker/purchased and in the delivery area waiting to be picked up.  Anywhere but in the store, not yet bought.
    The normal amount is the amount that is in a storage building / pasture and not reserved by a task that is under way (amount that could be sold/removed right now).
    Setting or Adjusting the players amount will add/remove items to/from random Storeage Buildings or Pastures that can take the items
    If the items can not be added or removed they will not be.  This includes items that are reserved.
    The amount the player has after setting/adjustment will be returned
    GetPlayerRoomForItem returns the number of that item that could be added to the players current buildings
		
	int GetPlayerTotalAmount(string itemName)
    int GetPlayerTotalAmount(string itemName, int quality)
    int GetPlayerTotalAmount(string itemName, string uniqueName)
    int GetPlayerTotalAmountAll(string itemName, string uniqueName)
    	
	int GetPlayerAmount(string itemName)
    int GetPlayerAmount(string itemName, int quality)
    int GetPlayerAmount(string itemName, string uniqueName)
    int GetPlayerAmountAll(string itemName, string uniqueName)
    
	int AdjustPlayerAmount(string itemName, int adjustment)
    int AdjustPlayerAmount(string itemName, int quality, int adjustment)
    int AdjustPlayerAmount(string itemName, string uniqueName, int adjustment)
    int AdjustPlayerAmountAll(string itemName, int adjustment)
    
	int SetPlayerAmount(string itemName, int amount)
    int SetPlayerAmount(string itemName, int quality, int amount)
    int SetPlayerAmount(string itemName, string uniqueName, int amount)
    int AdjustPlayerAmountAll(string itemName, int amount)
    
    int GetPlayerRoomForItem(string itemName)
    
        
        
	Methods for setting, adjusting, or getting the price of an item in the store.
    Price is based on item name and quality (if the item has quality)
    
    void SetItemPrice(string itemName, int price)
    void SetItemPrice(string itemName, int itemQuality, int price)     
	
    int GetItemPrice(string itemName)
    int GetItemPrice(string itemName, int itemQuality)    

    int AdjustItemPrice(string itemName, int adjustment)
    int AdjustItemPrice(string itemName, int itemQuality, int adjustment)    


    Methods for setting, adjusting or getting the price of game objects.    
    The building name is the name of the building from the FarmData file.
    Setting a price to -1 will make it so the player is unable to build that object
            
    void SetLandBuyPrice(int price)
    void SetLandRisePrice(int price)
    void SetFieldFencePrice(int price)
    void SetPastureFencePrice(int price)
    void SetRoadPrice(int price)
    void SetWorkerPrice(int price)      
    void SetProductionBuildingPrice(string buildingName, int price)
    void SetStorageBuildingPrice(string buildingName, int price)
    void SetTroughPrice(string buildingName, int price)
    void SetSceneryPrice(string buildingName, int price)
        
    int GetXXXPrice()    	
    int GetXXXPrice(string buildingName)    	
    
	int AdjustXXXPrice(int adjustment)        
    int AdjustXXXPrice(string buildingName, int adjustment)        
    
	
    
    --------------
	
	Methods to determine what a player has built.
	
    for field and crop you can optionally pass a crop name to get only field planted with that crop
    for pasture you can optionally pass an animal name to get only pasture containing that animal
    for the building you can get the count for all storage buildings or all storage building of a certain type
     
    int GetOwnedLandCount()
    int GetWorkerCount()
    int GetAvailableWorkerCount()
    int GetRoadCount()
    int GetFieldCount()
    int GetCropCount()
    int GetPastureCount()
    int GetProductionBuildingCount()
    int GetStorageBuildingCount()
    int GetTroughCount()
    int GetSceneryCount()
    
    int GetFieldCount(string seedName)
    int GetCropCount(string seedName)
    int GetPastureCount(string animalName)
    int GetProductionBuildingCount(string buildingName)
    int GetStorageBuildingCount(string buildingName)
    int GetTroughCount(string buildingName)
    int GetSceneryCount(string buildingName)
    
						
	
	
	Methods for Adjusting/Getting player money amount
	Buy make the player loose the cost, and sell makes them gain the profit.
    Catagory and Subcatagory are seen when the player looks at their finacial statement.
    The game uses "Items", and item name when the player sells or buys at the store.
    It uses "Construction" and building name when the player builds things or buys/raises land
    			
    int GetCurrentMoney()
    int Buy(string catagory, string subCatagory, int cost)
    int Sell(string catagory, string subCatagory, int profit)
	
    
    
	
	
	Apply item passed to all workers/equipment/plants/animal/land/all
    Or randomly apply the item pass to some workers/equipment/plants/animal/land
	
    void ApplyItemToWorkers(string itemName)
	void ApplyItemToEquipment(string itemName)
	void ApplyItemToLand(string itemName)
    void ApplyItemToCrops(string itemName)
	void ApplyItemToAnimals(string itemName)
	void ApplyItemToAll(string itemName)
	
    void ApplyItemToSomeWorkers(string itemName, int count)
	void ApplyItemToSomeEquipment(string itemName, int count)
	void ApplyItemToSomeLand(string itemName, int count)
    void ApplyItemToSomeCrops(string itemName, int count)
	void ApplyItemToSomeAnimals(string itemName, int count)
	
    
        
        
	
	Methods for UI interface
	Weather just sets the string that show in the UI.  To apply wether you should use ApplyItemToAll.
	Vistory progress is also just a string that show in the UI.  It should tell the user how close they are to meeting the win conditions. 
	Message shows a message box to the user, and optionaly pauses the game until they close it.
    Choice shows a message box to the user, where they can choose between two options (default yes no), and optionaly pauses the game until they close it.
	Win marks the scenario as won.  You should still show a message telling them they won.
    You should also show a message telling them they have lost, if there is a time limit or some loosing condition.
	
	void SetWeather(string weatherDescription)
	void SetVictoryProgress(string vitoryProgress)
	void ShowMessage(int width, int height, string title, string message)
	void ShowMessage(int width, int height, string title, string message, bool pause)
	void ShowChoice(int width, int height, string title, string message, Action yesAction, Action noAction)
	void ShowChoice(int width, int height, string title, string message, string yesLabel, string noLabel, Action yesAction, Action noAction)
    void ShowChoice(int width, int height, string title, string message, string yesLabel, string noLabel, Action yesAction, Action noAction, bool pause)
	void Win()
	
        
	
	
*/

namespace FarmScript
{   
   public class Script : IScenarioScript
   {
		Random rnd = new Random();
   
		public void DoScript(int day, ScriptGameInterface game)
		{				
			if (day == 0)
			{
				FirstDay(game);
			}
			EveryDay(game);
			if (day % 30 == 0)
			{
				EveryMonth(game);
			}
		}
		
		private void FirstDay(ScriptGameInterface game)
		{
        /*
			game.AdjustTreasury(999999, SpendingCatagory.IncomeEvemts);
	
			game.MakeObjectAvailable("StorageBuilding","Barn");
			game.MakeObjectAvailable("StorageBuilding","Silo");
			game.MakeObjectAvailable("ProductionBuilding","Well");
			game.MakeObjectAvailable("Scenery","Tree");
            
			game.SetItemPrice("Wheat Seed", 0, 10);
			game.SetItemPrice("Fertilizer", 0, 20);
			
			game.SetItemPrice("Slow Tractor", 0, 10000);
			game.SetItemPrice("Slow Plow", 0, 10000);
			game.SetItemPrice("Slow Sprayer", 0, 10000);	
			game.SetItemPrice("Slow Planter", 0, 10000);
			game.SetItemPrice("Slow Harvester", 0, 10000);	
			game.SetItemPrice("Small Trailer", 0, 10000);			
            
			game.SetObjectPrice("StorageBuilding", "Barn", 500);
			game.SetObjectPrice("StorageBuilding", "Shed", 800);
			game.SetObjectPrice("StorageBuilding", "Silo", 500);
			game.SetObjectPrice("ProductionBuilding", "Well", 600);
			game.SetObjectPrice("Scenery", "Tree", 10);
			
			game.SetOtherPrice(OtherPrice.Worker, 200);
			game.SetOtherPrice(OtherPrice.Road, 50);
			game.SetOtherPrice(OtherPrice.Fence, 50);
			game.SetOtherPrice(OtherPrice.LandRaise, 50);
			game.SetOtherPrice(OtherPrice.LandBuy, 1000);
			game.SetOtherPrice(OtherPrice.WorkerSalary, 200);
			game.SetOtherPrice(OtherPrice.LandTax, 100);
			
			game.SetStock("Apple_5", 1000);
			game.SetStock("Wheat Seed_0", 1000);
			game.SetStock("Apple Seed_0", 1000);
			game.SetStock("Fertilizer_0", 1000);
			
			game.SetStock("Slow Tractor_1", 1);	
			game.SetStock("Slow Tractor_2", 1);	
			game.SetStock("Slow Tractor_3", 1);	
			
			game.SetStock("Slow Plow_1", 1);		
			game.SetStock("Slow Plow_2", 1);		
			game.SetStock("Slow Plow_3", 1);		
			
			game.SetStock("Slow Sprayer_1", 1);
			game.SetStock("Slow Sprayer_2", 1);
			game.SetStock("Slow Sprayer_3", 1);
			
			game.SetStock("Slow Planter_1", 1);
			game.SetStock("Slow Planter_2", 1);
			game.SetStock("Slow Planter_3", 1);
						
			game.SetStock("Small Trailer_1", 1);
			game.SetStock("Small Trailer_2", 1);
			game.SetStock("Small Trailer_3", 1);
			
			game.SetStock("Slow Harvester_1", 1);
			game.SetStock("Slow Harvester_2", 1);
			game.SetStock("Slow Harvester_3", 1);
			
			game.SetStock("Cow_1", 1);
			game.SetStock("Cow_2", 1);
			game.SetStock("Cow_3", 1);
			game.SetStock("Cow_4", 1);
			game.SetStock("Cow_5", 1);
			game.SetStock("Cow_6", 1);
			game.SetStock("Cow_7", 1);
			game.SetStock("Cow_8", 1);
			game.SetStock("Cow_9", 1);
			game.SetStock("Cow_10", 1);
            
            */
		}
				
		private void EveryDay(ScriptGameInterface game)
		{
        /*
			game.AdjustStock("Wheat Seed_0", rnd.Next(-5,1));
			game.AdjustStock("Fertilizer_0", rnd.Next(-5,1));

			AdjustItemPriceWithRange(game, "Wheat Seed", 0, rnd.Next(-2,3), 8,  12);
			AdjustItemPriceWithRange(game, "Fertilizer", 0, rnd.Next(-2,3), 18,  22);	
			
			int randomWeather = rnd.Next(3);
			if (randomWeather == 0)
			{
				game.Weather("Rain");
				game.ApplyItemToAll("Rain");
			}
			else if (randomWeather == 1)
			{
				game.Weather("Sunny");		
				game.ApplyItemToAll("Sunlight");	
			}
			else if (randomWeather == 2)
			{
				game.Weather("Cloudy");			
			}
            */
		}
		
		private void EveryMonth(ScriptGameInterface game)
		{
        /*
			game.AdjustStock("Wheat Seed_0", 500);
			game.AdjustStock("Fertilizer_0", 500);

			if (game.GetStock("Wheat Seed_0") < 1000)
			{
				game.AdjustStock("Wheat Seed_0", 500);
			}
		*/
		}
		
		
		
		
		private void AdjustItemPriceWithRange(ScriptGameInterface game, string itemName, int quality, int adjustment, int minValue, int maxValue)
		{
			game.AdjustItemPrice(itemName, quality, adjustment);
			int itemPrice = game.GetItemPrice(itemName, quality);
			if (itemPrice > maxValue)
			{
				game.SetItemPrice(itemName, quality, maxValue);
			}
			else if (itemPrice < minValue)
			{
				game.SetItemPrice(itemName, quality, minValue);
			}		
		}
		
		
		#region Save and Load State
		
		//you should not need to modify these methods, they currently get and save the value of all public and private member variables used in the script
				
		public string SaveState()
		{		
            string state = "";
            foreach(System.Reflection.FieldInfo fieldInfo in this.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public))
            {
				object value = fieldInfo.GetValue(this);
				string valString="";
				if (Type.GetTypeCode(fieldInfo.FieldType) == TypeCode.Double)                    
                {                    
                    valString = ((double)value).ToString("R"); //R forces it to convert to string with max percision
                }
                else if (Type.GetTypeCode(fieldInfo.FieldType) == TypeCode.Single)
                {
                    valString = ((float)value).ToString("R");
                }
                else if (Type.GetTypeCode(fieldInfo.FieldType) == TypeCode.String)
                {
                    //replace new lines with special newline strings (
                    valString = ((string)value).Replace("\r\n", "**NEWLINE2**").Replace("\n", "**NEWLINE2**");
                }
                else if (fieldInfo.FieldType.IsValueType)
                {                    
                    valString = (string)Convert.ChangeType(value, typeof(string));
                }
                else
                {
                    continue;
                }			
                state += (fieldInfo.Name + "=" + valString + "\r\n");
            }
			return state;
		}
		
		public void LoadState(string state)
		{		
			foreach (string stateLine in state.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                string fieldName = stateLine.Substring(0, stateLine.IndexOf('='));
                string valString = stateLine.Substring(stateLine.IndexOf('=')+1);
                System.Reflection.FieldInfo fieldInfo = this.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

                object fieldValue = null;
                if (Type.GetTypeCode(fieldInfo.FieldType) == TypeCode.String)
                {
                    //replace "**NEWLINE2**" with new lines
                    fieldValue = valString.Replace("**NEWLINE2**", "\r\n");
                }
                else
                {
                    fieldValue = Convert.ChangeType(valString, fieldInfo.FieldType);                
                }

                fieldInfo.SetValue(this, fieldValue);
            }
		}
		
		#endregion
		
   }
}





