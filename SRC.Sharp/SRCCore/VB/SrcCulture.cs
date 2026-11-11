using System.Globalization;

namespace SRCCore.VB
{
    /// <summary>
    /// 元の SRC が前提とする、日本語版の Windows の地域の設定に当たるカルチャ。
    /// VB5 の CStr・CDbl・IsNumeric・Format・CDate などはユーザーの地域の設定で数値と日付を読み書きする。
    /// SRC# は実行する環境の言語の設定によらず、日本語（ja-JP）の書式に固定する（7474/SRC#1554）。
    /// </summary>
    public static class SrcCulture
    {
        /// <summary>
        /// 数値と日付の書式に使うカルチャ。ja-JP。カルチャのデータが無い環境（InvariantGlobalization）では InvariantCulture。
        /// 数値の書式（小数点「.」・桁区切り「,」・負の符号「-」）は ja-JP と InvariantCulture で同じ。
        /// </summary>
        public static readonly CultureInfo Japanese = GetJapanese();

        private static CultureInfo GetJapanese()
        {
            try
            {
                return CultureInfo.GetCultureInfo("ja-JP");
            }
            catch (CultureNotFoundException)
            {
                // カルチャのデータが無い環境（InvariantGlobalization）
                return CultureInfo.InvariantCulture;
            }
        }
    }
}
