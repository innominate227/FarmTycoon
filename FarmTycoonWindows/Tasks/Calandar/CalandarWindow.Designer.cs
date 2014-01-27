namespace FarmTycoonWindowGen.Buildings
{
    partial class CalandarWindow
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
            this.taskDetailsPanel = new TycoonWindowGenerationLib.TycoonPanel_Gen();
            this.verticelLine = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.eventCalandarPanel = new FarmTycoonWindowGen.Events.CalandarDatesPanel();
            this.SuspendLayout();
            // 
            // taskDetailsPanel
            // 
            this.taskDetailsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.taskDetailsPanel.AutoScroll = true;
            this.taskDetailsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.taskDetailsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.taskDetailsPanel.ForeColor = System.Drawing.Color.White;
            this.taskDetailsPanel.Location = new System.Drawing.Point(390, 5);
            this.taskDetailsPanel.Name = "taskDetailsPanel";
            this.taskDetailsPanel.Size = new System.Drawing.Size(208, 470);
            this.taskDetailsPanel.TabIndex = 19;
            this.taskDetailsPanel.Tycoon_AlwaysShowScroll = false;
            this.taskDetailsPanel.Tycoon_Border = false;
            this.taskDetailsPanel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.taskDetailsPanel.Tycoon_ScrollDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.taskDetailsPanel.Tycoon_ScrollDownTexture = "arrowdown";
            this.taskDetailsPanel.Tycoon_ScrollLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.taskDetailsPanel.Tycoon_ScrollPosition = 0;
            this.taskDetailsPanel.Tycoon_ScrollUpTexture = "arrowup";
            this.taskDetailsPanel.Tycoon_Tooltip = "";
            this.taskDetailsPanel.Tycoon_TooltipTime = 1D;
            this.taskDetailsPanel.Tycoon_Visible = true;
            // 
            // verticelLine
            // 
            this.verticelLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.verticelLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.verticelLine.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.verticelLine.ForeColor = System.Drawing.Color.White;
            this.verticelLine.Location = new System.Drawing.Point(385, 0);
            this.verticelLine.Name = "verticelLine";
            this.verticelLine.Size = new System.Drawing.Size(1, 480);
            this.verticelLine.TabIndex = 18;
            this.verticelLine.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.verticelLine.Tycoon_Tooltip = "";
            this.verticelLine.Tycoon_TooltipTime = 1D;
            this.verticelLine.Tycoon_Visible = true;
            // 
            // eventCalandarPanel
            // 
            this.eventCalandarPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.eventCalandarPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.eventCalandarPanel.ForeColor = System.Drawing.Color.White;
            this.eventCalandarPanel.Location = new System.Drawing.Point(5, 5);
            this.eventCalandarPanel.Name = "eventCalandarPanel";
            this.eventCalandarPanel.Size = new System.Drawing.Size(371, 470);
            this.eventCalandarPanel.TabIndex = 17;
            this.eventCalandarPanel.Tycoon_AlwaysShowScroll = false;
            this.eventCalandarPanel.Tycoon_Border = false;
            this.eventCalandarPanel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.eventCalandarPanel.Tycoon_ScrollDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.eventCalandarPanel.Tycoon_ScrollDownTexture = "arrowdown";
            this.eventCalandarPanel.Tycoon_ScrollLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.eventCalandarPanel.Tycoon_ScrollPosition = 0;
            this.eventCalandarPanel.Tycoon_ScrollUpTexture = "arrowup";
            this.eventCalandarPanel.Tycoon_Tooltip = "";
            this.eventCalandarPanel.Tycoon_TooltipTime = 1D;
            this.eventCalandarPanel.Tycoon_Visible = true;
            // 
            // CalandarWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 483);
            this.Controls.Add(this.taskDetailsPanel);
            this.Controls.Add(this.verticelLine);
            this.Controls.Add(this.eventCalandarPanel);
            this.Name = "CalandarWindow";
            this.Text = "StorageBuildingWindow";
            this.Tycoon_TitleText = "Store Window";
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonPanel_Gen taskDetailsPanel;
        private TycoonWindowGenerationLib.TycoonLabel_Gen verticelLine;
        private Events.CalandarDatesPanel eventCalandarPanel;

    }
}