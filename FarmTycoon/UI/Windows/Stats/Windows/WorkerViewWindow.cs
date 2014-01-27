using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public partial class WorkerViewWindow : TycoonWindow
    {
        /// <summary>
        /// Worker being shown in the window
        /// </summary>
        private Worker _worker;

        public WorkerViewWindow(Worker worker)
        {
            _worker = worker;

            //initilize componenet
            InitializeComponent();

            this.TitleText = _worker.Name;

            ////setup view panel
            //m_worker.Moved += new Action(RefreshWorkerView);
            //RefreshWorkerView();            
            
            ////remove the window on clicking close
            //this.CloseClicked += new Action<TycoonWindow>(delegate
            //{
            //    _worker.Moved -= new Action(RefreshWorkerView);
            //    _worker.CurrentActionChanged -= new Action(RefreshCurrentAction);                
            //    this.Visible = false;
            //    Program.UserInterface.WindowManager.RemoveWindow(this);
            //});
            
            ////refresh action now, and when it changes
            //RefreshCurrentAction();
            //m_worker.CurrentActionChanged += new Action(RefreshCurrentAction);

            //add window to graphics
            Program.UserInterface.WindowManager.AddWindow(this);
        }
        
        private void RefreshWorkerView()
        {
            //workerWorldView.ViewX = ((_worker.WorkerPositionManager.LocationLeaving.X * (16 - _worker.WorkerPositionManager.DistToDest)) + (_worker.WorkerPositionManager.LocationGoing.X * _worker.WorkerPositionManager.DistToDest)) / 16.0f;
            //workerWorldView.ViewY = ((_worker.WorkerPositionManager.LocationLeaving.Y * (16 - _worker.WorkerPositionManager.DistToDest)) + (_worker.WorkerPositionManager.LocationGoing.Y * _worker.WorkerPositionManager.DistToDest)) / 16.0f;
            //workerWorldView.ViewZ = ((_worker.WorkerPositionManager.LocationLeaving.Z * (16 - _worker.WorkerPositionManager.DistToDest)) + (_worker.WorkerPositionManager.LocationGoing.Z * _worker.WorkerPositionManager.DistToDest)) / 16.0f;
        }

        private void RefreshCurrentAction()
        {
            //if (_worker.WorkerActionManager.CurrentAction != null)
            //{
            //    workerStatusLabel.Text = _worker.WorkerActionManager.CurrentAction.Description();
            //}
            //else
            //{
            //    workerStatusLabel.Text = "Pacing";
            //}
        }   



    }
}
