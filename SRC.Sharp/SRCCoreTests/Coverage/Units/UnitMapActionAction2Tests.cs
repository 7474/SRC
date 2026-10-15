using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Pilots;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests.Units
{
    /// <summary>
    /// Unit.action2.cs（GetExp / ChangeParty / IncreaseMorale / Die / SuicidalExplosion）のテスト。
    /// 元の SRC の Unit.cls の同名メソッドを仕様とする。
    /// </summary>
    [TestClass]
    public class UnitMapActionAction2Tests
    {
        // ──────────────────────────────────────────────
        // GetExp
        // ──────────────────────────────────────────────

        // 味方ユニット（パイロットのレベル plevel）と敵ユニット（パイロットのレベル tlevel）を用意する
        private static (UnitMapActionFixture f, Unit u, Pilot p, Unit t) CreateExpSet(
            int plevel = 10, int tlevel = 10, int unitExp = 0, int pilotExp = 0)
        {
            var f = new UnitMapActionFixture();
            f.AddUnitData("味方機");
            f.AddUnitData("敵機").ExpValue = unitExp;
            f.AddPilotData("敵パイロット", expValue: pilotExp);
            var p = f.CreatePilot("パイロット", plevel);
            var u = f.CreateUnit("味方機", "味方", p);
            var t = f.CreateUnit("敵機", "敵", f.CreatePilot("敵パイロット", tlevel, "敵"));
            return (f, u, p, t);
        }

        [TestMethod]
        public void GetExp_EnemyUnit_GetsNothing()
        {
            // 経験値を得るのは味方ユニットとＮＰＣの召喚ユニットだけ
            var (f, u, p, t) = CreateExpSet();
            var ep = t.MainPilot();

            Assert.AreEqual(0, t.GetExp(u, "修理"));
            Assert.AreEqual(0, ep.Exp);
            Assert.AreEqual(0, f.SysMessages.Count);
        }

        [TestMethod]
        public void GetExp_TargetWithoutPilot_GetsNothing()
        {
            // ターゲットにパイロットが乗っていなければ経験値は得られない
            var (f, u, p, _) = CreateExpSet();
            var empty = f.CreateUnit("敵機", "敵");

            Assert.AreEqual(0, u.GetExp(empty, "修理"));
            Assert.AreEqual(0, p.Exp);
        }

        [TestMethod]
        public void GetExp_Repair_SameLevel_GivesAllPilotsAndShowsMessage()
        {
            // 修理は100。同レベルなら補正なし。サポートパイロットも同じだけ得る
            var (f, u, p, t) = CreateExpSet();
            var support = f.CreatePilot("サポート", 10);
            u.AddSupport(support);
            support.Unit = u;

            var xp = u.GetExp(t, "修理");

            Assert.AreEqual(100, xp);
            Assert.AreEqual(100, p.Exp);
            Assert.AreEqual(100, support.Exp);
            Assert.AreEqual("パイロットは100の経験値を得た。", f.SysMessages.Single());
        }

        [TestMethod]
        public void GetExp_MapMode_SkipsMessage()
        {
            // マップ攻撃による入手ではメッセージを表示しない
            var (f, u, p, t) = CreateExpSet();

            Assert.AreEqual(150, u.GetExp(t, "補給", "マップ"));
            Assert.AreEqual(150, p.Exp);
            Assert.AreEqual(0, f.SysMessages.Count);
        }

        [DataTestMethod]
        [DataRow(10, 19, 500)]  // 相手が8以上高い: 5倍
        [DataRow(10, 17, 450)]  // 7: 4.5倍
        [DataRow(10, 16, 400)]
        [DataRow(10, 15, 350)]
        [DataRow(10, 14, 300)]
        [DataRow(10, 13, 250)]
        [DataRow(10, 12, 200)]
        [DataRow(10, 11, 150)]
        [DataRow(10, 9, 50)]    // -1: 1/2
        [DataRow(10, 8, 25)]    // -2: 1/4
        [DataRow(10, 7, 16)]    // -3: 100 \ 6
        [DataRow(10, 6, 12)]    // -4: 100 \ 8
        [DataRow(10, 5, 10)]    // -5: 1/10
        [DataRow(10, 1, 8)]     // -6以下: 100 \ 12
        public void GetExp_LevelDifferenceModifier(int plevel, int tlevel, int expected)
        {
            // 相手のメインパイロットとのレベル差で経験値が増減する
            var (_, u, _, t) = CreateExpSet(plevel, tlevel);

            Assert.AreEqual(expected, u.GetExp(t, "修理", "マップ"));
        }

        [TestMethod]
        public void GetExp_DestroyAndAttack_UseTargetExpValues()
        {
            // 破壊はユニットとパイロットの経験値の合計、攻撃はその 1/10
            var (_, u, p, t) = CreateExpSet(unitExp: 120, pilotExp: 30);

            Assert.AreEqual(150, u.GetExp(t, "破壊", "マップ"));
            Assert.AreEqual(15, u.GetExp(t, "攻撃", "マップ"));
            Assert.AreEqual(165, p.Exp);
        }

        [TestMethod]
        public void GetExp_Ability_SelfTargetGivesHalf()
        {
            // アビリティは自分が対象なら50、他ユニットなら100（ターゲット省略時は自分）
            var (_, u, _, t) = CreateExpSet();

            Assert.AreEqual(50, u.GetExp(null, "アビリティ", "マップ"));
            Assert.AreEqual(100, u.GetExp(t, "アビリティ", "マップ"));
        }

        [TestMethod]
        public void GetExp_TalentAndSlowGrowthSkills()
        {
            // 素質（レベル指定なし）は1.5倍、素質Lv5 は (10+5)/10 倍、遅成長は半分
            var f = new UnitMapActionFixture();
            f.AddUnitData("味方機");
            f.AddPilotData("素質持ち").AddSkill("素質", Constants.DEFAULT_LEVEL, "", 1);
            f.AddPilotData("素質Lv持ち").AddSkill("素質", 5d, "", 1);
            f.AddPilotData("晩成").AddSkill("遅成長", Constants.DEFAULT_LEVEL, "", 1);
            var t = f.CreateUnit("味方機", "味方", f.CreatePilot("相手", 10));
            var u1 = f.CreateUnit("味方機", "味方", f.CreatePilot("素質持ち", 10));
            var u2 = f.CreateUnit("味方機", "味方", f.CreatePilot("素質Lv持ち", 10));
            var u3 = f.CreateUnit("味方機", "味方", f.CreatePilot("晩成", 10));

            Assert.AreEqual(150, u1.GetExp(t, "修理", "マップ"));
            Assert.AreEqual(150, u2.GetExp(t, "修理", "マップ"));
            Assert.AreEqual(50, u3.GetExp(t, "修理", "マップ"));
        }

        [TestMethod]
        [Ignore("移植の不具合: 経験値の小数倍（1.5倍等）を切り捨てている。元の SRC は Long への代入で丸める")]
        public void GetExp_FractionalMultiplier_RoundsLikeVbLongAssignment()
        {
            // 元の SRC では xp は Long で、xp = 1.5 * xp は銀行丸めで整数化される（7.5 → 8）
            var (_, u, _, t) = CreateExpSet(10, 11, unitExp: 50);

            // 攻撃: 50 \ 10 = 5、レベル差+1で 1.5 * 5 = 7.5 → 8
            Assert.AreEqual(8, u.GetExp(t, "攻撃", "マップ"));
        }

        [TestMethod]
        public void GetExp_LevelUp_ShowsLevelAndLearnedSkill()
        {
            // レベルアップ時は新しいレベルと習得した特殊能力をメッセージで知らせる
            var f = new UnitMapActionFixture();
            f.AddUnitData("味方機");
            f.AddPilotData("新人").AddSkill("底力", 2d, "", 2);
            var p = f.CreatePilot("新人", 1);
            var u = f.CreateUnit("味方機", "味方", p);
            var t = f.CreateUnit("味方機", "味方", f.CreatePilot("相手", 1));
            p.Exp = 450;

            var xp = u.GetExp(t, "修理");

            Assert.AreEqual(100, xp);
            Assert.AreEqual(2, p.Level);
            Assert.AreEqual(50, p.Exp);
            Assert.AreEqual("新人は経験値[100]を獲得、レベル[2]にレベルアップ。;底力Lv2を習得した。", f.SysMessages.Single());
        }

        [TestMethod]
        public void GetExp_NpcSummonedUnit_SummonerAlsoGetsExp()
        {
            // ＮＰＣの召喚ユニットも経験値を得る。召喚主もパートナーとして経験値を得る
            var f = new UnitMapActionFixture();
            f.AddUnitData("召喚主機");
            f.AddUnitData("召喚獣", features: "召喚ユニット");
            var master = f.CreatePilot("召喚主", 10);
            var summoner = f.CreateUnit("召喚主機", "味方", master);
            var beastPilot = f.CreatePilot("召喚獣パイロット", 10, "ＮＰＣ");
            var beast = f.CreateUnit("召喚獣", "ＮＰＣ", beastPilot);
            beast.Summoner = summoner;
            var t = f.CreateUnit("召喚主機", "味方", f.CreatePilot("相手", 10));

            Assert.AreEqual(100, beast.GetExp(t, "修理", "マップ"));
            Assert.AreEqual(100, beastPilot.Exp);
            Assert.AreEqual(100, master.Exp);
        }

        // ──────────────────────────────────────────────
        // ChangeParty
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ChangeParty_Deployed_OnOwnStage_RestsAndRepaints()
        {
            // 出撃中に陣営が変わると画像を描き直し、新しい陣営のステージ中なら行動可能（Rest）にする。
            // 他形態とパイロットの陣営も変わる（Unit.cls ChangeParty）
            var f = new UnitMapActionFixture();
            f.Src.Stage = "敵";
            f.Src.Turn = 2;
            f.AddUnitData("機体", features: "変形=変形 別形態");
            f.AddUnitData("別形態", features: "変形=変形 機体");
            var p = f.CreatePilot("パイロット");
            var support = f.CreatePilot("サポート");
            var u = f.Deploy(f.CreateUnit("機体", "味方", p), 3, 3);
            u.AddSupport(support);
            support.Unit = u;
            u.UsedAction = 1;
            u.Mode = "固定";

            u.ChangeParty("敵");

            Assert.AreEqual("敵", u.Party0);
            Assert.AreEqual("敵", u.OtherForm("別形態").Party0);
            Assert.AreEqual("敵", p.Party);
            Assert.AreEqual("敵", support.Party);
            Assert.AreEqual("通常", u.Mode);
            // Rest により再び行動可能になる
            Assert.AreEqual(0, u.UsedAction);
            Assert.AreSame(u, f.Painted.Single().u);
        }

        [TestMethod]
        public void ChangeParty_NotOwnStage_DoesNotRest()
        {
            // 新しい陣営のステージでなければ Rest しないので行動済みのまま
            var f = new UnitMapActionFixture();
            f.Src.Stage = "味方";
            f.Src.Turn = 2;
            f.AddUnitData("機体");
            var u = f.Deploy(f.CreateUnit("機体", "味方", f.CreatePilot("パイロット")), 3, 3);
            u.UsedAction = 1;

            u.ChangeParty("敵");

            Assert.AreEqual("敵", u.Party0);
            Assert.AreEqual(1, u.UsedAction);
            Assert.AreEqual(1, f.Painted.Count);
        }

        // ──────────────────────────────────────────────
        // IncreaseMorale
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IncreaseMorale_MachinePersonalityIgnoredUnlessEvent()
        {
            // 機械の性格のパイロットは気力が変化しない。イベントによる増減なら変化する
            var f = new UnitMapActionFixture();
            f.AddUnitData("複座機").PilotNum = 2;
            f.AddPilotData("ロボ", personality: "機械");
            var main = f.CreatePilot("ロボ");
            var sub = f.CreatePilot("副操縦士");
            var support = f.CreatePilot("サポート");
            var u = f.CreateUnit("複座機", "味方", main, sub);
            u.AddSupport(support);
            support.Unit = u;
            main.Morale = 100;
            sub.Morale = 100;
            support.Morale = 100;

            u.IncreaseMorale(10);
            Assert.AreEqual(100, main.Morale);
            Assert.AreEqual(110, sub.Morale);
            Assert.AreEqual(110, support.Morale);

            u.IncreaseMorale(-5, is_event: true);
            Assert.AreEqual(95, main.Morale);
            Assert.AreEqual(105, sub.Morale);
            Assert.AreEqual(105, support.Morale);
        }

        // ──────────────────────────────────────────────
        // Die
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Die_RemovesFromMapAndPlaysDieAnimation()
        {
            // 破壊されると HP0・「破壊」になり、マップから消えて爆発表示を行う
            var f = new UnitMapActionFixture();
            f.AddUnitData("機体");
            var u = f.Deploy(f.CreateUnit("機体", "敵", f.CreatePilot("敵", 1, "敵")), 4, 4);

            u.Die();

            Assert.AreEqual(0, u.HP);
            Assert.AreEqual("破壊", u.Status);
            Assert.IsNull(f.Map.MapDataForUnit[4, 4]);
            Assert.AreSame(u, f.DieAnimations.Single());
        }

        [TestMethod]
        public void Die_WithDestroyCancel_SkipsAnimationAndKeepsMapEntry()
        {
            // 破壊キャンセル状態なら爆発表示とマップからの除去を行わず、状態を解除する
            var f = new UnitMapActionFixture();
            f.AddUnitData("機体");
            var u = f.Deploy(f.CreateUnit("機体", "敵", f.CreatePilot("敵", 1, "敵")), 4, 4);
            u.AddCondition("破壊キャンセル", 1, cdata: "");

            u.Die();

            Assert.AreEqual("破壊", u.Status);
            Assert.AreSame(u, f.Map.MapDataForUnit[4, 4]);
            Assert.AreEqual(0, f.DieAnimations.Count);
            Assert.IsFalse(u.IsConditionSatisfied("破壊キャンセル"));
        }

        [TestMethod]
        public void Die_UnlinksFromSummonerAndMaster()
        {
            // 召喚ユニット・隷属ユニットが破壊されると召喚主・支配者のリストから外れる
            var f = new UnitMapActionFixture();
            f.AddUnitData("主機");
            f.AddUnitData("召喚機");
            f.AddUnitData("隷属機");
            var master = f.Deploy(f.CreateUnit("主機", "味方", f.CreatePilot("主")), 1, 1);
            var servant = f.Deploy(f.CreateUnit("召喚機", "味方", f.CreatePilot("召喚")), 2, 2);
            var slave = f.Deploy(f.CreateUnit("隷属機", "敵", f.CreatePilot("隷属", 1, "敵")), 3, 3);
            master.AddServant(servant);
            servant.Summoner = master;
            master.AddSlave(slave);
            slave.Master = master;

            servant.Die();
            slave.Die();

            Assert.AreEqual(0, master.CountServant());
            Assert.AreEqual(0, master.CountSlave());
            Assert.IsNull(servant.Summoner);
            // PreviousSummonerId は元の SRC にない C# 版独自の拡張（スペシャルパワー「復活」で召喚主へ再リンクするため）
            Assert.AreEqual(master.ID, servant.PreviousSummonerId);
            Assert.IsNull(slave.Master);
        }

        [TestMethod]
        public void Die_DominatorDestroyed_ForcesDominatedPilotsUnitsToDie()
        {
            // 「支配」で指定したパイロットのユニットは、支配者の破壊と同時に破壊される
            var f = new UnitMapActionFixture();
            f.AddUnitData("支配者機", features: "支配=支配 手下");
            f.AddUnitData("機体");
            var boss = f.Deploy(f.CreateUnit("支配者機", "敵", f.CreatePilot("ボス", 1, "敵")), 1, 1);
            var minion = f.Deploy(f.CreateUnit("機体", "敵", f.CreatePilot("手下", 1, "敵")), 3, 3);
            var other = f.Deploy(f.CreateUnit("機体", "敵", f.CreatePilot("他人", 1, "敵")), 5, 5);

            boss.Die();

            Assert.AreEqual("破壊", minion.Status);
            Assert.IsNull(f.Map.MapDataForUnit[3, 3]);
            Assert.AreEqual("出撃", other.Status);
        }

        // ──────────────────────────────────────────────
        // SuicidalExplosion
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SuicidalExplosion_DamagesAdjacentUnitsByOwnHp()
        {
            // 自爆は自分の残りHPぶんのダメージを隣接ユニットに与える。
            // 無敵はダメージなし、不死身はHP10で止まる、不安定はHP1/4以下で暴走、睡眠は覚める
            var f = new UnitMapActionFixture();
            f.AddUnitData("自爆機");
            f.AddUnitData("機体");
            f.AddUnitData("不安定機", features: "不安定");
            var bomber = f.Deploy(f.CreateUnit("自爆機", "味方", f.CreatePilot("自爆者")), 5, 5);
            bomber.HP = 800;
            var unstable = f.Deploy(f.CreateUnit("不安定機", "敵", f.CreatePilot("敵1", 1, "敵")), 4, 5);
            var immortal = f.Deploy(f.CreateUnit("機体", "敵", f.CreatePilot("敵2", 1, "敵")), 5, 4);
            immortal.HP = 500;
            immortal.AddCondition("不死身", 1, cdata: "");
            var invincible = f.Deploy(f.CreateUnit("機体", "敵", f.CreatePilot("敵3", 1, "敵")), 6, 5);
            invincible.AddCondition("無敵", 1, cdata: "");
            var sleeper = f.Deploy(f.CreateUnit("機体", "敵", f.CreatePilot("敵4", 1, "敵")), 5, 6);
            sleeper.AddCondition("睡眠", 2, cdata: "");
            var far = f.Deploy(f.CreateUnit("機体", "敵", f.CreatePilot("敵5", 1, "敵")), 7, 5);

            bomber.SuicidalExplosion(is_event: true);

            Assert.AreEqual(0, bomber.HP);
            Assert.AreEqual("破壊", bomber.Status);
            Assert.IsNull(f.Map.MapDataForUnit[5, 5]);
            Assert.AreEqual(("M", 5, 5), f.Explosions.Single());
            // 自爆したユニット自身は既に爆発を表示しているので破壊アニメは出さない
            Assert.IsFalse(f.DieAnimations.Contains(bomber));
            Assert.AreEqual("自爆機は自爆した。", f.SysMessages.First());

            Assert.AreEqual(200, unstable.HP);
            Assert.IsTrue(unstable.IsConditionSatisfied("暴走"));
            Assert.AreEqual(10, immortal.HP);
            Assert.AreEqual(1000, invincible.HP);
            Assert.AreEqual(200, sleeper.HP);
            Assert.IsFalse(sleeper.IsConditionSatisfied("睡眠"));
            Assert.AreEqual(1000, far.HP);
        }

        // 持続期間「破壊」で効果「復活」を持つスペシャルパワーを定義する
        private static void DefineRevive(UnitMapActionFixture f)
        {
            var sp = f.Src.SPDList.Add("復活SP");
            sp.Duration = "破壊";
            sp.SetEffect("復活");
        }

        [TestMethod]
        public void SuicidalExplosion_KillsWeakNeighbor_UnlessRevived()
        {
            // HP が0になった隣接ユニットは破壊される。
            // ただし「復活」の効果を受けていればHP全快で復活する（Unit.cls SuicidalExplosion）
            var f = new UnitMapActionFixture();
            DefineRevive(f);
            f.AddUnitData("機体");
            var bomber = f.Deploy(f.CreateUnit("機体", "味方", f.CreatePilot("自爆者")), 5, 5);
            var weak = f.Deploy(f.CreateUnit("機体", "敵", f.CreatePilot("敵1", 1, "敵")), 4, 5);
            weak.HP = 300;
            var revived = f.Deploy(f.CreateUnit("機体", "敵", f.CreatePilot("敵2", 1, "敵")), 6, 5);
            revived.HP = 300;
            revived.MakeSpecialPowerInEffect("復活SP");

            bomber.SuicidalExplosion(is_event: true);

            Assert.AreEqual("破壊", weak.Status);
            Assert.IsNull(f.Map.MapDataForUnit[4, 5]);
            Assert.AreSame(weak, f.DieAnimations.Single());

            Assert.AreEqual("出撃", revived.Status);
            Assert.AreEqual(revived.MaxHP, revived.HP);
            Assert.AreSame(revived, f.Map.MapDataForUnit[6, 5]);
            // 持続期間「破壊」のスペシャルパワーは使い切られる
            Assert.IsFalse(revived.IsSpecialPowerInEffect("復活SP"));
            CollectionAssert.Contains(f.SysMessages, "機体は復活した！");
        }

        [TestMethod]
        public void SuicidalExplosion_NotEvent_ReportsDestructionAndDamage()
        {
            // イベントによらない自爆では、隣接ユニットの破壊と受けたダメージ・損傷率をメッセージで知らせる
            var f = new UnitMapActionFixture();
            f.AddUnitData("自爆機");
            f.AddUnitData("弱い機体");
            f.AddUnitData("頑丈な機体");
            var bomber = f.Deploy(f.CreateUnit("自爆機", "味方", f.CreatePilot("自爆者")), 5, 5);
            bomber.HP = 200;
            var weak = f.Deploy(f.CreateUnit("弱い機体", "敵", f.CreatePilot("敵1", 1, "敵")), 4, 5);
            weak.HP = 100;
            var tough = f.Deploy(f.CreateUnit("頑丈な機体", "敵", f.CreatePilot("敵2", 1, "敵")), 6, 5);

            bomber.SuicidalExplosion(is_event: false);

            Assert.AreEqual("破壊", weak.Status);
            Assert.AreEqual(800, tough.HP);
            CollectionAssert.Contains(f.SysMessages, "弱い機体は破壊された");
            CollectionAssert.Contains(f.SysMessages, "頑丈な機体は200のダメージを受けた。;残りＨＰは800（損傷率 = 20％）");
        }
    }
}
