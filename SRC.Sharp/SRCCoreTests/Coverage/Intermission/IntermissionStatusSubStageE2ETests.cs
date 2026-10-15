using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// シナリオから Continue でインターミッションに入り、ステータス表示コマンドを選ぶまでを HeadlessGUI で実行する。
    /// ステータス表示（Lib\ユニットステータス表示.eve など）はサブステージとして始まり、
    /// Continue で隠したメインウィンドウを再び表示しなければならない（元の SRC の StartScenario の MainForm.Show）。
    /// </summary>
    [TestClass]
    public class IntermissionStatusSubStageE2ETests
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(20);

        private string appPath;

        [TestInitialize]
        public void Initialize()
        {
            appPath = Path.Combine(Path.GetTempPath(), "srcs-im-e2e-" + Guid.NewGuid());
            // SRC.ValidateEnvironment / LoadIni が要求する最小構成
            Directory.CreateDirectory(Path.Combine(appPath, "Bitmap", "Event"));
            Directory.CreateDirectory(Path.Combine(appPath, "Bitmap", "Map"));
            File.WriteAllBytes(Path.Combine(appPath, "Bitmap", "Map", "plain0000.bmp"), new byte[0]);
            Directory.CreateDirectory(Path.Combine(appPath, "Sound"));
            File.WriteAllBytes(Path.Combine(appPath, "Sound", "dummy.wav"), new byte[0]);
            Directory.CreateDirectory(Path.Combine(appPath, "Data", "System"));
            File.WriteAllText(Path.Combine(appPath, "Data", "System", "terrain.txt"),
                "0\r\n平地, plain\r\n陸, 1, 0, 0\r\n\r\n", new UTF8Encoding(false));
            Write("test.eve",
                "プロローグ:",
                "Exit",
                "",
                "スタート:",
                "Continue",
                "");
            Directory.CreateDirectory(Path.Combine(appPath, "Lib"));
            foreach (var name in new[] { "ユニットステータス表示", "パイロットステータス表示" })
            {
                Write(Path.Combine("Lib", name + ".eve"),
                    "プロローグ:",
                    "Exit",
                    "",
                    "スタート:",
                    "Global 表示した画面",
                    "Set 表示した画面 " + name,
                    "Quit",
                    "");
            }
        }

        private void Write(string name, params string[] lines)
        {
            File.WriteAllText(Path.Combine(appPath, name), string.Join("\r\n", lines), new UTF8Encoding(false));
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

        private (SRC src, HeadlessGUI gui, Exception error) Run(string command)
        {
            var src = new SRC
            {
                SystemConfig = new LocalFileConfig { AppPath = appPath },
            };
            var fs = new LocalFileSystem();
            fs.AddPath(appPath);
            src.FileSystem = fs;

            var gui = new HeadlessGUI();
            gui.Handle(nameof(IGUI.ListBox), args =>
            {
                var lb = (ListBoxArgs)args[0];
                if (lb.lb_caption == "コマンド")
                {
                    var index = lb.Items.Select(x => x.Text).ToList().IndexOf(command);
                    Assert.IsTrue(index >= 0, command + " がインターミッションのコマンドにない");
                    return index + 1;
                }
                return 0;
            });
            gui.Install(src);

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
            game.Start();
            Assert.IsTrue(game.Join(Timeout), "scenario timeout");
            return (src, gui, error);
        }

        private static int LastIndexOf(IList<HeadlessGUICall> calls, string member, int before = int.MaxValue)
        {
            for (var i = Math.Min(before, calls.Count) - 1; i >= 0; i--)
            {
                if (calls[i].Member == member)
                {
                    return i;
                }
            }
            return -1;
        }

        [TestMethod]
        [DataRow("ユニットステータス", "ユニットステータス表示")]
        [DataRow("パイロットステータス", "パイロットステータス表示")]
        public void StatusCommand_StartsStatusEventAndShowsMainForm(string command, string eveName)
        {
            var (src, gui, error) = Run(command);
            Assert.IsInstanceOfType(error, typeof(HeadlessTerminatedException), error?.ToString());

            // ステータス表示のイベントファイルのスタートイベントが実行された
            Assert.AreEqual(eveName, src.Expression.GetValueAsString("表示した画面"));
            StringAssert.EndsWith(src.ScenarioFileName, eveName + ".eve");

            var calls = gui.Calls.ToList();
            var listBox = LastIndexOf(calls, nameof(IGUI.ListBox));
            var hide = LastIndexOf(calls, nameof(IGUI.MainFormHide), listBox);
            var close = calls.FindIndex(listBox, x => x.Member == nameof(IGUI.CloseListBox));
            var reduce = calls.FindIndex(listBox, x => x.Member == nameof(IGUI.ReduceListBoxHeight));
            var show = calls.FindIndex(listBox, x => x.Member == nameof(IGUI.MainFormShow));
            // Continue でメインウィンドウを隠してからインターミッションのコマンドを選ぶ
            Assert.IsTrue(hide >= 0 && hide < listBox, "Continue でメインウィンドウを隠していない");
            // コマンドを選んだら、リストを閉じて縮め、メインウィンドウを再び表示する
            Assert.IsTrue(close > listBox, "インターミッションのリストを閉じていない");
            Assert.IsTrue(reduce > close);
            Assert.IsTrue(show > close, "ステータス表示でメインウィンドウを表示していない");
            // メインウィンドウの表示より後で再び隠すことはない
            Assert.AreEqual(-1, calls.FindIndex(show, x => x.Member == nameof(IGUI.MainFormHide)));
        }
    }
}
