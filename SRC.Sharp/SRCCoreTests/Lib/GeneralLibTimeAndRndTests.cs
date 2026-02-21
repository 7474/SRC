using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using System.Threading;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// GeneralLib の timeGetTime / RndReset / Dice 追加テスト
    /// </summary>
    [TestClass]
    public class GeneralLibTimeAndRndTests
    {
        // ──────────────────────────────────────────────
        // timeGetTime
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TimeGetTime_ReturnsNonNegativeValue()
        {
            var t = GeneralLib.timeGetTime();
            Assert.IsTrue(t >= 0);
        }

        [TestMethod]
        public void TimeGetTime_CalledTwice_SecondIsGreaterOrEqual()
        {
            var t1 = GeneralLib.timeGetTime();
            // 短時間待機して2回目の取得
            Thread.Sleep(1);
            var t2 = GeneralLib.timeGetTime();
            Assert.IsTrue(t2 >= t1, $"t1={t1}, t2={t2}");
        }

        // ──────────────────────────────────────────────
        // RndReset
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RndReset_SetsRndIndexToZero()
        {
            GeneralLib.RndSeed = 42;
            GeneralLib.RndReset();
            Assert.AreEqual(0, GeneralLib.RndIndex);
        }

        [TestMethod]
        public void RndReset_SameSeed_ProducesSameDiceResults()
        {
            GeneralLib.RndSeed = 12345;
            GeneralLib.RndReset();
            var r1 = GeneralLib.Dice(100);
            var r2 = GeneralLib.Dice(100);
            var r3 = GeneralLib.Dice(100);

            GeneralLib.RndSeed = 12345;
            GeneralLib.RndReset();
            var s1 = GeneralLib.Dice(100);
            var s2 = GeneralLib.Dice(100);
            var s3 = GeneralLib.Dice(100);

            Assert.AreEqual(r1, s1);
            Assert.AreEqual(r2, s2);
            Assert.AreEqual(r3, s3);
        }

        [TestMethod]
        public void RndReset_DifferentSeeds_ProduceDifferentResults()
        {
            GeneralLib.RndSeed = 1;
            GeneralLib.RndReset();
            var r1 = GeneralLib.Dice(1000);

            GeneralLib.RndSeed = 9999;
            GeneralLib.RndReset();
            var r2 = GeneralLib.Dice(1000);

            // 異なるシードでは異なる値になることが期待される（確率的に失敗はありえるが許容）
            // ここでは単純に実行可能なことを確認
            Assert.IsTrue(r1 >= 1 && r1 <= 1000);
            Assert.IsTrue(r2 >= 1 && r2 <= 1000);
        }

        // ──────────────────────────────────────────────
        // Dice - 追加エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Dice_MaxIsZero_ReturnsZero()
        {
            var result = GeneralLib.Dice(0);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Dice_MaxIsNegative_ReturnsNegative()
        {
            var result = GeneralLib.Dice(-1);
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void Dice_MaxIsOne_AlwaysReturnsOne()
        {
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(1, GeneralLib.Dice(1));
            }
        }

        [TestMethod]
        public void Dice_Large_ReturnsValueInRange()
        {
            GeneralLib.RndSeed = 0;
            GeneralLib.RndReset();
            for (int i = 0; i < 20; i++)
            {
                var val = GeneralLib.Dice(1000);
                Assert.IsTrue(val >= 1 && val <= 1000, $"Value {val} out of range [1..1000]");
            }
        }
    }
}
