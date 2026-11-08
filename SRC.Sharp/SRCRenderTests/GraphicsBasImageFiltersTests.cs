using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCPlay;
using System.IO;
using System.Linq;

namespace SRCRender.Tests
{
    /// <summary>
    /// 元の SRC の Graphics.bas の画像の加工（Bright・Dark・Monotone・Sepia・Sunset・Water・HReverse・VReverse・
    /// NegPosReverse・Silhouette・Rotate・ColorFilter）と同じ値になるか。
    /// 期待値は VB の式（Double から Long・Byte への代入は銀行丸め、\ は切り捨て）で計算したもの。7474/SRC#1498
    /// </summary>
    [TestClass]
    public class GraphicsBasImageFiltersTests
    {
        private static readonly SKColor[] Samples =
        {
            new SKColor(200, 100, 50),
            new SKColor(10, 20, 30),
            new SKColor(255, 255, 255),
            new SKColor(0, 0, 0),
            new SKColor(13, 77, 201),
            new SKColor(250, 240, 230),
        };

        private static SKBitmap Make(int w, int h, params SKColor[] colors)
        {
            var b = new SKBitmap(w, h, SKColorType.Bgra8888, SKAlphaType.Premul);
            for (var i = 0; i < w * h; i++)
            {
                b.SetPixel(i % w, i / w, colors[i]);
            }
            return b;
        }

        private static SKBitmap MakeSamples() => Make(Samples.Length, 1, Samples);

        private static SKColor[] PixelsOf(SKBitmap b)
        {
            var result = new SKColor[b.Width * b.Height];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = b.GetPixel(i % b.Width, i / b.Width);
            }
            return result;
        }

        private static void AssertPixels(SKColor[] expected, SKBitmap actual)
        {
            CollectionAssert.AreEqual(expected, PixelsOf(actual));
        }

        [TestMethod]
        public void Monotone_UsesVbCoefficientsAndBankersRounding()
        {
            // rgb = 0.299 * R + 0.587 * G + 0.114 * B（Long への代入で銀行丸め）
            using var b = ImageFilters.ApplyMonotone(MakeSamples());
            AssertPixels(new[] { 124, 18, 255, 0, 72, 242 }.Select(v => new SKColor((byte)v, (byte)v, (byte)v)).ToArray(), b);
        }

        [TestMethod]
        public void Sepia_MatchesVb()
        {
            using var b = ImageFilters.ApplySepia(MakeSamples());
            AssertPixels(new[]
            {
                new SKColor(136, 112, 87),
                new SKColor(20, 16, 13),
                new SKColor(255, 230, 178),
                new SKColor(0, 0, 0),
                new SKColor(79, 65, 50),
                new SKColor(255, 218, 169),
            }, b);
        }

        [TestMethod]
        public void Sunset_MatchesVb()
        {
            using var b = ImageFilters.ApplySunset(MakeSamples());
            AssertPixels(new[]
            {
                new SKColor(201, 70, 35),
                new SKColor(25, 11, 10),
                new SKColor(255, 153, 102),
                new SKColor(0, 0, 0),
                new SKColor(96, 44, 55),
                new SKColor(255, 145, 94),
            }, b);
        }

        [TestMethod]
        public void Water_MatchesVb()
        {
            using var b = ImageFilters.ApplyWater(MakeSamples());
            AssertPixels(new[]
            {
                new SKColor(74, 99, 124),
                new SKColor(11, 14, 18),
                new SKColor(153, 204, 255),
                new SKColor(0, 0, 0),
                new SKColor(43, 58, 72),
                new SKColor(145, 194, 242),
            }, b);
        }

        [TestMethod]
        public void Dark_HalvesWithIntegerDivision()
        {
            using var b = ImageFilters.ApplyDark(MakeSamples());
            AssertPixels(new[]
            {
                new SKColor(100, 50, 25),
                new SKColor(5, 10, 15),
                new SKColor(127, 127, 127),
                new SKColor(0, 0, 0),
                new SKColor(6, 38, 100),
                new SKColor(125, 120, 115),
            }, b);
        }

        [TestMethod]
        public void NegPos_Inverts()
        {
            using var b = ImageFilters.ApplyNegPos(MakeSamples());
            AssertPixels(Samples.Select(c => new SKColor((byte)(255 - c.Red), (byte)(255 - c.Green), (byte)(255 - c.Blue))).ToArray(), b);
        }

