namespace EncodingConverter
{
    using Infrastructure;
    using System.Diagnostics;

    /// <summary>
    /// コマンドラインベースでのファイル変換処理を司る実行クラス。
    /// コマンドライン引数に応じて各種変換オプションを設定し、
    /// ファイルリストのロード・変換・エラーログ出力・コンソール制御を行う。
    /// </summary>
    public class ConsoleMain
    {
        /// <summary>
        /// Win32 API を使用してコンソールの割り当てを行う。
        /// </summary>
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        /// <summary>
        /// Win32 API を使用してコンソールの解放を行う。
        /// </summary>
        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        /// <summary>
        /// ファイル一覧管理用の TargetFileManager インスタンス。
        /// </summary>
        private readonly TargetFileManager targetFileManager;

        /// <summary>
        /// 文字コード変換処理のメインロジックを提供する EncodingConverter インスタンス。
        /// </summary>
        private readonly EncodingConverter converter;

        ///// <summary>
        ///// コマンドライン引数から解析した変換オプション。
        ///// </summary>
        //public readonly ConversionOptions options;

        /// <summary>
        /// コンソール出力用のロガー。
        /// LogLevel に応じて情報／エラーの出力制御が可能。
        /// </summary>
        private readonly ConsoleLogger logger;

        private readonly string appBasePath;
        private readonly string prjBaseName = "WTGyges";
        private string prjBasePath = "";

        /// <summary>
        /// コンソール変換処理の初期化。
        /// コマンドライン引数を解析し、各種オプション・ロガー・ファイル管理を初期化する。
        /// </summary>
        /// <param name="args">コマンドライン引数配列</param>
        public ConsoleMain()
        {
            // アプリケーションの起動パスを取得
            // デバッグ実行時はプロジェクトルート直下のbinフォルダを指すように調整
            // 通常実行時は実際のexe配置ディレクトリをそのまま表示
            //if (Debugger.IsAttached)
            //{
            //    // Visual Studio等のIDEからデバッグ実行中
            //    appBasePath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\..\..\bin"));
            //}
            //else
            //{
            //    // exeを直接ダブルクリック等で起動
            //    appBasePath = Path.GetFullPath(Application.StartupPath);
            //}

            // アプリケーションの起動パスを取得
            appBasePath = AppConfig.Instance.AppBasePath;

            // プロジェクト名を取得
            prjBaseName = AppConfig.Instance.ProjectName;

            prjBasePath = string.Empty; // 初期状態では空にしておく
            var seekPrjBasePath = appBasePath;
            while ((seekPrjBasePath != null) && (prjBasePath != seekPrjBasePath))
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
                // プロジェクトベース名が見つからなかった場合はエラー終了するしかないかな。
                return;
            }

            //// 起動引数を解析して設定
            //options = ParseStartupArgs(args);
            //startupArgs = args;
            // 起動引数を取得
            var startupArgs = StartupArgsAnalyzer.Instance.StartupArgs;

            // DataTable の初期化
            var table = new DataTable();
            table.Columns.Add("No", typeof(int));
            table.Columns.Add("FilePath", typeof(string));
            table.Columns.Add("Status", typeof(string));

            logger = new ConsoleLogger(appBasePath, AppConfig.Instance.IsOutputLogFile) { Level = startupArgs.LogLevel };
            targetFileManager = new TargetFileManager(logger, table);
            converter = new EncodingConverter(logger, prjBasePath);
        }

        ///// <summary>
        ///// コマンドライン引数を解析し、変換オプション構造体を生成する。
        ///// </summary>
        ///// <param name="args">コマンドライン引数配列</param>
        ///// <returns>解析結果の ConversionOptions 構造体</returns>
        //private ConversionOptions ParseStartupArgs(string[] args)
        //{
        //    // 引数を辞書形式に変換
        //    var dict = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        //    foreach (var arg in args)
        //    {
        //        if (!arg.StartsWith("--")) continue;
        //        var parts = arg.Substring(2).Split(new[] { '=' }, 2);
        //        var key = parts[0].Trim().ToLower();
        //        var value = parts.Length == 2 ? parts[1].Trim() : null;
        //        dict[key] = value ?? "true";
        //    }

