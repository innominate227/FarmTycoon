namespace TycoonTextureTool
{
    partial class MainWindow
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.RefreshTexturesButton = new System.Windows.Forms.Button();
            this.TexturesListView = new System.Windows.Forms.ListView();
            this.ColumnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FilterTexturesComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SearchTexturesTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.RefreshQuartetButton = new System.Windows.Forms.Button();
            this.QuartetListView = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FilterQuartetsComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SearchQuartetsTextbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ExportButton = new System.Windows.Forms.Button();
            this.ExportProgress = new System.Windows.Forms.ProgressBar();
            this.SaveQuickCheckBox = new System.Windows.Forms.CheckBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.RefreshWindowTexturesButton = new System.Windows.Forms.Button();
            this.WindowTexturesListView = new System.Windows.Forms.ListView();
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FilterWindowTexturesComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SearchWindowTexturesTextbox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(810, 536);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.RefreshTexturesButton);
            this.tabPage1.Controls.Add(this.TexturesListView);
            this.tabPage1.Controls.Add(this.FilterTexturesComboBox);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.SearchTexturesTextbox);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(802, 510);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Textures";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // RefreshTexturesButton
            // 
            this.RefreshTexturesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RefreshTexturesButton.Location = new System.Drawing.Point(699, 3);
            this.RefreshTexturesButton.Name = "RefreshTexturesButton";
            this.RefreshTexturesButton.Size = new System.Drawing.Size(75, 23);
            this.RefreshTexturesButton.TabIndex = 5;
            this.RefreshTexturesButton.Text = "Refresh";
            this.RefreshTexturesButton.UseVisualStyleBackColor = true;
            this.RefreshTexturesButton.Click += new System.EventHandler(this.RefreshTexturesButton_Click);
            // 
            // TexturesListView
            // 
            this.TexturesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TexturesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeaderName,
            this.columnHeader1,
            this.columnHeader2});
            this.TexturesListView.FullRowSelect = true;
            this.TexturesListView.Location = new System.Drawing.Point(9, 29);
            this.TexturesListView.MultiSelect = false;
            this.TexturesListView.Name = "TexturesListView";
            this.TexturesListView.Size = new System.Drawing.Size(765, 475);
            this.TexturesListView.TabIndex = 4;
            this.TexturesListView.UseCompatibleStateImageBehavior = false;
            this.TexturesListView.View = System.Windows.Forms.View.Details;
            this.TexturesListView.SelectedIndexChanged += new System.EventHandler(this.TexturesListView_SelectedIndexChanged);
            // 
            // ColumnHeaderName
            // 
            this.ColumnHeaderName.Text = "Name";
            this.ColumnHeaderName.Width = 322;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Catagory";
            this.columnHeader1.Width = 305;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Size";
            this.columnHeader2.Width = 118;
            // 
            // FilterTexturesComboBox
            // 
            this.FilterTexturesComboBox.FormattingEnabled = true;
            this.FilterTexturesComboBox.Location = new System.Drawing.Point(427, 3);
            this.FilterTexturesComboBox.Name = "FilterTexturesComboBox";
            this.FilterTexturesComboBox.Size = new System.Drawing.Size(215, 21);
            this.FilterTexturesComboBox.TabIndex = 3;
            this.FilterTexturesComboBox.SelectedIndexChanged += new System.EventHandler(this.FilterTexturesComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(389, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Filter:";
            // 
            // SearchTexturesTextbox
            // 
            this.SearchTexturesTextbox.Location = new System.Drawing.Point(56, 3);
            this.SearchTexturesTextbox.Name = "SearchTexturesTextbox";
            this.SearchTexturesTextbox.Size = new System.Drawing.Size(318, 20);
            this.SearchTexturesTextbox.TabIndex = 1;
            this.SearchTexturesTextbox.TextChanged += new System.EventHandler(this.SearchTexturesTextbox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Search:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.RefreshQuartetButton);
            this.tabPage2.Controls.Add(this.QuartetListView);
            this.tabPage2.Controls.Add(this.FilterQuartetsComboBox);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.SearchQuartetsTextbox);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(802, 510);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Quartets";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // RefreshQuartetButton
            // 
            this.RefreshQuartetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RefreshQuartetButton.Location = new System.Drawing.Point(721, 3);
            this.RefreshQuartetButton.Name = "RefreshQuartetButton";
            this.RefreshQuartetButton.Size = new System.Drawing.Size(75, 23);
            this.RefreshQuartetButton.TabIndex = 9;
            this.RefreshQuartetButton.Text = "Refresh";
            this.RefreshQuartetButton.UseVisualStyleBackColor = true;
            this.RefreshQuartetButton.Click += new System.EventHandler(this.RefreshQuartetButton_Click);
            // 
            // QuartetListView
            // 
            this.QuartetListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.QuartetListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
            this.QuartetListView.FullRowSelect = true;
            this.QuartetListView.Location = new System.Drawing.Point(9, 29);
            this.QuartetListView.MultiSelect = false;
            this.QuartetListView.Name = "QuartetListView";
            this.QuartetListView.Size = new System.Drawing.Size(787, 475);
            this.QuartetListView.TabIndex = 8;
            this.QuartetListView.UseCompatibleStateImageBehavior = false;
            this.QuartetListView.View = System.Windows.Forms.View.Details;
            this.QuartetListView.SelectedIndexChanged += new System.EventHandler(this.QuartetListView_SelectedIndexChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Name";
            this.columnHeader3.Width = 263;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Catagory";
            this.columnHeader4.Width = 210;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "North";
            this.columnHeader5.Width = 72;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "East";
            this.columnHeader6.Width = 72;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "South";
            this.columnHeader7.Width = 72;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "West";
            this.columnHeader8.Width = 72;
            // 
            // FilterQuartetsComboBox
            // 
            this.FilterQuartetsComboBox.FormattingEnabled = true;
            this.FilterQuartetsComboBox.Location = new System.Drawing.Point(427, 3);
            this.FilterQuartetsComboBox.Name = "FilterQuartetsComboBox";
            this.FilterQuartetsComboBox.Size = new System.Drawing.Size(215, 21);
            this.FilterQuartetsComboBox.TabIndex = 7;
            this.FilterQuartetsComboBox.SelectedIndexChanged += new System.EventHandler(this.FilterQuartetsComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(389, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Filter:";
            // 
            // SearchQuartetsTextbox
            // 
            this.SearchQuartetsTextbox.Location = new System.Drawing.Point(56, 3);
            this.SearchQuartetsTextbox.Name = "SearchQuartetsTextbox";
            this.SearchQuartetsTextbox.Size = new System.Drawing.Size(318, 20);
            this.SearchQuartetsTextbox.TabIndex = 5;
            this.SearchQuartetsTextbox.TextChanged += new System.EventHandler(this.SearchQuartetsTextbox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Search:";
            // 
            // ExportButton
            // 
            this.ExportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ExportButton.Location = new System.Drawing.Point(12, 554);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(93, 23);
            this.ExportButton.TabIndex = 1;
            this.ExportButton.Text = "Save And Close";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // ExportProgress
            // 
            this.ExportProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ExportProgress.Location = new System.Drawing.Point(172, 554);
            this.ExportProgress.Name = "ExportProgress";
            this.ExportProgress.Size = new System.Drawing.Size(650, 23);
            this.ExportProgress.TabIndex = 2;
            // 
            // SaveQuickCheckBox
            // 
            this.SaveQuickCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SaveQuickCheckBox.AutoSize = true;
            this.SaveQuickCheckBox.Checked = true;
            this.SaveQuickCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SaveQuickCheckBox.Location = new System.Drawing.Point(112, 559);
            this.SaveQuickCheckBox.Name = "SaveQuickCheckBox";
            this.SaveQuickCheckBox.Size = new System.Drawing.Size(54, 17);
            this.SaveQuickCheckBox.TabIndex = 3;
            this.SaveQuickCheckBox.Text = "Quick";
            this.SaveQuickCheckBox.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.RefreshWindowTexturesButton);
            this.tabPage3.Controls.Add(this.WindowTexturesListView);
            this.tabPage3.Controls.Add(this.FilterWindowTexturesComboBox);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.SearchWindowTexturesTextbox);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(802, 510);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Window Textures";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // RefreshWindowTexturesButton
            // 
            this.RefreshWindowTexturesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RefreshWindowTexturesButton.Location = new System.Drawing.Point(699, 3);
            this.RefreshWindowTexturesButton.Name = "RefreshWindowTexturesButton";
            this.RefreshWindowTexturesButton.Size = new System.Drawing.Size(75, 23);
            this.RefreshWindowTexturesButton.TabIndex = 11;
            this.RefreshWindowTexturesButton.Text = "Refresh";
            this.RefreshWindowTexturesButton.UseVisualStyleBackColor = true;
            this.RefreshWindowTexturesButton.Click += new System.EventHandler(this.RefreshWindowTexturesButton_Click);
            // 
            // WindowTexturesListView
            // 
            this.WindowTexturesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WindowTexturesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11});
            this.WindowTexturesListView.FullRowSelect = true;
            this.WindowTexturesListView.Location = new System.Drawing.Point(9, 29);
            this.WindowTexturesListView.MultiSelect = false;
            this.WindowTexturesListView.Name = "WindowTexturesListView";
            this.WindowTexturesListView.Size = new System.Drawing.Size(765, 475);
            this.WindowTexturesListView.TabIndex = 10;
            this.WindowTexturesListView.UseCompatibleStateImageBehavior = false;
            this.WindowTexturesListView.View = System.Windows.Forms.View.Details;
            this.WindowTexturesListView.SelectedIndexChanged += new System.EventHandler(this.WindowTexturesListView_SelectedIndexChanged);
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Name";
            this.columnHeader9.Width = 322;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Catagory";
            this.columnHeader10.Width = 305;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Size";
            this.columnHeader11.Width = 118;
            // 
            // FilterWindowTexturesComboBox
            // 
            this.FilterWindowTexturesComboBox.FormattingEnabled = true;
            this.FilterWindowTexturesComboBox.Location = new System.Drawing.Point(427, 3);
            this.FilterWindowTexturesComboBox.Name = "FilterWindowTexturesComboBox";
            this.FilterWindowTexturesComboBox.Size = new System.Drawing.Size(215, 21);
            this.FilterWindowTexturesComboBox.TabIndex = 9;
            this.FilterWindowTexturesComboBox.SelectedIndexChanged += new System.EventHandler(this.FilterWindowTexturesComboBox_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(389, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Filter:";
            // 
            // SearchWindowTexturesTextbox
            // 
            this.SearchWindowTexturesTextbox.Location = new System.Drawing.Point(56, 3);
            this.SearchWindowTexturesTextbox.Name = "SearchWindowTexturesTextbox";
            this.SearchWindowTexturesTextbox.Size = new System.Drawing.Size(318, 20);
            this.SearchWindowTexturesTextbox.TabIndex = 7;
            this.SearchWindowTexturesTextbox.TextChanged += new System.EventHandler(this.SearchWindowTexturesTextbox_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Search:";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 589);
            this.Controls.Add(this.SaveQuickCheckBox);
            this.Controls.Add(this.ExportProgress);
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainWindow";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ComboBox FilterTexturesComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox SearchTexturesTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox FilterQuartetsComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox SearchQuartetsTextbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.ProgressBar ExportProgress;
        private System.Windows.Forms.ListView TexturesListView;
        private System.Windows.Forms.ColumnHeader ColumnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ListView QuartetListView;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.Button RefreshTexturesButton;
        private System.Windows.Forms.Button RefreshQuartetButton;
        private System.Windows.Forms.CheckBox SaveQuickCheckBox;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button RefreshWindowTexturesButton;
        private System.Windows.Forms.ListView WindowTexturesListView;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ComboBox FilterWindowTexturesComboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox SearchWindowTexturesTextbox;
        private System.Windows.Forms.Label label6;
    }
}

