using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using SRCCore.Events;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// VB: Event.bas の LoadEventData・LoadEventData2・AddSysLabel と LabelData.cls の動作に合わせたことのテスト（#1498）。
    /// </summary>
    [TestClass]
    public class EventDataLabelVBMatchTests
    {
        private string appPath;

        [TestInitialize]
        public void Initialize()
        {
            appPath = Path.Combine(Path.GetTempPath(), "srcs-edl-" + Guid.NewGuid());
            Directory.CreateDirectory(appPath);
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

        private static void WriteEve(string path, params string[] lines)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, string.Join("\r\n", lines) + "\r\n", new UTF8Encoding(false));
        }

        private (SRC src, MockGUI gui) CreateSrc()
        {
            var gui = new MockGUI();
            var src = new SRC
            {
                GUI = gui,
                SystemConfig = new LocalFileConfig { AppPath = appPath },
            };
            var fs = new LocalFileSystem();
            fs.AddPath(appPath);
            src.FileSystem = fs;
            src.ScenarioPath = appPath;
            src.Event.SkipExternalSourceLoad = true;
            return (src, gui);
        }

        private (SRC src, MockGUI gui) Load(params string[] lines)
        {
            WriteEve(Path.Combine(appPath, "test.eve"), lines);
            var (src, gui) = CreateSrc();
            src.Event.InitEventData();
            src.Event.LoadEventData(Path.Combine(appPath, "test.eve"));
            return (src, gui);
        }

        private static string[] ErrorMessages(SRC src)
        {
            return src.DataErrors.Select(x => x.msg).ToArray();
        }

        // VB: LabelData.Data の ListLength が -1（括弧の対応が取れていない）なら、
        // ラベルを登録している行（CurrentLineNum）でエラーを表示する
        [TestMethod]
        [Timeout(20000)]
        public void LoadEventData_LabelWithUnbalancedParen_ReportsErrorAtLabelLine()
        {
            var (src, _) = Load(
                "スタート:",
                "Exit",
                "テスト (a:",
                "Exit");

            var error = src.DataErrors.FirstOrDefault(x => x.msg == "ラベルの引数の括弧の対応が取れていません");
            Assert.IsNotNull(error, string.Join(",", ErrorMessages(src)));
            Assert.AreEqual("テスト (a:", error.line_buf);
            Assert.AreEqual(3, error.line_num);
        }

        // VB: Talk 中の空行（NopCmd）の次の行が半角文字だけの語（Talk・End・Suspend 以外）なら
        // Talk に対応する End が無いとみなす
        [TestMethod]
        [Timeout(20000)]
        public void LoadEventData_BlankLineInTalkFollowedByCommand_ReportsMissingEnd()
        {
            var (src, gui) = Load(
                "スタート:",
                "Talk アリス",
                "こんにちは",
                "",
                "Set x 1",
                "End",
                "Exit");

            var error = src.DataErrors.FirstOrDefault(x => x.msg == "Talkに対応するEndがありません");
            Assert.IsNotNull(error, string.Join(",", ErrorMessages(src)));
            Assert.AreEqual("Talk アリス", error.line_buf);
            Assert.IsTrue(gui.TerminateCalled);
        }

        [TestMethod]
        [Timeout(20000)]
        public void LoadEventData_BlankLineInTalkFollowedByEnd_IsNotError()
        {
            var (src, gui) = Load(
                "スタート:",
                "Talk アリス",
                "こんにちは",
                "",
                "End",
                "Exit");

            CollectionAssert.AreEqual(new string[0], ErrorMessages(src));
            Assert.IsFalse(gui.TerminateCalled);
        }

        [TestMethod]
        [Timeout(20000)]
        public void LoadEventData_BlankLineInTalkFollowedByMessage_IsNotError()
        {
            // 次の行の最初の語に全角文字があればメッセージとみなす
            var (src, gui) = Load(
                "スタート:",
                "Talk アリス",
                "こんにちは",
                "",
                "さようなら",
                "End",
                "Exit");

            CollectionAssert.AreEqual(new string[0], ErrorMessages(src));
            Assert.IsFalse(gui.TerminateCalled);
        }

        [TestMethod]
        [Timeout(20000)]
        public void LoadEventData_BlankLineInTalkFollowedByTalk_IsNotError()
        {
            var (src, gui) = Load(
                "スタート:",
                "Talk アリス",
                "こんにちは",
                "",
                "Talk ボブ",
                "やあ",
                "End",
                "Exit");

            CollectionAssert.AreEqual(new string[0], ErrorMessages(src));
            Assert.IsFalse(gui.TerminateCalled);
        }

        // VB: Exit・PlaySound・Wait が Talk などの中にあれば「Talkに対応するEndがありません」
        [TestMethod]
        [Timeout(20000)]
        public void LoadEventData_ExitInsideTalk_ReportsVBMessage()
        {
            var (src, gui) = Load(
                "スタート:",
                "Talk アリス",
                "こんにちは",
                "Exit");

            var error = src.DataErrors.FirstOrDefault(x => x.msg == "Talkに対応するEndがありません");
            Assert.IsNotNull(error, string.Join(",", ErrorMessages(src)));
            Assert.AreEqual("Talk アリス", error.line_buf);
            Assert.IsTrue(gui.TerminateCalled);
        }

        // VB: 主なコマンドの引数の数のチェック（書式チェックその２）はシナリオ側の行だけに行う
        [TestMethod]
        [Timeout(20000)]
        public void LoadEventData_DoesNotCheckArgumentCountOfSystemData()
        {
            WriteEve(Path.Combine(appPath, "Lib", "include.eve"),
                "システム側:",
                "Pilot a",
                "Return");

            var (src, gui) = Load(
                "スタート:",
                "Exit");

            CollectionAssert.AreEqual(new string[0], ErrorMessages(src));
            Assert.IsFalse(gui.TerminateCalled);
        }

        // VB: システム側のロード（load_mode = "システム"）ではラベルの末尾の全角文字を調べない
        [TestMethod]
        [Timeout(20000)]
        public void InitEventData_DoesNotCheckFullWidthColonOfSystemData()
        {
            WriteEve(Path.Combine(appPath, "Lib", "スペシャルパワー.eve"),
                "システム側：",
                "Return");

            var (src, _) = CreateSrc();
            src.Event.InitEventData();

            CollectionAssert.AreEqual(new string[0], ErrorMessages(src));
        }

        // VB: LoadEventData2 は、読み込んだ行が無いうちのエラーなら「が開けません」と表示する
        [TestMethod]
        [Timeout(20000)]
        [DoNotParallelize]
        public void LoadEventData2_MissingFile_ReportsCannotOpen()
        {
            var (src, gui) = CreateSrc();
            src.Event.InitEventData();
            var fname = Path.Combine(appPath, "missing.eve");

            var original = Console.Error;
            var sw = new StringWriter();
            Console.SetError(sw);
            try
            {
                src.Event.LoadEventData2(fname, EventDataSource.Scenario);
            }
            catch
            {
                // SRC# は TerminateSRC の後に例外を投げ直す
            }
            finally
            {
                Console.SetError(original);
            }

            StringAssert.Contains(sw.ToString(), fname + "が開けません");
            Assert.IsTrue(gui.TerminateCalled);
        }

        // VB: AddSysLabel は On Error で、キーの重複（エラー 457）を無視する
        [TestMethod]
        public void AddSysLabel_DuplicateKeyWithLineNumber_IsIgnored()
        {
            var (src, _) = CreateSrc();
            src.Event.AddSysLabel("破壊 A(6)", 1);
            src.Event.AddSysLabel("破壊 A", 5);
            src.Event.AddSysLabel("破壊 A", 6);

            Assert.AreEqual(1, src.Event.FindEventLabel("破壊 A(6)"));
            Assert.AreEqual(5, src.Event.FindEventLabel("破壊 A"));
        }

        // VB: LabelData.Para は範囲外なら空文字列（String の既定値）、1 番目は GetValueAsString("") を返す
        [TestMethod]
        public void LabelDataPara_OutOfRangeOrFirst_ReturnsEmpty()
        {
            var (src, _) = CreateSrc();
            var ld = new LabelData(src) { Data = "破壊 味方" };
            Assert.AreEqual("", ld.Para(3));
            Assert.AreEqual("", ld.Para(1));
        }

        // VB: ListLength が -1 なら CountPara は -1 で、パラメータは空文字列
        [TestMethod]
        public void LabelData_UnclosedQuote_CountParaIsMinusOne()
        {
            var (src, _) = CreateSrc();
            src.Event.InitEventData();
            var ld = new LabelData(src) { Data = "会話 \"A B" };
            Assert.AreEqual(-1, ld.CountPara());
            Assert.AreEqual(LabelType.TalkEventLabel, ld.Name);
            Assert.AreEqual("", ld.Para(2));
        }
        // VB: 括弧の閉じていない行（ArgNum = -1）は「括弧の対応が取れていません」を出す。Ask の行なら、
        // 後ろの引数を読むループに入らず、選択肢の始まり（End で閉じる）として扱う（Event.bas の LoadEventData）
        [TestMethod]
        [Timeout(20000)]
        public void LoadEventData_AskWithUnbalancedParen_ReportsErrorAndKeepsBlock()
        {
            var (src, gui) = Load(
                "スタート:",
                "Ask (選んでください",
                "はい",
                "いいえ",
                "End",
                "Exit");

            CollectionAssert.AreEqual(new[] { "括弧の対応が取れていません" }, ErrorMessages(src));
            Assert.AreEqual("Ask (選んでください", src.DataErrors[0].line_buf);
            Assert.IsTrue(gui.TerminateCalled);
        }

        // VB: Ask の後ろのオプションを読み飛ばし、「終了」があれば選択肢を続けて書かない形（End は要らない）
        [TestMethod]
        [Timeout(20000)]
        public void LoadEventData_AskWithEndOptionAfterOptions_DoesNotOpenBlock()
        {
            var (src, gui) = Load(
                "スタート:",
                "Ask 選んでください 終了 通常 キャンセル可",
                "Exit");

            CollectionAssert.AreEqual(new string[0], ErrorMessages(src));
            Assert.IsFalse(gui.TerminateCalled);
        }

        // VB: オプションだけの Ask は選択肢の始まり。選択肢の次に半角だけのコマンドが来ると、End が無いとみなす
        // （メッセージは Ask でも「Talkに対応するEndがありません」。Event.bas の LoadEventData の NopCmd）
        [TestMethod]
        [Timeout(20000)]
        public void LoadEventData_AskWithOptionsOnly_RequiresEnd()
        {
            var (src, _) = Load(
                "スタート:",
                "Ask 選んでください 通常",
                "はい",
                "Exit");

            CollectionAssert.AreEqual(new[] { "Talkに対応するEndがありません" }, ErrorMessages(src));
            Assert.AreEqual("Ask 選んでください 通常", src.DataErrors[0].line_buf);
        }
    }
}
