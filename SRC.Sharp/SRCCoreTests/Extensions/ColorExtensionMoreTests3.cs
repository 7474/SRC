using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;
using System.Drawing;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// ColorExtension の追加ユニットテスト（その3）
    /// </summary>
    [TestClass]
    public class ColorExtensionMoreTests3
    {
        // ──────────────────────────────────────────────
        // ToHexString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToHexString_Black_ReturnsBlackHex()
        {
            Assert.AreEqual("#000000", Color.Black.ToHexString());
        }

        [TestMethod]
        public void ToHexString_White_ReturnsWhiteHex()
        {
            Assert.AreEqual("#ffffff", Color.White.ToHexString());
        }

        [TestMethod]
        public void ToHexString_Green_ReturnsGreenHex()
        {
            Assert.AreEqual("#008000", Color.Green.ToHexString());
        }

        [TestMethod]
        public void ToHexString_Red_ReturnsRedHex()
        {
            Assert.AreEqual("#ff0000", Color.Red.ToHexString());
        }

        [TestMethod]
        public void ToHexString_Blue_ReturnsBlueHex()
        {
            Assert.AreEqual("#0000ff", Color.Blue.ToHexString());
        }

        [TestMethod]
        public void ToHexString_CustomColor_ReturnsCorrectHex()
        {
            var c = Color.FromArgb(18, 52, 86);  // #123456
            Assert.AreEqual("#123456", c.ToHexString());
        }

        [TestMethod]
        public void ToHexString_AlphaIgnored_OnlyRgbInOutput()
        {
            // Alpha channel should not appear in output
            var c = Color.FromArgb(200, 255, 0, 0);
            var hex = c.ToHexString();
            Assert.AreEqual(7, hex.Length);  // "#rrggbb"
            Assert.AreEqual('#', hex[0]);
        }

        // ──────────────────────────────────────────────
        // FromHexString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FromHexString_RedHex_ReturnsRedColor()
        {
            var c = ColorExtension.FromHexString("#ff0000");
            Assert.AreEqual(255, c.R);
            Assert.AreEqual(0, c.G);
            Assert.AreEqual(0, c.B);
        }

        [TestMethod]
        public void FromHexString_BlackHex_ReturnsBlackColor()
        {
            var c = ColorExtension.FromHexString("#000000");
            Assert.AreEqual(0, c.R);
            Assert.AreEqual(0, c.G);
            Assert.AreEqual(0, c.B);
        }

        [TestMethod]
        public void FromHexString_WhiteHex_ReturnsWhiteColor()
        {
            var c = ColorExtension.FromHexString("#ffffff");
            Assert.AreEqual(255, c.R);
            Assert.AreEqual(255, c.G);
            Assert.AreEqual(255, c.B);
        }

        // ──────────────────────────────────────────────
        // TryFromHexString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TryFromHexString_ValidBlue_ReturnsTrueAndColor()
        {
            var ok = ColorExtension.TryFromHexString("#0000ff", out var c);
            Assert.IsTrue(ok);
            Assert.AreEqual(0, c.R);
            Assert.AreEqual(0, c.G);
            Assert.AreEqual(255, c.B);
        }

        [TestMethod]
        public void TryFromHexString_InvalidInput_ReturnsFalse()
        {
            var ok = ColorExtension.TryFromHexString("notacolor", out var c);
            Assert.IsFalse(ok);
            Assert.AreEqual(Color.Empty, c);
        }

        [TestMethod]
        public void TryFromHexString_GarbageInput_ReturnsFalse()
        {
            var ok = ColorExtension.TryFromHexString("$$$$", out var c);
            Assert.IsFalse(ok);
        }

        [TestMethod]
        public void RoundTrip_ToHexAndBack_SameColor()
        {
            var original = Color.FromArgb(100, 150, 200);
            var hex = original.ToHexString();
            var restored = ColorExtension.FromHexString(hex);
            Assert.AreEqual(original.R, restored.R);
            Assert.AreEqual(original.G, restored.G);
            Assert.AreEqual(original.B, restored.B);
        }
    }
}
