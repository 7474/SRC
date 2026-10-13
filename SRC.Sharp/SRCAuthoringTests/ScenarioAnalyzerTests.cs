using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Analysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCAuthoring.Tests
{
    [TestClass]
    public class ScenarioAnalyzerTests
    {
        [TestMethod]
        public void Analyze_CollectsLabelsAndIncludes()
        {
            using (var scenario = new TestScenario())
            {
                var lib = scenario.Write(Path.Combine("Lib", "共通.eve"), "共通処理:", "Return");
                var eve = scenario.Write("test.eve",
                    "プロローグ:",
                    "Call 共通処理",
                    "Exit",
                    "",
                    "スタート:",
                    "Exit",
                    "<Lib\\共通.eve>");

                var analysis = new ScenarioAnalyzer().Analyze(eve);

                Assert.AreEqual(scenario.Root, analysis.ScenarioRoot);
                CollectionAssert.IsSubsetOf(new[] { "プロローグ", "スタート", "共通処理" }, analysis.Labels.Select(x => x.Name).ToList());
                var common = analysis.Labels.Single(x => x.Name == "共通処理");
                Assert.IsTrue(ScenarioAnalysis.SamePath(lib, common.File));
                Assert.AreEqual(1, common.Line);
                var include = analysis.Includes.Single();
                Assert.AreEqual(7, include.Line);
                Assert.IsTrue(ScenarioAnalysis.SamePath(lib, include.ResolvedPath));
                Assert.AreEqual(0, analysis.Diagnostics.Count, string.Join("\n", analysis.Diagnostics));
            }
        }

        [TestMethod]
        public void Analyze_ReportsStructureErrorsAndMissingLabels()
        {
            using (var scenario = new TestScenario())
            {
                var eve = scenario.Write("test.eve",
                    "スタート:",
                    "If 1 = 1 Then",
                    "Set x 1",
                    "Call 無いラベル",
                    "Exit",
                    "<Lib\\無い.eve>");

                var analysis = new ScenarioAnalyzer().Analyze(eve);
                var messages = analysis.DiagnosticsFor(eve).Select(x => $"{x.Line}: {x.Message}").ToList();

                Assert.IsTrue(messages.Any(x => x.Contains("EndIf") || x.Contains("Ifに対応")), string.Join("\n", messages));
                Assert.IsTrue(messages.Contains("4: ラベル「無いラベル」が見つかりません"), string.Join("\n", messages));
                Assert.IsTrue(messages.Any(x => x.StartsWith("6:") && x.Contains("無い.eve")), string.Join("\n", messages));
            }
        }

        [TestMethod]
        public void Analyze_UsesUnsavedText()
        {
            using (var scenario = new TestScenario())
            {
                var eve = scenario.Write("test.eve", "スタート:", "Exit");
                var overlays = new Dictionary<string, string> { [eve] = "スタート:\nCall 新しいラベル\nExit\n新しいラベル:\nReturn" };

                var analysis = new ScenarioAnalyzer().Analyze(eve, overlays);

                Assert.IsTrue(analysis.Labels.Any(x => x.Name == "新しいラベル" && x.Line == 4));
                Assert.AreEqual(0, analysis.Diagnostics.Count, string.Join("\n", analysis.Diagnostics));
            }
        }

        [TestMethod]
        public void Analyze_DoesNotReportLabelHeldInVariable()
        {
            using (var scenario = new TestScenario())
            {
                var eve = scenario.Write("test.eve", "スタート:", "Set 行き先 処理A", "Call 行き先", "Exit", "処理A:", "Return");

                var analysis = new ScenarioAnalyzer().Analyze(eve);

                Assert.AreEqual(0, analysis.Diagnostics.Count, string.Join("\n", analysis.Diagnostics));
            }
        }
    }
}
