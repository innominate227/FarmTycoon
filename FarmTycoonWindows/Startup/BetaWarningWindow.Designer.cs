namespace FarmTycoonWindowGen
{
    partial class BetaWarningWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BetaWarningWindow));
            this.tycoonLabel_Gen1 = new TycoonWindowGenerationLib.TycoonLabel_Gen();
            this.SuspendLayout();
            // 
            // tycoonLabel_Gen1
            // 
            this.tycoonLabel_Gen1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tycoonLabel_Gen1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tycoonLabel_Gen1.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.tycoonLabel_Gen1.ForeColor = System.Drawing.Color.White;
            this.tycoonLabel_Gen1.Location = new System.Drawing.Point(0, 0);
            this.tycoonLabel_Gen1.Name = "tycoonLabel_Gen1";
            this.tycoonLabel_Gen1.Size = new System.Drawing.Size(650, 54);
            this.tycoonLabel_Gen1.TabIndex = 0;
            this.tycoonLabel_Gen1.Text = resources.GetString("tycoonLabel_Gen1.Text");
            this.tycoonLabel_Gen1.Tycoon_BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tycoonLabel_Gen1.Tycoon_Tooltip = "";
            this.tycoonLabel_Gen1.Tycoon_TooltipTime = 1D;
            this.tycoonLabel_Gen1.Tycoon_Visible = true;
            // 
            // BetaWarningWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(650, 54);
            this.Controls.Add(this.tycoonLabel_Gen1);
            this.Name = "BetaWarningWindow";
            this.Tycoon_Border = false;
            this.Tycoon_Resizable = false;
            this.Tycoon_TitleBar = false;
            this.Tycoon_TitleText = "";
            this.ResumeLayout(false);

        }

        #endregion

        private TycoonWindowGenerationLib.TycoonLabel_Gen tycoonLabel_Gen1;



    }
}