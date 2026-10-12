using SkiaSharp;

namespace SRCRender
{
    /// <summary>
    /// 画面全体のレイアウト。
    /// マップ（左上）、ステータス（右）、メッセージ（下）を1枚の画面に配置する。
    /// ステータスとメッセージの部品は元の SRC と同じ大きさ・配置にする。
    /// </summary>
    public static class ScreenLayout
    {
        public const int CellPx = 32;
        public const int MapColumns = 15;
        public const int MapRows = 15;

        public const int MapWidthPx = CellPx * MapColumns;
        public const int MapHeightPx = CellPx * MapRows;
        public const int StatusWidthPx = 240;
        // 元の SRC のメインウィンドウの高さ（マップ＋スクロールバー）
        public const int StatusHeightPx = MapHeightPx + 16;
        public const int MessageHeightPx = 124;

        public const int Width = MapWidthPx + StatusWidthPx;
        public const int Height = StatusHeightPx + MessageHeightPx;

        public static readonly SKRectI MapArea = new SKRectI(0, 0, MapWidthPx, MapHeightPx);
        public static readonly SKRectI StatusArea = new SKRectI(MapWidthPx, 0, Width, StatusHeightPx);
        public static readonly SKRectI MessageArea = new SKRectI(0, StatusHeightPx, Width, Height);

        /// <summary>顔グラフィックの大きさ</summary>
        public const int FaceSize = 64;

        public static bool IsInMap(int x, int y) => MapArea.Contains(x, y);
    }
}
