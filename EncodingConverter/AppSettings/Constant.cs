namespace EncodingConverter.AppSetting
{
    public static class Constant
    {
        // エンコーディング名
        public const string ShiftJISEncoding    = "shift-jis";
        public const string ISO2022JPEncoding   = "iso-2022-jp";
        public const string EUCJPEncoding       = "euc-jp";
        public const string UTF8Encoding        = "utf-8";
        public const string DefaultEncoding = UTF8Encoding;
        public static readonly string[] TargetEncords = { ShiftJISEncoding, ISO2022JPEncoding, EUCJPEncoding, UTF8Encoding };

        // ログレベル
        public const string LogLevelTextAll       = "ALL";
        public const string LogLevelTextDebug     = "DEBUG";
        public const string LogLevelTextInfo      = "INFO";
        public const string LogLevelTextWarn      = "WARN";
        public const string LogLevelTextError     = "ERROR";
        public static readonly string[] LofLevelFilters = { LogLevelTextAll, LogLevelTextDebug, LogLevelTextInfo, LogLevelTextWarn, LogLevelTextError };

        // ステータス名
        public const string StatusTextNone        = "未処理";
        public const string StatusTextSuccess     = "成功";
        public const string StatusTextFailure     = "失敗";
        public const string StatusTextSkip        = "スキップ";
        public const string StatusTextExecution   = "実行中";

        // タイマー間隔
        public const int RowBackColorTimerInterval    = 333;      // 行の背景色を変更するタイマー間隔（ミリ秒）
        public const int LogFlushTimerInterval        = 100;      // ログフラッシュタイマーの間隔（ミリ秒）

        // 設定ファイル名
        public const string ConfigFileName = "EncodingConverterConfig.xml";

        // XMLノード名
        public const string XmlRootNode = "AppConfig";
        public const string XmlProjectNameNode = "ProjectName";
        public const string XmlIsOutputLogFileNode = "IsOutputLogFile";
        public const string XmlLogFileMaxNode = "LogFileMax";

        // 既定値
        public const string DefaultProjectName = "";
        public const bool DefaultIsExecute = false;
        public const int DefaultLogFileMax = 0;

        // コマンドライン引数既定値
        public const string DefaultEncodingName = "utf-8";
        public const string DefaultNewlineOption = "CR+LF";
        public const string DefaultTargetListPath = "";
        public const string DefaultFilter = StatusTextNone;

        // 改行コード
        public const string NewLineCRLF = "\r\n";
        public const string NewLineLF = "\n";
        public const string NewLineCR = "\r";

        // ファイルダイアログ関連
        public static readonly string[] FileDialogFilters = { "すべてのファイル (*.*)|*.*", "テキストファイル (*.txt)|*.txt", "CSVファイル (*.csv)|*.csv" };
        public const string SaveFileDialogTitle = "ファイルリストの保存";
        public const string SaveFileDialogFilter = "CSVファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*";
        public const string OpenFileDialogTitle = "ファイルリストの読込";
        public const string OpenFileDialogFilter = "CSVファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*";

        // メッセージ・タイトル
        public static readonly string RootDirectorySearchFailedMessage = "プロジェクトベース名 \"{0}\" が \"{1}\" 以下に見つかりませんでした。";
        public const string SearchFailedTitle = "ルートディレクトリ検索失敗";
        public const string WarningTitle = "警告";
        public static readonly string InvalidFolderMessage = "保存先はアプリケーションフォルダ配下のみ許可されています。\n[{0}]";
        public static readonly string SaveCompleteMessage = "変換元ファイルリストの保存が完了しました。 (変換元ファイル数: {0})";
        public const string SaveCompleteTitle = "保存完了";
        public const string SaveSuccessMessage = "変換元ファイルリストを保存しました。";
        public static readonly string LoadCompleteMessage = "変換元ファイルリストの読込が完了しました。 (変換元ファイル数: {0})";
        public static readonly string LoadCompleteMessageAtAppStartup = "変換元ファイルリストの起動時自動読込が完了しました。 (変換元ファイル数: {0})";
        public static readonly string AddedFilesMessage = "{0} 件のファイルを追加しました。";
        public const string ClearMessage = "一覧をクリアしました。";
        public const string ConversionStartMessage = "変換処理を開始します...";
        public const string ConversionCompleteMessage = "変換処理が完了しました。";
        public const string ConversionErrorMessage = "変換処理中にエラーが発生しました。";
        public static readonly string ConversionErrorLogMessage = "変換エラー: {0}";
        public const string FileNotFoundMessage = "ファイルが見つかりません。";
        public static readonly string StatusSummaryMessage = "総数:{0}件  成功:{1}  失敗:{2}  スキップ:{3}  実行中:{4}  未処理:{5}";
        public const string UndefinedStatus = "未定義";

        // ステータス名（DataTable用）
        public const string UnprocessedStatus = "未処理";
        public const string SuccessStatus = "成功";
        public const string FailureStatus = "失敗";
        public const string SkipStatus = "スキップ";
        public const string RunningStatus = "実行中";

        // 改行コード（UI用）
        public const string CRLF = "CR+LF";
        public const string LF = "LF";
        public const string CR = "CR";

        // バッチサイズ（並列変換用）
        public const int BatchSize = 8;

        // デバッグ関連
        public const string DebugRunAppPathOffset = @"..\..\..\..\bin";     // デバッグ用binパス
        public const bool IsDebuggerAttachOnStartup = false;                // デバッグ時にデバッガをアタッチするかどうか

        // ログファイル関連
        public const string LogDirectoryName = "Log";                       // ログディレクトリ名
        public const string AppLogFilePrefix = "App";                       // アプリログのファイル名プレフィックス
        public const string ConvLogFilePrefix = "Conv";                     // 変換ログのファイル名プレフィックス
        public const string LogFileExtension = ".log";                      // ログファイルの拡張子
        public const string LogFileTimestampFormat = "yyyyMMdd-HHmmss";     // ログファイルのタイムスタンプ形式
    }
}
