using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.Units;
using System;
using static SRCCore.Coverage.Tests.UnitWeaponCov.UnitWeaponCovHelper;

namespace SRCCore.Coverage.Tests.UnitWeaponCov
{
    /// <summary>
    /// UnitWeapon.HitProbability / Damage / CriticalProbability のテスト。
    /// 期待値は VB 版 Unit.cls の HitProbability / Damage / CriticalProbability に従う。
    /// 基本の組み合わせ:
    ///   攻撃側 命中 50・技量 50、武器 命中 10・CT 10・攻撃力 1000（地形適応Ａ → 1.2 倍）
    ///   防御側 回避 40・技量 20、装甲 500、地形適応Ｂ（装甲 1.0 倍）、サイズＭ
    ///   → 命中率 (160 - 40) = 120、ダメージ 1200 - 500 = 700、ＣＴ率 (10 + 50) - 20 = 40
    /// </summary>
    [TestClass]
    public class UnitWeaponCovCombatTests
    {
        private class Battle
        {
            public SRC Src;
            public Unit Attacker;
            public Unit Target;
            public UnitWeapon W;
        }

        private static Battle Setup(
            string wclass = "射", int maxRange = 4, int tx = 2, int ty = 1, string targetArea = "地上",
            Action<SRC> configureSrc = null,
            Action<UnitData> configureAttacker = null,
            Action<UnitData> configureTarget = null,
            Action<PilotData> configureTargetPilot = null,
            Action<PilotData> configureAttackerPilot = null)
        {
            var src = CreateSrc();
            src.Stage = "味方";
            configureSrc?.Invoke(src);
            var a = CreateUnit(src, x: 1, y: 1, configure: ud =>
            {
                configureAttacker?.Invoke(ud);
                AddWeapon(ud, "武器", power: 1000, wclass: wclass, maxRange: maxRange, precision: 10, critical: 10);
            }, configurePilot: configureAttackerPilot);
            a.MainPilot().Hit = 50;
            a.MainPilot().Technique = 50;
            var t = CreateUnit(src, party: "敵", x: tx, y: ty, area: targetArea, configure: ud =>
            {
                ud.Armor = 500;
                ud.Adaption = "BBBB";
                configureTarget?.Invoke(ud);
            }, configurePilot: configureTargetPilot);
            t.MainPilot().Dodge = 40;
            t.MainPilot().Technique = 20;
            return new Battle { Src = src, Attacker = a, Target = t, W = Weapon(a, "武器") };
        }

        // ══════════════════════════════════════════════
        // HitProbability
        // ══════════════════════════════════════════════

        [TestMethod]
        public void HitProbability_Basic()
        {
            // (100 + 命中 + 直感 + 運動性 + 武器命中) - (回避 + 直感 + 運動性)
            var b = Setup();
            Assert.AreEqual(120, b.W.HitProbability(b.Target, true));
        }

        [TestMethod]
        public void HitProbability_TerrainHitMod_NotForAirTarget()
        {
            // 地形の回避修正 25% → 0.75 倍。空中の相手には地形修正なし
            var b = Setup();
            SetTerrain(b.Src, 2, 1, "森", "陸", hitMod: 25);
            Assert.AreEqual(90, b.W.HitProbability(b.Target, true));
            b.Target.Area = "空中";
            Assert.AreEqual(120, b.W.HitProbability(b.Target, true));
        }

        [TestMethod]
        public void HitProbability_TargetSize()
        {
            // サイズ補正 L=1.2 / SS=0.5
            Assert.AreEqual(144, Setup(configureTarget: ud => ud.Size = "L").Let(b => b.W.HitProbability(b.Target, true)));
            Assert.AreEqual(60, Setup(configureTarget: ud => ud.Size = "SS").Let(b => b.W.HitProbability(b.Target, true)));
        }

        [TestMethod]
        public void HitProbability_ScatterBonusByDistance()
        {
            // 散属性: 距離 3 で +10
            var b = Setup(wclass: "射散", tx: 4, ty: 1);
            Assert.AreEqual(130, b.W.HitProbability(b.Target, true));
        }

        [TestMethod]
        public void HitProbability_DistanceOption()
        {
            // 距離修正: 通常マップで距離 4 は 0.9 倍。追尾系（追）は対象外
            var b = Setup(tx: 5, ty: 1, configureSrc: s => s.Expression.DefineGlobalVariable("Option(距離修正)"));
            Assert.AreEqual(108, b.W.HitProbability(b.Target, true));
            var b2 = Setup(wclass: "射追", tx: 5, ty: 1, configureSrc: s => s.Expression.DefineGlobalVariable("Option(距離修正)"));
            Assert.AreEqual(120, b2.W.HitProbability(b2.Target, true));
        }

