using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// Handler for handeling day notification
    /// </summary>
    public delegate void DayNotificationHandler();
        
    /// <summary>
    /// Keeps track of the current game date.  Notifies clients when a new date has arrived.
    /// </summary>
    public partial class Calandar : ISavable
    {
        #region Events

        /// <summary>
        /// Raised when the date has changed.
        /// (this should be used only by people who want to actualy know when the date has changed, people 
        /// that just want to know when a day has passed should create a notification with an interval or 1 day, and add it to the notificatio group)
        /// </summary>
        public event Action DateChanged;

        #endregion

        #region Member Vars

        /// <summary>
        /// The current game date (where day 0 is Jan 1 2000, day 12*30 is Jan 1 2001, etc)
        /// </summary>
        private int _date = -1;
        
        /// <summary>
        /// The notification the calandar uses to know when a date passes
        /// </summary>
        private Notification _notification;

        #endregion

        #region Setup Delete

        /// <summary>
        /// Create a new Calandar call setup or readstate before using
        /// </summary>
        public Calandar() { }

        /// <summary>
        /// Setup a calandar
        /// </summary>
        public void Setup()
        {                 
            //interval everytime a day passes
            _notification = Program.GameThread.Clock.RegisterNotification(OneDayPassed, 1.0, false);
        }

        /// <summary>
        /// Delete the calandar object
        /// </summary>
        public void Delete()
        {                         
            //remove notifcation
            Program.GameThread.Clock.RemoveNotification(_notification);        
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// The date (where 0 is Jan 1 2000,  30 is Feb 1 2000, and 360 is Jan 1 2001, etc)
        /// </summary>
        public int Date
        {
            get { return _date; }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Have the calandar move to day 0, with out this it would take the lenght of a day to reach day 0, but we need it to happen immediantly when starting a new game
        /// </summary>
        public void DoDay0()
        {
            Debug.Assert(_date == -1);
            OneDayPassed();
        }
        
        /// <summary>
        /// Handels when a game day has passed
        /// </summary>
        private void OneDayPassed()
        {
            //incrment the date
            _date++;

            //raise day changed event
            if (DateChanged != null)
            {
                DateChanged();
            }
        }

        /// <summary>
        /// Return an int date in a string format        
        /// </summary>
        public static string DateAsString(int date)
        {
            int month = ((date % (12 * 30)) / 30) + 1;
            int dayOfMonth = ((date % (12 * 30)) % 30) + 1;
            int year = (date / (12 * 30)) + 2000;
            return month.ToString("D2") + "/" + dayOfMonth.ToString("D2") + "/" + year.ToString("D4");
        }

        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteInt(_date);
            writer.WriteNotification(_notification);
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _date = reader.ReadInt();
            _notification = reader.ReadNotification(OneDayPassed);
        }

        public void AfterReadStateV1()
        {
        }
        #endregion

    }
}
