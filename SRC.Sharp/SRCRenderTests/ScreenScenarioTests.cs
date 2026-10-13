using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using SRCPlay;

namespace SRCRender.Tests
{
    /// <summary>
    /// ScreenGUI でシナリオをプレイする E2E テストと VRT。
    /// </summary>
    [TestClass]
    public class ScreenScenarioTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
            // 左半分が平地、右半分が海の 20x20 マップ
            workspace.WriteMap("test.map", 20, 20, (x, y) => x <= 10 ? 0 : 1);
            workspace.Write("test.eve",
                "プロローグ:",
                "ChangeMap test.map",
                "Talk システム",
                "ヘッドレステスト開始",
                "End",
                "Ask 選択してください",
                "はい",
                "いいえ",
                "End",
                "If 選択 = 1 Then",
                "Talk システム",
                "はいが選ばれた",
                "End",
                "EndIf",
                "Quit",
                "");
        }

        [TestCleanup]
        public void Cleanup()
        {
            workspace.Dispose();
        }

        [TestMethod]
        public void PlayScenario_MessagesListAndMap()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));

                Assert.AreEqual("ヘッドレステスト開始", runner.WaitForMessage());
                Assert.IsTrue(runner.State.ShowMap, "map is visible");
                VisualAssert.MatchesBaseline(runner.SnapshotPng(), "scenario-01-message");
                runner.ClickMessage();

                var list = runner.WaitForList();
                Assert.AreEqual("選択", list.Caption);
                CollectionAssert.AreEqual(new[] { "はい", "いいえ" }, new[] { list.Items[0].Text, list.Items[1].Text });
                VisualAssert.MatchesBaseline(runner.SnapshotPng(), "scenario-02-list");
                runner.ChooseListItem(0);

                Assert.AreEqual("はいが選ばれた", runner.WaitForMessage());
                VisualAssert.MatchesBaseline(runner.SnapshotPng(), "scenario-03-after-choice");
                runner.ClickMessage();

                runner.WaitForTermination();
                Assert.IsTrue(runner.IsTerminated);
            }
        }

        [TestMethod]
        public void PlayScenario_RightClickOnAskWithoutCancelOption_ShowsListAgain()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitForMessage();
                runner.ClickMessage();

                // キャンセル可オプションの無い Ask は、右クリックしても選択肢を出し直す
                var first = runner.WaitForList();
                runner.GUI.PointerPressed(10, 10, SRCCore.GuiButton.Right);
                runner.GUI.PointerReleased(10, 10, SRCCore.GuiButton.Right);
                runner.WaitFor(s => s.List != null && s.List != first, "list shown again");

                // 「いいえ」を選ぶとメッセージを出さずに終了する
                runner.ChooseListItem(1);
                runner.WaitForTermination();
                Assert.IsTrue(runner.IsTerminated);
            }
        }
    
        [TestMethod]
        public void VisualAssert_DetectsDifference()
        {
            if (!VisualAssert.IsBaselineEnvironment)
            {
                Assert.Inconclusive("VRT の基準環境ではありません");
            }
            if (VisualAssert.IsUpdateMode)
            {
                // 基準画像を上書きしてしまうため、更新モードでは実行しない
                Assert.Inconclusive("基準画像の更新中です");
            }
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitForMessage();
                // 基準画像と異なる画面（メッセージ送り後の選択肢）と比較すると失敗する
                runner.ClickMessage();
                runner.WaitForList();
                var png = runner.SnapshotPng();
                Assert.ThrowsExactly<AssertFailedException>(() => VisualAssert.MatchesBaseline(png, "scenario-01-message"));
            }
        }
    }
}
