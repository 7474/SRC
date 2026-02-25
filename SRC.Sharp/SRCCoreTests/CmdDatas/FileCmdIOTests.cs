using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// ファイル入出力コマンド (Open / Close / LineRead / Print) のユニットテスト
    /// ヘルプ「Openコマンド.md」「Closeコマンド.md」「LineReadコマンド.md」「Printコマンド.md」に基づく
    /// </summary>
    [TestClass]
    public class FileCmdIOTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new System.Collections.Generic.Queue<string>();
            return src;
        }

        private CmdData CreateCmd(SRC src, string cmdText, int id = 0)
        {
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var parser = new CmdParser();
            var cmd = parser.Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        // ──────────────────────────────────────────────
        // OpenCmd
        // ヘルプ: Open file For mode As var
        // 引数は6個 (Open, file, For, mode, As, var) でなければエラー
        // ──────────────────────────────────────────────

        [TestMethod]
        public void OpenCmd_WrongArgCount_ReturnsError()
        {
            // 書式: Open file For mode As var (引数6個必須)
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Open ファイル For 出力");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void OpenCmd_NoArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Open");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void OpenCmd_PathTraversalBackslash_ReturnsError()
        {
            // ヘルプ: 「..\」は使えません
            var src = CreateSrc();
            var cmd = CreateCmd(src, @"Open ..\evil.txt For 入力 As F");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void OpenCmd_PathTraversalSlash_ReturnsError()
        {
            // ヘルプ: 「../」は使えません
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Open ../evil.txt For 入力 As F");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void OpenCmd_InvalidMode_ReturnsError()
        {
            // ヘルプ: mode は 出力/追加出力/入力 のいずれか
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Open file.txt For 不正モード As F");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // CloseCmd
        // ヘルプ: Close handle
        // 引数は2個 (Close, handle) でなければエラー
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CloseCmd_NoArgs_ReturnsError()
        {
            // 書式: Close handle (引数2個必須)
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Close");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void CloseCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Close 1 2");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // LineReadCmd
        // ヘルプ: LineRead handle var
        // 引数は3個 (LineRead, handle, var) でなければエラー
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LineReadCmd_WrongArgCount_NoArgs_ReturnsError()
        {
            // 書式: LineRead handle var (引数3個必須)
            var src = CreateSrc();
            var cmd = CreateCmd(src, "LineRead");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void LineReadCmd_OneArg_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "LineRead 1");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void LineReadCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "LineRead 1 変数 余分");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // PrintCmd
        // ヘルプ: Print handle string
        // 引数は2個以上 (Print, handle [, string]) でなければエラー
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PrintCmd_NoArgs_ReturnsError()
        {
            // 書式: Print handle [string] (引数1個のみはエラー)
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Print");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }
    }
}
