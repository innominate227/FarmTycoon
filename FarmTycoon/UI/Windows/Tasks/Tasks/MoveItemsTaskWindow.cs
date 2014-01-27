using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;
using System.Diagnostics;

namespace FarmTycoon
{


    /// <summary>
    /// Move window is used for Move Buy and Sell tasks
    /// </summary>
    public partial class MoveItemsTaskWindow : TycoonWindow
    {        
        private Task _task;

        public MoveItemsTaskWindow()
        {
            InitializeComponent();

            this.Visible = false;
        }

        private void SetupWindow()
        {
            this.Top = (Program.UserInterface.Graphics.WindowHeight / 2) - (this.Height / 2);
            this.Left = (Program.UserInterface.Graphics.WindowWidth / 2) - (this.Width / 2);

            okButton.Clicked += new Action<TycoonControl>(OkButton_Clicked);
            cancelButton.Clicked += new Action<TycoonControl>(delegate { CloseWindow(); });
            Program.UserInterface.Graphics.Events.MouseDown += new MouseEventHandler(Graphics_MouseDown);

            //add window to game
            this.Visible = true;
            Program.UserInterface.WindowManager.AddWindow(this);

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                //delete panels
                ItemsShooterPanel.Delete();

                //unhandle global events
                Program.UserInterface.Graphics.Events.MouseDown -= new MouseEventHandler(Graphics_MouseDown);

                //remove window
                Program.UserInterface.WindowManager.RemoveWindow(this);
            });
        }
            
        
        private void CommonSetup(Task task, IGameObject lastUsedObject)
        {
            _task = task;

            //default to same number of workers as last time for that building (or delivery area if no building)            
            _task.NumberOfWorkers = GameState.Current.LastUsedValues.GetNumberOfWorkersLastUsed(lastUsedObject);

            //setup number of workers panel
            numberOfWorkersPanel.PreferredList = _task.PreferredWorkers;
            numberOfWorkersPanel.NumberOfWorkers = _task.NumberOfWorkers;
            numberOfWorkersPanel.NumberOfWorkersChanged += new Action(delegate
            {
                _task.NumberOfWorkers = numberOfWorkersPanel.NumberOfWorkers;
                issuesAndTimePanel.Refresh();
            });
            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                GameState.Current.LastUsedValues.SetNumberOfWorkersLastUsed(lastUsedObject, _task.NumberOfWorkers);
            });
            
            //setup issues panel
            issuesAndTimePanel.SetTask(_task);

            //setup window
            SetupWindow();
        }



        public void SetupForSellTask(IStorageBuilding preferedSource)
        {
            this.TitleText = "Sell Items";

            SellItemsTask sellItemsTask = new SellItemsTask();
            sellItemsTask.PreferedSource = preferedSource;

            //setup the catagory filter on the shooter panel for buy / sell tasks
            ItemsShooterPanel.SetFilter(delegate(ItemType item)
            {
                return item.HasTag(ItemCatagoriesPanel.Catagory);
            });
            ItemCatagoriesPanel.CatagoryChanged += new Action(delegate
            {
                ItemsShooterPanel.Refresh();
            });


            //set up the shooter panel
            ItemsShooterPanel.Setup(GameState.Current.StoreStock, sellItemsTask.WhatToSell);
            ItemsShooterPanel.SelectedItemsChanged += new Action(delegate
            {
                issuesAndTimePanel.Refresh();
            });


            //setup prefered destination panel 
            TakeToLabel.Text = "Get From:";
            TakeToDropbox.Setup(delegate(IStorageBuilding building)
            {
                return (building is StorageBuilding || building is Pasture);
            });
            TakeToDropbox.SelectedLocation = preferedSource;
            TakeToDropbox.LocationChanged += new Action(delegate
            {
                sellItemsTask.PreferedSource = TakeToDropbox.SelectedLocation;
            });


            //setup common task elements
            if (preferedSource == null)
            {
                CommonSetup(sellItemsTask, GameState.Current.MasterObjectList.Find<DeliveryArea>());
            }
            else
            {
                CommonSetup(sellItemsTask, preferedSource);
            }
        }



        public void SetupForBuyTask(IStorageBuilding preferedDesitnation)
        {
            this.TitleText = "Buy Items";

            BuyItemsTask buyItemsTask = new BuyItemsTask();
            buyItemsTask.PreferedDestination = preferedDesitnation;

            //setup the catagory filter on the shooter panel for buy / sell tasks
            ItemsShooterPanel.SetFilter(delegate(ItemType item)
            {
                return item.HasTag(ItemCatagoriesPanel.Catagory);
            });
            ItemCatagoriesPanel.CatagoryChanged += new Action(delegate
            {
                ItemsShooterPanel.Refresh();
            });


            //set up the shooter panel
            ItemsShooterPanel.Setup(GameState.Current.StoreStock, buyItemsTask.WhatToBuy);
            ItemsShooterPanel.SelectedItemsChanged += new Action(delegate
            {
                issuesAndTimePanel.Refresh();
            });


            //setup prefered destination panel 
            TakeToDropbox.Setup(delegate(IStorageBuilding building)
            {
                return (building is StorageBuilding || building is Pasture);
            });
            TakeToDropbox.SelectedLocation = preferedDesitnation;
            TakeToDropbox.LocationChanged += new Action(delegate
            {
                buyItemsTask.PreferedDestination = TakeToDropbox.SelectedLocation;
            });


            //setup common task elements
            if (preferedDesitnation == null)
            {
                CommonSetup(buyItemsTask, GameState.Current.MasterObjectList.Find<DeliveryArea>());
            }
            else
            {
                CommonSetup(buyItemsTask, preferedDesitnation);
            }

        }



        public void SetupForMoveTask(IStorageBuilding source)
        {
            this.TitleText = "Move Items";

            MoveItemsTask moveItemsTask = new MoveItemsTask();
            moveItemsTask.UseTask = false;
            moveItemsTask.Source = source;


            //hide the filter panel at the top for move task
            ItemCatagoriesPanel.Visible = false;
            foreach (TycoonControl con in this.Children)
            {
                con.Top -= (ItemCatagoriesPanel.Height + 2);
            }


            //set up the shooter panel
            ItemsShooterPanel.Setup(source.Inventory.UnderlyingList, (moveItemsTask as MoveItemsTask).WhatToMove);
            ItemsShooterPanel.SelectedItemsChanged += new Action(delegate
            {
                issuesAndTimePanel.Refresh();
            });


            //for move items dont allow them to select the building they are moving from
            TakeToDropbox.Setup(delegate(IStorageBuilding building)
            {
                if (building == source) { return false; }
                return (building is StorageBuilding || building is Pasture);
            });
            TakeToDropbox.LocationChanged += new Action(delegate
            {
                moveItemsTask.PreferedDestination = TakeToDropbox.SelectedLocation;
            });
            moveItemsTask.PreferedDestination = TakeToDropbox.SelectedLocation;


            //setup common task elements
            CommonSetup(moveItemsTask, source);
        }



        public void SetupForUseTask(IStorageBuilding source)
        {
            this.TitleText = "Use Items";

            MoveItemsTask useItemsTask = new MoveItemsTask();
            useItemsTask.UseTask = true;
            useItemsTask.Source = source;
            
            //hide the filter panel at the top for use task
            ItemCatagoriesPanel.Visible = false;
            foreach (TycoonControl con in this.Children)
            {
                con.Top -= (ItemCatagoriesPanel.Height + 2);
            }
            
            //set up the shooter panel
            ItemsShooterPanel.Setup(source.Inventory.UnderlyingList, (useItemsTask as MoveItemsTask).WhatToMove);
            ItemsShooterPanel.SelectedItemsChanged += new Action(delegate
            {
                issuesAndTimePanel.Refresh();
            });


            //for move items dont allow them to select the building they are moving from, and they have to select a production building
            TakeToDropbox.Setup(delegate(IStorageBuilding building)
            {
                if (building == source) { return false; }
                return (building is ProductionBuilding);
            });
            TakeToDropbox.LocationChanged += new Action(delegate
            {
                useItemsTask.PreferedDestination = TakeToDropbox.SelectedLocation;
            });
            useItemsTask.PreferedDestination = TakeToDropbox.SelectedLocation;


            //setup common task elements
            CommonSetup(useItemsTask, source);
        }







        private void Graphics_MouseDown(ClickInfo clickInfo)
        {
            //close window if something that is not in this window was clicked
            //dont close if event is from pie menu button clicked to create this window, or from control clicked in dropbox window
            if (clickInfo.ControlClicked == null || (clickInfo.ControlClicked.ParentWindow != this && clickInfo.ControlClicked.ParentWindow is PieMenuWindow == false && clickInfo.ControlClicked.ParentWindow is MoveItemsTaskWindow == false && clickInfo.ControlClicked.ParentWindow.TitleText != "Dropbox" && clickInfo.ControlClicked.ParentWindow is PreferredWorkersWindow == false && clickInfo.ControlClicked.ParentWindow is TaskScheduleWindow == false))
            {
                CloseWindow();
            }
        }


        private void OkButton_Clicked(TycoonControl obj)
        {
            ScheduledTask schedule = SchedulePanel.Schedule;
            schedule.TemplateTask = _task;
            schedule.ActivateSchedule();
            CloseWindow();
        }
        

    }
}
