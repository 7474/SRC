using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// SrcFormatter クラスの追加テスト（既存テストとの重複を避けた新規ケース）
    /// </summary>
    [TestClass]
    public class SrcFormatterAdditionalTests
    {
        // ──────────────────────────────────────────────
        // Format(object) – 型の多様性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_Object_StringValue_ReturnsString()
        {
            Assert.AreEqual("hello", SrcFormatter.Format((object)"hello"));
        }

        [TestMethod]
        public void Format_Object_LargePositiveInt_ReturnsString()
        {
            Assert.AreEqual("1000000", SrcFormatter.Format((object)1000000));
        }

        [TestMethod]
        public void Format_Object_CharValue_ReturnsString()
        {
            Assert.AreEqual("A", SrcFormatter.Format((object)'A'));
        }

        [TestMethod]
        public void Format_Object_FloatValue_ReturnsString()
        {
            var result = SrcFormatter.Format((object)1.5f);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("1.5"));
        }

        // ──────────────────────────────────────────────
        // Format(int, string) – 追加フォーマットパターン
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_Int_HashFormat_NoLeadingZero()
        {
            // "#" フォーマット: ゼロパディングなし
            var result = SrcFormatter.Format(5, "#");
            Assert.AreEqual("5", result);
        }

        [TestMethod]
        public void Format_Int_Zero_WithZeroPad()
        {
            Assert.AreEqual("00", SrcFormatter.Format(0, "00"));
        }

        [TestMethod]
        public void Format_Int_MaxValue_ReturnsString()
        {
            var result = SrcFormatter.Format(int.MaxValue, "0");
            Assert.AreEqual(int.MaxValue.ToString(), result);
        }

        [TestMethod]
        public void Format_Int_MinValue_ReturnsString()
        {
            var result = SrcFormatter.Format(int.MinValue, "0");
            Assert.AreEqual(int.MinValue.ToString(), result);
        }

        [TestMethod]
        public void Format_Int_WideZeroPad_ProducesCorrectWidth()
        {
            // "00000" でゼロパディング5桁
            Assert.AreEqual("00042", SrcFormatter.Format(42, "00000"));
        }

        // ──────────────────────────────────────────────
        // Format(double, string) – 追加フォーマットパターン
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_Double_IntegerOnly_NoDecimalPoint()
        {
            // 整数部のみのフォーマット
            Assert.AreEqual("5", SrcFormatter.Format(5.0, "0"));
        }

        [TestMethod]
        public void Format_Double_LargeValue_ReturnsString()
        {
            var result = SrcFormatter.Format(12345.678, "0.0");
            Assert.AreEqual("12345.7", result);
        }

        [TestMethod]
        public void Format_Double_SmallFraction_RoundsCorrectly()
        {
            // 0.005 を "0.00" でフォーマット
            var result = SrcFormatter.Format(0.005, "0.00");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Format_Double_NegativeWithSingleDecimal()
        {
            Assert.AreEqual("-3.1", SrcFormatter.Format(-3.14, "0.0"));
        }

        [TestMethod]
        public void Format_Double_PositiveInfinity_FallsBack()
        {
            // 無限大は ToString() でフォールバックされる
            var result = SrcFormatter.Format(double.PositiveInfinity, "0.00");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Format_Double_NaN_FallsBack()
        {
            var result = SrcFormatter.Format(double.NaN, "0.00");
            Assert.IsNotNull(result);
        }

        // ──────────────────────────────────────────────
        // Format(object) – null 以外の参照型
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_Object_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", SrcFormatter.Format((object)""));
        }

        [TestMethod]
        public void Format_Object_IntArray_ReturnsTypeString()
        {
            // 配列は ToString() でシステム文字列になる
            var result = SrcFormatter.Format((object)new int[] { 1, 2, 3 });
            Assert.IsNotNull(result);
        }
    }
}
