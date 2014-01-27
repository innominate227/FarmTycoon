namespace FarmTycoonWindowGen.Buildings
{
    partial class SingleTraitsWindow
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
            this.traitsPanel = new FarmTycoonWindowGen.TraitsPanel();
            this.SuspendLayout();
            // 
            // traitsPanel
            // 
            this.traitsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.traitsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.traitsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.traitsPanel.ForeColor = System.Drawing.Color.White;
            this.traitsPanel.Location = new System.Drawing.Point(0, 0);
            this.traitsPanel.Name = "traitsPanel";
            this.traitsPanel.Size = new System.Drawing.Size(300, 429);
            this.traitsPanel.TabIndex = 42;
            this.traitsPanel.Tycoon_AlwaysShowScroll = false;
            this.traitsPanel.Tycoon_Border = false;
            this.traitsPanel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.traitsPanel.Tycoon_ScrollDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.traitsPanel.Tycoon_ScrollDownTexture = "arrowdown";
            this.traitsPanel.Tycoon_ScrollLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.traitsPanel.Tycoon_ScrollPosition = 0;
            this.traitsPanel.Tycoon_ScrollUpTexture = "arrowup";
            this.traitsPanel.Tycoon_Tooltip = "";
            this.traitsPanel.Tycoon_TooltipTime = 1D;
            this.traitsPanel.Tycoon_Visible = true;
            // 
            // SingleTraitsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 430);
            this.Controls.Add(this.traitsPanel);
            this.Name = "SingleTraitsWindow";
            this.Text = "StorageBuildingWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private TraitsPanel traitsPanel;

    }
}