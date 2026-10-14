using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;
using SRCPlay;

namespace SRCRender.Tests
{
    /// <summary>
    /// セーブデータからの再開。SRC#Web と同じく、シナリオのフォルダを SRC のフォルダとは別の絶対パスに置く
    /// （Linux・ブラウザの絶対パスは / から始まる）。
    /// </summary>
    [TestClass]
    public class ResumeSaveTests
    {
        [TestMethod]
        public void RestartSave_InSeparateScenarioFolder_RestoresStageAndLocalData()
        {
            var source = Path.Combine(E2ECorpusTests.CorpusDirectory, "intermission-save");
            using (var workspace = new ScenarioWorkspace())
            {
                var scenario = Path.Combine(Path.GetTempPath(), "srcs-scenario-" + Guid.NewGuid());
                try
                {
                    Copy(source, scenario);
                    // 次のステージでは、シナリオの Data（ローカルデータ）のユニットを作る
                    Directory.CreateDirectory(Path.Combine(scenario, "Data"));
                    foreach (var file in Directory.GetFiles(Path.Combine(scenario, "Data", "テスト")))
                    {
                        File.Move(file, Path.Combine(scenario, "Data", Path.GetFileName(file)));
                    }
                    File.WriteAllText(Path.Combine(scenario, "next.eve"), string.Join("\r\n",
                        "プロローグ:", "Exit", "", "スタート:", "ChangeMap test.map", "Create 敵 標的機 0 標的パイロット 1 3 3",
                        "Talk システム", "ステージ2", "End", "Quit"), new UTF8Encoding(false));

                    // ステージ1 からインターミッションを経て次のステージへ進むと、次のステージのリスタート用のデータが残る
                    using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                    {
                        var script = new PlayScript(runner);
                        runner.Start(Path.Combine(scenario, "test.eve"));
                        script.Run(File.ReadAllLines(Path.Combine(source, "play.txt")));
                    }
                    var save = Path.Combine(scenario, "_リスタート.srcq");
                    Assert.IsTrue(File.Exists(save));

                    // リスタート用のデータから再開する
                    using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                    {
                        var script = new PlayScript(runner);
                        runner.Start(save);
                        try
                        {
                            script.Run(new[] { "auto 60", "expect message ステージ2", "expect error none" });
                        }
                        catch (Exception ex)
                        {
                            Assert.Fail(ex.Message + Environment.NewLine
                                + string.Join(Environment.NewLine, script.TraceLog.Skip(Math.Max(0, script.TraceLog.Count - 20)))
                                + Environment.NewLine + runner.Error);
                        }
                    }
                }
                finally
                {
                    Directory.Delete(scenario, true);
                }
            }
        }

        private static void Copy(string from, string to)
        {
            Directory.CreateDirectory(to);
            foreach (var file in Directory.GetFiles(from))
            {
                File.Copy(file, Path.Combine(to, Path.GetFileName(file)), true);
            }
            foreach (var dir in Directory.GetDirectories(from))
            {
                Copy(dir, Path.Combine(to, Path.GetFileName(dir)));
            }
        }
    }
}
