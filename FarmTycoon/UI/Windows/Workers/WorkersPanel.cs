using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public delegate bool WorkerFilterDelegate(Worker worker);

    /// <summary>
    /// Worker list panel
    /// </summary>
    public partial class WorkersPanel : TycoonPanel
    {
        /// <summary>
        /// Worker selected in the panel has changed
        /// </summary>
        public event Action SelectedWorkerChanged;

        /// <summary>
        /// List of workers being shown in the panel
        /// </summary>
        private List<Worker> _workerList;

        /// <summary>
        /// Worker in the that is selected
        /// </summary>
        private Worker _selectedWorker;

        /// <summary>
        /// Index of the currently selected worker
        /// </summary>
        private int _selectedWorkerIndex = 0;
                
        /// <summary>
        /// Label for each of the four columns that should be shown
        /// </summary>
        private string[] _columns = new string[0];
                        
        /// <summary>
        /// The list allows workers to be selected
        /// </summary>
        private bool _allowSelection = true;

        /// <summary>
        /// Filter used to determine what workers in the list should actually be shown
        /// </summary>
        private WorkerFilterDelegate _workerFilter = null;
                
        /// <summary>
        /// Dictionary that points to a control for each worker
        /// </summary>
        private Dictionary<Worker, WorkerPanel> _workerControls = new Dictionary<Worker, WorkerPanel>();

        /// <summary>
        /// If non-null is used instead of the workers list to determine what workers to show in the panel.
        /// </summary>
        private IHoldsWorkers _building = null;



        public WorkersPanel()
        {
            //intilize
            InitializeComponent();

            Program.GameThread.RefreshTimePassed += new Action(Refresh);
        }

        public void Delete()
        {
            Program.GameThread.RefreshTimePassed -= new Action(Refresh);
            if (_building != null)
            {
                _building.WorkersInside.Changed -= new Action(Refresh);
            }
        }


        /// <summary>
        /// Set the columns for the item list
        /// </summary>
        public void SetColumns(params string[] columnNames)
        {
            _columns = columnNames;

            TycoonLabel[] columnLabels = new TycoonLabel[] { Info1Label, Info2Label, Info3Label, Info4Label };
            for (int i = 0; i < 4; i++)
            {
                if (columnNames.Length > i)
                {
                    columnLabels[i].Text = columnNames[i];
                }
            }
        }


        /// <summary>
        /// If non-null is used for each worker to set the value for the column that tells wether they inside the building or not
        /// </summary>
        public IHoldsWorkers Building
        {
            get { return _building; }
            set 
            {
                if (_building != null)
                {
                    _building.WorkersInside.Changed -=new Action(Refresh);
                }
                
                _building = value;

                if (_building != null)
                {
                    _building.WorkersInside.Changed += new Action(Refresh);
                }

                Refresh();
            }
        }
                
        /// <summary>
        /// List of workers to shown in the panel
        /// </summary>
        public List<Worker> WorkerList
        {
            get { return _workerList; }
            set 
            {
                _workerList = value;                
                Refresh();
            }
        }
        
        /// <summary>
        /// The worker that is selected
        /// </summary>
        public Worker SelectedWorker
        {
            get 
            {                
                return _selectedWorker; 
            }
            set
            {
                _selectedWorker = value;
                Refresh();
            }
        }
        
        /// <summary>
        /// The list allows items to be selected
        /// </summary>
        public bool AllowSelection
        {
            get { return _allowSelection; }
            set { _allowSelection = value; }
        }

        /// <summary>
        /// Set the filter to use to decide what items to show.
        /// Set to null to show all items
        /// </summary>
        public void SetFilter(WorkerFilterDelegate workerFilter)
        {
            _workerFilter = workerFilter;
            Refresh();
        }
        
        
        /// <summary>
        /// Refresh the workers list
        /// </summary>
        public void Refresh()
        {
            //dont do anything if there is no item list
            if (_workerList == null && _building == null) { return; }

            //get a list of workers to show
            //also search to make sure the currently selected worker is still being shown
            List<Worker> workersToShow = new List<Worker>();
            bool foundSelectedWorker = false;
            foreach (Worker worker in GetWorkersList())
            {
                if (_workerFilter != null && _workerFilter(worker) == false)
                {
                    //if filtering dont show items that dont pass the filter
                    continue;
                }

                workersToShow.Add(worker);
                if (worker == _selectedWorker)
                {
                    foundSelectedWorker = true;
                }
            }

            //the selected worker is not being shown any more choose a new worker to select
            if (foundSelectedWorker == false && _allowSelection)
            {
                _selectedWorker = null;
                if (workersToShow.Count > 0)
                {
                    if (workersToShow.Count <= _selectedWorkerIndex)
                    {
                        _selectedWorkerIndex = workersToShow.Count - 1;
                    }
                    _selectedWorker = workersToShow[_selectedWorkerIndex];                    
                }
            }
            
            //hide controls for all workers that are no longer showing
            foreach (Worker worker in _workerControls.Keys)
            {
                if (workersToShow.Contains(worker) == false)
                {
                    TycoonControl control = _workerControls[worker];
                    control.Visible = false;
                }
            }
                        
            //create or update panels for items that are showing
            int workerNum = 0;
            foreach (Worker worker in workersToShow)
            {
                //if this the selected worker
                bool isSelected = (worker == _selectedWorker);
                if (isSelected)
                {
                    _selectedWorkerIndex = workerNum;
                }
                                
                //create a new control for new items
                if (_workerControls.ContainsKey(worker) == false)
                {
                    WorkerPanel newControl = new WorkerPanel();
                    newControl.SetColumns(_columns);                    
                    newControl.Left = 0;                                        
                    newControl.AnchorBottom = false;
                    newControl.AnchorTop = true;
                    newControl.AnchorLeft = true;
                    newControl.AnchorRight = false;
                    newControl.Worker = worker;                    
                    newControl.IsSelectable = _allowSelection;
                    newControl.BorderColor = newControl.BackColor;                    
                    
                    //if we are allowing selection handel Selected event
                    if (_allowSelection)
                    {
                        newControl.Selected += new Action<WorkerPanel>(WorkerPanel_Selected);
                    }

                    //add to controls dictionary
                    _workerControls.Add(worker, newControl);

                    //add control to panel
                    ItemPanelInner.AddChild(newControl);
                }
                
                //update the new/old control
                WorkerPanel control = _workerControls[worker];
                control.Tag = workerNum;
                control.Top = workerNum * control.Height;      
                control.IsSelected = isSelected;
                SetTextForInsideBuilding(control);
                control.Visible = true;
                control.Refresh();                

                workerNum++;
            }
        }


        /// <summary>
        /// Get the list of workers.  This is either the workers List, or the building we are showing if we are showing a building
        /// </summary>
        private List<Worker> GetWorkersList()
        {
            if (_building != null)
            {
                List<Worker> toRet = new List<Worker>();
                toRet.AddRange(_building.WorkersInside.WorkersInside);
                toRet.AddRange(_building.WorkersInside.WorkersHeadingToward);
                return toRet;
            }
            else
            {
                return _workerList;
            }
        }


        /// <summary>
        /// If we are showing Inside Break house state in the panel set the text for that
        /// </summary>
        private void SetTextForInsideBuilding(WorkerPanel control)
        {
            if (_building == null) { return; }

            bool spotReserved = _building.WorkersInside.WorkersWithSpotReserved.Contains(control.Worker);
            bool inside = _building.WorkersInside.WorkersInside.Contains(control.Worker);
            bool headingToward = _building.WorkersInside.WorkersHeadingToward.Contains(control.Worker);
            
            //text is different depending on the type of building
            if (_building is BreakHouse)
            {
                if (spotReserved && inside)
                {
                    control.InsideBuildingText = "Resting";
                }
                else if (spotReserved == false && inside)
                {
                    control.InsideBuildingText = "Waiting";
                }
                else if (headingToward)
                {
                    control.InsideBuildingText = "Heading To";
                }
            }
            else if (_building is ProductionBuilding)
            {
                if (inside)
                {
                    control.InsideBuildingText = "Worker";
                }
                else if (headingToward)
                {
                    control.InsideBuildingText = "Heading To";
                }
            }
        }


        private void WorkerPanel_Selected(WorkerPanel selected)
        {
            //do nothing if already selected
            if (selected.Worker == _selectedWorker) { return; }

            //unselect old and select new
            _workerControls[_selectedWorker].IsSelected = false;
            _workerControls[selected.Worker].IsSelected = true;

            //update currently selected item, and panel
            _selectedWorker = selected.Worker;
            _selectedWorkerIndex = (int)_workerControls[selected.Worker].Tag;

            //selected worker has changed
            if (SelectedWorkerChanged != null)
            {
                SelectedWorkerChanged();
            }
        }

    }
}
