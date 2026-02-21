using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// CreateFolder/RemoveFolder/RenameFile コマンドのユニットテスト
    /// ヘルプの記載に基づく期待値を検証する
    /// </summary>
    [TestClass]
    public class FileCmdMoreTests
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
        // CreateFolderCmd
        // ヘルプ: 新規フォルダを作成
        // 書式: CreateFolder folder
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CreateFolderCmd_WrongArgCount_ReturnsError()
        {
            // 書式: CreateFolder folder (引数1個必須)
            var src = CreateSrc();
            var cmd = CreateCmd(src, "CreateFolder");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void CreateFolderCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "CreateFolder folder1 folder2");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void CreateFolderCmd_PathTraversalBackslash_ReturnsError()
        {
            // ヘルプ準拠: "..\」は使えません" セキュリティ制限
            var src = CreateSrc();
            var cmd = CreateCmd(src, @"CreateFolder ..\evil");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void CreateFolderCmd_PathTraversalSlash_ReturnsError()
        {
            // ヘルプ準拠: "../」は使えません" セキュリティ制限
            var src = CreateSrc();
            var cmd = CreateCmd(src, "CreateFolder ../evil");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // RemoveFolderCmd
        // ヘルプ: 指定したフォルダを削除
        // 書式: RemoveFolder folder
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RemoveFolderCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RemoveFolder");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RemoveFolderCmd_PathTraversalBackslash_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, @"RemoveFolder ..\evil");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RemoveFolderCmd_PathTraversalSlash_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RemoveFolder ../evil");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // RenameFileCmd
        // ヘルプ: 指定したファイルのファイル名を変更
        // 書式: RenameFile file1 file2
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RenameFileCmd_WrongArgCount_ReturnsError()
        {
            // 書式: RenameFile file1 file2 (引数2個必須)
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameFile onlyone");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RenameFileCmd_NoArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameFile");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RenameFileCmd_SourcePathTraversalBackslash_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, @"RenameFile ..\src.txt dest.txt");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RenameFileCmd_SourcePathTraversalSlash_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameFile ../src.txt dest.txt");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RenameFileCmd_DestPathTraversalBackslash_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, @"RenameFile src.txt ..\dest.txt");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RenameFileCmd_DestPathTraversalSlash_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameFile src.txt ../dest.txt");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }
    }
}
