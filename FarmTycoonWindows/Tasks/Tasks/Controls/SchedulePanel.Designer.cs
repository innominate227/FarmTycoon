namespace FarmTycoonWindowGen
{
    partial class SchedulePanel
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
            this.EditScheduleButton = new TycoonWindowGenerationLib.TycoonButton_Gen();
            this.ScheduleLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.SuspendLayout();
            // 
            // EditScheduleButton
            // 
            this.EditScheduleButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.EditScheduleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EditScheduleButton.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.EditScheduleButton.ForeColor = System.Drawing.Color.White;
            this.EditScheduleButton.Location = new System.Drawing.Point(0, 2);
            this.EditScheduleButton.Name = "EditScheduleButton";
            this.EditScheduleButton.Size = new System.Drawing.Size(20, 22);
            this.EditScheduleButton.TabIndex = 39;
            this.EditScheduleButton.Tycoon_Depressed = false;
            this.EditScheduleButton.Tycoon_IconTexture = "calandar";
            this.EditScheduleButton.Tycoon_ShadowDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.EditScheduleButton.Tycoon_ShadowLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.EditScheduleButton.Tycoon_Tooltip = "";
            this.EditScheduleButton.Tycoon_TooltipTime = 1D;
            this.EditScheduleButton.Tycoon_Visible = true;
            this.EditScheduleButton.UseVisualStyleBackColor = false;
            // 
            // ScheduleLabel
            // 
            this.ScheduleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ScheduleLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ScheduleLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.ScheduleLabel.ForeColor = System.Drawing.Color.White;
            this.ScheduleLabel.Location = new System.Drawing.Point(24, 0);
            this.ScheduleLabel.Name = "ScheduleLabel";
            this.ScheduleLabel.Size = new System.Drawing.Size(168, 26);
            this.ScheduleLabel.TabIndex = 37;
            this.ScheduleLabel.Text = "One Time";
            this.ScheduleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ScheduleLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ScheduleLabel.Tycoon_Tooltip = "";
            this.ScheduleLabel.Tycoon_TooltipTime = 1D;
            this.ScheduleLabel.Tycoon_Visible = true;
            // 
            // SchedulePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Controls.Add(this.EditScheduleButton);
            this.Controls.Add(this.ScheduleLabel);
            this.Name = "SchedulePanel";
            this.Size = new System.Drawing.Size(192, 26);
            this.Tycoon_Border = false;
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonButton_Gen EditScheduleButton;
        private TycoonWindowGenerationLib.TycoonLabel_Gen ScheduleLabel;

    }
}
