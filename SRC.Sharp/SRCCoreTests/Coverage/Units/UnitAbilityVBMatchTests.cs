using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// アビリティの消費ＥＮ・属性のレベル・適用の判定が、元の SRC（Unit.cls の
    /// AbilityENConsumption・AbilityLevel・IsAbilityApplicable）と同じになることのテスト（7474/SRC#1502）。
    /// </summary>
    [TestClass]
    public class UnitAbilityVBMatchTests
    {
        private UnitAbilityCoverageFixture fx;

        [TestInitialize]
        public void Setup()
        {
            fx = new UnitAbilityCoverageFixture();
        }

        private Unit CreateUser(string aclass = "", int en = 0, string feature = null)
        {
            var ud = fx.AddUnitData("使用機");
            if (feature != null)
            {
                ud.AddFeature(feature);
            }
            var ad = ud.AddAbility("テストアビリティ");
            ad.SetEffect("回復Lv1");
            ad.Class = aclass;
            ad.MinRange = 1;
            ad.MaxRange = 1;
            ad.ENConsumption = en;
            return fx.CreateUnit("使用機", "使用者");
        }

        // ===== AbilityENConsumption =====

        [TestMethod]
        public void ENConsumption_ENSaveFeatureWithoutLevel_CountsAsLevel1()
        {
            // VB: rate = rate - 0.1 * FeatureLevel(i)。FeatureLevel はレベル指定が無ければ 1
            // 50 * 0.9 = 45
            var u = CreateUser(en: 50, feature: "ＥＮ消費減少");
            Assert.AreEqual(45, u.Ability(1).AbilityENConsumption());
        }

        // ===== AbilityLevel =====

        [TestMethod]
        public void AbilityLevel_InvalidLevel_ShowsErrorWithUnitNameAndReturnsZero()
        {
            // VB: CDbl が失敗すると ErrorHandler で
            // ErrorMessage Name & "の" & "アビリティ「" & Ability(a).Name & "」の" & "属性「" & attr & "」のレベル指定が不正です"
            // （Name はユニットの名前）
            var u = CreateUser(aclass: "霊L");

            Assert.AreEqual(0d, u.Ability(1).AbilityLevel("霊"));
            var messages = fx.Gui.CallsOf(nameof(IGUI.ErrorMessage)).Select(x => (string)x.Args[0]).ToList();
            CollectionAssert.Contains(messages, u.Name + "のアビリティ「テストアビリティ」の属性「霊」のレベル指定が不正です");
        }

        [TestMethod]
        public void AbilityLevel_Decimal_IsRead()
        {
            var u = CreateUser(aclass: "霊L1.5");
            Assert.AreEqual(1.5d, u.Ability(1).AbilityLevel("霊"));
        }

        // ===== IsAbilityApplicable =====

        private Unit CreateTarget(int level)
        {
            fx.AddUnitData("対象機");
            fx.AddPilotData("対象者");
            var t = fx.Src.UList.Add("対象機", 0, "味方");
            var p = fx.Src.PList.Add("対象者", level, "味方");
            t.AddPilot(p);
            p.Unit = t;
            t.Update();
            t.FullRecover();
            return t;
        }

        [TestMethod]
        public void Applicable_LevelLimited_RealLevelRoundsToEvenBeforeMod()
        {
            // VB: If .Level Mod AbilityLevel(a, "対") <> 0 Then Exit Function
            // Mod は 2.5 を偶数丸めで 2 にしてから割る。4 Mod 2 = 0 なので適用できる
            //（実数のまま割ると 4 % 2.5 = 1.5 で適用できなかった）
            var u = CreateUser(aclass: "対L2.5");
            var t = CreateTarget(4);
            Assert.IsTrue(u.Ability(1).IsAbilityApplicable(t));
        }

        [TestMethod]
        public void Applicable_LevelLimited_NotMultiple_IsNotApplicable()
        {
            // 3 Mod 2 = 1 なので適用できない
            var u = CreateUser(aclass: "対L2");
            var t = CreateTarget(3);
            Assert.IsFalse(u.Ability(1).IsAbilityApplicable(t));
        }
    }
}
