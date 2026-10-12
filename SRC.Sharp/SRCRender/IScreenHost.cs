using SkiaSharp;

namespace SRCRender
{
    /// <summary>
    /// ScreenGUI を表示する側（Avalonia などの UI フレームワーク）が実装する。
    /// いずれのメソッドもゲームスレッドから呼ばれる。
    /// </summary>
    public interface IScreenHost
    {
        /// <summary>描画済みの画面を表示する。frame は呼び出し後に再利用されるため、必要なら複製すること。</summary>
        void Present(SKBitmap frame);

        /// <summary>ウィンドウタイトルを設定する。</summary>
        void SetTitle(string title);

        /// <summary>
        /// SRC を終了する。true を返すと SRCCore が Environment.Exit を呼ぶ。
        /// </summary>
        bool Terminate();
    }
}
