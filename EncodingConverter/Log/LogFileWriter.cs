namespace EncodingConverter.Core
{
    /// <summary>
    /// ログファイルへの書き込みを担うクラス。
    /// アプリケーションログおよび文字コード変換ログをファイルに出力する。
    /// シングルトンとして使用され、アプリケーション全体で一意に管理される。
    /// </summary>
    public class LogFileWriter
    {
        /// <summary>シングルトンインスタンス本体。</summary>
        private static LogFileWriter? _instance;

        /// <summary>インスタンス生成時の競合回避用ロックオブジェクト。</summary>
        private static readonly object _lock = new();

        /// <summary>ログファイル出力先の基準パス。指定されない場合は AppContext.BaseDirectory を使用。</summary>
        private static string? _basePath;

        private static bool _isEnabled = false;

        /// <summary>ログファイル出力先の基準パスを初期化します（初回のみ設定可能）。</summary>
        /// <param name="basePath">ログディレクトリの基準となるアプリ起動パス</param>
        public static void Initialize(string basePath, bool isEnabled)
        {
            try
            {
                if (_instance != null)
                    throw new InvalidOperationException("LogFileWriter はすでに初期化されています。");

                _basePath = basePath;
                _isEnabled = isEnabled;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LogFileWriter 初期化失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// シングルトンインスタンスへのアクセス。
        /// 初回アクセス時にアプリケーションログファイル(App_*.log)を生成して初期化される。
        /// </summary>
        public static LogFileWriter Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new LogFileWriter();
                    }
                }
                return _instance;
            }
        }

        /// <summary>アプリケーション全体のログファイルへの書き込みストリーム。</summary>
        private readonly StreamWriter _appLogWriter;

        /// <summary>文字コード変換処理専用のログファイルストリーム。変換中のみ使用される。</summary>
        private StreamWriter? _convLogWriter;

        /// <summary>
        /// コンストラクタ（private）。
        /// App_yyyyMMdd-HHmmss.log を生成し、古いログファイルが設定数を超えている場合は削除する。
        /// </summary>
        private LogFileWriter()
        {
            var basePath = _basePath ?? AppContext.BaseDirectory; // fallback
            var logDir = Path.Combine(basePath, Constant.LogDirectoryName);
            Directory.CreateDirectory(logDir);

            var fileName = $"{Constant.AppLogFilePrefix}_{DateTime.Now:yyyyMMdd-HHmmss}{Constant.LogFileExtension}";
            var appLogPath = Path.Combine(logDir, fileName);
            _appLogWriter = new StreamWriter(appLogPath, append: true, encoding: Encoding.UTF8)
            {
                AutoFlush = true
            };

            DeleteOldLogs(logDir, Constant.AppLogFilePrefix);
        }

        /// <summary>
        /// Conv_yyyyMMdd-HHmmss.log を生成して変換ログの出力を開始する。
        /// すでに変換ログが開かれている場合は再初期化しない。
        /// 初期化後、古い変換ログファイルが上限を超えている場合は削除する。
        /// </summary>
        public void StartConversionLog()
        {
            if (_convLogWriter != null) return;

            var basePath = _basePath ?? AppContext.BaseDirectory; // fallback
            var logDir = Path.Combine(basePath, Constant.LogDirectoryName);
            var fileName = $"{Constant.ConvLogFilePrefix}_{DateTime.Now:yyyyMMdd-HHmmss}{Constant.LogFileExtension}";
            var convLogPath = Path.Combine(logDir, fileName);
            _convLogWriter = new StreamWriter(convLogPath, append: true, encoding: Encoding.UTF8)
            {
                AutoFlush = true
            };

            DeleteOldLogs(logDir, Constant.ConvLogFilePrefix);
        }

        /// <summary>
        /// 開いていた変換ログファイルを閉じ、ストリームを解放する。
        /// 次に StartConversionLog を呼び出すまで変換ログは出力されない。
        /// </summary>
        public void StopConversionLog()
        {
            _convLogWriter?.Dispose();
            _convLogWriter = null;
        }

        /// <summary>
        /// 指定されたログ種別のファイルを上限件数までに収めるよう、古いファイルを削除する。
        /// </summary>
        /// <param name="directory">ログファイルが格納されているディレクトリ</param>
        /// <param name="prefix">ファイル名の先頭（App または Conv）</param>
        private void DeleteOldLogs(string directory, string prefix)
        {
            var maxFiles = AppConfig.Instance.LogFileMax;
            if (maxFiles <= 0) return;

            var files = Directory.GetFiles(directory, $"{prefix}_*{Constant.LogFileExtension}")
                                 .OrderByDescending(File.GetCreationTime)
                                 .ToList();

            var excessFiles = files.Skip(maxFiles);
            foreach (var file in excessFiles)
            {
                try { File.Delete(file); }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ログ削除失敗: {file} → {ex.Message}");
                }
            }
        }

        /// <summary>
        /// ログメッセージをアプリログに常時出力し、変換ログが有効な場合は LogDebug を除いて同時出力する。
        /// </summary>
        /// <param name="text">ログ出力する内容</param>
        /// <param name="level">ログレベル。変換ログには LogDebug を除いたログのみが出力対象。</param>
        public void InjectLog(string text, LogLevel level)
        {
            if (_isEnabled)
            {
                _appLogWriter.WriteLine(text);
                if (_convLogWriter != null && level != LogLevel.Debug)
                    _convLogWriter.WriteLine(text);
            }
        }

        /// <summary>
        /// アプリログおよび変換ログのストリームを解放する。
        /// アプリ終了時に呼び出すことでログファイルの整合性を保つ。
        /// </summary>
        public void Dispose()
        {
            _convLogWriter?.Dispose();
            _convLogWriter = null;

            _appLogWriter.Dispose();
        }
    }
}