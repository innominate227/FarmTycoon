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
    public class SoloNotification : Notification
    {        
        /// <summary>
        /// The count downs to when the notification will be raised.   In real world nano seconds.
        /// </summary>
        private long _countDownNano;
        
        /// <summary>
        /// Node of this notification in the to notify list
        /// </summary>
        private LinkedListNode<SoloNotification> _node;        

        

        /// <summary>
        /// The count downs to when the notification will be raised.   In real world nano seconds.
        /// </summary>
        public long CountDownNano
        {
            get { return _countDownNano; }
            set { _countDownNano = value; }
        }
        
        /// <summary>
        /// Node of this notification in the tonotify list.
        /// If it is not in that list this should be null
        /// </summary>
        public LinkedListNode<SoloNotification> Node
        {
            get { return _node; }
            set { _node = value; }
        }


        /// <summary>
        /// Create a new notification that calls the method passed
        /// </summary>
        public SoloNotification(TimeElapsedNotificationHandler method) : base(method)
        {
        }
        
    }
}
