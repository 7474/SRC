using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCRender.Compat;

namespace SRCRenderTests
{
    [TestClass]
    public class SkiaFontsTests
    {
        [DataTestMethod]
        [DataRow("ＭＳ 明朝", true)]
        [DataRow("ＭＳ ゴシック", true)]
        [DataRow("MS Gothic", true)]
        [DataRow("ＭＳ Ｐ明朝", false)]
        [DataRow("ＭＳ Ｐゴシック", false)]
        [DataRow("MS UI Gothic", false)]
        [DataRow("Meiryo UI", false)]
        [DataRow("", false)]
        public void IsFixedPitchName(string name, bool expected)
        {
            Assert.AreEqual(expected, SkiaFonts.IsFixedPitchName(name));
        }

        [TestMethod]
        public void FixedPitchFont_AlignsSpacesWithFullWidthCharacters()
        {
            using (var font = SkiaFonts.Create("ＭＳ 明朝", 16, false, false))
            {
                if (font.Typeface.FamilyName != SkiaFonts.FixedPitchFamily)
                {
                    Assert.Inconclusive($"{SkiaFonts.FixedPitchFamily} が無い環境");
                }
                // 半角 2 文字で全角 1 文字の幅になること（空白で桁を揃える表示がずれない）
                Assert.AreEqual(font.MeasureText("あ"), font.MeasureText("  "), 0.01f);
                Assert.AreEqual(font.MeasureText("ＨＰ"), font.MeasureText("HP  "), 0.01f);
                Assert.AreEqual(font.MeasureText("1000"), font.MeasureText("    "), 0.01f);
            }
        }

        [TestMethod]
        public void LineHeight_IsOneEmForNotoCjk()
        {
            using (var font = SkiaFonts.Create("ＭＳ 明朝", 16, false, false))
            {
                if (!font.Typeface.FamilyName.StartsWith("Noto "))
                {
                    Assert.Inconclusive("Noto CJK が無い環境");
                }
                // ＭＳ 明朝と同じく 1em（16px）を行の高さとする
                Assert.AreEqual(16f, new Font("ＭＳ 明朝", 12f).GetHeight(), 0.01f);
                Assert.AreEqual(16f, SkiaFonts.LineHeight(font), 0.01f);
            }
        }
    }
}
