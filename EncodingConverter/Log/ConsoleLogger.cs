namespace EncodingConverter.Core
{
    /// <summary>
    /// コンソールアプリケーション向けのロガー。
    /// 出力内容に応じて表示色を変え、LogLevel に応じた制御を行う。
    /// ログは標準出力または標準エラー出力に表示され、ファイルにも同時に記録される。
    /// </summary>
    public class ConsoleLogger : BaseLogger, IDisposable
    {
        /// <summary>
        /// ConsoleLogger のインスタンスを初期化する。
        /// ログファイルの出力先パスを受け取り、ログ基盤を初期化する。
        /// </summary>
        /// <param name="appBasePath">ログファイル出力の基準パス（通常はアプリケーション起動パス）</param>
        /// <param name="isOutputLogFile">ログファイルへの書き出しを有効にするかどうかを示すフラグ。</param>
        public ConsoleLogger(string appBasePath, bool isOutputLogFile) : base(appBasePath, isOutputLogFile)
        {
        }

        /// <summary>
        /// ログテキストをコンソールとログファイルに同時出力する。
        /// ログレベルに応じて文字色を変え、エラー系は標準エラー出力に書き出す。
        /// 成功・失敗ログはプレフィックスから判定し、色付けを明示する。
        /// </summary>
        /// <param name="text">表示・記録するログメッセージ（タイムスタンプ付き）</param>
        /// <param name="level">ログ出力の重要度（DEBUG, INFO, WARN, ERROR）</param>
        protected override void OutputLog(string text, LogLevel level)
        {
            var defaultColor = Console.ForegroundColor;

            ConsoleColor color = level switch
            {
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Info => ConsoleColor.DarkGray,
                LogLevel.Warn => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Magenta,
                _ => defaultColor
            };

            if (text.Contains("文字コード変換完了:"))
                color = ConsoleColor.Blue;
            else if (text.Contains("文字コード変換失敗:"))
                color = ConsoleColor.Red;

            Console.ForegroundColor = color;

            if (level == LogLevel.Error || color == ConsoleColor.Red)
                Console.Error.WriteLine(text);
            else
                Console.WriteLine(text);

            Console.ForegroundColor = defaultColor;

            // ファイルにも出力
            _logFileWriter.InjectLog(text, level);
        }

        /// <summary>
        /// 使用中のリソースを破棄し、ログファイルストリームを閉じる。
        /// アプリケーション終了時に必ず呼び出すことで、ログファイルの整合性を保つ。
        /// </summary>
        public void Dispose()
        {
            _logFileWriter.Dispose();
        }
    }
}