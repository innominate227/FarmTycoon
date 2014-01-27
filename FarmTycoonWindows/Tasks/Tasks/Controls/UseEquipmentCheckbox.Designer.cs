namespace FarmTycoonWindowGen
{
    partial class UseEquipmentCheckbox
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
            this.useEquipmentLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.useEquipmentButton = new TycoonWindowGenerationLib.TycoonButton_Gen();
            this.SuspendLayout();
            // 
            // useEquipmentLabel
            // 
            this.useEquipmentLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.useEquipmentLabel.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.useEquipmentLabel.ForeColor = System.Drawing.Color.White;
            this.useEquipmentLabel.Location = new System.Drawing.Point(18, 0);
            this.useEquipmentLabel.Name = "useEquipmentLabel";
            this.useEquipmentLabel.Size = new System.Drawing.Size(248, 15);
            this.useEquipmentLabel.TabIndex = 52;
            this.useEquipmentLabel.Text = "Use Equipment";
            this.useEquipmentLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.useEquipmentLabel.Tycoon_Tooltip = "";
            this.useEquipmentLabel.Tycoon_TooltipTime = 1D;
            this.useEquipmentLabel.Tycoon_Visible = true;
            // 
            // useEquipmentButton
            // 
            this.useEquipmentButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.useEquipmentButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.useEquipmentButton.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.useEquipmentButton.ForeColor = System.Drawing.Color.White;
            this.useEquipmentButton.Location = new System.Drawing.Point(0, 0);
            this.useEquipmentButton.Name = "useEquipmentButton";
            this.useEquipmentButton.Size = new System.Drawing.Size(15, 15);
            this.useEquipmentButton.TabIndex = 51;
            this.useEquipmentButton.Tycoon_Depressed = false;
            this.useEquipmentButton.Tycoon_IconTexture = "";
            this.useEquipmentButton.Tycoon_ShadowDarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(32)))), ((int)(((byte)(0)))));
            this.useEquipmentButton.Tycoon_ShadowLightColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.useEquipmentButton.Tycoon_Tooltip = "";
            this.useEquipmentButton.Tycoon_TooltipTime = 1D;
            this.useEquipmentButton.Tycoon_Visible = true;
            this.useEquipmentButton.UseVisualStyleBackColor = false;
            // 
            // UseEquipmentCheckbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Controls.Add(this.useEquipmentLabel);
            this.Controls.Add(this.useEquipmentButton);
            this.Name = "UseEquipmentCheckbox";
            this.Size = new System.Drawing.Size(266, 15);
            this.Tycoon_Border = false;
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonLabel_Gen useEquipmentLabel;
        private TycoonWindowGenerationLib.TycoonButton_Gen useEquipmentButton;



    }
}
