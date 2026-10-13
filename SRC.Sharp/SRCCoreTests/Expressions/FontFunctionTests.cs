using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Font 関数のユニットテスト
    /// </summary>
    [TestClass]
    public class FontFunctionTests
    {
        private Expression Create(PaintFont font)
        {
            var src = new SRC { GUI = new MockGUI { CurrentPaintFont = font } };
            return new Expression(src);
        }

        [TestMethod]
        public void Font_Name_ReturnsCurrentPaintFontName()
        {
            var exp = Create(new PaintFont { Name = "ＭＳ ゴシック", Size = 12 });
            Assert.AreEqual("ＭＳ ゴシック", exp.GetValueAsString("Font(フォント名)"));
        }

        [TestMethod]
        public void Font_Size_ReturnsCurrentPaintFontSize()
        {
            var exp = Create(new PaintFont { Name = "ＭＳ ゴシック", Size = 12 });
            Assert.AreEqual(12d, exp.GetValueAsDouble("Font(サイズ)"));
        }

        [TestMethod]
        public void Font_BoldAndItalic_ReturnFlags()
        {
            var exp = Create(new PaintFont { Name = "ＭＳ Ｐ明朝", Size = 16, Bold = true, Italic = false });
            Assert.AreEqual(1d, exp.GetValueAsDouble("Font(太字)"));
            Assert.AreEqual(0d, exp.GetValueAsDouble("Font(斜体)"));
        }
    }
}
