using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SRCCore.Coverage.Tests.SpecialPower
{
    /// <summary>
    /// Pilot.sp.cs (最大ＳＰ・ＳＰ値・スペシャルパワーの修得/消費ＳＰ/使用)。
    /// 元の SRC の Pilot.MaxSP / SP / SpecialPowerCost / UseSpecialPower に対応。
    /// </summary>
    [TestClass]
    public class SpecialPowerPilotTests
    {
        private static Pilots.Pilot CreatePilot(SpecialPowerTestFixture f, int level, int baseSP = 50,
            params (string name, double level, string data)[] skills)
        {
            var pd = f.DefinePilot("パイロット", sp: baseSP);
            foreach (var s in skills)
            {
                pd.AddSkill(s.name, s.level, s.data, 1);
            }
            return f.AddUnit("機体", "パイロット", "味方", 1, 1, level: level).MainPilot();
        }

        // ──────────────────────────────────────────────
        // MaxSP
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxSP_GrowsTwicePerLevelUpTo40_ThenOnePerLevel()
        {
            // 最大ＳＰ = 基本値 + (Lv<=40 なら 2×Lv、それ以上は Lv+40)
            Assert.AreEqual(50 + 20, CreatePilot(new SpecialPowerTestFixture(), 10).MaxSP);
            Assert.AreEqual(50 + 80, CreatePilot(new SpecialPowerTestFixture(), 40).MaxSP);
            Assert.AreEqual(50 + 90, CreatePilot(new SpecialPowerTestFixture(), 50).MaxSP);
            // レベル100超は100として扱う
            Assert.AreEqual(50 + 140, CreatePilot(new SpecialPowerTestFixture(), 150).MaxSP);
        }

        [TestMethod]
        public void MaxSP_ZeroBaseSP_IsZeroRegardlessOfLevel()
        {
            // ＳＰを持たないパイロットはレベルに関わらず最大ＳＰ0
            var p = CreatePilot(new SpecialPowerTestFixture(), 30, baseSP: 0);
            Assert.AreEqual(0, p.MaxSP);
            Assert.AreEqual(0, p.SP);
        }

        [TestMethod]
        public void MaxSP_GrowthSkillsAndModifiers()
        {
            // ＳＰ低成長は成長分を半分、ＳＰ高成長は1.5倍、ＳＰＵＰ/ＳＰＤＯＷＮはレベル分を加減
            Assert.AreEqual(50 + 10, CreatePilot(new SpecialPowerTestFixture(), 10, 50, ("ＳＰ低成長", 1, "")).MaxSP);
            Assert.AreEqual(50 + 30, CreatePilot(new SpecialPowerTestFixture(), 10, 50, ("ＳＰ高成長", 1, "")).MaxSP);
            Assert.AreEqual(50 + 20 + 15, CreatePilot(new SpecialPowerTestFixture(), 10, 50, ("ＳＰＵＰ", 15, "")).MaxSP);
            Assert.AreEqual(50 + 20 - 5, CreatePilot(new SpecialPowerTestFixture(), 10, 50, ("ＳＰＤＯＷＮ", 5, "")).MaxSP);
            // 追加レベルは成長計算上のレベルに加算
            Assert.AreEqual(50 + 30, CreatePilot(new SpecialPowerTestFixture(), 10, 50, ("追加レベル", 5, "")).MaxSP);
        }

        [TestMethod]
        public void MaxSP_LowGrowthOption_HalvesGrowth()
        {
            // オプション「ＳＰ低成長」は全パイロットの成長分を半分にする
            var f = new SpecialPowerTestFixture();
            f.Src.Expression.DefineGlobalVariable("Option(ＳＰ低成長)");
            f.Src.Expression.SetVariableAsLong("Option(ＳＰ低成長)", 1);
            Assert.AreEqual(50 + 10, CreatePilot(f, 10).MaxSP);
        }

        [TestMethod]
        public void MaxSP_CappedAt9999()
        {
            Assert.AreEqual(9999, CreatePilot(new SpecialPowerTestFixture(), 10, baseSP: 9990).MaxSP);
        }

        [TestMethod]
        public void MaxSP_HighGrowth_RoundsLikeVBLongAssignment()
        {
            // VB: MaxSP = 1.5 * MaxSP は Long への代入で偶数丸め。Lv41 → (41+40)×1.5 = 121.5 → 122
            Assert.AreEqual(50 + 122, CreatePilot(new SpecialPowerTestFixture(), 41, 50, ("ＳＰ高成長", 1, "")).MaxSP);
        }

        // ──────────────────────────────────────────────
        // SP
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SP_SetterClampsBetweenZeroAndMax()
        {
            var p = CreatePilot(new SpecialPowerTestFixture(), 10);
            Assert.AreEqual(70, p.SP, "全回復で最大ＳＰになる");

            p.SP = 1000;
            Assert.AreEqual(70, p.SP);
            p.SP = -5;
            Assert.AreEqual(0, p.SP);
            p.SP = 33;
            Assert.AreEqual(33, p.SP);
            Assert.AreEqual(33, p.SP0);
        }

        // ──────────────────────────────────────────────
        // 修得
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SpecialPowers_AvailableByLevel()
        {
            // 修得レベルに達したスペシャルパワーのみ使用可能
            var f = new SpecialPowerTestFixture();
            var pd = f.DefinePilot("パイロット");
            pd.AddSpecialPower("集中", 1, 0);
            pd.AddSpecialPower("熱血", 5, 0);
            pd.AddSpecialPower("魂", 30, 0);
            var p = f.AddUnit("機体", "パイロット", "味方", 1, 1, level: 10).MainPilot();

            Assert.AreEqual(2, p.CountSpecialPower);
            CollectionAssert.AreEqual(new[] { "集中", "熱血" }, p.SpecialPowerNames.ToArray());
            Assert.AreEqual("熱血", p.get_SpecialPower(2));
            Assert.IsTrue(p.IsSpecialPowerAvailable("熱血"));
            Assert.IsFalse(p.IsSpecialPowerAvailable("魂"));
        }

        // ──────────────────────────────────────────────
        // 消費ＳＰ
        // ──────────────────────────────────────────────

        private static Pilots.Pilot CreatePilotWithSP(SpecialPowerTestFixture f, params (string name, double level, string data)[] skills)
        {
            f.DefineSP("熱血", "自分", "攻撃", "ダメージ増加Lv1", cost: 40);
            f.DefineSP("必中", "自分", "ターン", "絶対命中", cost: 15);
            var pd = f.DefinePilot("パイロット");
            pd.AddSpecialPower("熱血", 1, 0);
            pd.AddSpecialPower("必中", 1, 25);
            foreach (var s in skills)
            {
                pd.AddSkill(s.name, s.level, s.data, 1);
            }
            return f.AddUnit("機体", "パイロット", "味方", 1, 1, level: 10).MainPilot();
        }

        [TestMethod]
        public void SpecialPowerCost_PilotDefinitionOverridesDefault()
        {
            // パイロットデータ側の消費ＳＰ指定があればそれを、なければスペシャルパワーデータの値を使う
            var p = CreatePilotWithSP(new SpecialPowerTestFixture());
            Assert.AreEqual(40, p.SpecialPowerCost("熱血"));
            Assert.AreEqual(25, p.SpecialPowerCost("必中"));
        }

        [TestMethod]
        public void SpecialPowerCost_SkillModifiers()
        {
            // 集中力/超能力は 0.8 倍、知覚強化は 1.2 倍
            Assert.AreEqual(32, CreatePilotWithSP(new SpecialPowerTestFixture(), ("集中力", 1, "")).SpecialPowerCost("熱血"));
            Assert.AreEqual(32, CreatePilotWithSP(new SpecialPowerTestFixture(), ("超能力", 1, "")).SpecialPowerCost("熱血"));
            Assert.AreEqual(48, CreatePilotWithSP(new SpecialPowerTestFixture(), ("知覚強化", 1, "")).SpecialPowerCost("熱血"));
        }

        [TestMethod]
        public void SpecialPowerCost_SPCostReduction_AppliesToListedPowersOnly()
        {
            // ＳＰ消費減少Lvn=別名 名称… は指定したスペシャルパワーの消費を 10×n% 減らす
            var p = CreatePilotWithSP(new SpecialPowerTestFixture(), ("ＳＰ消費減少", 2, "熱い心 熱血"));
            Assert.AreEqual(32, p.SpecialPowerCost("熱血"));
            Assert.AreEqual(25, p.SpecialPowerCost("必中"), "別名や未指定のものは減らない");
        }

        [TestMethod]
        public void SpecialPowerCost_ConcentrationRoundsLikeVBIntegerAssignment()
        {
            // VB: SpecialPowerCost = 0.8 * SpecialPowerCost は Integer 代入で丸め。7 × 0.8 = 5.6 → 6
            var f = new SpecialPowerTestFixture();
            f.DefineSP("加速", "自分", "ターン", "移動力強化Lv2", cost: 7);
            var pd = f.DefinePilot("パイロット");
            pd.AddSpecialPower("加速", 1, 0);
            pd.AddSkill("集中力", 1, "", 1);
            var p = f.AddUnit("機体", "パイロット", "味方", 1, 1, level: 10).MainPilot();

            Assert.AreEqual(6, p.SpecialPowerCost("加速"));
        }

        // ──────────────────────────────────────────────
        // UseSpecialPower
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UseSpecialPower_AppliesEffectAndConsumesSP()
        {
            // 使用するとアニメ (無効時はメッセージ) を表示し、効果を適用して消費ＳＰを引く
            var f = new SpecialPowerTestFixture();
            f.DefineSP("根性", "自分", "即効", "ＨＰ回復Lv3", cost: 20);
            var pd = f.DefinePilot("パイロット");
            pd.AddSpecialPower("根性", 1, 0);
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1, level: 10);
            var p = u.MainPilot();
            u.HP = 100;

            p.UseSpecialPower("根性");

            Assert.AreEqual(400, u.HP);
            Assert.AreEqual(70 - 20, p.SP);
            Assert.AreSame(p, f.Src.Commands.SelectedPilot);
            CollectionAssert.Contains(f.SysMessages, "パイロットは根性を使った。");
        }

        [TestMethod]
        public void UseSpecialPower_SPModMultipliesCost()
        {
            // sp_mod は消費ＳＰの倍率
            var f = new SpecialPowerTestFixture();
            f.DefineSP("熱血", "自分", "攻撃", "ダメージ増加Lv1", cost: 20);
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1, level: 10);
            var p = u.MainPilot();

            p.UseSpecialPower("熱血", 2d);

            Assert.IsTrue(u.IsSpecialPowerInEffect("熱血"));
            Assert.AreEqual(70 - 40, p.SP);
        }

        [TestMethod]
        public void UseSpecialPower_Undefined_DoesNothing()
        {
            var f = new SpecialPowerTestFixture();
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1, level: 10);
            var p = u.MainPilot();

            p.UseSpecialPower("未定義");

            Assert.AreEqual(70, p.SP);
            Assert.AreEqual(0, f.SysMessages.Count);
        }
    }
}
