using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TycoonGraphicsLib
{
    /// <summary>
    /// Value that can be set by Thread1.  Once Thread1 is happy with the value.  If puts the values into readyA or readyB.
    /// Then Thread2 when its wants the value take it from readyA or readyB.
    /// We dont want to Thread2 to take ready values from the same place that Thread1 is placing them (bacause then we dont know if they were actually taken)
    /// So one thread should always be doing MakeReady on A while the other thread does UseReady on B.  Or vica versa.
    /// </summary>    
    internal class DelayedValue<T>
    {
        /// <summary>
        /// The value that has been fully set
        /// </summary>
        private T _current;

        /// <summary>
        /// Value between dealyed and current.  
        /// Thread T1 is happy with its value in delayed, and ready to push them to T2.  
        /// It puts them in readyA if it added to changeListA 
        /// </summary>
        private T _readyA;

        /// <summary>
        /// Value between dealyed and current.  
        /// Thread T1 is happy with its value in delayed, and ready to push them to T2.  
        /// It puts them in readyB if it was added to changeListB 
        /// </summary>
        private T _readyB;

        /// <summary>
        /// The value whos set is being delayed.
        /// </summary>
        private T _delayed;
        
        /// <summary>
        /// Create the delayed value.
        /// </summary>
        public DelayedValue(T inital)
        {
            //the constuctore should inherintly only every used by one thread, because just one thread creates this class. 
            //so its safe to touch all the values
            _current = inital;
            _readyA = inital;
            _readyB = inital;
            _delayed = inital;
        }

        /// <summary>
        /// The fully set current value
        /// </summary>
        public T Current
        {
            get { return _current; }            
        }

        /// <summary>
        /// The delayed value
        /// </summary>
        public T Delayed
        {
            set { _delayed = value; }
            get { return _delayed; }
        }


        /// <summary>
        /// Set the delayed value is ready to be used.
        /// But it will not become currnet until the thread control current wants it current
        /// </summary>
        public void MakeReady(bool makeReadyA)
        {
            if (makeReadyA)
            {
                _readyA = _delayed;
            }
            else
            {
                _readyB = _delayed;
            }
        }

        /// <summary>
        /// The thread using current is ready to use the new value
        /// </summary>
        public void UseReady(bool useReadyA)
        {
            if (useReadyA)
            {
                _current = _readyA;
            }
            else
            {
                _current = _readyB;
            }
        }

    }








    /// <summary>
    /// Value that can be set by Thread1.  And then Thread 2 can later decide it wants to use the value, and propigate it to current.
    /// </summary>    
    internal class SimpleDelayedValue<T>
    {
        /// <summary>
        /// The value that has been fully set
        /// </summary>
        private T _current;
        
        /// <summary>
        /// The value whos set is being delayed.
        /// </summary>
        private T _delayed;

        /// <summary>
        /// Create the delayed value.
        /// </summary>
        public SimpleDelayedValue(T inital)
        {
            //the constuctore should inherintly only every used by one thread, because just one thread creates this class. 
            //so its safe to touch all the values
            _current = inital;
            _delayed = inital;

            if (typeof(T) == typeof(double))
            {
                throw new Exception("The way this class is being used is not thread safe with doubles, or other 64 bit values");
            }
        }

        /// <summary>
        /// The fully set current value
        /// </summary>
        public T Current
        {
            get { return _current; }
        }

        /// <summary>
        /// The delayed value
        /// </summary>
        public T Delayed
        {
            set { _delayed = value; }
            get { return _delayed; }
        }
                
        /// <summary>
        /// The thread using current is ready to use the new value (return if the _current value was actually changed)
        /// Only safe to do this not in a lock if its a 32 bit type
        /// </summary>
        public bool UseDelayed()
        {
            if (_current.Equals(_delayed))
            {                
                return false;
            }
            else
            {
                _current = _delayed;
                return true;
            }
        }

    }
}
