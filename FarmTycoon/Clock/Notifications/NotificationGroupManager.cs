using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// There are lots of objects that all want to be notified about something at the same interval.
    /// For instance all crop trais want to be notified at the same interval.  Instead of each registering with the Clock which is expensive because its worse case n^2 on the nuber of objects.
    /// </summary>
    public class NotificationGroupManager
    {
        /// <summary>
        /// Notification id mapped to what notification gorup it is in
        /// </summary>
        private Dictionary<Notification, NotificationGroup> m_notificationsToGroup = new Dictionary<Notification, NotificationGroup>();

        /// <summary>
        /// the notification groups, keyed on the interval for the group
        /// </summary>
        private Dictionary<double, NotificationGroup> m_groups = new Dictionary<double, NotificationGroup>();


        /// <summary>
        /// The clock that is notifing us
        /// </summary>
        private Clock m_clock;

        /// <summary>
        /// Create a NotificationGroupManager
        /// </summary>
        public NotificationGroupManager(Clock clock)
        {
            m_clock = clock;
        }


        /// <summary>
        /// Add the handler to a notification group with the name passed.
        /// Create a new group if the group with that name does not exsists
        /// </summary>
        public Notification Add(TimeElapsedNotificationHandler handler, double groupPeriod)
        {
            //crate group if one does not exsist
            if (m_groups.ContainsKey(groupPeriod) == false)
            {
                m_groups.Add(groupPeriod, new NotificationGroup(m_clock, groupPeriod));
            }
            
            //add handler to the group
            Notification newNotificationId = m_groups[groupPeriod].AddHandler(handler);

            //add to mapping         
            m_notificationsToGroup.Add(newNotificationId, m_groups[groupPeriod]);

            //return the notification if
            return newNotificationId;
        }

        /// <summary>
        /// Remove the handler from the notification group it is in
        /// </summary>
        public void Remove(Notification notificationId)
        {
            //find which group the handler is in, remove if from the notification to group mapping
            NotificationGroup groupIn = m_notificationsToGroup[notificationId];
            m_notificationsToGroup.Remove(notificationId);

            //remove the notificatio nfrom the group
            groupIn.RemoveHandler(notificationId);

            //if group is empty delete it, and remove it from the dictiornay of groups
            if (groupIn.NumberOfMemebers == 0)
            {
                groupIn.Delete();
                m_groups.Remove(groupIn.Period);
            }
        }

        /// <summary>
        /// Return if the notification is in a group
        /// </summary>
        public bool IsInGroup(Notification notificationId)
        {
            return m_notificationsToGroup.ContainsKey(notificationId);
        }


        /// <summary>
        /// Save the time that remains before the notification method will be called.        
        /// </summary>
        public void WriteTimeRemainingTillNotification(Notification notificationId, ObjectStateInstant state, string countDownName)
        {
            ////find which group that handler is in
            //NotificationGroup groupIn = m_notificationsToGroup[notificationId];
            //groupIn.WriteTimeRemainingTillNotification(state, countDownName);
        }


        /// <summary>
        /// Load the time that remains before the notification method will be called.
        /// </summary>
        public void ReadTimeRemainingTillNotification(Notification notificationId, ObjectStateInstant state, string countDownName)
        {
            ////find which group that handler is in
            //NotificationGroup groupIn = m_notificationsToGroup[notificationId];
            //groupIn.ReadTimeRemainingTillNotification(state, countDownName);
        }

    }
}
