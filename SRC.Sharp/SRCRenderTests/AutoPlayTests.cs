using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using SRCPlay;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SRCRender.Tests
{
    /// <summary>
    /// マップコマンド「オートプレイ」（コーパス E2E/autoplay のシナリオ）。
    /// オートプレイで最後までプレイする場合はコーパスの play.txt で確かめる。ここでは右ボタンで止められることを確かめる
    /// </summary>
    [TestClass]
    public class AutoPlayTests
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
        public void RightButtonStopsAutoPlay()
        {
            var source = Path.Combine(E2ECorpusTests.CorpusDirectory, "autoplay");
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
                        script.Run(new[] { "auto 60", "expect message 戦闘開始", "cell 10 1 right", "menu オートプレイ", "wait dialog", "dialog OK" });
                        // OK を押した後、ゲームのスレッドでオートプレイが始まるのを待つ
                        Assert.IsTrue(SpinWait.SpinUntil(() => script.Src.AutoPlay, TimeSpan.FromSeconds(20)), "オートプレイが始まらない");

                        // 右ボタンを押し続けると、味方フェイズで止まり、操作がプレイヤーに戻る
                        runner.GUI.PointerPressed(1, 1, GuiButton.Right);
                        script.Run(new[] { "wait ready 60" });
                        runner.GUI.PointerReleased(1, 1, GuiButton.Right);

                        Assert.IsFalse(script.Src.AutoPlay);
                        Assert.IsFalse(runner.IsFinished, "敵が残っているのでシナリオは続く");
                        Assert.AreEqual("味方", script.Src.Stage);
                        script.Run(new[] { "expect error none" });
                    }
                    catch (Exception ex) when (!(ex is AssertFailedException))
                    {
                        Assert.Fail(ex.Message + Environment.NewLine
                            + string.Join(Environment.NewLine, script.TraceLog.Skip(Math.Max(0, script.TraceLog.Count - 40)))
                            + Environment.NewLine + runner.Error);
                    }
                }
            }
        }
    }
}
