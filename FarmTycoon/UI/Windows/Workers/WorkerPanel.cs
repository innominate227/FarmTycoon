using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    
    public partial class WorkerPanel : TycoonPanel
    {
        /// <summary>
        /// Raised when the panel becomes selected
        /// </summary>
        public event Action<WorkerPanel> Selected;

        /// <summary>
        /// Worker being shown
        /// </summary>
        private Worker _worker;
        
        /// <summary>
        /// Is selection allowed for the worker
        /// </summary>
        private bool _allowSelection;
                
        /// <summary>
        /// Labels for each of the fours columns
        /// </summary>
        private Dictionary<int, TycoonLabel> _columns = new Dictionary<int, TycoonLabel>();

        /// <summary>
        /// Text for the custom column
        /// </summary>
        private string _insideBuildingText = "";


        public WorkerPanel()
        {
            //intilize
            InitializeComponent();

            _columns.Add(0, Info1Label);
            _columns.Add(1, Info2Label);
            _columns.Add(2, Info3Label);
            _columns.Add(3, Info4Label);

            for (int colNum = 0; colNum < 4; colNum++)
            {
                _columns[colNum].TextVerticelAlignment = StringAlignment.Center;
                _columns[colNum].TextAlignment = StringAlignment.Far;
                _columns[colNum].DrawNumericValue = true;
                _columns[colNum].DrawDollarSign = false;                                
            }

            ItemImage.IconTexture = "Worker";

            this.Clicked += new Action<TycoonControl>(Item_Clicked);
            NameLabel.Clicked += new Action<TycoonControl>(Item_Clicked);
            ItemImage.Clicked += new Action<TycoonControl>(Item_Clicked);
            Info1Label.Clicked += new Action<TycoonControl>(Item_Clicked);
            Info2Label.Clicked += new Action<TycoonControl>(Item_Clicked);
            Info3Label.Clicked += new Action<TycoonControl>(Item_Clicked);
            Info4Label.Clicked += new Action<TycoonControl>(Item_Clicked);
        }

        private void Item_Clicked(TycoonControl obj)
        {
            if (this.IsSelected == false && _allowSelection)
            {
                this.IsSelected = true;
                if (Selected != null)
                {
                    Selected(this);
                }
            }
        }
        
        /// <summary>
        /// Set the columns for the item list
        /// </summary>
        public void SetColumns(string[] columnNames)
        {
            for (int colNum = 0; colNum < 4; colNum++)
            {
                if (columnNames.Length <= colNum)
                {
                    this.RemoveChild(_columns[colNum]);
                }
                else
                {
                    _columns[colNum].Tag = columnNames[colNum];

                    if (columnNames[colNum] == "Status" || columnNames[colNum] == "Action")
                    {
                        _columns[colNum].DrawNumericValue = false;
                    }
                }
            }
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
                ItemImage.IconTexture = "Worker";
                NameLabel.Text = _worker.Name;             
            }
        }
        
        /// <summary>
        /// Is the worker selectable
        /// </summary>
        public bool IsSelectable
        {
            get { return _allowSelection; }
            set { _allowSelection = value;  }
        }

        /// <summary>
        /// Is the worker selected
        /// </summary>
        public bool IsSelected
        {
            get { return this.BackColor == Color.Blue; }
            set 
            {
                Color colorToSet = Color.FromArgb(192, 64, 64);
                if (value)
                {
                    colorToSet = Color.Blue;
                }

                this.BackColor = colorToSet;
                ItemImage.BackColor = colorToSet;
                ItemImage.ShadowDarkColor = colorToSet;
                ItemImage.ShadowLightColor = colorToSet;
                NameLabel.BackColor = colorToSet;
                NameLabel.BorderColor = colorToSet;
                for (int colNum = 0; colNum < 4; colNum++)
                {
                    if (_columns[colNum].Tag != null)
                    {
                        _columns[colNum].BackColor = colorToSet;
                        _columns[colNum].BorderColor = colorToSet;
                    }
                }
            }
        }


        /// <summary>
        /// Text for if the worker is inside a building or not
        /// </summary>
        public string InsideBuildingText
        {
            get { return _insideBuildingText; }
            set { _insideBuildingText = value; }
        }


        /// <summary>
        /// Refresh the columns for the worker
        /// </summary>
        public void Refresh()
        {
            for (int colNum = 0; colNum < 4; colNum++)
            {
                if (_columns[colNum].Tag == null) { continue; }
                string colTag = _columns[colNum].Tag.ToString();

                if (colTag == "Energy")
                {
                    _columns[colNum].NumericValue = _worker.Traits.GetTraitInstantaneousQuality(SpecialTraits.ENERGY_TRAIT);
                }
                else if (colTag == "Action")
                {
                    _columns[colNum].Text = "Action";
                }
                else if (colTag == "Status")
                {
                    _columns[colNum].Text = _insideBuildingText;
                }
            }

        }

    }
}
