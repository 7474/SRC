using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCRender.Compat;

namespace SRCRenderTests
{
    [TestClass]
    public class SkiaFontsTests
    {
        [TestMethod]
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
        public void RegisterFile_RegistersEveryFaceOfCollection()
        {
            // 複数の書体を持つ .ttc（WSL の Windows の msgothic.ttc など）は、すべての書体を登録する
            var dir = "/usr/share/fonts";
            // 並びはファイルシステムで変わるため、名前の順にする
            var ttc = System.IO.Directory.Exists(dir)
                ? System.Linq.Enumerable.FirstOrDefault(System.Linq.Enumerable.OrderBy(System.IO.Directory.GetFiles(dir, "*.ttc", System.IO.SearchOption.AllDirectories), x => x, System.StringComparer.Ordinal))
                : null;
            if (ttc == null)
            {
                Assert.Inconclusive("フォントのコレクション（.ttc）が無い");
            }
            SkiaFonts.RegisterFile(ttc);
            using (var second = SKTypeface.FromFile(ttc, 1))
            {
                if (second == null)
                {
                    Assert.Inconclusive("書体が 1 つしかない .ttc");
                }
                Assert.AreEqual(second.FamilyName, SkiaFonts.ResolveTypeface(second.FamilyName, SKFontStyle.Normal).FamilyName);
            }
        }

        [TestMethod]
        public void RegisterFile_BoldOnlyCollection_DoesNotReplaceRegular()
        {
            // 太字だけの .ttc を登録しても、標準の太さは OS の標準の書体で描く（太字の書体で描かない）
            var bold = "/usr/share/fonts/opentype/noto/NotoSansCJK-Bold.ttc";
            if (!System.IO.File.Exists(bold))
            {
                Assert.Inconclusive("NotoSansCJK-Bold.ttc が無い");
            }
            SkiaFonts.RegisterFile(bold);
            var regular = SkiaFonts.ResolveTypeface(SkiaFonts.SansFamily, SKFontStyle.Normal);
            Assert.AreEqual(SkiaFonts.SansFamily, regular.FamilyName);
            Assert.AreEqual((int)SKFontStyleWeight.Normal, regular.FontWeight);
            Assert.AreEqual((int)SKFontStyleWeight.Bold, SkiaFonts.ResolveTypeface(SkiaFonts.SansFamily, SKFontStyle.Bold).FontWeight);
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
