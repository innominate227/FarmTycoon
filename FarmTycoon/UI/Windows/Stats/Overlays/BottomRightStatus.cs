using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;
using System.Diagnostics;

namespace FarmTycoon
{
    public partial class BottomRightStatus : TycoonWindow
    {
        public BottomRightStatus()
        {
            InitializeComponent();
            
            MoveToBottomRight();
            RefreshWeather();
            RefreshDate();

            GameState.Current.Calandar.DateChanged += new Action(RefreshDate);
            GameState.Current.UIStrings.WeatherChanged += new Action(RefreshWeather);
            Program.UserInterface.Graphics.Events.WindowSizeChanged += new Action(MoveToBottomRight);                                                
            Program.GameThread.ClockDriver.DesiredRateChanged += new Action(RefreshDate);

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                GameState.Current.Calandar.DateChanged -= new Action(RefreshDate);
                GameState.Current.UIStrings.WeatherChanged -= new Action(RefreshWeather);
                Program.UserInterface.Graphics.Events.WindowSizeChanged -= new Action(MoveToBottomRight);
                Program.GameThread.ClockDriver.DesiredRateChanged -= new Action(RefreshDate);
            });
            

            Program.UserInterface.WindowManager.AddWindow(this);
        }


        private void MoveToBottomRight()
        {
            Left = Program.UserInterface.Graphics.WindowWidth - this.Width;
            Top = Program.UserInterface.Graphics.WindowHeight - this.Height;
        }

        private void RefreshWeather()
        {
            weatherLabel.Text = GameState.Current.UIStrings.Weather.ToString();
        }
        
        private void RefreshDate()
        {
            string dateString = Calandar.DateAsString(GameState.Current.Calandar.Date);
            dateLabel.Text = dateString + "    (" + Program.GameThread.ClockDriver.DesiredRate.ToString() + "x)";
        }


    }
}
