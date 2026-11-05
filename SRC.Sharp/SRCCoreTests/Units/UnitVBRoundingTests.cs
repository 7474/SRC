using Microsoft.VisualStudio.TestTools.UnitTesting;
using static SRCCore.Coverage.Tests.UnitWeaponCov.UnitWeaponCovHelper;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit.Update などで実数を整数にするときの丸めが VB 版（Unit.cls）と同じかのテスト。
    /// VB は Long・Integer への代入で偶数へ丸め、\ 演算子は割る前に被除数を偶数へ丸める。
    /// </summary>
    [TestClass]
    public class UnitVBRoundingTests
    {
        [TestMethod]
        public void Update_MobilityEnhancement_RoundsHalfToEven()
        {
            // VB: intMobility = intMobility + 5 * .Level → 91 + 2.5 = 93.5 を偶数へ丸めて 94
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                ud.Mobility = 91;
                ud.AddFeature("運動性強化Lv0.5");
            });
            Assert.AreEqual(94, u.get_Mobility(""));
        }

        [TestMethod]
        public void Update_HpRatioEnhancement_RoundsDividendBeforeIntegerDivision()
        {
            // VB: lngMaxHP = lngMaxHP + Data.HP * .Level \ 20
            // 2999 * 0.5 = 1499.5 → 1500 に丸めてから \ 20 → 75
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                ud.HP = 2999;
                ud.AddFeature("ＨＰ割合強化Lv0.5");
            });
            Assert.AreEqual(2999 + 75, u.MaxHP);
        }

        [TestMethod]
        public void Update_WeaponRatioEnhancement_RoundsDividendBeforeIntegerDivision()
        {
            // VB: lngWeaponPower(i) = lngWeaponPower(i) + Weapon(i).Power * FeatureLevel(j) \ 20
            // 2999 * 0.5 = 1499.5 → 1500 に丸めてから \ 20 → 75
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                AddWeapon(ud, "剣", power: 2999);
                ud.AddFeature("武器割合強化Lv0.5");
            });
            Assert.AreEqual(2999 + 75, Weapon(u, "剣").WeaponPower(""));
        }

        [TestMethod]
        public void Update_PrecisionEnhancement_RoundsTotalIncludingBaseValue()
        {
            // VB: intWeaponPrecision(i) = intWeaponPrecision(i) + 5 * FeatureLevel(j)
            // 元の命中率 15 から始めて 15 + 2.5 = 17.5 を偶数へ丸めて 18
            // （補正の 2.5 だけを丸めて 2 にしてから足すと 17 になる）
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                AddWeapon(ud, "銃", wclass: "射", maxRange: 3, precision: 15);
                ud.AddFeature("命中率強化Lv0.5");
            });
            Assert.AreEqual(18, Weapon(u, "銃").WeaponPrecision());
        }

        [TestMethod]
        public void Update_PsychoGuidedRange_DividesEachSkillLevelSeparately()
        {
            // VB: intWeaponMaxRange(i) + .SkillLevel("超感覚") \ 4 + .SkillLevel("知覚強化") \ 4
            // 超感覚Lv3 → 3 \ 4 = 0 なので射程は延びない（3 / 4 + ... を合計してから切り捨てると延びることがある）
            var src = CreateSrc();
            var u = CreateUnit(src,
                configure: ud => AddWeapon(ud, "ファンネル", wclass: "サ", maxRange: 5),
                configurePilot: pd =>
                {
                    pd.AddSkill("超感覚", 3d, "", 1);
                    pd.AddSkill("知覚強化", 3d, "", 1);
                });
            Assert.AreEqual(5, Weapon(u, "ファンネル").WeaponMaxRange());
        }
    }
}
