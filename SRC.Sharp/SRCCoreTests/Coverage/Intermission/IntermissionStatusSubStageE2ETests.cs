using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
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
    /// シナリオから Continue でインターミッションに入り、インターミッションのコマンドを選ぶところから
    /// HeadlessGUI で実行する E2E テスト。
    /// コマンドを選んだ後にメインウィンドウやコマンドの一覧が戻り、操作を続けられることを確かめる
    /// （元の SRC の Intermission.bas の InterMissionCommand と SRC.bas の StartScenario）。
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
            // 味方ユニットとパイロット（ユニットリストに並べる対象）
            Write(Path.Combine("Data", "unit.txt"),
                "試験機",
                "試験機, しけんき, 汎用, 1, 1",
                "陸, 4, M, 1000, 100",
                "特殊能力なし",
                "3000, 150, 800, 70",
                "AAAA, -",
                "");
            Write(Path.Combine("Data", "pilot.txt"),
                "試験パイロット",
                "試験パイロット, しけんぱいろっと, 男性, 汎用, AAAA, 100",
                "特殊能力なし",
                "150, 150, 150, 150, 150, 150, 普通",
                "ＳＰ, 50, 加速, 1",
                "-.bmp, -.mid",
                "");
            Write("test.eve",
                "プロローグ:",
                "Exit",
                "",
                "スタート:",
                "Unit 試験機 0",
                "Pilot 試験パイロット 1",
                "Ride 試験パイロット",
                "Option アイテム交換",
                "IntermissionCommand ユニットリスト list.eve",
                "IntermissionCommand 作戦会議 stage.eve",
                "Continue",
                "");
            // プロローグだけで Continue するインターミッションコマンド（ヘルプの IntermissionCommand の例と同じ形）
            Write("list.eve",
                "プロローグ:",
                "Global 一覧表示回数",
                "Incr 一覧表示回数",
                "Talk システム",
                "ユニットリストを表示しました",
                "End",
                "Continue",
                "");
            // スタートイベントを持つインターミッションコマンド（通常のステージとして実行される）
            Write("stage.eve",
                "プロローグ:",
                "Exit",
                "",
                "スタート:",
                "Global 作戦会議を開始",
                "Set 作戦会議を開始 1",
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

        /// <summary>
        /// インターミッションのコマンド一覧を表示した時点の状態。
        /// </summary>
        private class MenuShown
        {
            public int CallIndex;
            public bool IsSubStage;
            public string Stage;
            public bool MainFormVisible;
            public List<string> Items;
        }

        private class Session
        {
            public SRC Src;
            public HeadlessGUI Gui;
            public List<MenuShown> Menus = new List<MenuShown>();
            public List<ListBoxArgs> OtherLists = new List<ListBoxArgs>();
            public bool MainFormVisible;
        }

        private static bool IsIntermissionMenu(ListBoxArgs lb)
        {
            // インターミッションのコマンド一覧は「連続表示」で、最後が「SRCを終了」
            return lb.lb_mode == "連続表示" && lb.Items.LastOrDefault()?.Text == "SRCを終了";
        }

        /// <summary>
        /// commands の順にインターミッションのコマンドを選ぶ。コマンドが尽きたら「SRCを終了」を選ぶ。
        /// インターミッション以外のリスト（機体改造のユニット一覧など）はキャンセルする。
        /// </summary>
        private Session CreateSession(params string[] commands)
        {
            var s = new Session();
            s.Src = new SRC
            {
                SystemConfig = new LocalFileConfig { AppPath = appPath },
            };
            var fs = new LocalFileSystem();
            fs.AddPath(appPath);
            s.Src.FileSystem = fs;

            s.Gui = new HeadlessGUI();
            var gui = s.Gui;
            // メインウィンドウの表示状態を実際の GUI と同じように保つ
            gui.Handle(nameof(IGUI.MainFormShow), _ => { s.MainFormVisible = true; return null; });
            gui.Handle(nameof(IGUI.MainFormHide), _ => { s.MainFormVisible = false; return null; });
            gui.Handle("get_" + nameof(IGUI.MainFormVisible), _ => s.MainFormVisible);
            gui.Handle(nameof(IGUI.Confirm), _ => GuiDialogResult.Ok);
            // マップ画面の大きさ（ステータス表示のマップの幅になる）
            gui.Handle("get_" + nameof(IGUI.MainWidth), _ => 15);
            gui.Handle("get_" + nameof(IGUI.MainHeight), _ => 15);
            gui.Handle(nameof(IGUI.ListBox), args =>
            {
                var lb = (ListBoxArgs)args[0];
                if (!IsIntermissionMenu(lb))
                {
                    s.OtherLists.Add(lb);
                    return 0;
                }
                var items = lb.Items.Select(x => x.Text).ToList();
                s.Menus.Add(new MenuShown
                {
                    CallIndex = gui.Calls.Count - 1,
                    IsSubStage = s.Src.IsSubStage,
                    Stage = s.Src.Stage,
                    MainFormVisible = s.MainFormVisible,
                    Items = items,
                });
                var command = s.Menus.Count <= commands.Length ? commands[s.Menus.Count - 1] : "SRCを終了";
                var index = items.IndexOf(command);
                Assert.IsTrue(index >= 0, command + " がインターミッションのコマンドにない: " + string.Join(",", items));
                return index + 1;
            });
            gui.Install(s.Src);
            return s;
        }

        private static Exception RunOnThread(Action action)
        {
            Exception error = null;
            var game = new Thread(() =>
            {
                try
                {
                    action();
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
            return error;
        }

        private Exception Execute(Session s)
        {
            return RunOnThread(() => s.Src.Execute(Path.Combine(appPath, "test.eve")));
        }

        private static void AssertTerminated(Exception error)
        {
            Assert.IsNotNull(error, "SRCを終了していない");
            Assert.IsInstanceOfType(error, typeof(HeadlessTerminatedException), error.ToString());
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

        private static int IndexOf(IList<HeadlessGUICall> calls, string member, int from, int to = int.MaxValue)
        {
            for (var i = from; i < Math.Min(to, calls.Count); i++)
            {
                if (calls[i].Member == member)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 「SRCを終了」を選ぶと、リストを閉じて縮めてから終了する。
        /// </summary>
        private static void AssertExitedFromLastMenu(Session s, Exception error)
        {
            AssertTerminated(error);
            var calls = s.Gui.Calls.ToList();
            var last = s.Menus.Last().CallIndex;
            var close = IndexOf(calls, nameof(IGUI.CloseListBox), last);
            var reduce = IndexOf(calls, nameof(IGUI.ReduceListBoxHeight), last);
            var terminate = IndexOf(calls, nameof(IGUI.Terminate), last);
            Assert.IsTrue(close > last && reduce > close && terminate > reduce, "SRCを終了でリストを閉じてから終了していない");
        }

        [TestMethod]
        [DataRow("ユニットステータス", "ユニットステータス表示")]
        [DataRow("パイロットステータス", "パイロットステータス表示")]
        public void StatusCommand_StartsStatusEventAndShowsMainForm(string command, string eveName)
        {
            var s = CreateSession(command);
            var error = Execute(s);
            AssertTerminated(error);

            // ステータス表示のイベントファイルのスタートイベントが実行された
            Assert.AreEqual(eveName, s.Src.Expression.GetValueAsString("表示した画面"));
            StringAssert.EndsWith(s.Src.ScenarioFileName, eveName + ".eve");

            var calls = s.Gui.Calls.ToList();
            var listBox = s.Menus.Single().CallIndex;
            var hide = LastIndexOf(calls, nameof(IGUI.MainFormHide), listBox);
            var close = IndexOf(calls, nameof(IGUI.CloseListBox), listBox);
            var reduce = IndexOf(calls, nameof(IGUI.ReduceListBoxHeight), listBox);
            var show = IndexOf(calls, nameof(IGUI.MainFormShow), listBox);
            // Continue でメインウィンドウを隠してからインターミッションのコマンドを選ぶ
            Assert.IsTrue(hide >= 0 && hide < listBox, "Continue でメインウィンドウを隠していない");
            Assert.IsFalse(s.Menus[0].MainFormVisible);
            // コマンドを選んだら、リストを閉じて縮め、メインウィンドウを再び表示する
            Assert.IsTrue(close > listBox, "インターミッションのリストを閉じていない");
            Assert.IsTrue(reduce > close);
            Assert.IsTrue(show > close, "ステータス表示でメインウィンドウを表示していない");
            // メインウィンドウの表示より後で再び隠すことはない
            Assert.AreEqual(-1, IndexOf(calls, nameof(IGUI.MainFormHide), show));
            Assert.IsTrue(s.MainFormVisible);
        }

        [TestMethod]
        public void UnitStatus_WithBundledLibEvent_ListsUnitsAndReturnsToIntermissionByMapCommand()
        {
            // 同梱の Lib\ユニットステータス表示.eve（MakeUnitList でユニットリストを作る）で、
            // ユニットリストを表示 → マップコマンド「インターミッションに戻る」→ コマンド一覧に戻る、までを通す
            var libDir = FindBundledLib();
            File.Copy(Path.Combine(libDir, "ユニットステータス表示.eve"),
                Path.Combine(appPath, "Lib", "ユニットステータス表示.eve"), true);
            var s = CreateSession("ユニットステータス");
            var drawn = new List<string>();
            s.Gui.Handle(nameof(IGUI.DrawString), args => { drawn.Add((string)args[0]); return null; });
            IList<UiCommand> mapCommands = null;
            s.Gui.Handle(nameof(IGUI.ShowMapCommandMenu), args => { mapCommands = ((IList<UiCommand>)args[0]).ToList(); return null; });

            // ステータス表示のステージは味方フェイズでプレイヤーの操作を待つ（Execute から戻る）
            var error = Execute(s);
            Assert.IsNull(error, error?.ToString());
            Assert.AreEqual(1, s.Menus.Count);
            Assert.IsTrue(s.MainFormVisible, "ユニットリストを表示するのにメインウィンドウが隠れたまま");
            Assert.AreEqual("味方", s.Src.Stage);
            Assert.IsFalse(s.Src.IsSubStage);
            Assert.IsTrue(s.Src.Map.IsStatusView);
            // 味方ユニットがユニットリストに並ぶ
            CollectionAssert.Contains(drawn, "試験機");
            var su = s.Src.UList.Items.Single();
            Assert.AreEqual("出撃", su.Status);
            Assert.AreEqual((1, 1), (su.x, su.y));

            // 右クリックでマップコマンドを開くと、イベントで定義したコマンドが並ぶ
            var openMenu = RunOnThread(() => s.Src.Commands.ProceedInput(GuiButton.Right, null, null));
            Assert.IsNull(openMenu, openMenu?.ToString());
            Assert.IsNotNull(mapCommands, "マップコマンドを表示していない");
            var labels = mapCommands.Select(x => x.Label).ToList();
            CollectionAssert.Contains(labels, "並べ替え");
            var back = mapCommands.Single(x => x.Label == "インターミッションに戻る");

            // インターミッションに戻ると、再びコマンド一覧を表示して操作を続けられる
            var backError = RunOnThread(() => s.Src.Commands.MapCommand(back));
            Assert.AreEqual(2, s.Menus.Count, "インターミッションのコマンド一覧に戻っていない");
            Assert.AreEqual("インターミッション", s.Menus[1].Stage);
            Assert.IsFalse(s.Menus[1].IsSubStage);
            // 戻る Continue でメインウィンドウを隠す（元の SRC の Continue と同じ）
            Assert.IsFalse(s.Menus[1].MainFormVisible);
            AssertExitedFromLastMenu(s, backError);
        }

        [TestMethod]
        public void UserDefinedCommand_PrologueOnly_ResumesIntermission()
        {
            // IntermissionCommand で追加したコマンド（プロローグで処理して Continue）を選ぶと、
            // リストを閉じて縮めてからイベントファイルを実行し、
            // サブステージのまま戻ってくればインターミッションを再開してコマンド一覧を再び表示する
            var s = CreateSession("ユニットリスト");
            var error = Execute(s);
            AssertExitedFromLastMenu(s, error);

            Assert.AreEqual(2, s.Menus.Count, "インターミッションのコマンド一覧に戻っていない");
            CollectionAssert.Contains(s.Menus[0].Items, "ユニットリスト");
            // ユーザー定義コマンドは「ユニットステータス」の後、「SRCを終了」の前
            Assert.IsTrue(s.Menus[0].Items.IndexOf("ユニットリスト") > s.Menus[0].Items.IndexOf("ユニットステータス"));
            Assert.AreEqual(1d, s.Src.Expression.GetValueAsDouble("一覧表示回数"));
            StringAssert.EndsWith(s.Src.ScenarioFileName, "list.eve");

            var calls = s.Gui.Calls.ToList();
            var first = s.Menus[0].CallIndex;
            var second = s.Menus[1].CallIndex;
            var close = IndexOf(calls, nameof(IGUI.CloseListBox), first, second);
            var reduce = IndexOf(calls, nameof(IGUI.ReduceListBoxHeight), first, second);
            var talk = IndexOf(calls, nameof(IGUI.DisplayMessage), first, second);
            var enlarge = LastIndexOf(calls, nameof(IGUI.EnlargeListBoxHeight), second);
            Assert.IsTrue(close > first && reduce > close, "リストを閉じて縮めていない");
            Assert.IsTrue(talk > reduce, "イベントファイルのプロローグを実行していない");
            // 再開時にリストを拡大し直してからコマンド一覧を表示する
            Assert.IsTrue(enlarge > talk, "インターミッション再開時にリストを拡大していない");
            Assert.IsFalse(s.Menus[1].IsSubStage, "サブステージのままインターミッションを再開している");
            CollectionAssert.AreEqual(s.Menus[0].Items, s.Menus[1].Items);
        }

        [TestMethod]
        public void UserDefinedCommand_WithStartEvent_RunsAsNormalStageAndShowsMainForm()
        {
            // スタートイベントがあれば通常のステージとして実行し、メインウィンドウを表示する。
            // そのステージの Continue で改めてインターミッションに入る
            var s = CreateSession("作戦会議");
            var error = Execute(s);
            AssertExitedFromLastMenu(s, error);

            Assert.AreEqual(1d, s.Src.Expression.GetValueAsDouble("作戦会議を開始"));
            Assert.AreEqual(2, s.Menus.Count);
            var calls = s.Gui.Calls.ToList();
            var first = s.Menus[0].CallIndex;
            var second = s.Menus[1].CallIndex;
            var show = IndexOf(calls, nameof(IGUI.MainFormShow), first, second);
            var hide = IndexOf(calls, nameof(IGUI.MainFormHide), show, second);
            Assert.IsTrue(show > IndexOf(calls, nameof(IGUI.CloseListBox), first, second), "スタートイベントの前にメインウィンドウを表示していない");
            Assert.IsTrue(hide > show, "Continue でメインウィンドウを隠していない");
            Assert.AreEqual("インターミッション", s.Menus[1].Stage);
            Assert.IsFalse(s.Menus[1].IsSubStage);
        }

        [TestMethod]
        public void RankUpAndExchangeItem_Cancelled_ReturnToCommandList()
        {
            // 機体改造・アイテム交換のユニット一覧をキャンセルすると、コマンド一覧に戻る
            var s = CreateSession("機体改造", "アイテム交換");
            var error = Execute(s);
            AssertExitedFromLastMenu(s, error);

            Assert.AreEqual(3, s.Menus.Count);
            Assert.AreEqual(2, s.OtherLists.Count);
            StringAssert.StartsWith(s.OtherLists[0].lb_caption, "ユニット選択： ");
            StringAssert.StartsWith(s.OtherLists[1].lb_caption, "アイテムを交換するユニットを選択");
            // それぞれのユニット一覧に味方ユニットが並んでいた
            Assert.IsTrue(s.OtherLists.All(lb => lb.Items.Any(x => x.Text?.Contains("試験機") == true)));
            // キャンセルではリストを閉じず、メインウィンドウも隠したまま
            var calls = s.Gui.Calls.ToList();
            Assert.AreEqual(-1, IndexOf(calls, nameof(IGUI.CloseListBox), s.Menus[0].CallIndex, s.Menus[2].CallIndex));
            Assert.IsTrue(s.Menus.All(m => m.Stage == "インターミッション" && !m.IsSubStage));
        }

        private static string FindBundledLib()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                var lib = Path.Combine(dir.FullName, "Content", "basic-data", "Lib");
                if (File.Exists(Path.Combine(lib, "ユニットステータス表示.eve")))
                {
                    return lib;
                }
                dir = dir.Parent;
            }
            Assert.Inconclusive("Content/basic-data/Lib が見つからない");
            return null;
        }
    }
}
