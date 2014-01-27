using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TycoonGraphicsLib
{
    /// <summary>
    /// enum of all the event types we can raise
    /// </summary>
    internal enum EventType
    {
        MouseDown,
        MouseUp,
        MouseMoved,
        MouseScroll,
        KeyDown,
        KeyUp,
        WindowSizeChanged
    }

    internal class QueuedEvent
    {
        /// <summary>
        /// The type of event to raise
        /// </summary>
        public EventType Type;

        /// <summary>
        /// The first argument to pass with the event
        /// </summary>
        public object Arg1;

        /// <summary>
        /// The second argument to pass with the event
        /// </summary>
        public object Arg2;

        //no events we raise currnetly take two args but that would be here if we did
    }
}
