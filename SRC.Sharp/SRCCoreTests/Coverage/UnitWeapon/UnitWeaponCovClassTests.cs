using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using static SRCCore.Coverage.Tests.UnitWeaponCov.UnitWeaponCovHelper;

namespace SRCCore.Coverage.Tests.UnitWeaponCov
{
    /// <summary>
    /// UnitWeapon の属性判定（IsWeaponClassifiedAs / WeaponLevel / IsNormalWeapon / CountWeaponEffect /
    /// IsMatchFeatureTarget）と消費ＥＮ（WeaponENConsumption）のテスト。
    /// 期待値は VB 版 Unit.cls の同名関数と特殊能力のヘルプに従う。
    /// </summary>
    [TestClass]
    public class UnitWeaponCovClassTests
    {
        private static Units.UnitWeapon SingleWeapon(string wclass, int maxRange = 1, int en = 0,
            System.Action<Models.UnitData> configure = null,
            System.Action<Models.PilotData> configurePilot = null)
        {
            var src = CreateSrc();
            var u = CreateUnit(src,
                configure: ud =>
                {
                    configure?.Invoke(ud);
                    AddWeapon(ud, "武器", wclass: wclass, maxRange: maxRange, en: en);
                },
                configurePilot: configurePilot);
            return Weapon(u, "武器");
        }

        // ══════════════════════════════════════════════
        // IsWeaponClassifiedAs
        // ══════════════════════════════════════════════

        [TestMethod]
        public void IsWeaponClassifiedAs_MeleeOrShooting_ByClassThenRange()
        {
            // 格闘系/射撃系: 格・射の指定を優先し、無ければ射程 1 を格闘系とみなす
            Assert.IsTrue(SingleWeapon("格", maxRange: 3).IsWeaponClassifiedAs("格闘系"));
            Assert.IsFalse(SingleWeapon("格", maxRange: 3).IsWeaponClassifiedAs("射撃系"));
            Assert.IsFalse(SingleWeapon("射", maxRange: 1).IsWeaponClassifiedAs("格闘系"));
            Assert.IsTrue(SingleWeapon("射", maxRange: 1).IsWeaponClassifiedAs("射撃系"));
            Assert.IsTrue(SingleWeapon("", maxRange: 1).IsWeaponClassifiedAs("格闘系"));
            Assert.IsFalse(SingleWeapon("", maxRange: 1).IsWeaponClassifiedAs("射撃系"));
            Assert.IsFalse(SingleWeapon("", maxRange: 2).IsWeaponClassifiedAs("格闘系"));
            Assert.IsTrue(SingleWeapon("", maxRange: 2).IsWeaponClassifiedAs("射撃系"));
        }

        [TestMethod]
        public void IsWeaponClassifiedAs_UsableAfterMove()
        {
            // 移動後攻撃可: 射程 1 はＱが無ければ可、射程 2 以上はＰがあれば可
            Assert.IsTrue(SingleWeapon("", maxRange: 1).IsWeaponClassifiedAs("移動後攻撃可"));
            Assert.IsFalse(SingleWeapon("Ｑ", maxRange: 1).IsWeaponClassifiedAs("移動後攻撃可"));
            Assert.IsTrue(SingleWeapon("Ｐ", maxRange: 3).IsWeaponClassifiedAs("移動後攻撃可"));
            Assert.IsFalse(SingleWeapon("", maxRange: 3).IsWeaponClassifiedAs("移動後攻撃可"));
        }

        [TestMethod]
        public void IsWeaponClassifiedAs_IgnoresAttributeNestedInWeakness()
        {
            // 弱/効/剋 の後ろの属性は武器自身の属性ではない
            var w = SingleWeapon("弱火格");
            Assert.IsFalse(w.IsWeaponClassifiedAs("火"));
            Assert.IsTrue(w.IsWeaponClassifiedAs("弱火"));
            Assert.IsTrue(w.IsWeaponClassifiedAs("格"));
        }

        // ══════════════════════════════════════════════
        // WeaponLevel / IsWeaponLevelSpecified
        // ══════════════════════════════════════════════

        [TestMethod]
        public void WeaponLevel_ParsesLevelAtEnd()
        {
            var w = SingleWeapon("格ＡL3");
            Assert.AreEqual(3d, w.WeaponLevel("Ａ"));
            Assert.IsTrue(w.IsWeaponLevelSpecified("Ａ"));
            // 指定が無ければ 0
            Assert.AreEqual(0d, w.WeaponLevel("体"));
            Assert.IsFalse(w.IsWeaponLevelSpecified("体"));
            Assert.AreEqual(1.5d, SingleWeapon("体L1.5").WeaponLevel("体"));
        }

        [TestMethod]
        public void WeaponLevel_StopsAtNextAttribute()
        {
            // VB: Select Case Asc(c) の Case Else で Exit Do し、数字部分だけを CDbl する
            var w = SingleWeapon("体L2格");
            Assert.AreEqual(2d, w.WeaponLevel("体"));
        }

        // ══════════════════════════════════════════════
        // IsNormalWeapon / CountWeaponEffect
        // ══════════════════════════════════════════════

        [TestMethod]
        public void IsNormalWeapon_FalseWhenHasSpecialEffect()
        {
            Assert.IsTrue(SingleWeapon("格実").IsNormalWeapon());
            Assert.IsFalse(SingleWeapon("格Ｓ").IsNormalWeapon());
            Assert.IsFalse(SingleWeapon("射毒").IsNormalWeapon());
        }

