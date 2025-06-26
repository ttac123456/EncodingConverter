// ───── プリプロセッサ定義 ─────
//#define DEBUGGER_ATTACH     // デバッグ時にデバッガをアタッチするかどうか

// ↓ 以降はロジック本体に集中
namespace EncodingConverter
{
    /// <summary>
    /// アプリケーションのメインエントリーポイントを定義するクラス。
    /// コマンドライン引数に応じて GUI または CLI を起動する。
    /// </summary>
    internal static class Program
    {
        enum ExitCode
        {
            Success = 0,                    // 正常終了
            SuccessWithSkipedFiles = 1,     // 変換に成功したがスキップされたファイルがある
            FailureAppConfigLoad = 2,       // 設定ファイルの読み込みに失敗
            FailureStartupArgsParse = 3,    // コマンドライン引数の解析に失敗
            FailureTargetFileNotFound = 4,  // 変換対象ファイルリストが見つからない
            FailureConversion = 5,          // 文字コード変換に失敗
            Failure = 6,                    // その他の異常終了
        }

        /// <summary>
        /// アプリケーションのエントリーポイント。
        /// コマンドライン引数の有無によりCLIまたはGUIモードで起動する。
        /// </summary>
        /// <param name="args">コマンドライン引数配列</param>
        [STAThread]
        static int Main(string[] args)
        {

#if DEBUGGER_ATTACH
            Debugger.Launch(); // デバッグ用：アプリケーション起動時にデバッガをアタッチする
#endif

            // エラー状態をクリア
            ErrorManager.ClearError();

            // マルチバイト文字系のエンコーディング（Shift_JISなど）を有効化
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // 設定ファイルを読み込み
            var resultLoadAppConfig = LoadAppConfig(Path.Combine(GetAppBasePath(), Constant.ConfigFileName));
            if (ErrorCode.AppConfigLoadingFailed == ErrorManager.GetLastError())
                // 設定ファイルの読み込みに失敗
                return (int)ExitCode.FailureAppConfigLoad;

            // コマンドライン引数を解析
            LoadStartupArgs(args);
            var startupArgs = StartupArgsAnalyzer.Instance.StartupArgs;

            // コマンドライン引数がある場合は CLI モードで処理を実行
            if (args.Length > 0)
            {

                // CLI モードで文字コード変換を実行（--nogui 指定があるか後で判定）
                var (isNoGui, result) = RunConversionFromCommandLine(args).GetAwaiter().GetResult();
                if (isNoGui)
                {
                    // CLI処理が行われた場合はGUI起動せずに終了
                    if (result)
                    {
                        // 変換処理が成功した場合は完了メッセージを表示
                        //Console.WriteLine("文字コード変換が完了しました。");
                        //return (int)ExitCode.Success; // 正常終了
                        if (ErrorManager.HasConversionSkipedFiles())
                            // 変換に成功したがスキップされたファイルがある
                            return (int)ExitCode.SuccessWithSkipedFiles;
                        else
                            // 正常終了
                            return (int)ExitCode.Success;

                    }
                    else
                    {
                        // 変換処理が失敗した場合はエラーメッセージを表示
                        //Console.WriteLine("文字コード変換に失敗しました。詳細はログを確認してください。");
                        //return (int)ExitCode.Failure; // 異常終了
                        // 変換後の処理（必要に応じて追加）
                        if (ErrorManager.HasError())
                        {
                            switch (ErrorManager.GetLastError())
                            {
                                case ErrorCode.ConvertTargetFileListFileFotFound:   // 変換対象ファイルリストが見つからない
                                    return (int)ExitCode.FailureTargetFileNotFound;
                                case ErrorCode.EncodingConversionFailed:            // 文字コード変換に失敗
                                    return (int)ExitCode.FailureConversion;
                                default:                                            // その他の異常終了
                                    return (int)ExitCode.Failure;
                            }
                        }
                    }
                }
            }

            // GUI モードで起動
            ApplicationConfiguration.Initialize();
            Application.Run(new FormMain());

            return (int)ExitCode.Success; // 正常終了
        }

        static string GetAppBasePath()
        {
            string appBasePath = string.Empty;
#if DEBUGGER_ATTACH
            // デバッグ用：アプリケーション起動時にデバッガがアタッチされた場合
            appBasePath = Path.GetFullPath(Application.StartupPath);
#else    // DEBUGGER_ATTACH
            // アプリケーションの起動パスを取得
            // デバッグ実行時はプロジェクトルート直下のbinフォルダを指すように調整
            // 通常実行時は実際のexe配置ディレクトリをそのまま表示
            if (Debugger.IsAttached)
            {
                // Visual Studio等のIDEからデバッグ実行中
                appBasePath = Path.GetFullPath(Path.Combine(Application.StartupPath, Constant.DebugRunAppPathOffset));
            }
            else
            {
                // exeを直接ダブルクリック等で起動
                appBasePath = Path.GetFullPath(Application.StartupPath);
            }
#endif   // DEBUGGER_ATTACH
            return appBasePath;
        }

        /// <summary>
        /// コマンドライン引数を解析し、StartupArgAnalyzerのシングルトンインスタンスを初期化する。
        /// </summary>
        /// <param name="args">コマンドライン引数配列</param>
        static void LoadStartupArgs(string[] args)
        {
            StartupArgsAnalyzer.Load(args);
        }

        /// <summary>
        /// 設定ファイルを読み込み、AppConfigのシングルトンインスタンスを初期化する。
        /// 読み込みに失敗した場合はエラーメッセージを出力し、異常終了コードを返す。
        /// </summary>
        /// <param name="configPath">設定ファイルのパス</param>
        /// <returns>ExitCode.Success: 正常終了／ExitCode.Failure: 異常終了</returns>
        static ExitCode LoadAppConfig(string configPath)
        {
            try
            {
                AppConfig.Load(configPath);
                var config = AppConfig.Instance;
                var projectName = config.ProjectName;
                var isExecute = config.IsOutputLogFile;
                var storedFileMax = config.LogFileMax;
                return ExitCode.Success;
            }
            catch (Exception ex)
            {
                ErrorManager.SetError(ErrorCode.AppConfigLoadingFailed);
                Console.Error.WriteLine($"設定ファイルの読み込みに失敗しました: {ex.Message}");
                return ExitCode.Failure;
            }
        }

        /// <summary>
        /// コマンドライン引数に基づいて CLI 変換処理を実行。
        /// "--nogui" 指定がなければ false を返して GUI にフォールバックする。
        /// </summary>
        /// <param name="args">コマンドライン引数配列</param>
        /// <returns>true: CLI処理を実施した／false: GUIに切り替え</returns>
        static async Task<(bool isNoGui, bool result)> RunConversionFromCommandLine(string[] args)
        {
            // 起動引数を取得
            var startupArgs = StartupArgsAnalyzer.Instance.StartupArgs;

            // 引数の解析とオプション設定
            var runner = new ConsoleMain();

            bool isNoGui = startupArgs.IsNoGui;
            bool result = false;

            // オプション --nogui が指定されていない場合は、GUI に切り替え
            if (isNoGui)
            {
                try
                {
                    // Console で文字コード変換を実行
                    runner.Run();
                }
                catch
                {
                    // 例外は握りつぶす（必要に応じてログ出力等を追加）
                }
                finally
                {
                    // 変換後の処理（必要に応じて追加）
                    if (!ErrorManager.HasError())
                    {
                        result = true;
                    }
                }
            }

            return (isNoGui, result);
        }
    }
}