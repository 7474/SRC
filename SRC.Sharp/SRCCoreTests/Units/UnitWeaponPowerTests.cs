using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// 移植精度検証: UnitWeapon.WeaponPower() のパイロット能力値による補正計算テスト。
    /// VB6 版の数値をそのまま期待値として検証する。
    /// tarea="初期値" を使用して地形補正前の値を検証する。
    /// </summary>
    [TestClass]
    public class UnitWeaponPowerTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        /// <summary>
        /// テスト用のユニット・パイロット・武器を作成する。
        /// </summary>
        private (Unit unit, Pilots.Pilot pilot, UnitWeapon weapon) CreateSetup(
            SRC src,
            int weaponPower,
            string weaponClass = "実",
            int shooting = 100,
            int infight = 100,
            int morale = 100)
        {
            // パイロット作成
            var pdName = "テストパイロット_" + System.Guid.NewGuid().ToString("N");
            var pd = src.PDList.Add(pdName);
            pd.SP = 10;
            var pilot = src.PList.Add(pdName, 1, "味方");
            pilot.Shooting = shooting;
            pilot.Infight = infight;
            pilot.Morale = morale;

            // ユニット作成・パイロット搭乗
            var unit = new Unit(src);
            unit.AddPilot(pilot);
            pilot.Unit = unit;

            // 武器作成
            var wd = new WeaponData(src)
            {
                Name = "テスト武器",
                Power = weaponPower,
                Class = weaponClass,
                MinRange = 1,
                MaxRange = 3,
            };
            var weapon = new UnitWeapon(src, unit, wd);

            return (unit, pilot, weapon);
        }

        // ══════════════════════════════════════════════
        // WeaponPower — 武器一覧表示用 (tarea="")
        // パイロット補正なし、基本攻撃力のみ
        // ══════════════════════════════════════════════

        [TestMethod]
        public void WeaponPower_EmptyTarea_ReturnsBasePower()
        {
            var src = CreateSrc();
            var (_, _, weapon) = CreateSetup(src, weaponPower: 2000);
            // tarea="" → パイロット補正なし → 基本攻撃力そのまま
            Assert.AreEqual(2000, weapon.WeaponPower(""));
        }

        [TestMethod]
        public void WeaponPower_EmptyTarea_ZeroPower_ReturnsZero()
        {
            var src = CreateSrc();
            var (_, _, weapon) = CreateSetup(src, weaponPower: 0);
            Assert.AreEqual(0, weapon.WeaponPower(""));
        }

        // ══════════════════════════════════════════════
        // WeaponPower — 初期値モード (パイロット補正あり、地形補正なし)
        // 射撃系武器 (class="実") → Shooting 補正
        // 公式: Power * Shooting / 100 * Morale / 100
        // ══════════════════════════════════════════════

        [TestMethod]
        public void WeaponPower_Shooting100_Morale100_ReturnsBasePower()
        {
            var src = CreateSrc();
            var (_, _, weapon) = CreateSetup(src, weaponPower: 2000, shooting: 100, morale: 100);
            // 2000 * 100 / 100 * 100 / 100 = 2000
            Assert.AreEqual(2000, weapon.WeaponPower("初期値"));
        }

        [TestMethod]
        public void WeaponPower_Shooting150_Morale100_ReturnsBoostedPower()
        {
            var src = CreateSrc();
            var (_, _, weapon) = CreateSetup(src, weaponPower: 2000, shooting: 150, morale: 100);
            // 2000 * 150 / 100 = 3000; 3000 * 100 / 100 = 3000
            Assert.AreEqual(3000, weapon.WeaponPower("初期値"));
        }

        [TestMethod]
        public void WeaponPower_Shooting100_Morale150_ReturnsMoraleBoostedPower()
        {
            var src = CreateSrc();
            var (_, _, weapon) = CreateSetup(src, weaponPower: 2000, shooting: 100, morale: 150);
            // 2000 * 100 / 100 = 2000; 2000 * 150 / 100 = 3000
            Assert.AreEqual(3000, weapon.WeaponPower("初期値"));
        }

        [TestMethod]
        public void WeaponPower_Shooting150_Morale120_ReturnsCombinedBoost()
        {
            var src = CreateSrc();
            var (_, _, weapon) = CreateSetup(src, weaponPower: 1000, shooting: 150, morale: 120);
            // 1000 * 150 / 100 = 1500; 1500 * 120 / 100 = 1800
            Assert.AreEqual(1800, weapon.WeaponPower("初期値"));
        }

        [TestMethod]
        public void WeaponPower_IntegerTruncation_ShootingModifier()
        {
            var src = CreateSrc();
            // 1500 * 133 / 100 = 1995 (切り捨て, 1500*1.33=1995.0)
            var (_, _, weapon) = CreateSetup(src, weaponPower: 1500, shooting: 133, morale: 100);
            Assert.AreEqual(1995, weapon.WeaponPower("初期値"));
        }

        [TestMethod]
        public void WeaponPower_IntegerTruncation_MoraleModifier()
        {
            var src = CreateSrc();
            // 1000 * 100 / 100 = 1000; 1000 * 133 / 100 = 1330
            var (_, _, weapon) = CreateSetup(src, weaponPower: 1000, shooting: 100, morale: 133);
            Assert.AreEqual(1330, weapon.WeaponPower("初期値"));
        }

        [TestMethod]
        public void WeaponPower_IntegerTruncation_BothModifiers()
        {
            var src = CreateSrc();
            // 999 * 150 / 100 = 1498 (切り捨て); 1498 * 110 / 100 = 1647 (切り捨て)
            var (_, _, weapon) = CreateSetup(src, weaponPower: 999, shooting: 150, morale: 110);
            Assert.AreEqual(1647, weapon.WeaponPower("初期値"));
        }

        // ══════════════════════════════════════════════
        // WeaponPower — 格闘系武器 (class="格") → Infight 補正
        // 公式: Power * Infight / 100 * Morale / 100
        // ══════════════════════════════════════════════

        [TestMethod]
        public void WeaponPower_MeleeWeapon_Infight150_Morale100_ReturnsBoostedPower()
        {
            var src = CreateSrc();
            // 格闘系武器 (class="格") → Infight 使用
            var (_, _, weapon) = CreateSetup(src, weaponPower: 2000, weaponClass: "格", infight: 150, morale: 100);
            // 2000 * 150 / 100 = 3000; 3000 * 100 / 100 = 3000
            Assert.AreEqual(3000, weapon.WeaponPower("初期値"));
        }

        [TestMethod]
        public void WeaponPower_MeleeWeapon_Infight100_Morale100_ReturnsBasePower()
        {
            var src = CreateSrc();
            var (_, _, weapon) = CreateSetup(src, weaponPower: 2000, weaponClass: "格", infight: 100, morale: 100);
            Assert.AreEqual(2000, weapon.WeaponPower("初期値"));
        }

        [TestMethod]
        public void WeaponPower_MeleeWeapon_IntegerTruncation()
        {
            var src = CreateSrc();
            // 1001 * 150 / 100 = 1501 (切り捨て); 1501 * 100 / 100 = 1501
            var (_, _, weapon) = CreateSetup(src, weaponPower: 1001, weaponClass: "格", infight: 150, morale: 100);
            Assert.AreEqual(1501, weapon.WeaponPower("初期値"));
        }

        // ══════════════════════════════════════════════
        // WeaponPower — 気力効果小オプション
        // 公式: WeaponPowerRet * (50 + morale / 2) / 100
        // ══════════════════════════════════════════════

        [TestMethod]
        public void WeaponPower_KiryokuEffectSmall_Morale100_ReturnsCorrectPower()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(気力効果小)");
            var (_, _, weapon) = CreateSetup(src, weaponPower: 2000, shooting: 100, morale: 100);
            // 2000 * 100/100 = 2000; 2000 * (50 + 100/2) / 100 = 2000 * 100/100 = 2000
            Assert.AreEqual(2000, weapon.WeaponPower("初期値"));
        }

        [TestMethod]
        public void WeaponPower_KiryokuEffectSmall_Morale120_ReturnsLessThanNormal()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(気力効果小)");
            var (_, _, weapon) = CreateSetup(src, weaponPower: 2000, shooting: 100, morale: 120);
            // 2000 * 100/100 = 2000; 2000 * (50 + 120/2) / 100 = 2000 * 110/100 = 2200
            // 通常モード: 2000 * 120/100 = 2400 → 気力効果小では 2200
            Assert.AreEqual(2200, weapon.WeaponPower("初期値"));
        }

        [TestMethod]
        public void WeaponPower_KiryokuEffectSmall_Morale150_ReturnsCorrectValue()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(気力効果小)");
            var (_, _, weapon) = CreateSetup(src, weaponPower: 1000, shooting: 100, morale: 150);
            // 1000 * (50 + 150/2) / 100 = 1000 * 125/100 = 1250
            Assert.AreEqual(1250, weapon.WeaponPower("初期値"));
        }
    }
}