        //    // Note: コマンドライン実行時に指定して欲しいオプション
        //    // <必須パラメータ> --nogui, --encoding, --newline, --targetlist
        //    // <任意パラメータ> --quiet, --bom, --log, --dry-run

        //    // 引数を解析して文字コード変換オプションを取得
        //    bool isNogui = dict.ContainsKey("nogui");
        //    string encodingName = dict.GetValueOrDefault("encoding") ?? "utf-8";
        //    bool useBom = dict.GetValueOrDefault("bom")?.ToLower() == "true";
        //    var encoding = (encodingName == "utf-8") ? new UTF8Encoding(useBom) : Encoding.GetEncoding(encodingName);
        //    string newlineOption = dict.GetValueOrDefault("newline")?.ToUpperInvariant() ?? "CR+LF";
        //    string newline = newlineOption switch
        //    {
        //        "CR+LF" => "\r\n",
        //        "LF" => "\n",
        //        "CR" => "\r",
        //        _ => Environment.NewLine,
        //    };
        //    string targetListPath = dict.GetValueOrDefault("targetlist") ?? "";
        //    bool isQuiet = dict.ContainsKey("quiet");
        //    string filter = dict.GetValueOrDefault("INFO") ?? "未処理";
        //    bool dryRun = dict.GetValueOrDefault("dry-run") == "true";
        //    LogLevel logLevel = Enum.TryParse(dict.GetValueOrDefault("log"), true, out LogLevel level) ? level : LogLevel.Info;

        //    // 文字コード変換オプションを取得を返却
        //    return new ConversionOptions
        //    {
        //        IsNoGui = isNogui,
        //        EncodingName = encodingName,
        //        UseBom = useBom,
        //        Encoding = encoding,
        //        NewlineOption = newlineOption,
        //        Newline = newline,
        //        TargetListPath = targetListPath,
        //        IsQuiet = isQuiet,
        //        Filter = filter,
        //        DryRun = dryRun,
        //        LogLevel = logLevel
        //    };
        //}

        /// <summary>
        /// 指定された変換条件に従い、ファイルリストの変換処理を実行。
        /// フィルタや静音モード対応、変換結果のログ出力とエラーCSV保存を行う。
        /// </summary>
        public void Run()
        {
            string logText = "";

            try
            {
                // コマンドプロンプトから起動された場合のみコンソールを割り当てる
                StartConsole();

                logger?.StartConversionLog();

                logText = $"== Encoding Convert: START! ==";
                Console.WriteLine(logText);
                logger?.LogInfo(logText);

                // 変換処理を実行
                Convert();

                logText = $"== Encoding Convert: DONE! ==";
                Console.WriteLine(logText);
                logger?.LogInfo(logText);
            }
            finally
            {
                logger?.StopConversionLog();

                // 必要な出力が終わったらコンソールを開放
                EndConsole();
            }
        }

