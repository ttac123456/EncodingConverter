namespace EncodingConverter
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            cmbEncodingTarget = new ComboBox();
            lblEncodingTarget = new Label();
            btnAddTargetFile = new Button();
            btnConvert = new Button();
            btnClear = new Button();
            statusStripFormBottom = new StatusStrip();
            toolStripStatusLabelFormMainBottom = new ToolStripStatusLabel();
            txtTargetFileListName = new TextBox();
            lblTargetFileListName = new Label();
            btnSaveTargetFileList = new Button();
            btnLoadTargetFileList = new Button();
            chkAutoLoad = new CheckBox();
            dataGridTargetFileList = new DataGridView();
            chkIncludeBOM = new CheckBox();
            rdBtnCRLF = new RadioButton();
            rdBtnLF = new RadioButton();
            rdBtnCR = new RadioButton();
            pnlNewLine = new Panel();
            chkShowLog = new CheckBox();
            txtPrjBasePath = new TextBox();
            lblPrjBasePath = new Label();
            lblPrjBaseName = new Label();
            txtPrjBaseName = new TextBox();
            statusStripFormBottom.SuspendLayout();
            ((ISupportInitialize)dataGridTargetFileList).BeginInit();
            pnlNewLine.SuspendLayout();
            SuspendLayout();
            // 
            // cmbEncodingTarget
            // 
            cmbEncodingTarget.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEncodingTarget.FormattingEnabled = true;
            cmbEncodingTarget.Location = new Point(243, 7);
            cmbEncodingTarget.Name = "cmbEncodingTarget";
            cmbEncodingTarget.Size = new Size(121, 23);
            cmbEncodingTarget.TabIndex = 0;
            // 
            // lblEncodingTarget
            // 
            lblEncodingTarget.AutoSize = true;
            lblEncodingTarget.Location = new Point(146, 10);
            lblEncodingTarget.Name = "lblEncodingTarget";
            lblEncodingTarget.Size = new Size(91, 15);
            lblEncodingTarget.TabIndex = 1;
            lblEncodingTarget.Text = "変換先文字コード";
            // 
            // btnAddTargetFile
            // 
            btnAddTargetFile.Location = new Point(12, 6);
            btnAddTargetFile.Name = "btnAddTargetFile";
            btnAddTargetFile.Size = new Size(128, 23);
            btnAddTargetFile.TabIndex = 3;
            btnAddTargetFile.Text = "変換元ファイル追加";
            btnAddTargetFile.UseVisualStyleBackColor = true;
            btnAddTargetFile.Click += btnAddTargetFile_Click;
            // 
            // btnConvert
            // 
            btnConvert.Location = new Point(602, 6);
            btnConvert.Name = "btnConvert";
            btnConvert.Size = new Size(52, 23);
            btnConvert.TabIndex = 4;
            btnConvert.Text = "変換";
            btnConvert.UseVisualStyleBackColor = true;
            btnConvert.Click += btnConvert_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(660, 6);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(52, 23);
            btnClear.TabIndex = 5;
            btnClear.Text = "クリア";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // statusStripFormBottom
            // 
            statusStripFormBottom.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelFormMainBottom });
            statusStripFormBottom.Location = new Point(0, 439);
            statusStripFormBottom.Name = "statusStripFormBottom";
            statusStripFormBottom.Size = new Size(984, 22);
            statusStripFormBottom.TabIndex = 6;
            statusStripFormBottom.Text = "statusStrip1";
            // 
            // toolStripStatusLabelFormMainBottom
            // 
            toolStripStatusLabelFormMainBottom.Name = "toolStripStatusLabelFormMainBottom";
            toolStripStatusLabelFormMainBottom.Size = new Size(205, 17);
            toolStripStatusLabelFormMainBottom.Text = "toolStripStatusLabelFormMainBottom";
            toolStripStatusLabelFormMainBottom.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtTargetFileListName
            // 
            txtTargetFileListName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTargetFileListName.Location = new Point(190, 36);
            txtTargetFileListName.Name = "txtTargetFileListName";
            txtTargetFileListName.Size = new Size(550, 23);
            txtTargetFileListName.TabIndex = 8;
            // 
            // lblTargetFileListName
            // 
            lblTargetFileListName.AutoSize = true;
            lblTargetFileListName.Location = new Point(12, 39);
            lblTargetFileListName.Name = "lblTargetFileListName";
            lblTargetFileListName.Size = new Size(172, 15);
            lblTargetFileListName.TabIndex = 1;
            lblTargetFileListName.Text = "変換元ファイルリスト保存ファイル名";
            // 
            // btnSaveTargetFileList
            // 
            btnSaveTargetFileList.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSaveTargetFileList.Location = new Point(746, 35);
            btnSaveTargetFileList.Name = "btnSaveTargetFileList";
            btnSaveTargetFileList.Size = new Size(52, 23);
            btnSaveTargetFileList.TabIndex = 9;
            btnSaveTargetFileList.Text = "保存";
            btnSaveTargetFileList.UseVisualStyleBackColor = true;
            btnSaveTargetFileList.Click += btnSaveTargetFileList_Click;
            // 
            // btnLoadTargetFileList
            // 
            btnLoadTargetFileList.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLoadTargetFileList.Location = new Point(804, 35);
            btnLoadTargetFileList.Name = "btnLoadTargetFileList";
            btnLoadTargetFileList.Size = new Size(52, 23);
            btnLoadTargetFileList.TabIndex = 10;
            btnLoadTargetFileList.Text = "読込";
            btnLoadTargetFileList.UseVisualStyleBackColor = true;
            btnLoadTargetFileList.Click += btnLoadTargetFileList_Click;
            // 
            // chkAutoLoad
            // 
            chkAutoLoad.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkAutoLoad.AutoSize = true;
            chkAutoLoad.Location = new Point(862, 38);
            chkAutoLoad.Name = "chkAutoLoad";
            chkAutoLoad.Size = new Size(110, 19);
            chkAutoLoad.TabIndex = 11;
            chkAutoLoad.Text = "起動時自動読込";
            chkAutoLoad.UseVisualStyleBackColor = true;
            // 
            // dataGridTargetFileList
            // 
            dataGridTargetFileList.AllowDrop = true;
            dataGridTargetFileList.AllowUserToAddRows = false;
            dataGridTargetFileList.AllowUserToResizeRows = false;
            dataGridTargetFileList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridTargetFileList.BackgroundColor = SystemColors.Window;
            dataGridTargetFileList.CellBorderStyle = DataGridViewCellBorderStyle.SingleVertical;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Control;
            dataGridViewCellStyle2.Font = new Font("Yu Gothic UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Control;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            dataGridTargetFileList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dataGridTargetFileList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridTargetFileList.EnableHeadersVisualStyles = false;
            dataGridTargetFileList.Location = new Point(0, 94);
            dataGridTargetFileList.MultiSelect = false;
            dataGridTargetFileList.Name = "dataGridTargetFileList";
            dataGridTargetFileList.ReadOnly = true;
            dataGridTargetFileList.RowHeadersVisible = false;
            dataGridTargetFileList.RowTemplate.Height = 22;
            dataGridTargetFileList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridTargetFileList.Size = new Size(984, 342);
            dataGridTargetFileList.TabIndex = 12;
            dataGridTargetFileList.DataBindingComplete += dataGridTargetFileList_DataBindingComplete;
            dataGridTargetFileList.DragDrop += txtTargetFileList_DragDrop;
            dataGridTargetFileList.DragEnter += txtTargetFileList_DragEnter;
            // 
            // chkIncludeBOM
            // 
            chkIncludeBOM.AutoSize = true;
            chkIncludeBOM.Location = new Point(370, 9);
            chkIncludeBOM.Name = "chkIncludeBOM";
            chkIncludeBOM.Size = new Size(53, 19);
            chkIncludeBOM.TabIndex = 13;
            chkIncludeBOM.Text = "BOM";
            chkIncludeBOM.UseVisualStyleBackColor = true;
            // 
            // rdBtnCRLF
            // 
            rdBtnCRLF.AutoSize = true;
            rdBtnCRLF.Location = new Point(3, 3);
            rdBtnCRLF.Name = "rdBtnCRLF";
            rdBtnCRLF.Size = new Size(59, 19);
            rdBtnCRLF.TabIndex = 14;
            rdBtnCRLF.TabStop = true;
            rdBtnCRLF.Text = "CR+LF";
            rdBtnCRLF.UseVisualStyleBackColor = true;
            // 
            // rdBtnLF
            // 
            rdBtnLF.AutoSize = true;
            rdBtnLF.Location = new Point(68, 3);
            rdBtnLF.Name = "rdBtnLF";
            rdBtnLF.Size = new Size(37, 19);
            rdBtnLF.TabIndex = 14;
            rdBtnLF.TabStop = true;
            rdBtnLF.Text = "LF";
            rdBtnLF.UseVisualStyleBackColor = true;
            // 
            // rdBtnCR
            // 
            rdBtnCR.AutoSize = true;
            rdBtnCR.Location = new Point(111, 3);
            rdBtnCR.Name = "rdBtnCR";
            rdBtnCR.Size = new Size(39, 19);
            rdBtnCR.TabIndex = 14;
            rdBtnCR.TabStop = true;
            rdBtnCR.Text = "CR";
            rdBtnCR.UseVisualStyleBackColor = true;
            // 
            // pnlNewLine
            // 
            pnlNewLine.Controls.Add(rdBtnCRLF);
            pnlNewLine.Controls.Add(rdBtnCR);
            pnlNewLine.Controls.Add(rdBtnLF);
            pnlNewLine.Location = new Point(429, 5);
            pnlNewLine.Name = "pnlNewLine";
            pnlNewLine.Size = new Size(154, 25);
            pnlNewLine.TabIndex = 15;
            // 
            // chkShowLog
            // 
            chkShowLog.AutoSize = true;
            chkShowLog.Location = new Point(718, 9);
            chkShowLog.Name = "chkShowLog";
            chkShowLog.Size = new Size(68, 19);
            chkShowLog.TabIndex = 16;
            chkShowLog.Text = "ログ表示";
            chkShowLog.UseVisualStyleBackColor = true;
            chkShowLog.Visible = false;
            chkShowLog.CheckedChanged += chkShowLog_CheckedChanged;
            // 
            // txtPrjBasePath
            // 
            txtPrjBasePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPrjBasePath.Location = new Point(432, 65);
            txtPrjBasePath.Name = "txtPrjBasePath";
            txtPrjBasePath.ReadOnly = true;
            txtPrjBasePath.Size = new Size(540, 23);
            txtPrjBasePath.TabIndex = 17;
            // 
            // lblPrjBasePath
            // 
            lblPrjBasePath.AutoSize = true;
            lblPrjBasePath.Location = new Point(318, 68);
            lblPrjBasePath.Name = "lblPrjBasePath";
            lblPrjBasePath.Size = new Size(105, 15);
            lblPrjBasePath.TabIndex = 18;
            lblPrjBasePath.Text = "プロジェクトベースパス";
            // 
            // lblPrjBaseName
            // 
            lblPrjBaseName.AutoSize = true;
            lblPrjBaseName.Location = new Point(12, 68);
            lblPrjBaseName.Name = "lblPrjBaseName";
            lblPrjBaseName.Size = new Size(98, 15);
            lblPrjBaseName.TabIndex = 19;
            lblPrjBaseName.Text = "プロジェクトベース名";
            // 
            // txtPrjBaseName
            // 
            txtPrjBaseName.Location = new Point(116, 65);
            txtPrjBaseName.Name = "txtPrjBaseName";
            txtPrjBaseName.Size = new Size(196, 23);
            txtPrjBaseName.TabIndex = 20;
            txtPrjBaseName.Text = "WTGyges";
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 461);
            Controls.Add(txtPrjBaseName);
            Controls.Add(lblPrjBaseName);
            Controls.Add(lblPrjBasePath);
            Controls.Add(txtPrjBasePath);
            Controls.Add(chkShowLog);
            Controls.Add(pnlNewLine);
            Controls.Add(chkIncludeBOM);
            Controls.Add(dataGridTargetFileList);
            Controls.Add(chkAutoLoad);
            Controls.Add(btnLoadTargetFileList);
            Controls.Add(btnSaveTargetFileList);
            Controls.Add(txtTargetFileListName);
            Controls.Add(statusStripFormBottom);
            Controls.Add(btnClear);
            Controls.Add(btnConvert);
            Controls.Add(btnAddTargetFile);
            Controls.Add(lblTargetFileListName);
            Controls.Add(lblEncodingTarget);
            Controls.Add(cmbEncodingTarget);
            MinimumSize = new Size(740, 200);
            Name = "FormMain";
            Text = "EncordingConverter";
            Activated += FormMain_Activated;
            FormClosing += FormMain_FormClosing;
            Load += FormMain_Load;
            LocationChanged += FormMain_LocationChanged;
            Resize += FormMain_Resize;
            statusStripFormBottom.ResumeLayout(false);
            statusStripFormBottom.PerformLayout();
            ((ISupportInitialize)dataGridTargetFileList).EndInit();
            pnlNewLine.ResumeLayout(false);
            pnlNewLine.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cmbEncodingTarget;
        private Label lblEncodingTarget;
        private Button btnAddTargetFile;
        private Button btnConvert;
        private Button btnClear;
        private StatusStrip statusStripFormBottom;
        private ToolStripStatusLabel toolStripStatusLabelFormMainBottom;
        private TextBox txtTargetFileListName;
        private Label lblTargetFileListName;
        private Button btnSaveTargetFileList;
        private Button btnLoadTargetFileList;
        private CheckBox chkAutoLoad;
        private Label label4;
        private TextBox txtAutoBuildPath;
        private CheckBox chkAutoBuild;
        private Button btnBrowsAutoBuild;
        private Button btnExecuteAutoBuild;
        private DataGridView dataGridTargetFileList;
        private CheckBox chkIncludeBOM;
        private RadioButton rdBtnCRLF;
        private RadioButton rdBtnLF;
        private RadioButton rdBtnCR;
        private Panel pnlNewLine;
        private CheckBox chkShowLog;
        private TextBox txtPrjBasePath;
        private Label lblPrjBasePath;
        private Label lblPrjBaseName;
        private TextBox txtPrjBaseName;
    }
}
