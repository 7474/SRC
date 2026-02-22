using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    [TestClass]
    public class UnitPartyMoreTests2
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ===== IsEnemy with Mode="味方" targeting partner =====

        [TestMethod]
        public void IsEnemy_EnemyWithModeAlly_TargetsAllyParty()
        {
            var src = CreateSrc();
            var enemy = new Unit(src) { Party = "敵", Mode = "味方" };
            var ally = new Unit(src) { Party = "味方" };
            // Mode="味方" means it targets "味方" and "ＮＰＣ" parties
            Assert.IsTrue(enemy.IsEnemy(ally));
        }

        [TestMethod]
        public void IsEnemy_EnemyWithModeAlly_NotTargetingOtherEnemy()
        {
            var src = CreateSrc();
            var enemy1 = new Unit(src) { Party = "敵", Mode = "味方" };
            var enemy2 = new Unit(src) { Party = "敵" };
            Assert.IsFalse(enemy1.IsEnemy(enemy2));
        }

        // ===== IsEnemy between NPC parties =====

        [TestMethod]
        public void IsEnemy_BetweenNPCs_ReturnsFalse()
        {
            var src = CreateSrc();
            var npc1 = new Unit(src) { Party = "ＮＰＣ" };
            var npc2 = new Unit(src) { Party = "ＮＰＣ" };
            Assert.IsFalse(npc1.IsEnemy(npc2));
        }

        [TestMethod]
        public void IsEnemy_NPCVsEnemy_ReturnsTrue()
        {
            var src = CreateSrc();
            var npc = new Unit(src) { Party = "ＮＰＣ" };
            var enemy = new Unit(src) { Party = "敵" };
            Assert.IsTrue(npc.IsEnemy(enemy));
        }

        // ===== IsAlly with NPC and NPC =====

        [TestMethod]
        public void IsAlly_NPCAndNPC_ReturnsTrue()
        {
            var src = CreateSrc();
            var npc1 = new Unit(src) { Party = "ＮＰＣ" };
            var npc2 = new Unit(src) { Party = "ＮＰＣ" };
            Assert.IsTrue(npc1.IsAlly(npc2));
        }

        [TestMethod]
        public void IsAlly_AllyAndNPC_ReturnsTrue()
        {
            var src = CreateSrc();
            var ally = new Unit(src) { Party = "味方" };
            var npc = new Unit(src) { Party = "ＮＰＣ" };
            Assert.IsTrue(ally.IsAlly(npc));
        }

        [TestMethod]
        public void IsAlly_AllyAndEnemy_ReturnsFalse()
        {
            var src = CreateSrc();
            var ally = new Unit(src) { Party = "味方" };
            var enemy = new Unit(src) { Party = "敵" };
            Assert.IsFalse(ally.IsAlly(enemy));
        }

        // ===== IsEnemy with 混乱 condition (for_move=true) =====

        [TestMethod]
        public void IsEnemy_ConfusedUnit_ForMove_ReturnsTrue()
        {
            var src = CreateSrc();
            var confused = new Unit(src) { Party = "味方" };
            var ally = new Unit(src) { Party = "味方" };
            confused.AddCondition("混乱", -1);
            // for_move=true always returns true for confused unit
            Assert.IsTrue(confused.IsEnemy(ally, for_move: true));
        }

        [TestMethod]
        public void IsEnemy_Self_AlwaysReturnsFalse()
        {
            var src = CreateSrc();
            var unit = new Unit(src) { Party = "味方" };
            Assert.IsFalse(unit.IsEnemy(unit));
        }

        [TestMethod]
        public void IsAlly_Self_AlwaysReturnsTrue()
        {
            var src = CreateSrc();
            var unit = new Unit(src) { Party = "敵" };
            Assert.IsTrue(unit.IsAlly(unit));
        }

        [TestMethod]
        public void IsEnemy_BetweenDifferentEnemyParties_ReturnsTrue()
        {
            var src = CreateSrc();
            var enemy = new Unit(src) { Party = "敵" };
            var neutral = new Unit(src) { Party = "中立" };
            Assert.IsTrue(enemy.IsEnemy(neutral));
        }
    }
}
