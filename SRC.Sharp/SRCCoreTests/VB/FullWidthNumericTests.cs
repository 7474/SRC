using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// 数値の文字列の全角の扱い（7474/SRC#1323）。
    /// 日本語の Windows の VB（OLE Automation の VarParseNumFromStr）は、全角の文字を半角にした写しを読み、前後の空白を無視する。
    /// IsNumeric と変換（CLng・CDbl）が同じ規則で読むこと（判定だけ通り、変換で 0 になることがないこと）。
    /// </summary>
    [TestClass]
    public class FullWidthNumericTests
    {
        [TestMethod]
        [DataRow("　1400", 1400)]
        [DataRow("1400　", 1400)]
        [DataRow("　 1400 　", 1400)]
        [DataRow("１４００", 1400)]
        [DataRow("＋１０", 10)]
        [DataRow("－５", -5)]
        public void FullWidth_IsNumericAndConvertsLikeVb(string text, int expected)
        {
            Assert.IsTrue(Information.IsNumeric(text), text);
            Assert.AreEqual(expected, Conversions.ToInteger(text), text);
            Assert.AreEqual(expected, Conversions.ToDouble(text), text);
            Assert.AreEqual(expected, GeneralLib.StrToLng(text), text);
        }

        [TestMethod]
        [DataRow("- 10", -10)]
        [DataRow("+ 20", 20)]
        [DataRow("-  5", -5)]
        [DataRow("－　５", -5)]
        [DataRow(" + 0", 0)]
        public void SignFollowedBySpace_IsNumeric(string text, int expected)
        {
            // GSC データパックの武器データにある「- 10」「+ 20」。データの実態として数値として扱う（7474/SRC#1413）
            Assert.IsTrue(Information.IsNumeric(text), text);
            Assert.AreEqual(expected, Conversions.ToInteger(text), text);
            Assert.AreEqual(expected, Conversions.ToDouble(text), text);
            Assert.AreEqual(expected, GeneralLib.StrToLng(text), text);
        }

        [TestMethod]
        [DataRow("- ")]
        [DataRow("-")]
        [DataRow("- a")]
        [DataRow("- 1 0")]
        [DataRow("+ - 1")]
        public void SignFollowedBySpace_WithoutNumber_IsNotNumeric(string text)
        {
            Assert.IsFalse(Information.IsNumeric(text), text);
        }

        [TestMethod]
        [DataRow("1400　1")]
        [DataRow("1400 1")]
        [DataRow("　")]
        [DataRow("１４００ａ")]
        public void FullWidth_SpaceInsideOrNoDigits_IsNotNumeric(string text)
        {
            Assert.IsFalse(Information.IsNumeric(text), text);
        }

        [TestMethod]
        public void ListSplit_DoesNotSplitOnFullWidthSpace()
        {
            // VB の ListSplit・LLength・LIndex は全角の空白で分けない（Asc が 9・32 だけ。全角の空白は &H8140）
            Assert.AreEqual(1, GeneralLib.LLength("a　b"));
            Assert.AreEqual(1, GeneralLib.ListSplit("a　b", out var arr));
            Assert.AreEqual("a　b", arr[0]);
        }
    }
}
