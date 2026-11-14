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

        /// <summary>
        /// 等幅の指定は、文字ごとに Shift_JIS のバイト数の升目（半角 = 全角の半分）に並べる。
        /// 代わりの書体（Noto Sans Mono CJK JP など）が半角でも全角でもない幅で描く文字（ギリシャ文字・°・引用符など）でも桁がずれない（7474/SRC#1650）
        /// </summary>
        [TestMethod]
        [DataRow("α")]
        [DataRow("Ω")]
        [DataRow("Ж")]
        [DataRow("°")]
        [DataRow("“")]
        [DataRow("’")]
        [DataRow("あ")]
        [DataRow("×")]
        public void FixedPitchFont_DoubleByteCharactersTakeTwoCells(string ch)
        {
            using (var font = SkiaFonts.Create("ＭＳ 明朝", 16, false, false))
            {
                Assert.IsTrue(SkiaFonts.IsFixedPitch(font));
                Assert.AreEqual(16f, SkiaFonts.MeasureText(font, ch), 0.01f);
                Assert.AreEqual(SkiaFonts.MeasureText(font, ch + "1000"), SkiaFonts.MeasureText(font, "      "), 0.01f);
            }
        }

        [TestMethod]
        public void ProportionalFont_UsesTypefaceWidths()
        {
            using (var font = SkiaFonts.Create("ＭＳ Ｐ明朝", 16, false, false))
            {
                Assert.IsFalse(SkiaFonts.IsFixedPitch(font));
                Assert.AreEqual(font.MeasureText("αi"), SkiaFonts.MeasureText(font, "αi"), 0.01f);
            }
        }

        /// <summary>升目に置いて描くと、次の文字は升目の境から始まる（幅の違う文字の後でも位置がずれない）</summary>
        [TestMethod]
        public void FixedPitchFont_DrawsEachCharacterInItsCell()
        {
            using (var font = SkiaFonts.Create("ＭＳ 明朝", 16, false, false))
            using (var bitmap = new SKBitmap(64, 24))
            using (var canvas = new SKCanvas(bitmap))
            using (var paint = new SKPaint { Color = SKColors.Black, IsAntialias = false })
            {
                if (!SkiaFonts.HasJapaneseFont())
                {
                    Assert.Inconclusive("日本語の書体が無い環境");
                }
                canvas.Clear(SKColors.White);
                // 「°」（2 升）の後の「W」（1 升）は、x = 16〜24 の升目に描かれる
                SkiaFonts.DrawText(canvas, "°W", 0, 18, SKTextAlign.Left, font, paint);
                var inked = new System.Collections.Generic.List<int>();
                for (var x = 16; x < 64; x++)
                {
                    for (var y = 0; y < 24; y++)
                    {
                        if (bitmap.GetPixel(x, y).Red < 200)
                        {
                            inked.Add(x);
                            break;
                        }
                    }
                }
                Assert.IsTrue(inked.Count > 0, "「W」が描かれていない");
                Assert.IsTrue(inked.TrueForAll(x => x >= 16 && x < 24), string.Join(",", inked));
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
