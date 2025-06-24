namespace EncodingConverter.Core
{
    /// <summary>
    /// ファイル一覧（DataTable）の読み書き・変換ジョブ生成を担当するクラス。
    /// GUI・CLI 双方の共通処理として機能する。
    /// </summary>
    public class TargetFileManager
    {
        /// <summary>
        /// 管理対象のファイル一覧テーブル。
        /// 列構成：No, FilePath, Status
        /// </summary>
        private readonly DataTable _table;

        /// <summary>
        /// 指定された DataTable を使って TargetFileManager を構築する。
        /// </summary>
        /// <param name="table">ファイル一覧用の DataTable</param>
        public TargetFileManager(DataTable table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        /// <summary>
        /// テーブル内の行から変換ジョブを作成する。
        /// ステータスなどに基づく条件指定も可能。
        /// </summary>
        /// <param name="sourceEncoding">変換元エンコーディング</param>
        /// <param name="targetEncoding">変換先エンコーディング</param>
        /// <param name="includeBom">BOMを含めるか</param>
        /// <param name="newLine">改行コード文字列</param>
        /// <param name="rowFilter">ジョブ対象行を絞り込むフィルター</param>
        /// <returns>EncodingConversionJob の列挙</returns>
        public IEnumerable<EncodingConversionJob> CreateJobs(
            Encoding? sourceEncoding,
            Encoding targetEncoding,
            bool includeBom,
            string newLine,
            Func<DataRow, bool> rowFilter = null)
        {
            foreach (DataRow row in _table.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;
                if (rowFilter != null && !rowFilter(row)) continue;

                string inputPath = row["FilePath"]?.ToString()??"";
                if (string.IsNullOrWhiteSpace(inputPath)) continue;

                string outputPath = GetOutputPath(inputPath);

                yield return new EncodingConversionJob
                {
                    InputPath = inputPath,
                    OutputPath = outputPath,
                    SourceEncoding = sourceEncoding,
                    TargetEncoding = targetEncoding,
                    IncludeBom = includeBom,
                    NewLine = newLine
                };
            }
        }

        /// <summary>
        /// 入力ファイル名から出力ファイルパスを構築する（拡張子変換）。
        /// 必要に応じてカスタマイズ可能。
        /// </summary>
        private string GetOutputPath(string inputPath)
        {
            return Path.ChangeExtension(inputPath, ".converted.bak");
        }

        /// <summary>
        /// 指定されたファイル群をファイル一覧に追加する。
        /// 重複・非存在ファイルは自動除外される。
        /// </summary>
        /// <param name="filePaths">追加対象のファイルパス配列</param>
        /// <returns>追加された件数</returns>
        public int AppendTargetFileList(string appBasePath, string[] filePaths)
        {
            int rowNum = _table.Rows.Count + 1;
            int countAdded = 0;

            foreach (var path in filePaths.Distinct())
            {
                if (!File.Exists(path)) continue;

                // アプリ起動パスからの相対パスに変換
                string relPath = Path.GetRelativePath(appBasePath, path);

                if (_table.AsEnumerable().Any(r => r["FilePath"].ToString() == relPath)) continue;

                AppendTargetFileListRow(rowNum++, relPath, Constant.StatusTextNone);
                countAdded++;
            }

            return countAdded;
        }

        /// <summary>
        /// 現在のファイル一覧をCSV形式で保存する。
        /// カンマ・クオート等は自動エスケープ処理される。
        /// </summary>
        /// <param name="filePath">保存先のファイルパス</param>
        public void SaveTargetFileList(string prjBasePath, string targetFileListPath)
        {
            using var writer = new StreamWriter(targetFileListPath, false, new UTF8Encoding(true));

            foreach (DataRow row in _table.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;

                // DataTableから相対パスで取得
                string relPath = row["FilePath"]?.ToString() ?? string.Empty;

                //// アプリ起動パスからの相対パスに変換
                //string relPath = Path.GetRelativePath(prjBasePath, fullPath);

                // CSV仕様に従ってクオート処理
                if (relPath.Contains(',') || relPath.Contains('"'))
                {
                    relPath = "\"" + relPath.Replace("\"", "\"\"") + "\"";
                }

                writer.Write(relPath + "\r\n");
            }
        }

        /// <summary>
        /// 指定されたCSVファイルを読み込み、ファイル一覧を構築する。
        /// 既存のデータはクリアされる。
        /// </summary>
        /// <param name="filePath">CSV形式のファイル一覧パス</param>
        public void LoadTargetFileList(string prjBasePath, string targetFileListPath)
        {
            if (!File.Exists(targetFileListPath)) return;

            int rowNum = 1;
            _table.Clear();

            using var reader = new StreamReader(targetFileListPath, new UTF8Encoding(true));
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                // CSV仕様に従ってクオート処理
                string relPath = line.Trim().Trim('"').Replace("\"\"", "\"");


                // DataTableにも相対パスで格納

                AppendTargetFileListRow(rowNum++, relPath, Constant.StatusTextNone);
            }
        }

        /// <summary>
        /// 指定された情報をもとに1行分のレコードを作成・追加する。
        /// </summary>
        private void AppendTargetFileListRow(int no, string path, string status)
        {
            var row = _table.NewRow();
            row["No"] = no;
            row["FilePath"] = path;
            row["Status"] = status;
            _table.Rows.Add(row);
        }
    }
}