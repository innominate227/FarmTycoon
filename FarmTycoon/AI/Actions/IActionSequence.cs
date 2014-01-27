using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{

    /// <summary>
    /// Methods shared between actions seqwuences with different T
    /// </summary>    
    public interface IActionSequence : ISavable
    {
        
        /// <summary>
        /// Get the expected time it will take to do all the actions in the action sequence.
        /// Assuming the actor has started at the start location for the first action
        /// </summary>
        double ExpectedTime();
               

        /// <summary>
        /// Get the expected time it will take to do all the actions in the action sequence.
        /// Assuming the actor has started on foot from the startLand
        /// </summary>
        double ExpectedTime(Location startLocation);
                

        /// <summary>
        /// Inform the action sequence that the actor has aborted the sequence. 
        /// </summary>
        void Abort();
        
        /// <summary>
        /// Inform the action sequence that the actor has finished the sequence
        /// </summary>
        void Finished();
                
        /// <summary>
        /// Is the game object passed involved with any of the actions in the sequence
        /// </summary>
        bool IsObjectInvolved(IGameObject obj);
        
    }
}