        [TestMethod]
        public void Bright_AddsEightyUpTo255()
        {
            using var b = ImageFilters.ApplyBright(MakeSamples());
            AssertPixels(new[]
            {
                new SKColor(255, 180, 130),
                new SKColor(90, 100, 110),
                new SKColor(255, 255, 255),
                new SKColor(80, 80, 80),
                new SKColor(93, 157, 255),
                new SKColor(255, 255, 255),
            }, b);
        }

        [TestMethod]
        public void Bright_Transparent_KeepsMaxAndStopsAt254()
        {
            // 透過のときは 255 の色はそのまま、ほかは 254 で止める（背景色の白にならないように）
            using var b = ImageFilters.ApplyBright(MakeSamples(), transparent: true);
            AssertPixels(new[]
            {
                new SKColor(254, 180, 130),
                new SKColor(90, 100, 110),
                new SKColor(255, 255, 255),
                new SKColor(80, 80, 80),
                new SKColor(93, 157, 254),
                new SKColor(254, 254, 254),
            }, b);
        }

        [TestMethod]
        public void ColorFilter_UsesPercentAndIntegerDivision()
        {
            // tratio = 30、(元の色 * 70 + 赤 * 30) \ 100
            using var b = ImageFilters.ApplyColorFilter(MakeSamples(), System.Drawing.Color.FromArgb(255, 0, 0), 0.3);
            AssertPixels(new[]
            {
                new SKColor(216, 70, 35),
                new SKColor(83, 14, 21),
                new SKColor(255, 178, 178),
                new SKColor(76, 0, 0),
                new SKColor(85, 53, 140),
                new SKColor(251, 168, 161),
            }, b);
        }

        [TestMethod]
        public void ColorFilter_ZeroPercent_LeavesImage()
        {
            // 100 * 0.004 = 0.4 は 0 に丸めるので何もしない
            using var b = ImageFilters.ApplyColorFilter(MakeSamples(), System.Drawing.Color.FromArgb(255, 0, 0), 0.004);
            AssertPixels(Samples, b);
        }

        [TestMethod]
        public void ColorFilter_Transparent_SameColorAsBackground_ShiftsFilterColor()
        {
            // 背景色と同じ白のフィルタは、透過で消えないように赤を 254 にずらす
            using var b = ImageFilters.ApplyColorFilter(Make(1, 1, new SKColor(0, 0, 0)), System.Drawing.Color.White, 1.0,
                transparent: true, background: SKColors.White);
            AssertPixels(new[] { new SKColor(254, 255, 255) }, b);
        }

        [TestMethod]
        public void Silhouette_BackgroundToWhiteOthersBlack()
        {
            using var b = ImageFilters.ApplySilhouette(MakeSamples());
            AssertPixels(new[]
            {
                SKColors.Black, SKColors.Black, SKColors.White, SKColors.Black, SKColors.Black, SKColors.Black,
            }, b);

            // 背景色を指定したときは、その色が白に、白も含めてほかは黒になる
            using var c = ImageFilters.ApplySilhouette(MakeSamples(), background: new SKColor(10, 20, 30));
            AssertPixels(new[]
            {
                SKColors.Black, SKColors.White, SKColors.Black, SKColors.Black, SKColors.Black, SKColors.Black,
            }, c);
        }

        [TestMethod]
        public void Filters_KeepTransparentPixels()
        {
            using var b = ImageFilters.ApplyNegPos(Make(2, 1, SKColors.Transparent, new SKColor(0, 0, 0)));
            Assert.AreEqual(0, b.GetPixel(0, 0).Alpha);
            Assert.AreEqual(SKColors.White, b.GetPixel(1, 0));
        }

        private static SKBitmap MakeIndexed(int w, int h)
        {
            // 画素の番号を色にする（青 = 番号、赤 = 1）
            return Make(w, h, Enumerable.Range(0, w * h).Select(i => new SKColor(1, 0, (byte)i)).ToArray());
        }

        private static void AssertIndexes(int[] expected, SKBitmap actual, SKColor background)
        {
            var pixels = PixelsOf(actual);
            var expectedColors = expected.Select(i => i < 0 ? background : new SKColor(1, 0, (byte)i)).ToArray();
            CollectionAssert.AreEqual(expectedColors, pixels);
        }

