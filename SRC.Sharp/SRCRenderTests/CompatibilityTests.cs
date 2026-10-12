using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace SRCRender.Tests
{
    /// <summary>
    /// 実際のシナリオ（ロボッとダンジョンＢ！など）で見つかった、元の SRC との挙動差の回帰テスト。
    /// </summary>
    [TestClass]
    public class CompatibilityTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
        }

        [TestCleanup]
        public void Cleanup()
        {
            workspace.Dispose();
        }

        private string Play(params string[] lines)
        {
            workspace.Write("test.eve", lines);
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                return runner.WaitForMessage();
            }
        }

        [TestMethod]
        public void CallFunction_PassesArgumentsWithoutLabel()
        {
            // 式中の Call(ラベル, 引数...) では Args(1) が 1 番目の引数（ラベル名ではない）
            Assert.AreEqual("A1=0 A2=480 A3=難易度 R=難易度", Play(
                "プロローグ:",
                "Set R Call(連結, 0, 480, \"難易度\")",
                "Talk システム",
                "A1=$(A1) A2=$(A2) A3=$(A3) R=$(R)",
                "End",
                "Quit",
                "",
                "連結:",
                "Set A1 Args(1)",
                "Set A2 Args(2)",
                "Set A3 Args(3)",
                "Return Args(3)"));
        }

        [TestMethod]
        public void CallFunction_InsideCallCommandArguments()
        {
            // Call コマンドの引数に Call 関数を書いた場合（RD_文字作成_フチドリ Call(RD_字幕中間座標, ...) ...）
            Assert.AreEqual("X=150 Y=24 T=タイトル", Play(
                "プロローグ:",
                "Call 表示 Call(中間, 0, 480, 180) (20 + 4) \"タイトル\"",
                "Quit",
                "",
                "表示:",
                "Talk システム",
                "X=$(Args(1)) Y=$(Args(2)) T=$(Args(3))",
                "End",
                "Return",
                "",
                "中間:",
                "Return Int(Args(1) + ((Args(2) - Args(1) - Args(3)) / 2))"));
        }
    }
}
