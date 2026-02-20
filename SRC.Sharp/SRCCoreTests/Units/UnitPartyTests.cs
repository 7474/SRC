using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    [TestClass]
    public class UnitPartyTests
    {
        private SRC CreateSrc()
        {
            return new SRC
            {
                GUI = new MockGUI(),
            };
        }

        // ===== IsEnemy =====

        [TestMethod]
        public void IsEnemy_ReturnsFalse_ForSelf()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Party = "味方";

            Assert.IsFalse(unit.IsEnemy(unit));
        }

        [TestMethod]
        public void IsEnemy_ReturnsTrue_WhenBerserk()
        {
            var src = CreateSrc();
            var unit = new Unit(src) { Party = "味方" };
            var target = new Unit(src) { Party = "味方" };

            unit.AddCondition("暴走", -1);

            Assert.IsTrue(unit.IsEnemy(target));
        }

        [TestMethod]
        public void IsEnemy_ReturnsFalse_BetweenAllies()
        {
            var src = CreateSrc();
            var unit1 = new Unit(src) { Party = "味方" };
            var unit2 = new Unit(src) { Party = "味方" };

            Assert.IsFalse(unit1.IsEnemy(unit2));
        }

        [TestMethod]
        public void IsEnemy_ReturnsFalse_BetweenAllyAndNPC()
        {
            var src = CreateSrc();
            var ally = new Unit(src) { Party = "味方" };
            var npc = new Unit(src) { Party = "ＮＰＣ" };

            Assert.IsFalse(ally.IsEnemy(npc));
        }

        [TestMethod]
        public void IsEnemy_ReturnsTrue_BetweenAllyAndEnemy()
        {
            var src = CreateSrc();
            var ally = new Unit(src) { Party = "味方" };
            var enemy = new Unit(src) { Party = "敵" };

            Assert.IsTrue(ally.IsEnemy(enemy));
        }

        [TestMethod]
        public void IsEnemy_ReturnsTrue_BetweenAllyAndBerserkNPC()
        {
            var src = CreateSrc();
            var ally = new Unit(src) { Party = "味方" };
            var npc = new Unit(src) { Party = "ＮＰＣ" };

            npc.AddCondition("暴走", -1);

            Assert.IsTrue(ally.IsEnemy(npc));
        }

        [TestMethod]
        public void IsEnemy_ReturnsFalse_BetweenSamePartyEnemies()
        {
            var src = CreateSrc();
            var enemy1 = new Unit(src) { Party = "敵" };
            var enemy2 = new Unit(src) { Party = "敵" };

            Assert.IsFalse(enemy1.IsEnemy(enemy2));
        }

        [TestMethod]
        public void IsEnemy_ReturnsTrue_BetweenDifferentNonAllyParties()
        {
            var src = CreateSrc();
            var enemy = new Unit(src) { Party = "敵" };
            var neutral = new Unit(src) { Party = "中立" };

            Assert.IsTrue(enemy.IsEnemy(neutral));
        }

        // ===== IsAlly =====

        [TestMethod]
        public void IsAlly_ReturnsTrue_ForSelf()
        {
            var src = CreateSrc();
            var unit = new Unit(src) { Party = "味方" };

            Assert.IsTrue(unit.IsAlly(unit));
        }

        [TestMethod]
        public void IsAlly_ReturnsFalse_WhenBerserk()
        {
            var src = CreateSrc();
            var unit = new Unit(src) { Party = "味方" };
            var target = new Unit(src) { Party = "味方" };

            unit.AddCondition("暴走", -1);

            Assert.IsFalse(unit.IsAlly(target));
        }

        [TestMethod]
        public void IsAlly_ReturnsTrue_BetweenAllies()
        {
            var src = CreateSrc();
            var unit1 = new Unit(src) { Party = "味方" };
            var unit2 = new Unit(src) { Party = "味方" };

            Assert.IsTrue(unit1.IsAlly(unit2));
        }

        [TestMethod]
        public void IsAlly_ReturnsTrue_BetweenAllyAndNPC()
        {
            var src = CreateSrc();
            var ally = new Unit(src) { Party = "味方" };
            var npc = new Unit(src) { Party = "ＮＰＣ" };

            Assert.IsTrue(ally.IsAlly(npc));
        }

        [TestMethod]
        public void IsAlly_ReturnsFalse_BetweenAllyAndEnemy()
        {
            var src = CreateSrc();
            var ally = new Unit(src) { Party = "味方" };
            var enemy = new Unit(src) { Party = "敵" };

            Assert.IsFalse(ally.IsAlly(enemy));
        }

        [TestMethod]
        public void IsAlly_ReturnsTrue_BetweenSamePartyEnemies()
        {
            var src = CreateSrc();
            var enemy1 = new Unit(src) { Party = "敵" };
            var enemy2 = new Unit(src) { Party = "敵" };

            Assert.IsTrue(enemy1.IsAlly(enemy2));
        }

        [TestMethod]
        public void IsAlly_ReturnsFalse_BetweenDifferentNonAllyParties()
        {
            var src = CreateSrc();
            var enemy = new Unit(src) { Party = "敵" };
            var neutral = new Unit(src) { Party = "中立" };

            Assert.IsFalse(enemy.IsAlly(neutral));
        }
    }
}
