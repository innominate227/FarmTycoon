using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class GoalWindow : TycoonWindow
    {   
        public GoalWindow()
        {
            InitializeComponent();

            this.TitleText = FarmData.Current.ScenarioInfo.Name;
            DescriptionLabel.Text = FarmData.Current.ScenarioInfo.Description;
            ObjectiveLabel.Text = FarmData.Current.ScenarioInfo.Objective;
            ProgressLabel.Text = GameState.Current.UIStrings.VictoryProgress;

            GameState.Current.UIStrings.VictoryProgressChanged += new Action(UIStrings_VictoryProgressChanged);

            //add window to game
            Program.UserInterface.WindowManager.AddWindow(this);
            
            //hide on close
            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                GameState.Current.UIStrings.VictoryProgressChanged -= new Action(UIStrings_VictoryProgressChanged);
                Program.UserInterface.WindowManager.RemoveWindow(this);       
            });
        }


        private void UIStrings_VictoryProgressChanged()
        {
            ProgressLabel.Text = GameState.Current.UIStrings.VictoryProgress;
        }


    }
}
