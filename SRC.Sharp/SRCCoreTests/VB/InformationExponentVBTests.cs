using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using SRCCore.TestLib;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// 指数表記の数値（7474/SRC#1534）。
    /// 元の SRC（VB6）の IsNumeric・CDbl などは OLE Automation（VarParseNumFromStr）で読み、
    /// 「1e5」のような指数表記を数値として扱う。指数の記号には E のほか D も使える（「1d5」も 100000）。
    /// 指数の記号の後ろに数字が無いもの（「1e」）や、仮数部の無いもの（「e5」）は数値ではない。
    /// </summary>
    [TestClass]
    public class InformationExponentVBTests
    {
        [TestMethod]
        [DataRow("1e5", 100000d)]
        [DataRow("1E5", 100000d)]
        [DataRow("1e+5", 100000d)]
        [DataRow("1e-2", 0.01d)]
        [DataRow("-1.5e3", -1500d)]
        [DataRow(".5e1", 5d)]
        [DataRow("5.e1", 50d)]
        [DataRow(" 1e5 ", 100000d)]
        [DataRow("１ｅ５", 100000d)]
        [DataRow("1d5", 100000d)]
        [DataRow("1D-2", 0.01d)]
        [DataRow("1,000e1", 10000d)]
        public void ExponentNotation_IsNumericAndConverts(string text, double expected)
        {
            Assert.IsTrue(Information.IsNumeric(text), text);
            Assert.AreEqual(expected, Conversions.ToDouble(text), 1e-9, text);
        }

        [TestMethod]
        [DataRow("1e")]
        [DataRow("1e+")]
        [DataRow("e5")]
        [DataRow("1e5.5")]
        [DataRow("1e 5")]
        [DataRow("1ee5")]
        [DataRow("1e5e5")]
        [DataRow("1x5")]
        public void MalformedExponent_IsNotNumeric(string text)
        {
            Assert.IsFalse(Information.IsNumeric(text), text);
            Assert.AreEqual(0d, Conversions.ToDouble(text), text);
        }

        [TestMethod]
        public void ExponentNotation_ConvertsToInteger()
        {
            // VB: CLng("1e3") = 1000
            Assert.AreEqual(1000, Conversions.ToInteger("1e3"));
            Assert.AreEqual(1000, GeneralLib.StrToLng("1e3"));
            Assert.AreEqual(100000d, GeneralLib.StrToDbl("1d5"));
        }

        [TestMethod]
        public void ThousandsSeparator_IsNumeric()
        {
            // VB: IsNumeric("1,000") は True、CDbl("1,000") は 1000
            Assert.IsTrue(Information.IsNumeric("1,000"));
            Assert.AreEqual(1000d, Conversions.ToDouble("1,000"));
        }

        [TestMethod]
        public void Expression_ExponentNotation_IsNumber()
        {
            // VB: 式の中の「1e3」も数値として計算する
            var src = new SRC { GUI = new MockGUI() };
            Assert.AreEqual(1001d, src.Expression.GetValueAsDouble("1e3 + 1"));
        }
    }
}
