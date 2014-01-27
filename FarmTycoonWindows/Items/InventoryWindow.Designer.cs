namespace FarmTycoonWindowGen.Buildings
{
    partial class InventoryWindow
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
            this.itemsPanel = new FarmTycoonWindowGen.ItemsPanel();
            this.SpaceProgress = new TycoonWindowGenerationLib.TycoonProgress_Gen();
            this.SuspendLayout();
            // 
            // itemsPanel
            // 
            this.itemsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.itemsPanel.AutoScroll = true;
            this.itemsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.itemsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.itemsPanel.ForeColor = System.Drawing.Color.White;
            this.itemsPanel.Location = new System.Drawing.Point(5, 5);
            this.itemsPanel.Name = "itemsPanel";
            this.itemsPanel.Size = new System.Drawing.Size(290, 220);
            this.itemsPanel.TabIndex = 5;
            this.itemsPanel.Tycoon_AlwaysShowScroll = false;
            this.itemsPanel.Tycoon_Border = true;
            this.itemsPanel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.itemsPanel.Tycoon_ScrollDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.itemsPanel.Tycoon_ScrollDownTexture = "arrowdown";
            this.itemsPanel.Tycoon_ScrollLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.itemsPanel.Tycoon_ScrollPosition = 0;
            this.itemsPanel.Tycoon_ScrollUpTexture = "arrowup";
            this.itemsPanel.Tycoon_Tooltip = "";
            this.itemsPanel.Tycoon_TooltipTime = 1D;
            this.itemsPanel.Tycoon_Visible = true;
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
            // InventoryWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 250);
            this.Controls.Add(this.SpaceProgress);
            this.Controls.Add(this.itemsPanel);
            this.Name = "InventoryWindow";
            this.Text = "StorageBuildingWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private ItemsPanel itemsPanel;
        private TycoonWindowGenerationLib.TycoonProgress_Gen SpaceProgress;
    }
}