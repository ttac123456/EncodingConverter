namespace EncodingConverter
{
    public partial class FormLog : FormBase
    {
        private readonly BindingList<string> logEntries;
        private readonly BindingSource logBindingSource;
        private readonly ConcurrentQueue<string> logQueue;
        private readonly System.Windows.Forms.Timer logFlushTimer;

        public FormLog()
        {
            InitializeComponent();

            logEntries = new BindingList<string>();
            logBindingSource = new BindingSource { DataSource = logEntries };
            logQueue = new();

            logBindingSource.DataSource = logEntries;
            logBindingSource.ListChanged += (sender, e) =>
            {
                txtLog.Text = string.Join(Environment.NewLine, logEntries);
            };

            // 100msごとにキューからまとめて書き出すタイマー
            logFlushTimer = new System.Windows.Forms.Timer();
            logFlushTimer.Interval = Constant.LogFlushTimerInterval;
            logFlushTimer.Tick += (s, e) => FlushLogQueue();
            logFlushTimer.Start();
        }

        private void FormLog_Load(object sender, EventArgs e)
        {
            // フォームのアイコンを設定
            SetFormIcon();

            cmbLogFilter.Items.AddRange(Constant.LofLevelFilters);
            cmbLogFilter.SelectedIndexChanged += (s, e) => ApplyLogFilter();
            cmbLogFilter.SelectedIndex = 0; // "ALL" をデフォルト選択

            btnClearLogFilter.Click += (s, e) => logBindingSource.RemoveFilter();
        }

        private void btnLogClear_Click(object sender, EventArgs e)
        {
            ClearLog();
        }

        public void AppendLog(string message)
        {
            logQueue.Enqueue(message);
        }

        private void FlushLogQueue()
        {
            if (logQueue.IsEmpty) return;
            var batch = new List<string>();
            while (logQueue.TryDequeue(out var msg))
            {
                batch.Add(msg);
            }
            if (batch.Count > 0)
            {
                logEntries.RaiseListChangedEvents = false;
                foreach (var msg in batch)
                {
                    logEntries.Add(msg);
                }
                logEntries.RaiseListChangedEvents = true;
                logEntries.ResetBindings();

                // 自動スクロール：最終行を表示
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToCaret();
            }
        }

        public void ClearLog()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => logEntries.Clear()));
            }
            else
            {
                logEntries.Clear();
            }
        }

        private void ApplyLogFilter()
        {
            string? selectedLevel = cmbLogFilter.SelectedItem as string;

            // フィルタリング対象を抽出
            IEnumerable<string> filteredLogs = logEntries;

            // フィルタ解除（全ログ表示）
            if (string.IsNullOrEmpty(selectedLevel) || selectedLevel == Constant.LogLevelTextAll)
            {
                txtLog.Text = string.Join(Environment.NewLine, logEntries);
                return;
            }

            // ログレベルを含む行のみ抽出
            filteredLogs = logEntries.Where(log => log.Contains($"[{selectedLevel}]"));

            // 絞り込んだログを表示
            txtLog.Text = string.Join(Environment.NewLine, filteredLogs);
        }
    }
}
