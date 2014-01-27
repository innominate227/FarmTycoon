using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;
using System.IO;

namespace FarmTycoon
{
    public partial class SaveGameWindow : TycoonWindow
    {
        private bool _pausedOnOpen = false;

        public SaveGameWindow(string folder)
        {
            InitializeComponent();

            if (Program.GameThread.ClockDriver.Paused == false)
            {
                Program.GameThread.ClockDriver.Paused = true;
                _pausedOnOpen = true;
            }
                                    
            //center
            this.Top = (Program.UserInterface.Graphics.WindowHeight / 2) - (this.Height / 2);
            this.Left = (Program.UserInterface.Graphics.WindowWidth / 2) - (this.Width / 2);

            //get the list of file we want to show in the save window
            string[] files = Directory.GetFiles(folder, "*.farm", SearchOption.AllDirectories);
            
            //look at each file in the folder
            int topLoc = 0;
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);

                TycoonLabel fileLabel = new TycoonLabel();
                fileLabel.Left = 0;
                fileLabel.Top = topLoc;
                fileLabel.Width = FilesPanel.Width;
                fileLabel.Height = 30;
                fileLabel.BackColor = FilesPanel.BackColor;
                fileLabel.Visible = true;
                fileLabel.Text = fileName;
                fileLabel.Tag = file;                

                fileLabel.Clicked += new Action<TycoonControl>(delegate(TycoonControl control)
                {
                    FileNameTextbox.Text = (control as TycoonLabel).Text;
                });

                FilesPanel.AddChild(fileLabel);

                topLoc += fileLabel.Height;
            }

            FileNameTextbox.TextChanged += new Action<TycoonTextbox>(delegate
            {
                SelectCorrectLabel();
            });


            CloseButton.Clicked += new Action<TycoonControl>(delegate
            {
                this.CloseWindow();                
            });

            SaveButton.Clicked += new Action<TycoonControl>(delegate
            {
                this.CloseWindow();

                string filePath = folder + Path.DirectorySeparatorChar + FileNameTextbox.Text;
                if (filePath.EndsWith(".farm") == false)
                {
                    filePath += ".farm";
                }

                GameFile.Save(filePath);               

            });


            SelectCorrectLabel();

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                if (_pausedOnOpen)
                {
                    Program.GameThread.ClockDriver.Paused = false;
                }
                Program.UserInterface.WindowManager.RemoveWindow(this);
            });                     
            Program.UserInterface.WindowManager.AddWindow(this);
        }


        private void SelectCorrectLabel()
        {
            foreach (TycoonControl control in FilesPanel.Children)
            {
                TycoonLabel label = (TycoonLabel)control;

                label.BackColor = FilesPanel.BackColor;
                if (FileNameTextbox.Text == label.Text)
                {
                    label.BackColor = Color.Blue;
                }
            }            
        }

    }
}
