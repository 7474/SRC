using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using static SRCCore.Coverage.Tests.UnitWeaponCov.UnitWeaponCovHelper;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// VB 版 Unit.cls の武器の手続き（Weapon〜IsTargetReachable）と突き合わせて見つけた違いのテスト（7474/SRC#1499）。
    /// 期待値は元の SRC の動作。
    /// </summary>
    [TestClass]
    public class UnitWeaponVBMatchTests
    {
        // ──────────────────────────────────────────────
        // WeaponMaxRange（Unit.Update の武器射程の更新）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WeaponMaxRange_RangeExtensionBelowZero_MinimumIsOne()
        {
            // VB: If intWeaponMaxRange(i) <= 0 Then intWeaponMaxRange(i) = 1
            // 最低値は 1 で、最小射程ではない
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                AddWeapon(ud, "銃", wclass: "射", minRange: 2, maxRange: 3);
                ud.AddFeature("射程延長Lv-5");
            });
            Assert.AreEqual(1, Weapon(u, "銃").WeaponMaxRange());
        }

        [TestMethod]
        public void WeaponMaxRange_RangeExtensionToMinRange_IsNotRaisedToMinRange()
        {
            // 3 - 2 = 1。最小射程 2 には引き上げない
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                AddWeapon(ud, "銃", wclass: "射", minRange: 2, maxRange: 3);
                ud.AddFeature("射程延長Lv-2");
            });
            Assert.AreEqual(1, Weapon(u, "銃").WeaponMaxRange());
        }

        [TestMethod]
        public void WeaponMaxRange_PsychoGuidedMapAndMeleeWeapons_KeepSkillBonus()
        {
            // VB: 思念誘導攻撃の射程延長を足してから、Ｍ・武/突/接・有 は GoTo NextWeapon2 で抜ける。
            // 足した射程はそのまま残る（超感覚Lv4 \ 4 = 1）
            var src = CreateSrc();
            var u = CreateUnit(src,
                configure: ud =>
                {
                    AddWeapon(ud, "マップ", wclass: "サＭ全", maxRange: 3);
                    AddWeapon(ud, "投擲", wclass: "サ武", maxRange: 2);
                    AddWeapon(ud, "有線", wclass: "サ有", maxRange: 4);
                },
                configurePilot: pd => pd.AddSkill("超感覚", 4d, "", 1));
            Assert.AreEqual(4, Weapon(u, "マップ").WeaponMaxRange());
            Assert.AreEqual(3, Weapon(u, "投擲").WeaponMaxRange());
            Assert.AreEqual(5, Weapon(u, "有線").WeaponMaxRange());
        }

        // ──────────────────────────────────────────────
        // IsWeaponClassifiedAs
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsWeaponClassifiedAs_MeleeOrRanged_UsesDataMaxRange()
        {
            // VB: ElseIf Weapon(w).MaxRange = 1 Then（補正前の武器データの最大射程）
            // 射程延長で最大射程が 1 になっても、データの最大射程が 2 なら射撃系
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                AddWeapon(ud, "銃", wclass: "実", minRange: 1, maxRange: 2);
                ud.AddFeature("射程延長Lv-1");
            });
            var w = Weapon(u, "銃");
            Assert.AreEqual(1, w.WeaponMaxRange());
            Assert.IsFalse(w.IsWeaponClassifiedAs("格闘系"));
            Assert.IsTrue(w.IsWeaponClassifiedAs("射撃系"));
            Assert.IsFalse(w.IsWeaponClassifiedAs("移動後攻撃可"));
        }

        // ──────────────────────────────────────────────
        // WeaponLevel
        // ──────────────────────────────────────────────

        [TestMethod]
        [DoNotParallelize]
        public void WeaponLevel_InvalidLevel_ShowsErrorWithUnitNameAndReturnsZero()
        {
            // VB: CDbl が失敗すると ErrorHandler で
            // Name & "の武装「" & Weapon(w).Name & "」の属性「" & attr & "」のレベル指定が不正です"
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "剣", wclass: "武体L"));
            var w = Weapon(u, "剣");

            var original = Console.Error;
            var sw = new StringWriter();
            Console.SetError(sw);
            double lv;
            try
            {
                lv = w.WeaponLevel("体");
            }
            finally
            {
                Console.SetError(original);
            }
            Assert.AreEqual(0d, lv);
            StringAssert.Contains(sw.ToString(), u.Name + "の武装「剣」の属性「体」のレベル指定が不正です");
        }

        [TestMethod]
        public void WeaponLevel_TrailingDot_IsReadAsNumber()
        {
            // VB の CDbl("2.") は 2
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "剣", wclass: "武体L2."));
            Assert.AreEqual(2d, Weapon(u, "剣").WeaponLevel("体"));
        }

        // ──────────────────────────────────────────────
        // WeaponPower（バトルコンフィグの攻撃補正）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WeaponPower_AttackConfig_SetsAttackerUnit()
        {
            // VB: Set BCVariable.AtkUnit = Me を設定してから「攻撃補正」を計算する
            var src = CreateSrc();
            var u = CreateUnit(src, x: 1, y: 1, configure: ud => AddWeapon(ud, "銃", power: 1000, wclass: "射", maxRange: 3));
            var other = CreateUnit(src, x: 3, y: 3);
            var battle = "攻撃補正\n武器威力 + HP(攻撃側ユニットＩＤ)\n";
            src.BCList.Load("battle.txt", new MemoryStream(Encoding.UTF8.GetBytes(battle)));
            u.HP = 100;
            other.HP = 900;
            src.Event.BCVariable.AtkUnit = other;

            Assert.AreEqual(1100, Weapon(u, "銃").WeaponPower("初期値"));
        }

        // ──────────────────────────────────────────────
        // IsWeaponAvailable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsWeaponAvailable_ConditionNamedOnlyAttributeDisabled_DoesNotDisable()
        {
            // VB: If Len(Condition(i)) > 6 Then（属性の無い「属性使用不能」は対象外）
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "剣", wclass: "武"));
            u.AddCondition("属性使用不能", 1);
            Assert.IsTrue(Weapon(u, "剣").IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_AttributeDisabled_DisablesWeapon()
        {
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                AddWeapon(ud, "剣", wclass: "武");
                AddWeapon(ud, "銃", wclass: "射", maxRange: 3);
            });
            u.AddCondition("武属性使用不能", 1);
            Assert.IsFalse(Weapon(u, "剣").IsWeaponAvailable("移動前"));
            Assert.IsTrue(Weapon(u, "銃").IsWeaponAvailable("移動前"));
        }

        // ──────────────────────────────────────────────
        // IsTargetWithinRange
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsTargetWithinRange_StealthLevelIsRoundedToInteger()
        {
            // VB: lv は Integer。ステルスLv3.5 は偶数丸めで 4 になり、距離 4 の敵は射程内
            var src = CreateSrc();
            var u = CreateUnit(src, x: 1, y: 1, configure: ud => AddWeapon(ud, "銃", wclass: "射", maxRange: 5));
            var t = CreateUnit(src, party: "敵", x: 5, y: 1, configure: ud => ud.AddFeature("ステルスLv3.5"));
            Assert.IsTrue(Weapon(u, "銃").IsTargetWithinRange(t));
        }
    }
}
