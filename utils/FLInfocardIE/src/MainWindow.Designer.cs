namespace FLInfocardIE
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
            this.components = new System.ComponentModel.Container();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.buttonExportInfocards = new System.Windows.Forms.Button();
            this.buttonSaveChanges = new System.Windows.Forms.Button();
            this.buttonImportInfocards = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFlPath = new System.Windows.Forms.TextBox();
            this.buttonScan = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridViewIDS = new System.Windows.Forms.DataGridView();
            this.richTextBoxRawCard = new System.Windows.Forms.RichTextBox();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.textBoxSearch = new System.Windows.Forms.TextBox();
            this.textBoxCardInfo = new System.Windows.Forms.TextBox();
            this.richTextBoxFormattedCard = new System.Windows.Forms.RichTextBox();
            this.descDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.idsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.typeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.infocardsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.infocards1 = new FLInfocardIE.Infocards();
            this.buttonAbout = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewIDS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.infocardsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.infocards1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(13, 13);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(451, 331);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.buttonAbout);
            this.tabPage1.Controls.Add(this.richTextBox1);
            this.tabPage1.Controls.Add(this.buttonExportInfocards);
            this.tabPage1.Controls.Add(this.buttonSaveChanges);
            this.tabPage1.Controls.Add(this.buttonImportInfocards);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.textBoxFlPath);
            this.tabPage1.Controls.Add(this.buttonScan);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(443, 305);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Commands";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(9, 83);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(428, 215);
            this.richTextBox1.TabIndex = 16;
            this.richTextBox1.Text = "";
            // 
            // buttonExportInfocards
            // 
            this.buttonExportInfocards.Location = new System.Drawing.Point(234, 55);
            this.buttonExportInfocards.Name = "buttonExportInfocards";
            this.buttonExportInfocards.Size = new System.Drawing.Size(69, 22);
            this.buttonExportInfocards.TabIndex = 15;
            this.buttonExportInfocards.Text = "Export Infocards";
            this.buttonExportInfocards.UseVisualStyleBackColor = true;
            this.buttonExportInfocards.Click += new System.EventHandler(this.buttonExportInfocards_Click);
            // 
            // buttonSaveChanges
            // 
            this.buttonSaveChanges.Location = new System.Drawing.Point(84, 55);
            this.buttonSaveChanges.Name = "buttonSaveChanges";
            this.buttonSaveChanges.Size = new System.Drawing.Size(69, 22);
            this.buttonSaveChanges.TabIndex = 14;
            this.buttonSaveChanges.Text = "Save Changes";
            this.buttonSaveChanges.UseVisualStyleBackColor = true;
            this.buttonSaveChanges.Click += new System.EventHandler(this.buttonSaveChanges_Click);
            // 
            // buttonImportInfocards
            // 
            this.buttonImportInfocards.Location = new System.Drawing.Point(159, 55);
            this.buttonImportInfocards.Name = "buttonImportInfocards";
            this.buttonImportInfocards.Size = new System.Drawing.Size(69, 22);
            this.buttonImportInfocards.TabIndex = 13;
            this.buttonImportInfocards.Text = "Import Infocards";
            this.buttonImportInfocards.UseVisualStyleBackColor = true;
            this.buttonImportInfocards.Click += new System.EventHandler(this.buttonImportInfocards_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Freelancer Path";
            // 
            // textBoxFlPath
            // 
            this.textBoxFlPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFlPath.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::FLInfocardIE.Properties.Settings.Default, "setFlDir", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBoxFlPath.Location = new System.Drawing.Point(8, 29);
            this.textBoxFlPath.Name = "textBoxFlPath";
            this.textBoxFlPath.Size = new System.Drawing.Size(429, 20);
            this.textBoxFlPath.TabIndex = 11;
            this.textBoxFlPath.Text = global::FLInfocardIE.Properties.Settings.Default.setFlDir;
            // 
            // buttonScan
            // 
            this.buttonScan.Location = new System.Drawing.Point(9, 55);
            this.buttonScan.Name = "buttonScan";
            this.buttonScan.Size = new System.Drawing.Size(69, 22);
            this.buttonScan.TabIndex = 10;
            this.buttonScan.Text = "Load";
            this.buttonScan.UseVisualStyleBackColor = true;
            this.buttonScan.Click += new System.EventHandler(this.buttonScan_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(443, 305);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Browse";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridViewIDS);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.richTextBoxRawCard);
            this.splitContainer1.Panel2.Controls.Add(this.buttonSearch);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxSearch);
            this.splitContainer1.Panel2.Controls.Add(this.textBoxCardInfo);
            this.splitContainer1.Panel2.Controls.Add(this.richTextBoxFormattedCard);
            this.splitContainer1.Size = new System.Drawing.Size(437, 302);
            this.splitContainer1.SplitterDistance = 145;
            this.splitContainer1.TabIndex = 6;
            // 
            // dataGridViewIDS
            // 
            this.dataGridViewIDS.AllowUserToAddRows = false;
            this.dataGridViewIDS.AllowUserToDeleteRows = false;
            this.dataGridViewIDS.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewIDS.AutoGenerateColumns = false;
            this.dataGridViewIDS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewIDS.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.descDataGridViewTextBoxColumn,
            this.textDataGridViewTextBoxColumn,
            this.idsDataGridViewTextBoxColumn,
            this.typeDataGridViewTextBoxColumn});
            this.dataGridViewIDS.DataSource = this.infocardsBindingSource;
            this.dataGridViewIDS.Location = new System.Drawing.Point(6, 6);
            this.dataGridViewIDS.Name = "dataGridViewIDS";
            this.dataGridViewIDS.ReadOnly = true;
            this.dataGridViewIDS.RowHeadersVisible = false;
            this.dataGridViewIDS.Size = new System.Drawing.Size(136, 291);
            this.dataGridViewIDS.TabIndex = 1;
            this.dataGridViewIDS.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewIDS_CellClick);
            // 
            // richTextBoxRawCard
            // 
            this.richTextBoxRawCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxRawCard.Location = new System.Drawing.Point(3, 60);
            this.richTextBoxRawCard.Name = "richTextBoxRawCard";
            this.richTextBoxRawCard.Size = new System.Drawing.Size(282, 95);
            this.richTextBoxRawCard.TabIndex = 10;
            this.richTextBoxRawCard.Text = "";
            // 
            // buttonSearch
            // 
            this.buttonSearch.Location = new System.Drawing.Point(210, 6);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(75, 23);
            this.buttonSearch.TabIndex = 9;
            this.buttonSearch.Text = "Search";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // textBoxSearch
            // 
            this.textBoxSearch.Location = new System.Drawing.Point(3, 8);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Size = new System.Drawing.Size(184, 20);
            this.textBoxSearch.TabIndex = 8;
            // 
            // textBoxCardInfo
            // 
            this.textBoxCardInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCardInfo.Location = new System.Drawing.Point(3, 34);
            this.textBoxCardInfo.Name = "textBoxCardInfo";
            this.textBoxCardInfo.ReadOnly = true;
            this.textBoxCardInfo.Size = new System.Drawing.Size(282, 20);
            this.textBoxCardInfo.TabIndex = 7;
            this.textBoxCardInfo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // richTextBoxFormattedCard
            // 
            this.richTextBoxFormattedCard.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxFormattedCard.Location = new System.Drawing.Point(3, 161);
            this.richTextBoxFormattedCard.Name = "richTextBoxFormattedCard";
            this.richTextBoxFormattedCard.Size = new System.Drawing.Size(282, 136);
            this.richTextBoxFormattedCard.TabIndex = 6;
            this.richTextBoxFormattedCard.Text = "";
            // 
            // descDataGridViewTextBoxColumn
            // 
            this.descDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.descDataGridViewTextBoxColumn.DataPropertyName = "desc";
            this.descDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descDataGridViewTextBoxColumn.Name = "descDataGridViewTextBoxColumn";
            this.descDataGridViewTextBoxColumn.ReadOnly = true;
            this.descDataGridViewTextBoxColumn.Width = 80;
            // 
            // textDataGridViewTextBoxColumn
            // 
            this.textDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.textDataGridViewTextBoxColumn.DataPropertyName = "text";
            this.textDataGridViewTextBoxColumn.HeaderText = "Text";
            this.textDataGridViewTextBoxColumn.Name = "textDataGridViewTextBoxColumn";
            this.textDataGridViewTextBoxColumn.ReadOnly = true;
            this.textDataGridViewTextBoxColumn.Width = 500;
            // 
            // idsDataGridViewTextBoxColumn
            // 
            this.idsDataGridViewTextBoxColumn.DataPropertyName = "ids";
            this.idsDataGridViewTextBoxColumn.HeaderText = "IDS";
            this.idsDataGridViewTextBoxColumn.Name = "idsDataGridViewTextBoxColumn";
            this.idsDataGridViewTextBoxColumn.ReadOnly = true;
            this.idsDataGridViewTextBoxColumn.Visible = false;
            // 
            // typeDataGridViewTextBoxColumn
            // 
            this.typeDataGridViewTextBoxColumn.DataPropertyName = "type";
            this.typeDataGridViewTextBoxColumn.HeaderText = "Type";
            this.typeDataGridViewTextBoxColumn.Name = "typeDataGridViewTextBoxColumn";
            this.typeDataGridViewTextBoxColumn.ReadOnly = true;
            this.typeDataGridViewTextBoxColumn.Visible = false;
            // 
            // infocardsBindingSource
            // 
            this.infocardsBindingSource.DataMember = "Infocards";
            this.infocardsBindingSource.DataSource = this.infocards1;
            // 
            // infocards1
            // 
            this.infocards1.DataSetName = "Infocards";
            this.infocards1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // buttonAbout
            // 
            this.buttonAbout.Location = new System.Drawing.Point(362, 54);
            this.buttonAbout.Name = "buttonAbout";
            this.buttonAbout.Size = new System.Drawing.Size(75, 23);
            this.buttonAbout.TabIndex = 17;
            this.buttonAbout.Text = "Help";
            this.buttonAbout.UseVisualStyleBackColor = true;
            this.buttonAbout.Click += new System.EventHandler(this.buttonAbout_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 356);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(484, 390);
            this.Name = "MainWindow";
            this.ShowIcon = false;
            this.Text = "FL Infocard Import/Export";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewIDS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.infocardsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.infocards1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button buttonExportInfocards;
        private System.Windows.Forms.Button buttonSaveChanges;
        private System.Windows.Forms.Button buttonImportInfocards;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFlPath;
        private System.Windows.Forms.Button buttonScan;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.BindingSource infocardsBindingSource;
        private Infocards infocards1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox richTextBoxRawCard;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.TextBox textBoxCardInfo;
        private System.Windows.Forms.RichTextBox richTextBoxFormattedCard;
        private System.Windows.Forms.DataGridView dataGridViewIDS;
        private System.Windows.Forms.DataGridViewTextBoxColumn descDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn textDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn idsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn typeDataGridViewTextBoxColumn;
        private System.Windows.Forms.Button buttonAbout;

    }
}

