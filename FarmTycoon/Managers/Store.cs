//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace FarmTycoon
//{

//    /// <summary>
//    /// Manages the store tasks
//    /// </summary>
//    public class Store : ISavable
//    {

//        public event Action BuyNowTaskChanged;
//        public event Action SellNowTaskChanged;


//        /// <summary>
//        /// List of scheduled store tasks
//        /// </summary>
//        private TaskList m_storeTaskList = new TaskList();

//        /// <summary>
//        /// The buy now stpre task
//        /// </summary>
//        private BuyItemsTask m_buyNowTask = new BuyItemsTask();

//        /// <summary>
//        /// The sell now stpre task
//        /// </summary>
//        private SellItemsTask m_sellNowTask = new SellItemsTask();
        

//        /// <summary>
//        /// Do the current buy now task, and create a new task to do next time we want to buy now
//        /// </summary>
//        public void DoBuyNowTask()
//        {
//            m_buyNowTask.DoTask();
//            BuyNowTask = new BuyItemsTask();
//        }

//        /// <summary>
//        /// Do the current sell now task, and create a new task to do next time we want to sell now
//        /// </summary>
//        public void DoSellNowTask()
//        {
//            m_sellNowTask.DoTask();
//            SellNowTask = new SellItemsTask();
//        }

//        /// <summary>
//        /// List of scheduled store tasks
//        /// </summary>
//        public TaskList StoreTaskList
//        {
//            get { return m_storeTaskList; }
//        }

//        /// <summary>
//        /// The buy now task
//        /// </summary>
//        public BuyItemsTask BuyNowTask
//        {
//            get { return m_buyNowTask; }
//            set 
//            {
//                m_buyNowTask = value;
//                if (BuyNowTaskChanged != null)
//                {
//                    BuyNowTaskChanged();
//                }
//            }
//        }

//        /// <summary>
//        /// The sell now task
//        /// </summary>
//        public SellItemsTask SellNowTask
//        {
//            get { return m_sellNowTask; }
//            set 
//            {
//                m_sellNowTask = value;
//                if (SellNowTaskChanged != null)
//                {
//                    SellNowTaskChanged();
//                }
//            }
//        }

//        public void WriteState(ObjectStateInstant state)
//        {
//            state.SetValue("TaskCount", m_storeTaskList.Tasks.Count);
//            for (int i = 0; i < m_storeTaskList.Tasks.Count; i++)
//            {
//                state.SetValue("Task" + i.ToString(), m_storeTaskList.Tasks[i]);
//            }
//            state.SetValue("BuyNowTask", m_buyNowTask);
//            state.SetValue("SellNowTask", m_sellNowTask);
//        }

//        public void ReadState(ObjectStateInstant state)
//        {
//            //clear all current store tasks
//            foreach (Task task in m_storeTaskList.Tasks)
//            {
//                m_storeTaskList.RemoveTask(task);
//            }

//            int taskCount = state.GetValue<int>("TaskCount");
//            for (int i = 0; i < taskCount; i++)
//            {
//                m_storeTaskList.AddTask(state.GetValue<Task>("Task" + i.ToString()));
//            }
//            BuyNowTask = (BuyItemsTask)state.GetValue<Task>("BuyNowTask");
//            SellNowTask = (SellItemsTask)state.GetValue<Task>("SellNowTask");
//        }
//    }
//}
