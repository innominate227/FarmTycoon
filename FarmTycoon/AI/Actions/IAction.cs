using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{

    /// <summary>
    /// Interface for all actions.
    /// Constinas method not specific to a "T"
    /// </summary>
    public interface IAction : ISavable
    {

        /// <summary>
        /// The first location the IActor should go to when doing this action.
        /// This method will be called during the planning phase of the action so it should not relay on the action having a IActor assigned.
        /// </summary>
        Location FirstLocation();

        /// <summary>
        /// The last location the IActor should go to when doing this action.
        /// This method will be called during the planning phase of the action so it should not relay on the action having a IActor assigned.
        /// </summary>
        Location LastLocation();

        /// <summary>
        /// The time expected to complete the action assuming
        /// This method will be called during the planning phase of the action so it should not relay on the action having a IActor assigned.
        /// </summary>        
        double ExpectedTime(DelaySet expectedDelays);

        /// <summary>
        /// Return true if the game objects passed is needed to complete the action.        
        /// </summary>
        bool IsObjectInvolved(IGameObject obj);

        /// <summary>
        /// Description of the action.
        /// This method will be called during the planning phase of the action so it should not relay on the action having a IActor assigned.
        /// </summary>  
        string Description();



        /// <summary>
        /// Called when the actor has arrived at the next desitionation for the action.
        /// Return the number of seconds that the IActor should wait at the location passed.  
        /// This method will only be called after the action has been started.
        /// </summary>
        double ArrivedAtDestination(Location location);


        /// <summary>
        /// Called when the IActor has arrived at the location passed and waited for the approprate amount of time.
        /// The IActor should complete the part of this action that needs to be completed at this location.
        /// This method will only be called after the action has been started.
        /// </summary>
        void DoLocationAction(Location location);
                
        
        /// <summary>
        /// Called by the IActor doing this action right before they start working on the action
        /// </summary>
        void Started();
        
        
        /// <summary>
        /// Call if the IActor has aborted trying to preform the action.  The action should have been assigned if this is to be call, but it has not nessisarily been started.
        /// Do not call Abort on an action that never left the planning state.
        /// </summary>
        void Abort();
                
        /// <summary>
        /// The location the IActor should go to next.
        /// This method will only be called after the action has been started.
        /// Return null if there are no more locations to go to.
        /// This will be called multiple times before the IActor actually arrives at the location.   
        /// </summary>
        Location NextLocation();
        
        
    }
}
