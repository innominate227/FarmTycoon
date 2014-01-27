namespace FarmTycoonWindowGen
{
    partial class BuildingDropbox
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.takeToDropbox = new TycoonWindowGenerationLib.TycoonDropbox_Gen();
            this.SuspendLayout();
            // 
            // takeToDropbox
            // 
            this.takeToDropbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.takeToDropbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.takeToDropbox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.takeToDropbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.takeToDropbox.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.takeToDropbox.ForeColor = System.Drawing.Color.White;
            this.takeToDropbox.FormattingEnabled = true;
            this.takeToDropbox.Items.AddRange(new object[] {
            "Nearest"});
            this.takeToDropbox.Location = new System.Drawing.Point(0, 0);
            this.takeToDropbox.Name = "takeToDropbox";
            this.takeToDropbox.Size = new System.Drawing.Size(150, 15);
            this.takeToDropbox.TabIndex = 55;
            this.takeToDropbox.Text = "Nearest";
            this.takeToDropbox.Tycoon_DropArrowTexture = "arrowdown";
            this.takeToDropbox.Tycoon_DropHeight = 50;
            this.takeToDropbox.Tycoon_DropTextColor = System.Drawing.Color.White;
            this.takeToDropbox.Tycoon_SelectionColor = System.Drawing.Color.Blue;
            this.takeToDropbox.Tycoon_ShadowDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.takeToDropbox.Tycoon_ShadowLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.takeToDropbox.Tycoon_TextAlignment = System.Drawing.StringAlignment.Near;
            this.takeToDropbox.Tycoon_TextVerticelAlignment = System.Drawing.StringAlignment.Near;
            this.takeToDropbox.Tycoon_Tooltip = "";
            this.takeToDropbox.Tycoon_TooltipTime = 1D;
            this.takeToDropbox.Tycoon_Visible = true;
            // 
            // BuildingDropbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Controls.Add(this.takeToDropbox);
            this.Name = "BuildingDropbox";
            this.Size = new System.Drawing.Size(150, 15);
            this.Tycoon_Border = false;
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonDropbox_Gen takeToDropbox;


    }
}
