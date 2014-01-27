using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class TaskScheduleWindow : TycoonWindow
    {
        public event Action ScheduleChanged;

        /// <summary>
        /// The schedule being edited
        /// </summary>
        private ScheduledTask _schedule;

        public TaskScheduleWindow(ScheduledTask schedule)
        {
            InitializeComponent();

            this.Top = (Program.UserInterface.Graphics.WindowHeight / 2) - (this.Height / 2);
            this.Left = (Program.UserInterface.Graphics.WindowWidth / 2) - (this.Width / 2);

            _schedule = schedule;

            ScheduleToWindow();
            SetupButtonEvents();

            Program.UserInterface.Graphics.Events.MouseDown += new MouseEventHandler(Graphics_MouseDown);

            this.CloseClicked += new Action<TycoonWindow>(delegate
            {
                WindowToSchedule();

                //remove from windows manager
                Program.UserInterface.Graphics.Events.MouseDown -= new MouseEventHandler(Graphics_MouseDown);
                Program.UserInterface.WindowManager.RemoveWindow(this);                              
            });                 
            Program.UserInterface.WindowManager.AddWindow(this);
        }
        
        private void Graphics_MouseDown(ClickInfo clickInfo)
        {
            //close window if something that is not in this window was clicked            
            if (clickInfo.ControlClicked == null || (clickInfo.ControlClicked.ParentWindow != this && clickInfo.ControlClicked.ParentWindow.TitleText != "Dropbox" && clickInfo.ControlClicked.Name != "EditScheduleButton"))
            {
                CloseWindow();
            }
        }


        private void SetupButtonEvents()
        {
            //Frequency;
            FrequencyOnceButton.Clicked += new Action<TycoonControl>(delegate
            {
                if (FrequencyOnceButton.Depressed == false)
                {
                    EndPanel.Visible = false;
                    this.Height -= (EndPanel.Height + 5);
                }
                FrequencyOnceButton.Depressed = true;
                FrequencyWeeklyButton.Depressed = false;
                FrequencyMonthlyButton.Depressed = false;
                FrequencyYearlyButton.Depressed = false;
                FrequencyEveryDaysButton.Depressed = false;
                ScheudleChanged();
            });
            FrequencyWeeklyButton.Clicked += new Action<TycoonControl>(delegate
            {
                if (FrequencyOnceButton.Depressed)
                {
                    EndPanel.Visible = true;
                    this.Height += (EndPanel.Height + 5);
                }
                FrequencyOnceButton.Depressed = false;
                FrequencyWeeklyButton.Depressed = true;
                FrequencyMonthlyButton.Depressed = false;
                FrequencyYearlyButton.Depressed = false;
                FrequencyEveryDaysButton.Depressed = false;
                ScheudleChanged();
            });
            FrequencyMonthlyButton.Clicked += new Action<TycoonControl>(delegate
            {
                if (FrequencyOnceButton.Depressed)
                {
                    EndPanel.Visible = true;
                    this.Height += (EndPanel.Height + 5);
                }
                FrequencyOnceButton.Depressed = false;
                FrequencyWeeklyButton.Depressed = false;
                FrequencyMonthlyButton.Depressed = true;
                FrequencyYearlyButton.Depressed = false;
                FrequencyEveryDaysButton.Depressed = false;
                ScheudleChanged();
            });
            FrequencyYearlyButton.Clicked += new Action<TycoonControl>(delegate
            {
                if (FrequencyOnceButton.Depressed)
                {
                    EndPanel.Visible = true;
                    this.Height += (EndPanel.Height + 5);
                }
                FrequencyOnceButton.Depressed = false;
                FrequencyWeeklyButton.Depressed = false;
                FrequencyMonthlyButton.Depressed = false;
                FrequencyYearlyButton.Depressed = true;
                FrequencyEveryDaysButton.Depressed = false;
                ScheudleChanged();
            });
            FrequencyEveryDaysButton.Clicked += new Action<TycoonControl>(delegate
            {
                if (FrequencyOnceButton.Depressed)
                {
                    EndPanel.Visible = true;
                    this.Height += (EndPanel.Height + 5);
                }
                FrequencyOnceButton.Depressed = false;
                FrequencyWeeklyButton.Depressed = false;
                FrequencyMonthlyButton.Depressed = false;
                FrequencyYearlyButton.Depressed = false;
                FrequencyEveryDaysButton.Depressed = true;
                ScheudleChanged();
            });

            FrequencyEveryDaysDropbox.TextChanged += new Action<TycoonControl>(delegate
            {
                ScheudleChanged();
            });




            //Start;
            StartTodayButton.Clicked += new Action<TycoonControl>(delegate
            {
                StartTodayButton.Depressed = true;
                StartAfterDaysButton.Depressed = false;
                StartOnButton.Depressed = false;
                ScheudleChanged();
            });
            StartAfterDaysButton.Clicked += new Action<TycoonControl>(delegate
            {
                StartTodayButton.Depressed = false;
                StartAfterDaysButton.Depressed = true;
                StartOnButton.Depressed = false;
                ScheudleChanged();
            });
            StartOnButton.Clicked += new Action<TycoonControl>(delegate
            {
                StartTodayButton.Depressed = false;
                StartAfterDaysButton.Depressed = false;
                StartOnButton.Depressed = true;
                ScheudleChanged();
            });

            StartAfterDaysDropbox.TextChanged += new Action<TycoonControl>(delegate
            {
                ScheudleChanged();
            });
            StartOnDropbox.TextChanged += new Action<TycoonControl>(delegate
            {
                ScheudleChanged();
            });





            //End;
            EndNeverButton.Clicked += new Action<TycoonControl>(delegate
            {
                EndNeverButton.Depressed = true;
                EndAfterTimesButton.Depressed = false;
                EndAfterDaysButton.Depressed = false;
                ScheudleChanged();
            });
            EndAfterTimesButton.Clicked += new Action<TycoonControl>(delegate
            {
                EndNeverButton.Depressed = false;
                EndAfterTimesButton.Depressed = true;
                EndAfterDaysButton.Depressed = false;
                ScheudleChanged();
            });
            EndAfterDaysButton.Clicked += new Action<TycoonControl>(delegate
            {
                EndNeverButton.Depressed = false;
                EndAfterTimesButton.Depressed = false;
                EndAfterDaysButton.Depressed = true;
                ScheudleChanged();
            });
            
            EndAfterDaysDropbox.TextChanged += new Action<TycoonControl>(delegate
            {
                ScheudleChanged();
            });
            EndAfterTimesDropbox.TextChanged += new Action<TycoonControl>(delegate
            {
                ScheudleChanged();
            });

        }
        
        private void ScheduleToWindow()
        {
            //Frequency;
            if (_schedule.TimesToRepeate == 1)
            {
                FrequencyOnceButton.Depressed = true;
            }
            else
            {
                FrequencyWeeklyButton.Depressed = (_schedule.Interval == 7);
                FrequencyMonthlyButton.Depressed = (_schedule.Interval == 30);
                FrequencyYearlyButton.Depressed = (_schedule.Interval == 360);
                FrequencyEveryDaysButton.Depressed = (FrequencyOnceButton.Depressed == false && FrequencyWeeklyButton.Depressed == false && FrequencyMonthlyButton.Depressed == false && FrequencyYearlyButton.Depressed == false);
                FrequencyEveryDaysDropbox.Text = _schedule.Interval.ToString();
            }
            
            //Start            
            if (_schedule.StartOn == -1)
            {
                StartPanel.Visible = true;
                StartPanel2.Visible = false;
                StartTodayButton.Depressed = true;
            }
            else
            {
                //we have already started just show the start date
                StartPanel.Visible = false;
                StartPanel2.Visible = true;
                StartedOnLabel.Text = "Started on " + Calandar.DateAsString(_schedule.StartOn);
                EndPanel.Top -= (StartPanel.Height - StartPanel2.Height);
                this.Height -= (StartPanel.Height - StartPanel2.Height);
            }

            //End          
            if (_schedule.TimesToRepeate == 1)
            {
                EndPanel.Visible = false;
                EndNeverButton.Depressed = true;
                this.Height -= (EndPanel.Height + 5);
            }
            else
            {
                if (_schedule.EndOn == int.MaxValue && _schedule.TimesToRepeate == int.MaxValue)
                {
                    EndNeverButton.Depressed = true;
                }
                else if (_schedule.TimesToRepeate == int.MaxValue)
                {
                    EndAfterDaysButton.Depressed = true;
                    int days = _schedule.EndOn - _schedule.StartOn;
                    EndAfterDaysDropbox.Text = days.ToString();
                }
                else
                {
                    EndAfterTimesButton.Depressed = true;
                    EndAfterTimesDropbox.Text = _schedule.TimesToRepeate.ToString();
                }
            }
        }

        private void WindowToSchedule()
        {
            //Frequency;
            if (FrequencyOnceButton.Depressed)
            {
                _schedule.TimesToRepeate = 1;                
            }
            else if (FrequencyWeeklyButton.Depressed)
            {
                _schedule.Interval = 7;
            }
            else if (FrequencyMonthlyButton.Depressed)
            {
                _schedule.Interval = 30;
            }
            else if (FrequencyYearlyButton.Depressed)
            {
                _schedule.Interval = 360;
            }
            else if (FrequencyEveryDaysButton.Depressed)
            {
                _schedule.Interval = int.Parse(FrequencyEveryDaysDropbox.Text);
            }

            //Start            
            if (StartPanel.Visible)
            {
                //if start on was not -1 then we are looking at an already started schedule so we cant change start date
                if (StartAfterDaysButton.Depressed)
                {
                    _schedule.StartDelay = int.Parse(StartAfterDaysDropbox.Text);
                    _schedule.StartOn = -1;
                }
                else if (StartOnButton.Depressed)
                {
                    int yearNumber = GameState.Current.Calandar.Date / 360;
                    _schedule.StartOn = yearNumber * 360 + int.Parse(StartOnDropbox.Text.Trim('s', 't', 'n', 'd', 'h')) - 1;
                    _schedule.StartDelay = -1;
                }
                else
                {
                    _schedule.StartDelay = 0;
                    _schedule.StartOn = -1;
                }
            }

            //End         
            if (FrequencyOnceButton.Depressed == false)
            {
                if (EndNeverButton.Depressed)
                {
                    _schedule.EndDelay = int.MaxValue;
                    _schedule.TimesToRepeate = int.MaxValue;
                }
                else if (EndAfterDaysButton.Depressed)            
                {
                    int startOn = _schedule.StartOn;
                    if (startOn == -1) { startOn = GameState.Current.Calandar.Date; }

                    _schedule.EndDelay = int.Parse(EndAfterDaysDropbox.Text);
                    _schedule.TimesToRepeate = int.MaxValue;
                }
                else if (EndAfterTimesButton.Depressed)
                {
                    _schedule.EndDelay = int.MaxValue;
                    _schedule.TimesToRepeate = int.Parse(EndAfterTimesDropbox.Text);
                }
            }
            
        }
        
        private void ScheudleChanged()
        {
            WindowToSchedule();

            if (ScheduleChanged != null)
            {
                ScheduleChanged();
            }
        }

        private string NumberWithTh(int num)
        {
            string numString = num.ToString();
            if (numString.EndsWith("1") && num != 11)
            {
                return numString + "st";
            }
            else if (numString.EndsWith("2") && num != 12)
            {
                return numString + "nd";
            }
            else if (numString.EndsWith("3") && num != 13)
            {
                return numString + "rd";
            }
            return numString + "th";
        }


    }
}
