using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using System.Drawing;

namespace SRCCore.Tests
{
    /// <summary>
    /// DrawFontOption クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class DrawFontOptionTests
    {
        // ──────────────────────────────────────────────
        // プロパティ設定・読み取り
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FontFamily_CanBeSetAndRead()
        {
            var option = new DrawFontOption { FontFamily = "メイリオ" };
            Assert.AreEqual("メイリオ", option.FontFamily);
        }

        [TestMethod]
        public void Bold_CanBeSetAndRead()
        {
            var option = new DrawFontOption { Bold = true };
            Assert.IsTrue(option.Bold);
        }

        [TestMethod]
        public void Italic_CanBeSetAndRead()
        {
            var option = new DrawFontOption { Italic = true };
            Assert.IsTrue(option.Italic);
        }

        [TestMethod]
        public void Size_CanBeSetAndRead()
        {
            var option = new DrawFontOption { Size = 12.5f };
            Assert.AreEqual(12.5f, option.Size);
        }

        [TestMethod]
        public void Color_CanBeSetAndRead()
        {
            var option = new DrawFontOption { Color = Color.Red };
            Assert.AreEqual(Color.Red, option.Color);
        }

        [TestMethod]
        public void DefaultValues_AreExpected()
        {
            var option = new DrawFontOption();
            Assert.IsNull(option.FontFamily);
            Assert.IsFalse(option.Bold);
            Assert.IsFalse(option.Italic);
            Assert.AreEqual(0f, option.Size);
            Assert.AreEqual(Color.Empty, option.Color);
        }

        [TestMethod]
        public void AllProperties_CanBeSetTogether()
        {
            var option = new DrawFontOption
            {
                FontFamily = "Arial",
                Bold = true,
                Italic = false,
                Size = 16.0f,
                Color = Color.Blue
            };
            Assert.AreEqual("Arial", option.FontFamily);
            Assert.IsTrue(option.Bold);
            Assert.IsFalse(option.Italic);
            Assert.AreEqual(16.0f, option.Size);
            Assert.AreEqual(Color.Blue, option.Color);
        }
    }
}
