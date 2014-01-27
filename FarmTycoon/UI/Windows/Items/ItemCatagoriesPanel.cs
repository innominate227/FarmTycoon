using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TycoonGraphicsLib;
using System.Drawing;

namespace FarmTycoon
{

    public partial class ItemCatagoriesPanel : TycoonPanel
    {
        public event Action CatagoryChanged;

        private TycoonButton _currentCatagory = null;
        
        public ItemCatagoriesPanel()
        {
            //intilize
            InitializeComponent();

            //find all catagories to show
            List<string> catagories = new List<string>();
            foreach (ItemTypeInfo itemTypes in FarmData.Current.GetInfos<ItemTypeInfo>())
            {
                foreach (string tag in itemTypes.Tags)
                {
                    if (tag.StartsWith(SpecialTags.STORE_TAG_PREFIX))
                    {
                        string catagory = tag.Substring(SpecialTags.STORE_TAG_PREFIX.Length);
                        if (catagories.Contains(catagory) == false)
                        {
                            catagories.Add(catagory);
                        }
                    }
                }
            }

            //create a button for each catagroy
            int left = 0;
            foreach (string catagory in catagories)
            {
                TycoonButton catagoryButton = new TycoonButton();
                catagoryButton.Text = "";
                catagoryButton.Tag = SpecialTags.STORE_TAG_PREFIX + catagory;
                catagoryButton.Top = 0;
                catagoryButton.Left = left;
                catagoryButton.Width = 30;
                catagoryButton.Height = 30;                
                catagoryButton.AnchorBottom = false;
                catagoryButton.AnchorTop = true;
                catagoryButton.AnchorLeft = true;
                catagoryButton.AnchorRight = false;
                catagoryButton.Visible = true;
                catagoryButton.IconTexture = catagory.ToLower();
                catagoryButton.BackColor = this.BackColor;
                catagoryButton.ShadowLightColor = this.ScrollLightColor;
                catagoryButton.ShadowDarkColor = this.ScrollDarkColor;
                if (_currentCatagory == null)
                {
                    _currentCatagory = catagoryButton;
                    catagoryButton.Depressed = true;
                }
                else
                {
                    catagoryButton.Depressed = false;                    
                }
                catagoryButton.Clicked += new Action<TycoonControl>(CatagoryButton_Clicked);

                this.AddChild(catagoryButton);

                left += 32;
            }
        }

        private void CatagoryButton_Clicked(TycoonControl catagoryButton)
        {
            _currentCatagory.Depressed = false;
            _currentCatagory = (catagoryButton as TycoonButton);
            _currentCatagory.Depressed = true;

            if (CatagoryChanged != null)
            {
                CatagoryChanged();
            }
        }

        public string Catagory
        {
            get { return (string)_currentCatagory.Tag; }
        }


    }
}
