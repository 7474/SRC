using SkiaSharp;
using SRCCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCRender
{
    /// <summary>
    /// SRC の画像ファイルを SKBitmap として読み込みキャッシュする。
    /// </summary>
    public class ImageCache
    {
        private readonly SRC src;
        private readonly ConcurrentDictionary<string, SKBitmap> cache = new ConcurrentDictionary<string, SKBitmap>();
        private readonly ConcurrentDictionary<string, SKBitmap> transparentCache = new ConcurrentDictionary<string, SKBitmap>();
        private IList<string> bitmapDirectories;

        public ImageCache(SRC src)
        {
            this.src = src;
        }

        private IList<string> BitmapDirectories => bitmapDirectories ?? (bitmapDirectories = new List<string>
        {
            "",
            src.FileSystem.PathCombine("Bitmap"),
            src.FileSystem.PathCombine("Bitmap", "Anime"),
            src.FileSystem.PathCombine("Bitmap", "Event"),
            src.FileSystem.PathCombine("Bitmap", "Cutin"),
            src.FileSystem.PathCombine("Bitmap", "Pilot"),
            src.FileSystem.PathCombine("Bitmap", "Unit"),
            src.FileSystem.PathCombine("Bitmap", "Map"),
        });

        /// <summary>
        /// 画像を取得する。見つからない、または読み込めない場合は null。
        /// </summary>
        public SKBitmap Get(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            return cache.GetOrAdd(name, Load);
        }

        /// <summary>
        /// 白を透過色とした画像を取得する。
        /// </summary>
        public SKBitmap GetTransparent(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            return transparentCache.GetOrAdd(name, x => MakeTransparent(Get(x), SKColors.White));
        }

        private SKBitmap Load(string name)
        {
            try
            {
                var path = src.FileSystem.FileExists(name)
                    ? name
                    : BitmapDirectories
                        .Select(x => src.FileSystem.PathCombine(x, name))
                        .FirstOrDefault(x => src.FileSystem.FileExists(x));
                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }
                using (var stream = src.FileSystem.Open(path))
                using (var memory = new MemoryStream())
                {
                    stream.CopyTo(memory);
                    memory.Position = 0;
                    return SKBitmap.Decode(memory);
                }
            }
            catch (Exception ex)
            {
                src.LogError(ex);
                return null;
            }
        }

        private static SKBitmap MakeTransparent(SKBitmap bitmap, SKColor transparentColor)
        {
            if (bitmap == null)
            {
                return null;
            }
            var result = new SKBitmap(bitmap.Width, bitmap.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    var c = bitmap.GetPixel(x, y);
                    result.SetPixel(x, y, c.Red == transparentColor.Red && c.Green == transparentColor.Green && c.Blue == transparentColor.Blue
                        ? SKColors.Transparent
                        : c);
                }
            }
            return result;
        }
    }
}
