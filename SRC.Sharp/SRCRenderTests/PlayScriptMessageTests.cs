using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCPlay;
using System;
using System.IO;
using System.Linq;

namespace SRCRender.Tests
{
    /// <summary>
    /// PlayScript の expect message は、auto が見て進めたメッセージだけでなく、ほかの入力（click・停滞時に押したままのボタン）で
    /// 進んだメッセージも見つける（7474/SRC#1623。CI が遅いと、stall のボタンで Talk が auto の記録の前に進み、expect が失敗していた）
    /// </summary>
    [TestClass]
    public class PlayScriptMessageTests
    {
        private static void CopyDirectory(string from, string to)
        {
            Directory.CreateDirectory(to);
            foreach (var file in Directory.GetFiles(from))
            {
                File.Copy(file, Path.Combine(to, Path.GetFileName(file)), true);
            }
            foreach (var dir in Directory.GetDirectories(from))
            {
                CopyDirectory(dir, Path.Combine(to, Path.GetFileName(dir)));
            }
        }

        [TestMethod]
        public void ExpectMessageFindsMessageAdvancedWithoutAuto()
        {
            var source = Path.Combine(E2ECorpusTests.CorpusDirectory, "battle-message");
            using (var workspace = new ScenarioWorkspace())
            {
                CopyDirectory(source, workspace.AppPath);
                CopyDirectory(Path.Combine(Path.GetDirectoryName(E2ECorpusTests.CorpusDirectory), "Content", "basic-data", "Lib"), Path.Combine(workspace.AppPath, "Lib"));
                using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                {
                    var script = new PlayScript(runner);
                    runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                    try
                    {
                        // auto を使わずに、クリックでメッセージを進める
                        script.Run(new[]
                        {
                            "wait message 戦闘中のメッセージのテストを始める",
                            "click 240 240",
                            "wait ready 60",
                            "expect message 戦闘中のメッセージのテストを始める",
                        });
                        Assert.IsFalse(script.Messages.Any(x => x.Contains("戦闘中のメッセージのテストを始める")), "auto は進めていない");
                        Assert.IsTrue(script.ShownMessages.Any(x => x.Contains("戦闘中のメッセージのテストを始める")));
                    }
                    catch (Exception ex) when (!(ex is AssertFailedException))
                    {
                        Assert.Fail(ex.Message + Environment.NewLine + string.Join(Environment.NewLine, script.TraceLog.Skip(Math.Max(0, script.TraceLog.Count - 40))) + Environment.NewLine + runner.Error);
                    }
                }
            }
        }
    }
}
