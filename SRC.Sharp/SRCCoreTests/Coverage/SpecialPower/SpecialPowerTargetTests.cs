using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Coverage.Tests.SpecialPower
{
    /// <summary>
    /// SpecialPowerData.Useful / CountTarget / Execute / PlayAnimation の対象選別。
    /// 元の SRC の SpecialPowerData.Useful / CountTarget / Execute に対応。
    /// </summary>
    [TestClass]
    public class SpecialPowerTargetTests
    {
        [TestMethod]
        public void Useful_Self_DependsOnOwnUnit()
        {
            // 対象「自分」は自ユニットに効果があるかで判定
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("根性", "自分", "即効", "ＨＰ回復Lv3");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            var p = u.MainPilot();

            Assert.IsFalse(sd.Useful(p));
            Assert.AreEqual(0, sd.CountTarget(p));
            u.HP = 10;
            Assert.IsTrue(sd.Useful(p));
            Assert.AreEqual(1, sd.CountTarget(p));
            Assert.IsTrue(p.IsSpecialPowerUseful("根性"));
        }

        [TestMethod]
        public void Ally_CountsOnlySortiedFriendlyUnitsWithEffect()
        {
            // 対象「味方」: 出撃中の味方 (ＮＰＣ含む) で効果のあるユニットだけを数える
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("信頼", "味方", "即効", "ＨＰ回復Lv3");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var ally = f.AddUnit("僚機", "僚友", "味方", 2, 1);
            var npc = f.AddUnit("友軍機", "友軍", "ＮＰＣ", 3, 1);
            var enemy = f.AddUnit("敵機", "敵", "敵", 4, 1);
            var stored = f.AddUnit("待機機", "待機", "味方", 5, 1);
            var p = me.MainPilot();

            Assert.IsFalse(sd.Useful(p), "全員ＨＰ満タン");

            enemy.HP = 10;
            stored.HP = 10;
            stored.Status = "待機";
            Assert.IsFalse(sd.Useful(p), "敵と未出撃ユニットは対象外");
            Assert.AreEqual(0, sd.CountTarget(p));

            ally.HP = 10;
            npc.HP = 10;
            Assert.IsTrue(sd.Useful(p));
            Assert.AreEqual(2, sd.CountTarget(p));
        }

        [TestMethod]
        public void Enemy_CountsOnlySortiedHostileUnits()
        {
            // 対象「敵」: 味方・ＮＰＣ以外の出撃ユニットを数える
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("脱力", "敵", "即効", "気力低下Lv1");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            f.AddUnit("友軍機", "友軍", "ＮＰＣ", 2, 1);
            f.AddUnit("敵機", "敵1", "敵", 3, 1);
            var enemy2 = f.AddUnit("敵機", "敵2", "中立", 4, 1);
            var p = me.MainPilot();

            Assert.IsTrue(sd.Useful(p));
            Assert.AreEqual(2, sd.CountTarget(p));

            enemy2.MainPilot().Morale = enemy2.MainPilot().MinMorale;
            Assert.AreEqual(1, sd.CountTarget(p));
        }

        [TestMethod]
        public void Enemy_FromEnemyPilot_TargetsOtherParties()
        {
            // 敵側が使う場合は自陣営と異なる陣営が対象
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("脱力", "敵", "即効", "気力低下Lv1");
            f.AddUnit("自機", "自分", "味方", 1, 1);
            var e1 = f.AddUnit("敵機", "敵1", "敵", 3, 1);
            f.AddUnit("敵機", "敵2", "敵", 4, 1);

            Assert.AreEqual(1, sd.CountTarget(e1.MainPilot()));
        }

        [TestMethod]
        public void DestroyedAlly_CountsDestroyedUnitsOfSameParty()
        {
            // 対象「破壊味方」: 破壊された同陣営のユニットが対象
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("祈り", "破壊味方", "即効", "復活");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var p = me.MainPilot();
            Assert.IsFalse(sd.Useful(p));

            var dead = f.AddUnit("僚機", "僚友", "味方", 2, 1);
            dead.Status = "破壊";
            var deadEnemy = f.AddUnit("敵機", "敵", "敵", 3, 1);
            deadEnemy.Status = "破壊";

            Assert.IsTrue(sd.Useful(p));
            Assert.AreEqual(1, sd.CountTarget(p));
        }

        [TestMethod]
        public void Any_CountsAllSortiedUnitsWithEffect()
        {
            // 対象「全」「任意」: 陣営を問わず出撃中で効果のあるユニット
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("大地震", "全", "即効", "ＨＰ減少Lv1");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            f.AddUnit("敵機", "敵", "敵", 3, 1);
            var inv = f.AddUnit("敵機", "敵2", "敵", 4, 1);
            inv.AddCondition("無敵", 1);

            Assert.IsTrue(sd.Useful(me.MainPilot()));
            Assert.AreEqual(2, sd.CountTarget(me.MainPilot()));
        }

        // ──────────────────────────────────────────────
        // Execute
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Execute_AllAllies_AppliesToFriendlyUnitsOnMap()
        {
            // 対象「全味方」: マップ上の味方・ＮＰＣユニット全てに適用
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("愛", "全味方", "即効", "ＨＰ回復Lv10");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var npc = f.AddUnit("友軍機", "友軍", "ＮＰＣ", 2, 2);
            var enemy = f.AddUnit("敵機", "敵", "敵", 3, 3);
            me.HP = 10;
            npc.HP = 10;
            enemy.HP = 10;

            sd.Execute(me.MainPilot(), is_event: true);

            Assert.AreEqual(1000, me.HP);
            Assert.AreEqual(1000, npc.HP);
            Assert.AreEqual(10, enemy.HP);
        }

        [TestMethod]
        public void Execute_AllEnemies_AppliesToHostileUnitsOnMap()
        {
            // 対象「全敵」: マップ上の敵陣営ユニット全てに適用
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("戦慄", "全敵", "即効", "気力低下Lv1");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var e1 = f.AddUnit("敵機", "敵1", "敵", 3, 3);
            var e2 = f.AddUnit("敵機", "敵2", "敵", 4, 4);

            sd.Execute(me.MainPilot(), is_event: true);

            Assert.AreEqual(100, me.MainPilot().Morale);
            Assert.AreEqual(90, e1.MainPilot().Morale);
            Assert.AreEqual(90, e2.MainPilot().Morale);
        }

        [TestMethod]
        public void Execute_All_AppliesToEveryUnitOnMap()
        {
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("大地震", "全", "即効", "ＨＰ減少Lv5");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var e1 = f.AddUnit("敵機", "敵1", "敵", 3, 3);

            sd.Execute(me.MainPilot(), is_event: true);

            Assert.AreEqual(500, me.HP);
            Assert.AreEqual(500, e1.HP);
        }

        [TestMethod]
        public void Execute_SingleTarget_UsesSelectedTarget()
        {
            // 対象「味方」等の単体対象は SelectedTarget に適用する
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("信頼", "味方", "即効", "ＨＰ回復Lv3");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var ally = f.AddUnit("僚機", "僚友", "味方", 2, 1);
            var other = f.AddUnit("僚機", "僚友2", "味方", 3, 1);
            ally.HP = 100;
            other.HP = 100;
            f.Src.Commands.SelectedTarget = ally;

            sd.Execute(me.MainPilot(), is_event: true);

            Assert.AreEqual(400, ally.HP);
            Assert.AreEqual(100, other.HP);
        }

        [TestMethod]
        public void PlayAnimation_DisabledOrHyphen()
        {
            // アニメ表示が無効なら false (メッセージ表示に切り替え)、アニメ指定が「-」なら表示なしで true
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("熱血", "自分", "攻撃", "ダメージ増加Lv1");

            f.Src.SystemConfig.SpecialPowerAnimation = false;
            Assert.IsFalse(sd.PlayAnimation());

            f.Src.SystemConfig.SpecialPowerAnimation = true;
            sd.Animation = "-";
            Assert.IsTrue(sd.PlayAnimation());
        }
    }
}
