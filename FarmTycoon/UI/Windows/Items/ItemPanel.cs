using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    
    public partial class ItemPanel : TycoonPanel
    {
        public event Action<ItemPanel> Selected;

        /// <summary>
        /// Item type being shown
        /// </summary>
        private ItemType _itemType;
        
        /// <summary>
        /// Is selection allowed for the item
        /// </summary>
        private bool _allowSelection;

        /// <summary>
        /// The count that the panel should show
        /// </summary>
        private int _count;

        /// <summary>
        /// Inventory being shown, or null if not showing an inventory
        /// </summary>
        private Inventory _inventory;
        
        /// <summary>
        /// Labels for each of the fours columns
        /// </summary>
        private Dictionary<int, TycoonLabel> _columns = new Dictionary<int, TycoonLabel>();
        
        public ItemPanel()
        {
            //intilize
            InitializeComponent();

            _columns.Add(0, Info1Label);
            _columns.Add(1, Info2Label);
            _columns.Add(2, Info3Label);
            _columns.Add(3, Info4Label);

            for (int colNum = 0; colNum < 4; colNum++)
            {
                _columns[colNum].DrawNumericValue = true;
                _columns[colNum].DrawDollarSign = false;                                
            }

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

                    if (columnNames[colNum] == "Price")
                    {
                        _columns[colNum].DrawDollarSign = true;
                    }
                    else if (columnNames[colNum] == "Quality")
                    {
                        _columns[colNum].DrawAsStars = true;
                    }
                }
            }
        }
        
        /// <summary>
        /// Item type being shown
        /// </summary>
        public ItemType ItemType
        {
            get { return _itemType; }
            set 
            {
                _itemType = value;
                ItemImage.IconTexture = _itemType.Icon;
                NameLabel.Text = _itemType.BaseName;             
            }
        }

        /// <summary>
        /// Count to show with the item
        /// </summary>
        public int Count
        {
            set 
            {
                _count = value;
            }
            get { return _count; }
        }

        /// <summary>
        /// Inventory being shown, or null if not showing an inventory
        /// </summary>
        public Inventory Inventory
        {
            set 
            {
                _inventory = value;                             
            }
            get { return _inventory; }
        }


        /// <summary>
        /// Is the item selectable
        /// </summary>
        public bool IsSelectable
        {
            get { return _allowSelection; }
            set { _allowSelection = value;  }
        }

        /// <summary>
        /// Is the item selected
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
        /// Refresh the columns for the item
        /// </summary>
        public void Refresh()
        {
            for (int colNum = 0; colNum < 4; colNum++)
            {
                if (_columns[colNum].Tag == null) { continue; }
                string colTag = _columns[colNum].Tag.ToString();

                if (colTag == "Count")
                {
                    _columns[colNum].NumericValue = _count;
                }
                else if (colTag == "Price")
                {
                    _columns[colNum].NumericValue = GameState.Current.Prices.GetPrice(_itemType);
                }
                else if (colTag == "Quality")
                {
                    _columns[colNum].NumericValue = _itemType.Quality;
                }
                else if (colTag == "Age")
                {
                    double itemAge = _itemType.Age / 360.0;
                    int years = (int)(_itemType.Age / 360.0);
                    int fracYears = (int)(_itemType.Age / 36.0) % 10;

                    _columns[colNum].NumericValue = years;
                    _columns[colNum].DecimalValue = fracYears;
                }
                else if (colTag == "Reserved")
                {
                    if (_inventory != null)
                    {
                        _columns[colNum].NumericValue = _inventory.GetTypeCount(_itemType) - _inventory.GetTypeCountThatsFree(_itemType);
                    }
                }
                else if (colTag == "Size")
                {
                    _columns[colNum].NumericValue = _itemType.Size;
                }
            }

        }

    }
}
