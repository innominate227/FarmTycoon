namespace FarmTycoonWindowGen.Buildings
{
    partial class MultiQualityWindow
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
            this.hLine = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.fieldSizeLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.fieldSizeTitle = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.cropPlantedLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.cropPlantedTitle = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.showSubQualitiesButton = new TycoonWindowGenerationLib.TycoonButton_Gen();
            this.showTraitsButton = new TycoonWindowGenerationLib.TycoonButton_Gen();
            this.qualityGauge = new TycoonWindowGenerationLib.TycoonGauge_Gen();
            this.qualityCaption = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.hline2 = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.subQualitiesPanel = new FarmTycoonWindowGen.SubQualitiesPanel();
            this.traitsPanel = new FarmTycoonWindowGen.TraitsPanel();
            this.SuspendLayout();
            // 
            // hLine
            // 
            this.hLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.hLine.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.hLine.ForeColor = System.Drawing.Color.White;
            this.hLine.Location = new System.Drawing.Point(0, 52);
            this.hLine.Name = "hLine";
            this.hLine.Size = new System.Drawing.Size(300, 1);
            this.hLine.TabIndex = 41;
            this.hLine.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.hLine.Tycoon_Tooltip = "";
            this.hLine.Tycoon_TooltipTime = 1D;
            this.hLine.Tycoon_Visible = true;
            // 
            // fieldSizeLabel
            // 
            this.fieldSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fieldSizeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.fieldSizeLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.fieldSizeLabel.ForeColor = System.Drawing.Color.White;
            this.fieldSizeLabel.Location = new System.Drawing.Point(75, 5);
            this.fieldSizeLabel.Name = "fieldSizeLabel";
            this.fieldSizeLabel.Size = new System.Drawing.Size(190, 15);
            this.fieldSizeLabel.TabIndex = 39;
            this.fieldSizeLabel.Text = "100";
            this.fieldSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fieldSizeLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.fieldSizeLabel.Tycoon_Tooltip = "";
            this.fieldSizeLabel.Tycoon_TooltipTime = 1D;
            this.fieldSizeLabel.Tycoon_Visible = true;
            // 
            // fieldSizeTitle
            // 
            this.fieldSizeTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.fieldSizeTitle.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.fieldSizeTitle.ForeColor = System.Drawing.Color.White;
            this.fieldSizeTitle.Location = new System.Drawing.Point(5, 5);
            this.fieldSizeTitle.Name = "fieldSizeTitle";
            this.fieldSizeTitle.Size = new System.Drawing.Size(64, 15);
            this.fieldSizeTitle.TabIndex = 38;
            this.fieldSizeTitle.Text = "Size:";
            this.fieldSizeTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.fieldSizeTitle.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.fieldSizeTitle.Tycoon_Tooltip = "";
            this.fieldSizeTitle.Tycoon_TooltipTime = 1D;
            this.fieldSizeTitle.Tycoon_Visible = true;
            // 
            // cropPlantedLabel
            // 
            this.cropPlantedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cropPlantedLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cropPlantedLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.cropPlantedLabel.ForeColor = System.Drawing.Color.White;
            this.cropPlantedLabel.Location = new System.Drawing.Point(75, 30);
            this.cropPlantedLabel.Name = "cropPlantedLabel";
            this.cropPlantedLabel.Size = new System.Drawing.Size(190, 15);
            this.cropPlantedLabel.TabIndex = 37;
            this.cropPlantedLabel.Text = "CORN";
            this.cropPlantedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cropPlantedLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cropPlantedLabel.Tycoon_Tooltip = "";
            this.cropPlantedLabel.Tycoon_TooltipTime = 1D;
            this.cropPlantedLabel.Tycoon_Visible = true;
            // 
            // cropPlantedTitle
            // 
            this.cropPlantedTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cropPlantedTitle.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.cropPlantedTitle.ForeColor = System.Drawing.Color.White;
            this.cropPlantedTitle.Location = new System.Drawing.Point(5, 30);
            this.cropPlantedTitle.Name = "cropPlantedTitle";
            this.cropPlantedTitle.Size = new System.Drawing.Size(64, 15);
            this.cropPlantedTitle.TabIndex = 36;
            this.cropPlantedTitle.Text = "Planted:";
            this.cropPlantedTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cropPlantedTitle.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cropPlantedTitle.Tycoon_Tooltip = "";
            this.cropPlantedTitle.Tycoon_TooltipTime = 1D;
            this.cropPlantedTitle.Tycoon_Visible = true;
            // 
            // showSubQualitiesButton
            // 
            this.showSubQualitiesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.showSubQualitiesButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.showSubQualitiesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.showSubQualitiesButton.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.showSubQualitiesButton.ForeColor = System.Drawing.Color.White;
            this.showSubQualitiesButton.Location = new System.Drawing.Point(273, 28);
            this.showSubQualitiesButton.Name = "showSubQualitiesButton";
            this.showSubQualitiesButton.Size = new System.Drawing.Size(22, 22);
            this.showSubQualitiesButton.TabIndex = 76;
            this.showSubQualitiesButton.Text = "S";
            this.showSubQualitiesButton.Tycoon_Depressed = false;
            this.showSubQualitiesButton.Tycoon_IconTexture = "";
            this.showSubQualitiesButton.Tycoon_ShadowDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.showSubQualitiesButton.Tycoon_ShadowLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.showSubQualitiesButton.Tycoon_Tooltip = "";
            this.showSubQualitiesButton.Tycoon_TooltipTime = 1D;
            this.showSubQualitiesButton.Tycoon_Visible = true;
            this.showSubQualitiesButton.UseVisualStyleBackColor = false;
            // 
            // showTraitsButton
            // 
            this.showTraitsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.showTraitsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.showTraitsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.showTraitsButton.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.showTraitsButton.ForeColor = System.Drawing.Color.White;
            this.showTraitsButton.Location = new System.Drawing.Point(273, 3);
            this.showTraitsButton.Name = "showTraitsButton";
            this.showTraitsButton.Size = new System.Drawing.Size(22, 22);
            this.showTraitsButton.TabIndex = 75;
            this.showTraitsButton.Text = "T";
            this.showTraitsButton.Tycoon_Depressed = true;
            this.showTraitsButton.Tycoon_IconTexture = "";
            this.showTraitsButton.Tycoon_ShadowDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.showTraitsButton.Tycoon_ShadowLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.showTraitsButton.Tycoon_Tooltip = "";
            this.showTraitsButton.Tycoon_TooltipTime = 1D;
            this.showTraitsButton.Tycoon_Visible = true;
            this.showTraitsButton.UseVisualStyleBackColor = false;
            // 
            // qualityGauge
            // 
            this.qualityGauge.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.qualityGauge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.qualityGauge.Font = new System.Drawing.Font("Segoe UI", 6F);
            this.qualityGauge.ForeColor = System.Drawing.Color.Black;
            this.qualityGauge.Location = new System.Drawing.Point(75, 58);
            this.qualityGauge.Name = "qualityGauge";
            this.qualityGauge.Size = new System.Drawing.Size(220, 17);
            this.qualityGauge.TabIndex = 79;
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
            this.qualityCaption.Location = new System.Drawing.Point(5, 60);
            this.qualityCaption.Name = "qualityCaption";
            this.qualityCaption.Size = new System.Drawing.Size(64, 15);
            this.qualityCaption.TabIndex = 77;
            this.qualityCaption.Text = "Quality:";
            this.qualityCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.qualityCaption.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.qualityCaption.Tycoon_Tooltip = "";
            this.qualityCaption.Tycoon_TooltipTime = 1D;
            this.qualityCaption.Tycoon_Visible = true;
            this.qualityCaption.Visible = false;
            // 
            // hline2
            // 
            this.hline2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hline2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.hline2.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.hline2.ForeColor = System.Drawing.Color.White;
            this.hline2.Location = new System.Drawing.Point(0, 81);
            this.hline2.Name = "hline2";
            this.hline2.Size = new System.Drawing.Size(300, 1);
            this.hline2.TabIndex = 82;
            this.hline2.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.hline2.Tycoon_Tooltip = "";
            this.hline2.Tycoon_TooltipTime = 1D;
            this.hline2.Tycoon_Visible = true;
            // 
            // subQualitiesPanel
            // 
            this.subQualitiesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.subQualitiesPanel.AutoScroll = true;
            this.subQualitiesPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.subQualitiesPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.subQualitiesPanel.ForeColor = System.Drawing.Color.White;
            this.subQualitiesPanel.Location = new System.Drawing.Point(0, 82);
            this.subQualitiesPanel.Name = "subQualitiesPanel";
            this.subQualitiesPanel.Size = new System.Drawing.Size(300, 348);
            this.subQualitiesPanel.TabIndex = 81;
            this.subQualitiesPanel.Tycoon_AlwaysShowScroll = false;
            this.subQualitiesPanel.Tycoon_Border = false;
            this.subQualitiesPanel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.subQualitiesPanel.Tycoon_ScrollDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.subQualitiesPanel.Tycoon_ScrollDownTexture = "arrowdown";
            this.subQualitiesPanel.Tycoon_ScrollLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.subQualitiesPanel.Tycoon_ScrollPosition = 0;
            this.subQualitiesPanel.Tycoon_ScrollUpTexture = "arrowup";
            this.subQualitiesPanel.Tycoon_Tooltip = "";
            this.subQualitiesPanel.Tycoon_TooltipTime = 1D;
            this.subQualitiesPanel.Tycoon_Visible = true;
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
            this.traitsPanel.Location = new System.Drawing.Point(0, 82);
            this.traitsPanel.Name = "traitsPanel";
            this.traitsPanel.Size = new System.Drawing.Size(300, 348);
            this.traitsPanel.TabIndex = 80;
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
            // MultiQualityWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 430);
            this.Controls.Add(this.hline2);
            this.Controls.Add(this.qualityGauge);
            this.Controls.Add(this.qualityCaption);
            this.Controls.Add(this.showSubQualitiesButton);
            this.Controls.Add(this.showTraitsButton);
            this.Controls.Add(this.hLine);
            this.Controls.Add(this.fieldSizeLabel);
            this.Controls.Add(this.fieldSizeTitle);
            this.Controls.Add(this.cropPlantedLabel);
            this.Controls.Add(this.cropPlantedTitle);
            this.Controls.Add(this.traitsPanel);
            this.Controls.Add(this.subQualitiesPanel);
            this.Name = "MultiQualityWindow";
            this.Text = "StorageBuildingWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonLabel_Gen hLine;
        private TycoonWindowGenerationLib.TycoonLabel_Gen fieldSizeLabel;
        private TycoonWindowGenerationLib.TycoonLabel_Gen fieldSizeTitle;
        private TycoonWindowGenerationLib.TycoonLabel_Gen cropPlantedLabel;
        private TycoonWindowGenerationLib.TycoonLabel_Gen cropPlantedTitle;
        private TycoonWindowGenerationLib.TycoonButton_Gen showSubQualitiesButton;
        private TycoonWindowGenerationLib.TycoonButton_Gen showTraitsButton;
        private TycoonWindowGenerationLib.TycoonGauge_Gen qualityGauge;
        private TycoonWindowGenerationLib.TycoonLabel_Gen qualityCaption;
        private TraitsPanel traitsPanel;
        private SubQualitiesPanel subQualitiesPanel;
        private TycoonWindowGenerationLib.TycoonLabel_Gen hline2;

    }
}