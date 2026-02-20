using Microsoft.Extensions.Logging;
using SRCCore;
using SRCSharpForm.Extensions;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Caching;

namespace SRCSharpForm.Resoruces
{
    /// <summary>
    /// 画像のバッファ。
    /// <see cref="MemoryCache"/>のほぼ既定の設定でキャッシュしている。
    /// </summary>
    public class ImageBuffer
    {
        // キャッシュインスタンスの共有を考えてキーに空間を持っておく。
        private const string IMAGE_KEY_PREFIX = "img:";
        private const string TRANSPARENT_IMAGE_KEY_PREFIX = "t:";

        // XXX 何かバッファを増やすときには共有する
        private MemoryCache cache;
        private CacheItemPolicy cacheItemPolicy;

        private SRC SRC;

        private IList<string> bitmapDirectories;

        public ImageBuffer(SRC src)
        {
            SRC = src;
            InitFileSystemInfo();
            cacheItemPolicy = new CacheItemPolicy()
            {
                RemovedCallback = (arg) =>
                {
                    SRC.Log.LogTrace($"ImageBuffer Removed {arg.RemovedReason} {arg.CacheItem.Key}");
                },
            };
            cache = new MemoryCache("ImageBuffer", new System.Collections.Specialized.NameValueCollection()
            {
                ["CacheMemoryLimitMegabytes"] = SRC.SystemConfig.MaxImageBufferByteSize > 0
                    ? $"{SRC.SystemConfig.MaxImageBufferByteSize / 1024 / 1024}"
                    : "128",
            });
        }

        public bool Load(string name)
        {
            var path = SearchFile(name);
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            var image = NormalizeImage(name, Image.FromStream(SRC.FileSystem.Open(path)));
            cache.Add(ToKey(name), image, cacheItemPolicy);
            return true;
        }

        private Bitmap NormalizeImage(string name, Image image)
        {
            if (image.PixelFormat.HasFlag(PixelFormat.Alpha))
            {
                // 画像がアルファチャネルを持っているなら透過画像として登録
                // PixelFormat.Alpha フラグはフォーマット上のアルファチャネル有無を示すが、
                // 実際に全ピクセルが透過されているかどうかはまた別の話。
                // 現状の実装は保守的だが機能上問題ない（透過情報を保持したまま登録する）。
                Transparent(ToTransparentKey(name), image);
                // アルファチャネルを消して返す
                image = image.RemoveAlpha(Color.White);
            }

            return new Bitmap(image);
        }

        public Image Get(string name)
        {
            var key = ToKey(name);
            if (cache.Contains(key))
            {
                return (Image)cache[key];
            }
            if (Load(name))
            {
                return (Image)cache[key];
            }
            else
            {
                return null;
            }
        }

        public Image GetTransparent(string name)
        {
            return GetTransparent(name, Color.White);
        }

        public Image GetTransparent(string name, Color transparentColor)
        {
            var key = ToTransparentKey(name);
            if (cache.Contains(key))
            {
                return (Image)cache[key];
            }
            var image = Get(name);
            if (image == null)
            {
                return null;
            }
            return Transparent(key, image, transparentColor);
        }

        private Image Transparent(string key, Image image, Color transparentColor)
        {
            var transparentImage = new Bitmap(image);
            transparentImage.MakeTransparent(transparentColor);
            cache.Add(key, transparentImage, cacheItemPolicy);
            return transparentImage;
        }

        private Image Transparent(string key, Image image)
        {
            var transparentImage = new Bitmap(image);
            cache.Add(key, transparentImage, cacheItemPolicy);
            return transparentImage;
        }

        public string ToKey(string name)
        {
            return IMAGE_KEY_PREFIX + name.ToLower();
        }

        public string ToTransparentKey(string name)
        {
            return TRANSPARENT_IMAGE_KEY_PREFIX + ToKey(name);
        }

        public string SearchFile(string name)
        {
            var path =  ImageFilders()
                .Select(x => SRC.FileSystem.PathCombine(x, name))
                .FirstOrDefault(x => SRC.FileSystem.FileExists(x));
            SRC.LogDebug($"{name} -> {path}");
            return path;
        }

        private IEnumerable<string> ImageFilders()
        {
            return bitmapDirectories;
        }

        public void InitFileSystemInfo()
        {
            // 当面事前チェックやマップ向けディレクトリの特別視はしない
            var subDirectories = new List<string>()
            {
                "",
                SRC.FileSystem.PathCombine("Bitmap"),
                SRC.FileSystem.PathCombine("Bitmap", "Anime"),
                SRC.FileSystem.PathCombine("Bitmap", "Event"),
                SRC.FileSystem.PathCombine("Bitmap", "Cutin"),
                SRC.FileSystem.PathCombine("Bitmap", "Pilot"),
                SRC.FileSystem.PathCombine("Bitmap", "Unit"),
                SRC.FileSystem.PathCombine("Bitmap", "Map"),
            };
            bitmapDirectories = subDirectories
                .ToList();
        }
    }
}
