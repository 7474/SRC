using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SRCCore.Coverage.Tests.Units
{
    /// <summary>
    /// Unit.lookup.cs の援護・かばう・連携攻撃の候補探索
    /// （LookForSupportAttack / LookForSupportGuard / LookForSupport / LookForAttackHelp / LookForGuardHelp）のテスト。
    /// 元の SRC の Unit.cls の同名関数に対応する。
    /// </summary>
    [TestClass]
    public class UnitLookupSupportTests
    {
        private const string Rifle = "ライフル, 1500, 1, 3, +0, -, -, -, AAAA, +0, 射";
        private const string Punch = "パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突";
        private const string Cannon = "キャノン, 2500, 1, 1, +0, -, -, -, AAAA, +0, 実";
        private const string MapBomb = "マップ爆弾, 3000, 1, 1, +0, -, -, -, AAAA, +0, Ｍ投";

        // 援護役「ベルク」「カイ」、本人「アレン」、敵「ゾルグ」を定義する
        private static UnitLookupTestHelper CreateWorld(string supporterSkills = "援護Lv1, 1",
            string selfSkills = null, string supporterWeapons = Rifle,
            string selfPersonality = "普通", string supporterPersonality = "普通")
        {
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン", skills: selfSkills, stats: $"150, 150, 150, 150, 150, 150, {selfPersonality}");
            h.DefinePilot("ベルク", skills: supporterSkills, stats: $"150, 150, 150, 150, 150, 150, {supporterPersonality}");
            h.DefinePilot("カイ", skills: supporterSkills);
            h.DefinePilot("ゾルグ");
            h.DefineUnit("自機", weapons: Punch);
            h.DefineUnit("僚機", weapons: supporterWeapons, stats: "50000, 200, 1000, 100");
            h.DefineUnit("重装機", weapons: supporterWeapons, stats: "80000, 200, 1000, 100");
            h.DefineUnit("敵機", weapons: Rifle + "\n" + MapBomb);
            h.Src.SystemConfig.AutoDefense = false;
            return h;
        }

        // ===== LookForSupportAttack =====

        [TestMethod]
        public void LookForSupportAttack_WithoutTarget_ReturnsAdjacentAllyWithSupportSkill()
        {
            // ターゲット未定の場合は援護攻撃回数が残っている隣接味方を返す
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            var ally = h.Place("僚機", "ベルク", 4, 3);

            Assert.AreSame(ally, me.LookForSupportAttack(null));

            // 援護攻撃回数を使い切ると対象外
            ally.UsedSupportAttack = 1;
            Assert.IsNull(me.LookForSupportAttack(null));
        }

        [TestMethod]
        public void LookForSupportAttack_ExcludesUnitsWithoutSkillOrAction()
        {
            var h = CreateWorld(supporterSkills: null);
            var me = h.Place("自機", "アレン", 3, 3);
            h.Place("僚機", "ベルク", 4, 3);
            // 援護技能がなければ援護攻撃しない
            Assert.IsNull(me.LookForSupportAttack(null));

            var h2 = CreateWorld();
            var me2 = h2.Place("自機", "アレン", 3, 3);
            var ally2 = h2.Place("僚機", "ベルク", 4, 3);
            // 行動済みなら援護攻撃しない
            ally2.UsedAction = ally2.MaxAction();
            Assert.IsNull(me2.LookForSupportAttack(null));
        }

        [TestMethod]
        public void LookForSupportAttack_ExcludesOtherPartyAndOtherTeam()
        {
            // 陣営が異なる、またはチームが異なるユニットは援護攻撃しない
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            h.Place("僚機", "ベルク", 4, 3, party: "敵");
            Assert.IsNull(me.LookForSupportAttack(null));

            var h2 = CreateWorld(supporterSkills: "援護Lv1, 1\nチーム=赤, 1", selfSkills: "チーム=青, 1");
            var me2 = h2.Place("自機", "アレン", 3, 3);
            h2.Place("僚機", "ベルク", 4, 3);
            Assert.IsNull(me2.LookForSupportAttack(null));

            // 援護側にチーム指定がなければチームを問わない
            var h3 = CreateWorld(selfSkills: "チーム=青, 1");
            var me3 = h3.Place("自機", "アレン", 3, 3);
            var ally3 = h3.Place("僚機", "ベルク", 4, 3);
            Assert.AreSame(ally3, me3.LookForSupportAttack(null));
        }

        [TestMethod]
        public void LookForSupportAttack_NpcSupportsAllyButNotEnemy()
        {
            // ＮＰＣは味方の援護攻撃は行うが、敵・中立の援護攻撃は行わない
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            var npc = h.Place("僚機", "ベルク", 4, 3, party: "ＮＰＣ");
            Assert.AreSame(npc, me.LookForSupportAttack(null));

            var h2 = CreateWorld();
            var enemy = h2.Place("自機", "アレン", 3, 3, party: "敵");
            h2.Place("僚機", "ベルク", 4, 3, party: "ＮＰＣ");
            Assert.IsNull(enemy.LookForSupportAttack(null));
        }

        [TestMethod]
        public void LookForSupportAttack_SameParty_Target_ReturnsNull()
        {
            // 同士討ちの場合はどちらに荷担すべきか分からないので援護しない
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            h.Place("僚機", "ベルク", 4, 3);
            var other = h.Place("自機", "ゾルグ", 2, 3);

            Assert.IsNull(me.LookForSupportAttack(other));
        }

        [TestMethod]
        public void LookForSupportAttack_ConfusedAttacker_ReturnsNull()
        {
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            h.Place("僚機", "ベルク", 4, 3);
            me.AddCondition("混乱", 2);

            Assert.IsNull(me.LookForSupportAttack(null));
        }

        [TestMethod]
        public void LookForSupportAttack_RequiresWeaponInRangeOfTarget()
        {
            // ターゲットが射程内にある武器を持つユニットのみ援護攻撃できる
            var h = CreateWorld(supporterWeapons: Punch);
            var me = h.Place("自機", "アレン", 3, 3);
            h.Place("僚機", "ベルク", 3, 4);
            var target = h.Place("敵機", "ゾルグ", 3, 2, party: "敵");

            // 僚機(3,4) からターゲット(3,2) は距離2で、射程1のパンチは届かない
            Assert.IsNull(me.LookForSupportAttack(target));

            var h2 = CreateWorld(supporterWeapons: Rifle);
            var me2 = h2.Place("自機", "アレン", 3, 3);
            var ally2 = h2.Place("僚機", "ベルク", 3, 4);
            var target2 = h2.Place("敵機", "ゾルグ", 3, 2, party: "敵");
            Assert.AreSame(ally2, me2.LookForSupportAttack(target2));
        }

        [TestMethod]
        public void LookForSupportAttack_PrefersStrongestWeapon_AndSkipsMapWeapons()
        {
            // 複数の候補がいる場合は最も威力の高い武器で攻撃できるユニットを選ぶ。
            // マップ兵器(Ｍ)は援護攻撃に使えない
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン");
            h.DefinePilot("ベルク", skills: "援護Lv1, 1");
            h.DefinePilot("カイ", skills: "援護Lv1, 1");
            h.DefinePilot("ゾルグ");
            h.DefineUnit("自機", weapons: Punch);
            h.DefineUnit("弱機", weapons: "ショット, 1000, 1, 2, +0, -, -, -, AAAA, +0, 射");
            h.DefineUnit("強機", weapons: "ショット, 1000, 1, 2, +0, -, -, -, AAAA, +0, 射\n強キャノン, 2500, 1, 2, +0, -, -, -, AAAA, +0, 実");
            h.DefineUnit("Ｍ機", weapons: "マップ砲, 4000, 1, 2, +0, -, -, -, AAAA, +0, Ｍ直");
            h.DefineUnit("敵機", weapons: Punch);
            var me = h.Place("自機", "アレン", 3, 3);
            var target = h.Place("敵機", "ゾルグ", 3, 2, party: "敵");
            // 援護役はターゲットから距離2の位置（射程2の武器なら届く）
            h.Place("弱機", "ベルク", 2, 3);
            var strong = h.Place("強機", "カイ", 4, 3);

            Assert.AreSame(strong, me.LookForSupportAttack(target));

            var h2 = new UnitLookupTestHelper();
            h2.DefinePilot("アレン");
            h2.DefinePilot("ベルク", skills: "援護Lv1, 1");
            h2.DefinePilot("ゾルグ");
            h2.DefineUnit("自機", weapons: Punch);
            h2.DefineUnit("Ｍ機", weapons: "マップ砲, 4000, 1, 2, +0, -, -, -, AAAA, +0, Ｍ直");
            h2.DefineUnit("敵機", weapons: Punch);
            var me2 = h2.Place("自機", "アレン", 3, 3);
            var target2 = h2.Place("敵機", "ゾルグ", 3, 2, party: "敵");
            h2.Place("Ｍ機", "ベルク", 2, 3);
            Assert.IsNull(me2.LookForSupportAttack(target2));
        }

        // ===== LookForSupportGuard =====

        [TestMethod]
        public void LookForSupportGuard_ReturnsAdjacentGuardian()
        {
            // 手動反撃の味方は常にかばってもらう。援護防御回数が残っている隣接味方を返す
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            var ally = h.Place("僚機", "ベルク", 4, 3);
            var attacker = h.Place("敵機", "ゾルグ", 2, 3, party: "敵");
            var tw = attacker.Weapons.First(w => w.Name == "ライフル");

            Assert.AreSame(ally, me.LookForSupportGuard(attacker, tw));

            // 援護防御回数を使い切ると対象外
            ally.UsedSupportGuard = 1;
            Assert.IsNull(me.LookForSupportGuard(attacker, tw));
        }

        [TestMethod]
        public void LookForSupportGuard_MapWeapon_ReturnsNull()
        {
            // マップ攻撃はサポートガード不能
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            h.Place("僚機", "ベルク", 4, 3);
            var attacker = h.Place("敵機", "ゾルグ", 2, 3, party: "敵");
            var tw = attacker.Weapons.First(w => w.Name == "マップ爆弾");

            Assert.IsNull(me.LookForSupportGuard(attacker, tw));
        }

        [TestMethod]
        public void LookForSupportGuard_PrefersHigherHp()
        {
            // 候補が複数いる場合はＨＰが高い方を優先
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            h.Place("僚機", "ベルク", 4, 3);
            var heavy = h.Place("重装機", "カイ", 3, 4);
            var attacker = h.Place("敵機", "ゾルグ", 2, 3, party: "敵");
            var tw = attacker.Weapons.First(w => w.Name == "ライフル");

            Assert.AreSame(heavy, me.LookForSupportGuard(attacker, tw));
        }

        [TestMethod]
        public void LookForSupportGuard_DoesNotGuardWhenItWouldBeDestroyed()
        {
            // 自分が倒されてしまうような場合はかばわない
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            var ally = h.Place("僚機", "ベルク", 4, 3);
            var attacker = h.Place("敵機", "ゾルグ", 2, 3, party: "敵");
            var tw = attacker.Weapons.First(w => w.Name == "ライフル");
            ally.HP = 1;

            Assert.IsNull(me.LookForSupportGuard(attacker, tw));
        }

        [TestMethod]
        public void LookForSupportGuard_MachineIsGuardedOnlyByMachine()
        {
            // 機械をかばうのは機械のみ
            var h = CreateWorld(selfPersonality: "機械");
            var me = h.Place("自機", "アレン", 3, 3);
            h.Place("僚機", "ベルク", 4, 3);
            var attacker = h.Place("敵機", "ゾルグ", 2, 3, party: "敵");
            var tw = attacker.Weapons.First(w => w.Name == "ライフル");
            Assert.IsNull(me.LookForSupportGuard(attacker, tw));

            var h2 = CreateWorld(selfPersonality: "機械", supporterPersonality: "機械");
            var me2 = h2.Place("自機", "アレン", 3, 3);
            var ally2 = h2.Place("僚機", "ベルク", 4, 3);
            var attacker2 = h2.Place("敵機", "ゾルグ", 2, 3, party: "敵");
            var tw2 = attacker2.Weapons.First(w => w.Name == "ライフル");
            Assert.AreSame(ally2, me2.LookForSupportGuard(attacker2, tw2));
        }

        [TestMethod]
        public void LookForSupportGuard_EnemyPartyIsNotGuardedByPlayer()
        {
            // 味方ユニットは敵をかばわない
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3, party: "敵");
            h.Place("僚機", "ベルク", 4, 3);
            var attacker = h.Place("敵機", "ゾルグ", 2, 3, party: "味方");
            var tw = attacker.Weapons.First(w => w.Name == "ライフル");
            // 味方以外はダメージが小さいとかばわないため、ＨＰを減らしておく
            me.HP = 50;

            Assert.IsNull(me.LookForSupportGuard(attacker, tw));

            // 同じ陣営（敵）のユニットならかばう
            var h2 = CreateWorld();
            var me2 = h2.Place("自機", "アレン", 3, 3, party: "敵");
            var guard2 = h2.Place("僚機", "ベルク", 4, 3, party: "敵");
            var attacker2 = h2.Place("敵機", "ゾルグ", 2, 3, party: "味方");
            var tw2 = attacker2.Weapons.First(w => w.Name == "ライフル");
            me2.HP = 50;
            Assert.AreSame(guard2, me2.LookForSupportGuard(attacker2, tw2));
        }

        [TestMethod]
        public void LookForSupportGuard_SkipsGuardianThatCannotEnterArea()
        {
            // 援護相手のいる地形（空中）に進入できないユニットはかばえない
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3, area: "空中");
            h.Place("僚機", "ベルク", 4, 3);
            var attacker = h.Place("敵機", "ゾルグ", 2, 3, party: "敵");
            var tw = attacker.Weapons.First(w => w.Name == "ライフル");

            Assert.IsNull(me.LookForSupportGuard(attacker, tw));
        }

        // ===== LookForSupport =====

        [TestMethod]
        public void LookForSupport_CountsSupportersAroundPoint()
        {
            // 自分がサポート可能なら隣接味方1体につき+1、相手が援護持ちならさらに+1
            var h = CreateWorld(selfSkills: "援護Lv1, 1");
            var me = h.Create("自機", "アレン");
            var ally = h.Place("僚機", "ベルク", 4, 3);
            // 移動のみの判定では、まだ行動できる相手は数えない
            Assert.AreEqual(0, me.LookForSupport(3, 3));

            ally.UsedAction = ally.MaxAction();
            Assert.AreEqual(2, me.LookForSupport(3, 3));
        }

        [TestMethod]
        public void LookForSupport_ForAttack_AddsSupportAttackBonus()
        {
            // 攻撃前の判定で相手が行動可能なら援護攻撃分+1、統率持ちならさらに+1
            var h = CreateWorld(selfSkills: "統率Lv1, 1");
            var me = h.Create("自機", "アレン");
            h.Place("僚機", "ベルク", 4, 3);

            Assert.AreEqual(3, me.LookForSupport(3, 3, true));

            var h2 = CreateWorld();
            var me2 = h2.Create("自機", "アレン");
            h2.Place("僚機", "ベルク", 4, 3);
            Assert.AreEqual(2, me2.LookForSupport(3, 3, true));
        }

        [TestMethod]
        public void LookForSupport_IgnoresEnemies()
        {
            var h = CreateWorld(selfSkills: "援護Lv1, 1");
            var me = h.Create("自機", "アレン");
            var enemy = h.Place("僚機", "ベルク", 4, 3, party: "敵");
            enemy.UsedAction = enemy.MaxAction();

            Assert.AreEqual(0, me.LookForSupport(3, 3, true));
        }

        [TestMethod]
        public void LookForSupport_GuardOnlySkillCountsOnce()
        {
            // 援護防御・指揮のみの相手は+1だけ
            var h = CreateWorld(supporterSkills: "援護防御Lv1, 1");
            var me = h.Create("自機", "アレン");
            h.Place("僚機", "ベルク", 4, 3);

            Assert.AreEqual(1, me.LookForSupport(3, 3, true));
        }

        // ===== LookForAttackHelp / LookForGuardHelp =====

        [TestMethod]
        public void LookForAttackHelp_RequiresRelationAndMessage()
        {
            // 連携攻撃は信頼度が足りており、攻撃側に「連携攻撃(相手)」メッセージがある場合のみ
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            var ally = h.Place("僚機", "ベルク", 4, 3);
            h.Place("敵機", "ゾルグ", 3, 2, party: "敵");

            // メッセージ未定義
            h.SetRelation(ally.MainPilot(), me.MainPilot(), 10);
            Assert.IsNull(me.LookForAttackHelp(3, 3));

            h.LoadMessages("アレン\n連携攻撃(ベルク), いくぞ！\n");
            Assert.AreSame(ally, me.LookForAttackHelp(3, 3));

            // 信頼度 0 では Dice(10) を必ず上回るので選ばれない
            h.SetRelation(ally.MainPilot(), me.MainPilot(), 0);
            Assert.IsNull(me.LookForAttackHelp(3, 3));
        }

        [TestMethod]
        public void LookForGuardHelp_RequiresRelationMessageAndSurvival()
        {
            // かばうは信頼度とかばう側の「かばう(相手)」メッセージが必要で、倒される場合はかばわない
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            var ally = h.Place("僚機", "ベルク", 4, 3);
            var attacker = h.Place("敵機", "ゾルグ", 2, 3, party: "敵");
            var tw = attacker.Weapons.First(w => w.Name == "ライフル");
            h.SetRelation(ally.MainPilot(), me.MainPilot(), 10);

            Assert.IsNull(me.LookForGuardHelp(attacker, tw, false));

            h.LoadMessages("ベルク\nかばう(アレン), 危ない！\n");
            Assert.AreSame(ally, me.LookForGuardHelp(attacker, tw, false));

            ally.HP = 1;
            Assert.IsNull(me.LookForGuardHelp(attacker, tw, true));
        }
    }
}
