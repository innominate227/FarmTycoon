using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{    
    /// <summary>
    /// There are lots of objects that all want to be notified about something at the same interval.
    /// For instance all crop trais want to be notified at the same interval.  
    /// Instead of each registering with the Clock which is expensive because its worse case n^2 on the nuber of noitification.  Create a notification group.
    /// Its also bad to have all the objects notified at the exact same time,  so the objects are split into sub groups, and one sub group is notified at a time.
    /// </summary>
    public class NotificationGroup
    {
        /// <summary>
        /// The number of subgroups to split the notification group into
        /// </summary>
        private const int SUB_GROUPS = 100;

        /// <summary>
        /// The notification used to notify the notifications in this group
        /// </summary>
        private SoloNotification _notificaton;

        /// <summary>
        /// The clock we are using
        /// </summary>
        private Clock _clock;
        
        /// <summary>
        /// The notifications in the notification group
        /// </summary>
        private HashSet<GroupedNotification>[] _subgroup = new HashSet<GroupedNotification>[SUB_GROUPS];

        /// <summary>
        /// The interval real world nano seconds that objects in this group are notified at
        /// </summary>
        private long _intervalNano = 0;

        /// <summary>
        /// The next subgroup to notify
        /// </summary>
        private int _nextSubgroup = 0;

        /// <summary>
        /// Count of all the notifications in this group
        /// </summary>
        private int _count = 0;

        /// <summary>
        /// Random number generator
        /// </summary>
        private Random _rnd = new Random();



        /// <summary>
        /// Get the number of notification in the group
        /// </summary>
        public int Count
        {
            get { return _count; }
        }


        /// <summary>
        /// The next subgroup to notify.
        /// This should only be set when loading the game
        /// </summary>
        public int NextSubgroup
        {
            get { return _nextSubgroup; }
            set { _nextSubgroup = value; }
        }


        /// <summary>
        /// The interval real world nano seconds that objects in this group are notified at
        /// </summary>
        public long IntervalNano
        {
            get { return _intervalNano; }
        }

        /// <summary>
        /// The solo notification that notifies this notification group that it should notifiy its grouped notifications
        /// </summary>
        public SoloNotification Notification
        {
            get { return _notificaton; }
        }






        /// <summary>
        /// Create a notification group, to raised notifications at the interval passed
        /// </summary>
        public NotificationGroup(Clock clock, long intervalNano)
        {
            //make sure the interval does not loose its percision when split into sub groups
            Debug.Assert(double.Parse((intervalNano / SUB_GROUPS).ToString()) * SUB_GROUPS == intervalNano);

            _intervalNano = intervalNano;

            //register for the notifications
            _clock = clock;
            _notificaton = (SoloNotification)_clock.RegisterNotificationNano(NotificationMethod, _intervalNano / SUB_GROUPS, false);

            //create the sub groups
            for (int i = 0; i < SUB_GROUPS; i++)
            {
                _subgroup[i] = new HashSet<GroupedNotification>();
            }
        }

        /// <summary>
        /// Delete the notification group
        /// </summary>
        public void Delete()
        {
            _clock.RemoveNotification(_notificaton);
        }
        
        /// <summary>
        /// Add a notification to the group
        /// </summary>
        public GroupedNotification RegisterNotification(TimeElapsedNotificationHandler method)
        {
            //create a new notification
            GroupedNotification newNotification = new GroupedNotification(method);

            //set the group the notification is in
            newNotification.GroupIn = this;
            newNotification.IntervalNano = _intervalNano;

            //randomly choose a subgroup to put the notification in
            int subGroupIndex = _rnd.Next(SUB_GROUPS);
            
            //add it to the subgroup
            _subgroup[subGroupIndex].Add(newNotification);
            _count++;

            //set the subgroup it is in
            newNotification.SubGroupIn = subGroupIndex;
            
            //return the new notifcation
            return newNotification;
        }

        /// <summary>
        /// Remove a notification from the group
        /// </summary>
        public void RemoveNotification(GroupedNotification notification)
        {
            //determine the sub group the notification is in
            int subGroupIndex = notification.SubGroupIn;

            //remove it from the subgroup
            _subgroup[subGroupIndex].Remove(notification);
            _count--;
        }

        /// <summary>
        /// Notification method that the clock calls
        /// </summary>
        private void NotificationMethod()
        {
            //notify everyone in the subgroup
            //TODO: would like to remove the toarray here (critical path), but a notification can remove itself from a subgroup during its notification call
            foreach (GroupedNotification notification in _subgroup[_nextSubgroup].ToArray())
            {
                notification.Method();
            }

            //notify the next group next time
            _nextSubgroup++;

            //wrap around
            if (_nextSubgroup >= SUB_GROUPS)
            {
                _nextSubgroup = 0;
            }
        }
        

        /// <summary>
        /// Move the notification to a different sub group.  
        /// This is used when loading to ensure the notification is in the same subgroup it was in when we saved.
        /// </summary>
        public void MoveNotificationToDifferentSubGroup(int newSubGroup, GroupedNotification notification)
        {
            //remove it from the old subgroup
            _subgroup[notification.SubGroupIn].Remove(notification);

            //add it to the new subgroup
            _subgroup[newSubGroup].Add(notification);

            //set the sub group its in now
            notification.SubGroupIn = newSubGroup;
        }


    }
}
