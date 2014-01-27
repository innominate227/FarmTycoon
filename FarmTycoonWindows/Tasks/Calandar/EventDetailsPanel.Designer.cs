namespace FarmTycoonWindowGen
{
    partial class EventDetailsPanel
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
            this.eventDetailsLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.cancelButton = new TycoonWindowGenerationLib.TycoonButton_Gen();
            this.eventTitleLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.vLine = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.SuspendLayout();
            // 
            // eventDetailsLabel
            // 
            this.eventDetailsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.eventDetailsLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.eventDetailsLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.eventDetailsLabel.ForeColor = System.Drawing.Color.White;
            this.eventDetailsLabel.Location = new System.Drawing.Point(5, 25);
            this.eventDetailsLabel.Name = "eventDetailsLabel";
            this.eventDetailsLabel.Size = new System.Drawing.Size(288, 100);
            this.eventDetailsLabel.TabIndex = 29;
            this.eventDetailsLabel.Text = "Event Details bla, bla, bla";
            this.eventDetailsLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.eventDetailsLabel.Tycoon_Tooltip = "";
            this.eventDetailsLabel.Tycoon_TooltipTime = 1D;
            this.eventDetailsLabel.Tycoon_Visible = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.cancelButton.ForeColor = System.Drawing.Color.White;
            this.cancelButton.Location = new System.Drawing.Point(218, 130);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 15);
            this.cancelButton.TabIndex = 30;
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
            // eventTitleLabel
            // 
            this.eventTitleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.eventTitleLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.eventTitleLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.eventTitleLabel.ForeColor = System.Drawing.Color.White;
            this.eventTitleLabel.Location = new System.Drawing.Point(5, 5);
            this.eventTitleLabel.Name = "eventTitleLabel";
            this.eventTitleLabel.Size = new System.Drawing.Size(288, 15);
            this.eventTitleLabel.TabIndex = 31;
            this.eventTitleLabel.Text = "Event Title";
            this.eventTitleLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.eventTitleLabel.Tycoon_Tooltip = "";
            this.eventTitleLabel.Tycoon_TooltipTime = 1D;
            this.eventTitleLabel.Tycoon_Visible = true;
            // 
            // vLine
            // 
            this.vLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.vLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.vLine.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.vLine.ForeColor = System.Drawing.Color.White;
            this.vLine.Location = new System.Drawing.Point(5, 148);
            this.vLine.Name = "vLine";
            this.vLine.Size = new System.Drawing.Size(290, 1);
            this.vLine.TabIndex = 32;
            this.vLine.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.vLine.Tycoon_Tooltip = "";
            this.vLine.Tycoon_TooltipTime = 1D;
            this.vLine.Tycoon_Visible = true;
            // 
            // EventDetailsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.vLine);
            this.Controls.Add(this.eventTitleLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.eventDetailsLabel);
            this.Name = "EventDetailsPanel";
            this.Size = new System.Drawing.Size(298, 150);
            this.Tycoon_Border = false;
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonLabel_Gen eventDetailsLabel;
        private TycoonWindowGenerationLib.TycoonButton_Gen cancelButton;
        private TycoonWindowGenerationLib.TycoonLabel_Gen eventTitleLabel;
        private TycoonWindowGenerationLib.TycoonLabel_Gen vLine;
    }
}
