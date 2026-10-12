using SkiaSharp;

namespace SRCRender
{
    /// <summary>
    /// 画面全体のレイアウト。
    /// マップ（左上）、ステータス（右）、メッセージ（下）を1枚の画面に配置する。
    /// </summary>
    public static class ScreenLayout
    {
        public const int CellPx = 32;
        public const int MapColumns = 15;
        public const int MapRows = 15;

        public const int MapWidthPx = CellPx * MapColumns;
        public const int MapHeightPx = CellPx * MapRows;
        public const int StatusWidthPx = 240;
        public const int MessageHeightPx = 128;

        public const int Width = MapWidthPx + StatusWidthPx;
        public const int Height = MapHeightPx + MessageHeightPx;

        public static readonly SKRectI MapArea = new SKRectI(0, 0, MapWidthPx, MapHeightPx);
        public static readonly SKRectI StatusArea = new SKRectI(MapWidthPx, 0, Width, MapHeightPx);
        public static readonly SKRectI MessageArea = new SKRectI(0, MapHeightPx, Width, Height);

        /// <summary>顔グラフィックの大きさ</summary>
        public const int FaceSize = 64;

        public static bool IsInMap(int x, int y) => MapArea.Contains(x, y);
    }
}
