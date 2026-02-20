using SRCCore.Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Lib.Tests
{
    [TestClass()]
    public class GeneralLibTests
    {
        struct ToListTestCase
        {
            public string input;
            public IList<string> expected;
        }
        [TestMethod()]
        public void ToListTest()
        {
            // TODO カッコの扱いちゃんと見る
            var cases = new ToListTestCase[]
            {
                new ToListTestCase{ input= "いちご ニンジン サンダル", expected= new List<string>{ "いちご", "ニンジン", "サンダル" } },
                new ToListTestCase{ input= "This is a pen.", expected= new List<string>{ "This", "is", "a", "pen." } },
                //new ToListTestCase{ input= "a (b c) (d (e f))", expected= new List<string>{ "a", "b c", "d (e f)" } },
                new ToListTestCase{ input= "こぶた たぬき きつね ねこ", expected= new List<string>{ "こぶた", "たぬき", "きつね", "ねこ" } },
                //new ToListTestCase{ input= "a (b c)) (d (e f))", expected= new List<string>{ "a", "b c", ") (d (e f))" } },
            };
            foreach (var c in cases)
            {
                var actual = GeneralLib.ToList(c.input);
                Console.WriteLine(c.input + ": " + JsonConvert.SerializeObject(actual));
                Assert.IsTrue(c.expected.SequenceEqual(actual), $"case: {c.input}");
            }
        }

        [TestMethod()]
        public void FormatNumTest()
        {
            Assert.AreEqual("100000000000000000000", GeneralLib.FormatNum(1e20));
            Assert.AreEqual("0.1", GeneralLib.FormatNum(1e-1));
        }

        [TestMethod()]
        public void StrWidthTest()
        {
            Assert.AreEqual(0, GeneralLib.StrWidth(null));
            Assert.AreEqual(0, GeneralLib.StrWidth(""));
            Assert.AreEqual(3, GeneralLib.StrWidth("123"));
            Assert.AreEqual(4, GeneralLib.StrWidth("1２3"));
            Assert.AreEqual(6, GeneralLib.StrWidth("１２３"));
        }

        [TestMethod()]
        public void StrToLngTest()
        {
            Assert.AreEqual(0, GeneralLib.StrToLng(""));
            Assert.AreEqual(1, GeneralLib.StrToLng("1"));
            Assert.AreEqual(1, GeneralLib.StrToLng("1.0"));
            Assert.AreEqual(1, GeneralLib.StrToLng("1.1"));
            Assert.AreEqual(1, GeneralLib.StrToLng("1.9"));
            //Assert.AreEqual(1, GeneralLib.StrToLng("0x1"));
        }

        [TestMethod()]
        public void DiceTest_ReturnsValueInRange()
        {
            GeneralLib.RndSeed = 42;
            GeneralLib.RndReset();

            for (int i = 0; i < 100; i++)
            {
                int result = GeneralLib.Dice(6);
                Assert.IsTrue(result >= 1 && result <= 6, $"Dice(6) returned {result} which is out of range [1, 6]");
            }
        }

        [TestMethod()]
        public void DiceTest_MaxOne_ReturnsOne()
        {
            Assert.AreEqual(1, GeneralLib.Dice(1));
            // Dice(0) returns 0 (same behavior as original VB: max <= 1 returns max)
            Assert.AreEqual(0, GeneralLib.Dice(0));
        }

        [TestMethod()]
        public void RndReset_SameSeed_ProducesSameSequence()
        {
            // Same seed should produce reproducible dice results
            GeneralLib.RndSeed = 12345;
            GeneralLib.RndReset();
            var seq1 = Enumerable.Range(0, 20).Select(_ => GeneralLib.Dice(100)).ToArray();

            GeneralLib.RndSeed = 12345;
            GeneralLib.RndReset();
            var seq2 = Enumerable.Range(0, 20).Select(_ => GeneralLib.Dice(100)).ToArray();

            CollectionAssert.AreEqual(seq1, seq2, "Same seed should produce same dice sequence");
        }

        [TestMethod()]
        public void RndReset_DifferentSeed_ProducesDifferentSequence()
        {
            GeneralLib.RndSeed = 1;
            GeneralLib.RndReset();
            var seq1 = Enumerable.Range(0, 20).Select(_ => GeneralLib.Dice(100)).ToArray();

            GeneralLib.RndSeed = 2;
            GeneralLib.RndReset();
            var seq2 = Enumerable.Range(0, 20).Select(_ => GeneralLib.Dice(100)).ToArray();

            Assert.IsFalse(seq1.SequenceEqual(seq2), "Different seeds should produce different dice sequences");
        }

        [TestMethod()]
        public void RndIndex_SaveRestore_ResumesSequence()
        {
            // Simulate saving and restoring the random state mid-sequence
            GeneralLib.RndSeed = 999;
            GeneralLib.RndReset();

            // Advance some steps
            for (int i = 0; i < 50; i++) GeneralLib.Dice(100);

            // Save state
            int savedSeed = GeneralLib.RndSeed;
            int savedIndex = GeneralLib.RndIndex;

            // Generate next 10 values
            var expected = Enumerable.Range(0, 10).Select(_ => GeneralLib.Dice(100)).ToArray();

            // Restore state
            GeneralLib.RndSeed = savedSeed;
            GeneralLib.RndReset();
            GeneralLib.RndIndex = savedIndex;

            // Should produce same values
            var actual = Enumerable.Range(0, 10).Select(_ => GeneralLib.Dice(100)).ToArray();
            CollectionAssert.AreEqual(expected, actual, "Restored random state should resume the same sequence");
        }

        [TestMethod()]
        public void RndIndex_WrapsAround()
        {
            GeneralLib.RndSeed = 1;
            GeneralLib.RndReset();
            // Set index to near the end to test wrap-around
            GeneralLib.RndIndex = 4095;
            // Should advance to 4096
            GeneralLib.Dice(100);
            Assert.AreEqual(4096, GeneralLib.RndIndex);
            // Should wrap back to 1
            GeneralLib.Dice(100);
            Assert.AreEqual(1, GeneralLib.RndIndex);
        }

        // ──────────────────────────────────────────────
        // LNormalize / LIndex / LLength / LSplit / IsSpace
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void LNormalizeTest()
        {
            Assert.AreEqual("a b c", GeneralLib.LNormalize("a  b  c"));
            Assert.AreEqual("a b c", GeneralLib.LNormalize("  a b c  "));
            Assert.AreEqual("", GeneralLib.LNormalize(""));
            Assert.AreEqual("", GeneralLib.LNormalize(null));
        }

        [TestMethod()]
        public void LIndexTest()
        {
            Assert.AreEqual("いちご", GeneralLib.LIndex("いちご みかん ぶどう", 1));
            Assert.AreEqual("みかん", GeneralLib.LIndex("いちご みかん ぶどう", 2));
            Assert.AreEqual("ぶどう", GeneralLib.LIndex("いちご みかん ぶどう", 3));
            Assert.AreEqual("", GeneralLib.LIndex("いちご みかん ぶどう", 4));
            Assert.AreEqual("", GeneralLib.LIndex("いちご みかん ぶどう", 0));
            Assert.AreEqual("", GeneralLib.LIndex("", 1));
        }

        [TestMethod()]
        public void LLengthTest()
        {
            Assert.AreEqual(3, GeneralLib.LLength("a b c"));
            Assert.AreEqual(1, GeneralLib.LLength("a"));
            Assert.AreEqual(0, GeneralLib.LLength(""));
            Assert.AreEqual(0, GeneralLib.LLength(null));
        }

        [TestMethod()]
        public void LSplitTest()
        {
            var count = GeneralLib.LSplit("a b c", out var arr);
            Assert.AreEqual(3, count);
            Assert.AreEqual("a", arr[0]);
            Assert.AreEqual("b", arr[1]);
            Assert.AreEqual("c", arr[2]);

            var count2 = GeneralLib.LSplit("", out var arr2);
            Assert.AreEqual(0, count2);
            Assert.AreEqual(0, arr2.Length);
        }

        [TestMethod()]
        public void IsSpaceTest()
        {
            Assert.IsTrue(GeneralLib.IsSpace(" "));
            Assert.IsTrue(GeneralLib.IsSpace("\t"));
            Assert.IsTrue(GeneralLib.IsSpace(""));
            Assert.IsTrue(GeneralLib.IsSpace(null));
            Assert.IsFalse(GeneralLib.IsSpace("a"));
            Assert.IsFalse(GeneralLib.IsSpace("1"));
        }

        // ──────────────────────────────────────────────
        // ListIndex / ListLength / ListSplit / ListTail
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void ListIndexTest()
        {
            Assert.AreEqual("a", GeneralLib.ListIndex("a b c", 1));
            Assert.AreEqual("b", GeneralLib.ListIndex("a b c", 2));
            Assert.AreEqual("c", GeneralLib.ListIndex("a b c", 3));
            Assert.AreEqual("", GeneralLib.ListIndex("a b c", 4));
            Assert.AreEqual("", GeneralLib.ListIndex("a b c", 0));
        }

        [TestMethod()]
        public void ListLengthTest()
        {
            Assert.AreEqual(3, GeneralLib.ListLength("a b c"));
            Assert.AreEqual(1, GeneralLib.ListLength("a"));
            Assert.AreEqual(0, GeneralLib.ListLength(""));
        }

        [TestMethod()]
        public void ListSplitTest()
        {
            var count = GeneralLib.ListSplit("a b c", out var arr);
            Assert.AreEqual(3, count);
            Assert.AreEqual("a", arr[0]);
            Assert.AreEqual("b", arr[1]);
            Assert.AreEqual("c", arr[2]);

            var count2 = GeneralLib.ListSplit("", out var arr2);
            Assert.AreEqual(0, count2);
            Assert.AreEqual(0, arr2.Length);
        }

        [TestMethod()]
        public void ListTailTest()
        {
            Assert.AreEqual("b c", GeneralLib.ListTail("a b c", 2));
            Assert.AreEqual("c", GeneralLib.ListTail("a b c", 3));
            Assert.AreEqual("", GeneralLib.ListTail("a b c", 4));
            // idx <= 1 の場合は空文字列を返す（VB6の仕様に基づく）
            Assert.AreEqual("", GeneralLib.ListTail("a b c", 1));
        }

        // ──────────────────────────────────────────────
        // InStr2
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void InStr2Test()
        {
            // InStr2 は末尾から検索して最後の一致位置を返す
            Assert.AreEqual(4, GeneralLib.InStr2("abcabc", "abc"));
            Assert.AreEqual(1, GeneralLib.InStr2("abcdef", "abc"));
            Assert.AreEqual(0, GeneralLib.InStr2("abcdef", "xyz"));
            Assert.AreEqual(4, GeneralLib.InStr2("abcbc", "bc"));
        }

        // ──────────────────────────────────────────────
        // StrToDbl
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void StrToDblTest()
        {
            Assert.AreEqual(3.14, GeneralLib.StrToDbl("3.14"));
            Assert.AreEqual(0d, GeneralLib.StrToDbl("abc"));
            Assert.AreEqual(0d, GeneralLib.StrToDbl(""));
            Assert.AreEqual(42d, GeneralLib.StrToDbl("42"));
        }

        // ──────────────────────────────────────────────
        // StrToHiragana
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void StrToHiraganaTest()
        {
            Assert.AreEqual("あいう", GeneralLib.StrToHiragana("アイウ"));
            Assert.AreEqual("あいう", GeneralLib.StrToHiragana("あいう"));
            Assert.AreEqual("abc", GeneralLib.StrToHiragana("abc"));
        }

        // ──────────────────────────────────────────────
        // MaxLng / MinLng / MaxDbl / MinDbl
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void MaxLngTest()
        {
            Assert.AreEqual(5, GeneralLib.MaxLng(3, 5));
            Assert.AreEqual(5, GeneralLib.MaxLng(5, 3));
            Assert.AreEqual(5, GeneralLib.MaxLng(5, 5));
            Assert.AreEqual(0, GeneralLib.MaxLng(-1, 0));
        }

        [TestMethod()]
        public void MinLngTest()
        {
            Assert.AreEqual(3, GeneralLib.MinLng(3, 5));
            Assert.AreEqual(3, GeneralLib.MinLng(5, 3));
            Assert.AreEqual(3, GeneralLib.MinLng(3, 3));
            Assert.AreEqual(-1, GeneralLib.MinLng(-1, 0));
        }

        [TestMethod()]
        public void MaxDblTest()
        {
            Assert.AreEqual(5.5, GeneralLib.MaxDbl(3.3, 5.5));
            Assert.AreEqual(5.5, GeneralLib.MaxDbl(5.5, 3.3));
        }

        [TestMethod()]
        public void MinDblTest()
        {
            Assert.AreEqual(3.3, GeneralLib.MinDbl(3.3, 5.5));
            Assert.AreEqual(3.3, GeneralLib.MinDbl(5.5, 3.3));
        }

        // ──────────────────────────────────────────────
        // LeftPaddedString / RightPaddedString
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void LeftPaddedStringTest()
        {
            Assert.AreEqual("  abc", GeneralLib.LeftPaddedString("abc", 5));
            Assert.AreEqual("abc", GeneralLib.LeftPaddedString("abc", 3));
            Assert.AreEqual("abc", GeneralLib.LeftPaddedString("abc", 2));
            Assert.AreEqual(" abc", GeneralLib.LeftPaddedString("abc", 4));
        }

        [TestMethod()]
        public void RightPaddedStringTest()
        {
            Assert.AreEqual("abc  ", GeneralLib.RightPaddedString("abc", 5));
            Assert.AreEqual("abc", GeneralLib.RightPaddedString("abc", 3));
            Assert.AreEqual("abc", GeneralLib.RightPaddedString("abc", 2));
            Assert.AreEqual("abc ", GeneralLib.RightPaddedString("abc", 4));
        }

        // ──────────────────────────────────────────────
        // IsNumber
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void IsNumberTest()
        {
            Assert.IsTrue(GeneralLib.IsNumber("42"));
            Assert.IsTrue(GeneralLib.IsNumber("3.14"));
            Assert.IsTrue(GeneralLib.IsNumber("-10"));
            Assert.IsFalse(GeneralLib.IsNumber("abc"));
            Assert.IsFalse(GeneralLib.IsNumber(""));
            Assert.IsFalse(GeneralLib.IsNumber("(1)"));
        }

        // ──────────────────────────────────────────────
        // GetClassBundle
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void GetClassBundleTest_SimpleChar()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("炎水", ref idx);
            Assert.AreEqual("炎", result);
            Assert.AreEqual(1, idx);
        }

        [TestMethod()]
        public void GetClassBundleTest_WeakPrefix()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("弱炎", ref idx);
            Assert.AreEqual("弱炎", result);
            Assert.AreEqual(2, idx);
        }

        [TestMethod()]
        public void GetClassBundleTest_LowPrefix()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("低攻", ref idx);
            Assert.AreEqual("低攻", result);
            Assert.AreEqual(2, idx);
        }

        // ──────────────────────────────────────────────
        // InStrNotNest
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void InStrNotNestTest_Found()
        {
            Assert.AreEqual(1, GeneralLib.InStrNotNest("炎水", "炎"));
            Assert.AreEqual(2, GeneralLib.InStrNotNest("水炎", "炎"));
        }

        [TestMethod()]
        public void InStrNotNestTest_NotFound()
        {
            Assert.AreEqual(0, GeneralLib.InStrNotNest("水風", "炎"));
        }

        [TestMethod()]
        public void InStrNotNestTest_IgnoresNested()
        {
            // 「弱」は属性の接頭辞として働き「弱炎」は複合属性を構成する。
            // そのため「弱炎」内の「炎」は独立した属性ではなく「弱炎」の一部として扱われ0を返す。
            Assert.AreEqual(0, GeneralLib.InStrNotNest("弱炎", "炎"));
        }

        [TestMethod()]
        public void InStrNotNestTest_WithStartPosition()
        {
            Assert.AreEqual(3, GeneralLib.InStrNotNest("水炎炎", "炎", 3));
        }
    }
}
