using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCPlay;
using System.IO;
using System.Linq;

namespace SRCRender.Tests
{
    /// <summary>
    /// Load コマンドで読み込んだ作品のデータ。
    /// </summary>
    [TestClass]
    public class LoadCmdTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
            workspace.WriteMap("test.map", 20, 20, (x, y) => (x + y) % 2);
            workspace.Write(Path.Combine("Data", "テスト作品", "include.eve"),
                "# テスト作品のインクルードファイル",
                "");
        }

        [TestCleanup]
        public void Cleanup()
        {
            workspace.Dispose();
        }

        [TestMethod]
        public void Load_RegistersTitle_AndDoesNotReloadIt()
        {
            // 元の SRC と同じく、読み込んだ作品は Titles に加わり、次の Load では読み込み直さない
            workspace.Write("test.eve",
                "プロローグ:",
                "ChangeMap test.map",
                "Load テスト作品",
                "Load テスト作品",
                "Wait Click",
                "Quit",
                "");
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitFor(s => runner.GUI.IsWaitingClick, "wait click");
                var titles = runner.Session.SRC.Titles;
                Assert.AreEqual(1, titles.Count(x => x == "テスト作品"), string.Join(", ", titles));
            }
        }
    }
}
