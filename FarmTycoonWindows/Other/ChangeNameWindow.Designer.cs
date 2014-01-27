namespace FarmTycoonWindowGen.Buildings
{
    partial class ChangeNameWindow
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
            this.cancelButton = new TycoonWindowGenerationLib.TycoonButton_Gen();
            this.okButton = new TycoonWindowGenerationLib.TycoonButton_Gen();
            this.nameTextbox = new TycoonWindowGenerationLib.TycoonTextbox_Gen();
            this.nameLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.cancelButton.ForeColor = System.Drawing.Color.White;
            this.cancelButton.Location = new System.Drawing.Point(103, 41);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(92, 15);
            this.cancelButton.TabIndex = 50;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Tycoon_Depressed = false;
            this.cancelButton.Tycoon_IconTexture = "";
            this.cancelButton.Tycoon_ShadowDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.cancelButton.Tycoon_ShadowLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.cancelButton.Tycoon_Tooltip = "";
            this.cancelButton.Tycoon_TooltipTime = 1D;
            this.cancelButton.Tycoon_Visible = true;
            this.cancelButton.UseVisualStyleBackColor = false;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.okButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okButton.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.okButton.ForeColor = System.Drawing.Color.White;
            this.okButton.Location = new System.Drawing.Point(5, 41);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(92, 15);
            this.okButton.TabIndex = 49;
            this.okButton.Text = "OK";
            this.okButton.Tycoon_Depressed = false;
            this.okButton.Tycoon_IconTexture = "";
            this.okButton.Tycoon_ShadowDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.okButton.Tycoon_ShadowLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.okButton.Tycoon_Tooltip = "";
            this.okButton.Tycoon_TooltipTime = 1D;
            this.okButton.Tycoon_Visible = true;
            this.okButton.UseVisualStyleBackColor = false;
            // 
            // nameTextbox
            // 
            this.nameTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.nameTextbox.BackColor = System.Drawing.Color.White;
            this.nameTextbox.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.nameTextbox.ForeColor = System.Drawing.Color.Black;
            this.nameTextbox.Location = new System.Drawing.Point(5, 20);
            this.nameTextbox.Multiline = true;
            this.nameTextbox.Name = "nameTextbox";
            this.nameTextbox.Size = new System.Drawing.Size(190, 15);
            this.nameTextbox.TabIndex = 51;
            this.nameTextbox.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.nameTextbox.Tycoon_MaxLenght = 2147483647;
            this.nameTextbox.Tycoon_NumbersOnly = false;
            this.nameTextbox.Tycoon_Tooltip = "";
            this.nameTextbox.Tycoon_TooltipTime = 1D;
            this.nameTextbox.Tycoon_Visible = true;
            // 
            // nameLabel
            // 
            this.nameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.nameLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.nameLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.nameLabel.ForeColor = System.Drawing.Color.White;
            this.nameLabel.Location = new System.Drawing.Point(5, 5);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(190, 15);
            this.nameLabel.TabIndex = 52;
            this.nameLabel.Text = "Name:";
            this.nameLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.nameLabel.Tycoon_Tooltip = "";
            this.nameLabel.Tycoon_TooltipTime = 1D;
            this.nameLabel.Tycoon_Visible = true;
            // 
            // ChangeNameWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(200, 61);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.nameTextbox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Name = "ChangeNameWindow";
            this.Text = "StorageBuildingWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonButton_Gen cancelButton;
        private TycoonWindowGenerationLib.TycoonButton_Gen okButton;
        private TycoonWindowGenerationLib.TycoonTextbox_Gen nameTextbox;
        private TycoonWindowGenerationLib.TycoonLabel_Gen nameLabel;


    }
}