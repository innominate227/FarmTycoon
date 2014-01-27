using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;
using System.IO;

namespace FarmTycoon
{
    public partial class ScenarioListPanel : TycoonPanel
    {
        public event Action SelectionChanged;

        private string _fileSelected = "";

        private bool _fileSelectedIsLocked = false;

        public ScenarioListPanel()
        {
            InitializeComponent();
        }

        public void SetFolderToShow(string folder)
        {
            //remove all current labels
            foreach (TycoonControl control in filesPanel.Children.ToArray())
            {
                filesPanel.RemoveChild(control);
            }

            //read the scores file if present
            string scoresFile = folder + Path.DirectorySeparatorChar + "scores.dat";            
            Dictionary<string, int> scores = new Dictionary<string, int>();            
            if (File.Exists(scoresFile))
            {
                StreamReader scoresFileReader = new StreamReader(scoresFile);
                
                //read all lines in the file
                string line = scoresFileReader.ReadLine();
                while (line != null)
                {
                    //parse the line
                    string[] tokens = line.Split(',');
                    string name = tokens[0];                    
                    int score = int.Parse(line.Split(',')[1]);
                    scores.Add(name, score);                  

                    line = scoresFileReader.ReadLine();
                }

                scoresFileReader.Close();
            }


            //read the ordering file if present    
            int leftUnlock = int.MaxValue;        
            string orderingFile = folder + Path.DirectorySeparatorChar + "ordering.dat";
            Dictionary<string, int> ordering = new Dictionary<string, int>();
            if (File.Exists(orderingFile))
            {
                StreamReader orderingFileReader = new StreamReader(scoresFile);
                
                //read number that will be unlocked at once
                leftUnlock = int.Parse(orderingFileReader.ReadLine());

                //read all lines in the file
                string line = orderingFileReader.ReadLine();
                while (line != null)
                {
                    //parse the line                    
                    string name = line;
                    ordering.Add(name, ordering.Count);                    

                    line = orderingFileReader.ReadLine();
                }

                orderingFileReader.Close();
            }





            //get the list of file we want to show in the load window
            List<string> files = new List<string>(Directory.GetFiles(folder, "*.farm", SearchOption.AllDirectories));

            //sort the files
            if (ordering.Count > 0)
            {
                files.Sort(delegate(string file1, string file2)
                {
                    return ordering[Path.GetFileNameWithoutExtension(file1)].CompareTo(ordering[Path.GetFileNameWithoutExtension(file2)]);
                });
            }


            //determine if levels are locked
            Dictionary<string, bool> locked = new Dictionary<string, bool>();
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);

                //determine if the level is locked (it is unlocked if it is beaten, or it is one of the top X unbeaten)
                bool fileLocked = true;
                if (scores.ContainsKey(fileName))
                {
                    fileLocked = false;
                }
                else if (leftUnlock > 0)
                {
                    leftUnlock--;
                    fileLocked = false;
                }                
                locked.Add(file, fileLocked);
            }


            
            //look at each file in the folder
            int topLoc = 0;
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                
                TycoonButton fileIcon = new TycoonButton();
                fileIcon.Left = 0;
                fileIcon.Top = topLoc;
                fileIcon.Width = 25;
                fileIcon.Height = 25;
                fileIcon.BackColor = filesPanel.BackColor;
                fileIcon.ShadowDarkColor = filesPanel.BackColor;
                fileIcon.ShadowLightColor = filesPanel.BackColor;
                fileIcon.Visible = true;
                fileIcon.Text = "";
                fileIcon.Tag = file;
                if (locked[file])
                {
                    fileIcon.IconTexture = "lock_icon";
                }
                else
                {
                    fileIcon.IconTexture = "";
                }


