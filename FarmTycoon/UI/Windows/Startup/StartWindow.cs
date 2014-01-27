using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;
using System.IO;

namespace FarmTycoon
{
    public partial class StartWindow : TycoonWindow
    {
        BetaWarningWindow _betaWarningWindow;

        public StartWindow()
        {
            InitializeComponent();

            _betaWarningWindow = new BetaWarningWindow();
                                    
            //center
            this.Top = (Program.UserInterface.Graphics.WindowHeight / 2) - (this.Height / 2);
            this.Left = (Program.UserInterface.Graphics.WindowWidth / 2) - (this.Width / 2);

            ScenarioList.SetFolderToShow(Program.Settings.ScenariosFolder);
            StartLoadButton.Text = "Start";
            NewScenarioTabButton.Depressed = true;
            LoadGameTabButton.Depressed = false;
            UserScenarioTabButton.Depressed = false;
            ScenarioEditorTabButton.Depressed = false;
            NewScenarioButton.Visible = false;            
            MapSizeTextbox.Visible = false;
            MapSizeLabel.Visible = false;

            NewScenarioTabButton.Clicked += new Action<TycoonControl>(delegate
            {
                ScenarioList.SetFolderToShow(Program.Settings.ScenariosFolder);
                StartLoadButton.Text = "Start";
                NewScenarioTabButton.Depressed = true;
                LoadGameTabButton.Depressed = false;
                UserScenarioTabButton.Depressed = false;
                ScenarioEditorTabButton.Depressed = false;
                NewScenarioButton.Visible = false;
                MapSizeTextbox.Visible = false;
                MapSizeLabel.Visible = false;
            });

            LoadGameTabButton.Clicked += new Action<TycoonControl>(delegate
            {
                ScenarioList.SetFolderToShow(Program.Settings.SavesFolder);
                StartLoadButton.Text = "Load";
                NewScenarioTabButton.Depressed = false;
                LoadGameTabButton.Depressed = true;
                UserScenarioTabButton.Depressed = false;
                ScenarioEditorTabButton.Depressed = false;
                NewScenarioButton.Visible = false;
                MapSizeTextbox.Visible = false;
                MapSizeLabel.Visible = false;
            });

            UserScenarioTabButton.Clicked += new Action<TycoonControl>(delegate
            {
                ScenarioList.SetFolderToShow(Program.Settings.UserScenariosFolder);
                StartLoadButton.Text = "Start";
                NewScenarioTabButton.Depressed = false;
                LoadGameTabButton.Depressed = false;
                UserScenarioTabButton.Depressed = true;                
                ScenarioEditorTabButton.Depressed = false;
                NewScenarioButton.Visible = false;
                MapSizeTextbox.Visible = false;
                MapSizeLabel.Visible = false;
            });

            ScenarioEditorTabButton.Clicked += new Action<TycoonControl>(delegate
            {
                ScenarioList.SetFolderToShow(Program.Settings.UserScenariosFolder);
                StartLoadButton.Text = "Load";
                NewScenarioTabButton.Depressed = false;
                LoadGameTabButton.Depressed = false;
                UserScenarioTabButton.Depressed = false;
                ScenarioEditorTabButton.Depressed = true;
                NewScenarioButton.Visible = true;
                MapSizeTextbox.Visible = true;
                MapSizeLabel.Visible = true;
            });

            ScenarioList.SelectionChanged += new Action(delegate
            {
                StartLoadButton.Visible = (ScenarioList.FileSelectedIsLocked == false);
            });


            StartLoadButton.Clicked += new Action<TycoonControl>(StartLoadButton_Clicked);
            NewScenarioButton.Clicked += new Action<TycoonControl>(NewScenarioButton_Clicked);


            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                Program.UserInterface.WindowManager.RemoveWindow(this);
            });
            Program.UserInterface.WindowManager.AddWindow(this);
        }

                
        private void NewScenarioButton_Clicked(TycoonControl obj)
        {
            CloseWindow();

            int size;
            bool parsed = int.TryParse(MapSizeTextbox.Text, out size);
            if (parsed)
            {
                //create a new game                
                GameFile.New(size);
            }
        }



        private void StartLoadButton_Clicked(TycoonControl obj)
        {
            if (ScenarioList.FileSelectedIsLocked) { return; }

            CloseWindow();

            bool scenarioEditMode = ScenarioEditorTabButton.Depressed;
            string selectedFile = ScenarioList.FileSelected;
            GameFile.Load(selectedFile, scenarioEditMode);
        }
   
    


    }
}
