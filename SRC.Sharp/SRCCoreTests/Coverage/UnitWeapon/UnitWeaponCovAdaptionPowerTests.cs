using Microsoft.VisualStudio.TestTools.UnitTesting;
using static SRCCore.Coverage.Tests.UnitWeaponCov.UnitWeaponCovHelper;

namespace SRCCore.Coverage.Tests.UnitWeaponCov
{
    /// <summary>
    /// UnitWeapon.WeaponAdaption / WeaponPower / WeaponMaxRange / Set*Correction のテスト。
    /// 期待値は VB 版 Unit.cls の WeaponAdaption / WeaponPower / WeaponMaxRange に従う。
    /// </summary>
    [TestClass]
    public class UnitWeaponCovAdaptionPowerTests
    {
        // ══════════════════════════════════════════════
        // WeaponAdaption
        // ══════════════════════════════════════════════

        [TestMethod]
        public void WeaponAdaption_UnknownArea_UsesAdaptionA()
        {
            // 地形指定が無い（Case Else）場合は適応Ａ扱い → 1.2
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "銃", wclass: "射", maxRange: 3, adaption: "DDDD"));
            Assert.AreEqual(1.2d, Weapon(u, "銃").WeaponAdaption(""), 1e-9);
        }

        [TestMethod]
        public void WeaponAdaption_Underground_IsZero()
        {
            // 地中の敵には攻撃できない
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "銃", wclass: "射", maxRange: 3));
            Assert.AreEqual(0d, Weapon(u, "銃").WeaponAdaption("地中"));
        }

        [TestMethod]
        public void WeaponAdaption_WeaponHyphen_IsZero()
        {
            // 武器の地形適応が「-」の地形には攻撃できない（空中が 1 文字目）
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "銃", wclass: "射", maxRange: 3, adaption: "-AAA"));
            var w = Weapon(u, "銃");
            Assert.AreEqual(0d, w.WeaponAdaption("空中"));
            Assert.AreEqual(1.2d, w.WeaponAdaption("地上"), 1e-9);
        }

        [TestMethod]
        public void WeaponAdaption_LowerOfWeaponAndUnitIsUsed()
        {
            // 武器側とユニット側の地形適応の低い方を優先する
            // 武器Ｂ(3)×ユニットＡ(4) → 3 → 1.0、武器Ａ(4)×ユニットＣ(2) → 2 → 0.8
            var src = CreateSrc();
            var u1 = CreateUnit(src, x: 1, configure: ud => AddWeapon(ud, "銃", wclass: "射", maxRange: 3, adaption: "BBBB"));
            Assert.AreEqual(1d, Weapon(u1, "銃").WeaponAdaption("地上"), 1e-9);

            var u2 = CreateUnit(src, x: 2, configure: ud =>
            {
                ud.Adaption = "ACAA";
                AddWeapon(ud, "銃", wclass: "射", maxRange: 3, adaption: "SSSS");
            });
            Assert.AreEqual(0.8d, Weapon(u2, "銃").WeaponAdaption("地上"), 1e-9);
        }

        [TestMethod]
        public void WeaponAdaption_ShootingUsesOwnArea_MeleeUsesTargetArea()
        {
            // 格闘戦（武/突/接）以外はユニットがいる地形の適応、格闘戦は相手のいる地形の適応を見る
            // ユニットは空中Ｄ・地上Ａで地上にいる
            var src = CreateSrc();
            var u = CreateUnit(src, area: "地上", configure: ud =>
            {
                ud.Adaption = "DAAA";
                AddWeapon(ud, "銃", wclass: "射", maxRange: 3);
                AddWeapon(ud, "剣", wclass: "武");
            });
            // 射撃: 地上(Ａ) を参照 → 1.2
            Assert.AreEqual(1.2d, Weapon(u, "銃").WeaponAdaption("空中"), 1e-9);
            // 格闘: 空中(Ｄ=1) を参照 → 0.6
            Assert.AreEqual(0.6d, Weapon(u, "剣").WeaponAdaption("空中"), 1e-9);
        }

        [TestMethod]
        public void WeaponAdaption_JumpAttack_AddsJLevel()
        {
            // Ｊ属性の格闘武器は空中の敵に対してユニットの空中適応をレベル分底上げする
            // 空中Ｄ(1) + ＪL1 → 2 → 0.8
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                ud.Adaption = "DAAA";
                AddWeapon(ud, "跳び蹴り", wclass: "武ＪL1");
            });
            Assert.AreEqual(0.8d, Weapon(u, "跳び蹴り").WeaponAdaption("空中"), 1e-9);
        }

        [TestMethod]
        public void WeaponAdaption_AirOnlyUnitMeleeAgainstGround_UsesAirMinusOne()
        {
            // 地上適応が無い（-）ユニットは空中適応を 1 段下げて地上の敵に格闘戦をしかけられる
            // 空中Ａ(4) - 1 → 3 → 1.0
            var src = CreateSrc();
            var u = CreateUnit(src, area: "空中", configure: ud =>
            {
                ud.Adaption = "A-AA";
                ud.Transportation = "空";
                AddWeapon(ud, "爪", wclass: "突");
            });
            Assert.AreEqual(1d, Weapon(u, "爪").WeaponAdaption("地上"), 1e-9);
        }

        [TestMethod]
        public void WeaponAdaption_GroundOnMoonSurface_UsesSpaceAdaption()
        {
            // 月面の地上は宇宙適応(4 文字目)を参照する
            var src = CreateSrc(terrainClass: "月面");
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "銃", wclass: "射", maxRange: 3, adaption: "AAAC"));
            Assert.AreEqual(0.8d, Weapon(u, "銃").WeaponAdaption("地上"), 1e-9);
        }

        [TestMethod]
        public void WeaponAdaption_WaterSurface_UsesWaterOnlyWhenWeaponWaterIsA()
        {
            // 水上の敵には武器の水中適応がＡなら水中、そうでなければ地上の適応を使う
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                AddWeapon(ud, "魚雷", wclass: "射", maxRange: 3, adaption: "-DA-");
                AddWeapon(ud, "機銃", wclass: "射", maxRange: 3, adaption: "-CB-");
            });
            Assert.AreEqual(1.2d, Weapon(u, "魚雷").WeaponAdaption("水上"), 1e-9);
            Assert.AreEqual(0.8d, Weapon(u, "機銃").WeaponAdaption("水上"), 1e-9);
        }

        [TestMethod]
        public void WeaponAdaption_Options_ChangeTable()
        {
            // 地形適応修正緩和: Ａ=1.1、繰り下げ: Ａ=1.0、両方: Ｂ=0.9
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                AddWeapon(ud, "銃", wclass: "射", maxRange: 3);
                AddWeapon(ud, "砲", wclass: "射", maxRange: 3, adaption: "BBBB");
            });
            src.Expression.DefineGlobalVariable("Option(地形適応修正緩和)");
            Assert.AreEqual(1.1d, Weapon(u, "銃").WeaponAdaption("地上"), 1e-9);
            src.Expression.DefineGlobalVariable("Option(地形適応修正繰り下げ)");
            Assert.AreEqual(0.9d, Weapon(u, "砲").WeaponAdaption("地上"), 1e-9);
        }

        [TestMethod]
        public void WeaponAdaption_HitRateOption_UsesWeaponAdaptionOnly()
        {
            // 地形適応命中率修正: ユニット側の適応は攻撃可否にのみ使い、修正値は武器側の適応で決まる
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(地形適応命中率修正)");
            var u = CreateUnit(src, configure: ud =>
            {
                ud.Adaption = "ADAA";
                AddWeapon(ud, "銃", wclass: "射", maxRange: 3, adaption: "AAAA");
            });
            // ユニットの地上適応Ｄでも武器Ａ → 1.2
            Assert.AreEqual(1.2d, Weapon(u, "銃").WeaponAdaption("地上"), 1e-9);
        }

        // ══════════════════════════════════════════════
        // WeaponPower
        // ══════════════════════════════════════════════

        [TestMethod]
        public void WeaponPower_AppliesTerrainAdaption()
        {
            // 射撃 100・気力 100 なら攻撃力は地形適応修正のみ: 1000 × 1.2 = 1200
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                AddWeapon(ud, "銃", power: 1000, wclass: "射", maxRange: 3);
                AddWeapon(ud, "弱い銃", power: 1000, wclass: "射", maxRange: 3, adaption: "CCCC");
            });
            Assert.AreEqual(1200, Weapon(u, "銃").WeaponPower("地上"));
            Assert.AreEqual(800, Weapon(u, "弱い銃").WeaponPower("地上"));
            Assert.AreEqual(0, Weapon(u, "銃").WeaponPower("地中"));
        }

        [TestMethod]
        public void WeaponPower_ShortRangeWithoutClass_UsesInfight()
        {
            // 格/射 の指定が無ければ射程 1 の武器は格闘系として格闘値で補正、射程 2 以上は射撃値
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                AddWeapon(ud, "拳", power: 1000, maxRange: 1);
                AddWeapon(ud, "石", power: 1000, maxRange: 2);
            });
            u.MainPilot().Infight = 150;
            u.MainPilot().Shooting = 80;
            Assert.AreEqual(1500, Weapon(u, "拳").WeaponPower("初期値"));
            Assert.AreEqual(800, Weapon(u, "石").WeaponPower("初期値"));
        }

        [TestMethod]
        public void WeaponPower_Composite_UsesAverageOfInfightAndShooting()
        {
            // 複属性は (格闘 + 射撃) / 200 で補正
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "複合", power: 1000, wclass: "複", maxRange: 2));
            u.MainPilot().Infight = 150;
            u.MainPilot().Shooting = 90;
            Assert.AreEqual(1200, Weapon(u, "複合").WeaponPower("初期値"));
        }

        [TestMethod]
        public void WeaponPower_BodyAttribute_AddsByHpRatio()
        {
            // 体属性: 攻撃力 + 残りＨＰ率 × 100 × レベル
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "体当たり", power: 1000, wclass: "体L2"));
            var w = Weapon(u, "体当たり");
            Assert.AreEqual(1200, w.WeaponPower(""));
            u.HP = u.MaxHP / 2;
            Assert.AreEqual(1100, w.WeaponPower(""));
        }

        [TestMethod]
        public void WeaponPower_ExhaustAttribute_AddsRemainingEn()
        {
            // 尽属性: 消費ＥＮを差し引いた残りＥＮ × レベル を加算（ＥＮ不足時は加算なし）
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "全力", power: 1000, wclass: "尽L2", en: 10));
            var w = Weapon(u, "全力");
            Assert.AreEqual(1000 + (100 - 10) * 2, w.WeaponPower(""));
            u.EN = 5;
            Assert.AreEqual(1000, w.WeaponPower(""));
        }

        [TestMethod]
        public void WeaponPower_FixedDamage_IgnoresPilotAndShiftsAdaption()
        {
            // 固属性: パイロット能力・気力の補正なし。地形適応Ａで攻撃力そのまま（修正値 - 0.2）
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                AddWeapon(ud, "固定", power: 1000, wclass: "固", maxRange: 2);
                AddWeapon(ud, "固定Ｂ", power: 1000, wclass: "固", maxRange: 2, adaption: "BBBB");
            });
            u.MainPilot().Shooting = 150;
            u.MainPilot().Morale = 150;
            Assert.AreEqual(1000, Weapon(u, "固定").WeaponPower(""));
            Assert.AreEqual(1000, Weapon(u, "固定").WeaponPower("地上"));
            // Ｂ: 1.0 - 0.2 = 0.8
            Assert.AreEqual(800, Weapon(u, "固定Ｂ").WeaponPower("地上"));
        }

        [TestMethod]
        public void WeaponPower_FixedDamage_WithLoweredAdaptionOption_KeepsPower()
        {
            // 地形適応修正繰り下げの効果は固属性には適用しない（Ａで攻撃力そのまま）
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(地形適応修正繰り下げ)");
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "固定", power: 1000, wclass: "固", maxRange: 2));
            Assert.AreEqual(1000, Weapon(u, "固定").WeaponPower("地上"));
        }

        [TestMethod]
        public void WeaponPower_SquadUnit_DecreasesWithDamage()
        {
            // 部隊ユニットは (50 + 50 × ＨＰ率) % の攻撃力になる
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                ud.AddFeature("部隊ユニット");
                AddWeapon(ud, "銃", power: 1000, wclass: "射", maxRange: 3);
            });
            var w = Weapon(u, "銃");
            Assert.AreEqual(1000, w.WeaponPower(""));
            u.HP = u.MaxHP / 2;
            Assert.AreEqual(750, w.WeaponPower(""));
        }

        [TestMethod]
        [Ignore("移植の不具合: 部隊ユニットの攻撃力低下を VB の \\ 演算（被除数を丸めてから整数除算）ではなく除算後の切り捨てで計算している")]
        public void WeaponPower_SquadUnit_RoundsBeforeIntegerDivision()
        {
            // VB: WeaponPower * (50 + 50 * HP / MaxHP) \ 100
            // 102 × (50 + 50 × 98 / 1000) = 5599.8 → CLng 5600 \ 100 = 56
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                ud.AddFeature("部隊ユニット");
                AddWeapon(ud, "銃", power: 102, wclass: "射", maxRange: 3);
            });
            Assert.AreEqual(1000, u.MaxHP);
            u.HP = 98;
            Assert.AreEqual(56, Weapon(u, "銃").WeaponPower(""));
        }

        [TestMethod]
        public void WeaponPower_Resolve_BoostsAtLowHp()
        {
            // 覚悟: ＨＰが最大値の 1/4 以下で攻撃力 1.2 倍（ダメージ倍率低下なら 1.1 倍）
            var src = CreateSrc();
            var u = CreateUnit(src,
                configure: ud => AddWeapon(ud, "銃", power: 1000, wclass: "射", maxRange: 3),
                configurePilot: pd => pd.AddSkill("覚悟", 1d, "", 1));
            var w = Weapon(u, "銃");
            Assert.AreEqual(1000, w.WeaponPower("初期値"));
            u.HP = u.MaxHP / 4;
            Assert.AreEqual(1200, w.WeaponPower("初期値"));
            src.Expression.DefineGlobalVariable("Option(ダメージ倍率低下)");
            Assert.AreEqual(1100, w.WeaponPower("初期値"));
        }

        [TestMethod]
        public void WeaponPower_MapAttack_UsesSavedPower()
        {
            // マップ攻撃は攻撃開始時に保存した攻撃力に地形適応のみを掛ける
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "マップ兵器", power: 1000, wclass: "Ｍ全", maxRange: 3));
            var w = Weapon(u, "マップ兵器");
            u.SelectedMapAttackPower = 1500;
            Assert.AreEqual(1800, w.WeaponPower("地上"));
            // 初期値（攻撃開始時の算出）では保存値を使わない
            Assert.AreEqual(1000, w.WeaponPower("初期値"));
        }

        [TestMethod]
        public void SetPowerCorrection_ClampsToRange()
        {
            // 攻撃力の補正は 1 ～ 99999 に収め、元の攻撃力が 0 の武器は 0 のまま
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                AddWeapon(ud, "銃", power: 1000, wclass: "射", maxRange: 3);
                AddWeapon(ud, "大砲", power: 99000, wclass: "射", maxRange: 3);
                AddWeapon(ud, "ネット", power: 0, wclass: "射", maxRange: 3);
            });
            Weapon(u, "銃").SetPowerCorrection(-5000);
            Weapon(u, "大砲").SetPowerCorrection(5000);
            Weapon(u, "ネット").SetPowerCorrection(500);
            Assert.AreEqual(1, Weapon(u, "銃").WeaponPower(""));
            Assert.AreEqual(99999, Weapon(u, "大砲").WeaponPower(""));
            Assert.AreEqual(0, Weapon(u, "ネット").WeaponPower(""));
        }

        // ══════════════════════════════════════════════
        // WeaponMaxRange / SetMaxRangeCorrection
        // ══════════════════════════════════════════════

        [TestMethod]
        public void SetMaxRangeCorrection_NotBelowMinRange()
        {
            // 最大射程の補正は最小射程を下回らない
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "ライフル", wclass: "射", minRange: 2, maxRange: 4));
            var w = Weapon(u, "ライフル");
            w.SetMaxRangeCorrection(2);
            Assert.AreEqual(6, w.WeaponMaxRange());
            w.SetMaxRangeCorrection(-5);
            Assert.AreEqual(2, w.WeaponMaxRange());
        }
    }
}
