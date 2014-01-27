using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public delegate bool TaskFilter(Task task);

    public partial class CalandarDatesPanel : TycoonPanel
    {
        /// <summary>
        /// Raised when the selected date is change which can happen because the user has clicked a date, or because the month changed
        /// </summary>
        public event Action SelectedDateChanged;
        
        /// <summary>
        /// The date for the first day of the month that is showing in the calandar
        /// </summary>
        private int m_startDate = 0;

        /// <summary>
        /// The day of the month thats selected
        /// </summary>
        private int m_dayOfMonthSelected = 0;

        /// <summary>
        /// Filter to use to filter tasks shown
        /// </summary>
        private TaskFilter m_filter = null;

        //mapping from day of the month to the contorl that should show that day
        private Dictionary<int, TycoonButton> m_dayButtons = new Dictionary<int, TycoonButton>();
        private Dictionary<int, TycoonLabel> m_dayNumberLables = new Dictionary<int, TycoonLabel>();
        private Dictionary<int, TycoonLabel>[] m_dayLines = { new Dictionary<int, TycoonLabel>(), new Dictionary<int, TycoonLabel>(), new Dictionary<int, TycoonLabel>(), new Dictionary<int, TycoonLabel>(), new Dictionary<int, TycoonLabel>() };

        /// <summary>
        /// List of tasks to show in the panel
        /// </summary>
        private MasterTaskList m_taskList;


        /// <summary>
        /// Set the list of tasks to show in the panel, and the filter to filter the list by
        /// </summary>
        public void Setup(MasterTaskList taskList, TaskFilter filter)
        {
            m_taskList = taskList;
            m_filter = filter;
            Refresh();
        }
        


        public CalandarDatesPanel()
        {
            InitializeComponent();

            //setup mappings            
            foreach (TycoonControl control in this.Children)
            {                
                if (control is TycoonButton && control.Name.Contains("day") && control.Name.Contains("Button"))
                {
                    int day = int.Parse(control.Name.Replace("day", "").Replace("Button", ""));
                    m_dayButtons.Add(day-1, control as TycoonButton);
                }                
                else if (control is TycoonLabel && control.Name.Contains("day") && control.Name.Contains("Line"))
                {
                    int lineNum = int.Parse(control.Name.Substring(control.Name.Length - 1));
                    int day = int.Parse(control.Name.Replace("day", "").Replace("Line" + lineNum.ToString(), ""));
                    m_dayLines[lineNum-1].Add(day-1, control as TycoonLabel);
                }
                else if (control is TycoonLabel && control.Name.Contains("day") && control.Name.Contains("Label"))
                {
                    int day = int.Parse(control.Name.Replace("day", "").Replace("Label", ""));
                    m_dayNumberLables.Add(day-1, control as TycoonLabel);
                }
            }


            //handel when one of the days is clicked
            for (int dayOfMonth=0; dayOfMonth<30; dayOfMonth++)
            {
                TycoonButton dayButton = m_dayButtons[dayOfMonth];
                dayButton.Tag = dayOfMonth;
                dayButton.Clicked += new Action<TycoonControl>(delegate(TycoonControl control)
                {
                    m_dayOfMonthSelected = (int)control.Tag;
                    Refresh();
                    if (SelectedDateChanged != null)
                    {
                        SelectedDateChanged();
                    }
                });
            }

            //handel when one of the day lines is clicked
            for (int dayLineNum = 0; dayLineNum < m_dayLines.Length; dayLineNum++)
            {                
                for (int dayOfMonth = 0; dayOfMonth < 30; dayOfMonth++)
                {
                    TycoonLabel dayLineLabel = m_dayLines[dayLineNum][dayOfMonth];
                    dayLineLabel.Tag = dayOfMonth;
                    dayLineLabel.Clicked += new Action<TycoonControl>(delegate(TycoonControl control)
                    {
                        m_dayOfMonthSelected = (int)control.Tag;
                        Refresh();
                        if (SelectedDateChanged != null)
                        {
                            SelectedDateChanged();
                        }
                    });
                }
            }


            //handel when prev or next month is clicked
            nextMonthButton.Clicked += new Action<TycoonControl>(delegate
            {
                m_startDate+=30;                
                Refresh();
                if (SelectedDateChanged != null)
                {
                    SelectedDateChanged();
                }
            });
            prevMonthButton.Clicked += new Action<TycoonControl>(delegate
            {
                m_startDate -= 30;
                if (m_startDate < 0) { m_startDate = 0; }
                Refresh();
                if (SelectedDateChanged != null)
                {
                    SelectedDateChanged();
                }
            });


            //handel resizing
            this.SizeChanged += new Action<TycoonControl>(delegate
            {
                int dateWidth = (this.Width) / 5;
                int dateHeight = (this.Height - 25) / 6;

                //set width and height of all day buttons (and width of lines)
                for (int i = 0; i < 30; i++)
                {
                    m_dayButtons[i].Width = dateWidth;
                    m_dayButtons[i].Height = dateHeight;
                    for (int dayLineNum = 0; dayLineNum < m_dayLines.Length; dayLineNum++)
                    {
                        m_dayLines[dayLineNum][i].Width = dateWidth - 2;
                    }                    
                }
  
                //set top, and left for the day buttons
                for (int i = 0; i < 30; i+=5)
                {
                    if (i == 0)
                    {
                        m_dayButtons[i].Top = 25;
                    }
                    else
                    {
                        m_dayButtons[i].Top = m_dayButtons[i - 5].Top + m_dayButtons[i - 5].Height - 1;
                    }
                    m_dayButtons[i].Left = 0;
                    for (int j = i + 1; j <= i + 4; j++)
                    {
                        m_dayButtons[j].Top = m_dayButtons[j - 1].Top;
                        m_dayButtons[j].Left = m_dayButtons[j - 1].Left + m_dayButtons[j - 1].Width - 1;
                    }
                }
                
                //set location for low, and high lines, and days
                for (int i = 0; i < 30; i++)
                {
                    m_dayNumberLables[i].Top = m_dayButtons[i].Top;
                    m_dayNumberLables[i].Left = m_dayButtons[i].Left;
                    for (int dayLineNum = 0; dayLineNum < m_dayLines.Length; dayLineNum++)
                    {
                        m_dayLines[dayLineNum][i].Top = m_dayButtons[i].Top + m_dayButtons[i].Height - 4 - (3 * dayLineNum);
                        m_dayLines[dayLineNum][i].Left = m_dayButtons[i].Left + 1;                        
                    }
                }

                //set location of prev next, and month label
                monthLabel.Left = m_dayButtons[2].Left - (m_dayButtons[2].Width / 2);
                monthLabel.Width = m_dayButtons[2].Width * 2;
                prevMonthButton.Left = monthLabel.Left - prevMonthButton.Width;
                nextMonthButton.Left = monthLabel.Left + monthLabel.Width;
            });
          
        }
        

        public void Delete()
        {
            m_taskList.TaskListChanged -= new Action(Refresh);
        }

        public void Refresh()
        {
            int showingStartDate = m_startDate;
            int showingEndDate = m_startDate + 29;

            //set the month label at the top
            monthLabel.Text = IntToMonth(((m_startDate % (12 * 30)) / 30)) + "  " + (m_startDate / (12 * 30) + 2000).ToString();

            //clear all the controls
            foreach (TycoonButton control in m_dayButtons.Values)
            {
                control.Text = "";
                control.BackColor = Color.FromArgb(255, 192, 64, 64);
            }
            for (int dayLineNum = 0; dayLineNum < m_dayLines.Length; dayLineNum++)
            {
                foreach (TycoonLabel control in m_dayLines[dayLineNum].Values)
                {
                    control.BorderColor = Color.Transparent;
                    control.BackColor = Color.Transparent;
                }
            }
                         
            //highlight the selected day on the calandar            
            m_dayButtons[m_dayOfMonthSelected].BackColor = Color.Red;
            
            //look at each task in the list
            List<Task> tasksInRange = m_taskList.GetTasksInRange(showingStartDate, showingEndDate);

            foreach (Task task in tasksInRange)
            {
                if (m_filter != null && m_filter(task) == false)
                {
                    continue;
                }

                int startDayOfMonth = task.Schedule.StartDate - showingStartDate;
                int endDayOfMonth = task.Schedule.EndDate - showingStartDate;

                //what day should be used to determine the tasks line number
                int taskFirstLineDay = 0;                
                int taskLastLineDay = 29;    

                //show descirption on start day
                if (startDayOfMonth >= 0)
                {
                    m_dayButtons[startDayOfMonth].Text = task.ShortDescription();
                    taskFirstLineDay = startDayOfMonth;
                }

                //day of the month the task line should end
                if (endDayOfMonth < 30)
                {
                    taskLastLineDay = endDayOfMonth;
                }

                //what line the task should use (-1 if no line is available)
                int taskLineNum = -1;
                for (int lineNum = 0; lineNum < m_dayLines.Length; lineNum++)
                {
                    if (m_dayLines[lineNum][taskFirstLineDay].BackColor == Color.Transparent)
                    {
                        taskLineNum = lineNum;
                        break;
                    }
                }

                //if the line is not -1 add the line all on days the task overlaps with the calandar
                if (taskLineNum != -1)
                {
                    for (int lineDay = taskFirstLineDay; lineDay <= taskLastLineDay; lineDay++)
                    {
                        m_dayLines[taskLineNum][lineDay].BackColor = task.TaskColor();
                        m_dayLines[taskLineNum][lineDay].BorderColor = task.TaskColor();
                    }
                }
            }
        }

        /// <summary>
        /// The date selected in the calandar
        /// </summary>
        public int DateSelected
        {
            get
            {
                return m_startDate + m_dayOfMonthSelected;
            }
            set
            {
                m_dayOfMonthSelected = value % 30;
                m_startDate = value - (value % 30);

                Refresh();
                if (SelectedDateChanged != null)
                {
                    SelectedDateChanged();
                }
            }
        }
        
        private string IntToMonth(int month)
        {
            if (month == 0) { return "Jan"; }
            else if (month == 1) { return "Feb"; }
            else if (month == 2) { return "Mar"; }
            else if (month == 3) { return "Apr"; }
            else if (month == 4) { return "May"; }
            else if (month == 5) { return "Jun"; }
            else if (month == 6) { return "Jul"; }
            else if (month == 7) { return "Aug"; }
            else if (month == 8) { return "Sep"; }
            else if (month == 9) { return "Oct"; }
            else if (month == 10) { return "Nov"; }
            else { return "Dec"; }
            
        }
    }
}

