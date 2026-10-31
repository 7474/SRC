using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// 追加パイロットを持つユニットの Unit.MainPilot（#1407）。
    /// VB 版 Unit.cls の MainPilot は、登録済みの追加パイロットがまだこのユニットのものでないときだけ、
    /// ユニット・陣営・経験値・気力・レベルを設定し直す。
    /// </summary>
    [TestClass]
    public class UnitAdditionalPilotTests
    {
        private static UnitAbilityCoverageFixture Prepare(bool withSkillGrant)
        {
            var fx = new UnitAbilityCoverageFixture();
            var ud = fx.AddUnitData("テスト機");
            ud.PilotNum = 1;
            ud.AddFeature("追加パイロット=合体パイロット");
            if (withSkillGrant)
            {
                ud.AddFeature("パイロット能力付加=\"素質=自己進化能力\"");
            }
            fx.AddPilotData("乗るパイロット");
            fx.AddPilotData("合体パイロット");
            return fx;
        }

        [TestMethod]
        public void Ride_UnitWithAdditionalPilotAndSkillGrant_DoesNotRecurse()
        {
            // 元は MainPilot → 気力の設定 → 気力上限の技能 → MainPilot … と止まらず、スタックオーバーフローで落ちていた
            var fx = Prepare(withSkillGrant: true);
            var u = fx.Src.UList.Add("テスト機", 0, "味方");
            var p = fx.Src.PList.Add("乗るパイロット", 1, "味方");

            p.Ride(u);

            var main = u.MainPilot();
            Assert.AreEqual("合体パイロット", main.Name);
            Assert.AreSame(u, main.Unit);
            Assert.IsTrue(main.IsAdditionalPilot);
        }

        [TestMethod]
        public void MainPilot_AdditionalPilotOfThisUnit_KeepsValuesUntilMovedToAnotherUnit()
        {
            // 既にこのユニットの追加パイロットなら、呼ぶたびに設定し直さない（VB 版と同じ）
            var fx = Prepare(withSkillGrant: false);
            var u = fx.Src.UList.Add("テスト機", 0, "味方");
            var p = fx.Src.PList.Add("乗るパイロット", 1, "味方");
            p.Ride(u);
            var main = u.MainPilot();
            Assert.AreEqual("合体パイロット", main.Name);

            p.Morale = 130;
            main.Morale = 110;

            Assert.AreSame(main, u.MainPilot());
            Assert.AreEqual(110, u.MainPilot().Morale);
        }
    }
}
