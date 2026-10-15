using SkiaSharp;

namespace SRCRender
{
    /// <summary>
    /// 画面全体のレイアウト。
    /// 元の SRC のメインウィンドウと同じく、マップ（左）とステータス（右）を並べる。
    /// メッセージウィンドウは元の SRC と同じく、メインウィンドウの左下に重ねて表示する。
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

        public const int Width = MapWidthPx + StatusWidthPx;
        public const int Height = StatusHeightPx;

        /// <summary>メッセージウィンドウの枠の太さ</summary>
        public const int MessageFramePx = 2;
        /// <summary>メッセージウィンドウのタイトルバーの高さ</summary>
        public const int MessageTitleBarPx = 20;

        public static readonly SKRectI MapArea = new SKRectI(0, 0, MapWidthPx, MapHeightPx);
        public static readonly SKRectI StatusArea = new SKRectI(MapWidthPx, 0, Width, StatusHeightPx);

        /// <summary>
        /// メッセージウィンドウ（枠・タイトルバーを含む）の位置。
        /// 元の SRC と同じく、メインウィンドウの左下に下端を合わせる。
        /// </summary>
        public static SKRectI MessageWindowRect(int clientWidth, int clientHeight)
        {
            var width = clientWidth + MessageFramePx * 2;
            var height = clientHeight + MessageTitleBarPx + MessageFramePx;
            return new SKRectI(0, Height - height, width, Height);
        }

        /// <summary>ユニットを表示するとき（最も大きいとき）のメッセージウィンドウの位置</summary>
        public static readonly SKRectI MessageArea = MessageWindowRect(508, 118);

        /// <summary>顔グラフィックの大きさ</summary>
        public const int FaceSize = 64;

        public static bool IsInMap(int x, int y) => MapArea.Contains(x, y);
    }
}
