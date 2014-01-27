using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TycoonTextureTool
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            RefreshWindowTextureCatagories();
            RefreshTextureCatagories();
            RefreshQuartetsCatagories();
            RefreshWindowTexturesList();
            RefreshTexturesList();
            RefreshQuartetsList();
        }



        private void RefreshWindowTextureCatagories()
        {
            string selectedNow = "[All]";
            if (FilterWindowTexturesComboBox.SelectedItem != null)
            {
                selectedNow = FilterWindowTexturesComboBox.SelectedItem.ToString();
            }

            FilterWindowTexturesComboBox.Items.Clear();
            FilterWindowTexturesComboBox.Items.Add("[All]");
            foreach (string catagory in TextureTool.Instance.TextureCatagories.Keys)
            {
                FilterWindowTexturesComboBox.Items.Add(catagory);
            }

            FilterWindowTexturesComboBox.SelectedItem = selectedNow;
            if (FilterWindowTexturesComboBox.SelectedItem == null)
            {
                FilterWindowTexturesComboBox.SelectedItem = "[All]";
            }
        }

        private void RefreshTextureCatagories()
        {
            string selectedNow = "[All]";
            if (FilterTexturesComboBox.SelectedItem != null)
            {
                selectedNow = FilterTexturesComboBox.SelectedItem.ToString();
            }

            FilterTexturesComboBox.Items.Clear();
            FilterTexturesComboBox.Items.Add("[All]");
            foreach (string catagory in TextureTool.Instance.TextureCatagories.Keys)
            {
                FilterTexturesComboBox.Items.Add(catagory);
            }

            FilterTexturesComboBox.SelectedItem = selectedNow;
            if (FilterTexturesComboBox.SelectedItem == null)
            {
                FilterTexturesComboBox.SelectedItem = "[All]";
            }
        }

        private void RefreshQuartetsCatagories()
        {
            string selectedNow = "[All]";
            if (FilterQuartetsComboBox.SelectedItem != null)
            {
                selectedNow = FilterQuartetsComboBox.SelectedItem.ToString();
            }

            FilterQuartetsComboBox.Items.Clear();
            FilterQuartetsComboBox.Items.Add("[All]");
            foreach (string catagory in TextureTool.Instance.QuartetCatagories.Keys)
            {
                FilterQuartetsComboBox.Items.Add(catagory);
            }

            FilterQuartetsComboBox.SelectedItem = selectedNow;
            if (FilterQuartetsComboBox.SelectedItem == null)
            {
                FilterQuartetsComboBox.SelectedItem = "[All]";
            }
        }

        private void RefreshWindowTexturesList()
        {
            WindowTexturesListView.Items.Clear();

            foreach (string textureName in TextureTool.Instance.Textures.Keys)
            {
                Texture texture = TextureTool.Instance.Textures[textureName];
                if (texture.TextureSheet != TextureSheet.Window) { continue; }
                if (textureName.Contains(SearchTexturesTextbox.Text))
                {                    
                    if (texture.Catagory == FilterWindowTexturesComboBox.Text || FilterWindowTexturesComboBox.Text == "[All]")
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = texture.Name;
                        item.SubItems.Add(texture.Catagory);
                        item.SubItems.Add(texture.Width.ToString() + "x" + texture.Height.ToString());
                        item.Tag = texture;
                        WindowTexturesListView.Items.Add(item);
                    }
                }
            }
        }
        
        private void RefreshTexturesList()
        {
            TexturesListView.Items.Clear();

            foreach (string textureName in TextureTool.Instance.Textures.Keys)
            {
                Texture texture = TextureTool.Instance.Textures[textureName];
                if (texture.TextureSheet != TextureSheet.Game) { continue; }
                if (textureName.Contains(SearchTexturesTextbox.Text))
                {
                    if (texture.Catagory == FilterTexturesComboBox.Text || FilterTexturesComboBox.Text == "[All]")
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = texture.Name;
                        item.SubItems.Add(texture.Catagory);
                        item.SubItems.Add(texture.Width.ToString() + "x" + texture.Height.ToString());
                        item.Tag = texture;
                        TexturesListView.Items.Add(item);
                    }
                }
            }
        }
        
        private void RefreshQuartetsList()
        {
            QuartetListView.Items.Clear();

            foreach (string quartetName in TextureTool.Instance.Quartets.Keys)
            {
                if (quartetName.Contains(SearchQuartetsTextbox.Text))
                {
                    Quartet quartet = TextureTool.Instance.Quartets[quartetName];
                    if (quartet.Catagory == FilterQuartetsComboBox.Text || FilterQuartetsComboBox.Text == "[All]")
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = quartet.Name;
                        item.SubItems.Add(quartet.Catagory);
                        item.SubItems.Add(quartet.North.Name);
                        item.SubItems.Add(quartet.East.Name);
                        item.SubItems.Add(quartet.South.Name);
                        item.SubItems.Add(quartet.West.Name);
                        item.Tag = quartet;
                        QuartetListView.Items.Add(item);
                    }
                }
            }
        }



        private void SearchWindowTexturesTextbox_TextChanged(object sender, EventArgs e)
        {
            RefreshWindowTexturesList();
        }

        private void FilterWindowTexturesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshWindowTexturesList();
        }

        private void RefreshWindowTexturesButton_Click(object sender, EventArgs e)
        {
            RefreshWindowTexturesList();
            RefreshWindowTextureCatagories();
        }



        private void SearchTexturesTextbox_TextChanged(object sender, EventArgs e)
        {
            RefreshTexturesList();
        }

        private void FilterTexturesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshTexturesList();
        }

        private void RefreshTexturesButton_Click(object sender, EventArgs e)
        {
            RefreshTexturesList();
            RefreshTextureCatagories();
        }



        private void SearchQuartetsTextbox_TextChanged(object sender, EventArgs e)
        {
            RefreshQuartetsList();
        }

        private void FilterQuartetsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshQuartetsList();
        }

        private void RefreshQuartetButton_Click(object sender, EventArgs e)
        {
            RefreshQuartetsList();
            RefreshQuartetsCatagories();
        }

        

        private void WindowTexturesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WindowTexturesListView.SelectedItems.Count > 0)
            {
                Texture texture = (Texture)WindowTexturesListView.SelectedItems[0].Tag;
                WindowManager.Instance.ShowTextureWindow(texture);
            }
        }
        
        private void TexturesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TexturesListView.SelectedItems.Count > 0)
            {
                Texture texture = (Texture)TexturesListView.SelectedItems[0].Tag;
                WindowManager.Instance.ShowTextureWindow(texture);
            }
        }

        private void QuartetListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (QuartetListView.SelectedItems.Count > 0)
            {
                Quartet quartet = (Quartet)QuartetListView.SelectedItems[0].Tag;
                WindowManager.Instance.ShowQuartetWindow(quartet);
            }
        }






        private void ExportButton_Click(object sender, EventArgs e)
        {
            //Closing event does the save
            this.Close();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                WindowManager.Instance.CloseAllWindows();

                FileWriter writer = new FileWriter();

                writer.CreateTextureMap(delegate(int progress)
                {
                    ExportProgress.Value = progress;
                }, SaveQuickCheckBox.Checked, TextureSheet.Game, "texturemap.bmp");

                writer.CreateTextureMap(delegate(int progress)
                {
                    ExportProgress.Value = progress;
                }, SaveQuickCheckBox.Checked, TextureSheet.Window, "wintexturemap.bmp");

                writer.WriteWindowsTexturesFile();
                writer.WriteTexturesFile();
                writer.WriteQuartetsFile();

            }
            catch (Exception ex)
            {                
                MessageBox.Show(ex.ToString());
                e.Cancel = true;
            }
        }

        

    }
}
