using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    public partial class ScriptGameInterface
    {


        public void SetItemPrice(string itemName, int price)
        {
            SetItemPrice(itemName, 0, price);
        }
        public void SetItemPrice(string itemName, int itemQuality, int price)
        {
            ItemTypeInfo item = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);
            GameState.Current.Prices.SetPrice(item, itemQuality, price);
        }


        public int GetItemPrice(string itemName)
        {
            return GetItemPrice(itemName, 0);
        }
        public int GetItemPrice(string itemName, int itemQuality)
        {
            ItemTypeInfo item = (ItemTypeInfo)FarmData.Current.GetInfo(ItemTypeInfo.UNIQUE_PREPEND + itemName);
            return GameState.Current.Prices.GetPrice(item, itemQuality);
        }


        public int AdjustItemPrice(string itemName, int adjustment)
        {
            SetItemPrice(itemName, GetItemPrice(itemName) + adjustment);
            return GetItemPrice(itemName);
        }
        public int AdjustItemPrice(string itemName, int itemQuality, int adjustment)
        {
            SetItemPrice(itemName, itemQuality, GetItemPrice(itemName, itemQuality) + adjustment);
            return GetItemPrice(itemName, itemQuality);
        }








        public void SetLandBuyPrice(int price)
        {
            GameState.Current.Prices.SetPrice(PriceType.LandBuy, price);
        }
        public void SetLandRaisePrice(int price)
        {
            GameState.Current.Prices.SetPrice(PriceType.LandRaise, price);
        }
        public void SetFieldFencePrice(int price)
        {
            GameState.Current.Prices.SetPrice(PriceType.FieldFence, price);
        }
        public void SetPastureFencePrice(int price)
        {
            GameState.Current.Prices.SetPrice(PriceType.PastureFence, price);
        }
        public void SetRoadPrice(int price)
        {
            GameState.Current.Prices.SetPrice(PriceType.Road, price);
        }
        public void SetWorkerPrice(int price)
        {
            GameState.Current.Prices.SetPrice(PriceType.Worker, price);
        }
        public void SetProductionBuildingPrice(string buildingName, int price)
        {
            GameState.Current.Prices.SetPrice(PriceType.ProductionBuilding, buildingName, price);
        }
        public void SetStorageBuildingPrice(string buildingName, int price)
        {
            GameState.Current.Prices.SetPrice(PriceType.StorageBuilding, buildingName, price);
        }
        public void SetTroughPrice(string buildingName, int price)
        {
            GameState.Current.Prices.SetPrice(PriceType.Trough, buildingName, price);
        }
        public void SetSceneryPrice(string buildingName, int price)
        {
            GameState.Current.Prices.SetPrice(PriceType.Scenery, buildingName, price);
        }




        public int GetLandBuyPrice()
        {
            return GameState.Current.Prices.GetPrice(PriceType.LandBuy);
        }
        public int GetLandRaisePrice()
        {
            return GameState.Current.Prices.GetPrice(PriceType.LandRaise);
        }
        public int GetFieldFencePrice()
        {
            return GameState.Current.Prices.GetPrice(PriceType.FieldFence);
        }
        public int GetPastureFencePrice()
        {
            return GameState.Current.Prices.GetPrice(PriceType.PastureFence);
        }
        public int GetRoadPrice()
        {
            return GameState.Current.Prices.GetPrice(PriceType.Road);
        }
        public int GetWorkerPrice()
        {
            return GameState.Current.Prices.GetPrice(PriceType.Worker);
        }
        public int GetProductionBuildingPrice(string buildingName)
        {
            return GameState.Current.Prices.GetPrice(PriceType.ProductionBuilding, buildingName);
        }
        public int GetStorageBuildingPrice(string buildingName)
        {
            return GameState.Current.Prices.GetPrice(PriceType.StorageBuilding, buildingName);
        }
        public int GetTroughPrice(string buildingName)
        {
            return GameState.Current.Prices.GetPrice(PriceType.Trough, buildingName);
        }
        public int GetSceneryPrice(string buildingName)
        {
            return GameState.Current.Prices.GetPrice(PriceType.Scenery, buildingName);
        }





        public int AdjustLandBuyPrice(int adjustment)
        {
            SetLandBuyPrice(GetLandBuyPrice() + adjustment);
            return GetLandBuyPrice();
        }
        public int AdjustLandRaisePrice(int adjustment)
        {
            SetLandRaisePrice(GetLandRaisePrice() + adjustment);
            return GetLandRaisePrice();
        }
        public int AdjustFieldFencePrice(int adjustment)
        {
            SetFieldFencePrice(GetFieldFencePrice() + adjustment);
            return GetFieldFencePrice();
        }
        public int AdjustPastureFencePrice(int adjustment)
        {
            SetPastureFencePrice(GetPastureFencePrice() + adjustment);
            return GetPastureFencePrice();
        }
        public int AdjustRoadPrice(int adjustment)
        {
            SetRoadPrice(GetRoadPrice() + adjustment);
            return GetRoadPrice();
        }
        public int AdjustWorkerPrice(int adjustment)
        {
            SetWorkerPrice(GetWorkerPrice() + adjustment);
            return GetWorkerPrice();
        }
        public int AdjustProductionBuildingPrice(string buildingName, int adjustment)
        {
            SetProductionBuildingPrice(buildingName, GetProductionBuildingPrice(buildingName) + adjustment);
            return GetProductionBuildingPrice(buildingName);
        }
        public int AdjustStorageBuildingPrice(string buildingName, int adjustment)
        {
            SetStorageBuildingPrice(buildingName, GetStorageBuildingPrice(buildingName) + adjustment);
            return GetStorageBuildingPrice(buildingName);
        }
        public int AdjustTroughPrice(string buildingName, int adjustment)
        {
            SetTroughPrice(buildingName, GetTroughPrice(buildingName) + adjustment);
            return GetTroughPrice(buildingName);
        }
        public int AdjustSceneryPrice(string buildingName, int adjustment)
        {
            SetSceneryPrice(buildingName, GetSceneryPrice(buildingName) + adjustment);
            return GetSceneryPrice(buildingName);
        }





        public int GetCurrentMoney()
        {
            return GameState.Current.Treasury.CurrentMoney;
        }        
        public void Buy(string catagory, string subCatagory, int cost)
        {
            GameState.Current.Treasury.Buy(catagory, subCatagory, cost);
        }
        public void Sell(string catagory, string subCatagory, int profit)
        {
            GameState.Current.Treasury.Sell(catagory, subCatagory, profit);
        }
        
        
    }
}
