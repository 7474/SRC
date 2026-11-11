using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using SRCPlay;
using System.IO;
using System.Linq;

namespace SRCRender.Tests
{
    /// <summary>
    /// ゲームとして実行したときのデータの読み込みの誤り（元の SRC の GUI.bas の DataErrorMessage と、各 *DataList.cls の Load の ErrorHandler。7474/SRC#1550）。
    /// 続ける誤りはダイアログを出して続け、打ち切る誤りはダイアログを出してから終了する
    /// </summary>
    [TestClass]
    public class DataErrorDialogTests
    {
        private static void WriteUnits(ScenarioWorkspace workspace, string stats, string name = "誤り機")
        {
            workspace.Write(Path.Combine("Data", "unit.txt"),
                name,
                name + ", しけんき, 試験機, 1, 1",
                "陸, 4, M, 1000, 100",
                "特殊能力なし",
                stats,
                "AAAA, a.bmp",
                "");
            workspace.Write("test.eve",
                "プロローグ:",
                "Global 到達",
                "Set 到達 1",
                "Quit",
                "");
        }

        private static (PlayScript script, ScreenScenarioRunner runner) Run(ScenarioWorkspace workspace)
        {
            var runner = new ScreenScenarioRunner(workspace.AppPath);
            var script = new PlayScript(runner);
            runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
            script.Run(new[] { "auto 60", "expect finished" });
            return (script, runner);
        }

        [TestMethod]
        public void ContinuingError_ShowsDialogAndRunsScenario()
        {
            using (var workspace = new ScenarioWorkspace())
            {
                WriteUnits(workspace, "abc, 100, 600, 80");
                var (script, runner) = Run(workspace);
                using (runner)
                {
                    var fname = Path.Combine(workspace.AppPath, "Data", "unit.txt");
                    CollectionAssert.AreEqual(new[]
                    {
                        fname + "：5行目\r\n誤り機のデータが不正です。\r\n最大ＨＰの設定が間違っています。\r\nabc, 100, 600, 80",
                    }, script.ErrorMessages.ToArray());
                    // 続けてシナリオを最後（Quit）まで進める
                    Assert.IsTrue(runner.IsTerminated);
                    Assert.IsFalse(runner.Error is InvalidSrcDataException, runner.Error?.ToString());
                    Assert.IsTrue(runner.Session.SRC.Expression.IsGlobalVariableDefined("到達"), runner.Error?.ToString());
                }
            }
        }

        [TestMethod]
        public void StoppingError_ShowsDialogAndTerminates()
        {
            using (var workspace = new ScenarioWorkspace())
            {
                WriteUnits(workspace, "3000, 100, 600, 80", name: "誤り 機");
                var (script, runner) = Run(workspace);
                using (runner)
                {
                    var fname = Path.Combine(workspace.AppPath, "Data", "unit.txt");
                    CollectionAssert.AreEqual(new[]
                    {
                        fname + "：1行目\r\n誤り 機のデータが不正です。\r\n名称に半角スペースは使用出来ません。\r\n誤り 機",
                    }, script.ErrorMessages.ToArray());
                    // ダイアログを閉じたら、シナリオを始めずに終了する（TerminateSRC）
                    Assert.IsTrue(runner.IsTerminated);
                    Assert.IsInstanceOfType<InvalidSrcDataException>(runner.Error);
                    Assert.IsFalse(runner.Session.SRC.Expression.IsGlobalVariableDefined("到達"), "シナリオを始めていない");
                }
            }
        }
    }
}
