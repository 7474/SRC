using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Coverage.Tests.SpecialPower
{
    /// <summary>
    /// SpecialPowerData.Apply で、効果レベルを掛けた実数を整数にするときの丸めが
    /// 元の SRC（SpecialPowerData.cls）と同じになることのテスト。
    /// VB の Long・Integer への代入は偶数への丸め、\ は被除数を丸めてから整数除算する。
    /// </summary>
    [TestClass]
    public class SpecialPowerVBRoundingTests
    {
        [TestMethod]
        public void HPDecrease_RoundsDividendBeforeIntegerDivision()
        {
            // VB: .HP = .HP - .HP * EffectLevel(i) \ 10
            // 39 * 0.5 = 19.5 → 20 \ 10 = 2 → 37（切り捨ててから割ると 1 → 38）
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("脱力", "敵", "即効", "ＨＰ減少Lv0.5");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var enemy = f.AddUnit("敵機", "敵パイロット", "敵", 5, 5, hp: 39);

            sd.Apply(me.MainPilot(), enemy, is_event: true);

            Assert.AreEqual(37, enemy.HP);
        }

        [TestMethod]
        public void ENDecrease_RoundsDividendBeforeIntegerDivision()
        {
            // VB: .EN = .EN - .EN * EffectLevel(i) \ 10
            // 39 * 0.5 = 19.5 → 20 \ 10 = 2 → 37
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("吸収", "敵", "即効", "ＥＮ減少Lv0.5");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var enemy = f.AddUnit("敵機", "敵パイロット", "敵", 5, 5, en: 39);

            sd.Apply(me.MainPilot(), enemy, is_event: true);

            Assert.AreEqual(37, enemy.EN);
        }

        [TestMethod]
        public void MoraleIncrease_RoundsToEven()
        {
            // VB: .IncreaseMorale 10 * EffectLevel(i)（Integer の引数なので丸め）
            // 10 * 0.35 = 3.5 → 4（切り捨てなら 3）
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("気合", "自分", "即効", "気力増加Lv0.35");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            var p = u.MainPilot();
            Assert.AreEqual(100, p.Morale);

            sd.Apply(p, u, is_event: true);

            Assert.AreEqual(104, p.Morale);
        }

        [TestMethod]
        public void SPRecovery_MainPilotOnly_RoundsToEven()
        {
            // VB: .MainPilot.SP = .MainPilot.SP + 10 * EffectLevel(i)（Long の SP への代入なので丸め）
            // 10 * 0.35 = 3.5 → 4
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("補給精神", "自分", "即効", "ＳＰ回復Lv0.35");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            var p = u.MainPilot();
            p.SP = 10;

            sd.Apply(p, u, is_event: true);

            Assert.AreEqual(14, p.SP);
        }
    }
}
