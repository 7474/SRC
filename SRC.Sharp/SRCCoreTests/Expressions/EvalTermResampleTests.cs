using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using VT = SRCCore.Expressions.ValueType;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression.EvalTerm を元の SRC（Expression.bas の EvalTerm）に合わせたことのテスト（7474/SRC#1556 の読み直しで見つけた違い）。
    /// VB は「"…"」「`…`」「(…」の項を数値として求められたとき、GeneralLib の StrToDbl（IsNumeric で数値に読めなければ 0）で数値にする。
    /// 「NaN」は VB の IsNumeric では数値に読めないので 0 になる
    /// </summary>
    [TestClass]
    public class EvalTermResampleTests
    {
        private static SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        [TestMethod]
        [DataRow("\"NaN\"")]
        [DataRow("`NaN`")]
        public void QuotedNaN_AsNumber_IsZero(string term)
        {
            var src = CreateSrc();

            var t = src.Expression.EvalTerm(term, VT.NumericType, out var s, out var n);

            Assert.AreEqual(VT.StringType, t);
            Assert.AreEqual("NaN", s);
            Assert.AreEqual(0d, n);
        }

        [TestMethod]
        public void QuotedNaN_InArithmetic_IsZero()
        {
            // VB: 「"NaN" + 1」は 0 + 1 で 1
            var src = CreateSrc();

            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("\"NaN\" + 1"));
        }

        [TestMethod]
        public void QuotedNumber_AsNumber_IsConverted()
        {
            // 数値に読める文字列はそのまま数値になる（変えていないことの確認）
            var src = CreateSrc();

            src.Expression.EvalTerm("\"12.5\"", VT.NumericType, out _, out var n);

            Assert.AreEqual(12.5d, n);
        }
    }
}
