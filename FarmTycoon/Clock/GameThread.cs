using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Thread that runs the game.
    /// </summary>
    public class GameThread
    {
        /// <summary>
        /// RefreshTimePassed event below gets raised every this many nano secs
        /// </summary>
        public const long REFRESH_INTERVAL = 100000000; //100 ms


        /// <summary>
        /// Event raised when any amount of real world time has passed (amount of time passed in nano seconds is passed)
        /// </summary>
        public event Action<long> TimePassed;

        /// <summary>
        /// Event raised every refresh interval of real world time has passed
        /// </summary>
        public event Action RefreshTimePassed;






        /// <summary>
        /// The nano sec of the last time we raised the time passed event
        /// </summary>
        private long _lastTimePassedNanoSec;
        
        
        /// <summary>
        /// Amount of time passed since the last time we raised RefreshTimePassed
        /// </summary>
        private long _timeSinceLastUpdate = 0;
        

        /// <summary>
        /// Stopwatch that keeps track of how much time has passed since the game started being driven
        /// </summary>
        private Stopwatch _timer;

        /// <summary>
        /// Number of nano secound that pass for each tick of the stopwatch
        /// </summary>
        private long _nanosecPerTick;

        /// <summary>
        /// Thread the game runs on
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// Set true to kill the game thread
        /// </summary>
        private volatile bool _kill = false;
        
        /// <summary>
        /// The game clock
        /// </summary>
        private Clock _clock;
        
        /// <summary>
        /// Driver for the game clock
        /// </summary>
        private ClockDriver _clockDriver;
        
        /// <summary>
        /// Create a new game driver
        /// </summary>
        public GameThread()
        {
            //create game clock
            _clock = new Clock(this);

            //if the clock spend alot of time in its move time forward then raise the time passed event, so we dont starve click processing
            _clock.StarvationPreventionEvent += new Action(RaiseTimePassed);

            //create driver for the clock
            _clockDriver = new ClockDriver(_clock, this);

            if (Program.Settings.MultiThread)
            {
                //create the game thread (if we are running multithreaded)
                _thread = new Thread(ThreadFunction);
                _thread.Name = "Game Thread";            
            }

            //create game timer
            _timer = new Stopwatch();
            
            //determine how many anno secound for each tick of the timer
            _nanosecPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;
            
            //when time passes handel window events
            TimePassed += new Action<long>(RaiseQueuedEvents);

            //when time passes raise refresh time passed (if enough time has passed)
            TimePassed  += new Action<long>(RaiseRefreshTimePassed);
        }
                        
        /// <summary>
        /// The game clock
        /// </summary>
        public Clock Clock
        {
            get { return _clock; }
        }

        /// <summary>
        /// Driver for the game clock
        /// </summary>
        public ClockDriver ClockDriver
        {
            get { return _clockDriver; }
        }
        
        /// <summary>
        /// The number of nano secounds it has been since the game thread satrted
        /// </summary>
        public long CurrentNanosecond
        {
            get { return _timer.ElapsedTicks * _nanosecPerTick; }
        }

        /// <summary>
        /// Start the game driver thread
        /// </summary>
        public void Start()
        {            
            _kill = false;
            
            //start the game timer
            _timer.Start();

            //initlize last nano sec
            _lastTimePassedNanoSec = CurrentNanosecond;

            if (Program.Settings.MultiThread)
            {
                //run the game thread
                _thread.Start();
            }
            else
            {
                //or handel the update event for single threaded
                Program.UserInterface.Graphics.SingleThreadUpdate += new Action(DriveGame);
            }
        }

        /// <summary>
        /// Stop the game driver thread
        /// </summary>
        public void Stop()
        {
            //abort current time slice so we can kill the thread faster
            _clock.AbortSlice();
            _kill = true;

            if (Program.Settings.MultiThread == false)
            {
                //unhandel the update event for single threaded
                Program.UserInterface.Graphics.SingleThreadUpdate -= new Action(DriveGame);
            }
        }

        /// <summary>
        /// Game driver thread function
        /// </summary>
        private void ThreadFunction()
        {
            //run the game loop until we kill it
            while (_kill == false)
            {
                DriveGame();
            }
        }


        /// <summary>
        /// Drive the game forward
        /// </summary>
        private void DriveGame()
        {
            //drive the clock forward
            _clockDriver.DriveClock();

            //raise the time passed event
            RaiseTimePassed();              
        }


        /// <summary>
        /// Call to raise the time passed event
        /// </summary>
        private void RaiseTimePassed()
        {
            //get the nano sec right now
            long nanoSecNow = CurrentNanosecond;

            //the numerb of nano secs that have passed since the last time we raied time passed
            long nanoSecElapsed = nanoSecNow - _lastTimePassedNanoSec;

            //remeber the nano sec
            _lastTimePassedNanoSec = nanoSecNow;

            //riase time passed
            if (TimePassed != null)
            {
                TimePassed(nanoSecElapsed);
            }
        }
        

        /// <summary>
        /// Raise queued gui events.
        /// This method handels the TimePassed event so it gets raised everytime some time passes.
        /// </summary>
        private void RaiseQueuedEvents(long timePassed)
        {
            //process any queued gui events
            double secondsPassed = timePassed / 1000000000.0;
            Program.UserInterface.Graphics.Events.RaiseQueuedEvents(secondsPassed);
        }



        /// <summary>
        /// Raise the RefreshTimePassed event is enough time has passed.
        /// This method handels the TimePassed event so it gets raised everytime some time passes.
        /// </summary>
        private void RaiseRefreshTimePassed(long timePassed)
        {
            _timeSinceLastUpdate += timePassed;

            if (_timeSinceLastUpdate > REFRESH_INTERVAL)
            {
                //lower until not more than update interval
                while (_timeSinceLastUpdate > REFRESH_INTERVAL)
                {
                    _timeSinceLastUpdate -= REFRESH_INTERVAL;
                }

                //raise refresh tiem passed
                if (RefreshTimePassed != null)
                {
                    RefreshTimePassed();
                }
            }
        }

    }
}
