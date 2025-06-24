/// <summary>
/// ファイルの文字エンコーディングを自動判定するユーティリティクラス。
/// Mozilla Ude ライブラリ（Ude.NetStandard）を利用。
/// </summary>
public static class EncodingDetector
{
    /// <summary>
    /// 指定されたファイルの文字エンコーディングを推定する。
    /// 判定できなかった場合は null を返す。
    /// </summary>
    /// <param name="filePath">判定対象のファイルパス</param>
    /// <returns>判定された Encoding オブジェクト、または null</returns>
    public static Encoding? Detect(string filePath)
    {
        using var fs = File.OpenRead(filePath);
        var detector = new CharsetDetector();
        detector.Feed(fs);
        detector.DataEnd();

        return detector.Charset != null
            ? Encoding.GetEncoding(detector.Charset)
            : null;
    }
}