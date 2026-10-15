using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.Units;
using System;
using static SRCCore.Coverage.Tests.UnitWeaponCov.UnitWeaponCovHelper;

namespace SRCCore.Coverage.Tests.UnitWeaponCov
{
    /// <summary>
    /// UnitWeapon.ExpDamage（AI が武器を選ぶためのダメージ期待値）のテスト。
    /// 期待値は VB 版 Unit.cls の ExpDamage に従う。
    /// 基本の組み合わせでは Damage = 1200 - 500 = 700（UnitWeaponCovCombatTests と同じ）。
    /// バリア等で無効化される場合は「武器番号」をダミーの期待値として返す。
    /// </summary>
    [TestClass]
    public class UnitWeaponCovExpDamageTests
    {
        private static (SRC src, Unit t, UnitWeapon w) Setup(string wclass = "射", int maxRange = 4,
            Action<UnitData> configureTarget = null, Action<PilotData> configureTargetPilot = null)
        {
            var src = CreateSrc();
            src.Stage = "味方";
            var a = CreateUnit(src, x: 1, y: 1, configure: ud => AddWeapon(ud, "武器", power: 1000, wclass: wclass, maxRange: maxRange));
            var t = CreateUnit(src, party: "敵", x: 2, y: 1, configure: ud =>
            {
                ud.Armor = 500;
                ud.Adaption = "BBBB";
                configureTarget?.Invoke(ud);
            }, configurePilot: configureTargetPilot);
            return (src, t, Weapon(a, "武器"));
        }

        [TestMethod]
        public void ExpDamage_EqualsDamage_WithoutDefenseFeatures()
        {
            var (_, t, w) = Setup();
            Assert.AreEqual(700, w.ExpDamage(t, true));
            // ダメージ修正倍率
            Assert.AreEqual(1050, w.ExpDamage(t, true, 1.5d));
        }

        [TestMethod]
        public void ExpDamage_Annihilation_OnlyWhenLethal()
        {
            // 殺属性: 一撃で倒せない限り期待値 0
            var (_, t, w) = Setup(wclass: "射殺");
            Assert.AreEqual(0, w.ExpDamage(t, true));
            t.HP = 700;
            Assert.AreEqual(700, w.ExpDamage(t, true));
        }

        [TestMethod]
        public void ExpDamage_NoDamage_ReturnsWeaponNoUnlessHopeless()
        {
            // ダメージを与えられなくても、性別限定などで効かないのでなければ武器番号をダミー値とする
            var (src, t, w) = Setup(configureTarget: ud => ud.Armor = 5000);
            src.Expression.DefineGlobalVariable("Option(ダメージ下限解除)");
            Assert.AreEqual(w.WeaponNo(), w.ExpDamage(t, true));

            var (_, t2, w2) = Setup(wclass: "射♂", configureTargetPilot: pd => pd.Sex = "女性");
            Assert.AreEqual(0, w2.ExpDamage(t2, true));
        }

        [TestMethod]
        public void ExpDamage_Barrier_BlocksDamageUpTo1000PerLevel()
        {
            // バリアLv1: 1000 以下のダメージを無効化 → ダミー値（武器番号）
            var (_, t, w) = Setup(configureTarget: ud => ud.AddFeature("バリアLv1=バリア 全"));
            Assert.AreEqual(w.WeaponNo(), w.ExpDamage(t, true));
            // 発動に必要なＥＮ（省略時 10）が無ければ発動しない
            t.EN = 9;
            Assert.AreEqual(700, w.ExpDamage(t, true));
        }

        [TestMethod]
        public void ExpDamage_Barrier_IgnoredByBarrierPiercing()
        {
            // 無属性（バリア無効化）の攻撃はバリアを考慮しない
            var (_, t, w) = Setup(wclass: "射無", configureTarget: ud => ud.AddFeature("バリアLv1=バリア 全"));
            Assert.AreEqual(700, w.ExpDamage(t, true));
        }

        [TestMethod]
        public void ExpDamage_Barrier_NeutralizedByCondition()
        {
            // バリア無効化状態の相手のバリアは発動しない
            var (_, t, w) = Setup(configureTarget: ud => ud.AddFeature("バリアLv1=バリア 全"));
            t.AddCondition("バリア無効化", 1);
            Assert.AreEqual(700, w.ExpDamage(t, true));
        }

        [TestMethod]
        public void ExpDamage_Barrier_MeleeInvalidOption()
        {
            // 近接無効オプション付きのバリアは格闘戦（武/突/接）には発動しない
            var (_, t, w) = Setup(wclass: "武", maxRange: 1, configureTarget: ud => ud.AddFeature("バリアLv1=バリア 全 10 0 近接無効"));
            Assert.AreEqual(700, w.ExpDamage(t, true));
        }

        [TestMethod]
        public void ExpDamage_Field_ReducesBy500PerLevel()
        {
            // フィールドLv1: 500 以下なら無効化、超えれば 500 減
            var (_, t, w) = Setup(configureTarget: ud => ud.AddFeature("フィールドLv1=フィールド 全"));
            Assert.AreEqual(200, w.ExpDamage(t, true));
            var (_, t2, w2) = Setup(configureTarget: ud => ud.AddFeature("フィールドLv2=フィールド 全"));
            Assert.AreEqual(w2.WeaponNo(), w2.ExpDamage(t2, true));
        }

        [TestMethod]
        public void ExpDamage_Protection_ReducesByPercent()
        {
            // プロテクションLv2: ダメージ × (100 - 20) / 100
            var (_, t, w) = Setup(configureTarget: ud => ud.AddFeature("プロテクションLv2=プロテクション 全"));
            Assert.AreEqual(560, w.ExpDamage(t, true));
        }

        [TestMethod]
        public void ExpDamage_BeamAbsorb_OnlyForBeam()
        {
            // ビーム吸収はＢ属性の攻撃を無効化する
            var (_, t, w) = Setup(wclass: "射Ｂ", configureTarget: ud => ud.AddFeature("ビーム吸収"));
            Assert.AreEqual(w.WeaponNo(), w.ExpDamage(t, true));
            var (_, t2, w2) = Setup(wclass: "射", configureTarget: ud => ud.AddFeature("ビーム吸収"));
            Assert.AreEqual(700, w2.ExpDamage(t2, true));
        }
    }
}