                TycoonLabel fileLabel = new TycoonLabel();
                fileLabel.Left = 24;
                fileLabel.Top = topLoc;
                fileLabel.Width = filesPanel.Width - 124;
                fileLabel.Height = 25;
                fileLabel.BackColor = filesPanel.BackColor;
                fileLabel.BorderColor = filesPanel.BackColor;
                fileLabel.Visible = true;
                fileLabel.Text = "  " +  fileName;
                fileLabel.TextVerticelAlignment = StringAlignment.Center;
                fileLabel.Tag = file;


                TycoonLabel scoreLabel = new TycoonLabel();
                scoreLabel.Left = filesPanel.Width - 100;
                scoreLabel.Top = topLoc;
                scoreLabel.Width = 100;
                scoreLabel.Height = 25;
                scoreLabel.BackColor = filesPanel.BackColor;
                scoreLabel.BorderColor = filesPanel.BackColor;
                scoreLabel.Visible = true;
                scoreLabel.Text = "";
                if (scores.ContainsKey(fileName))
                {
                    scoreLabel.Text = "Score: " + scores[fileName].ToString();
                }
                scoreLabel.TextVerticelAlignment = StringAlignment.Center;
                scoreLabel.Tag = file;


                //if any are clicked select that one

                fileIcon.Clicked += new Action<TycoonControl>(delegate(TycoonControl contorl)
                {
                    _fileSelectedIsLocked = locked[file];
                    _fileSelected = (string)contorl.Tag;
                    RefreshSelected();
                });

                fileLabel.Clicked += new Action<TycoonControl>(delegate(TycoonControl contorl)
                {
                    _fileSelectedIsLocked = locked[file];
                    _fileSelected = (string)contorl.Tag;
                    RefreshSelected();
                });

                scoreLabel.Clicked += new Action<TycoonControl>(delegate(TycoonControl contorl)
                {
                    _fileSelectedIsLocked = locked[file];
                    _fileSelected = (string)contorl.Tag;
                    RefreshSelected();
                });

                filesPanel.AddChild(fileIcon);
                filesPanel.AddChild(fileLabel);
                filesPanel.AddChild(scoreLabel);
                topLoc += fileLabel.Height;
            }


            RefreshSelected();

        }

        private void RefreshSelected()
        {
            //select correct file in list
            foreach (TycoonControl control in filesPanel.Children)
            {
                if (control is TycoonLabel)
                {
                    TycoonLabel label = (TycoonLabel)control;
                    label.BackColor = filesPanel.BackColor;
                    label.BorderColor = filesPanel.BackColor;
                    if (_fileSelected == (label.Tag as string))
                    {
                        label.BackColor = Color.Blue;
                        label.BorderColor = Color.Blue;
                    }
                }
                else
                {
                    TycoonButton button = (TycoonButton)control;
                    button.BackColor = filesPanel.BackColor;
                    button.ShadowDarkColor = filesPanel.BackColor;
                    button.ShadowLightColor = filesPanel.BackColor;
                    if (_fileSelected == (button.Tag as string))
                    {
                        button.BackColor = Color.Blue;
                        button.ShadowDarkColor = Color.Blue;
                        button.ShadowLightColor = Color.Blue;
                    }
                }
            }  


            if (_fileSelected == "")
            {
                scenarioDateLabel.Text = "";
                scenarioDescriptionLabel.Text = "";
                scenarioNameLabel.Text = "";
                scenarioObjectiveLabel.Text = "";
            }
            else
            {                
                SaveInfo fileInfo = GameFile.LoadInfo(_fileSelected);

                scenarioDateLabel.Text = fileInfo.GameDateLastSave;
                scenarioDescriptionLabel.Text = fileInfo.ScenarioDescription;
                scenarioNameLabel.Text = fileInfo.ScenarioName;
                scenarioObjectiveLabel.Text = fileInfo.ScenarioObjective;
            }


            if (SelectionChanged != null)
            {
                SelectionChanged();
            }

        }



        public string FileSelected
        {
            get { return _fileSelected; }
        }

        public bool FileSelectedIsLocked
        {
            get { return _fileSelectedIsLocked; }
        }


    }
}
