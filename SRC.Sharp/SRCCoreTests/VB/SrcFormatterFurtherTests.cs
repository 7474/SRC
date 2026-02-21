using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// SrcFormatter クラスのさらなる追加ユニットテスト
    /// </summary>
    [TestClass]
    public class SrcFormatterFurtherTests
    {
        // ──────────────────────────────────────────────
        // Format(object)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_Object_Int_ReturnsIntString()
        {
            Assert.AreEqual("100", SrcFormatter.Format((object)100));
        }

        [TestMethod]
        public void Format_Object_NegativeInt_ReturnsString()
        {
            Assert.AreEqual("-5", SrcFormatter.Format((object)(-5)));
        }

        [TestMethod]
        public void Format_Object_Null_ReturnsEmpty()
        {
            Assert.AreEqual("", SrcFormatter.Format(null));
        }

        [TestMethod]
        public void Format_Object_Bool_ReturnsString()
        {
            Assert.AreEqual("True", SrcFormatter.Format((object)true));
            Assert.AreEqual("False", SrcFormatter.Format((object)false));
        }

        // ──────────────────────────────────────────────
        // Format(int, string)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_Int_HexFormat_ReturnsHex()
        {
            Assert.AreEqual("ff", SrcFormatter.Format(255, "x2"));
        }

        [TestMethod]
        public void Format_Int_LeadingZeros_Returns3Digits()
        {
            Assert.AreEqual("042", SrcFormatter.Format(42, "000"));
        }

        [TestMethod]
        public void Format_Int_NoPadding_ReturnsNormal()
        {
            Assert.AreEqual("1234", SrcFormatter.Format(1234, "0"));
        }

        [TestMethod]
        public void Format_Int_MaxValue_ReturnsString()
        {
            var result = SrcFormatter.Format(int.MaxValue, "0");
            Assert.AreEqual("2147483647", result);
        }

        [TestMethod]
        public void Format_Int_NegativeValue_ReturnsString()
        {
            Assert.AreEqual("-042", SrcFormatter.Format(-42, "000"));
        }

        // ──────────────────────────────────────────────
        // Format(double, string)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_Double_OneDecimalPlace_ReturnsFormatted()
        {
            Assert.AreEqual("3.1", SrcFormatter.Format(3.14, "0.0"));
        }

        [TestMethod]
        public void Format_Double_IntegerOnly_RoundsValue()
        {
            // 3.9.ToString("0") = "4" (四捨五入)
            Assert.AreEqual("4", SrcFormatter.Format(3.9, "0"));
        }

        [TestMethod]
        public void Format_Double_Zero_ReturnsZeroString()
        {
            Assert.AreEqual("0", SrcFormatter.Format(0.0, "0"));
        }

        [TestMethod]
        public void Format_Double_Negative_ReturnsNegative()
        {
            Assert.AreEqual("-3.14", SrcFormatter.Format(-3.14, "0.##"));
        }

        [TestMethod]
        public void Format_Double_LargeNumber_ReturnsString()
        {
            var result = SrcFormatter.Format(1000000.0, "0");
            Assert.AreEqual("1000000", result);
        }

        [TestMethod]
        public void Format_Double_InvalidFormat_FallsBack()
        {
            var result = SrcFormatter.Format(42.0, "invalid\x01format");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Format_Double_ScientificNotation_CanBeFormatted()
        {
            var result = SrcFormatter.Format(1.5e10, "0.##E+0");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }
    }
}
