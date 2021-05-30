using System.Runtime.Caching;
using SRCCore;
using SRCCore.Lib;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Drawing.Imaging;
using SRCSharpForm.Extensions;

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

        private IList<string> existBitmapDirectories;
        private IList<string> existMapBitmapDirectories;

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
            var image = NormalizeImage(name, Image.FromFile(path));
            cache.Add(ToKey(name), image, cacheItemPolicy);
            return true;
        }

        private Bitmap NormalizeImage(string name, Image image)
        {
            if (image.PixelFormat.HasFlag(PixelFormat.Alpha))
            {
                // 画像がアルファチャネルを持っているなら透過画像として登録
                // TODO アルファチャネル精査
                // フォーマット上アルファチャネルがあるかと、実際に透過されているかはまた別なのでちょっと微妙かもしれない。
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
            return ImageFilders()
                .Select(x => SRC.FileSystem.PathCombine(x, name))
                .FirstOrDefault(x => File.Exists(x));
        }

        private IEnumerable<string> ImageFilders()
        {
            return existBitmapDirectories;
        }

        public void InitFileSystemInfo()
        {
            // TODO この辺りは走査順だけ定義して他は FileSystem に任せる
            // 各フォルダにBitmapフォルダがあるかチェック
            var baseDirectories = new List<string>()
            {
                SRC.ScenarioPath,
                SRC.ExtDataPath,
                SRC.ExtDataPath2,
                SRC.AppPath,
            }.Where(x => !string.IsNullOrEmpty(FileSystem.Dir(x, FileAttribute.Directory)))
            .ToList();

            var subDirectories = new List<string>()
            {
                SRC.FileSystem.PathCombine("Bitmap"),
                SRC.FileSystem.PathCombine("Bitmap", "Anime"),
                SRC.FileSystem.PathCombine("Bitmap", "Event"),
                SRC.FileSystem.PathCombine("Bitmap", "Cutin"),
                SRC.FileSystem.PathCombine("Bitmap", "Pilot"),
                SRC.FileSystem.PathCombine("Bitmap", "Unit"),
                SRC.FileSystem.PathCombine("Bitmap", "Map"),
            };

            // XXX 走査順とか多分固定的に持ったほうがよさそう。
            existBitmapDirectories = subDirectories
                .SelectMany(x => baseDirectories.Select(y => SRC.FileSystem.PathCombine(y, x)))
                .Where(x => !string.IsNullOrEmpty(FileSystem.Dir(x, FileAttribute.Directory)))
                .ToList();
            existMapBitmapDirectories = existBitmapDirectories.Where(x => x.EndsWith("Map")).ToList();

            //// 画面の色数を参照
            //display_byte_pixel = GetDeviceCaps(MainForm.picMain(0).hDC, BITSPIXEL) / 8;
            //init_draw_pitcure = true;
        }
    }
}
