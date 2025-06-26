namespace EncodingConverter.AppSetting
{
    /// <summary>アプリケーション設定を管理するシングルトンクラス。</summary>
    internal class AppConfig
    {
        /// <summary>シングルトンインスタンス本体。</summary>
        private static AppConfig? _instance;

        /// <summary>インスタンス生成時の排他制御用ロックオブジェクト。</summary>
        private static readonly object _lock = new();

        // アプリケーションのベースパス（コンストラクタで設定し、外部からは読み取り専用）
        private readonly string _appBasePath;

        /// <summary>アプリケーションのベースパス（ファイルパスから取得されたディレクトリパス）。</summary>
        public string AppBasePath => _appBasePath;

        /// <summary>プロジェクト名。</summary>
        public string ProjectName { get; set; } = "";

        /// <summary>ログファイルを出力するかどうかのフラグ。</summary>
        public bool IsOutputLogFile { get; set; } = false;

        /// <summary>ログファイルの最大保持件数。</summary>
        public int LogFileMax { get; set; } = 0;

        /// <summary>設定インスタンスを取得（事前に Load 呼び出しが必要）。</summary>
        public static AppConfig Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("AppSettingsはLoadで初期化してください。");
                return _instance;
            }
        }

        /// <summary>XML設定ファイルを読み込んで AppConfig を初期化します。</summary>
        /// <param name="filePath">読み込む設定ファイルのパス</param>
        /// <returns>初期化された AppConfig インスタンス</returns>
        public static AppConfig Load(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                ErrorManager.SetError(ErrorCode.AppConfigFileFotFound);
                Console.Error.WriteLine($"設定ファイルが見つかりません: {filePath}");
                throw new FileNotFoundException("設定ファイルが見つかりません。", filePath);
            }

            var doc = XDocument.Load(filePath);
            XNamespace ns = doc.Root?.GetDefaultNamespace() ?? "";
            var root = doc.Element(ns + Constant.XmlRootNode);
            var projectName = (string?)root?.Element(ns + Constant.XmlProjectNameNode) ?? Constant.DefaultProjectName;
            var isOutputLogFile = bool.TryParse((string?)root?.Element(ns + Constant.XmlIsOutputLogFileNode), out var b) && b;
            var logFileMax = int.TryParse((string?)root?.Element(ns + Constant.XmlLogFileMaxNode), out var i) ? i : Constant.DefaultLogFileMax;

            lock (_lock)
            {
                if (_instance == null)
                    _instance = new AppConfig(Path.GetDirectoryName(filePath));

                _instance.ProjectName = projectName;
                _instance.IsOutputLogFile = isOutputLogFile;
                _instance.LogFileMax = logFileMax;
            }
            return _instance;
        }

        /// <summary>
        /// AppConfig の非公開コンストラクタ。
        /// ファイルパスからディレクトリ部分を抽出してアプリケーションのベースパスとして設定する。
        /// </summary>
        /// <param name="filePath">設定ファイルの存在するディレクトリ</param>
        private AppConfig(string filePath)
        {
            _appBasePath = filePath;
        }
    }
}