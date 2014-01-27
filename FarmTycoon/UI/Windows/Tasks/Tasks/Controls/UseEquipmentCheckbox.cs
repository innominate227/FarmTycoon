using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{

    public partial class UseEquipmentCheckbox : TycoonPanel
    {
        public event Action CheckChanged;

        public UseEquipmentCheckbox()
        {
            //intilize
            InitializeComponent();

            useEquipmentButton.Clicked += new Action<TycoonControl>(useEquipmentButton_Clicked);
        }

        private void useEquipmentButton_Clicked(TycoonControl obj)
        {
            if (CheckChanged != null)
            {
                CheckChanged();
            }
        }

        public void Hide()
        {
            this.Visible = false;

            foreach (TycoonControl con in this.Parent.Children)
            {
                if (con.Top >= this.Top && con != this)
                {
                    con.Top -= 20;
                }
            }
            this.Parent.Height -= 20;

            useEquipmentButton.Depressed = false;   

        }

        public void Setup(EquipmentType typeNeeded1, EquipmentType typeNeeded2)
        {            
            bool hasType1 = false;
            bool hasType2 = false;
            
            foreach (ItemType itemType in GameState.Current.PlayersItemsList.ItemTypes)
            {                
                EquipmentInfo equipmentInfo = FarmData.Current.GetEquipmentInfoForItemInfo(itemType.BaseType);
                if (equipmentInfo != null)
                {
                    if (equipmentInfo.EquipmentType == typeNeeded1)
                    {
                        hasType1 = true;
                    }
                    else if (equipmentInfo.EquipmentType == typeNeeded2)
                    {
                        hasType2 = true;
                    }
                }
            }


            if (hasType1 == false || hasType2 == false)
            {
                Hide();   
            }            
        }


        public bool Checked
        {
            get { return useEquipmentButton.Depressed; }
            set { useEquipmentButton.Depressed = value; }
        }



    }
}
