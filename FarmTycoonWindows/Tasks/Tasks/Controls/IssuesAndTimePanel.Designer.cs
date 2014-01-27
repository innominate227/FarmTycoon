namespace FarmTycoonWindowGen
{
    partial class IssuesAndTimePanel
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
            this.expectedTimeLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.issuesLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.SuspendLayout();
            // 
            // expectedTimeLabel
            // 
            this.expectedTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.expectedTimeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.expectedTimeLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.expectedTimeLabel.ForeColor = System.Drawing.Color.White;
            this.expectedTimeLabel.Location = new System.Drawing.Point(0, 0);
            this.expectedTimeLabel.Name = "expectedTimeLabel";
            this.expectedTimeLabel.Size = new System.Drawing.Size(188, 15);
            this.expectedTimeLabel.TabIndex = 39;
            this.expectedTimeLabel.Text = "Expected Time:  4 days";
            this.expectedTimeLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.expectedTimeLabel.Tycoon_Tooltip = "";
            this.expectedTimeLabel.Tycoon_TooltipTime = 1D;
            this.expectedTimeLabel.Tycoon_Visible = true;
            // 
            // issuesLabel
            // 
            this.issuesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.issuesLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.issuesLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.issuesLabel.ForeColor = System.Drawing.Color.White;
            this.issuesLabel.Location = new System.Drawing.Point(0, 20);
            this.issuesLabel.Name = "issuesLabel";
            this.issuesLabel.Size = new System.Drawing.Size(188, 70);
            this.issuesLabel.TabIndex = 38;
            this.issuesLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.issuesLabel.Tycoon_Tooltip = "";
            this.issuesLabel.Tycoon_TooltipTime = 1D;
            this.issuesLabel.Tycoon_Visible = true;
            // 
            // IssuesAndTimePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Controls.Add(this.expectedTimeLabel);
            this.Controls.Add(this.issuesLabel);
            this.Name = "IssuesAndTimePanel";
            this.Size = new System.Drawing.Size(190, 90);
            this.Tycoon_Border = false;
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonLabel_Gen expectedTimeLabel;
        private TycoonWindowGenerationLib.TycoonLabel_Gen issuesLabel;


    }
}
