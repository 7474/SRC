using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit のステータス・回復関連のユニットテスト
    /// </summary>
    [TestClass]
    public class UnitStatusTests
    {
        private SRC CreateSrc()
        {
            return new SRC
            {
                GUI = new MockGUI(),
            };
        }

        // ──────────────────────────────────────────────
        // Status フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Status_Default_IsWaiting()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.AreEqual("待機", unit.Status);
        }

        [TestMethod]
        public void Status_CanBeSetToSortie()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Status = "出撃";
            Assert.AreEqual("出撃", unit.Status);
        }

        [TestMethod]
        public void Status_CanBeSetToOtherForm()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Status = "他形態";
            Assert.AreEqual("他形態", unit.Status);
        }

        // ──────────────────────────────────────────────
        // CanHPRecovery
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CanHPRecovery_True_WhenHPBelowMax()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            // デフォルトでは HP=0, MaxHP>=1 なので回復可能
            Assert.IsTrue(unit.CanHPRecovery);
        }

        [TestMethod]
        public void CanHPRecovery_False_WhenHPEqualsMax()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.HP = unit.MaxHP;
            Assert.IsFalse(unit.CanHPRecovery);
        }

        [TestMethod]
        public void CanHPRecovery_False_WhenZombieCondition()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            // HP < MaxHP だがゾンビ状態なら回復不可
            unit.AddCondition("ゾンビ", -1);
            Assert.IsFalse(unit.CanHPRecovery);
        }

        // ──────────────────────────────────────────────
        // CanENRecovery
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CanENRecovery_True_WhenENBelowMax()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            // EN=0, MaxEN>0 なら回復可能
            if (unit.MaxEN > 0)
            {
                Assert.IsTrue(unit.CanENRecovery);
            }
            else
            {
                // MaxEN=0 なら回復不要
                Assert.IsFalse(unit.CanENRecovery);
            }
        }

        [TestMethod]
        public void CanENRecovery_False_WhenENEqualsMax()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.EN = unit.MaxEN;
            Assert.IsFalse(unit.CanENRecovery);
        }

        [TestMethod]
        public void CanENRecovery_False_WhenZombieCondition()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.EN = 0;
            unit.AddCondition("ゾンビ", -1);
            Assert.IsFalse(unit.CanENRecovery);
        }

        // ──────────────────────────────────────────────
        // CanSupply
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CanSupply_True_WhenENBelowMax()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            // ENが満タンでない場合は補給可能
            if (unit.MaxEN > 0)
            {
                Assert.IsTrue(unit.CanSupply);
            }
        }

        [TestMethod]
        public void CanSupply_WhenENBelowMax_ReturnsTrue()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            // EN=0、MaxEN>0 なら補給可能
            if (unit.MaxEN > 0)
            {
                Assert.IsTrue(unit.CanSupply);
            }
            else
            {
                // MaxEN=0 かつ弾数・ストックもなければ補給不要
                Assert.IsFalse(unit.CanSupply);
            }
        }

        // ──────────────────────────────────────────────
        // RecoverHP
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RecoverHP_IncreasesHP()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            // HP=0, MaxHP>=1 の状態で回復
            var beforeHP = unit.HP;
            unit.RecoverHP(50);
            Assert.IsTrue(unit.HP > beforeHP || unit.HP == unit.MaxHP);
        }

        [TestMethod]
        public void RecoverHP_FullPercent_SetsHPToMax()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.HP = 0;
            unit.RecoverHP(100);
            Assert.AreEqual(unit.MaxHP, unit.HP);
        }

        [TestMethod]
        public void RecoverHP_AtLeastOne_AfterRecovery()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.HP = 0;
            unit.RecoverHP(1);
            // RecoverHP は HP が 0 以下になるのを防ぐため最低 1 を保証
            Assert.IsTrue(unit.HP >= 1);
        }

        // ──────────────────────────────────────────────
        // RecoverEN
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RecoverEN_FullPercent_SetsENToMax()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.EN = 0;
            unit.RecoverEN(100);
            Assert.AreEqual(unit.MaxEN, unit.EN);
        }

        [TestMethod]
        public void RecoverEN_ZeroPercent_DoesNotChangeEN()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            var initialEN = unit.EN;
            unit.RecoverEN(0);
            Assert.AreEqual(initialEN, unit.EN);
        }
    }
}
