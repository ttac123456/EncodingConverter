namespace EncodingConverter.Core
{
    /// <summary>
    /// 単一ファイルの文字コード変換処理に関するジョブ情報。
    /// 入力・出力ファイルパスや変換条件をまとめて保持する。
    /// </summary>
    public class EncodingConversionJob
    {
        /// <summary>変換対象となる入力ファイルのパス。</summary>
        public string InputPath { get; set; }

        /// <summary>出力ファイルのパス。CLIツールで上書き変換を行う場合は InputPath と同一になる。</summary>
        public string OutputPath { get; set; }

        /// <summary>元ファイルの文字エンコーディング（null にすることで自動判定可能）。</summary>
        public Encoding? SourceEncoding { get; set; }

        /// <summary>変換後に使用する出力エンコーディング。</summary>
        public Encoding TargetEncoding { get; set; }

        /// <summary>出力時に BOM（Byte Order Mark）を付与するかどうか。</summary>
        public bool IncludeBom { get; set; }

        /// <summary>変換後に適用する改行コード文字列（例: "\r\n", "\n"）。</summary>
        public string NewLine { get; set; }

        /// <summary>CLI環境での実行時に GUI 表示を抑制するフラグ。</summary>
        public bool IsNoGUI { get; set; }

        /// <summary>ログ出力などを控える「静音モード」フラグ。</summary>
        public bool IsQuiet { get; set; }

        /// <summary>変換対象ファイルの内容（必要に応じて利用）。</summary>
        public string? content { get; set; }
    }
}