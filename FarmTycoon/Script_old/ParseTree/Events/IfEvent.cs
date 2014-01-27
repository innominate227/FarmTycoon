using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Special event that causes on of a list of event to be chosen from and executed
    /// </summary>
    public class IfEvent : ScriptEvent
    {
        private enum Comparator
        {
            Equals,
            NotEquals,
            GreaterThan,
            LessThan,
            GreaterThanOrEqual,
            LessThanOrEqual,
        }


        /// <summary>
        /// Name of the event in the script file
        /// </summary>
        public const string NAME = "IF";

        /// <summary>
        /// Region to run if the condition is true
        /// </summary>
        private ScriptRegion m_region;

        /// <summary>
        /// Var on the left side of the if
        /// </summary>
        private ScriptVar m_leftVar;

        /// <summary>
        /// Var on the right side of the if
        /// </summary>
        private ScriptVar m_rightVar;

        /// <summary>
        /// Comparator to use
        /// </summary>
        private Comparator m_comparator;

        /// <summary>
        /// Create a supply evnet
        /// </summary>
        public IfEvent(string ifEvent)
        {
            string firstLine = ifEvent.Substring(0, ifEvent.IndexOf("{") - 1);
            string otherLines = ifEvent.Substring(ifEvent.IndexOf("{"));
            
            string ifSection = firstLine.Substring(firstLine.IndexOf("(") + 1);
            ifSection = ifSection.Substring(0, ifSection.LastIndexOf(")"));

            string[] ifSides = null;            

            if (ifSection.Contains("<="))
            {
                m_comparator = Comparator.LessThanOrEqual;
                ifSides = ifSection.Split(new string[] { "<=" }, StringSplitOptions.RemoveEmptyEntries);
            }
            else if (ifSection.Contains(">="))
            {
                m_comparator = Comparator.GreaterThanOrEqual;
                ifSides = ifSection.Split(new string[] { ">=" }, StringSplitOptions.RemoveEmptyEntries);
            }
            else if (ifSection.Contains("<"))
            {
                m_comparator = Comparator.LessThan;
                ifSides = ifSection.Split(new string[] { "<" }, StringSplitOptions.RemoveEmptyEntries);
            }
            else if (ifSection.Contains(">"))
            {
                m_comparator = Comparator.GreaterThan;
                ifSides = ifSection.Split(new string[] { ">" }, StringSplitOptions.RemoveEmptyEntries);
            }
            else if (ifSection.Contains("=="))
            {
                m_comparator = Comparator.Equals;
                ifSides = ifSection.Split(new string[] { "==" }, StringSplitOptions.RemoveEmptyEntries);
            }
            else if (ifSection.Contains("!="))
            {
                m_comparator = Comparator.NotEquals;
                ifSides = ifSection.Split(new string[] { "!=" }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                Debug.Assert(false);
            }

            //there shoud be two side of the if
            Debug.Assert(ifSides.Length == 2);

            //get each side of the if
            VarFactory varFacotry = new VarFactory();
            m_leftVar = varFacotry.Create(ifSides[0]);
            m_rightVar = varFacotry.Create(ifSides[1]);
            
            //the rest of it is the region
            m_region = new ScriptRegion(otherLines);
        }


        public override void DoEvent()
        {
            //get the value for each side of the condition
            int leftValue = m_leftVar.GetValue();
            int rightValue = m_rightVar.GetValue();

            //compare based on the compatator specified
            bool comparesWell = false;
            if (m_comparator == Comparator.LessThanOrEqual)
            {
                comparesWell = (leftValue <= rightValue);
            }
            else if (m_comparator == Comparator.GreaterThanOrEqual)
            {
                comparesWell = (leftValue >= rightValue);
            }
            else if (m_comparator == Comparator.LessThan)
            {
                comparesWell = (leftValue < rightValue);
            }
            else if (m_comparator == Comparator.GreaterThan)
            {
                comparesWell = (leftValue > rightValue);
            }
            else if (m_comparator == Comparator.Equals)
            {
                comparesWell = (leftValue == rightValue);
            }
            else if (m_comparator == Comparator.NotEquals)
            {
                comparesWell = (leftValue != rightValue);
            }

            //do region if the comparasion was true
            if (comparesWell)
            {
                m_region.DoRegion();
            }
        }


    }


}
