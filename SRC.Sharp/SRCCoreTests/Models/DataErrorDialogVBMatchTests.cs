using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// データの読み込みの誤りの出し方を、元の SRC（GUI.bas の DataErrorMessage と、各 *DataList.cls の Load の ErrorHandler）と突き合わせるテスト（7474/SRC#1550）。
    /// 元の SRC は、続ける誤りでは DataErrorMessage でダイアログを出して続け、打ち切る誤りではダイアログを出してから TerminateSRC する。
    /// 開けないファイルは「fname が開けません。」（PilotDataList.cls だけ「。」が無い）を出して TerminateSRC する。
    /// SRC# は、ゲームとして実行するとき（SRC.ShowDataErrorDialogs）だけダイアログを出して終了し、lint などでは集めるだけにする
    /// </summary>
    [TestClass]
    public class DataErrorDialogVBMatchTests
    {
        private static string Unit(string name, string stats = "3000, 100, 600, 80")
        {
            return name + "\n"
                + name + ", しけんき, 試験機, 1, 1\n"
                + "陸, 4, M, 1000, 100\n"
                + "特殊能力なし\n"
                + stats + "\n"
                + "AAAA, a.bmp\n"
                + "\n";
        }

        private string dir;

        [TestInitialize]
        public void Setup()
        {
            dir = Path.Combine(Path.GetTempPath(), "srcs-data-error-" + Guid.NewGuid());
            Directory.CreateDirectory(dir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }

        private static (SRC src, MockGUI gui) CreateSrc(bool showDialogs)
        {
            var gui = new MockGUI();
            var src = new SRC { GUI = gui, FileSystem = new LocalFileSystem(), ShowDataErrorDialogs = showDialogs };
            return (src, gui);
        }

        private string Write(string name, string text)
        {
            var path = Path.Combine(dir, name);
            File.WriteAllText(path, text, new UTF8Encoding(false));
            return path;
        }

        [TestMethod]
        public void ContinuingError_ShowsDialogAndContinues()
        {
            // UnitDataList.cls: 最大ＨＰが数値でなければ DataErrorMessage を直に呼び、1000 にして続ける
            var path = Write("unit.txt", Unit("正常機") + Unit("誤り機", "abc, 100, 600, 80") + Unit("後の機"));
            var (src, gui) = CreateSrc(true);

            src.UDList.Load(path);

            Assert.AreEqual(1, gui.DataErrorMessages.Count);
            var shown = gui.DataErrorMessages.Single();
            Assert.AreEqual("最大ＨＰの設定が間違っています。", shown.msg);
            Assert.AreEqual("誤り機", shown.dname);
            Assert.AreEqual(path, shown.fname);
            Assert.AreEqual(12, shown.line_num);
            Assert.AreEqual("abc, 100, 600, 80", shown.line_buf);
            Assert.IsFalse(gui.TerminateCalled);
            Assert.IsTrue(src.UDList.IsDefined("後の機"));
            // lint などのために集めることは変えない
            Assert.AreEqual(1, src.DataErrors.Count);
        }

        [TestMethod]
        public void ContinuingError_WithoutDialogs_OnlyCollects()
        {
            var path = Write("unit.txt", Unit("誤り機", "abc, 100, 600, 80"));
            var (src, gui) = CreateSrc(false);

            src.UDList.Load(path);

            Assert.AreEqual(0, gui.DataErrorMessages.Count);
            Assert.AreEqual(1, src.DataErrors.Count);
        }

        [TestMethod]
        public void StoppingError_ShowsDialogAndTerminates()
        {
            // UnitDataList.cls: 名称に半角スペースがあれば err_msg を作って Error 0。ErrorHandler が DataErrorMessage を出して TerminateSRC
            var path = Write("unit.txt", Unit("正常機") + Unit("誤り 機") + Unit("後の機"));
            var (src, gui) = CreateSrc(true);

            Assert.Throws<InvalidSrcDataException>(() => src.UDList.Load(path));

            Assert.AreEqual(1, gui.DataErrorMessages.Count);
            var shown = gui.DataErrorMessages.Single();
            Assert.AreEqual("名称に半角スペースは使用出来ません。", shown.msg);
            Assert.AreEqual("誤り 機", shown.dname);
            Assert.AreEqual(path, shown.fname);
            Assert.IsTrue(gui.TerminateCalled);
            Assert.IsFalse(src.UDList.IsDefined("後の機"));
        }

        [TestMethod]
        public void StoppingError_WithoutDialogs_OnlyThrows()
        {
            var path = Write("unit.txt", Unit("誤り 機"));
            var (src, gui) = CreateSrc(false);

            Assert.Throws<InvalidSrcDataException>(() => src.UDList.Load(path));

            Assert.AreEqual(0, gui.DataErrorMessages.Count);
            Assert.IsFalse(gui.TerminateCalled);
        }

        [TestMethod]
        public void StoppingError_MessageData_ShowsDialogAndTerminates()
        {
            // MessageDataList.cls の ErrorHandler は err_msg ではなく "" を渡す（DataErrorMessage "", fname, line_num, line_buf, data_name）が、
            // VB の不具合として合わせず（対応表 MessageDataList_cls.md の Load）、原因を出す
            var path = Write("pilot_message.txt", "誤り 員\n攻撃,やあ\n");
            var (src, gui) = CreateSrc(true);

            Assert.Throws<InvalidSrcDataException>(() => src.MDList.Load(path, false));

            var shown = gui.DataErrorMessages.Single();
            Assert.AreEqual("名称に半角スペースは使用出来ません。", shown.msg);
            Assert.AreEqual("誤り 員", shown.dname);
            Assert.AreEqual(1, shown.line_num);
            Assert.IsTrue(gui.TerminateCalled);
        }

        [TestMethod]
        public void OpenError_ShowsCannotOpenAndTerminates()
        {
            // 各 *DataList.cls: line_num = 0 なら ErrorMessage fname & "が開けません。" の後 TerminateSRC
            var path = Path.Combine(dir, "unit.txt");
            var (src, gui) = CreateSrc(true);

            Assert.Throws<Exception>(() => src.UDList.Load(path));

            CollectionAssert.AreEqual(new[] { path + "が開けません。" }, gui.ErrorMessages);
            Assert.AreEqual(0, gui.DataErrorMessages.Count);
            Assert.IsTrue(gui.TerminateCalled);
        }

        [TestMethod]
        public void OpenError_Pilot_HasNoPeriod()
        {
            // PilotDataList.cls だけ ErrorMessage fname & "が開けません"（「。」が無い）
            var path = Path.Combine(dir, "pilot.txt");
            var (src, gui) = CreateSrc(true);

            Assert.Throws<Exception>(() => src.PDList.Load(path));

            CollectionAssert.AreEqual(new[] { path + "が開けません" }, gui.ErrorMessages);
            Assert.IsTrue(gui.TerminateCalled);
        }
    }
}
