using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;
using System.Diagnostics;

namespace FarmTycoon
{
    public partial class SimpleTaskWindow : TycoonWindow
    {        
        private Task _task;

        public SimpleTaskWindow()
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
                ItemPanel.Delete();

                //unhandle global events
                Program.UserInterface.Graphics.Events.MouseDown -= new MouseEventHandler(Graphics_MouseDown);

                //remove window
                Program.UserInterface.WindowManager.RemoveWindow(this);
            });
        }


        private void CommonSetup(Task task, GameObject lastUsedObject)
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

            //remeber use equipmnet on close
            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                GameState.Current.LastUsedValues.SetEquipmentWasLastUsed(lastUsedObject, UseEquipmentCheckbox.Checked);
            });

            //setup issues panel
            issuesAndTimePanel.SetTask(_task);

            //setup window
            SetupWindow();
        }


        public void SetupForSprayTask(Field field)
        {
            SprayTask sprayTask = new SprayTask();
            sprayTask.AreaBeingSprayed = field;

            //show only spary items in the item panel
            this.ItemPanel.ItemList = GameState.Current.PlayersItemsList;
            this.ItemPanel.SetFilter(delegate(ItemType item)
            {
                return item.HasTag(SpecialTags.SPRAY_TAG);
            });
            ItemPanel.SelectedItemChanged += new Action(delegate
            {
                sprayTask.WhatToSpray = ItemPanel.SelectedItem;
                issuesAndTimePanel.Refresh();
            });
            sprayTask.WhatToSpray = ItemPanel.SelectedItem;
            

            //setup to allow using equipment if tractor and sprayer are available
            sprayTask.UseEquipment = GameState.Current.LastUsedValues.GetEquipmentWasLastUsed(field);
            UseEquipmentCheckbox.Setup(EquipmentType.Tractor, EquipmentType.Sprayer);
            UseEquipmentCheckbox.CheckChanged += new Action(delegate
            {
                sprayTask.UseEquipment = UseEquipmentCheckbox.Checked;
                issuesAndTimePanel.Refresh();
            });
            
            //setup common task elements
            CommonSetup(sprayTask, field);
        }


        public void SetupForPlantTask(Field field)
        {
            PlantTask plantTask = new PlantTask();
            plantTask.PlantedArea = field;

            //show only seed items in the item panel
            this.ItemPanel.ItemList = GameState.Current.PlayersItemsList;
            this.ItemPanel.SetFilter(delegate(ItemType item)
            {
                return item.HasTag(SpecialTags.SEED_TAG);
            });
            ItemPanel.SelectedItemChanged += new Action(delegate
            {
                plantTask.SeedToPlant = ItemPanel.SelectedItem;
                issuesAndTimePanel.Refresh();
            });
            plantTask.SeedToPlant = ItemPanel.SelectedItem;            


            //setup to allow using equipment if tractor and sprayer are available
            plantTask.UseEquipment = GameState.Current.LastUsedValues.GetEquipmentWasLastUsed(field);
            UseEquipmentCheckbox.Setup(EquipmentType.Tractor, EquipmentType.Planter);
            UseEquipmentCheckbox.CheckChanged += new Action(delegate
            {
                plantTask.UseEquipment = UseEquipmentCheckbox.Checked;
                issuesAndTimePanel.Refresh();
            });

            //setup common task elements
            CommonSetup(plantTask, field);
        }


        public void SetupForHarvestTask(Field field)
        {
            PickTask harvestTask = new PickTask();
            harvestTask.Field = field;
            harvestTask.Harvest = true;

            //hide items panel
            HideItemsPanel();

            //setup use equipment checkbox
            harvestTask.UseEquipment = GameState.Current.LastUsedValues.GetEquipmentWasLastUsed(field);
            UseEquipmentCheckbox.Setup(EquipmentType.Harvester, EquipmentType.Trailer);
            UseEquipmentCheckbox.CheckChanged += new Action(delegate
            {
                harvestTask.UseEquipment = UseEquipmentCheckbox.Checked;
                issuesAndTimePanel.Refresh();
            });

            //setup common task elements
            CommonSetup(harvestTask, field);
        }


        public void SetupForPickTask(Field field)
        {
            PickTask pickTask = new PickTask();
            pickTask.Field = field;
            pickTask.Harvest = false;

            //hide items panel
            HideItemsPanel();

            //hide equipment checkbox            
            UseEquipmentCheckbox.Hide();

            //setup common task elements
            CommonSetup(pickTask, field);
        }


        public void SetupForPlowTask(Field field)
        {
            PlowTask plowTask = new PlowTask();
            plowTask.PlantedArea = field;
            
            //hide items panel
            HideItemsPanel();

            //setup use equipment checkbox
            plowTask.UseEquipment = GameState.Current.LastUsedValues.GetEquipmentWasLastUsed(field);
            UseEquipmentCheckbox.Setup(EquipmentType.Tractor, EquipmentType.Plow);
            UseEquipmentCheckbox.CheckChanged += new Action(delegate
            {
                plowTask.UseEquipment = UseEquipmentCheckbox.Checked;
                issuesAndTimePanel.Refresh();
            });

            //setup common task elements
            CommonSetup(plowTask, field);
        }


        public void SetupForFillTroughTask(Trough trough)
        {
            FillTroughTask fillTroughTask = new FillTroughTask();
            fillTroughTask.Trough = trough;

            //show only seed items in the item panel
            this.ItemPanel.ItemList = GameState.Current.PlayersItemsList;
            this.ItemPanel.SetFilter(delegate(ItemType item)
            {
                return item.HasTag(SpecialTags.ANIMAL_FOOD_TAG) || item.HasTag(SpecialTags.ANIMAL_WATER_TAG);
            });
            ItemPanel.SelectedItemChanged += new Action(delegate
            {
                fillTroughTask.WhatToFill = ItemPanel.SelectedItem;
            });


            //hide use equipment
            UseEquipmentCheckbox.Hide();

            //setup common task elements
            CommonSetup(fillTroughTask, trough);
        }




        private void HideItemsPanel()
        {
            ItemLabel.Visible = false;
            ItemPanel.Visible = false;
            foreach (TycoonControl con in this.Children)
            {
                if (con.Top > ItemPanel.Top && con != ItemPanel)
                {
                    con.Top -= (ItemPanel.Height + ItemLabel.Height);
                }
            }
            this.Height -= (ItemPanel.Height + ItemLabel.Height);
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
