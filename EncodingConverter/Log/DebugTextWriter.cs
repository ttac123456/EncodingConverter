namespace EncodingConverter.Infrastructure
{
    /// <summary>
    /// Console.Out への出力内容を同時に Debug 出力にも転送する TextWriter 実装。
    /// </summary>
    public class DebugTextWriter : TextWriter
    {
        /// <summary>元の Console.Out を保持。</summary>
        private readonly TextWriter originalOut;

        /// <summary>DebugTextWriter を初期化し、元の Console.Out を記憶する。</summary>
        public DebugTextWriter()
        {
            // 元の Console.Out を保持
            originalOut = Console.Out;
        }

        /// <summary>出力に使用するエンコーディング（UTF-8固定）。</summary>
        public override Encoding Encoding => Encoding.UTF8;

        /// <summary>1文字出力を Console.Out と Debug に転送。</summary>
        public override void Write(char value)
        {
            originalOut.Write(value);       // 元の Console.Out に出力
            Debug.Write(value);             // デバッグ出力にも送る
        }

        /// <summary>文字列出力を Console.Out と Debug に転送。</summary>
        public override void Write(string? value)
        {
            originalOut.Write(value);       // 元の Console.Out に出力
            Debug.Write(value);             // デバッグ出力にも送る
        }

        /// <summary>改行付き文字列出力を Console.Out と Debug に転送。</summary>
        public override void WriteLine(string? value)
        {
            originalOut.WriteLine(value);   // 元の Console.Out に出力
            Debug.WriteLine(value);         // デバッグ出力にも送る
        }

        /// <summary>バッファをフラッシュする。</summary>
        public override void Flush()
        {
            originalOut.Flush();            // 元の Console.Out にデリゲート
        }
    }
}
