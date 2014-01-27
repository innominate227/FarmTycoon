namespace FarmTycoonWindowGen
{
    partial class DeleteWindow
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
            this.messageLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.noButton = new TycoonWindowGenerationLib.TycoonButton_Gen();
            this.yesButton = new TycoonWindowGenerationLib.TycoonButton_Gen();
            this.SuspendLayout();
            // 
            // messageLabel
            // 
            this.messageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.messageLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.messageLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.messageLabel.ForeColor = System.Drawing.Color.White;
            this.messageLabel.Location = new System.Drawing.Point(5, 5);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(390, 60);
            this.messageLabel.TabIndex = 0;
            this.messageLabel.Text = "messageLabel";
            this.messageLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.messageLabel.Tycoon_Tooltip = "";
            this.messageLabel.Tycoon_TooltipTime = 1D;
            this.messageLabel.Tycoon_Visible = true;
            // 
            // noButton
            // 
            this.noButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.noButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.noButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.noButton.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.noButton.ForeColor = System.Drawing.Color.White;
            this.noButton.Location = new System.Drawing.Point(205, 70);
            this.noButton.Name = "noButton";
            this.noButton.Size = new System.Drawing.Size(190, 15);
            this.noButton.TabIndex = 4;
            this.noButton.Text = "No";
            this.noButton.Tycoon_Depressed = false;
            this.noButton.Tycoon_IconTexture = "";
            this.noButton.Tycoon_ShadowDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.noButton.Tycoon_ShadowLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.noButton.Tycoon_Tooltip = "";
            this.noButton.Tycoon_TooltipTime = 1D;
            this.noButton.Tycoon_Visible = true;
            this.noButton.UseVisualStyleBackColor = false;
            // 
            // yesButton
            // 
            this.yesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.yesButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.yesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.yesButton.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.yesButton.ForeColor = System.Drawing.Color.White;
            this.yesButton.Location = new System.Drawing.Point(5, 70);
            this.yesButton.Name = "yesButton";
            this.yesButton.Size = new System.Drawing.Size(190, 15);
            this.yesButton.TabIndex = 3;
            this.yesButton.Text = "Yes";
            this.yesButton.Tycoon_Depressed = false;
            this.yesButton.Tycoon_IconTexture = "";
            this.yesButton.Tycoon_ShadowDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.yesButton.Tycoon_ShadowLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.yesButton.Tycoon_Tooltip = "";
            this.yesButton.Tycoon_TooltipTime = 1D;
            this.yesButton.Tycoon_Visible = true;
            this.yesButton.UseVisualStyleBackColor = false;
            // 
            // DeleteWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(400, 90);
            this.Controls.Add(this.noButton);
            this.Controls.Add(this.yesButton);
            this.Controls.Add(this.messageLabel);
            this.Name = "DeleteWindow";
            this.Tycoon_Resizable = false;
            this.Tycoon_TitleText = "";
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonLabel_Gen messageLabel;
        private TycoonWindowGenerationLib.TycoonButton_Gen noButton;
        private TycoonWindowGenerationLib.TycoonButton_Gen yesButton;


    }
}