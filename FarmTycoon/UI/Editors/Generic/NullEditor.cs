using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    /// <summary>
    /// Editor that does nothing
    /// </summary>
    public class NullEditor : Editor
    {

        /// <summary>
        /// Create a new selection editor.
        /// </summary>
        public NullEditor() : base() { }

        /// <summary>
        /// Start editing
        /// </summary>
        protected override void StartEditingInner()
        {
        }

        /// <summary>
        /// Stop editing
        /// </summary>
        protected override void StopEditingInner()
        {
        }
               


    }
}
