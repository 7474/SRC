using SkiaSharp;
using SRCCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCRender
{
    /// <summary>
    /// SRC の画像ファイルを SKBitmap として読み込みキャッシュする。
    /// 大きいシナリオでは画像が多く、読み込んだものをすべて持ち続けるとメモリが増え続ける（ブラウザ版ではスマートフォンのタブが落ちる）。
    /// そのため、デコードした画素の合計が上限を超えたら、最も長く使っていないものからキャッシュを外す。
    /// 外した画像は Dispose しない（描画中の呼び出し元やウィンドウが持っていることがあるため）。参照が無くなれば GC で解放される。
    /// </summary>
    public class ImageCache
    {
        /// <summary>既定の上限（デコードした画素のバイト数）。640x480 の画像でおよそ 100 枚分</summary>
        public const long DefaultCapacityBytes = 128L * 1024 * 1024;

        private readonly SRC src;
        private readonly object sync = new object();
        private readonly Dictionary<(bool Transparent, string Name), LinkedListNode<Entry>> entries
            = new Dictionary<(bool Transparent, string Name), LinkedListNode<Entry>>();
        // 先頭ほど最近使ったもの
        private readonly LinkedList<Entry> recent = new LinkedList<Entry>();
        private long cachedBytes;
        private IList<string> bitmapDirectories;

        private sealed class Entry
        {
            public (bool Transparent, string Name) Key;
            public SKBitmap Bitmap;
            public long Bytes;
        }

        public ImageCache(SRC src, long capacityBytes = DefaultCapacityBytes)
        {
            this.src = src;
            CapacityBytes = capacityBytes;
        }

        /// <summary>キャッシュする画素の合計の上限（バイト）</summary>
        public long CapacityBytes { get; }

        /// <summary>キャッシュしている画素の合計（バイト）</summary>
        public long CachedBytes
        {
            get
            {
                lock (sync)
                {
                    return cachedBytes;
                }
            }
        }

        /// <summary>キャッシュしている件数（見つからなかった画像も含む）</summary>
        public int Count
        {
            get
            {
                lock (sync)
                {
                    return entries.Count;
                }
            }
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
            return GetOrAdd((false, name), () => Load(name));
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
            return GetOrAdd((true, name), () => MakeTransparent(Get(name), SKColors.White));
        }

        private SKBitmap GetOrAdd((bool Transparent, string Name) key, Func<SKBitmap> load)
        {
            lock (sync)
            {
                if (entries.TryGetValue(key, out var node))
                {
                    recent.Remove(node);
                    recent.AddFirst(node);
                    return node.Value.Bitmap;
                }
            }
            // 読み込み（デコード）はロックの外で行う。同時に同じ画像を読んだ場合は先に登録したものを使う
            var bitmap = load();
            lock (sync)
            {
                if (entries.TryGetValue(key, out var node))
                {
                    return node.Value.Bitmap;
                }
                var entry = new Entry
                {
                    Key = key,
                    Bitmap = bitmap,
                    Bytes = bitmap == null ? 0 : (long)bitmap.RowBytes * bitmap.Height,
                };
                entries[key] = recent.AddFirst(entry);
                cachedBytes += entry.Bytes;
                // 今読んだものは残す
                while (cachedBytes > CapacityBytes && recent.Count > 1)
                {
                    var last = recent.Last;
                    recent.RemoveLast();
                    entries.Remove(last.Value.Key);
                    cachedBytes -= last.Value.Bytes;
                }
                return bitmap;
            }
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
