namespace FarmTycoonWindowGen.Buildings
{
    partial class SingleQualityWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.hline2 = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.qualityGauge = new TycoonWindowGenerationLib.TycoonGauge_Gen();
            this.qualityCaption = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.traitsPanel = new FarmTycoonWindowGen.TraitsPanel();
            this.SuspendLayout();
            // 
            // hline2
            // 
            this.hline2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hline2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.hline2.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.hline2.ForeColor = System.Drawing.Color.White;
            this.hline2.Location = new System.Drawing.Point(0, 30);
            this.hline2.Name = "hline2";
            this.hline2.Size = new System.Drawing.Size(300, 1);
            this.hline2.TabIndex = 86;
            this.hline2.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.hline2.Tycoon_Tooltip = "";
            this.hline2.Tycoon_TooltipTime = 1D;
            this.hline2.Tycoon_Visible = true;
            // 
            // qualityGauge
            // 
            this.qualityGauge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.qualityGauge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.qualityGauge.Font = new System.Drawing.Font("Segoe UI", 6F);
            this.qualityGauge.ForeColor = System.Drawing.Color.Black;
            this.qualityGauge.Location = new System.Drawing.Point(75, 7);
            this.qualityGauge.Name = "qualityGauge";
            this.qualityGauge.Size = new System.Drawing.Size(220, 17);
            this.qualityGauge.TabIndex = 84;
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
            this.qualityGauge.Visible = false;
            // 
            // qualityCaption
            // 
            this.qualityCaption.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.qualityCaption.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.qualityCaption.ForeColor = System.Drawing.Color.White;
            this.qualityCaption.Location = new System.Drawing.Point(5, 9);
            this.qualityCaption.Name = "qualityCaption";
            this.qualityCaption.Size = new System.Drawing.Size(64, 15);
            this.qualityCaption.TabIndex = 83;
            this.qualityCaption.Text = "Quality:";
            this.qualityCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.qualityCaption.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.qualityCaption.Tycoon_Tooltip = "";
            this.qualityCaption.Tycoon_TooltipTime = 1D;
            this.qualityCaption.Tycoon_Visible = true;
            this.qualityCaption.Visible = false;
            // 
            // traitsPanel
            // 
            this.traitsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.traitsPanel.AutoScroll = true;
            this.traitsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.traitsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.traitsPanel.ForeColor = System.Drawing.Color.White;
            this.traitsPanel.Location = new System.Drawing.Point(0, 31);
            this.traitsPanel.Name = "traitsPanel";
            this.traitsPanel.Size = new System.Drawing.Size(300, 399);
            this.traitsPanel.TabIndex = 85;
            this.traitsPanel.Tycoon_AlwaysShowScroll = false;
            this.traitsPanel.Tycoon_Border = false;
            this.traitsPanel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.traitsPanel.Tycoon_ScrollDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.traitsPanel.Tycoon_ScrollDownTexture = "arrowdown";
            this.traitsPanel.Tycoon_ScrollLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.traitsPanel.Tycoon_ScrollPosition = 0;
            this.traitsPanel.Tycoon_ScrollUpTexture = "arrowup";
            this.traitsPanel.Tycoon_Tooltip = "";
            this.traitsPanel.Tycoon_TooltipTime = 1D;
            this.traitsPanel.Tycoon_Visible = true;
            // 
            // SingleQualityWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 430);
            this.Controls.Add(this.hline2);
            this.Controls.Add(this.qualityGauge);
            this.Controls.Add(this.qualityCaption);
            this.Controls.Add(this.traitsPanel);
            this.Name = "SingleQualityWindow";
            this.Text = "StorageBuildingWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonLabel_Gen hline2;
        private TycoonWindowGenerationLib.TycoonGauge_Gen qualityGauge;
        private TycoonWindowGenerationLib.TycoonLabel_Gen qualityCaption;
        private TraitsPanel traitsPanel;


    }
}