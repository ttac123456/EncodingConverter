namespace EncodingConverter.Core
{
    /// <summary>
    /// ログ出力の共通基底クラス。
    /// 出力先は派生クラスで実装する。
    /// ログの表示内容は LogLevel に応じてフィルタリングされる。
    /// </summary>
    public abstract class BaseLogger : IEncodingLogger
    {
        ////////////////////////////////
        // ログレベル設定
        //

        /// <summary>
        /// 現在のログ出力レベル。
        /// Debug に設定すると、LogDebug / LogInfo / LogWarn / LogError / LogConvSuccess / LogConvFalire がすべて出力対象となる。
        /// Info に設定すると、LogInfo / LogWarn / LogError / LogConvSuccess / LogConvFalire のみ出力される。
        /// Warn に設定すると、LogWarn / LogError / LogConvFalire のみ出力される。
        /// Error に設定すると、LogError / LogConvFalire のみ出力される。
        /// </summary>
        public LogLevel Level { get; set; } = LogLevel.Info;

        /// <summary>全てのログを出力するレベルに設定する。</summary>
        public void SetLogLevelAll() => Level = LogLevel.Debug;  // 全ログを出力

        /// <summary>INFO以上のログを出力するレベルに設定する。</summary>
        public void SetLogLevelInfo() => Level = LogLevel.Info;  // INFO 以上を出力

        /// <summary>WARNING以上のログを出力するレベルに設定する。</summary>
        public void SetLogLevelWarning() => Level = LogLevel.Warn;  // WARNING 以上を出力

        /// <summary>ERRORのみ出力するレベルに設定する。</summary>
        public void SetLogLevelError() => Level = LogLevel.Error; // ERROR のみ出力

        ////////////////////////////////
        // ログ出力
        //

        /// <summary>
        /// 詳細なデバッグログを出力する（灰色）。
        /// </summary>
        public void LogDebug(string message)
        {
            if (Level < LogLevel.Debug) return;
            OutputLog(FormatLog("DEBUG", message), LogLevel.Debug);
        }

        /// <summary>
        /// 情報ログ（処理メッセージなど）を出力する。
        /// </summary>
        public void LogInfo(string message)
        {
            if (Level < LogLevel.Info) return;
            OutputLog(FormatLog("INFO", message), LogLevel.Info);
        }


        /// <summary>
        /// ウォーニングログを出力する。設定を切り替えて処理を継続した時などに使用される。
        /// </summary>
        public void LogWarn(string message)
        {
            if (Level < LogLevel.Warn) return;
            OutputLog(FormatLog("WARN", message), LogLevel.Warn);
        }

        /// <summary>
        /// エラーログを出力する。変換失敗や例外時に使用される。
        /// </summary>
        public void LogError(string message)
        {
            if (Level < LogLevel.Error) return;
            OutputLog(FormatLog("ERROR", message), LogLevel.Error);
        }

        /// <summary>
        /// 文字コード変換が正常終了したログを出力する。
        /// </summary>
        public void LogConvSuccess(string filePath)
        {
            if (Level < LogLevel.Info) return;
            OutputLog(FormatLog("INFO", $"文字コード変換完了: {filePath}"), LogLevel.Info);
        }

        /// <summary>
        /// 文字コード変換がエラー終了したログを出力する。
        /// </summary>
        public void LogConvFailure(string filePath, string errorMessage)
        {
            if (Level < LogLevel.Error) return;
            OutputLog(FormatLog("ERROR", $"文字コード変換失敗: {filePath} → {errorMessage}"), LogLevel.Error);
        }


        ////////////////////////////////
        // ログテキスト組み立て
        //

        /// <summary>
        /// ログ出力用のテキストを日付・時刻（ミリ秒付き）・レベル付きで組み立てる。
        /// </summary>
        /// <param name="level">ログレベル文字列（例: "INFO"）</param>
        /// <param name="message">出力するメッセージ</param>
        /// <returns>書式化されたログテキスト</returns>
        protected string FormatLog(string level, string message)
            => $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] {message}";


        ////////////////////////////////
        // 派生クラスで出力先を実装
        //

        /// <summary>
        /// 実際のログ出力処理を行う抽象メソッド。
        /// 出力先（コンソール、GUI等）ごとに派生クラスで実装する。
        /// </summary>
        /// <param name="text">出力するログテキスト（整形済み）</param>
        /// <param name="level">ログレベル（色分けや装飾用）</param>
        protected abstract void OutputLog(string text, LogLevel level);

        ////////////////////////////////
        // ログファイル出力設定
        //

        /// <summary>
        /// アプリケーションおよび文字コード変換ログをログファイルに記録するファイルロガー。
        /// シングルトンパターンで初期化され、派生先クラスで共有される。
        /// </summary>
        protected readonly LogFileWriter _logFileWriter;

        /// <summary>
        /// ロガーの基底コンストラクタ。ログ出力先の基準パスを受け取り、ファイルロガーの初期化を行う。
        /// </summary>
        /// <param name="appBasePath">ログ出力先の基準となるアプリケーションパス</param>
        /// <param name="isOutputLogFile">ログファイルへの書き出しを有効にするかどうかを示すフラグ。</param>
        protected BaseLogger(string appBasePath, bool isOutputLogFile)
        {
            LogFileWriter.Initialize(appBasePath, isOutputLogFile);
            _logFileWriter = LogFileWriter.Instance;
        }

        ////////////////////////////////
        // ログファイル出力
        //

        /// <summary>変換ログの出力を開始する。</summary>
        public virtual void StartConversionLog()
        {
            _logFileWriter.StartConversionLog();
        }

        /// <summary>変換ログの出力を終了する。</summary>
        public virtual void StopConversionLog()
        {
            _logFileWriter.StopConversionLog();
        }
    }
}