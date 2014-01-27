using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{
    public delegate bool ItemFilterDelegate(ItemType item);

    /// <summary>
    /// Item list panel
    /// </summary>
    public partial class ItemsPanel : TycoonPanel
    {
        /// <summary>
        /// Item selected in the panel has changed
        /// </summary>
        public event Action SelectedItemChanged;

        /// <summary>
        /// ItemList being shown in the panel
        /// </summary>
        private ItemList _itemList;

        /// <summary>
        /// Item in the ItemList that is selected
        /// </summary>
        private ItemType _selectedItem;

        /// <summary>
        /// Index of the currently selected item
        /// </summary>
        private int _selectedItemIndex = 0;
                
        /// <summary>
        /// Label for each of the four columns that should be shown
        /// </summary>
        private string[] _columns = new string[0];
                        
        /// <summary>
        /// The list allows items to be selected
        /// </summary>
        private bool _allowSelection = true;

        /// <summary>
        /// Filter used to determine what items in the list should actually be shown
        /// </summary>
        private ItemFilterDelegate _itemFilter = null;

        /// <summary>
        /// Dictionary with count offsets, if null use normal items counts
        /// </summary>
        private Dictionary<ItemType, int> _countOffsets = null;

        /// <summary>
        /// Inventory being shown, or null if showing a list and not an inventory
        /// </summary>
        private Inventory _inventory = null;
        
        
        /// <summary>
        /// Dictionary that points to a control for each item
        /// </summary>
        private Dictionary<ItemType, ItemPanel> _itemControls = new Dictionary<ItemType, ItemPanel>();



        public ItemsPanel()
        {
            //intilize
            InitializeComponent();

            GameState.Current.Prices.PriceChanged +=new Action(Refresh);
            Program.GameThread.RefreshTimePassed += new Action(Refresh);
        }

        public void Delete()
        {
            GameState.Current.Prices.PriceChanged -= new Action(Refresh);
            Program.GameThread.RefreshTimePassed -= new Action(Refresh);

            if (_inventory != null)
            {                
                _inventory.ReservedItemsChanged -= new Action(Refresh);
            }
            if (_itemList != null)
            {
                _itemList.ListChanged -= new Action(Refresh);
            }
        }

        /// <summary>
        /// Set the columns for the item list
        /// </summary>
        public void SetColumns(params string[] columnNames)
        {
            _columns = columnNames;
            if (_columns.Length >= 1)
            {
                Info1Label.Text = columnNames[0];
            }
            if (_columns.Length >= 2)
            {
                Info2Label.Text = columnNames[1];
            }
            if (_columns.Length >= 3)
            {
                Info3Label.Text = columnNames[2];
            }
            if (_columns.Length >= 4)
            {
                Info4Label.Text = columnNames[3];
            }
        }

        /// <summary>
        /// Inventory being shown, or null if showing a list and not an inventory
        /// </summary>
        public Inventory Inventory
        {
            get { return _inventory; }
            set 
            {
                if (_inventory != null)
                {
                    ItemList = null;
                    _inventory.ReservedItemsChanged -= new Action(Refresh);
                }

                _inventory = value;

                if (_inventory != null)
                {
                    ItemList = _inventory.UnderlyingList;
                    _inventory.ReservedItemsChanged += new Action(Refresh);
                }
            }
        }

        /// <summary>
        /// ItemList being shown in the panel
        /// </summary>
        public ItemList ItemList
        {
            get { return _itemList; }
            set 
            {
                if (_itemList != null)
                {
                    _itemList.ListChanged -=new Action(Refresh);
                }
                _itemList = value;
                if (_itemList != null)
                {
                    _itemList.ListChanged += new Action(Refresh);
                }
                Refresh();
            }
        }
        
        /// <summary>
        /// The item that is selected
        /// </summary>
        public ItemType SelectedItem
        {
            get 
            {                
                return _selectedItem; 
            }
            set
            {
                _selectedItem = value;
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
        public void SetFilter(ItemFilterDelegate itemFilter)
        {
            _itemFilter = itemFilter;
            Refresh();
        }

        /// <summary>
        /// Dictionary with count offsets, if null use normal items counts
        /// </summary>
        public Dictionary<ItemType, int> CountOffsets
        {
            get { return _countOffsets; }
            set { _countOffsets = value; }
        }
        
        /// <summary>
        /// Refresh the items list
        /// </summary>
        public void Refresh()
        {
            //dont do anything if there is no item list
            if (_itemList == null) { return; }

            //get a list of items to show
            //also search to make sure the currently selected item is still being shown
            List<ItemType> itemsToShow = new List<ItemType>();
            bool foundSelectedItem = false;
            foreach (ItemType itemType in _itemList.ItemTypes)
            {
                if (_itemFilter != null && _itemFilter(itemType) == false)
                {
                    //if filtering dont show items that dont pass the filter
                    continue;
                }
                if (_countOffsets != null && _itemList.GetItemCount(itemType) + _countOffsets[itemType] <= 0)
                {
                    //if overriding counts dont show items with a count of 0
                    continue;
                }

                itemsToShow.Add(itemType);
                if (itemType == _selectedItem)
                {
                    foundSelectedItem = true;
                }
            }
            
            //the selected item is not being shown any more choose a new item to select
            if (foundSelectedItem == false && _allowSelection)
            {
                _selectedItem = null;
                if (itemsToShow.Count > 0)
                {
                    if (itemsToShow.Count <= _selectedItemIndex)
                    {
                        _selectedItemIndex = itemsToShow.Count - 1;
                    }
                    _selectedItem = itemsToShow[_selectedItemIndex];                    
                }
            }
            
            //hide controls for all items that are no longer showing
            foreach (ItemType itemType in _itemControls.Keys)
            {
                if (itemsToShow.Contains(itemType) == false)
                {
                    TycoonControl control = _itemControls[itemType];
                    control.Visible = false;
                }
            }
                        
            //create or update panels for items that are showing
            int itemNum = 0;
            foreach (ItemType itemType in itemsToShow)
            {
                //count of the item
                int itemCount = _itemList.GetItemCount(itemType);
                if (_countOffsets != null)
                {
                    itemCount += _countOffsets[itemType];
                }

                //if this the selected item
                bool isSelected = (itemType == _selectedItem);
                if (isSelected)
                {
                    _selectedItemIndex = itemNum;
                }
                                
                //create a new control for new items
                if (_itemControls.ContainsKey(itemType) == false)
                {
                    ItemPanel newControl = new ItemPanel();
                    newControl.SetColumns(_columns);                    
                    newControl.Left = 0;                                        
                    newControl.AnchorBottom = false;
                    newControl.AnchorTop = true;
                    newControl.AnchorLeft = true;
                    newControl.AnchorRight = false;
                    newControl.ItemType = itemType;                    
                    newControl.IsSelectable = _allowSelection;
                    newControl.BorderColor = newControl.BackColor;
                    newControl.Inventory = _inventory;
                    
                    //if we are allowing selection handel Selected event
                    if (_allowSelection)
                    {
                        newControl.Selected += new Action<ItemPanel>(ItemsPanel_Selected);
                    }

                    //add to controls dictionary
                    _itemControls.Add(itemType, newControl);

                    //add control to panel
                    ItemPanelInner.AddChild(newControl);
                }
                
                //update the new/old control
                ItemPanel control = _itemControls[itemType];
                control.Tag = itemNum;
                control.Top = itemNum * control.Height;                
                control.Count = itemCount;
                control.IsSelected = isSelected;
                control.Visible = true;
                control.Refresh();                

                itemNum++;
            }
        }
        
        private void ItemsPanel_Selected(ItemPanel selected)
        {
            //do nothing if already selected
            if (selected.ItemType == _selectedItem) { return; }

            //unselect old and select new
            _itemControls[_selectedItem].IsSelected = false;
            _itemControls[selected.ItemType].IsSelected = true;

            //update currently selected item, and panel
            _selectedItem = selected.ItemType;
            _selectedItemIndex = (int)_itemControls[selected.ItemType].Tag;

            //selected item has changed
            if (SelectedItemChanged != null)
            {
                SelectedItemChanged();
            }
        }

    }
}