        /// <summary>
        /// ファイルリストのロード・変換ジョブ生成・変換処理・エラー保存をまとめて実行する。
        /// </summary>
        private void Convert()
        {
            // 起動引数を取得
            var startupArgs = StartupArgsAnalyzer.Instance.StartupArgs;

            // 変換対象ファイルリストをロード
            targetFileManager.LoadTargetFileList(prjBasePath, startupArgs.TargetListPath);

            // 変換ジョブを作成
            var jobs = targetFileManager.CreateJobs(
                null, startupArgs.Encoding, startupArgs.UseBom, startupArgs.Newline,
                row => row["Status"]?.ToString() == startupArgs.Filter
            ).ToList();

            // 変換処理の実行
            var failedFiles = new List<string>();
            int ok = 0, ng = 0;
            foreach (var job in jobs)
            {
                // 静音モードなら変換を実行せずログ出力のみ
                if (startupArgs.DryRun)
                {
                    logger?.LogInfo($"[DRY] {job.InputPath} → {job.OutputPath}");
                    continue;
                }

                try
                {
                    // 変換処理の実行
                    converter.Convert(job);
                    logger.LogConvSuccess(job.InputPath);
                    ok++;
                }
                catch (FileNotFoundException)
                {
                    // ファイルが見つからない場合のエラーログ出力
                    logger?.LogConvFailure(job.InputPath, "ファイルが見つかりませんでした。");
                    ng++;
                    failedFiles.Add(job.InputPath);
                }
                catch (Exception ex)
                {
                    // その他の例外発生時のエラーログ出力
                    logger.LogConvFailure(job.InputPath, ex.Message);
                    ng++;
                    failedFiles.Add(job.InputPath);
                }
            }

            // 変換結果のログ出力
            logger?.LogInfo($"変換完了：成功 {ok} 件 ／ 失敗 {ng} 件");

            // エラーが発生した場合は失敗ファイル一覧を保存
            if (ng > 0)
            {
                SaveErrorsCsv(startupArgs.TargetListPath, failedFiles);
            }
        }

        /// <summary>
        /// 変換に失敗したファイル一覧を .errors.csv として保存。
        /// </summary>
        /// <param name="sourcePath">元のファイルリストCSVのパス</param>
        /// <param name="failedFiles">変換に失敗したファイルパスのリスト</param>
        private void SaveErrorsCsv(string sourcePath, List<string> failedFiles)
        {
            // 失敗ファイル一覧の保存パスを決定
            string errorListPath = Path.ChangeExtension(sourcePath, ".errors.csv");

            // 既存のエラーリストファイルがあれば削除
            using var writer = new StreamWriter(errorListPath, false, new UTF8Encoding(true));

            // 失敗ファイル一覧を書き込む
            foreach (var path in failedFiles)
            {
                writer.WriteLine(path.Contains(',') ? $"\"{path}\"" : path);
            }
            Console.WriteLine($"失敗ファイル一覧を保存しました: {errorListPath}");
        }

        /// <summary>
        /// コンソールの割り当て・起動情報の表示・変換開始メッセージの出力を行う。
        /// </summary>
        public void StartConsole()
        {
            // 起動引数を取得
            var startupArgs = StartupArgsAnalyzer.Instance.StartupArgs;
            var startupArgsAxArray = StartupArgsAnalyzer.Instance.GetStartupArgsAsArray();

            // コマンドプロンプトから起動された場合のみコンソールを割り当てる
            if (Environment.UserInteractive && Console.OpenStandardOutput(1) == Stream.Null && !startupArgs.IsQuiet)
            {
                AllocConsole();
            }

            logger.LogInfo("Console起動");

            // アプリケーション起動パスと引数を表示
            Console.WriteLine($"** Launched \"{Application.StartupPath}{Application.ProductName}.exe\".");
            for (int index = 0; index < startupArgsAxArray.Length; index++)
            {
                Console.WriteLine($"**   - Argument[{index}]: {startupArgsAxArray[index]}");
            }
            Console.WriteLine($"**");
            Console.WriteLine();

            // 文字コード変換開始メッセージ
            Console.WriteLine($"「{startupArgs.EncodingName}」への文字コード変換を開始します。");
            Console.WriteLine();
        }

        /// <summary>
        /// 変換完了メッセージの出力・キー入力待ち・コンソール解放を行う。
        /// </summary>
        public void EndConsole()
        {
            // 起動引数を取得
            var startupArgs = StartupArgsAnalyzer.Instance.StartupArgs;

            // 文字コード変換完了メッセージ
            Console.WriteLine();
            Console.WriteLine($"「{startupArgs.EncodingName}」への文字コード変換を完了しました。終了する場合は何かキーを押してください。");

            // ユーザによるキー入力を待つ
            try
            {
                Console.ReadKey(true);
            }
            catch { /* キー入力待ちで例外が発生した場合は無視 */ }

            logger.LogInfo("Console終了");

            // 必要な出力が終わったらコンソールを開放
            FreeConsole();
        }
    }
}