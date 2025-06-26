namespace EncodingConverter
{
    /// <summary>
    /// EncodingConverter アプリケーションで発生する可能性のあるエラーコードを定義する列挙型。
    /// </summary>
    public enum ErrorCode
    {
        NoError = 0,                            // 正常終了
        AppConfigFileFotFound,                  // 設定ファイルが見つからない
        AppConfigLoadingFailed,                 // 設定ファイルの読み込みに失敗
        StartupArgParsingFailed,                // コマンドライン引数の解析に失敗
        ConvertTargetFileListFileFotFound,      // 変換対象ファイルリストが見つからない
        EncodingConversionFailed,               // 文字コード変換に失敗
        ConversionSucceededWithSkipedFiles,     // 変換に成功したがスキップされたファイルがある
    }

    public static class ErrorManager
    {
        public static ErrorCode _LastErrorCode = ErrorCode.NoError;
        public static List<string> _conversionSkipedFiles = new List<string>();

        public static void ClearError()
        {
            _LastErrorCode = ErrorCode.NoError;
            _conversionSkipedFiles.Clear();
        }

        public static void SetError(ErrorCode errorCode)
        {
            _LastErrorCode = errorCode;
        }

        public static ErrorCode GetLastError()
        {
            return _LastErrorCode;
        }

        public static bool HasError()
        {
            return _LastErrorCode != ErrorCode.NoError;
        }

        public static void AppendConversionSkipedFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && !_conversionSkipedFiles.Contains(filePath))
            {
                _conversionSkipedFiles.Add(filePath);
            }
        }

        public static bool HasConversionSkipedFiles()
        {
            return _conversionSkipedFiles.Count > 0;
        }

        public static int getConversionSkipedFileCount()
        {
            return _conversionSkipedFiles.Count;
        }

        public static List<string> GetConversionSkipedFiles()
        {
            return _conversionSkipedFiles;
        }
    }
}
