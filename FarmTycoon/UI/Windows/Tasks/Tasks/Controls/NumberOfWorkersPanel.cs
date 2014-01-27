using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{

    /// <summary>
    /// Number of workers panel
    /// </summary>
    public partial class NumberOfWorkersPanel : TycoonPanel
    {
        public event Action NumberOfWorkersChanged;

        /// <summary>
        /// The list of workers that are preferred to do the task
        /// </summary>
        private List<Worker> _preferredList;

        /// <summary>
        /// The number of workers
        /// </summary>
        private int _numberOfWorkers = 1;

        /// <summary>
        /// Should the player be allowed to select 0 workers
        /// </summary>
        private bool _allow0Workers = false;

        public NumberOfWorkersPanel()
        {
            //intilize
            InitializeComponent();
                        
            //handel increase and decrease buttons
            increaseWorkersButton.Clicked += new Action<TycoonControl>(delegate
            {
                //if (NumberOfWorkers >= 10) { NumberOfWorkers += 10; }

                NumberOfWorkers++;
            });
            decreaseWorkersButton.Clicked += new Action<TycoonControl>(delegate
            {
                NumberOfWorkers--;
            });

            PreferredWorkersButton.Clicked += new Action<TycoonControl>(delegate
            {
                new PreferredWorkersWindow(_preferredList);                
            });
        }

        
        /// <summary>
        /// Should the player be allowed to select 0 workers
        /// </summary>
        public bool Allow0Workers
        {
            get { return _allow0Workers; }
            set { _allow0Workers = value; }
        }

        public int NumberOfWorkers
        {
            get { return _numberOfWorkers; }
            set
            {
                if (_allow0Workers)
                {
                    if (value < 0) { value = 0; }
                }
                else
                {
                    if (value < 1) { value = 1; }
                }
                if (value > 99) { value = 99; }
                _numberOfWorkers = value;
                Refresh();
                if (NumberOfWorkersChanged != null)
                {
                    NumberOfWorkersChanged();
                }
            }
        }

        /// <summary>
        /// The list of workers that are preferred to do the task
        /// </summary>
        public List<Worker> PreferredList
        {
            get { return _preferredList; }
            set { _preferredList = value; }
        }        


        private void Refresh()
        {            
            numberOfWorkersLabel.Text = _numberOfWorkers.ToString();         
        }

         

    }
}
