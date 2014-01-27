using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FarmTycoon
{
    /// <summary>
    /// List of actions sequences that are currently active.    
    /// </summary>
    public class ActiveActionList : ISavable
    {
        #region Member Vars
        
        /// <summary>
        /// List of active actions sequences
        /// </summary>
        private List<IActionSequence> _activeActionSequences = new List<IActionSequence>();
                
        #endregion
        
        #region Logic

        /// <summary>
        /// Add an active action sequence to the list of active action seuqnces
        /// </summary>
        public void AddActiveActionSequence(IActionSequence activeSequence)
        {
            _activeActionSequences.Add(activeSequence);
        }

        /// <summary>
        /// Remove an action sequence from the list of active action seuqnces
        /// </summary>
        public void RemoveActionSequence(IActionSequence noLongerActiveSequence)
        {
            _activeActionSequences.Remove(noLongerActiveSequence);
        }

        /// <summary>
        /// Is the game object passed involved with an active action
        /// </summary>
        public bool IsObjectInvolvedWithActiveAction(IGameObject obj)
        {
            foreach (IActionSequence actionSequence in _activeActionSequences)
            {
                if (actionSequence.IsObjectInvolved(obj))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Find all ActionSequences involving the game object passed
        /// </summary>
        public List<IActionSequence> ActionSequencesInvolving(IGameObject obj)
        {
            List<IActionSequence> ret = new List<IActionSequence>();

            foreach (IActionSequence actionSequence in _activeActionSequences)
            {
                if (actionSequence.IsObjectInvolved(obj))
                {
                    ret.Add(actionSequence);
                }
            }
            return ret;
        }

        /// <summary>
        /// Abort all actions (used when closing the game)
        /// </summary>
        public void AbortAll()
        {
            foreach (IActionSequence actionSequence in _activeActionSequences.ToArray())
            {
                actionSequence.Abort();
            }
        }

        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            writer.WriteObjectList<IActionSequence>(_activeActionSequences);
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            _activeActionSequences = reader.ReadObjectList<IActionSequence>();
        }

        public void AfterReadStateV1()
        {
        }
        #endregion
    }
}
