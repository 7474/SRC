using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCCore;
using SRCCore.Filesystem;
using System;
using System.IO;

namespace SRCRender.Tests
{
    [TestClass]
    public class ImageCacheTests
    {
        private const int Size = 100;
        // 100x100 の RGBA
        private const long ImageBytes = Size * Size * 4;

        private string dir;
        private SRC src;

        [TestInitialize]
        public void Setup()
        {
            dir = Path.Combine(Path.GetTempPath(), "srcs-imagecache-" + Guid.NewGuid());
            Directory.CreateDirectory(dir);
            foreach (var name in new[] { "a", "b", "c" })
            {
                using (var bitmap = new SKBitmap(Size, Size, SKColorType.Rgba8888, SKAlphaType.Premul))
                using (var file = File.Create(Path.Combine(dir, name + ".png")))
                {
                    bitmap.Erase(SKColors.White);
                    bitmap.Encode(file, SKEncodedImageFormat.Png, 100);
                }
            }
            src = new SRC { FileSystem = new LocalFileSystem() };
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(dir, true);
        }

        private string PathOf(string name) => Path.Combine(dir, name + ".png");

        [TestMethod]
        public void 同じ画像はキャッシュから返す()
        {
            var cache = new ImageCache(src);
            var a = cache.Get(PathOf("a"));
            Assert.IsNotNull(a);
            Assert.AreSame(a, cache.Get(PathOf("a")));
            Assert.AreEqual(ImageBytes, cache.CachedBytes);
        }

        [TestMethod]
        public void 上限を超えたら長く使っていない画像から外す()
        {
            // 2 枚分
            var cache = new ImageCache(src, ImageBytes * 2);
            var a = cache.Get(PathOf("a"));
            var b = cache.Get(PathOf("b"));
            // a を使い直すので、次に外れるのは b
            Assert.AreSame(a, cache.Get(PathOf("a")));
            var c = cache.Get(PathOf("c"));

            Assert.AreEqual(2, cache.Count);
            Assert.AreEqual(ImageBytes * 2, cache.CachedBytes);
            Assert.AreSame(a, cache.Get(PathOf("a")));
            Assert.AreSame(c, cache.Get(PathOf("c")));
            // 外した画像は読み直す。外した画像は使える（Dispose しない）
            var b2 = cache.Get(PathOf("b"));
            Assert.IsNotNull(b2);
            Assert.AreNotSame(b, b2);
            Assert.AreEqual(Size, b.Width);
            Assert.AreEqual(SKColors.White, b.GetPixel(0, 0));
            Assert.IsTrue(cache.CachedBytes <= ImageBytes * 2);
        }

        [TestMethod]
        public void アルファチャネルで透過を指定した画像は白を抜かずにそのまま使う()
        {
            // 左半分は透明、右半分は不透明な白（#1212）
            var path = Path.Combine(dir, "alpha.png");
            using (var bitmap = new SKBitmap(Size, Size, SKColorType.Rgba8888, SKAlphaType.Unpremul))
            {
                bitmap.Erase(SKColors.White);
                for (var y = 0; y < Size; y++)
                {
                    for (var x = 0; x < Size / 2; x++)
                    {
                        bitmap.SetPixel(x, y, SKColors.Transparent);
                    }
                }
                using (var file = File.Create(path))
                {
                    bitmap.Encode(file, SKEncodedImageFormat.Png, 100);
                }
            }
            var cache = new ImageCache(src);
            var t = cache.GetTransparent(path);
            Assert.AreEqual(0, t.GetPixel(0, 0).Alpha);
            // 白でも抜かない
            Assert.AreEqual(SKColors.White, t.GetPixel(Size - 1, 0));

            // アルファチャネルの無い（全画素が不透明な）画像は、これまでどおり白を抜く
            Assert.AreEqual(0, cache.GetTransparent(PathOf("a")).GetPixel(0, 0).Alpha);
        }

        [TestMethod]
        public void 透過色の画像も上限に数える()
        {
            var cache = new ImageCache(src, ImageBytes * 2);
            var t = cache.GetTransparent(PathOf("a"));
            Assert.IsNotNull(t);
            // 白は透過になる
            Assert.AreEqual((byte)0, t.GetPixel(0, 0).Alpha);
            // 元の画像と透過色の画像
            Assert.AreEqual(2, cache.Count);
            Assert.AreEqual(ImageBytes * 2, cache.CachedBytes);
            cache.Get(PathOf("b"));
            Assert.IsTrue(cache.CachedBytes <= ImageBytes * 2);
        }

        [TestMethod]
        public void 上限より大きい画像も読める()
        {
            var cache = new ImageCache(src, 1);
            var a = cache.Get(PathOf("a"));
            Assert.IsNotNull(a);
            Assert.AreEqual(1, cache.Count);
            var b = cache.Get(PathOf("b"));
            Assert.IsNotNull(b);
            Assert.AreEqual(1, cache.Count);
        }

        [TestMethod]
        public void 見つからない画像は_null()
        {
            var cache = new ImageCache(src);
            Assert.IsNull(cache.Get(PathOf("missing")));
            Assert.IsNull(cache.GetTransparent(PathOf("missing")));
            Assert.AreEqual(0, cache.CachedBytes);
        }
    }
}
