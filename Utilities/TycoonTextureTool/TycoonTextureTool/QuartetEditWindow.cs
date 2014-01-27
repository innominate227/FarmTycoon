using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TycoonTextureTool
{
    public partial class QuartetEditWindow : Form
    {
        private Quartet _quartet;
        

        /// <summary>
        /// Quartet the window is editing
        /// </summary>
        public Quartet Quartet
        {
            get { return _quartet; }
        }

        public QuartetEditWindow(Quartet quartet)
        {
            InitializeComponent();
            _quartet = quartet;
            QuartetToWindow();
        }

        private void QuartetToWindow()
        {
            foreach (string catagory in TextureTool.Instance.QuartetCatagories.Keys)
            {
                CatagoryComboBox.Items.Add(catagory);
            }

            foreach (Texture texture in TextureTool.Instance.Textures.Values)
            {
                if (texture.TextureSheet != TextureSheet.Game) { continue; }

                NorthComboBox.Items.Add(texture.Name);
                SouthComboBox.Items.Add(texture.Name);
                EastComboBox.Items.Add(texture.Name);
                WestComboBox.Items.Add(texture.Name);
            }
            
            NameTextBox.Text = _quartet.Name;
            CatagoryComboBox.Text = _quartet.Catagory;
            NorthComboBox.Text = _quartet.North.Name;
            SouthComboBox.Text = _quartet.South.Name;
            EastComboBox.Text = _quartet.East.Name;
            WestComboBox.Text = _quartet.West.Name;
            RefreshImages();
        }
        

        private void File_Changed(object sender, FileSystemEventArgs e)
        {
            RefreshImages();
        }


        /// <summary>
        /// Load image into a picbox without locking the file
        /// </summary>
        private void LoadImage(string file, PictureBox picBox)
        {
            try
            {
                //get the image
                Bitmap imageFile = new Bitmap(file);
                Bitmap image = new Bitmap(imageFile.Width, imageFile.Height);

                //draw on the offset pixel            
                Graphics drawer = Graphics.FromImage(image);
                drawer.DrawImage(imageFile, 0, 0);
                drawer.Dispose();

                picBox.Image = image;
                imageFile.Dispose();
            }
            catch { }
        }         
        
        private void RefreshImages()
        {
            LoadImage(_quartet.North.FullFileName, NorthPictureBox);
            LoadImage(_quartet.South.FullFileName, SouthPictureBox);
            LoadImage(_quartet.East.FullFileName, EastPictureBox);
            LoadImage(_quartet.West.FullFileName, WestPictureBox);                
        }
        

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            //remove from where it used to be in the texture collection
            TextureTool.Instance.RemoveQuartet(_quartet);

            //rember old name incase we have to revert
            string oldName = _quartet.Name;            

            //update property
            _quartet.Name = NameTextBox.Text;

            //try to place the texture into the structure
            try
            {
                TextureTool.Instance.AddQuartet(_quartet);
                ErrorProvider.SetError(NameTextBox, "");
            }
            catch
            {
                //we failed revet to old name show that current name is bad
                _quartet.Name = oldName;
                TextureTool.Instance.AddQuartet(_quartet);
                ErrorProvider.SetError(NameTextBox, "Duplicate Name");
            }            
        }

        private void CatagoryComboBox_TextChanged(object sender, EventArgs e)
        {
            //remove from where it used to be in the texture collection
            TextureTool.Instance.RemoveQuartet(_quartet);
            _quartet.Catagory = CatagoryComboBox.Text;
            TextureTool.Instance.AddQuartet(_quartet);
        }


        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshImages();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RefreshImages();
        }





        private void EastComboBox_TextChanged(object sender, EventArgs e)
        {
            string textureFullName = TextureSheet.Game.ToString() + "_" + EastComboBox.Text;
            if (TextureTool.Instance.Textures.ContainsKey(textureFullName))
            {
                _quartet.East = TextureTool.Instance.Textures[textureFullName];
            }
        }

        private void NorthComboBox_TextChanged(object sender, EventArgs e)
        {
            string textureFullName = TextureSheet.Game.ToString() + "_" + NorthComboBox.Text;
            if (TextureTool.Instance.Textures.ContainsKey(textureFullName))
            {
                _quartet.North = TextureTool.Instance.Textures[textureFullName];
            }
        }

        private void WestComboBox_TextChanged(object sender, EventArgs e)
        {
            string textureFullName = TextureSheet.Game.ToString() + "_" + WestComboBox.Text;
            if (TextureTool.Instance.Textures.ContainsKey(textureFullName))
            {
                _quartet.West = TextureTool.Instance.Textures[textureFullName];
            }
        }

        private void SouthComboBox_TextChanged(object sender, EventArgs e)
        {
            string textureFullName = TextureSheet.Game.ToString() + "_" + SouthComboBox.Text;
            if (TextureTool.Instance.Textures.ContainsKey(textureFullName))
            {
                _quartet.South = TextureTool.Instance.Textures[textureFullName];
            }
        }








        private void NorthPictureBox_Click(object sender, EventArgs e)
        {
            WindowManager.Instance.ShowTextureWindow(_quartet.North);
        }

        private void EastPictureBox_Click(object sender, EventArgs e)
        {
            WindowManager.Instance.ShowTextureWindow(_quartet.East);
        }

        private void WestPictureBox_Click(object sender, EventArgs e)
        {
            WindowManager.Instance.ShowTextureWindow(_quartet.West);
        }

        private void SouthPictureBox_Click(object sender, EventArgs e)
        {
            WindowManager.Instance.ShowTextureWindow(_quartet.South);
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            Quartet copy = _quartet.Clone();
            WindowManager.Instance.ShowQuartetWindow(copy);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                //remove the quartet
                TextureTool.Instance.RemoveQuartet(_quartet);
                this.Close();
            }
        }

        private void CopyButton2_Click(object sender, EventArgs e)
        {
            Quartet copy = _quartet.CloneKeepTextures();
            WindowManager.Instance.ShowQuartetWindow(copy);
        }



        

    }
}
