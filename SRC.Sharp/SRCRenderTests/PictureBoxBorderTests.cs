using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCRender.Compat;
using System.Linq;

namespace SRCRender.Tests
{
    /// <summary>
    /// 元の SRC の 3D の PictureBox（BorderStyle = 1、Appearance = 1）と同じく、
    /// へこんだ 2px の枠を描き、画像は枠の内側（ScaleWidth × ScaleHeight）に表示する。7474/SRC#1424
    /// </summary>
    [TestClass]
    public class PictureBoxBorderTests
    {
        [TestMethod]
        public void ClientSize_Border3D_ExcludesBorder()
        {
            // 元の SRC の picFace（Width = Height = 1020 twip、ScaleWidth = ScaleHeight = 64）
            var face = new PictureBox { Width = 68, Height = 68, Border3D = true };
            Assert.AreEqual(new System.Drawing.Size(64, 64), face.ClientSize);
            Assert.AreEqual(64, face.NewImageIfNull().Image.Width);
            Assert.AreEqual(64, face.Image.Height);

            var plain = new PictureBox { Width = 32, Height = 32 };
            Assert.AreEqual(new System.Drawing.Size(32, 32), plain.ClientSize);
        }

        [TestMethod]
        public void DrawBar_FillsInsideBorder_WithTruncatedWidth()
        {
            // 元の SRC の Message.frm の picHP1（1740 × 120 twip、ScaleWidth = 112、ScaleHeight = 4）
            var hp = new PictureBox { Width = 116, Height = 8, Border3D = true };
            hp.DrawBar(1f / 3, new SolidBrush(System.Drawing.Color.Red), new SolidBrush(System.Drawing.Color.Lime));
            Assert.AreEqual(112, hp.Image.Width);
            Assert.AreEqual(4, hp.Image.Height);
            // 112 * 1 \ 3 = 37
            Assert.AreEqual(new SKColor(0, 255, 0), hp.Image.GetPixel(36, 0));
            Assert.AreEqual(new SKColor(255, 0, 0), hp.Image.GetPixel(37, 0));
        }

        [TestMethod]
        public void BarWidth_TruncatesLikeIntegerDivision()
        {
            Assert.AreEqual(112, PictureBox.BarWidth(112, 1f));
            Assert.AreEqual(48, PictureBox.BarWidth(112, 3f / 7));
            Assert.AreEqual(111, PictureBox.BarWidth(112, 999f / 1000));
            Assert.AreEqual(0, PictureBox.BarWidth(112, 0f));
            Assert.AreEqual(0, PictureBox.BarWidth(112, float.NaN));
        }

        [TestMethod]
        public void StatusFace_64x64IsDrawnAtActualSizeInsideSunkenBorder()
        {
            // 列ごとに色の違う 64 × 64 の顔画像
            var image = new SKBitmap(64, 64, SKColorType.Bgra8888, SKAlphaType.Premul);
            for (var y = 0; y < 64; y++)
            {
                for (var x = 0; x < 64; x++)
                {
                    image.SetPixel(x, y, new SKColor((byte)(x * 4), (byte)(y * 4), 128));
                }
            }
            var face = new PictureBox { Left = 4, Top = 4, Width = 68, Height = 68, StretchImage = true, Border3D = true, Image = image };

            using (var renderer = new ScreenRenderer())
            using (var screen = new SKBitmap(ScreenLayout.Width, ScreenLayout.Height))
            using (var canvas = new SKCanvas(screen))
            {
                renderer.Render(canvas, new ScreenState { MainFormVisible = true }, new ScreenRenderer.ScreenLayers(null, new[] { face }, null));
                var left = ScreenLayout.StatusArea.Left + 4;
                var top = 4;

                // 1 ドットずつ等倍で並ぶ（64 → 68 に引き伸ばさない）
                var columns = Enumerable.Range(0, 64).Select(i => screen.GetPixel(left + 2 + i, top + 30).Red / 4).ToArray();
                CollectionAssert.AreEqual(Enumerable.Range(0, 64).ToArray(), columns);
                var rows = Enumerable.Range(0, 64).Select(i => screen.GetPixel(left + 30, top + 2 + i).Green / 4).ToArray();
                CollectionAssert.AreEqual(Enumerable.Range(0, 64).ToArray(), rows);

                // へこんだ枠（外側：左上が影・右下が白、内側：左上が濃い影・右下が明るい灰色）
                Assert.AreEqual(new SKColor(128, 128, 128), screen.GetPixel(left, top + 30));
                Assert.AreEqual(new SKColor(64, 64, 64), screen.GetPixel(left + 1, top + 30));
                Assert.AreEqual(new SKColor(192, 192, 192), screen.GetPixel(left + 66, top + 30));
                Assert.AreEqual(SKColors.White, screen.GetPixel(left + 67, top + 30));
                Assert.AreEqual(new SKColor(128, 128, 128), screen.GetPixel(left + 30, top));
                Assert.AreEqual(SKColors.White, screen.GetPixel(left + 30, top + 67));
            }
        }
    }
}
