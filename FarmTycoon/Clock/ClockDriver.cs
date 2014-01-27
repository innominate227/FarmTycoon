using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Drives the game clock at a certain rate.
    /// </summary>
    public class ClockDriver
    {
        /// <summary>
        /// Event raised when the rate the user desires has changed
        /// </summary>
        public event Action DesiredRateChanged;

        /// <summary>
        /// Event raised when the actual game rate has changed
        /// </summary>
        public event Action ActualRateChanged;
        


        /// <summary>
        /// Notification id for our notification that tells us how fast the clock is actualy going
        /// </summary>
        private Notification _notification;


        /// <summary>
        /// the nano sec count that the stopwatch was at during the last time we drove time forward
        /// </summary>
        private long _lastDriveNano = 0;
        
        /// <summary>
        /// the nano sec count that the stopwatch was at during the last clock notification
        /// </summary>
        private long _lastNotificationNano = 0;
        
        /// <summary>
        /// interval in game days we have instructed the clock to notify us at
        /// </summary>
        private double _notificationInterval;



        /// <summary>
        /// The rate that the user desires to run the clock at
        /// </summary>
        private double _desiredRate = 1.0;

        /// <summary>
        /// The rate the clock is actually currently running at
        /// </summary>
        private double _actualRate = 1.0;
        
        /// <summary>
        /// Is the game pasued
        /// </summary>
        private bool _paused = false;


        /// <summary>
        /// The clock we are managing the rate of
        /// </summary>
        private Clock _clock;

        /// <summary>
        /// The game thread drving this clock driver
        /// </summary>
        private GameThread _gameThread;



        /// <summary>
        /// Drives the game clock at a certain rate
        /// </summary>
        public ClockDriver(Clock clock, GameThread gameThread)
        {
            _clock = clock;
            _gameThread = gameThread;
            
            //create notification
            _notification = _clock.RegisterNotification(ClockNotification, 1.0, false);
            AdjustClockNotificationInterval();
        }


        /// <summary>
        /// Is the game pasued
        /// </summary>
        public bool Paused
        {            
            set 
            {
                if (_paused && value == false)
                {
                    //we were pause and were unpausing
                    //set last nano sec to the unpaused time so time doesnt try to jump forward from when we paused
                    _lastDriveNano = _gameThread.CurrentNanosecond;
                }
                if (value)
                {
                    //we are pausing
                    //abort the current time slice, so control can return to the clock driver, and so that the pause will not appear to lag
                    _clock.AbortSlice();
                }

                _paused = value;
            }
            get { return _paused; }
        }

        /// <summary>
        /// Get or set the desired game speed
        /// </summary>
        public double DesiredRate
        {
            get { return _desiredRate; }
            set 
            {
                _desiredRate = value;

                //abort the current time slice, if we set the speed to slower this is nessisary so control can return to the clock driver
                _clock.AbortSlice();
                                                   
                if (DesiredRateChanged != null) { DesiredRateChanged(); }

                //TEMP debug
                DebugToolWindow.DesiredRate = "Desired: " + _desiredRate.ToString("N2") + "x";                
            }
        }


        /// <summary>
        /// Get the actual game speed
        /// </summary>
        public double ActualRate
        {
            get { return _actualRate; }
        }

        /// <summary>
        /// Drive the clock forward based on how many nano secound have passed since this was last called
        /// </summary>
        public void DriveClock()
        {            
            //determine how many nano have passed since this was last called
            long currentNano = _gameThread.CurrentNanosecond;
            long nanoPassed = currentNano - _lastDriveNano;
            _lastDriveNano = currentNano;

            //adjust the time passed for the game rate we want to play at
            long adjustedTimePassed = (long)(nanoPassed * _desiredRate);

            //drive the clock (unless were paused)
            if (_paused == false)
            {                
                //move the clock forward that much                    
                _clock.MoveTimeForward(adjustedTimePassed);           
            }
        }
        
        /// <summary>
        /// Adjust the clock notification so that ClockNotification gets called about every 100 ms 
        /// </summary>
        private void AdjustClockNotificationInterval()
        {
            //use the actual rate, unless it is higher than the desired rate (we had a stall followed by a burst)
            //if that happens use the desired rate, so that we dont end up with a gaitn notification interval
            double rate = _actualRate;
            if (rate > _desiredRate)
            {
                rate = _desiredRate;
            }

            //calculate how often we should be notified, (0.1 = 100ms)
            _notificationInterval = (0.1 * rate) / Clock.SEC_PER_DAY;

            //tell clock to notifiy us at this interval
            _notification = _clock.UpdateNotification(_notification, _notificationInterval);
        }


        /// <summary>
        /// This method is set up as a normal game clock notification (every X game days).
        /// X is calculated so that at the current game speed it expects to be called every 100 ms (real world).
        /// _current rate calculation does not rely on this being called every 100ms its just the target current rate recalculation interval.
        /// </summary>
        private void ClockNotification()
        {
            //determine how many nano have passed since this was last called
            long currentNano = _gameThread.CurrentNanosecond;
            long nanoPassed = currentNano - _lastNotificationNano;
            _lastNotificationNano = currentNano;

            //determine how fast we are actually running. (how much real world time should have passed if we were at 1x / how much real world time actually passed).
            _actualRate = (Clock.NANO_SEC_PER_DAY * _notificationInterval) / (double)(nanoPassed);

            AdjustClockNotificationInterval();

            //raise actual rate changed event
            if (ActualRateChanged != null) { ActualRateChanged(); }

            //TEMP debug            
            DebugToolWindow.ActualRate="Actual: " + _actualRate.ToString("N2") + "x";
            
        }



    }
}
