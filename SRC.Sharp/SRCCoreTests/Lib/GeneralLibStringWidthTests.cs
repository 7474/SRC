using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// GeneralLib の StrWidth, LNormalize, LSplit などの追加テスト
    /// </summary>
    [TestClass]
    public class GeneralLibStringWidthTests
    {
        // ──────────────────────────────────────────────
        // StrWidth - 文字列の表示幅テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrWidth_AllAscii_ReturnsSameAsLength()
        {
            Assert.AreEqual(5, GeneralLib.StrWidth("hello"));
        }

        [TestMethod]
        public void StrWidth_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.StrWidth(""));
        }

        [TestMethod]
        public void StrWidth_NullString_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.StrWidth(null));
        }

        [TestMethod]
        public void StrWidth_AllJapanese_ReturnsDoubleLength()
        {
            // 全角文字は幅2
            Assert.AreEqual(6, GeneralLib.StrWidth("日本語"));
        }

        [TestMethod]
        public void StrWidth_MixedString_ReturnsCorrectWidth()
        {
            // "a日b" = 1 + 2 + 1 = 4
            Assert.AreEqual(4, GeneralLib.StrWidth("a日b"));
        }

        [TestMethod]
        public void StrWidth_SingleAsciiChar_ReturnsOne()
        {
            Assert.AreEqual(1, GeneralLib.StrWidth("A"));
        }

        [TestMethod]
        public void StrWidth_SingleJapaneseChar_ReturnsTwo()
        {
            Assert.AreEqual(2, GeneralLib.StrWidth("あ"));
        }

        [TestMethod]
        public void StrWidth_HalfWidthKatakana_ReturnsOne()
        {
            // 半角カタカナは幅1
            Assert.AreEqual(1, GeneralLib.StrWidth("ｱ"));
        }

        [TestMethod]
        public void StrWidth_FullWidthDigits_ReturnsTwo()
        {
            // 全角数字は幅2
            Assert.AreEqual(2, GeneralLib.StrWidth("１"));
        }

        // ──────────────────────────────────────────────
        // LNormalize - リストの正規化
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LNormalize_MultipleSpaces_ReturnsNormalized()
        {
            Assert.AreEqual("a b c", GeneralLib.LNormalize("a  b  c"));
        }

        [TestMethod]
        public void LNormalize_LeadingTrailingSpaces_ReturnsNormalized()
        {
            Assert.AreEqual("a b", GeneralLib.LNormalize("  a  b  "));
        }

        [TestMethod]
        public void LNormalize_SingleElement_ReturnsElement()
        {
            Assert.AreEqual("hello", GeneralLib.LNormalize("hello"));
        }

        [TestMethod]
        public void LNormalize_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.LNormalize(""));
        }

        [TestMethod]
        public void LNormalize_NullString_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.LNormalize(null));
        }

        [TestMethod]
        public void LNormalize_OnlySpaces_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.LNormalize("   "));
        }

        // ──────────────────────────────────────────────
        // LSplit - 要素分割（括弧非対応）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LSplit_ThreeElements_ReturnsArrayOfThree()
        {
            int count = GeneralLib.LSplit("a b c", out string[] arr);
            Assert.AreEqual(3, count);
            Assert.AreEqual("a", arr[0]);
            Assert.AreEqual("b", arr[1]);
            Assert.AreEqual("c", arr[2]);
        }

        [TestMethod]
        public void LSplit_EmptyString_ReturnsZeroAndEmptyArray()
        {
            int count = GeneralLib.LSplit("", out string[] arr);
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void LSplit_NullString_ReturnsZeroAndEmptyArray()
        {
            int count = GeneralLib.LSplit(null, out string[] arr);
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void LSplit_SingleElement_ReturnsArrayOfOne()
        {
            int count = GeneralLib.LSplit("abc", out string[] arr);
            Assert.AreEqual(1, count);
            Assert.AreEqual("abc", arr[0]);
        }

        // ──────────────────────────────────────────────
        // LIndex - L系（括弧非考慮）のインデックス
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LIndex_ZeroIndex_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.LIndex("a b c", 0));
        }

        [TestMethod]
        public void LIndex_NegativeIndex_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.LIndex("a b c", -1));
        }

        [TestMethod]
        public void LIndex_ValidIndex_ReturnsElement()
        {
            Assert.AreEqual("a", GeneralLib.LIndex("a b c", 1));
            Assert.AreEqual("b", GeneralLib.LIndex("a b c", 2));
            Assert.AreEqual("c", GeneralLib.LIndex("a b c", 3));
        }

        [TestMethod]
        public void LIndex_OutOfBounds_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.LIndex("a b c", 10));
        }

        [TestMethod]
        public void LIndex_NullString_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.LIndex(null, 1));
        }

        // ──────────────────────────────────────────────
        // LLength - L系（括弧非考慮）の長さ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LLength_ThreeElements_ReturnsThree()
        {
            Assert.AreEqual(3, GeneralLib.LLength("a b c"));
        }

        [TestMethod]
        public void LLength_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.LLength(""));
        }

        [TestMethod]
        public void LLength_NullString_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.LLength(null));
        }

        [TestMethod]
        public void LLength_ParensNotSpecial_CountsAsSeparateTokens()
        {
            // LLength は括弧を考慮しない (ToL を使用)
            Assert.AreEqual(3, GeneralLib.LLength("(a b) c"));
        }

        // ──────────────────────────────────────────────
        // IsSpace
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsSpace_EmptyString_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsSpace(""));
        }

        [TestMethod]
        public void IsSpace_NullString_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsSpace(null));
        }

        [TestMethod]
        public void IsSpace_SingleSpace_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsSpace(" "));
        }

        [TestMethod]
        public void IsSpace_Tab_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsSpace("\t"));
        }

        [TestMethod]
        public void IsSpace_NonSpace_ReturnsFalse()
        {
            Assert.IsFalse(GeneralLib.IsSpace("a"));
        }

        [TestMethod]
        public void IsSpace_SpaceAndNonSpace_ReturnsFalse()
        {
            Assert.IsFalse(GeneralLib.IsSpace(" a "));
        }
    }
}
