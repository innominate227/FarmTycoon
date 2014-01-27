namespace FarmTycoonWindowGen
{
    partial class ProductionWindow
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
            this.cancelButton = new TycoonWindowGenerationLib.TycoonButton_Gen();
            this.okButton = new TycoonWindowGenerationLib.TycoonButton_Gen();
            this.CurrentWorkersLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.tycoonPanel_Gen1 = new TycoonWindowGenerationLib.TycoonPanel_Gen();
            this.tycoonLabel_Gen4 = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.numberOfWorkersPanel = new FarmTycoonWindowGen.NumberOfWorkersPanel();
            this.issuesAndTimePanel = new FarmTycoonWindowGen.IssuesAndTimePanel();
            this.MaxWorkersLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.MaxWorkersCaption = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.CurrentWorkersCaption = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.tycoonPanel_Gen1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.cancelButton.ForeColor = System.Drawing.Color.White;
            this.cancelButton.Location = new System.Drawing.Point(223, 150);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(92, 15);
            this.cancelButton.TabIndex = 42;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Tycoon_Depressed = false;
            this.cancelButton.Tycoon_IconTexture = "";
            this.cancelButton.Tycoon_ShadowDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.cancelButton.Tycoon_ShadowLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.cancelButton.Tycoon_Tooltip = "";
            this.cancelButton.Tycoon_TooltipTime = 1D;
            this.cancelButton.Tycoon_Visible = true;
            this.cancelButton.UseVisualStyleBackColor = false;
            // 
            // okButton
            // 
            this.okButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okButton.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.okButton.ForeColor = System.Drawing.Color.White;
            this.okButton.Location = new System.Drawing.Point(5, 150);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(92, 15);
            this.okButton.TabIndex = 41;
            this.okButton.Text = "OK";
            this.okButton.Tycoon_Depressed = false;
            this.okButton.Tycoon_IconTexture = "";
            this.okButton.Tycoon_ShadowDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.okButton.Tycoon_ShadowLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.okButton.Tycoon_Tooltip = "";
            this.okButton.Tycoon_TooltipTime = 1D;
            this.okButton.Tycoon_Visible = true;
            this.okButton.UseVisualStyleBackColor = false;
            // 
            // CurrentWorkersLabel
            // 
            this.CurrentWorkersLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CurrentWorkersLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.CurrentWorkersLabel.ForeColor = System.Drawing.Color.White;
            this.CurrentWorkersLabel.Location = new System.Drawing.Point(260, 11);
            this.CurrentWorkersLabel.Name = "CurrentWorkersLabel";
            this.CurrentWorkersLabel.Size = new System.Drawing.Size(47, 15);
            this.CurrentWorkersLabel.TabIndex = 46;
            this.CurrentWorkersLabel.Text = "XX Workers Currently Working";
            this.CurrentWorkersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CurrentWorkersLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CurrentWorkersLabel.Tycoon_Tooltip = "";
            this.CurrentWorkersLabel.Tycoon_TooltipTime = 1D;
            this.CurrentWorkersLabel.Tycoon_Visible = true;
            // 
            // tycoonPanel_Gen1
            // 
            this.tycoonPanel_Gen1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tycoonPanel_Gen1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tycoonPanel_Gen1.Controls.Add(this.MaxWorkersCaption);
            this.tycoonPanel_Gen1.Controls.Add(this.CurrentWorkersCaption);
            this.tycoonPanel_Gen1.Controls.Add(this.MaxWorkersLabel);
            this.tycoonPanel_Gen1.Controls.Add(this.CurrentWorkersLabel);
            this.tycoonPanel_Gen1.Controls.Add(this.tycoonLabel_Gen4);
            this.tycoonPanel_Gen1.Controls.Add(this.numberOfWorkersPanel);
            this.tycoonPanel_Gen1.ForeColor = System.Drawing.Color.White;
            this.tycoonPanel_Gen1.Location = new System.Drawing.Point(0, 0);
            this.tycoonPanel_Gen1.Name = "tycoonPanel_Gen1";
            this.tycoonPanel_Gen1.Size = new System.Drawing.Size(320, 49);
            this.tycoonPanel_Gen1.TabIndex = 69;
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
            // issuesAndTimePanel
            // 
            this.issuesAndTimePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.issuesAndTimePanel.ForeColor = System.Drawing.Color.White;
            this.issuesAndTimePanel.Location = new System.Drawing.Point(5, 50);
            this.issuesAndTimePanel.Name = "issuesAndTimePanel";
            this.issuesAndTimePanel.Size = new System.Drawing.Size(310, 90);
            this.issuesAndTimePanel.TabIndex = 45;
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
            // 
            // MaxWorkersLabel
            // 
            this.MaxWorkersLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MaxWorkersLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.MaxWorkersLabel.ForeColor = System.Drawing.Color.White;
            this.MaxWorkersLabel.Location = new System.Drawing.Point(260, 28);
            this.MaxWorkersLabel.Name = "MaxWorkersLabel";
            this.MaxWorkersLabel.Size = new System.Drawing.Size(47, 15);
            this.MaxWorkersLabel.TabIndex = 66;
            this.MaxWorkersLabel.Text = "XX Workers Currently Working";
            this.MaxWorkersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.MaxWorkersLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MaxWorkersLabel.Tycoon_Tooltip = "";
            this.MaxWorkersLabel.Tycoon_TooltipTime = 1D;
            this.MaxWorkersLabel.Tycoon_Visible = true;
            // 
            // MaxWorkersCaption
            // 
            this.MaxWorkersCaption.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MaxWorkersCaption.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.MaxWorkersCaption.ForeColor = System.Drawing.Color.White;
            this.MaxWorkersCaption.Location = new System.Drawing.Point(150, 28);
            this.MaxWorkersCaption.Name = "MaxWorkersCaption";
            this.MaxWorkersCaption.Size = new System.Drawing.Size(104, 15);
            this.MaxWorkersCaption.TabIndex = 68;
            this.MaxWorkersCaption.Text = "Max Workers:";
            this.MaxWorkersCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.MaxWorkersCaption.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MaxWorkersCaption.Tycoon_Tooltip = "";
            this.MaxWorkersCaption.Tycoon_TooltipTime = 1D;
            this.MaxWorkersCaption.Tycoon_Visible = true;
            // 
            // CurrentWorkersCaption
            // 
            this.CurrentWorkersCaption.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CurrentWorkersCaption.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.CurrentWorkersCaption.ForeColor = System.Drawing.Color.White;
            this.CurrentWorkersCaption.Location = new System.Drawing.Point(150, 11);
            this.CurrentWorkersCaption.Name = "CurrentWorkersCaption";
            this.CurrentWorkersCaption.Size = new System.Drawing.Size(104, 15);
            this.CurrentWorkersCaption.TabIndex = 67;
            this.CurrentWorkersCaption.Text = "Current Workers:";
            this.CurrentWorkersCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CurrentWorkersCaption.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CurrentWorkersCaption.Tycoon_Tooltip = "";
            this.CurrentWorkersCaption.Tycoon_TooltipTime = 1D;
            this.CurrentWorkersCaption.Tycoon_Visible = true;
            // 
            // ProductionWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 170);
            this.Controls.Add(this.tycoonPanel_Gen1);
            this.Controls.Add(this.issuesAndTimePanel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Name = "ProductionWindow";
            this.Tycoon_TitleText = "Plow Field";
            this.tycoonPanel_Gen1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonButton_Gen cancelButton;
        private TycoonWindowGenerationLib.TycoonButton_Gen okButton;
        private IssuesAndTimePanel issuesAndTimePanel;
        private TycoonWindowGenerationLib.TycoonLabel_Gen CurrentWorkersLabel;
        private TycoonWindowGenerationLib.TycoonPanel_Gen tycoonPanel_Gen1;
        private TycoonWindowGenerationLib.TycoonLabel_Gen tycoonLabel_Gen4;
        private NumberOfWorkersPanel numberOfWorkersPanel;
        private TycoonWindowGenerationLib.TycoonLabel_Gen MaxWorkersLabel;
        private TycoonWindowGenerationLib.TycoonLabel_Gen MaxWorkersCaption;
        private TycoonWindowGenerationLib.TycoonLabel_Gen CurrentWorkersCaption;


    }
}
