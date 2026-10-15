using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;
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

        // 援護攻撃の候補判定用に、射程2の武器を持つ援護役2体とターゲットを配置する。
        // 自機(3,3) の左右 (2,3)/(4,3) に援護役を置く。ターゲット(3,2) からは距離2
        private static (UnitLookupTestHelper h, Unit me, Unit target, Unit left, Unit right) PlaceTwoSupporters(
            string leftUnit, string rightUnit, string extraUnits)
        {
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン");
            h.DefinePilot("ベルク", skills: "援護Lv1, 1");
            h.DefinePilot("カイ", skills: "援護Lv1, 1");
            h.DefinePilot("ゾルグ");
            h.DefineUnit("自機", weapons: Punch);
            h.DefineUnit("敵機", weapons: Punch);
            foreach (var def in extraUnits.Split('|'))
            {
                var parts = def.Split('=');
                h.DefineUnit(parts[0], weapons: parts[1].Replace(";", "\n"));
            }
            var me = h.Place("自機", "アレン", 3, 3);
            var target = h.Place("敵機", "ゾルグ", 3, 2, party: "敵");
            var left = h.Place(leftUnit, "ベルク", 2, 3);
            var right = h.Place(rightUnit, "カイ", 4, 3);
            return (h, me, target, left, right);
        }

        private const string ShotDefs =
            "弱機=ショット, 1000, 1, 2, +0, -, -, -, AAAA, +0, 射"
            + "|強機=ショット, 1000, 1, 2, +0, -, -, -, AAAA, +0, 射;強キャノン, 2500, 1, 2, +0, -, -, -, AAAA, +0, 実"
            + "|中機=ミドルショット, 1500, 1, 2, +0, -, -, -, AAAA, +0, 射"
            + "|Ｍ機=ショット, 1000, 1, 2, +0, -, -, -, AAAA, +0, 射;マップ砲, 4000, 1, 2, +0, -, -, -, AAAA, +0, Ｍ直";

        [TestMethod]
        public void LookForSupportAttack_PrefersStrongestWeapon_RegardlessOfPosition()
        {
            // 複数の候補がいる場合は最も威力の高い武器で攻撃できるユニットを選ぶ。
            // 探索順に依存しないことを確かめるため、左右を入れ替えた両方の配置で判定する
            var a = PlaceTwoSupporters("弱機", "強機", ShotDefs);
            Assert.AreSame(a.right, a.me.LookForSupportAttack(a.target));
            Assert.AreEqual("強機", a.right.Name);

            var b = PlaceTwoSupporters("強機", "弱機", ShotDefs);
            Assert.AreSame(b.left, b.me.LookForSupportAttack(b.target));
            Assert.AreEqual("強機", b.left.Name);
        }

        [TestMethod]
        public void LookForSupportAttack_MapWeaponPowerIsNotConsidered()
        {
            // マップ兵器(Ｍ)は援護攻撃に使えない。Ｍ機のマップ砲(4000)は数えられず、
            // 通常武器(1000)の威力で比較されるので、1500 の武器を持つ中機が選ばれる
            var a = PlaceTwoSupporters("Ｍ機", "中機", ShotDefs);
            Assert.AreSame(a.right, a.me.LookForSupportAttack(a.target));

            var b = PlaceTwoSupporters("中機", "Ｍ機", ShotDefs);
            Assert.AreSame(b.left, b.me.LookForSupportAttack(b.target));

            // Ｍ機自体は除外されない（通常武器で援護攻撃できる）
            var c = PlaceTwoSupporters("Ｍ機", "Ｍ機", ShotDefs);
            c.right.UsedSupportAttack = 1;
            Assert.AreSame(c.left, c.me.LookForSupportAttack(c.target));
        }

        [TestMethod]
        public void LookForSupportAttack_AutoDefense_PlayerAvoidsScarceWeapons()
        {
            // 自動反撃時、味方ユニットは残弾数の少ない武器・ＥＮ消費の大きい武器・尽属性の武器を援護攻撃に使わない
            const string defs = "弾少機=バズーカ, 2500, 1, 2, +0, 1, -, -, AAAA, +0, 実"
                + "|弾三機=ランチャー, 2500, 1, 2, +0, 3, -, -, AAAA, +0, 実"
                + "|ＥＮ機=メガ砲, 2500, 1, 2, +0, -, 60, -, AAAA, +0, Ｂ"
                + "|尽機=全力砲, 2500, 1, 2, +0, -, -, -, AAAA, +0, 尽"
                + "|弱機=ショット, 1000, 1, 2, +0, -, -, -, AAAA, +0, 射";
            foreach (var name in new[] { "弾少機", "弾三機", "ＥＮ機", "尽機" })
            {
                // 手動反撃なら威力の高い方が選ばれる
                var a = PlaceTwoSupporters(name, "弱機", defs);
                a.h.Src.SystemConfig.AutoDefense = false;
                Assert.AreSame(a.left, a.me.LookForSupportAttack(a.target), name);

                // 自動反撃では使えない武器なので弱機が選ばれる
                a.h.Src.SystemConfig.AutoDefense = true;
                Assert.AreSame(a.right, a.me.LookForSupportAttack(a.target), name);
            }
        }

        [TestMethod]
        public void LookForSupportAttack_AutoDefense_NpcStillUsesScarceWeapons()
        {
            // 残弾等の制限は味方ユニットのみ。ＮＰＣ は自動反撃時でも使う
            const string defs = "弾少機=バズーカ, 2500, 1, 2, +0, 1, -, -, AAAA, +0, 実"
                + "|弱機=ショット, 1000, 1, 2, +0, -, -, -, AAAA, +0, 射";
            var a = PlaceTwoSupporters("弾少機", "弱機", defs);
            a.left.Party = "ＮＰＣ";
            a.h.Src.SystemConfig.AutoDefense = true;
            Assert.AreSame(a.left, a.me.LookForSupportAttack(a.target));
        }

        [TestMethod]
        [Ignore("移植の不具合: LookForSupportAttack の隣接ユニット探索順が VB(左,右,上,下) と異なり Map.AdjacentUnit(右,左,下,上) になっている")]
        public void LookForSupportAttack_WithoutTarget_PrefersLeftNeighbourLikeVb()
        {
            // ターゲット未定の場合は最初に見つかった候補を返す。VB は左→右→上→下の順に調べる
            // (VB Unit.cls LookForSupportAttack: Case 1 は MapDataForUnit(x - 1, y))
            var a = PlaceTwoSupporters("弱機", "弱機", ShotDefs);
            Assert.AreSame(a.left, a.me.LookForSupportAttack(null));
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

            // 同じ配置でも、空を飛べる（空陸）ユニットならかばえる
            var h2 = CreateWorld();
            h2.DefineUnit("飛行僚機", weapons: Rifle, trans: "空陸", stats: "50000, 200, 1000, 100");
            var me2 = h2.Place("自機", "アレン", 3, 3, area: "空中");
            var flyer = h2.Place("飛行僚機", "ベルク", 4, 3);
            var attacker2 = h2.Place("敵機", "ゾルグ", 2, 3, party: "敵");
            var tw2 = attacker2.Weapons.First(w => w.Name == "ライフル");
            Assert.AreSame(flyer, me2.LookForSupportGuard(attacker2, tw2));
        }

        [TestMethod]
        public void LookForSupportGuard_BossDoesNotGuardLowerRankUnlessDamageIsSmall()
        {
            // ボスランクが上のユニットは、被ダメージが大きければザコをかばわない
            // (VB: If .BossRank > BossRank Then If dmg >= .MaxHP \ 20 Or dmg >= .HP \ 5 ...)
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            var boss = h.Place("僚機", "ベルク", 4, 3);
            var attacker = h.Place("敵機", "ゾルグ", 2, 3, party: "敵");
            var tw = attacker.Weapons.First(w => w.Name == "ライフル");
            boss.BossRank = 1;
            // 最大ＨＰ 50000 なら 1/20 は 2500 でライフルのダメージはそれ未満、
            // ＨＰ の 1/5 を下回るようにしておけば「被るダメージが少ない」のでかばう
            Assert.AreSame(boss, me.LookForSupportGuard(attacker, tw));

            // ＨＰが減って HP/5 以上のダメージを受けるならかばわない
            boss.HP = 500;
            Assert.IsNull(me.LookForSupportGuard(attacker, tw));
        }

        [TestMethod]
        public void LookForSupportGuard_LowerRankGuardsBossEvenIfDestroyed()
        {
            // 倒されてしまう場合でも、かばう側のボスランクが守る相手より低ければかばう
            var h = CreateWorld();
            var me = h.Place("自機", "アレン", 3, 3);
            var ally = h.Place("僚機", "ベルク", 4, 3);
            var attacker = h.Place("敵機", "ゾルグ", 2, 3, party: "敵");
            var tw = attacker.Weapons.First(w => w.Name == "ライフル");
            ally.HP = 1;
            Assert.IsNull(me.LookForSupportGuard(attacker, tw));

            me.BossRank = 1;
            Assert.AreSame(ally, me.LookForSupportGuard(attacker, tw));
        }

        [TestMethod]
        public void LookForSupportGuard_AutoDefense_SkipsWhenOwnDamageIsSmall()
        {
            // 自動反撃時は味方でも、受けるダメージが MaxHP/20 未満かつ HP/5 未満ならかばってもらわない
            var h = CreateWorld();
            h.DefineUnit("巨体機", weapons: Punch, stats: "100000, 200, 1000, 100");
            var me = h.Place("巨体機", "アレン", 3, 3);
            var ally = h.Place("僚機", "ベルク", 4, 3);
            var attacker = h.Place("敵機", "ゾルグ", 2, 3, party: "敵");
            var tw = attacker.Weapons.First(w => w.Name == "ライフル");

            // 手動反撃なら常にかばう
            Assert.AreSame(ally, me.LookForSupportGuard(attacker, tw));

            h.Src.SystemConfig.AutoDefense = true;
            Assert.IsNull(me.LookForSupportGuard(attacker, tw));

            // ＨＰが減って HP/5 以上のダメージになればかばう
            me.HP = 100;
            Assert.AreSame(ally, me.LookForSupportGuard(attacker, tw));
        }

        [TestMethod]
        public void LookForSupportGuard_NpcOptionRestrictsPlayerGuardingNpc()
        {
            // 味方ユニットは通常ＮＰＣもかばうが、Option(対ＮＰＣサポートガード無効) では同じ陣営のみ
            var h = CreateWorld();
            var npc = h.Place("自機", "アレン", 3, 3, party: "ＮＰＣ");
            var ally = h.Place("僚機", "ベルク", 4, 3);
            var attacker = h.Place("敵機", "ゾルグ", 2, 3, party: "敵");
            var tw = attacker.Weapons.First(w => w.Name == "ライフル");
            // ＮＰＣは味方以外なのでダメージが小さいとかばわない。ＨＰを減らしておく
            npc.HP = 50;
            Assert.AreSame(ally, npc.LookForSupportGuard(attacker, tw));

            h.Src.Expression.DefineGlobalVariable("Option(対ＮＰＣサポートガード無効)");
            Assert.IsNull(npc.LookForSupportGuard(attacker, tw));
        }

        [TestMethod]
        public void LookForSupportGuard_DifferentTeamDoesNotGuard()
        {
            // かばう側にチーム指定があり、守る相手と異なればかばわない
            var h = CreateWorld(supporterSkills: "援護Lv1, 1\nチーム=赤, 1", selfSkills: "チーム=青, 1");
            var me = h.Place("自機", "アレン", 3, 3);
            h.Place("僚機", "ベルク", 4, 3);
            var attacker = h.Place("敵機", "ゾルグ", 2, 3, party: "敵");
            var tw = attacker.Weapons.First(w => w.Name == "ライフル");
            Assert.IsNull(me.LookForSupportGuard(attacker, tw));

            var h2 = CreateWorld(supporterSkills: "援護Lv1, 1\nチーム=青, 1", selfSkills: "チーム=青, 1");
            var me2 = h2.Place("自機", "アレン", 3, 3);
            var ally2 = h2.Place("僚機", "ベルク", 4, 3);
            var attacker2 = h2.Place("敵機", "ゾルグ", 2, 3, party: "敵");
            var tw2 = attacker2.Weapons.First(w => w.Name == "ライフル");
            Assert.AreSame(ally2, me2.LookForSupportGuard(attacker2, tw2));
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
        public void LookForSupport_TeamMismatchOnlyWhenBothTeamsAreSet()
        {
            // LookForSupport では双方にチーム指定があって異なる場合のみ除外する
            // （LookForSupportAttack と違い、片方だけの指定では除外しない）
            var h = CreateWorld(selfSkills: "チーム=青, 1");
            var me = h.Create("自機", "アレン");
            h.Place("僚機", "ベルク", 4, 3);
            Assert.AreEqual(2, me.LookForSupport(3, 3, true));

            var h2 = CreateWorld(supporterSkills: "援護Lv1, 1\nチーム=赤, 1");
            var me2 = h2.Create("自機", "アレン");
            h2.Place("僚機", "ベルク", 4, 3);
            Assert.AreEqual(2, me2.LookForSupport(3, 3, true));

            var h3 = CreateWorld(supporterSkills: "援護Lv1, 1\nチーム=赤, 1", selfSkills: "チーム=青, 1");
            var me3 = h3.Create("自機", "アレン");
            h3.Place("僚機", "ベルク", 4, 3);
            Assert.AreEqual(0, me3.LookForSupport(3, 3, true));
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
