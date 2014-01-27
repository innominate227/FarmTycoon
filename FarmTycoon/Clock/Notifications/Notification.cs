using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// Represents a notification that is given by the clock.
    /// </summary>
    public abstract class Notification
    {
        /// <summary>
        /// The notification will be called at this interval.  In real world nano seconds.
        /// </summary>
        private long _intervalNano;
        
        /// <summary>
        /// The method to call.
        /// </summary>
        private TimeElapsedNotificationHandler _method;




        /// <summary>
        /// The notification will be called at this interval.  In real world nano seconds.
        /// </summary>
        public long IntervalNano
        {
            get { return _intervalNano; }
            set
            {
                Debug.Assert(value > 0);
                _intervalNano = value;
            }
        }
        
        /// <summary>
        /// The method to call.
        /// </summary>
        public TimeElapsedNotificationHandler Method
        {
            get { return _method; }
            set { _method = value; }
        }
        
        /// <summary>
        /// Create a new notification that calls the method passed
        /// </summary>
        public Notification(TimeElapsedNotificationHandler method)
        {
            _method = method;
        }

        
    }
}
