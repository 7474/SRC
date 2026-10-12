using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace SRCRender.Tests
{
    /// <summary>
    /// 文字入力・ファイル選択をホスト（IScreenDialogHost）に任せる。
    /// </summary>
    [TestClass]
    public class DialogHostTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
            workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
        }

        [TestCleanup]
        public void Cleanup()
        {
            workspace.Dispose();
        }

        [TestMethod]
        public void Input_UsesHostTextInput()
        {
            workspace.Write("test.eve",
                "プロローグ:",
                "ChangeMap test.map",
                "Input 名前 名前を入力してください 名無し",
                "Talk システム",
                "名前は$(名前)",
                "End",
                "Quit",
                "");
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                string askedMessage = null;
                string askedDefault = null;
                runner.InputHandler = (title, message, defaultValue) =>
                {
                    askedMessage = message;
                    askedDefault = defaultValue;
                    return "ジェイド";
                };
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                Assert.AreEqual("名前はジェイド", runner.WaitForMessage());
                Assert.AreEqual("名前を入力してください", askedMessage);
                Assert.AreEqual("名無し", askedDefault);
            }
        }

        [TestMethod]
        public void Input_CancelSetsEmpty()
        {
            workspace.Write("test.eve",
                "プロローグ:",
                "ChangeMap test.map",
                "Input 名前 名前を入力してください 名無し",
                "Talk システム",
                "名前は[$(名前)]",
                "End",
                "Quit",
                "");
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.InputHandler = (title, message, defaultValue) => null;
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                Assert.AreEqual("名前は[]", runner.WaitForMessage());
            }
        }
    }
}
