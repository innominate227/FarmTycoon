using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FarmTycoon
{
    [Serializable]
    public partial class ScriptGameInterface
    {

        public void SetVictoryProgress(string vitoryProgress)
        {
            GameState.Current.UIStrings.VictoryProgress = vitoryProgress;
        }

        public void SetWeather(string weatherDescription)
        {
            GameState.Current.UIStrings.Weather = weatherDescription;
        }

        public void ShowMessage(int width, int height, string title, string message)
        {
            ShowMessage(width, height, title, message, false);
        }

        public void ShowMessage(int width, int height, string title, string message, bool pause)
        {
            new MessageWindow(title, message, pause, width, height);
        }


        public void ShowChoice(int width, int height, string title, string message, Action yesAction, Action noAction)
        {
            ShowChoice(width, height, title, message, "Yes", "No", yesAction, noAction);
        }
        public void ShowChoice(int width, int height, string title, string message, string yesLabel, string noLabel, Action yesAction, Action noAction)
        {
            ShowChoice(width, height, title, message, yesLabel, noLabel, yesAction, noAction, false);
        }
        public void ShowChoice(int width, int height, string title, string message, string yesLabel, string noLabel, Action yesAction, Action noAction, bool pause)
        {
            new YesNoWindow(title, message, yesLabel, noLabel, pause, width, height, yesAction, noAction);
        }



        public void Win(string message)
        {
            int newScore = GameState.Current.Treasury.CurrentMoney;

            //get the scores file from the directory the game was in
            string scoresFile = Path.GetDirectoryName(Program.Game.GameFile) + Path.DirectorySeparatorChar + "scores.dat";
            string[] scoresFileLines = new string[0];
            if (File.Exists(scoresFile))
            {
                scoresFileLines = File.ReadAllLines(scoresFile);
            }

            //recreate scores file,
            StreamWriter writer = new StreamWriter(scoresFile);
            string scenarioName = Path.GetFileNameWithoutExtension(Program.Game.GameFile);
            bool addNewLine = true;
            foreach (string line in scoresFileLines)            
            {
                //get the file name/ score that was in the  old file
                string file = line.Split(',')[0];
                int score = int.Parse(line.Split(',')[1]);
                                
                if (file != scenarioName)
                {
                    //if it a different game than the one just beaten add it back
                    writer.WriteLine(line);
                }
                else
                {
                    //if its the same add it back if it has a higher score
                    //and we dont want to add a new line to the end in this case
                    if (score > newScore)
                    {
                        addNewLine = false;
                        writer.WriteLine(line);
                    }
                }                
            }

            //add line for the scenario just beat, (if the new score was higher)
            if (addNewLine)
            {
                writer.WriteLine(scenarioName + "," + newScore.ToString());
            }

            //done writting
            writer.Close();

            //show player "You Win"
            new YesNoWindow("You Win", message, "Continue", "Main Menu", true, 100, 50, delegate { }, delegate
            {
                GameFile.Close();                
                new StartWindow();
            });
        }



    }
}
