using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;
using System.Drawing;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// ColorExtension の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class ColorExtensionMoreTests
    {
        // ──────────────────────────────────────────────
        // ToHexString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToHexString_Black_Returns000000()
        {
            Assert.AreEqual("#000000", Color.Black.ToHexString());
        }

        [TestMethod]
        public void ToHexString_White_ReturnsFFFFFF()
        {
            Assert.AreEqual("#ffffff", Color.White.ToHexString());
        }

        [TestMethod]
        public void ToHexString_Green_Returns00FF00()
        {
            Assert.AreEqual("#00ff00", Color.Lime.ToHexString());
        }

        [TestMethod]
        public void ToHexString_AllZero_Returns000000()
        {
            var color = Color.FromArgb(0, 0, 0);
            Assert.AreEqual("#000000", color.ToHexString());
        }

        [TestMethod]
        public void ToHexString_128_128_128_Returns808080()
        {
            var color = Color.FromArgb(128, 128, 128);
            Assert.AreEqual("#808080", color.ToHexString());
        }

        [TestMethod]
        public void ToHexString_AlphaIgnored_UsesRGB()
        {
            // Alpha channel is ignored (only R, G, B used)
            var color = Color.FromArgb(255, 255, 0, 0);
            Assert.AreEqual("#ff0000", color.ToHexString());
        }

        // ──────────────────────────────────────────────
        // FromHexString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FromHexString_Black_ReturnsBlack()
        {
            var color = ColorExtension.FromHexString("#000000");
            Assert.AreEqual(0, color.R);
            Assert.AreEqual(0, color.G);
            Assert.AreEqual(0, color.B);
        }

        [TestMethod]
        public void FromHexString_White_ReturnsWhite()
        {
            var color = ColorExtension.FromHexString("#ffffff");
            Assert.AreEqual(255, color.R);
            Assert.AreEqual(255, color.G);
            Assert.AreEqual(255, color.B);
        }

        [TestMethod]
        public void FromHexString_UpperCase_ParsesCorrectly()
        {
            var color = ColorExtension.FromHexString("#FF0000");
            Assert.AreEqual(255, color.R);
            Assert.AreEqual(0, color.G);
            Assert.AreEqual(0, color.B);
        }

        [TestMethod]
        public void FromHexString_NamedColor_ParsesCorrectly()
        {
            var color = ColorExtension.FromHexString("red");
            Assert.AreEqual(255, color.R);
            Assert.AreEqual(0, color.G);
            Assert.AreEqual(0, color.B);
        }

        // ──────────────────────────────────────────────
        // TryFromHexString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TryFromHexString_Black_ReturnsTrue()
        {
            var result = ColorExtension.TryFromHexString("#000000", out var color);
            Assert.IsTrue(result);
            Assert.AreEqual(0, color.R);
        }

        [TestMethod]
        public void TryFromHexString_White_ReturnsTrue()
        {
            var result = ColorExtension.TryFromHexString("#ffffff", out var color);
            Assert.IsTrue(result);
            Assert.AreEqual(255, color.R);
            Assert.AreEqual(255, color.G);
            Assert.AreEqual(255, color.B);
        }

        [TestMethod]
        public void TryFromHexString_InvalidString_ReturnsFalse()
        {
            var result = ColorExtension.TryFromHexString("notAColor", out var color);
            Assert.IsFalse(result);
            Assert.AreEqual(Color.Empty, color);
        }

        [TestMethod]
        public void TryFromHexString_MalformedHex_ReturnsFalse()
        {
            // 有効なカラー名でも16進でもない文字列
            var result = ColorExtension.TryFromHexString("#xyz123", out var color);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TryFromHexString_GreenColor_ReturnsTrue()
        {
            var result = ColorExtension.TryFromHexString("#00ff00", out var color);
            Assert.IsTrue(result);
            Assert.AreEqual(0, color.R);
            Assert.AreEqual(255, color.G);
            Assert.AreEqual(0, color.B);
        }

        [TestMethod]
        public void TryFromHexString_RoundTrip_OriginalAndParsed_AreEqual()
        {
            var originalColor = Color.FromArgb(64, 128, 192);
            var hexStr = originalColor.ToHexString();
            var result = ColorExtension.TryFromHexString(hexStr, out var parsedColor);
            Assert.IsTrue(result);
            Assert.AreEqual(originalColor.R, parsedColor.R);
            Assert.AreEqual(originalColor.G, parsedColor.G);
            Assert.AreEqual(originalColor.B, parsedColor.B);
        }
    }
}
