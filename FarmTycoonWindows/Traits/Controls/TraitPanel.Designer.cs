namespace FarmTycoonWindowGen
{
    partial class TraitPanel
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
            this.traitName = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.traitGauge = new TycoonWindowGenerationLib.TycoonGauge_Gen();
            this.SuspendLayout();
            // 
            // traitName
            // 
            this.traitName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.traitName.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.traitName.ForeColor = System.Drawing.Color.White;
            this.traitName.Location = new System.Drawing.Point(2, 5);
            this.traitName.Name = "traitName";
            this.traitName.Size = new System.Drawing.Size(67, 15);
            this.traitName.TabIndex = 53;
            this.traitName.Text = "Trait Name:";
            this.traitName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.traitName.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.traitName.Tycoon_Tooltip = "";
            this.traitName.Tycoon_TooltipTime = 1D;
            this.traitName.Tycoon_Visible = true;
            // 
            // traitGauge
            // 
            this.traitGauge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.traitGauge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.traitGauge.Font = new System.Drawing.Font("Segoe UI", 6F);
            this.traitGauge.ForeColor = System.Drawing.Color.Black;
            this.traitGauge.Location = new System.Drawing.Point(75, 3);
            this.traitGauge.Name = "traitGauge";
            this.traitGauge.Size = new System.Drawing.Size(215, 17);
            this.traitGauge.TabIndex = 0;
            this.traitGauge.Text = "100";
            this.traitGauge.Tycoon_BadColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(86)))), ((int)(((byte)(91)))));
            this.traitGauge.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.traitGauge.Tycoon_GoodColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(230)))), ((int)(((byte)(29)))));
            this.traitGauge.Tycoon_GoodColorEnd = 60;
            this.traitGauge.Tycoon_GoodColorStart = 40;
            this.traitGauge.Tycoon_MaxValue = 100;
            this.traitGauge.Tycoon_MidColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(242)))), ((int)(((byte)(0)))));
            this.traitGauge.Tycoon_MidColorEnd = 75;
            this.traitGauge.Tycoon_MidColorStart = 25;
            this.traitGauge.Tycoon_MinValue = 0;
            this.traitGauge.Tycoon_Quality = 50;
            this.traitGauge.Tycoon_Tooltip = "";
            this.traitGauge.Tycoon_TooltipTime = 1D;
            this.traitGauge.Tycoon_Value = 50;
            this.traitGauge.Tycoon_Visible = true;
            // 
            // TraitPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.traitGauge);
            this.Controls.Add(this.traitName);
            this.Name = "TraitPanel";
            this.Size = new System.Drawing.Size(295, 27);
            this.Tycoon_Border = false;
            this.Load += new System.EventHandler(this.TraitPanelPanel_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonLabel_Gen traitName;
        private TycoonWindowGenerationLib.TycoonGauge_Gen traitGauge;

    }
}
