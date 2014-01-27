using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace FarmTycoon
{
    public partial class ScenarioSettingsWindow : TycoonWindow
    {
        public ScenarioSettingsWindow()
        {
            InitializeComponent();
                                    
            //center
            this.Top = (Program.UserInterface.Graphics.WindowHeight / 2) - (this.Height / 2);
            this.Left = (Program.UserInterface.Graphics.WindowWidth / 2) - (this.Width / 2);


            this.EditScenarioScriptButton.Clicked += new Action<TycoonControl>(EditScenarioScriptButton_Clicked);
            
            this.EditFarmDataFileButton.Clicked += new Action<TycoonControl>(EditFarmDataFileButton_Clicked);
            
            Refresh();

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                Program.UserInterface.WindowManager.RemoveWindow(this);
            });
            Program.UserInterface.WindowManager.AddWindow(this);
        }


        private void EditScenarioScriptButton_Clicked(TycoonControl obj)
        {
            //write script to a temp file
            string tempFileName = Path.GetTempFileName() + ".cs";
            File.WriteAllText(tempFileName, Program.Game.ScriptPlayer.ScriptText);
            
            
            MessageWindow oldWidow = null;

            string scriptText;
            while (true)
            {
                OpenEditorAndWait(tempFileName);

                //make sure we dont go poping up a bunch of windows if they edit the script badly
                if (oldWidow != null)
                {
                    oldWidow.CloseWindow();
                }

                //load edited script
                try
                {
                    scriptText = File.ReadAllText(tempFileName);
                    new ScriptPlayer(scriptText);
                }
                catch (Exception e)
                {
                    oldWidow = new MessageWindow("Script Error", "Error Parsing Script:\r\n\r\n" + e.Message, false, 150, 100);
                    continue;
                }
                break;
            }

            Program.Game.ScriptPlayer.ScriptText = scriptText;
            new MessageWindow("Script Loaded", "Script Loaded Successfully", false, 150, 100);
                           
            
        }



        private void EditFarmDataFileButton_Clicked(TycoonControl obj)
        {
            string tempFileName = Path.GetTempFileName() + ".xml";
            File.WriteAllText(tempFileName, FarmData.Current.FarmDataXml);


            MessageWindow oldWidow = null;
            string farmDataText;
            while (true)
            {

                OpenEditorAndWait(tempFileName);

                //make sure we dont go poping up a bunch of windows if they edit the script badly
                if (oldWidow != null)
                {
                    oldWidow.CloseWindow();
                }

                //load edited farm data file
                try
                {
                    //this object is just created to make sure we can load the farm data
                    farmDataText = File.ReadAllText(tempFileName);
                    FarmData testLoad = new FarmData(farmDataText);                    
                }
                catch
                {
                    oldWidow = new MessageWindow("File Error", "Error Parsing File", false, 150, 100);
                    continue;
                }

                //we are able to load the farm data correctly so we can break out of the loop
                break;
            }

            //hide this window and show the user a warning, about what is hapening
            this.Visible = false;
            MessageWindow messageWindow = new MessageWindow("Farm Data Loaded", "Game will save twice then load again.  Dont touch anything!", false, 150, 100);
            messageWindow.Top -= 200;

            //path we will save the game to
            string savePath = Program.Settings.ScenariosFolder + Path.DirectorySeparatorChar + GameState.Current.LastUsedValues.SaveName + ".farm";

            //first save a backup of the current scenario before doing anything                
            GameFile.Save(savePath + ".backup");
                
            //we dont want to actually update the farm data object until we save everything and reopen the scenario editor
            //or else we will have some object using old data object, and others using new data objects
            FarmData.Current.FarmDataXml = farmDataText;
            
            //now save the game with the new farm data                            
            GameFile.Save(savePath);

            //close the game
            GameFile.Close();

            //now load the we just saved                
            GameFile.Load(savePath, true);                        
        }
        




        private void OpenEditorAndWait(string filename)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "\"" + Program.Settings.TextEditor + "\"";
                startInfo.Arguments = "\"" + filename + "\"";                                
                Process process = System.Diagnostics.Process.Start(startInfo);

                //sleep until editor closed
                while (process.HasExited == false)
                {
                    Thread.Sleep(1000);
                }
            }
            catch
            {
                new MessageWindow("Text Editor Error", "Unable to open text editor, ensure editor is set in settings.xml", false, 150, 100);
            }
        }

        private void Refresh()
        {
            scenarioNameLabel.Text = FarmData.Current.ScenarioInfo.Name;
            scenarioDescriptionLabel.Text = FarmData.Current.ScenarioInfo.Description;
            scenarioObjectiveLabel.Text = FarmData.Current.ScenarioInfo.Objective;
        }

   
    


    }
}
