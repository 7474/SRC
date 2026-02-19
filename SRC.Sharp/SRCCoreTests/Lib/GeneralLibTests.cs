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
    }
}