        [TestMethod]
        public void HitProbability_ImmobileTarget_AlwaysHits()
        {
            var b = Setup();
            b.Target.AddCondition("麻痺", 1);
            Assert.AreEqual(1000, b.W.HitProbability(b.Target, true));
        }

        [TestMethod]
        public void HitProbability_AttackerStatus_HalvesExceptHoming()
        {
            // 撹乱状態では命中率半減。ホーミング（Ｈ）は影響を受けない
            var b = Setup();
            b.Attacker.AddCondition("撹乱", 1);
            Assert.AreEqual(60, b.W.HitProbability(b.Target, true));
            var bh = Setup(wclass: "射Ｈ");
            bh.Attacker.AddCondition("撹乱", 1);
            Assert.AreEqual(120, bh.W.HitProbability(bh.Target, true));
        }

        [TestMethod]
        public void HitProbability_BlindTarget_Increases()
        {
            var b = Setup();
            b.Target.AddCondition("盲目", 1);
            Assert.AreEqual(180, b.W.HitProbability(b.Target, true));
        }

        [TestMethod]
        public void HitProbability_JumpAttackFromGround()
        {
            // 地上から空中の相手への格闘攻撃は (空中適応 + 6) / 10 倍（空中適応Ｃ=2 → 0.8）
            var b = Setup(wclass: "武", maxRange: 1, targetArea: "空中", configureAttacker: ud => ud.Adaption = "CAAA");
            Assert.AreEqual(96, b.W.HitProbability(b.Target, true));
        }

        [TestMethod]
        public void HitProbability_AltitudeOption()
        {
            // 高度修正: 地上から空中への射撃は 0.7 倍。空属性は対象外
            Action<SRC> opt = s => s.Expression.DefineGlobalVariable("Option(高度修正)");
            var b = Setup(targetArea: "空中", configureSrc: opt);
            Assert.AreEqual(84, b.W.HitProbability(b.Target, true));
            var b2 = Setup(wclass: "射空", targetArea: "空中", configureSrc: opt);
            Assert.AreEqual(120, b2.W.HitProbability(b2.Target, true));
        }

        [TestMethod]
        public void HitProbability_AttackerStealth_Bonus()
        {
            // 不意打ち: 攻撃側がステルスなら +20
            var b = Setup(configureAttacker: ud => ud.AddFeature("ステルス"));
            Assert.AreEqual(140, b.W.HitProbability(b.Target, true));
        }

        [TestMethod]
        public void HitProbability_Guts_AdjustsAtLowHp()
        {
            // 底力: ＨＰ 1/4 以下で攻撃側 +30、防御側 -30
            var b = Setup(configureAttackerPilot: pd => pd.AddSkill("底力", 1d, "", 1));
            Assert.AreEqual(120, b.W.HitProbability(b.Target, true));
            b.Attacker.HP = b.Attacker.MaxHP / 4;
            Assert.AreEqual(150, b.W.HitProbability(b.Target, true));

            var b2 = Setup(configureTargetPilot: pd => pd.AddSkill("底力", 1d, "", 1));
            b2.Target.HP = b2.Target.MaxHP / 4;
            Assert.AreEqual(90, b2.W.HitProbability(b2.Target, true));
        }

        [TestMethod]
        public void HitProbability_AttackEvasion_ReducesByLevel()
        {
            // 攻撃回避Lv3: 命中率 × (10 - 3) / 10。必要気力に満たなければ発動しない
            var b = Setup(configureTarget: ud => ud.AddFeature("攻撃回避Lv3=ブリンク 全"));
            Assert.AreEqual(84, b.W.HitProbability(b.Target, true));
            var b2 = Setup(configureTarget: ud => ud.AddFeature("攻撃回避Lv3=ブリンク 全 110"));
            Assert.AreEqual(120, b2.W.HitProbability(b2.Target, true));
        }

        [TestMethod]
        public void HitProbability_NeverNegative()
        {
            var b = Setup();
            b.Target.MainPilot().Dodge = 500;
            Assert.AreEqual(0, b.W.HitProbability(b.Target, true));
        }

        // ══════════════════════════════════════════════
        // Damage
        // ══════════════════════════════════════════════

        [TestMethod]
        public void Damage_Basic_PowerMinusArmor()
        {
            var b = Setup();
            Assert.AreEqual(700, b.W.Damage(b.Target, true));
        }

        [TestMethod]
        public void Damage_TargetAdaption_ScalesArmor()
        {
            // 防御側の地形適応で装甲が変わる（Ａ → 1.2 倍 = 600）
            var b = Setup(configureTarget: ud => ud.Adaption = "AAAA");
            Assert.AreEqual(600, b.W.Damage(b.Target, true));
        }

