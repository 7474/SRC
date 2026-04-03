using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;
using SRCCore.VB;
using System.Reflection;

namespace SRCCore.Intermissions.Tests
{
    /// <summary>
    /// InterMission クラスの RankUpCost / MaxRank メソッドのユニットテスト。
    /// これらは純粋な計算ロジックであり、GUI 依存なしでテスト可能。
    /// </summary>
    [TestClass]
    public class InterMissionRankUpTests
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        /// <summary>
        /// UnitData を登録し、UList 経由でユニットを生成するヘルパー。
        /// rank パラメータは UList.Add の第2引数で設定される。
        /// </summary>
        private Unit CreateUnit(SRC src, string name = "テスト機体", int rank = 0)
        {
            if (!src.UDList.IsDefined(name))
            {
                src.UDList.Add(name);
            }
            return src.UList.Add(name, rank, "味方");
        }

        /// <summary>
        /// colFeature にリフレクション経由で特殊能力を直接追加するヘルパー。
        /// UnitFeatureMoreTests3 と同じパターン。
        /// </summary>
        private static void AddFeatureToUnit(Unit unit, string name, double level = Constants.DEFAULT_LEVEL, string data = "")
        {
            var field = typeof(Unit).GetField("colFeature", BindingFlags.NonPublic | BindingFlags.Instance);
            var colFeature = (SrcCollection<FeatureData>)field.GetValue(unit);
            var fd = new FeatureData { Name = name, Level = level, StrData = data };
            if (!colFeature.ContainsKey(name))
            {
                colFeature.Add(fd, name);
            }
        }

        /// <summary>
        /// Expression にオプションを設定するヘルパー。
        /// IsOptionDefined は "Option(name)" のグローバル変数の存在をチェックする。
        /// </summary>
        private static void SetOption(SRC src, string optionName)
        {
            src.Expression.DefineGlobalVariable("Option(" + optionName + ")");
            src.Expression.SetVariableAsLong("Option(" + optionName + ")", 1);
        }

        // ──────────────────────────────────────────────
        // MaxRank テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxRank_Default_Returns10()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src);
            var im = new InterMission(src);

