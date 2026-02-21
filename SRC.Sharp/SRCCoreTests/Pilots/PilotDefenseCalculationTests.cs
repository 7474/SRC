using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Pilots.Tests
{
    /// <summary>
    /// 移植精度検証: Pilot の防御力計算 (Defense) のユニットテスト。
    /// VB6 版の数値をそのまま期待値として検証する。
    /// </summary>
    [TestClass]
    public class PilotDefenseCalculationTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        private Pilot CreatePilot(SRC src, string name = "テストパイロット", int level = 1)
        {
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            return src.PList.Add(name, level, "味方");
        }

        // ══════════════════════════════════════════════
        // Defense — 防御力成長オプションなし
        // ══════════════════════════════════════════════
        // 公式: 100 + 5 * SkillLevel("耐久")
        // レベルに依存しない

        [TestMethod]
        public void Defense_NoGrowthOption_Level1_NoDurability_Returns100()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 1);
            // 耐久スキルなし → 100 + 5*0 = 100
            Assert.AreEqual(100, pilot.Defense);
        }

        [TestMethod]
        public void Defense_NoGrowthOption_Level10_NoDurability_Returns100()
        {
            // レベルが上がっても防御力成長オプションなしなら変化しない
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 10);
            Assert.AreEqual(100, pilot.Defense);
        }

        [TestMethod]
        public void Defense_NoGrowthOption_Level50_NoDurability_Returns100()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, level: 50);
            Assert.AreEqual(100, pilot.Defense);
        }

        // ══════════════════════════════════════════════
        // Defense — 防御力成長オプションあり
        // ══════════════════════════════════════════════
        // 公式: 100 + 5*耐久 + Level * (1 + SkillLevel("防御成長"))

        [TestMethod]
        public void Defense_WithGrowthOption_Level1_NoDurability_Returns101()
        {
            // 100 + 5*0 + 1*(1+0) = 101
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(防御力成長)");
            var pilot = CreatePilot(src, level: 1);
            Assert.AreEqual(101, pilot.Defense);
        }

        [TestMethod]
        public void Defense_WithGrowthOption_Level10_NoDurability_Returns110()
        {
            // 100 + 5*0 + 10*(1+0) = 110
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(防御力成長)");
            var pilot = CreatePilot(src, level: 10);
            Assert.AreEqual(110, pilot.Defense);
        }

        [TestMethod]
        public void Defense_WithGrowthOption_Level20_NoDurability_Returns120()
        {
            // 100 + 20*1 = 120
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(防御力成長)");
            var pilot = CreatePilot(src, level: 20);
            Assert.AreEqual(120, pilot.Defense);
        }

        [TestMethod]
        public void Defense_WithLevelUpOption_Level10_NoDurability_Returns110()
        {
            // 防御力レベルアップ も防御力成長と同等
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(防御力レベルアップ)");
            var pilot = CreatePilot(src, level: 10);
            Assert.AreEqual(110, pilot.Defense);
        }

        // ══════════════════════════════════════════════
        // Defense — 防御力低成長オプション
        // ══════════════════════════════════════════════
        // 公式: 100 + 5*耐久 + floor(Level * (1 + 2*防御成長)) / 2

        [TestMethod]
        public void Defense_WithLowGrowthOption_Level10_NoDurability_Returns105()
        {
            // 100 + floor(10*(1+0)) / 2 = 100 + 5 = 105
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(防御力成長)");
            src.Expression.DefineGlobalVariable("Option(防御力低成長)");
            var pilot = CreatePilot(src, level: 10);
            Assert.AreEqual(105, pilot.Defense);
        }

        [TestMethod]
        public void Defense_WithLowGrowthOption_Level20_NoDurability_Returns110()
        {
            // 100 + floor(20*(1+0)) / 2 = 100 + 10 = 110
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(防御力成長)");
            src.Expression.DefineGlobalVariable("Option(防御力低成長)");
            var pilot = CreatePilot(src, level: 20);
            Assert.AreEqual(110, pilot.Defense);
        }

        [TestMethod]
        public void Defense_WithLowGrowthOption_Level1_NoDurability_Returns100()
        {
            // 100 + floor(1*(1+0)) / 2 = 100 + 0 = 100 (整数除算)
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(防御力成長)");
            src.Expression.DefineGlobalVariable("Option(防御力低成長)");
            var pilot = CreatePilot(src, level: 1);
            Assert.AreEqual(100, pilot.Defense);
        }

        [TestMethod]
        public void Defense_WithLowGrowthOption_Level2_NoDurability_Returns101()
        {
            // 100 + floor(2*(1+0)) / 2 = 100 + 1 = 101
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(防御力成長)");
            src.Expression.DefineGlobalVariable("Option(防御力低成長)");
            var pilot = CreatePilot(src, level: 2);
            Assert.AreEqual(101, pilot.Defense);
        }
    }
}
