using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Unit.Update（元の SRC の Unit.Update）による能力値・地形適応・武器補正・パイロット能力付加の再計算
    /// </summary>
    [TestClass]
    public class UnitStatusRecoverUpdateTests
    {
        private static SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private static UnitData AddUnitData(SRC src, string name = "テスト機体", string uclass = "汎用")
        {
            var ud = src.UDList.Add(name);
            ud.Class = uclass;
            ud.HP = 1000;
            ud.EN = 100;
            ud.Armor = 500;
            ud.Mobility = 50;
            ud.Speed = 4;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            return ud;
        }

        private static Unit CreateUnit(SRC src, UnitData ud, int pilotLevel = 1, bool withPilot = true)
        {
            var unit = src.UList.Add(ud.Name, 0, "味方");
            if (withPilot)
            {
                var pd = src.PDList.Add("テストパイロット");
                pd.Adaption = "SSSS";
                var pilot = src.PList.Add("テストパイロット", pilotLevel, "味方");
                pilot.Adaption = "SSSS";
                unit.AddPilot(pilot);
                pilot.Unit = unit;
            }
            unit.Update();
            unit.FullRecover();
            return unit;
        }

        private static void SetOption(SRC src, string name)
        {
            src.Expression.DefineGlobalVariable("Option(" + name + ")");
            src.Expression.SetVariableAsLong("Option(" + name + ")", 1);
        }

        private static string Adaption(Unit unit)
        {
            // パイロット側は SSSS なのでユニット側の地形適応がそのまま得られる
            return string.Concat(Enumerable.Range(1, 4).Select(i => "-DCBAS"[unit.get_Adaption(i)]));
        }

        // ──────────────────────────────────────────────
        // ランク・ボスランク
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Rank_IncreasesParametersPerRank()
        {
            // ランク1ごとにＨＰ+200、ＥＮ+10、装甲+100、運動性+5（移動力は変化しない）
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src));

            unit.Rank = 3;

            Assert.AreEqual(1600, unit.MaxHP);
            Assert.AreEqual(130, unit.MaxEN);
            Assert.AreEqual(800, unit.get_Armor("基本値"));
            Assert.AreEqual(65, unit.get_Mobility("基本値"));
            Assert.AreEqual(4, unit.Speed0);
        }

        [TestMethod]
        public void Update_KeepsHPAndENRatioWhenMaxChanges()
        {
            // 最大値が変動しても残りＨＰ・ＥＮの割合は保たれる
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src));
            unit.HP = 500;
            unit.EN = 25;

            unit.Rank = 5;

            Assert.AreEqual(2000, unit.MaxHP);
            Assert.AreEqual(1000, unit.HP);
            Assert.AreEqual(150, unit.MaxEN);
            Assert.AreEqual(38, unit.EN);
        }

        [TestMethod]
        public void Update_HPDoesNotBecomeZeroByRounding()
        {
            // 切り下げの結果ＨＰが0になる場合は1に留める
            var src = CreateSrc();
            var ud = AddUnitData(src);
            var unit = CreateUnit(src, ud);
            unit.HP = 1;

            ud.HP = 100;
            unit.Update();

            Assert.AreEqual(100, unit.MaxHP);
            Assert.AreEqual(1, unit.HP);
        }

        [TestMethod]
        public void BossRank_NonHero_UsesRobotTable()
        {
            // ロボットユニットはボスランク1でＨＰ+50%、装甲+300、ＥＮ+20、運動性+5
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src));

            unit.BossRank = 1;

            Assert.AreEqual(1500, unit.MaxHP);
            Assert.AreEqual(800, unit.get_Armor("基本値"));
            Assert.AreEqual(120, unit.MaxEN);
            Assert.AreEqual(55, unit.get_Mobility("基本値"));

            // ボスランク3ではＨＰ+(ＨＰ+10000)、装甲+1000
            unit.BossRank = 3;
            Assert.AreEqual(12000, unit.MaxHP);
            Assert.AreEqual(1500, unit.get_Armor("基本値"));
            Assert.AreEqual(160, unit.MaxEN);
        }

        [TestMethod]
        public void BossRank_Hero_UsesHeroTable()
        {
            // 人間ユニット（クラスが「(」で始まる）はボスランク2でＨＰ+(ＨＰ+10000)、装甲+200×ボスランク
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src, uclass: "(人間)"));

            unit.BossRank = 2;

            Assert.AreEqual(12000, unit.MaxHP);
            Assert.AreEqual(900, unit.get_Armor("基本値"));
        }

        [TestMethod]
        public void BossRank_ArmorReducedOption_Adds300PerRank()
        {
            // Option(BossRank装甲修正低下) では装甲修正が 300×ボスランク になる
            var src = CreateSrc();
            SetOption(src, "BossRank装甲修正低下");
            var unit = CreateUnit(src, AddUnitData(src));

            unit.BossRank = 3;

            Assert.AreEqual(1400, unit.get_Armor("基本値"));
        }

        [TestMethod]
        public void HPAndENGrowthOption_ScalesWithPilotLevel()
        {
            // Option(ＨＰ成長)/(ＥＮ成長) では最大値が (100 + パイロットレベル)% になる
            var src = CreateSrc();
            SetOption(src, "ＨＰ成長");
            SetOption(src, "ＥＮ成長");
            var unit = CreateUnit(src, AddUnitData(src), pilotLevel: 50);

            Assert.AreEqual(1500, unit.MaxHP);
            Assert.AreEqual(150, unit.MaxEN);
        }

        // ──────────────────────────────────────────────
        // 特殊能力による能力値の修正
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FixedEnhanceFeatures_AddPerLevel()
        {
            // ＨＰ強化 200/Lv、ＥＮ強化 10/Lv、装甲強化 100/Lv、運動性強化 5/Lv、移動力強化 1/Lv
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.AddFeature("ＨＰ強化Lv2");
            ud.AddFeature("ＥＮ強化Lv3");
            ud.AddFeature("装甲強化Lv1");
            ud.AddFeature("運動性強化Lv2");
            ud.AddFeature("移動力強化Lv1");
            var unit = CreateUnit(src, ud);

            Assert.AreEqual(1400, unit.MaxHP);
            Assert.AreEqual(130, unit.MaxEN);
            Assert.AreEqual(600, unit.get_Armor("基本値"));
            Assert.AreEqual(60, unit.get_Mobility("基本値"));
            Assert.AreEqual(5, unit.Speed0);
        }

        [TestMethod]
        public void RateEnhanceFeatures_AddDataValueTimesLevelOver20()
        {
            // 割合強化は 元の値×Lv÷20（Lv1 = 5%）
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.AddFeature("ＨＰ割合強化Lv2");
            ud.AddFeature("ＥＮ割合強化Lv4");
            ud.AddFeature("装甲割合強化Lv4");
            ud.AddFeature("運動性割合強化Lv2");
            var unit = CreateUnit(src, ud);

            Assert.AreEqual(1100, unit.MaxHP);
            Assert.AreEqual(120, unit.MaxEN);
            Assert.AreEqual(600, unit.get_Armor("基本値"));
            Assert.AreEqual(55, unit.get_Mobility("基本値"));
        }

        [TestMethod]
        public void EnhanceFeature_RequiresMorale()
        {
            // 「ＨＰ強化LvN=名称 気力」の気力条件を満たしたときだけ強化される
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.AddFeature("ＨＰ強化Lv2=ＨＰ強化 120");
            var unit = CreateUnit(src, ud);

            Assert.AreEqual(1000, unit.MaxHP);

            unit.MainPilot().Morale = 120;
            unit.Update();

            Assert.AreEqual(1400, unit.MaxHP);
        }

        [TestMethod]
        public void GrantedConditionFeature_IsAppliedAsFeature()
        {
            // 「～付加」状態は特殊能力として扱われる
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src));

            unit.AddCondition("ＨＰ強化付加", 3, 3d, "");
            unit.Update();

            Assert.IsTrue(unit.IsFeatureAvailable("ＨＰ強化"));
            Assert.AreEqual(1600, unit.MaxHP);
        }

        [TestMethod]
        public void AdditionalSpeed_AppliesOnlyInMatchingArea()
        {
            // 追加移動力は指定された場所にいる場合のみ有効。移動力は0未満にならない
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.AddFeature("追加移動力Lv2=追加移動力 空中");
            ud.AddFeature("追加移動力Lv-10=追加移動力 水中");
            var unit = CreateUnit(src, ud);

            unit.Area = "地上";
            unit.Update();
            Assert.AreEqual(4, unit.Speed0);

            unit.Area = "空中";
            unit.Update();
            Assert.AreEqual(6, unit.Speed0);

            unit.Area = "水中";
            unit.Update();
            Assert.AreEqual(0, unit.Speed0);
        }

        [TestMethod]
        public void Parameters_AreCappedAtUpperLimits()
        {
            // 最大ＨＰ 9999999、最大ＥＮ 9999、装甲 99999、運動性 9999、移動力 99 が上限
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.HP = 9999000;
            ud.EN = 9990;
            ud.Armor = 99990;
            ud.Mobility = 9990;
            ud.Speed = 98;
            var unit = CreateUnit(src, ud);

            unit.Rank = 10;

            Assert.AreEqual(9999999, unit.MaxHP);
            Assert.AreEqual(9999, unit.MaxEN);
            Assert.AreEqual(99999, unit.get_Armor("基本値"));
            Assert.AreEqual(9999, unit.get_Mobility("基本値"));

            unit.AddCondition("移動力強化付加", 1, 5d, "");
            unit.Update();
            Assert.AreEqual(99, unit.Speed0);
        }

        [TestMethod]
        public void VUpUnitItem_AddsByItemSlotCount()
        {
            // 「Ｖ－ＵＰ=ユニット」は ＨＰ+100×(スロット数+1)、ＥＮ+20×スロット数、装甲+50×スロット数、運動性+5×スロット数
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.ItemNum = 4;
            var unit = CreateUnit(src, ud);
            var idata = src.IDList.Add("Ｖ－ＵＰユニット");
            idata.Class = "固定";
            idata.Part = "非表示";
            idata.AddFeature("Ｖ－ＵＰ=ユニット");

            unit.AddItem(src.IList.Add("Ｖ－ＵＰユニット"));

            Assert.AreEqual(1500, unit.MaxHP);
            Assert.AreEqual(180, unit.MaxEN);
            Assert.AreEqual(700, unit.get_Armor("基本値"));
            Assert.AreEqual(70, unit.get_Mobility("基本値"));
        }

        [TestMethod]
        public void RankUp_ChangesUnitDataWhenRankReached()
        {
            // ランクアップLvN=ユニット はランクが N 以上になるとデータが置き換わる
            var src = CreateSrc();
            var upper = AddUnitData(src, "テスト機体改");
            upper.HP = 3000;
            var ud = AddUnitData(src);
            ud.AddFeature("ランクアップLv3=テスト機体改");
            var unit = CreateUnit(src, ud);

            unit.Rank = 2;
            Assert.AreEqual("テスト機体", unit.Data.Name);

            unit.Rank = 3;
            Assert.AreEqual("テスト機体改", unit.Data.Name);
            Assert.AreEqual(3600, unit.MaxHP);
        }

        // ──────────────────────────────────────────────
        // 地形適応
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MoveTypeFeatures_RaiseAdaptionToA()
        {
            // 空中移動・陸上移動・水中移動・宇宙移動は対応する地形適応を最低でもAにする
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.Adaption = "-DC-";
            ud.AddFeature("空中移動");
            ud.AddFeature("水中移動");
            var unit = CreateUnit(src, ud);

            Assert.AreEqual("ADA-", Adaption(unit));
        }

        [TestMethod]
        public void AdaptionChange_AddsButNotAboveA()
        {
            // 地形適応変更は加算するが、Aより高くはならない（元からA以上なら上げない）。負の値は下げる
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.Adaption = "CBAS";
            ud.AddFeature("地形適応変更=1 3 -1 1");
            var unit = CreateUnit(src, ud);

            Assert.AreEqual("BABS", Adaption(unit));
        }

        [TestMethod]
        public void AdaptionFixedChange_PrefersHigherUnlessForced()
        {
            // 地形適応固定変更は高い方を優先し、「強制」指定時はそのまま置き換える
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.Adaption = "ABCD";
            ud.AddFeature("地形適応固定変更=5 2 0 0");
            var unit = CreateUnit(src, ud);
            Assert.AreEqual("SBCD", Adaption(unit));

            var src2 = CreateSrc();
            var ud2 = AddUnitData(src2);
            ud2.Adaption = "ABCD";
            ud2.AddFeature("地形適応固定変更=5 2 0 0 強制");
            var unit2 = CreateUnit(src2, ud2);
            Assert.AreEqual("SC--", Adaption(unit2));
        }

        // ──────────────────────────────────────────────
        // パイロット能力付加・強化
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PilotSkillGrant_AddsGrant2Condition()
        {
            // パイロット能力付加=技能LvN は「技能付加２」状態として付加される
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.AddFeature("パイロット能力付加=超感覚Lv2");
            var unit = CreateUnit(src, ud);

            Assert.IsTrue(unit.IsConditionSatisfied("超感覚付加２"));
            Assert.AreEqual(2d, unit.ConditionLevel("超感覚付加２"));
            Assert.AreEqual(2d, unit.MainPilot().SkillLevel("超感覚", ref_mode: ""));
        }

        [TestMethod]
        public void PilotSkillGrant_SealedSkillIsNotGranted()
        {
            // 「技能使用不能」状態の技能は付加されない
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.AddFeature("パイロット能力付加=超感覚Lv2");
            var unit = CreateUnit(src, ud);

            unit.AddCondition("超感覚使用不能", 2, 0d, "");
            unit.Update();

            Assert.IsFalse(unit.IsConditionSatisfied("超感覚付加２"));
        }

        [TestMethod]
        public void PilotSkillGrant_RemovedWhenSourceFeatureIsGone()
        {
            // 付加２状態は Update ごとに作り直されるため、付加元の特殊能力が無くなると消える
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src));
            unit.AddCondition("パイロット能力付加付加", 2, 0d, "超感覚Lv3");
            unit.Update();
            Assert.AreEqual(3d, unit.ConditionLevel("超感覚付加２"));

            unit.DeleteCondition("パイロット能力付加付加");
            unit.Update();

            Assert.IsFalse(unit.IsConditionSatisfied("超感覚付加２"));
        }

        [TestMethod]
        public void PilotSkillEnhance_LevelsAccumulate()
        {
            // パイロット能力強化は同じ技能に対して強化レベルが累積する
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.AddFeature("パイロット能力強化=超感覚Lv1");
            ud.AddFeature("パイロット能力強化=超感覚Lv2");
            var unit = CreateUnit(src, ud);

            Assert.IsTrue(unit.IsConditionSatisfied("超感覚強化２"));
            Assert.AreEqual(3d, unit.ConditionLevel("超感覚強化２"));
        }

        // ──────────────────────────────────────────────
        // 武器の補正
        // ──────────────────────────────────────────────

        private static WeaponData AddWeapon(UnitData ud, string name, int power, string wclass, int maxRange = 3, int bullet = 0)
        {
            var wd = ud.AddWeapon(name);
            wd.Power = power;
            wd.Class = wclass;
            wd.MinRange = 1;
            wd.MaxRange = maxRange;
            wd.Bullet = bullet;
            wd.Precision = 0;
            wd.Critical = 0;
            return wd;
        }

        private static UnitWeapon Weapon(Unit unit, string name)
        {
            return unit.Weapons.First(w => w.Name == name);
        }

        [TestMethod]
        public void WeaponPower_RankCorrectionDependsOnClass()
        {
            // 通常 +100×ランク、Ｒ（Lv無し）・改（Lv無し）+50×ランク、ＲLvN +10×N×ランク、固 は補正なし、攻撃力0 は0のまま
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "通常", 1000, "射");
            AddWeapon(ud, "低成長", 1000, "射Ｒ");
            AddWeapon(ud, "低成長Lv", 1000, "射ＲL3");
            AddWeapon(ud, "改造", 1000, "射改");
            AddWeapon(ud, "固定", 1000, "射固");
            AddWeapon(ud, "ゼロ", 0, "射");
            var unit = CreateUnit(src, ud);

            unit.Rank = 2;

            Assert.AreEqual(1200, Weapon(unit, "通常").WeaponPower(""));
            Assert.AreEqual(1100, Weapon(unit, "低成長").WeaponPower(""));
            Assert.AreEqual(1060, Weapon(unit, "低成長Lv").WeaponPower(""));
            Assert.AreEqual(1100, Weapon(unit, "改造").WeaponPower(""));
            Assert.AreEqual(1000, Weapon(unit, "固定").WeaponPower(""));
            Assert.AreEqual(0, Weapon(unit, "ゼロ").WeaponPower(""));
        }

        [TestMethod]
        public void WeaponPower_BossRankAddsUpTo300()
        {
            // ボスランクによる攻撃力修正は 100×ボスランク（最大300）
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "通常", 1000, "射");
            var unit = CreateUnit(src, ud);

            unit.BossRank = 2;
            Assert.AreEqual(1200, Weapon(unit, "通常").WeaponPower(""));

            unit.BossRank = 5;
            Assert.AreEqual(1300, Weapon(unit, "通常").WeaponPower(""));
        }

        [TestMethod]
        public void WeaponEnhanceFeatures_AddPower()
        {
            // 武器強化 +100/Lv、武器割合強化 +攻撃力×Lv÷20。固属性は明示指定された場合のみ
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "通常", 1000, "射");
            AddWeapon(ud, "固定", 1000, "射固");
            ud.AddFeature("武器強化Lv1");
            ud.AddFeature("武器割合強化Lv2");
            var unit = CreateUnit(src, ud);

            Assert.AreEqual(1200, Weapon(unit, "通常").WeaponPower(""));
            Assert.AreEqual(1000, Weapon(unit, "固定").WeaponPower(""));
        }

        [TestMethod]
        public void WeaponEnhance_FixedWeaponNamedExplicitly_IsEnhanced()
        {
            // 固属性の武器でも名前で指定されていれば強化される
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "固定", 1000, "射固");
            ud.AddFeature("武器強化Lv1=固定");
            var unit = CreateUnit(src, ud);

            Assert.AreEqual(1100, Weapon(unit, "固定").WeaponPower(""));
        }

        [TestMethod]
        public void RangeExtension_AppliesOnlyToRangedNonMeleeWeapons()
        {
            // 射程延長は最大射程1の武器・Ｍ・武/突/接・有 属性には適用されない
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "ライフル", 1000, "射", maxRange: 3);
            AddWeapon(ud, "格闘", 1000, "格", maxRange: 1);
            AddWeapon(ud, "投擲", 1000, "武", maxRange: 2);
            AddWeapon(ud, "有線", 1000, "有", maxRange: 4);
            ud.AddFeature("射程延長Lv2");
            var unit = CreateUnit(src, ud);

            Assert.AreEqual(5, Weapon(unit, "ライフル").WeaponMaxRange());
            Assert.AreEqual(1, Weapon(unit, "格闘").WeaponMaxRange());
            Assert.AreEqual(2, Weapon(unit, "投擲").WeaponMaxRange());
            Assert.AreEqual(4, Weapon(unit, "有線").WeaponMaxRange());
        }

        [TestMethod]
        public void PrecisionEnhance_Adds5PerLevel()
        {
            // 命中率強化は +5/Lv
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "ライフル", 1000, "射");
            ud.AddFeature("命中率強化Lv2");
            var unit = CreateUnit(src, ud);

            Assert.AreEqual(10, Weapon(unit, "ライフル").WeaponPrecision());
        }

        [TestMethod]
        [Ignore("移植の不具合: Unit.Update がＣＴ率強化の補正値を計算するが SetCriticalCorrection を呼ばず捨てている")]
        public void CriticalEnhance_Adds5PerLevel()
        {
            // ＣＴ率強化は通常武器のＣＴ率を +5/Lv（元の SRC の Unit.Update「ＣＴ率強化による修正」）
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "ライフル", 1000, "射");
            ud.AddFeature("ＣＴ率強化Lv2");
            var unit = CreateUnit(src, ud);

            Assert.AreEqual(10, Weapon(unit, "ライフル").WeaponCritical());
        }

        [TestMethod]
        public void MaxBullet_IncreasedByFeatureAndBossRank()
        {
            // 最大弾数増加は +50%/Lv、ボスランクは +20%/ランク
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "ミサイル", 1000, "射", bullet: 10);
            ud.AddFeature("最大弾数増加Lv1");
            var unit = CreateUnit(src, ud);

            Assert.AreEqual(15, Weapon(unit, "ミサイル").MaxBullet());

            unit.BossRank = 1;
            Assert.AreEqual(17, Weapon(unit, "ミサイル").MaxBullet());
        }

        [TestMethod]
        public void AttackAttribute_AddsAttributeToWeapons()
        {
            // 攻撃属性=火 は全武器に「火」属性を追加する
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "パンチ", 1000, "格", maxRange: 1);
            ud.AddFeature("攻撃属性=火");
            var unit = CreateUnit(src, ud);

            Assert.IsTrue(Weapon(unit, "パンチ").IsWeaponClassifiedAs("火"));
            Assert.IsTrue(Weapon(unit, "パンチ").IsWeaponClassifiedAs("格"));
        }

        [TestMethod]
        [Ignore("移植の不具合: 攻撃属性の処理で武器属性を「|」で切った後の文字列から非表示属性を取り出しているため非表示属性が失われる")]
        public void AttackAttribute_KeepsHiddenAttribute()
        {
            // 元の SRC は「|」以降の非表示属性を退避し、攻撃属性を付けた後に戻す
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "パンチ", 1000, "格|毒", maxRange: 1);
            ud.AddFeature("攻撃属性=火");
            var unit = CreateUnit(src, ud);

            Assert.AreEqual("格火|毒", Weapon(unit, "パンチ").WeaponClass());
        }

        [TestMethod]
        public void Abilities_IncludeUnitAndPilotAbilities()
        {
            // アビリティはユニット・パイロットのデータから作り直される
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.AddAbility("修理").Stock = 2;
            var unit = CreateUnit(src, ud);
            unit.MainPilot().Data.AddAbility("祈り").Stock = 1;

            unit.Update();

            CollectionAssert.AreEquivalent(new[] { "修理", "祈り" }, unit.Abilities.Select(a => a.Data.Name).ToList());
        }

        // ──────────────────────────────────────────────
        // 攻撃への耐性
        // ──────────────────────────────────────────────

        [TestMethod]
        [Ignore("移植の不具合: Unit.Update が吸収/無効化/耐性/弱点/有効/特殊効果無効化の属性文字列を更新していない")]
        public void DefenseFeatures_UpdateAttributeResistance()
        {
            // 元の SRC の Unit.Update「攻撃への耐性を更新」で特殊能力から耐性文字列が作られる
            var src = CreateSrc();
            var ud = AddUnitData(src);
            ud.AddFeature("弱点=火");
            ud.AddFeature("耐性=冷");
            ud.AddFeature("吸収=雷");
            ud.AddFeature("無効化=水");
            ud.AddFeature("特殊効果無効化=毒");
            var unit = CreateUnit(src, ud);

            Assert.IsTrue(unit.Weakness("火"));
            Assert.IsTrue(unit.Resist("冷"));
            Assert.IsTrue(unit.Absorb("雷"));
            Assert.IsTrue(unit.Immune("水"));
            Assert.IsTrue(unit.SpecialEffectImmune("毒"));
        }
    }
}