        [TestMethod]
        public void Damage_TerrainDamageMod_NotForAirTarget()
        {
            // 地形のダメージ修正 20% → 0.8 倍。空中の相手には適用しない
            var b = Setup();
            SetTerrain(b.Src, 2, 1, "森", "陸", damageMod: 20);
            Assert.AreEqual(560, b.W.Damage(b.Target, true));
            b.Target.Area = "空中";
            Assert.AreEqual(700, b.W.Damage(b.Target, true));
        }

        [TestMethod]
        public void Damage_Pierce_ReducesArmor()
        {
            // 貫: 装甲半減、貫Lv3: 装甲 × 7 / 10
            Assert.AreEqual(950, Setup(wclass: "射貫").Let(b => b.W.Damage(b.Target, true)));
            Assert.AreEqual(850, Setup(wclass: "射貫L3").Let(b => b.W.Damage(b.Target, true)));
        }

        [TestMethod]
        public void Damage_Fixed_IgnoresArmor()
        {
            // 固: 装甲を無視し、地形適応Ａで攻撃力そのもの
            var b = Setup(wclass: "射固");
            Assert.AreEqual(1000, b.W.Damage(b.Target, true));
        }

        [TestMethod]
        public void Damage_MinimumDamage_AndOptions()
        {
            // 最低ダメージは 10。ダメージ下限解除なら 0、ダメージ下限１なら 1
            var b = Setup(configureTarget: ud => ud.Armor = 5000);
            Assert.AreEqual(10, b.W.Damage(b.Target, true));
            b.Src.Expression.DefineGlobalVariable("Option(ダメージ下限１)");
            Assert.AreEqual(1, b.W.Damage(b.Target, true));
            b.Src.Expression.DefineGlobalVariable("Option(ダメージ下限解除)");
            Assert.AreEqual(0, b.W.Damage(b.Target, true));
        }

        [TestMethod]
        public void Damage_ZeroPowerWeapon_IsZero()
        {
            var src = CreateSrc();
            var a = CreateUnit(src, configure: ud => AddWeapon(ud, "ネット", power: 0, wclass: "射", maxRange: 3));
            var t = CreateUnit(src, party: "敵", x: 2, y: 1);
            Assert.AreEqual(0, Weapon(a, "ネット").Damage(t, true));
        }

        [TestMethod]
        public void Damage_UndergroundTarget_IsZero()
        {
            var b = Setup(targetArea: "地中");
            Assert.AreEqual(0, b.W.Damage(b.Target, true));
        }

        [TestMethod]
        public void Damage_SleepingTarget_OneAndHalf()
        {
            var b = Setup();
            b.Target.AddCondition("睡眠", 1);
            Assert.AreEqual(1050, b.W.Damage(b.Target, true));
        }

        [TestMethod]
        public void Damage_SexLimited()
        {
            // ♂属性は男性にのみ、♀属性は女性にのみ効く
            Assert.AreEqual(0, Setup(wclass: "射♂", configureTargetPilot: pd => pd.Sex = "女性").Let(b => b.W.Damage(b.Target, true)));
            Assert.AreEqual(700, Setup(wclass: "射♂", configureTargetPilot: pd => pd.Sex = "男性").Let(b => b.W.Damage(b.Target, true)));
            Assert.AreEqual(0, Setup(wclass: "射♀", configureTargetPilot: pd => pd.Sex = "男性").Let(b => b.W.Damage(b.Target, true)));
        }

        [TestMethod]
        public void Damage_MentalAttack_NoEffectOnMachine()
        {
            // 精神攻撃（精）は性格が機械のパイロットには効かない
            Assert.AreEqual(0, Setup(wclass: "射精", configureTargetPilot: pd => pd.Personality = "機械").Let(b => b.W.Damage(b.Target, true)));
            Assert.AreEqual(700, Setup(wclass: "射精", configureTargetPilot: pd => pd.Personality = "普通").Let(b => b.W.Damage(b.Target, true)));
        }

        [TestMethod]
        public void Damage_LevelLimited()
        {
            // 対Lv2: レベルが 2 の倍数の相手にのみ効く（相手はレベル 1）
            Assert.AreEqual(0, Setup(wclass: "射対L2").Let(b => b.W.Damage(b.Target, true)));
            Assert.AreEqual(700, Setup(wclass: "射対L1").Let(b => b.W.Damage(b.Target, true)));
        }

        [TestMethod]
        public void Damage_AttackerConditions()
        {
            // 攻撃力ＵＰ: 1.25 倍、恐怖: 0.8 倍
            var b = Setup();
            b.Attacker.AddCondition("攻撃力ＵＰ", 1);
            Assert.AreEqual(875, b.W.Damage(b.Target, true));
            var b2 = Setup();
            b2.Attacker.AddCondition("恐怖", 1);
            Assert.AreEqual(560, b2.W.Damage(b2.Target, true));
        }

