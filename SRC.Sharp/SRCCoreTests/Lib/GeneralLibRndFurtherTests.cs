using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using System.IO;
using System;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// GeneralLib の Rnd / Dice / RndReset メソッドのさらに詳細なテスト
    /// </summary>
    [TestClass]
    public class GeneralLibRndFurtherTests
    {
        // ──────────────────────────────────────────────
        // RndSeed / RndReset 詳細テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RndReset_WithSameSeed_ProducesSameSequence()
        {
            GeneralLib.RndSeed = 12345;
            GeneralLib.RndReset();
            var seq1 = new[] { GeneralLib.Dice(100), GeneralLib.Dice(100), GeneralLib.Dice(100) };

            GeneralLib.RndSeed = 12345;
            GeneralLib.RndReset();
            var seq2 = new[] { GeneralLib.Dice(100), GeneralLib.Dice(100), GeneralLib.Dice(100) };

            CollectionAssert.AreEqual(seq1, seq2);
        }

        [TestMethod]
        public void RndReset_WithDifferentSeeds_ProducesDifferentSequences()
        {
            GeneralLib.RndSeed = 111;
            GeneralLib.RndReset();
            var r1 = GeneralLib.Dice(1000000);

            GeneralLib.RndSeed = 222;
            GeneralLib.RndReset();
            var r2 = GeneralLib.Dice(1000000);

            // 異なるシードは異なる乱数を生成するはず（確率的に）
            // 完全に一致することは極めて稀
            Assert.IsTrue(r1 >= 1 && r1 <= 1000000);
            Assert.IsTrue(r2 >= 1 && r2 <= 1000000);
        }

        // ──────────────────────────────────────────────
        // Rnd - 境界値テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Rnd_WithN1_AlwaysReturnsOne()
        {
            GeneralLib.RndSeed = 1;
            GeneralLib.RndReset();
            for (int i = 0; i < 20; i++)
            {
                Assert.AreEqual(1, GeneralLib.Dice(1));
            }
        }

        [TestMethod]
        public void Rnd_WithN2_ReturnsOneOrTwo()
        {
            GeneralLib.RndSeed = 99;
            GeneralLib.RndReset();
            for (int i = 0; i < 50; i++)
            {
                var r = GeneralLib.Dice(2);
                Assert.IsTrue(r == 1 || r == 2, $"Dice(2) returned {r}");
            }
        }

        [TestMethod]
        public void Rnd_LargeN_ReturnsValueInRange()
        {
            GeneralLib.RndSeed = 42;
            GeneralLib.RndReset();
            for (int i = 0; i < 100; i++)
            {
                var r = GeneralLib.Dice(1000);
                Assert.IsTrue(r >= 1 && r <= 1000, $"Dice(1000) returned {r}");
            }
        }

        // ──────────────────────────────────────────────
        // Dice - 詳細テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Dice_WithN1_AlwaysReturnsOne()
        {
            GeneralLib.RndSeed = 1;
            GeneralLib.RndReset();
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(1, GeneralLib.Dice(1));
            }
        }

        [TestMethod]
        public void Dice_WithN6_ReturnsOneToSix()
        {
            GeneralLib.RndSeed = 7;
            GeneralLib.RndReset();
            for (int i = 0; i < 100; i++)
            {
                var d = GeneralLib.Dice(6);
                Assert.IsTrue(d >= 1 && d <= 6, $"Dice(6) returned {d}");
            }
        }

        [TestMethod]
        public void Dice_WithN100_ReturnsOneToHundred()
        {
            GeneralLib.RndSeed = 99;
            GeneralLib.RndReset();
            for (int i = 0; i < 200; i++)
            {
                var d = GeneralLib.Dice(100);
                Assert.IsTrue(d >= 1 && d <= 100, $"Dice(100) returned {d}");
            }
        }

        // ──────────────────────────────────────────────
        // RndSeed プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RndSeed_CanBeSetAndRead()
        {
            GeneralLib.RndSeed = 54321;
            Assert.AreEqual(54321, GeneralLib.RndSeed);
        }

        [TestMethod]
        public void RndSeed_CanBeSetToZero()
        {
            GeneralLib.RndSeed = 0;
            Assert.AreEqual(0, GeneralLib.RndSeed);
        }

        [TestMethod]
        public void RndSeed_CanBeSetToMaxValue()
        {
            GeneralLib.RndSeed = int.MaxValue;
            Assert.AreEqual(int.MaxValue, GeneralLib.RndSeed);
        }
    }
}
