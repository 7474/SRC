using System.Drawing.Text;

namespace SRCSharpForm.Lib
{
    /// <summary>
    /// 描画設定を一元管理するクラス。
    /// Windows 11 の PerMonitorV2 DPI モードでオフスクリーンビットマップに描画する際、
    /// デフォルトの ClearType では文字がぼやけるため、シャープな描画ヒントを使用する。
    /// </summary>
    internal static class RenderingConfig
    {
        /// <summary>テキスト描画に使用する TextRenderingHint。</summary>
        public const TextRenderingHint TextHint = TextRenderingHint.SingleBitPerPixelGridFit;
    }
}