        [TestMethod]
        public void Damage_Tenacity_RaisesArmorAtLowHp()
        {
            // 不屈: ＨＰ 1/2 以下で装甲 1.1 倍、1/4 以下で 1.2 倍
            var b = Setup(configureTargetPilot: pd => pd.AddSkill("不屈", 1d, "", 1));
            Assert.AreEqual(700, b.W.Damage(b.Target, true));
            b.Target.HP = b.Target.MaxHP / 2;
            Assert.AreEqual(650, b.W.Damage(b.Target, true));
            b.Target.HP = b.Target.MaxHP / 4;
            Assert.AreEqual(600, b.W.Damage(b.Target, true));
        }

        [TestMethod]
        public void Damage_ArmorFeature_AddsArmor()
        {
            // アーマーLv2: 装甲 + 200
            var b = Setup(configureTarget: ud => ud.AddFeature("アーマーLv2=追加装甲 全"));
            Assert.AreEqual(500, b.W.Damage(b.Target, true));
        }

        [TestMethod]
        public void Damage_ResistFeature_ReducesByPercent()
        {
            // レジストLv3: ダメージ × (100 - 30) / 100
            var b = Setup(configureTarget: ud => ud.AddFeature("レジストLv3=防護 全"));
            Assert.AreEqual(490, b.W.Damage(b.Target, true));
        }

        [TestMethod]
        public void Damage_ResistanceAttribute_Halves()
        {
            // 耐性=火 を持つ相手への火属性攻撃はダメージ半減
            var b = Setup(wclass: "射火", configureTarget: ud => ud.AddFeature("耐性=火"));
            Assert.AreEqual(350, b.W.Damage(b.Target, true));
        }

        // ══════════════════════════════════════════════
        // CriticalProbability
        // ══════════════════════════════════════════════

        [TestMethod]
        public void CriticalProbability_NormalWeapon()
        {
            // 通常武器: (武器ＣＴ率 + 技量) - 相手の技量。防御時は半減
            var b = Setup();
            Assert.AreEqual(40, b.W.CriticalProbability(b.Target));
            Assert.AreEqual(20, b.W.CriticalProbability(b.Target, "防御"));
        }

        [TestMethod]
        public void CriticalProbability_SpecialEffectWeapon_UsesHalfTechnique()
        {
            // 特殊効果武器: (武器ＣＴ率 + 技量 / 2) - 相手の技量 / 2 - 10 × 抵抗力Lv
            Assert.AreEqual(25, Setup(wclass: "射Ｓ").Let(b => b.W.CriticalProbability(b.Target)));
            Assert.AreEqual(5, Setup(wclass: "射Ｓ", configureTarget: ud => ud.AddFeature("抵抗力Lv2")).Let(b => b.W.CriticalProbability(b.Target)));
        }

        [TestMethod]
        public void CriticalProbability_ClampedTo1And100()
        {
            var b = Setup();
            b.Target.MainPilot().Technique = 500;
            Assert.AreEqual(1, b.W.CriticalProbability(b.Target));
            b.Target.MainPilot().Technique = 0;
            b.Attacker.MainPilot().Technique = 500;
            Assert.AreEqual(100, b.W.CriticalProbability(b.Target));
        }

        [TestMethod]
        public void CriticalProbability_ImmobileTarget_Plus10()
        {
            var b = Setup();
            b.Target.AddCondition("睡眠", 1);
            Assert.AreEqual(50, b.W.CriticalProbability(b.Target));
        }

        [TestMethod]
        public void CriticalProbability_GutsAtLowHp_Plus50()
        {
            // 底力・超底力・覚悟: ＨＰ 1/4 以下でクリティカル率 +50
            var b = Setup(configureAttackerPilot: pd => pd.AddSkill("覚悟", 1d, "", 1));
            b.Attacker.HP = b.Attacker.MaxHP / 4;
            Assert.AreEqual(90, b.W.CriticalProbability(b.Target));
        }

        [TestMethod]
        public void CriticalProbability_SpecialEffect_VisualAgainstBlind_IsZero()
        {
            // 視覚攻撃（視）の特殊効果は盲目の相手に効かない
            var b = Setup(wclass: "射Ｓ視");
            b.Target.AddCondition("盲目", 1);
            Assert.AreEqual(0, b.W.CriticalProbability(b.Target));
        }
    }

    internal static class UnitWeaponCovLetExtension
    {
        public static TResult Let<T, TResult>(this T self, Func<T, TResult> f) => f(self);
    }
}
