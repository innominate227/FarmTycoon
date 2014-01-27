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
    public class GroupedNotification : Notification
    {        
        /// <summary>
        /// The group the notification is in
        /// </summary>
        private NotificationGroup _groupIn;

        /// <summary>
        /// The index of the sub group the notification is in
        /// </summary>
        private int _subGroupIn;
        
        /// <summary>
        /// The group the notification is in
        /// </summary>
        public NotificationGroup GroupIn
        {
            get { return _groupIn; }
            set { _groupIn = value; }
        }

        /// <summary>
        /// The index of the sub group the notification is in
        /// </summary>
        public int SubGroupIn
        {
            get { return _subGroupIn; }
            set { _subGroupIn = value; }
        }

        /// <summary>
        /// Create a new notification that calls the method passed
        /// </summary>
        public GroupedNotification(TimeElapsedNotificationHandler method) : base(method)
        {
        }

        
    }
}
