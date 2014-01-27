using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// A task that is scheduled to occurr at one or more times in the future
    /// </summary>
    public class ScheduledTask : ISavable
    {
        #region Member Vars

        /// <summary>
        /// The task that will be started on the days scheduled.
        /// This task never gets actually gets started, it is cloned and the clone is started on the correct days according to the schedule.        
        /// </summary>
        private Task _templateTask;

        /// <summary>
        /// The number of days after the scheudle is activated that it should wait before it starts
        /// If set to 0 we should start right away. When the schedule becomes active
        /// </summary>
        private int _startDelay = 0;

        /// <summary>
        /// The number of days after the scheudle is activated that is should wait before it ends
        /// </summary>
        private int _endDelay = int.MaxValue;

        /// <summary>
        /// The number of times the task should be repeated        
        /// </summary>
        private int _timesToRepeate = 1;
        
        /// <summary>
        /// The interval for how often the task should be done        
        /// This value means nothing if the task will only be done once
        /// </summary>
        private int _interval = 10;
        
        /// <summary>
        /// The first date that the scheudled task should be done.  This differs slightly from StartDelay as this is the actual date.
        /// If set to -1 that means the schedule has not been activated
        /// </summary>
        private int _startOn = -1;

        /// <summary>
        /// The date that the scheduled task should end.  This differs slightly from EndDelay as this is the actual date.  
        /// If set to -1 that means the schedule has not been activated
        /// </summary>
        private int _endOn = -1;

        /// <summary>
        /// The number of times the scheduled task has been done
        /// </summary>
        private int _timesDone = 0;

        #endregion
                
        #region Member Vars

        /// <summary>
        /// The task that will be started on the days scheduled.
        /// This task never gets actually gets started, it is cloned and the clone is started on the correct days according to the schedule.             
        /// </summary>
        public Task TemplateTask
        {
            get{ return _templateTask; }
            set{ _templateTask = value; }
        }
        
        /// <summary>
        /// The number of days after the scheudle is activated that it should wait before it starts
        /// If set to 0 we should start right away. When the schedule becomes active
        /// Either this or StartOn should be set before the Schedule is activated.
        /// </summary>        
        public int StartDelay
        {
            get { return _startDelay; }
            set { _startDelay = value; }
        }

        /// <summary>
        /// The number of days after the scheudle is activated that it should wait before it ends
        /// If set to 0 we should start right away. When the schedule becomes active        
        /// Either this, TimesToRepeate or EndOn should be set before the Schedule is activated.
        /// </summary>
        public int EndDelay
        {
            get { return _endDelay; }
            set { _endDelay = value; }
        }
        
        /// <summary>
        /// The maximum number of times the task will run before it stops.
        /// Either this, EndDelay or EndOn should be set before the Schedule is activated.
        /// </summary>
        public int TimesToRepeate
        {
            get { return _timesToRepeate; }
            set { _timesToRepeate = value; }
        }
        
        /// <summary>
        /// The interval for how often the task should be done        
        /// This value means nothing if the task will only be done once
        /// </summary>
        public int Interval
        {
            get{ return _interval; }
            set{ _interval = value; }
        }
        
        
        /// <summary>
        /// The first date that the scheudled task should be done. 
        /// Either this or StartDelay should be set before the Schedule is activated.       
        /// </summary>        
        public int StartOn
        {
            get { return _startOn; }
            set { _startOn = value; }
        }

        /// <summary>
        /// The date that the scheduled task should end.  
        /// Either this, EndDelay or TimesToRepeate should be set before the Schedule is activated.
        /// </summary>
        public int EndOn
        {
            get { return _endOn; }
            set { _endOn = value; }
        }

        /// <summary>
        /// The number of times the scheduled task has been done
        /// </summary>        
        public int TimesDone
        {
            get { return _timesDone; }            
        }

        /// <summary>
        /// Get the next date that the task will run. Or -1 if it will not run again.
        /// </summary>
        public int NextRunDate
        {
            get
            {
                int today = GameState.Current.Calandar.Date;

                if (today > _endOn)
                {
                    //if we passed the end date yet we will no run any more
                    return -1;
                }                    
                else if (_timesDone >= _timesToRepeate)
                {
                    //if we have already run it enough times we will not run it any more
                    return -1;
                }
                else if (today < _startOn)
                {
                    //if we have not reached start date yet we run first on the start date
                    return _startOn;
                }
                else
                {
                    //the next run date is based on the start date + the interval
                    int nextRun = _startOn + (_interval * _timesDone);
                    if (nextRun == today)
                    {
                        nextRun += _interval;
                    }
                    return nextRun;
                }
            }
        }


        #endregion
        
        #region Logic

        /// <summary>
        /// Start the scehdule
        /// </summary>
        public void ActivateSchedule()
        {
            //a start delay was sepcifed instead of a start date, determine the start date
            if (_startDelay != -1)
            {
                _startOn = GameState.Current.Calandar.Date + _startDelay;
            }
            
            //a end delay was sepcifed instead of a end date, determine the end date
            if (_endDelay != -1)
            {
                if (_endDelay == int.MaxValue)
                {
                    _endOn = int.MaxValue;
                }
                else
                {
                    _endOn = GameState.Current.Calandar.Date + _endDelay;
                }
            }
            
            
            //if starting today do it now
            if (_startOn == GameState.Current.Calandar.Date)
            {
                //start the task now
                StartTask();

                //if we are just doing the task once we are done (we should not add the scheudle to the master task list)
                if (_timesToRepeate == 1)
                {
                    return;
                }
            }

            //add the schedule to the master task list
            GameState.Current.MasterTaskList.AddScheduledTask(this);            
        }

        
        /// <summary>
        /// Abort the scehdule (but not any tasks the scheudle has already started)
        /// </summary>
        public void AbortSchedule()
        {
            GameState.Current.MasterTaskList.RemoveScheduledTask(this);
        }

        /// <summary>
        /// Check the schedule and if the task should start on the date passed then start the task
        /// </summary>
        public void CheckSchedule(int date)
        {
            //if we have not reached start date yet do nothing
            if (date < _startOn)
            {
                return;
            }
            
            //determine the number of days since the start date
            int daysSinceStart = date - _startOn;

            //if it has been interval days we want to do the task again
            if (daysSinceStart % _interval == 0)
            {
                StartTask();
            }

            //if we have started the task enough times we are done.   remove from the schedule
            if (_timesDone >= _timesToRepeate)
            {
                GameState.Current.MasterTaskList.RemoveScheduledTask(this);
                return;
            }

            //if tommorrow we will be passed the end on date we are done.  remove from the schedule
            if (date + 1 > _endOn)
            {
                GameState.Current.MasterTaskList.RemoveScheduledTask(this);
                return;
            }
        }

        /// <summary>
        /// Called when the task should be started
        /// </summary>
        private void StartTask()
        {
            //clone the task to get a task to start
            Task task = _templateTask.Clone();
            task.DesiredStartDate = GameState.Current.Calandar.Date;
            task.TryToStart();

            //we have done the task once more
            _timesDone++;
        }

        #endregion

        #region Save Load
		public void WriteStateV1(StateWriterV1 writer)
		{
			writer.WriteObject(_templateTask);
			writer.WriteInt(_startDelay);
			writer.WriteInt(_endDelay);
			writer.WriteInt(_timesToRepeate);
			writer.WriteInt(_interval);
			writer.WriteInt(_startOn);
			writer.WriteInt(_endOn);
			writer.WriteInt(_timesDone);
		}
		
		public void ReadStateV1(StateReaderV1 reader)
		{
			_templateTask = reader.ReadObject<Task>();
			_startDelay = reader.ReadInt();
			_endDelay = reader.ReadInt();
			_timesToRepeate = reader.ReadInt();
			_interval = reader.ReadInt();
			_startOn = reader.ReadInt();
			_endOn = reader.ReadInt();
			_timesDone = reader.ReadInt();
		}
		
		public void AfterReadStateV1()
		{
		}
		#endregion
    }
}
