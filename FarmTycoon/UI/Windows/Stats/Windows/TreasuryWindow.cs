using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class TreasuryWindow : TycoonWindow
    {
        public TreasuryWindow()
        {
            InitializeComponent();
            
            this.Top = (Program.UserInterface.Graphics.WindowHeight / 2) - (this.Height / 2);
            this.Left = (Program.UserInterface.Graphics.WindowWidth / 2) - (this.Width / 2);
            
            GameState.Current.Treasury.MoneyChanged += new Action(RefreshWindow);

            RefreshWindow();

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                Program.UserInterface.WindowManager.RemoveWindow(this);
            });
                 
            Program.UserInterface.WindowManager.AddWindow(this);
        }

        private void RefreshWindow()
        {
            //if (GameState.Current.Treasury.LastStatements.Count > 0)
            //{
            //    FinacialStatement statementMonth0 = GameState.Current.Treasury.LastStatements[0];
            //    int sales = statementMonth0.GetAmountInCatagory(SpendingCatagory.ItemSales);
            //    int incomeEvents = statementMonth0.GetAmountInCatagory(SpendingCatagory.IncomeEvemts);
            //    int allIncome = sales + incomeEvents;

            //    int construction = statementMonth0.GetAmountInCatagory(SpendingCatagory.Construction);
            //    int purchases = statementMonth0.GetAmountInCatagory(SpendingCatagory.ItemsPurchase);
            //    int salaries = statementMonth0.GetAmountInCatagory(SpendingCatagory.Salaries);
            //    int tax = statementMonth0.GetAmountInCatagory(SpendingCatagory.LandTax);
            //    int expenseEvents = statementMonth0.GetAmountInCatagory(SpendingCatagory.ExpenseEvemts);
            //    int allExpense = construction + purchases + salaries + tax + expenseEvents;
                
            //    SalesMonth0.Text = sales.ToString();
            //    IncomeEventsMonth0.Text = incomeEvents.ToString();
            //    TotalIncomeMonth0.Text = allIncome.ToString();

            //    ConstructionMonth0.Text = construction.ToString();
            //    PurchasesMonth0.Text = purchases.ToString();
            //    SalariesMonth0.Text = salaries.ToString();
            //    TaxMonth0.Text = tax.ToString();
            //    ExpenseEventsMonth0.Text = expenseEvents.ToString();
            //    TotalExpenseMonth0.Text = allExpense.ToString();

            //    NetMonth0.Text = (allIncome + allExpense).ToString();
            //}
            //if (GameState.Current.Treasury.LastStatements.Count > 1)
            //{
            //    FinacialStatement statementMonth1 = GameState.Current.Treasury.LastStatements[1];
            //    int sales = statementMonth1.GetAmountInCatagory(SpendingCatagory.ItemSales);
            //    int incomeEvents = statementMonth1.GetAmountInCatagory(SpendingCatagory.IncomeEvemts);
            //    int allIncome = sales + incomeEvents;

            //    int construction = statementMonth1.GetAmountInCatagory(SpendingCatagory.Construction);
            //    int purchases = statementMonth1.GetAmountInCatagory(SpendingCatagory.ItemsPurchase);
            //    int salaries = statementMonth1.GetAmountInCatagory(SpendingCatagory.Salaries);
            //    int tax = statementMonth1.GetAmountInCatagory(SpendingCatagory.LandTax);
            //    int expenseEvents = statementMonth1.GetAmountInCatagory(SpendingCatagory.ExpenseEvemts);
            //    int allExpense = construction + purchases + salaries + tax + expenseEvents;

            //    SalesMonth1.Text = sales.ToString();
            //    IncomeEventsMonth1.Text = incomeEvents.ToString();
            //    TotalIncomeMonth1.Text = allIncome.ToString();

            //    ConstructionMonth1.Text = construction.ToString();
            //    PurchasesMonth1.Text = purchases.ToString();
            //    SalariesMonth1.Text = salaries.ToString();
            //    TaxMonth1.Text = tax.ToString();
            //    ExpenseEventsMonth1.Text = expenseEvents.ToString();
            //    TotalExpenseMonth1.Text = allExpense.ToString();

            //    NetMonth1.Text = (allIncome + allExpense).ToString();
            //}
        }
   
    


    }
}
