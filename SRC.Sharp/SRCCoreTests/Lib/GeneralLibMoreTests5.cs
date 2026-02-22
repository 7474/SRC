using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// GeneralLib の追加ユニットテスト (GeneralLibMoreTests5)
    /// </summary>
    [TestClass]
    public class GeneralLibMoreTests5
    {
        // ──────────────────────────────────────────────
        // Dice - 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Dice_N10_ReturnsValueBetween1And10()
        {
            for (int i = 0; i < 30; i++)
            {
                int result = GeneralLib.Dice(10);
                Assert.IsTrue(result >= 1 && result <= 10, $"Dice(10) returned {result}");
            }
        }

        [TestMethod]
        public void Dice_N3_ReturnsValueBetween1And3()
        {
            for (int i = 0; i < 30; i++)
            {
                int result = GeneralLib.Dice(3);
                Assert.IsTrue(result >= 1 && result <= 3, $"Dice(3) returned {result}");
            }
        }

        [TestMethod]
        public void Dice_N100_ReturnsValueBetween1And100()
        {
            for (int i = 0; i < 30; i++)
            {
                int result = GeneralLib.Dice(100);
                Assert.IsTrue(result >= 1 && result <= 100, $"Dice(100) returned {result}");
            }
        }

        // ──────────────────────────────────────────────
        // InStr2 (末尾から検索)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStr2_SingleOccurrence_ReturnsPosition()
        {
            Assert.AreEqual(3, GeneralLib.InStr2("abcde", "c"));
        }

        [TestMethod]
        public void InStr2_MultipleOccurrences_ReturnsLastPosition()
        {
            // "abcabc" contains "c" at positions 3 and 6; InStr2 returns last occurrence
            Assert.AreEqual(6, GeneralLib.InStr2("abcabc", "c"));
        }

        [TestMethod]
        public void InStr2_NotFound_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.InStr2("hello", "xyz"));
        }

        [TestMethod]
        public void InStr2_EmptyHaystack_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.InStr2("", "a"));
        }

        // ──────────────────────────────────────────────
        // StrToHiragana
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrToHiragana_Katakana_ConvertsToHiragana()
        {
            // アイウ → あいう
            var result = GeneralLib.StrToHiragana("アイウ");
            Assert.AreEqual("あいう", result);
        }

        [TestMethod]
        public void StrToHiragana_Hiragana_ReturnsSame()
        {
            var result = GeneralLib.StrToHiragana("あいう");
            Assert.AreEqual("あいう", result);
        }

        [TestMethod]
        public void StrToHiragana_EmptyString_ReturnsEmpty()
        {
            var result = GeneralLib.StrToHiragana("");
            Assert.AreEqual("", result);
        }

        // ──────────────────────────────────────────────
        // LeftPaddedString / RightPaddedString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LeftPaddedString_ShortString_PadsOnLeft()
        {
            var result = GeneralLib.LeftPaddedString("abc", 6);
            Assert.AreEqual("   abc", result);
        }

        [TestMethod]
        public void LeftPaddedString_ExactLength_NoPadding()
        {
            var result = GeneralLib.LeftPaddedString("abc", 3);
            Assert.AreEqual("abc", result);
        }

        [TestMethod]
        public void RightPaddedString_ShortString_PadsOnRight()
        {
            var result = GeneralLib.RightPaddedString("abc", 6);
            Assert.AreEqual("abc   ", result);
        }

        [TestMethod]
        public void RightPaddedString_ExactLength_NoPadding()
        {
            var result = GeneralLib.RightPaddedString("abc", 3);
            Assert.AreEqual("abc", result);
        }

        // ──────────────────────────────────────────────
        // StrToDbl
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrToDbl_ValidDouble_ReturnsDouble()
        {
            Assert.AreEqual(3.14, GeneralLib.StrToDbl("3.14"), 0.0001);
        }

        [TestMethod]
        public void StrToDbl_IntegerString_ReturnsDouble()
        {
            Assert.AreEqual(42.0, GeneralLib.StrToDbl("42"), 0.0001);
        }
    }
}
