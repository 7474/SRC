using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// Information クラスの追加テスト（エッジケース）
    /// </summary>
    [TestClass]
    public class InformationAdditionalTests
    {
        // ──────────────────────────────────────────────
        // IsNumeric (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsNumeric_LargeInteger_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("1000000"));
        }

        [TestMethod]
        public void IsNumeric_ScientificNotation_ReturnsFalse()
        {
            // decimal.TryParse は科学的表記法をサポートしていないため False
            Assert.IsFalse(Information.IsNumeric("1e5"));
        }

        [TestMethod]
        public void IsNumeric_NegativeFloat_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("-3.14"));
        }

        [TestMethod]
        public void IsNumeric_PositiveZero_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("0.0"));
        }

        [TestMethod]
        public void IsNumeric_Integer_Object_ReturnsTrue()
        {
            // オブジェクト型の整数
            Assert.IsTrue(Information.IsNumeric(42));
        }

        [TestMethod]
        public void IsNumeric_Double_Object_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric(3.14));
        }

        [TestMethod]
        public void IsNumeric_TabSeparated_ReturnsTrue()
        {
            // タブは空白扱いで無視される
            Assert.IsTrue(Information.IsNumeric("\t42\t"));
        }

        [TestMethod]
        public void IsNumeric_NewlineSeparated_ReturnsTrue()
        {
            // 改行は空白扱いで無視される
            Assert.IsTrue(Information.IsNumeric("\n42\n"));
        }

        [TestMethod]
        public void IsNumeric_OnlyWhitespace_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric("   "));
        }

        [TestMethod]
        public void IsNumeric_AlphanumericMixed_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric("1a2b"));
        }

        [TestMethod]
        public void IsNumeric_JapaneseChars_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric("百"));
        }

        [TestMethod]
        public void IsNumeric_PlusSign_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("+5"));
        }
    }
}
