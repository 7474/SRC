using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// GeneralLib.StrWidth, Dice, RndReset のさらなるユニットテスト
    /// </summary>
    [TestClass]
    public class GeneralLibStrWidthTests
    {
        // ──────────────────────────────────────────────
        // StrWidth
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrWidth_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.StrWidth(""));
        }

        [TestMethod]
        public void StrWidth_Null_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.StrWidth(null));
        }

        [TestMethod]
        public void StrWidth_HalfWidthAscii_ReturnsCharCount()
        {
            // ASCII文字は1文字=1幅
            Assert.AreEqual(5, GeneralLib.StrWidth("hello"));
        }

        [TestMethod]
        public void StrWidth_JapaneseChars_DoubleWidth()
        {
            // 全角文字は1文字=2幅
            Assert.AreEqual(6, GeneralLib.StrWidth("あいう"));
        }

        [TestMethod]
        public void StrWidth_MixedChars_CountsCorrectly()
        {
            // "a" + "あ" = 1 + 2 = 3
            Assert.AreEqual(3, GeneralLib.StrWidth("aあ"));
        }

        [TestMethod]
        public void StrWidth_HalfWidthKatakana_ReturnsCharCount()
        {
            // 半角カタカナは1文字=1幅
            Assert.AreEqual(3, GeneralLib.StrWidth("ｱｲｳ"));
        }

        [TestMethod]
        public void StrWidth_Numbers_HalfWidth()
        {
            // 数字は1文字=1幅
            Assert.AreEqual(3, GeneralLib.StrWidth("123"));
        }

        // ──────────────────────────────────────────────
        // Dice
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Dice_MaxIsOne_ReturnsOne()
        {
            // max=1の場合は常に1を返す
            Assert.AreEqual(1, GeneralLib.Dice(1));
        }

        [TestMethod]
        public void Dice_MaxIsZero_ReturnsZero()
        {
            // max <= 1のため0を返す
            Assert.AreEqual(0, GeneralLib.Dice(0));
        }

        [TestMethod]
        public void Dice_MaxNegative_ReturnsNegative()
        {
            Assert.AreEqual(-1, GeneralLib.Dice(-1));
        }

        [TestMethod]
        public void Dice_ValidRange_ReturnsBetween1AndMax()
        {
            GeneralLib.RndReset();
            for (int i = 0; i < 100; i++)
            {
                var result = GeneralLib.Dice(10);
                Assert.IsTrue(result >= 1 && result <= 10, $"Dice returned {result} for max=10");
            }
        }

        [TestMethod]
        public void Dice_Max2_Returns1Or2()
        {
            GeneralLib.RndReset();
            for (int i = 0; i < 20; i++)
            {
                var result = GeneralLib.Dice(2);
                Assert.IsTrue(result == 1 || result == 2);
            }
        }

        // ──────────────────────────────────────────────
        // RndReset
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RndReset_SetsIndexToZero()
        {
            GeneralLib.RndReset();
            Assert.AreEqual(0, GeneralLib.RndIndex);
        }

        [TestMethod]
        public void RndReset_CalledTwice_IsConsistent()
        {
            GeneralLib.RndReset();
            var result1 = GeneralLib.Dice(100);
            GeneralLib.RndReset();
            var result2 = GeneralLib.Dice(100);
            // 同じシードなので同じ結果になるはず
            Assert.AreEqual(result1, result2);
        }

        // ──────────────────────────────────────────────
        // FormatNum
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FormatNum_PositiveInt_ReturnsString()
        {
            var result = GeneralLib.FormatNum(42);
            Assert.AreEqual("42", result);
        }

        [TestMethod]
        public void FormatNum_Zero_ReturnsZero()
        {
            Assert.AreEqual("0", GeneralLib.FormatNum(0));
        }

        [TestMethod]
        public void FormatNum_NegativeInt_ReturnsNegativeString()
        {
            Assert.AreEqual("-5", GeneralLib.FormatNum(-5));
        }

        [TestMethod]
        public void FormatNum_LargeNumber_ReturnsString()
        {
            Assert.AreEqual("1000000", GeneralLib.FormatNum(1000000));
        }
    }
}
