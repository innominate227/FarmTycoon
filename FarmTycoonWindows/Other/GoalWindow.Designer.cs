namespace FarmTycoonWindowGen.Buildings
{
    partial class GoalWindow
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
            this.DescriptionLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.ObjectiveLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.ProgressLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.SuspendLayout();
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DescriptionLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.DescriptionLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.DescriptionLabel.ForeColor = System.Drawing.Color.White;
            this.DescriptionLabel.Location = new System.Drawing.Point(5, 5);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(340, 60);
            this.DescriptionLabel.TabIndex = 52;
            this.DescriptionLabel.Text = "Description bla bla bla\r\nLine2\r\nLine3\r\nLine4";
            this.DescriptionLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.DescriptionLabel.Tycoon_Tooltip = "";
            this.DescriptionLabel.Tycoon_TooltipTime = 1D;
            this.DescriptionLabel.Tycoon_Visible = true;
            // 
            // ObjectiveLabel
            // 
            this.ObjectiveLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ObjectiveLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ObjectiveLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.ObjectiveLabel.ForeColor = System.Drawing.Color.White;
            this.ObjectiveLabel.Location = new System.Drawing.Point(5, 65);
            this.ObjectiveLabel.Name = "ObjectiveLabel";
            this.ObjectiveLabel.Size = new System.Drawing.Size(340, 60);
            this.ObjectiveLabel.TabIndex = 53;
            this.ObjectiveLabel.Text = "Objective bla bla bla";
            this.ObjectiveLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ObjectiveLabel.Tycoon_Tooltip = "";
            this.ObjectiveLabel.Tycoon_TooltipTime = 1D;
            this.ObjectiveLabel.Tycoon_Visible = true;
            // 
            // ProgressLabel
            // 
            this.ProgressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ProgressLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.ProgressLabel.ForeColor = System.Drawing.Color.White;
            this.ProgressLabel.Location = new System.Drawing.Point(5, 125);
            this.ProgressLabel.Name = "ProgressLabel";
            this.ProgressLabel.Size = new System.Drawing.Size(340, 60);
            this.ProgressLabel.TabIndex = 54;
            this.ProgressLabel.Text = "Progress bla bla bla";
            this.ProgressLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ProgressLabel.Tycoon_Tooltip = "";
            this.ProgressLabel.Tycoon_TooltipTime = 1D;
            this.ProgressLabel.Tycoon_Visible = true;
            // 
            // GoalWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 190);
            this.Controls.Add(this.ProgressLabel);
            this.Controls.Add(this.ObjectiveLabel);
            this.Controls.Add(this.DescriptionLabel);
            this.Name = "GoalWindow";
            this.Text = "StorageBuildingWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonLabel_Gen DescriptionLabel;
        private TycoonWindowGenerationLib.TycoonLabel_Gen ObjectiveLabel;
        private TycoonWindowGenerationLib.TycoonLabel_Gen ProgressLabel;


    }
}