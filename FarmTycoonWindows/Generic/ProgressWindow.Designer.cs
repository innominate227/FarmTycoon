namespace FarmTycoonWindowGen
{
    partial class ProgressWindow
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
            this.progress = new TycoonWindowGenerationLib.TycoonProgress_Gen();
            this.SuspendLayout();
            // 
            // progress
            // 
            this.progress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.progress.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.progress.ForeColor = System.Drawing.Color.White;
            this.progress.Location = new System.Drawing.Point(5, 5);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(290, 20);
            this.progress.TabIndex = 0;
            this.progress.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.progress.Tycoon_ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.progress.Tycoon_Text = "";
            this.progress.Tycoon_Tooltip = "";
            this.progress.Tycoon_TooltipTime = 1D;
            this.progress.Tycoon_Visible = true;
            // 
            // ProgressWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(300, 30);
            this.Controls.Add(this.progress);
            this.Name = "ProgressWindow";
            this.Tycoon_TitleText = "";
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonProgress_Gen progress;



    }
}