namespace EncodingConverter
{
    /// <summary>
    /// 文字コード変換のメイン処理クラス。
    /// 単一ファイルに対する変換機能（同期／非同期）を提供する。
    /// </summary>
    public class EncodingConverter
    {
        private readonly IEncodingLogger logger;
        private string prjBasePath;

        public EncodingConverter(IEncodingLogger logger, string basePath)
        {
            this.logger = logger;
            prjBasePath = basePath;
        }

        public void SetPrjBasePath(string basePath)
        {
            prjBasePath = basePath;
        }

        /// <summary>
        /// 指定されたジョブ情報に基づいて文字コードを変換する（同期版）。
        /// 上書きではなく指定された OutputPath に出力される。
        /// </summary>
        /// <param name="job">変換対象ファイルおよび条件を含むジョブ</param>
        public void Convert(EncodingConversionJob job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            // 相対パスを絶対パスに変換
            string inputFullPath = Path.Combine(prjBasePath, job.InputPath);
            string outputFullPath = Path.Combine(prjBasePath, job.OutputPath);

            if (!File.Exists(inputFullPath))
                throw new FileNotFoundException("入力ファイルが見つかりません", job.InputPath);

            // エンコーディングが未指定なら自動判定
            if (job.SourceEncoding == null)
            {
                job.SourceEncoding = EncodingDetector.Detect(inputFullPath) ?? Encoding.UTF8;
                logger?.LogDebug($"[ENCODE] {job.InputPath} の推定エンコーディングは {job.SourceEncoding?.WebName}");
            }

            // 一時ファイルに出力
            using (var reader = new StreamReader(inputFullPath, job.SourceEncoding))
            using (var writer = new StreamWriter(outputFullPath, false, job.TargetEncoding))
            {
                logger?.LogDebug($"{job.InputPath} から {job.OutputPath} への文字コード変換開始");
                // 1行毎に読み出し
                while ((job.content = reader.ReadLine()) != null)
                {
                    // 改行コード変換
                    if (!string.IsNullOrEmpty(job.NewLine))
                    {
                        job.content = NormalizeNewLines(job.content, job.NewLine);
                    }
                    // 1行毎に書き込み
                    writer.WriteLine(job.content);
                }
                logger?.LogDebug($"{job.InputPath} から {job.OutputPath} への文字コード変換終了");
            }

            // 一時ファイルから本体へ上書き
            try
            {
                //File.WriteAllText(job.OutputPath, job.content, job.TargetEncoding);
                File.Copy(outputFullPath, inputFullPath, overwrite: true);
            }
            finally
            {
                if (File.Exists(outputFullPath))
                {
                    File.Delete(outputFullPath);
                }
            }
        }

        /// <summary>
        /// 指定されたジョブに基づいて非同期に文字コードを変換する。
        /// 自動エンコーディング判定や一時ファイルを利用した安全な上書きに対応。
        /// </summary>
        /// <param name="job">変換対象ファイルおよび条件を含むジョブ</param>
        public async Task ConvertAsync(EncodingConversionJob job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));
            if (!File.Exists(job.InputPath))
                throw new FileNotFoundException("入力ファイルが見つかりません", job.InputPath);

            // エンコーディングが未指定なら自動判定
            if (job.SourceEncoding == null)
            {
                job.SourceEncoding = EncodingDetector.Detect(job.InputPath) ?? Encoding.UTF8;
                logger?.LogDebug($"[ENCODE] {job.InputPath} の推定エンコーディングは {job.SourceEncoding?.WebName}");
            }

            // 相対パスを絶対パスに変換
            string inputFullPath = Path.GetFullPath(Path.Combine(prjBasePath, job.InputPath));
            string outputFullPath = Path.GetFullPath(Path.Combine(prjBasePath, job.OutputPath));

            try
            {
                using (var reader = new StreamReader(inputFullPath, job.SourceEncoding))
                using (var writer = new StreamWriter(outputFullPath, false, job.TargetEncoding))
                {
                    logger?.LogDebug($"{job.InputPath} から {job.OutputPath} への文字コード変換開始");
                    // 1行毎に読み出し
                    while ((job.content = await reader.ReadLineAsync()) != null)
                    {
                        // 改行コード変換
                        if (!string.IsNullOrEmpty(job.NewLine))
                        {
                            job.content = NormalizeNewLines(job.content, job.NewLine);
                        }
                        // 1行毎に書き込み
                        await writer.WriteLineAsync(job.content);
                    }
                    logger?.LogDebug($"{job.InputPath} から {job.OutputPath} への文字コード変換終了");
                }

                try
                {
                    logger?.LogDebug($"{job.OutputPath} で {job.InputPath} ファイルを上書きコピー開始");
                    File.Copy(job.OutputPath, job.InputPath, overwrite: true);
                    logger?.LogDebug($"{job.OutputPath} で {job.InputPath} ファイルを上書きコピー成功");
                }
                catch
                {
                    logger?.LogDebug($"{job.OutputPath} で {job.InputPath} ファイルを上書きコピー失敗");
                }
            }
            finally
            {
                if (File.Exists(job.OutputPath))
                {
                    File.Delete(job.OutputPath);
                }
            }
        }

        /// <summary>
        /// 改行コードを統一するためのユーティリティ。
        /// 改行の混在（CRLF / LF / CR）を一つの形式に揃える。
        /// </summary>
        /// <param name="text">変換前のテキスト</param>
        /// <param name="newLine">統一後の改行文字（例: "\r\n"）</param>
        /// <returns>改行を正規化したテキスト</returns>
        private string NormalizeNewLines(string text, string newLine)
        {
            return text
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", newLine);
        }
    }
}