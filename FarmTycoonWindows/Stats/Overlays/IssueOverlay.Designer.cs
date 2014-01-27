namespace FarmTycoonWindowGen
{
    partial class IssueOverlay
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
            this.StatusLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.XMoreLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.ActionButton = new TycoonWindowGenerationLib.TycoonButton_Gen();
            this.SuspendLayout();
            // 
            // StatusLabel
            // 
            this.StatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.StatusLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusLabel.ForeColor = System.Drawing.Color.White;
            this.StatusLabel.Location = new System.Drawing.Point(0, 0);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(550, 30);
            this.StatusLabel.TabIndex = 0;
            this.StatusLabel.Text = "Big Long Status String Goes Here\r\nLine2\r\n";
            this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.StatusLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.StatusLabel.Tycoon_Tooltip = "";
            this.StatusLabel.Tycoon_TooltipTime = 1D;
            this.StatusLabel.Tycoon_Visible = true;
            // 
            // XMoreLabel
            // 
            this.XMoreLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.XMoreLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.XMoreLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.XMoreLabel.ForeColor = System.Drawing.Color.White;
            this.XMoreLabel.Location = new System.Drawing.Point(0, 30);
            this.XMoreLabel.Name = "XMoreLabel";
            this.XMoreLabel.Size = new System.Drawing.Size(550, 15);
            this.XMoreLabel.TabIndex = 1;
            this.XMoreLabel.Text = "(X more)";
            this.XMoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.XMoreLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.XMoreLabel.Tycoon_Tooltip = "";
            this.XMoreLabel.Tycoon_TooltipTime = 1D;
            this.XMoreLabel.Tycoon_Visible = true;
            // 
            // ActionButton
            // 
            this.ActionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ActionButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ActionButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ActionButton.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.ActionButton.ForeColor = System.Drawing.Color.White;
            this.ActionButton.Location = new System.Drawing.Point(485, 30);
            this.ActionButton.Name = "ActionButton";
            this.ActionButton.Size = new System.Drawing.Size(60, 15);
            this.ActionButton.TabIndex = 3;
            this.ActionButton.Text = "Cancel";
            this.ActionButton.Tycoon_Depressed = false;
            this.ActionButton.Tycoon_IconTexture = "";
            this.ActionButton.Tycoon_ShadowDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.ActionButton.Tycoon_ShadowLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.ActionButton.Tycoon_Tooltip = "";
            this.ActionButton.Tycoon_TooltipTime = 1D;
            this.ActionButton.Tycoon_Visible = true;
            this.ActionButton.UseVisualStyleBackColor = false;
            // 
            // IssueOverlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(550, 50);
            this.Controls.Add(this.ActionButton);
            this.Controls.Add(this.XMoreLabel);
            this.Controls.Add(this.StatusLabel);
            this.Name = "IssueOverlay";
            this.Text = "SelectBuildingWindow";
            this.Tycoon_TitleBar = false;
            this.Tycoon_TitleText = "Select What To Build";
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonLabel_Gen StatusLabel;
        private TycoonWindowGenerationLib.TycoonLabel_Gen XMoreLabel;
        private TycoonWindowGenerationLib.TycoonButton_Gen ActionButton;


    }
}