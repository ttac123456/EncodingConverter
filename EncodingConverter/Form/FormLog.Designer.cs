namespace EncodingConverter
{
    partial class FormLog
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
            txtLog = new TextBox();
            btnLogClear = new Button();
            label1 = new Label();
            cmbLogFilter = new ComboBox();
            btnClearLogFilter = new Button();
            SuspendLayout();
            // 
            // txtLog
            // 
            txtLog.Dock = DockStyle.Bottom;
            txtLog.Location = new Point(0, 30);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ScrollBars = ScrollBars.Both;
            txtLog.Size = new Size(800, 420);
            txtLog.TabIndex = 0;
            // 
            // btnLogClear
            // 
            btnLogClear.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLogClear.Location = new Point(724, 3);
            btnLogClear.Name = "btnLogClear";
            btnLogClear.Size = new Size(64, 23);
            btnLogClear.TabIndex = 1;
            btnLogClear.Text = "ログクリア";
            btnLogClear.UseVisualStyleBackColor = true;
            btnLogClear.Click += btnLogClear_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 7);
            label1.Name = "label1";
            label1.Size = new Size(59, 15);
            label1.TabIndex = 2;
            label1.Text = "ログフィルタ";
            // 
            // cmbLogFilter
            // 
            cmbLogFilter.FormattingEnabled = true;
            cmbLogFilter.Location = new Point(77, 4);
            cmbLogFilter.Name = "cmbLogFilter";
            cmbLogFilter.Size = new Size(80, 23);
            cmbLogFilter.TabIndex = 3;
            // 
            // btnClearLogFilter
            // 
            btnClearLogFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearLogFilter.Location = new Point(163, 3);
            btnClearLogFilter.Name = "btnClearLogFilter";
            btnClearLogFilter.Size = new Size(64, 23);
            btnClearLogFilter.TabIndex = 4;
            btnClearLogFilter.Text = "全て表示";
            btnClearLogFilter.UseVisualStyleBackColor = true;
            // 
            // FormLog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnClearLogFilter);
            Controls.Add(cmbLogFilter);
            Controls.Add(label1);
            Controls.Add(btnLogClear);
            Controls.Add(txtLog);
            Name = "FormLog";
            Text = "EncodingConverter - Log";
            Load += FormLog_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtLog;
        private Button btnLogClear;
        private Label label1;
        private ComboBox cmbLogFilter;
        private Button btnClearLogFilter;
    }
}