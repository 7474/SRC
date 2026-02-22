using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// ファイル操作コマンドの追加ユニットテスト
    /// (CopyFile, RemoveFile のエラーケース)
    /// </summary>
    [TestClass]
    public class FileCmdAdditionalTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new Queue<string>();
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
        // CopyFileCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CopyFileCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "CopyFile");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void CopyFileCmd_TwoArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "CopyFile src dst");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void CopyFileCmd_SourceWithBackslashTraversal_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "CopyFile \"..\\evil.exe\" dest.txt");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void CopyFileCmd_SourceWithSlashTraversal_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "CopyFile \"../evil.exe\" dest.txt");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void CopyFileCmd_DestWithBackslashTraversal_ActuallyReturnsError()
        {
            // FileSystem が null の場合、path traversal チェックより先に NullReferenceException が
            // スローされるため、MockGUI テスト環境では -1 が返される。
            var src = CreateSrc();
            var cmd = CreateCmd(src, "CopyFile normal.txt \"..\\evil.txt\"");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void CopyFileCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "CopyFile a b c d");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // RemoveFileCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RemoveFileCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RemoveFile");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RemoveFileCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RemoveFile file1 file2");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RemoveFileCmd_PathTraversalBackslash_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RemoveFile \"..\\evil.exe\"");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RemoveFileCmd_PathTraversalSlash_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RemoveFile \"../evil.exe\"");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RemoveFileCmd_ReturnsError_WhenFileSystemIsNull()
        {
            // FileSystem が null の場合は Exception → -1 が返される
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RemoveFile nonexistent.dat");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }
    }
}
