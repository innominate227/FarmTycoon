using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public partial class Treasury : ISavable
    {
        public static string ITEMS_CATAGORY = "Items";
        public static string CONSTRUCTION_CATAGORY = "Construction";
        
        #region Events

        public event Action MoneyChanged;

        #endregion

        #region Member Vars

        /// <summary>
        /// A list of the last 12 finacial statements, where the front of the list is the current months statement
        /// </summary>
        private List<FinacialStatement> _lastStatements = new List<FinacialStatement>();

        /// <summary>
        /// The amount of money the player currently has
        /// </summary>
        private int _currentMoney = 0;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create tresury, call setup or read state before using
        /// </summary>
        public Treasury() { }

        /// <summary>
        /// Setup the treasurey
        /// </summary>
        public void Setup()
        {
            GameState.Current.Calandar.DateChanged += new Action(Calandar_DateChanged);
        }
        
        #endregion
        
        #region Properties

        /// <summary>
        /// Get the amount of money the player has
        /// </summary>
        public int CurrentMoney
        {
            get { return _currentMoney; }
        }

        /// <summary>
        /// List of FinacialStatements from the last 12 months.  Most recent month in front.
        /// </summary>
        public IList<FinacialStatement> LastStatements
        {
            get { return _lastStatements.AsReadOnly(); }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Called when a day passes
        /// </summary>
        private void Calandar_DateChanged()
        {
            //if its the start of a month
            if (GameState.Current.Calandar.Date % 30 == 0)
            {
                //remove old statements if we have 12 months worth
                if (_lastStatements.Count == 12)
                {
                    _lastStatements.RemoveAt(11);
                }

                //create a statement for the new month and add to the front of the calandar
                FinacialStatement statement = new FinacialStatement();
                statement.Setup(GameState.Current.Calandar.Date);
                _lastStatements.Insert(0, statement);

                //raise that something changed about the money (amount didnt change but we want to refresh detailed fincial window now)
                if (MoneyChanged != null)
                {
                    MoneyChanged();
                }
            }
        }
        
        /// <summary>
        /// Add the profit to the treasury in the catagory passed for the amount passed.        
        /// </summary>
        public void Sell(string catagory, string subCatagory, int profit)
        {
            //dont spend money in scnario edit mode
            if (Program.Game.ScenarioEditMode) { return; }

            _currentMoney += profit;
            _lastStatements[0].RecordIncome(catagory, subCatagory, profit);
            if (MoneyChanged != null)
            {
                MoneyChanged();
            }
        }

        /// <summary>
        /// Remove money from the treasury in the catagory passed for the amount passed.        
        /// </summary>
        public void Buy(string catagory, string subCatagory, int cost)
        {
            _currentMoney -= cost;
            _lastStatements[0].RecordExpenses(catagory, subCatagory, cost);
            if (MoneyChanged != null)
            {
                MoneyChanged();
            }
        }

        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteObjectList<FinacialStatement>(_lastStatements);
            writer.WriteInt(_currentMoney);
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _lastStatements = reader.ReadObjectList<FinacialStatement>();
            _currentMoney = reader.ReadInt();
        }

        public void AfterReadStateV1()
        {
            GameState.Current.Calandar.DateChanged += new Action(Calandar_DateChanged);
        }
        #endregion

    }
}
