using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using System;

namespace SRCCore.Tests
{
    /// <summary>
    /// SRC.util.cs の追加テスト（IncrMoney, Rand, Version, ConvertUnitID）
    /// </summary>
    [TestClass]
    public class SRCIncrMoneyTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // IncrMoney 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IncrMoney_NegativeFromMax_DecreasesCorrectly()
        {
            var src = CreateSrc();
            src.Money = 999999999;
            src.IncrMoney(-999999999);
            Assert.AreEqual(0, src.Money);
        }

        [TestMethod]
        public void IncrMoney_AddOneToMaxMinusOne_ReachesMax()
        {
            var src = CreateSrc();
            src.Money = 999999998;
            src.IncrMoney(1);
            Assert.AreEqual(999999999, src.Money);
        }

        [TestMethod]
        public void IncrMoney_LargeNegative_ClampedToZero()
        {
            var src = CreateSrc();
            src.Money = 500;
            src.IncrMoney(int.MinValue + 501);
            Assert.AreEqual(0, src.Money);
        }

        [TestMethod]
        public void IncrMoney_SequentialCalls_AccumulateCorrectly()
        {
            var src = CreateSrc();
            src.Money = 0;
            src.IncrMoney(100);
            src.IncrMoney(200);
            src.IncrMoney(300);
            Assert.AreEqual(600, src.Money);
        }

        [TestMethod]
        public void IncrMoney_SequentialCalls_ClampAtZero()
        {
            var src = CreateSrc();
            src.Money = 50;
            src.IncrMoney(-30);
            Assert.AreEqual(20, src.Money);
            src.IncrMoney(-30);
            Assert.AreEqual(0, src.Money);
        }

        [TestMethod]
        public void IncrMoney_MaxThenPositive_StaysAtMax()
        {
            var src = CreateSrc();
            src.Money = 999999999;
            src.IncrMoney(1);
            Assert.AreEqual(999999999, src.Money);
        }

        // ──────────────────────────────────────────────
        // Rand 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Rand_MultipleInstances_ProduceValues()
        {
            var src1 = CreateSrc();
            var src2 = CreateSrc();
            var v1 = src1.Rand();
            var v2 = src2.Rand();
            Assert.IsTrue(v1 >= 0.0 && v1 < 1.0);
            Assert.IsTrue(v2 >= 0.0 && v2 < 1.0);
        }

        [TestMethod]
        public void Rand_LargeNumberOfCalls_AllInRange()
        {
            var src = CreateSrc();
            for (int i = 0; i < 1000; i++)
            {
                var value = src.Rand();
                Assert.IsTrue(value >= 0.0 && value < 1.0,
                    $"反復{i}: {value} が [0, 1) の範囲外");
            }
        }

        // ──────────────────────────────────────────────
        // Version 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Version_SetAndRetrieve_MatchesExactVersion()
        {
            var src = CreateSrc();
            var ver = new Version(2, 5, 0, 0);
            src.Version = ver;
            Assert.AreEqual(2, src.Version.Major);
            Assert.AreEqual(5, src.Version.Minor);
        }

        [TestMethod]
        public void Version_Default_HasMajorAndMinor()
        {
            var src = CreateSrc();
            Assert.IsTrue(src.Version.Major >= 0);
            Assert.IsTrue(src.Version.Minor >= 0);
        }

        // ──────────────────────────────────────────────
        // ConvertUnitID 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ConvertUnitID_MultipleColons_Unchanged()
        {
            var src = CreateSrc();
            string id = "ユニット:1:2";
            src.ConvertUnitID(ref id);
            Assert.AreEqual("ユニット:1:2", id);
        }

        [TestMethod]
        public void ConvertUnitID_NoNumericSuffix_InsertsColonAtEnd()
        {
            var src = CreateSrc();
            string id = "ガンダム";
            src.ConvertUnitID(ref id);
            // 数値部分がないのでコロンが末尾に挿入される
            Assert.AreEqual(":ガンダム", id);
        }

        [TestMethod]
        public void ConvertUnitID_OnlyDigits_InsertsColonAtStart()
        {
            var src = CreateSrc();
            string id = "123";
            src.ConvertUnitID(ref id);
            Assert.AreEqual(":123", id);
        }

        [TestMethod]
        public void ConvertUnitID_ColonOnly_Unchanged()
        {
            var src = CreateSrc();
            string id = ":";
            src.ConvertUnitID(ref id);
            Assert.AreEqual(":", id);
        }
    }
}
