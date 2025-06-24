namespace EncodingConverter.Core
{
    /// <summary>
    /// ログ出力の詳細度を示すレベル。
    /// 数値が高いほど詳細な情報を出力する。
    /// Debug に設定すると、LogDebug / LogInfo / LogWarn / LogError / LogConvSuccess / LogConvFalire がすべて出力対象となる。
    /// Info に設定すると、LogInfo / LogWarn / LogError / LogConvSuccess / LogConvFalire のみ出力される。
    /// Warn に設定すると、LogWarn / LogError / LogConvFalire のみ出力される。
    /// Error に設定すると、LogError / LogConvFalire のみ出力される。
    /// </summary>
    public enum LogLevel
    {
        /// <summary>NONE-ログを一切出力しない。</summary>
        None,

        /// <summary>ERROR-エラーのみ出力する。</summary>
        Error,

        /// <summary>WARN-ウォーニングのみ出力する。</summary>
        Warn,

        /// <summary>INFO-情報および成功メッセージを出力する（既定値）。</summary>
        Info,

        /// <summary>DEBUG-詳細なデバッグ情報も出力する。</summary>
        Debug
    }

    /// <summary>
    /// 文字コード変換処理用のロギングインターフェース。
    /// GUI／コンソール等、出力先の違いを意識せずログを記録できる。
    /// </summary>
    public interface IEncodingLogger
    {
        /// <summary>現在のログ出力レベル。</summary>
        LogLevel Level { get; set; }

        /// <summary>詳細なデバッグログを出力する。</summary>
        /// <param name="message">ログ出力する内容</param>
        void LogDebug(string message);

        /// <summary>情報ログ（処理メッセージなど）を出力する。</summary>
        /// <param name="message">ログ出力する内容</param>
        void LogInfo(string message);

        /// <summary>ウォーニングログを出力する。</summary>
        /// <param name="message">ログ出力する内容</param>
        void LogWarn(string message);

        /// <summary>エラーログを出力する。</summary>
        /// <param name="message">ログ出力する内容</param>
        void LogError(string message);

        /// <summary>文字コード変換が正常終了したログを出力する。</summary>
        /// <param name="filePath">正常に変換されたファイルのパス</param>
        void LogConvSuccess(string filePath);

        /// <summary>文字コード変換がエラー終了したログを出力する。</summary>
        /// <param name="filePath">変換エラー対象のファイルのパス</param>
        /// <param name="errorMessage">変換エラー内容の説明</param>
        void LogConvFailure(string filePath, string errorMessage);
    }
}