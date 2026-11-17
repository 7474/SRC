using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Analysis;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SRCAuthoring.Tests
{
    [TestClass]
    public class ScenarioFlowAnalyzerTests
    {
        private static string FindRepoRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "SRC.Sharp.sln")))
            {
                dir = dir.Parent;
            }
            Assert.IsNotNull(dir, "SRC.Sharp.sln が見つからない");
            return dir.FullName;
        }

        [TestMethod]
        public void Analyze_LinearStagesIntermissionAndExcludesIncludeLibrary()
        {
            using var scenario = new TestScenario();
            scenario.Write("Data/robot.txt", "");
            scenario.Write("Lib/include.eve",
                "敗北:",
                "GameOver");
            scenario.Write("Lib/作戦会議.eve",
                "プロローグ:",
                "Talk システム",
                "作戦会議",
                "End",
                "Continue");
            scenario.Write("第1話.eve",
                "プロローグ:",
                "IntermissionCommand 作戦会議 Lib\\作戦会議.eve",
                "Exit",
                "スタート:",
                "Exit",
                "全滅 敵:",
                "Continue 第2話.eve");
            scenario.Write("第2話.eve",
                "スタート:",
                "Exit",
                "全滅 敵:",
                "GameClear");

            var flow = new ScenarioFlowAnalyzer().Analyze(scenario.Root);

            Assert.AreEqual(0, flow.Warnings.Count, string.Join("\n", flow.Warnings.Select(w => w.Message)));
            Assert.IsFalse(flow.Nodes.Any(n => n.Path == "Lib/include.eve"), "サブルーチン専用の include.eve はノードに出さない");

            var ep1 = flow.Nodes.Single(n => n.Path == "第1話.eve");
            var ep2 = flow.Nodes.Single(n => n.Path == "第2話.eve");
            var briefing = flow.Nodes.Single(n => n.Path == "Lib/作戦会議.eve");
            var clear = flow.Nodes.Single(n => n.Kind == FlowNodeKind.GameClear);

            Assert.IsTrue(ep1.IsEntry);
            Assert.IsFalse(ep2.IsEntry);
            Assert.AreEqual(FlowNodeKind.Intermission, briefing.Kind);

            Assert.IsTrue(flow.Edges.Any(e => e.From == ep1.Id && e.To == ep2.Id && e.Kind == FlowEdgeKind.Continue));
            Assert.IsTrue(flow.Edges.Any(e => e.From == ep1.Id && e.To == briefing.Id && e.Kind == FlowEdgeKind.IntermissionCommand && e.CommandArg == "作戦会議"));
            Assert.IsTrue(flow.Edges.Any(e => e.From == briefing.Id && e.To == briefing.Id && e.Kind == FlowEdgeKind.IntermissionReturn));
            Assert.IsTrue(flow.Edges.Any(e => e.From == ep2.Id && e.To == clear.Id && e.Kind == FlowEdgeKind.GameClear));

            var mermaid = flow.ToMermaid();
            StringAssert.Contains(mermaid, "flowchart TD");
            StringAssert.Contains(mermaid, "第1話.eve");
            StringAssert.Contains(mermaid, "メニュー: 作戦会議");
            StringAssert.Contains(mermaid, "GameClear");
        }

        [TestMethod]
        public void Analyze_IfNextStageAndSwitchExecBranches()
        {
            using var scenario = new TestScenario();
            scenario.Write("Data/robot.txt", "");
            scenario.Write("ep01.eve",
                "プロローグ:",
                "Exit",
                "全滅 敵:",
                "If ルート = \"地上\" Then",
                "  次ステージ = \"ep02_a.eve\"",
                "Else",
                "  次ステージ = \"ep02_b.eve\"",
                "EndIf",
                "Continue");
            scenario.Write("ep02_a.eve",
                "スタート:",
                "Switch 分岐",
                "Case 1",
                "  Exec ep03.eve 通常ステージ",
                "Case 2",
                "  Exec sub.eve",
                "Case Else",
                "  GameClear",
                "EndSw");
            scenario.Write("ep02_b.eve",
                "スタート:",
                "GameClear");
            scenario.Write("ep03.eve",
                "スタート:",
                "GameClear");
            scenario.Write("sub.eve",
                "スタート:",
                "Exit");

            var flow = new ScenarioFlowAnalyzer().Analyze(scenario.Root);

            Assert.AreEqual(0, flow.Warnings.Count, string.Join("\n", flow.Warnings.Select(w => w.Message)));

            var ep1 = flow.Nodes.Single(n => n.Path == "ep01.eve");
            var ep2a = flow.Nodes.Single(n => n.Path == "ep02_a.eve");
            var ep2b = flow.Nodes.Single(n => n.Path == "ep02_b.eve");
            var ep3 = flow.Nodes.Single(n => n.Path == "ep03.eve");
            var sub = flow.Nodes.Single(n => n.Path == "sub.eve");

            Assert.AreEqual(FlowNodeKind.Stage, ep3.Kind);
            Assert.AreEqual(FlowNodeKind.SubStage, sub.Kind);

            var toA = flow.Edges.Single(e => e.From == ep1.Id && e.To == ep2a.Id);
            Assert.AreEqual(FlowEdgeKind.NextStage, toA.Kind);
            StringAssert.Contains(toA.Condition, "ルート = \"地上\"");

            var toB = flow.Edges.Single(e => e.From == ep1.Id && e.To == ep2b.Id);
            Assert.AreEqual("Else", toB.Condition);

            var toEp3 = flow.Edges.Single(e => e.From == ep2a.Id && e.To == ep3.Id);
            Assert.AreEqual(FlowEdgeKind.Exec, toEp3.Kind);
            StringAssert.Contains(toEp3.Condition, "分岐 = 1");

            var toSub = flow.Edges.Single(e => e.From == ep2a.Id && e.To == sub.Id);
            StringAssert.Contains(toSub.Condition, "分岐 = 2");

            var toClearCaseElse = flow.Edges.Single(e => e.From == ep2a.Id && e.Kind == FlowEdgeKind.GameClear);
            StringAssert.Contains(toClearCaseElse.Condition, "分岐: その他");

            // Mermaid で引用符がエスケープされているか、JSON・Markdown が壊れないか
            var mermaid = flow.ToMermaid();
            StringAssert.Contains(mermaid, "#quot;地上#quot;");
            var md = flow.ToMarkdown();
            StringAssert.Contains(md, "```mermaid");
            StringAssert.Contains(md, "## 話の流れ");
            using var json = JsonDocument.Parse(flow.ToJson());
            Assert.IsTrue(json.RootElement.GetProperty("nodes").GetArrayLength() >= 6);
        }

        [TestMethod]
        public void Analyze_LoadDataDynamicTargetsAndMissingWarnings()
        {
            using var scenario = new TestScenario();
            scenario.Write("Data/敵部隊/pilot.txt", "");
            scenario.Write("Data/敵部隊/include.eve", "敵増援:", "Return");
            scenario.Write("stage1.eve",
                "プロローグ:",
                "Load 敵部隊",
                "Load 無い部隊",
                "Set 行き先 \"stage2.eve\"",
                "Exit",
                "全滅 敵:",
                "If 分岐 = 1 Then",
                "  Continue stage2.eve",
                "ElseIf 分岐 = 2 Then",
                "  Continue 行き先",
                "Else",
                "  Continue $(動的先)",
                "EndIf",
                "Continue 無い話.eve");
            scenario.Write("stage2.eve",
                "スタート:",
                "GameClear");

            var flow = new ScenarioFlowAnalyzer().Analyze(Path.Combine(scenario.Root, "stage1.eve"));

            // Load 敵部隊 は Data/敵部隊/include.eve に解決され警告なし
            var loadOk = flow.Nodes.Single(n => n.Kind == FlowNodeKind.LoadData && n.Title == "敵部隊");
            Assert.IsTrue(loadOk.Exists);
            StringAssert.EndsWith(loadOk.FullPath, "include.eve");

            // 動的変数・式は警告にならない
            Assert.IsFalse(flow.Warnings.Any(w => w.Target == "行き先" || w.Target == "$(動的先)"));
            Assert.AreEqual(2, flow.Edges.Count(e => e.IsDynamic));

            // 見つからない先だけ警告になる（無い部隊・無い話.eve）
            Assert.AreEqual(2, flow.Warnings.Count, string.Join("\n", flow.Warnings.Select(w => w.Message)));
            Assert.IsTrue(flow.Warnings.Any(w => w.Target == "無い話.eve"));
            Assert.IsTrue(flow.Warnings.Any(w => w.Target == "無い部隊"));
            Assert.IsTrue(flow.Nodes.Any(n => n.Kind == FlowNodeKind.Missing && n.Path == "無い話.eve"));
        }

        [TestMethod]
        public void Analyze_RealMultiEpisodeScenarios()
        {
            var root = FindRepoRoot();
            var lantern = Path.Combine(root, "Content", "lantern");
            var lanternFlow = new ScenarioFlowAnalyzer().Analyze(lantern);

            Assert.AreEqual(0, lanternFlow.Warnings.Count, string.Join("\n", lanternFlow.Warnings.Select(w => $"{w.File}:{w.Line} {w.Message}")));
            Assert.IsTrue(lanternFlow.Nodes.Any(n => n.Path == "01_灯の港.eve" && n.IsEntry));
            Assert.IsTrue(lanternFlow.Nodes.Any(n => n.Path == "ステージ/06_灯のゆくえ.eve"));
            Assert.IsTrue(lanternFlow.Nodes.Any(n => n.Kind == FlowNodeKind.LoadData && n.Title == "煤の軍勢"));
            Assert.AreEqual(3, lanternFlow.Edges.Count(e => e.Kind == FlowEdgeKind.IntermissionCommand));
            // 補助サブルーチンのみのファイルはノイズとして入らない
            Assert.IsFalse(lanternFlow.Nodes.Any(n => n.Path == "Lib/灯台の記録.eve"));
            Assert.IsFalse(lanternFlow.Nodes.Any(n => n.Path == "Lib/include.eve"));

            var torres = Path.Combine(root, "Compat", "torres");
            var torresFlow = new ScenarioFlowAnalyzer().Analyze(torres);

            Assert.AreEqual(0, torresFlow.Warnings.Count, string.Join("\n", torresFlow.Warnings.Select(w => $"{w.File}:{w.Line} {w.Message}")));
            Assert.IsTrue(torresFlow.Nodes.Any(n => n.Path == "ep01.eve" && n.IsEntry));
            Assert.IsTrue(torresFlow.Nodes.Any(n => n.Path == "ep08.eve"));
            Assert.IsTrue(torresFlow.Nodes.Any(n => n.Kind == FlowNodeKind.LoadData && n.Title == "moon"));
            Assert.IsTrue(torresFlow.Edges.Any(e => e.Kind == FlowEdgeKind.GameClear));
        }
    }
}
