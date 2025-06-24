namespace EncodingConverter
{
    /// <summary>
    /// 変換元ファイルの読み込み／UI構成／ユーザー設定の復元などを行うフォームクラス。
    /// GUIアプリケーションの中心機能を持つ。
    /// </summary>
    public partial class FormMain : FormBase
    {
        /// <summary>文字コード変換処理を行うロジック本体。</summary>
        private EncodingConverter converter;

        /// <summary>変換対象のファイル情報を保持するテーブル。</summary>
        private DataTable targetFilePathTable;

        /// <summary>DataTable を操作・永続化するファイル一覧管理クラス。</summary>
        private readonly TargetFileManager targetFileManager;

        /// <summary>ログ出力先を GUI に向けるロガー。</summary>
        private readonly GuiLogger logger;

        /// <summary>ログ表示用のサブフォーム。</summary>
        private readonly FormLog formLog = new FormLog();

        /// <summary>ToolStrip内のログ表示切替チェックボックス。</summary>
        private CheckBox chkLogVisible;

        /// <summary>ToolStrip内のプログレスバー。</summary>
        private ToolStripProgressBar toolStripProgressBar;

        /// <summary>文字コード変換の成功件数。</summary>
        private int successCount = 0;

        /// <summary>文字コード変換の失敗件数。</summary>
        private int failCount = 0;

        /// <summary>No列の値ごとに行をまとめたインデックス。</summary>
        private Dictionary<int, List<DataRow>> indexTargetNo;

        /// <summary>FilePath列の値ごとに行をまとめたインデックス。</summary>
        private Dictionary<string, List<DataRow>> indexTargetFilePath;

        /// <summary>Status列の値ごとに行をまとめたインデックス。</summary>
        private Dictionary<string, List<DataRow>> indexTargetStatus;

        private string appBasePath = "";

        private System.Windows.Forms.Timer rowBackColorTimer;

        /// <summary> ファイルごとの変換ステータス。 </summary>
        public enum FileConversionStatus
        {
            未処理,
            成功,
            失敗,
            スキップ,
            実行中
        }

        /// <summary> ステータスごとの背景色を定義。 </summary>
        private static readonly Dictionary<FileConversionStatus, Color> StatusColorMap = new()
        {
            [FileConversionStatus.未処理] = Color.White,
            [FileConversionStatus.成功] = Color.LightGreen,
            [FileConversionStatus.失敗] = Color.LightCoral,
            [FileConversionStatus.スキップ] = Color.LightYellow,
            [FileConversionStatus.実行中] = Color.LightSkyBlue
        };

        /// <summary>
        /// FormMain の初期化処理。コントロールのセットアップや状態復元を行う。
        /// </summary>
        public FormMain()
        {
            InitializeComponent();

            //logger = new GuiLogger(txtBox, appBasePath, AppConfig.Instance.IsOutputLogFile) { Level = LogLevel.Error };
            //logger = new GuiLogger(formLog, appBasePath, AppConfig.Instance.IsOutputLogFile) { Level = LogLevel.Warn };
            logger = new GuiLogger(formLog, appBasePath, AppConfig.Instance.IsOutputLogFile) { Level = LogLevel.Info };
            //logger = new GuiLogger(formLog, appBasePath, AppConfig.Instance.IsOutputLogFile) { Level = LogLevel.Debug };
            //logger = new GuiLogger(formLog, appBasePath, AppConfig.Instance.IsOutputLogFile) { Level = LogLevel.None };      // ログ出力するとものすごく重いためNoneにしておく

            converter = new EncodingConverter(logger, txtPrjBasePath.Text.Trim());

            targetFilePathTable = new DataTable();
            InitializeTargetFilePathTableColumns();

            targetFileManager = new TargetFileManager(targetFilePathTable);
        }

        private void SeekAndSetAppBasePath()
        {
            //string appBasePathTmp = string.Empty;
            //// アプリケーションの起動パスを取得
            //// デバッグ実行時はプロジェクトルート直下のbinフォルダを指すように調整
            //// 通常実行時は実際のexe配置ディレクトリをそのまま表示
            //if (Debugger.IsAttached)
            //{
            //    // Visual Studio等のIDEからデバッグ実行中
            //    appBasePathTmp = Path.GetFullPath(Path.Combine(Application.StartupPath, Constant.DebugRunAppPathOffset));
            //}
            //else
            //{
            //    // exeを直接ダブルクリック等で起動
            //    appBasePathTmp = Path.GetFullPath(Application.StartupPath);
            //}
            //appBasePath = appBasePathTmp;

            // アプリケーションの起動パスを取得 (アプリケーション設定マネージャが設定ファイルを読み込んだ際のファイル格納先ディレクトリをアプリケーションのベースパスとする)
            appBasePath = AppConfig.Instance.AppBasePath;
        }

        private void SeekAndSetPrjBasePath()
        {
            // プロジェクトベース名の初期値を設定
            string prjBasePath = string.Empty; // 初期状態では空にしておく
            var prjBaseName = txtPrjBaseName.Text.Trim();
            var seekPrjBasePath = Path.GetFullPath(appBasePath);
            while ((seekPrjBasePath != null) && (prjBaseName != seekPrjBasePath))
            {
                seekPrjBasePath = Path.GetDirectoryName(seekPrjBasePath);
                if (seekPrjBasePath == null || seekPrjBasePath == string.Empty)
                    break; // ルートディレクトリに到達したら終了
                else if (prjBaseName == Path.GetFileName(seekPrjBasePath))
                {
                    prjBasePath = seekPrjBasePath;
                    break;
                }
            }
            if (prjBasePath == string.Empty)
            {
                // プロジェクトベース名が見つからなかった場合
                ShowMessageRootDirectorySearchFailed();
            }
            txtPrjBasePath.Text = prjBasePath;
        }

        private void ShowMessageRootDirectorySearchFailed()
        {
            MessageBox.Show(
                string.Format(Constant.RootDirectorySearchFailedMessage, txtPrjBaseName.Text, txtPrjBaseName.Text),
                Constant.SearchFailedTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }

        private void SetConverterParamPjhBasePath()
        {
            // プロジェクトベース名をEncodingConverterに再設定
            var prjBasePath = txtPrjBasePath.Text.Trim();
            converter.SetPrjBasePath(prjBasePath);
        }


        /// <summary>
        /// フォームの初期表示時にUI状態／ユーザー設定を復元する。
        /// </summary>
        private void FormMain_Load(object sender, EventArgs e)
        {
            // フォームのアイコンを設定
            SetFormIcon();

            // アプリケーションの起動パスを取得
            SeekAndSetAppBasePath();

            // プロジェクト名を取得
            txtPrjBaseName.Text = AppConfig.Instance.ProjectName;

            // プロジェクトベース名を検索して設定
            SeekAndSetPrjBasePath();

            // プロジェクトベース名をEncodingConverterに再設定
            SetConverterParamPjhBasePath();

            // 変換先エンコーディングの選択肢を初期化
            cmbEncodingTarget.Items.AddRange(Constant.TargetEncords);
            cmbEncodingTarget.SelectedIndex = 0;

            // 変換・クリア・保存ボタンを初期状態で無効化
            btnConvert.Enabled = false;
            btnClear.Enabled = false;
            btnSaveTargetFileList.Enabled = false;

            // ファイル一覧グリッドの初期設定
            dataGridTargetFileList.DataSource = targetFilePathTable;
            dataGridTargetFileList.Columns["No"].HeaderText = "No";
            dataGridTargetFileList.Columns["FilePath"].HeaderText = "変換元ファイルパス";
            dataGridTargetFileList.Columns["Status"].HeaderText = "ステータス";
            dataGridTargetFileList.Columns["No"].Width = 40;
            dataGridTargetFileList.Columns["FilePath"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridTargetFileList.Columns["Status"].Width = 80;
            // 行の高さをフォントに合わせて自動調整
            dataGridTargetFileList.RowTemplate.Height =
                TextRenderer.MeasureText("あ", dataGridTargetFileList.DefaultCellStyle.Font).Height;

            // 1. チェックボックスの作成
            chkLogVisible = new CheckBox
            {
                Text = "ログ表示",
                AutoSize = true,
                Visible = true,     // 初期状態から表示
                Checked = false     // 初期状態
            };

            chkLogVisible.CheckedChanged += (s, e) =>
            {
                if (chkLogVisible.Checked)
                    ShowFormLog();
                else
                    formLog.Hide();
            };

            // 2. チェックボックスをToolStripControlHost にラップ
            var chkLogHost = new ToolStripControlHost(chkLogVisible)
            {
                Margin = new Padding(5, 2, 10, 0)
            };

            // 3. 文字コード変換プログレスバーを作成
            toolStripProgressBar = new ToolStripProgressBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Visible = true,     // 初期状態から表示
                Width = 100         // 必要に応じて幅を調整
            };

            // 4. ToolStripStatusLabel を追加して空白を埋める
            var springLabel = new ToolStripStatusLabel
            {
                Spring = true // 空白を埋めるために Spring を有効化
            };

            // 5. StatusStrip に要素を追加
            statusStripFormBottom.Items.Add(springLabel);           // 空白を追加
            statusStripFormBottom.Items.Add(toolStripProgressBar);  // プログレスバーを右寄せで追加
            statusStripFormBottom.Items.Add(chkLogHost);            // チェックボックスを右寄せで追加

            // ✅ 他ラベルがある場合は「伸びる」項目に .Spring = true を設定
            toolStripStatusLabelFormMainBottom.Spring = true;
            toolStripStatusLabelFormMainBottom.Alignment = ToolStripItemAlignment.Left;

            // フォーム下部のステータスバーを初期化
            toolStripStatusLabelFormMainBottom.Text = string.Empty;

            // ユーザー設定の復元
            RestoreUserSettings();

            // 画面外に表示されないようチェック
            var screens = Screen.AllScreens.Select(s => s.WorkingArea).ToList();
            bool isInVisibleArea = screens.Any(area => area.Contains(Properties.Settings.Default.LastLocation));
            this.StartPosition = isInVisibleArea
                ? FormStartPosition.Manual
                : FormStartPosition.WindowsDefaultLocation;
            if (isInVisibleArea)
            {
                this.Location = Properties.Settings.Default.LastLocation;
            }

            if (Properties.Settings.Default.LastSize.Width > 0 && Properties.Settings.Default.LastSize.Height > 0)
            {
                this.Size = Properties.Settings.Default.LastSize;
            }

            this.WindowState = Properties.Settings.Default.LastWindowState;

            // 以下に自動実行などの処理コードを記載。

            if (chkAutoLoad.Checked && !string.IsNullOrEmpty(txtTargetFileListName.Text))
            {
                var targetFileListPath = Path.Combine(appBasePath, txtTargetFileListName.Text.Trim());
                targetFileManager.LoadTargetFileList(txtPrjBasePath.Text, targetFileListPath);
                MakeIndexTargetFilePathTable();
                UpdateButtonStates();
                toolStripStatusLabelFormMainBottom.Text = string.Format(Constant.LoadCompleteMessageAtAppStartup, dataGridTargetFileList.RowCount);
            }

            logger.LogInfo("Gui起動");
        }

        /// <summary>
        /// フォーム終了時にユーザー設定（レイアウト・変換条件など）を保存する。
        /// </summary>
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            logger.LogInfo("Gui終了");

            // ユーザー設定を保存
            StoreUserSettings();

            // ウィンドウ状態に応じてサイズ・位置を保存
            if (this.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.LastSize = this.Size;
                Properties.Settings.Default.LastLocation = this.Location;
            }
            else
            {
                Properties.Settings.Default.LastSize = this.RestoreBounds.Size;
                Properties.Settings.Default.LastLocation = this.RestoreBounds.Location;
            }

            Properties.Settings.Default.LastWindowState = this.WindowState;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// ファイル選択ダイアログから変換対象ファイルを追加する。
        /// </summary>
        private void btnAddTargetFile_Click(object sender, EventArgs e)
        {
            string[] itemFilters = Constant.FileDialogFilters;

            using var dlg = new OpenFileDialog()
            {
                Filter = string.Join("|", itemFilters),
                Multiselect = true
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtTargetFileListName.Text = Path.GetFileName(dlg.FileName);
                int added = targetFileManager.AppendTargetFileList(txtPrjBasePath.Text.Trim(), dlg.FileNames);
                MakeIndexTargetFilePathTable();
                toolStripStatusLabelFormMainBottom.Text = string.Format(Constant.AddedFilesMessage, added);
            }
            UpdateButtonStates();
        }

        /// <summary>
        /// ファイル一覧テーブルの各列でインデックスを作成する。
        /// </summary>
        private void MakeIndexTargetFilePathTable()
        {
            // "No" のインデックス作成
            indexTargetNo = targetFilePathTable.AsEnumerable()
                .GroupBy(row => row.Field<int>("No"))
                .ToDictionary(g => g.Key, g => g.ToList());

            // "FilePath" のインデックス作成
            indexTargetFilePath = targetFilePathTable.AsEnumerable()
                .GroupBy(row => row.Field<string>("FilePath") ?? string.Empty)  // null を空文字列に置き換え
                .ToDictionary(g => g.Key, g => g.ToList());

            // "Status" のインデックス作成
            indexTargetStatus = targetFilePathTable.AsEnumerable()
                .GroupBy(row => row.Field<string>("Status") ?? string.Empty)    // null を空文字列に置き換え
                .ToDictionary(g => g.Key, g => g.ToList());
        }
        /// <summary>
        /// [変換] ボタンクリック時、同期版の変換を実行。
        /// </summary>
        private async void btnConvert_Click(object sender, EventArgs e)
        {
            // FormLogのログをクリア
            formLog.ClearLog();

            // 変換処理の準備
            InitTargetFilePathTable();

            // UI を変換中の状態に更新
            this.Invoke(() =>
            {
                toolStripStatusLabelFormMainBottom.Text = Constant.ConversionStartMessage;
                UpdateButtonStates(false); // ボタンを無効化
            });

            // 非同期タスクをバックグラウンドで実行
            _ = Task.Run(async () =>
            {
                try
                {
                    logger?.StartConversionLog();

                    StartRowBackColorTimer(); // 行の背景色更新タイマーを開始

                    bool useSync = false;
                    //bool useSync = true;

                    if (useSync)
                    {
                        // 非同期版で変換処理を実行
                        await RunAllConversionsAsync();

                        // 処理完了後のメッセージ
                        this.Invoke(() =>
                        {
                            toolStripStatusLabelFormMainBottom.Text = Constant.ConversionCompleteMessage;
                        });
                    }
                    else
                    {
                        // 同期版で変換処理を実行
                        ExecuteEncodingForAll();
                    }
                }
                catch (Exception ex)
                {
                    // エラー発生時の処理
                    this.Invoke(() =>
                    {
                        toolStripStatusLabelFormMainBottom.Text = Constant.ConversionErrorMessage;
                        logger?.LogError(string.Format(Constant.ConversionErrorLogMessage, ex.Message));
                    });
                }
                finally
                {
                    logger?.StopConversionLog();

                    StopRowBackColorTimer(); // 行の背景色更新タイマーを停止

                    // UI をアイドル状態に戻す
                    this.Invoke(() =>
                    {
                        UpdateButtonStates(true); // ボタンを有効化
                    });
                }
            });
        }

        private void StartRowBackColorTimer()
        {
            if (rowBackColorTimer == null)
            {
                rowBackColorTimer = new System.Windows.Forms.Timer();
                rowBackColorTimer.Interval = Constant.RowBackColorTimerInterval;
                rowBackColorTimer.Tick += (s, e) => UpdateAllRowBackColor();
            }
            rowBackColorTimer.Start();
        }

        private void StopRowBackColorTimer()
        {
            rowBackColorTimer?.Stop();
        }

        /// <summary>
        /// [クリア] ボタン押下時、一覧をすべて初期化する。
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            if (targetFilePathTable.Rows.Count > 0)
            {
                targetFilePathTable.Clear();
                toolStripStatusLabelFormMainBottom.Text = Constant.ClearMessage;
                MakeIndexTargetFilePathTable();
                UpdateStatusSummary();    // ← 件数サマリを再反映！
            }
            UpdateButtonStates();
        }

        /// <summary>
        /// テキストボックスへのドラッグ開始時の挙動を定義。
        /// </summary>
        private void txtTargetFileList_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        /// <summary>
        /// テキストボックスへのファイルドロップ時の処理。
        /// </summary>
        private void txtTargetFileList_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
                int added = targetFileManager.AppendTargetFileList(txtPrjBasePath.Text.Trim(), droppedFiles);
                MakeIndexTargetFilePathTable();
                toolStripStatusLabelFormMainBottom.Text = string.Format(Constant.AddedFilesMessage, added);
                UpdateButtonStates();
            }
        }

        /// <summary>
        /// グリッドへのドラッグ受付時の処理。
        /// </summary>
        private void dataGridFiles_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        /// <summary>
        /// グリッドへのファイルドロップ時の処理。
        /// </summary>
        private void dataGridFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            int added = targetFileManager.AppendTargetFileList(txtPrjBasePath.Text.Trim(), droppedFiles);
            MakeIndexTargetFilePathTable();
            toolStripStatusLabelFormMainBottom.Text = string.Format(Constant.AddedFilesMessage, added);
        }

        /// <summary>
        /// [保存] ボタンクリック時の処理。ファイルリストをCSV形式で保存。
        /// </summary>
        private void btnSaveTargetFileList_Click(object sender, EventArgs e)
        {
            string prjBasePath = txtPrjBasePath.Text.Trim();
            string targetFileListName = txtTargetFileListName.Text.Trim();

            using var saveDialog = new SaveFileDialog()
            {
                Title = Constant.SaveFileDialogTitle,
                Filter = Constant.SaveFileDialogFilter,
                InitialDirectory = appBasePath,
                FileName = Path.Combine(appBasePath, targetFileListName)
            };

            while (true)
            {
                if (saveDialog.ShowDialog() != DialogResult.OK)
                    break; // キャンセルされたら抜ける

                string selected = saveDialog.FileName;
                if (!selected.StartsWith(appBasePath, StringComparison.OrdinalIgnoreCase))
                {
                    var result = MessageBox.Show(
                        string.Format(Constant.InvalidFolderMessage, appBasePath),
                        Constant.WarningTitle,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );
                    if (result == DialogResult.Yes)
                        continue; // 再度ダイアログを表示
                    else
                        break;    // 終了
                }
                else
                {
                    txtTargetFileListName.Text = Path.GetFileName(selected);
                    targetFileManager.SaveTargetFileList(prjBasePath, selected);
                    toolStripStatusLabelFormMainBottom.Text = string.Format(Constant.SaveCompleteMessage, dataGridTargetFileList.RowCount);

                    MessageBox.Show(
                        Constant.SaveSuccessMessage,
                        Constant.SaveCompleteTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    break;
                }
            }
        }

        /// <summary>
        /// [読込] ボタンクリック時の処理。CSV形式のファイルリストを読み込む。
        /// </summary>
        private void btnLoadTargetFileList_Click(object sender, EventArgs e)
        {
            string prjBasePath = txtPrjBasePath.Text.Trim();
            string targetFileListName = txtTargetFileListName.Text.Trim();

            using var openDialog = new OpenFileDialog()
            {
                Title = Constant.OpenFileDialogTitle,
                Filter = Constant.OpenFileDialogFilter,
                InitialDirectory = appBasePath,
                FileName = Path.Combine(appBasePath, targetFileListName)
            };

            while (true)
            {
                if (openDialog.ShowDialog() != DialogResult.OK)
                    break; // キャンセルされたら抜ける

                string selected = openDialog.FileName;
                if (!selected.StartsWith(appBasePath, StringComparison.OrdinalIgnoreCase))
                {
                    var result = MessageBox.Show(
                        string.Format(Constant.InvalidFolderMessage, appBasePath),
                        Constant.WarningTitle,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );
                    if (result == DialogResult.Yes)
                        continue; // 再度ダイアログを表示
                    else
                        break;    // 終了
                }
                else
                {
                    txtTargetFileListName.Text = Path.GetFileName(selected);
                    targetFileManager.LoadTargetFileList(prjBasePath, selected);
                    toolStripStatusLabelFormMainBottom.Text = string.Format(Constant.LoadCompleteMessage, dataGridTargetFileList.RowCount);
                    MakeIndexTargetFilePathTable();
                    UpdateButtonStates();
                    break;
                }
            }
        }

        /// <summary>
        /// DataGridViewのデータバインディング完了時にインデックス再構築とボタン状態更新を行う。
        /// </summary>
        private void dataGridTargetFileList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            MakeIndexTargetFilePathTable();
            UpdateButtonStates();
        }

        /// <summary>
        /// ログ表示チェックボックスの状態変更時の処理（未使用）。
        /// </summary>
        private void chkShowLog_CheckedChanged(object sender, EventArgs e)
        {
            //if (chkLogDocked.Checked != chkShowLog.Checked)
            //    chkLogDocked.Checked = chkShowLog.Checked;

            //if (chkShowLog.Checked)
            //    ShowFormLog();
            //else
            //    formLog.Hide();
        }

        /// <summary>
        /// メインフォームの位置変更時、ログフォームの位置も追従させる。
        /// </summary>
        private void FormMain_LocationChanged(object sender, EventArgs e)
        {
            if (formLog.Visible)
            {
                PositionLogFormBelowMain();
            }
        }

        /// <summary>
        /// メインフォームのリサイズ時、ログフォームの位置も追従させる。
        /// </summary>
        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (formLog.Visible)
            {
                PositionLogFormBelowMain();
            }
        }

        /// <summary>
        /// ログフォームをメインフォームの下に配置する。
        /// </summary>
        private void PositionLogFormBelowMain()
        {
            // MainForm の画面左下座標を取得
            var screen = Screen.FromControl(this);
            Point newLocation = new Point(this.Left, this.Bottom);

            // LogForm が画面下にはみ出さないよう補正
            if (newLocation.Y + formLog.Height > screen.WorkingArea.Bottom)
            {
                newLocation.Y = screen.WorkingArea.Bottom - formLog.Height;
            }

            formLog.Location = newLocation;
        }

        /// <summary>
        /// メインフォームがアクティブ化された際、ログフォームのZ順も調整する。
        /// </summary>
        private void FormMain_Activated(object sender, EventArgs e)
        {
            if (chkShowLog.Checked && formLog?.Visible == true)
            {
                formLog.BringToFront();  // BringToFront() だけで OK（Owner 設定済）
                this.Activate();         // Z順を「Main → Log」に再調整
            }
        }
        /// <summary>
        /// ファイル一覧テーブルの列構成を定義する。
        /// </summary>
        private void InitializeTargetFilePathTableColumns()
        {
            targetFilePathTable.Columns.Add("No", typeof(int));
            targetFilePathTable.Columns.Add("FilePath", typeof(string));
            targetFilePathTable.Columns.Add("Status", typeof(string));
        }

        /// <summary>
        /// 現在のUI状態をユーザー設定として保存する。
        /// </summary>
        private void StoreUserSettings()
        {
            // ユーザー設定を保存
            Properties.Settings.Default.LastTargetFileListPath = txtTargetFileListName.Text;
            Properties.Settings.Default.LastAutoLoad = chkAutoLoad.Checked;
            Properties.Settings.Default.LastEncodingTarget = cmbEncodingTarget.SelectedIndex;
            Properties.Settings.Default.LastIncludeBOM = chkIncludeBOM.Checked;
            Properties.Settings.Default.LastLineEndingCRLF = rdBtnCRLF.Checked;
            Properties.Settings.Default.LastLineEndingLF = rdBtnLF.Checked;
            Properties.Settings.Default.LastLineEndingCR = rdBtnCR.Checked;
            Properties.Settings.Default.LastLogVisible = chkLogVisible.Checked;
        }

        /// <summary>
        /// ユーザー設定からUI状態を復元する。
        /// </summary>
        private void RestoreUserSettings()
        {
            // ユーザー設定の復元
            txtTargetFileListName.Text = Properties.Settings.Default.LastTargetFileListPath;
            chkAutoLoad.Checked = Properties.Settings.Default.LastAutoLoad;
            cmbEncodingTarget.SelectedIndex = Properties.Settings.Default.LastEncodingTarget;
            chkIncludeBOM.Checked = Properties.Settings.Default.LastIncludeBOM;
            rdBtnCRLF.Checked = Properties.Settings.Default.LastLineEndingCRLF;
            rdBtnLF.Checked = Properties.Settings.Default.LastLineEndingLF;
            rdBtnCR.Checked = Properties.Settings.Default.LastLineEndingCR;
            chkLogVisible.Checked = Properties.Settings.Default.LastLogVisible;
        }

        /// <summary>
        /// ファイルリストの有無や変換中状態に応じて各種ボタンの有効・無効を切り替える。
        /// </summary>
        private void UpdateButtonStates(bool idle = true)
        {
            Invoke(() =>
            {
                bool hasRows = targetFilePathTable.AsEnumerable()
                    .Any(r => r.RowState != DataRowState.Deleted);

                btnAddTargetFile.Enabled = idle;
                cmbEncodingTarget.Enabled = idle;
                chkIncludeBOM.Enabled = idle;
                pnlNewLine.Enabled = idle;
                btnConvert.Enabled = idle && hasRows;
                btnClear.Enabled = idle && hasRows;
                txtTargetFileListName.Enabled = idle;
                btnSaveTargetFileList.Enabled = idle && hasRows;
                btnLoadTargetFileList.Enabled = idle;
            });
        }

        /// <summary>
        /// UIから選択された文字コード（変換元）を取得。
        /// </summary>
        private Encoding GetSourceEncodingFromUI()
        {
            return null;
        }

        /// <summary>
        /// UIから選択された文字コード（変換先）を取得。
        /// </summary>
        private Encoding GetTargetEncodingFromUI()
        {
            Encoding targetEncoding = Encoding.UTF8;
            this.Invoke(() =>
            {
                var selectedEncoding = cmbEncodingTarget.SelectedItem?.ToString() ?? Constant.DefaultEncoding;
                targetEncoding = selectedEncoding switch
                {
                    Constant.ShiftJISEncoding => Encoding.GetEncoding(Constant.ShiftJISEncoding),
                    Constant.ISO2022JPEncoding => Encoding.GetEncoding(Constant.ISO2022JPEncoding),
                    Constant.EUCJPEncoding => Encoding.GetEncoding(Constant.EUCJPEncoding),
                    Constant.UTF8Encoding => new UTF8Encoding(chkIncludeBOM.Checked),
                    _ => Encoding.UTF8, // デフォルトは UTF-8
                };
            });
            return targetEncoding;
        }

        /// <summary>
        /// UIから選択された改行コードオプションを取得。
        /// </summary>
        private string GetNewLineOptionFromUI()
        {
            if (rdBtnCRLF.Checked) return Constant.CRLF;
            if (rdBtnLF.Checked) return Constant.LF;
            if (rdBtnCR.Checked) return Constant.CR;
            return Environment.NewLine;
        }

        /// <summary>
        /// ファイル一覧テーブルの全行を「未処理」状態に初期化し、インデックスと背景色を更新する。
        /// </summary>
        public void InitTargetFilePathTable()
        {
            foreach (DataRow row in targetFilePathTable.Rows)
            {
                UpdateRowStatus(row["FilePath"].ToString(), FileConversionStatus.未処理);
            }
            MakeIndexTargetFilePathTable();
            UpdateAllRowBackColor();    // 全ての対象ファイルの背景色を更新する。
        }
        /// <summary>
        /// DataTable上の未処理ファイルに対して同期的に変換処理を実行。
        /// </summary>
        private void ExecuteEncodingForAll()
        {
            int rowNum = 0;

            var jobs = targetFileManager.CreateJobs(
                GetSourceEncodingFromUI(),
                GetTargetEncodingFromUI(),
                chkIncludeBOM.Checked,
                GetNewLineOptionFromUI(),
                row => row["Status"]?.ToString() == Constant.UnprocessedStatus);

            Invoke(() =>
            {
                // 文字コード変換プログレスバーの初期化
                toolStripProgressBar.Minimum = 0;
                toolStripProgressBar.Maximum = jobs.Count();
                toolStripProgressBar.Value = 0;
                toolStripProgressBar.Visible = true;
            });

            foreach (var job in jobs)
            {
                try
                {
                    UpdateRowStatus(job.InputPath, FileConversionStatus.実行中);
                    converter.Convert(job);
                    logger?.LogConvSuccess(job.InputPath);
                    UpdateRowStatus(job.InputPath, FileConversionStatus.成功);
                }
                catch (FileNotFoundException)
                {
                    UpdateRowStatus(job.InputPath, FileConversionStatus.スキップ);
                    logger?.LogConvFailure(job.InputPath, Constant.FileNotFoundMessage);
                }
                catch (Exception ex)
                {
                    UpdateRowStatus(job.InputPath, FileConversionStatus.失敗);
                    logger?.LogConvFailure(job.InputPath, ex.Message);
                }
                finally
                {
                    rowNum++;
                    Invoke(() =>
                    {
                        // 文字コード変換プログレスバーの進捗を更新
                        toolStripProgressBar.Value = rowNum;
                    });
                }
            }

            UpdateAllRowBackColor();    // 全ての対象ファイルの背景色を更新する。
            UpdateStatusSummary();      // 最後にステータスサマリーを更新
        }

        /// <summary>
        /// 非同期・並列で変換を実行（Parallel.ForEachAsync）
        /// UIスレッド更新には Invoke を使用。
        /// </summary>
        private async Task RunAllConversionsAsync()
        {
            int rowNum = 0;

            // UIスレッド上で jobs を取得しておく
            IEnumerable<EncodingConversionJob> jobs = this.Invoke(() =>
            {
                return targetFileManager.CreateJobs(
                    GetSourceEncodingFromUI(),
                    GetTargetEncodingFromUI(),
                    chkIncludeBOM.Checked,
                    GetNewLineOptionFromUI(),
                    row => row["Status"]?.ToString() == Constant.UnprocessedStatus).ToList();
            });

            // Jobを小さなバッチに分割し、バッチ単位で処理を行うことで、スレッドプールの負荷を軽減
            var batchSize = Constant.BatchSize; // バッチサイズを設定
            var jobBatches = jobs.Select((job, index) => new { job, index })
                                 .GroupBy(x => x.index / batchSize)
                                 .Select(g => g.Select(x => x.job).ToList());

            // ParallelOptions を設定（スレッド数を増やす）
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 };

            // 並列処理を実行
            await Task.Run(async () =>
            {
                await Parallel.ForEachAsync(jobs, options, async (job, _) =>
                {
                    try
                    {
                        foreach (var batch in jobBatches)
                        {
                            await Task.WhenAll(batch.Select(job => converter.ConvertAsync(job)));
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        this.Invoke(() =>
                        {
                            UpdateRowStatus(job.InputPath, FileConversionStatus.失敗);
                            logger?.LogConvFailure(job.InputPath, Constant.FileNotFoundMessage);
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(() =>
                        {
                            UpdateRowStatus(job.InputPath, FileConversionStatus.失敗);
                            logger?.LogConvFailure(job.InputPath, ex.Message);
                        });
                    }
                    rowNum++;
                });
            });

            // 最後にステータスサマリーを更新
            this.Invoke(() => UpdateStatusSummary());
        }

        /// <summary>
        /// 指定ファイルに対応する行の状態・背景色を更新し、ログ出力も行う。
        /// </summary>
        private void UpdateRowStatus(string inputPath, FileConversionStatus status)
        {
            if (InvokeRequired)
            {
                Invoke(() => UpdateRowStatus(inputPath, status));
                return;
            }
            string statusText = status.ToString();
            Color backColor = StatusColorMap[status];

            UpdateRowStatus(inputPath, statusText, backColor);
            LogStatus(inputPath, status);

            if (status == FileConversionStatus.成功) successCount++;
            else if (status == FileConversionStatus.失敗) failCount++;
        }

        /// <summary>
        /// 指定ファイルのステータスと背景色を更新し、行の色を反映する。
        /// 状態更新後にステータスサマリーも更新される。
        /// </summary>
        private void UpdateRowStatus(string inputPath, string statusText, Color? backColor = null)
        {
            // 指定されたファイルパスに対応する行を検索 (予め対象テーブルのインデックスを作成済み)
            if (indexTargetFilePath.TryGetValue(inputPath, out var rows))
            {
                // 更新対象行を取得
                var row = rows.FirstOrDefault();

                // 対象行のステータスを更新
                row["Status"] = statusText;

                // Note: 背景色の更新処理がめちゃくちゃ時間が掛かるため、全ての文字コード変換完了後に一気に背景色を変更する方針に切り替えてみる。

                //// 対象行の背景色を更新
                //if (backColor.HasValue && dataGridTargetFileList.DataSource == targetFilePathTable)
                //{
                //    int rowIndex = targetFilePathTable.Rows.IndexOf(row);
                //    dataGridTargetFileList.Rows[rowIndex].DefaultCellStyle.BackColor = backColor.Value;
                //}
            }

            // ステータスサマリーの更新は一度だけ行う
            UpdateStatusSummary();
        }

        /// <summary>
        /// 指定ファイルのステータスと背景色を更新し、行の色を反映する。
        /// 対象の文字コード変換が全て完了したタイミングで、全ての対象ファイルの背景色を更新する。
        /// </summary>
        private void UpdateAllRowBackColor()
        {
            if (InvokeRequired)
            {
                Invoke(() => UpdateAllRowBackColor());
                return;
            }
            foreach (DataRow row in targetFilePathTable.Rows)
            {
                // 対象行のステータスを取得
                var statusValue = row["Status"]?.ToString() ?? Constant.UnprocessedStatus;
                Enum.TryParse<FileConversionStatus>(statusValue, out var status);

                // 対象行の背景色を更新
                var backColor = StatusColorMap[status];
                int rowIndex = targetFilePathTable.Rows.IndexOf(row);
                dataGridTargetFileList.Rows[rowIndex].DefaultCellStyle.BackColor = backColor;
            }
        }

        /// <summary>
        /// ステータス更新時に情報ログを出力する。
        /// </summary>
        private void LogStatus(string inputPath, FileConversionStatus status)
        {
            logger?.LogDebug($"[{status}] {inputPath}");
        }

        /// <summary>
        /// DataTable 全体のステータスを集計し、ステータスバーに要約を表示。
        /// </summary>
        private void UpdateStatusSummary()
        {
            var groups = targetFilePathTable.AsEnumerable()
                .Where(row => row.RowState != DataRowState.Deleted)
                .GroupBy(row => row["Status"]?.ToString() ?? Constant.UndefinedStatus)
                .ToDictionary(g => g.Key, g => g.Count());

            int total = targetFilePathTable.Rows.Count;
            int success = groups.TryGetValue(Constant.SuccessStatus, out var ok) ? ok : 0;
            int failed = groups.TryGetValue(Constant.FailureStatus, out var ng) ? ng : 0;
            int skipped = groups.TryGetValue(Constant.SkipStatus, out var sk) ? sk : 0;
            int running = groups.TryGetValue(Constant.RunningStatus, out var run) ? run : 0;
            int pending = groups.TryGetValue(Constant.UnprocessedStatus, out var pd) ? pd : 0;

            toolStripStatusLabelFormMainBottom.Text =
                string.Format(Constant.StatusSummaryMessage, total, success, failed, skipped, running, pending);
        }

        /// <summary>
        /// ログフォームをメインフォーム直下に表示し、Z順・所有関係を調整する。
        /// </summary>
        private void ShowFormLog()
        {
            // MainForm の左下の画面座標を取得
            var mainFormBottomLeft = new Point(this.Left, this.Bottom);

            // LogForm のサイズ（Width, Height）が既に分かっていれば合わせて補正もOK
            formLog.StartPosition = FormStartPosition.Manual;
            formLog.Location = mainFormBottomLeft;

            PositionLogFormBelowMain();
            formLog.Owner = this;         // ← 所有関係を明示
            formLog.Show();
            formLog.BringToFront();       // ログフォーム前面へ
            this.Activate();              // メインフォームを前面に戻して LogForm を「次に」持ってくる
        }
    }
}