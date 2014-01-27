using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Creates the correct event given a string representation of the event
    /// </summary>
    public class EventFactory
    {
        /// <summary>
        /// Create an event given the string for the event.  (Does not handel the special Choose event, or special IF event)
        /// </summary>
        public ScriptEvent Create(string eventString)
        {
            //get the name of the event
            string eventName = eventString.Substring(0, eventString.IndexOf('(')).Trim().ToUpper();

            //get params in a string[] and trim extra spaces
            string actionParamsText = eventString.Substring(eventString.IndexOf('(') + 1).Trim();
            actionParamsText = actionParamsText.Substring(0, actionParamsText.Length - 1);
            string[] actionParams = actionParamsText.Split2(',', '\\');
            for (int paramNum = 0; paramNum < actionParams.Length; paramNum++)
            {
                actionParams[paramNum] = actionParams[paramNum].Trim();
            }

            //if we have one empty string paramter, we actualy had no parameters
            if (actionParams.Length == 1 && actionParams[0].Trim() == "")
            {
                actionParams = new string[0];
            }

            //create the correct event
            ScriptEvent newEvent = null;
            if (eventName == SetPriceEvent.NAME)
            {
                newEvent = new SetPriceEvent(actionParams);
            }
            else if (eventName == AdjustPriceEvent.NAME)
            {
                newEvent = new AdjustPriceEvent(actionParams);
            }
            else if (eventName == WeatherEvent.NAME)
            {
                newEvent = new WeatherEvent(actionParams);
            }
            else if (eventName == SprayEvent.NAME)
            {
                newEvent = new SprayEvent(actionParams);
            }
            else if (eventName == StoreStockEvent.NAME)
            {
                newEvent = new StoreStockEvent(actionParams);
            }
            else if (eventName == MessageEvent.NAME)
            {
                newEvent = new MessageEvent(actionParams);
            }
            else if (eventName == TreasuryEvent.NAME)
            {
                newEvent = new TreasuryEvent(actionParams);
            }
            else if (eventName == MakeAvailableEvent.NAME)
            {
                newEvent = new MakeAvailableEvent(actionParams);
            }
            else if (eventName == WinEvent.NAME)
            {
                newEvent = new WinEvent(actionParams);
            }
            else if (eventName == LoseEvent.NAME)
            {
                newEvent = new LoseEvent(actionParams);
            }
            else if (eventName == AddToBuildingInventoryEvent.NAME)
            {
                newEvent = new AddToBuildingInventoryEvent(actionParams);
            }
            else if (eventName == NothingEvent.NAME)
            {
                newEvent = new NothingEvent(actionParams);
            }
            else if (eventName == SetVariableEvent.NAME)
            {
                newEvent = new SetVariableEvent(actionParams);
            }
            else
            {
                Debug.Assert(false);
            }

            //return the newly created event
            return newEvent;
        }

    }
}
