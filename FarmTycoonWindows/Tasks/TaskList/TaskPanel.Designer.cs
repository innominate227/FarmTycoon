namespace FarmTycoonWindowGen
{
    partial class TaskPanel
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
            this.NameLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.WorkersLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.DateLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.SuspendLayout();
            // 
            // NameLabel
            // 
            this.NameLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.NameLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.NameLabel.ForeColor = System.Drawing.Color.White;
            this.NameLabel.Location = new System.Drawing.Point(2, 0);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(298, 30);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Spray XXXXXXXXXXXXX On YYYYYYYYYYYY";
            this.NameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.NameLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.NameLabel.Tycoon_Tooltip = "";
            this.NameLabel.Tycoon_TooltipTime = 1D;
            this.NameLabel.Tycoon_Visible = true;
            // 
            // WorkersLabel
            // 
            this.WorkersLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.WorkersLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.WorkersLabel.ForeColor = System.Drawing.Color.White;
            this.WorkersLabel.Location = new System.Drawing.Point(300, 0);
            this.WorkersLabel.Name = "WorkersLabel";
            this.WorkersLabel.Size = new System.Drawing.Size(50, 30);
            this.WorkersLabel.TabIndex = 3;
            this.WorkersLabel.Text = "Workers";
            this.WorkersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.WorkersLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.WorkersLabel.Tycoon_Tooltip = "";
            this.WorkersLabel.Tycoon_TooltipTime = 1D;
            this.WorkersLabel.Tycoon_Visible = true;
            // 
            // DateLabel
            // 
            this.DateLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.DateLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.DateLabel.ForeColor = System.Drawing.Color.White;
            this.DateLabel.Location = new System.Drawing.Point(350, 0);
            this.DateLabel.Name = "DateLabel";
            this.DateLabel.Size = new System.Drawing.Size(75, 30);
            this.DateLabel.TabIndex = 4;
            this.DateLabel.Text = "01/01/9999";
            this.DateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.DateLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.DateLabel.Tycoon_Tooltip = "";
            this.DateLabel.Tycoon_TooltipTime = 1D;
            this.DateLabel.Tycoon_Visible = true;
            // 
            // TaskPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.DateLabel);
            this.Controls.Add(this.WorkersLabel);
            this.Controls.Add(this.NameLabel);
            this.Name = "TaskPanel";
            this.Size = new System.Drawing.Size(440, 30);
            this.Tycoon_Border = false;
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonLabel_Gen NameLabel;
        private TycoonWindowGenerationLib.TycoonLabel_Gen WorkersLabel;
        private TycoonWindowGenerationLib.TycoonLabel_Gen DateLabel;

    }
}
