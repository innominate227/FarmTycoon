using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TycoonGraphicsLib
{

    /// <summary>
    /// Wraps T and protects T from being read and written at the same time.
    /// Needed for type that can not be written to aomicly
    /// </summary>
    public class Safe<T>
    {
        /// <summary>
        /// The value
        /// </summary>
        private T _val;

        public Safe()
        {
        }
        public Safe(T initial)
        {
            _val = initial;
        }

        /// <summary>
        /// The color
        /// </summary>
        public T Value
        {
            get { lock (this) { return _val; } }
            set { lock (this) { _val = value; } }
        }

    }
}
