namespace FarmTycoonWindowGen
{
    partial class BottomLeftStatus
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
            this.moneyLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.numberOfWorkersLabel = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.SuspendLayout();
            // 
            // moneyLabel
            // 
            this.moneyLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.moneyLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.moneyLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.moneyLabel.ForeColor = System.Drawing.Color.White;
            this.moneyLabel.Location = new System.Drawing.Point(0, 0);
            this.moneyLabel.Name = "moneyLabel";
            this.moneyLabel.Size = new System.Drawing.Size(150, 25);
            this.moneyLabel.TabIndex = 0;
            this.moneyLabel.Text = "$ 1000";
            this.moneyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.moneyLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.moneyLabel.Tycoon_Tooltip = "";
            this.moneyLabel.Tycoon_TooltipTime = 1D;
            this.moneyLabel.Tycoon_Visible = true;
            // 
            // numberOfWorkersLabel
            // 
            this.numberOfWorkersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numberOfWorkersLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.numberOfWorkersLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numberOfWorkersLabel.ForeColor = System.Drawing.Color.White;
            this.numberOfWorkersLabel.Location = new System.Drawing.Point(0, 25);
            this.numberOfWorkersLabel.Name = "numberOfWorkersLabel";
            this.numberOfWorkersLabel.Size = new System.Drawing.Size(150, 25);
            this.numberOfWorkersLabel.TabIndex = 1;
            this.numberOfWorkersLabel.Text = "# of Workers";
            this.numberOfWorkersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.numberOfWorkersLabel.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.numberOfWorkersLabel.Tycoon_Tooltip = "";
            this.numberOfWorkersLabel.Tycoon_TooltipTime = 1D;
            this.numberOfWorkersLabel.Tycoon_Visible = true;
            // 
            // BottomLeftStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(150, 50);
            this.Controls.Add(this.numberOfWorkersLabel);
            this.Controls.Add(this.moneyLabel);
            this.Name = "BottomLeftStatus";
            this.Text = "SelectBuildingWindow";
            this.Tycoon_Resizable = false;
            this.Tycoon_TitleBar = false;
            this.Tycoon_TitleText = "Select What To Build";
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonLabel_Gen moneyLabel;
        private TycoonWindowGenerationLib.TycoonLabel_Gen numberOfWorkersLabel;


    }
}