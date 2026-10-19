using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCPlay;
using System.IO;

namespace SRCRender.Tests
{
    /// <summary>
    /// サブフォルダのイベントファイルを、シナリオのフォルダを基準に動かす（ScreenScenarioRunner.Start の scenarioRoot）。
    /// </summary>
    [TestClass]
    public class ScenarioRootTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
            workspace.WriteMap("root.map", 12, 10, (x, y) => 0);
            workspace.Write(Path.Combine("Lib", "共通.eve"),
                "共通の挨拶:",
                "Talk システム",
                "共通",
                "End",
                "Return",
                "");
            workspace.Write(Path.Combine("章", "test.eve"),
                "プロローグ:",
                "Require Lib\\共通.eve",
                "Call 共通の挨拶",
                "ChangeMap root.map",
                "Talk システム",
                "地図",
                "End",
                "Quit",
                "");
            // 章のマップ（WriteMap はフォルダを作らないので、イベントファイルの後に書く）
            workspace.WriteMap(Path.Combine("章", "test.map"), 20, 20, (x, y) => (x + y) % 2);
        }

        [TestCleanup]
        public void Cleanup()
        {
            workspace.Dispose();
        }

        [TestMethod]
        public void Start_WithScenarioRoot_ResolvesLibAndMapsFromRoot()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "章", "test.eve"), workspace.AppPath);
                Assert.AreEqual("共通", runner.WaitForMessage());
                Assert.AreEqual(workspace.AppPath, runner.Session.SRC.ScenarioPath);
                runner.ClickMessage();
                Assert.AreEqual("地図", runner.WaitForMessage());
                // ChangeMap root.map はシナリオのフォルダから読む
                Assert.AreEqual(12, runner.Session.SRC.Map.MapWidth);
            }
        }

        [TestMethod]
        public void Start_WithoutScenarioRoot_UsesTheFileFolder()
        {
            // 元の SRC と同じく、イベントファイルのあるフォルダが基準になる
            workspace.Write(Path.Combine("章", "plain.eve"),
                "プロローグ:",
                "Talk システム",
                "こんにちは",
                "End",
                "Quit",
                "");
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "章", "plain.eve"));
                Assert.AreEqual("こんにちは", runner.WaitForMessage());
                Assert.AreEqual(Path.Combine(workspace.AppPath, "章"), runner.Session.SRC.ScenarioPath);
            }
        }
    }
}
