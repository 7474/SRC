namespace SRCRender
{
    /// <summary>
    /// マウスカーソルを動かせるホストが IScreenHost と併せて実装する（設定「マウスカーソルを自動的に移動する」用）。
    /// 座標は画面（ScreenLayout）のピクセル座標。ゲームスレッドから呼ばれる。
    /// </summary>
    public interface IScreenCursorHost
    {
        /// <summary>カーソルの位置を得る。得られない場合は false。</summary>
        bool TryGetCursorPosition(out float x, out float y);

        /// <summary>カーソルを移動する。移動できない場合は false。</summary>
        bool SetCursorPosition(float x, float y);
    }
}