        [TestMethod]
        public void CountWeaponEffect_CountsEffectAttributesBeforeHiddenPart()
        {
            // Ｓ（ＣＴ時発動）と 貫（それ以外の特殊効果）を数え、格は数えない
            Assert.AreEqual(2, SingleWeapon("格Ｓ貫").CountWeaponEffect());
            // 弱火 はひとまとまりで 1 つ
            Assert.AreEqual(1, SingleWeapon("弱火").CountWeaponEffect());
            // | 以降の非表示部分は数えない
            Assert.AreEqual(1, SingleWeapon("Ｓ|貫").CountWeaponEffect());
        }

        // ══════════════════════════════════════════════
        // IsMatchFeatureTarget
        // ══════════════════════════════════════════════

        [TestMethod]
        public void IsMatchFeatureTarget_EmptyAndAll()
        {
            var w = SingleWeapon("射実", maxRange: 3);
            Assert.IsTrue(w.IsMatchFeatureTarget(new List<string>()));
            Assert.IsTrue(w.IsMatchFeatureTarget(new List<string> { "全" }));
        }

        [TestMethod]
        public void IsMatchFeatureTarget_ByClassNameAndNegation()
        {
            var w = SingleWeapon("射実", maxRange: 3);
            Assert.IsTrue(w.IsMatchFeatureTarget(new List<string> { "実" }));
            Assert.IsTrue(w.IsMatchFeatureTarget(new List<string> { "武器" }));
            Assert.IsFalse(w.IsMatchFeatureTarget(new List<string> { "格" }));
            // ! 指定に該当すれば適用しない
            Assert.IsFalse(w.IsMatchFeatureTarget(new List<string> { "!実" }));
            Assert.IsTrue(w.IsMatchFeatureTarget(new List<string> { "!格" }));
            Assert.IsFalse(w.IsMatchFeatureTarget(new List<string> { "射", "!実" }));
        }

        [TestMethod]
        public void IsMatchFeatureTarget_Physical_ExcludesPureMagic()
        {
            // 物: 魔属性を持たないか、魔武・魔突などの物理扱いの魔属性
            Assert.IsTrue(SingleWeapon("射").IsMatchFeatureTarget(new List<string> { "物" }));
            Assert.IsFalse(SingleWeapon("魔").IsMatchFeatureTarget(new List<string> { "物" }));
            Assert.IsTrue(SingleWeapon("魔武").IsMatchFeatureTarget(new List<string> { "物" }));
        }

        [TestMethod]
        public void IsMatchFeatureTarget_ByNecessarySkill()
        {
            // 必要技能名（Lv 指定を除く）でも一致する
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                var wd = AddWeapon(ud, "念動", wclass: "射", maxRange: 3);
                wd.NecessarySkill = "超能力Lv2";
            });
            Assert.IsTrue(Weapon(u, "念動").IsMatchFeatureTarget(new List<string> { "超能力" }));
        }

        // ══════════════════════════════════════════════
        // WeaponENConsumption
        // ══════════════════════════════════════════════

        [TestMethod]
        public void WeaponENConsumption_SpellSkillReducesCost()
        {
            // 術属性: 術技能レベル 3 で 0.8 倍
            var w = SingleWeapon("術", en: 50, configurePilot: pd => pd.AddSkill("術", 3d, "", 1));
            Assert.IsTrue(w.IsSpellWeapon());
            Assert.AreEqual(40, w.WeaponENConsumption());
        }

        [TestMethod]
        public void WeaponENConsumption_SpellReduction_HasFloorOfFive()
        {
            // 術技能による軽減後も最低 5（ただし元の消費量は超えない）
            Assert.AreEqual(5, SingleWeapon("術", en: 10, configurePilot: pd => pd.AddSkill("術", 10d, "", 1)).WeaponENConsumption());
            Assert.AreEqual(3, SingleWeapon("術", en: 3, configurePilot: pd => pd.AddSkill("術", 10d, "", 1)).WeaponENConsumption());
        }

        [TestMethod]
        public void WeaponENConsumption_FeatSkillReducesCost()
        {
            // 技属性: 技技能レベル 2 で 0.9 倍
            var w = SingleWeapon("技", en: 50, configurePilot: pd => pd.AddSkill("技", 2d, "", 1));
            Assert.IsTrue(w.IsFeatWeapon());
            Assert.IsFalse(w.IsSpellWeapon());
            Assert.AreEqual(45, w.WeaponENConsumption());
        }

        [TestMethod]
        public void WeaponENConsumption_EnSavingFeature()
        {
            // ＥＮ消費減少: レベルごとに 1 割減、下限は 1 割
            Assert.AreEqual(40, SingleWeapon("射", en: 50, configure: ud => ud.AddFeature("ＥＮ消費減少Lv2")).WeaponENConsumption());
            Assert.AreEqual(5, SingleWeapon("射", en: 50, configure: ud =>
            {
                ud.AddFeature("ＥＮ消費減少Lv5");
                ud.AddFeature("ＥＮ消費減少Lv6");
            }).WeaponENConsumption());
        }

        [TestMethod]
        public void WeaponENConsumption_RoundsLikeVbIntegerAssignment()
        {
            // VB: WeaponENConsumption = rate * WeaponENConsumption（Integer への代入で丸め）
            // 11 × 0.9 = 9.9 → 10
            Assert.AreEqual(10, SingleWeapon("射", en: 11, configure: ud => ud.AddFeature("ＥＮ消費減少Lv1")).WeaponENConsumption());
            // 術Lv2: 11 × 0.9 = 9.9 → 10
            Assert.AreEqual(10, SingleWeapon("術", en: 11, configurePilot: pd => pd.AddSkill("術", 2d, "", 1)).WeaponENConsumption());
        }
    }
}
