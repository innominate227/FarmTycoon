namespace FarmTycoonWindowGen
{
    partial class SimpleTaskWindow
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
            this.ItemLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.cancelButton = new TycoonWindowGenerationLib.TycoonButton_Gen();
            this.okButton = new TycoonWindowGenerationLib.TycoonButton_Gen();
            this.tycoonPanel_Gen1 = new TycoonWindowGenerationLib.TycoonPanel_Gen();
            this.tycoonLabel_Gen5 = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.tycoonLabel_Gen4 = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.SchedulePanel = new FarmTycoonWindowGen.SchedulePanel();
            this.numberOfWorkersPanel = new FarmTycoonWindowGen.NumberOfWorkersPanel();
            this.UseEquipmentCheckbox = new FarmTycoonWindowGen.UseEquipmentCheckbox();
            this.issuesAndTimePanel = new FarmTycoonWindowGen.IssuesAndTimePanel();
            this.ItemPanel = new FarmTycoonWindowGen.ItemsPanel();
            this.tycoonPanel_Gen1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ItemLabel
            // 
            this.ItemLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ItemLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.ItemLabel.ForeColor = System.Drawing.Color.White;
            this.ItemLabel.Location = new System.Drawing.Point(5, 70);
            this.ItemLabel.Name = "ItemLabel";
            this.ItemLabel.Size = new System.Drawing.Size(190, 15);
            this.ItemLabel.TabIndex = 3;
            this.ItemLabel.Text = "Seed to plant:";
            this.ItemLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ItemLabel.Tycoon_Tooltip = "";
            this.ItemLabel.Tycoon_TooltipTime = 1D;
            this.ItemLabel.Tycoon_Visible = true;
            this.ItemLabel.Click += new System.EventHandler(this.seedToPlantLabel_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.cancelButton.ForeColor = System.Drawing.Color.White;
            this.cancelButton.Location = new System.Drawing.Point(220, 330);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(92, 15);
            this.cancelButton.TabIndex = 39;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Tycoon_Depressed = false;
            this.cancelButton.Tycoon_IconTexture = "";
            this.cancelButton.Tycoon_ShadowDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.cancelButton.Tycoon_ShadowLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.cancelButton.Tycoon_Tooltip = "";
            this.cancelButton.Tycoon_TooltipTime = 1D;
            this.cancelButton.Tycoon_Visible = true;
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okButton.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.okButton.ForeColor = System.Drawing.Color.White;
            this.okButton.Location = new System.Drawing.Point(5, 330);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(92, 15);
            this.okButton.TabIndex = 38;
            this.okButton.Text = "OK";
            this.okButton.Tycoon_Depressed = false;
            this.okButton.Tycoon_IconTexture = "";
            this.okButton.Tycoon_ShadowDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.okButton.Tycoon_ShadowLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.okButton.Tycoon_Tooltip = "";
            this.okButton.Tycoon_TooltipTime = 1D;
            this.okButton.Tycoon_Visible = true;
            this.okButton.UseVisualStyleBackColor = false;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // tycoonPanel_Gen1
            // 
            this.tycoonPanel_Gen1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tycoonPanel_Gen1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tycoonPanel_Gen1.Controls.Add(this.tycoonLabel_Gen5);
            this.tycoonPanel_Gen1.Controls.Add(this.tycoonLabel_Gen4);
            this.tycoonPanel_Gen1.Controls.Add(this.SchedulePanel);
            this.tycoonPanel_Gen1.Controls.Add(this.numberOfWorkersPanel);
            this.tycoonPanel_Gen1.ForeColor = System.Drawing.Color.White;
            this.tycoonPanel_Gen1.Location = new System.Drawing.Point(-3, -5);
            this.tycoonPanel_Gen1.Name = "tycoonPanel_Gen1";
            this.tycoonPanel_Gen1.Size = new System.Drawing.Size(320, 49);
            this.tycoonPanel_Gen1.TabIndex = 68;
            this.tycoonPanel_Gen1.Tycoon_AlwaysShowScroll = false;
            this.tycoonPanel_Gen1.Tycoon_Border = false;
            this.tycoonPanel_Gen1.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.tycoonPanel_Gen1.Tycoon_ScrollDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.tycoonPanel_Gen1.Tycoon_ScrollDownTexture = "arrowdown";
            this.tycoonPanel_Gen1.Tycoon_ScrollLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.tycoonPanel_Gen1.Tycoon_ScrollPosition = 0;
            this.tycoonPanel_Gen1.Tycoon_ScrollUpTexture = "arrowup";
            this.tycoonPanel_Gen1.Tycoon_Tooltip = "";
            this.tycoonPanel_Gen1.Tycoon_TooltipTime = 1D;
            this.tycoonPanel_Gen1.Tycoon_Visible = true;
            // 
            // tycoonLabel_Gen5
            // 
            this.tycoonLabel_Gen5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tycoonLabel_Gen5.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.tycoonLabel_Gen5.ForeColor = System.Drawing.Color.White;
            this.tycoonLabel_Gen5.Location = new System.Drawing.Point(162, 3);
            this.tycoonLabel_Gen5.Name = "tycoonLabel_Gen5";
            this.tycoonLabel_Gen5.Size = new System.Drawing.Size(62, 15);
            this.tycoonLabel_Gen5.TabIndex = 66;
            this.tycoonLabel_Gen5.Text = "Schedule:";
            this.tycoonLabel_Gen5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tycoonLabel_Gen5.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tycoonLabel_Gen5.Tycoon_Tooltip = "";
            this.tycoonLabel_Gen5.Tycoon_TooltipTime = 1D;
            this.tycoonLabel_Gen5.Tycoon_Visible = true;
            // 
            // tycoonLabel_Gen4
            // 
            this.tycoonLabel_Gen4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tycoonLabel_Gen4.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.tycoonLabel_Gen4.ForeColor = System.Drawing.Color.White;
            this.tycoonLabel_Gen4.Location = new System.Drawing.Point(5, 3);
            this.tycoonLabel_Gen4.Name = "tycoonLabel_Gen4";
            this.tycoonLabel_Gen4.Size = new System.Drawing.Size(62, 15);
            this.tycoonLabel_Gen4.TabIndex = 65;
            this.tycoonLabel_Gen4.Text = "Workers:";
            this.tycoonLabel_Gen4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tycoonLabel_Gen4.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tycoonLabel_Gen4.Tycoon_Tooltip = "";
            this.tycoonLabel_Gen4.Tycoon_TooltipTime = 1D;
            this.tycoonLabel_Gen4.Tycoon_Visible = true;
            // 
            // SchedulePanel
            // 
            this.SchedulePanel.AutoScroll = true;
            this.SchedulePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.SchedulePanel.ForeColor = System.Drawing.Color.White;
            this.SchedulePanel.Location = new System.Drawing.Point(162, 18);
            this.SchedulePanel.Name = "SchedulePanel";
            this.SchedulePanel.Size = new System.Drawing.Size(155, 26);
            this.SchedulePanel.TabIndex = 62;
            this.SchedulePanel.Tycoon_AlwaysShowScroll = false;
            this.SchedulePanel.Tycoon_Border = false;
            this.SchedulePanel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.SchedulePanel.Tycoon_ScrollDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.SchedulePanel.Tycoon_ScrollDownTexture = "arrowdown";
            this.SchedulePanel.Tycoon_ScrollLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.SchedulePanel.Tycoon_ScrollPosition = 0;
            this.SchedulePanel.Tycoon_ScrollUpTexture = "arrowup";
            this.SchedulePanel.Tycoon_Tooltip = "";
            this.SchedulePanel.Tycoon_TooltipTime = 1D;
            this.SchedulePanel.Tycoon_Visible = true;
            // 
            // numberOfWorkersPanel
            // 
            this.numberOfWorkersPanel.AutoScroll = true;
            this.numberOfWorkersPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.numberOfWorkersPanel.ForeColor = System.Drawing.Color.White;
            this.numberOfWorkersPanel.Location = new System.Drawing.Point(8, 20);
            this.numberOfWorkersPanel.Name = "numberOfWorkersPanel";
            this.numberOfWorkersPanel.Size = new System.Drawing.Size(89, 22);
            this.numberOfWorkersPanel.TabIndex = 49;
            this.numberOfWorkersPanel.Tycoon_AlwaysShowScroll = false;
            this.numberOfWorkersPanel.Tycoon_Border = false;
            this.numberOfWorkersPanel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.numberOfWorkersPanel.Tycoon_ScrollDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.numberOfWorkersPanel.Tycoon_ScrollDownTexture = "arrowdown";
            this.numberOfWorkersPanel.Tycoon_ScrollLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.numberOfWorkersPanel.Tycoon_ScrollPosition = 0;
            this.numberOfWorkersPanel.Tycoon_ScrollUpTexture = "arrowup";
            this.numberOfWorkersPanel.Tycoon_Tooltip = "";
            this.numberOfWorkersPanel.Tycoon_TooltipTime = 1D;
            this.numberOfWorkersPanel.Tycoon_Visible = true;
            // 
            // UseEquipmentCheckbox
            // 
            this.UseEquipmentCheckbox.AutoScroll = true;
            this.UseEquipmentCheckbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.UseEquipmentCheckbox.ForeColor = System.Drawing.Color.White;
            this.UseEquipmentCheckbox.Location = new System.Drawing.Point(9, 50);
            this.UseEquipmentCheckbox.Name = "UseEquipmentCheckbox";
            this.UseEquipmentCheckbox.Size = new System.Drawing.Size(303, 15);
            this.UseEquipmentCheckbox.TabIndex = 69;
            this.UseEquipmentCheckbox.Tycoon_AlwaysShowScroll = false;
            this.UseEquipmentCheckbox.Tycoon_Border = false;
            this.UseEquipmentCheckbox.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.UseEquipmentCheckbox.Tycoon_ScrollDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.UseEquipmentCheckbox.Tycoon_ScrollDownTexture = "arrowdown";
            this.UseEquipmentCheckbox.Tycoon_ScrollLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.UseEquipmentCheckbox.Tycoon_ScrollPosition = 0;
            this.UseEquipmentCheckbox.Tycoon_ScrollUpTexture = "arrowup";
            this.UseEquipmentCheckbox.Tycoon_Tooltip = "";
            this.UseEquipmentCheckbox.Tycoon_TooltipTime = 1D;
            this.UseEquipmentCheckbox.Tycoon_Visible = true;
            // 
            // issuesAndTimePanel
            // 
            this.issuesAndTimePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.issuesAndTimePanel.ForeColor = System.Drawing.Color.White;
            this.issuesAndTimePanel.Location = new System.Drawing.Point(5, 230);
            this.issuesAndTimePanel.Name = "issuesAndTimePanel";
            this.issuesAndTimePanel.Size = new System.Drawing.Size(312, 90);
            this.issuesAndTimePanel.TabIndex = 46;
            this.issuesAndTimePanel.Tycoon_AlwaysShowScroll = false;
            this.issuesAndTimePanel.Tycoon_Border = false;
            this.issuesAndTimePanel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.issuesAndTimePanel.Tycoon_ScrollDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.issuesAndTimePanel.Tycoon_ScrollDownTexture = "arrowdown";
            this.issuesAndTimePanel.Tycoon_ScrollLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.issuesAndTimePanel.Tycoon_ScrollPosition = 0;
            this.issuesAndTimePanel.Tycoon_ScrollUpTexture = "arrowup";
            this.issuesAndTimePanel.Tycoon_Tooltip = "";
            this.issuesAndTimePanel.Tycoon_TooltipTime = 1D;
            this.issuesAndTimePanel.Tycoon_Visible = true;
            this.issuesAndTimePanel.Load += new System.EventHandler(this.issuesAndTimePanel_Load);
            // 
            // ItemPanel
            // 
            this.ItemPanel.AutoScroll = true;
            this.ItemPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ItemPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ItemPanel.ForeColor = System.Drawing.Color.White;
            this.ItemPanel.Location = new System.Drawing.Point(5, 85);
            this.ItemPanel.Name = "ItemPanel";
            this.ItemPanel.Size = new System.Drawing.Size(310, 135);
            this.ItemPanel.TabIndex = 4;
            this.ItemPanel.Tycoon_AlwaysShowScroll = false;
            this.ItemPanel.Tycoon_Border = true;
            this.ItemPanel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.ItemPanel.Tycoon_ScrollDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.ItemPanel.Tycoon_ScrollDownTexture = "arrowdown";
            this.ItemPanel.Tycoon_ScrollLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.ItemPanel.Tycoon_ScrollPosition = 0;
            this.ItemPanel.Tycoon_ScrollUpTexture = "arrowup";
            this.ItemPanel.Tycoon_Tooltip = "";
            this.ItemPanel.Tycoon_TooltipTime = 1D;
            this.ItemPanel.Tycoon_Visible = true;
            // 
            // SimpleTaskWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 350);
            this.Controls.Add(this.UseEquipmentCheckbox);
            this.Controls.Add(this.tycoonPanel_Gen1);
            this.Controls.Add(this.issuesAndTimePanel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.ItemPanel);
            this.Controls.Add(this.ItemLabel);
            this.Name = "SimpleTaskWindow";
            this.tycoonPanel_Gen1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonLabel_Gen ItemLabel;
        private ItemsPanel ItemPanel;
        private TycoonWindowGenerationLib.TycoonButton_Gen cancelButton;
        private TycoonWindowGenerationLib.TycoonButton_Gen okButton;
        private IssuesAndTimePanel issuesAndTimePanel;
        private TycoonWindowGenerationLib.TycoonPanel_Gen tycoonPanel_Gen1;
        private TycoonWindowGenerationLib.TycoonLabel_Gen tycoonLabel_Gen5;
        private TycoonWindowGenerationLib.TycoonLabel_Gen tycoonLabel_Gen4;
        private SchedulePanel SchedulePanel;
        private NumberOfWorkersPanel numberOfWorkersPanel;
        private UseEquipmentCheckbox UseEquipmentCheckbox;

    }
}
