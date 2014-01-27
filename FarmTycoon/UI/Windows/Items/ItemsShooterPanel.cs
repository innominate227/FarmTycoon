using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{

    public partial class ItemsShooterPanel : TycoonPanel
    {
        public event Action SelectedItemsChanged;

        public ItemsShooterPanel()
        {
            //intilize
            InitializeComponent();

            OneLeftButton.Clicked += new Action<TycoonControl>(delegate
            {
                MoveLeft(1);
            });
            TenLeftButton.Clicked += new Action<TycoonControl>(delegate
            {
                MoveLeft(10);
            });
            OneHundredLeftButton.Clicked += new Action<TycoonControl>(delegate
            {
                MoveLeft(100);
            });

            OneRightButton.Clicked += new Action<TycoonControl>(delegate
            {
                MoveRight(1);
            });
            TenRightButton.Clicked += new Action<TycoonControl>(delegate
            {
                MoveRight(10);
            });
            OneHundredRightButton.Clicked += new Action<TycoonControl>(delegate
            {
                MoveRight(100);
            });
        }

        public void Delete()
        {
            LeftItemsPanel.Delete();
            RightItemsPanel.Delete();
        }


        /// <summary>
        /// Setup the shooter panel.  
        /// Pass list of items to select from and the list to fill with selected items.
        /// </summary>
        public void Setup(ItemList selectedFromList, ItemList selectedItems)
        {
            LeftItemsPanel.SetColumns("Price", "Count", "Quality", "Age");
            LeftItemsPanel.AllowSelection = true;
            RightItemsPanel.SetColumns("Price", "Count");
            RightItemsPanel.AllowSelection = true;


            LeftItemsPanel.CountOffsets = new Dictionary<ItemType, int>();
            foreach (ItemType itemType in selectedFromList.ItemTypes)
            {
                LeftItemsPanel.CountOffsets.Add(itemType, 0);
            }
            LeftItemsPanel.ItemList = selectedFromList;


            RightItemsPanel.ItemList = selectedItems;
        }


        /// <summary>
        /// Set the filter to use to decide what items from the ItemList (on the left) to show.
        /// Set to null to show all items
        /// </summary>
        public void SetFilter(ItemFilterDelegate itemFilter)
        {
            LeftItemsPanel.SetFilter(itemFilter);
        }

        /// <summary>
        /// Refresh both item panels
        /// </summary>
        public void Refresh()
        {
            LeftItemsPanel.Refresh();
            RightItemsPanel.Refresh();
        }


        private void MoveRight(int amount)
        {
            //the item selected in the left side
            ItemType selectedInLeft = LeftItemsPanel.SelectedItem;
            if (selectedInLeft == null) { return; }

            //amount of that item on the left side
            int amountInLeft = LeftItemsPanel.ItemList.GetItemCount(selectedInLeft) + LeftItemsPanel.CountOffsets[selectedInLeft];
            if (amountInLeft < amount)
            {
                amount = amountInLeft;
            }

            //increase amount in the right panel
            RightItemsPanel.ItemList.IncreaseItemCount(selectedInLeft, amount);

            //decrease amount in the left panel
            LeftItemsPanel.CountOffsets[selectedInLeft] -= amount;

            //refresh them both
            LeftItemsPanel.Refresh();
            RightItemsPanel.Refresh();

            if (SelectedItemsChanged != null)
            {
                SelectedItemsChanged();
            }
        }

        private void MoveLeft(int amount)
        {
            //the item selected in the right side
            ItemType selectedInRight = RightItemsPanel.SelectedItem;
            if (selectedInRight == null) { return; }

            //amount of that item on the right side
            int amountInRight = RightItemsPanel.ItemList.GetItemCount(selectedInRight);
            if (amountInRight < amount)
            {
                amount = amountInRight;
            }

            //decrease amount in the right panel
            RightItemsPanel.ItemList.DecreaseItemCount(selectedInRight, amount);
            
            //increase amount in the left panel
            LeftItemsPanel.CountOffsets[selectedInRight] += amount;

            //refresh them both
            LeftItemsPanel.Refresh();
            RightItemsPanel.Refresh();

            if (SelectedItemsChanged != null)
            {
                SelectedItemsChanged();
            }
        }

    }
}
