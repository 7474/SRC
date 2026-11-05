using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// ファイルを操作するコマンドを、一時フォルダの実際のファイルに対して動かすテスト。
    /// 元の SRC（VB 版）と同じ振る舞いになるかを確かめる。
    /// </summary>
    [TestClass]
    public class FileCmdLocalFsTests
    {
        private string dir;

        [TestInitialize]
        public void Setup()
        {
            dir = Path.Combine(Path.GetTempPath(), "srcfilecmd-" + Guid.NewGuid().ToString("N"));
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

        private SRC CreateSrc()
        {
            var fs = new LocalFileSystem();
            fs.AddPath(dir);
            fs.AddSafePath(dir);
            var src = new SRC { GUI = new MockGUI(), FileSystem = fs, ScenarioPath = dir };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            return src;
        }

        private int Exec(SRC src, string cmdText)
        {
            var id = src.Event.EventData.Count;
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd.Exec();
        }

        private void ExecOk(SRC src, string cmdText)
        {
            Assert.AreNotEqual(-1, Exec(src, cmdText), cmdText);
        }

        // ──────────────────────────────────────────────
        // Open … For 出力（7474/SRC#1461）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void OpenOutput_ExistingFile_IsTruncated()
        {
            // 元の SRC は Open … For Output で開くので、既にあるファイルは空になる
            var path = Path.Combine(dir, "out.txt");
            File.WriteAllText(path, "0123456789ABCDEFGHIJ");
            var src = CreateSrc();

            ExecOk(src, "Open out.txt For 出力 As F");
            ExecOk(src, "Print F abc");
            ExecOk(src, "Close F");

            Assert.AreEqual("abc\r\n", File.ReadAllText(path));
        }

        [TestMethod]
        public void OpenAppend_ExistingFile_IsKept()
        {
            var path = Path.Combine(dir, "out.txt");
            File.WriteAllText(path, "abc\r\n");
            var src = CreateSrc();

            ExecOk(src, "Open out.txt For 追加出力 As F");
            ExecOk(src, "Print F def");
            ExecOk(src, "Close F");

            Assert.AreEqual("abc\r\ndef\r\n", File.ReadAllText(path));
        }

        [TestMethod]
        public void OpenSafeWrite_ExistingFile_IsTruncated()
        {
            var path = Path.Combine(dir, "a.bin");
            File.WriteAllBytes(path, new byte[10]);
            var fs = new LocalFileSystem();
            fs.AddSafePath(dir);

            using (var stream = fs.OpenSafe(SafeOpenMode.Write, "a.bin"))
            {
                stream.WriteByte(1);
            }

            Assert.AreEqual(1, new FileInfo(path).Length);
        }

        // ──────────────────────────────────────────────
        // Print の改行（7474/SRC#1476）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Print_WritesCrLf_OnAnyPlatform()
        {
            // 元の SRC の Print # は常に CR+LF
            var src = CreateSrc();

            ExecOk(src, "Open out.txt For 出力 As F");
            ExecOk(src, "Print F line1");
            ExecOk(src, "Print F next;");
            ExecOk(src, "Print F line2");
            ExecOk(src, "Close F");

            var bytes = File.ReadAllBytes(Path.Combine(dir, "out.txt"));
            Assert.AreEqual("line1\r\nnextline2\r\n", Encoding.ASCII.GetString(bytes));
        }
    }
}
