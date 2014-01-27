using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    
    public partial class PreferredWorkerPanel : TycoonPanel
    {

        /// <summary>
        /// Worker being shown
        /// </summary>
        private Worker _worker;
        

        public PreferredWorkerPanel()
        {
            //intilize
            InitializeComponent();


            CheckButton.Clicked += new Action<TycoonControl>(delegate
            {
                CheckButton.Depressed = !CheckButton.Depressed;
            });
        }

        /// <summary>
        /// Worker being shown
        /// </summary>
        public Worker Worker
        {
            get { return _worker; }
            set 
            {
                _worker = value;
                WorkerNameLabel.Text = _worker.Name;
            }
        }

        /// <summary>
        /// Is worker selected as perferred
        /// </summary>
        public bool Selected
        {
            get { return CheckButton.Depressed; }
            set { CheckButton.Depressed = value; }
        }

    }
}
