using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace SRCRender.Tests
{
    /// <summary>
    /// 汎用グラフィック集・効果音集・地形データが無い環境でシナリオをプレイする。
    /// </summary>
    [TestClass]
    public class NoAssetsScenarioTests
    {
        [TestMethod]
        public void PlayScenario_WithoutAssets_UsesPlaceholderTerrain()
        {
            using (var workspace = new ScenarioWorkspace(withAssets: false))
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                // 地形番号 0〜3 を縞状に使うマップ。地形データは定義されていない
                workspace.WriteMap("test.map", 20, 20, (x, y) => (x + y) % 4);
                workspace.Write("test.eve",
                    "プロローグ:",
                    "ChangeMap test.map",
                    "Talk システム",
                    "素材なしで実行中",
                    "End",
                    "Quit",
                    "");

                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));

                Assert.AreEqual("素材なしで実行中", runner.WaitForMessage());
                Assert.IsTrue(runner.State.ShowMap, "map is visible");
                var src = runner.Session.SRC;
                Assert.AreEqual(20, src.Map.MapWidth);
                for (var id = 0; id < 4; id++)
                {
                    Assert.IsTrue(src.TDList.IsDefined(id), "placeholder terrain " + id);
                    Assert.AreEqual("陸", src.TDList.Item(id).Class);
                }
                VisualAssert.MatchesBaseline(runner.SnapshotPng(), "no-assets-01-message");
                runner.ClickMessage();

                runner.WaitForTermination();
                Assert.IsTrue(runner.IsTerminated);
            }
        }
    }
}
