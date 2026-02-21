using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// SrcFormatter クラスの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class SrcFormatterMoreTests
    {
        // ──────────────────────────────────────────────
        // Format(object)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_NullObject_ReturnsEmpty()
        {
            Assert.AreEqual("", SrcFormatter.Format(null));
        }

        [TestMethod]
        public void Format_IntObject_ReturnsString()
        {
            Assert.AreEqual("0", SrcFormatter.Format((object)0));
        }

        [TestMethod]
        public void Format_NegativeInt_ReturnsString()
        {
            Assert.AreEqual("-5", SrcFormatter.Format((object)(-5)));
        }

        [TestMethod]
        public void Format_DoubleObject_ReturnsString()
        {
            // 小数部0の場合の文字列化
            Assert.AreEqual("1", SrcFormatter.Format((object)1.0));
        }

        [TestMethod]
        public void Format_BoolTrue_ReturnsString()
        {
            Assert.AreEqual("True", SrcFormatter.Format((object)true));
        }

        [TestMethod]
        public void Format_BoolFalse_ReturnsString()
        {
            Assert.AreEqual("False", SrcFormatter.Format((object)false));
        }

        // ──────────────────────────────────────────────
        // Format(int, string)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_Int_ZeroPad2_Returns01()
        {
            Assert.AreEqual("01", SrcFormatter.Format(1, "00"));
        }

        [TestMethod]
        public void Format_Int_ZeroPad3_Returns007()
        {
            Assert.AreEqual("007", SrcFormatter.Format(7, "000"));
        }

        [TestMethod]
        public void Format_Int_ZeroPad3_Returns100()
        {
            Assert.AreEqual("100", SrcFormatter.Format(100, "000"));
        }

        [TestMethod]
        public void Format_Int_NoFormat_ReturnsNumber()
        {
            Assert.AreEqual("42", SrcFormatter.Format(42, ""));
        }

        [TestMethod]
        public void Format_Int_NegativeWithFormat()
        {
            Assert.AreEqual("-5", SrcFormatter.Format(-5, "0"));
        }

        // ──────────────────────────────────────────────
        // Format(double, string)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_Double_TwoDecimals()
        {
            Assert.AreEqual("3.14", SrcFormatter.Format(3.14159, "0.00"));
        }

        [TestMethod]
        public void Format_Double_NoDecimals()
        {
            Assert.AreEqual("3", SrcFormatter.Format(3.0, "0"));
        }

        [TestMethod]
        public void Format_Double_OptionalDecimals()
        {
            Assert.AreEqual("3.14", SrcFormatter.Format(3.14, "0.##"));
            Assert.AreEqual("3", SrcFormatter.Format(3.0, "0.##"));
        }

        [TestMethod]
        public void Format_Double_ZeroValue()
        {
            Assert.AreEqual("0.00", SrcFormatter.Format(0.0, "0.00"));
        }

        [TestMethod]
        public void Format_Double_NegativeValue()
        {
            Assert.AreEqual("-1.50", SrcFormatter.Format(-1.5, "0.00"));
        }

        [TestMethod]
        public void Format_Double_InvalidFormat_FallsBack()
        {
            // 無効なフォーマットはフォールバックして数値を文字列化する
            var result = SrcFormatter.Format(3.14, "\x00invalid");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Format_Int_InvalidFormat_FallsBack()
        {
            // 無効なフォーマットはフォールバックして数値を文字列化する
            var result = SrcFormatter.Format(42, "\x00invalid");
            Assert.IsNotNull(result);
        }
    }
}
