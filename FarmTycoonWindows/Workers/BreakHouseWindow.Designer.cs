namespace FarmTycoonWindowGen.Buildings
{
    partial class BreakHouseWindow
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
            this.WorkersPanel = new FarmTycoonWindowGen.WorkersPanel();
            this.SpaceProgress = new TycoonWindowGenerationLib.TycoonProgress_Gen();
            this.SuspendLayout();
            // 
            // WorkersPanel
            // 
            this.WorkersPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WorkersPanel.AutoScroll = true;
            this.WorkersPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.WorkersPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.WorkersPanel.ForeColor = System.Drawing.Color.White;
            this.WorkersPanel.Location = new System.Drawing.Point(5, 5);
            this.WorkersPanel.Name = "WorkersPanel";
            this.WorkersPanel.Size = new System.Drawing.Size(290, 220);
            this.WorkersPanel.TabIndex = 5;
            this.WorkersPanel.Tycoon_AlwaysShowScroll = false;
            this.WorkersPanel.Tycoon_Border = true;
            this.WorkersPanel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.WorkersPanel.Tycoon_ScrollDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.WorkersPanel.Tycoon_ScrollDownTexture = "arrowdown";
            this.WorkersPanel.Tycoon_ScrollLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.WorkersPanel.Tycoon_ScrollPosition = 0;
            this.WorkersPanel.Tycoon_ScrollUpTexture = "arrowup";
            this.WorkersPanel.Tycoon_Tooltip = "";
            this.WorkersPanel.Tycoon_TooltipTime = 1D;
            this.WorkersPanel.Tycoon_Visible = true;
            // 
            // SpaceProgress
            // 
            this.SpaceProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SpaceProgress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.SpaceProgress.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.SpaceProgress.ForeColor = System.Drawing.Color.White;
            this.SpaceProgress.Location = new System.Drawing.Point(5, 230);
            this.SpaceProgress.Name = "SpaceProgress";
            this.SpaceProgress.Size = new System.Drawing.Size(290, 15);
            this.SpaceProgress.TabIndex = 6;
            this.SpaceProgress.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.SpaceProgress.Tycoon_ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.SpaceProgress.Tycoon_Text = "";
            this.SpaceProgress.Tycoon_Tooltip = "";
            this.SpaceProgress.Tycoon_TooltipTime = 1D;
            this.SpaceProgress.Tycoon_Visible = true;
            // 
            // BreakHouseWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 250);
            this.Controls.Add(this.SpaceProgress);
            this.Controls.Add(this.WorkersPanel);
            this.Name = "BreakHouseWindow";
            this.Text = "StorageBuildingWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private WorkersPanel WorkersPanel;
        private TycoonWindowGenerationLib.TycoonProgress_Gen SpaceProgress;
    }
}