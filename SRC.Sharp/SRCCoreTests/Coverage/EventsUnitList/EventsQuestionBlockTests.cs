using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests.EventsUnitList
{
    /// <summary>
    /// イベント読み込み時の Question のブロック判定（VB: Event.LoadEventData の QuestionCmd）のテスト。
    /// VB は最後の引数がオプション（通常・拡大・連続表示・キャンセル可）でなければその時点で判定を終え、
    /// 引数の数が 4 未満ならブロック形式（End まで）とみなす。「終了」指定はブロックにしない。
    /// </summary>
    [TestClass]
    public class EventsQuestionBlockTests
    {
        private string appPath;

        [TestInitialize]
        public void Initialize()
        {
            appPath = Path.Combine(Path.GetTempPath(), "srcs-eqb-" + Guid.NewGuid());
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

        private (SRC src, MockGUI gui) Load(params string[] lines)
        {
            File.WriteAllText(Path.Combine(appPath, "test.eve"), string.Join("\r\n", lines) + "\r\n", new UTF8Encoding(false));
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
            src.Event.InitEventData();
            src.Event.LoadEventData(Path.Combine(appPath, "test.eve"));
            return (src, gui);
        }

        private static string[] ErrorMessages(SRC src)
        {
            return src.DataErrors.Select(x => x.msg).ToArray();
        }

        [TestMethod]
        [Timeout(20000)]
        public void LoadEventData_QuestionWithFourNonOptionArgs_IsNotBlock()
        {
            // 最後の引数がオプションでなく引数が 4 つあれば End を必要としない
            var (src, gui) = Load(
                "スタート:",
                "Question 25 その声は アリス",
                "Exit");

            CollectionAssert.AreEqual(new string[0], ErrorMessages(src));
            Assert.IsFalse(gui.TerminateCalled);
        }

        [TestMethod]
        [Timeout(20000)]
        public void LoadEventData_QuestionWithEndOption_IsNotBlock()
        {
            // 「終了」指定のある Question はブロックにしない
            var (src, gui) = Load(
                "スタート:",
                "Question 25 その声は アリス 終了",
                "Exit");

            CollectionAssert.AreEqual(new string[0], ErrorMessages(src));
            Assert.IsFalse(gui.TerminateCalled);
        }

        [TestMethod]
        [Timeout(20000)]
        public void LoadEventData_QuestionEndingWithDisplayOption_IsBlock()
        {
            // 最後の引数がオプションならブロック形式とみなし、End がなければエラー
            var (src, gui) = Load(
                "スタート:",
                "Question 25 その声は 通常",
                "アリスだな？");

            var error = src.DataErrors.FirstOrDefault(x => x.msg == "Questionに対応するEndがありません");
            Assert.IsNotNull(error, string.Join(",", ErrorMessages(src)));
            Assert.AreEqual("Question 25 その声は 通常", error.line_buf);
            Assert.IsTrue(gui.TerminateCalled);
        }
    }
}