            Assert.AreEqual(10, im.MaxRank(unit));
        }

        [TestMethod]
        public void MaxRank_With5StepOption_Returns5()
        {
            var src = CreateSrc();
            SetOption(src, "５段階改造");
            var unit = CreateUnit(src);
            var im = new InterMission(src);

            Assert.AreEqual(5, im.MaxRank(unit));
        }

        [TestMethod]
        public void MaxRank_With15StepOption_Returns15()
        {
            var src = CreateSrc();
            SetOption(src, "１５段階改造");
            var unit = CreateUnit(src);
            var im = new InterMission(src);

            Assert.AreEqual(15, im.MaxRank(unit));
        }

        [TestMethod]
        public void MaxRank_DisabledByGlobalVariable_Returns0()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, "改造不可機体");
            var im = new InterMission(src);

            // Disable(ユニット名,改造) グローバル変数が定義されると改造不可
            src.Expression.DefineGlobalVariable("Disable(改造不可機体,改造)");

            Assert.AreEqual(0, im.MaxRank(unit));
        }

        [TestMethod]
        public void MaxRank_WithMaxUpgradeFeature_ReturnsMinimum()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, "限定改造機体");
            var im = new InterMission(src);

            // 最大改造数=3 の特殊能力を追加
            AddFeatureToUnit(unit, "最大改造数", 3);

            // デフォルト10と3の小さい方 → 3
            Assert.AreEqual(3, im.MaxRank(unit));
        }

        [TestMethod]
        public void MaxRank_WithMaxUpgradeFeatureHigherThanDefault_ReturnsDefault()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, "高改造機体");
            var im = new InterMission(src);

            // 最大改造数=20 (デフォルト10より大きい)
            AddFeatureToUnit(unit, "最大改造数", 20);

            // デフォルト10と20の小さい方 → 10
            Assert.AreEqual(10, im.MaxRank(unit));
        }

        [TestMethod]
        public void MaxRank_With15StepAndMaxUpgradeFeature_ReturnsMinimum()
        {
            var src = CreateSrc();
            SetOption(src, "１５段階改造");
            var unit = CreateUnit(src, "限定15段機体");
            var im = new InterMission(src);

            AddFeatureToUnit(unit, "最大改造数", 8);

            // 15と8の小さい方 → 8
            Assert.AreEqual(8, im.MaxRank(unit));
        }

        // ──────────────────────────────────────────────
        // RankUpCost テスト - 通常10段改造モード
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RankUpCost_StandardMode_Rank0_Returns10000()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, "標準機A", 0);
            var im = new InterMission(src);

            Assert.AreEqual(10000, im.RankUpCost(unit));
        }

        [TestMethod]
        public void RankUpCost_StandardMode_Rank1_Returns15000()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, "標準機B", 1);
            var im = new InterMission(src);

            Assert.AreEqual(15000, im.RankUpCost(unit));
        }

        [TestMethod]
        public void RankUpCost_StandardMode_Rank5_Returns150000()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, "標準機C", 5);
            var im = new InterMission(src);

            Assert.AreEqual(150000, im.RankUpCost(unit));
        }

        [TestMethod]
        public void RankUpCost_StandardMode_Rank9_Returns500000()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, "標準機D", 9);
            var im = new InterMission(src);

            Assert.AreEqual(500000, im.RankUpCost(unit));
        }

        [TestMethod]
        public void RankUpCost_StandardMode_AllRanks_MatchExpected()
        {
            // 通常10段改造: 0→10000, 1→15000, 2→20000, 3→40000, 4→80000,
            //              5→150000, 6→200000, 7→300000, 8→400000, 9→500000
            var expected = new[] { 10000, 15000, 20000, 40000, 80000, 150000, 200000, 300000, 400000, 500000 };

            for (int rank = 0; rank < expected.Length; rank++)
            {
                var src = CreateSrc();
                var unit = CreateUnit(src, "全ランク機" + rank, rank);
                var im = new InterMission(src);

                Assert.AreEqual(expected[rank], im.RankUpCost(unit),
                    $"Standard mode: Rank {rank} should cost {expected[rank]}");
            }
        }

        // ──────────────────────────────────────────────
        // RankUpCost テスト - 低改造費モード
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RankUpCost_LowCostMode_Rank0_Returns10000()
        {
            var src = CreateSrc();
            SetOption(src, "低改造費");
            var unit = CreateUnit(src, "低費用機A", 0);
            var im = new InterMission(src);

            Assert.AreEqual(10000, im.RankUpCost(unit));
        }

        [TestMethod]
        public void RankUpCost_LowCostMode_Rank14_Returns200000()
        {
            var src = CreateSrc();
            SetOption(src, "低改造費");
            SetOption(src, "１５段階改造"); // 低改造費では15段まで必要
            var unit = CreateUnit(src, "低費用機B", 14);
            var im = new InterMission(src);

            Assert.AreEqual(200000, im.RankUpCost(unit));
        }

        [TestMethod]
        public void RankUpCost_LowCostMode_AllRanks_MatchExpected()
        {
            // 低改造費: 0→10000, 1→15000, 2→20000, 3→30000, 4→40000,
            //          5→50000, 6→60000, 7→70000, 8→80000, 9→100000,
            //          10→120000, 11→140000, 12→160000, 13→180000, 14→200000
            var expected = new[] { 10000, 15000, 20000, 30000, 40000, 50000, 60000, 70000, 80000, 100000,
                                   120000, 140000, 160000, 180000, 200000 };

            for (int rank = 0; rank < expected.Length; rank++)
            {
                var src = CreateSrc();
                SetOption(src, "低改造費");
                SetOption(src, "１５段階改造"); // MaxRank を15にする
                var unit = CreateUnit(src, "低費用全ランク" + rank, rank);
                var im = new InterMission(src);

                Assert.AreEqual(expected[rank], im.RankUpCost(unit),
                    $"Low cost mode: Rank {rank} should cost {expected[rank]}");
            }
        }

        // ──────────────────────────────────────────────
        // RankUpCost テスト - 15段階改造モード
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RankUpCost_15StepMode_Rank0_Returns10000()
        {
            var src = CreateSrc();
            SetOption(src, "１５段階改造");
            var unit = CreateUnit(src, "15段機A", 0);
            var im = new InterMission(src);

            Assert.AreEqual(10000, im.RankUpCost(unit));
        }

        [TestMethod]
        public void RankUpCost_15StepMode_Rank14_Returns550000()
        {
            var src = CreateSrc();
            SetOption(src, "１５段階改造");
            var unit = CreateUnit(src, "15段機B", 14);
            var im = new InterMission(src);

            Assert.AreEqual(550000, im.RankUpCost(unit));
        }

        [TestMethod]
        public void RankUpCost_15StepMode_AllRanks_MatchExpected()
        {
            // 15段階改造: 0→10000, 1→15000, 2→20000, 3→40000, 4→80000,
            //            5→120000, 6→160000, 7→200000, 8→250000, 9→300000,
            //            10→350000, 11→400000, 12→450000, 13→500000, 14→550000
            var expected = new[] { 10000, 15000, 20000, 40000, 80000, 120000, 160000, 200000, 250000, 300000,
                                   350000, 400000, 450000, 500000, 550000 };

            for (int rank = 0; rank < expected.Length; rank++)
            {
                var src = CreateSrc();
                SetOption(src, "１５段階改造");
                var unit = CreateUnit(src, "15段全ランク" + rank, rank);
                var im = new InterMission(src);

                Assert.AreEqual(expected[rank], im.RankUpCost(unit),
                    $"15-step mode: Rank {rank} should cost {expected[rank]}");
            }
        }

        // ──────────────────────────────────────────────
        // RankUpCost テスト - 最大ランク到達時
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RankUpCost_AtMaxRank_Returns999999999()
        {
            var src = CreateSrc();
            // デフォルト MaxRank=10
            var unit = CreateUnit(src, "最大ランク機", 10);
            var im = new InterMission(src);

            Assert.AreEqual(999999999, im.RankUpCost(unit));
        }

        [TestMethod]
        public void RankUpCost_BeyondMaxRank_Returns999999999()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, "超過ランク機", 15);
            var im = new InterMission(src);

            Assert.AreEqual(999999999, im.RankUpCost(unit));
        }

        // ──────────────────────────────────────────────
        // RankUpCost テスト - 分離/合体制限
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RankUpCost_SeparatedNonMainForm_Returns999999999()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, "合体形態機A", 0);
            var im = new InterMission(src);

            // 分離の StrData が3要素 (LLength==3) で主形態でない場合は改造不可
            AddFeatureToUnit(unit, "分離", data: "パーツA パーツB パーツC");

            Assert.AreEqual(999999999, im.RankUpCost(unit));
        }

        [TestMethod]
        public void RankUpCost_SeparatedMainForm_ReturnsNormalCost()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, "合体主形態機", 0);
            var im = new InterMission(src);

            // 分離ありかつ主形態あり → 改造可能
            AddFeatureToUnit(unit, "分離", data: "パーツA パーツB パーツC");
            AddFeatureToUnit(unit, "主形態");

            Assert.AreEqual(10000, im.RankUpCost(unit));
        }

        [TestMethod]
        public void RankUpCost_WithTimeLimit_Returns999999999()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, "制限時間機", 0);
            var im = new InterMission(src);

            // 制限時間がある場合は改造不可
            AddFeatureToUnit(unit, "分離", data: "パーツA");
            AddFeatureToUnit(unit, "制限時間", 3);

            Assert.AreEqual(999999999, im.RankUpCost(unit));
        }

        // ──────────────────────────────────────────────
        // RankUpCost テスト - 改造費修正
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RankUpCost_WithCostModifierPositive_IncreasesBaseCost()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, "改造費修正機A", 0);
            var im = new InterMission(src);

            // 改造費修正 Level=5 → 基本費用 * (1 + 5/10) = 基本費用 * 1.5
            AddFeatureToUnit(unit, "改造費修正", 5);

            // Rank0 の基本費用 10000 * 1.5 = 15000
            Assert.AreEqual(15000, im.RankUpCost(unit));
        }

        [TestMethod]
        public void RankUpCost_WithCostModifierNegative_DecreasesBaseCost()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, "改造費修正機B", 0);
            var im = new InterMission(src);

            // 改造費修正 Level=-5 → 基本費用 * (1 + (-5)/10) = 基本費用 * 0.5
            AddFeatureToUnit(unit, "改造費修正", -5);

            // Rank0 の基本費用 10000 * 0.5 = 5000
            Assert.AreEqual(5000, im.RankUpCost(unit));
        }

        [TestMethod]
        public void RankUpCost_UpgradeDisabledByDisableGlobal_Returns999999999()
        {
            var src = CreateSrc();
            var unit = CreateUnit(src, "改造禁止機体");
            var im = new InterMission(src);

            // Disable(ユニット名,改造) が定義されると MaxRank=0 → 改造不可
            src.Expression.DefineGlobalVariable("Disable(改造禁止機体,改造)");

            Assert.AreEqual(999999999, im.RankUpCost(unit));
        }
    }
}
