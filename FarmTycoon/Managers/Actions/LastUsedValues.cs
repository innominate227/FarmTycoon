using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    /// <summary>
    /// Keeps track of the values the player last used for various dialog in the game.
    /// </summary>
    public class LastUsedValues : ISavable
    {
        #region Member Vars

        /// <summary>
        /// Keeps track of the number of workers last used for a task done on this game object 
        /// </summary>
        private Dictionary<IGameObject, int> _numberOfWorkersLastUsed = new Dictionary<IGameObject, int>();
        
        /// <summary>
        /// Keeps track of if equipment was used last time for a task done on this game object
        /// </summary>
        private Dictionary<IGameObject, bool> _equipmnetWasLastUsed = new Dictionary<IGameObject, bool>();
        
        /// <summary>
        /// Last name used for the file when saving the game
        /// </summary>
        private string _saveName = "Save";

        #endregion

        #region Properties


        /// <summary>
        /// Last name used for the file when saving the game
        /// </summary>        
        public string SaveName
        {
            get { return _saveName; }
            set { _saveName = value; }
        }

        #endregion

        #region Logic

        /// <summary>
        /// Set the number of workers that were used for a task done on the game object passed.
        /// </summary>
        public void SetNumberOfWorkersLastUsed(IGameObject obj, int numberOfWorkers)
        {
            if (_numberOfWorkersLastUsed.ContainsKey(obj) == false)
            {
                _numberOfWorkersLastUsed.Add(obj, 0);
            }
            _numberOfWorkersLastUsed[obj] = numberOfWorkers;
        }

        /// <summary>
        /// Set the number of workers that were used for a task done on the game object passed.
        /// </summary>
        public int GetNumberOfWorkersLastUsed(IGameObject obj)
        {
            if (_numberOfWorkersLastUsed.ContainsKey(obj) == false)
            {
                return 1;
            }
            return _numberOfWorkersLastUsed[obj];
        }




        /// <summary>
        /// Set the number of workers that were used for a task done on the game object passed.
        /// </summary>
        public void SetEquipmentWasLastUsed(IGameObject obj, bool equipmentWasUsed)
        {
            if (_equipmnetWasLastUsed.ContainsKey(obj) == false)
            {
                _equipmnetWasLastUsed.Add(obj, false);
            }
            _equipmnetWasLastUsed[obj] = equipmentWasUsed;
        }

        /// <summary>
        /// Set the number of workers that were used for a task done on the game object passed.
        /// </summary>
        public bool GetEquipmentWasLastUsed(IGameObject obj)
        {
            if (_equipmnetWasLastUsed.ContainsKey(obj) == false)
            {
                return false;
            }
            return _equipmnetWasLastUsed[obj];
        }

        #endregion

        #region Save Load
		public void WriteStateV1(StateWriterV1 writer)
		{
            writer.WriteInt(_numberOfWorkersLastUsed.Count);
            foreach (IGameObject key in _numberOfWorkersLastUsed.Keys)
            {
                writer.WriteObject(key);
                writer.WriteInt(_numberOfWorkersLastUsed[key]);
            }

            writer.WriteInt(_equipmnetWasLastUsed.Count);
            foreach (IGameObject key in _equipmnetWasLastUsed.Keys)
            {
                writer.WriteObject(key);
                writer.WriteBool(_equipmnetWasLastUsed[key]);
            }  

			writer.WriteString(_saveName);
		}
		
		public void ReadStateV1(StateReaderV1 reader)
		{
            int count = reader.ReadInt();
            for (int num = 0; num < count; num++)
            {
                IGameObject key = reader.ReadObject<IGameObject>();
                int item = reader.ReadInt();
                _numberOfWorkersLastUsed.Add(key, item);
            }

            count = reader.ReadInt();
            for (int num = 0; num < count; num++)
            {
                IGameObject key = reader.ReadObject<IGameObject>();
                bool item = reader.ReadBool();
                _equipmnetWasLastUsed.Add(key, item);
            }

			_saveName = reader.ReadString();
		}
		
		public void AfterReadStateV1()
		{
		}
		#endregion

    }
}
