using Microsoft.VisualStudio.TestTools.UnitTesting;
using static SRCCore.Coverage.Tests.PilotsCoverage.PilotSkillStatusFixture;

namespace SRCCore.Coverage.Tests.PilotsCoverage
{
    /// <summary>
    /// 特殊能力レベルが小数のときの Pilot の能力値の計算が、元の SRC（VB 版 Pilot.cls）と同じ丸めになることを確かめる。
    /// VB では Double を Integer・Long に代入すると偶数への丸めになり、\ は被除数を偶数への丸めで整数にしてから割る。
    /// パイロットデータの能力値はすべて 100（PilotSkillStatusFixture.AddPilotData）。
    /// </summary>
    [TestClass]
    public class PilotVBRoundingTests
    {
        [TestMethod]
        public void Update_攻撃力低成長は被除数を丸めてから2で割る()
        {
            // VB: InfightBase = InfightBase + (lv * (1 + 2 * SkillLevel("格闘成長"))) \ 2
            // 3 * (1 + 2 * 0.75) = 7.5 → 8 \ 2 = 4（切り捨ててから割ると 3）
            var src = CreateSrc();
            DefineOption(src, "攻撃力低成長");
            AddPilotData(src, "テストパイロット").AddSkill("格闘成長", 0.75d, "", 1);
            var p = AddPilot(src, "テストパイロット", 3);

            Assert.AreEqual(104, p.InfightBase);
        }

        [TestMethod]
        public void Update_能力ＵＰの小数は偶数へ丸める()
        {
            // VB: InfightBase = InfightBase + SkillLevel("格闘ＵＰ")（Integer への代入）
            // レベル 2 で 100 + 2 = 102、102 + 1.5 = 103.5 → 104
            var src = CreateSrc();
            AddPilotData(src, "テストパイロット").AddSkill("格闘ＵＰ", 1.5d, "", 1);
            var p = AddPilot(src, "テストパイロット", 2);

            Assert.AreEqual(104, p.InfightBase);
        }

        [TestMethod]
        public void Update_超感覚の修正値は偶数へ丸める()
        {
            // VB: HitMod = HitMod + 2 * lv + 3（HitMod は Integer、lv は Double）
            // 2 * 1.25 + 3 = 5.5 → 6
            var src = CreateSrc();
            AddPilotData(src, "テストパイロット").AddSkill("超感覚", 1.25d, "", 1);
            var p = AddPilot(src, "テストパイロット", 1);

            Assert.AreEqual(6, p.HitMod);
            Assert.AreEqual(6, p.DodgeMod);
        }

        [TestMethod]
        public void Defense_防御力低成長は被除数を丸めてから2で割る()
        {
            // VB: Defense = Defense + (Level * (1 + 2 * SkillLevel("防御成長"))) \ 2
            // 100 + (3 * 2.5 = 7.5 → 8) \ 2 = 104
            var src = CreateSrc();
            DefineOption(src, "防御力成長");
            DefineOption(src, "防御力低成長");
            AddPilotData(src, "テストパイロット").AddSkill("防御成長", 0.75d, "", 1);
            var p = AddPilot(src, "テストパイロット", 3);

            Assert.AreEqual(104, p.Defense);
        }

        [TestMethod]
        public void SynchroRate_同調率成長は被除数を丸めてから10で割る()
        {
            // VB: SynchroRate = SynchroRate + lv * (10 + SkillLevel("同調率成長")) \ 10
            // 19 * 10.5 = 199.5 → 200 \ 10 = 20、50 + 20 = 70（実数で割って切り捨てると 69）
            var src = CreateSrc();
            var pd = AddPilotData(src, "テストパイロット");
            pd.AddSkill("同調率", 50d, "", 1);
            pd.AddSkill("同調率成長", 0.5d, "", 1);
            var p = AddPilot(src, "テストパイロット", 19);

            Assert.AreEqual(70, p.SynchroRate());
        }

        [TestMethod]
        public void TacticalTechnique0_戦術の小数は偶数へ丸める()
        {
            // VB: TacticalTechnique0 = TechniqueBase - Level + 10 * SkillLevel("戦術")（戻り値は Integer）
            // TechniqueBase = 100 + 1、101 - 1 + 7.5 = 107.5 → 108
            var src = CreateSrc();
            AddPilotData(src, "テストパイロット").AddSkill("戦術", 0.75d, "", 1);
            var p = AddPilot(src, "テストパイロット", 1);

            Assert.AreEqual(108, p.TacticalTechnique0());
        }
    }
}
