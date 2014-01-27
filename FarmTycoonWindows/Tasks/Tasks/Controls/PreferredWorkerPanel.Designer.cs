namespace FarmTycoonWindowGen
{
    partial class PreferredWorkerPanel
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
            this.WorkerNameLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.CheckButton = new TycoonWindowGenerationLib.TycoonButton_Gen();
            this.SuspendLayout();
            // 
            // WorkerNameLabel
            // 
            this.WorkerNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WorkerNameLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.WorkerNameLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.WorkerNameLabel.ForeColor = System.Drawing.Color.White;
            this.WorkerNameLabel.Location = new System.Drawing.Point(20, 2);
            this.WorkerNameLabel.Name = "WorkerNameLabel";
            this.WorkerNameLabel.Size = new System.Drawing.Size(200, 16);
            this.WorkerNameLabel.TabIndex = 0;
            this.WorkerNameLabel.Text = "Worker 1";
            this.WorkerNameLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.WorkerNameLabel.Tycoon_Tooltip = "";
            this.WorkerNameLabel.Tycoon_TooltipTime = 1D;
            this.WorkerNameLabel.Tycoon_Visible = true;
            // 
            // CheckButton
            // 
            this.CheckButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CheckButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CheckButton.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.CheckButton.ForeColor = System.Drawing.Color.White;
            this.CheckButton.Location = new System.Drawing.Point(2, 2);
            this.CheckButton.Name = "CheckButton";
            this.CheckButton.Size = new System.Drawing.Size(16, 16);
            this.CheckButton.TabIndex = 2;
            this.CheckButton.Tycoon_Depressed = false;
            this.CheckButton.Tycoon_IconTexture = "";
            this.CheckButton.Tycoon_ShadowDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.CheckButton.Tycoon_ShadowLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.CheckButton.Tycoon_Tooltip = "";
            this.CheckButton.Tycoon_TooltipTime = 1D;
            this.CheckButton.Tycoon_Visible = true;
            this.CheckButton.UseVisualStyleBackColor = false;
            // 
            // PreferredWorkerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.CheckButton);
            this.Controls.Add(this.WorkerNameLabel);
            this.Name = "PreferredWorkerPanel";
            this.Size = new System.Drawing.Size(222, 20);
            this.Tycoon_Border = false;
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonLabel_Gen WorkerNameLabel;
        private TycoonWindowGenerationLib.TycoonButton_Gen CheckButton;

    }
}
