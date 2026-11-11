using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.TestLib;
using System;
using System.Drawing;
using System.Globalization;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// 実行する環境の言語の設定に左右されないことを確かめる（7474/SRC#1554・#1545）。
    /// 元の SRC は日本語版の Windows で動く前提で、VB5 の CStr・CDbl・Format・LCase などは日本語の地域の設定で動く。
    /// 期待値は日本語版の Windows の VB5 の結果（推定。小数点は「.」、桁区切りは「,」、LCase("I") は "i"）。
    /// ドイツ語（小数点が「,」）・トルコ語（I の小文字が ı）の設定で、日本語の設定と同じ結果になることを確かめる。
    /// </summary>
    [TestClass]
    public class CultureIndependenceTests
    {
        private sealed class CultureScope : IDisposable
        {
            private readonly CultureInfo culture;
            private readonly CultureInfo uiCulture;

            public CultureScope(string name)
            {
                culture = CultureInfo.CurrentCulture;
                uiCulture = CultureInfo.CurrentUICulture;
                CultureInfo specified;
                try
                {
                    specified = CultureInfo.GetCultureInfo(name);
                }
                catch (CultureNotFoundException)
                {
                    Assert.Inconclusive($"カルチャ {name} のデータが無い環境です");
                    throw;
                }
                CultureInfo.CurrentCulture = specified;
                CultureInfo.CurrentUICulture = specified;
            }

            public void Dispose()
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = uiCulture;
            }
        }

        private static Expression CreateExpression()
        {
            var src = new SRC
            {
                GUI = new MockGUI(),
            };
            return new Expression(src);
        }

        // VB: CDbl("1.5") = 1.5、CDbl("1,000.5") = 1000.5（日本語の地域の設定）
        [TestMethod]
        [DataRow("ja-JP")]
        [DataRow("de-DE")]
        [DataRow("tr-TR")]
        [DataRow("fr-FR")]
        public void ToDouble_ReadsJapaneseNumberFormat(string culture)
        {
            using (new CultureScope(culture))
            {
                Assert.AreEqual(1.5d, Conversions.ToDouble("1.5"));
                Assert.AreEqual(-0.25d, Conversions.ToDouble("-0.25"));
                Assert.AreEqual(1000.5d, Conversions.ToDouble("1,000.5"));
                Assert.AreEqual(2, Conversions.ToInteger("1.5"));
                Assert.AreEqual(1.5d, GeneralLib.StrToDbl("1.5"));
                Assert.AreEqual(2, GeneralLib.StrToLng("1.5"));
            }
        }

        // VB: IsNumeric("1.5") = True。ドイツ語の書式で読むと「1.5」は 15 になり、IsNumeric の判定と変換の値がずれる
        [TestMethod]
        [DataRow("de-DE")]
        [DataRow("tr-TR")]
        public void IsNumeric_ReadsJapaneseNumberFormat(string culture)
        {
            using (new CultureScope(culture))
            {
                Assert.IsTrue(Information.IsNumeric("1.5"));
                Assert.IsTrue(Information.IsNumeric("-1.5"));
                Assert.IsTrue(Information.IsNumeric("1,000.5"));
                Assert.IsFalse(Information.IsNumeric("1.000,5"));
            }
        }

        // VB: CStr(1.5) = "1.5"
        [TestMethod]
        [DataRow("ja-JP")]
        [DataRow("de-DE")]
        [DataRow("tr-TR")]
        [DataRow("fr-FR")]
        public void NumberToString_WritesJapaneseNumberFormat(string culture)
        {
            using (new CultureScope(culture))
            {
                Assert.AreEqual("1.5", Conversions.ToString(1.5d));
                Assert.AreEqual("-3.14", Conversions.ToString(-3.14d));
                Assert.AreEqual("1.5", SrcFormatter.Format((object)1.5d));
                Assert.AreEqual("1.50", SrcFormatter.Format(1.5d, "0.00"));
                Assert.AreEqual("1,234.5", SrcFormatter.Format(1234.5d, "#,##0.0"));
                Assert.AreEqual("1,234", SrcFormatter.Format(1234, "#,##0"));
                Assert.AreEqual("1.5", GeneralLib.FormatNum(1.5d));
                Assert.AreEqual("-0.125", GeneralLib.FormatNum(-0.125d));
            }
        }

        // 式の計算結果を文字列にするときも日本語の書式（小数点「.」）にする
        [TestMethod]
        [DataRow("ja-JP")]
        [DataRow("de-DE")]
        [DataRow("tr-TR")]
        public void Expression_UsesJapaneseNumberFormat(string culture)
        {
            using (new CultureScope(culture))
            {
                var exp = CreateExpression();
                Assert.AreEqual("2.5", exp.GetValueAsString("1.5 + 1"));
                Assert.AreEqual(2.5d, exp.GetValueAsDouble("1.5 + 1"));
                Assert.AreEqual("0.5", exp.GetValueAsString("1 / 2"));
                Assert.AreEqual("1.5", exp.GetValueAsString("Format(1.5, \"0.0\")"));
                exp.SetVariableAsDouble("x", 0.25d);
                Assert.AreEqual("0.25", exp.GetValueAsString("x"));
                Assert.AreEqual("1.25", exp.GetValueAsString("x + 1"));
            }
        }

        // VB: LCase("I") = "i"、Format("i", ">") = "I"（日本語の地域の設定。トルコ語の ı・İ にしない）
        [TestMethod]
        [DataRow("ja-JP")]
        [DataRow("tr-TR")]
        public void CaseConversion_DoesNotUseTurkishRules(string culture)
        {
            using (new CultureScope(culture))
            {
                Assert.AreEqual("title", Strings.LCase("TITLE"));
                Assert.AreEqual('i', Strings.LCase('I'));
                var exp = CreateExpression();
                Assert.AreEqual("insert", exp.GetValueAsString("LCase(\"INSERT\")"));
                Assert.AreEqual("insert", exp.GetValueAsString("Format(\"INSERT\", \"<\")"));
                Assert.AreEqual("INSERT", exp.GetValueAsString("Format(\"insert\", \">\")"));
                // 関数の名前は大文字・小文字を区別しない（「INSTR」も InStr）
                Assert.AreEqual(2d, exp.GetValueAsDouble("INSTR(\"abc\", \"b\")"));
            }
        }

        // 色の指定（「255,0,0」）の区切りは、実行する環境のリストの区切り（ドイツ語では「;」）によらず「,」
        [TestMethod]
        [DataRow("ja-JP")]
        [DataRow("de-DE")]
        public void ColorFromString_UsesCommaSeparator(string culture)
        {
            using (new CultureScope(culture))
            {
                var color = ColorExtension.FromHexString("255,0,0");
                Assert.AreEqual(Color.FromArgb(255, 0, 0).ToArgb(), color.ToArgb());
            }
        }
    }
}
