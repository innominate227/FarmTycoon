namespace FarmTycoonWindowGen.Buildings
{
    partial class WorkerViewWindow
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
            this.workerStatusLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.workerWorldView = new TycoonWindowGenerationLib.TycoonWorldViewPanel_Gen();
            this.SuspendLayout();
            // 
            // workerStatusLabel
            // 
            this.workerStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.workerStatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.workerStatusLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.workerStatusLabel.ForeColor = System.Drawing.Color.White;
            this.workerStatusLabel.Location = new System.Drawing.Point(5, 180);
            this.workerStatusLabel.Name = "workerStatusLabel";
            this.workerStatusLabel.Size = new System.Drawing.Size(240, 15);
            this.workerStatusLabel.TabIndex = 7;
            this.workerStatusLabel.Text = "STATUS";
            this.workerStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.workerStatusLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.workerStatusLabel.Tycoon_Tooltip = "";
            this.workerStatusLabel.Tycoon_TooltipTime = 1D;
            this.workerStatusLabel.Tycoon_Visible = true;
            // 
            // workerWorldView
            // 
            this.workerWorldView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.workerWorldView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.workerWorldView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.workerWorldView.ForeColor = System.Drawing.Color.White;
            this.workerWorldView.Location = new System.Drawing.Point(5, 5);
            this.workerWorldView.Name = "workerWorldView";
            this.workerWorldView.Size = new System.Drawing.Size(240, 175);
            this.workerWorldView.TabIndex = 8;
            this.workerWorldView.Text = "tycoonWorldViewPanel_Gen1";
            this.workerWorldView.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.workerWorldView.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.workerWorldView.Tycoon_ScrollDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.workerWorldView.Tycoon_ScrollLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.workerWorldView.Tycoon_Tooltip = "";
            this.workerWorldView.Tycoon_TooltipTime = 1D;
            this.workerWorldView.Tycoon_Visible = true;
            // 
            // WorkerViewWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(250, 200);
            this.Controls.Add(this.workerWorldView);
            this.Controls.Add(this.workerStatusLabel);
            this.Name = "WorkerViewWindow";
            this.Text = "StorageBuildingWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonLabel_Gen workerStatusLabel;
        private TycoonWindowGenerationLib.TycoonWorldViewPanel_Gen workerWorldView;
    }
}