        [TestMethod]
        public void HReverse_VReverse_SwapPixels()
        {
            using var h = ImageFilters.Flip(MakeIndexed(3, 2), true, false);
            AssertIndexes(new[] { 2, 1, 0, 5, 4, 3 }, h, SKColors.Transparent);
            using var v = ImageFilters.Flip(MakeIndexed(3, 3), false, true);
            AssertIndexes(new[] { 6, 7, 8, 3, 4, 5, 0, 1, 2 }, v, SKColors.Transparent);
        }

        [TestMethod]
        public void Rotate_Square90_And180_And270()
        {
            var bg = new SKColor(9, 9, 9);
            using var r90 = ImageFilters.Rotate(MakeIndexed(3, 3), 90, bg);
            AssertIndexes(new[] { 6, 3, 0, 7, 4, 1, 8, 5, 2 }, r90, bg);
            using var r180 = ImageFilters.Rotate(MakeIndexed(3, 2), 180, bg);
            AssertIndexes(new[] { 5, 4, 3, 2, 1, 0 }, r180, bg);
            using var r270 = ImageFilters.Rotate(MakeIndexed(3, 3), -90, bg);
            AssertIndexes(new[] { 2, 5, 8, 1, 4, 7, 0, 3, 6 }, r270, bg);
        }

        [TestMethod]
        public void Rotate_ArbitraryAngle_UsesNearestPixelLikeVb()
        {
            // VB の Rotate の式（中心を原点に、CLng で最も近い画素を取る。範囲外は背景色）で計算した値
            var bg = new SKColor(9, 9, 9);
            using var r30 = ImageFilters.Rotate(MakeIndexed(5, 4), 30, bg);
            AssertIndexes(new[] { 5, 5, 1, 2, -1, 10, 11, 7, 8, 3, 16, 11, 12, 8, 9, -1, 17, 18, 14, 14 }, r30, bg);
            using var r45 = ImageFilters.Rotate(MakeIndexed(5, 4), 45, bg);
            AssertIndexes(new[] { 10, 5, 1, 2, -1, 15, 11, 7, 2, 3, 16, 17, 12, 8, 4, -1, 17, 18, 14, 9 }, r45, bg);
            using var r200 = ImageFilters.Rotate(MakeIndexed(5, 4), 200, bg);
            AssertIndexes(new[] { 14, 18, 18, 17, -1, 9, 13, 12, 11, 15, 4, 8, 7, 6, 10, -1, 2, 1, 1, 5 }, r200, bg);
        }

        [TestMethod]
        public void Rotate_NonSquare90_UsesGeneralFormula()
        {
            // 正方形でない画像の 90 度は、元の SRC でも一般の式で回す（はみ出した部分は切れ、空いた部分は背景色）
            var bg = new SKColor(9, 9, 9);
            using var r = ImageFilters.Rotate(MakeIndexed(5, 4), 90, bg);
            AssertIndexes(new[] { -1, 10, 10, 0, 0, -1, 12, 12, 2, 2, -1, 12, 12, 2, 2, -1, 14, 14, 4, 4 }, r, bg);
        }

        [TestMethod]
        public void PaintPicture_Rotate_FillsWithBackgroundColorOption()
        {
            // 元の SRC の DrawPicture は、透過しないときの回転の空いた部分を BGColor（既定は白、色の指定で変わる）で塗る
            using (var workspace = new ScenarioWorkspace())
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
                workspace.WriteTestImage(Path.Combine("Bitmap", "Event", "test.png"));
                workspace.Write("test.eve",
                    "プロローグ:",
                    "ChangeMap test.map",
                    "Show",
                    "PaintPicture test.png 100 100 右回転 45",
                    "PaintPicture test.png 200 100 右回転 45 #00FF00",
                    "Refresh",
                    "Talk システム",
                    "回転",
                    "End",
                    "Quit",
                    "");

                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));

                Assert.AreEqual("回転", runner.WaitForMessage());
                using (var bitmap = SKBitmap.Decode(runner.SnapshotPng()))
                {
                    Assert.AreEqual(SKColors.White, bitmap.GetPixel(100 + 1, 100 + 1));
                    Assert.AreEqual(new SKColor(0, 255, 0), bitmap.GetPixel(200 + 1, 100 + 1));
                }
                runner.ClickMessage();
                runner.WaitForTermination();
            }
        }
    }
}
