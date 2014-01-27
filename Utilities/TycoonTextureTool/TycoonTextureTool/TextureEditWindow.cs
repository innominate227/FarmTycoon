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
    public partial class TextureEditWindow : Form
    {
        private Texture _texture;


        /// <summary>
        /// Texture the window is editing
        /// </summary>
        public Texture Texture
        {
            get { return _texture; }
        }

        public TextureEditWindow(Texture texture)
        {
            InitializeComponent();
            _texture = texture;
            TextureToWindow();
        }

        private void TextureToWindow()
        {
            if (_texture.TextureSheet != TextureSheet.Game)
            {                
                OffsetXTextbox.Visible = false;
                OffsetYTextbox.Visible = false;
            }


            foreach (string catagory in TextureTool.Instance.TextureCatagories.Keys)
            {
                CatagoryComboBox.Items.Add(catagory);
            }

            NameTextBox.Text = _texture.Name;
            CatagoryComboBox.Text = _texture.Catagory;
            OffsetXTextbox.Value = _texture.CenterOffsetX;
            OffsetYTextbox.Value = _texture.CenterOffsetY;
            RefreshImage();
        }


        private void File_Changed(object sender, FileSystemEventArgs e)
        {
            RefreshImage();
        }



        private void RefreshImage()
        {
            try
            {
                int scale = 2;
                if (ScaleCheckbox.Checked)
                {
                    scale = 4;
                }


                //get the image
                Bitmap imageFile = new Bitmap(_texture.FullFileName);
                Bitmap image = new Bitmap((imageFile.Width + 20) * scale, (imageFile.Height + 20) * scale);

                PictureBox.Width = image.Width;
                PictureBox.Height = image.Height;


                Graphics drawer = Graphics.FromImage(image);

                drawer.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                drawer.ScaleTransform(scale, scale);
                drawer.TranslateTransform(10f, 10f);

                drawer.DrawImage(imageFile, 0, 0);

                //draw on the offset pixel  
                if (_texture.TextureSheet == TextureSheet.Game)
                {
                    int centerX = _texture.CenterOffsetX;
                    int centerY = imageFile.Height - _texture.CenterOffsetY;
                    drawer.FillRectangle(new SolidBrush(Color.HotPink), new Rectangle(centerX - 1, centerY - 1, 3, 3));
                    
                    drawer.Dispose();                    
                }

                PictureBox.Image = image;
                imageFile.Dispose();
            }
            catch { }
        }
        
        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            //remove from where it used to be in the texture collection
            TextureTool.Instance.RemoveTexture(_texture);

            //rember old name incase we have to revert
            string oldName = _texture.Name;
            string oldFileName = _texture.FullFileName;

            //update property
            _texture.Name = NameTextBox.Text;

            //try to place the texture into the structure
            try
            {
                TextureTool.Instance.AddTexture(_texture);
                ErrorProvider.SetError(NameTextBox, "");
            }
            catch
            {
                //we failed revet to old name show that current name is bad
                _texture.Name = oldName;
                TextureTool.Instance.AddTexture(_texture);
                ErrorProvider.SetError(NameTextBox, "Duplicate Name");
            }

            //rename the file to match
            File.Move(oldFileName, _texture.FullFileName);

        }

        private void CatagoryComboBox_TextChanged(object sender, EventArgs e)
        {
            //remove from where it used to be in the texture collection
            TextureTool.Instance.RemoveTexture(_texture);
            _texture.Catagory = CatagoryComboBox.Text;
            TextureTool.Instance.AddTexture(_texture);
        }

        private void OffsetXTextbox_ValueChanged(object sender, EventArgs e)
        {
            _texture.CenterOffsetX = (int)OffsetXTextbox.Value;
            RefreshImage();
        }

        private void OffsetYTextbox_ValueChanged(object sender, EventArgs e)
        {
            _texture.CenterOffsetY = (int)OffsetYTextbox.Value;
            RefreshImage();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshImage();
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {            
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.FileName = ("mspaint.exe");
            info.Arguments = "\"" + _texture.FullFileName + "\"";
            System.Diagnostics.Process.Start(info);
        }

        private void TextureEditWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            RefreshImage();
        }


        private void CopyButton_Click(object sender, EventArgs e)
        {
            //create a copy of the texture, and show it
            Texture textureCopy = _texture.Clone();
            WindowManager.Instance.ShowTextureWindow(textureCopy);
        }


        private void DeleteButton_Click(object sender, EventArgs e)
        {
            //make sure no quartets reference the texture
            foreach (Quartet quartet in TextureTool.Instance.Quartets.Values)
            {
                if (quartet.North == _texture || quartet.West == _texture || quartet.East == _texture || quartet.South == _texture)
                {
                    MessageBox.Show("Can not Delete. Texture is reference by quartet: " + quartet.Name, "Can not Delete");
                    return;
                }
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Delete", MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                //remove texture
                TextureTool.Instance.RemoveTexture(_texture);

                //delete the texture file
                File.Delete(_texture.FullFileName);
                this.Close();
            }
        }



        private void LinesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            RefreshImage();
        }
        private void BackPointsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            RefreshImage();
        }
        private void ScaleCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            RefreshImage();
        }


    }
}
