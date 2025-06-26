namespace EncodingConverter.AppSetting
{
    public struct StartupArgs
    {
        /// <summary>GUIを表示しない場合はtrue。</summary>
        public bool IsNoGui { get; set; }
        /// <summary>使用するエンコーディング名。</summary>
        public string EncodingName { get; set; }
        /// <summary>BOMを付与する場合はtrue。</summary>
        public bool UseBom { get; set; }
        /// <summary>出力に使用するエンコーディング。</summary>
        public Encoding Encoding { get; set; }
        /// <summary>改行コードオプション名。</summary>
        public string NewlineOption { get; set; }
        /// <summary>実際に適用する改行コード。</summary>
        public string Newline { get; set; }
        /// <summary>変換対象ファイルリストのパス。</summary>
        public string TargetListPath { get; set; }
        /// <summary>静音モードの場合はtrue。</summary>
        public bool IsQuiet { get; set; }
        /// <summary>ファイルフィルター。</summary>
        public string Filter { get; set; }
        /// <summary>ドライラン（実際には変換しない）場合はtrue。</summary>
        public bool DryRun { get; set; }
        /// <summary>ログ出力レベル。</summary>
        public LogLevel LogLevel { get; set; }
    }

    /// <summary>
    /// コマンドライン引数を解析し、変換オプションを生成するシングルトンクラス。
    /// </summary>
    public class StartupArgsAnalyzer
    {
        private static StartupArgsAnalyzer? _instance;
        private static readonly object _lock = new();

        public StartupArgs StartupArgs { get; private set; } = default;

        /// <summary>
        /// シングルトンインスタンスを取得します。
        /// </summary>
        public static StartupArgsAnalyzer Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("StartupArgsAnalyzerはLoadで初期化してください。");
                return _instance;
            }
        }

        /// <summary>
        /// コマンドライン引数を解析し、シングルトンインスタンスを初期化します。
        /// </summary>
        /// <param name="args">コマンドライン引数配列</param>
        public static void Load(string[] args)
        {
            var startupArgs = Parse(args);
            lock (_lock)
            {
                if (_instance == null)
                    _instance = new StartupArgsAnalyzer();
                _instance.StartupArgs = startupArgs;
            }
        }

        /// <summary>
        /// コマンドライン引数を解析し、ConversionOptions構造体を生成する。
        /// </summary>
        /// <param name="args">コマンドライン引数配列</param>
        /// <returns>解析結果のStartupArgs</returns>
        private static StartupArgs Parse(string[] args)
        {
            var dict = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var arg in args)
            {
                if (!arg.StartsWith("--")) continue;
                var parts = arg.Substring(2).Split(new[] { '=' }, 2);
                var key = parts[0].Trim().ToLower();
                var value = parts.Length == 2 ? parts[1].Trim() : null;
                dict[key] = value ?? "true";
            }

            bool isNogui = dict.ContainsKey("nogui");
            string encodingName = dict.GetValueOrDefault("encoding") ?? Constant.DefaultEncodingName;
            bool useBom = dict.GetValueOrDefault("bom")?.ToLower() == "true";
            var encoding = (encodingName == Constant.DefaultEncodingName) ? new UTF8Encoding(useBom) : Encoding.GetEncoding(encodingName);
            string newlineOption = dict.GetValueOrDefault("newline")?.ToUpperInvariant() ?? Constant.DefaultNewlineOption;
            string newline = newlineOption switch
            {
                "CR+LF" => Constant.NewLineCRLF,
                "LF" => Constant.NewLineLF,
                "CR" => Constant.NewLineCR,
                _ => Environment.NewLine,
            };
            string targetListPath = dict.GetValueOrDefault("targetlist") ?? Constant.DefaultTargetListPath;
            bool isQuiet = dict.ContainsKey("quiet");
            string filter = dict.GetValueOrDefault("INFO") ?? Constant.DefaultFilter;
            bool dryRun = dict.GetValueOrDefault("dry-run") == "true";
            LogLevel logLevel = Enum.TryParse(dict.GetValueOrDefault("log"), true, out LogLevel level) ? level : LogLevel.Info;

            //// --nogui が指定されている場合は、必須パラメータの有無をチェックする
            //if (isNogui)
            //{
            //    // Note: 必ずデフォルト値を設定しているため、このエラーチェックは省略可能
            //    Error.SetError(ErrorCode.StartupArgParsingFailed);
            //}

            return new StartupArgs
            {
                IsNoGui = isNogui,
                EncodingName = encodingName,
                UseBom = useBom,
                Encoding = encoding,
                NewlineOption = newlineOption,
                Newline = newline,
                TargetListPath = targetListPath,
                IsQuiet = isQuiet,
                Filter = filter,
                DryRun = dryRun,
                LogLevel = logLevel
            };
        }

        /// <summary>
        /// 現在のStartupArgsプロパティの内容をコマンドライン引数形式の文字列配列に変換する。
        /// </summary>
        /// <returns>コマンドライン引数形式の文字列配列</returns>
        public string[] GetStartupArgsAsArray()
        {
            var args = new List<string>();
            if (StartupArgs.IsNoGui) args.Add("--nogui");                                                                   // GUIを表示しない
            if (!string.IsNullOrEmpty(StartupArgs.EncodingName)) args.Add($"--encoding={StartupArgs.EncodingName}");        // エンコーディング名
            if (StartupArgs.UseBom) args.Add("--bom=true");                                                                 // BOM付与
            if (!string.IsNullOrEmpty(StartupArgs.NewlineOption)) args.Add($"--newline={StartupArgs.NewlineOption}");       // 改行コード
            if (!string.IsNullOrEmpty(StartupArgs.TargetListPath)) args.Add($"--targetlist={StartupArgs.TargetListPath}");  // ファイルリストパス
            if (StartupArgs.IsQuiet) args.Add("--quiet");                                                                   // 静音モード
            if (!string.IsNullOrEmpty(StartupArgs.Filter)) args.Add($"--filter={StartupArgs.Filter}");                      // ファイルフィルター
            if (StartupArgs.DryRun) args.Add("--dry-run=true");                                                             // ドライラン
            if (StartupArgs.LogLevel != LogLevel.Info) args.Add($"--log={StartupArgs.LogLevel.ToString().ToLower()}");      // ログレベル
            return args.ToArray();
        }
    }
}