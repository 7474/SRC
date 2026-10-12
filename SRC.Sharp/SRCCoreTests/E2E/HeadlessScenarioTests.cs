using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using SRCCore.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SRCCore.E2E.Tests
{
    /// <summary>
    /// シナリオファイルを HeadlessGUI で実際に実行する E2E テスト。
    /// ゲームスレッドで実行した場合と、呼び出し元スレッドで直接実行した場合の GUI 呼び出しが一致することを確認する。
    /// </summary>
    [TestClass]
    public class HeadlessScenarioTests
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(20);

        private static readonly string[] Scenario =
        {
            "プロローグ:",
            "Talk システム",
            "ヘッドレステスト開始",
            "End",
            "Exit",
            "",
            "スタート:",
            "Wait Click",
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
            "",
        };

        private string appPath;

        [TestInitialize]
        public void Initialize()
        {
            appPath = Path.Combine(Path.GetTempPath(), "srcs-e2e-" + Guid.NewGuid());
            // SRC.ValidateEnvironment / LoadIni が要求する最小構成
            Directory.CreateDirectory(Path.Combine(appPath, "Bitmap", "Event"));
            Directory.CreateDirectory(Path.Combine(appPath, "Bitmap", "Map"));
            File.WriteAllBytes(Path.Combine(appPath, "Bitmap", "Map", "plain0000.bmp"), new byte[0]);
            Directory.CreateDirectory(Path.Combine(appPath, "Sound"));
            File.WriteAllBytes(Path.Combine(appPath, "Sound", "dummy.wav"), new byte[0]);
            Directory.CreateDirectory(Path.Combine(appPath, "Data", "System"));
            File.WriteAllText(Path.Combine(appPath, "Data", "System", "terrain.txt"),
                "0\r\n平地, plain\r\n陸, 1, 0, 0\r\n\r\n", new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(appPath, "test.eve"), string.Join("\r\n", Scenario), new UTF8Encoding(false));
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                Directory.Delete(appPath, true);
            }
            catch
            {
                // ignore
            }
        }

        private (SRC src, HeadlessGUI gui) CreateSrc()
        {
            var src = new SRC
            {
                SystemConfig = new LocalFileConfig { AppPath = appPath },
            };
            var fs = new LocalFileSystem();
            fs.AddPath(appPath);
            src.FileSystem = fs;

            var gui = new HeadlessGUI();
            // Ask の選択肢は1番目を選ぶ
            gui.Handle(nameof(IGUI.ListBox), _ => 1);
            gui.Install(src);
            return (src, gui);
        }

        /// <summary>
        /// Wait Click のクリック待ちに入ったら、clickAction でクリックを模擬する。
        /// </summary>
        private static Thread StartUser(HeadlessGUI gui, Action clickAction)
        {
            var user = new Thread(() =>
            {
                var deadline = DateTime.UtcNow + Timeout;
                while (DateTime.UtcNow < deadline)
                {
                    if (gui.CallsOf(nameof(IGUI.IsRButtonPressed)).Any())
                    {
                        clickAction();
                        return;
                    }
                    Thread.Sleep(10);
                }
            })
            {
                IsBackground = true,
            };
            user.Start();
            return user;
        }

        private static IList<string> Transcript(HeadlessGUI gui)
        {
            // スレッドの扱いで回数が変わる待ち関連の呼び出しは除く
            var ignore = new HashSet<string>
            {
                nameof(IGUI.Sleep),
                nameof(IGUI.DoEvents),
                nameof(IGUI.IsRButtonPressed),
                "set_MouseButton",
            };
            return gui.Calls.Where(x => !ignore.Contains(x.Member)).Select(x => x.ToString()).ToList();
        }

        private static void AssertTerminated(Exception error)
        {
            Assert.IsNotNull(error, "scenario did not terminate");
            Assert.IsInstanceOfType(error, typeof(HeadlessTerminatedException), error.ToString());
        }

        private IList<string> RunDirect()
        {
            var (src, gui) = CreateSrc();
            Exception error = null;
            var game = new Thread(() =>
            {
                try
                {
                    src.Execute(Path.Combine(appPath, "test.eve"));
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            })
            {
                IsBackground = true,
            };
            var user = StartUser(gui, () => gui.GUI.IsFormClicked = true);
            game.Start();
            Assert.IsTrue(game.Join(Timeout), "scenario timeout");
            user.Join(Timeout);
            AssertTerminated(error);
            return Transcript(gui);
        }

        [TestMethod]
        public void Scenario_Direct_DisplaysMessagesAndSelection()
        {
            var transcript = RunDirect();

            CollectionAssert.Contains(transcript.ToList(), "IGUI.DisplayMessage(システム, ヘッドレステスト開始, )");
            CollectionAssert.Contains(transcript.ToList(), "IGUI.ListBox(SRCCore.ListBoxArgs)");
            CollectionAssert.Contains(transcript.ToList(), "IGUI.DisplayMessage(システム, はいが選ばれた, )");
            Assert.AreEqual("IGUI.Terminate()", transcript.Last());
        }

        [TestMethod]
        public void Scenario_OnGameThread_MarshalsGUICallsToUIThreadAndMatchesDirectRun()
        {
            var expected = RunDirect();

            using (var ui = new UIThreadSimulator())
            {
                var (src, gui) = CreateSrc();
                var threading = ui.Invoke(() => new CoreThreading(ui.Context));
                Exception error = null;
                using (var finished = new ManualResetEventSlim())
                {
                    threading.Game.UnhandledException += (s, e) =>
                    {
                        error = e.Exception;
                        finished.Set();
                    };
                    threading.Install(src);
                    threading.Start();
                    // クリックは UI スレッドから行う
                    var user = StartUser(gui, () => ui.Invoke(() => gui.GUI.IsFormClicked = true));
                    threading.Post(() => src.Execute(Path.Combine(appPath, "test.eve")));

                    Assert.IsTrue(finished.Wait(Timeout), "scenario timeout");
                    user.Join(Timeout);
                }
                threading.Dispose();
                AssertTerminated(error);

                // GUI はすべて UI スレッドで呼ばれ、Sleep / DoEvents は GUI へ転送されない
                var calls = gui.Calls;
                Assert.IsTrue(calls.Count > 0);
                Assert.IsTrue(calls.All(x => x.Thread == ui.Thread),
                    string.Join(Environment.NewLine, calls.Where(x => x.Thread != ui.Thread)));
                Assert.IsFalse(calls.Any(x => x.Member == nameof(IGUI.Sleep) || x.Member == nameof(IGUI.DoEvents)));

                CollectionAssert.AreEqual(expected.ToList(), Transcript(gui).ToList(),
                    "expected:" + Environment.NewLine + string.Join(Environment.NewLine, expected)
                    + Environment.NewLine + "actual:" + Environment.NewLine + string.Join(Environment.NewLine, Transcript(gui)));
            }
        }
    }
}
