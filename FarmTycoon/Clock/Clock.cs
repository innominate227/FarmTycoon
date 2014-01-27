using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace FarmTycoon
{
    /// <summary>
    /// delegate for handeling time elapsed notifications
    /// </summary>
    public delegate void TimeElapsedNotificationHandler();
    
    public class Clock
    {
        /// <summary>
        /// If the clock spends more than 100ms in the MoveTimeForward method this event is raised once every 100ms
        /// </summary>
        public event Action StarvationPreventionEvent;

        /// <summary>
        /// How often the startvation prevention event should be raised
        /// </summary>
        private const long STARTVATION_PREVENTION_INTERVAL = 100000000; //100ms

        /// <summary>
        /// Number of real world seconds in one game day.
        /// </summary>
        public const long SEC_PER_DAY = 20;

        /// <summary>
        /// Number of real world nano-seconds in one game day.
        /// </summary>
        public const long NANO_SEC_PER_DAY = SEC_PER_DAY * 1000000000;

        
        /// <summary>
        /// Groups of notifications, keyed on the interval for the group in nano seconds
        /// </summary>
        private Dictionary<long, NotificationGroup> _notificationGroups = new Dictionary<long, NotificationGroup>();
                        
        /// <summary>
        /// All the notifications
        /// </summary>
        private HashSet<SoloNotification> _notifications = new HashSet<SoloNotification>();

        /// <summary>
        /// list of all the nodes that need to notified this time slice
        /// </summary>
        private LinkedList<SoloNotification> _notifyThisSlice = new LinkedList<SoloNotification>();

        /// <summary>
        /// How manay nano seconds are left in the current time slice.
        /// </summary>
        private long _nanosLeftThisSlice = 0;
        
        /// <summary>
        /// Reference to the game thread
        /// </summary>
        private GameThread _gameThread;

        /// <summary>
        /// Create a new clock
        /// </summary>
        public Clock(GameThread gameThread)
        {
            _gameThread = gameThread;
        }
        
        /// <summary>
        /// Tell the clock that time has passed.  Pass the number of real world nanoseconds that have passed since the last call (the time slice).
        /// To play at non 1x speed a greater or lesser number of nano seconds can be passed.
        /// </summary>        
        public void MoveTimeForward(long slice)
        {
            //all of them are left
            _nanosLeftThisSlice = slice;

            //we want to do a starvation prevention evnet every 100ms this method has had control.  get the time we gave control to the method.
            long timeOfLastStarvationPrevention = _gameThread.CurrentNanosecond;
            long timeAtStart_Debug = _gameThread.CurrentNanosecond;

            //add the nodes we need to notify this slice
            foreach (SoloNotification notification in _notifications)
            {
                //decrement all count downs by the time passed
                notification.CountDownNano -= slice;
                
                //if count down less than 0 then add to the toNotify list
                if (notification.CountDownNano < 0)
                {
                    //add to notify list
                    LinkedListNode<SoloNotification> notificationNode = new LinkedListNode<SoloNotification>(notification);
                    SortInNotifyList(notificationNode);

                    //tell it its node in the list
                    notification.Node = notificationNode;
                }
            }

            //notify them until there are none left to notify
            while (_notifyThisSlice.Count > 0)
            {
                //notify the one in the front of the list
                LinkedListNode<SoloNotification> frontNode = _notifyThisSlice.First;
                SoloNotification front = frontNode.Value;

                Debug.Assert(front.CountDownNano <= 0);
                
                //determine how much time is left in the slice.  This will be the opposite of the notifications count down.
                //let say count down was 2 before slice and slice was 100.  
                //first notification will occur 2 into the slice at wich point count down will be -98 
                //next notification will occur 12 into the slice at wich point count down will be -88 
                _nanosLeftThisSlice = -1 * front.CountDownNano;
                
                //call time passed method 
                //the notification may delete itself during this method in which case it will have removed itself from the notify list, and set its Node property to null.
                //it also might change its interval during the call so dont update the countdown until afterward
                front.Method();

                //if notification was deleted dont do anythting
                if (front.Node == null) { continue; }

                //add to the count down the interval so the count down restarts
                front.CountDownNano += front.IntervalNano;
                                                                                
                //if count down is still less than 0 we will need to call again.  so add it back to the list keeping it sorted
                if (front.CountDownNano < 0)
                {
                    //move it to the correct place in the notify list
                    SortInNotifyList(frontNode);
                }
                else
                {
                    //remove it from the notify this slice list
                    _notifyThisSlice.Remove(frontNode);

                    //front is no longer in the list, so clear its node property
                    front.Node = null;
                }
                                                               
                //determine how much time has passed in this method
                long timePassedInMethod = _gameThread.CurrentNanosecond - timeOfLastStarvationPrevention;
                if (timePassedInMethod > STARTVATION_PREVENTION_INTERVAL && StarvationPreventionEvent != null)
                {
                    StarvationPreventionEvent();
                    timeOfLastStarvationPrevention = _gameThread.CurrentNanosecond;
                }

            }

            //no nano seconds left in the slice
            _nanosLeftThisSlice = 0;

            long timeInMethod_Debug = _gameThread.CurrentNanosecond - timeAtStart_Debug;
            //Program.Debug.SetDebug3("Time in MoveForward: " + timeInMethod_Debug.ToString());
        }

        /// <summary>
        /// Abort the current slice.
        /// </summary>
        public void AbortSlice()
        {
            //if this is the case were not actually in a slice (or everybody in the slice is already above 0 so its about to end anyway)
            if (_notifyThisSlice.Count == 0){ return; }

            //to abort the slice we add time back to the count down of all notifies.
            //add time back such that all notify have count down less than 0.  To ensure this we add the oposite of the count down of the node in the front
            //of the notify list.  The notify list is always sorted so this will be the node that has had the most time pass
            long timeToAddBack = -1 * _notifyThisSlice.First.Value.CountDownNano;

            //add the time back to the count down for each node
            foreach (SoloNotification notification in _notifications)
            {
                //increment all the count downs so they are all above 0
                notification.CountDownNano += timeToAddBack;
            }

            //clear the node of all the notification in the notiry list since we are about to clear the list
            foreach (SoloNotification notification in _notifyThisSlice)
            {
                notification.Node = null;
            }
            
            //all count downs are greater than zero now clear the notify this slice list
            _notifyThisSlice.Clear();

            //slice is over
            _nanosLeftThisSlice = 0;
        }

        /// <summary>
        /// Returns the nuber of notifications.  
        /// (This does not include the notification the clock driver uses to determine the speed the clock is going)
        /// </summary>
        public int NotificationsCount
        {
            get { return _notifications.Count - 1; }
        }

        /// <summary>
        /// Put the TimerCountDown Node in the correct location in the notify list.  Keeping the list sorted by lowest count down first.
        /// If the node was already in the list it will be moved, if it was not it will be added.        
        /// </summary>
        private void SortInNotifyList(LinkedListNode<SoloNotification> countDownNode)
        {            
            //theres only one in there no need to sort
            if (_notifyThisSlice.First == countDownNode && _notifyThisSlice.Count == 1) { return; }
            
            //remove from current location in the list if its in there
            if (countDownNode.List == _notifyThisSlice)
            {
                _notifyThisSlice.Remove(countDownNode);
            }

            //search from the back of the list, we just increase the value so it probably belongs near the back of the list
            LinkedListNode<SoloNotification> checkNode = _notifyThisSlice.Last;
            while (checkNode != null && countDownNode.Value.CountDownNano < checkNode.Value.CountDownNano)
            {
                //we were less so try the prvious
                checkNode = checkNode.Previous;
            }
            
            if (checkNode != null)
            {
                //add right before the node that we are greater than or equal to
                _notifyThisSlice.AddAfter(checkNode, countDownNode);
            }
            else
            {
                //or add to the very front as we are the samllest
                _notifyThisSlice.AddFirst(countDownNode);
            }
        }





        /// <summary>
        /// Create a new solo notification, and add it to the notifications list
        /// </summary>
        private SoloNotification CreateSoloNotification(TimeElapsedNotificationHandler method, long intervalNano)
        {
            //create a new solo notification
            SoloNotification newNotification = new SoloNotification(method);
            
            //set the interval of the notification (in nano seconds)
            newNotification.IntervalNano = intervalNano;

            //start the count down at the notificatoin interval, minus the nano seconds that of the current slice that still need to be resolved
            newNotification.CountDownNano = newNotification.IntervalNano - _nanosLeftThisSlice;
            
            //add to the notificatiosn list
            _notifications.Add(newNotification);
            
            //if count down of the new notification is less than 0 then add it to the list of what to notify this slice
            if (newNotification.CountDownNano < 0)
            {
                //add to notify list
                LinkedListNode<SoloNotification> countOownNode = new LinkedListNode<SoloNotification>(newNotification);                
                SortInNotifyList(countOownNode);

                //tell it its node in the list
                newNotification.Node = countOownNode;
            }
                        
            //TEMP: debug            
            DebugToolWindow.SoloNotifications = "Solo Notifications: " + _notifications.Count.ToString();
            
            //return the notification
            return newNotification;
        }

        /// <summary>
        /// remove the solo notification
        /// </summary>
        private void RemoveSoloNotification(SoloNotification notification)
        {
            //remove from the list of solo notifications
            _notifications.Remove(notification);

            //remove from the notify this slice list if its in there
            if(notification.Node != null)
            {
                _notifyThisSlice.Remove(notification.Node);
                notification.Node = null;
            }

            //TEMP: debug
            DebugToolWindow.SoloNotifications = "Solo Notifications: " + _notifications.Count.ToString();
        }

        /// <summary>
        /// Give this notitification to the notification group it belongs in.  And return the new notification.
        /// Create a new gorup if needed.
        /// </summary>
        private GroupedNotification CreateGroupedNotification(TimeElapsedNotificationHandler method, long intervalNano)
        {
            //determine if a new notification group needs to be created
            if (_notificationGroups.ContainsKey(intervalNano) == false)
            {
                //create the new group
                _notificationGroups.Add(intervalNano, new NotificationGroup(this, intervalNano));
            }
            
            //get the group this notification belongs in
            NotificationGroup group = _notificationGroups[intervalNano];

            //register the notification with the group
            return group.RegisterNotification(method);
        }
        
        /// <summary>
        /// Remove this notitification from the notification group it belongs in.  
        /// Delete the group if it was the last one.
        /// </summary>
        private void RemoveGroupedNotification(GroupedNotification notification)
        {
            //determine what group it is in
            NotificationGroup groupIn = notification.GroupIn;

            //remove from the group its in
            groupIn.RemoveNotification(notification);

            //remove the group
            if (groupIn.Count == 0)
            {                
                groupIn.Delete();
                _notificationGroups.Remove(notification.IntervalNano);
            }
        }

        /// <summary>
        /// Register a time notification handler.  The handler will be called every notifyInterval days.  
        /// If acuracy is not important grouping the notification will increase game speed.  
        /// Group notifications will first be called between 0 and notify interval days from now, and will then be called every notify interval days.
        /// </summary>
        public Notification RegisterNotification(TimeElapsedNotificationHandler method, double interval, bool allowGrouping)
        {
            //determine the interval of the notification (in nano seconds)
            long intervalNano = (long)(interval * NANO_SEC_PER_DAY);

            //register the notification
            return RegisterNotificationNano(method, intervalNano, allowGrouping);
        }

        /// <summary>
        /// Register a time notification handler.  This method takes a interval that has already been converted into nano seconds, and it used internaly you should use the other signature isntead.
        /// If acuracy is not important grouping the notification will increase game speed.  
        /// Group notifications will first be called between 0 and notify interval days from now, and will then be called every notify interval days.
        /// </summary>
        public Notification RegisterNotificationNano(TimeElapsedNotificationHandler method, long intervalNano, bool allowGrouping)
        {
            //register a group or notmal notification depending on what was called            
            if (allowGrouping)
            {
                return CreateGroupedNotification(method, intervalNano);
            }
            else
            {
                return CreateSoloNotification(method, intervalNano);
            }
        }
                
        /// <summary>
        /// Remove the notification
        /// </summary>
        public void RemoveNotification(Notification notification)
        {
            if (notification is GroupedNotification)
            {
                RemoveGroupedNotification(notification as GroupedNotification);
            }
            else
            {
                RemoveSoloNotification(notification as SoloNotification);
            }
        }

        /// <summary>
        /// Update interval for notification.  The old notification will be deleted and a new one returned
        /// </summary>
        public Notification UpdateNotification(Notification notification, double interval)
        {
            //determine the new interval for the notification
            long newInterval = (long)(interval * NANO_SEC_PER_DAY);

            //do nothing is current interval is already correct
            if (notification.IntervalNano == newInterval) { return notification; }

            if (notification is SoloNotification)
            {
                //if it is a solo notification, we can just adjust the notification itself to account for the new interval

                SoloNotification soloNotification = (SoloNotification)notification;

                //set the interval for the notification
                soloNotification.IntervalNano = (long)(interval * NANO_SEC_PER_DAY);

                //start the count down at the notificatoin interval, minus the nano seconds that of the current slice that still need to be resolved
                soloNotification.CountDownNano = soloNotification.IntervalNano - _nanosLeftThisSlice;

                //make sure its in the correct spot in the notift list
                if (soloNotification.CountDownNano < 0)
                {
                    //if the notification doesnt have a node assigned, make it a node
                    if (soloNotification.Node == null)
                    {
                        soloNotification.Node = new LinkedListNode<SoloNotification>(soloNotification);
                    }

                    //sort it in the notify list
                    SortInNotifyList(soloNotification.Node);
                }
                else if (soloNotification.Node != null)
                {
                    //remove it from the notification list as its now greater than 0
                    _notifyThisSlice.Remove(soloNotification.Node);
                    soloNotification.Node = null;
                }

                Debug.Assert(soloNotification.Node == null || soloNotification.CountDownNano < 0);

                //return the updated notification
                return notification;
            }
            else
            {
                //it is a group notification we remove and reregister
                RemoveNotification(notification);
                return RegisterNotification(notification.Method, interval, true);
            }

        }






        /// <summary>
        /// Writes the state of the notification into the state passed
        /// </summary>
        public void WriteNotificationState(BinaryWriter writer, Notification notification)
        {
            if (notification == null)
            {
                //write that the notification is null
                writer.Write('N');
            }
            else if (notification is SoloNotification)
            {
                SoloNotification soloNotification = (SoloNotification)notification;

                //write properties of the solo notification                            
                writer.Write('S');
                writer.Write(soloNotification.IntervalNano);
                writer.Write(soloNotification.CountDownNano);

            }
            else if (notification is GroupedNotification)
            {
                GroupedNotification groupedNotification = (GroupedNotification)notification;

                //write properties of the grouped notification                            
                writer.Write('G');                
                writer.Write(groupedNotification.SubGroupIn);

                //also write properties of the notifiction group
                //these are properties of the group, but it is almost as cheap to rewrite them every time as it would be to write a guid to identify the group, and a lot easier
                writer.Write(groupedNotification.GroupIn.IntervalNano);
                writer.Write(groupedNotification.GroupIn.Notification.CountDownNano);
                writer.Write(groupedNotification.GroupIn.NextSubgroup);                
            }
        }
        
        /// <summary>
        /// Read the state of the notification from the binary reader.  The notification will call the method passed
        /// </summary>
        public Notification ReadNotificationState(BinaryReader reader, TimeElapsedNotificationHandler method)
        {
            char notificationType = reader.ReadChar();

            if (notificationType == 'N')
            {
                return null;
            }
            else if (notificationType == 'S')
            {
                long intervalNano = reader.ReadInt64();
                long countDownNano = reader.ReadInt64();
                
                //create the solo notification
                SoloNotification soloNotification = CreateSoloNotification(method, intervalNano);

                //resotre its count down value
                soloNotification.CountDownNano = countDownNano;

                //retunr the notification
                return soloNotification;
            }
            else if (notificationType == 'G')
            {
                int subGroupIn = reader.ReadInt32();
                long intervalNano = reader.ReadInt64();
                long countDownNano = reader.ReadInt64();
                int nextSubGroup = reader.ReadInt32();
                
                //create the grouped notification
                GroupedNotification groupedNotification = CreateGroupedNotification(method, intervalNano);

                //set the count down on the solo notification the group notification uses
                groupedNotification.GroupIn.Notification.CountDownNano = countDownNano;

                //set the next sub group it should notifiy
                groupedNotification.GroupIn.NextSubgroup = nextSubGroup;

                //move the group notification to the same group it was in on save
                groupedNotification.GroupIn.MoveNotificationToDifferentSubGroup(subGroupIn, groupedNotification);

                //retunr the notification
                return groupedNotification;
            }
            else
            {
                Debug.Assert(false);
                return null;
            }
        }


          

    }
}
