using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FarmTycoon
{

    /// <summary>
    /// Overall state of the game at a single instant.  
    /// Stored in such a way as to be easy to save to disk.
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// State of actions that are in progress
        /// </summary>
        private List<ObjectState> m_actionsStates = new List<ObjectState>();

        /// <summary>
        /// State of each task
        /// </summary>
        private List<ObjectState> m_taskStates = new List<ObjectState>();
        
        /// <summary>
        /// State of each game object
        /// </summary>
        private List<ObjectState> m_gameObjectStates = new List<ObjectState>();

        /// <summary>
        /// State of the objects that are not task, actions, or gameobjects.  For instance Time, and Treasury.
        /// </summary>
        private ObjectState m_globalObjectsState = new ObjectState();



        /// <summary>
        /// State of actions that are in progress
        /// </summary>
        public List<ObjectState> ActionsStates
        {
            get { return m_actionsStates; }
        }

        /// <summary>
        /// State of each task
        /// </summary>
        public List<ObjectState> TaskStates
        {
            get { return m_taskStates; }
        }

        /// <summary>
        /// State of each game object
        /// </summary>
        public List<ObjectState> GameObjectStates
        {
            get { return m_gameObjectStates; }
        }

        /// <summary>
        /// State of the objects that are not task, actions, or gameobjects.  For instance Time, and Treasury.
        /// </summary>
        public ObjectState GlobalObjectsState
        {
            get { return m_globalObjectsState; }
        }


        /// <summary>
        /// Wrtie the object state to the stream
        /// </summary>
        public void Write(StreamWriter writer)
        {
            m_globalObjectsState.Write(writer);
            writer.WriteLine(m_actionsStates.Count);
            foreach (ObjectState actionState in m_actionsStates)
            {
                actionState.Write(writer);
            }        
            writer.WriteLine(m_taskStates.Count);
            foreach (ObjectState taskState in m_taskStates)
            {
                taskState.Write(writer);
            }
            writer.WriteLine(m_gameObjectStates.Count);
            foreach (ObjectState objState in m_gameObjectStates)
            {
                objState.Write(writer);
            }
        }


        /// <summary>
        /// Wrtie the object state from a stream
        /// </summary>
        public void Read(StreamReader reader)
        {
            m_actionsStates.Clear();
            m_taskStates.Clear();
            m_gameObjectStates.Clear();

            m_globalObjectsState = new ObjectState();
            m_globalObjectsState.Read(reader);

            int actionCount = int.Parse(reader.ReadLine());
            for (int i = 0; i < actionCount; i++)
            {
                ObjectState actionState = new ObjectState();
                actionState.Read(reader);
                m_actionsStates.Add(actionState);
            }

            int taskCount = int.Parse(reader.ReadLine());
            for (int i = 0; i < taskCount; i++)
            {
                ObjectState taskState = new ObjectState();
                taskState.Read(reader);
                m_taskStates.Add(taskState);
            }

            int objCount = int.Parse(reader.ReadLine());
            for (int i = 0; i < objCount; i++)
            {
                ObjectState objState = new ObjectState();
                objState.Read(reader);
                m_gameObjectStates.Add(objState);
            }
        }

    }
}
