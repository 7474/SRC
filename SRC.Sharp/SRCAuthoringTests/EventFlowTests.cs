using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCAuthoring.Analysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCAuthoring.Tests
{
    [TestClass]
    public class EventFlowTests
    {
        [TestMethod]
        public void EventFlow_TracksTriggersCallsGotosRequireAndConditions()
        {
            using (var scenario = new TestScenario())
            {
                scenario.Write("sub.eve",
                    "外部サブルーチン:",
                    "Return");

                var eve = scenario.Write("ep01.eve",
                    "プロローグ:",
                    "Require sub.eve",
                    "Exit",
                    "",
                    "スタート:",
                    "If 変数 = 1 Then",
                    "Call 味方会話",
                    "Else",
                    "Call 外部サブルーチン",
                    "EndIf",
                    "Exit",
                    "",
                    "ターン 1 味方:",
                    "Switch モード",
                    "Case 1",
                    "Goto 勝利イベント",
                    "EndSw",
                    "Exit",
                    "",
                    "味方会話:",
                    "Return",
                    "",
                    "勝利イベント:",
                    "Call 無いラベル",
                    "Return",
                    "",
                    "マップコマンド テストコマンド:",
                    "Return");

                var graph = new EventFlowAnalyzer().Analyze(eve);

                Assert.IsTrue(graph.Nodes.Any(n => n.Name == "プロローグ" && n.Kind == EventNodeKind.Entry));
                Assert.IsTrue(graph.Nodes.Any(n => n.Name == "スタート" && n.Kind == EventNodeKind.Entry));
                Assert.IsTrue(graph.Nodes.Any(n => n.Name == "ターン 1 味方" && n.Kind == EventNodeKind.Trigger));
                Assert.IsTrue(graph.Nodes.Any(n => n.Name == "マップコマンド テストコマンド" && n.Kind == EventNodeKind.Command));
                Assert.IsTrue(graph.Nodes.Any(n => n.Name == "味方会話" && n.Kind == EventNodeKind.Subroutine));
                Assert.IsTrue(graph.Nodes.Any(n => n.Name == "sub.eve" && n.Kind == EventNodeKind.RequireFile && n.Exists));
                Assert.IsTrue(graph.Nodes.Any(n => n.Name == "外部サブルーチン" && n.Required));
                Assert.IsTrue(graph.Nodes.Any(n => n.Name == "無いラベル" && n.Kind == EventNodeKind.Missing && !n.Exists));

                var requireEdge = graph.Edges.FirstOrDefault(e => e.Kind == EventEdgeKind.Require && e.Target == "sub.eve");
                Assert.IsNotNull(requireEdge);

                var callCond = graph.Edges.FirstOrDefault(e => e.Kind == EventEdgeKind.Call && e.Target == "味方会話");
                Assert.IsNotNull(callCond);
                StringAssert.Contains(callCond.Condition ?? "", "変数 = 1");

                var callElse = graph.Edges.FirstOrDefault(e => e.Kind == EventEdgeKind.Call && e.Target == "外部サブルーチン");
                Assert.IsNotNull(callElse);
                StringAssert.Contains(callElse.Condition ?? "", "Else");

                var gotoSwitch = graph.Edges.FirstOrDefault(e => e.Kind == EventEdgeKind.Goto && e.Target == "勝利イベント");
                Assert.IsNotNull(gotoSwitch);
                StringAssert.Contains(gotoSwitch.Condition ?? "", "モード");

                Assert.IsTrue(graph.Warnings.Any(w => w.Target == "無いラベル"));

                var mmd = graph.ToMermaid();
                StringAssert.Contains(mmd, "flowchart TD");
                StringAssert.Contains(mmd, "味方会話");

                var md = graph.ToMarkdown();
                StringAssert.Contains(md, "# イベントの流れ");
                StringAssert.Contains(md, "Call・Goto・Require");

                var json = graph.ToJson();
                StringAssert.Contains(json, "\"nodes\"");
                StringAssert.Contains(json, "\"edges\"");
                StringAssert.Contains(json, "\"warnings\"");
            }
        }

        [TestMethod]
        public void EventFlow_UsesUnsavedBufferOverlay()
        {
            using (var scenario = new TestScenario())
            {
                var eve = scenario.Write("stage.eve",
                    "スタート:",
                    "Exit",
                    "",
                    "元のサブルーチン:",
                    "Return");

                var overlays = new Dictionary<string, string>
                {
                    [eve] = string.Join("\r\n",
                        "スタート:",
                        "Call 未保存サブルーチン",
                        "Exit",
                        "",
                        "未保存サブルーチン:",
                        "Return"),
                };

                var graph = new EventFlowAnalyzer().Analyze(eve, overlays);
                Assert.IsTrue(graph.Nodes.Any(n => n.Name == "未保存サブルーチン"));
                Assert.IsTrue(graph.Edges.Any(e => e.Kind == EventEdgeKind.Call && e.Target == "未保存サブルーチン"));
                Assert.AreEqual(0, graph.Warnings.Count);
            }
        }
    }
}
