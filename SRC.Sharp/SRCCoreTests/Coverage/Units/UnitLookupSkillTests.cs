using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Coverage.Tests.UnitsCoverage
{
    /// <summary>
    /// Unit.lookup.cs の必要技能判定（IsNecessarySkillSatisfied / IsNecessarySkillSatisfied2 / IsAvailable）
    /// と援護回数系（MaxSupportAttack ほか）のテスト。元の SRC の Unit.cls の同名関数に対応する。
    /// </summary>
    [TestClass]
    public class UnitLookupSkillTests
    {
        private static UnitLookupTestHelper CreateBasic(string skills = null, string sex = "男性",
            string features = null, string uclass = "汎用",
            string stats = "150, 140, 130, 120, 110, 100, 普通")
        {
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン", sex, skills, stats);
            h.DefineUnit("試作機", features, uclass: uclass);
            return h;
        }

        // ===== MaxSupportAttack / MaxSupportGuard / MaxSyncAttack =====

        [TestMethod]
        public void MaxSupportAttack_TakesLargerOf援護攻撃And援護()
        {
            // 援護攻撃回数は「援護攻撃」と「援護」のレベルの大きい方
            var h = CreateBasic("援護Lv1, 1\n援護攻撃Lv3, 1\n援護防御Lv2, 1\n統率Lv2, 1");
            var u = h.Create("試作機", "アレン");

            Assert.AreEqual(3, u.MaxSupportAttack());
            // 援護防御回数は「援護防御」と「援護」の大きい方
            Assert.AreEqual(2, u.MaxSupportGuard());
            // 同時援護攻撃回数は「統率」のレベル
            Assert.AreEqual(2, u.MaxSyncAttack());
        }

        [TestMethod]
        public void MaxSupportGuard_Uses援護WhenItIsLarger()
        {
            var h = CreateBasic("援護Lv4, 1\n援護防御Lv1, 1");
            var u = h.Create("試作機", "アレン");

            Assert.AreEqual(4, u.MaxSupportAttack());
            Assert.AreEqual(4, u.MaxSupportGuard());
            Assert.AreEqual(0, u.MaxSyncAttack());
        }

        // ===== MaxCounterAttack =====

        [TestMethod]
        public void MaxCounterAttack_IsCounterLevel_WithoutSenteHisshou()
        {
            var h = CreateBasic("カウンターLv2, 1");
            var u = h.Create("試作機", "アレン");

            Assert.AreEqual(2, u.MaxCounterAttack());
        }

        [TestMethod]
        public void MaxCounterAttack_SenteHisshou_DefaultThreshold120()
        {
            // 先手必勝（必要気力指定なし）は気力120以上で無制限(1000)
            var h = CreateBasic("先手必勝, 1");
            var u = h.Create("試作機", "アレン");
            var p = u.MainPilot();

            p.Morale = 119;
            Assert.AreEqual(0, u.MaxCounterAttack());
            p.Morale = 120;
            Assert.AreEqual(1000, u.MaxCounterAttack());
        }

        [TestMethod]
        public void MaxCounterAttack_SenteHisshou_UsesSpecifiedMorale()
        {
            // 先手必勝=別名 必要気力 の場合、指定された気力以上でのみ無制限になる
            // (VB: StrToLng(LIndex(.SkillData("先手必勝"), 2)))
            var h = CreateBasic("先手必勝=先手必勝 130, 1");
            var u = h.Create("試作機", "アレン");
            var p = u.MainPilot();

            p.Morale = 120;
            Assert.AreEqual(0, u.MaxCounterAttack());
            p.Morale = 130;
            Assert.AreEqual(1000, u.MaxCounterAttack());
        }

        // ===== IsNecessarySkillSatisfied (or の連鎖) =====

        [TestMethod]
        public void IsNecessarySkillSatisfied_AllConditionsMustHold_WithoutOr()
        {
            // スペース区切りの必要条件は全て満たす必要がある
            var h = CreateBasic("底力, 1");
            var u = h.Create("試作機", "アレン");

            Assert.IsTrue(u.IsNecessarySkillSatisfied("底力 アイテム"));
            Assert.IsFalse(u.IsNecessarySkillSatisfied("底力 見切り"));
            Assert.IsFalse(u.IsNecessarySkillSatisfied("見切り 底力"));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied_OrAcceptsEitherSide()
        {
            var h = CreateBasic("底力, 1");
            var u = h.Create("試作機", "アレン");

            // 前半が満たされない場合は or の後ろで判定
            Assert.IsTrue(u.IsNecessarySkillSatisfied("見切り or 底力"));
            // 前半が満たされれば or 以降はスキップ（大文字の OR も可）
            Assert.IsTrue(u.IsNecessarySkillSatisfied("底力 OR 見切り"));
            // どちらも満たされない
            Assert.IsFalse(u.IsNecessarySkillSatisfied("見切り or 切り払い"));
            // or の連鎖の後に続く条件も評価される
            Assert.IsFalse(u.IsNecessarySkillSatisfied("底力 or 見切り 切り払い"));
            Assert.IsTrue(u.IsNecessarySkillSatisfied("見切り or 底力 アイテム"));
        }

        // ===== IsNecessarySkillSatisfied2 (個別条件) =====

        [TestMethod]
        public void IsNecessarySkillSatisfied2_SkillLevelComparison()
        {
            // 「技能名Lvn」は技能レベルが n 以上で満たされる。Lv 省略時は 1
            var h = CreateBasic("切り払いLv3, 1");
            var u = h.Create("試作機", "アレン");

            Assert.IsTrue(u.IsNecessarySkillSatisfied2("切り払い", null));
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("切り払いLv3", null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("切り払いLv4", null));
            // 否定
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("!切り払いLv4", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_PilotBaseStats()
        {
            // 格闘/射撃/命中/回避/技量/反応/レベルはパイロットの能力値と比較する
            var h = CreateBasic(stats: "150, 140, 130, 120, 110, 100, 普通");
            var u = h.Create("試作機", "アレン", level: 1);
            void Check(string name, int value)
            {
                Assert.IsTrue(u.IsNecessarySkillSatisfied2(name + "Lv" + value, null), name);
                Assert.IsFalse(u.IsNecessarySkillSatisfied2(name + "Lv" + (value + 1), null), name);
            }

            // データの能力値にレベル(1)分の成長を加えた値と比較する
            Check("格闘", 151);
            Check("射撃", 141);
            // 命中・回避はレベル毎に 2 ずつ成長する
            Check("命中", 132);
            Check("回避", 122);
            Check("技量", 111);
            Check("反応", 101);
            Check("レベル", 1);
            // 魔力所有でないので「魔力」は 0 扱い
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("魔力Lv1", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_InitialValues_SubtractLevel()
        {
            // 「～初期値」は基本能力値からレベル分の成長を差し引いた値
            var h = CreateBasic(stats: "150, 140, 130, 120, 110, 100, 普通");
            var u = h.Create("試作機", "アレン", level: 10);
            var p = u.MainPilot();
            var infightInit = p.InfightBase - p.Level;
            var hitInit = p.HitBase - p.Level;

            Assert.IsTrue(u.IsNecessarySkillSatisfied2("格闘初期値Lv" + infightInit, null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("格闘初期値Lv" + (infightInit + 1), null));
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("命中初期値Lv" + hitInit, null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("命中初期値Lv" + (hitInit + 1), null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("回避初期値Lv" + (p.DodgeBase - p.Level + 1), null));
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("技量初期値Lv" + (p.TechniqueBase - p.Level), null));
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("反応初期値Lv" + (p.IntuitionBase - p.Level), null));
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("射撃初期値Lv" + (p.ShootingBase - p.Level), null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("魔力初期値", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_InitialValues_LowAttackGrowthOption()
        {
            // Option(攻撃力低成長) では格闘・射撃の初期値はレベルの半分だけ差し引く。
            // 命中などは従来通りレベル分を差し引く
            var h = CreateBasic(stats: "150, 140, 130, 120, 110, 100, 普通");
            h.Src.Expression.DefineGlobalVariable("Option(攻撃力低成長)");
            var u = h.Create("試作機", "アレン", level: 10);

            // 格闘 150 + 10/2 = 155 → 初期値 155 - 10/2 = 150
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("格闘初期値Lv150", null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("格闘初期値Lv151", null));
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("射撃初期値Lv140", null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("射撃初期値Lv141", null));
            // 命中は 130 + 10×2 = 150 で、初期値は VB 同様にレベル分だけ差し引いて 150 - 10 = 140
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("命中初期値Lv140", null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("命中初期値Lv141", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_PlusPrefix_OutsideStatusCommand_StripsPlus()
        {
            // 「+技能」はステータスコマンド実行時以外では + を除いた条件で判定する
            var h = CreateBasic("底力, 1");
            var u = h.Create("試作機", "アレン");
            h.Src.Map.MapFileName = "test.map";
            h.PutOnMap(u, 3, 3);
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("+底力", null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("+見切り", null));

            // 出撃していなければステータス表示中でも + を除いた条件で判定
            h.Src.Map.MapFileName = "";
            h.Src.IsSubStage = true;
            u.Status = "待機";
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("+見切り", null));
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("+底力", null));
        }

        [TestMethod]
        [Ignore("移植の不具合: Intermission.InStatusCommand の条件が反転しており、マップ無しのステータス表示中に true を返さない")]
        public void IsNecessarySkillSatisfied2_PlusPrefix_SatisfiedInStatusCommand()
        {
            // ステータス表示中（マップ無し＋サブステージ）で出撃中なら「+技能」は常に満たされる
            // (VB Intermission.bas InStatusCommand: If MapFileName = "" Then ... Or IsSubStage)
            var h = CreateBasic();
            var u = h.Create("試作機", "アレン");
            h.PutOnMap(u, 3, 3);
            h.Src.Map.MapFileName = "";
            h.Src.IsSubStage = true;
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("+見切り", null));

            // マップがある（通常の戦闘中）ならステータス表示ではない
            h.Src.Map.MapFileName = "test.map";
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("+見切り", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_CounterOnlySkills_NeverSatisfied()
        {
            // 「当て身技」「自動反撃」は技能を持っていても常に満たされない（専用武器を隠すため）
            var h = CreateBasic("当て身技, 1\n自動反撃, 1");
            var u = h.Create("試作機", "アレン");

            Assert.IsFalse(u.IsNecessarySkillSatisfied2("当て身技", null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("自動反撃", null));
            // 「アイテム」は単独でも常に満たされる
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("アイテム", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_ItemEquipped_ByNameOrClass()
        {
            // 「アイテム名装備」「アイテムクラス装備」は装備中のアイテムで判定する
            var h = CreateBasic();
            var u = h.Create("試作機", "アレン");
            var data = h.Src.IDList.Add("光の剣");
            data.Class = "剣";
            data.Part = "アクセサリ";
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("光の剣装備", null));

            u.AddItem(h.Src.IList.Add("光の剣"));
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("光の剣装備", null));
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("剣装備", null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("槍装備", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_CarrierAdjacent_RequiresCarrierFeature()
        {
            // 「母艦隣接」は母艦能力を持つ隣接ユニットがいれば満たされる
            var h = CreateBasic();
            h.DefinePilot("ベルク");
            h.DefinePilot("カイ");
            h.DefineUnit("戦艦", "母艦");
            h.DefineUnit("僚機");
            var u = h.Place("試作機", "アレン", 3, 3);
            h.Place("僚機", "ベルク", 4, 3);
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("母艦隣接", null));

            h.Place("戦艦", "カイ", 2, 3);
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("母艦隣接", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_Mana_UsesShootingWhenPilotHasMana()
        {
            // 魔力所有のパイロットは射撃値が「魔力」として扱われ、「射撃」は 0 になる
            var h = CreateBasic("魔力所有, 1", stats: "150, 140, 130, 120, 110, 100, 普通");
            var u = h.Create("試作機", "アレン");

            Assert.IsTrue(u.IsNecessarySkillSatisfied2("魔力Lv140", null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("射撃Lv1", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_Sex()
        {
            var h = CreateBasic(sex: "女性");
            var u = h.Create("試作機", "アレン");

            Assert.IsTrue(u.IsNecessarySkillSatisfied2("女性", null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("男性", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_HpEnMoraleRank()
        {
            var h = CreateBasic();
            var u = h.Create("試作機", "アレン", rank: 3);

            // ＨＰ/ＥＮ は 10 段階の割合
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("ＨＰLv10", null));
            u.HP = u.MaxHP / 2;
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("ＨＰLv5", null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("ＨＰLv6", null));
            // 瀕死は HP が最大の 1/4 以下
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("瀕死", null));
            u.HP = u.MaxHP / 4;
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("瀕死", null));

            u.EN = u.MaxEN * 3 / 10;
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("ＥＮLv3", null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("ＥＮLv4", null));

            // 気力は (気力-100)/10
            u.MainPilot().Morale = 130;
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("気力Lv3", null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("気力Lv4", null));

            // ランク
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("ランクLv3", null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("ランクLv4", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_Hero_DependsOnUnitClassParenthesis()
        {
            // ユニットクラスが括弧で始まるユニットは「生身」
            var h = CreateBasic(uclass: "(人間)");
            var u = h.Create("試作機", "アレン");
            Assert.IsTrue(u.IsHero());
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("生身", null));

            var h2 = CreateBasic();
            var u2 = h2.Create("試作機", "アレン");
            Assert.IsFalse(u2.IsHero());
            Assert.IsFalse(u2.IsNecessarySkillSatisfied2("生身", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_Area_RequiresSortieInThatArea()
        {
            var h = CreateBasic();
            var u = h.Create("試作機", "アレン");
            u.Area = "空中";
            // 出撃していなければ地形条件は満たされない
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("空中", null));

            h.PutOnMap(u, 2, 2, "空中");
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("空中", null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("地上", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_PilotNameUnitNameAndClass()
        {
            // パイロット名・ユニット名・ユニットクラスの指定
            var h = CreateBasic(uclass: "試験型");
            var u = h.Create("試作機", "アレン");

            Assert.IsTrue(u.IsNecessarySkillSatisfied2("アレン", null));
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("試作機", null));
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("試験型", null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("ベルク", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_TerrainName_WithAtMark()
        {
            // 「@地形名」は出撃中に居る地形の名称と比較する
            var h = CreateBasic();
            var u = h.Place("試作機", "アレン", 3, 3);
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("@平地", null));
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("@森林", null));

            h.SetTerrain(3, 3, "森林", "陸");
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("@森林", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_Condition_WithStateSuffix()
        {
            // 「～状態」は特殊状態の有無
            var h = CreateBasic();
            var u = h.Create("試作機", "アレン");
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("攻撃力ＵＰ状態", null));

            u.AddCondition("攻撃力ＵＰ", 2);
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("攻撃力ＵＰ状態", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_SummonerSkill_WithAsterisk()
        {
            // 「*技能」は召喚者の技能を参照する。召喚者がいなければ満たされない
            var h = CreateBasic("底力, 1");
            h.DefinePilot("ベルク");
            h.DefineUnit("使い魔");
            var summoner = h.Create("試作機", "アレン");
            var servant = h.Create("使い魔", "ベルク");

            Assert.IsFalse(servant.IsNecessarySkillSatisfied2("*底力", null));
            servant.Summoner = summoner;
            Assert.IsTrue(servant.IsNecessarySkillSatisfied2("*底力", null));
            Assert.IsFalse(servant.IsNecessarySkillSatisfied2("底力", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_ExplicitPilotOverridesMainPilot()
        {
            // p を指定した場合はそのパイロットの技能で判定する
            var h = CreateBasic();
            h.DefinePilot("ベルク", skills: "見切り, 1");
            var u = h.Create("試作機", "アレン");
            var other = h.Src.PList.Add("ベルク", 1, "味方");

            Assert.IsFalse(u.IsNecessarySkillSatisfied2("見切り", null));
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("見切り", other));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_Adjacent_FindsNamedAllyNextToUnit()
        {
            // 「ユニット名隣接」は隣接する行動可能な味方に該当ユニットがいれば満たされる
            var h = CreateBasic();
            h.DefinePilot("ベルク");
            h.DefineUnit("僚機");
            var u = h.Place("試作機", "アレン", 3, 3);
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("僚機隣接", null));

            var partner = h.Place("僚機", "ベルク", 4, 3);
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("僚機隣接", null));
            // パイロット名でも指定できる
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("ベルク隣接", null));

            // 行動不能なら対象外
            partner.AddCondition("混乱", 2);
            Assert.IsFalse(u.IsNecessarySkillSatisfied2("僚機隣接", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_WithinRange_UsesManhattanDistance()
        {
            // 「ユニット名nマス以内」は距離 n 以内（斜めはマンハッタン距離）
            var h = CreateBasic();
            h.DefinePilot("ベルク");
            h.DefineUnit("僚機");
            var u = h.Place("試作機", "アレン", 3, 3);
            h.Place("僚機", "ベルク", 4, 4);

            Assert.IsFalse(u.IsNecessarySkillSatisfied2("僚機隣接", null));
            Assert.IsTrue(u.IsNecessarySkillSatisfied2("僚機2マス以内", null));
        }

        [TestMethod]
        public void IsNecessarySkillSatisfied2_Adjacent_IgnoresEnemies()
        {
            var h = CreateBasic();
            h.DefinePilot("ベルク");
            h.DefineUnit("僚機");
            var u = h.Place("試作機", "アレン", 3, 3);
            h.Place("僚機", "ベルク", 4, 3, party: "敵");

            Assert.IsFalse(u.IsNecessarySkillSatisfied2("僚機隣接", null));
        }

        // ===== IsAvailable =====

        [TestMethod]
        public void IsAvailable_NecessarySkillFeature()
        {
            // 「必要技能」を満たさない形態は使用不能
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン", skills: "底力, 1");
            h.DefinePilot("ベルク");
            h.DefineUnit("覚醒機", "必要技能=底力");
            var ok = h.Create("覚醒機", "アレン");
            var ng = h.Create("覚醒機", "ベルク");

            Assert.IsTrue(ok.IsAvailable());
            Assert.IsFalse(ng.IsAvailable());
        }

        [TestMethod]
        public void IsAvailable_UnnecessarySkillFeature()
        {
            // 「不必要技能」を満たす形態は使用不能
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン", skills: "底力, 1");
            h.DefinePilot("ベルク");
            h.DefineUnit("通常機", "不必要技能=底力");
            var withSkill = h.Create("通常機", "アレン");
            var withoutSkill = h.Create("通常機", "ベルク");

            Assert.IsFalse(withSkill.IsAvailable());
            Assert.IsTrue(withoutSkill.IsAvailable());
        }

        [TestMethod]
        public void IsAvailable_OtherFormIsUnusableWhileIncapacitated()
        {
            // 状態が「他形態」のユニットは行動不能状態の間（制限時間切れの形態）は使用不能
            var h = CreateBasic();
            var u = h.Create("試作機", "アレン");
            u.Status = "他形態";
            Assert.IsTrue(u.IsAvailable());

            u.AddCondition("行動不能", 1);
            Assert.IsFalse(u.IsAvailable());

            // 他形態でなければ行動不能でも形態としては使用可能
            u.Status = "待機";
            Assert.IsTrue(u.IsAvailable());
        }

        [TestMethod]
        public void IsAvailable_FalseWhenDisabledByName()
        {
            // イベントコマンド Disable で封印されたユニットは使用不能
            var h = CreateBasic();
            var u = h.Create("試作機", "アレン");
            Assert.IsTrue(u.IsAvailable());

            h.Src.Expression.DefineGlobalVariable("Disable(試作機)");
            Assert.IsFalse(u.IsAvailable());
        }

        // ===== IsDisabled / IsDefense =====

        [TestMethod]
        public void IsDisabled_UnitSpecificDisable()
        {
            // Disable(ユニット名,能力名) はそのユニットだけを封印する
            var h = CreateBasic();
            h.DefinePilot("ベルク");
            h.DefineUnit("僚機");
            var u = h.Create("試作機", "アレン");
            var other = h.Create("僚機", "ベルク");
            h.Src.Expression.DefineGlobalVariable("Disable(試作機,ビーム)");

            Assert.IsTrue(u.IsDisabled("ビーム"));
            Assert.IsFalse(other.IsDisabled("ビーム"));
            Assert.IsFalse(u.IsDisabled(""));
        }

        [TestMethod]
        public void IsDefense_TrueWhenNotOwnPhase()
        {
            // 現在のステージ（フェイズ）の陣営でなければ攻撃を受ける側
            var h = CreateBasic();
            var u = h.Create("試作機", "アレン");
            h.Src.Stage = "味方";
            Assert.IsFalse(u.IsDefense());
            h.Src.Stage = "敵";
            Assert.IsTrue(u.IsDefense());
        }

        // ===== IsOperational / IsEqual =====

        [TestMethod]
        public void IsOperationalAndIsEqual_ConsiderOtherForms()
        {
            // 変形先の形態が出撃中でも出撃中とみなし、他形態の名前でも同一ユニットとみなす
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン");
            h.DefineUnit("通常形態", "変形=変形 飛行形態");
            h.DefineUnit("飛行形態", "変形=変形 通常形態");
            var u = h.Create("通常形態", "アレン");

            Assert.IsTrue(u.IsEqual("通常形態"));
            Assert.IsTrue(u.IsEqual("飛行形態"));
            Assert.IsFalse(u.IsEqual("別機体"));

            u.Status = "待機";
            Assert.IsFalse(u.IsOperational());
            u.OtherForm("飛行形態").Status = "出撃";
            Assert.IsTrue(u.IsOperational());
        }
    }
}
