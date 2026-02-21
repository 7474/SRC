using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// SrcFormatter クラスの追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class SrcFormatterEdgeCaseTests
    {
        // ──────────────────────────────────────────────
        // Format(object) の各種型テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_Bool_True_ReturnsTrueString()
        {
            var result = SrcFormatter.Format((object)true);
            Assert.AreEqual("True", result);
        }

        [TestMethod]
        public void Format_Bool_False_ReturnsFalseString()
        {
            var result = SrcFormatter.Format((object)false);
            Assert.AreEqual("False", result);
        }

        [TestMethod]
        public void Format_Long_ReturnsLongString()
        {
            var result = SrcFormatter.Format((object)1234567890L);
            Assert.AreEqual("1234567890", result);
        }

        [TestMethod]
        public void Format_Float_ReturnsFloatString()
        {
            var result = SrcFormatter.Format((object)3.14f);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.StartsWith("3.14"), $"Expected to start with '3.14' but got '{result}'");
        }

        [TestMethod]
        public void Format_Char_ReturnsCharString()
        {
            var result = SrcFormatter.Format((object)'A');
            Assert.AreEqual("A", result);
        }

        [TestMethod]
        public void Format_JapaneseString_ReturnsJapaneseString()
        {
            var result = SrcFormatter.Format((object)"日本語テスト");
            Assert.AreEqual("日本語テスト", result);
        }

        // ──────────────────────────────────────────────
        // Format(int, string) のパターンテスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_Int_StandardNumericFormat_ReturnsFormattedString()
        {
            // "D5" - 5桁の10進数
            var result = SrcFormatter.Format(42, "D5");
            Assert.AreEqual("00042", result);
        }

        [TestMethod]
        public void Format_Int_HexFormat_ReturnsHexString()
        {
            var result = SrcFormatter.Format(255, "X");
            Assert.AreEqual("FF", result);
        }

        [TestMethod]
        public void Format_Int_NegativeWithLeadingZeros_ReturnsFormattedString()
        {
            // 負数をゼロパディング
            var result = SrcFormatter.Format(-5, "000");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Format_Int_Zero_WithPaddingFormat()
        {
            var result = SrcFormatter.Format(0, "000");
            Assert.AreEqual("000", result);
        }

        [TestMethod]
        public void Format_Int_Large_WithPaddingFormat()
        {
            var result = SrcFormatter.Format(12345, "00000");
            Assert.AreEqual("12345", result);
        }

        // ──────────────────────────────────────────────
        // Format(double, string) のパターンテスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_Double_DecimalFormat_ReturnsFormattedString()
        {
            var result = SrcFormatter.Format(3.14159, "F2");
            Assert.AreEqual("3.14", result);
        }

        [TestMethod]
        public void Format_Double_ZeroFormat_ReturnsFormattedString()
        {
            var result = SrcFormatter.Format(1.5, "0");
            Assert.AreEqual("2", result);  // 丸め
        }

        [TestMethod]
        public void Format_Double_PercentFormat()
        {
            var result = SrcFormatter.Format(0.5, "P0");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("50"), $"Expected '50' in '{result}'");
        }

        [TestMethod]
        public void Format_Double_Zero_ReturnsZeroString()
        {
            var result = SrcFormatter.Format(0.0, "0.00");
            Assert.AreEqual("0.00", result);
        }

        [TestMethod]
        public void Format_Double_Negative_ReturnsNegativeString()
        {
            var result = SrcFormatter.Format(-3.14, "0.00");
            Assert.AreEqual("-3.14", result);
        }

        // ──────────────────────────────────────────────
        // Format 無効フォーマット - フォールバックテスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_Int_EmptyFormat_FallsBack()
        {
            var result = SrcFormatter.Format(42, "");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Format_Double_EmptyFormat_FallsBack()
        {
            var result = SrcFormatter.Format(3.14, "");
            Assert.IsNotNull(result);
        }
    }
}
