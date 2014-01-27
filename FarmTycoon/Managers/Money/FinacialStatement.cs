using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// An account of what money was spent on, and made on over a month
    /// </summary>
    public class FinacialStatement : ISavable
    {
        #region Member Vars

        /// <summary>
        /// Game day at which the statement starts
        /// </summary>
        private int _startDay;

        /// <summary>
        /// The income for each catagory-subcatagory
        /// </summary>
        private Dictionary<string, int> _income = new Dictionary<string, int>();

        /// <summary>
        /// The expenses for each catagory-subcatagory
        /// </summary>
        private Dictionary<string, int> _expenses = new Dictionary<string, int>();

        #endregion

        #region Setup

        /// <summary>
        /// Create a new finacial statment, call setup or ReadState
        /// </summary>
        public FinacialStatement()
        {
        }

        /// <summary>
        /// Setup a new finacial statment starting on the day passed
        /// </summary>
        public void Setup(int startDay)
        {
            _startDay = startDay;
        }

        #endregion

        #region Logic

        /// <summary>
        /// Add an income transaction to the finacial statement for the month
        /// </summary>
        public void RecordIncome(string catagory, string subCatagory, int amount)
        {
            string fullCatagory = catagory + "_" + subCatagory;            
            if (_income.ContainsKey(fullCatagory) == false)
            {
                _income.Add(fullCatagory, 0);
            }
            _income[fullCatagory] += amount;
        }

        /// <summary>
        /// Add an expense transaction to the finacial statement for the month
        /// </summary>
        public void RecordExpenses(string catagory, string subCatagory, int amount)
        {
            string fullCatagory = catagory + "_" + subCatagory;
            if (_expenses.ContainsKey(fullCatagory) == false)
            {
                _expenses.Add(fullCatagory, 0);
            }
            _expenses[fullCatagory] += amount;
        }


        /// <summary>
        /// Get the amount that was made in a catagory and subcatagory
        /// </summary>
        public int GetIncomeForCatagory(string catagory, string subCatagory)
        {
            string fullCatagory = catagory + "_" + subCatagory;
            if (_income.ContainsKey(fullCatagory) == false)
            {
                _income.Add(fullCatagory, 0);
            }
            return _income[fullCatagory];
        }


        /// <summary>
        /// Get the amount that was lost in a catagory and subcatagory
        /// </summary>
        public int GetExpensesForCatagory(string catagory, string subCatagory)
        {
            string fullCatagory = catagory + "_" + subCatagory;
            if (_expenses.ContainsKey(fullCatagory) == false)
            {
                _expenses.Add(fullCatagory, 0);
            }
            return _expenses[fullCatagory];
        }





        /// <summary>
        /// Get a list of income catagories
        /// </summary>
        public List<Tuple<string, string>> GetIncomeCatagories()
        {
            List<string> sortedCatagories = new List<string>();
            sortedCatagories.AddRange(_income.Keys);
            sortedCatagories.Sort();

            List<Tuple<string, string>> toRet = new List<Tuple<string, string>>();
            foreach (string fullCatagory in sortedCatagories)
            {
                toRet.Add(new Tuple<string,string>(fullCatagory.Split('_')[0], fullCatagory.Split('_')[1]));
            }
            return toRet;
        }


        /// <summary>
        /// Get a list of expense catagories
        /// </summary>
        private List<Tuple<string, string>> GetExpenseCatagories()
        {
            List<string> sortedCatagories = new List<string>();
            sortedCatagories.AddRange(_expenses.Keys);
            sortedCatagories.Sort();

            List<Tuple<string, string>> toRet = new List<Tuple<string, string>>();
            foreach (string fullCatagory in sortedCatagories)
            {
                toRet.Add(new Tuple<string, string>(fullCatagory.Split('_')[0], fullCatagory.Split('_')[1]));
            }
            return toRet;

        }

        #endregion
        
        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteInt(_startDay);

            writer.WriteInt(_income.Count);
            foreach (string key in _income.Keys)
            {
                writer.WriteString(key);
                writer.WriteInt(_income[key]);
            }

            writer.WriteInt(_expenses.Count);
            foreach (string key in _expenses.Keys)
            {
                writer.WriteString(key);
                writer.WriteInt(_expenses[key]);
            }
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _startDay = reader.ReadInt();

            int count = reader.ReadInt();
            for (int num = 0; num < count; num++)
            {
                string key = reader.ReadString();
                int val = reader.ReadInt();
                _income.Add(key, val);
            }

            int count2 = reader.ReadInt();
            for (int num = 0; num < count2; num++)
            {
                string key = reader.ReadString();
                int val = reader.ReadInt();
                _expenses.Add(key, val);
            }
        }

        public void AfterReadStateV1()
        {
        }
        #endregion

    }
}
