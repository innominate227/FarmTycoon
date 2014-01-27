using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class BottomLeftStatus : TycoonWindow
    {
        
        public BottomLeftStatus()
        {
            InitializeComponent();
            
            MoveToBottomLeft();
            RefreshMoney();
            RefreshNumberOfWorkers();
            
            Program.UserInterface.Graphics.Events.WindowSizeChanged += new Action(MoveToBottomLeft);
            GameState.Current.Treasury.MoneyChanged += new Action(RefreshMoney);
            GameState.Current.MasterObjectList.ItemRemoved += new Action<Type>(ItemAddedOrRemoved);
            GameState.Current.MasterObjectList.ItemAdded += new Action<Type>(ItemAddedOrRemoved);
            GameState.Current.WorkerAssigner.NumberOfAvaiableWorkersChanged += new Action(RefreshNumberOfWorkers);

            this.moneyLabel.Clicked += new Action<TycoonControl>(MoneyLabel_Clicked);
            this.numberOfWorkersLabel.Clicked += new Action<TycoonControl>(NumberOfWorkersLabel_Clicked);

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                Program.UserInterface.Graphics.Events.WindowSizeChanged -= new Action(MoveToBottomLeft);
                GameState.Current.Treasury.MoneyChanged -= new Action(RefreshMoney);
                GameState.Current.MasterObjectList.ItemRemoved -= new Action<Type>(ItemAddedOrRemoved);
                GameState.Current.MasterObjectList.ItemAdded -= new Action<Type>(ItemAddedOrRemoved);
                GameState.Current.WorkerAssigner.NumberOfAvaiableWorkersChanged -= new Action(RefreshNumberOfWorkers);
            });

            Program.UserInterface.WindowManager.AddWindow(this);
        }

        private void NumberOfWorkersLabel_Clicked(TycoonControl obj)
        {
            new TasksWindow();
        }

        private void MoneyLabel_Clicked(TycoonControl obj)
        {
            new TreasuryWindow();
        }

        private void ItemAddedOrRemoved(Type type)
        {
            if (type == typeof(Worker))
            {
                RefreshNumberOfWorkers();
            }
        }

        private void RefreshNumberOfWorkers()
        {
            numberOfWorkersLabel.Text = GameState.Current.WorkerAssigner.NumberOfAvailableWorkers.ToString() + "/" + GameState.Current.MasterObjectList.FindAll<Worker>().Count.ToString() + " Workers";
        }

        private void RefreshMoney()
        {
            moneyLabel.Text = "$" + GameState.Current.Treasury.CurrentMoney.ToString();
        }

        private void MoveToBottomLeft()
        {
            Left = 0;
            Top = Program.UserInterface.Graphics.WindowHeight - this.Height;
        }


    }
}
