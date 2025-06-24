using System.Drawing.Interop;

namespace EncodingConverter.Core
{
    /// <summary>
    /// GUI上でログを出力するためのロガー。
    /// 出力先は Action&lt;string&gt; で渡されるため、TextBox や別Formへの柔軟な表示に対応可能。
    /// ログの表示内容は LogLevel に応じてフィルタリングされる。
    /// </summary>
    public class GuiLogger : BaseLogger, IDisposable
    {
        /// <summary>
        /// ログ出力先（例: TextBox への AppendText）。
        /// ログ文字列を受け取り、GUI上に表示するためのデリゲート。
        /// </summary>
        private readonly Action<string> _outputAction;

        /// <summary>
        /// GUIロガーのインスタンスを生成します。
        /// ログ出力先として指定されたログ表示フォームにログメッセージを表示し、
        /// 必要に応じてログファイルへの出力も併せて行います。
        /// </summary>
        /// <param name="logForm">ログをGUIに表示するためのフォーム。AppendLog メソッドにより出力されます。</param>
        /// <param name="appBasePath">ログファイル保存用の基準ディレクトリパス。</param>
        /// <param name="isOutputLogFile">ログファイルへの書き出しを有効にするかどうかを示すフラグ。</param>
        public GuiLogger(FormLog logForm, string appBasePath, bool isOutputLogFile) : base(appBasePath, isOutputLogFile)
        {
            _outputAction = logForm.AppendLog;
        }

        /// <summary>
        /// ログテキストをGUIとログファイルの両方に出力する。
        /// GUI出力は構築時に指定された出力デリゲートに送信され、
        /// ファイル出力は LogFileWriter を通じて記録される。
        /// </summary>
        /// <param name="text">出力するログテキスト（タイムスタンプ付き）</param>
        /// <param name="level">ログの重要度レベル（INFO, WARN, ERROR等）</param>
        protected override void OutputLog(string text, LogLevel level)
        {
            _outputAction?.Invoke(text);
            _logFileWriter.InjectLog(text, level);
        }

        /// <summary>
        /// 利用しているリソース（特にログファイル書き込み用）の破棄処理を実行する。
        /// アプリケーション終了時やログ出力の終了時に呼び出して、ログファイルを正しく閉じる。
        /// </summary>
        public void Dispose()
        {
            _logFileWriter.Dispose();
        }
    }
}