namespace FarmTycoonWindowGen
{
    partial class SubQualityPanel
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
            this.itemButton = new TycoonWindowGenerationLib.TycoonButton_Gen();
            this.qualityGauge = new TycoonWindowGenerationLib.TycoonGauge_Gen();
            this.SuspendLayout();
            // 
            // itemButton
            // 
            this.itemButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.itemButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.itemButton.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.itemButton.ForeColor = System.Drawing.Color.White;
            this.itemButton.Location = new System.Drawing.Point(6, 3);
            this.itemButton.Name = "itemButton";
            this.itemButton.Size = new System.Drawing.Size(62, 17);
            this.itemButton.TabIndex = 69;
            this.itemButton.Text = "ITEM";
            this.itemButton.Tycoon_Depressed = false;
            this.itemButton.Tycoon_IconTexture = "";
            this.itemButton.Tycoon_ShadowDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.itemButton.Tycoon_ShadowLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.itemButton.Tycoon_Tooltip = "";
            this.itemButton.Tycoon_TooltipTime = 1D;
            this.itemButton.Tycoon_Visible = true;
            this.itemButton.UseVisualStyleBackColor = false;
            this.itemButton.Click += new System.EventHandler(this.detailsButton_Click);
            // 
            // qualityGauge
            // 
            this.qualityGauge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.qualityGauge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.qualityGauge.Font = new System.Drawing.Font("Segoe UI", 6F);
            this.qualityGauge.ForeColor = System.Drawing.Color.Black;
            this.qualityGauge.Location = new System.Drawing.Point(75, 3);
            this.qualityGauge.Name = "qualityGauge";
            this.qualityGauge.Size = new System.Drawing.Size(215, 17);
            this.qualityGauge.TabIndex = 75;
            this.qualityGauge.Text = "100";
            this.qualityGauge.Tycoon_BadColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(86)))), ((int)(((byte)(91)))));
            this.qualityGauge.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.qualityGauge.Tycoon_GoodColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(230)))), ((int)(((byte)(29)))));
            this.qualityGauge.Tycoon_GoodColorEnd = 100;
            this.qualityGauge.Tycoon_GoodColorStart = 66;
            this.qualityGauge.Tycoon_MaxValue = 100;
            this.qualityGauge.Tycoon_MidColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(242)))), ((int)(((byte)(0)))));
            this.qualityGauge.Tycoon_MidColorEnd = 66;
            this.qualityGauge.Tycoon_MidColorStart = 33;
            this.qualityGauge.Tycoon_MinValue = 0;
            this.qualityGauge.Tycoon_Quality = 50;
            this.qualityGauge.Tycoon_Tooltip = "";
            this.qualityGauge.Tycoon_TooltipTime = 1D;
            this.qualityGauge.Tycoon_Value = 50;
            this.qualityGauge.Tycoon_Visible = true;
            // 
            // SubQualityPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.qualityGauge);
            this.Controls.Add(this.itemButton);
            this.Name = "SubQualityPanel";
            this.Size = new System.Drawing.Size(295, 27);
            this.Tycoon_Border = false;
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonButton_Gen itemButton;
        private TycoonWindowGenerationLib.TycoonGauge_Gen qualityGauge;

    }
}
