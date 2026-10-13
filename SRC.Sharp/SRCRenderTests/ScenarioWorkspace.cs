using System;
using System.IO;
using System.Text;

namespace SRCRender.Tests
{
    /// <summary>
    /// シナリオ実行に必要な最小構成（Bitmap / Sound / Data\System\terrain.txt）を一時フォルダに作る。
    /// </summary>
    public sealed class ScenarioWorkspace : IDisposable
    {
        private static readonly Encoding Utf8 = new UTF8Encoding(false);

        /// <param name="withAssets">false の場合は素材（Bitmap / Sound / 地形データ）を置かない</param>
        public ScenarioWorkspace(bool withAssets = true)
        {
            AppPath = Path.Combine(Path.GetTempPath(), "srcs-render-" + Guid.NewGuid());
            Directory.CreateDirectory(AppPath);
            if (!withAssets)
            {
                return;
            }
            Directory.CreateDirectory(Path.Combine(AppPath, "Bitmap", "Event"));
            Directory.CreateDirectory(Path.Combine(AppPath, "Bitmap", "Map"));
            File.WriteAllBytes(Path.Combine(AppPath, "Bitmap", "Map", "plain0000.bmp"), new byte[0]);
            Directory.CreateDirectory(Path.Combine(AppPath, "Sound"));
            File.WriteAllBytes(Path.Combine(AppPath, "Sound", "dummy.wav"), new byte[0]);
            Write(Path.Combine("Data", "System", "terrain.txt"),
                "0", "平地, plain", "陸, 1, 0, 0", "",
                "1", "海, sea", "水, 2, 0, 0", "",
                "");
        }

        public string AppPath { get; }

        public string Write(string relativePath, params string[] lines)
        {
            var path = Path.Combine(AppPath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, string.Join("\r\n", lines), Utf8);
            return path;
        }

        /// <summary>
        /// terrain(x, y) で地形番号を決めるマップファイルを書く。
        /// </summary>
        public string WriteMap(string relativePath, int width, int height, Func<int, int, int> terrain)
        {
            var sb = new StringBuilder();
            sb.Append("MapData\r\n\r\n").Append(width).Append(',').Append(height).Append("\r\n");
            for (var x = 1; x <= width; x++)
            {
                for (var y = 1; y <= height; y++)
                {
                    sb.Append(terrain(x, y)).Append(",0\r\n");
                }
            }
            var path = Path.Combine(AppPath, relativePath);
            File.WriteAllText(path, sb.ToString(), Utf8);
            return path;
        }

        /// <summary>
        /// 描画コマンドのテスト用の画像（64x64。白地に左半分が赤、右半分が青の円）を書く。
        /// </summary>
        public string WriteTestImage(string relativePath)
        {
            var path = Path.Combine(AppPath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var bitmap = new SkiaSharp.SKBitmap(64, 64))
            using (var canvas = new SkiaSharp.SKCanvas(bitmap))
            using (var paint = new SkiaSharp.SKPaint { IsAntialias = false })
            {
                canvas.Clear(SkiaSharp.SKColors.White);
                paint.Color = SkiaSharp.SKColors.Red;
                canvas.DrawRect(8, 8, 24, 48, paint);
                paint.Color = SkiaSharp.SKColors.Blue;
                canvas.DrawRect(32, 8, 24, 48, paint);
                using (var image = SkiaSharp.SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100))
                using (var stream = File.Create(path))
                {
                    data.SaveTo(stream);
                }
            }
            return path;
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(AppPath, true);
            }
            catch
            {
                // ignore
            }
        }
    }
}
