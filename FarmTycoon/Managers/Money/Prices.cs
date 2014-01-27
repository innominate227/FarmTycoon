using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// Types of things that you can set the price of
    /// </summary>
    public enum PriceType
    {
        Item,
        Worker,
        Road,
        Highway,
        FieldFence,
        PastureFence,
        LandRaise,
        LandBuy,
        StorageBuilding,
        ProductionBuilding,
        Trough,
        Scenery,
        BreakHouse,
    }


    /// <summary>
    /// Manages the prices of everythingin the game
    /// </summary>
    public class Prices : ISavable
    {
        #region Events

        /// <summary>
        /// Event raised any time the price of anything changes
        /// </summary>
        public event Action PriceChanged;

        #endregion
        
        #region Member Vars

        /// <summary>
        /// The price for items / buildings / scenery / other in the game.
        /// The key is the unique name of the IInfo object.  
        /// For Items the quality is appened to the unique name.
        /// For Other prices its "OtherPrice_" + the name of the other price.
        /// </summary>
        private Dictionary<string, int> _prices = new Dictionary<string, int>();


        /// <summary>
        /// True if the preice of at least one object / item has changed since the prices were last checked
        /// This does not need to be saved since only UIs are interested in the event, and UI state is not saved.
        /// </summary>
        private bool _pricesChangedSinceLastCheck = false;

        #endregion
        
        #region Logic

        /// <summary>
        /// This should be called each day after the scenario script.
        /// If any prices changed because of the scenario script this will raise the price changed event.
        /// </summary>
        public void CheckForChangesAfterScript()
        {
            if (_pricesChangedSinceLastCheck)
            {
                if (PriceChanged != null)
                {
                    PriceChanged();
                }
                _pricesChangedSinceLastCheck = false;
            }
        }




        /// <summary>
        /// Set the price of an item/object/other
        /// </summary>
        public void SetPrice(PriceType priceType, int value)
        {
            SetPrice(priceType, "", value);
        }

        /// <summary>
        /// Set the price of an item/object/other
        /// </summary>
        public void SetPrice(PriceType priceType, string priceName, int value)
        {
            string priceKey = priceType.ToString() + "_" + priceName;
            if (_prices.ContainsKey(priceKey) == false) { _prices.Add(priceKey, 0); }
            _prices[priceKey] = value;
            _pricesChangedSinceLastCheck = true;
        }



        /// <summary>
        /// Set the price of an item/object/other
        /// </summary>
        public int GetPrice(PriceType priceType)
        {
            return GetPrice(priceType, "");
        }

        /// <summary>
        /// Get the price of an item/object/other in the Prices
        /// </summary>
        public int GetPrice(PriceType priceType, string priceName)
        {
            string priceKey = priceType.ToString() + "_" + priceName;
            if (_prices.ContainsKey(priceKey) == false) { _prices.Add(priceKey, 0); }
            return _prices[priceKey];
        }



        #region Items


        /// <summary>
        /// Set the price of an item (that does not have multiple quality levels)
        /// </summary>
        public void SetPrice(ItemTypeInfo itemType, int price)
        {            
            SetPrice(itemType, 0, price);
        }

        /// <summary>
        /// Set the price of an item
        /// </summary>
        public void SetPrice(ItemTypeInfo itemType, int quality, int price)
        {
            string priceName = itemType.UniqueName + "_" + quality.ToString();
            SetPrice(PriceType.Item, priceName, price);
        }
                
        /// <summary>
        /// Get the price of an item
        /// </summary>
        public int GetPrice(ItemTypeInfo itemType, int quality)
        {
            string priceName = itemType.UniqueName + "_" + quality.ToString();
            return GetPrice(PriceType.Item, priceName);
        }

        /// <summary>
        /// Get the price of an item
        /// </summary>
        public int GetPrice(ItemType itemType)
        {
            return GetPrice(itemType.BaseType, itemType.Quality);
        }

        #endregion

        #region Objects
        
        /// <summary>
        /// Set the price of a gameobject
        /// </summary>
        public void SetPrice(IPloppableInfo objectType, int price)
        {
            SetPrice(objectType.PriceType, objectType.Name, price);
        }

        /// <summary>
        /// Get the price of a gameobject
        /// </summary>
        public int GetPrice(IPloppableInfo objectType)
        {
            return GetPrice(objectType.PriceType, objectType.Name);
        }


        #endregion

        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteInt(_prices.Count);
            foreach (string key in _prices.Keys)
            {
                writer.WriteString(key);
                writer.WriteInt(_prices[key]);
            }
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            int count = reader.ReadInt();
            for (int num = 0; num < count; num++)
            {
                string key = reader.ReadString();
                int val = reader.ReadInt();
                _prices.Add(key, val);
            }
        }

        public void AfterReadStateV1()
        {
        }
        #endregion

    }
}
